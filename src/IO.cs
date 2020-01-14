///@file IO.cs
///@brief Классы, реализующие минимальную функциональность, необходимую для 
///экспорта описания модулей IO для PAC.

using System;
using System.Collections.Generic;   //Использование List.
using System.Globalization;
using System.Drawing;

/// <summary>
/// Пространство имен классов IO.
/// </summary>
namespace IO
{
    /// <summary>
    /// Описание модуля ввода-вывода IO.
    /// </summary>
    public class IOModuleInfo
    {

        /// <summary>
        /// Получение описания модуля ввода-вывода IO на основе его имени.
        /// </summary>
        /// <param name="name">Имя модуля (750-860).</param>
        /// <param name="isStub">Признак неидентифицированного модуля.</param>
        public IOModuleInfo GetIOModuleInfo(string name, out bool isStub)
        {
            isStub = false;

            if (modules == null)
            {
                modules = new List<IOModuleInfo>();
                // В Phoenix Contact особенность, адресное пространство кратно 32
                // в связи с этим, увеличивается цифра DO/DI count в два раза в отличие от WAGO.
                // При добавлении модуля учитывать, что первый [0] индекс массива только для Phoenix,
                // а в WAGO его необходимо ставить -1

                modules.Add(new IOModuleInfo(1504, "750-1504",
                    "16-Channel Digital Output Module 24 V DC",
                    ADDRESS_SPACE_TYPE.DO,
                    "Дискретный выход", "750-1500",
                    new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 
                        14, 15, 16 },
                    new int[0],
                    new int[] { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 
                        12, 13, 14, 15 },
                    16, 0, 0, 0, Color.Red));

                modules.Add(new IOModuleInfo(1515, "750-1515",
                    "8-Channel Digital Output Module 24 V DC",
                    ADDRESS_SPACE_TYPE.DO,
                    "Дискретный выход", "750-1500",
                    new int[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                    new int[0],
                    new int[] { -1, 0, 1, 2, 3, 4, 5, 6, 7 },
                    8, 0, 0, 0, Color.Red));

                modules.Add(new IOModuleInfo(504, "750-504",
                    "4-Channel Digital Output Module DC 24 V",
                    ADDRESS_SPACE_TYPE.DO,
                    "Дискретный выход", "750-500",
                    new int[] { 1, 5, 4, 8 },
                    new int[0],
                    new int[] { -1, 0, -1, -1, 1, 2, -1, -1, 3 },
                    4, 0, 0, 0, Color.Red));

                modules.Add(new IOModuleInfo(512, "750-512",
                    "2-Channel Relay Output Module 230 V AC, 30 V DC",
                    ADDRESS_SPACE_TYPE.DO,
                    "Дискретный выход", "750-500",
                    new int[] { 1, 5 },
                    new int[0],
                    new int[] { -1, 0, -1, -1, -1, 1, -1, -1, -1 },
                    4, 0, 0, 0, Color.Red));

                modules.Add(new IOModuleInfo(530, "750-530",
                    "8-Channel Digital Output Module DC 24 V",
                    ADDRESS_SPACE_TYPE.DO,
                    "Дискретный выход", "750-500",
                    new int[] { 1, 5, 2, 6, 3, 7, 4, 8 },
                    new int[0],
                    new int[] { -1, 0, 4, 1, 5, 2, 6, 3, 7 },
                    8, 0, 0, 0, Color.Red));

                modules.Add(new IOModuleInfo(402, "750-402",
                    "4-Channel Digital Input Module 24 V DC",
                    ADDRESS_SPACE_TYPE.DI,
                    "Дискретный вход", "750-400",
                    new int[] { 1, 5, 4, 8 },
                    new int[] { -1, 0, -1, -1, 2, 3, -1, -1, 3 },
                    new int[0],
                    0, 4, 0, 0, Color.Yellow));

                modules.Add(new IOModuleInfo(430, "750-430",
                    "8-Channel Digital Input Module 24 V DC",
                    ADDRESS_SPACE_TYPE.DI,
                    "Дискретный вход", "750-400",
                    new int[] { 1, 5, 2, 6, 3, 7, 4, 8 },
                    new int[] { -1, 0, 2, 5, 6, 1, 3, 5, 7 },
                    new int[0],
                    0, 8, 0, 0, Color.Yellow));

                modules.Add(new IOModuleInfo(1405, "750-1405",
                    "16-Channel Digital Input Module 24 V DC",
                    ADDRESS_SPACE_TYPE.DI,
                    "Дискретный вход", "750-1400",
                    new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 
                        14, 15, 16 },
                    new int[] { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 
                        13, 14, 15 },
                    new int[0],
                    0, 16, 0, 0, Color.Yellow));

                modules.Add(new IOModuleInfo(1415, "750-1415",
                    "8-Channel Digital Input Module 24 V DC",
                    ADDRESS_SPACE_TYPE.DI,
                    "Дискретный вход", "750-1400",
                    new int[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                    new int[] { -1, 0, 1, 2, 3, 4, 5, 6, 7 },
                    new int[0],
                    0, 8, 0, 0, Color.Yellow));

                modules.Add(new IOModuleInfo(1420, "750-1420",
                    "4-Channel Digital Input Module 24 V DC",
                    ADDRESS_SPACE_TYPE.DI,
                    "Дискретный вход", "750-1400",
                    new int[] { 1, 6, 9, 14 },
                    new int[] { -1, 0, -1, -1, -1, -1, 1, -1, -1, 2, -1, 
                        -1, -1, -1, 3, -1, -1 },
                    new int[0],
                    0, 4, 0, 0, Color.Yellow));

                modules.Add(new IOModuleInfo(554, "750-554",
                    "2-Channel Analog Output Module 4-20mA",
                    ADDRESS_SPACE_TYPE.AO,
                    "Аналоговый выход", "750-500",
                    new int[] { 1, 5 },
                    new int[0],
                    new int[] { -1, 0, -1, -1, -1, 1, -1, -1, -1 },
                    0, 0, 2, 0, Color.Blue));

                modules.Add(new IOModuleInfo(555, "750-555",
                    "4-Channel Analog Output Module 4-20mA",
                    ADDRESS_SPACE_TYPE.AO,
                    "Аналоговый выход", "750-500",
                    new int[] { 1, 5, 3, 7 },
                    new int[0],
                    new int[] { -1, 0, -1, 2, -1, 1, -1, 3, -1 },
                    0, 0, 4, 0, Color.Blue));

                modules.Add(new IOModuleInfo(638, "750-638",
                    "2-Channel Up/Down Counter 24 V DC, 500 Hz",
                    ADDRESS_SPACE_TYPE.AI,
                    "Счетчик", "750-600",
                    new int[] { 1, 5 },
                    new int[] { -1, 0, -1, -1, -1, 1, -1, -1, -1 },
                    new int[0],
                    0, 0, 2, 2, Color.Gray));

                modules.Add(new IOModuleInfo(450, "750-450",
                    "4-Channel Analog Input Module for RTDs",
                    ADDRESS_SPACE_TYPE.AI,
                    "Аналоговый вход", "750-400",
                    new int[] { 1, 3, 5, 7 },
                    new int[] { -1, 0, -1, 1, -1, 2, -1, 3, -1 },
                    new int[0],
                    0, 0, 0, 4, Color.Lime));

                modules.Add(new IOModuleInfo(461, "750-461",
                    "2-Channel Analog Input Module for RTDs",
                    ADDRESS_SPACE_TYPE.AI,
                    "Аналоговый вход", "750-400",
                    new int[] { 1, 5 },
                    new int[] { -1, 0, -1, -1, -1, 1, -1, -1, -1 },
                    new int[0],
                    0, 0, 0, 2, Color.Lime));

                modules.Add(new IOModuleInfo(466, "750-466",
                    "2-Channel Analog Input Module 4-20 mA",
                    ADDRESS_SPACE_TYPE.AI,
                    "Аналоговый вход", "750-400",
                    new int[] { 1, 5 },
                    new int[] { -1, 0, -1, -1, -1, 1, -1, -1, -1 },
                    new int[0],
                    0, 0, 0, 2, Color.Lime));

                modules.Add(new IOModuleInfo(496, "750-496",
                    "8-Channel Analog Input Module 4-20 mA",
                    ADDRESS_SPACE_TYPE.AI,
                    "Аналоговый вход", "750-400",
                    new int[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                    new int[] { -1, 0, 1, 2, 3, 4, 5, 6, 7, -1, -1, -1, 
                        -1, -1, -1, -1, -1 },
                    new int[0],
                    0, 0, 0, 8, Color.Lime));

                modules.Add(new IOModuleInfo(655, "750-655",
                    "AS-Interface Master",
                    ADDRESS_SPACE_TYPE.AOAI,
                    "AS-интерфейс мастер", "750-600",
                    new int[] { 3 },
                    new int[] { -1, -1, -1, 0, -1, -1, -1, -1, -1 },
                    new int[] { -1, -1, -1, 0, -1, -1, -1, -1, -1 },
                    0, 0, 20, 20, Color.Gray));

                modules.Add(new IOModuleInfo(657, "750-657",
                    "IO-Link Master",
                    ADDRESS_SPACE_TYPE.AOAI,
                    "AS-интерфейс мастер", "750-600",
                    new int[] { 1, 6, 9, 14 },
                    new int[] { -1, 0, -1, -1, -1, -1, 1, -1, -1, 2, -1, 
                        -1, -1, -1, 3, -1, -1 },
                    new int[] { -1, 0, -1, -1, -1, -1, 1, -1, -1, 2, -1, 
                        -1, -1, -1, 3, -1, -1 },
                    0, 0, 12, 12, Color.Gray));

                modules.Add(new IOModuleInfo(600, "750-600",
                    "End Module",
                    ADDRESS_SPACE_TYPE.NONE,
                    "Терминатор", "750-600",
                    new int[] { },
                    new int[] { },
                    new int[] { },
                    0, 0, 0, 0, Color.Gray));

                modules.Add(new IOModuleInfo(602, "750-602",
                    "Supply Module DC 24 V / AC/DC 230 V",
                    ADDRESS_SPACE_TYPE.NONE,
                    "Питание", "750-600",
                    new int[] { },
                    new int[] { },
                    new int[] { },
                    0, 0, 0, 0, Color.Gray));

                modules.Add(new IOModuleInfo(612, "750-612",
                    "Supply Module 24 V DC / 230 V AC/DC",
                    ADDRESS_SPACE_TYPE.NONE,
                    "Питание", "750-600",
                    new int[] { },
                    new int[] { },
                    new int[] { },
                    0, 0, 0, 0, Color.Gray));

                modules.Add(new IOModuleInfo(627, "750-627",
                    "Internal Data Bus Extension End Module",
                    ADDRESS_SPACE_TYPE.NONE,
                    "Расширение шины", "750-600",
                    new int[] { },
                    new int[] { },
                    new int[] { },
                    0, 0, 0, 0, Color.Gray));

                modules.Add(new IOModuleInfo(628, "750-628",
                    "Internal Data Bus Extension Coupler Module",
                    ADDRESS_SPACE_TYPE.NONE,
                    "Расширение шины", "750-600",
                    new int[] { },
                    new int[] { },
                    new int[] { },
                    0, 0, 0, 0, Color.Gray));

                modules.Add(new IOModuleInfo(491, "750-491",
                    "1-channel Resistance measuring bridge",
                    ADDRESS_SPACE_TYPE.AI,
                    "Аналоговый вход", "750-400",
                    new int[] { 1, 5 },
                    new int[] { -1, 0, -1, -1, -1, 1, -1, -1, -1 },
                    new int[] { },
                    0, 0, 0, 2, Color.Gray));

                modules.Add(new IOModuleInfo(1027843, "AXL F IOL8 2H",
                    "IO-Link Master",
                    ADDRESS_SPACE_TYPE.AOAIDODI,
                    "IO-Link Master", "AXL F",
                    new int[] { 30, 31, 32, 33, 70, 71, 72, 73 },
                    new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, 4, 5, 6, 7},
                    new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, 4, 5, 6, 7},
                    512, 512, 32, 32, Color.Orange));

                modules.Add(new IOModuleInfo(2701916, "AXL F DI8/1 DO8/1 1H",
                    "8-channel Digital I/O module 24V DC, 500 mA, 1-wire connection",
                    ADDRESS_SPACE_TYPE.DODI,
                    "Цифровой вход/выход", "AXL F",
                    new int[] { 00, 01, 02, 03, 10, 11, 12, 13, 20, 21, 22, 
                        23, 30, 31, 32, 33 },
                    new int[] { 0, 1, -1, -1, -1, -1, -1, -1, -1, 2, 3, -1, 
                        -1, -1, -1, -1, -1, -1, -1, 4, 5, -1, -1, -1, -1, -1, 
                        -1, -1, -1, 6, 7, -1, -1, -1 },
                    new int[] {  -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, 2, 3, 
                        -1, -1, -1, -1, -1, -1, -1, -1, 4, 5, -1, -1, -1, -1, 
                        -1, -1, -1, -1, 6, 7, -1, -1},
                    16, 16, 1, 1, Color.Violet));

                modules.Add(new IOModuleInfo(2702071, "AXL F DI8/3 DO8/3 2H",
                    "8-channel Digital I/O Module 24V DC, 500 mA, 3-wire connection",
                    ADDRESS_SPACE_TYPE.DODI,
                    "Цифровой вход/выход", "AXL F",
                    new int[] { 00, 01, 02, 03, 20, 21, 22, 23, 40, 41, 42, 
                        43, 60, 61, 62, 63 },
                    new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, 8, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, 13, 14, 15, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    new int[] { 8, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, 13, 14, 15, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    16, 16, 1, 1, Color.Violet));

                modules.Add(new IOModuleInfo(2688491, "AXL F AI4 I 1H",
                    "4-channel Analog Input Module 4..20mA, 2,3,4-wire connection",
                    ADDRESS_SPACE_TYPE.AI,
                    "Аналоговый вход", "AXL F",
                    new int[] { 00, 01, 02, 03 },
                    new int[] { 0, 1, 2, 3, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1 },
                    new int[0],
                    64, 64, 4, 4, Color.Green));

                modules.Add(new IOModuleInfo(2688527, "AXL F AO4 1H",
                    "4-channel Analog Output Module 4..20mA, 2,3,4-wire connection",
                    ADDRESS_SPACE_TYPE.AO,
                    "Аналоговый выход", "AXL F",
                    new int[] { 10, 11, 12, 13 },
                    new int[0],
                    new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 
                        1, 2, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1 },
                    64, 64, 4, 4, Color.Yellow));

                modules.Add(new IOModuleInfo(2688048, "AXL F DO16/3 2F",
                    "16-channel Digital Output Module 24V DC, 500 mA, 3-wire connection",
                    ADDRESS_SPACE_TYPE.DO,
                    "Цифровой выход", "AXL F",
                    new int[] { 00, 01, 02, 03, 04, 05, 06, 07, 40, 41, 42, 
                        43, 44, 45, 46, 47 },
                    new int[0],
                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, 8, 9, 10, 11, 12, 13, 14, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    16, 16, 1, 1, Color.Red));

                modules.Add(new IOModuleInfo(2688556, "AXL F RTD4 1H",
                    "4-channel Analog Input Module for RTDs, 2,3,4-wire connection",
                    ADDRESS_SPACE_TYPE.AI,
                    "Аналоговый вход", "AXL F",
                    new int[] { 00, 01, 02, 03 },
                    new int[] { 0, 1, 2, 3, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1 },
                    new int[0],
                    64, 64, 4, 4, Color.Green));

                modules.Add(new IOModuleInfo(2688022, "AXL F DI16/4 2F",
                    "16-channel Digital Input Module 24V DC, 4-wire connection",
                    ADDRESS_SPACE_TYPE.DI,
                    "Цифровой вход", "AXL F",
                    new int[] { 00, 01, 02, 03, 04, 05, 06, 07, 40, 41, 42, 
                        43, 44, 45, 46, 47 },
                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                        -1, 8, 9, 10, 11, 12, 13, 14, 15, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    new int[0],
                    16, 16, 1, 1, Color.Red));

                modules.Add(new IOModuleInfo(2688093, "AXL F CNT2 INC2 1F",
                    "2-Channel Up/Down Counter, 24V DC",
                    ADDRESS_SPACE_TYPE.AI,
                    "Счетчик", "AXL F",
                    new int[] { 00, 04 },
                    new int[] { 3, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, 
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
                    new int[0],
                    224, 224, 14, 14, Color.Orange));
            }

            IOModuleInfo res = modules.Find( x => x.Name == name);

            if (res != null)
            {
                return res;
            }
                
            isStub = true;
            return stub;
        }

