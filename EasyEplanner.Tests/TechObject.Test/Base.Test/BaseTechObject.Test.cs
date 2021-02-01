using System;
using System.Collections.Generic;
using System.Text;
using TechObject;
using NUnit.Framework;

namespace Tests.TechObject
{
    class BaseTechObjectTest
    {
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(100)]
        public void AddSystemParameter_EmptyBaseTechObject_AddParametersToList(
            int parametersCount)
        {
            BaseTechObject emptyObject = GetEmpty();
            for (int i = 0; i < parametersCount; i++)
            {
                emptyObject.AddSystemParameter("LuaName", "Name", 0, "шт");
            }

            Assert.AreEqual(parametersCount, emptyObject.SystemParams.Count);
        }

        private BaseTechObject GetEmpty()
        {
            return new BaseTechObject();
        }
    }
}
