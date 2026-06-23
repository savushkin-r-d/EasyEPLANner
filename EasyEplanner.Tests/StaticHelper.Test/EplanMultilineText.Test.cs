using NUnit.Framework;
using StaticHelper;

namespace IOTests
{
    public class EplanMultilineTextTest
    {
        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("single", "single")]
        [TestCase("first\nsecond", "first · second")]
        [TestCase("first\r\nsecond", "first · second")]
        [TestCase("first\u00B6second", "first · second")]
        public void FormatForCell_JoinsLinesWithMiddleDot(string input,
            string expected)
        {
            Assert.AreEqual(expected, EplanMultilineText.FormatForCell(input));
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
                EplanMultilineText.FormatForTooltip(input));
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("single", "single")]
        [TestCase("first\u00B6second", "first\r\nsecond")]
        public void FormatForEditor_ConvertsSeparatorsToLineBreaks(string input,
            string expected)
        {
            Assert.AreEqual(expected,
                EplanMultilineText.FormatForEditor(input));
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("single", "single")]
        [TestCase("first\r\nsecond", "first\u00B6second")]
        [TestCase("first\nsecond", "first\u00B6second")]
        public void ParseFromEditor_ConvertsLineBreaksToParagraphSign(
            string input, string expected)
        {
            Assert.AreEqual(expected,
                EplanMultilineText.ParseFromEditor(input));
        }
    }
}