        /// <summary>
        /// Закрытый конструктор. Используется для создания списка применяемых
        /// модулей.
        /// </summary>
        private IOModuleInfo(int n, string name, string descr,
            ADDRESS_SPACE_TYPE addressSpaceType, string typeName,
            string groupName, int[] channelClamps, int[] clampsAddressIn, 
            int[] clampsAddressOut, int DO_count, int DI_count, int AO_count, 
            int AI_count, Color color)
        {
            this.n = n;
            this.name = name;
            this.description = descr;

            this.addressSpaceType = addressSpaceType;
            this.typeName = typeName;
            this.groupName = groupName;

            this.channelClamps = channelClamps;
            this.channelAddressesIn = clampsAddressIn;
            this.channelAddressesOut = clampsAddressOut;

            this.DO_cnt = DO_count;
            this.DI_cnt = DI_count;
            this.AO_cnt = AO_count;
            this.AI_cnt = AI_count;

            this.moduleColor = color;
        }

        /// <summary>
        /// Пустой конструктор класса
        /// </summary>
        public IOModuleInfo() { }

        /// <summary>
        /// Имя модуля ввода-вывода IO (серия-номер, например: 750-860).
        /// </summary>        
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Номер модуля ввода-вывода IO (например: 860).
        /// </summary>  
        public int Number
        {
            get
            {
                return n;
            }
        }

