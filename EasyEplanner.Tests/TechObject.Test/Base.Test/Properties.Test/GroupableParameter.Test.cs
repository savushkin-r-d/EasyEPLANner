using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace TechObjectTests
{
    public class GroupableParametersTest
    {
        [Test]
        public void AddGroupParameter()
        {
            var parameter = new GroupableParameters("", "", true, false);

            var addedParameter = parameter.AddGroupParameter("", "", false, true);

            Assert.AreSame(addedParameter, parameter.Parameters[0]);
        }

        [Test]
        public void AddActiveParameter()
        {
            var parameter = new GroupableParameters("PREFIX", "", true, false);

            var addedParameter = parameter.AddActiveParameter("NAME", "", "");

            Assert.AreSame(addedParameter, parameter.Parameters[0]);
            Assert.AreEqual("PREFIX_NAME", addedParameter.LuaName);
        }

        [Test]
        public void AddActiveBoolParameter()
        {
            var parameter = new GroupableParameters("PREFIX", "", true, false);

            var addedParameter = parameter.AddActiveBoolParameter("NAME", "", "");

            Assert.AreSame(addedParameter, parameter.Parameters[0]);
            Assert.AreEqual("PREFIX_NAME", addedParameter.LuaName);
        }

        [Test]
        public void Clone()
        {
            var parameter = new GroupableParameters("PREFIX", "", true, false);
            var addedParameter = parameter.AddActiveBoolParameter("NAME", "", "");

            var clone = parameter.Clone() as GroupableParameters;

            Assert.AreNotSame(clone, parameter);
            Assert.AreNotSame(clone.Parameters[0], parameter.Parameters[0]);
            Assert.AreEqual(clone.Parameters[0].LuaName, parameter.Parameters[0].LuaName);
            Assert.AreEqual(clone.LuaName, parameter.LuaName);
            Assert.AreEqual(clone.Name, parameter.Name);
            Assert.AreEqual(clone.Main, parameter.Main);
            Assert.AreEqual(clone.IgnoreCompoundName, parameter.IgnoreCompoundName);
            Assert.AreEqual(clone.NeedDisable, parameter.NeedDisable);
        }


        [Test]
        public void Properties()
        {
            var parameter = new GroupableParameters("GROUP", "Группа", true, false);

            CollectionAssert.AreEqual(new string[] { "Группа", "Нет" }, parameter.DisplayText);
            Assert.AreEqual(parameter.Main, parameter.IsEditable);
        }

        [Test]
        public void SetUpParametersVisibility()
        {
            var parameter = new GroupableParameters("PREFIX", "", true, false);
            var addedParameter = parameter.AddActiveBoolParameter("NAME", "", "");

            Assert.Multiple(() =>
            {
                parameter.SetNewValue("true");
                Assert.IsNotNull(parameter.Items.ElementAtOrDefault(0));

                parameter.SetNewValue("false");
                Assert.IsNull(parameter.Items.ElementAtOrDefault(0));
            });
        }

    }
}
