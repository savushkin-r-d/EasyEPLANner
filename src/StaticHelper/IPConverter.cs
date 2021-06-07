using System;

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
        /// <returns></returns>
        public static long ConvertIPStrToLong(string IP)
        {
            long convertedIP;
            const int oneDigit = 1;
            const int twoDigits = 2;

            string[] IPPairs = IP.Split('.');
            for (int i = 0; i < IPPairs.Length; i++)
            {
                if (IPPairs[i].Length == oneDigit)
                {
                    IPPairs[i] = string.Format("00{0}", IPPairs[i]);
                    continue;
                }

                if (IPPairs[i].Length == twoDigits)
                {
                    IPPairs[i] = string.Format("0{0}", IPPairs[i]);
                    continue;
                }
            }

            string IPstring = string.Concat(IPPairs);

            bool isCorrectIP = long.TryParse(IPstring, out convertedIP);
            if (isCorrectIP)
            {
                return convertedIP;
            }
            else
            {
                return 0;
            }
        }
    }
}
