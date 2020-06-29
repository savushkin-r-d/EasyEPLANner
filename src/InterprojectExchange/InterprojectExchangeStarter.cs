using LuaInterface;
using System.IO;
using System.Text;
using EasyEPlanner;
using System.Linq;

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

            InitLuaInstance();
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
            bool isReadSignals = UpdateDevices();
            bool isLoadData = LoadCurrentInterprojectExchange(isReadSignals);
            ShowForm(isLoadData);
        }

        /// <summary>
        /// Обновление текущих устройств.
        /// </summary>
        private bool UpdateDevices()
        {
            bool saveDescrSilentMode = true;
            EProjectManager.GetInstance().SyncAndSave(saveDescrSilentMode);
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

            string projName = GetMainProjectName();
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
            var model = interprojectExchange.Models
                .Where(x => x.Selected == true).FirstOrDefault();
            return model;
        }

        /// <summary>
        ///  Получить имя главного (открытого в Eplan) проекта.
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
        /// <param name="pathToProjectDir">Путь к папке с проектами</param>
        /// <param name="projName">Имя проекта</param>
        /// <returns></returns>
        public bool LoadProjectData(string pathToProjectDir, 
            string projName = "")
        {
            InitLuaInstance();
            LoadScripts();
            LoadMainIOData(pathToProjectDir, projName);
            LoadDevicesFile(pathToProjectDir, projName);
            LoadCurrentProjectSharedLuaData(pathToProjectDir, projName);
            return true;
        }

        /// <summary>
        /// Загрузить данные проекта
        /// </summary>
        /// <param name="pathToProjectDir">Путь к папке с проектами</param>
        /// <param name="projName">Имя проекта</param>
        /// <returns></returns>
        public void LoadProjectsData(string pathToProjectDir, 
            string projName = "")
        {
            InitLuaInstance();
            LoadScripts();
            LoadMainIOData(pathToProjectDir, projName);
            LoadDevicesFile(pathToProjectDir, projName);
            LoadCurrentProjectSharedLuaData(pathToProjectDir, projName);

            foreach (var model in interprojectExchange.Models)
            {
                if (model.ProjectName != projName)
                {
                    InitLuaInstance();
                    LoadScripts();
                    model.Selected = true;
                    LoadMainIOData(pathToProjectDir, model.ProjectName);
                    LoadDevicesFile(pathToProjectDir, projName);
                    LoadAdvancedProjectSharedLuaData(pathToProjectDir,
                        model.ProjectName);
                    model.Selected = false;
                }
            }
        }

        /// <summary>
        /// Загрузить основные скрипты, нужные для чтения LUA-файлов.
        /// </summary>
        private void LoadScripts()
        {
            // Загрузка скрипта для чтения shared.lua
            string pathToSharedLua = Path.Combine(ProjectManager
                .GetInstance().SystemFilesPath, sharedLuaIntializerFile);
            var reader = new StreamReader(pathToSharedLua,
                Encoding.GetEncoding(1251));
            string scriptForReadingSharedFile = reader.ReadToEnd();
            reader.Close();
            lua.DoString(scriptForReadingSharedFile);

            // Загрузка скрипта для чтения main.io.lua
            string pathToInterprojectIOLua = Path.Combine(ProjectManager
                    .GetInstance().SystemFilesPath, devicesAndPLCInitializer);
            reader = new StreamReader(pathToInterprojectIOLua,
                Encoding.GetEncoding(1251));
            string scriptForMainIO = reader.ReadToEnd();
            reader.Close();
            lua.DoString(scriptForMainIO);

            // Загрузка скрипта с mock функциями для устройств
            string pathToDevicesMockFile = Path.Combine(ProjectManager
                    .GetInstance().SystemFilesPath, scriptWithDevicesMock);
            reader = new StreamReader(pathToDevicesMockFile,
                Encoding.GetEncoding(1251));
            string scriptWithMock = reader.ReadToEnd();
            reader.Close();
            lua.DoString(scriptWithMock);
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
                fileWithDevicesAndPLC);
            if (File.Exists(pathToIOFile))
            {
                var reader = new StreamReader(pathToIOFile,
                    Encoding.GetEncoding(1251));
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
        /// Загрузка devices.lua файла проекта
        /// </summary>
        /// <param name="pathToProjectsDir"></param>
        /// <param name="projName"></param>
        private void LoadDevicesFile(string pathToProjectsDir,
            string projName)
        {
            string pathToDevices = Path.Combine(pathToProjectsDir, projName,
                devicesDescriptionFile);
            if (File.Exists(pathToDevices))
            {
                var reader = new StreamReader(pathToDevices,
                    Encoding.GetEncoding(1251));
                string devicesInfo = reader.ReadToEnd();
                reader.Close();
                lua.DoString(devicesInfo);
                // Функция из Lua
                lua.DoString("system.init_dev_names()");
            }
            else
            {
                form.ShowErrorMessage($"Не найден файл main.devices.lua " +
                    $"проекта \"{projName}\"");
            }
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
            string pathToSharedfile = Path.Combine(pathToProjectsDir, projName,
                fileWithSignals);
            if (File.Exists(pathToSharedfile))
            {
                var reader = new StreamReader(pathToSharedfile,
                    Encoding.GetEncoding(1251));
                string sharedInfo = reader.ReadToEnd();
                reader.Close();
                lua.DoString(sharedInfo);
                // Функция из Lua
                lua.DoString("init_current_project_shared_lua()");
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
            string pathToSharedfile = Path.Combine(pathToProjectsDir, projName,
                fileWithSignals);
            if (File.Exists(pathToSharedfile))
            {
                var reader = new StreamReader(pathToSharedfile,
                    Encoding.GetEncoding(1251));
                string sharedInfo = reader.ReadToEnd();
                reader.Close();
                lua.DoString(sharedInfo);
                // Функция из Lua
                lua.DoString("init_advanced_project_shared_lua()");
            }
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
                fileWithDevicesAndPLC);
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

        private string fileWithDevicesAndPLC = "main.io.lua";
        private string fileWithSignals = "shared.lua";
        private string scriptWithDevicesMock = "sys_devices_mock_generator.lua";
        private string devicesDescriptionFile = "main.devices.lua";
        private string sharedLuaIntializerFile = "sys_shared_initializer.lua";
        private string devicesAndPLCInitializer = "sys_interproject_io.lua";

        private InterprojectExchangeForm form;
        private InterprojectExchange interprojectExchange;

        Lua lua;
    }
}
