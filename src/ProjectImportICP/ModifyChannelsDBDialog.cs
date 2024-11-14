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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

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
            
            ExportChbase(dstPath);

            // Чтение баз каналов
            var srcChbase = ReadFile(srcPath);
            var dstChbase = ReadFile(dstPath);

            Logs.Clear();
            Logs.Show();

            // Модификация названий устройств старой базы каналов
            var modifiedSrcChbase = ChannelBaseTransformer.ModifyDescription(srcChbase, dstChbase, GetDevicesNames());

            // Получение индекса драйвера старой базы каналов
            var srcDriverID = ChannelBaseTransformer.GetDriverID(srcChbase);

            // Смещение типов новой базы каналов для вставки старой
            var modifiedDstChbase = ChannelBaseTransformer.ShiftSubtypeID(dstChbase, ChannelBaseTransformer.GetFreeSubtypeID(srcChbase));

            // Изменение ID драйвера и всех каналов
            modifiedDstChbase = ChannelBaseTransformer.ModifyDriverID(modifiedDstChbase, srcDriverID);

            // Выключение всех тегов базы каналов
            modifiedDstChbase = ChannelBaseTransformer.DisableAllSubtypesChannels(modifiedDstChbase);
               
            var srcXmlDoc = new XmlDocument();
            srcXmlDoc.LoadXml(modifiedSrcChbase);
            
            var dstXmlDoc = new XmlDocument();
            dstXmlDoc.LoadXml(modifiedDstChbase);

           
            // Disable all channels except devices
            foreach (XmlNode node in srcXmlDoc.GetElementsByTagName("driver:subtypes")[0].ChildNodes)
            {
                if (int.Parse(node.ChildNodes.OfType<XmlNode>().FirstOrDefault(n => n.Name == "subtypes:sid")?.InnerText ?? "0") != 0)
                {
                    if (node.ChildNodes.OfType<XmlNode>().FirstOrDefault(n => n.Name == "subtypes:enabled") is XmlNode subtypeEnabled)
                    {
                        subtypeEnabled.InnerText = "0";
                    }
                    
                    foreach (XmlNode channelsNode in node.ChildNodes.OfType<XmlNode>().FirstOrDefault(n => n.Name == "subtypes:channels")?.ChildNodes ?? (XmlNodeList)Enumerable.Empty<XmlNode>())
                    {
                        if (channelsNode.ChildNodes.OfType<XmlNode>().FirstOrDefault(n => n.Name == "channels:enabled") is XmlNode enabled)
                        {
                            enabled.InnerText = "0";
                        }
                    }
                }
            }

            var subtypesNode = dstXmlDoc.GetElementsByTagName("driver:subtypes")[0];
            foreach (XmlNode node in srcXmlDoc.GetElementsByTagName("driver:subtypes")[0].ChildNodes)
            {
                subtypesNode.InsertBefore(dstXmlDoc.ImportNode(node, true), subtypesNode.FirstChild);
            }

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
