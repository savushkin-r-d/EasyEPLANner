using NUnit.Framework;


namespace TechObject.Tests
{
    class ModesManagerTest
    {

        [Test]
        public void SetUpFromBaseTechObject_CheckModes()
        {
            var techObject = new TechObject("name", getN => 1, 1, 1, "eplanName", 1, "NameBc", "", null);
            var modesManager = new ModesManager(techObject);
            var baseTechObject = new BaseTechObject();
            
            baseTechObject.AddBaseOperation("LuaName1", "Name1", 1);
            baseTechObject.AddBaseOperation("LuaName2", "Name2", 2);
            baseTechObject.AddBaseOperation("LuaName3", "Name3", 3);

            modesManager.SetUpFromBaseTechObject(baseTechObject);
            Assert.AreEqual(3, modesManager.Modes.Count);
        }
    }
}