        /// <summary>
        /// Описание модуля ввода-вывода IO.
        /// </summary>  
        public string Description
        {
            get
            {
                return description;
            }
        }

        /// <summary>
        /// Тип адресного пространства модуля ввода-вывода IO.
        /// </summary>
        public ADDRESS_SPACE_TYPE AddressSpaceType
        {
            get
            {
                return addressSpaceType;
            }
        }

        /// <summary>
        /// Клеммы каналов ввода-вывода.
        /// </summary>
        public int[] ChannelClamps
        {
            get
            {
                return channelClamps;
            }
        }

        /// <summary>
        /// Адреса каналов ввода.
        /// </summary>
        public int[] ChannelAddressesIn
        {
            get
            {
                return channelAddressesIn;
            }
        }

        /// <summary>
        /// Адреса каналов вывода.
        /// </summary>
        public int[] ChannelAddressesOut
        {
            get
            {
                return channelAddressesOut;
            }
        }

        /// <summary>
        ///Количество дискретных выходов. 
        /// </summary>
        public int DO_count
        {
            get
            {
                return DO_cnt;
            }
        }

        /// <summary>
        ///Количество дискретных входов. 
        /// </summary>
        public int DI_count
        {
            get
            {
                return DI_cnt;
            }
        }

        /// <summary>
        ///Количество аналоговых выходов. 
        /// </summary>
        public int AO_count
        {
            get
            {
                return AO_cnt;
            }
        }

