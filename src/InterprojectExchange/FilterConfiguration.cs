using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PInvoke;
using System.Windows.Forms;

namespace EasyEPlanner
{
    /// <summary>
    /// Конфигурация фильтра сигналов и устройств в межпроектном обмене
    /// </summary>
    sealed public class FilterConfiguration
    {
        public void Save() 
        {
            var iniFile = new IniFile(pathToConfig);
            foreach(var section in filterParameters.Keys)
            {
                foreach(var key in filterParameters[section].Keys)
                {
                    bool value = filterParameters[section][key];
                    iniFile.WriteString(section, key, value.ToString());
                }
            }
        }

        public void Read() 
        {
            if (File.Exists(pathToConfig))
            {
                var newFilterParameters = 
                    new Dictionary<string, Dictionary<string, bool>>();

                var iniFile = new IniFile(pathToConfig);
                foreach(var section in filterParameters.Keys)
                {
                    var itemParameters = new Dictionary<string, bool>();

                    var parameters = filterParameters[section];
                    foreach (var keyValuePair in parameters)
                    {
                        string readValue = iniFile
                            .ReadString(section, keyValuePair.Key, "false");
                        bool.TryParse(readValue, out bool isEnabled);

                        itemParameters.Add(keyValuePair.Key, isEnabled);
                    }

                    newFilterParameters.Add(section, itemParameters);
                }

                filterParameters = newFilterParameters;
            }
            else
            {
                MessageBox.Show("Не найден файл конфигурации для фильтра",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Accept() 
        {
            //TODO: Accept changes and invoke event
        }

        /// <summary>
        /// Получить список устройств для фильтрации.
        /// </summary>
        /// <returns></returns>
        public List<string> GetDevicesList()
        {
            var devices = new List<string>();
            var types = Enum.GetValues(typeof(Device.DeviceType));
            foreach (var type in types)
            {
                if (type.ToString() != "NONE")
                {
                    devices.Add(type.ToString());
                }
            }
            return devices;
        }

        /// <summary>
        /// Экземпляр класса. Singleton
        /// </summary>
        /// <returns></returns>
        public static FilterConfiguration GetInstance()
        {
            if (filterConfiguration == null)
            {
                filterConfiguration = new FilterConfiguration();
            }

            return filterConfiguration;
        }

        /// <summary>
        /// Первоначальная настройка фильтра
        /// </summary>
        private void SetUpFilterParameters()
        {
            filterParameters = new Dictionary<string, Dictionary<string, bool>>();

            var currProjDevList = new Dictionary<string, bool>();
            var advProjDevList = new Dictionary<string, bool>();
            var bindedGridGroupBox = new Dictionary<string, bool>();

            foreach(var dev in GetDevicesList())
            {
                currProjDevList.Add(dev, false);
                advProjDevList.Add(dev, false);
            }

            bindedGridGroupBox.Add("groupAsPairsCheckBox", false);

            filterParameters.Add("currProjDevList", currProjDevList);
            filterParameters.Add("advProjDevList", advProjDevList);
            filterParameters.Add("bindedGridGroupBox", bindedGridGroupBox);
        }

        /// <summary>
        /// Параметры фильтра
        /// </summary>
        public Dictionary<string, Dictionary<string, bool>> FilterParameters
        {
            get
            {
                return filterParameters;
            }
        }

        private FilterConfiguration()
        {
            string pathToDir = ProjectManager.GetInstance()
                .OriginalAssemblyPath;
            pathToConfig = Path.Combine(pathToDir, 
                StaticHelper.CommonConst.ConfigFileName);
            SetUpFilterParameters();
        }

        private static FilterConfiguration filterConfiguration;
        private string pathToConfig;
        private Dictionary<string, Dictionary<string, bool>> filterParameters;
    }
}
