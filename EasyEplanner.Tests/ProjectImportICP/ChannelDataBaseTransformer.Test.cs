using EasyEPlanner.ProjectImportICP;
using EplanDevice;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ICPImportTests
{
    public class ChannelDataBaseTransformerTest
    {
        private readonly string oldChannelDataBase =
            "<channels:channel>\n" +
            "    <channels:id>1</channels:id>\n" +
            "    <channels:descr>V3201 : descr</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>2</channels:id>\n" +
            "    <channels:descr>V3202 : descr</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>3</channels:id>\n" +
            "    <channels:descr>V3203 : descr</channels:descr>\n" +
            "</channels:channel>\n";

        private readonly string newChannelDataBase =
            "<channels:channel>\n" +
            "    <channels:id>10</channels:id>\n" +
            "    <channels:descr>TANK32V1.V</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>11</channels:id>\n" +
            "    <channels:descr>TANK32V1.ST</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>12</channels:id>\n" +
            "    <channels:descr>TANK32V2.V</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>13</channels:id>\n" +
            "    <channels:descr>TANK32V2.ST</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>15</channels:id>\n" +
            "    <channels:descr>TANK32V3.ST</channels:descr>\n" +
            "</channels:channel>\n";

        private readonly string ExpectedChannelDataBase =
            "<channels:channel>\n" +
            "    <channels:id>1</channels:id>\n" +
            "    <channels:descr>TANK32V1.V</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>11</channels:id>\n" +
            "    <channels:descr>TANK32V1.ST</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>2</channels:id>\n" +
            "    <channels:descr>TANK32V2.V</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>13</channels:id>\n" +
            "    <channels:descr>TANK32V2.ST</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>3</channels:id>\n" +
            "    <channels:descr>TANK32V3.ST</channels:descr>\n" +
            "</channels:channel>\n";



        [Test]
        public void Template()
        {
            var res = new ChannelBaseTransformer().TransformID(
                newChannelDataBase,
                oldChannelDataBase,
                new List<(string newName, string oldName)>()
            {
                ( "TANK32V1" , "V3201" ),
                ( "TANK32V2" , "V3202" ),
                ( "TANK32V3" , "V3203" ),
                ( "TANK33V1" , "V3301" ),
                ( "TANK33V1" , ""      ),
            });

            Assert.AreEqual(ExpectedChannelDataBase, res);
        }

        [TestCaseSource(nameof(ToWagoDevicesCases))]
        public void ToWagoDevice(IODevice device, string expectedWagoName)
        {
            var wagoName = ChannelBaseTransformer.ToWagoDevice(device);
            Assert.AreEqual(expectedWagoName, wagoName);
        }

        private static readonly object[] ToWagoDevicesCases = new []
        {
            new object[] { new V("TANK32V1", "", "", 1, "TANK", 32, ""),  "V3201" },
            new object[] { new V("F15V1", "", "", 1, "F", 15, ""),  "V31501" },
            new object[] { new V("LINE15V1", "", "", 1, "LINE", 15, ""),  "" },
            new object[] { new M("TANK32M1", "", "", 1, "TANK", 32, ""),  "" },
        };
    }
}
