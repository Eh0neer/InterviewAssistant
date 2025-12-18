using InterviewAssistant.Shared.Abstractions;
using System;
using System.Runtime.InteropServices;

namespace InterviewAssistant.Win32.Hotkeys
{
    internal sealed class Win32HotkeyService : IHotkeyService
    {
        public event Action? AltPressed;
        public event Action? AltReleased;
        public event Action? LeftMouseDown;

        private LowLevelKeyboardProc? _keyboardProc;
        private LowLevelMouseProc? _mouseProc;

        private IntPtr _keyboardHookId = IntPtr.Zero;
        private IntPtr _mouseHookId = IntPtr.Zero;

        private bool _altDown;

        #region Public API

        public void Start()
        {
            if (_keyboardHookId != IntPtr.Zero)
                return;

            _keyboardProc = KeyboardHookCallback;
            _mouseProc = MouseHookCallback;

            _keyboardHookId = SetWindowsHookEx(
                WH_KEYBOARD_LL,
                _keyboardProc,
                IntPtr.Zero,
                0);

            _mouseHookId = SetWindowsHookEx(
                WH_MOUSE_LL,
                _mouseProc,
                IntPtr.Zero,
                0);
        }

        public void Stop()
        {
            if (_keyboardHookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_keyboardHookId);
                _keyboardHookId = IntPtr.Zero;
            }

            if (_mouseHookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_mouseHookId);
                _mouseHookId = IntPtr.Zero;
            }
        }

        #endregion

        #region Keyboard Hook

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (vkCode == VK_MENU || vkCode == VK_LMENU || vkCode == VK_RMENU)
                {
                    if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)
                    {
                        if (!_altDown)
                        {
                            _altDown = true;
                            AltPressed?.Invoke();
                        }
                    }
                    else if (wParam == WM_KEYUP || wParam == WM_SYSKEYUP)
                    {
                        if (_altDown)
                        {
                            _altDown = false;
                            AltReleased?.Invoke();
                        }
                    }
                }
            }

            return CallNextHookEx(_keyboardHookId, nCode, wParam, lParam);
        }

        #endregion

        #region Mouse Hook

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == WM_LBUTTONDOWN)
            {
                if (_altDown)
                {
                    LeftMouseDown?.Invoke();
                }
            }

            return CallNextHookEx(_mouseHookId, nCode, wParam, lParam);
        }

        #endregion

        #region Win32

        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        private const int WM_LBUTTONDOWN = 0x0201;

        private const int VK_MENU = 0x12;
        private const int VK_LMENU = 0xA4;
        private const int VK_RMENU = 0xA5;

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode,
            IntPtr wParam,
            IntPtr lParam);

        private delegate IntPtr LowLevelMouseProc(
            int nCode,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(
            int idHook,
            LowLevelKeyboardProc lpfn,
            IntPtr hMod,
            uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(
            int idHook,
            LowLevelMouseProc lpfn,
            IntPtr hMod,
            uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(
            IntPtr hhk,
            int nCode,
            IntPtr wParam,
            IntPtr lParam);

        #endregion
    }
}
