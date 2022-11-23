using TechObject;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace Tests.TechObject
{
    class BaseTechObjectLoaderTest
    {
        [Test]
        public void LoadBaseTechObjectsDescription_returnReadedDataFromFile()
        {
            using (StreamWriter writer = new StreamWriter("pathToFile.txt"))
            {
                writer.WriteLine("Base Tech Object Dwscription");
            }
                var baseTechObjectLoader = new BaseTechObjectLoader();
            var method = typeof(BaseTechObjectLoader).GetMethod(
                "LoadBaseTechObjectsDescription", BindingFlags.NonPublic | BindingFlags.Instance);
            object res =  method.Invoke(baseTechObjectLoader, new object[] { "pathToFile.txt" });

            File.Delete("pathToFile.txt");

            Assert.AreEqual("Base Tech Object Dwscription\r\n", res);
        }
    }

}