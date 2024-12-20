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

            var descriptionColumn = new OLVColumn("Описание", "Description")
            {
                IsEditable = true,
                AspectGetter = (obj) => (obj as ImportDevice).Description,
                Sortable = false,
            };


            objectListView.Columns.Add(oldNameColumn);
            objectListView.Columns.Add(toColumn);
            objectListView.Columns.Add(newObjectColumn);
            objectListView.Columns.Add(typeColumn);
            objectListView.Columns.Add(newNameColumn);
            objectListView.Columns.Add(descriptionColumn);
        }

        public void Init(List<ImportDevice> devices)
        {
            objectListView.BeginUpdate();

            objectListView.Objects = devices;

            objectListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            objectListView.Columns[0].Width = 70;  // old name
            objectListView.Columns[1].Width = 25;  // ->
            objectListView.Columns[2].Width = 60;  // Object
            objectListView.Columns[3].Width = 40;  // Type
            objectListView.Columns[4].Width = 50;  // Number
            objectListView.Columns[5].Width = 250; // Description 

            objectListView.EndUpdate();

        }

        private void objectListView_CellEditStarting(object sender, CellEditEventArgs e)
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
                    editText = item.Description;
                    break;

                default:
                    return;
            }

            IsCellEditing = true;
            InitTextBoxCellEditor();
            textBoxCellEditor.Text = editText;
            textBoxCellEditor.Bounds = e.CellBounds;
            e.Control = textBoxCellEditor;
            textBoxCellEditor.Focus();
            objectListView.Freeze();
        }

        void InitTextBoxCellEditor()
        {
            textBoxCellEditor = new TextBox();
            textBoxCellEditor.Enabled = true;
            textBoxCellEditor.Visible = true;
            textBoxCellEditor.LostFocus += editorTView_LostFocus;
            textBoxCellEditor.KeyDown += CellEditor_KeyDown;
            objectListView.Controls.Add(textBoxCellEditor);
        }

        private void editorTView_LostFocus(object sender, EventArgs e)
        {
            if (IsCellEditing)
            {
                cancelChanges = true;
                objectListView.FinishCellEdit();
            }
        }

        private void CellEditor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    objectListView.FinishCellEdit();
                    break;

                case Keys.Escape:
                    cancelChanges = true;
                    objectListView.FinishCellEdit();
                    break;

                default:
                    return; // exit without e.Handled
            }

            e.Handled = true;
        }



        private void objectListView_CellEditFinishing(object sender, CellEditEventArgs e)
        {
            IsCellEditing = false;
            objectListView.LabelEdit = false;

            var device = e.RowObject as ImportDevice;

            if (cancelChanges || device == null)
            {
                e.Cancel = true;
                cancelChanges = false;
                objectListView.Unfreeze();
                return;
            }

            objectListView.Controls.Remove(textBoxCellEditor);

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
                    device.Description = e.NewValue.ToString();
                    break;
            }

            objectListView.Refresh();
            e.Cancel = true;
            objectListView.Unfreeze();
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
                @"\s*(?<wago_name>[\w]*?)(?<wago_number>[\d]*)\s*=>\s*(?<object>[\w]*)\s*\|\s*(?<type>[\w]*)\s*\|\s*(?<number>[\d]*)\s*\|\s*\'(?<description>[\w\W]*?)\'\s*",
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
                var description = match.Groups["description"].Value;

                var dev = objectListView.Objects.OfType<ImportDevice>().FirstOrDefault(d => d.FullNumber == wagoNumber && d.WagoType == wagoType);

                if (dev is null)
                    continue;
                
                dev.Object = obj.ToUpper();
                dev.Type = type.ToUpper();
                dev.Number = number;
                dev.Description = description;

                objectListView.RefreshObject(dev);
            }
        }
    }
}
