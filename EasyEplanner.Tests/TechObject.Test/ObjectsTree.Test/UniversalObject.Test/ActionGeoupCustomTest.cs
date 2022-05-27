using System.Collections.Generic;
using TechObject;
using NUnit.Framework;
using Moq;

namespace Tests.TechObject
{
    class ActionGroupCustomTest
    {

        [TestCase(new string[] { "action1" },
            new string[] { "parameter1" })]
        [TestCase(new string[] { "action1", "action2" },
            new string[] { "parameter1", "parameter2" })]
        [TestCase(new string[] { "action1", "action2", "action3" },
            new string[] { "parameter1", "parameter2", "parameter3" })]
        public void Constructor_NewAction_CheckActionsAndPropertiesSequenceByLuaName(
            string[] expectedActions, string[] expectedParameters)
        {
            var action = new ActionGroupCustom("", null, "",
                () =>
                {
                    var openedDeviceAction = new ActionCustom("",
                        null, "");
                    foreach (var actionLuaName in expectedActions)
                    {
                        openedDeviceAction.CreateAction(new Action("", null,
                            actionLuaName, null));
                    }

                    foreach (var parameterLuaName in expectedParameters)
                    {
                        openedDeviceAction.CreateParameter(
                            new ActiveParameter(parameterLuaName, ""));

                    }
                    return openedDeviceAction;
                });

            List<IAction> actions = action.SubActions[0].SubActions;
            List<BaseParameter> parameters = (action.SubActions[0]
                as ActionCustom).Parameters;

            for (int i = 0; i < actions.Count; i++)
            {
                Assert.AreEqual(expectedActions[i], actions[i].LuaName);
            }

            for (int i = 0; i < actions.Count; i++)
            {
                Assert.AreEqual(expectedParameters[i], parameters[i].LuaName);
            }
        }

        [Test]
        public void SaveAsLuaTable_NewAction_ReturnEmptyString()
        {
            var action = new ActionGroupCustom("", null, "",
                () =>
                {
                    var openedDeviceAction = new ActionCustom("",
                        null, "");
                    openedDeviceAction.CreateAction(new Action("", null,
                        "", null));

                    openedDeviceAction.CreateParameter(
                        new ActiveParameter("", ""));

                    return openedDeviceAction;
                });

            Assert.AreEqual(string.Empty, action.SaveAsLuaTable(""));
        }

        [TestCaseSource(nameof(SaveAsLuaTableTestCaseSource))]
        public void SaveAsLuaTable_NewAction_ReturnCodeToSave(
            ActionGroupCustom action, string expectedCode)
        {
            Assert.AreEqual(expectedCode, action.SaveAsLuaTable("\t"));
        }

        private static object[] SaveAsLuaTableTestCaseSource()
        {
            var prefix = "\t";
            var deviceName = "device";
            var groupActionLuaName = "groupAction";
            var action1_LuaName = "action1";
            var parameter1_LuaName = "parameter1";

            var param1_value = 1;
            var param2_value = 2;
            var devs = new int[] { 1, 2 };

            var validDevMock = new Mock<EplanDevice.IDevice>();
            validDevMock.SetupGet(x => x.Name).Returns(deviceName);
            var deviceManagerMock = new Mock<EplanDevice.IDeviceManager>();
            deviceManagerMock
                .Setup(x => x.GetDeviceByIndex(It.Is<int>(y => true)))
                .Returns(validDevMock.Object);

            var actionDF = new ActionGroupCustom("", null, groupActionLuaName,
                () =>
                {
                    var openedDeviceAction = new ActionCustom("",
                        null, "");
                    openedDeviceAction.CreateAction(new Action("", null,
                        action1_LuaName,
                        null, null, null,
                        deviceManagerMock.Object));

                    openedDeviceAction.CreateParameter(
                        new ActiveParameter(parameter1_LuaName, ""));

                    return openedDeviceAction;
                });

            actionDF.SubActions[0].SubActions[0]
                .DeviceIndex.AddRange(devs);
            actionDF.AddParam(param1_value, parameter1_LuaName, 0);

            var actionDFCode =
                $"{prefix}{groupActionLuaName} =\n" +
                $"{prefix}\t{{\n" +
                $"{prefix}\t\t{{\n" +
                $"{prefix}\t\t{action1_LuaName} =\n" +
                $"{prefix}\t\t\t{{\n" +
                $"{prefix}\t\t\t'{deviceName}', '{deviceName}'\n" +
                $"{prefix}\t\t\t}},\n" +
                $"{prefix}\t\t{parameter1_LuaName} = {param1_value},\n" +
                $"{prefix}\t\t}},\n" +
                $"{prefix}\t}},\n";


