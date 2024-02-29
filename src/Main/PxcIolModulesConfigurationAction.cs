using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using System;
using System.Windows.Forms;

namespace EasyEPlanner
{
    /// <summary>
    /// Действие "Сгенерировать описание IO-Link модулей Phoenix Contact
    /// </summary>
    internal class PxcIolModulesConfigurationAction : IEplAction
    {
        ~PxcIolModulesConfigurationAction() { }

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
                    ProjectManager.GetInstance().GenerateIolConf();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "PxcIolModulesConfiguration";
            Ordinal = 31;

            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {

        }
    }
}
