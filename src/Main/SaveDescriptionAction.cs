using Eplan.EplApi.ApplicationFramework;
using System;
using System.Windows.Forms;

namespace EasyEPlanner
{
    /// <summary>
    /// Действие сохранения описания проекта в lua файлы
    /// </summary>
    public class SaveDescriptionAction : IEplAction
    {
        /// <summary>
        ///This function is called when executing the action.
        /// </summary>
        ///<returns>true, if the action performed successfully</returns>
        public bool Execute(ActionCallingContext ctx)
        {
            try
            {
                string pVal = "no";
                ctx.GetParameter("silentMode", ref pVal);
                bool silentMode = false;
                if (pVal == "yes")
                {
                    silentMode = true;
                }

                if (EProjectManager.GetInstance().GetCurrentPrj() == null)
                {
                    if (!silentMode)
                    {
                        MessageBox.Show("Нет открытого проекта!", "EPlaner",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    string projectName = EProjectManager.GetInstance()
                        .GetCurrentProjectName();
                    EProjectManager.GetInstance()
                        .CheckProjectName(ref projectName);
                    string path = ProjectManager.GetInstance()
                        .GetPtusaProjectsPath(projectName) + projectName;
                    ProjectManager.GetInstance().SaveAsLua(projectName, path,
                        silentMode);

                    SVGStatisticsSaver.Save(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return true;
        }

        /// <summary>
        ///This function is called by the application framework, when 
        ///registering the add-in.
        /// </summary>
        /// <param name="Name">The action is registered in EPLAN under 
        /// this name.</param>
        /// <param name="Ordinal">The action is registered with this overload 
        /// priority.</param>
        ///<returns>true, if OnRegister succeeds</returns>
        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "SaveDescriptionAction";
            Ordinal = 20;
            return true;
        }

        /// <summary>
        /// Documentation function for the action.
        /// </summary>
        /// <param name="actionProperties"> This object needs to be filled 
        /// with information about the action.</param>
        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}
