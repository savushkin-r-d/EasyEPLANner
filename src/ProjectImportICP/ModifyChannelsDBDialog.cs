using Eplan.EplApi.DataModel.Graphics;
using Eplan.EplApi.HEServices;
using EplanDevice;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static Eplan.EplApi.DataModel.E3D.BasePointMate.Enums;

namespace EasyEPlanner.ProjectImportICP
{
    /// <summary>
    /// Диалог для модификации базы каналов
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class ModifyChannelsDBDialog : Form
    {
        private readonly string projectName;

        public ModifyChannelsDBDialog(string projectName)
        {
            this.projectName = projectName;

            InitializeComponent();
        }


        private sealed class BoolComboBox
        {
            public bool Value { get; set; }
            public string Display { get; set; }
        }
        
        private void ModifyChannelsDBDialog_Load(object sender, EventArgs e)
        {
            var YN = new List<BoolComboBox>()
            {
                new BoolComboBox() { Value = true, Display = "Да" },
                new BoolComboBox() { Value = false, Display = "Нет" },
            };

            CombineTagCmbBx.DropDownStyle = ComboBoxStyle.DropDownList;
            CombineTagCmbBx.DataSource = YN;
            CombineTagCmbBx.DisplayMember = nameof(BoolComboBox.Display);
            CombineTagCmbBx.ValueMember = nameof(BoolComboBox.Value);
            CombineTagCmbBx.SelectedValue = true;

            var Formats = new List<BoolComboBox>()
            {
                new BoolComboBox() { Value = true, Display = "По индексам" },
                new BoolComboBox() { Value = false, Display = "По именам" },
            };

            FormatTagCmbBx.DropDownStyle = ComboBoxStyle.DropDownList;
            FormatTagCmbBx.DataSource = Formats;
            FormatTagCmbBx.DisplayMember = nameof(BoolComboBox.Display);
            FormatTagCmbBx.ValueMember = nameof(BoolComboBox.Value);
            FormatTagCmbBx.SelectedValue = true;
        }

        private void SrcChbasePathBttn_Click(object sender, EventArgs e)
        {
            var open = new OpenFileDialog()
            {
                Title = "Исходная база каналов (ICP-CON)",
                Filter = "*.cdbx|*.cdbx",
                Multiselect = false,
            };

            if (open.ShowDialog() == DialogResult.Cancel)
                return;

            SrcChbasePathTextBox.Text = open.FileName;
        }

        private void ChbasePathBttn_Click(object sender, EventArgs e)
        {
            var openFileDlg = new SaveFileDialog()
            {
                Title = "База каналов",
                Filter = "База каналов (*.cdbx)|*.cdbx",
                FileName = $"{projectName}",
                DefaultExt = "cdbx",
                InitialDirectory = Path.GetDirectoryName(DstChbasePathTextBox.Text),
            };

            if (openFileDlg.ShowDialog() == DialogResult.Cancel)
                return;

            DstChbasePathTextBox.Text = openFileDlg.FileName;
        }


        private void ModifyBttn_Click(object sender, EventArgs e)
        {
            var srcPath = SrcChbasePathTextBox.Text;
            var dstPath = DstChbasePathTextBox.Text;

            if (!File.Exists(srcPath))
                return;
            
            Logs.Show();
            Logs.Clear();
            ExportChbase(dstPath);

            // Чтение баз каналов
            var srcChbase = ReadFile(srcPath);
            var dstChbase = ReadFile(dstPath);

            var srcXmlDoc = new XmlDocument();
            srcXmlDoc.LoadXml(srcChbase);

            var dstXmlDoc = new XmlDocument();
            dstXmlDoc.LoadXml(dstChbase);

            XmlNamespaceManager nsMngr = new XmlNamespaceManager(dstXmlDoc.NameTable);
            nsMngr.AddNamespace("driver", "http://brestmilk.by/driver/");
            nsMngr.AddNamespace("subtypes", "http://brestmilk.by/subtypes/");
            nsMngr.AddNamespace("channels", "http://brestmilk.by/channels/");


            XmlNode dstRoot = dstXmlDoc.DocumentElement;
            XmlNode srcRoot = srcXmlDoc.DocumentElement;

            // Замена названий старых каналов на новые 
            var namingMap = GetDevicesNames();
            foreach (var replacingName in namingMap)
            {
                if (replacingName.WagoName is null)
                {
                   Logs.AddMessage($"{replacingName.Name}: wago name is null\n");

                    continue;
                }

                var replacingNode = srcRoot.SelectSingleNode($"//channels:channel[contains(channels:descr, '{replacingName.WagoName}')]", nsMngr);

                if (replacingNode is null)
                {
                    Logs.AddMessage($"Канал {replacingName.WagoName} не найден в исходной базе каналов.\n");
                    continue;
                }

                var dev = DeviceManager.GetInstance().GetDevice(replacingName.Name);

                if (dev.DeviceSubType is DeviceSubType.NONE)
                {
                    Logs.AddMessage($"У устройства {replacingName.Name} не задан подтип.\n");
                    continue;
                }

                var ST = dev.GetDeviceProperties(dev.DeviceType, dev.DeviceSubType).ContainsKey(IODevice.Tag.ST);
                replacingNode.SelectSingleNode("./channels:descr/text()", nsMngr).Value = $"{replacingName.Name}.{(ST ? IODevice.Tag.ST : IODevice.Tag.V)}";
            }

            // Получаем ID старой базы каналов
            var _srcDriverID = int.Parse(srcRoot.SelectSingleNode("//driver:id/text()", nsMngr).Value);

            // Получаем первый свободный подтип в старой базе каналов
            var  srcFirstFreeSubtypeID = 1 + srcRoot.SelectNodes("//subtypes:sid/text()", nsMngr).OfType<XmlNode>().Max(n => int.Parse(n.Value));

            // Смещение индексов подтипов и каналов новой базы каналов
            dstRoot.SelectSingleNode("//driver:id", nsMngr).InnerText = _srcDriverID.ToString();
            var channelDriverIdOctet = _srcDriverID << 24; // 0x[driver id]000000
            foreach (XmlNode subtype in dstRoot.SelectNodes("//subtypes:subtype", nsMngr))
            {
                var subtypeIdNode = subtype.SelectSingleNode("./subtypes:sid/text()", nsMngr);
                var subtypeId = int.Parse(subtypeIdNode.Value) + srcFirstFreeSubtypeID;

                subtypeIdNode.Value = subtypeId.ToString();

                var channelSubtypeOctet = subtypeId << 16; // 0x00[channel subtype id]0000
                foreach (XmlNode channel in subtype.SelectNodes("./subtypes:channels/channels:channel", nsMngr))
                {
                    var channelIdNode = channel.SelectSingleNode("./channels:id/text()", nsMngr);

                    channelIdNode.Value = (int.Parse(channelIdNode.Value) & 0x0000FFFF | channelDriverIdOctet | channelSubtypeOctet).ToString();
                }
            }

            // Выключение всех каналов в новой базе каналов
            foreach (XmlNode enabled in dstRoot.SelectNodes("//subtypes:enabled/text()", nsMngr))
            {
                enabled.Value = "0"; // disabled
            }

            // Выключение всех подтипов в старой базе каналов кроме устройств
            foreach (XmlNode enabled in srcRoot.SelectNodes("//subtypes:enabled[../subtypes:sid!='0']/text()", nsMngr))
            {
                enabled.Value = "0"; // disabled
            }

            // Вставка старой базы каналов в новую
            var dstSubtypes = dstRoot.SelectSingleNode("//driver:subtypes", nsMngr);
            foreach (XmlNode srcSubtype in srcRoot.SelectNodes("//subtypes:subtype", nsMngr).OfType<XmlNode>().Reverse())
            {
                dstSubtypes.PrependChild(dstXmlDoc.ImportNode(srcSubtype, true));
            }

            // Сохранение новой базы каналов
            using (var writer = new StreamWriter(dstPath, false, Encoding.UTF8))
                dstXmlDoc.Save(writer);

            Logs.SetProgress(100);
            Logs.EnableButtons();

            Close();
        }

        /// <summary>
        /// Экспорт новой базы каналов
        /// </summary>
        private void ExportChbase(string path)
        {
            var reporter = new XMLReporter();
            reporter.AwaitSaveAsCDBX(
                path,
                (bool)CombineTagCmbBx.SelectedValue,
                (bool)FormatTagCmbBx.SelectedValue,
                true);
        }

        /// <summary>
        /// Прочитать файл базы каналов
        /// </summary>
        private static string ReadFile(string path)
        {
            using (var reader = new StreamReader(path, EncodingDetector.DetectFileEncoding(path), true))
               return reader.ReadToEnd();
        }

        /// <summary>
        /// Получить названия устройств (новое - старое)
        /// </summary>
        private static IEnumerable<(string Name, string WagoName)> GetDevicesNames()
        {
            var apiHelper = new ApiHelper();
            var deviceNames = DeviceManager.GetInstance().Devices.Select(d =>
            {
                var wagoName = apiHelper.GetSupplementaryFieldValue(d.EplanObjectFunction, 10);

                if (wagoName == string.Empty)
                {
                    wagoName = ChannelBaseTransformer.ToWagoDevice(d);
                    if (wagoName != string.Empty)
                        Logs.AddMessage($"Старое название тега для устройства {d.Name} не указано, используется название: {wagoName} \n");
                }

                return (d.Name, wagoName);
            });

            var duplicates = deviceNames.GroupBy(names => names.wagoName)
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicates.Any())
            {
                duplicates.ForEach(g => Logs.AddMessage($"Тег {g.Key} дублируется в устройствах {string.Join(", ", g.Select(names => names.Name))}.\n"));
                return Enumerable.Empty<(string Name, string WagoName)>();
            }

            return deviceNames;
        }

        private void CancelBttn_Click(object sender, EventArgs e) => Close();
    }
}
