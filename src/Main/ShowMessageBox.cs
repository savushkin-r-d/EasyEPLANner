using Eplan.EplApi.ApplicationFramework;
using System.Windows.Forms;

namespace EasyEPlanner.Main
{
    class ShowMessageBox : IEplAction
    {
        ~ShowMessageBox() { }

        public bool Execute(ActionCallingContext oActionCallingContext)
        {
            MessageBox.Show("Кнопка меню нажата", "MB");
            return true;
        }


        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = nameof(ShowMessageBox);

            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}