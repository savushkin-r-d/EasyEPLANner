using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;
using NUnit.Framework;

namespace TechObjectTests
{
    public class TechObjectManagerTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var instance = typeof(TechObjectManager).GetField("instance",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Static);
            instance.SetValue(null, null);
            
            techObjectManager = TechObjectManager.GetInstance();
            techObjects = techObjectManager.TechObjects;
            genericTechObjects = techObjectManager.GenericTechObjects;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            var instance = typeof(TechObjectManager).GetField("instance",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Static);
            instance.SetValue(null, null);
        }

        [SetUp]
        public void SetUp()
        {
            var baseTechObject = new BaseTechObject()
            {
                EplanName = "TANK",
            };

            genericTank = new GenericTechObject("Танк", 2, "TANK", -1, "TANK", "", baseTechObject);
            genericCreamTank = new GenericTechObject("Сливки танк", 3, "CREAM_TANK", -1, "CREAM_TANK", "", baseTechObject);
            genericSkimmilkTank = new GenericTechObject("Обрат танк", 4, "SKIMMILK_TANK", -1, "SKIMMILK_TANK", "", baseTechObject);

            Tank = new TechObject.TechObject("Танк", GetN => 1, 1, 2, "TANK", -1, "TANK1", "", baseTechObject);
            CreamTank = new TechObject.TechObject("Сливки танк", GetN => 2, 1, 3, "CREAM_TANK", -1, "CREAM_TANK1", "", baseTechObject);
            SkimmilkTank = new TechObject.TechObject("Обрат танк", GetN => 3, 1, 4, "SKIMMILK_TANK", -1, "SKIMMILK_TANK1", "", baseTechObject);


            genericTechObjects.Add(genericTank);
            genericTechObjects.Add(genericCreamTank);
            genericTechObjects.Add(genericSkimmilkTank);

            techObjects.Add(Tank);
            techObjects.Add(CreamTank);
            techObjects.Add(SkimmilkTank);
        }

        [TearDown]
        public void TearDown()
        {
            genericTechObjects?.Clear();
            techObjects?.Clear();
        }

        [Test]
        public void GetGenericTObject()
        {
            Assert.Multiple(() => 
            {
                var genericTechObject = techObjectManager.GetGenericTObject(1);
                Assert.AreSame(genericTank, genericTechObject);

                genericTechObject = techObjectManager.GetGenericTObject(3);
                Assert.AreSame(genericSkimmilkTank, genericTechObject);

                genericTechObject = techObjectManager.GetGenericTObject(4);
                Assert.IsNull(genericTechObject);
            });
        }

        [Test]
        public void GetTechObjectN_ByDisplayText()
        {
            Assert.Multiple(() => 
            {
                var techObjectN = techObjectManager.GetTechObjectN("1. Танк №1 (#1)");
                Assert.AreEqual(1, techObjectN);

                techObjectN = techObjectManager.GetTechObjectN("2. Сливки танк №1 (#2)");
                Assert.AreEqual(2, techObjectN);

                techObjectN = techObjectManager.GetTechObjectN("qwerty");
                Assert.AreEqual(0, techObjectN);
            });
        }

        [Test]
        public void GetTechObjectN_By_BaseObjectName_EplanName_TechNumber()
        {
            Assert.Multiple(() =>
            {
                var techObjectN = techObjectManager.GetTechObjectN("TANK", "TANK", 1);
                Assert.AreEqual(1, techObjectN);

                techObjectN = techObjectManager.GetTechObjectN("TANK", "CREAM_TANK", 1);
                Assert.AreEqual(2, techObjectN);

                techObjectN = techObjectManager.GetTechObjectN("TANK", "CREAM_TANK", 0);
                Assert.AreEqual(0, techObjectN);

                techObjects.Add(new TechObject.TechObject("TANK", GetN => 4, 2, 2, "TANK", -1, "TANK2", "", null));
                techObjectN = techObjectManager.GetTechObjectN("TANK", "CREAM_TANK", 2);
                Assert.AreEqual(0, techObjectN);
            });
        }

        [Test]
        public void SaveAsLuaTable()
        {
            techObjects.Remove(CreamTank);
            techObjects.Remove(SkimmilkTank);

            genericTechObjects.Remove(genericCreamTank);
            genericTechObjects.Remove(genericSkimmilkTank);

            Tank.GenericTechObject = genericTank;            

            var result = techObjectManager.SaveAsLuaTable("");
            

            Assert.AreEqual(string.Join("\n", expectedSaveAsLuaTableResult), result);
        }

        private readonly string[] expectedSaveAsLuaTableResult = new string[]
        {
            "init_generic_tech_objects = function()",
            "    return",
            "    {",
            "    [ 1 ] =",
            "        {",
            "        n          = 0,",
            "        tech_type  = 2,",
            "        name       = 'Танк',",
            "        name_eplan = 'TANK',",
            "        name_BC    = 'TANK',",
            "        cooper_param_number = -1,",
            "        base_tech_object = 'TANK',",
            "",
            "        modes =",
            "            {",
            "            },",
            "        },",
            "    }",
            "end",
            new string('-', 80),
            new string('-', 80),
            "init_tech_objects_modes = function()",
            "    return",
            "    {",
            "    [ 1 ] =",
            "        {",
            "        n          = 1,",
            "        tech_type  = 2,",
            "        name       = 'Танк',",
            "        name_eplan = 'TANK',",
            "        name_BC    = 'TANK1',",
            "        cooper_param_number = -1,",
            "        base_tech_object = 'TANK',",
            "        generic_tech_object = 1,",
            "",
            "        modes =",
            "            {",
            "            },",
            "        },",
            "    }",
            "end",
        };

        [Test]
        public void AddObject()
        {
            var baseObject = new BaseObject("TANK", techObjectManager);

            BaseTechObjectManager.GetInstance().AddBaseObject("TANK", "TANK", 2, "TANK", "TANK", false, "TANK", "TANK", false);
            var genericGroup = new GenericGroup(genericTank, baseObject, techObjectManager);

            Assert.Multiple(() =>
            {
                var techObject = techObjectManager.AddObject(4, 2, "Танк", 2, "TANK", -1, "TANK2", "TANK", "", 1, false);
                Assert.AreSame(techObjectManager.GetTObject(4), techObject);

                var genericTechObject = techObjectManager.AddObject(4, 2, "Танк", 2, "TANK", -1, "TANK", "TANK", "", 1, true);
                Assert.AreSame(techObjectManager.GetGenericTObject(4), genericTechObject);

                techObject = techObjectManager.AddObject(5, 3, "Танк", 3, "tank_", -1, "tank_3", "tank_", "", 0, false);
                Assert.AreSame(techObjectManager.GetTObject(5), techObject);
            });
        }

        private TechObjectManager techObjectManager;
        private List<TechObject.TechObject> techObjects;
        private List<GenericTechObject> genericTechObjects;

        private GenericTechObject genericTank;
        private GenericTechObject genericCreamTank;
        private GenericTechObject genericSkimmilkTank;

        private TechObject.TechObject Tank;
        private TechObject.TechObject CreamTank;
        private TechObject.TechObject SkimmilkTank;
    }
}
