using Eplan.EplApi.ApplicationFramework;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace EasyEPlanner
{
    /// <summary>
    /// Действие для открытия средства обновления надстройки
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OpenUpdater : IEplAction
    {
        ~OpenUpdater() { }

        public bool Execute(ActionCallingContext ctx)
        {
            var path = new FileInfo(AddInModule.OriginalAssemblyPath).Directory.FullName;
            if (File.Exists(path + AddInModule.LauncherPath))
            {
                var proc = Process.Start(path + AddInModule.LauncherPath, $"{AddInModule.RunUpdaterFromMenu} {Process.GetCurrentProcess().Id}");
                proc.WaitForExit();
                return true;
            }

            MessageBox.Show($"Средство обновления {AddInModule.LauncherPath} не найдено!",
                "Eplanner", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = nameof(OpenUpdater);

            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties) { }
    }
}
