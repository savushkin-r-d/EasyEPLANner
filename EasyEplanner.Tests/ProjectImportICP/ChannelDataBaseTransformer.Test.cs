using EasyEPlanner.ProjectImportICP;
using EplanDevice;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ICPImportTests
{
    public class ChannelDataBaseTransformerTest
    {
        private readonly string oldChannelDataBase =
            "<channels:channel>\n" +
            "    <channels:id>1</channels:id>\n" +
            "    <channels:enabled>0</channels:enabled>\n" +
            "    <channels:descr>V3201 : descr</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>2</channels:id>\n" +
            "    <channels:enabled>-1</channels:enabled>\n" +
            "    <channels:descr>V3202 : descr</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>3</channels:id>\n" +
            "    <channels:enabled>-1</channels:enabled>\n" +
            "    <channels:descr>V3203 : descr</channels:descr>\n" +
            "</channels:channel>\n";

        private readonly string newChannelDataBase =
            "<channels:channel>\n" +
            "    <channels:id>10</channels:id>\n" +
            "    <channels:enabled>0</channels:enabled>\n" +
            "    <channels:descr>TANK32V1.V</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>11</channels:id>\n" +
            "    <channels:enabled>0</channels:enabled>\n" +
            "    <channels:descr>TANK32V1.ST</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>12</channels:id>\n" +
            "    <channels:enabled>0</channels:enabled>\n" +
            "    <channels:descr>TANK32V2.V</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>13</channels:id>\n" +
            "    <channels:enabled>0</channels:enabled>\n" +
            "    <channels:descr>TANK32V2.ST</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>14</channels:id>\n" +
            "    <channels:enabled>0</channels:enabled>\n" +
            "    <channels:descr>TANK32V3.ST</channels:descr>\n" +
            "</channels:channel>\n";

        private readonly string ExpectedChannelDataBase =
            "<channels:channel>\n" +
            "    <channels:id>1</channels:id>\n" +
            "    <channels:enabled>0</channels:enabled>\n" +
            "    <channels:descr>TANK32V1.V</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>11</channels:id>\n" +
            "    <channels:enabled>0</channels:enabled>\n" +
            "    <channels:descr>TANK32V1.ST</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>2</channels:id>\n" +
            "    <channels:enabled>-1</channels:enabled>\n" +
            "    <channels:descr>TANK32V2.V</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>13</channels:id>\n" +
            "    <channels:enabled>0</channels:enabled>\n" +
            "    <channels:descr>TANK32V2.ST</channels:descr>\n" +
            "</channels:channel>\n" +
            "<channels:channel>\n" +
            "    <channels:id>3</channels:id>\n" +
            "    <channels:enabled>-1</channels:enabled>\n" +
            "    <channels:descr>TANK32V3.ST</channels:descr>\n" +
            "</channels:channel>\n";



        [Test]
        public void Template()
        {
            var  res = ChannelBaseTransformer.ModifyID(
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

        [Test]
        public void CheckChbaseSameID_NoAssert()
        {
            ChannelBaseTransformer.CheckChbaseID(
                "<channels:id>1</channels:id>" +
                "<channels:id>2</channels:id>" +
                "<channels:id>2</channels:id>");
        }

        [Test]
        public void DisableAllChannels()
        {
            Assert.AreEqual("<channels:enabled>0</channels:enabled><channels:enabled>0</channels:enabled><channels:enabled>0</channels:enabled>",
                ChannelBaseTransformer.DisableAllSubtypesChannels("<channels:enabled>-1</channels:enabled><channels:enabled>-1</channels:enabled><channels:enabled>0</channels:enabled>"));
        }

        [Test]
        public void ShiftID()
        {
            Assert.AreEqual("<channels:id>9</channels:id><channels:id>10</channels:id>",
                ChannelBaseTransformer.ShiftID("<channels:id>1</channels:id><channels:id>2</channels:id>", 0b1000));
        }

        [Test]
        public void ModifyDriverID()
        {
            Assert.AreEqual($"<driver:id>{0x22}</driver:id><channels:id>{0x22010001}</channels:id><channels:id>{0x22010002}</channels:id>",
                ChannelBaseTransformer.ModifyDriverID($"<driver:id>{0x01}</driver:id><channels:id>{0x01010001}</channels:id><channels:id>{0x01010002}</channels:id>", 0x22));
        }

        [Test]
        public void GetDeriverID()
        {
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, ChannelBaseTransformer.GetDriverID(""));
                Assert.AreEqual(22, ChannelBaseTransformer.GetDriverID("<driver:id>22</driver:id>"));
            });
        }

        [Test]
        public void GetFreeSubtypeID()
        {
            Assert.AreEqual(5, ChannelBaseTransformer.GetFreeSubtypeID("<subtypes:sid>1</subtypes:sid><subtypes:sid>3</subtypes:sid><subtypes:sid>4</subtypes:sid>"));
        }

        [Test]
        public void ShiftSubtypeID()
        {
            Assert.AreEqual($"<subtypes:sid>5</subtypes:sid><channels:id>{0x01050001}</channels:id>",
                ChannelBaseTransformer.ShiftSubtypeID($"<subtypes:sid>2</subtypes:sid><channels:id>{0x01020001}</channels:id>", 3));
        }
    }

}
