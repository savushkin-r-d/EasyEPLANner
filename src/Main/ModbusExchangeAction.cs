using EasyEPlanner.ModbusExchange.View;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.Main
{
    [ExcludeFromCodeCoverage]
    internal class ModbusExchangeAction : IEplAction
    {
        ~ModbusExchangeAction()
        {
        }

        public bool Execute(ActionCallingContext oActionCallingContext)
        {
            try
            {
                Project currentProject = EProjectManager.GetInstance().
                    GetCurrentPrj();
                if (currentProject == null)
                {
                    MessageBox.Show("Нет открытого проекта!", "EPlaner",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    var exchange = new ModbusExchangeView();
                    exchange.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;
        }

        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = nameof(ModbusExchangeAction);
            Ordinal = 30;

            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}
