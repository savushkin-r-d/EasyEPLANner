using LuaInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterprojectExchange
{
    /// <summary>
    /// Модель межпроектного обмена альтернативного проекта.
    /// </summary>
    public class AdvancedProjectModel : InterprojectExchangeModel, IProjectModel
    {
        public AdvancedProjectModel() : base()
        {
            receiverSignals = new DeviceSignalsDTO();
            sourceSignals = new DeviceSignalsDTO();
            SharedFileAsStringList = new List<string>();
        }

        /// <summary>
        /// Добавление сигнала к модели, вызывается из LUA
        /// </summary>
        /// <param name="name">Имя сигнала</param>
        /// <param name="signalType">Тип сигнала</param>
        /// <param name="receiveMode">Режим получения сигналов</param>
        public void AddSignal(string name, string signalType, 
            bool receiveMode)
        {
            switch(signalType)
            {
                case "AI":
                    if (receiveMode)
                    {
                        ReceiverSignals.AI.Add(name);
                    }
                    else
                    {
                        SourceSignals.AI.Add(name);
                    }
                    break;

                case "AO":
                    if (receiveMode)
                    {
                        ReceiverSignals.AO.Add(name);
                    }
                    else
                    {
                        SourceSignals.AO.Add(name);
                    }
                    break;

                case "DI":
                    if (receiveMode)
                    {
                        ReceiverSignals.DI.Add(name);
                    }
                    else
                    {
                        SourceSignals.DI.Add(name);
                    }
                    break;

                case "DO":
                    if (receiveMode)
                    {
                        ReceiverSignals.DO.Add(name);
                    }
                    else
                    {
                        SourceSignals.DO.Add(name);
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
        /// <param name="projName">Имя проекта</param>
        public void AddSignal(string name, string signalType, bool receiveMode, 
            string projName)
        {
            // Заглушка.
            return;
        }

        /// <summary>
        /// Выбрана ли модель сейчас в GUI
        /// </summary>
        public bool Selected { get; set; } = false;

        /// <summary>
        /// Помечена на удаление
        /// </summary>
        public bool MarkedForDelete { get; set; } = false;

        /// <summary>
        /// Файл с межконтроллерным обменом в виде списка строк
        /// </summary>
        public List<string> SharedFileAsStringList { get; set; }

        /// <summary>
        /// Сигналы-источники (отдаем)
        /// </summary>
        virtual public DeviceSignalsDTO SourceSignals
        {
            get
            {
                return sourceSignals;
            }
        }

        /// <summary>
        /// Сигналы-приемники (принимаем)
        /// </summary>
        virtual public DeviceSignalsDTO ReceiverSignals
        {
            get
            {
                return receiverSignals;
            }
        }

        private DeviceSignalsDTO sourceSignals;
        private DeviceSignalsDTO receiverSignals;
    }
}
