using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace TechObjectTests
{
    public class DevModifyOptionsTest
    {
        [Test]
        public void Constructor()
        {

            var techObject = new TechObject.TechObject("", getN => 2, 2, 2, "OBJ", -1, "", "",
                new BaseTechObject() { S88Level = (int)BaseTechObjectManager.ObjectType.Unit });
            var options = new DevModifyOptions(techObject, "OBJ", 1);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(options.IsUnit);
                Assert.IsTrue(options.NumberModified);
                Assert.IsFalse(options.NameModified);
                Assert.AreEqual("OBJ", options.OldTechObjectName);
                Assert.AreEqual("OBJ", options.NewTechObjectName);
                Assert.AreEqual(1, options.OldTechObjectNumber);
                Assert.AreEqual(2, options.NewTechObjectNumber);
            });
        }

    }
}
