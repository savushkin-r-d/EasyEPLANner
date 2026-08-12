using System;
using PInvoke;

namespace StaticHelper
{
    public static class KeyboardHookHelper
    {
        private const int KeyPressed = 0x8000;

        private static Func<uint, short> getKeyState =
            key => PI.GetKeyState((int)key);

        public static bool IsCtrlPressed() =>
            IsPressed(PI.VIRTUAL_KEY.VK_CONTROL);

        public static bool ShouldBlockPlainTab(PI.WM message, uint vkCode) =>
            ShouldBlockPlainTab(
                message,
                vkCode,
                IsPressed(PI.VIRTUAL_KEY.VK_CONTROL),
                IsPressed(PI.VIRTUAL_KEY.VK_SHIFT),
                IsPressed(PI.VIRTUAL_KEY.VK_MENU));

        private static bool ShouldBlockPlainTab(
            PI.WM message, uint vkCode, bool isCtrlPressed,
            bool isShiftPressed, bool isAltPressed) =>
            IsPlainTab(vkCode, isCtrlPressed, isShiftPressed, isAltPressed) &&
            IsKeyboardInputMessage(message);

        private static bool IsPlainTab(
            uint vkCode, bool isCtrlPressed, bool isShiftPressed,
            bool isAltPressed) =>
            vkCode == PI.VIRTUAL_KEY.VK_TAB &&
            !isCtrlPressed &&
            !isShiftPressed &&
            !isAltPressed;

        private static bool IsKeyboardInputMessage(PI.WM message) =>
            message is PI.WM.KEYDOWN or PI.WM.KEYUP or PI.WM.CHAR;

        private static bool IsPressed(uint key) =>
            (getKeyState(key) & KeyPressed) != 0;
    }
}
