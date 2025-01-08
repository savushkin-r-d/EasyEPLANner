using System;
using NUnit.Framework;
using InterprojectExchange;
using System.IO;

namespace Tests.InterprojectExchangeTest
{
    public class InterprojectExchangeStarterTest
    {

        [Test]
        public void SaveTest()
        {
            var ipe = new InterprojectExchangeStarter();
            InterprojectExchange.InterprojectExchange.GetInstance()
                 .AddModel(new AdvancedProjectModel());

            Assert.Throws<ApplicationException>(() => ipe.Save());
        }

        [Test]
        public void LoadMainIOData_undefined()
        {
            var ipe = new InterprojectExchangeStarter();

            var method = typeof(InterprojectExchangeStarter).GetMethod("LoadMainIOData",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            Assert.IsFalse((bool)method.Invoke(ipe, new object[] {"", ""}));
        }

        [Test]
        public void LoadMainIOData()
        {
            var ipe = new InterprojectExchangeStarter();

            var initLua = typeof(InterprojectExchangeStarter).GetMethod("InitLuaInstance",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var loadScript = typeof(InterprojectExchangeStarter).GetMethod("LoadScript",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var method = typeof(InterprojectExchangeStarter).GetMethod("LoadMainIOData",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            initLua.Invoke(ipe, null);
            loadScript.Invoke(ipe,
                new object[] { Path.Combine(TestContext.CurrentContext.TestDirectory, "InterprojectExchange.Test", "TestData", "mock_script.lua") });

            Assert.IsTrue((bool)method.Invoke(ipe,
                new object[] { Path.Combine(TestContext.CurrentContext.TestDirectory, "InterprojectExchange.Test", "TestData"), "project" }));
        }
    }
}
