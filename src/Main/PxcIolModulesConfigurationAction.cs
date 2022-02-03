using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using System;
using System.Windows.Forms;
using EasyEPlanner.PxcIolinkConfiguration;
using System.Threading.Tasks;

namespace EasyEPlanner
{
    /// <summary>
    /// Действие "Сгенерировать описание IO-Link модулей Phoenix Contact
    /// </summary>
    internal class PxcIolModulesConfigurationAction : IEplAction
    {
        ~PxcIolModulesConfigurationAction() { }

        public bool Execute(ActionCallingContext ctx)
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
                    var oProgress = new Eplan.EplApi.Base.Progress("EnhancedProgress");
                    oProgress.SetAllowCancel(true);
                    oProgress.SetTitle("Генерация описания IOL-Conf");

                    oProgress.BeginPart(50, "Чтение шаблонов и генерация файлов");
                    IPxcIolinkConfiguration pxcConfiguration = new PxcIolinkModulesConfiguration();
                    pxcConfiguration.Run();
                    oProgress.EndPart();
                    
                    oProgress.BeginPart(90, "Обработка ошибок");
                    if (pxcConfiguration.HasErrors)
                    {
                        Logs.Clear();
                        var errorsList = pxcConfiguration.ErrorsList;
                        Logs.AddMessage("Генерация IOL-Conf завершилась с ошибками.");
                        foreach (var errorMessage in errorsList)
                        {
                            Logs.AddMessage(errorMessage);
                        }
                        oProgress.EndPart(true);
                        Logs.EnableButtons();
                        Logs.Show();
                        return false;
                    }
                    oProgress.EndPart(true);
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
