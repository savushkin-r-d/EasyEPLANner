using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Действие над устройствами (включение, выключение и т.д.).
    /// </summary>
    public class Action : Editor.TreeViewItem
    {
        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться 
        /// в таблице Lua.</param>
        /// <param name="devTypes">Типы устройств, допустимые для 
        /// редактирования.</param>
        /// <param name="devSubTypes">Подтипы устройств, допустимые 
        /// для редактирования.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public Action(string name, Step owner, string luaName = "",
            Device.DeviceType[] devTypes = null,
            Device.DeviceSubType[] devSubTypes = null)
        {
            this.name = name;
            this.luaName = luaName;
            this.devTypes = devTypes;
            this.devSubTypes = devSubTypes;
            this.deviceIndex = new List<int>();
            this.owner = owner;

            DrawStyle = Editor.DrawInfo.Style.GREEN_BOX;
        }

        public virtual Action Clone()
        {
            Action clone = (Action)MemberwiseClone();

            clone.deviceIndex = new List<int>();
            foreach (int index in deviceIndex)
            {
                clone.deviceIndex.Add(index);
            }

            return clone;
        }

        virtual public void ModifyDevNames(int newTechObjectN,
            int oldTechObjectN, string techObjectName)
        {
            List<int> tmpIndex = new List<int>();
            foreach (int index in deviceIndex)
            {
                tmpIndex.Add(index);
            }

            Device.DeviceManager deviceManager = Device.DeviceManager
                .GetInstance();
            foreach (int index in deviceIndex)
            {
                var newDevName = string.Empty;
                Device.IODevice device = deviceManager.GetDeviceByIndex(index);
                int objNum = device.ObjectNumber;
                string objName = device.ObjectName;

                if (objNum > 0)
                {
                    //Для устройств в пределах объекта меняем номер объекта.
                    if (techObjectName == objName)
                    {
                        // COAG2V1 --> COAG1V1
                        if (objNum == newTechObjectN && oldTechObjectN != -1)
                        {
                            newDevName = objName + oldTechObjectN +
                                device.DeviceType.ToString() + device.
                                DeviceNumber;
                        }
                        if (oldTechObjectN == -1 ||
                            oldTechObjectN == objNum)
                        {
                            //COAG1V1 --> COAG2V1
                            newDevName = objName + newTechObjectN +
                                device.DeviceType.ToString() + device
                                .DeviceNumber;
                        }
                    }
                }

                if (newDevName != string.Empty)
                {
                    int indexOfDeletingElement = tmpIndex.IndexOf(index);
                    tmpIndex.Remove(index);
                    int tmpDevInd = Device.DeviceManager.GetInstance()
                        .GetDeviceIndex(newDevName);
                    if (tmpDevInd >= 0)
                    {
                        tmpIndex.Insert(indexOfDeletingElement, tmpDevInd);
                    }
                }
            }

            deviceIndex = tmpIndex;
        }

        virtual public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName, 
            int oldTechObjNumber)
        {
            List<int> tmpIndex = new List<int>();
            foreach (int index in deviceIndex)
            {
                tmpIndex.Add(index);
            }

            Device.DeviceManager deviceManager = Device.DeviceManager
                .GetInstance();
            foreach (int index in deviceIndex)
            {
                string newDevName = string.Empty;
                Device.IODevice device = deviceManager.GetDeviceByIndex(index);
                int objNum = newTechObjectNumber;
                string objName = device.ObjectName;

                if (objName == oldTechObjectName &&
                    device.ObjectNumber == oldTechObjNumber)
                {
                    newDevName = newTechObjectName + objNum +
                        device.DeviceType.ToString() + device.DeviceNumber;
                }

                if (newDevName != string.Empty)
                {
                    int indexOfDeletingElement = tmpIndex.IndexOf(index);
                    tmpIndex.Remove(index);
                    int tmpDevInd = Device.DeviceManager.GetInstance()
                        .GetDeviceIndex(newDevName);
                    if (tmpDevInd >= 0)
                    {
                        tmpIndex.Insert(indexOfDeletingElement, tmpDevInd);
                    }
                }
            }

            deviceIndex = tmpIndex;
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public virtual string SaveAsLuaTable(string prefix)
        {
            if (deviceIndex.Count == 0)
            {
                return "";
            }

            string res = prefix;
            if (LuaName != "")
            {
                res += LuaName + " = ";
            }

            res += "--" + name + "\n" + prefix + "\t{\n";

            res += prefix + "\t";
            Device.DeviceManager deviceManager = Device.DeviceManager.
                GetInstance();
            int devicesCounter = 0;
            foreach (int index in deviceIndex)
            {
                if (deviceManager.GetDeviceByIndex(index).Name != "Заглушка")
                {
                    devicesCounter++;
                    res += "'" + deviceManager.GetDeviceByIndex(index).Name +
                        "', ";
                }
            }

            if (devicesCounter == 0)
            {
                return "";
            }

            res = res.Remove(res.Length - 2, 2);
            res += "\n";

            res += prefix + "\t},\n";
            return res;
        }

        /// <summary>
        /// Добавление устройства к действию.
        /// </summary>
        /// <param name="device">Устройство.</param>
        /// <param name="additionalParam">Дополнительный параметр.</param>
        public virtual void AddDev(int index, int additionalParam)
        {
            var device = Device.DeviceManager.GetInstance()
                .GetDeviceByIndex(index);
            if (device.Description != "Заглушка")
            {
                deviceIndex.Add(index);
            }
        }

        /// <summary>
        /// Добавление параметра к действию.
        /// </summary>
        /// <param name="index">Индекс параметра.</param>
        /// <param name="val">Значение параметра.</param>
        public virtual void AddParam(int index, int val)
        {
        }

        /// <summary>
        /// Очищение списка устройств.
        /// </summary>
        virtual public void Clear()
        {
            deviceIndex.Clear();
        }

        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение 
        /// индексов.</param>
        virtual public void Synch(int[] array)
        {
            List<int> del = new List<int>();
            for (int j = 0; j < deviceIndex.Count; j++)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (deviceIndex[j] == i)
                    {
                        // Что бы не учитывало "-2" из array
                        if (array[i] == -1)
                        {
                            del.Add(j);
                            break;
                        }
                        if (array[i] >= 0)
                        {
                            deviceIndex[j] = array[i];
                            break;
                        }
                    }
                }
            }

            int dx = 0;
            foreach (int index in del)
            {
                deviceIndex.RemoveAt(index - dx++);
            }
        }

        /// <summary>
        /// Получение/установка устройств.
        /// </summary>
        public List<int> DeviceIndex
        {
            get
            {
                return deviceIndex;
            }
            set
            {
                deviceIndex = value;
            }
        }

        public string LuaName
        {
            get
            {
                return luaName;
            }
        }

        /// <summary>
        /// Функция проверки добавляемого устройства
        /// </summary>
        /// <param name="deviceName">Имя устройства</param>
        /// <returns></returns>
        private bool ValidateDevice(string deviceName)
        {
            bool isValidType = false;

            Device.Device device = Device.DeviceManager.GetInstance().
                GetDeviceByEplanName(deviceName);
            Device.DeviceType deviceType = device.DeviceType;
            Device.DeviceSubType deviceSubType = device.DeviceSubType;

            Device.DeviceType[] validTypes;
            Device.DeviceSubType[] validSubTypes;
            GetDevTypes(out validTypes, out validSubTypes);

            if (validTypes == null)
            {
                return true;
            }
            else
            {
                foreach (Device.DeviceType type in validTypes)
                {
                    if (type == deviceType)
                    {
                        isValidType = true;
                        break;
                    }
                    else
                    {
                        isValidType = false;
                    }
                }

                if (validSubTypes != null)
                {
                    bool isValidSubType = false;
                    foreach (Device.DeviceSubType subType in validSubTypes)
                    {
                        if ((subType == deviceSubType) && isValidType)
                        {
                            isValidSubType = true;
                        }
                    }

                    if (isValidSubType && isValidSubType)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return isValidType;
        }

        #region Реализация ITreeViewItem

        override public string[] DisplayText
        {
            get
            {
                Device.DeviceManager deviceManager = Device.DeviceManager
                    .GetInstance();
                string res = "";

                foreach (int index in deviceIndex)
                {
                    res += deviceManager.GetDeviceByIndex(index).Name +
                        " ";
                }
                if (res != "")
                {
                    res = res.Remove(res.Length - 1);
                }

                return new string[] { name, res };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return null;
            }
        }

        override public bool SetNewValue(string newName)
        {
            newName = newName.Trim();

            if (newName == "")
            {
                Clear();
                return true;
            }

            Match strMatch = Regex.Match(newName,
                Device.DeviceManager.DESCRIPTION_PATTERN_MULTYLINE,
                RegexOptions.IgnoreCase);
            if (!strMatch.Success)
            {
                return false;
            }

            Match match = Regex.Match(newName,
                Device.DeviceManager.DESCRIPTION_PATTERN, RegexOptions.
                IgnoreCase);
            deviceIndex.Clear();
            while (match.Success)
            {
                string str = match.Groups["name"].Value;

                // Если устройство нельзя вставлять сюда - пропускаем его.
                bool isValid = ValidateDevice(str);
                if (isValid != false)
                {
                    int tmpDeviceIndex = Device.DeviceManager.GetInstance().
                        GetDeviceIndex(str);
                    if (tmpDeviceIndex >= 0)
                    {
                        deviceIndex.Add(tmpDeviceIndex);
                    }
                }

                match = match.NextMatch();
            }

            return true;
        }

        override public bool IsEditable
        {
            get
            {
                return true;
            }
        }

        override public int[] EditablePart
        {
            get
            {//Можем редактировать содержимое второй колонки.
                return new int[] { -1, 1 };
            }
        }

        override public string[] EditText
        {
            get
            {
                Device.DeviceManager deviceManager = Device.DeviceManager
                    .GetInstance();
                string res = "";
                foreach (int index in deviceIndex)
                {
                    res += deviceManager.GetDeviceByIndex(index).Name + " ";
                }

                if (res != "")
                {
                    res = res.Remove(res.Length - 1);
                }

                return new string[] { "", res };
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public bool IsUseDevList
        {
            get
            {
                return true;
            }
        }

        override public void GetDevTypes(out Device.DeviceType[] devTypes,
            out Device.DeviceSubType[] devSubTypes)
        {
            devTypes = this.devTypes;
            devSubTypes = this.devSubTypes;
        }

        override public bool IsDrawOnEplanPage
        {
            get
            {
                return true;
            }
        }

        virtual public Editor.DrawInfo.Style DrawStyle
        {
            get;
            set;
        }

        override public List<Editor.DrawInfo> GetObjectToDrawOnEplanPage()
        {
            var deviceManager = Device.DeviceManager.GetInstance();

            var devToDraw = new List<Editor.DrawInfo>();
            foreach (int index in deviceIndex)
            {
                devToDraw.Add(new Editor.DrawInfo(
                    DrawStyle, deviceManager.GetDeviceByIndex(index)));
            }

            return devToDraw;
        }

        public virtual Device.DeviceSubType[] GetDevSubTypes()
        {
            return devSubTypes;
        }

        public override bool IsFilled
        {
            get
            {
                if (Empty)
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

        public string stepName
        {
            get
            {
                return name;
            }
        }

        public bool Empty
        {
            get
            {
                if (deviceIndex.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        protected string luaName;               /// Имя действия в таблице Lua.
        protected string name;                  /// Имя действия.
        protected List<int> deviceIndex;  /// Список устройств.

        protected Device.DeviceType[] devTypes;
        protected Device.DeviceSubType[] devSubTypes;

        protected Step owner; // Владелец элемента
    }
}
