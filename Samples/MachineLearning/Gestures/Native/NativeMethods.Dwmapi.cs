

namespace Gestures.Native
{
    using System;
    using System.CodeDom.Compiler;
    using System.Runtime.InteropServices;

    /// <summary>
    ///   Native Win32 methods.
    /// </summary>
    /// 
    internal static partial class NativeMethods
    {

        [DllImport("dwmapi.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref ThemeMargins margins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DwmIsCompositionEnabled();

        [DllImport("gdi32")]
        internal static extern System.IntPtr CreateEllipticRgn(
          int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        public struct DWM_BLURBEHIND
        {
            public int dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;
        }

        [System.Runtime.InteropServices.DllImport("dwmapi")]
        internal static extern int DwmEnableBlurBehindWindow(IntPtr hWnd, ref DWM_BLURBEHIND pBlurBehind);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
    }

    /// <summary>
    ///   MARGINS structure (Windows).
    /// </summary>
    /// 
    /// <remarks>
    ///   Returned by the GetThemeMargins function to define the 
    ///   margins of windows that have visual styles applied.
    ///   
    ///   http://msdn.microsoft.com/en-us/library/windows/desktop/bb773244(v=vs.85).aspx
    /// </remarks>
    /// 
    [StructLayout(LayoutKind.Sequential)]
    [GeneratedCode("PInvoke", "1.0.0.0")]
    public struct ThemeMargins
    {
        /// <summary>
        ///   Width of the left border that retains its size.
        /// </summary>
        /// 
        public int LeftWidth;

        /// <summary>
        ///   Width of the right border that retains its size.
        /// </summary>
        /// 
        public int RightWidth;

        /// <summary>
        ///   Height of the top border that retains its size.
        /// </summary>
        /// 
        public int TopHeight;

        /// <summary>
        ///   Height of the bottom border that retains its size.
        /// </summary>
        /// 
        public int BottomHeight;
    }
}
