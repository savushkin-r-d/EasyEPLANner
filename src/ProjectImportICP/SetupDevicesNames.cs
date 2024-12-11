using BrightIdeasSoftware;
using Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
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

        List<ImportDevice> Devices;

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

            var newNameColumn = new OLVColumn("Номер", "Number")
            {
                IsEditable = true,
                AspectGetter = (obj) => (obj as ImportDevice).Type + (obj as ImportDevice).Number,
                Sortable = false,
            };


            objectListView.Columns.Add(oldNameColumn);
            objectListView.Columns.Add(toColumn);
            objectListView.Columns.Add(newObjectColumn);
            objectListView.Columns.Add(newNameColumn);
        }

        public void Init(List<ImportDevice> devices)
        {
            objectListView.BeginUpdate();

            Devices = new List<ImportDevice>();
            Devices.AddRange(devices);

            objectListView.Objects = Devices;

            objectListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            objectListView.Columns[0].Width = 70;
            objectListView.Columns[1].Width = 20;
            objectListView.Columns[2].Width = 70;
            objectListView.Columns[2].TextAlign = HorizontalAlignment.Right;
            objectListView.Columns[3].Width = 70;

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
                    editText = item.Number.ToString();
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
                    device.Object = e.NewValue.ToString();
                    objectListView.Refresh();
                    break;

                case 3:
                    if (int.TryParse(e.NewValue.ToString(), out var number))
                    {
                        device.Number = number;
                        objectListView.Refresh();
                    } 
                    else
                    {
                        cancelChanges = false;
                    }
                    
                    break;
            }


            e.Cancel = true;
            objectListView.Unfreeze();
        }
    }
}
