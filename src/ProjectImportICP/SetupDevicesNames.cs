using BrightIdeasSoftware;
using Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.ProjectImportICP
{
    [ExcludeFromCodeCoverage]
    public partial class SetupDevicesNames : Form
    {
        bool cancelChanges;

        bool IsCellEditing;

        TextBox textBoxCellEditor;

        public SetupDevicesNames()
        {
            InitializeComponent();
            InitObjectListView();
        }

        private void InitObjectListView()
        {
            var oldNameColumn = new OLVColumn("Старое название", "WagoName")
            {
                IsEditable = false,
                AspectGetter = obj => (obj as ImportDevice).WagoType + (obj as ImportDevice).FullNumber,
                Sortable = false,
            };

            var toColumn = new OLVColumn("->", "")
            {
                IsEditable = false,
                AspectGetter = obj => "->",
            };

            var newObjectColumn = new OLVColumn("Объект", "Object")
            {
                IsEditable = true,
                AspectGetter = (obj) => (obj as ImportDevice).Object,
                Sortable = false,
            };

            var typeColumn = new OLVColumn("Тип", "Type")
            {
                IsEditable = true,
                AspectGetter = (obj) => (obj as ImportDevice).Type,
                Sortable = false,
            };

            var newNameColumn = new OLVColumn("Номер", "Number")
            {
                IsEditable = true,
                AspectGetter = (obj) => (obj as ImportDevice).Number,
                Sortable = false,
            };

            var subtype = new OLVColumn("Подтип", "Subtype")
            {
                IsEditable = true,
                AspectGetter = (obj) => (obj as ImportDevice).Subtype,
                Sortable = false,
            };

            var descriptionColumn = new OLVColumn("Описание", "Description")
            {
                IsEditable = true,
                AspectGetter = (obj) => (obj as ImportDevice).Description,
                Sortable = false,
            };


            renameDevicesOLV.Columns.Add(oldNameColumn);
            renameDevicesOLV.Columns.Add(toColumn);
            renameDevicesOLV.Columns.Add(newObjectColumn);
            renameDevicesOLV.Columns.Add(typeColumn);
            renameDevicesOLV.Columns.Add(newNameColumn);
            renameDevicesOLV.Columns.Add(subtype);
            renameDevicesOLV.Columns.Add(descriptionColumn);


            var deviceType = new OLVColumn("Тип устройства", "DeviceType")
            {
                IsEditable = false,
                AspectGetter = obj => (obj as ImportDefaultDeviceParamter).DeviceType,
                Sortable = false,
            };

            var parameter = new OLVColumn("Параметр", "Parameter")
            {
                IsEditable = false,
                AspectGetter = obj => (obj as ImportDefaultDeviceParamter).Parameter,
                Sortable = false,
            };

            var value = new OLVColumn("Значение по умолчанию", "Value")
            {
                IsEditable = true,
                AspectGetter = obj => (obj as ImportDefaultDeviceParamter).DefaultValue,
                Sortable = false,
            };

            defaultParametersOLV.Columns.Add(deviceType);
            defaultParametersOLV.Columns.Add(parameter);
            defaultParametersOLV.Columns.Add(value);
        }

        public void InitRenamingDevices(List<ImportDevice> devices)
        {
            renameDevicesOLV.BeginUpdate();

            renameDevicesOLV.Objects = devices;

            renameDevicesOLV.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            renameDevicesOLV.Columns[0].Width = 70;  // old name
            renameDevicesOLV.Columns[1].Width = 25;  // ->
            renameDevicesOLV.Columns[2].Width = 60;  // Object
            renameDevicesOLV.Columns[3].Width = 40;  // Type
            renameDevicesOLV.Columns[4].Width = 50;  // Number
            renameDevicesOLV.Columns[5].Width = 50;  // Subtype
            renameDevicesOLV.Columns[6].Width = 250; // Description 

            renameDevicesOLV.EndUpdate();
        }

        public void InitDefaultParameters(List<ImportDefaultDeviceParamter> defaultParameters)
        {
            defaultParametersOLV.BeginUpdate();

            defaultParametersOLV.Objects = defaultParameters;

            defaultParametersOLV.Columns[0].Width = 100;
            defaultParametersOLV.Columns[1].Width = 75;
            defaultParametersOLV.Columns[2].Width = 150;

            defaultParametersOLV.EndUpdate();
        }

        void InitTextBoxCellEditor(ObjectListView olv)
        {
            textBoxCellEditor = new TextBox();
            textBoxCellEditor.Enabled = true;
            textBoxCellEditor.Visible = true;
            textBoxCellEditor.LostFocus += textBoxCellEditor_LostFocus;
            textBoxCellEditor.KeyDown += CellEditor_KeyDown;
            olv.Controls.Add(textBoxCellEditor);
        }

        private void textBoxCellEditor_LostFocus(object sender, EventArgs e)
        {
            if (IsCellEditing)
            {
                cancelChanges = true;
                (textBoxCellEditor.Parent as ObjectListView).FinishCellEdit();
            }
        }

        private void CellEditor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    renameDevicesOLV.FinishCellEdit();
                    break;

                case Keys.Escape:
                    cancelChanges = true;
                    renameDevicesOLV.FinishCellEdit();
                    break;

                default:
                    return; // exit without e.Handled
            }

            e.Handled = true;
        }


        private void renameDevicesOLV_CellEditStarting(object sender, CellEditEventArgs e)
        {
            var item = e.RowObject as ImportDevice;

            var editText = "";

            switch (e.Column.Index)
            {
                case 2:
                    editText = item.Object;
                    break;

                case 3:
                    editText = item.Type;
                    break;

                case 4:
                    editText = item.Number.ToString();
                    break;

                case 5:
                    editText = item.Subtype;
                    break;

                case 6:
                    editText = item.Description;
                    break;

                default:
                    return;
            }

            IsCellEditing = true;
            InitTextBoxCellEditor(renameDevicesOLV);
            textBoxCellEditor.Text = editText;
            textBoxCellEditor.Bounds = e.CellBounds;
            e.Control = textBoxCellEditor;
            textBoxCellEditor.Focus();
            renameDevicesOLV.Freeze();
        }


        private void renameDevicesOLV_CellEditFinishing(object sender, CellEditEventArgs e)
        {
            IsCellEditing = false;
            renameDevicesOLV.LabelEdit = false;

            var device = e.RowObject as ImportDevice;

            if (cancelChanges || device == null)
            {
                e.Cancel = true;
                cancelChanges = false;
                renameDevicesOLV.Unfreeze();
                return;
            }

            renameDevicesOLV.Controls.Remove(textBoxCellEditor);

            switch (e.Column.Index)
            {
                case 2:
                    device.Object = e.NewValue.ToString().ToUpper();
                    break;

                case 3:
                    device.Type = e.NewValue.ToString().ToUpper();
                    break;

                case 4:
                    if (int.TryParse(e.NewValue.ToString(), out var number))
                    {
                        device.Number = number;
                    } 
                    else
                    {
                        cancelChanges = false;
                    }
                    
                    break;

                case 5:
                    device.Subtype = e.NewValue.ToString();
                    break;

                case 6:
                    device.Description = e.NewValue.ToString();
                    break;
            }

            renameDevicesOLV.Refresh();
            e.Cancel = true;
            renameDevicesOLV.Unfreeze();
        }

        private void OkBttn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadRenameMapBttn_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Открыть файл карты переименования устройств",
                Filter = "Текстовый файл|*.txt",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            var data = "";
            using (var reader = new StreamReader(openFileDialog.FileName, EncodingDetector.DetectFileEncoding(openFileDialog.FileName), true))
            {
                // read main.wago.plua file data
                data = reader.ReadToEnd();
            }

            var matches = Regex.Matches(data,
                @"\s*(?<wago_name>[\w]*?)(?<wago_number>[\d]*)\s*=>\s*(?<object>[\w]*)\s*\|\s*(?<type>[\w]*)\s*\|\s*(?<number>[\d]*)\s*\|\s*(?<subtype>[\w]*)\s*\|\s*\'(?<description>[\w\W]*?)\'\s*",
                RegexOptions.None, 
                TimeSpan.FromMilliseconds(100));

            foreach (Match match in matches)
            {
                if (!match.Success)
                    continue;

                var wagoType = match.Groups["wago_name"].Value;
                var wagoNumber = int.Parse(match.Groups["wago_number"].Value);
                var obj = match.Groups["object"].Value;
                var type = match.Groups["type"].Value;
                var number = int.Parse(match.Groups["number"].Value);
                var subtype = match.Groups["subtype"].Value;
                var description = match.Groups["description"].Value;

                var dev = renameDevicesOLV.Objects.OfType<ImportDevice>().FirstOrDefault(d => d.FullNumber == wagoNumber && d.WagoType == wagoType);

                if (dev is null)
                    continue;
                
                dev.Object = obj.ToUpper();
                dev.Type = type.ToUpper();
                dev.Number = number;
                dev.Subtype = subtype.ToUpper();
                dev.Description = description;

                renameDevicesOLV.RefreshObject(dev);
            }
        }

        private void defaultParametersOLV_CellEditStarting(object sender, CellEditEventArgs e)
        {
            var item = e.RowObject as ImportDefaultDeviceParamter;

            var editText = "";

            switch (e.Column.Index)
            {
                case 2:
                    editText = item.DefaultValue.ToString();
                    break;

                default:
                    return;
            }

            IsCellEditing = true;
            InitTextBoxCellEditor(defaultParametersOLV);
            textBoxCellEditor.Text = editText;
            textBoxCellEditor.Bounds = e.CellBounds;
            e.Control = textBoxCellEditor;
            textBoxCellEditor.Focus();
            defaultParametersOLV.Freeze();
        }

        private void defaultParametersOLV_CellEditFinishing(object sender, CellEditEventArgs e)
        {
            IsCellEditing = false;
            defaultParametersOLV.LabelEdit = false;

            var par = e.RowObject as ImportDefaultDeviceParamter;

            if (cancelChanges)
            {
                e.Cancel = true;
                cancelChanges = false;
                defaultParametersOLV.Unfreeze();
                return;
            }

            defaultParametersOLV.Controls.Remove(textBoxCellEditor);

            switch (e.Column.Index)
            {
                case 2:
                    if (int.TryParse(e.NewValue.ToString(), out var number))
                    {
                        par.DefaultValue = number;
                    }
                    else
                    {
                        cancelChanges = false;
                    }

                    break;
            }

            defaultParametersOLV.Refresh();
            e.Cancel = true;
            defaultParametersOLV.Unfreeze();
        }
    }
}
