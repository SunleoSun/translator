using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

namespace Volumer
{
    delegate int HookProc(int nCode, int wParam, IntPtr lParam);
    public delegate void KeyHooksEventHandler(object sender, KeyHooksEventArgs e);
    public class KeyHooksEventArgs
    {
        public bool control;
    }

    public class HookKeys
    {

        #region DllImports
        [DllImport("user32.dll", CharSet = CharSet.Auto,
CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern int SetWindowsHookEx(
            int idHook,
            HookProc lpfn,
            IntPtr hMod,
            int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern int UnhookWindowsHookEx(int idHook);


        [DllImport("user32.dll", CharSet = CharSet.Auto,
             CallingConvention = CallingConvention.StdCall)]
        static extern int CallNextHookEx(
           int idHook,
           int nCode,
           int wParam,
           IntPtr lParam);

        [DllImport("user32")]
        static extern int ToAscii(
            int uVirtKey,
            int uScanCode,
            byte[] lpbKeyState,
            byte[] lpwTransKey,
            int fuState);

        [DllImport("user32")]
        static extern int GetKeyboardState(byte[] pbKeyState);
        [DllImport("user32")]
        static extern int GetAsyncKeyState(int pbKeyState);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern short GetKeyState(int vKey);

        #endregion

        const int WH_KEYBOARD_LL = 13;
        const int VK_LCONTROL = 0xA2;
        const int VK_RCONTROL = 0xA3;
        const int VK_LMENU = 0xA4;
        const int VK_LEFT = 0x25;
        const int VK_RIGHT = 0x27;
        const int VK_LALT = 0xA4;
        const int VK_RALT = 0xA5;
        const int VK_T = 0x54;

        HookProc _hookCallback;
        private bool _isStarted;
        int _handleToHook;

        public void Start()
        {
            _hookCallback = new HookProc(HookCallbackProcedure);
            _handleToHook = SetWindowsHookEx(
                13,
                _hookCallback,
               Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                0);
            int i = Marshal.GetLastWin32Error();
            if (_handleToHook != 0)
            {
                _isStarted = true;
            }
        }
        public void Stop()
        {
            if (_isStarted)
            {
                UnhookWindowsHookEx(_handleToHook);
                _isStarted = false;
            }
        }
        public event KeyHooksEventHandler KeyHooked;

        void OnKeyHooks(KeyHooksEventArgs e)
        {
            KeyHooksEventHandler del = KeyHooked;
            if (del != null)
                del(this, e);
        }
        public int HookCallbackProcedure(int nCode, Int32 wParam, IntPtr lParam)
        {
            bool control = ((GetKeyState(VK_LCONTROL) & 0x80) != 0) &&
                       ((GetKeyState(VK_RCONTROL) & 0x80) != 0);
            KeyHooksEventArgs arg = new KeyHooksEventArgs();
            arg.control = control;
            OnKeyHooks(arg);
            return CallNextHookEx(_handleToHook, nCode, wParam, lParam);

        }

    }
}
