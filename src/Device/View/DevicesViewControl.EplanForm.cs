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

            if (TryEmbedInEplanPanel(currentProcess, windowName, wndWmCommand))
                return;

            ShowFloating();
            Show();
        }

        private bool TryEmbedInEplanPanel(Process currentProcess,
            string windowName, int wndWmCommand)
        {
            if (!GUIHelper.SearchWindowDescriptor(currentProcess, windowName,
                wndWmCommand, ref dialogHandle, ref wndDevicesVisiblePtr))
            {
                return false;
            }

            if (wndDevicesVisiblePtr == IntPtr.Zero)
                return false;

            System.Threading.Thread.Sleep(200);

            GUIHelper.ShowHiddenWindow(currentProcess,
                wndDevicesVisiblePtr, wndWmCommand);

            if (!GUIHelper.ChangeWindowMainPanels(ref dialogHandle, ref panelPtr))
                return false;

            Controls.Clear();
            PI.SetParent(MainTableLayoutPanel.Handle, dialogHandle);
            ChangeUISize();
            SetUpHook();
            isLoaded = true;
            ChangeUISize();
            return true;
        }

        private void ShowFloating()
        {
            if (MainTableLayoutPanel.Parent != this)
            {
                PI.SetParent(MainTableLayoutPanel.Handle, Handle);
                Controls.Add(MainTableLayoutPanel);
            }

            MainTableLayoutPanel.Dock = DockStyle.Fill;
            MainTableLayoutPanel.Show();
            isLoaded = false;
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

        private void InitKeyboardHook()
        {
            mainWndKeyboardCallbackDelegate ??= GlobalHookKeyboardCallbackFunction;
        }

        private void InstallKeyboardHook()
        {
            if (globalKeyboardHookPtr != IntPtr.Zero)
                return;

            InitKeyboardHook();
            globalKeyboardHookPtr = PI.SetWindowsHookEx(
                PI.HookType.WH_KEYBOARD_LL, mainWndKeyboardCallbackDelegate,
                IntPtr.Zero, 0);

            if (globalKeyboardHookPtr == IntPtr.Zero)
            {
                MessageBox.Show("Ошибка! Не удалось переназначить клавиши!");
            }
        }

        private void ReleaseKeyboardHook()
        {
            if (globalKeyboardHookPtr == IntPtr.Zero)
                return;

            PI.UnhookWindowsHookEx(globalKeyboardHookPtr);
            globalKeyboardHookPtr = IntPtr.Zero;
        }

        private void SetUpHook()
        {
            dialogCallbackDelegate = DlgWndHookCallbackFunction;

            uint pid = PI.GetWindowThreadProcessId(dialogHandle, IntPtr.Zero);
            dialogHookPtr = PI.SetWindowsHookEx(PI.HookType.WH_CALLWNDPROC,
                dialogCallbackDelegate, IntPtr.Zero, pid);

            InstallKeyboardHook();
        }

        private bool IsKeyboardHookActive =>
            devicesTree?.Focused == true || isCellEditing || textBox_search.Focused;

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

            if (code < 0 || devicesTree is null || !IsKeyboardHookActive)
            {
                return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }

            if (wParam is PI.WM.KEYUP or PI.WM.CHAR)
            {
                switch ((Keys)vkCode)
                {
                    case Keys.Delete:
                    case Keys.C when ctrl:
                    case Keys.V when ctrl:
                    case Keys.X when ctrl:
                        if (IsKeyboardHookActive)
                            return (IntPtr)1;
                        break;
                }
            }

            if (wParam is not PI.WM.KEYDOWN)
                return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);

            if (KeyCommands.ContainsKey(vkCode) && ctrl &&
                (isCellEditing || textBox_search.Focused))
            {
                PI.SendMessage(PI.GetFocus(), KeyCommands[vkCode], 0, 0);
                return (IntPtr)1;
            }

            switch (vkCode)
            {
                case (int)Keys.F when ctrl:
                    searchTSButton.PerformClick();
                    return (IntPtr)1;

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
                    ReleaseKeyboardHook();

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
            InstallKeyboardHook();
        }

        private void DevicesTree_MouseLeave(object sender, EventArgs e)
        {
            ReleaseKeyboardHook();
        }

        private void SearchInput_MouseEnter(object sender, EventArgs e)
        {
            InstallKeyboardHook();
        }

        private void SearchInput_MouseLeave(object sender, EventArgs e)
        {
            ReleaseKeyboardHook();
        }
    }
}
