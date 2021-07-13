using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Device
{
    /// <summary>
    /// ПИД-регулятор
    /// </summary>
    public class C : IODevice
    {
        public C(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber) : base(name,
                eplanName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.C;

            properties.Add(Property.IN_VALUE, null);
            properties.Add(Property.OUT_VALUE, null);

            parameters.Add(Parameter.P_k, 1);
            parameters.Add(Parameter.P_Ti, 15);
            parameters.Add(Parameter.P_Td, 0.01);
            parameters.Add(Parameter.P_dt, 1000);

            parameters.Add(Parameter.P_max, 100);
            parameters.Add(Parameter.P_min, 0);

            parameters.Add(Parameter.P_acceleration_time, 30);
            parameters.Add(Parameter.P_is_manual_mode, 0);
            parameters.Add(Parameter.P_U_manual, 65);

            parameters.Add(Parameter.P_k2, 0);
            parameters.Add(Parameter.P_Ti2, 0);
            parameters.Add(Parameter.P_Td2, 0);

            parameters.Add(Parameter.P_out_max, 100);
            parameters.Add(Parameter.P_out_min, 0);

            parameters.Add(Parameter.P_is_reverse, 0);
            parameters.Add(Parameter.P_is_zero_start, 1);
        }

        public override string GetDeviceSubTypeStr(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.C:
                    return dt.ToString();
            }

            return string.Empty;
        }

        public override Dictionary<string, int> GetDeviceProperties(
            DeviceType dt, DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.C:
                    return new Dictionary<string, int>()
                    {
                        {Tag.ST, 1},
                        {Tag.M, 1},
                        {Tag.V, 1},
                        {Tag.Z, 1},
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
            var devTagsNames = GetDeviceProperties(DeviceType, DeviceSubType)
                .Keys;
            var devParameters = new List<string>();
            devParameters.AddRange(devTagsNames);
            devParameters.AddRange(parameters.Keys);

            TreeNode newNode;
            if (!rootNode.Nodes.ContainsKey(Name))
            {
                newNode = rootNode.Nodes.Add(Name, Name);
            }
            else
            {
                bool searchChildren = false;
                newNode = rootNode.Nodes.Find(Name, searchChildren)
                    .First();
            }

            foreach (var parName in devParameters)
            {
                newNode.Nodes.Add($"{Name}.{parName}", $"{Name}.{parName}");
            }
        }
        #endregion
    }
}
