using EplanDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    /// <summary>
    /// Модуль привязки
    /// </summary>
    public static class SignalBinder
    {
        /// <summary>
        /// Можно ли привязать устройство к полю.
        /// </summary>
        /// <param name="model">Модель обмена</param>
        /// <param name="signal">Поле</param>
        /// <param name="device">Устройство для привязки</param>
        public static bool CanBind(this IGateway model, ISignal signal, IIODevice device)
        {
            if (device.DeviceType is DeviceType.AI or DeviceType.DI)
            {
                if (model.Read.NestedSignals.Contains(signal))
                    return false;
            }
            else
            {
                if (model.Write.NestedSignals.Contains(signal))
                    return false;
            }


            if (device.DeviceType is DeviceType.AO or DeviceType.AI && signal.DataType is "Bool")
                return false;

            return true;
        }

        /// <summary>
        /// Привязать устройство к полю.
        /// </summary>
        /// <param name="model">Модель обмена</param>
        /// <param name="signal">Поле</param>
        /// <param name="device">Устройство для привязки</param>
        public static void Bind(this IGateway model, ISignal signal, IIODevice device)
        {
            if (!model.CanBind(signal, device))
                return;

            signal.Device = device;
        }
    }
}
