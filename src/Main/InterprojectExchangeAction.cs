using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using System;
using System.Windows.Forms;

namespace EasyEPlanner
{
    /// <summary>
    /// Действие "Обмен сигналами между проектами"
    /// </summary>
    public class InterprojectExchangeAction : IEplAction
    {
        ~InterprojectExchangeAction()
        {
        }
        /// <summary>
        ///This function is called when executing the action.
        /// </summary>
        ///<returns>true, if the action performed successfully</returns>
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
                    //TODO: Open form, load files.
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
        /// <param name="Name">The action is registered in EPLAN 
        /// under this name.</param>
        /// <param name="Ordinal">The action is registered with 
        /// this overload priority.</param>
        ///<returns>true, if OnRegister succeeds</returns>
        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "InterprojectExchange";
            Ordinal = 18;

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
