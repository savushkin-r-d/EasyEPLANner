using EasyEPlanner.PxcIolinkConfiguration.Models;
using NUnit.Framework;

namespace Tests.PxcIolinkConfigration.Models
{
    public class ParamTest
    {
        [Test]
        public void Clone_FilledParam_EqualValues()
        {
            var paramToClone = new Param()
            {
                Text = "1",
                Unit = "2",
                Value = "3",
                InternalValue = "4",
                Id = "5",
                Name = "6",
                Subindex = "7"
            };

            var cloned = (Param)paramToClone.Clone();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(paramToClone.Text, cloned.Text);
                Assert.AreEqual(paramToClone.Unit, cloned.Unit);
                Assert.AreEqual(paramToClone.Value, cloned.Value);
                Assert.AreEqual(paramToClone.InternalValue, cloned.InternalValue);
                Assert.AreEqual(paramToClone.Id, cloned.Id);
                Assert.AreEqual(paramToClone.Name, cloned.Name);
                Assert.AreEqual(paramToClone.Subindex, cloned.Subindex);
            });
        }
    }
}
