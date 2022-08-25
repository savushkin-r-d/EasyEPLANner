using LuaInterface;
using System.IO;
using System.Text;
using EasyEPlanner;
using System.Linq;
using System.Windows.Forms;
using System;

namespace InterprojectExchange
{
    /// <summary>
    /// Межконтроллерный обмен сигналами. Стартовый класс
    /// </summary>
    public class InterprojectExchangeStarter
    {
        public InterprojectExchangeStarter()
        {
            interprojectExchange = InterprojectExchange.GetInstance();
            interprojectExchange.Clear();
        }

        /// <summary>
        /// Инициализация экземпляра Lua
        /// </summary>
        private void InitLuaInstance()
        {
            lua = new Lua();
            lua.RegisterFunction("CreateModel", this,
                GetType().GetMethod("CreateModel"));
            lua.RegisterFunction("CreateMainModel", this,
                GetType().GetMethod("CreateMainModel"));
            lua.RegisterFunction("GetModel", this,
                GetType().GetMethod("GetModel"));
            lua.RegisterFunction("GetMainProjectName", this,
                GetType().GetMethod("GetMainProjectName"));
            lua.RegisterFunction("GetSelectedModel", this,
                GetType().GetMethod("GetSelectedModel"));
        }

        /// <summary>
        /// Начало настройки межконтроллерного обмена.
        /// </summary>
        public void Start()
        {
            interprojectExchange.Owner = this;
            form = new InterprojectExchangeForm();
            bool isReadSignals = UpdateDevices();
            bool isLoadData = LoadCurrentInterprojectExchange(isReadSignals);
            ShowForm(isLoadData);
        }

        /// <summary>
        /// Обновление текущих устройств.
        /// </summary>
        private bool UpdateDevices()
        {
            EProjectManager.GetInstance().SyncAndSave();
            return true;
        }

        /// <summary>
        /// Загрузить текущие данные по межпроектному обмену сигналами.
        /// </summary>
        /// <param name="isReadDevices">Прочитаны ли устройства текущие</param>
        private bool LoadCurrentInterprojectExchange(bool isReadDevices)
        {
            if (isReadDevices == false)
            {
                return false;
            }

            string projName = interprojectExchange.MainProjectName;
            string pathToProjectDir = ProjectManager.GetInstance()
                .GetPtusaProjectsPath(projName);
            LoadProjectsData(pathToProjectDir, projName);
            return true;
        }

        /// <summary>
        /// Создать модель, вызывается из Lua
        /// </summary>
        /// <returns></returns>
        public IProjectModel CreateModel()
        {
            IProjectModel model = new AdvancedProjectModel();
            interprojectExchange.AddModel(model);
            return model;
        }

        /// <summary>
        /// Создать главную модель, вызывается из Lua
        /// </summary>
        /// <returns></returns>
        public IProjectModel CreateMainModel()
        {
            IProjectModel model = new CurrentProjectModel();
            interprojectExchange.AddModel(model);
            return model;
        }

        /// <summary>
        /// Получить модель, вызывается из Lua
        /// </summary>
        /// <param name="projectName">Имя проекта</param>
        /// <returns></returns>
        public IProjectModel GetModel(string projectName)
        {
            var model = interprojectExchange.GetModel(projectName);
            return model;
        }

        /// <summary>
        /// Получить выбранную модель
        /// </summary>
        /// <returns></returns>
        public IProjectModel GetSelectedModel()
        {
            var model = interprojectExchange.SelectedModel;
            return model;
        }

        /// <summary>
        ///  Получить имя главного (открытого в Eplan) проекта, вызывается
        ///  из LUA
        /// </summary>
        /// <returns></returns>
        public string GetMainProjectName()
        {
            return EProjectManager.GetInstance()
                .GetModifyingCurrentProjectName();
        }

