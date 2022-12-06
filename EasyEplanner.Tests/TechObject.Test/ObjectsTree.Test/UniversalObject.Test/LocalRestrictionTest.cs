using Editor;
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

    }
}
