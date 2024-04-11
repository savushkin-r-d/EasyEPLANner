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
            int offset = 0;
            return IP.Split('.').Select(long.Parse).Reverse().Aggregate((result, octet) => result | octet << (offset += 8));
        }
    }
}
