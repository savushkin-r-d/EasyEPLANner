using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace InterprojectExchange
{
    /// <summary>
    /// Компаратор для устройств проектов
    /// </summary>
    class DeviceComparer : IComparer<DeviceInfo>
    {
        public int Compare(DeviceInfo dev1, DeviceInfo dev2)
        {
            var device1 = Regex.Match(dev1.Name, pattern);
            var device2 = Regex.Match(dev2.Name, pattern);
            if (!(device1.Success || device2.Success))
            {
                return 0;
            }

            string dev1Obj = device1.Groups["object"].Value;
            string dev2Obj = device2.Groups["object"].Value;
            if (dev1Obj != dev2Obj)
            {
                return dev1Obj.CompareTo(dev2Obj);
            }

            string dev1ObjNumStr = device1.Groups["object_n"].Value;
            string dev2ObjNumStr = device2.Groups["object_n"].Value;
            if (dev1ObjNumStr != dev2ObjNumStr)
            {
                int dev1ObjNum = int.Parse(dev1ObjNumStr);
                int dev2ObjNum = int.Parse(dev2ObjNumStr);
                return dev1ObjNum.CompareTo(dev2ObjNum);
            }

            string dev1Type = device1.Groups["type"].Value;
            string dev2Type = device2.Groups["type"].Value;
            if (dev1Type != dev2Type)
            {
                return dev1Type.CompareTo(dev2Type);
            }

            string dev1NumStr = device1.Groups["n"].Value;
            string dev2NumStr = device2.Groups["n"].Value;
            if (dev1NumStr != dev2NumStr)
            {
                int dev1Num = int.Parse(dev1NumStr);
                int dev2Num = int.Parse(dev2NumStr);
                return dev1Num.CompareTo(dev2Num);
            }

            return 0;
        }

        public static string RegExpPattern
        {
            get
            {
                return pattern;
            }
        }

        private static string pattern = Device.DeviceManager.DESCRIPTION_PATTERN;
    }
}
