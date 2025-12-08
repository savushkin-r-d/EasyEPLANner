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
            var container = new Container();

            var expected = "<container>\r\n" +
                "  <build>20</build>\r\n" +
                "  <version>1</version>\r\n" +
                "  <attributes>\r\n" +
                "    <theme></theme>\r\n" +
                "    <author></author>\r\n" +
                "    <organization></organization>\r\n" +
                "    <telefon></telefon>\r\n" +
                "    <comment></comment>\r\n" +
                $"    <lastdate>{container.Attributes.CurrentDate}</lastdate>\r\n" +
                "  </attributes>\r\n" +
                "  <components />\r\n" +
                "</container>";

            var actual = new ContainerSerializer(container).Serialize();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SerializeComponentTest()
        {
            var expected = "<component>\r\n" +
                "  <imageslist>\r\n" +
                "    <width>0</width>\r\n" +
                "    <height>0</height>\r\n" +
                "    <startx>0</startx>\r\n" +
                "    <starty>0</starty>\r\n" +
                "    <wallpaper>False</wallpaper>\r\n" +
                "    <animation>False</animation>\r\n" +
                "    <animationstart>1</animationstart>\r\n" +
                "    <animationend>1</animationend>\r\n" +
                "    <animationspeed>1</animationspeed>\r\n" +
                "  </imageslist>\r\n" +
                "  <propertieslist>\r\n" +
                "    <property>\r\n" +
                "      <name />\r\n" +
                "      <caption />\r\n" +
                "      <visible>True</visible>\r\n" +
                "      <report>False</report>\r\n" +
                "      <saved>False</saved>\r\n" +
                "      <tagname></tagname>\r\n" +
                "      <propmodel>0</propmodel>\r\n" +
                "      <proptype>0</proptype>\r\n" +
                "      <value></value>\r\n" +
                "      <channelid>0</channelid>\r\n" +
                "      <priority>5</priority>\r\n" +
                "    </property>\r\n" +
                "    <property>\r\n" +
                "      <name />\r\n" +
                "      <caption />\r\n" +
                "      <visible>True</visible>\r\n" +
                "      <report>False</report>\r\n" +
                "      <saved>False</saved>\r\n" +
                "      <tagname></tagname>\r\n" +
                "      <propmodel>0</propmodel>\r\n" +
                "      <proptype>2</proptype>\r\n" +
                "      <value>0</value>\r\n" +
                "      <channelid>0</channelid>\r\n" +
                "      <priority>5</priority>\r\n" +
                "    </property>\r\n" +
                "  </propertieslist>\r\n" +
                "</component>";

            var actual = new ComponentSerializer(new Component()
            {
                Properties = new List<IProperty>() 
                { 
                    new Property() { PropType = PropertyType.String },
                    new Property() { PropType = PropertyType.Float }
                }
            }).Serialize();

            Assert.AreEqual(expected, actual);
        }


    }
}
