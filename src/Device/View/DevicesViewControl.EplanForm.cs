using PInvoke;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace EasyEPlanner.Devices.View
{
    /// <summary>
    /// Встройка окна в панель EPLAN «Устройства».
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class DevicesViewControl : Form
    {
        public static readonly string CfgShowWindowKey = "show_devices_new_window";

        private bool isLoaded;

        private static readonly string caption = "Устройства\0";
        private static readonly byte[] newCapt =
            EncodingDetector.Windows1251.GetBytes(caption);

        private IntPtr dialogHookPtr = IntPtr.Zero;
        private PI.HookProc dialogCallbackDelegate;
        private IntPtr dialogHandle = IntPtr.Zero;
        private IntPtr panelPtr = IntPtr.Zero;
        private static IntPtr wndDevicesVisiblePtr;

        private PI.LowLevelKeyboardProc mainWndKeyboardCallbackDelegate;
        private IntPtr globalKeyboardHookPtr = IntPtr.Zero;

        /// <summary>
        /// Показать окно в панели EPLAN.
        /// </summary>
        public void ShowDlg()
        {
            Process currentProcess = Process.GetCurrentProcess();

            const int wndWmCommand = 35116;
            const string windowName = "Устройства";

            if (isLoaded)
            {
                GUIHelper.ShowHiddenWindow(currentProcess,
                    wndDevicesVisiblePtr, wndWmCommand);
                return;
            }

            GUIHelper.SearchWindowDescriptor(currentProcess, windowName,
                wndWmCommand, ref dialogHandle, ref wndDevicesVisiblePtr);
            if (wndDevicesVisiblePtr == IntPtr.Zero)
                return;

            GUIHelper.ShowHiddenWindow(currentProcess,
                wndDevicesVisiblePtr, wndWmCommand);

            GUIHelper.ChangeWindowMainPanels(ref dialogHandle, ref panelPtr);

            Controls.Clear();
            PI.SetParent(MainTableLayoutPanel.Handle, dialogHandle);
            ChangeUISize();
            SetUpHook();
            isLoaded = true;
            ChangeUISize();
        }

        public static void SaveCfg()
        {
            SaveCfg(PI.IsWindowVisible(wndDevicesVisiblePtr));
        }

        public static void SaveCfg(bool wndState)
        {
            var path = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData);
            var ini = new IniFile(path + @"\Eplan\eplan.cfg");
            ini.WriteString("main", CfgShowWindowKey, wndState.ToString().ToLower());
        }

        private void SetUpHook()
        {
            dialogCallbackDelegate = DlgWndHookCallbackFunction;
            mainWndKeyboardCallbackDelegate = GlobalHookKeyboardCallbackFunction;

            uint pid = PI.GetWindowThreadProcessId(dialogHandle, IntPtr.Zero);
            dialogHookPtr = PI.SetWindowsHookEx(PI.HookType.WH_CALLWNDPROC,
                dialogCallbackDelegate, IntPtr.Zero, pid);

            globalKeyboardHookPtr = PI.SetWindowsHookEx(
                PI.HookType.WH_KEYBOARD_LL, mainWndKeyboardCallbackDelegate,
                IntPtr.Zero, 0);

            if (globalKeyboardHookPtr == IntPtr.Zero)
            {
                MessageBox.Show("Ошибка! Не удалось переназначить клавиши!");
            }
        }

        private IntPtr GlobalHookKeyboardCallbackFunction(int code,
            PI.WM wParam, PI.KBDLLHOOKSTRUCT lParam)
        {
            const short shifted = 0x80;
            bool ctrl = (PI.GetKeyState((int)PI.VIRTUAL_KEY.VK_CONTROL) & shifted) > 0;
            uint vkCode = lParam.vkCode;

            if (wParam == PI.WM.KEYDOWN && ctrl &&
                (vkCode is PI.VIRTUAL_KEY.VK_PRIOR or PI.VIRTUAL_KEY.VK_NEXT))
            {
                return (IntPtr)1;
            }

            if (code < 0 || devicesTree is null ||
                !(devicesTree.Focused || isCellEditing))
            {
                return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }

            if (wParam is PI.WM.KEYUP or PI.WM.CHAR &&
                vkCode is PI.VIRTUAL_KEY.VK_DELETE &&
                (devicesTree.Focused || isCellEditing))
            {
                return (IntPtr)1;
            }

            if (wParam is not PI.WM.KEYDOWN)
                return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);

            if (KeyCommands.ContainsKey(vkCode) && ctrl && isCellEditing)
            {
                PI.SendMessage(PI.GetFocus(), KeyCommands[vkCode], 0, 0);
                return (IntPtr)1;
            }

            switch (vkCode)
            {
                case PI.VIRTUAL_KEY.VK_ESCAPE:
                case PI.VIRTUAL_KEY.VK_RETURN:
                case PI.VIRTUAL_KEY.VK_DELETE:
                case PI.VIRTUAL_KEY.VK_UP:
                case PI.VIRTUAL_KEY.VK_DOWN:
                case PI.VIRTUAL_KEY.VK_LEFT:
                case PI.VIRTUAL_KEY.VK_RIGHT:
                    PI.SendMessage(PI.GetFocus(), (int)PI.WM.KEYDOWN, (int)vkCode, 0);
                    return (IntPtr)1;
            }

            return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        private static readonly Dictionary<uint, uint> KeyCommands =
            new Dictionary<uint, uint>
            {
                [(uint)Keys.X] = (int)PI.WM.CUT,
                [(uint)Keys.C] = (int)PI.WM.COPY,
                [(uint)Keys.V] = (int)PI.WM.PASTE,
            };

        private IntPtr DlgWndHookCallbackFunction(int code, IntPtr wParam,
            IntPtr lParam)
        {
            PI.CWPSTRUCT msg = (PI.CWPSTRUCT)System.Runtime.InteropServices
                .Marshal.PtrToStructure(lParam, typeof(PI.CWPSTRUCT));

            if (msg.hwnd == panelPtr)
            {
                switch (msg.message)
                {
                    case (int)PI.WM.MOVE:
                    case (int)PI.WM.SIZE:
                        ChangeUISize();
                        break;
                }

                return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }

            if (msg.hwnd != dialogHandle)
                return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);

            switch (msg.message)
            {
                case (int)PI.WM.GETTEXTLENGTH:
                    return (IntPtr)newCapt.Length;

                case (int)PI.WM.SETTEXT:
                    return IntPtr.Zero;

                case (int)PI.WM.DESTROY:
                    PI.UnhookWindowsHookEx(dialogHookPtr);
                    dialogHookPtr = IntPtr.Zero;
                    dialogHandle = IntPtr.Zero;

                    PI.SetParent(MainTableLayoutPanel.Handle, Handle);
                    Controls.Add(MainTableLayoutPanel);
                    MainTableLayoutPanel.Hide();
                    System.Threading.Thread.Sleep(1);
                    isLoaded = false;
                    break;

                case (int)PI.WM.GETTEXT:
                    System.Runtime.InteropServices.Marshal.Copy(
                        newCapt, 0, lParam, newCapt.Length);
                    return (IntPtr)newCapt.Length;
            }

            return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        private void ChangeUISize()
        {
            IntPtr dialogPtr = PI.GetParent(MainTableLayoutPanel.Handle);

            PI.GetWindowRect(dialogPtr, out PI.RECT rctDialog);

            MainTableLayoutPanel.Location = new Point(0, 0);
            MainTableLayoutPanel.Width = rctDialog.Right - rctDialog.Left;
            MainTableLayoutPanel.Height = rctDialog.Bottom - rctDialog.Top;
            searchBoxTLP.Invalidate();
        }

        private void DevicesTree_MouseEnter(object sender, EventArgs e)
        {
            if (mainWndKeyboardCallbackDelegate is null)
                return;

            globalKeyboardHookPtr = PI.SetWindowsHookEx(PI.HookType.WH_KEYBOARD_LL,
                mainWndKeyboardCallbackDelegate, IntPtr.Zero, 0);
        }

        private void DevicesTree_MouseLeave(object sender, EventArgs e)
        {
            if (globalKeyboardHookPtr == IntPtr.Zero)
                return;

            PI.UnhookWindowsHookEx(globalKeyboardHookPtr);
            globalKeyboardHookPtr = IntPtr.Zero;
        }
    }
}
