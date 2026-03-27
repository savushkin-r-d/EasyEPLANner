using EplanDevice;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace EasyEplannerTests.TechObjectTest.BasePropertiesTest
{
    internal class EquipmentParameterTest
    {
        [Test]
        public void Delete_Test()
        {
            var equip = new EquipmentParameter("par", "par name");
            var rpar = equip.AddEquipment("rpar", "rpar name", "");

            rpar.SetValue("value");

            Assert.Multiple(() =>
            {
                Assert.AreEqual("value", rpar.Value);
                Assert.IsTrue(equip.Delete(rpar));
                Assert.AreEqual("", rpar.Value);

                Assert.IsFalse (equip.Delete(1));
            });
        }

        [Test]
        public void Replace_Test()
        {
            var equip = new EquipmentParameter("par", "rparname");
            var rpar = equip.AddEquipment("rpar", "rparname", "");

            var copy = new EquipmentParameter("copy", "");

            rpar.SetValue("value");
            copy.SetValue("other value");

            Assert.Multiple(() =>
            {
                Assert.IsNull(equip.Replace(rpar, "-"));
                Assert.AreSame(rpar, equip.Replace(rpar, copy));
                Assert.AreEqual("", rpar.Value); // b.c. for SetNewValue need DeviceManager; 

                Assert.IsNull(equip.Replace(0, "-"));
            });
        }

        [Test]
        public void InsertCopy_Test()
        {
            var equip = new EquipmentParameter("par", "rparname");
            var rpar = equip.AddEquipment("rpar", "rparname", "");

            var copy = new EquipmentParameter("copy", "");

            rpar.SetValue("value");
            copy.SetValue("other value");

            Assert.Multiple(() =>
            {
                Assert.IsNull(rpar.InsertCopy("-"));
                Assert.AreSame(rpar, rpar.InsertCopy(copy));
                Assert.AreEqual("", rpar.Value); // b.c. for SetNewValue need DeviceManager; 
            });
        }

        [Test]
        public void GetDisplayObjects_Test()
        {
            var equip = new EquipmentParameter("LT", "rparname");
            var equip_set_value = new EquipmentParameter("P_SET_VALUE", "set value");


            Assert.Multiple(() =>
            { 
                equip.GetDisplayObjects(out var devTypes, out var devSubTypes, out var displayParameters);
                Assert.IsNull(devTypes);
                Assert.IsNull(devSubTypes);
                Assert.IsFalse(displayParameters);
                equip_set_value.GetDisplayObjects(out devTypes, out devSubTypes, out displayParameters);
                Assert.IsNull(devTypes);
                Assert.IsNull(devSubTypes);
                Assert.IsTrue(displayParameters);
            });
        }

        [Test]
        public void Props_Test()
        {
            var equip = new EquipmentParameter("equip", "equip name", "");
            Assert.Multiple(() =>
            {

                Assert.IsTrue(equip.IsCopyable);
                Assert.IsTrue(equip.IsReplaceable);
                Assert.IsTrue(equip.IsDeletable);
                Assert.IsTrue(equip.IsInsertableCopy);

                Assert.AreEqual("equip name", equip.DisplayText[0]);

                Assert.AreEqual("Нет", equip.DisplayText[1]);

                equip.SetValue("value");
                Assert.AreEqual("value", equip.DisplayText[1]);
            });
        }
    }
}
