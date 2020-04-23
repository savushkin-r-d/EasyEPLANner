using Eplan.EplApi.ApplicationFramework;
using System.Windows.Forms;

namespace EasyEPlanner
{
    /// <summary>
    /// Действие загрузки описания проекта из lua файлов
    /// </summary>
    public class LoadDescriptionAction : IEplAction
    {
        ~LoadDescriptionAction()
        {
        }

        /// <summary>
        ///This function is called when executing the action.
        /// </summary>
        ///<returns>true, if the action performed successfully</returns>
        public bool Execute(ActionCallingContext ctx)
        {
            string pVal = "no";
            ctx.GetParameter("loadFromLua", ref pVal);
            bool loadFromLua = true;
            if (pVal == "no")
            {
                loadFromLua = false;
            }

            string errStr;

            string projectName = EProjectManager.GetInstance()
                .GetCurrentProjectName();
            EProjectManager.GetInstance().CheckProjectName(ref projectName);

            int res = ProjectManager.GetInstance().LoadDescription(out errStr,
                projectName, loadFromLua);
            if (res > 0)
            {
                MessageBox.Show(errStr, "EPlaner", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }


            return true;
        }

        /// <summary>
        ///This function is called by the application framework, when 
        ///registering the add-in.
        /// </summary>
        /// <param name="Name">The action is registered in EPLAN under 
        /// this name.</param>
        /// <param name="Ordinal">The action is registered with this 
        /// overload priority.</param>
        ///<returns>true, if OnRegister succeeds</returns>
        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "LoadDescriptionAction";
            Ordinal = 30;

            return true;
        }

        /// <summary>
        /// Documentation function for the action.
        /// </summary>
        /// <param name="actionProperties"> This object needs to be 
        /// filled with information about the action.</param>
        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}
