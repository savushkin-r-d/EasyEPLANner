using NUnit.Framework;
using PInvoke;
using StaticHelper;
using System;
using System.Linq;
using System.Reflection;

namespace EasyEplannerTests.StaticHelperTest
{
    [NonParallelizable]
    public class KeyboardHookHelperTest
    {
        [TestCase(PI.WM.KEYDOWN)]
        [TestCase(PI.WM.KEYUP)]
        [TestCase(PI.WM.CHAR)]
        public void ShouldBlockPlainTab_PlainTabKeyboardInput_ReturnsTrue(
            PI.WM message)
        {
            SetPressedKeys();

            var result = KeyboardHookHelper.ShouldBlockPlainTab(
                message, PI.VIRTUAL_KEY.VK_TAB);

            Assert.IsTrue(result);
        }

        [TestCase(PI.VIRTUAL_KEY.VK_CONTROL)]
        [TestCase(PI.VIRTUAL_KEY.VK_SHIFT)]
        [TestCase(PI.VIRTUAL_KEY.VK_MENU)]
        public void ShouldBlockPlainTab_TabWithModifiers_ReturnsFalse(
            uint modifier)
        {
            SetPressedKeys(modifier);

            var result = KeyboardHookHelper.ShouldBlockPlainTab(
                PI.WM.KEYDOWN, PI.VIRTUAL_KEY.VK_TAB);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldBlockPlainTab_NotTab_ReturnsFalse()
        {
            SetPressedKeys();

            var result = KeyboardHookHelper.ShouldBlockPlainTab(
                PI.WM.KEYDOWN, PI.VIRTUAL_KEY.VK_RETURN);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldBlockPlainTab_NotKeyboardInputMessage_ReturnsFalse()
        {
            SetPressedKeys();

            var result = KeyboardHookHelper.ShouldBlockPlainTab(
                PI.WM.MOUSEMOVE, PI.VIRTUAL_KEY.VK_TAB);

            Assert.IsFalse(result);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void IsCtrlPressed_UsesControlKeyState(bool isCtrlPressed)
        {
            SetPressedKeys(isCtrlPressed
                ? new[] { PI.VIRTUAL_KEY.VK_CONTROL }
                : Array.Empty<uint>());

            Assert.AreEqual(isCtrlPressed, KeyboardHookHelper.IsCtrlPressed());
        }

        [TestCase((short)0, false)]
        [TestCase((short)0x80, false)]
        [TestCase(unchecked((short)0x8000), true)]
        public void IsPressed_UsesHighOrderBit(short keyState, bool expected)
        {
            SetKeyState(_ => keyState);

            Assert.AreEqual(expected, IsPressed(PI.VIRTUAL_KEY.VK_CONTROL));
        }

        [TearDown]
        public void TearDown()
        {
            SetKeyState(key => PI.GetKeyState((int)key));
        }

        private static bool IsPressed(uint key)
        {
            var method = typeof(KeyboardHookHelper).GetMethod(
                "IsPressed",
                BindingFlags.NonPublic | BindingFlags.Static,
                null,
                new[]
                {
                    typeof(uint),
                },
                null);

            return (bool)method.Invoke(null, new object[]
            {
                key,
            });
        }

        private static void SetPressedKeys(params uint[] pressedKeys)
        {
            SetKeyState(key => pressedKeys.Contains(key)
                ? unchecked((short)0x8000)
                : (short)0);
        }

        private static void SetKeyState(Func<uint, short> keyState)
        {
            var field = typeof(KeyboardHookHelper).GetField(
                "getKeyState",
                BindingFlags.NonPublic | BindingFlags.Static);

            field.SetValue(null, keyState);
        }
    }
}