            var actionSF_1A_1P = new ActionGroupCustom("", null, groupActionLuaName,
                () =>
                {
                    var openedDeviceAction = new ActionCustom("",
                        null, "");
                    openedDeviceAction.CreateAction(new Action("", null,
                        "",
                        null, null, null,
                        deviceManagerMock.Object));

                    openedDeviceAction.CreateParameter(
                        new ActiveParameter("", ""));

                    return openedDeviceAction;
                });

            actionSF_1A_1P.SubActions[0].SubActions[0]
                .DeviceIndex.AddRange(devs);
            actionSF_1A_1P.AddParam(param1_value, "P_0", 0);

            var actionSF_1A_1P_Code =
                $"{prefix}{groupActionLuaName} =\n" +
                $"{prefix}\t{{\n" +
                $"{prefix}\t{{ {{ '{deviceName}', '{deviceName}' }}, {param1_value}, }},\n" +
                $"{prefix}\t}},\n";

            var actionSF_2A_2P = new ActionGroupCustom("", null, groupActionLuaName,
                () =>
                {
                    var openedDeviceAction = new ActionCustom("",
                        null, "");
                    openedDeviceAction.CreateAction(new Action("", null,
                        "", null, null, null,
                        deviceManagerMock.Object));
                    openedDeviceAction.CreateAction(new Action("", null,
                        "", null, null, null,
                        deviceManagerMock.Object));

                    openedDeviceAction.CreateParameter(
                        new ActiveParameter("", ""));
                    openedDeviceAction.CreateParameter(
                        new ActiveParameter("", ""));

                    return openedDeviceAction;
                });

            actionSF_2A_2P.SubActions[0].SubActions[0]
                .DeviceIndex.AddRange(devs);
            actionSF_2A_2P.SubActions[0].SubActions[1]
                .DeviceIndex.AddRange(devs);
            actionSF_2A_2P.AddParam(param1_value, "P_0", 0);
            actionSF_2A_2P.AddParam(param2_value, "P_1", 0);

            var actionSF_2A_2P_Code =
                $"{prefix}{groupActionLuaName} =\n" +
                $"{prefix}\t{{\n" +
                $"{prefix}\t{{\n" +
                $"{prefix}\t\t{{ '{deviceName}', '{deviceName}' }},\n" +
                $"{prefix}\t\t{{ '{deviceName}', '{deviceName}' }},\n" +
                $"{prefix}\t\t {param1_value}, {param2_value},\n" +
                $"{prefix}\t}},\n" +
                $"{prefix}\t}},\n";

            var actionSF_3A_3P_withEmpties = new ActionGroupCustom("", null, groupActionLuaName,
                () =>
                {
                    var openedDeviceAction = new ActionCustom("",
                        null, "");
                    openedDeviceAction.CreateAction(new Action("", null,
                        "", null, null, null,
                        deviceManagerMock.Object));
                    openedDeviceAction.CreateAction(new Action("", null,
                        "", null, null, null,
                        deviceManagerMock.Object));
                    openedDeviceAction.CreateAction(new Action("", null,
                        "", null, null, null,
                        deviceManagerMock.Object));

                    openedDeviceAction.CreateParameter(
                        new ActiveParameter("", ""));
                    openedDeviceAction.CreateParameter(
                        new ActiveParameter("", ""));
                    openedDeviceAction.CreateParameter(
                        new ActiveParameter("", ""));

                    return openedDeviceAction;
                });

            actionSF_3A_3P_withEmpties.SubActions[0].SubActions[1]
                .DeviceIndex.AddRange(devs);
            actionSF_3A_3P_withEmpties.AddParam(param2_value, "P_1", 0);

            var actionSF_3A_3P_withEmpties_Code =
                $"{prefix}{groupActionLuaName} =\n" +
                $"{prefix}\t{{\n" +
                $"{prefix}\t{{\n" +
                $"{prefix}\t\t{{}},\n" +
                $"{prefix}\t\t{{ '{deviceName}', '{deviceName}' }},\n" +
                $"{prefix}\t\t -1, {param2_value},\n" +
                $"{prefix}\t}},\n" +
                $"{prefix}\t}},\n";


            var SaveDefaultFormat = new object[]
            {
                actionDF,
                actionDFCode
            };

            var SaveShortFormat_1A_1P = new object[]
            {
                actionSF_1A_1P,
                actionSF_1A_1P_Code
            };

            var SaveShortFormat_2A_2P = new object[]
            {
                actionSF_2A_2P,
                actionSF_2A_2P_Code
            };

            var SaveShortFormat_3A_3P_withEmpties = new object[]
            {
                actionSF_3A_3P_withEmpties,
                actionSF_3A_3P_withEmpties_Code
            };

            return new object[]
            {
                SaveDefaultFormat,
                SaveShortFormat_1A_1P,
                SaveShortFormat_2A_2P,
                SaveShortFormat_3A_3P_withEmpties,
            };
        }

    }
}
