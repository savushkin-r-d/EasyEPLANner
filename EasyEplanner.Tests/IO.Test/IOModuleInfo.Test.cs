using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using IO;
using NUnit.Framework;

namespace Tests.IO
{
    public class IOModuleInfoTest
    {
        // TODO: Add module info test
        public void TestAddModuleInfo(int count, bool repeatable)
        {

        }

        public void TestGetModuleInfo(string expectedModuleName)
        {
            const int countOfModulesInfoForTest = 5;
            FillRandomModulesInfo(countOfModulesInfoForTest);
            if (!string.IsNullOrEmpty(expectedModuleName))
            {
                GetModuleInfoForTest(expectedModuleName);
            }

            var moduleInfo = IOModuleInfo
                .GetModuleInfo(expectedModuleName, out bool isStub);

            if(moduleInfo != IOModuleInfo.Stub)
            {
                Assert.Multiple(() =>
                {
                    Assert.AreEqual(expectedModuleName, moduleInfo.Name);
                    Assert.IsFalse(isStub);
                });
            }
            else
            {
                Assert.Multiple(() =>
                {
                    Assert.AreEqual(IOModuleInfo.Stub.Name, moduleInfo.Name);
                    Assert.IsTrue(isStub);
                });
            }
        }

        [Test]
        public void TestClone()
        {
            IOModuleInfo actualModuleInfo = GetModuleInfoForTest();
            var cloned = (IOModuleInfo)actualModuleInfo.Clone();

            bool isEqual =
                actualModuleInfo.AddressSpaceType == cloned.AddressSpaceType &&
                actualModuleInfo.AICount == cloned.AICount &&
                actualModuleInfo.AOCount == cloned.AOCount &&
                actualModuleInfo.DICount == cloned.DICount &&
                actualModuleInfo.DOCount == cloned.DOCount &&
                actualModuleInfo.ChannelAddressesIn.SequenceEqual(cloned.ChannelAddressesIn) &&
                actualModuleInfo.ChannelAddressesOut.SequenceEqual(cloned.ChannelAddressesOut) &&
                actualModuleInfo.ChannelClamps.SequenceEqual(cloned.ChannelClamps) &&
                actualModuleInfo.Description == cloned.Description &&
                actualModuleInfo.GroupName == cloned.GroupName &&
                actualModuleInfo.ModuleColor == actualModuleInfo.ModuleColor &&
                actualModuleInfo.Name == cloned.Name &&
                actualModuleInfo.Number == cloned.Number &&
                actualModuleInfo.TypeName == cloned.TypeName;

            Assert.IsTrue(isEqual);
        }

        [TestCase("", null)]
        [TestCase("Name", "Name")]
        [TestCase("", "")]
        public void TestSetGetName(string expected, string actual)
        {
            IOModuleInfo testModule = GetModuleInfoForTest(actual);
            Assert.AreEqual(expected, testModule.Name);
        }

        [TestCase(0, 0)]
        [TestCase(2, 2)]
        [TestCase(4, 4)]
        public void TestSetGetNumber(int expected, int actual)
        {
            IOModuleInfo testModule = GetModuleInfoForTest();
            testModule.Number = actual;
            Assert.AreEqual(expected, testModule.Number);
        }

        [TestCase(null, null)]
        [TestCase("Description", "Description")]
        [TestCase("", "")]
        public void TestSetGetDescription(string expected, string actual)
        {
            IOModuleInfo testModule = GetModuleInfoForTest();
            testModule.Description = actual;
            Assert.AreEqual(expected, testModule.Description);
        }

        public void TestSetGetAddressSpaceType(
            IOModuleInfo.ADDRESS_SPACE_TYPE expected,
            IOModuleInfo.ADDRESS_SPACE_TYPE actual)
        {
            IOModuleInfo testModule = GetModuleInfoForTest();
            testModule.AddressSpaceType = actual;
            Assert.AreEqual(expected, testModule.AddressSpaceType);
        }

        [TestCase(null, null)]
        [TestCase(new int[0], new int[0])]
        [TestCase(new int[] { -1, -1, 0, 1, 2 }, new int[] { -1, -1, 0, 1, 2 })]
        public void TestSetGetChannelClamps(int[] expected, int[] actual)
        {
            IOModuleInfo testModule = GetModuleInfoForTest();
            testModule.ChannelClamps = actual;
            Assert.AreEqual(expected, testModule.ChannelClamps);
        }

