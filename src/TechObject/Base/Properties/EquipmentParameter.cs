using EasyEPlanner;
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
            IDeviceSynchronizeService synchronizer = DeviceSynchronizer
                .GetSynchronizeService();
            synchronizer.SynchronizeDevices(array, ref devicesIndexes);

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
            devicesIndexes.Clear();
            parameterIndexes.Clear();

            bool emptyOrDefault = newValue == string.Empty ||
                newValue == DefaultValue;
            if (emptyOrDefault)
            {
                SetValue(newValue.Trim());
                return true;
            }

            List<string> values = newValue.Split(' ').ToList();

            List<int> devices = GetDevicesIndexes(values);
            List<int> parameters = GetParametersIndexes(values);

            devicesIndexes.AddRange(devices);
            parameterIndexes.AddRange(parameters);

            devicesIndexes.Sort();
            parameterIndexes.Sort();

            SetValue(GetDevicesAndParametersString());

            return true;
        }

        /// <summary>
        /// Получить индексы параметров
        /// </summary>
        /// <param name="values">Список значений</param>
        /// <returns></returns>
        private List<int> GetParametersIndexes(List<string> values)
        {
            var indexes = new List<int>();
            var equipment = Owner as Equipment;
            if(equipment != null)
            {
                TechObject techObject = equipment.Owner;
                if(techObject != null)
                {
                    foreach(var value in values)
                    {
                        Param param = techObject.GetParamsManager()
                            .Float.GetParam(value);
                        if(param == null)
                        {
                            continue;
                        }

                        indexes.Add(param.GetParameterNumber - 1);
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
        /// Получить строку с параметрами
        /// </summary>
        /// <returns></returns>
        private string GetParametersString()
        {
            var parameters = new List<string>();
            if (Owner is Equipment equipment)
            {
                var techObject = equipment.Owner;
                if (techObject != null)
                {
                    foreach (var paramIndex in parameterIndexes)
                    {
                        Param param = techObject.GetParamsManager()
                            .Float.GetParam(paramIndex);
                        parameters.Add(param.GetNameLua());
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
        #endregion

        /// <summary>
        /// Индексы параметров
        /// </summary>
        private List<int> parameterIndexes;
    }
}