        /// <summary>
        ///Количество аналоговых входов. 
        /// </summary>
        public int AI_count
        {
            get
            {
                return AI_cnt;
            }
        }

        /// <summary>
        ///Имя типа (дискретный выход, аналоговый выход, ...).
        /// </summary>
        public string TypeName
        {
            get
            {
                return typeName;
            }
        }

        /// <summary>
        ///Физический цвет модуля
        /// </summary>
        public Color ModuleColor
        {
            get
            {
                return moduleColor;
            }
        }

        /// <summary>
        /// Имя серии модуля ввода-вывода IO (например 750-800).
        /// </summary>        
        public string GroupName
        {
            get
            {
                return groupName;
            }
        }

        public enum ADDRESS_SPACE_TYPE
        {
            NONE,
            DO,
            DI,
            AO,
            AI,
            AOAI,
            DODI,
            AOAIDODI,
        };


        #region Закрытые поля.
        private int n;                   ///Номер.
        private string name;             ///Имя.
        private string typeName;         ///Имя типа (дискретный выход, аналоговый выход, ...).
        private string groupName;        ///Серия модуля (750-800, 750-1500, ...).
        private string description;      ///Описание.

        ///Тип адресного пространства ( DO, DI, AO, AI ).
        private ADDRESS_SPACE_TYPE addressSpaceType;

        private int[] channelClamps;         ///Клеммы каналов ввода/вывода.
        private int[] channelAddressesOut;   ///Адреса каналов вывода.
        private int[] channelAddressesIn;    ///Адреса каналов ввода.

        private int DO_cnt;     ///Количество дискретных выходов. 
        private int DI_cnt;     ///Количество дискретных входов. 
        private int AO_cnt;     ///Количество аналоговых выходов. 
        private int AI_cnt;     ///Количество аналоговых входов. 

        private Color moduleColor;

        ///Список используемых модулей. 
        private List<IOModuleInfo> modules = null;

        ///Заглушка, для возврата в случае поиска неописанных модулей. 
        private static IOModuleInfo stub = new IOModuleInfo(0,
            "не определен", "", ADDRESS_SPACE_TYPE.NONE, "", "", new int[0], 
            new int[0], new int[0], 0, 0, 0, 0, Color.LightGray);
        #endregion

    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Модуль ввода-вывода IO.
    /// </summary>
    class IOModule
    {

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="inAddressSpaceOffset">Смещение входного адресного 
        /// пространства модуля.</param>
        /// <param name="outAddressSpaceOffset">Смещение выходного адресного 
        /// пространства модуля.</param>
        /// <param name="info">Описание модуля.</param>
        /// <param name="physicalNumber">Физический номер (из ОУ) устройства.
        /// </param>
        public IOModule(int inAddressSpaceOffset, int outAddressSpaceOffset,
            IOModuleInfo info, int physicalNumber)
        {
            this.inAddressSpaceOffset = inAddressSpaceOffset;
            this.outAddressSpaceOffset = outAddressSpaceOffset;
            this.info = info;
            this.physicalNumber = physicalNumber;

            devicesChannels = new List<Device.IODevice.IOChannel>[80];
            devices = new List<Device.IODevice>[80];
        }

        public IOModule(int inAddressSpaceOffset, int outAddressSpaceOffset,
            IOModuleInfo info) : this(inAddressSpaceOffset, 
                outAddressSpaceOffset, info, 0)
        {
            // Делегировано в конструктор с 4 параметрами.
        }

        public void AssignChannelToDevice(int chN, Device.IODevice dev,
            Device.IODevice.IOChannel ch)
        {
            if (devices.GetLength(0) <= chN)
            {
                System.Windows.Forms.MessageBox.Show("Error!");
            }

            if (devices[chN] == null)
            {
                devices[chN] = new List<Device.IODevice>();
            }
            if (devicesChannels[chN] == null)
            {
                devicesChannels[chN] = new List<Device.IODevice.IOChannel>();
            }

            devices[chN].Add(dev);
            devicesChannels[chN].Add(ch);
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = "";

            if (info != null)
            {
                res = String.Format("{0} {{ {1,7} }},        --{2,7}", prefix, info.Number, info.Name);
            }
            else
            {
                res = prefix + "{ ? },";
            }

            return res;
        }

