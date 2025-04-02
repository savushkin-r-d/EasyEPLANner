using EasyEPlanner;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IO.View
{
    public partial class IOViewControl : Form
    {
        private bool isLoaded = false;

        static string caption = "ПЛК\0";

        static byte[] newCapt = EncodingDetector.Windows1251.GetBytes(caption);

        private IntPtr dialogHookPtr = IntPtr.Zero;

        private PI.HookProc dialogCallbackDelegate = null;

        private IntPtr dialogHandle = IntPtr.Zero;

        private IntPtr panelPtr = IntPtr.Zero;

        public static IntPtr wndDevVisibilePtr;

        /// <summary>
        /// Показать окно в формой
        /// </summary>
        public void ShowDlg()
        {
            System.Diagnostics.Process oCurrent = System.Diagnostics.Process.GetCurrentProcess();

            // Идентификатор команды вызова окна "ПЛК"
            const int wndWmCommand = 35084;
            string windowName = "ПЛК";

            if (isLoaded == true)
            {
                StaticHelper.GUIHelper.ShowHiddenWindow(oCurrent,
                    wndDevVisibilePtr, wndWmCommand);
                return;
            }

            StaticHelper.GUIHelper.SearchWindowDescriptor(oCurrent, windowName,
                wndWmCommand, ref dialogHandle, ref wndDevVisibilePtr);
            if (wndDevVisibilePtr != IntPtr.Zero)
            {
                StaticHelper.GUIHelper.ShowHiddenWindow(oCurrent,
                    wndDevVisibilePtr, wndWmCommand);

                if (isLoaded == false)
                {
                    StaticHelper.GUIHelper.ChangeWindowMainPanels(
                        ref dialogHandle, ref panelPtr);

                    Controls.Clear();

                    // Переносим на найденное окно свои элементы (SetParent) и
                    // подгоняем их размеры и позицию.
                    PI.SetParent(MainTableLayoutPanel.Handle, dialogHandle);
                    ChangeUISize();

                    // Устанавливаем свой хук для найденного окна
                    // (для изменения размеров своих элементов, сохранения
                    // изменений при закрытии и отключения хука).
                    SetUpHook();

                    //deviceIsShown = true;
                    isLoaded = true;
                }
            }
            ChangeUISize();
        }

        private void SetUpHook()
        {
            dialogCallbackDelegate = new PI.HookProc(DlgWndHookCallbackFunction);

            uint pid = PI.GetWindowThreadProcessId(dialogHandle, IntPtr.Zero);
            dialogHookPtr = PI.SetWindowsHookEx(PI.HookType.WH_CALLWNDPROC,
                dialogCallbackDelegate, IntPtr.Zero, pid);
        }

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

            if (msg.hwnd == dialogHandle)
            {
                switch (msg.message)
                {
                    case (int)PI.WM.GETTEXTLENGTH:
                        return (IntPtr)newCapt.Length;

                    case (int)PI.WM.SETTEXT:
                        return IntPtr.Zero;

                    case (int)PI.WM.WINDOWPOSCHANGED:
                        PI.WINDOWPOS p = new PI.WINDOWPOS();
                        p = (PI.WINDOWPOS)
                            System.Runtime.InteropServices.Marshal
                            .PtrToStructure(lParam, typeof(PI.WINDOWPOS));

                        break;

                    case (int)PI.WM.DESTROY:
                        PI.UnhookWindowsHookEx(dialogHookPtr);
                        dialogHookPtr = IntPtr.Zero;
                        dialogHandle = IntPtr.Zero;

                        PI.SetParent(MainTableLayoutPanel.Handle, this.Handle);
                        //PI.SetParent(toolStrip.Handle, this.Handle);
                        this.Controls.Add(MainTableLayoutPanel);
                        //this.Controls.Add(toolStrip);
                        MainTableLayoutPanel.Hide();
                        System.Threading.Thread.Sleep(1);
                        //deviceIsShown = false;
                        isLoaded = false;
                        break;

                    case (int)PI.WM.GETTEXT:
                        System.Runtime.InteropServices.Marshal.Copy(
                            newCapt, 0, lParam, newCapt.Length);
                        return (IntPtr)newCapt.Length;
                }
            }

            return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }


        /// <summary>
        /// Изменить UI размер
        /// </summary>
        private void ChangeUISize()
        {
            IntPtr dialogPtr = PI.GetParent(MainTableLayoutPanel.Handle);

            PI.RECT rctDialog;
            PI.GetWindowRect(dialogPtr, out rctDialog);

            int w = rctDialog.Right - rctDialog.Left;
            int h = rctDialog.Bottom - rctDialog.Top;

            //toolStrip.Location = new Point(0, 0);
            MainTableLayoutPanel.Location = new Point(0, 0);

            //toolStrip.Width = w;
            MainTableLayoutPanel.Width = w;
            MainTableLayoutPanel.Height = h;
            //MainTableLayoutPanel.Columns.First().Width = w;
        }


    }
}
