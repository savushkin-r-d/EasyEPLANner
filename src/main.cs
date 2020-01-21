///@file main.cs
///@brief Классы, реализующие дополнение.
///
/// @author  Иванюк Дмитрий Сергеевич.
///
/// @par Текущая версия:
/// @$Rev ---$.\n
/// @$Author sedr$.\n
/// @$Date: 2019-10-21#$.
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using Eplan.EplApi.DataModel;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Base;
using Eplan.EplApi.HEServices;

#region Signing
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Eplan.EplApi.Starter;
using Eplan.EplApi.DataModel.EObjects;

[assembly: EplanSignedAssemblyAttribute(true)]
#endregion

namespace EasyEPlanner
{
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    public class AddInModule : IEplAddIn, IEplAddInShadowCopy
    {
        /// <summary>
        /// This function is called by the framework of EPLAN, when the framework already has initialized its
        /// graphical user interface (GUI) and the add-in can start to modify the GUI.
        /// The function only is called, if the add-in is loaded on system-startup.
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
                "Экспорт технологических устройств в Excel", menuID, 1, false, true);

            menuID = oMenu.AddMenuItem("Редактировать технологические объекты",
                "ShowTechObjectsAction",
                "Редактирование технологических объектов", menuID, 1, false, true);

            menuID = oMenu.AddMenuItem("Устройства", "ShowDevicesAction",
                "Отображение устройств", menuID, int.MaxValue, false, false);

            menuID = oMenu.AddMenuItem("Операции", "ShowOperationsAction",
                "Отображение операций", menuID, 1, false, false);

            menuID = oMenu.AddMenuItem("Синхронизация названий устройств и модулей", "BindingSynchronization",
                "Синхронизация названий устройств и модулей", menuID, 1, false, false);

            menuID = oMenu.AddMenuItem("О дополнении", "AboutProgramm", "", menuID, 1, true, false);

            ProjectManager.GetInstance().Init(EplanIOManager.GetInstance(),
                EplanDeviceManager.GetInstance(), Editor.Editor.GetInstance(),
                TechObject.TechObjectManager.GetInstance(), new LogFrm());

            // Вызов GetInstance() для создания объекта EProjectManager.
            EProjectManager.GetInstance();

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

