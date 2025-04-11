using IO;
using IO.ViewModel;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTests
{
    public class PropertyTest
    {
        [Test]
        public void Getters()
        {
            var nodes = new List<IIONode>() { };

            var value = "value";

            var property = new Property("property", () => value, v => value = v);
            var propertyNullSetter = new Property("property", () => value);

            Assert.Multiple(() =>
            {
                Assert.AreEqual("property", property.Name);
                Assert.AreEqual("value", property.Description);
                Assert.AreEqual("value", property.Value);

                Assert.IsTrue(property.SetValue("newValue"));
                Assert.AreEqual("newValue", value);
                
                Assert.IsFalse(propertyNullSetter.SetValue("newValue"));
            });
        }
    }
}
