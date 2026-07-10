using EplanDevice;
using IO;
using Moq;
using StaticHelper;
using System.Collections.Generic;

namespace IOTests
{
    internal static class BindingErrorTestHelper
    {
        public static IIOModule CreateIoModuleWithInvalidClamp(
            int clamp = 1,
            string functionalText = "+OBJ1-V1")
        {
            var clampFunction = Mock.Of<IEplanFunction>(f =>
                f.FunctionalText == functionalText);
            var devices = new List<IIODevice>[clamp + 1];

            var ioModule = Mock.Of<IIOModule>(m =>
                m.Info.ChannelClamps == new[] { clamp } &&
                m.Devices == devices);
            Mock.Get(ioModule)
                .Setup(m => m.ClampFunctions)
                .Returns(new Dictionary<int, IEplanFunction>()
                {
                    { clamp, clampFunction },
                });

            return ioModule;
        }

        public static IIOModule CreateIoModuleWithValidClamp(int clamp = 1)
        {
            var devices = new List<IIODevice>[clamp + 1];
            devices[clamp] = new List<IIODevice> { Mock.Of<IIODevice>() };

            return Mock.Of<IIOModule>(m =>
                m.Info.ChannelClamps == new[] { clamp } &&
                m.Devices == devices);
        }

        public static IIONode CreateIoNode(
            IList<IIOModule> modules = null,
            IList<IIONode> extensions = null,
            string location = "+CAB1",
            string locationDescription = "Шкаф 1")
        {
            return Mock.Of<IIONode>(n =>
                n.N == 1 &&
                n.Name == "A100" &&
                n.Type == IONode.TYPES.T_ETHERNET &&
                n.TypeStr == "AO" &&
                n.Location == location &&
                n.LocationDescription == locationDescription &&
                n.IOModules == (modules ?? new List<IIOModule>()) &&
                n.ExtensionModules == (extensions ?? new List<IIONode>()) &&
                n.Function.IP == "ip" &&
                n.Function.SubnetMask == "mask" &&
                n.Function.Gateway == "gateway" &&
                n.Function.Expanded == false);
        }
    }
}
