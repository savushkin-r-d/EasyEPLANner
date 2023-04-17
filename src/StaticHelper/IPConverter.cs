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
            long[] IPOctets = IP.Split('.').Select(long.Parse).ToArray();         
            return IPOctets[0] << 24 | IPOctets[1] << 16 | IPOctets[2] << 8 | IPOctets[3];
        }
    }
}
