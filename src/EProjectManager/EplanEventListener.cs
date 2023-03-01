using Eplan.EplApi.ApplicationFramework;
using System;
using IdleTimeModule;
using IdleTimeModule.EplanAPIHelper;
using System.Diagnostics;

namespace EasyEPlanner
{
    class EplanEventListener
    {
        Eplan.EplApi.ApplicationFramework.EventHandler onUserPreCloseProject =
            new Eplan.EplApi.ApplicationFramework.EventHandler();
        Eplan.EplApi.ApplicationFramework.EventHandler onPostOpenProject =
            new Eplan.EplApi.ApplicationFramework.EventHandler();

        Eplan.EplApi.ApplicationFramework.EventHandler onMainEnd =
            new Eplan.EplApi.ApplicationFramework.EventHandler();
        Eplan.EplApi.ApplicationFramework.EventHandler onMainStart =
            new Eplan.EplApi.ApplicationFramework.EventHandler();

        Eplan.EplApi.ApplicationFramework.EventHandler onNotifyPageChanged =
            new Eplan.EplApi.ApplicationFramework.EventHandler();

        public EplanEventListener()
        {
            var useTimeModule = ProjectManager.GetInstance().GetAppSetting("UseIdleTimeModule");
            if (!bool.TryParse(useTimeModule, out UseIdleTimeModule))
            {
                UseIdleTimeModule= true;
            }

            onUserPreCloseProject
                .SetEvent("Eplan.EplApi.OnUserPreCloseProject");
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

            if (UseIdleTimeModule)
            {
                IEplanHelper eplanHelper = new EplanHelper();
                IModuleConfiguration moduleConfiguration =
                    new ModuleConfiguration();
                IRunningProcess runningProcess =
                    new RunningProcess(Process.GetCurrentProcess());
                idleTimeModule = new IdleTimeModule.IdleTimeModule(
                    eplanHelper, moduleConfiguration, runningProcess);
            }
        }

        private void OnNotifyPageChanged(IEventParameter eventParameter)
        {
            SelectInteractionWhileEditModes interaction = EProjectManager
                .GetInstance().GetEditInteraction();
            if (interaction != null && interaction.IsFinish == false)
            {
                EProjectManager.GetInstance().StartEditModesWithDelay();
            }
        }

        private void OnUserPreCloseProject(IEventParameter iEventParameter)
        {
            string projectName =
                (new EventParameterString(iEventParameter)).String;
            int dot_pos = projectName.LastIndexOf('\\');
            if (dot_pos > 0)
            {
                projectName = projectName.Substring(dot_pos + 1);
            }

            projectName = projectName.Remove(projectName.IndexOf('.'));

            string currentProjectName = EProjectManager.GetInstance()
                .GetCurrentProjectName();
            if (projectName == currentProjectName)
            {
                EProjectManager.GetInstance().SaveAndClose();

                DFrm.GetInstance().ShowNoDevices();

                EProjectManager.GetInstance().ResetCurrentPrj();
                EProjectManager.isPreCloseProjectComplete = true;
            }
        }

        private void OnPostOpenProject(IEventParameter iEventParameter)
        {
            // Если нет активного проекта, считываем описание и работаем с ним.
            if (EProjectManager.GetInstance().GetCurrentPrj() == null)
            {
                // Получение текущего проекта.
                var selectionSet = new Eplan.EplApi.HEServices.SelectionSet();
                selectionSet.LockSelectionByDefault = false;
                selectionSet.LockProjectByDefault = false;

                EProjectManager.GetInstance()
                    .SetCurrentPrj(selectionSet.GetCurrentProject(true));

                string strAction = "LoadDescriptionAction";
                var oAMnr = new ActionManager();
                Eplan.EplApi.ApplicationFramework.Action oAction =
                    oAMnr.FindAction(strAction);
                var ctx = new ActionCallingContext();

                if (oAction != null)
                {
                    oAction.Execute(ctx);
                }

                strAction = "ShowTechObjectsAction";
                oAction = oAMnr.FindAction(strAction);
                if (oAction != null)
                {
                    // Восстановление при необходимости окна редактора.
                    string path = Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData) +
                        @"\Eplan\eplan.cfg";
                    var iniFile = new PInvoke.IniFile(path);
                    string res = iniFile
                        .ReadString("main", "show_obj_window", "false");
                    if (res == "true")
                    {
                        oAction.Execute(ctx);
                    }
                }

                strAction = "ShowDevicesAction";
                oAction = oAMnr.FindAction(strAction);
                if (oAction != null)
                {
                    // Восстановление при необходимости окна устройств.
                    string path = Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData) +
                        @"\Eplan\eplan.cfg";
                    var iniFile = new PInvoke.IniFile(path);
                    string res = iniFile
                        .ReadString("main", "show_dev_window", "false");
                    if (res == "true")
                    {
                        oAction.Execute(ctx);
                    }
                }

                strAction = "ShowOperationsAction";
                oAction = oAMnr.FindAction(strAction);
                if (oAction != null)
                {
                    // Восстановление при необходимости окна операций.
                    string path = Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData) +
                        @"\Eplan\eplan.cfg";
                    var iniFile = new PInvoke.IniFile(path);
                    string res = iniFile
                        .ReadString("main", "show_oper_window", "false");
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

                // Проверяю, закрыт ли проект(выполнено ли PreCloseProject)
                // или же он закрыт через закрытие окна Eplan
                if (EProjectManager.isPreCloseProjectComplete == false)
                {
                    // Если проект закрыт через крестик
                    // при новом открытии Eplan не открывать окна
                    DFrm.SaveCfg(false);
                    ModeFrm.SaveCfg(false);
                    Editor.NewEditorControl.SaveCfg(false);
                }
            }

            if (UseIdleTimeModule)
            {
                idleTimeModule.Stop();
            }
        }

        private void OnMainStart(IEventParameter iEventParameter)
        {
            if (UseIdleTimeModule)
            {
                idleTimeModule.BeforeClosingProject +=
                    EProjectManager.GetInstance().SyncAndSave;
                idleTimeModule.Start(AddInModule.OriginalAssemblyPath);
            }
        }

        private IIdleTimeModule idleTimeModule;
        private readonly bool UseIdleTimeModule;
    }
}
