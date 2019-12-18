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
                Project currentProject = EProjectManager.GetInstance().GetCurrentPrj();
                if (currentProject == null)
                {
                    MessageBox.Show("Нет открытого проекта!", "EPlaner",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    //Получение текущего проекта.
                    EplanIOManager.GetInstance().UpdateModulesBinding();
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
                    int n = Convert.ToInt32(match.Groups["n"].Value);

                    // Проверка узлов
                    if (n % 100 == 0 || n == 1)                                      //3
                    {
                        is_exist_any_node = true;

                        // Имя узла/модуля
                        string name = $"A{n}";

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
                                if (n == 1)
                                {
                                    iOManager.AddNode(1, type, IP, name);
                                }
                                else
                                {
                                    iOManager.AddNode(n / 100 + 1, type, IP, name);
                                }
                            }
                            else
                            {
                                iOManager.AddNode(n / 100, type, IP, name);
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

                    int n = Convert.ToInt32(match_name.Groups["n"].Value);

                    if (n % 100 != 0 && n != 1)                                 //4
                    {
                        int module_n = n % 100;
                        int node_n;

                        // Если есть "A1", то учитываем, что он первый
                        if (isContainsA1 == true)
                        {
                            node_n = n / 100;
                        }
                        else
                        {
                            // А1 нету, оставляем как было
                            node_n = n / 100 - 1;
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
                            }

                            iOManager[node_n].DI_count += moduleInfo.DI_count;
                            iOManager[node_n].DO_count += moduleInfo.DO_count;
                            iOManager[node_n].AI_count += moduleInfo.AI_count;
                            iOManager[node_n].AO_count += moduleInfo.AO_count;

                            IO.IOModule new_module =
                                new IO.IOModule(inOffset, outOffset, moduleInfo);

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
        /// Синхронизация названий устройств и модулей
        /// </summary>
        public void UpdateModulesBinding()
        {
            ReadConfiguration();
            EplanDeviceManager.GetInstance().
                SynchAndReadConfigurationFromScheme();
            EplanDeviceManager.GetInstance().ReadConfigurationFromIOModules();
            EplanDeviceManager.GetInstance().CheckConfiguration();

            SelectionSet selection = new SelectionSet();
            Project currentProject = selection.GetCurrentProject(true);
            DMObjectsFinder objectFinder = new DMObjectsFinder(currentProject);

            FunctionsFilter functionsFilter = new FunctionsFilter();
            FunctionPropertyList properties = new FunctionPropertyList();
            properties.FUNC_MAINFUNCTION = true;
            functionsFilter.SetFilteredPropertyList(properties);
            functionsFilter.Category = Function.Enums.Category.PLCBox;
            Function[] functions = objectFinder.GetFunctions(functionsFilter);

            currentProject.LockAllObjects();

            Dictionary<string, string> deviceConnections = 
                CollectIOModulesData(functions);

            bool containsNewValveTerminal = GetProjectVersionFromDevices
                (deviceConnections);
            if (!containsNewValveTerminal)
            {
                SynchronizeAsOldProject(deviceConnections, functions);
                return;
            }
            else
            {
                SynchronizeAsNewProject(deviceConnections, functions);
                return;
            }
        }

        /// <summary>
        /// Сбор данных привязок для обновления
        /// </summary>
        /// <param name="functions">Главные функции для поиска обновленных 
        /// данных</param>
        /// <returns></returns>
        private Dictionary<string, string> CollectIOModulesData
            (Function[] functions)
        {
            Dictionary<string, string> deviceConnections = new 
                Dictionary<string, string>();

            foreach (Device.IODevice device in Device.DeviceManager.
                GetInstance().Devices)
            {
                foreach (Device.IODevice.IOChannel channel in device.Channels)
                {
                    if (!channel.IsEmpty())
                    {
                        CollectModuleData(device, channel, functions,
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
        /// <param name="functions">Главные функции для поиска обновленных 
        /// данных</param>
        private void CollectModuleData(Device.IODevice device, 
            Device.IODevice.IOChannel channel, Function[] functions,
            ref Dictionary<string, string> deviceConnections)
        {
            const string IOModulePrefix = "A";
            const string ASInterfaceModule = "655";

            string visibleName = IOModulePrefix + channel.fullModule;
            Function moduleFunction = functions.
                FirstOrDefault(x => x.VisibleName.Contains(visibleName));
            if (moduleFunction != null)
            {
                visibleName += ChannelPostfix + channel.GetKlemme.ToString();
                string functionalText = device.EPlanName;
                // Для модулей ASi не нужно добавлять комментарии 
                // к имени устройств.
                if (!moduleFunction.ArticleReferences[0].PartNr.
                    Contains(ASInterfaceModule))
                {
                    if (device.Description.Contains(PlusSymbol))
                    {
                        // Так как в комментариях может использоваться знак 
                        // "плюс", то заменим его на какую-то константу, 
                        // которую при обновлении функционального текста 
                        // преобразуем обратно.
                        string replacedDeviceDescription = device.Description.
                            Replace(PlusSymbol.ToString(), symbolForPlusReplacing);
                        functionalText += NewLine + replacedDeviceDescription +
                            NewLine + channel.komment;
                    }
                    else
                    {
                        functionalText += NewLine + device.Description +
                            NewLine + channel.komment;
                    }
                }
                else
                {
                    functionalText += WhiteSpace;
                }

                if (deviceConnections.ContainsKey(visibleName))
                {
                    deviceConnections[visibleName] += functionalText;
                }
                else
                {
                    deviceConnections.Add(visibleName,functionalText);
                }
            }
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
            bool isProjectWithValveTerminal = default;

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
        /// Первый ключ - модуль, второй - устройства</param>
        /// <param name="functions">Функции обновляемых объектов</param>
        /// <returns></returns>
        private void SynchronizeAsOldProject(Dictionary<string, string>
            deviceConnections, Function[] functions)
        {
            foreach (string key in deviceConnections.Keys)
            {
                string visibleName = key.Substring(0, key.
                    IndexOf(ChannelPostfix));
                Function synchronizedModule = functions.FirstOrDefault(x => x.
                VisibleName.Contains(visibleName));

                if (synchronizedModule == null)
                {
                    continue;
                }

                string clampNumberString = key.Remove(0, key.
                    IndexOf(ChannelPostfix) + ChannelPostfixSize);
                string devices = deviceConnections[key];

                bool deletingComments = NeedDeletingComments(devices);
                if (deletingComments)
                {
                    devices = SortDevices(devices);
                    devices = DeleteDevicesComments(devices);
                    devices = DeleteRepeatedDevices(devices);
                }

                int clamp = Convert.ToInt32(clampNumberString);
                SynchronizeIOModule(synchronizedModule, clamp, devices);
            }
        }

        /// <summary>
        /// Синхронизация привязки к I/O модулю
        /// </summary>
        /// <param name="functionalText">Обновленный функциональный текст
        /// </param>
        /// <param name="clamp">Номер клеммы</param>
        /// <param name="synchronizedModule">Обновляемый модуль</param>
        private void SynchronizeIOModule(Function synchronizedModule, 
            int clamp, string functionalText)
        {
            // Конвертируем символ "плюс" обратно.
            functionalText = functionalText.
                Replace(symbolForPlusReplacing, PlusSymbol.ToString());

            Function[] placedFunctions = synchronizedModule.SubFunctions.
                Where(x => x.IsPlaced == true).ToArray();

            Function clampFunction = placedFunctions.
                        Where(x => x.Page.PageType == 
                        DocumentTypeManager.DocumentType.Circuit).
                        FirstOrDefault(x => x.Properties.
                        FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToInt() == clamp);

            // Если null - клеммы нет на схеме.
            if (clampFunction != null)
            {
                clampFunction.Properties.FUNC_TEXT = functionalText;
            }
        }

        /// <summary>
        /// Синхронизация нового проекта
        /// </summary>
        /// <param name="deviceConnections">Объекты для обновления</param>
        /// <param name="functions">Функции обновляемых объектов</param>
        private void SynchronizeAsNewProject(Dictionary<string, string>
            deviceConnections, Function[] functions)
        {
            foreach (string moduleString in deviceConnections.Keys)
            {
                string visibleName = moduleString.Substring(
                    0, moduleString.IndexOf(ChannelPostfix));
                Function synchronizedModule = functions.
                    FirstOrDefault(x => x.VisibleName.Contains(visibleName));

                if (synchronizedModule == null)
                {
                    continue;
                }

                string clampNumberString = moduleString.Remove(0, 
                    moduleString.IndexOf(ChannelPostfix) + ChannelPostfixSize);

                // Если нет пневмоострова Y - то синхронизация ничем 
                // не отличается от старого проекта.
                if (!deviceConnections[moduleString].Contains(ValveTerminal))
                {
                    string devices = deviceConnections[moduleString];
                    bool isASInterface = CheckASInterface(devices);
                    bool deletingComments = NeedDeletingComments(devices);

                    if (deletingComments == true && isASInterface == false)
                    {
                        devices = SortDevices(devices);
                        devices = DeleteDevicesComments(devices);
                        devices = DeleteRepeatedDevices(devices);
                    }

                    if (deletingComments == true && isASInterface == true)
                    {
                        devices = DeleteRepeatedDevices(devices);
                        devices = SortASInterfaceDevices(devices);
                    }

                    int clamp = Convert.ToInt32(clampNumberString);
                    SynchronizeIOModule(synchronizedModule, clamp, devices);
                }
                else
                {
                    string functionalText = deviceConnections[moduleString].
                        Split(PlusSymbol).
                        FirstOrDefault(x => x.Contains(ValveTerminal)).
                        Insert(0, PlusSymbol.ToString());
                    int clamp = Convert.ToInt32(clampNumberString);
                    // Синхронизация пневмоострова.
                    SynchronizeIOModule(synchronizedModule, clamp,
                        functionalText);

                    // Синхронизация устройств привязанных к пневмоострову.
                    Dictionary<int, string> IOModuleDevices =
                        GetIOModuleDevices(deviceConnections[moduleString]);
                    string functionName = GetDeviceFunctionName(
                        deviceConnections[moduleString]);
                    Function synchronizedDevice = functions.
                        FirstOrDefault(x => x.Name.Contains(functionName));
                    SynchronizeIOModuleDevices(synchronizedDevice,
                        IOModuleDevices);
                }
            }
        }

        /// <summary>
        /// Получение имени функции устройства в котором обновляется привязка
        /// (в частности, пневмоострова)
        /// </summary>
        /// <param name="functionalText">>Функциональный текст устройства, 
        /// к которому привязано устройство</param>
        /// <returns></returns>
        private string GetDeviceFunctionName(string functionalText)
        {
            const string FunctionNamePattern = "(\\+[A-Z0-9_]+-[Y0-9]+)";
            Match functionNameMatch = Regex.Match(functionalText, 
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
        /// Получение устройств, привязанных к устройству.
        /// </summary>
        /// <param name="IOModuleFunctionalText">Функциональный текст 
        /// IO модуля, к которому привязано устройство</param>
        /// <returns></returns>
        private Dictionary<int, string> GetIOModuleDevices
            (string IOModuleFunctionalText)
        {
            const string ClampNumberParameter = "R_VTUG_NUMBER";

            string[] devicesArray = IOModuleFunctionalText.Split(PlusSymbol);
            Dictionary<int, string> IOModuleDevices = 
                new Dictionary<int, string>();

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
                string runtimeParameter = device.GetRuntimeParameter(
                    ClampNumberParameter);
                int clamp = Convert.ToInt32(runtimeParameter);

                if (IOModuleDevices.ContainsKey(clamp))
                {
                    IOModuleDevices[clamp] += deviceString.
                        Insert(0, PlusSymbol.ToString());
                }
                else
                {
                    IOModuleDevices.Add(clamp, deviceString.
                        Insert(0, PlusSymbol.ToString()));
                }
            }

            return IOModuleDevices;
        }

        /// <summary>
        /// Функция, проверяющая являются ли переданные устройства
        /// устройствами с AS-интерфейсом
        /// </summary>
        /// <param name="devices">Список ОУ устройств через разделитель</param>
        /// <returns></returns>
        private bool CheckASInterface(string devices)
        {
            bool isASInterface = false;
            const int MinimalDevicesCount = 2;
            MatchCollection deviceMatches = Regex.Matches(devices, 
                DeviceNamePattern);
            
            if (deviceMatches.Count < MinimalDevicesCount)
            {
                return isASInterface;
            }

            // Проверяем только первое устройство т.к остальные тоже будут
            // с AS-интерфейсом (согласно правилам использования Add-In).
            const int devicesCountForCheck = 1;
            for (int devicesCounter = 0; devicesCounter <= devicesCountForCheck; devicesCounter++)
            {
                Device.IODevice device = Device.DeviceManager.GetInstance().
                    GetDevice(deviceMatches[devicesCounter].Value);
                if (device.GetDeviceSubType == Device.DeviceSubType.V_AS_MIXPROOF ||
                    device.GetDeviceSubType == Device.DeviceSubType.V_AS_DO1_DI2)
                {
                    isASInterface = true;
                    return isASInterface;
                }
            }
            
            return isASInterface;
        }

        /// <summary>
        /// Функция, проверяющая необходимость удаления комментариев в 
        /// функциональном тексте.
        /// </summary>
        /// <param name="devices">Строка со списком устройств</param>
        /// <returns></returns>
        private bool NeedDeletingComments(string devices)
        {
            MatchCollection deviceMatches = Regex.Matches(devices,
                DeviceNamePattern);
            if (deviceMatches.Count > 1)
            {
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
            MatchCollection deviceMatches = Regex.Matches(devices,
                DeviceNamePattern);

            if (deviceMatches.Count <= 1)
            {
                return devices;
            }

            string devicesWithoutComments = string.Empty;
            foreach (Match match in deviceMatches)
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
            List<string> devicesList = new List<string>();

            MatchCollection deviceMatches = Regex.Matches(
                devicesWithoutComments, DeviceNamePattern);
            foreach (Match match in deviceMatches)
            {
                devicesList.Add(match.Value);
            }

            devicesList = devicesList.Distinct().ToList();

            string devicesWithoutRepeating = string.Empty;
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
            MatchCollection deviceMatches = Regex.Matches(devices,
                DeviceNamePattern);
            if (deviceMatches.Count <= 1)
            {
                return devices;
            }

            // valveTerminal - для вставки VTUG в старых проектах первым.
            Device.Device valveTerminal = null;
            List<Device.Device> devicesList = new List<Device.Device>();
            foreach (Match match in deviceMatches)
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

            devices = string.Empty;
            foreach (Device.Device device in devicesList)
            {
                if (device.Description.Contains(PlusSymbol))
                {
                    string replacedDeviceDescription = device.Description.
                        Replace(PlusSymbol.ToString(), symbolForPlusReplacing);
                    devices += device.EPlanName + NewLine + 
                        replacedDeviceDescription + NewLine;
                }
                else
                {
                    devices += device.EPlanName + NewLine + device.Description +
                        NewLine;
                }

            }

            return devices;
        }

        /// <summary>
        /// Функция для сортировки AS интерфейса по параметру R_AS_NUMBER
        /// </summary>
        /// <param name="devices">Строка с устройствами</param>
        /// <returns></returns>
        private string SortASInterfaceDevices(string devices)
        {
            string sortedDevices = string.Empty;
            
            MatchCollection deviceMatches = Regex.Matches(devices,
                DeviceNamePattern);
            if (deviceMatches.Count <= 1)
            {
                return devices;
            }

            List<Device.IODevice> devicesList = new List<Device.IODevice>();
            foreach (Match match in deviceMatches)
            {
                Device.IODevice device = Device.DeviceManager.GetInstance().
                    GetDevice(match.Value);
                devicesList.Add(device);
            }

            devicesList.Sort(ASInterfaceDevicesComparer);

            int lastASNumber = 0;
            string devicesWithoutASNumber = NewLine;
            foreach (Device.IODevice device in devicesList)
            {
                string numberAsString = device.GetRuntimeParameter("R_AS_NUMBER");
                if (numberAsString == null)
                {
                    devicesWithoutASNumber += device.EPlanName
                        + NewLine;
                    continue;
                }

                int number;
                int.TryParse(numberAsString, out number);

                const int MinimalNumber = 0;
                const int MaximalValue = 64;
                if (number <= MinimalNumber && number > MaximalValue)
                {
                    devicesWithoutASNumber += device.EPlanName
                        + NewLine;
                    continue;
                }

                if (lastASNumber != number)
                {
                    //TODO: Проверка порядковых номеров устройств и вставка
                    // пробелов (придумать как делить строки),
                    // что бы оставить строку с отсутствующим номером пустой.
                }

                lastASNumber = number;
            }

            return sortedDevices;
        }

        /// <summary>
        /// Компаратор для сравнения устройств с
        /// AS-интерфейсом (сортировки).
        /// </summary>
        /// <param name="device1">Устройство 1</param>
        /// <param name="device2">Устройство 2</param>
        /// <returns></returns>
        private int ASInterfaceDevicesComparer(Device.Device device1, 
            Device.Device device2)
        {
            int res = 0;
            //TODO: Comparer
            return res;
        }

        /// <summary>
        /// Синхронизация привязки устройств привязанных к модулю.
        /// Реализовано сразу два типа синхронизации
        /// </summary>
        /// <param name="synchronizedModule">Синхронизируемый модуль</param>
        /// <param name="IOModuleDevices">Устройства для синхронизации</param>
        private void SynchronizeIOModuleDevices(Function synchronizedModule, 
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
                    Replace(symbolForPlusReplacing, PlusSymbol.ToString());

                string functionalTextWithoutComments =
                    DeleteDevicesComments(functionalText);

                // На электрических схемах при множественной привязке
                // комментарий не пишем.
                SynchronizeDevice(synchronizedModule, clamp,
                    functionalTextWithoutComments, circuitDocument);

                // На странице "Обзор" при множественной привязке надо писать
                // комментарии.
                SynchronizeDevice(synchronizedModule, clamp, functionalText,
                    overviewDocument);
            }
        }

        /// <summary>
        /// Синхронизация привязки устройства.
        /// </summary>
        /// <param name="objectFunction">Функция обновляемого объекта</param>
        /// <param name="clamp">Номер клеммы</param>
        /// <param name="functionalText">Функциональный текст</param>
        /// <param name="documentType">Тип страницы для поиска подфункции
        /// </param>
        private void SynchronizeDevice(Function objectFunction, int clamp, 
            string functionalText, 
            DocumentTypeManager.DocumentType documentType)
        {
            const string ClampFunctionDefenitionName = "Дискретный выход";

            Function[] placedFunctions = objectFunction.SubFunctions.
                Where(x => x.IsPlaced == true).ToArray();

            Function clampFunction = placedFunctions.
                Where(x => x.Page.PageType == documentType &&
                x.FunctionDefinition.Name.GetString(ISOCode.Language.L_ru_RU).
                Contains(ClampFunctionDefenitionName)).
                FirstOrDefault(y => y.Properties.
                FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToInt() == clamp);

            // Если null - клеммы нет на схеме.
            if (clampFunction != null)
            {
                clampFunction.Properties.FUNC_TEXT = functionalText;
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

        public string SaveAsLuaTable(string prefix) 
        {
            return iOManager.SaveAsLuaTable(prefix);
        }

        private IO.IOManager iOManager;     // Экземпляр класса.
        private static EplanIOManager instance; // Экземпляр класса.  

        const string NewLine = "\r\n"; // Возврат каретки и перенос строки.
        const string DeviceNamePattern = "(\\+[A-Z0-9_]*-[A-Z0-9_]+)"; // ОУ.
        const string ValveTerminal = "-Y"; // Обозначение пневмоострова.
        const string ChannelPostfix = "_CH"; // Постфикс канала.
        const int ChannelPostfixSize = 3; // Размер постфикса канала.
        const char PlusSymbol = '+'; // Для обратной вставки после Split.
        const string WhiteSpace = " "; // Пробел для разделения ОУ.
        const string symbolForPlusReplacing = "plus"; // Замена символа "плюс".
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Все устройства проекта (клапана, насосы, ...).
    /// </summary>
    class EplanDeviceManager : Device.IDeviceManager
    {
        public static Dictionary<String, String> GetAssigment(
            Function oF2, IO.IOModuleInfo moduleInfo, string devsDescr = "")
        {
            Dictionary<String, String> res =
                new Dictionary<String, String>();

            int nn; // Номер клеммы
            string nnString = oF2.Properties.FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
            // Если клемма - число, возвращаем res
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

            string komment = "";

            int endPos = devsDescr.IndexOf("\n");
            if (endPos > 0)
            {
                komment = devsDescr.Substring(endPos + 1);
                devsDescr = devsDescr.Substring(0, endPos);
            }

            MatchEvaluator deviceEvaluator = new MatchEvaluator(GetInstance().RussianToEnglish);
            devsDescr = Regex.Replace(devsDescr, RussianLettersAsEnglish, deviceEvaluator);

            string klemmeKomment = "";
            Match actionMatch = Regex.Match(komment, ChannelComment, RegexOptions.IgnoreCase);

            komment = Regex.Replace(komment, ChannelComment, "", RegexOptions.IgnoreCase);
            komment = komment.Replace("\n", ". ").Trim();
            if (komment.Length > 0 && komment[komment.Length - 1] != '.')
            {
                komment += ".";
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
            Project cProject = EProjectManager.GetInstance().GetCurrentPrj();
            if (cProject == null)
            {
                return;
            }

            //Initialize the DMObjectsFinder with a project.
            DMObjectsFinder oFinder = new DMObjectsFinder(cProject);
            FunctionsFilter oFunctionsFilter = new FunctionsFilter();

            FunctionPropertyList props = new FunctionPropertyList();
            props.FUNC_MAINFUNCTION = true;
            //props.FUNC_ARTICLE_MANUFACTURER[ 1 ] = "WAGO";

            oFunctionsFilter.SetFilteredPropertyList(props);
            oFunctionsFilter.Category = Function.Enums.Category.PLCBox;

            //Get function with given name from project.
            Function[] arrFuncs = oFinder.GetFunctions(oFunctionsFilter); //1

            //Шаблоны для разбора названий модулей и узлов IO.
            const string PATTERN = @"=*-A(?<n>\d+)";                //-A101
            Regex regex = new Regex(PATTERN);

            FunctionsFilter oFunctionsFilter2 = new FunctionsFilter();
            oFunctionsFilter2.ExactNameMatching = true;
            oFunctionsFilter2.Category = Function.Enums.Category.PLCTerminal;

            //Если нет модулей IO - не считываем привязку
            int nodesCount = IO.IOManager.GetInstance().IONodes.Count;
            if (nodesCount == 0)
            {
                ProjectManager.GetInstance().AddLogMessage("В проекте отсутствуют узлы ввода/вывода.");
                return;
            }

            // Есть ли PXC A1 в проекте
            bool isContainsA1 = false;
            if (IO.IOManager.GetInstance().IONodes[0].Type ==
                IO.IONode.TYPES.T_PHOENIX_CONTACT_MAIN)
            {
                isContainsA1 = true;
            }

            foreach (Function oF in arrFuncs)
            {
                Match match = regex.Match(oF.VisibleName);
                if (match.Success)
                {
                    //Признак пропуска данного устройства.
                    if (!oF.Properties.FUNC_SUPPLEMENTARYFIELD[1].IsEmpty)
                    {
                        continue;
                    }

                    int n = Convert.ToInt32(match.Groups["n"].Value);

                    // Проверка модулей узлов
                    if (n % 100 != 0 && n != 1)                                      //4
                    {
                        int module_n = n % 100;
                        int node_n;

                        // Если есть "A1", то учитываем, что он первый
                        if (isContainsA1 == true)
                        {
                            node_n = n / 100;
                        }
                        else
                        {
                            // Если нету, оставляем как было
                            node_n = n / 100 - 1;
                        }

                        if (oF.Articles.GetLength(0) == 0)
                        {
                            ProjectManager.GetInstance().AddLogMessage("У модуля \"" +
                                oF.VisibleName + "\" не задано изделие.");
                            continue;
                        }

                        string name = "";
                        if (!oF.Articles[0].Properties[
                            Eplan.EplApi.DataModel.Properties.Article.ARTICLE_TYPENR].IsEmpty)
                        {
                            name = oF.Articles[0].Properties[
                                Eplan.EplApi.DataModel.Properties.Article.ARTICLE_TYPENR].ToString().Trim();
                        }

                        bool isStub;
                        IO.IOModuleInfo moduleInfo = new IO.IOModuleInfo();
                        moduleInfo = moduleInfo.GetIOModuleInfo(name, out isStub);

                        foreach (Function oF2 in oF.SubFunctions)
                        {
                            int nn; // Номер клеммы
                            string nnString = oF2.Properties.FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
                            // Если клемма - число, пропускаем
                            bool isDigit = Int32.TryParse(nnString, out nn);
                            if (isDigit == false)
                            {
                                continue;
                            }

                            if (Array.IndexOf(moduleInfo.ChannelClamps, nn) < 0)
                            {
                                continue;
                            }

                            if (oF2.Page.PageType != DocumentTypeManager.DocumentType.Circuit)
                            {
                                continue;
                            }

                            string descr = oF2.Properties.FUNC_TEXT_AUTOMATIC.ToString(
                                ISOCode.Language.L___);
                            if (descr == "")
                            {
                                descr = oF2.Properties.FUNC_TEXT_AUTOMATIC.ToString(
                                    ISOCode.Language.L_ru_RU);
                            }
                            if (descr == "")
                            {
                                descr = oF2.Properties.FUNC_TEXT.ToString(
                                    ISOCode.Language.L_ru_RU);
                            }

                            if (descr == null || descr == "")
                            {
                                continue;
                            }

                            // Проверка пневмоостровов FESTO
                            const string FESTO_PATTERN = @"=*-Y(?<n>\d+)";
                            Regex festo_regex = new Regex(FESTO_PATTERN);
                            Match festo_match = festo_regex.Match(descr);
                            // Нашло совпадение (Есть пневмоостров)
                            if (festo_match.Success)
                            {
                                // Инициализация начальных данных
                                Function[] subFunctions = new Function[0];

                                int descrName;
                                // Вырезается ОУ устройства для привязки, без комментария
                                if (descr.Contains("\r\n") == true)
                                {
                                    descrName = descr.IndexOf("\r\n");
                                    if (descrName > 0)
                                    {
                                        descr = descr.Substring(0, descrName);
                                    }
                                }
                                else
                                {
                                    descrName = descr.IndexOf("\n");
                                    if (descrName > 0)
                                    {
                                        descr = descr.Substring(0, descrName);
                                    }
                                }

                                // Поиск по функциям
                                foreach (Function f in arrFuncs)
                                {
                                    // Нашли нужную функцию устройства
                                    if (f.Name.Contains(descr))
                                    {
                                        subFunctions = f.SubFunctions;
                                        break;
                                    }
                                }

                                // Перебираем подфункции
                                foreach (Function subFunction in subFunctions)
                                {
                                    // Если это клемма
                                    if (subFunction.Category == Function.Enums.Category.PLCTerminal)
                                    {
                                        // Проверяем номер клеммы и текст
                                        string terminalNumber = subFunction.Properties.
                                            FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
                                        string terminalBindedDevice = subFunction.Properties.
                                            FUNC_TEXT_AUTOMATIC.ToString(ISOCode.Language.L___);
                                        bool isInt = Int32.TryParse(terminalNumber, out int doNotUse);
                                        // Если клемма - число, и есть текст для привязки
                                        if (isInt == true && terminalBindedDevice.Length != 0 && terminalBindedDevice != "Резерв")
                                        {
                                            Match festoDescrMatch = Regex.Match(
                                                terminalBindedDevice, Device.DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
                                            while (festoDescrMatch.Success)
                                            {
                                                string devName = festoDescrMatch.Groups["name"].Value;

                                                // Дополняется descr и отправляется, будто привязано к I/O
                                                descr += $" {devName}";

                                                // Дополнительно установить параметр R_VTUG_NUMBER
                                                int vtugNumber = Convert.ToInt32(terminalNumber);
                                                Dictionary<string, double> runtimeParams = new Dictionary<string, double>();
                                                runtimeParams.Add("R_VTUG_NUMBER", vtugNumber);
                                                SetUpDeviceRuntimeParameters(runtimeParams, devName);

                                                festoDescrMatch = festoDescrMatch.NextMatch();
                                            }
                                        }
                                    }
                                }
                            }

                            //Для многострочного описания убираем tab+\r\n
                            descr = descr.Replace("\t\r\n", "");
                            descr = descr.Replace("\t\n", "");

                            string komment = "";

                            int endPos = descr.IndexOf("\n");
                            if (endPos > 0)
                            {
                                komment = descr.Substring(endPos + 1);
                                descr = descr.Substring(0, endPos);
                            }

                            MatchEvaluator deviceEvaluator = new MatchEvaluator(RussianToEnglish);
                            descr = Regex.Replace(descr, RussianLettersAsEnglish, deviceEvaluator);

                            string klemmeKomment = "";
                            
                            Match actionMatch = Regex.Match(komment, ChannelComment,
                                RegexOptions.IgnoreCase);

                            komment = Regex.Replace(komment, ChannelComment,
                                "", RegexOptions.IgnoreCase);
                            komment = komment.Replace("\n", ". ").Trim();
                            if (komment.Length > 0 && komment[komment.Length - 1] != '.')
                            {
                                komment += ".";
                            }

                            Match descrMatch = Regex.Match(
                                descr, Device.DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);

                            if (!descrMatch.Success && !descr.Equals("Pезерв"))
                            {
                                ProjectManager.GetInstance().AddLogMessage(
                                    string.Format(
                                    "\"{0}:{1}\" - неверное имя привязанного устройства - \"{2}\".",
                                    oF.VisibleName, nn, descr));
                            }

                            while (descrMatch.Success)
                            {
                                string devName = descrMatch.Groups["name"].Value;

                                if (actionMatch.Success)
                                {
                                    klemmeKomment = actionMatch.Value;
                                }

                                string errStr = "";

                                int logicalPort = Array.IndexOf(moduleInfo.ChannelClamps, nn) + 1;
                                int moduleOffset = IO.IOManager.GetInstance().IONodes[node_n].IOModules[module_n - 1].InOffset;

                                Device.DeviceManager.GetInstance().AddDeviceChannel(
                                    Device.DeviceManager.GetInstance().GetDevice(devName),
                                    moduleInfo.AddressSpaceType, node_n,
                                    module_n, nn, klemmeKomment, out errStr, n, logicalPort, moduleOffset);

                                if (errStr != "")
                                {
                                    errStr = string.Format("\"{0}:{1}\" : {2}",
                                       oF.VisibleName, nn, errStr);

                                    ProjectManager.GetInstance().AddLogMessage(errStr);
                                }

                                descrMatch = descrMatch.NextMatch();
                            }
                        }
                    }
                }
            }
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
            string res = deviceManager.CheckDevicesConnection();
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
}
