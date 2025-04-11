using Eplan.EplApi.ApplicationFramework;
using System;
using IdleTimeModule;
using IdleTimeModule.EplanAPIHelper;
using System.Diagnostics;
using IO.View;
using System.Diagnostics.CodeAnalysis;
using EasyEPlanner.Main;

namespace EasyEPlanner
{
    [ExcludeFromCodeCoverage]
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

            IEplanHelper eplanHelper = new EplanHelper();
            IModuleConfiguration moduleConfiguration =
                new ModuleConfiguration();
            IRunningProcess runningProcess =
                new RunningProcess(Process.GetCurrentProcess());
            idleTimeModule = new IdleTimeModule.IdleTimeModule(
                eplanHelper, moduleConfiguration, runningProcess);
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
                IOViewControl.Instance.Clear();

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

                oAction?.Execute(ctx);

                AttemptRestoreWindow(oAMnr.FindAction(nameof(ShowTechObjectsAction)),
                    ctx, "show_obj_window");

                AttemptRestoreWindow(oAMnr.FindAction(nameof(ShowDevicesAction)),
                    ctx, "show_dev_window");

                AttemptRestoreWindow(oAMnr.FindAction(nameof(ShowOperationsAction)),
                    ctx, "show_oper_window");

                AttemptRestoreWindow(oAMnr.FindAction(nameof(ShowPlcAction)),
                    ctx, IOViewControl.CfgShowWindowKey);

                // Проект открыт, ставим флаг в изначальное состояние.
                EProjectManager.isPreCloseProjectComplete = false;
            }
        }

        /// <summary>
        /// Восстановить окно при необходимости
        /// </summary>
        /// <param name="action">Действие по открытию окна</param>
        /// <param name="ctx">Контекст запуска действия</param>
        /// <param name="cfgWindowKey">Ключ в конфигурации</param>
        private static void AttemptRestoreWindow(
            Eplan.EplApi.ApplicationFramework.Action action,
            ActionCallingContext ctx,
            string cfgWindowKey)
        {
            var iniFile = new PInvoke.IniFile(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                @"\Eplan\eplan.cfg");

            string res = iniFile.ReadString("main", cfgWindowKey, "false")
                .Trim().ToLower();
            
            if (res is "true")
            {
                action.Execute(ctx);
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
                    IOViewControl.SaveCfg(false);
                }
            }

            idleTimeModule.Stop();
        }

        private void OnMainStart(IEventParameter iEventParameter)
        {
            idleTimeModule.BeforeClosingProject += 
                EProjectManager.GetInstance().SyncAndSave;
            idleTimeModule.Start(AddInModule.OriginalAssemblyPath);
        }

        private IIdleTimeModule idleTimeModule;
    }
}
