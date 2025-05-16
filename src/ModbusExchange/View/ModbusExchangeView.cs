﻿using BrightIdeasSoftware;
using EasyEPlanner.ModbusExchange.Model;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using Editor;
using EplanDevice;
using Spire.Pdf.Exporting.XPS.Schema;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.ModbusExchange.View
{
    [ExcludeFromCodeCoverage]
    public partial class ModbusExchangeView : Form
    {
        IExchange DataContext;

        IDeviceManager deviceManager = DeviceManager.GetInstance();

        private TextBox textBoxCellEditor;

        IGateway SelectedModel => DataContext.SelectedModel;

        public ModbusExchangeView()
        {
            InitializeComponent();
            InitExchange();

            DataContext = new Exchange();
            DataContext.Load();

            DataContext.SelectModel(DataContext.Models.FirstOrDefault()?.Name);

            GatewaySelectionCB.Items.AddRange([.. DataContext.Models.Select(m => m.Name)]);
            GatewaySelectionCB.SelectedIndex = DataContext.Models.ToList().IndexOf(DataContext.SelectedModel);

            UpdateModel();
        }

        private void UpdateModel()
        {
            IPTextBox.Text = DataContext.SelectedModel?.IP ?? "";
            PortTextBox.Text = DataContext.SelectedModel?.Port.ToString() ?? "";

            InitExchangeData();
            RebuildSignalsList();
        }

        private void InitExchange()
        {
            Exchange.CanExpandGetter = obj => obj is IGroup g && (g.Items?.Any() ?? false);
            Exchange.ChildrenGetter = obj => (obj as IGroup)?.Items;

            Exchange.UseCellFormatEvents = true;
            Exchange.FormatCell += Exchange_FormatCell;

            Exchange.Columns.Add(new OLVColumn("Название", nameof(IGatewayViewItem.Name))
            {
                AspectGetter = obj => (obj as IGatewayViewItem).Name,
                IsEditable = true,
                Sortable = false,
            });

            Exchange.Columns.Add(new OLVColumn("Тип данных", nameof(IGatewayViewItem.DataType))
            {
                AspectGetter = obj => (obj as IGatewayViewItem).DataType,
                IsEditable = true,
                Sortable = false,
            });

            Exchange.Columns.Add(new OLVColumn("Адрес", nameof(IGatewayViewItem.Address))
            {
                AspectGetter = obj => (obj as IGatewayViewItem).Address,
                IsEditable = true,
                Sortable = false,
            });
        }

        private void Exchange_FormatCell(object sender, FormatCellEventArgs e)
        {
            if (e.ColumnIndex == 0 &&
                e.Model is ISignal signal &&
                signal.Device is null)
            {
                e.SubItem.ForeColor = Color.Gray;
            }
        }

        private void InitExchangeData()
        {
            Exchange.BeginUpdate();

            Exchange.Roots = DataContext.SelectedModel?.Roots;

            Exchange.Columns[0].Width = 150;
            Exchange.Columns[1].Width = 50;
            Exchange.Columns[2].Width = 50;

            Exchange.SelectedIndex = 0;
            Exchange.SelectedItem?.EnsureVisible();

            Exchange.EndUpdate();
        }


        private void AddGatewayBttn_Click(object sender, EventArgs e)
        {
            var input = new InputGatewayName(DataContext);
            
            if (input.ShowDialog(this) is DialogResult.Cancel)
                return;

            var name = input.GatewayName;

            DataContext.AddModel(name);
            GatewaySelectionCB.Items.Add(name);
            GatewaySelectionCB.SelectedIndex = DataContext.Models.ToList().
                IndexOf(DataContext.SelectedModel);

            UpdateModel();
        }

        private void ImportGatewayStructBttn_Click(object sender, EventArgs e)
        {
            if (DataContext.SelectedModel is null)
                return;

            DataContext.SelectedModel.ImportCSV();
            InitExchangeData(); 
        }

        public void RebuildSignalsListWithSaveScrollPosition()
        {
            var i = SignalsList.TopItem.Index;
            RebuildSignalsList();
            SignalsList.TopItem = SignalsList.Items[i];
        }

        public void RebuildSignalsList()
        {
            var signals = deviceManager
                .GetModbusExchangeSignals(SelectedModel, SignalsSerch.Text)
                .Select(d => new ListViewItem([d.Name, d.Description]) { Tag = d });

            SignalsList.Items.Clear();
            SignalsList.Items.AddRange(
                [.. signals]);
        }

        private void SignalsList_ItemDrag(object sender, ItemDragEventArgs e)
        {
            SignalsList.DoDragDrop(e.Item, DragDropEffects.Link);
        }

        private void Exchange_DragOver(object sender, DragEventArgs e)
        {
            var p = Exchange.PointToClient(new Point(e.X, e.Y));
            var item = Exchange.GetItemAt(p.X, p.Y, out var _);

            if (e.Data.GetDataPresent(typeof(ListViewItem)) &&
                item?.RowObject is ISignal signal && 
                SelectedModel.CanBind(signal, ((ListViewItem)e.Data.GetData(typeof(ListViewItem))).Tag as IIODevice))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void Exchange_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
            {
                var device = (ListViewItem)e.Data.GetData(typeof(ListViewItem));

                var p = Exchange.PointToClient(new Point(e.X, e.Y));
                var item = Exchange.GetItemAt(p.X, p.Y, out var _);
                if (item?.RowObject is ISignal signal)
                {
                    Bind(signal, device.Tag as IIODevice);
                }
            }
        }

        private void Exchange_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode is Keys.Delete &&
                Exchange.SelectedObject is ISignal signal)
            {
                signal.Device = null;

                RebuildSignalsListWithSaveScrollPosition();
            }
        }

        private void SaveBttn_Click(object sender, EventArgs e)
        {
            DataContext.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelBttn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void GatewaySelectionCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataContext.SelectModel(GatewaySelectionCB.SelectedItem.ToString());

            UpdateModel();
        }

        private void SignalsSerch_TextChanged(object sender, EventArgs e)
        {
            RebuildSignalsList();
        }

        private void Exchange_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (SignalsList.SelectedItems.Count == 0 ||
                Exchange.SelectedObject is not ISignal signal)
                return;

            var device = SignalsList.SelectedItems[0];

            Exchange.SelectedObject = null;
            SignalsList.SelectedItems.Clear();

            Bind(signal, device.Tag as IIODevice);
        }

        private void Bind(ISignal signal, IIODevice device)
        {
            SelectedModel.Bind(signal, device);
            RebuildSignalsListWithSaveScrollPosition();
        }

        private void Port_TextBox_Leave(object sender, EventArgs e)
        {
            if (sender is not TextBox textBox)
                return;

            if (DataContext.SelectedModel is null)
            {
                textBox.Clear();
                return;
            }

            if (int.TryParse(textBox.Text, out var port))
            {
                DataContext.SelectedModel.Port = port;
            }
            else 
            {
                textBox.Text = DataContext.SelectedModel.Port.ToString();
            }
                
        }

        private void IP_TextBox_Leave(object sender, EventArgs e)
        {
            if (sender is not TextBox textBox)
                return;

            if (DataContext.SelectedModel is null)
            {
                textBox.Clear();
                return;
            }

            if ( string.IsNullOrEmpty(textBox.Text) ||
                Regex.IsMatch(textBox.Text, CommonConst.IPAddressPattern,
                RegexOptions.None, TimeSpan.FromMilliseconds(100)))
            {
                DataContext.SelectedModel.IP = textBox.Text;
            }
            else
            {
                textBox.Text = DataContext.SelectedModel.IP;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                ActiveControl = null;
            }

            if (e.KeyCode == Keys.Escape)
            {
                (sender as TextBox)?.Undo();
                ActiveControl = null;
            }
        }

        private void Exchange_Expanded(object sender, TreeBranchExpandedEventArgs e)
        {
            Exchange.Columns[0]
                .AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void Exchange_Collapsed(object sender, TreeBranchCollapsedEventArgs e)
        {
            Exchange.Columns[0]
                .AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void ModbusExchangeView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult is not DialogResult.Cancel)
                return;

            switch (SaveAndCloseDialog.ShowDialog(this))
            {
                case DialogResult.Yes:
                    DataContext.Save();
                    return;

                case DialogResult.Cancel:
                    e.Cancel = true;
                    return;
            }
        }

        private void Exchange_CellEditStarting(object sender, CellEditEventArgs e)
        {
            if (e.Column.Index != 2 ||
                Exchange.SelectedObject is not IGroup group)
            {
                e.Cancel = true;
                return;
            }

            textBoxCellEditor = new TextBox
            {
                Enabled = true,
                Visible = true,
                Bounds = e.CellBounds,
                Text = group.Offset.ToString(),
            };
            Exchange.Controls.Add(textBoxCellEditor);
            e.Control = textBoxCellEditor;
            textBoxCellEditor.Focus();
            Exchange.Freeze();
        }

        private void Exchange_CellEditFinishing(object sender, CellEditEventArgs e)
        {
            if (Exchange.SelectedObject is not IGroup group)
            {
                e.Cancel = true;
                Exchange.Unfreeze();
                return;
            }

            if (int.TryParse(e.NewValue.ToString(), out var offset))
            {
                group.Offset = offset;
                Exchange.RefreshObject(group);
                Exchange.Controls.Remove(textBoxCellEditor);
            }

            e.Cancel = true;
            Exchange.Unfreeze();
        }
    }
}
