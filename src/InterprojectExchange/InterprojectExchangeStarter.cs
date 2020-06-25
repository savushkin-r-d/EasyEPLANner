using LuaInterface;
using System.IO;
using System.Text;
using EasyEPlanner;

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

            // Настройка lua
            lua = new Lua();
            lua.RegisterFunction("CreateModel", this,
                GetType().GetMethod("CreateModel"));
            lua.RegisterFunction("GetModel", this, 
                GetType().GetMethod("GetModel"));
            lua.RegisterFunction("GetMainProjectName", this,
                GetType().GetMethod("GetMainProjectName"));
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
            LoadProjectData(pathToProjectDir, projName);
            return true;
        }

        /// <summary>
        /// Создать модель, вызывается из Lua
        /// </summary>
        /// <returns></returns>
        public InterprojectExchangeModel CreateModel()
        {
            var model = new InterprojectExchangeModel();
            interprojectExchange.AddModel(model);
            return model;
        }

        /// <summary>
        /// Получить модель, вызывается из Lua
        /// </summary>
        /// <param name="projectName">Имя проекта</param>
        /// <returns></returns>
        public InterprojectExchangeModel GetModel(string projectName)
        {
            var model = interprojectExchange.GetModel(projectName);
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
        /// Загрузить данные проекта
        /// </summary>
        /// <param name="pathToProjectDir">Путь к папке с проектами</param>
        /// <param name="projName">Имя проекта</param>
        /// <returns></returns>
        public bool LoadProjectData(string pathToProjectDir, 
            string projName = "")
        {
            LoadMainIOData(pathToProjectDir, projName);
            LoadCurrentProjectSharedLuaData(pathToProjectDir, projName);

            foreach(var model in interprojectExchange.Models)
            {
                if (model.ProjectName != projName)
                {
                    LoadMainIOData(pathToProjectDir, model.ProjectName);
                    LoadAdvancedProjectSharedLuaData(pathToProjectDir, 
                        model.ProjectName);
                }
            }

            return true;
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
                fileWithDeviceAndPLC);
            if (File.Exists(pathToIOFile))
            {
                var reader = new StreamReader(pathToIOFile,
                    Encoding.GetEncoding(1251));
                string ioInfo = reader.ReadToEnd();
                reader.Close();
                lua.DoString(ioInfo);

                string pathToScripts = Path.Combine(ProjectManager
                    .GetInstance().SystemFilesPath, "sys_interproject_io.lua");
                reader = new StreamReader(pathToScripts,
                    Encoding.GetEncoding(1251));
                string mainIOData = reader.ReadToEnd();
                reader.Close();
                lua.DoString(mainIOData);
                lua.DoString("init_io_file()");
            }
            else
            {
                form.ShowErrorMessage($"Не найден файл main.io.lua проекта" +
                    $" \"{projName}\"");
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

                string pathToScripts = Path.Combine(ProjectManager
                   .GetInstance().SystemFilesPath, 
                   "sys_currentProject_shared_initializer.lua");
                reader = new StreamReader(pathToScripts,
                    Encoding.GetEncoding(1251));
                string scriptForReadingSharedFile = reader.ReadToEnd();
                reader.Close();
                lua.DoString(scriptForReadingSharedFile);
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

                string pathToScripts = Path.Combine(ProjectManager
                   .GetInstance().SystemFilesPath,
                   "sys_advancedProject_shared_initializer.lua");
                reader = new StreamReader(pathToScripts,
                    Encoding.GetEncoding(1251));
                string scriptForReadingSharedFile = reader.ReadToEnd();
                reader.Close();
                lua.DoString(scriptForReadingSharedFile);
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
                fileWithDeviceAndPLC);
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
        /// Экземпляр пространства Lua модели
        /// </summary>
        public Lua Lua
        {
            get
            {
                return lua;
            }
        }

        private string fileWithDeviceAndPLC = "main.io.lua";
        private string fileWithSignals = "shared.lua";

        private InterprojectExchangeForm form;
        private InterprojectExchange interprojectExchange;

        Lua lua;
    }
}
