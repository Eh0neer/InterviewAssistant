using System.Runtime.InteropServices;
using InterviewAssistant.Shared.Abstractions;

namespace InterviewAssistant.Win32.Overlay
{
    internal class Win32OverlayService : IOverlayService
    {
        private IntPtr _hwnd;
        public void Initialize(IntPtr windowHandle)
        {
            _hwnd = windowHandle;
            ApplyOverlayStyles(_hwnd);
        }
        public void Hide()
        {
            ShowWindow(_hwnd, SW_HIDE);
        }
        public void Show()
        {
            ShowWindow(_hwnd, SW_SHOW);
        }

        public void EnableInteraction()
        {
            RemoveTransparentFlag(_hwnd);
        }

        public void DisableInteraction()
        {
            AddTransparentFlag(_hwnd);
        }

        public void BeginDrag()
        {
            WindowDragHelper.BeginDrag(_hwnd);
        }

        public void TryBeginDrag()
        {
            if (IsLeftMouseButtonDown())
            {
                WindowDragHelper.BeginDrag(_hwnd);
            }
        }


        private static void ApplyOverlayStyles(IntPtr hwnd)
        {
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            exStyle |=
                WS_EX_LAYERED |
                WS_EX_TRANSPARENT |
                WS_EX_TOOLWINDOW;

            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);

            SetWindowPos(
                hwnd,
                HWND_TOPMOST,
                0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        private static void AddTransparentFlag(IntPtr hwnd)
        {
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT);
        }

        private static void RemoveTransparentFlag(IntPtr hwnd)
        {
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle & ~WS_EX_TRANSPARENT);
        }

        #region Win32

        private const int GWL_EXSTYLE = -20;

        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_TOOLWINDOW = 0x80;

        private static readonly IntPtr HWND_TOPMOST = new(-1);

        private const int SW_SHOW = 5;
        private const int SW_HIDE = 0;

        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private const int VK_LBUTTON = 0x01;

        private static bool IsLeftMouseButtonDown()
        {
            return (GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0;
        }

        #endregion
    }
}