        [TestCase(null, null)]
        [TestCase(new int[0], new int[0])]
        [TestCase(new int[] { -1, 0, 1, 2, -1 }, new int[] { -1, 0, 1, 2, -1 })]
        public void TestSetGetChannelAddressesOut(int[] expected, int[] actual)
        {
            IOModuleInfo testModule = GetModuleInfoForTest();
            testModule.ChannelAddressesOut = actual;
            Assert.AreEqual(expected, testModule.ChannelAddressesOut);
        }

        [TestCase(null, null)]
        [TestCase(new int[0], new int[0])]
        [TestCase(new int[] { -1, 0, 1, 2, -1 }, new int[] { -1, 0, 1, 2, -1 })]
        public void TestSetGetChannelAddressesIn(int[] expected, int[] actual)
        {
            IOModuleInfo testModule = GetModuleInfoForTest();
            testModule.ChannelAddressesIn = actual;
            Assert.AreEqual(expected, testModule.ChannelAddressesIn);
        }

        [TestCase(0, 0)]
        [TestCase(2, 2)]
        [TestCase(4, 4)]
        public void TestSetGetAICount(int expected, int actual)
        {
            IOModuleInfo testModule = GetModuleInfoForTest();
            testModule.AICount = actual;
            Assert.AreEqual(expected, testModule.AICount);
        }

        [TestCase(0, 0)]
        [TestCase(2, 2)]
        [TestCase(4, 4)]
        public void TestSetGetAOCount(int expected, int actual)
        {
            IOModuleInfo testModule = GetModuleInfoForTest();
            testModule.AOCount = actual;
            Assert.AreEqual(expected, testModule.AOCount);
        }

        [TestCase(0, 0)]
        [TestCase(2, 2)]
        [TestCase(4, 4)]
        public void TestSetGetDICount(int expected, int actual)
        {
            IOModuleInfo testModule = GetModuleInfoForTest();
            testModule.DICount = actual;
            Assert.AreEqual(expected, testModule.DICount);
        }

        [TestCase(0, 0)]
        [TestCase(2, 2)]
        [TestCase(4, 4)]
        public void TestSetGetDOCount(int expected, int actual)
        {
            IOModuleInfo testModule = GetModuleInfoForTest();
            testModule.DOCount = actual;
            Assert.AreEqual(expected, testModule.DOCount);
        }

        [TestCase("TypeName", "TypeName")]
        [TestCase(null, null)]
        [TestCase("", "")]
        public void TestSetGetTypeName(string expected, string actual)
        {
            IOModuleInfo testModule = GetModuleInfoForTest();
            testModule.TypeName = actual;
            Assert.AreEqual(expected, testModule.TypeName);
        }


        [TestCase("Black", "Black")]
        [TestCase("Empty", "Empty")]
        public void TestSetGetModuleColor(string expected, string actual)
        {
            IOModuleInfo testModule = GetModuleInfoForTest();
            testModule.ModuleColor = Color.FromName(actual);
            Assert.AreEqual(Color.FromName(expected), testModule.ModuleColor);
        }

        [TestCase("GroupName","GroupName")]
        [TestCase(null, null)]
        [TestCase("", "")]
        public void TestSetGetGroupName(string expected, string actual)
        {
            IOModuleInfo testModule = GetModuleInfoForTest();
            testModule.GroupName = actual;
            Assert.AreEqual(expected, testModule.GroupName);
        }

        private static void FillRandomModulesInfo(int count = 0)
        {

        }

        private static IOModuleInfo GetModuleInfoForTest(
            string expectedName = null)
        {
            const int IntStub = 0;
            const string StrStub = "";
            const string ColorStub = "White";

            string name = expectedName ?? string.Empty;

            IOModuleInfo.AddModuleInfo(IntStub, name, StrStub, IntStub,
                StrStub, StrStub, new int[0], new int[0], new int[0], IntStub,
                IntStub, IntStub, IntStub, ColorStub);

            return IOModuleInfo.GetModuleInfo(name, out _);
        }
    }
}
