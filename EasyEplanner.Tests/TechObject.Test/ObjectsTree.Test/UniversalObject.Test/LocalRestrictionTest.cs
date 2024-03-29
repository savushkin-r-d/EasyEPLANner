using Editor;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using TechObject;

namespace TechObject.Tests
{
    public class LocalRestrictionTest
    {
        [TestCaseSource(nameof(SetValue_caseSource))]
        public void SetValue_CheckDisplayText(
            SortedDictionary<int, List<int>> restriction, string expected)
        {
            var localrestriction = new LocalRestriction("Name", "value",
                "LuaName",
                new SortedDictionary<int, List<int>>()
                { { 1, new List<int>() { 1, 2, 3 } } });

            var techObjectManager = TechObjectManager.GetInstance();
            var techObjects = new List<TechObject>() {
                new TechObject("TO1", getN => 1, 1, 1, "EplanNameTO1", 1, "BCNameTO1", string.Empty, null),
                new TechObject("TO2", getN => 2, 2, 2, "EplanNameTO2", 2, "BCNameTO2", string.Empty, null)
            };
            var modes = new List<Mode>
            {
                new Mode("modeName_1", getN => 1, null),
                new Mode("modeName_2", getN => 2, null),
                new Mode("modeName_2", getN => 3, null),
            };
            techObjects[0].ModesManager.Modes.AddRange(modes);
            techObjects[1].ModesManager.Modes.AddRange(modes);

            techObjectManager.TechObjects.AddRange(techObjects);


            localrestriction.SetValue(restriction);
            Assert.AreEqual(localrestriction.DisplayText[1], expected);
        }

        static object[] SetValue_caseSource =
        {
            new object[] 
            { 
                new SortedDictionary<int, List<int>>() { 
                    { 1, new List<int>() { 1, 2 } } },
                "{ 1, 1 } { 1, 2 }" 
            },
            new object[]
            {
                new SortedDictionary<int, List<int>>() {
                    { 2, new List<int>() { 1, 3 } } },
                "{ 2, 1 } { 2, 3 }"
            },
            new object[]
            {
                new SortedDictionary<int, List<int>>() {
                    { 1, new List<int>() { 1, 2 } },
                    { 2, new List<int>() { 1, 3 } } },
                "{ 1, 1 } { 1, 2 } { 2, 1 } { 2, 3 }"
            },
        };


        [Test]
        public void SetNewValueAtTheSameObjects_Test()
        {
            var baseTechObject = new BaseTechObject();
            baseTechObject.EplanName = "BaseTechObjectName";

            var techObjects = new List<TechObject>();

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(obj => obj.TechObjects).Returns(techObjects);


            var processCell = new ProcessCell(techObjectManagerMock.Object);
            var processCellTechObject = processCell.Insert() as TechObject;
            var baseObject = new BaseObject("BaseTechObjectName", techObjectManagerMock.Object);

            var techObject1 = new TechObject("", GetN => 1, 1, 2, "TANK", -1, "", string.Empty, baseTechObject);
            var techObject2_SameTechType = new TechObject("", GetN => 2, 1, 2, "TANK", -1, "", string.Empty, baseTechObject);
            var techObject3_OtherTechType = new TechObject("", GetN => 3, 1, 3, "TANK", -1, "", string.Empty, baseTechObject);

            techObjects.Add(processCellTechObject);
            techObjects.Add(techObject1);
            techObjects.Add(techObject2_SameTechType);
            techObjects.Add(techObject3_OtherTechType);
            TechObjectManager.GetInstance().TechObjects.Add(processCellTechObject);
            TechObjectManager.GetInstance().TechObjects.Add(techObject1);
            TechObjectManager.GetInstance().TechObjects.Add(techObject2_SameTechType);
            TechObjectManager.GetInstance().TechObjects.Add(techObject3_OtherTechType);

            baseObject.AddObject(techObject1);
            baseObject.AddObject(techObject2_SameTechType);
            baseObject.AddObject(techObject3_OtherTechType);

            techObject1.AddParent(baseObject);
            techObject2_SameTechType.AddParent(baseObject);
            techObject3_OtherTechType.AddParent(baseObject);

            var to1Mode1 = techObject1.ModesManager.Insert() as Mode;
            var to1Mode2 = techObject1.ModesManager.Insert() as Mode;

            var to2Mode1 = techObject2_SameTechType.ModesManager.Insert() as Mode;
            var to2Mode2 = techObject2_SameTechType.ModesManager.Insert() as Mode;

            var to3Mode1 = techObject3_OtherTechType.ModesManager.Insert() as Mode;
            var to3Mode2 = techObject3_OtherTechType.ModesManager.Insert() as Mode;

            var processCellMode1 = processCellTechObject.ModesManager.Insert() as Mode;
            var processCellMode2 = processCellTechObject.ModesManager.Insert() as Mode;

            var localRestriction = to1Mode1.GetRestrictionManager().Restrictions[0] as LocalRestriction;
            localRestriction.SetNewValue(new Dictionary<int, List<int>> { { 1, new List<int> { 2 } } });

            (processCellMode1.GetRestrictionManager().Restrictions[0] as LocalRestriction).SetNewValue(new Dictionary<int, List<int>> { { 1, new List<int> { 2 } } });

            Assert.Multiple(() =>
            {
                Assert.AreEqual("{ 2, 2 }", localRestriction.EditText[1]);
                Assert.AreEqual("{ 2, 1 }", to1Mode2.GetRestrictionManager().Restrictions[0].EditText[1]);
                
                Assert.AreEqual("{ 3, 2 }", to2Mode1.GetRestrictionManager().Restrictions[0].EditText[1]);
                Assert.AreEqual("{ 3, 1 }", to2Mode2.GetRestrictionManager().Restrictions[0].EditText[1]);

                Assert.AreEqual(string.Empty, to3Mode1.GetRestrictionManager().Restrictions[0].EditText[1]);
                Assert.AreEqual(string.Empty, to3Mode2.GetRestrictionManager().Restrictions[0].EditText[1]);
            });
        }
    }
}
