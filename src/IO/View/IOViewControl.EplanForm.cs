using EasyEPlanner;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IO.View
{
    // Функционал окна отвечающий за его отображение в окне EPLAN
    [ExcludeFromCodeCoverage]
    public partial class IOViewControl : Form
    {
        private bool isLoaded = false;

        static string caption = "ПЛК\0";

        static byte[] newCapt = EncodingDetector.Windows1251.GetBytes(caption);

        private IntPtr dialogHookPtr = IntPtr.Zero;

        private PI.HookProc dialogCallbackDelegate = null;

        private IntPtr dialogHandle = IntPtr.Zero;

        private IntPtr panelPtr = IntPtr.Zero;

        private static IntPtr wndDevVisibilePtr;

        private PI.LowLevelKeyboardProc mainWndKeyboardCallbackDelegate = null;

        private IntPtr globalKeyboardHookPtr = IntPtr.Zero;

        /// <summary>
        /// Показать окно в формой
        /// </summary>
        public void ShowDlg()
        {
            System.Diagnostics.Process oCurrent = System.Diagnostics.Process.GetCurrentProcess();

            // Идентификатор команды вызова окна "ПЛК"
            const int wndWmCommand = 35084;
            string windowName = "ПЛК";

            if (isLoaded)
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

                isLoaded = true;
            }
            ChangeUISize();
        }

        private void SetUpHook()
        {
            dialogCallbackDelegate = new PI.HookProc(DlgWndHookCallbackFunction);

            mainWndKeyboardCallbackDelegate = new PI.LowLevelKeyboardProc(GlobalHookKeyboardCallbackFunction);

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
            const short SHIFTED = 0x80;
            bool Ctrl = (PI.GetKeyState((int)PI.VIRTUAL_KEY.VK_CONTROL) & SHIFTED) > 0;
            uint vkCode = lParam.vkCode;

            // Перехватываем комбинации Ctrl + PgDn/PgUp для всех окон,
            // так как они ломают отрисовку
            // (переключаются вкладки дерево-список на оригинальном окне)
            if (wParam == PI.WM.KEYDOWN && Ctrl &&
                (vkCode is PI.VIRTUAL_KEY.VK_PRIOR or PI.VIRTUAL_KEY.VK_NEXT))
            {
                return (IntPtr)1;
            }


            if (code < 0 || StructPLC is null || !(StructPLC.Focused || isCellEditing))
                return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);

            //Отпускание клавиш - если активно окно редактора, то не пускаем дальше.
            if (wParam is PI.WM.KEYUP or PI.WM.CHAR &&
                vkCode is PI.VIRTUAL_KEY.VK_DELETE &&
                (StructPLC.Focused || isCellEditing))
            {
                return (IntPtr)1;
            }

            //Нажатие клавиш - если активно окно редактора, то обрабатываем и
            //не пускаем дальше.
            if (wParam == PI.WM.KEYDOWN)
            {
                if (KeyCommands.ContainsKey(vkCode) && Ctrl && isCellEditing)
                {
                    // Если активен текстовый редактор
                    // - команды работы с текстом
                    PI.SendMessage(PI.GetFocus(), KeyCommands[vkCode], 0, 0);
                    return (IntPtr)1;
                }

                switch (vkCode)
                {
                    // Перехватываем используемые
                    // комбинации клавиш:
                    case PI.VIRTUAL_KEY.VK_ESCAPE:  // Esc
                    case PI.VIRTUAL_KEY.VK_RETURN:  // Enter
                    case PI.VIRTUAL_KEY.VK_DELETE:  // Delete

                    case PI.VIRTUAL_KEY.VK_UP:      // Up
                    case PI.VIRTUAL_KEY.VK_DOWN:    // Down
                    case PI.VIRTUAL_KEY.VK_LEFT:    // Left
                    case PI.VIRTUAL_KEY.VK_RIGHT:   // Right
                        PI.SendMessage(PI.GetFocus(), (int)PI.WM.KEYDOWN, (int)vkCode, 0);
                        return (IntPtr)1;
                }
            }

            return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        private static readonly Dictionary<uint, uint> KeyCommands
            = new Dictionary<uint, uint>
            {
                [(uint)Keys.X] = (int)PI.WM.CUT,    // Вырезать
                [(uint)Keys.C] = (int)PI.WM.COPY,   // Копировать
                [(uint)Keys.V] = (int)PI.WM.PASTE,  // Вставить
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

            if (msg.hwnd == dialogHandle)
            {
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

                        PI.SetParent(MainTableLayoutPanel.Handle, this.Handle);
                        this.Controls.Add(MainTableLayoutPanel);
                        MainTableLayoutPanel.Hide();
                        System.Threading.Thread.Sleep(1);
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

            PI.GetWindowRect(dialogPtr, out PI.RECT rctDialog);

            MainTableLayoutPanel.Location = new Point(0, 0);

            MainTableLayoutPanel.Width = rctDialog.Right - rctDialog.Left;
            MainTableLayoutPanel.Height = rctDialog.Bottom - rctDialog.Top;
        }


        private void StructPLC_MouseEnter(object sender, EventArgs e)
        {
            globalKeyboardHookPtr = PI.SetWindowsHookEx(PI.HookType.WH_KEYBOARD_LL,
                mainWndKeyboardCallbackDelegate, IntPtr.Zero, 0);
        }

        private void StructPLC_MouseLeave(object sender, EventArgs e)
        {
            if (globalKeyboardHookPtr != IntPtr.Zero)
            {
                PI.UnhookWindowsHookEx(globalKeyboardHookPtr);
                globalKeyboardHookPtr = IntPtr.Zero;
            }
        }
    }
}