        public void SaveAsConnectionArray(ref object[,] res, ref int idx, int p, Dictionary<string, int> modulesCount, Dictionary<string, Color> modulesColor)
        {
            string moduleName = Info.Number.ToString();
            if (modulesCount.ContainsKey(moduleName))
            {
                modulesCount[moduleName]++;
            }
            else
            {
                modulesCount.Add(moduleName, 1);
            }

            if (!modulesColor.ContainsKey(moduleName))
            {
                modulesColor.Add(moduleName, Info.ModuleColor);
            }

            if (Info.ChannelClamps.GetLength(0) != 0)
            {
                if (Info.AddressSpaceType == IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI ||
                    Info.AddressSpaceType == IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI)
                {
                    if (this.isIOLink() == true)
                    {
                        foreach (int clamp in Info.ChannelClamps)
                        {
                            res[idx, 0] = p;
                            res[idx, 1] = moduleName;
                            res[idx, 2] = clamp.ToString();
                            res[idx, 3] = "IO-Link";
                            idx++;
                        }
                    }
                    else
                    {
                        foreach (int clamp in Info.ChannelClamps)
                        {
                            res[idx, 0] = p;
                            res[idx, 1] = moduleName;
                            res[idx, 2] = clamp.ToString();
                            res[idx, 3] = "AS interface";
                            idx++;
                        }
                    }
                }
                else
                {
                    foreach (int clamp in Info.ChannelClamps)
                    {
                        res[idx, 0] = p;
                        res[idx, 1] = moduleName;
                        res[idx, 2] = clamp;
                        if (devices[clamp] != null)
                        {
                            string devName = "";
                            int devIdx = 0;
                            foreach (Device.IODevice dev in devices[clamp])
                            {

                                devName += dev.EPlanName + dev.GetConnectionType() + dev.GetRange() + ": " +
                                        devicesChannels[clamp][devIdx].Name + ": " + dev.Description + " " +
                                        devicesChannels[clamp][devIdx].Comment;
                                devName = devName.Replace('\n', ' ');
                                devIdx++;
                            }
                            res[idx, 3] = devName;

                        }
                        idx++;
                    }
                }

            }
            else
            {
                res[idx, 0] = p;
                res[idx, 1] = moduleName;
                idx++;
            }
        }

        public void SaveASInterfaceConnection(int nodeIdx, int moduleIdx, Dictionary<string, object[,]> asInterfaceConnection)
        {

            string key = "Узел №" + nodeIdx.ToString() + " Модуль №" + moduleIdx.ToString();
            if (!asInterfaceConnection.ContainsKey(key))
            {
                if (Info.ChannelClamps.GetLength(0) != 0)
                {
                    object[,] asConnection = new object[Info.ChannelClamps.GetLength(0) * 128, 2];
                    int devIdx = 0;
                    foreach (int clamp in Info.ChannelClamps)
                    {

                        if (devices[clamp] != null)
                        {
                            int deviceCounter = 0;
                            foreach (Device.IODevice dev in devices[clamp])
                            {

                                asConnection[devIdx, 0] = clamp.ToString() + "(" + (deviceCounter + 1).ToString() + ")";
                                string devDescription = dev.EPlanName + dev.GetConnectionType() + dev.GetRange() + ": " +
                                    devicesChannels[clamp][deviceCounter].Name + ": " + dev.Description + " " +
                                    devicesChannels[clamp][deviceCounter].Comment;
                                devDescription = devDescription.Replace('\n', ' ');

                                asConnection[devIdx, 1] = devDescription;

                                devIdx++;
                                deviceCounter++;
                            }
                        }
                    }
                    asInterfaceConnection.Add(key, asConnection);
                }
            }
        }

        /// <summary>
        /// Расчет адреса данных привязанных IO-link устройств.
        /// </summary>       
        public void CalculateIOLinkAdress()
        {
            IOLinkCalculator calculator = new IOLinkCalculator(devices, 
                devicesChannels, Info);
            calculator.Calculate();
        }

        /// <summary>
        /// Описание модуля.
        /// </summary>
        public IOModuleInfo Info
        {
            get
            {
                return info;
            }
        }

        /// <summary>
        /// Смещение входного адресного пространства модуля.
        /// </summary>
        public int InOffset
        {
            get
            {
                return inAddressSpaceOffset;
            }
        }

        /// <summary>
        /// Смещение выходного адресного пространства модуля.
        /// </summary>
        public int OutOffset
        {
            get
            {
                return outAddressSpaceOffset;
            }
        }

        /// <summary>
        /// Номер устройства (из ОУ) прим., 202.
        /// </summary>
        public int PhysicalNumber
        {
            get
            {
                return physicalNumber;
            }
        }

        /// <summary>
        /// Является ли модуль IO-Link 
        /// </summary>
        /// <returns></returns>
        public bool isIOLink()
        {
            bool isIOLink = false;

            int wago = (int) IOManager.IOLinkModules.Wago;
            int phoenixContactStandard = (int) IOManager.IOLinkModules
                .PhoenixContactStandard;
            int phoenixContactSmart = (int) IOManager.IOLinkModules
                .PhoenixContactSmart;

            if (Info.Number == wago ||
                Info.Number == phoenixContactStandard ||
                Info.Number == phoenixContactSmart)
            {
                isIOLink = true;
            }

            return isIOLink;
        }

        /// Привязанные устройства.
        public List<Device.IODevice>[] devices;
        /// Привязанные каналы.
        public List<Device.IODevice.IOChannel>[] devicesChannels;

