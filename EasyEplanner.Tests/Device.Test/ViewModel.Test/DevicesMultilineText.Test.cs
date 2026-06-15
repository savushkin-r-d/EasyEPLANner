using EasyEPlanner.Devices.ViewModel;
using EplanDevice;
using Moq;
using NUnit.Framework;
using StaticHelper;

namespace EasyEPlanner.Devices.Tests
{
    public class DevicesMultilineTextTest
    {
        [Test]
        public void GetEplanDescription_ReturnsFunctionDescriptionWhenPresent()
        {
            var device = CreateDevice();
            device.Function = Mock.Of<IEplanFunction>(f =>
                f.Description == "from eplan");

            Assert.AreEqual("from eplan",
                DevicesMultilineText.GetEplanDescription(device));
        }

        [Test]
        public void GetEplanDescription_FallsBackToDeviceDescriptionWithoutFunction()
        {
            var device = CreateDevice();

            Assert.AreEqual("device desc",
                DevicesMultilineText.GetEplanDescription(device));
        }

        [Test]
        public void GetEplanDescription_ReturnsEmptyForNullDevice()
        {
            Assert.AreEqual(string.Empty,
                DevicesMultilineText.GetEplanDescription(null));
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("single", "single")]
        [TestCase("first\nsecond", "first · second")]
        [TestCase("first\r\nsecond", "first · second")]
        [TestCase("first\u00B6second", "first · second")]
        [TestCase("  first  \n  second  ", "first · second")]
        public void FormatForCell_JoinsLinesWithMiddleDot(string input,
            string expected)
        {
            Assert.AreEqual(expected, DevicesMultilineText.FormatForCell(input));
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("single", "single")]
        [TestCase("first\nsecond", "first\r\nsecond")]
        [TestCase("first\u00B6second", "first\r\nsecond")]
        public void FormatForTooltip_PreservesLineBreaks(string input,
            string expected)
        {
            Assert.AreEqual(expected,
                DevicesMultilineText.FormatForTooltip(input));
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("single", "single")]
        [TestCase("first\u00B6second", "first\r\nsecond")]
        [TestCase("first\r\nsecond", "first\r\nsecond")]
        [TestCase("first\nsecond", "first\r\nsecond")]
        public void FormatForEditor_ConvertsSeparatorsToLineBreaks(string input,
            string expected)
        {
            Assert.AreEqual(expected,
                DevicesMultilineText.FormatForEditor(input));
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("single", "single")]
        [TestCase("first\r\nsecond", "first\u00B6second")]
        [TestCase("first\nsecond", "first\u00B6second")]
        [TestCase("first\rsecond", "first\u00B6second")]
        public void ParseFromEditor_ConvertsLineBreaksToParagraphSign(string input,
            string expected)
        {
            Assert.AreEqual(expected,
                DevicesMultilineText.ParseFromEditor(input));
        }

        [Test]
        public void ParseFromEditor_ThenFormatForEditor_RoundTripsEplanText()
        {
            const string eplanText = "line1\u00B6line2\u00B6line3";
            var editorText = DevicesMultilineText.FormatForEditor(eplanText);
            var restored = DevicesMultilineText.ParseFromEditor(editorText);

            Assert.AreEqual(eplanText, restored);
        }

        private static DO CreateDevice()
        {
            var device = new DO("TANK2DO1", "+TANK2-DO1", "device desc",
                1, "TANK", 2);
            device.SetSubType("DO");
            return device;
        }
    }
}
