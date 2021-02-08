using System.Collections.Generic;
using System.Linq;

namespace TechObject
{
    /// <summary>
    /// Параметр оборудования объекта
    /// </summary>
    public class EquipmentParameter : BaseParameter
    {
        public EquipmentParameter(string luaName, string name,
            string defaultValue = "", List<DisplayObject> displayObjects = null)
            : base(luaName, name, defaultValue, displayObjects) 
        {
            deviceIndexes = new List<int>();
            parameterIndexes = new List<int>();
        }

        public override BaseParameter Clone()
        {
            var newProperty = new EquipmentParameter(LuaName, Name,
                DefaultValue, DisplayObjects);
            newProperty.SetNewValue(Value);
            newProperty.NeedDisable = NeedDisable;
            return newProperty;
        }

        #region Синхронизация устройств
        /// <summary>
        /// Синхронизация устройств в объекте
        /// </summary>
        /// <param name="array">Массив с индексами синхронизации</param>
        public override void Synch(int[] array)
        {
            // parameterIndexes - не синхронизируем т.к это не устройства.
            bool noDevices = deviceIndexes.Count <= 0;
            if(noDevices)
            {
                return;
            }

            List<int> del = new List<int>();
            for (int j = 0; j < deviceIndexes.Count; j++)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (deviceIndexes[j] == i)
                    {
                        // Что бы не учитывало "-2" из array
                        if (array[i] == -1)
                        {
                            del.Add(j);
                            break;
                        }
                        if (array[i] >= 0)
                        {
                            deviceIndexes[j] = array[i];
                            break;
                        }
                    }
                }
            }

            int dx = 0;
            foreach (int index in del)
            {
                deviceIndexes.RemoveAt(index - dx++);
            }

            SetValue(GetDevicesAndParametersString());
        }
        #endregion

        public override bool IsEmpty
        {
            get
            {
                bool isEmpty = Value == DefaultValue &&
                    DefaultValue == "";
                if (isEmpty)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #region Реализация ITreeViewItem
        public override string[] DisplayText
        {
            get
            {
                if(Value == "")
                {
                    return new string[] 
                    { 
                        Name, 
                        StaticHelper.CommonConst.StubForCells 
                    };
                }
                else
                {
                    return new string[] { Name, Value };
                }
            }
        }

        public override string[] EditText
        {
            get
            {
                return new string[] { Name, Value };
            }
        }

        public override bool SetNewValue(string newValue)
        {
            deviceIndexes.Clear();
            parameterIndexes.Clear();

            bool emptyOrDefault = newValue == string.Empty ||
                newValue == DefaultValue;
            if (emptyOrDefault)
            {
                SetValue(newValue.Trim());
                return true;
            }

            List<string> values = newValue.Split(' ').ToList();

            List<int> devices = GetDevicesIndexes(ref values);
            List<int> parameters = GetParametersIndexes(ref values);

            deviceIndexes.AddRange(devices);
            parameterIndexes.AddRange(parameters);

            deviceIndexes.Sort();
            parameterIndexes.Sort();

            SetValue(GetDevicesAndParametersString());

            return true;
        }

        /// <summary>
        /// Получить индексы устройств
        /// </summary>
        /// <param name="values">Список значений</param>
        /// <returns></returns>
        private List<int> GetDevicesIndexes(ref List<string> values)
        {
            Device.DeviceManager deviceManager = Device.DeviceManager
                .GetInstance();
            var indexes = new List<int>();
            var copiedValues = new string[values.Count];
            values.CopyTo(copiedValues);

            foreach(var copiedValue in copiedValues) 
            {
                int index = deviceManager.GetDeviceIndex(copiedValue);
                if(index >= 0)
                {
                    indexes.Add(index);
                    values.Remove(copiedValue);
                }
            }

            return indexes;
        }

        /// <summary>
        /// Получить индексы параметров
        /// </summary>
        /// <param name="values">Список значений</param>
        /// <returns></returns>
        private List<int> GetParametersIndexes(ref List<string> values)
        {
            var indexes = new List<int>();
            var equipment = Owner as Equipment;
            if(equipment != null)
            {
                TechObject techObject = equipment.Owner;
                if(techObject != null)
                {
                    var copiedValues = new string[values.Count];
                    values.CopyTo(copiedValues);

                    foreach(var copiedValue in copiedValues)
                    {
                        Param param = techObject.GetParamsManager()
                            .GetParam(copiedValue);
                        if(param == null)
                        {
                            continue;
                        }

                        indexes.Add(param.GetParameterNumber - 1);
                        values.Remove(copiedValue);
                    }
                }
            }

            return indexes;
        }

        /// <summary>
        /// Получить представление значений в виде строки
        /// </summary>
        /// <returns></returns>
        private string GetDevicesAndParametersString()
        {
            string devices = GetDevicesString();
            string parameters = GetParametersString();

            string result = $"{devices} {parameters}";
            return result.Trim();
        }

        /// <summary>
        /// Получить строку с устройствами
        /// </summary>
        /// <returns></returns>
        private string GetDevicesString()
        {
            var devices = new List<string>();
            var deviceManager = Device.DeviceManager.GetInstance();
            foreach (var devIndex in deviceIndexes)
            {
                Device.Device dev = deviceManager.GetDeviceByIndex(devIndex);
                if (dev.Name != StaticHelper.CommonConst.Cap)
                {
                    devices.Add(dev.Name);
                }
            }

            devices = devices.Distinct().ToList();
            return string.Join(" ", devices);
        }

        /// <summary>
        /// Получить строку с параметрами
        /// </summary>
        /// <returns></returns>
        private string GetParametersString()
        {
            var parameters = new List<string>();
            var equipment = Owner as Equipment;
            if (equipment != null)
            {
                var techObject = equipment.Owner;
                if (techObject != null)
                {
                    foreach (var paramIndex in parameterIndexes)
                    {
                        Param param = techObject.GetParamsManager()
                            .GetParam(paramIndex);
                        parameters.Add(param.LuaNameProperty.Value);
                    }
                }
            }

            return string.Join(" ", parameters);
        }

        public override bool IsReplaceable
        {
            get
            {
                return true;
            }
        }

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        public override bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        public override bool IsFilled
        {
            get
            {
                if (Value == "")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        #endregion

        /// <summary>
        /// Индексы устройств
        /// </summary>
        private List<int> deviceIndexes;

        /// <summary>
        /// Индексы параметров
        /// </summary>
        private List<int> parameterIndexes;
    }
}
