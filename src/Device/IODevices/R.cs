using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    /// <summary>
    /// ПИД-регулятор
    /// </summary>
    public class R : IODevice
    {
        public R(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.R;

            properties.Add("IN_VALUE", null);
            properties.Add("OUT_VALUE", null);

            parameters.Add("k", 1);
            parameters.Add("Ti", 15);
            parameters.Add("Td", 0.01);
            parameters.Add("dt", 1000);

            parameters.Add("max", 100);
            parameters.Add("min", 0);

            parameters.Add("acceleration_time", 30);
            parameters.Add("is_manual_mode", 0);
            parameters.Add("U_manual", 65);

            parameters.Add("is_reverse", 0);
            parameters.Add("is_zero_start", 1);

            parameters.Add("k2", 0);
            parameters.Add("Ti2", 0);
            parameters.Add("Td2", 0);

            parameters.Add("out_max", 100);
            parameters.Add("out_min", 0);
        }

        public override string GetDeviceSubTypeStr(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.R:
                    return dt.ToString();
            }
            return string.Empty;
        }

        public override Dictionary<string, int> GetDeviceProperties(
            DeviceType dt, DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.R:
                    return new Dictionary<string, int>()
                    {
                        {"ST", 1},
                        {"M", 1}, // TODO: Tags
                    };
            }
            return null;
        }
    }
}
