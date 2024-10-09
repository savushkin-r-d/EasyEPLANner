using System;
using System.Linq;
using System.Windows.Forms;

namespace StaticHelper
{
    /// <summary>
    /// Класс, содержащий методы конвертирования IP-адреса в разные форматы.
    /// </summary>
    public static class IPConverter
    {
        /// <summary>
        /// Конвертировать IP-адрес из строкового типа в long.
        /// Из вида "000.000.000.000".
        /// </summary>
        /// <param name="IP">Строка с адресом</param>
        /// <returns>0 - если не конвертировалось</returns>
        public static long ConvertIPStrToLong(string IP)
        {
            try
            {
                return IP.Trim().Split('.').Select(long.Parse)
                    .Aggregate((result, octet) => (result << 8) + octet);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Конвертировать IP-адрес из long в строку
        /// </summary>
        /// <param name="ip">IP-адрес (long)</param>
        public static string ToString(long ip)
        {
            return $"{(ip & 0xFF000000) >> 24}.{(ip & 0xFF0000) >> 16}.{(ip & 0xFF00) >> 8}.{ip & 0xFF}";
        }
    }
}
