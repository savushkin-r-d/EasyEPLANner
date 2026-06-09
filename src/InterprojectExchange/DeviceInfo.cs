using System.Text.RegularExpressions;
using EplanDevice;

namespace InterprojectExchange
{
    /// <summary>
    /// Объект для обмена информацией об устройствах
    /// </summary>
    public class DeviceInfo
    {
        public DeviceInfo(string name, string description,
            int deviceType = -1, int subTypeIndex = 0)
        {
            Name = name;
            Description = description;
            Type = GetType(name, deviceType, subTypeIndex);
        }

        /// <summary>
        /// Получить тип для фильтрации на основе имени и подтипа устройства.
        /// </summary>
        /// <param name="devName">Имя устройства</param>
        /// <param name="deviceType">Тип устройства из main.io.lua</param>
        /// <param name="subTypeIndex">Индекс подтипа из main.io.lua</param>
        private static string GetType(string devName, int deviceType,
            int subTypeIndex)
        {
            if (deviceType >= 0 && subTypeIndex > 0)
            {
                var subType = (DeviceSubType)(deviceType * DSTExt.TypeMultiplier
                    + subTypeIndex);
                string subTypeName = subType.ToString();
                if (subTypeName.EndsWith("_VIRT"))
                {
                    return subTypeName;
                }
            }

            var match = Regex.Match(devName, DeviceComparer.RegExpPattern);
            if (match.Success)
            {
                return match.Groups["type"].Value.ToUpper();
            }

            return string.Empty;
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
