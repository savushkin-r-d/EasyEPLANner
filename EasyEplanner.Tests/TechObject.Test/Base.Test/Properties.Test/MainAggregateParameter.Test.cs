using NUnit.Framework;


namespace TechObject.Tests
{
    class MainAggregateParameterTest
    {
        [Test]
        public void NeedDisable_ReturnFalse()
        {
            var parameter = new MainAggregateParameter("LuaName", "name", "val");
            Assert.IsFalse(parameter.NeedDisable);
        }

    }
}
