﻿using System.Text.RegularExpressions;

namespace InterprojectExchange
{
    /// <summary>
    /// Объект для обмена информацией об устройствах
    /// </summary>
    public class DeviceInfo
    {
        public DeviceInfo(string name, string description)
        {
            Name = name;
            Description = description;
            Type = GetType(name);
        }

        /// <summary>
        /// Получить тип на основе имени устройства;
        /// </summary>
        /// <param name="devName">Имя устройства</param>
        /// <returns></returns>
        private string GetType(string devName)
        {
            var result = "";
            var match = Regex.Match(devName, DeviceComparer.RegExpPattern);
            if (match.Success)
            {
                result = match.Groups["type"].Value.ToUpper();
            }
            return result;
        }

        /// <summary>
        /// Имя устройства А1BBB2
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание устройства
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Тип устройства (AI,TE..)
        /// </summary>
        public string Type { get; set; }
    }
}
