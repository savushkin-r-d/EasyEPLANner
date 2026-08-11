using NUnit.Framework;
using PInvoke;
using StaticHelper;
using System.Reflection;

namespace EasyEplannerTests.StaticHelperTest
{
    public class KeyboardHookHelperTest
    {
        [TestCase(PI.WM.KEYDOWN)]
        [TestCase(PI.WM.KEYUP)]
        [TestCase(PI.WM.CHAR)]
        public void ShouldBlockPlainTab_PlainTabKeyboardInput_ReturnsTrue(
            PI.WM message)
        {
            var result = ShouldBlockPlainTab(
                message, PI.VIRTUAL_KEY.VK_TAB, false, false, false);

            Assert.IsTrue(result);
        }

        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        [TestCase(true, true, true)]
        public void ShouldBlockPlainTab_TabWithModifiers_ReturnsFalse(
            bool isCtrlPressed, bool isShiftPressed, bool isAltPressed)
        {
            var result = ShouldBlockPlainTab(
                PI.WM.KEYDOWN, PI.VIRTUAL_KEY.VK_TAB, isCtrlPressed,
                isShiftPressed, isAltPressed);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldBlockPlainTab_NotTab_ReturnsFalse()
        {
            var result = ShouldBlockPlainTab(
                PI.WM.KEYDOWN, PI.VIRTUAL_KEY.VK_RETURN, false, false, false);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldBlockPlainTab_NotKeyboardInputMessage_ReturnsFalse()
        {
            var result = ShouldBlockPlainTab(
                PI.WM.MOUSEMOVE, PI.VIRTUAL_KEY.VK_TAB, false, false, false);

            Assert.IsFalse(result);
        }

        private static bool ShouldBlockPlainTab(
            PI.WM message, uint vkCode, bool isCtrlPressed,
            bool isShiftPressed, bool isAltPressed)
        {
            var method = typeof(KeyboardHookHelper).GetMethod(
                nameof(KeyboardHookHelper.ShouldBlockPlainTab),
                BindingFlags.NonPublic | BindingFlags.Static,
                null,
                new[]
                {
                    typeof(PI.WM),
                    typeof(uint),
                    typeof(bool),
                    typeof(bool),
                    typeof(bool),
                },
                null);

            return (bool)method.Invoke(null, new object[]
            {
                message,
                vkCode,
                isCtrlPressed,
                isShiftPressed,
                isAltPressed,
            });
        }
    }
}
