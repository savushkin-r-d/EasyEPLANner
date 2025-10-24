using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.FileSavers.XML
{
    public interface IChannel
    {
        /// <summary>
        /// Описание канала
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Название канала
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Коментарий канала
        /// </summary>
        string Comment { get; set; }

        /// <summary>
        /// Протоколировать канал
        /// </summary>
        bool IsLogged { get; set; }

        /// <summary>
        /// Включить протоколирование
        /// </summary>
        IChannel Logged();

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
        /// Включить опрос по времени
        /// </summary>
        /// <param name="requestPeriod">Период вопроса</param>
        /// <param name="delta">Дельта</param>
        IChannel RequestByTime(int requestPeriod = 5000, double delta = 0.2);

        /// <summary>
        /// Получить копию канала с индексом
        /// </summary>
        IChannel GetIndexedCopy(int index);
    }
}
