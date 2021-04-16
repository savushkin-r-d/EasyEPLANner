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
    public class Regulator : IODevice
    {
        public Regulator(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.REGULATOR;
        }

        //TODO: implementation
    }
}
