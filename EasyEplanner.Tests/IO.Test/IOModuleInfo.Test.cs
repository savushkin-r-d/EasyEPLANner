using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using IO;
using NUnit.Framework;

namespace Tests.IO
{
    public class IOModuleInfoTest
    {
        // TODO: Add module info test

        public void TestGetModuleInfo(string expectedModuleName)
        {
            // TODO: Arrange modules info

            var moduleInfo = IOModuleInfo
                .GetModuleInfo(expectedModuleName, out bool isStub);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedModuleName, moduleInfo.Name);
                Assert.IsFalse(isStub);
            });
        }

        public void TestClone(IOModuleInfo testModule)
        {
            var cloned = (IOModuleInfo)testModule.Clone();

            bool isEqual =
                testModule.AddressSpaceType == cloned.AddressSpaceType &&
                testModule.AICount == cloned.AICount &&
                testModule.AOCount == cloned.AOCount &&
                testModule.DICount == cloned.DICount &&
                testModule.DOCount == cloned.DOCount &&
                testModule.ChannelAddressesIn == cloned.ChannelAddressesIn &&
                testModule.ChannelAddressesOut == cloned.ChannelAddressesOut &&
                testModule.ChannelClamps == cloned.ChannelClamps &&
                testModule.Description == cloned.Description &&
                testModule.GroupName == cloned.GroupName &&
                testModule.ModuleColor == testModule.ModuleColor &&
                testModule.Name == cloned.Name &&
                testModule.Number == cloned.Number &&
                testModule.TypeName == cloned.TypeName;

            Assert.IsTrue(isEqual);
        }

        public void TestSetGetName(IOModuleInfo testModule, string expected,
            string actual)
        {
            testModule.Name = actual;
            Assert.AreEqual(expected, testModule.Name);
        }

        public void TestSetGetNumber(IOModuleInfo testModule, int expected,
            int actual)
        {
            testModule.Number = actual;
            Assert.AreEqual(expected, testModule.Number);
        }

        public void TestSetGetDescription(IOModuleInfo testModule,
            string expected, string actual)
        {
            testModule.Description = actual;
            Assert.AreEqual(expected, testModule.Description);
        }

        public void TestSetGetAddressSpaceType(IOModuleInfo testModule,
            IOModuleInfo.ADDRESS_SPACE_TYPE expected,
            IOModuleInfo.ADDRESS_SPACE_TYPE actual)
        {
            testModule.AddressSpaceType = actual;
            Assert.AreEqual(expected, testModule.AddressSpaceType);
        }

        public void TestSetGetChannelClamps(IOModuleInfo testModule,
            int[] expected, int[] actual)
        {
            testModule.ChannelClamps = actual;
            Assert.AreEqual(expected, testModule.ChannelClamps);
        }

        public void TestSetGetChannelAddressesOut(IOModuleInfo testModule,
            int[] expected, int[] actual)
        {
            testModule.ChannelAddressesOut = actual;
            Assert.AreEqual(expected, testModule.ChannelAddressesOut);
        }

        public void TestSetGetChannelAddressesIn(IOModuleInfo testModule,
            int[] expected, int[] actual)
        {
            testModule.ChannelAddressesIn = actual;
            Assert.AreEqual(expected, testModule.ChannelAddressesIn);
        }

        public void TestSetGetAICount(IOModuleInfo testModule,
            int expected, int actual)
        {
            testModule.AICount = actual;
            Assert.AreEqual(expected, testModule.DOCount);
        }

        public void TestSetGetAOCount(IOModuleInfo testModule,
            int expected, int actual)
        {
            testModule.AOCount = actual;
            Assert.AreEqual(expected, testModule.DOCount);
        }

        public void TestSetGetDICount(IOModuleInfo testModule,
            int expected, int actual)
        {
            testModule.DICount = actual;
            Assert.AreEqual(expected, testModule.DOCount);
        }

        public void TestSetGetDOCount(IOModuleInfo testModule,
            int expected, int actual)
        {
            testModule.DOCount = actual;
            Assert.AreEqual(expected, testModule.DOCount);
        }

        public void TestSetGetTypeName(IOModuleInfo testModule,
            string expected, string actual)
        {
            testModule.TypeName = actual;
            Assert.AreEqual(expected, testModule.TypeName);
        }

        public void TestSetGetModuleColor(IOModuleInfo testModule,
            Color expected, Color actual)
        {
            testModule.ModuleColor = actual;
            Assert.AreEqual(expected, testModule.ModuleColor);
        }

        public void TestSetGetGroupName(IOModuleInfo testModule,
            string expected, string actual)
        {
            testModule.GroupName = actual;
            Assert.AreEqual(expected, testModule.GroupName);
        }

        // TODO: Write getter for simple IOModule for test getters and setters
    }
}
