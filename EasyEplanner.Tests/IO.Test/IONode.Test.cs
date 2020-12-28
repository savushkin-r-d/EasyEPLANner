using System;
using System.Collections.Generic;
using System.Text;
using IO;
using NUnit.Framework;

namespace Tests.IO
{
    class IONodeTest
    {
        [TestCaseSource(nameof(TestSetTypeSource))]
        public void TestSetGetType(string typeStr, IONode.TYPES expectedType)
        {
            // Arrange
            string strStub = string.Empty;
            const int intStub = 0;

            // Act
            var testNode = new IONode(typeStr, intStub, strStub, strStub);

            // Assert
            Assert.AreEqual(expectedType, testNode.Type);
        }

        private static object[] TestSetTypeSource()
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

        public void TestSetGetNodeName()
        {
            // TODO: Test name getter and setter
        }

        public void TestSetGetIP()
        {
            // TODO: Test IP getter and setter
        }

        public void TestSetGetTypeStr()
        {
            // TODO: Test typeStr getter and setter
        }

        public void TestSetGetN()
        {
            // TODO: Test N getter and setter
        }

        public void TestSetGetFullN()
        {
            // TODO: Test FullN getter and setter
        }
    }
}
