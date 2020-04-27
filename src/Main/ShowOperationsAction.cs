using Eplan.EplApi.ApplicationFramework;
using System;
using System.Windows.Forms;

namespace EasyEPlanner
{
    /// <summary>
    /// Действие "Операции"
    /// </summary>
    public class ShowOperationsAction : IEplAction
    {
        ~ShowOperationsAction()
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
                if (EProjectManager.GetInstance().GetCurrentPrj() == null)
                {
                    MessageBox.Show("Нет открытого проекта!", "EPlaner",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {

                    ModeFrm.OnSetNewValue OnSetNewValueFunction = null;
                    bool isRebuiltTree = true;

                    ModeFrm.GetInstance().ShowModes(
                        TechObject.TechObjectManager.GetInstance(),
                        false, false, null, null, OnSetNewValueFunction, 
                        isRebuiltTree);
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
        /// <param name="Ordinal">The action is registered with this 
        /// overload priority.</param>
        ///<returns>true, if OnRegister succeeds</returns>
        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "ShowOperationsAction";
            Ordinal = 36;

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
