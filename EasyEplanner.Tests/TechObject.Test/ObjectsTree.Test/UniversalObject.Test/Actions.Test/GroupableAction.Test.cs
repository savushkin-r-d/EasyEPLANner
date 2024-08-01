using Editor;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace TechObjectTests
{
    public class GroupableActionTest
    {
        [TestCaseSource(nameof(GetObjectToDrawOnEplanPageTestCaseSource))]
        public void GetObjectToDrawOnEplanPage_NewAction_ReturnsDrawInfoList(
            DrawInfo.Style drawStyle, List<int> devIdList_1, List<int> devIdList_2)
        {
            var deviceManagerMock = new Mock<EplanDevice.IDeviceManager>();
            deviceManagerMock.Setup(x => x.GetDeviceByIndex(It.IsAny<int>()))
                .Returns(new Mock<EplanDevice.IDevice>().Object);

            typeof(Action).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManagerMock.Object);


            var groupableAction = new ActionGroup(
                "Groupable action", null,"groupable_action", null, null);
            groupableAction.DrawStyle = drawStyle;

            var group_1 = groupableAction.Insert() as Action;
            var group_2 = groupableAction.Insert() as Action;

            group_1.DevicesIndex.AddRange(devIdList_1);
            group_2.DevicesIndex.AddRange(devIdList_2);

            List<DrawInfo> drawObjs = groupableAction.GetObjectToDrawOnEplanPage();

            Assert.Multiple(() =>
            {
                foreach (var drawObj in drawObjs)
                {
                    Assert.AreEqual(groupableAction.DrawStyle, drawObj.DrawingStyle);
                }

                Assert.AreEqual(devIdList_1.Concat(devIdList_2).Distinct().Count(), drawObjs.Count);
            });
        }

        private static object[] GetObjectToDrawOnEplanPageTestCaseSource()
        {
            return new object[]
            {
                new object[]
                {
                    DrawInfo.Style.GREEN_BOX,
                    new List<int>(),
                    new List<int>()
                },
                new object[]
                {
                    DrawInfo.Style.GREEN_LOWER_BOX,
                    new List<int>() { 8, 6, 4, 2, 7 },
                    new List<int>() { 3, 6, 9 },
                },
                new object[]
                {
                    DrawInfo.Style.GREEN_RED_BOX,
                    new List<int>() { 3, 6, 9 },
                    new List<int>() { 8, 3 },
                },
                new object[]
                {
                    DrawInfo.Style.GREEN_UPPER_BOX,
                    new List<int>() { 8, 3 },
                    new List<int>() { 4, 66, 33, 22 },
                },
                new object[]
                {
                    DrawInfo.Style.NO_DRAW,
                    new List<int>() { 4, 66, 33, 22 },
                    new List<int>(),
                },
                new object[]
                {
                    DrawInfo.Style.RED_BOX,
                    new List<int>(),
                    new List<int>() { 8, 6, 4, 2, 7 },
                },
            };
        }

    }
}
