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
            List<EplanDevice.DeviceType[]> expectedTypes,
            List<EplanDevice.DeviceSubType[]> expectedSubTypes)
        {
            var action = new ActionWash(string.Empty, null, string.Empty);

            List<IAction> actions = action.SubActions;

            for (int i = 0; i < actions.Count; i++)
            {
                EplanDevice.DeviceType[] actualTypes;
                EplanDevice.DeviceSubType[] actualSubTypes;

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
                    new List<EplanDevice.DeviceType[]>()
                    {
                        new EplanDevice.DeviceType[]
                        {
                            EplanDevice.DeviceType.DI,
                            EplanDevice.DeviceType.SB,
                            EplanDevice.DeviceType.GS,
                            EplanDevice.DeviceType.LS,
                            EplanDevice.DeviceType.FS
                        },
                        new EplanDevice.DeviceType[]
                        {
                            EplanDevice.DeviceType.DO
                        },
                        new EplanDevice.DeviceType[]
                        {
                            EplanDevice.DeviceType.M,
                            EplanDevice.DeviceType.V,
                            EplanDevice.DeviceType.DO,
                            EplanDevice.DeviceType.AO,
                            EplanDevice.DeviceType.VC,
                            EplanDevice.DeviceType.C
                        },
                        new EplanDevice.DeviceType[]
                        {
                            EplanDevice.DeviceType.M
                        },
                    },
                    new List<EplanDevice.DeviceSubType[]>()
                    {
                        null,
                        null,
                        new EplanDevice.DeviceSubType[]
                        {
                            EplanDevice.DeviceSubType.M_FREQ,
                            EplanDevice.DeviceSubType.M_REV_FREQ,
                            EplanDevice.DeviceSubType.M_REV_FREQ_2,
                            EplanDevice.DeviceSubType.M_REV_FREQ_2_ERROR,
                            EplanDevice.DeviceSubType.M_ATV,
                            EplanDevice.DeviceSubType.M_ATV_LINEAR,
                            EplanDevice.DeviceSubType.M,
                            EplanDevice.DeviceSubType.M_VIRT,
                            EplanDevice.DeviceSubType.V_AS_DO1_DI2,
                            EplanDevice.DeviceSubType.V_AS_MIXPROOF,
                            EplanDevice.DeviceSubType.V_BOTTOM_MIXPROOF,
                            EplanDevice.DeviceSubType.V_DO1,
                            EplanDevice.DeviceSubType.V_DO1_DI1_FB_OFF,
                            EplanDevice.DeviceSubType.V_DO1_DI1_FB_ON,
                            EplanDevice.DeviceSubType.V_DO1_DI2,
                            EplanDevice.DeviceSubType.V_DO2,
                            EplanDevice.DeviceSubType.V_DO2_DI2,
                            EplanDevice.DeviceSubType.V_DO2_DI2_BISTABLE,
                            EplanDevice.DeviceSubType.V_IOLINK_DO1_DI2,
                            EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF,
                            EplanDevice.DeviceSubType.V_IOLINK_VTUG_DO1,
                            EplanDevice.DeviceSubType.V_IOLINK_VTUG_DO1_DI2,
                            EplanDevice.DeviceSubType.V_IOLINK_VTUG_DO1_FB_OFF,
                            EplanDevice.DeviceSubType.V_IOLINK_VTUG_DO1_FB_ON,
                            EplanDevice.DeviceSubType.V_MIXPROOF,
                            EplanDevice.DeviceSubType.V_VIRT,
                            EplanDevice.DeviceSubType.AO,
                            EplanDevice.DeviceSubType.AO_VIRT,
                            EplanDevice.DeviceSubType.DO,
                            EplanDevice.DeviceSubType.DO_VIRT,
                            EplanDevice.DeviceSubType.VC,
                            EplanDevice.DeviceSubType.VC_IOLINK,
                            EplanDevice.DeviceSubType.VC_VIRT,
                            EplanDevice.DeviceSubType.NONE
                        },
                        new EplanDevice.DeviceSubType[]
                        {
                            EplanDevice.DeviceSubType.M_FREQ,
                            EplanDevice.DeviceSubType.M_REV_FREQ,
                            EplanDevice.DeviceSubType.M_REV_FREQ_2,
                            EplanDevice.DeviceSubType.M_REV_FREQ_2_ERROR,
                            EplanDevice.DeviceSubType.M_ATV,
                            EplanDevice.DeviceSubType.M_ATV_LINEAR,
                            EplanDevice.DeviceSubType.M,
                            EplanDevice.DeviceSubType.M_VIRT,
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
