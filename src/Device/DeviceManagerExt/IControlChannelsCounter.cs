using System.Collections.Generic;

namespace EplanDevice
{
    public interface IControlChannelsCounter
    {
        /// <summary>
        /// Список каналов подтипов
        /// </summary>
        List<SubtypeControlChannels> SubtypeChannels { get; set; }

        /// <summary>
        /// Добавить описание каналов подтипа
        /// </summary>
        /// <param name="dstStr">Название подтипа</param>
        /// <param name="DI">Количество каналов DI</param>
        /// <param name="DO">Количество каналов DO</param>
        /// <param name="AI">Количество каналов AI</param>
        /// <param name="AO">Количество каналов AO</param>
        void AddChannelsCount(string dstStr, int DI, int DO, int AI, int AO);

        /// <summary>
        /// Расчет статистики каналов по всем устройствам проекта
        /// </summary>
        (int DI, int DO, int AI, int AO) CalculateUsedChannelsCount();

        /// <summary>
        /// Получить количество каналов подтипа
        /// </summary>
        /// <param name="dstStr">Название подтипа</param>
        /// <returns>Количество каналов [DI, DO, AI, AO]</returns>
        int[] GetChannelsCount(string dstStr);

        /// <summary>
        /// Получить описание каналов подтипа
        /// </summary>
        /// <param name="dstStr"></param>
        SubtypeControlChannels GetSubtypeControlChannels(string dstStr);
    }
}