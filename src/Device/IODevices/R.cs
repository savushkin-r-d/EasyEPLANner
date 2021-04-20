using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            parameters.Add("P_k", 1);
            parameters.Add("P_Ti", 15);
            parameters.Add("P_Td", 0.01);
            parameters.Add("P_dt", 1000);

            parameters.Add("P_max", 100);
            parameters.Add("P_min", 0);

            parameters.Add("P_acceleration_time", 30);
            parameters.Add("P_is_manual_mode", 0);
            parameters.Add("P_U_manual", 65);

            parameters.Add("P_k2", 0);
            parameters.Add("P_Ti2", 0);
            parameters.Add("P_Td2", 0);

            parameters.Add("P_out_max", 100);
            parameters.Add("P_out_min", 0);

            parameters.Add("P_is_reverse", 0);
            parameters.Add("P_is_zero_start", 1);
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

        #region сохранение в Lua
        protected override string SaveParameters(string prefix)
        {
            string res = string.Empty;

            if (parameters.Count > 0)
            {
                string tmp = string.Empty;
                foreach (var par in parameters)
                {
                    if (par.Value != null)
                    {
                        tmp += $"{prefix}\t{par.Key} = {par.Value},\n";
                    }
                }

                if (tmp != string.Empty)
                {
                    res += $"{prefix}par =\n";
                    res += $"{prefix}\t{{\n";
                    res += tmp.Remove(tmp.Length - 2) + "\n";
                    res += $"{prefix}\t}}\n";
                }
            }

            return res;
        }
        #endregion

        #region сохранение базы каналов
        public override void GenerateDeviceTags(TreeNode rootNode)
        {
            GenerateDefaultDeviceTags(rootNode);
            GeneratePIDTags(rootNode);
        }

        private void GeneratePIDTags(TreeNode rootNode)
        {
            //TODO: Process channel base
        }
        #endregion
    }
}
