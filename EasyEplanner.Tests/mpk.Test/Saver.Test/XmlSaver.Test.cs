using EasyEPlanner.mpk.Model;
using EasyEPlanner.mpk.Saver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerTests.mpkTest.SaverTest
{
    public class XmlSaverTest
    {
        [Test]
        public void SerializeContainerTest()
        {
            using (var outputStream = new MemoryStream())
            {
                ContainerSerializer.Serialize(new Container(), outputStream);

                outputStream.Position = 0; // Reset position to read from the beginning
                using (var reader = new StreamReader(outputStream))
                {
                    var actualOutput = reader.ReadToEnd();
                }
            }
        }

        [Test]
        public void SerializeComponentTest()
        {
            using (var outputStream = new MemoryStream())
            {
                ComponentSerializer.Serialize(new Component()
                {
                    Properties = new List<Property>() { new Property() }
                }, outputStream);

                outputStream.Position = 0; // Reset position to read from the beginning
                using (var reader = new StreamReader(outputStream))
                {
                    var actualOutput = reader.ReadToEnd();
                }
            }
        }


    }
}
