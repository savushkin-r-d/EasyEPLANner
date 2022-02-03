using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Starter;
using System;
using System.Windows.Forms;
[assembly: EplanSignedAssemblyAttribute(true)]

namespace EasyEPlanner
{
    /// <summary>
    /// Точка входа в приложение, стартовый класс.
    /// </summary>
    public class AddInModule : IEplAddIn, IEplAddInShadowCopy
    {
        /// <summary>
        /// This function is called by the framework of EPLAN, when the 
        /// framework already has initialized its graphical user interface 
        /// (GUI) and the add-in can start to modify the GUI.
        /// The function only is called, if the add-in is loaded on 
        /// system-startup.
        /// </summary>
        /// <returns>true, if function succeeds</returns>
        public bool OnInitGui()
        {
            Eplan.EplApi.Gui.Menu oMenu = new Eplan.EplApi.Gui.Menu();

            uint menuID = oMenu.AddMainMenu(
                "EPlaner", Eplan.EplApi.Gui.Menu.MainMenuName.eMainMenuHelp,
                "Экспорт XML для EasyServer", "SaveAsXMLAction",
                "Экспорт XML для EasyServer", 0);

            menuID = oMenu.AddMenuItem(
                "Экспорт технологических устройств в Excel",
                "ExportTechDevsToExcel",
                "Экспорт технологических устройств в Excel", menuID, 1, 
                false, true);

            menuID = oMenu.AddMenuItem("Редактировать технологические объекты",
                "ShowTechObjectsAction",
                "Редактирование технологических объектов", menuID, 1, 
                false, false);

            menuID = oMenu.AddMenuItem("Устройства, параметры объектов", 
                "ShowDevicesAction", "Отображение устройств", menuID,
                int.MaxValue, false, false);

            menuID = oMenu.AddMenuItem(
                "Операции, ограничения и привязка объектов",
                "ShowOperationsAction","Отображение операций", menuID, 1,
                false, true);

            menuID = oMenu.AddMenuItem("Обмен сигналами между проектами",
                "InterprojectExchangeAction",
                "Настройка межпроектного обмена сигналами", menuID, 1, false, 
                false);

            menuID = oMenu.AddMenuItem(
                "Синхронизация названий устройств и модулей", 
                "BindingSynchronization",
                "Синхронизация названий устройств и модулей", menuID, 1, 
                false, false);

            menuID = oMenu.AddMenuItem(
                "Генерация описания IOL-Conf",
                "PxcIolModulesConfiguration",
                "Генерация описания IOL-Conf", menuID, 1,
                false, false);

            menuID = oMenu.AddMenuItem("О дополнении", "AboutProgramm", "", 
                menuID, 1, true, false);

            ProjectManager.GetInstance().Init();

            return true;
        }

        public bool OnRegister(ref bool bLoadOnStart)
        {
            bLoadOnStart = true;
            return true;
        }

        public bool OnUnregister()
        {
            return true;
        }

        public bool OnInit()
        {
            return true;
        }

        public bool OnExit()
        {
            return true;
        }

        public void OnBeforeInit(string strOriginalAssemblyPath)
        {
            OriginalAssemblyPath = strOriginalAssemblyPath;
        }

        /// <summary>
        /// Путь к дополнению (откуда подключена).
        /// </summary>
        public static string OriginalAssemblyPath;
    }

    /// <summary>    
    /// Вспомогательные функции.
    /// </summary>
    public class WindowWrapper : IWin32Window
    {
        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get
            {
                return _hwnd;
            }
        }

        private IntPtr _hwnd;
    }
}
