using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EasyEPlanner;

namespace InterprojectExchange
{
    /// <summary>
    /// Межконтроллерный обмен сигналами. Обмен с формами
    /// </summary>
    public class InterprojectExchange : IInterprojectExchange
    {
        protected InterprojectExchange()
        {
            interprojectExchangeModels = new List<IProjectModel>();
            if (eProjectManager is null)
            {
                try
                {
                    eProjectManager = EProjectManager.GetInstance();
                }
                catch
                {
                    // skip EProjectManager int TESTS
                }
            }

        }

        public void Clear()
        {
            interprojectExchangeStarter = null;
            interprojectExchangeModels.Clear();
        }

        public void AddModel(IProjectModel model)
        {
            interprojectExchangeModels.Add(model);
        }

        public bool LoadProjectData(string pathToProjectDir, out string errors)
        {
            // Генерация пути к папке с проектами и его имени из полного пути
            string[] splittedPath = pathToProjectDir.Split('\\');
            int lastElem = splittedPath.Length - 1;
            string projName = splittedPath[lastElem];
            string pathToProjectsDir = pathToProjectDir.Replace(projName, "");
            bool loaded = Owner.LoadProjectData(pathToProjectsDir, projName,
                out errors);
            return loaded;
        }

        public bool CheckPathToProjectFiles(string path)
        {
            return Owner.CheckProjectData(path);
        }

        public IProjectModel GetModel(string projName)
        {
            IProjectModel model = interprojectExchangeModels
                .Where(x => x.ProjectName == projName)
                .FirstOrDefault();
            return model;
        }

        public void SelectModel(IProjectModel selectingModel)
        {
            var currentProjectModel = MainModel;

            foreach (var model in Models)
            {
                if (model.ProjectName == selectingModel.ProjectName)
                {
                    model.Selected = true;
                    currentProjectModel.SelectedAdvancedProject = model
                        .ProjectName;
                }
                else
                {
                    model.Selected = false;
                }
            }
        }

        public void DeleteExchangeWithProject(string projectName)
        {
            IProjectModel model = GetModel(projectName);
            if (model != null)
            {
                model.MarkedForDelete = true;
            }
            else
            {
                throw new Exception("Ошибка при удалении связи с проектом");
            }
        }

        public void ChangeEditMode(int selectedModeIndex)
        {
            editMode = (EditMode)selectedModeIndex;
        }

        public Dictionary<string, List<string[]>> GetBindedSignals()
        {
            var signals = new Dictionary<string, List<string[]>>();

            foreach (var channelName in DeviceChannelsNames)
            {
                List<string[]> channelSignals = GetSignalsPairs(
                    GetCurrentProjectSignals(channelName),
                    GetAdvancedProjectSignals(channelName));
                signals.Add(channelName, channelSignals);
            }

            return signals;
        }

        public string CheckBindingSignals()
        {
            var err = new StringBuilder();
            var mainModel = MainModel;

            foreach (var model in Models.Where(m => m.Loaded && m != MainModel))
            {
                mainModel.SelectedAdvancedProject = model.ProjectName;

                string receiverErr = mainModel.ReceiverSignals.CountCompare(model.SourceSignals);
                if (!string.IsNullOrEmpty(receiverErr))
                {
                    err.Append($"remote_gateways: {model.ProjectName} - {receiverErr}\n");
                    model.Loaded = false;
                }

                string sourceErr = mainModel.SourceSignals.CountCompare(model.ReceiverSignals);
                if (!string.IsNullOrEmpty(sourceErr))
                {
                    err.Append($"shared_devices: {model.ProjectName} - {sourceErr}\n");
                    model.Loaded = false;
                }
            }

            return err.ToString();
        }

