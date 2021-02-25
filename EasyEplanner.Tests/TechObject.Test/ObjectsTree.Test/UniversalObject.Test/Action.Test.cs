using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;

namespace Tests.TechObject
{
    class ActionTest
    {
        
    }

    class DefaultActionProcessingStrategyTest
    {
        
    }

    class OneInManyOutActionProcessingStrategyTest
    {
        
    }

    static class DeviceManagerMock
    {
        static DeviceManagerMock()
        {
            var mock = new Mock<Device.IDeviceManager>();
            SetUpMock(mock);
            deviceManager = mock.Object;
        }

        private static void SetUpMock(Mock<Device.IDeviceManager> mock)
        {
            //TODO:
            //DeviceManager mock
            //GetDeviceByEplanName
            //GetDeviceIndex
            //GetDeviceByIndex

            //Devices mocks:
            //DeviceType
            //DeviceSubTypes
            //Description
            //Name
            //DeviceNumber
            //ObjectName

            //Action mock:
            //GetDisplayObjects
        }

        public static Device.IDeviceManager DeviceManager
            => deviceManager;

        private static Device.IDeviceManager deviceManager;
    }
}
