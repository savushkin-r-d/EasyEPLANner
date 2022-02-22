using EasyEPlanner.PxcIolinkConfiguration.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests.PxcIolinkConfigration.Models
{
    public class ParametersTest
    {
        [Test]
        public void IsEmpty_NewParameters_ReturnsTrue()
        {
            var parameters = new Parameters();

            Assert.IsTrue(parameters.IsEmpty());
        }

        [Test]
        public void IsEmpty_ParametersCountMoreThanZero_ReturnsFalse()
        {
            var parameters = new Parameters()
            {
                Param = new List<Param>
                {
                    new Param(),
                }
            };

            Assert.IsFalse(parameters.IsEmpty());
        }

        [Test]
        public void Clone_ParametersWithTwoParam_CloneValues()
        {
            var parametersToClone = new Parameters()
            {
                Param = new List<Param>
                {
                    new Param()
                    {
                        Value = "1",
                        InternalValue = "2",
                        Id = "3",
                        Name = "4",
                        Subindex = "5",
                        Text = "6",
                        Unit = "7"
                    },
                    new Param()
                    {
                        Value = "10",
                        InternalValue = "20",
                        Id = "30",
                        Name = "40",
                        Subindex = "50",
                        Text = "60",
                        Unit = "70"
                    },
                }
            };

            var cloned = (Parameters)parametersToClone.Clone();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(parametersToClone.Param.Count, cloned.Param.Count);
                for(int i = 0; i < cloned.Param.Count; i++)
                {
                    Assert.AreEqual(parametersToClone.Param[i].Text, cloned.Param[i].Text);
                    Assert.AreEqual(parametersToClone.Param[i].Unit, cloned.Param[i].Unit);
                    Assert.AreEqual(parametersToClone.Param[i].Value, cloned.Param[i].Value);
                    Assert.AreEqual(parametersToClone.Param[i].InternalValue, cloned.Param[i].InternalValue);
                    Assert.AreEqual(parametersToClone.Param[i].Id, cloned.Param[i].Id);
                    Assert.AreEqual(parametersToClone.Param[i].Name, cloned.Param[i].Name);
                    Assert.AreEqual(parametersToClone.Param[i].Subindex, cloned.Param[i].Subindex);
                }
            });
        }
    }
}
