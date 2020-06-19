using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            lua = new Lua();
            lua.RegisterFunction("CreateModel", this,
                GetType().GetMethod("CreateModel"));
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

            string projName = EProjectManager.GetInstance()
                .GetModifyingCurrentProjectName();
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
        /// Загрузить данные проекта
        /// </summary>
        /// <param name="pathToProjectDir">Путь к папке с проектами</param>
        /// <param name="projName">Имя проекта</param>
        /// <returns></returns>
        public bool LoadProjectData(string pathToProjectDir, 
            string projName = "")
        {
            bool res = false;
            string pathToIOFile = Path.Combine(pathToProjectDir, projName, 
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

                res = true;
            }
            else
            {
                form.ShowErrorMessage($"Не найден файл main.io.lua проекта" +
                    $" \"{projName}\"");
                res = false;
            }

            //shared.lua read if exist (if model.SharedLuaReaded = false)

            return res;
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
        //private string fileWithSignals = "shared.lua";

        private InterprojectExchangeForm form;
        private InterprojectExchange interprojectExchange;

        Lua lua;
    }
}
