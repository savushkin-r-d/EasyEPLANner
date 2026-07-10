using EplanDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.ModbusExchange.Model
{
    public static class SignalsListBuilder
    {
        /// <summary>
        /// Получить сигналы для обмена Modbus.
        /// </summary>
        /// <remarks>
        /// Список устройств фильтруется по тексту. 
        /// Также если сигнал уже привязан к модели, то он также отбрасывается. 
        /// </remarks>
        /// <param name="deviceManager">Менеджер устройств</param>
        /// <param name="model">Модель обмена сигналами</param>
        /// <param name="textFilter">Текстовый фильтр</param>
        /// <returns>Отфильтрованный список сигналов</returns>
        public static IEnumerable<IIODevice> GetModbusExchangeSignals(
            this IDeviceManager deviceManager, IGateway model, string textFilter)
        {
            var devices = deviceManager.Devices
                .Where(d => d.DeviceType is 
                    DeviceType.AO or DeviceType.AI or
                    DeviceType.DO or DeviceType.DI)
                .OfType<IIODevice>();


            if (model is not null)
            {
                var usedsignals = model.Read.NestedSignals
                    .Concat(model.Write.NestedSignals);

                devices = devices.Except(usedsignals.Select(s => s.Device));
            }

            if (textFilter != string.Empty)
            {
                textFilter = textFilter.ToLower();
                devices = devices.Where(d => 
                    d.Name.ToLower().Contains(textFilter) ||
                    d.Description.ToLower().Contains(textFilter));
            }

            return devices;
        }
    }
}