        /// <summary>
        /// Получить связанные сигналы
        /// </summary>
        /// <param name="currentProjectSignals">Список сигналов текущего проекта
        /// </param>
        /// <param name="advancedProjectSignals">Список сигналов альтернативного
        /// проекта</param>
        /// <returns></returns>
        private static List<string[]> GetSignalsPairs(
            List<string> currentProjectSignals,
            List<string> advancedProjectSignals)
        {
            if (currentProjectSignals.Count > 0 &&
                advancedProjectSignals.Count > 0)
            {
                return (from cps in currentProjectSignals
                        join aps in advancedProjectSignals
                        on currentProjectSignals.IndexOf(cps) equals advancedProjectSignals.IndexOf(aps)
                        select new[] { cps, aps }).ToList();
            }
            else
            {
                return new List<string[]>();
            }
        }

        public bool BindSignals(string signalType, string currentProjectDevice,
            string advancedProjectDevice)
        {
            List<string> currentProjSignals = GetCurrentProjectSignals(
                signalType);
            List<string> advancedProjSignals = GetAdvancedProjectSignals(
                signalType);

            if (currentProjSignals.Contains(currentProjectDevice) ||
                advancedProjSignals.Contains(advancedProjectDevice))
            {
                return false;
            }

            currentProjSignals.Add(currentProjectDevice);
            advancedProjSignals.Add(advancedProjectDevice);

            return true;
        }

        public bool DeleteSignalsBind(string signalType,
            string currentProjectDevice, string advancedProjectDevice)
        {
            List<string> currentProjSignals = GetCurrentProjectSignals(
                signalType);
            List<string> advancedProjSignals = GetAdvancedProjectSignals(
                signalType);

            currentProjSignals.Remove(currentProjectDevice);
            advancedProjSignals.Remove(advancedProjectDevice);

            return true;
        }

        public bool MoveSignalsBind(string signalType, string currProjSignal,
            string advProjSignal, int move)
        {
            List<string> currentProjSignals = GetCurrentProjectSignals(
                signalType);
            List<string> advancedProjSignals = GetAdvancedProjectSignals(
                signalType);

            int currSignalIndex = currentProjSignals.IndexOf(currProjSignal);
            int advSignalindex = advancedProjSignals.IndexOf(advProjSignal);

            bool blockMoveUp =
                (currSignalIndex == 0 || advSignalindex == 0) &&
                move == -1;
            bool blockMoveDown =
                (currSignalIndex == currentProjSignals.Count - 1 ||
                advSignalindex == advancedProjSignals.Count - 1) &&
                move == 1;
            if (blockMoveDown || blockMoveUp)
            {
                return false;
            }

            currentProjSignals.Remove(currProjSignal);
            currentProjSignals.Insert(currSignalIndex + move, currProjSignal);

            advancedProjSignals.Remove(advProjSignal);
            advancedProjSignals.Insert(advSignalindex + move, advProjSignal);

            return true;
        }

        public bool UpdateProjectBinding(string signalType,
            string oldValue, string newValue, bool mainProject,
            out bool needSwap)
        {
            needSwap = false;

            if (oldValue == newValue)
            {
                return false;
            }

            List<string> signals = mainProject ?
                GetCurrentProjectSignals(signalType) :
                GetAdvancedProjectSignals(signalType);

            void RemoveAndInsert<T>(List<T> collection, int index, T value)
            {
                collection.RemoveAt(index);
                collection.Insert(index, value);
            }

            int oldValueIndex = signals.IndexOf(oldValue);
            int newValueIndex = signals.IndexOf(newValue);
            if (newValueIndex >= 0)
            {
                RemoveAndInsert(signals, oldValueIndex, newValue);
                RemoveAndInsert(signals, newValueIndex, oldValue);

                needSwap = true;
            }
            else
            {
                RemoveAndInsert(signals, oldValueIndex, newValue);
            }

            return true;
        }

