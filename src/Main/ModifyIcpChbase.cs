using EasyEPlanner.ProjectImportICP;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using EplanDevice;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace EasyEPlanner
{
    [ExcludeFromCodeCoverage]
    public class ModifyIcpChbase : IEplAction
    {
        ~ModifyIcpChbase() { }

        public bool Execute(ActionCallingContext oActionCallingContext)
        {
            try
            {
                Project currentProject = EProjectManager.GetInstance()
                    .GetCurrentPrj();
                if (currentProject == null)
                {
                    MessageBox.Show("Нет открытого проекта!", "EPlaner",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return true;
                }

                new ModifyChannelsDBDialog(currentProject.ProjectName).ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return true;
        }

        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = nameof(ModifyIcpChbase);
            Ordinal = 30;

            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}
