using Eplan.EplApi.DataModel;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner
{
    /// <summary>
    /// Читатель привязки устройств.
    /// </summary>
    class DeviceBindingReader
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public DeviceBindingReader()
        {
            this.deviceManager = Device.DeviceManager.GetInstance();
        }

        /// <summary>
        /// Прочитать привязку.
        /// </summary>
        public void Read()
        {
            PrepareForReading();

            // TODO: Reading binding
            EplanDeviceManager.GetInstance().ReadConfigurationFromIOModules();
        }

        /// <summary>
        /// Подготовка к чтению привязки.
        /// </summary>
        private void PrepareForReading()
        {
            var objectsFinder = new DMObjectsFinder(ApiHelper.GetProject());
            var plcFilter = new FunctionsFilter();

            var properties = new FunctionPropertyList();
            properties.FUNC_MAINFUNCTION = true;

            plcFilter.SetFilteredPropertyList(properties);
            plcFilter.Category = Function.Enums.Category.PLCBox;

            functionsForSearching = objectsFinder.GetFunctions(plcFilter);
        }

        /// <summary>
        /// Функции для поиска модулей ввода-вывода
        /// </summary>
        Function[] functionsForSearching;

        /// <summary>
        /// Менеджер устройств.
        /// </summary>
        Device.DeviceManager deviceManager;
    }
}
