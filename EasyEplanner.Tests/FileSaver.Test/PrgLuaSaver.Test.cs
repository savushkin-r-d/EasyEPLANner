using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using EasyEPlanner;
using System.Reflection;
using Moq;
using TechObject;

namespace EasyEplannerTests.FileSaverTest
{
    public class PrgLuaSaverTest
    {
        private readonly string GetAttachedObjectsGroupsDictMethodName = "GetAttachedObjectsGroupsDict";


        [Test]
        public void GetAttachedObjectsGroupsDict_Test()
        {
            string bindingName1 = "BN1";
            string bindingName2 = "BN2";

            var method = typeof(PrgLuaSaver).GetMethod(
                GetAttachedObjectsGroupsDictMethodName,
                BindingFlags.NonPublic | BindingFlags.Static);

            var baseTechObject1 = new BaseTechObject(null);
            baseTechObject1.BindingName = bindingName1;

            var baseTechObject2 = new BaseTechObject(null);
            baseTechObject2.BindingName = bindingName2;

            var techObject1 = new TechObject.TechObject(string.Empty, getN => 1, 1, 1,
                string.Empty, 1, string.Empty, string.Empty, baseTechObject1);
            var techObject2 = new TechObject.TechObject(string.Empty, getN => 2, 2, 1,
                string.Empty, 1, string.Empty, string.Empty, baseTechObject1);
            var techObject3 = new TechObject.TechObject(string.Empty, getN => 3, 3, 1,
                string.Empty, 1, string.Empty, string.Empty, baseTechObject2);

            var result = method.Invoke(null, new object[] { new List<TechObject.TechObject>()
            {
                techObject1,
                techObject2,
                techObject3,
            }}) as Dictionary<string, List<TechObject.TechObject>>;

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.ContainsKey(bindingName1));
                Assert.IsTrue(result.ContainsKey(bindingName2));
                Assert.IsTrue(result[bindingName1]?.SequenceEqual(
                    new List<TechObject.TechObject>() { techObject1, techObject2 } ));
                Assert.IsTrue(result[bindingName2]?.SequenceEqual(
                    new List<TechObject.TechObject>() { techObject3 }));
            });
        }

        [Test]
        public void GetAttachedObjectsGroupsDict_EmptyResult()
        {
            var method = typeof(PrgLuaSaver).GetMethod(
                GetAttachedObjectsGroupsDictMethodName,
                BindingFlags.NonPublic | BindingFlags.Static);

            var techObject1 = new TechObject.TechObject(string.Empty, getN => 1, 1, 1,
                string.Empty, 1, string.Empty, string.Empty, null);

            var result = method.Invoke(null, new object[] { new List<TechObject.TechObject>()
            {
                techObject1,
            }}) as Dictionary<string, List<TechObject.TechObject>>;

            Assert.IsEmpty(result);
        }
    }
}
