// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © 2009-2014, César Souza
// All rights reserved. 3-BSD License:
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions are met:
//
//      * Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//
//      * Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//
//      * Neither the name of the Accord.NET Framework authors nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
// 
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 

namespace Gestures.Native
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using System.Text;

    /// <summary>
    ///   Managed wrapper around native methods.
    /// </summary>
    /// 
    public static class SafeNativeMethods
    {

        /// <summary>
        ///   Extends the window frame into the client area.
        /// </summary>
        /// 
        /// <param name="window">
        ///   The handle to the window in which the frame will
        ///   be extended into the client area.</param>
        /// <param name="margins">
        ///   A pointer to a <see cref="ThemeMargins"/> structure that describes
        ///   the margins to use when extending the frame into the client area.</param>
        ///   
        public static void ExtendAeroGlassIntoClientArea(IWin32Window window, ThemeMargins margins)
        {
            if (window == null) throw new ArgumentNullException("window");
            NativeMethods.DwmExtendFrameIntoClientArea(window.Handle, ref margins);
        }

        /// <summary>
        ///   Obtains a value that indicates whether Desktop
        ///   Window Manager (DWM) composition is enabled. 
        /// </summary>
        /// 
        public static bool IsAeroEnabled
        {
            get { return NativeMethods.DwmIsCompositionEnabled(); }
        }

        public static void EnabledBlurBehindWindow(IntPtr handle, Rectangle rectangle)
        {
            IntPtr hr = NativeMethods.CreateRectRgn(rectangle.Left,
                rectangle.Top, rectangle.Right, rectangle.Bottom);

            NativeMethods.DWM_BLURBEHIND dbb;
            dbb.fEnable = true;
            dbb.dwFlags = 1 | 2;
            dbb.hRgnBlur = hr;
            dbb.fTransitionOnMaximized = false;
            NativeMethods.DwmEnableBlurBehindWindow(handle, ref dbb);
        }
    }
}
