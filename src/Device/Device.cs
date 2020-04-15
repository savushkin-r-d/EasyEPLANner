﻿using System;

namespace Device
{
    /// <summary>
    /// Устройство - клапан, насос и т.д.
    /// </summary>
    public class Device : IComparable, IEquatable<Device>
    {

        public static int Compare(Device dx, Device dy)
        {
            if (dx == null && dy == null)
                return 0;

            if (dx == null)
                return -1;

            if (dy == null)
                return 1;


            if (dx.dType == dy.dType)
            {
                if (dx.objectName == dy.objectName)
                {
                    if (dx.ObjectNumber == dy.ObjectNumber)
                    {
                        if (dx.DeviceNumber == dy.DeviceNumber)
                        {
                            return 0;
                        }
                        else
                        {
                            return dx.DeviceNumber.CompareTo(dy.DeviceNumber);
                        }
                    }
                    else
                    {
                        return dx.ObjectNumber.CompareTo(dy.ObjectNumber);
                    }
                }
                else
                {
                    return dx.objectName.CompareTo(dy.objectName);
                }
            }
            else
            {
                return dx.dType.CompareTo(dy.dType);
            }
        }
        /// <summary>
        /// Конструктор на основе имени.
        /// </summary>
        /// <param name="name">Имя устройства (формат - А1V12).</param>
        /// <param name="description">Описание устройства.</param>
        protected Device(string fullName, string description,
            int deviceNumber, string objectName, int objectNumber)
        {
            this.name = fullName;
            this.description = description;

            this.deviceNumber = deviceNumber;

            this.objectName = objectName;
            this.objectNumber = objectNumber;
        }

        /// <summary>
        /// Установка подтипа устройства.
        /// </summary>
        public virtual string SetSubType(string subType)
        {
            try
            {
                dSubType = (DeviceSubType)Enum.Parse(typeof(DeviceSubType),
                    subType.ToUpper());
            }
            catch
            {
                dSubType = DeviceSubType.NONE;
            }

            return "";
        }

        /// <summary>
        /// Получение типа подключения для устройства
        /// </summary>
        public virtual string GetConnectionType()
        {
            return "";
        }

        /// <summary>
        /// Получение диапазона настройки
        /// </summary>
        public virtual string GetRange()
        {
            return "";
        }

        public Device Clone()
        {
            return (Device)MemberwiseClone();
        }

        #region Реализация интерфейсов IComparable, IEquatable<Device>.
        /// <summary>
        /// Метод проверки равенства объектов (для поиска).
        /// </summary>
        public bool Equals(Device otherDevice)
        {
            if (otherDevice == null)
            {
                return false;
            }

            if (name.Equals(otherDevice.name))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод проверки равенства объектов (для поиска).
        /// </summary>
        public override bool Equals(Object otherDevice)
        {
            if (otherDevice == null)
                return base.Equals(otherDevice);

            return Equals(otherDevice as Device);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        /// <summary>
        /// Метод сравнения объектов (для сортировки).
        /// </summary>
        public int CompareTo(object otherDevice)
        {
            if (otherDevice == null)
                return 1;

            Device obj = otherDevice as Device;

            if (dType == obj.dType)
            {
                return string.CompareOrdinal(name, obj.name);
            }
            else
            {
                return dType.CompareTo(obj.dType);
            }
        }
        #endregion

        /// <summary>
        /// Имя устройства (например - А1V12).
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Имя устройства (например "+А1-V12").
        /// </summary>
        public string EPlanName
        {
            get
            {
                return "+" + objectName +
                    (objectNumber > 0 ? objectNumber.ToString() : "") +
                    "-" + DeviceType + DeviceNumber;
            }
        }

        /// <summary>
        /// Номер устройства.
        /// </summary>
        public long DeviceNumber
        {
            get
            {
                return deviceNumber;
            }
        }

        /// <summary>
        /// Тип устройства.
        /// </summary>
        public DeviceType DeviceType
        {
            get
            {
                return dType;
            }
        }

        /// <summary>
        /// Тип устройства.
        /// </summary>
        public DeviceSubType DeviceSubType
        {
            get
            {
                return dSubType;
            }
        }

        /// <summary>
        /// Описание устройства.
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
        }

        /// <summary>
        /// Объект устройство.
        /// </summary>
        public string ObjectName
        {
            get
            {
                return objectName;
            }
        }

        /// <summary>
        /// Номер объекта устройства.
        /// </summary>
        public int ObjectNumber
        {
            get
            {
                return objectNumber;
            }
        }

        /// <summary>
        /// Отладочное строковое представление устройства.
        /// </summary>
        public virtual string DPrint()
        {
            return String.Format("{0,8} {1} ", name, description);
        }

        #region Защищенные поля.

        protected internal string name; /// Имя устройства (А1V12).

        protected int deviceNumber;     /// Номер устройства (12 в R1V12).

        protected DeviceType dType = DeviceType.NONE;
        protected DeviceSubType dSubType = DeviceSubType.NONE;

        protected int dLocation; /// Номер узла, в котором должно располагаться устройство.

        protected string objectName;    /// Объект устройства (R в R1V12).
        protected int objectNumber;     /// Номер объекта (1 в R1V12)

        protected internal string description;  /// Описание устройства (пример - "Отсечной клапан линии").   
        #endregion
    }
}
