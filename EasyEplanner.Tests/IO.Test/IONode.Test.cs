using System;
using System.Collections.Generic;
using System.Text;
using IO;
using NUnit.Framework;

namespace Tests.IO
{
    class IONodeTest
    {
        [TestCaseSource(nameof(TestSetGetTypeSource))]
        public void TestSetGetType(string typeStr, IONode.TYPES expectedType)
        {
            var testNode = new IONode(typeStr, IntStub, StrStub, StrStub);

            Assert.AreEqual(expectedType, testNode.Type);
        }

        private static object[] TestSetGetTypeSource()
        {
            var testData = new List<object[]>();

            IONode.TYPES internal750_86x = IONode.TYPES.T_INTERNAL_750_86x;         
            testData.Add(new object[] { "750-863", internal750_86x });

            IONode.TYPES ethernet = IONode.TYPES.T_ETHERNET;
            testData.Add(new object[] { "750-341", ethernet });
            testData.Add(new object[] { "750-841", ethernet });
            testData.Add(new object[] { "750-352", ethernet });

            IONode.TYPES internal750_820x = IONode.TYPES.T_INTERNAL_750_820x;
            testData.Add(new object[] { "750-8202", internal750_820x });
            testData.Add(new object[] { "750-8203", internal750_820x });
            testData.Add(new object[] { "750-8204", internal750_820x });
            testData.Add(new object[] { "750-8206", internal750_820x });

            IONode.TYPES pxcCoupler = IONode.TYPES.T_PHOENIX_CONTACT;
            testData.Add(new object[] { "AXL F BK ETH", pxcCoupler });

            IONode.TYPES pxcController = IONode.TYPES.T_PHOENIX_CONTACT_MAIN;
            testData.Add(new object[] { "AXC F 2152", pxcController });

            IONode.TYPES emptyType = IONode.TYPES.T_EMPTY;
            testData.Add(new object[] { "", emptyType });
            testData.Add(new object[] { default, emptyType });
            testData.Add(new object[] { "Wrong type", emptyType });

            return testData.ToArray();
        }

        [TestCase("NodeName","NodeName")]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("Имя узла", "Имя узла")]
        public void TestSetGetNodeName(string expected, string actual)
        {
            var testNode = new IONode(StrStub, IntStub, StrStub, actual);
            Assert.AreEqual(expected, testNode.Name);
        }

        [TestCase("", "")]
        [TestCase(null, null)]
        [TestCase("255.0.0.0", "255.0.0.0")]
        [TestCase("12.12.12.12", "12.12.12.12")]
        public void TestSetGetIP(string expected, string actual)
        {
            var testNode = new IONode(StrStub, IntStub, actual, StrStub);
            Assert.AreEqual(expected, testNode.IP);
        }

        [TestCase("TypeStrSetted", "TypeStrSetted")]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("Строковый тип", "Строковый тип")]
        public void TestSetGetTypeStr(string expected, string actual)
        {
            var testNode = new IONode(actual, IntStub, StrStub, StrStub);
            Assert.AreEqual(expected, testNode.TypeStr);
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(10, 10)]
        public void TestSetGetN(int expected, int actual)
        {
            var testNode = new IONode(StrStub, actual, StrStub, StrStub);
            Assert.AreEqual(expected, testNode.N);
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(200, 2)]
        [TestCase(1000, 10)]
        public void TestSetGetFullN(int expected, int actual)
        {
            var testNode = new IONode(StrStub, actual, StrStub, StrStub);
            Assert.AreEqual(expected, testNode.FullN);
        }

        const string StrStub = "";
        const int IntStub = 0;
    }
}
