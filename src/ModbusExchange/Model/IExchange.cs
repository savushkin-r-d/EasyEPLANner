using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    /// <summary>
    /// Модель обмена сигналами по Modbus
    /// </summary>
    public interface IExchange
    {
        /// <summary>
        /// Список моделей обмена со шлюзами
        /// </summary>
        IEnumerable<IGateway> Models { get; }

        /// <summary>
        /// Выбранная в редакторе модель шлюза
        /// </summary>
        IGateway SelectedModel { get; }

        /// <summary>
        /// Добавить новую <see cref="IGateway">модель</see> шлюза
        /// </summary>
        /// <param name="modelName">Название модели</param>
        void AddModel(string modelName);

        /// <summary>
        /// Выбрать <see cref="IGateway">модель</see> шлюза
        /// </summary>
        /// <param name="modelName">Название модели</param>
        void SelectModel(string modelName);
    }
}
