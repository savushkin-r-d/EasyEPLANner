using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.FileSavers.XML
{
    public interface ISubtype
    {
        /// <summary>
        /// Название подтипа
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Протоколировать
        /// </summary>
        bool IsLogged {  get; set; }

        /// <summary>
        /// Опрос по времени
        /// </summary>
        bool IsRequestByTime { get; set; }

        /// <summary>
        /// Период опроса
        /// </summary>
        int RequestPeriod { get; set; }

        /// <summary>
        /// Дельта
        /// </summary>
        double Delta { get; set; }

        /// <summary>
        /// Список каналов
        /// </summary>
        List<IChannel> Channels { get;}

        /// <summary>
        /// Добавить канал
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <param name="count">Количество индексированных копий</param>
        IChannel AddChannel(IChannel channel, int count = 1);
    }
}
