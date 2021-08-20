using System.Collections.Generic;
using TechObject;
using NUnit.Framework;

namespace Tests.TechObject
{
    class ActionWashTest
    {
        [TestCaseSource(nameof(CheckActionsSequenceByLuaName))]
        public void Constructor_NewAction_CheckActionsSequenceByLuaName(
            string[] expectedSequence)
        {
            var action = new ActionWash(string.Empty, null, string.Empty);

            List<IAction> actions = action.SubActions;

            for (int i = 0; i < actions.Count; i++)
            {
                Assert.AreEqual(expectedSequence[i], actions[i].LuaName);
            }
        }

        private static object[] CheckActionsSequenceByLuaName()
        {
            return new object[]
            {
                new string[]
                {
                    "DI",
                    "DO",
                    "devices",
                    "rev_devices"
                },
            };
        }

        [TestCaseSource(nameof(CheckAllowedDevicesInEveryAction))]
        public void Constructor_NewAction_CheckAlloweDevicesInEveryAction(
            List<Device.DeviceType[]> expectedTypes,
            List<Device.DeviceSubType[]> expectedSubTypes)
        {
            var action = new ActionWash(string.Empty, null, string.Empty);

            List<IAction> actions = action.SubActions;

            for (int i = 0; i < actions.Count; i++)
            {
                Device.DeviceType[] actualTypes;
                Device.DeviceSubType[] actualSubTypes;

                actions[i].GetDisplayObjects(out actualTypes,
                    out actualSubTypes, out _);

                Assert.Multiple(() =>
                {
                    Assert.AreEqual(expectedTypes[i], actualTypes);
                    Assert.AreEqual(expectedSubTypes[i], actualSubTypes);
                });
            }
        }

        private static object[] CheckAllowedDevicesInEveryAction()
        {
            return new object[]
            {
                new object[]
                {
                    new List<Device.DeviceType[]>()
                    {
                        new Device.DeviceType[]
                        {
                            Device.DeviceType.DI,
                            Device.DeviceType.SB
                        },
                        new Device.DeviceType[]
                        {
                            Device.DeviceType.DO
                        },
                        new Device.DeviceType[]
                        {
                            Device.DeviceType.M,
                            Device.DeviceType.V,
                            Device.DeviceType.DO,
                            Device.DeviceType.AO,
                            Device.DeviceType.VC,
                            Device.DeviceType.C
                        },
                        new Device.DeviceType[]
                        {
                            Device.DeviceType.M
                        },
                    },
                    new List<Device.DeviceSubType[]>()
                    {
                        null,
                        null,
                        new Device.DeviceSubType[]
                        {
                            Device.DeviceSubType.M_FREQ,
                            Device.DeviceSubType.M_REV_FREQ,
                            Device.DeviceSubType.M_REV_FREQ_2,
                            Device.DeviceSubType.M_REV_FREQ_2_ERROR,
                            Device.DeviceSubType.M_ATV,
                            Device.DeviceSubType.M_ATV_LINEAR,
                            Device.DeviceSubType.M,
                            Device.DeviceSubType.M_VIRT,
                            Device.DeviceSubType.V_AS_DO1_DI2,
                            Device.DeviceSubType.V_AS_MIXPROOF,
                            Device.DeviceSubType.V_BOTTOM_MIXPROOF,
                            Device.DeviceSubType.V_DO1,
                            Device.DeviceSubType.V_DO1_DI1_FB_OFF,
                            Device.DeviceSubType.V_DO1_DI1_FB_ON,
                            Device.DeviceSubType.V_DO1_DI2,
                            Device.DeviceSubType.V_DO2,
                            Device.DeviceSubType.V_DO2_DI2,
                            Device.DeviceSubType.V_DO2_DI2_BISTABLE,
                            Device.DeviceSubType.V_IOLINK_DO1_DI2,
                            Device.DeviceSubType.V_IOLINK_MIXPROOF,
                            Device.DeviceSubType.V_IOLINK_VTUG_DO1,
                            Device.DeviceSubType.V_IOLINK_VTUG_DO1_DI2,
                            Device.DeviceSubType.V_IOLINK_VTUG_DO1_FB_OFF,
                            Device.DeviceSubType.V_IOLINK_VTUG_DO1_FB_ON,
                            Device.DeviceSubType.V_MIXPROOF,
                            Device.DeviceSubType.V_VIRT,
                            Device.DeviceSubType.AO,
                            Device.DeviceSubType.AO_VIRT,
                            Device.DeviceSubType.DO,
                            Device.DeviceSubType.DO_VIRT,
                            Device.DeviceSubType.VC,
                            Device.DeviceSubType.VC_IOLINK,
                            Device.DeviceSubType.VC_VIRT,
                            Device.DeviceSubType.NONE
                        },
                        new Device.DeviceSubType[]
                        {
                            Device.DeviceSubType.M_FREQ,
                            Device.DeviceSubType.M_REV_FREQ,
                            Device.DeviceSubType.M_REV_FREQ_2,
                            Device.DeviceSubType.M_REV_FREQ_2_ERROR,
                            Device.DeviceSubType.M_ATV,
                            Device.DeviceSubType.M_ATV_LINEAR,
                            Device.DeviceSubType.M,
                            Device.DeviceSubType.M_VIRT,
                        }
                    },
                },
            };
        }

        [Test]
        public void Items_NewAction_Returns5AsItemsLength()
        {
            var action = new ActionWash(string.Empty, null, string.Empty);

            int expectedLength = 5;

            Assert.AreEqual(expectedLength, action.Items.Length);
        }
    }
}
