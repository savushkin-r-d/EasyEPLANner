using Eplan.EplApi.ApplicationFramework;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace EasyEPlanner
{
    /// <summary>
    /// Действие "О программе"
    /// </summary>
    public class AboutProgramm : IEplAction
    {
        ~AboutProgramm() { }

        public bool Execute(ActionCallingContext ctx)
        {
            string version = AssemblyVersion.GetVersion();
            string message = $"Версия надстройки - {version}\n" +
               "Проект распространяется под лицензией MIT.";
            MessageBox.Show(message, "Версия надстройки", 
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return true;
        }

        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "AboutProgramm";
            Ordinal = 30;

            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}
