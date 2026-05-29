using PInvoke;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace EasyEPlanner.Devices.View
{
    /// <summary>
    /// Заготовка для будущей встройки окна в панель EPLAN «Клеммники».
    /// В v1 окно открывается как плавающая форма.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class DevicesViewControl : Form
    {
        public static readonly string CfgShowWindowKey = "show_devices_new_window";

        public static void SaveCfg()
        {
            SaveCfg(Instance?.Visible ?? false);
        }

        public static void SaveCfg(bool wndState)
        {
            var path = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData);
            var ini = new IniFile(path + @"\Eplan\eplan.cfg");
            ini.WriteString("main", CfgShowWindowKey, wndState.ToString().ToLower());
        }
    }
}