        #region Закрытые поля.
        ///Смещение входного адресного пространства модуля.
        private int inAddressSpaceOffset;
        ///Смещение выходного адресного пространства модуля.
        private int outAddressSpaceOffset;
        ///Описание модуля
        private IOModuleInfo info;
        ///Физический номер модуля
        private int physicalNumber;
        #endregion
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Узел модулей ввода-вывода IO.
    /// </summary>
    class IONode
    {

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="typeStr">Тип.</param>
        /// <param name="n">Номер (также используется как адрес для COM-порта).</param>
        /// <param name="IP">IP-адрес.</param>
        public IONode(string typeStr, int n, string IP, string name)
        {
            this.typeStr = typeStr;
            switch (typeStr)
            {
                case "750-863":
                    type = TYPES.T_INTERNAL_750_86x;
                    break;

                case "750-341":
                case "750-841":
                case "750-352":
                    type = TYPES.T_ETHERNET;
                    break;

                case "750-8202":
                case "750-8203":
                case "750-8204":
                case "750-8206":
                    type = TYPES.T_INTERNAL_750_820x;
                    break;

                case "AXL F BK ETH":
                    type = TYPES.T_PHOENIX_CONTACT;
                    break;
                case "AXC F 2152":
                    type = TYPES.T_PHOENIX_CONTACT_MAIN;
                    break;

                default:
                    type = TYPES.T_EMPTY;
                    break;
            }

            this.IP = IP;
            this.n = n;
            this.name = name;

            iOModules = new List<IOModule>();

            DI_count = 0;
            DO_count = 0;
            AI_count = 0;
            AO_count = 0;
        }

        /// <summary>
        /// Получение списка модулей по имени.
        /// </summary>
        public Dictionary<IOModule, int> GetModulesList(string name)
        {
            Dictionary<IOModule, int> modulesList = new Dictionary<IOModule, int>();
            for (int i = 0; i < IOModules.Count; i++)
            {
                if (IOModules[i].Info.Name == name)
                {
                    modulesList.Add(IOModules[i], i);
                }
            }
            return modulesList;
        }

        /// <summary>
        /// Добавление модуль.
        /// </summary>
        /// <param name="iOModule">Добавляемый модуль.</param>
        private void AddModule(IOModule iOModule)
        {
            iOModules.Add(iOModule);
        }

        /// <summary>
        /// Добавление модуля в узел в заданную позицию.
        /// </summary>
        /// <param name="iOModule">Вставляемый модуль.</param>
        /// <param name="position">Позиция модуля, начиная с 1.</param>
        public void SetModule(IOModule iOModule, int position)
        {
            if (iOModules.Count < position)
            {
                for (int i = iOModules.Count; i < position; i++)
                {
                    iOModules.Add(new IOModule(0, 0, null));
                }
            }

            iOModules[position - 1] = iOModule;
        }

        /// <summary>
        /// Добавление модуля в узел в заданную позицию.
        /// </summary>
        /// <param name="iOModule">Вставляемый модуль.</param>
        /// <param name="position">Позиция модуля, начиная с 1.</param>
        public void InsertModule(IOModule iOModule, int position)
        {
            if (iOModules.Count < position)
            {
                for (int i = iOModules.Count; i < position; i++)
                {
                    iOModules.Add(new IOModule(0, 0, null));
                }
            }

            iOModules.Insert(position, iOModule);

        }

