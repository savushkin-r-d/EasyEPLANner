using LuaInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterprojectExchange
{
    /// <summary>
    /// Модель межпроектного обмена текущего проекта.
    /// </summary>
    public class CurrentProjectModel : InterprojectExchangeModel, IProjectModel
    {
        public CurrentProjectModel()
        {
            receiverSignals = new Dictionary<string, DeviceSignalsDTO>();
            sourceSignals = new Dictionary<string, DeviceSignalsDTO>();
        }

        /// <summary>
        /// Добавление сигнала к модели, вызывается из LUA
        /// </summary>
        /// <param name="name">Имя сигнала</param>
        /// <param name="signalType">Тип сигнала</param>
        /// <param name="receiveMode">Режим получения сигналов</param>
        /// <param name="projName">Имя проекта, связываемый</param>
        public void AddSignal(string name, string signalType, 
            bool receiveMode, string projName)
        {
            if(!receiverSignals.ContainsKey(projName))
            {
                receiverSignals.Add(projName, new DeviceSignalsDTO());
            }
            if(!sourceSignals.ContainsKey(projName))
            {
                sourceSignals.Add(projName, new DeviceSignalsDTO());
            }

            switch(signalType)
            {
                case "AI":
                    if (receiveMode)
                    {
                        receiverSignals[projName].AI.Add(name);
                    }
                    else
                    {
                        sourceSignals[projName].AI.Add(name);
                    }
                    break;

                case "AO":
                    if (receiveMode)
                    {
                        receiverSignals[projName].AO.Add(name);
                    }
                    else
                    {
                        sourceSignals[projName].AO.Add(name);
                    }
                    break;

                case "DI":
                    if (receiveMode)
                    {
                        receiverSignals[projName].DI.Add(name);
                    }
                    else
                    {
                        sourceSignals[projName].DI.Add(name);
                    }
                    break;

                case "DO":
                    if (receiveMode)
                    {
                        receiverSignals[projName].DO.Add(name);
                    }
                    else
                    {
                        sourceSignals[projName].DO.Add(name);
                    }
                    break;
            }
        }

        /// <summary>
        /// Добавление сигнала к модели, вызывается из LUA
        /// </summary>
        /// <param name="name">Имя сигнала</param>
        /// <param name="signalType">Тип сигнала</param>
        /// <param name="receiveMode">Режим получения сигналов</param>
        public void AddSignal(string name, string signalType, bool receiveMode)
        {
            // Заглушка.
            return;
        }

        /// <summary>
        /// Выбран ли проект в списке (здесь будет всегда false)
        /// </summary>
        public bool Selected { get; set; } = false;

        /// <summary>
        /// Альтернативный проект, который выбран для обмена с текущим
        /// </summary>
        public string SelectedAdvancedProject { get; set; }

        /// <summary>
        /// Сигналы-источники (отдаем)
        /// </summary>
        public DeviceSignalsDTO SourceSignals
        {
            get
            {
                return sourceSignals[SelectedAdvancedProject];
            }
        }

        /// <summary>
        /// Сигналы-приемники (принимаем)
        /// </summary>
        public DeviceSignalsDTO ReceiverSignals
        {
            get
            {
                return receiverSignals[SelectedAdvancedProject];
            }
        }

        private Dictionary<string, DeviceSignalsDTO> sourceSignals;
        private Dictionary<string, DeviceSignalsDTO> receiverSignals;
    }
}
