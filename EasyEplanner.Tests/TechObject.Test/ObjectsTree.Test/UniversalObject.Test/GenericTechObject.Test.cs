using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TechObject;

namespace TechObjectTests
{
    public class GenericTechObjectTest
    {
        [Test]
        public void TestProperties()
        {
            var generic = new GenericTechObject("", 2, "", -1, "", "", null);

            Assert.Multiple(() =>
            {
                Assert.IsFalse(generic.IsMainObject);
                Assert.IsFalse(generic.IsInsertableCopy);
                Assert.IsFalse(generic.IsInsertable);
                Assert.IsFalse(generic.IsReplaceable);
                Assert.IsFalse(generic.IsCopyable);
                Assert.IsFalse(generic.IsDeletable);
            });
        }

    }
}
