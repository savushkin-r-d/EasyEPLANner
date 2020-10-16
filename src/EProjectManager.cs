using System;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.ApplicationFramework;
using System.Text.RegularExpressions;
using Eplan.EplApi.EServices.Ged;
using Eplan.EplApi.DataModel.Graphics;

namespace EasyEPlanner
{
    /// <summary>
    /// Менеджер проекта в CAD Eplan.
    /// </summary>
    class EProjectManager
    {
        public static EProjectManager GetInstance()
        {
            return instance;
        }

        private EProjectManager()
        {
        }

        public void StartEditModesWithDelay(int ms = 1)
        {
            System.Threading.Thread.Sleep(ms);

            String strAction = "XGedStartInteractionAction";
            if (startInteractionAction == null)
            {
                startInteractionAction = ActMnr.FindAction(strAction);
            }

            if (startInteractionAction != null)
            {
                ActionCallingContext oContext = new ActionCallingContext();
                oContext.AddParameter("Name", "SelectInteractionWhileEditModes");

                bool res = startInteractionAction.Execute(oContext);
            }
        }

        public void StopEditModes()
        {
            if (selectInteractionWhileEditModes != null)
            {
                selectInteractionWhileEditModes.Stop();

                selectInteractionWhileEditModes = null;
            }
        }

        public void SetEditInteraction(SelectInteractionWhileEditModes inter)
        {
            selectInteractionWhileEditModes = inter;
        }

        public SelectInteractionWhileEditModes GetEditInteraction()
        {
            return selectInteractionWhileEditModes;
        }

        /// <summary>
        /// Проверка имени проекта
        /// </summary>
        /// <param name="name">Имя проекта</param>
        public void CheckProjectName(ref string name)
        {
            const int MaxProjectNameLength = 70;
            if (name.Length > MaxProjectNameLength)
            {
                string errorMessage = $"Некорректно задано имя проекта - " +
                    $"длина не должна превышать {MaxProjectNameLength} символов. ";
                Logs.AddMessage(errorMessage);
                name = name.Substring(0, MaxProjectNameLength);
            }

            const string RegexPattern =
                "(?<plant>[A-Z]+[1-9]{1})\\-(?<project>[а-яА-Я1-9\\-]+$)";
            Match match = Regex.Match(name, RegexPattern);
            if (!match.Success)
            {
                string errorMessage = "Некорректно задано имя проекта - " +
                    "рекомендуется для имени площадки использовать " +
                    "английский, а для проекта - русский, вместо пробелов - " +
                    "знак минус (прим., LOC1-Название-проекта). ";
                Logs.AddMessage(errorMessage);
                name = GetModifyingCurrentProjectName();
            }
        }

        /// <summary>
        /// Имя проекта EPLAN
        /// </summary>
        /// <returns></returns>
        public string GetCurrentProjectName()
        {
            string name = currentProject.ProjectName;
            return name;
        }

        /// <summary>
        /// Модифицированное имя проекта (пробелы заменены на минусы)
        /// </summary>
        /// <returns></returns>
        public string GetModifyingCurrentProjectName()
        {
            string name = currentProject.ProjectName.Replace(" ", "-");
            return name;
        }

        public Project GetCurrentPrj()
        {
            return currentProject;
        }

        public void ResetCurrentPrj()
        {
            currentProject = null;
        }

        public void SetCurrentPrj(Project project)
        {
            currentProject = project;
        }

        public void SyncAndSave(bool saveDescrSilentMode = true)
        {
            if (currentProject != null && ProjectDataIsLoaded)
            {
                String strAction = "LoadDescriptionAction";
                ActionManager oAMnr = new ActionManager();
                Eplan.EplApi.ApplicationFramework.Action oAction =
                    oAMnr.FindAction(strAction);
                ActionCallingContext ctx = new ActionCallingContext();

                if (oAction != null)
                {
                    ctx.AddParameter("loadFromLua", "no");
                    oAction.Execute(ctx);
                }

                strAction = "SaveDescriptionAction";
                oAction = oAMnr.FindAction(strAction);
                if (oAction != null)
                {
                    ctx.AddParameter("silentMode",
                        saveDescrSilentMode ? "yes" : "no");
                    oAction.Execute(ctx);
                }
            }
        }