        /// <summary>
        /// Получение модуля.
        /// </summary>
        /// <param name="iONode">Индекс модуля.</param>
        /// <returns>Модуль с заданным индексом.</returns>
        public IOModule this[int idx]
        {
            get
            {
                if (idx >= iOModules.Count || idx < 0)
                {
                    return null;
                }
                else
                {
                    return iOModules[idx];
                }
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        public string SaveAsLuaTable(string prefix)
        {
            string str = prefix + "{\n";
            str += prefix + "name    = \'" + name + "\',\n";
            str += prefix + "ntype   = " + (int)type + ", " + "--" + typeStr + "\n";
            str += prefix + "n       = " + n + ",\n";
            str += prefix + "IP      = \'" + IP + "\',\n";
            str += prefix + "modules =\n";
            str += prefix + "\t{\n";

            foreach (IOModule module in iOModules)
            {
                if (module != null)
                {
                    str += module.SaveAsLuaTable(prefix + "\t") + ",\n";
                }
                else
                {
                    str += prefix + "\t" + "{}" + ",\n";
                }
            }

            str += prefix + "\t}\n";
            str += prefix + "}";

            return str;
        }

        public void SaveAsConnectionArray(ref object[,] res, ref int idx, Dictionary<string, int> modulesCount,
            Dictionary<string, Color> modulesColor, int nodeIdx, Dictionary<string, object[,]> asInterfaceConnection)
        {
            for (int i = 0; i < iOModules.Count; i++)
            {
                iOModules[i].SaveAsConnectionArray(ref res, ref idx, i + 1, modulesCount, modulesColor);
                if (iOModules[i].Info.AddressSpaceType == IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI ||
                    iOModules[i].Info.AddressSpaceType == IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI)
                {
                    iOModules[i].SaveASInterfaceConnection(nodeIdx, i + 1, asInterfaceConnection);
                }
            }

        }

        /// <summary>
        /// Типы модулей.
        /// </summary>
        public enum TYPES
        {
            T_EMPTY = -1,            /// Не определен.

            T_INTERNAL_750_86x,      /// Модули в управляющем контроллере 750-863.

            T_INTERNAL_750_820x = 2, /// Модули в управляющем контроллере PFC200.

            T_ETHERNET = 100,        /// Удаленный Ethernet узел.             

            T_PHOENIX_CONTACT = 200, /// Модули Phoenix Contact.

            T_PHOENIX_CONTACT_MAIN = 201, /// Модуль Phoenix Contact с управляющей программой.
        };

        public int DI_count { get; set; } ///Количество дискретных входов.
        public int DO_count { get; set; } ///Количество дискретных выходов.
        public int AI_count { get; set; } ///Количество аналоговых входов.
        public int AO_count { get; set; } ///Количество аналоговых выходов.

        public List<IOModule> IOModules { get { return iOModules; } }

        public string IP_address { get { return IP; } }

        public TYPES Type { get { return type; } }

        public string TypeStr { get { return typeStr; } }

        public int N { get { return n; } }

        #region Закрытые поля.
        private List<IOModule> iOModules;      ///Модули узла.
        private string typeStr;               ///Тип узла (строка).
        private TYPES type;                   ///Тип узла.
        private string IP;                    ///IP.
        private int n;                        ///Номер.
        private string name;                  ///Имя узла (прим.,A100)
        #endregion
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Интерфейс менеджера описания IO для проекта.
    /// </summary>
    public interface IIOManager
    {

        /// <summary>
        /// Получение описания IO на основе проекта.
        /// </summary>
        void ReadConfiguration();

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        string SaveAsLuaTable(string prefix);

        /// <summary>
        /// Обновление подписи к клеммам модулей IO
        /// в соответствии с актуальным названием устройства.
        /// </summary>
        string UpdateModulesBinding();
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Все узлы модулей ввода-вывода IO. Содержит минимальную функциональность, 
    /// необходимую для экспорта для PAC.
    /// </summary>
    class IOManager
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        private IOManager()
        {
            iONodes = new List<IONode>();
        }

        /// <summary>
        /// Получение экземпляра класса.
        /// </summary>
        /// <returns>Единственный экземпляр класса.</returns>
        public static IOManager GetInstance()
        {
            if (null == instance)
            {
                instance = new IOManager();
            }

            return instance;
        }

        /// <summary>
        /// Получение модуля по номеру узла и смещение (поддержка считывания
        /// информации об устройствах из старого описания *.ds4). 
        /// </summary>        
        /// <param name="n">Номер (c единицы).</param>
        /// <param name="offset">Смещение.</param>
        /// <param name="addressSpaceType">Тип адресного пространства.</param>
        public IOModule GetModuleByOffset(int n, int offset,
            IO.IOModuleInfo.ADDRESS_SPACE_TYPE addressSpaceType)
        {
            IOModule res = null;

            if (iONodes.Count >= n && n > 0)
            {
                int idx = 0;

                foreach (IOModule module in IONodes[n - 1].IOModules)
                {
                    if (module.Info.AddressSpaceType == addressSpaceType)
                    {
                        int moduleOffset = 0;
                        switch (addressSpaceType)
                        {
                            case IOModuleInfo.ADDRESS_SPACE_TYPE.DI:
                            case IOModuleInfo.ADDRESS_SPACE_TYPE.AI:
                                moduleOffset = module.InOffset;
                                break;

                            case IOModuleInfo.ADDRESS_SPACE_TYPE.DO:
                            case IOModuleInfo.ADDRESS_SPACE_TYPE.AO:
                                moduleOffset = module.OutOffset;
                                break;
                        }

                        if (moduleOffset > offset)
                        {
                            break;
                        }
                        else
                        {
                            res = module;
                        }
                    }

                    idx++;
                }
            }

            return res;
        }

        /// <summary>
        /// Получить модуль ввода-вывода по его физическому номеру (прим., 202)
        /// </summary>
        /// <param name="number">Физический номер</param>
        /// <returns>Модуль ввода-вывода</returns>
        public IOModule GetModuleByPhysicalNumber(int number)
        {
            IOModule findedModule = null;
            foreach (IONode node in iONodes)
            {
                foreach (IOModule module in node.IOModules)
                {
                    if (module.PhysicalNumber == number)
                    {
                        return module;
                    }
                }
            }

            if (findedModule == null)
            {
                const string Message = "Модуль не найден";
                throw new Exception(Message);
            }
            return findedModule;
        }

        /// <summary>
        /// Добавление модуля в узел в заданную позицию.
        /// </summary>        
        /// <param name="n">Номер (c единицы).</param>
        /// <param name="type">Тип (например 750-352).</param>
        /// <param name="IP">IP-адрес.</param>
        public void AddNode(int n, string type, string IP, string name)
        {
            if (iONodes.Count < n)
            {
                for (int i = iONodes.Count; i < n; i++)
                {
                    iONodes.Add(new IONode("750-xxx", i + 1, "", ""));
                }
            }

            iONodes[n - 1] = new IONode(type, n, IP, name);
        }

        /// <summary>
        /// Получение узла.
        /// </summary>
        /// <param name="iONode">Индекс узла.</param>
        /// <returns>Узел с заданным индексом.</returns>
        public IONode this[int idx]
        {
            get
            {
                if (idx >= iONodes.Count || idx < 0)
                {
                    return null;
                }
                else
                {
                    return iONodes[idx];
                }
            }
        }

        /// <summary>
        /// Сброс информации о модулях IO.
        /// </summary>
        public void Clear()
        {
            iONodes.Clear();
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        public string SaveAsLuaTable(string prefix)
        {
            string str = "--Узлы IO\n" +
                "nodes =\n" + "\t{\n";
            foreach (IONode node in iONodes)
            {
                if (node == null)
                {
                    continue;
                }

                str += node.SaveAsLuaTable("\t\t") + ",\n";
            }
            str += "\t}\n";

            str = str.Replace("\t", "    ");
            return str;
        }

        /// <summary>
        /// Проверка корректного заполнения узлами.
        /// </summary>
        public string Check()
        {
            string str = "";

            int idx = 100;
            foreach (IONode node in iONodes)
            {
                if (node != null && node.Type == IONode.TYPES.T_EMPTY)
                {
                    str += "Отсутствует узел \"A" + idx + "\".\n";
                }
                idx += 100;

                foreach (IONode node2 in iONodes)
                {
                    if (node == node2) continue;

                    if (node.IP_address == node2.IP_address && node.IP_address != "")
                    {
                        str += "\"A" + 100 * node.N + "\" : IP адрес совпадает с \"A" +
                            100 * node2.N + "\" - " + node.IP_address + ".\n";
                    }
                }

            }

            return str;
        }

        public object[,] SaveAsConnectionArray(string prjName, Dictionary<string, int> modulesCount, Dictionary<string, Color> modulesColor, Dictionary<string, object[,]> asInterfaceConnection)
        {
            const int MAX_COL = 4;
            int MAX_ROW = iONodes.Count;
            foreach (IONode ioNode in iONodes)
            {
                MAX_ROW += ioNode.IOModules.Count;
            }
            MAX_ROW *= 16;
            object[,] res = new object[MAX_ROW, MAX_COL];
            int idx = 0;
            for (int i = 0; i < iONodes.Count; i++)
            {
                res[idx, 3] = prjName;
                idx++;
                DateTime localDate = DateTime.Now;
                res[idx, 3] = localDate.ToString(new CultureInfo("ru-RU"));
                string nodeName = "Узел №" + (i + 1).ToString() + " Адрес: " + iONodes[i].IP_address;
                res[idx, 0] = nodeName;
                idx++;

                res[idx, 0] = 0;
                nodeName = iONodes[i].TypeStr.Replace("750-", "");
                res[idx, 1] = nodeName;

                if (!modulesColor.ContainsKey(nodeName))
                {
                    modulesColor.Add(nodeName, Color.Gray);
                }

                idx++;

                iONodes[i].SaveAsConnectionArray(ref res, ref idx, modulesCount, modulesColor, i + 1, asInterfaceConnection);
            }


            return res;
        }

        public List<IONode> IONodes
        {
            get
            {
                return iONodes;
            }
        }

        /// <summary>
        /// Номера IO-Link модулей, которые используются
        /// </summary>
        public enum IOLinkModules
        {
            Wago = 657,
            PhoenixContactSmart = 1088132,
            PhoenixContactStandard = 1027843,
        }

        #region Закрытые поля.
        private List<IONode> iONodes;     ///Узлы проекта.
        private static IOManager instance;  ///Экземпляр класса.
        #endregion
    }

    /// <summary>
    /// Класс, рассчитывающий IO-Link адреса
    /// </summary>
    sealed class IOLinkCalculator
    {
        /// <summary>
        /// Закрытый конструктор для безопасности.
        /// </summary>
        private IOLinkCalculator() { }

        /// <summary>
        /// Конструктор стандартный
        /// </summary>
        /// <param name="devices">Список привязанных устройств</param>
        /// <param name="devicesChannels">Список привязанных каналов</param>
        /// <param name="moduleInfo">Информация о модуле ввода-вывода</param>
        public IOLinkCalculator(List<Device.IODevice>[] devices, 
            List<Device.IODevice.IOChannel>[] devicesChannels,
            IOModuleInfo moduleInfo) 
        {
            this.devices = devices;
            this.devicesChannels = devicesChannels;
            this.moduleInfo = moduleInfo;
        }

        public void Calculate()
        {
            int moduleNumber = moduleInfo.Number;

            switch (moduleNumber)
            {
                case (int) IOManager.IOLinkModules.Wago:
                    CalculateForWago();
                    break;

                case (int) IOManager.IOLinkModules.PhoenixContactStandard:
                    CalculateForPhoenixContact();
                    break;

                case (int) IOManager.IOLinkModules.PhoenixContactSmart:
                    // TODO: Будет добавлено после появления SMART модуля
                    break;
            }
        }

        /// <summary>
        /// Расчет для IO-Link модуля от Wago.
        /// </summary>
        private void CalculateForWago() 
        {
            foreach (int clamp in moduleInfo.ChannelClamps)
            {
                if (devices[clamp] != null && devices[clamp][0] != null)
                {
                    moduleInfo.ChannelAddressesIn[clamp] = offsetIn;
                    moduleInfo.ChannelAddressesOut[clamp] = offsetOut;
                    offsetIn += devices[clamp][0].IOLinkSizeIn;
                    offsetOut += devices[clamp][0].IOLinkSizeOut;
                }
            }
        }

        /// <summary>
        /// Расчет для стандартного IO-Link модуля от Phoenix Contact.
        /// </summary>
        private void CalculateForPhoenixContact() 
        {
            foreach (int clamp in moduleInfo.ChannelClamps)
            {
                if (devices[clamp] != null && devices[clamp][0] != null)
                {
                    int deviceOffset;
                    Device.IODevice.IOChannel channel = 
                        devicesChannels[clamp][0];
                    Device.IODevice device = devices[clamp][0];
                    if (channel.Name == "DI" || channel.Name == "DO")
                    {
                        int moduleOffset = channel.ModuleOffset;
                        int logicalClamp = channel.LogicalClamp;
                        int discreteOffset = CalculateDiscreteOffsetForIOLink(
                            moduleOffset, logicalClamp);
                        moduleInfo.ChannelAddressesIn[clamp] = discreteOffset;
                        moduleInfo.ChannelAddressesOut[clamp] = discreteOffset;
                    }
                    else
                    {
                        moduleInfo.ChannelAddressesIn[clamp] = offsetIn;
                        moduleInfo.ChannelAddressesOut[clamp] = offsetOut;
                        deviceOffset = device.GetMaxIOLinkSize();
                        offsetIn += deviceOffset;
                        offsetOut += deviceOffset;
                    }
                }
            }
        }

        /// <summary>
        /// Расчет дискретного смещения для IO-Link модуля Phoenix Contact.
        /// </summary>
        /// <param name="logicalClamp">Порядковый номер привязанной 
        /// клеммы</param>
        /// <param name="moduleOffset">Начало смещения модуля в словах (word)
        /// </param>
        /// <returns>Дискретное смещение</returns>
        private int CalculateDiscreteOffsetForIOLink(int moduleOffset,
            int logicalClamp)
        {
            int convertToBytes = moduleOffset * 2;
            int startingDiscreteOffsetInBytes = convertToBytes + 2;
            int convertToBits = startingDiscreteOffsetInBytes * 8;
            logicalClamp -= 1;
            int discreteOffset = convertToBits + logicalClamp;

            return discreteOffset;
        }

        List<Device.IODevice>[] devices;
        List<Device.IODevice.IOChannel>[] devicesChannels;
        IOModuleInfo moduleInfo;

        // Сервисные слова.
        int offsetIn = 3;
        int offsetOut = 3;
    }
}
