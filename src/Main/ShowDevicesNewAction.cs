using EasyEPlanner.Devices.View;
using Eplan.EplApi.ApplicationFramework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace EasyEPlanner.Main
{
    [ExcludeFromCodeCoverage]
    public class ShowDevicesNewAction : IEplAction
    {
        public bool Execute(ActionCallingContext oActionCallingContext)
        {
            try
            {
                if (EProjectManager.GetInstance().GetCurrentPrj() is null)
                {
                    MessageBox.Show("Нет открытого проекта!", "EPlaner",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    DevicesViewControl.Start();
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
            Name = nameof(ShowDevicesNewAction);
            Ordinal = 22;
            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}