        /// <summary>
        /// Сохранить данные и закончить работу с дополнением (при закрытии
        /// проекта или Eplan).
        /// </summary>
        public void SaveAndClose()
        {
            SyncAndSave();
            StopEditModes();

            ExcelRepoter.AutomaticExportExcelForSCADA(currentProject);

            // Проверка и сохранение состояний окон.
            ModeFrm.CheckShown();
            ModeFrm.SaveCfg(ModeFrm.modeIsShown);
            DFrm.CheckShown();
            DFrm.SaveCfg(DFrm.deviceIsShown);
            Editor.NewEditorControl.CheckShown();
            Editor.NewEditorControl.SaveCfg();

            if (Editor.Editor.GetInstance().IsShown())
            {
                Editor.Editor.GetInstance().CloseEditor();
            }

            ModeFrm.GetInstance().CloseEditor();
            DFrm.GetInstance().CloseEditor();
        }

        private Project currentProject = null;
        private static EProjectManager instance = new EProjectManager();
        private SelectInteractionWhileEditModes selectInteractionWhileEditModes =
            null;

        public static bool isPreCloseProjectComplete = false;

        //Обработчик событий Eplan.
        private EplanEventListener eplanEventListener = new EplanEventListener();

        private ActionManager ActMnr = new ActionManager();
        private Eplan.EplApi.ApplicationFramework.Action startInteractionAction;

