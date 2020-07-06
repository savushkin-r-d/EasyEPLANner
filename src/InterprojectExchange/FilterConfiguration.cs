using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using PInvoke;
using System.Windows.Forms;
using EasyEPlanner;

namespace InterprojectExchange
{
    /// <summary>
    /// Конфигурация фильтра сигналов и устройств в межпроектном обмене
    /// </summary>
    public class FilterConfiguration
    {
        /// <summary>
        /// Делегат для события изменения фильтра.
        /// </summary>
        public delegate void SignalsListFilterChanged();

        /// <summary>
        /// Событие изменение фильтра.
        /// </summary>
        public event SignalsListFilterChanged SignalsFilterChanged;

        /// <summary>
        /// Сохранить конфигурацию фильтрации в .ini
        /// </summary>
        public void Save() 
        {
            SignalsFilterChanged.Invoke();

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

        /// <summary>
        /// Прочитать конфигурацию фильтрации из .ini
        /// </summary>
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

        /// <summary>
        /// Фильтровать
        /// </summary>
        /// <param name="items">Элементы для фильтра</param>
        /// <param name="filterList">Какой список сортируется</param>
        public ListViewItem[] FilterOut(List<ListViewItem> items, 
            FilterList filterList)
        {
            var filteredList = new List<ListViewItem>();
            string[] allowedDevices = new string[0];
            if (filterList == FilterList.Current)
            {
                allowedDevices = FilterParameters["currProjDevList"]
                    .Where(x => x.Value == true)
                    .Select(x => x.Key)
                    .ToArray();
            }
            else
            {
                allowedDevices = FilterParameters["advProjDevList"]
                    .Where(x => x.Value == true)
                    .Select(x => x.Key)
                    .ToArray();
            }

            if(allowedDevices.Length != 0)
            {
                filteredList = items
                    .Where(x => allowedDevices.Contains(x.Tag.ToString()))
                    .ToList();
                return filteredList.ToArray();
            }
            else
            {
                return items.ToArray();
            }
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
            var bindedSignalsList = new Dictionary<string, bool>();

            foreach(var dev in GetDevicesList())
            {
                currProjDevList.Add(dev, false);
                advProjDevList.Add(dev, false);
            }

            bindedSignalsList.Add("groupAsPairsCheckBox", false);

            filterParameters.Add("currProjDevList", currProjDevList);
            filterParameters.Add("advProjDevList", advProjDevList);
            filterParameters.Add("bindedSignalsList", bindedSignalsList);
        }

        /// <summary>
        /// Отобразить форму
        /// </summary>
        public void ShowForm()
        {
            if (filterForm == null || filterForm.IsDisposed)
            {
                Read();
                filterForm = new FilterForm();
            }

            filterForm.Show();
        }

        /// <summary>
        /// Закрыть форму и освободить ресурсы
        /// </summary>
        public void Dispose()
        {
            if(filterForm != null && !filterForm.IsDisposed)
            {
                filterForm.Close();
                filterForm.Dispose();
            }
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

        /// <summary>
        /// Закрытый конструктор
        /// </summary>
        private FilterConfiguration()
        {
            string pathToDir = ProjectManager.GetInstance()
                .OriginalAssemblyPath;
            pathToConfig = Path.Combine(pathToDir, 
                StaticHelper.CommonConst.ConfigFileName);
            SetUpFilterParameters();
            Read();
        }

        /// <summary>
        /// Какой список фильтровать
        /// </summary>
        public enum FilterList
        {
            Current,
            Advanced,
        }

        private static FilterConfiguration filterConfiguration;
        private string pathToConfig;
        private FilterForm filterForm;
        private Dictionary<string, Dictionary<string, bool>> filterParameters;
    }
}
