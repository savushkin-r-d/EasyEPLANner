using BrightIdeasSoftware;
using EasyEPlanner.ModbusExchange.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.ModbusExchange.View
{
    public partial class ModbusExchangeView : Form
    {
        IExchange DataContext;

        public ModbusExchangeView()
        {
            InitializeComponent();
            InitExchange();

            DataContext = new Exchange();
        }

        private void InitExchange()
        {
            Exchange.CanExpandGetter = obj => obj is IGroup g && (g.Items?.Any() ?? false);
            Exchange.ChildrenGetter = obj => (obj as IGroup)?.Items;


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


        private void InitExchangeData()
        {
            Exchange.BeginUpdate();

            Exchange.Roots = DataContext.SelectedModel.Roots;

            Exchange.Columns[0].Width = 200;
            Exchange.Columns[1].Width = 100;
            Exchange.Columns[2].Width = 100;

            Exchange.SelectedIndex = 0;
            Exchange.SelectedItem.EnsureVisible();

            Exchange.EndUpdate();
        }


        private void AddGatewayBttn_Click(object sender, EventArgs e)
        {
            var input = new InputGatewayName();
            input.ShowDialog();

            if (input.DialogResult is DialogResult.Cancel)
                return;

            var name = input.GatewayName;

            DataContext.AddModel(name);
            InitExchangeData();
        }
    }
}