        // Путь к надстройке
        public static string OriginalAssemblyPath;
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    public class AboutProgramm : IEplAction
    {
        ~AboutProgramm() { }

        public bool Execute(ActionCallingContext ctx)
        {
            string version = $"Версия надстройки - {GetVersion()}\n";
            string license = "Проект распространяется под лицензией - MIT";
            string text = version + license;

            MessageBox.Show(text,
           "Версия надстройки", MessageBoxButtons.OK,
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

        /// <summary>
        /// Получить версию надстройки
        /// </summary>
        /// <returns></returns>
        private string GetVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().
                Version;
            return version.ToString();
        }
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
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

            string projectName = EProjectManager.GetInstance().GetCurrentProjectName();
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
        ///This function is called by the application framework, when registering the add-in.
        /// </summary>
        /// <param name="Name">The action is registered in EPLAN under this name.</param>
        /// <param name="Ordinal">The action is registered with this overload priority.</param>
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
        /// <param name="actionProperties"> This object needs to be filled with information about the action.</param>
        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    public class BindingSynchronization : IEplAction
    {
        ~BindingSynchronization()
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
                    // Синхронизация названий модулей и устройств в текущем
                    // проекте.
                    ProjectManager.GetInstance().UpdateModulesBinding();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;
        }

        /// <summary>
        ///This function is called by the application framework, when registering the add-in.
        /// </summary>
        /// <param name="Name">The action is registered in EPLAN under this name.</param>
        /// <param name="Ordinal">The action is registered with this overload priority.</param>
        ///<returns>true, if OnRegister succeeds</returns>
        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "BindingSynchronization";
            Ordinal = 17;

            return true;
        }

        /// <summary>
        /// Documentation function for the action.
        /// </summary>
        /// <param name="actionProperties"> This object needs to be filled with information about the action.</param>
        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }

    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    public class ExportTechDevsToExcel : IEplAction
    {
        ~ExportTechDevsToExcel()
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
                Project currentProject = EProjectManager.GetInstance().GetCurrentPrj();
                if (currentProject == null)
                {
                    MessageBox.Show("Нет открытого проекта!", "EPlaner",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    ProjectManager.GetInstance().SaveAsExcelDescription(
                        currentProject.ProjectDirectoryPath + @"\DOC\" +
                        currentProject.ProjectName + ".xlsx");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;
        }

        /// <summary>
        ///This function is called by the application framework, when registering the add-in.
        /// </summary>
        /// <param name="Name">The action is registered in EPLAN under this name.</param>
        /// <param name="Ordinal">The action is registered with this overload priority.</param>
        ///<returns>true, if OnRegister succeeds</returns>
        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "ExportTechDevsToExcel";
            Ordinal = 31;

            return true;
        }

        /// <summary>
        /// Documentation function for the action.
        /// </summary>
        /// <param name="actionProperties"> This object needs to be filled with 
        /// information about the action.</param>
        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    public class SaveAsXMLAction : IEplAction
    {
        ~SaveAsXMLAction()
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
                Project currentProject = EProjectManager.GetInstance().GetCurrentPrj();
                if (currentProject == null)
                {
                    MessageBox.Show("Нет открытого проекта!", "EPlaner",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    DialogResult result = MessageBox.Show("Желаете создать новый файл базы каналов?",
                        "Запись базы каналов", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        FolderBrowserDialog FBD = new FolderBrowserDialog();
                        if (FBD.ShowDialog() == DialogResult.OK)
                        {
                            DialogResult resultTag = MessageBox.Show("Сгруппировать теги технологических объектов в один подтип?",
                            "Запись базы каналов", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (resultTag == DialogResult.Yes)
                            {
                                ProjectManager.GetInstance().SaveAsCDBX(
                                FBD.SelectedPath + "\\" + currentProject.ProjectName + ".cdbx", true);
                            }
                            else
                            {
                                ProjectManager.GetInstance().SaveAsCDBX(
                                FBD.SelectedPath + "\\" + currentProject.ProjectName + ".cdbx");
                            }

                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        OpenFileDialog OPF = new OpenFileDialog();
                        OPF.Filter = "Файлы базы каналов|*.cdbx";
                        if (OPF.ShowDialog() == DialogResult.OK)
                        {
                            DialogResult resultTag = MessageBox.Show("Сгруппировать теги технологических объектов в один подтип?",
                            "Запись базы каналов", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (resultTag == DialogResult.Yes)
                            {
                                ProjectManager.GetInstance().SaveAsCDBX(OPF.FileName, true);
                            }
                            else
                            {
                                ProjectManager.GetInstance().SaveAsCDBX(OPF.FileName);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return true;
        }

        /// <summary>
        ///This function is called by the application framework, when registering the add-in.
        /// </summary>
        /// <param name="Name">The action is registered in EPLAN under this name.</param>
        /// <param name="Ordinal">The action is registered with this overload priority.</param>
        ///<returns>true, if OnRegister succeeds</returns>
        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "SaveAsXMLAction";
            Ordinal = 30;

            return true;
        }

        /// <summary>
        /// Documentation function for the action.
        /// </summary>
        /// <param name="actionProperties"> This object needs to be filled with information about the action.</param>
        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    public class ShowDevicesAction : IEplAction
    {
        ~ShowDevicesAction()
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
                    Device.DeviceType[] devTypes = null;            // All.
                    Device.DeviceSubType[] devSubTypes = null;      // All.

                    bool showChannels = true;
                    bool showCheckboxes = false;
                    string checkedDev = "";
                    DFrm.OnSetNewValue OnSetNewValueFunction = null;
                    bool isRebuiltTree = true;

                    DFrm.GetInstance().ShowDevices(
                        Device.DeviceManager.GetInstance(), devTypes, devSubTypes,
                        showChannels, showCheckboxes, checkedDev,
                        OnSetNewValueFunction, isRebuiltTree);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return true;
        }

        /// <summary>
        ///This function is called by the application framework, when registering the add-in.
        /// </summary>
        /// <param name="Name">The action is registered in EPLAN under this name.</param>
        /// <param name="Ordinal">The action is registered with this overload priority.</param>
        ///<returns>true, if OnRegister succeeds</returns>
        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "ShowDevicesAction";
            Ordinal = 21;

            return true;
        }

        /// <summary>
        /// Documentation function for the action.
        /// </summary>
        /// <param name="actionProperties"> This object needs to be filled with information about the action.</param>
        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
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
                        false, false, null, null, OnSetNewValueFunction, isRebuiltTree);
                    //false, false, null, "", OnSetNewValueFunction, isRebuiltTree);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return true;
        }

        /// <summary>
        ///This function is called by the application framework, when registering the add-in.
        /// </summary>
        /// <param name="Name">The action is registered in EPLAN under this name.</param>
        /// <param name="Ordinal">The action is registered with this overload priority.</param>
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
        /// <param name="actionProperties"> This object needs to be filled with information about the action.</param>
        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
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
                    string path = ProjectManager.GetInstance().GetPtusaProjectsPath() +
                        EProjectManager.GetInstance().GetModifyingCurrentProjectName();
                    string projectName = EProjectManager.GetInstance().GetCurrentProjectName();
                    EProjectManager.GetInstance().CheckProjectName(ref projectName);

                    ProjectManager.GetInstance().SaveAsLua(projectName, path, silentMode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return true;
        }

        /// <summary>
        ///This function is called by the application framework, when registering the add-in.
        /// </summary>
        /// <param name="Name">The action is registered in EPLAN under this name.</param>
        /// <param name="Ordinal">The action is registered with this overload priority.</param>
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
        /// <param name="actionProperties"> This object needs to be filled with information about the action.</param>
        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
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

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    public class ShowTechObjectsAction : IEplAction
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
                ProjectManager.GetInstance().Edit();
            }

            return true;
        }

        /// <summary>
        ///This function is called by the application framework, when registering the add-in.
        /// </summary>
        /// <param name="Name">The action is registered in EPLAN under this name.</param>
        /// <param name="Ordinal">The action is registered with this overload priority.</param>
        ///<returns>true, if OnRegister succeeds</returns>
        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "ShowTechObjectsAction";
            Ordinal = 21;
            return true;
        }

        /// <summary>
        /// Documentation function for the action.
        /// </summary>
        /// <param name="actionProperties"> This object needs to be filled with information about the action.</param>
        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------

    /// <summary>
    /// Все узлы проекта модулей ввода-вывода IO.
    /// </summary>
    class EplanIOManager : IO.IIOManager
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        private EplanIOManager()
        {
            iOManager = IO.IOManager.GetInstance();
        }

        /// <summary>
        /// Получение экземпляра класса.
        /// </summary>
        /// <returns>Единственный экземпляр класса.</returns>
        public static EplanIOManager GetInstance()
        {
            if (null == instance)
            {
                instance = new EplanIOManager();
            }

            return instance;
        }

        private static int CompareF(Function x, Function y)
        {
            return x.VisibleName.CompareTo(y.VisibleName);
        }

        /// <summary>
        /// Считывание информации о модулях из проекта.
        /// Информацию получаем на основе листа с шиной модулей ввода\вывода 
        /// IO. У изделий должны быть заполнены соответствующие свойства,
        /// они должны соответствующим образом именоваться.
        /// Алгоритм:
        /// 1. С помощью фильтра выбираем функциональные объекты  в пределах 
        /// открытого проекта со следующими ограничениями: категория PLCBox,
        /// главная функция, производитель изделия WAGO/ Phoenix.
        /// 2. Обрабатываем функциональные объекты со следующим именем:
        /// -Аxxx, где А - признак элемента IO, xxx - номер.
        /// Так же учитываем контроллер Phoenix Contact с ОУ "А1", если он есть.
        /// 3.  Вначале обрабатываем элементы с номером, кратным 100 - это узел 
        /// IO. Добавляем их в список узлов IO. Тип узла получаем из 
        /// изделия.
        /// 4. Обрабатываем остальные элементы - модули IO. Помещаем их в 
        /// списки согласно порядковому номеру (вставляем при этом пустые 
        /// элементы, если длина списка короче номера нового добавляемого 
        /// модуля).
        /// </summary>
        public void ReadConfiguration()
        {
            //Получение текущего проекта, для которого осуществляется экспорт.
            SelectionSet selection = new SelectionSet();
            selection.LockSelectionByDefault = false;

            Project cProject = selection.GetCurrentProject(true);

            //Initialize the DMObjectsFinder with a project.
            DMObjectsFinder oFinder = new DMObjectsFinder(cProject);
            FunctionsFilter oFunctionsFilter = new FunctionsFilter();

            FunctionPropertyList props = new FunctionPropertyList();
            props.FUNC_MAINFUNCTION = true;

            oFunctionsFilter.SetFilteredPropertyList(props);
            oFunctionsFilter.Category = Function.Enums.Category.PLCBox;

            List<Function> tmpFList = oFinder.GetFunctions(oFunctionsFilter).ToList(); //1
            tmpFList.Sort(CompareF);

            //Очищаем информацию о IO.
            iOManager.Clear();

            //Шаблоны для разбора названий модулей и узлов IO.
            string PATTERN_NAME = @"=*-A(?<n>\d+)";                //-A101
            Regex regex_name = new Regex(PATTERN_NAME);

            bool is_exist_any_node = false;

            // Наличие модуля типа "А1"          
            bool isContainsA1 = CheckPhoenixContactMainNode(tmpFList, regex_name);

            foreach (Function oF in tmpFList)
            {
                Match match = regex_name.Match(oF.VisibleName);                //2

                if (match.Success)
                {
                    //Признак пропуска данного устройства.
                    if (!oF.Properties.FUNC_SUPPLEMENTARYFIELD[1].IsEmpty)
                    {
                        continue;
                    }

                    // Номер узла/модуля
                    int physicalNumber = Convert.ToInt32(match.Groups["n"].Value);

                    // Проверка узлов
                    if (physicalNumber % 100 == 0 || physicalNumber == 1)                                      //3
                    {
                        is_exist_any_node = true;

                        // Имя узла/модуля
                        string name = $"A{physicalNumber}";

                        string IP = "";
                        if (!oF.Properties.FUNC_PLCGROUP_STARTADDRESS.IsEmpty)
                        {
                            IP = oF.Properties.FUNC_PLCGROUP_STARTADDRESS;
                        }
                        else
                        {
                            ProjectManager.GetInstance().AddLogMessage("У узла \"" +
                                oF.VisibleName + "\" не задан IP-адрес.");
                        }


                        if (oF.Articles.GetLength(0) == 0)
                        {
                            ProjectManager.GetInstance().AddLogMessage("У модуля \"" +
                                oF.VisibleName + "\" не задано изделие.");
                            continue;
                        }

                        string type = "";
                        foreach (Article art in oF.Articles)
                        {
                            if (!art.Properties[
                                Eplan.EplApi.DataModel.Properties.Article.ARTICLE_TYPENR].IsEmpty)
                            {
                                type = art.Properties[
                                    Eplan.EplApi.DataModel.Properties.Article.ARTICLE_TYPENR];

                                bool ok = false;

                                switch (type)
                                {
                                    case "750-863":
                                    case "750-341":
                                    case "750-841":
                                    case "750-352":
                                    case "750-8202":
                                    case "750-8203":
                                    case "750-8204":
                                    case "750-8206":
                                    case "AXL F BK ETH":
                                    case "AXC F 2152":
                                        ok = true;
                                        break;

                                    default:
                                        break;
                                }
                                if (ok) break;
                            }
                        }

                        if (type != "")
                        {
                            // Если в проекте есть узел PXC "A1"
                            if (isContainsA1 == true)
                            {
                                // Если это PXC А1
                                if (physicalNumber == 1)
                                {
                                    iOManager.AddNode(1, type, IP, name);
                                }
                                else
                                {
                                    iOManager.AddNode(physicalNumber / 100 + 1, type, IP, name);
                                }
                            }
                            else
                            {
                                iOManager.AddNode(physicalNumber / 100, type, IP, name);
                            }
                        }
                        else
                        {
                            ProjectManager.GetInstance().AddLogMessage("У модуля \"" +
                                oF.VisibleName + "\" не задан параметр изделия (номер типа изделия).");
                        }
                    }
                }
            }

            if (is_exist_any_node == false)
            {
                ProjectManager.GetInstance().AddLogMessage("Не найден ни один узловой модуль (A100, A200, ...).");

                return;
            }

            foreach (Function oF in tmpFList)
            {
                Match match_name = regex_name.Match(oF.VisibleName);

                if (match_name.Success)
                {
                    //Признак пропуска данного устройства.
                    if (!oF.Properties.FUNC_SUPPLEMENTARYFIELD[1].IsEmpty)
                    {
                        continue;
                    }

                    int physicalNumber = Convert.ToInt32(match_name.Groups["n"].Value);

                    if (physicalNumber % 100 != 0 && physicalNumber != 1)                                 //4
                    {
                        int module_n = physicalNumber % 100;
                        int node_n;

                        // Если есть "A1", то учитываем, что он первый
                        if (isContainsA1 == true)
                        {
                            node_n = physicalNumber / 100;
                        }
                        else
                        {
                            // А1 нету, оставляем как было
                            node_n = physicalNumber / 100 - 1;
                        }

                        string name = "";

                        if (oF.Articles.GetLength(0) == 0)
                        {
                            ProjectManager.GetInstance().AddLogMessage("У модуля \"" +
                                oF.VisibleName + "\" не задано изделие.");
                            continue;
                        }

                        if (!oF.Articles[0].Properties[
                            Eplan.EplApi.DataModel.Properties.Article.ARTICLE_TYPENR].IsEmpty)
                        {
                            name = oF.Articles[0].Properties[
                                Eplan.EplApi.DataModel.Properties.Article.ARTICLE_TYPENR].ToString().Trim();
                        }
                        else
                        {
                            ProjectManager.GetInstance().AddLogMessage("У модуля \"" +
                                oF.VisibleName + "\" не задан параметр изделия (номер для заказа).");
                        }

                        bool isStub;

                        IO.IOModuleInfo moduleInfo = new IO.IOModuleInfo();
                        moduleInfo = moduleInfo.GetIOModuleInfo(name, out isStub);

                        if (isStub && name != "")
                        {
                            ProjectManager.GetInstance().AddLogMessage("Неизвестный модуль \"" +
                                oF.VisibleName + "\" - \"" + name + "\".");
                        }

                        int inOffset = 0;
                        int outOffset = 0;

                        if (iOManager[node_n] != null)
                        {
                            switch (moduleInfo.AddressSpaceType)
                            {
                                case IO.IOModuleInfo.ADDRESS_SPACE_TYPE.DI:
                                    inOffset = iOManager[node_n].DI_count;
                                    break;

                                case IO.IOModuleInfo.ADDRESS_SPACE_TYPE.DO:
                                    outOffset = iOManager[node_n].DO_count;
                                    break;

                                case IO.IOModuleInfo.ADDRESS_SPACE_TYPE.AI:
                                    inOffset = iOManager[node_n].AI_count;
                                    break;

                                case IO.IOModuleInfo.ADDRESS_SPACE_TYPE.AO:
                                    outOffset = iOManager[node_n].AO_count;
                                    break;

                                case IO.IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI:
                                    inOffset = iOManager[node_n].AI_count;
                                    outOffset = iOManager[node_n].AO_count;
                                    break;

                                case IO.IOModuleInfo.ADDRESS_SPACE_TYPE.DODI:
                                    inOffset = iOManager[node_n].DI_count;
                                    outOffset = iOManager[node_n].DO_count;
                                    break;

                                case IO.IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI:
                                    inOffset = iOManager[node_n].AI_count;
                                    outOffset = iOManager[node_n].AO_count;
                                    break;
                            }

                            iOManager[node_n].DI_count += moduleInfo.DI_count;
                            iOManager[node_n].DO_count += moduleInfo.DO_count;
                            iOManager[node_n].AI_count += moduleInfo.AI_count;
                            iOManager[node_n].AO_count += moduleInfo.AO_count;

                            IO.IOModule new_module =
                                new IO.IOModule(inOffset, outOffset, moduleInfo, physicalNumber);

                            iOManager[node_n].SetModule(new_module, module_n);
                        }
                        else
                        {
                            ProjectManager.GetInstance().AddLogMessage("Для \"" +
                               oF.VisibleName + "\" - \"" + name +
                               "\", не найден узел номер " +
                               ++node_n + ".");
                        }
                    }
                }
            }

            string err_str = iOManager.Check();
            if (err_str != "")
            {
                ProjectManager.GetInstance().AddLogMessage(err_str);
            }
        }

        /// <summary>
        /// Функция проверки наличия узлового модуля
        /// типа А1, на котором находится управляющая
        /// программа (для Phoenix Contact)
        /// </summary>
        private bool CheckPhoenixContactMainNode(List<Function> tmpFList, Regex regex_name)
        {
            // Проверка, есть ли в устройствах узел типа A1.
            List<int> numbers = new List<int>();
            foreach (Function f in tmpFList)
            {
                Match match = regex_name.Match(f.VisibleName);

                if (match.Success)
                {
                    int n = Convert.ToInt32(match.Groups["n"].Value);
                    numbers.Add(n);
                }
            }

            if (numbers.Contains(1) == true)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Функция синхронизации названий устройств и модулей в привязке.
        /// </summary>
        /// <returns></returns>
        public string UpdateModulesBinding()
        {
            string errors = ModulesBindingUpdate.GetInstance().Execute();
            return errors;
        }

        public string SaveAsLuaTable(string prefix) 
        {
            return iOManager.SaveAsLuaTable(prefix);
        }

        private IO.IOManager iOManager;     // Экземпляр класса.
        private static EplanIOManager instance; // Экземпляр класса.  
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Все устройства проекта (клапана, насосы, ...).
    /// </summary>
    class EplanDeviceManager : Device.IDeviceManager
    {
        public static Dictionary<string, string> GetAssigment(
            Function oF2, IO.IOModuleInfo moduleInfo, string devsDescr = "")
        {
            Dictionary<string, string> res =
                new Dictionary<string, string>();

            int nn; // Номер клеммы
            string nnString = oF2.Properties.FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
            // Если клемма - не число, возвращаем res
            bool isDigit = Int32.TryParse(nnString, out nn);
            if (isDigit == false)
            {
                return res;
            }

            if (Array.IndexOf(moduleInfo.ChannelClamps, nn) < 0)
            {
                return res;
            }

            if (oF2.Page.PageType != DocumentTypeManager.DocumentType.Circuit)
            {
                return res;
            }

            if (devsDescr == "")
            {
                devsDescr = oF2.Properties.FUNC_TEXT_AUTOMATIC.ToString(
                       ISOCode.Language.L___);
                if (devsDescr == "")
                {
                    devsDescr = oF2.Properties.FUNC_TEXT_AUTOMATIC.ToString(
                        ISOCode.Language.L_ru_RU);
                }
                if (devsDescr == "")
                {
                    devsDescr = oF2.Properties.FUNC_TEXT.ToString(
                        ISOCode.Language.L_ru_RU);
                }
            }

            if (devsDescr == null || devsDescr == "")
            {
                return res;
            }

            string komment = string.Empty;
            string klemmeKomment = string.Empty;
            Match actionMatch;
            MatchEvaluator deviceEvaluator = new MatchEvaluator(GetInstance().RussianToEnglish);
            bool isMultipleBinding = Device.DeviceManager.GetInstance().IsMultipleBinding(devsDescr);
            if (isMultipleBinding == false)
            {
                int endPos = devsDescr.IndexOf("\n");
                if (endPos > 0)
                {
                    komment = devsDescr.Substring(endPos + 1);
                    devsDescr = devsDescr.Substring(0, endPos);
                }

                devsDescr = Regex.Replace(devsDescr, RussianLettersAsEnglish, deviceEvaluator);

                actionMatch = Regex.Match(komment, ChannelComment, RegexOptions.IgnoreCase);

                komment = Regex.Replace(komment, ChannelComment, "", RegexOptions.IgnoreCase);
                komment = komment.Replace("\n", ". ").Trim();
                if (komment.Length > 0 && komment[komment.Length - 1] != '.')
                {
                    komment += ".";
                }
            }
            else
            {
                devsDescr = Regex.Replace(devsDescr, RussianLettersAsEnglish, deviceEvaluator);
                actionMatch = Regex.Match(komment, ChannelComment, RegexOptions.IgnoreCase);
            }

            Match descrMatch = Regex.Match(
                devsDescr, Device.DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);

            while (descrMatch.Success)
            {
                string devName = descrMatch.Groups["name"].Value;

                if (actionMatch.Success)
                {
                    klemmeKomment = actionMatch.Value;
                }

                res.Add(devName, klemmeKomment);
                descrMatch = descrMatch.NextMatch();
            }

            return res;
        }

        /// <summary>
        /// Считывание информации о привязке устройств проекта, и
        /// установка автоматических параметров устройств.
        /// Информацию получаем на основе функционального текста к клеммам 
        /// модулей ввода\вывода IO. Этот комментарий должен быть оформлен 
        /// соответственно.
        /// Алгоритм:
        /// 1. С помощью фильтра выбираем функциональные объекты  в пределах 
        /// открытого проекта со следующими ограничениями: категория PLCBox,
        /// главная функция, производитель изделия WAGO/ Phoenix.
        /// 2. Обрабатываем функциональные объекты со следующим именем:
        /// -Аxxx, где А - признак элемента IO, xxx - номер.
        /// 3. Обрабатываем элементы с номером, не кратным 100 - это модули 
        /// IO. 
        /// 4. Для каждого модуля получаем имя функционального объекта - клеммы 
        /// - путем добавления ":x", x - номер от 1 до 20.
        /// 5. С помощью фильтра ищем такие функциональные объекты.
        /// 6. Разбираем функциональный текст объекта:
        /// 6.1 На основе первой строки получаем список устройств, относящихся 
        /// к данной клемме, а остальные строки - описание устройства.
        /// 6.2 На основе имени модуля получаем его тип (DO, DI, AO, AI) и 
        /// определяем тип канала. 
        /// 6.3 Для клапанов (символ V) анализируем комментарий на наличие 
        /// следующих предложений (без учета регистра): "открыть", "закрыть",
        /// "открыть НС", "открыть ВС", "открыт", "открыт", "открыть мини".
        /// На их основе добавляем описание к каналу.
        /// 6.4 Добавляем информацию о канале к устройствам.
        /// </summary>
        public void ReadConfigurationFromIOModules()
        {
            //Получение текущего проекта, для которого осуществляется экспорт.
            Project project = EProjectManager.GetInstance().GetCurrentPrj();
            if (project == null)
            {
                return;
            }

            //Initialize the DMObjectsFinder with a project.
            var objectsFinder = new DMObjectsFinder(project);
            var plcFunctionsFilter = new FunctionsFilter();

            var properties = new FunctionPropertyList();
            properties.FUNC_MAINFUNCTION = true;

            plcFunctionsFilter.SetFilteredPropertyList(properties);
            plcFunctionsFilter.Category = Function.Enums.Category.PLCBox;

            //Get function with given name from project.
            Function[] functions = objectsFinder.GetFunctions(plcFunctionsFilter); //1

            //Шаблоны для разбора названий модулей и узлов IO.
            const string IOPattern = @"=*-A(?<n>\d+)"; //-A101
            var regex = new Regex(IOPattern);

            var clampFuntionFilter = new FunctionsFilter();
            clampFuntionFilter.ExactNameMatching = true;
            clampFuntionFilter.Category = Function.Enums.Category.PLCTerminal;

            //Если нет модулей IO - не считываем привязку
            int nodesCount = IO.IOManager.GetInstance().IONodes.Count;
            if (nodesCount == 0)
            {
                ProjectManager.GetInstance().AddLogMessage("В проекте отсутствуют узлы ввода/вывода.");
                return;
            }

            // Есть ли узел с ОУ "A1" в проекте
            bool isContainsA1 = false;
            if (IO.IOManager.GetInstance().IONodes[0].Type ==
                IO.IONode.TYPES.T_PHOENIX_CONTACT_MAIN)
            {
                isContainsA1 = true;
            }

            foreach (Function function in functions)
            {
                var match = regex.Match(function.VisibleName);
                if (match.Success)
                {
                    //Признак пропуска данного устройства.
                    if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[1].IsEmpty)
                    {
                        continue;
                    }

                    int physicalNumber = Convert.ToInt32(match.Groups["n"].Value);

                    // Проверка модулей узлов
                    if (physicalNumber % 100 != 0 && physicalNumber != 1)   //4
                    {
                        int module = physicalNumber % 100;
                        int node;

                        // Если есть "A1", то учитываем, что он первый
                        if (isContainsA1 == true)
                        {
                            node = physicalNumber / 100;
                        }
                        else
                        {
                            // Если нету, оставляем как было
                            node = physicalNumber / 100 - 1;
                        }

                        if (function.Articles.GetLength(0) == 0)
                        {
                            ProjectManager.GetInstance().AddLogMessage("У модуля \"" +
                                function.VisibleName + "\" не задано изделие.");
                            continue;
                        }

                        var name = string.Empty;
                        if (!function.Articles[0].Properties[
                            Eplan.EplApi.DataModel.Properties.Article.ARTICLE_TYPENR].IsEmpty)
                        {
                            name = function.Articles[0].Properties[
                                Eplan.EplApi.DataModel.Properties.Article.ARTICLE_TYPENR].ToString().Trim();
                        }

                        var moduleInfo = new IO.IOModuleInfo();
                        moduleInfo = moduleInfo.GetIOModuleInfo(name, out _);

                        foreach (Function subFunction in function.SubFunctions)
                        {
                            int clamp; // Номер клеммы
                            string clampString = subFunction.Properties.FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
                            // Если клемма - число, пропускаем
                            bool isDigit = Int32.TryParse(clampString, out clamp);
                            if (isDigit == false)
                            {
                                continue;
                            }

                            if (Array.IndexOf(moduleInfo.ChannelClamps, clamp) < 0)
                            {
                                continue;
                            }

                            if (subFunction.Page.PageType != DocumentTypeManager.DocumentType.Circuit)
                            {
                                continue;
                            }

                            string description = subFunction.Properties.FUNC_TEXT_AUTOMATIC.ToString(
                                ISOCode.Language.L___);
                            if (description == "")
                            {
                                description = subFunction.Properties.FUNC_TEXT_AUTOMATIC.ToString(
                                    ISOCode.Language.L_ru_RU);
                            }
                            if (description == "")
                            {
                                description = subFunction.Properties.FUNC_TEXT.ToString(
                                    ISOCode.Language.L_ru_RU);
                            }

                            if (description == null || description == "")
                            {
                                continue;
                            }

                            MatchCollection descriptionMatches = Regex.Matches(
                                description, Device.DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
                            // Проверка пневмоостровов FESTO
                            const string FestoPattern = @"=*-Y(?<n>\d+)";
                            var festoRegex = new Regex(FestoPattern);
                            var festoMatch = festoRegex.Match(description);
                            // Нашло совпадение (Есть пневмоостров)
                            if (festoMatch.Success && descriptionMatches.Count == 1)
                            {
                                // Инициализация начальных данных
                                Function[] subFunctions = new Function[0];

                                int eplanNameLength;
                                // Вырезается ОУ устройства для привязки, без комментария
                                if (description.Contains("\r\n") == true)
                                {
                                    eplanNameLength = description.IndexOf("\r\n");
                                    if (eplanNameLength > 0)
                                    {
                                        description = description.Substring(0, eplanNameLength);
                                    }
                                }
                                else
                                {
                                    eplanNameLength = description.IndexOf("\n");
                                    if (eplanNameLength > 0)
                                    {
                                        description = description.Substring(0, eplanNameLength);
                                    }
                                }

                                // Поиск по функциям
                                foreach (Function valveTerminalFunction in functions)
                                {
                                    // Нашли нужную функцию устройства
                                    if (valveTerminalFunction.Name.Contains(description))
                                    {
                                        subFunctions = valveTerminalFunction.SubFunctions;
                                        break;
                                    }
                                }

                                // Перебираем подфункции
                                foreach (Function valteTerminalSubFunction in subFunctions)
                                {
                                    // Если это клемма
                                    if (valteTerminalSubFunction.Category == Function.Enums.Category.PLCTerminal)
                                    {
                                        // Проверяем номер клеммы и текст
                                        string terminalNumber = valteTerminalSubFunction.Properties.
                                            FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
                                        string terminalBindedDevice = valteTerminalSubFunction.Properties.
                                            FUNC_TEXT_AUTOMATIC.ToString(ISOCode.Language.L___);
                                        bool isInt = Int32.TryParse(terminalNumber, out _);
                                        // Если клемма - число, и есть текст для привязки
                                        if (isInt == true && terminalBindedDevice.Length != 0 && terminalBindedDevice != "Резерв")
                                        {
                                            var festoDescrMatch = Regex.Match(
                                                terminalBindedDevice, Device.DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
                                            while (festoDescrMatch.Success)
                                            {
                                                string deviceName = festoDescrMatch.Groups["name"].Value;

                                                // Дополняется descr и отправляется, будто привязано к I/O
                                                description += $"\r\n{deviceName}";

                                                // Дополнительно установить параметр R_VTUG_NUMBER
                                                int vtugNumber = Convert.ToInt32(terminalNumber);
                                                Dictionary<string, double> runtimeParams = new Dictionary<string, double>();
                                                runtimeParams.Add("R_VTUG_NUMBER", vtugNumber);
                                                SetUpDeviceRuntimeParameters(runtimeParams, deviceName);

                                                festoDescrMatch = festoDescrMatch.NextMatch();
                                            }
                                        }
                                    }
                                }
                            }

                            var clampComment = string.Empty;
                            Match actionMatch;
                            var comment = string.Empty;
                            var deviceEvaluator = new MatchEvaluator(RussianToEnglish);
                            // Собственная обработка для AS-i
                            bool isMultipleBinding = Device.DeviceManager.GetInstance().IsMultipleBinding(description);
                            if (isMultipleBinding == false)
                            {
                                //Для многострочного описания убираем tab+\r\n
                                description = description.Replace("\t\r\n", "");
                                description = description.Replace("\t\n", "");

                                int endPosition = description.IndexOf("\n");
                                if (endPosition > 0)
                                {
                                    comment = description.Substring(endPosition + 1);
                                    description = description.Substring(0, endPosition);
                                }

                                description = Regex.Replace(description, RussianLettersAsEnglish, deviceEvaluator);

                                actionMatch = Regex.Match(comment, ChannelComment,
                                    RegexOptions.IgnoreCase);

                                comment = Regex.Replace(comment, ChannelComment,
                                    "", RegexOptions.IgnoreCase);
                                comment = comment.Replace("\n", ". ").Trim();
                                if (comment.Length > 0 && comment[comment.Length - 1] != '.')
                                {
                                    comment += ".";
                                }
                            }
                            else
                            {
                                description = Regex.Replace(description, RussianLettersAsEnglish, deviceEvaluator);
                                actionMatch = Regex.Match(comment, ChannelComment,
                                    RegexOptions.IgnoreCase);
                            }

                            descriptionMatches = Regex.Matches(
                                description, Device.DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
                            int devicesCount = descriptionMatches.Count;
                            if (devicesCount < 1 && !description.Equals("Pезерв"))
                            {
                                ProjectManager.GetInstance().AddLogMessage(
                                    string.Format("\"{0}:{1}\" - неверное имя привязанного устройства - \"{2}\".",
                                    function.VisibleName, clamp, description));
                            }

                            foreach (Match descriptionMatch in descriptionMatches)
                            {
                                string deviceName = descriptionMatch.Groups["name"].Value;
                                Device.IODevice device = Device.DeviceManager.GetInstance().GetDevice(deviceName);

                                if (actionMatch.Success)
                                {
                                    clampComment = actionMatch.Value;
                                    if (clampComment.Contains("\r\n"))
                                    {
                                        clampComment = clampComment.Replace("\r\n", "");
                                    }
                                }

                                var error = string.Empty;
                                string channelName = "IO-Link";
                                int logicalPort = Array.IndexOf(moduleInfo.ChannelClamps, clamp) + 1;
                                int moduleOffset = IO.IOManager.GetInstance().IONodes[node].IOModules[module - 1].InOffset;
                                
                                if (devicesCount == 1 &&
                                    moduleInfo.AddressSpaceType == IO.IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI)
                                {
                                    if (device.Channels.Count == 1)
                                    {
                                        List<Device.IODevice.IOChannel> chanels = device.Channels;
                                        channelName = GetChannelNameForIOLinkModuleFromString(chanels.First().Name);
                                    }
                                    else
                                    {
                                        channelName = GetChannelNameForIOLinkModuleFromString(comment);
                                    }
                                }

                                Device.DeviceManager.GetInstance()
                                    .AddDeviceChannel(device, 
                                    moduleInfo.AddressSpaceType, node,
                                    module, clamp, clampComment, out error, 
                                    physicalNumber, logicalPort, moduleOffset, 
                                    channelName);

                                if (error != "")
                                {
                                    error = string.Format("\"{0}:{1}\" : {2}",
                                       function.VisibleName, clamp, error);

                                    ProjectManager.GetInstance().AddLogMessage(error);
                                }
                            }                        
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает имя канала (IO-Link, DI, DO) из строки для IO-Link
        /// модуля.
        /// </summary>
        /// <param name="source">Строка для поиска</param>
        /// <returns></returns>
        public string GetChannelNameForIOLinkModuleFromString(string source)
        {
            const string IOLink = "IO-Link";
            const string DI = "DI";
            const string DO = "DO";

            if (source.Contains(DI) &&
                !source.Contains(IOLink) &&
                !source.Contains(DO))
            {
                return DI;
            }

            if (source.Contains(DO) &&
                !source.Contains(IOLink) &&
                !source.Contains(DI))
            {
                return DO;
            }

            return IOLink;
        }

        /// <summary>
        /// Установка рабочих параметров устройства
        /// </summary>
        /// <param name="parameters">Словарь параметров</param>
        /// <param name="deviceName">Имя устройства</param>
        private void SetUpDeviceRuntimeParameters(Dictionary<string, double> parameters, string deviceName)
        {
            Device.IODevice dev = Device.DeviceManager.GetInstance().GetDevice(deviceName);
            foreach (KeyValuePair<string, double> parameter in parameters)
            {
                dev.SetRuntimeParameter(parameter.Key, parameter.Value);
            }
        }

        ///<summary>Проверка конфигурации.
        ///</summary>
        public void CheckConfiguration(bool useLog = false)
        {
            string res = deviceManager.CheckDevicesConnectionAndCalculateIOLink();
            if (res != "" && useLog == false) ProjectManager.GetInstance().AddLogMessage(res);
        }

        public void ClearDevices()
        {
            deviceManager.Devices.Clear();
        }

        /// <summary>
        ///Синхронизация проекта Eplan'а и описания Lua-скрипта.
        ///
        ///1.Создаем массив целочисленных флагов, количество элементов в котором
        ///равняется количеству элементов в массиве ранее считанных устройств.
        ///Все элементы массива флагов устанавливаются в 0.
        ///Далее будем считать, что если флаг = 0, то индекс объекта не изменился,
        ///если флаг = -1, то индекс объекта помечен на удаление, если флаг > 0,
        ///то изменяем старый индекс в операции на значение флага.
        ///2. Для каждого элемента массива предыдущих устройств проверяем 
        ///соответствие элементу нового списка.
        ///2.1. Пробуем проверить равенство объектов в двух списках устройств.
        ///Если элемент нового списка входит в старый, то проверяем равенство их
        ///имен.
        ///2.2.Если имена неравны, проверяем равенство их индексов в списках. 
        ///Индексы не совпадают - в массиве флагов изменяем соответствующий 
        ///элемент на новый индекс.
        ///2.3. Проверяем индексы одинаковых объектов, если они неравны, то 
        ///аналогично изменяем флаг на новый индекс.
        ///2.4. Если объект, находящийся в старом списке был уже удален, то 
        ///обрабатываем исключение. Устанавливаем флаг элемента старого списка 
        ///в -1.
        ///3. Вызываем функцию синхронизации индексов.
        /// </summary>
        /// <param name="devicesArray">Массив устройств до обновления списка.</param>
        public void SynchAndReadConfigurationFromScheme()
        {
            if (deviceManager.Devices.Count == 0)
            {
                ReadConfigurationFromScheme();
                return;
            }

            Device.IODevice[] prevDevices =
                new Device.IODevice[deviceManager.Devices.Count];
            deviceManager.Devices.CopyTo(prevDevices);

            ReadConfigurationFromScheme();

            int[] indexArray = new int[prevDevices.Length];             //1            
            for (int i = 0; i < prevDevices.Length; i++)
            {
                indexArray[i] = 0;
            }

            bool needSynch = false;
            for (int k = 0; k < prevDevices.Length; k++)                //2
            {
                Device.IODevice prevDev = prevDevices[k];

                try
                {
                    if (prevDev.EplanObjectFunction != null)
                    {
                        if (k < deviceManager.Devices.Count &&
                            prevDev.Name == deviceManager.Devices[k].Name)
                        {
                            continue;
                        }

                        needSynch = true;
                        int idx = -1;
                        foreach (Device.IODevice newDev in deviceManager.Devices)
                        {
                            idx++;
                            if (newDev.EplanObjectFunction ==
                                    prevDev.EplanObjectFunction)    //2.1
                            {
                                indexArray[k] = idx;
                                break;
                            }
                        }
                    }
                }
                catch                                                 //2.4
                {
                    indexArray[k] = -1;
                }
            }

            //3
            if (needSynch)
            {
                TechObject.TechObjectManager.GetInstance().Synch(indexArray);
            }
        }

        /// <summary>
        /// Считывание информации об устройствах проекта на основе 
        /// схемы автоматизации.
        /// Информацию получаем на основе названий устройств. Эти названия 
        /// должны быть оформлены соответственно.
        /// Алгоритм:
        /// 1. С помощью фильтра выбираем функциональные объекты  в пределах 
        /// открытого проекта со следующими ограничениями: главная функция.
        /// 2. Выбираем устройства на соответствие шаблону названия (например -
        /// N1V12).
        /// 3. Выбираем видимые устройства с типом страницы:
        /// DocumentTypeManager.DocumentType.ProcessAndInstrumentationDiagram.
        /// 4. Выбираем устройства с пустым свойством 
        /// FUNC_SUPPLEMENTARYFIELD[ 1 ] (признак игнорирования устройства
        /// при экспорте для PAC).
        /// 5. Добавляем устройства в список менеджера устройств.
        /// </summary>
        public void ReadConfigurationFromScheme()
        {
            deviceManager.Clear();
            //Получение текущего проекта, для которого осуществляется экспорт.
            Project project = EProjectManager.GetInstance().GetCurrentPrj();

            if (project == null)
            {
                return;
            }

            //Initialize the DMObjectsFinder with a project.
            DMObjectsFinder objectFinder = new DMObjectsFinder(project);
            FunctionsFilter functionsFilter = new FunctionsFilter();

            FunctionPropertyList propertyList = new FunctionPropertyList();
            propertyList.FUNC_MAINFUNCTION = true;
            functionsFilter.IsPlaced = true;

            functionsFilter.SetFilteredPropertyList(propertyList);

            //Get function with given name from project.        //1
            Function[] functions = objectFinder.GetFunctions(functionsFilter); 

            MatchEvaluator deviceEvaluator = new MatchEvaluator(
                RussianToEnglish);
            string descriptionPattern = "([\'\"])";

            foreach (Function function in functions)
            {
                if (function.VisibleName == "" ||
                    function.Page == null ||
                    function.Page.PageType != DocumentTypeManager.
                    DocumentType.ProcessAndInstrumentationDiagram)
                {
                    continue;
                }

                //Признак пропуска данного устройства (например, для 
                //ручной заслонки).
                if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[1].IsEmpty)
                {
                    continue;
                }

                string name = function.Name;
                name = Regex.Replace(name, RussianLettersAsEnglish, 
                    deviceEvaluator);

                string description = "";
                if (!function.Properties.FUNC_COMMENT.IsEmpty)
                {
                    description = function.Properties.FUNC_COMMENT.ToString(
                                ISOCode.Language.L___);

                    if (description == "")
                    {
                        description = function.Properties.FUNC_COMMENT.
                            ToString(ISOCode.Language.L_ru_RU);
                    }

                    description = Regex.Replace(description, 
                        descriptionPattern, "");
                }

                //Подтип устройства.
                string subType = "";
                if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[2].IsEmpty)
                {
                    subType = function.Properties.FUNC_SUPPLEMENTARYFIELD[2].
                        ToString(ISOCode.Language.L___);

                    if (subType == "")
                    {
                        subType = function.Properties.
                            FUNC_SUPPLEMENTARYFIELD[2].ToString(
                            ISOCode.Language.L_ru_RU);
                    };

                    subType = subType.Trim();
                    subType = Regex.Replace(subType, RussianLettersAsEnglish, 
                        deviceEvaluator);
                }

                //Параметры устройства.
                string parameters = "";
                if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[3].IsEmpty)
                {
                    parameters = function.Properties.
                        FUNC_SUPPLEMENTARYFIELD[3].
                        ToString(ISOCode.Language.L___);

                    if (parameters == "")
                    {
                        parameters = function.Properties.
                            FUNC_SUPPLEMENTARYFIELD[3].
                            ToString(ISOCode.Language.L_ru_RU);
                    };

                    parameters = Regex.Replace(parameters, 
                        RussianLettersAsEnglish, deviceEvaluator);
                }

                //Свойства устройства.
                string properties = "";
                if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[4].IsEmpty)
                {
                    properties = function.Properties.
                        FUNC_SUPPLEMENTARYFIELD[4].
                        ToString(ISOCode.Language.L___);

                    if (properties == "")
                    {
                        properties = function.Properties.
                            FUNC_SUPPLEMENTARYFIELD[4].
                            ToString(ISOCode.Language.L_ru_RU);
                    };

                    properties = Regex.Replace(properties, 
                        RussianLettersAsEnglish, deviceEvaluator);
                }

                //Рабочие параметры устройства.
                string runtimeParameters = "";
                if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[5].IsEmpty)
                {
                    runtimeParameters = function.Properties.
                        FUNC_SUPPLEMENTARYFIELD[5].
                        ToString(ISOCode.Language.L___);

                    if (runtimeParameters == "")
                    {
                        runtimeParameters = function.Properties.
                            FUNC_SUPPLEMENTARYFIELD[5].
                            ToString(ISOCode.Language.L_ru_RU);
                    };

                    runtimeParameters = Regex.Replace(runtimeParameters, 
                        RussianLettersAsEnglish, deviceEvaluator);
                }

                //Номер шкафа для расположения устройства.
                int deviceLocation = 1;
                if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[6].IsEmpty)
                {
                    deviceLocation = Int32.Parse(function.Properties.
                        FUNC_SUPPLEMENTARYFIELD[6].
                        ToString(ISOCode.Language.L___));
                }

                // Изделие устройства
                string articleName = "";
                ArticleReference[] articlesRefs = function.ArticleReferences;
                if (articlesRefs.Length > 0 &&
                    function.ArticleReferences[0].PartNr != string.Empty &&
                    function.ArticleReferences[0].PartNr != null)
                {
                    articleName = function.ArticleReferences[0].PartNr;
                }

                string error;
                deviceManager.AddDeviceAndEFunction(name, description,
                    subType, parameters, runtimeParameters, properties, 
                    deviceLocation, function, out error, articleName);

                if (error != "")
                {
                    ProjectManager.GetInstance().AddLogMessage(error);
                }
            }

            deviceManager.Sort();
        }

        /// <summary>
        /// MatchEvaluator для regular expression,
        /// замена русских букв на английские
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private string RussianToEnglish(Match m)
        {
            switch (m.ToString()[0])
            {
                case 'А':
                    return "A";
                case 'В':
                    return "B";
                case 'С':
                    return "C";
                case 'Е':
                    return "E";
                case 'К':
                    return "K";
                case 'М':
                    return "M";
                case 'Н':
                    return "H";
                case 'Х':
                    return "X";
                case 'Р':
                    return "P";
                case 'О':
                    return "O";
                case 'Т':
                    return "T";
            }

            return m.ToString();
        }

        public string SaveAsLuaTable(string prefix)
        {
            return deviceManager.SaveAsLuaTable(prefix);
        }

        public string SaveDevicesAsLuaScript()
        {
            return deviceManager.SaveDevicesAsLuaScript();
        }

        public void GetObjectForXML(TreeNode node)
        {
            deviceManager.GetObjectForXML(node);
        }

        public static EplanDeviceManager GetInstance()
        {
            if (instance == null)
            {
                instance = new EplanDeviceManager();
            }

            return instance;
        }

        private EplanDeviceManager()
        {
            deviceManager = Device.DeviceManager.GetInstance();
        }

        // Шаблон для замены русских букв
        const string RussianLettersAsEnglish = @"[АВСЕКМНХРОТ]";
        
        // Комментарий канала
        const string ChannelComment = 
            @"(Открыть мини(?n:\s+|$))|" +
            @"(Открыть НС(?n:\s+|$))|" + 
            @"(Открыть ВС(?n:\s+|$))|" + 
            @"(Открыть(?n:\s+|$))|" + 
            @"(Закрыть(?n:\s+|$))|" +
            @"(Открыт(?n:\s+|$))|" +
            @"(Закрыт(?n:\s+|$))|" +
            @"(Объем(?n:\s+|$))|" + 
            @"(Поток(?n:\s+|$))|" +
            @"(Пуск(?n:\s+|$))|" + 
            @"(Реверс(?n:\s+|$))|" + 
            @"(Обратная связь(?n:\s+|$))|" + 
            @"(Частота вращения(?n:\s+|$))|" +
            @"(Авария(?n:\s+|$))|" +
            @"(Напряжение моста\(\+Ud\)(?n:\s+|$))|" +
            @"(Референсное напряжение\(\+Uref\)(?n:\s+|$))";

        private static EplanDeviceManager instance; // Экземпляр класса.
        private Device.DeviceManager deviceManager;     
    }

    /// <summary>
    /// Класс, реализующий синхронизацию модулей и названий устройств.
    /// </summary>
    class ModulesBindingUpdate
    {
        /// <summary>
        /// Экземпляр класса синхронизации названий и модулей
        /// </summary>
        /// <returns></returns>
        public static ModulesBindingUpdate GetInstance()
        {
            return modulesBindingUpdate;
        }

        /// <summary>
        /// Синхронизация названий устройств и модулей
        /// </summary>
        /// <returns>Возвращает сообщения об ошибках во время выполнения.
        /// </returns>
        public string Execute()
        {
            EplanIOManager.GetInstance().ReadConfiguration();
            EplanDeviceManager.GetInstance().
                SynchAndReadConfigurationFromScheme();
            EplanDeviceManager.GetInstance().ReadConfigurationFromIOModules();
            EplanDeviceManager.GetInstance().CheckConfiguration();

            var selection = new SelectionSet();
            Project project = selection.GetCurrentProject(true);
            var objectFinder = new DMObjectsFinder(project);

            var functionsFilter = new FunctionsFilter();
            var properties = new FunctionPropertyList();
            properties.FUNC_MAINFUNCTION = true;
            functionsFilter.SetFilteredPropertyList(properties);
            functionsFilter.Category = Function.Enums.Category.PLCBox;
            Function[] devicesFunctions = objectFinder.
                GetFunctions(functionsFilter);

            project.LockAllObjects();

            Dictionary<string, string> deviceConnections =
                CollectIOModulesData(devicesFunctions);

            bool containsNewValveTerminal = GetProjectVersionFromDevices(
                deviceConnections);

            synchronizedDevices.Clear();

            if (!containsNewValveTerminal)
            {
                SynchronizeAsOldProject(deviceConnections, devicesFunctions);
            }
            else
            {
                SynchronizeAsNewProject(deviceConnections, devicesFunctions);
            }

            ClearNotExistingBinding();

            return errorMessage;
        }

        /// <summary>
        /// Сбор данных привязок для обновления
        /// </summary>
        /// <param name="devicesFunctions">Главные функции устройств 
        /// для поиска обновленных данных</param>
        /// <returns></returns>
        private Dictionary<string, string> CollectIOModulesData
            (Function[] devicesFunctions)
        {
            var deviceConnections = new Dictionary<string, string>();

            foreach (Device.IODevice device in Device.DeviceManager.
                GetInstance().Devices)
            {
                foreach (Device.IODevice.IOChannel channel in device.Channels)
                {
                    if (!channel.IsEmpty())
                    {
                        CollectModuleData(device, channel, devicesFunctions,
                            ref deviceConnections);
                    }
                }
            }

            return deviceConnections;
        }

        /// <summary>
        /// Добавление данных привязки конкретного модуля
        /// </summary>
        /// <param name="channel">Канал устройства</param>
        /// <param name="device">Устройство</param>
        /// <param name="deviceConnections">Словарь с собранными данными 
        /// привязок устройств</param>
        /// <param name="deviceFunctions">Главные функции устройств для 
        /// поиска обновленных данных</param>
        private void CollectModuleData(Device.IODevice device,
            Device.IODevice.IOChannel channel, Function[] deviceFunctions,
            ref Dictionary<string, string> deviceConnections)
        {
            const string IOModulePrefix = "A";
            const string ASInterfaceModule = "655";

            string deviceVisibleName = IOModulePrefix + channel.FullModule;
            var deviceFunction = deviceFunctions.
                FirstOrDefault(x => x.VisibleName.Contains(deviceVisibleName));
            if (deviceFunction == null)
            {
                return;
            }
            
            deviceVisibleName += ChannelPostfix + channel.PhysicalClamp.
                ToString();
            string functionalText = device.EPlanName;
            string devicePartNumber = deviceFunction.ArticleReferences[0]
                .PartNr;
            // Для модулей ASi не нужно добавлять комментарии 
            // к имени устройств.
            if (!devicePartNumber.Contains(ASInterfaceModule))
            {
                if (device.Description.Contains(PlusSymbol))
                {
                    // Так как в комментариях может использоваться знак 
                    // "плюс", то заменим его на какую-то константу, 
                    // которую при обновлении функционального текста 
                    // преобразуем обратно.
                    string replacedDeviceDescription = device.Description.
                        Replace(PlusSymbol.ToString(),
                        SymbolForPlusReplacing);
                    functionalText += NewLine + replacedDeviceDescription;
                    
                    if (!string.IsNullOrEmpty(channel.Comment))
                    {
                        functionalText += NewLine + channel.Comment;
                    }
                }
                else
                {
                    functionalText += NewLine + device.Description;

                    if(!string.IsNullOrEmpty(channel.Comment))
                    {
                        functionalText += NewLine + channel.Comment;
                    }
                }

                if (IsPhoenixContactIOLinkModule(devicePartNumber) &&
                    device.Channels.Count > 1)
                {
                    functionalText += NewLine + channel
                        .GetChannelTypeForIOLink();
                }
            }
            else
            {
                functionalText += WhiteSpace;
            }

            if (deviceConnections.ContainsKey(deviceVisibleName))
            {
                deviceConnections[deviceVisibleName] += functionalText;
            }
            else
            {
                deviceConnections.Add(deviceVisibleName, functionalText);
            }
        }

        /// <summary>
        /// Проверка, является ли данный модуль IO-Link модулем 
        /// от Phoenix Contact
        /// </summary>
        /// <param name="partNumber">Номер детали из API</param>
        /// <returns></returns>
        private bool IsPhoenixContactIOLinkModule(string partNumber)
        {
            var isIOLinkModule = false;

            int PhoenixContactStandard = (int)IO.IOManager.IOLinkModules
                .PhoenixContactStandard;
            int PhoenixContactSmart = (int)IO.IOManager.IOLinkModules
                .PhoenixContactSmart;

            if (partNumber.Contains(PhoenixContactStandard.ToString()) ||
                partNumber.Contains(PhoenixContactSmart.ToString()))
            {
                isIOLinkModule = true;
            }

            return isIOLinkModule;
        }

        /// <summary>
        /// Проверка версии проекта через устройства.
        /// У старых проектов пневмоостров Festo - DEV_VTUG, а у новых - Y.
        /// Остальное не влияет, все одинаково.
        /// </summary>
        /// <param name="deviceConnections">Устройства для проверки</param>
        /// <returns></returns>
        private bool GetProjectVersionFromDevices(Dictionary<string, string>
            deviceConnections)
        {
            var isProjectWithValveTerminal = new bool();

            if (deviceConnections.Values.
                Where(x => x.Contains(ValveTerminal)).Count() > 0)
            {
                isProjectWithValveTerminal = true;
                return isProjectWithValveTerminal;
            }
            else
            {
                isProjectWithValveTerminal = false;
                return isProjectWithValveTerminal;
            }
        }

        /// <summary>
        /// Синхронизация старого проекта
        /// </summary>
        /// <param name="deviceConnections">Устройства для синхронизации.
        /// Первый ключ - устройство ввода-вывода, 
        /// второй - привязанные устройства</param>
        /// <param name="functions">Функции обновляемых объектов</param>
        /// <returns></returns>
        private void SynchronizeAsOldProject(Dictionary<string, string>
            deviceConnections, Function[] functions)
        {
            foreach (string key in deviceConnections.Keys)
            {
                string deviceVisibleName = key.Substring(0, key.
                    IndexOf(ChannelPostfix));
                var synchronizingDevice = functions.FirstOrDefault(x => x.
                VisibleName.Contains(deviceVisibleName));

                if (synchronizingDevice == null)
                {
                    continue;
                }

                string clampNumberAsString = key.Remove(0, key.
                    IndexOf(ChannelPostfix) + ChannelPostfixSize);
                string bindedDevices = deviceConnections[key];
                var errors = string.Empty;
                bool? isASInterface = Device.DeviceManager.GetInstance().
                    IsASInterfaceDevices(bindedDevices, out errors);
                errorMessage += errors;
                bool deletingComments = NeedDeletingComments(bindedDevices);

                if (deletingComments == true)
                {
                    bindedDevices = SortDevices(bindedDevices);
                    bindedDevices = DeleteDevicesComments(bindedDevices);
                    bindedDevices = DeleteRepeatedDevices(bindedDevices);
                }

                if (isASInterface == true)
                {
                    bool isValidASNumbers = Device.DeviceManager.GetInstance().
                        CheckASNumbers(bindedDevices, out errors);
                    errorMessage += errors;
                    if (isValidASNumbers == true)
                    {
                        bindedDevices = DeleteRepeatedDevices(bindedDevices);
                        bindedDevices = SortASInterfaceDevices(bindedDevices);
                    }
                    else
                    {
                        // Если некорректные параметры - пропуск модуля.
                        continue;
                    }
                }
                else if (isASInterface == null)
                {
                    // Если AS-интерфейс привязан некорректно - пропуск модуля.
                    continue;
                }

                var clamp = Convert.ToInt32(clampNumberAsString);
                SynchronizeIOModule(synchronizingDevice, clamp, bindedDevices,
                    isASInterface);
            }
        }

        /// <summary>
        /// Синхронизация привязки к устройству
        /// </summary>
        /// <param name="functionalText">Обновленный функциональный текст
        /// </param>
        /// <param name="clamp">Номер клеммы</param>
        /// <param name="synchronizingDevice">Обновляемое устройство</param>
        private void SynchronizeIOModule(Function synchronizingDevice,
            int clamp, string functionalText, bool? isASInterface = false)
        {
            const int SequenceNumbersField = 20;
            // Конвертируем символ "плюс" обратно.
            functionalText = functionalText.
                Replace(SymbolForPlusReplacing, PlusSymbol.ToString());

            var placedFunctions = synchronizingDevice.SubFunctions.
                Where(x => x.IsPlaced == true).ToArray();

            var clampFunction = placedFunctions.
                        Where(x => x.Page.PageType ==
                        DocumentTypeManager.DocumentType.Circuit).
                        FirstOrDefault(x => x.Properties.
                        FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToInt() == clamp);

            // Если null - клеммы нет на схеме.
            if (clampFunction != null)
            {
                clampFunction.Properties.FUNC_TEXT = functionalText;
                AddSynchronizedDevice(synchronizingDevice, clamp);
                // Если AS-интерфейс, в доп поле записать нумерацию
                if (isASInterface == true)
                {
                    clampFunction.Properties.
                        FUNC_SUPPLEMENTARYFIELD[SequenceNumbersField] =
                        ASINumbering;
                }
            }
        }

        /// <summary>
        /// Синхронизация нового проекта
        /// </summary>
        /// <param name="deviceConnections">Объекты для обновления.
        /// Первый ключ - устройство ввода-вывода, 
        /// второй - привязанные устройства</param>
        /// <param name="functions">Функции обновляемых объектов</param>
        private void SynchronizeAsNewProject(Dictionary<string, string>
            deviceConnections, Function[] functions)
        {
            foreach (string key in deviceConnections.Keys)
            {
                string deviceVisibleName = key.Substring(
                    0, key.IndexOf(ChannelPostfix));
                var synchronizingDevice = functions.
                    FirstOrDefault(x => x.VisibleName.
                    Contains(deviceVisibleName));

                if (synchronizingDevice == null)
                {
                    continue;
                }

                string clampNumberAsString = key.Remove(0,
                    key.IndexOf(ChannelPostfix) + ChannelPostfixSize);

                // Если нет пневмоострова Y - то синхронизация ничем 
                // не отличается от старого проекта.
                if (!deviceConnections[key].Contains(ValveTerminal))
                {
                    var connections = new Dictionary<string, string>();
                    connections[key] = deviceConnections[key];
                    SynchronizeAsOldProject(connections, functions);
                }
                else
                {
                    var bindedDevices = deviceConnections[key].
                        Split(PlusSymbol).
                        FirstOrDefault(x => x.Contains(ValveTerminal)).
                        Insert(0, PlusSymbol.ToString());
                    var clamp = Convert.ToInt32(clampNumberAsString);
                    // Синхронизация пневмоострова.
                    SynchronizeIOModule(synchronizingDevice, clamp,
                        bindedDevices);

                    // Синхронизация устройств привязанных к пневмоострову.
                    Dictionary<int, string> bindedIOModuleDevices =
                        GetValveTerminalDevices(deviceConnections[key]);
                    deviceVisibleName = GetValveTerminalFunctionName(
                        deviceConnections[key]);
                    var synchronizingIOModuleDevice = functions.
                        FirstOrDefault(x => x.Name.Contains(
                            deviceVisibleName));
                    SynchronizeIOModuleDevices(synchronizingIOModuleDevice,
                        bindedIOModuleDevices);
                }
            }
        }

        /// <summary>
        /// Получение имени функции пневмоострова в котором обновляется 
        /// привязка
        /// </summary>
        /// <param name="bindedDevices">Привязанные устройства</param>
        /// <returns></returns>
        private string GetValveTerminalFunctionName(string bindedDevices)
        {
            const string FunctionNamePattern = "(\\+[A-Z0-9_]+-[Y0-9]+)";
            var functionNameMatch = Regex.Match(bindedDevices,
                FunctionNamePattern);

            if (functionNameMatch.Success)
            {
                return functionNameMatch.Value;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Получение устройств, привязанных к пневмоострову.
        /// </summary>
        /// <param name="functionalText">Функциональный текст 
        /// пневмоострова, к которому привязано устройство</param>
        /// <returns></returns>
        private Dictionary<int, string> GetValveTerminalDevices
            (string functionalText)
        {
            const string ClampNumberParameter = "R_VTUG_NUMBER";

            string[] devicesArray = functionalText.Split(PlusSymbol);
            var valveTerminalDevices = new Dictionary<int, string>();

            foreach (string deviceString in devicesArray)
            {
                if (string.IsNullOrEmpty(deviceString) ||
                    string.IsNullOrWhiteSpace(deviceString) ||
                    deviceString.Contains(ValveTerminal))
                {
                    continue;
                }

                string deviceName = Regex.Match(deviceString.
                    Insert(0, PlusSymbol.ToString()), DeviceNamePattern).Value;
                Device.IODevice device = Device.DeviceManager.
                    GetInstance().GetDevice(deviceName);
                string clampNumber = device.GetRuntimeParameter(
                    ClampNumberParameter);
                var clamp = Convert.ToInt32(clampNumber);

                if (valveTerminalDevices.ContainsKey(clamp))
                {
                    valveTerminalDevices[clamp] += deviceString.
                        Insert(0, PlusSymbol.ToString());
                }
                else
                {
                    valveTerminalDevices.Add(clamp, deviceString.
                        Insert(0, PlusSymbol.ToString()));
                }
            }

            return valveTerminalDevices;
        }

        /// <summary>
        /// Функция, проверяющая необходимость удаления комментариев в 
        /// функциональном тексте.
        /// </summary>
        /// <param name="devices">Строка со списком устройств</param>
        /// <returns></returns>
        private bool NeedDeletingComments(string devices)
        {
            var deviceMatches = Regex.Matches(devices, DeviceNamePattern);

            if (deviceMatches.Count > MinimalDevicesCountForCheck)
            {
                // Если первое устройство с подтипом AS-интерфейс, 
                // то все такие 
                for (int i = 0; i < MinimalDevicesCountForCheck; i++)
                {
                    Device.IODevice device = Device.DeviceManager.
                        GetInstance().GetDevice(deviceMatches[i].Value);
                    if (device.DeviceSubType == Device.DeviceSubType.
                        V_AS_DO1_DI2 ||
                        device.DeviceSubType == Device.DeviceSubType.
                        V_AS_MIXPROOF)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Функция убирает комментарии в функциональном тексте при привязке
        /// нескольких устройств к одной клемме.
        /// </summary>
        /// <param name="devices">Строка со списком устройств</param>
        /// <returns></returns>
        private string DeleteDevicesComments(string devices)
        {
            var devicesMatches = Regex.Matches(devices, DeviceNamePattern);

            if (devicesMatches.Count <= MinimalDevicesCountForCheck)
            {
                return devices;
            }

            var devicesWithoutComments = string.Empty;
            foreach (Match match in devicesMatches)
            {
                devicesWithoutComments += match.Value + WhiteSpace;
            }

            return devicesWithoutComments;
        }

        /// <summary>
        /// Удаляет повторяющиеся привязанные устройства.
        /// </summary>
        /// <param name="devicesWithoutComments">Устройства без комментариев
        /// </param>
        /// <returns></returns>
        private string DeleteRepeatedDevices(string devicesWithoutComments)
        {
            var devicesList = new List<string>();

            var devicesMatches = Regex.Matches(devicesWithoutComments,
                DeviceNamePattern);
            foreach (Match match in devicesMatches)
            {
                devicesList.Add(match.Value);
            }

            devicesList = devicesList.Distinct().ToList();

            var devicesWithoutRepeating = string.Empty;
            foreach (string device in devicesList)
            {
                devicesWithoutRepeating += device + WhiteSpace;
            }

            return devicesWithoutRepeating;
        }

        /// <summary>
        /// Сортировка устройств для корректного отображения.
        /// </summary>
        /// <param name="devices">Устройства для сортировки</param>
        /// <returns></returns>
        private string SortDevices(string devices)
        {
            var devicesMatches = Regex.Matches(devices, DeviceNamePattern);

            if (devicesMatches.Count <= MinimalDevicesCountForCheck)
            {
                return devices;
            }

            // valveTerminal - для вставки VTUG в старых проектах первым.
            Device.Device valveTerminal = null;
            var devicesList = new List<Device.Device>();
            foreach (Match match in devicesMatches)
            {
                Device.Device device = Device.DeviceManager.GetInstance().
                    GetDevice(match.Value);
                if (device.DeviceType != Device.DeviceType.DEV_VTUG)
                {
                    devicesList.Add(device);
                }
                else
                {
                    valveTerminal = device;
                }
            }

            devicesList.Sort(DevicesComparer);

            if (valveTerminal != null)
            {
                devicesList.Insert(0, valveTerminal);
            }

            var sortedDevices = string.Empty;
            foreach (Device.Device device in devicesList)
            {
                if (device.Description.Contains(PlusSymbol))
                {
                    // Заменяем символ плюс в комментарии, что бы не было
                    // конфликтов. Потом вернем обратно.
                    string replacedDeviceDescription = device.Description.
                        Replace(PlusSymbol.ToString(), SymbolForPlusReplacing);
                    sortedDevices += device.EPlanName + NewLine +
                        replacedDeviceDescription + NewLine;
                }
                else
                {
                    sortedDevices += device.EPlanName + NewLine +
                        device.Description + NewLine;
                }
            }

            return sortedDevices;
        }

        /// <summary>
        /// Функция для сортировки AS интерфейса по параметру R_AS_NUMBER
        /// </summary>
        /// <param name="devices">Строка с устройствами</param>
        /// <returns></returns>
        private string SortASInterfaceDevices(string devices)
        {
            var devicesMatches = Regex.Matches(devices, DeviceNamePattern);

            if (devicesMatches.Count <= MinimalDevicesCountForCheck)
            {
                return devices;
            }

            var devicesList = new List<Device.IODevice>();
            foreach (Match match in devicesMatches)
            {
                Device.IODevice device = Device.DeviceManager.GetInstance().
                    GetDevice(match.Value);
                devicesList.Add(device);
            }

            devicesList.Sort(ASInterfaceDevicesComparer);

            int lastASNumber = 0;
            string devicesWithoutASNumber = NewLine;
            var sortedDevices = string.Empty;

            foreach (Device.IODevice device in devicesList)
            {
                string numberAsString = device.
                    GetRuntimeParameter("R_AS_NUMBER");
                if (numberAsString == null)
                {
                    devicesWithoutASNumber += device.EPlanName
                        + NewLine;
                    continue;
                }

                int number;
                int.TryParse(numberAsString, out number);

                const int MinimalASNumber = 1;
                const int MaximalASNumber = 62;
                const int NormalDifference = 1; // Нормальные условия
                if (number < MinimalASNumber && number > MaximalASNumber)
                {
                    devicesWithoutASNumber += device.EPlanName
                        + WhiteSpace;
                    continue;
                }

                if (lastASNumber != number)
                {
                    int difference = number - lastASNumber;
                    if (difference > NormalDifference)
                    {
                        var NewLines = string.Empty;
                        for (int i = 0; i < difference; i++)
                        {
                            NewLines += NewLine;
                        }

                        sortedDevices += NewLines + device.EPlanName;
                    }
                    else
                    {
                        if (lastASNumber == 0 &&
                            number == MinimalASNumber)
                        {
                            sortedDevices += device.EPlanName;
                        }
                        else
                        {
                            sortedDevices += NewLine + device.EPlanName;

                        }
                    }
                }

                lastASNumber = number;
            }
            sortedDevices += devicesWithoutASNumber;

            return sortedDevices;
        }

        /// <summary>
        /// Компаратор для сравнения устройств с
        /// AS-интерфейсом (сортировки).
        /// </summary>
        /// <param name="device1">Устройство 1</param>
        /// <param name="device2">Устройство 2</param>
        /// <returns></returns>
        private int ASInterfaceDevicesComparer(Device.IODevice device1,
            Device.IODevice device2)
        {
            string device1ASNumberString = device1.
                GetRuntimeParameter("R_AS_NUMBER");
            string device2ASNumberString = device2.
                GetRuntimeParameter("R_AS_NUMBER");

            int device1ASNumber;
            int device2ASNumber;

            int.TryParse(device1ASNumberString, out device1ASNumber);
            int.TryParse(device2ASNumberString, out device2ASNumber);

            return device1ASNumber.CompareTo(device2ASNumber);
        }

        /// <summary>
        /// Функция очищающая привязку с модулей ввода/вывода и других
        /// устройств, если устройства нет на ФСА, а привязка есть
        /// </summary>
        private void ClearNotExistingBinding()
        {
            const string NotDeletableFunctionalText = "Резерв";

            foreach (Function device in synchronizedDevices.Keys)
            {
                string[] clamps = synchronizedDevices[device].
                    Split(WhiteSpace);

                var terminals = device.SubFunctions.
                    Where(x => x.IsPlaced == true &&
                    clamps.Contains(x.Properties.
                    FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToInt().
                    ToString()) == false &&
                    x.Category == Function.Enums.Category.PLCTerminal).
                    ToList();

                if (terminals != null && terminals.Count > 0)
                {
                    var sortedTerminals = new List<Function>();
                    foreach (Function terminal in terminals)
                    {
                        string functionalTextRuru = terminal.Properties.
                            FUNC_TEXT.ToString(ISOCode.Language.L_ru_RU);
                        if (functionalTextRuru == string.Empty)
                        {
                            functionalTextRuru = terminal.Properties.FUNC_TEXT.
                                ToString(ISOCode.Language.L___);
                        }

                        if (functionalTextRuru != NotDeletableFunctionalText)
                        {
                            sortedTerminals.Add(terminal);
                        }
                    }

                    foreach (Function terminal in sortedTerminals)
                    {
                        terminal.Properties.FUNC_TEXT = string.Empty;
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Добавляет в словарь синхронизированных устройств новое устройство
        /// </summary>
        /// <param name="synchronizedDevice">Функция устройства</param>
        /// <param name="clamp">Номер клеммы</param>
        private void AddSynchronizedDevice(Function synchronizedDevice,
            int clamp)
        {
            if (synchronizedDevices.ContainsKey(synchronizedDevice))
            {
                synchronizedDevices[synchronizedDevice] +=
                    $"{WhiteSpace}{clamp}";
            }
            else
            {
                synchronizedDevices[synchronizedDevice] = clamp.ToString();
            }
        }

        /// <summary>
        /// Синхронизация привязки устройств привязанных к модулю.
        /// Реализовано сразу два типа синхронизации
        /// </summary>
        /// <param name="synchronizingModule">Синхронизируемый модуль</param>
        /// <param name="IOModuleDevices">Устройства для синхронизации</param>
        private void SynchronizeIOModuleDevices(Function synchronizingModule,
            Dictionary<int, string> IOModuleDevices)
        {
            DocumentTypeManager.DocumentType circuitDocument =
                    DocumentTypeManager.DocumentType.Circuit;
            DocumentTypeManager.DocumentType overviewDocument =
                    DocumentTypeManager.DocumentType.Overview;

            foreach (int clamp in IOModuleDevices.Keys)
            {
                string functionalText =
                    SortDevices(IOModuleDevices[clamp]);
                // Конвертируем символ "плюс" обратно.
                functionalText = functionalText.
                    Replace(SymbolForPlusReplacing, PlusSymbol.ToString());

                string functionalTextWithoutComments =
                    DeleteDevicesComments(functionalText);

                // На электрических схемах при множественной привязке
                // комментарий не пишем.
                SynchronizeDevice(synchronizingModule, clamp,
                    functionalTextWithoutComments, circuitDocument);

                // На странице "Обзор" при множественной привязке надо писать
                // комментарии.
                SynchronizeDevice(synchronizingModule, clamp, functionalText,
                    overviewDocument);
            }
        }

        /// <summary>
        /// Синхронизация привязки устройства.
        /// </summary>
        /// <param name="synchronizingModule">Функция обновляемого модуля 
        /// ввода-вывода</param>
        /// <param name="clamp">Номер клеммы</param>
        /// <param name="functionalText">Функциональный текст</param>
        /// <param name="documentType">Тип страницы для поиска подфункции
        /// </param>
        private void SynchronizeDevice(Function synchronizingModule, int clamp,
            string functionalText,
            DocumentTypeManager.DocumentType documentType)
        {
            const string ClampFunctionDefenitionName = "Дискретный выход";

            var placedFunctions = synchronizingModule.SubFunctions.
                Where(x => x.IsPlaced == true).ToArray();

            var clampFunction = placedFunctions.
                Where(x => x.Page.PageType == documentType &&
                x.FunctionDefinition.Name.GetString(ISOCode.Language.L_ru_RU).
                Contains(ClampFunctionDefenitionName)).
                FirstOrDefault(y => y.Properties.
                FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToInt() == clamp);

            // Если null - клеммы нет на схеме.
            if (clampFunction != null)
            {
                clampFunction.Properties.FUNC_TEXT = functionalText;
                AddSynchronizedDevice(synchronizingModule, clamp);
            }
        }

        /// <summary>
        /// Компаратор для сравнения устройств (сортировки)
        /// </summary>
        /// <param name="x">Устройство 1</param>
        /// <param name="y">Устройство 2</param>
        /// <returns></returns>
        private int DevicesComparer(Device.Device x, Device.Device y)
        {
            int res;
            res = Device.Device.Compare(x, y);
            return res;
        }

        const string NewLine = "\r\n"; // Возврат каретки и перенос строки.
        const string DeviceNamePattern = "(\\+[A-Z0-9_]*-[A-Z0-9_]+)"; // ОУ.
        const string ValveTerminal = "-Y"; // Обозначение пневмоострова.
        const string ChannelPostfix = "_CH"; // Постфикс канала.
        const int ChannelPostfixSize = 3; // Размер постфикса канала.
        const char PlusSymbol = '+'; // Для обратной вставки после Split.
        const char WhiteSpace = ' '; // Пробел для разделения ОУ.
        const string SymbolForPlusReplacing = "plus"; // Замена символа "плюс".
        const int MinimalDevicesCountForCheck = 1; // Минимальное количество
        // устройств в строке, нужное для выполнения функции проверки.

        const string ASINumbering = "1\r\n2\r\n" + // Нумерация AS-i клапанов
            "3\r\n4\r\n5\r\n6\r\n7\r\n8\r\n9\r\n10\r\n11\r\n12\r\n13\r\n" +
            "14\r\n15\r\n16\r\n17\r\n18\r\n19\r\n20\r\n21\r\n22\r\n23\r\n" +
            "24\r\n25\r\n26\r\n27\r\n28\r\n29\r\n30\r\n31\r\n32\r\n33\r\n" +
            "34\r\n35\r\n36\r\n37\r\n38\r\n39\r\n40\r\n41\r\n42\r\n43\r\n" +
            "44\r\n45\r\n46\r\n47\r\n48\r\n49\r\n50\r\n51\r\n52\r\n53\r\n" +
            "54\r\n55\r\n56\r\n57\r\n58\r\n59\r\n60\r\n61\r\n62\r\n";

        static ModulesBindingUpdate modulesBindingUpdate =
            new ModulesBindingUpdate(); // Static экземпляр класса для доступа.
        string errorMessage; // Сообщение об ошибках во время работы.

        // Синхронизированные устройства, Функция - список клемм.
        Dictionary<Function, string> synchronizedDevices =
            new Dictionary<Function, string>();
    }
}
