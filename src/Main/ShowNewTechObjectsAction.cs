using Eplan.EplApi.ApplicationFramework;
using System.Windows.Forms;

namespace EasyEPlanner
{
    /// <summary>
    /// Действие "Новый редактор технологических объектов"
    /// </summary>
    public class ShowTechNewObjectsAction : IEplAction
    {
        /// <summary>
        ///This function is called when executing the action.
        /// </summary>
        ///<returns>true, if the action performed successfully</returns>
        public bool Execute(ActionCallingContext ctx)
        {
            if (EProjectManager.GetInstance().GetCurrentPrj() == null)
            {
                MessageBox.Show("Нет открытого проекта!", "EPlaner",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                //Редактирование объектов.
                ProjectManager.GetInstance().StartEdit();
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
            Name = "ShowNewTechObjectsAction";
            Ordinal = 21;
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
