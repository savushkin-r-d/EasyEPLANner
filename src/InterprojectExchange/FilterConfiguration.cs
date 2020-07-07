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
            foreach(var section in FilterParameters.Keys)
            {
                foreach(var key in FilterParameters[section].Keys)
                {
                    bool value = FilterParameters[section][key];
                    iniFile.WriteString(section, key, value.ToString());
                }
            }
        }

        /// <summary>
        /// Прочитать конфигурацию фильтрации из .ini
        /// </summary>
        public void Read() 
        {
            if (!File.Exists(pathToConfig))
            {
                MessageBox.Show("Не найден файл конфигурации для фильтра",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var newFilterParameters = 
                new Dictionary<string, Dictionary<string, bool>>();
            var iniFile = new IniFile(pathToConfig);
            // Стандартное значение ключа параметра в .ini
            string defaultValue = "false";

            foreach (var section in FilterParameters.Keys)
            {
                var itemParameters = new Dictionary<string, bool>();
                Dictionary<string, bool> parameters = FilterParameters[section];

                foreach (var keyValuePair in parameters)
                {
                    string readValue = iniFile.ReadString(section, 
                        keyValuePair.Key, defaultValue);
                    bool.TryParse(readValue, out bool isEnabled);
                    itemParameters.Add(keyValuePair.Key, isEnabled);
                }

                newFilterParameters.Add(section, itemParameters);
            }

            FilterParameters = newFilterParameters;
        }

        /// <summary>
        /// Фильтровать
        /// </summary>
        /// <param name="items">Элементы для фильтрации</param>
        /// <param name="filterList">Какой список фильтруется</param>
        public ListViewItem[] FilterOut(List<ListViewItem> items, 
            FilterList filterList)
        {
            var filteredList = new List<ListViewItem>();
            var allowedDevices = new string[0];

            if (filterList == FilterList.CurrentProject)
            {
                allowedDevices = CurrentProjectSelectedDevices;
            }
            else
            {
                allowedDevices = AdvancedProjectSelectedDevices;
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
            const string noneType = "NONE";

            foreach (var type in types)
            {
                if (type.ToString() != noneType)
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
            FilterParameters = 
                new Dictionary<string, Dictionary<string, bool>>();

            var currProjParameters = new Dictionary<string, bool>();
            var advProjParameters = new Dictionary<string, bool>();
            var bindedSignalsParameters = new Dictionary<string, bool>();

            List<string> devices = GetDevicesList();
            bool defaultValue = false;
            foreach(var dev in devices)
            {
                currProjParameters.Add(dev, defaultValue);
                advProjParameters.Add(dev, defaultValue);
            }
            
            // Строковые названия - названия UI-элементов на форме
            bindedSignalsParameters.Add("groupAsPairsCheckBox", defaultValue);
            
            FilterParameters.Add("currProjDevList", currProjParameters);
            FilterParameters.Add("advProjDevList", advProjParameters);
            FilterParameters.Add("bindedSignalsList", bindedSignalsParameters);
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
        /// Установить параметр фильтра
        /// </summary>
        /// <param name="controlName">Имя элемента управления</param>
        /// <param name="parameterName">Имя параметра</param>
        /// <param name="value">Значение</param>
        public void SetFilterParameter(string controlName, 
            string parameterName, bool value)
        {
            FilterParameters[controlName][parameterName] = value;
        }

        /// <summary>
        /// Параметры фильтра
        /// </summary>
        public Dictionary<string, Dictionary<string, bool>> FilterParameters 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Выбранные устройства для отображения в текущем проекте
        /// </summary>
        public string[] CurrentProjectSelectedDevices
        {
            get
            {
                return FilterParameters["currProjDevList"]
                    .Where(x => x.Value == true)
                    .Select(x => x.Key)
                    .ToArray();
            }
        }

        /// <summary>
        /// Выбранные устройства для отображения в альтернативном проекте
        /// </summary>
        public string[] AdvancedProjectSelectedDevices
        {
            get
            {
                return FilterParameters["advProjDevList"]
                    .Where(x => x.Value == true)
                    .Select(x => x.Key)
                    .ToArray();
            }
        }

        /// <summary>
        /// Использовать группировку устройств или нет
        /// </summary>
        public bool UseDeviceGroups
        {
            get
            {
                return FilterParameters["bindedSignalsList"]
                    ["groupAsPairsCheckBox"];
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
            CurrentProject,
            AdvancedProject,
        }

        private static FilterConfiguration filterConfiguration;
        private string pathToConfig;
        private FilterForm filterForm;
    }
}