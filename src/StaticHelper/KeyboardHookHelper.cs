using PInvoke;

namespace StaticHelper
{
    public static class KeyboardHookHelper
    {
        private const short Shifted = 0x80;

        public static bool IsCtrlPressed() =>
            IsPressed(PI.VIRTUAL_KEY.VK_CONTROL);

        public static bool ShouldBlockPlainTab(PI.WM message, uint vkCode) =>
            IsPlainTab(vkCode) && IsKeyboardInputMessage(message);

        private static bool IsPlainTab(uint vkCode) =>
            vkCode == PI.VIRTUAL_KEY.VK_TAB &&
            !IsPressed(PI.VIRTUAL_KEY.VK_CONTROL) &&
            !IsPressed(PI.VIRTUAL_KEY.VK_SHIFT) &&
            !IsPressed(PI.VIRTUAL_KEY.VK_MENU);

        private static bool IsKeyboardInputMessage(PI.WM message) =>
            message is PI.WM.KEYDOWN or PI.WM.KEYUP or PI.WM.CHAR;

        private static bool IsPressed(uint key) =>
            (PI.GetKeyState((int)key) & Shifted) > 0;
    }
}