        /// <summary>
        /// Загружены или нет данные проекта.
        /// </summary>
        public bool ProjectDataIsLoaded { get; set; } = false;
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    public class SelectInteractionWhileEditModes : Interaction
    {
        // Код для подсветки устройств и занесения в действие
        public SelectInteractionWhileEditModes()
        {
        }

        public override RequestCode OnStart(InteractionContext pContext)
        {
            EProjectManager.GetInstance().SetEditInteraction(this);

            return RequestCode.Select |
                   RequestCode.NoPreselect |
                   RequestCode.NoMultiSelect;
        }

        public override RequestCode OnSelect(
            StorableObject[] arrSelectedObjects,
            SelectionContext oContext)
        {
            // Execute standard operation.
            base.OnSelect(arrSelectedObjects, oContext);

            if (EProjectManager.GetInstance().GetEditInteraction() != this)
            {
                return RequestCode.Stop;
            }

            Function oF = SearchSelectedObjectFunction(arrSelectedObjects[0]);
            if (oF != null)
            {
                Editor.ITreeViewItem newEditorItem = Editor.Editor
                    .GetInstance().EditorForm.GetActiveItem();
                if (newEditorItem == null)
                {
                    return RequestCode.Nothing;
                }

                if(newEditorItem != null)
                {
                    ExecuteForEditor(newEditorItem, oF);
                }
            }

            return RequestCode.Select |
                   RequestCode.NoPreselect |
                   RequestCode.NoMultiSelect;
        }

        /// <summary>
        /// Поиск функции выбранного объекта
        /// </summary>
        /// <param name="selectedObject">Выбранный объект</param>
        /// <returns></returns>
        private Function SearchSelectedObjectFunction(StorableObject 
            selectedObject)
        {
            Function oF = null;
            if (selectedObject is Rectangle)
            {
                string oFName = (selectedObject as Rectangle).Properties
                    .get_PROPUSER_TEST(1);
                oF = Function.FromStringIdentifier(oFName) as Function;
            }

            if (selectedObject is Function)
            {
                oF = selectedObject as Function;
            }

            return oF;
        }

        /// <summary>
        /// Обработка для нового редактора
        /// </summary>
        /// <param name="editorItem">Элемент из редактора</param>
        /// <param name="oF">Функция</param>
        private void ExecuteForEditor(Editor.ITreeViewItem editorItem,
            Function oF)
        {
            if (editorItem.IsUseDevList)
            {
                string devName;
                bool res = Device.DeviceManager.CheckDeviceName(oF.Name,
                    out devName, out _, out _, out _, out _);

                if (res)
                {
                    string checkedDevices = editorItem.EditText[1];
                    string newDevices = MakeNewCheckedDevices(devName,
                        checkedDevices);
                    Editor.Editor.GetInstance().EditorForm
                        .SetNewVal(newDevices);

                    //Обновление списка устройств при его наличии.
                    string checkedDev = editorItem.EditText[1];
                    if (DFrm.GetInstance().IsVisible() == true)
                    {
                        Device.DeviceType[] devTypes;
                        Device.DeviceSubType[] devSubTypes;
                        editorItem.GetDevTypes(out devTypes,
                            out devSubTypes);

                        DFrm.GetInstance().ShowDevices(
                            Device.DeviceManager.GetInstance(), devTypes,
                            devSubTypes, false, true, checkedDev, null);
                    }
                }
            }
        }

        /// <summary>
        /// Сгенерировать новую строку устройств
        /// </summary>
        /// <param name="devName">Устройство</param>
        /// <param name="checkedDevices">Текущие устройства</param>
        /// <returns></returns>
        private string MakeNewCheckedDevices(string devName,
            string checkedDevices)
        {
            string oldDevices = " " + checkedDevices + " ";
            string newDevices;
            //Для корректного поиска отделяем пробелами.
            devName = " " + devName + " ";

            if (oldDevices.Contains(devName))
            {
                newDevices = oldDevices.Replace(devName, " ");
            }
            else
            {
                newDevices = oldDevices + devName.Trim();
            }

            return newDevices;
        }

        private delegate void DelegateWithParameters(int param1);

        public override void OnStop()
        {
            base.OnStop();
        }

        public void Stop()
        {
            IsFinish = true;
        }

        public bool IsFinish { get; set; } = false;
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    class EplanEventListener
    {
        Eplan.EplApi.ApplicationFramework.EventHandler onUserPreCloseProject = new
            Eplan.EplApi.ApplicationFramework.EventHandler();
        Eplan.EplApi.ApplicationFramework.EventHandler onPostOpenProject = new
            Eplan.EplApi.ApplicationFramework.EventHandler();

        Eplan.EplApi.ApplicationFramework.EventHandler onMainEnd = new
            Eplan.EplApi.ApplicationFramework.EventHandler();
        Eplan.EplApi.ApplicationFramework.EventHandler onMainStart = new
            Eplan.EplApi.ApplicationFramework.EventHandler();

        Eplan.EplApi.ApplicationFramework.EventHandler onNotifyPageChanged = new
            Eplan.EplApi.ApplicationFramework.EventHandler();

        public EplanEventListener()
        {
            onUserPreCloseProject.SetEvent("Eplan.EplApi.OnUserPreCloseProject");
            onUserPreCloseProject.EplanEvent +=
                new EventHandlerFunction(OnUserPreCloseProject);
            onPostOpenProject.SetEvent("Eplan.EplApi.OnPostOpenProject");
            onPostOpenProject.EplanEvent +=
                new EventHandlerFunction(OnPostOpenProject);

            onMainEnd.SetEvent("Eplan.EplApi.OnMainEnd");
            onMainEnd.EplanEvent +=
                new EventHandlerFunction(OnMainEnd);
            onMainStart.SetEvent("Eplan.EplApi.OnMainStart");
            onMainStart.EplanEvent +=
                new EventHandlerFunction(OnMainStart);

            onNotifyPageChanged.SetEvent("NotifyPageOpened");
            onNotifyPageChanged.EplanEvent += 
                new EventHandlerFunction(OnNotifyPageChanged);
        }

        private void OnNotifyPageChanged(IEventParameter eventParameter)
        {
            var interaction = EProjectManager.GetInstance()
                .GetEditInteraction();
            if (interaction != null && interaction.IsFinish == false)
            {
                EProjectManager.GetInstance().StartEditModesWithDelay();
            }
        }

        private void OnUserPreCloseProject(IEventParameter iEventParameter)
        {
            String projectName =
                (new EventParameterString(iEventParameter)).String;
            int dot_pos = projectName.LastIndexOf('\\');
            if (dot_pos > 0)
            {
                projectName = projectName.Substring(dot_pos + 1);
            }
            projectName = projectName.Remove(projectName.IndexOf('.'));

            if (projectName == EProjectManager.GetInstance().GetCurrentProjectName())
            {
                EProjectManager.GetInstance().SaveAndClose();

                DFrm.GetInstance().ShowNoDevices();

                EProjectManager.GetInstance().ResetCurrentPrj();
                EProjectManager.isPreCloseProjectComplete = true;
            }
        }

        private void OnPostOpenProject(IEventParameter iEventParameter)
        {
            //Если нет активного проекта, считываем описание и работаем с ним.
            if (EProjectManager.GetInstance().GetCurrentPrj() == null)
            {
                //Получение текущего проекта.
                Eplan.EplApi.HEServices.SelectionSet selection =
                    new Eplan.EplApi.HEServices.SelectionSet();
                selection.LockSelectionByDefault = false;
                selection.LockProjectByDefault = false;

                EProjectManager.GetInstance().SetCurrentPrj(
                    selection.GetCurrentProject(true));

                String strAction = "LoadDescriptionAction";
                ActionManager oAMnr = new ActionManager();
                Eplan.EplApi.ApplicationFramework.Action oAction =
                    oAMnr.FindAction(strAction);
                ActionCallingContext ctx = new ActionCallingContext();

                if (oAction != null)
                {
                    oAction.Execute(ctx);
                }

                strAction = "ShowTechObjectsAction";
                oAction = oAMnr.FindAction(strAction);

                if (oAction != null)
                {
                    //Восстановление при необходимости окна редактора.
                    string path = Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData) +
                        @"\Eplan\eplan.cfg";
                    PInvoke.IniFile ini = new PInvoke.IniFile(path);
                    string res = ini.ReadString("main", "show_obj_window",
                        "false");

                    if (res == "true")
                    {
                        oAction.Execute(ctx);
                    }
                }

                strAction = "ShowDevicesAction";
                oAction = oAMnr.FindAction(strAction);

                if (oAction != null)
                {
                    //Восстановление при необходимости окна устройств.
                    string path = Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData) +
                        @"\Eplan\eplan.cfg";
                    PInvoke.IniFile ini = new PInvoke.IniFile(path);
                    string res = ini.ReadString("main", "show_dev_window",
                        "false");

                    if (res == "true")
                    {
                        oAction.Execute(ctx);
                    }
                }

                strAction = "ShowOperationsAction";
                oAction = oAMnr.FindAction(strAction);

                if (oAction != null)
                {
                    //Восстановление при необходимости окна операций.
                    string path = Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData) +
                        @"\Eplan\eplan.cfg";
                    PInvoke.IniFile ini = new PInvoke.IniFile(path);
                    string res = ini.ReadString("main", "show_oper_window",
                        "false");

                    if (res == "true")
                    {
                        oAction.Execute(ctx);
                    }
                }

                // Проект открыт, ставим флаг в изначальное состояние.
                EProjectManager.isPreCloseProjectComplete = false;
            }
        }

        private void OnMainEnd(IEventParameter iEventParameter)
        {
            if (EProjectManager.GetInstance().GetCurrentPrj() != null)
            {
                EProjectManager.GetInstance().SaveAndClose();

                ////Проверяю, закрыт ли проект(выполнено ли PreCloseProject)
                //// Или же он закрыт "варварски"
                if (EProjectManager.isPreCloseProjectComplete == false)
                {
                    // Если проект закрыт варварски, то при новом открытия окна не открывать
                    DFrm.SaveCfg(false);
                    ModeFrm.SaveCfg(false);
                    Editor.NewEditorControl.SaveCfg(false);
                }
            }

            IdleModule.Stop();
        }

        private void OnMainStart(IEventParameter iEventParameter)
        {
            IdleModule.Start();
        }
    }
}