        /// <summary>
        /// Загрузить данные проекта при загрузке системы
        /// </summary>
        /// <param name="pathToProjectDir">Путь к папке с проектом</param>
        /// <param name="projName">Имя проекта</param>
        /// <param name="errors">Ошибки во время загрузки</param>
        /// <returns>Загружены данные или нет</returns>
        public bool LoadProjectData(string pathToProjectDir, 
            string projName, out string errors)
        {
            InitLuaInstance();
            LoadScripts();
            LoadMainIOData(pathToProjectDir, projName);
            LoadAdvancedProjectSharedLuaData(pathToProjectDir, projName);
            errors = SetIPFromMainModel(projName);
            if (string.IsNullOrEmpty(errors))
            {
                interprojectExchange.GetModel(projName)
                    .PathToProject = pathToProjectDir;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Загрузить данные проекта
        /// </summary>
        /// <param name="pathToMainProject">Путь к папке с главным проектом
        /// </param>
        /// <param name="projName">Имя проекта</param>
        /// <returns></returns>
        public void LoadProjectsData(string pathToMainProject, 
            string projName = "")
        {
            InitLuaInstance();
            LoadScripts();
            LoadMainIOData(pathToMainProject, projName);
            GenerateSharedDevices(projName);
            LoadCurrentProjectSharedLuaData(pathToMainProject, projName);
            interprojectExchange.MainModel.PathToProject = pathToMainProject;

            foreach (var model in interprojectExchange.Models)
            {
                string alternativeProject = model.ProjectName;
                if (alternativeProject != projName)
                {
                    string pathToProject = ProjectManager.GetInstance()
                        .GetPtusaProjectsPath(alternativeProject);
                    InitLuaInstance();
                    LoadScripts();
                    model.Selected = true;
                    LoadMainIOData(pathToProject, alternativeProject);
                    GenerateSharedDevices(alternativeProject);
                    LoadAdvancedProjectSharedLuaData(pathToProject,
                        alternativeProject);
                    model.Selected = false;
                    SetIPFromMainModel(alternativeProject);
                    model.PathToProject = pathToProject;
                }
            }
        }

        /// <summary>
        /// Загрузить основные скрипты, нужные для чтения LUA-файлов.
        /// </summary>
        private void LoadScripts()
        {
            string systemFilesPath = ProjectManager.GetInstance()
                .SystemFilesPath;
            // Загрузка скрипта для чтения shared.lua
            string pathToSharedLua = Path.Combine(systemFilesPath, 
                sharedLuaIntializerFile);
            LoadScript(pathToSharedLua);

            // Загрузка скрипта для чтения main.io.lua
            string pathToInterprojectIOLua = Path.Combine(systemFilesPath, 
                devicesAndPLCInitializer);
            LoadScript(pathToInterprojectIOLua);
        }

        /// <summary>
        /// Загрузить LUA-скрипт из файла
        /// </summary>
        /// <param name="path">Путь к файлу скрипта</param>
        private void LoadScript(string path)
        {
            var reader = new StreamReader(path,
                EncodingDetector.DetectFileEncoding(path));
            string script = reader.ReadToEnd();
            reader.Close();
            lua.DoString(script);
        }

        /// <summary>
        /// Чтение информации о ПЛК из main.io.lua
        /// </summary>
        /// <param name="pathToProjectsDir">Путь к папке с проектами</param>
        /// <param name="projName">Имя проекта</param>
        /// <returns></returns>
        private void LoadMainIOData(string pathToProjectsDir, 
            string projName)
        {
            string pathToIOFile = Path.Combine(pathToProjectsDir, projName,
                devicesAndPLCFile);
            if (File.Exists(pathToIOFile))
            {
                var reader = new StreamReader(pathToIOFile,
                    EncodingDetector.DetectFileEncoding(pathToIOFile));
                string ioInfo = reader.ReadToEnd();
                reader.Close();
                lua.DoString(ioInfo);
                // Функция из Lua
                lua.DoString("init_io_file()");
            }
            else
            {
                form.ShowErrorMessage($"Не найден файл main.io.lua проекта" +
                    $" \"{projName}\"");
            }
        }

        /// <summary>
        /// Инициализирует переменные с названиями стройств,
        /// использующиеся в shared.lua 
        /// </summary>
        /// <param name="projectName">Имя проекта</param>
        private void GenerateSharedDevices(string projectName)
        {
            var devices = interprojectExchange.
                GetModel(projectName).Devices;
            string devicesLua = string.Empty;
            foreach (var device in devices)
            {
                devicesLua += $"{device.Name} = \"{device.Name}\"\n" +
                    $"__{device.Name} = \"{device.Name}\"\n"; 
            }
            lua.DoString(devicesLua);
        }

        /// <summary>
        /// Чтение Shared файла текущего проекта
        /// </summary>
        /// <param name="pathToProjectsDir">Путь к каталогу с проектами</param>
        /// <param name="projName">Имя проекта</param>
        /// <returns></returns>
        private void LoadCurrentProjectSharedLuaData(string pathToProjectsDir,
            string projName)
        {
            string pathToSharedFile = Path.Combine(pathToProjectsDir, projName,
                signalsFile);
            if (File.Exists(pathToSharedFile))
            {
                var reader = new StreamReader(pathToSharedFile,
                    EncodingDetector.DetectFileEncoding(pathToSharedFile));
                string sharedInfo = reader.ReadToEnd();
                reader.Close();
                lua.DoString(sharedInfo);
                // Функция из Lua
                lua.DoString("init_current_project_shared_lua()");
                ReadModelSharedFileToList(projName, pathToSharedFile);
            }
        }

        /// <summary>
        /// Чтение Shared файла альтернативного проекта
        /// </summary>
        /// <param name="pathToProjectsDir">Путь к каталогу с проектами</param>
        /// <param name="projName">Имя проекта</param>
        /// <returns></returns>
        public void LoadAdvancedProjectSharedLuaData(string pathToProjectsDir,
            string projName)
        {
            string pathToSharedFile = Path.Combine(pathToProjectsDir, projName,
                signalsFile);
            if (File.Exists(pathToSharedFile))
            {
                var reader = new StreamReader(pathToSharedFile,
                    EncodingDetector.DetectFileEncoding(pathToSharedFile));
                string sharedInfo = reader.ReadToEnd();
                reader.Close();
                lua.DoString(sharedInfo);
                // Функция из Lua
                lua.DoString("init_advanced_project_shared_lua()");
                ReadModelSharedFileToList(projName, pathToSharedFile);
            }
        }

        /// <summary>
        /// Прочитать shared.lua в список для манипуляций
        /// </summary>
        /// <param name="projName">Имя проекта для поиска модели</param>
        /// <param name="pathToSharedFile">Путь к файлу shared.lua</param>
        private void ReadModelSharedFileToList(string projName, 
            string pathToSharedFile)
        {
            IProjectModel model = interprojectExchange.Models
                .Where(x => x.ProjectName == projName)
                .FirstOrDefault();
            if (model != null)
            {
                model.SharedFileAsStringList = File
                    .ReadAllLines(pathToSharedFile,
                    EncodingDetector.DetectFileEncoding(pathToSharedFile))
                    .ToList();
            }
        }

        /// <summary>
        /// Установка IP-адреса для альтернативных моделей из главной модели
        /// </summary>
        /// <returns>Ошибки</returns>
        private string SetIPFromMainModel(string projName)
        {
            CurrentProjectModel mainModel = interprojectExchange.MainModel;
            string alreadySelectedProject = mainModel.SelectedAdvancedProject;
            mainModel.SelectedAdvancedProject = interprojectExchange
                .MainProjectName;
            IProjectModel model = interprojectExchange.GetModel(projName);
            string errors = string.Empty;
            bool modelNotFound = model == null;
            if (modelNotFound)
            {
                errors += "Модель загружаемого проекта не была создана или" +
                    " создана неправильно. Проверьте файлы проекта. Откройте " +
                    "и закройте связываемый проект.\n";
            }
            else
            {
                model.PacInfo.IP = mainModel.PacInfo.IP;
                mainModel.SelectedAdvancedProject = alreadySelectedProject;
            }

            return errors;
        }

        /// <summary>
        /// Проверка корректности пути к проекту
        /// </summary>
        /// <param name="pathToDir">Путь к папке с файлами проекта</param>
        /// <returns></returns>
        public bool CheckProjectData(string pathToDir)
        {
            bool res = false;
            string fileWithDevicesPath = Path.Combine(pathToDir,
                devicesAndPLCFile);
            if (File.Exists(fileWithDevicesPath))
            {
                res = true;
            }
            return res;
        }

        /// <summary>
        /// Показать форму для работы с межпроектным обменом.
        /// </summary>
        private void ShowForm(bool isLoaded)
        {
            if (isLoaded == false)
            {
                return;
            }

            if (form == null || form.IsDisposed)
            {
                form = new InterprojectExchangeForm();
            }
            form.ShowDialog();
        }

        /// <summary>
        /// Сохранение межконтроллерного обмена
        /// </summary>
        public void Save()
        {
            interprojectExchange.Models
                .RemoveAll(model => string.IsNullOrEmpty(model.ProjectName));
            if (interprojectExchange.Models.Count < 1)
            {
                throw new ApplicationException();
            }
            interprojectExchangeSaver = new InterprojectExchangeSaver(
                interprojectExchange, signalsFile);
            interprojectExchangeSaver.SaveAsync();
        }

        const string devicesAndPLCFile = "main.io.lua";
        const string signalsFile = "shared.lua";
        const string sharedLuaIntializerFile = "sys_shared_initializer.lua";
        const string devicesAndPLCInitializer = "sys_interproject_io.lua";

        private InterprojectExchangeForm form;
        private InterprojectExchange interprojectExchange;
        private InterprojectExchangeSaver interprojectExchangeSaver;

        Lua lua;
    }
}
