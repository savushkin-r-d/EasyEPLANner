using IO.ViewModel;
using NUnit.Framework;

namespace IOTests
{
    public class ClampBindingTextTest
    {
        [Test]
        public void FormatForCell_JoinsBindingsWithMiddleDot()
        {
            const string text = "DEV1:AO || DEV2:AI";

            Assert.AreEqual("DEV1:AO · DEV2:AI",
                ClampBindingText.FormatForCell(text));
        }

        [Test]
        public void FormatForTooltip_SplitsBindingsByLine()
        {
            const string text = "DEV1:AO || DEV2:AI";

            Assert.AreEqual("DEV1:AO\r\nDEV2:AI",
                ClampBindingText.FormatForTooltip(text));
        }

        [Test]
        public void FormatForCell_FormatsMultilineSingleBinding()
        {
            const string text = "first\u00B6second";

            Assert.AreEqual("first · second",
                ClampBindingText.FormatForCell(text));
        }
    }
}
