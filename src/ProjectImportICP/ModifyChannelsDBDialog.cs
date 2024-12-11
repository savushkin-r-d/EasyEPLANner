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

            XmlNode srcRoot = srcXmlDoc.DocumentElement;
            XmlNode dstRoot = dstXmlDoc.DocumentElement;
            XmlNode srcCloneRoot = srcRoot.Clone();

            // Замена названий старых каналов на новые 
            srcRoot.UpdateDeviceTags(GetDevicesNames(), DeviceManager.GetInstance());



            // Смещение индексов подтипов и каналов новой базы каналов
            var srcDriverID = int.Parse(srcRoot.SelectSingleNode("//driver:id/text()", srcRoot.ChbaseNameSpace()).Value);
            var srcFirstFreeSubtypeID = 1 + srcRoot.SelectNodes("//subtypes:sid/text()", srcRoot.ChbaseNameSpace()).OfType<XmlNode>().Max(n => int.Parse(n.Value));
            dstRoot.ShiftIds(srcDriverID, srcFirstFreeSubtypeID);

            // Выключение всех подтипов в новой базе каналов
            dstRoot.DisableTags("//subtypes:enabled/text()");

            // Выключение всех подтипов в старой базе каналов кроме устройств
            srcRoot.DisableTags("//subtypes:enabled[../subtypes:sid!='0']/text()");

            // Вставка старой базы каналов в новую
            dstRoot.InsertSubtypes(srcRoot);

            dstRoot.CompareDeviceChannelsWith(srcCloneRoot);

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
                    wagoName = ToWagoDevice(d);
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


        private static readonly Dictionary<DeviceType, string> WagoTypes = new Dictionary<DeviceType, string>
        {
            { DeviceType.V, "V" },
            { DeviceType.LS, "LS"},
            { DeviceType.TE, "TE"},
            { DeviceType.FQT, "CTR"},
            { DeviceType.FS, "FS" },
            { DeviceType.AO, "AO"},
            { DeviceType.LT, "LE"},
            { DeviceType.DI, "FB"},
            { DeviceType.DO, "UPR"},
            { DeviceType.QT, "QE"},
            { DeviceType.AI, "AI"},
        };


        /// <summary>
        /// Получить старое название устройства
        /// </summary>
        /// <param name="device"></param>
        public static string ToWagoDevice(IODevice device)
        {
            if (WagoTypes.TryGetValue(device.DeviceType, out var wagoType))
            {
                if (device.ObjectName == "TANK")
                    return $"{wagoType}{device.ObjectNumber}{device.DeviceNumber:00}";

                if (device.ObjectName.Length == 1)
                    return $"{wagoType}{(device.ObjectName[0] - 'A') * 20 + 200 + device.ObjectNumber}{device.DeviceNumber:00}";
            }

            Logs.AddMessage($"Старое название тега для устройства {device.Name} не указано, сигнатура устройства не распознана\n");
            return "";
        }

        private void CancelBttn_Click(object sender, EventArgs e) => Close();
    }
}
