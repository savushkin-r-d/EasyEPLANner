using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.FileSavers.XML
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDriver
    {
        /// <summary>
        /// Получить подтип по его описанию
        /// </summary>
        /// <param name="subtype">Описание подтипа</param>
        ISubtype this[string subtype] { get; } 
        
        /// <summary>
        /// Список всех подтипов
        /// </summary>
        List<ISubtype> Subtypes { get; }

        /// <summary>
        /// Добавить канал
        /// </summary>
        /// <param name="stDescription">Описание подтипа</param>
        /// <param name="channel">Канал</param>
        /// <param name="count">Количествол каналов для индексации</param>
        /// <returns></returns>
        IChannel AddChannel(string stDescription, IChannel channel, int count = 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stDescription"></param>
        /// <param name="channelName"></param>
        /// <param name="comment"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IChannel AddChannel(string stDescription, string channelName,
            string comment = "", int count = 1);
    }
}