        /// <summary>
        /// Получить сигналы текущего проекта
        /// </summary>
        /// <param name="signalType">Тип сигнала</param>
        /// <returns></returns>
        private List<string> GetCurrentProjectSignals(string signalType)
        {
            IProjectModel currentProjectModel = MainModel;
            DeviceSignalsInfo currentProjectSignals;
            if (editMode == EditMode.SourceReciever)
            {
                currentProjectSignals = currentProjectModel.SourceSignals;
            }
            else
            {
                currentProjectSignals = currentProjectModel.ReceiverSignals;
            }

            switch (signalType)
            {
                case "AI":
                    return currentProjectSignals.AI;

                case "AO":
                    return currentProjectSignals.AO;

                case "DI":
                    return currentProjectSignals.DI;

                case "DO":
                    return currentProjectSignals.DO;
            }

            return new List<string>();
        }

        /// <summary>
        /// Получить сигналы альтернативного проекта
        /// </summary>
        /// <param name="signalType">Тип сигнала</param>
        /// <returns></returns>
        private List<string> GetAdvancedProjectSignals(string signalType)
        {
            IProjectModel advancedProjectModel = SelectedModel;
            DeviceSignalsInfo advancedProjectSignals;
            if (editMode == EditMode.SourceReciever)
            {
                advancedProjectSignals = advancedProjectModel.ReceiverSignals;
            }
            else
            {
                advancedProjectSignals = advancedProjectModel.SourceSignals;
            }

            switch (signalType)
            {
                case "AI":
                    return advancedProjectSignals.AI;

                case "AO":
                    return advancedProjectSignals.AO;

                case "DI":
                    return advancedProjectSignals.DI;

                case "DO":
                    return advancedProjectSignals.DO;
            }

            return new List<string>();
        }

        public void Save()
        {
            Owner.Save();
        }

        public bool RestoreModel(string projectName)
        {
            var canRestore = false;

            foreach (var model in Models)
            {
                bool foundMarkedModel =
                    projectName == model.ProjectName && model.MarkedForDelete;
                if (foundMarkedModel)
                {
                    canRestore = true;
                    model.MarkedForDelete = false;
                }
            }

            return canRestore;
        }

        public string[] LoadedAdvancedModelNames
        {
            get
            {
                return interprojectExchange.Models
                    .Where(x => x.ProjectName != interprojectExchange
                    .MainProjectName &&
                    x.MarkedForDelete == false)
                    .Select(x => x.ProjectName)
                    .ToArray();
            }
        }

        public virtual ICurrentProjectModel MainModel
        {
            get
            {
                return GetModel(MainProjectName) as ICurrentProjectModel;
            }
        }

        public string MainProjectName => eProjectManager.GetModifyingCurrentProjectName();

        public string DefaultPathWithProjects
        {
            get
            {
                return ProjectManager.GetInstance().GetPtusaProjectsPath("");
            }
        }

        public InterprojectExchangeStarter Owner
        {
            get
            {
                return interprojectExchangeStarter;
            }
            set
            {
                interprojectExchangeStarter = value;
            }
        }

        public List<IProjectModel> Models
        {
            get
            {
                return interprojectExchangeModels;
            }
        }

        public IProjectModel SelectedModel
        {
            get
            {
                return Models.Where(x => x.Selected == true).FirstOrDefault();
            }
        }

        public string[] DeviceChannelsNames
        {
            get
            {
                return new string[] { "AI", "AO", "DI", "DO" };
            }
        }

        public EditMode EditMode => editMode;

        /// <summary>
        /// Получить экземпляр класса. Singleton
        /// </summary>
        /// <returns></returns>
        public static IInterprojectExchange GetInstance()
        {
            if (interprojectExchange == null)
            {
                interprojectExchange = new InterprojectExchange();
            }

            return interprojectExchange;
        }

        private static IEProjectManager eProjectManager = null;
        private EditMode editMode;
        private InterprojectExchangeStarter interprojectExchangeStarter;
        private static InterprojectExchange interprojectExchange;
        private List<IProjectModel> interprojectExchangeModels;
    }

    /// <summary>
    /// Режим редактирования связей
    /// </summary>
    public enum EditMode
    {
        SourceReciever, // Источник >> Приемник
        RecieverSource  // Приемник >> Источник
    }
}