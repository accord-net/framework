// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2013
// contacts@aforgenet.com
//

namespace Accord.Video.DirectShow.Internals
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The IAMVideoProcAmp interface controls video camera settings such as brightness, contrast, hue,
    /// or saturation. To obtain this interface, cast the MediaSource.
    /// </summary>
    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("C6E13360-30AC-11D0-A18C-00A0C9118956"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMVideoProcAmp
    {
        /// <summary>
        /// Get the range and default value of a camera property.
        /// </summary>
        ///
        /// <param name="Property">The property.</param>
        /// <param name="pMin">The min value.</param>
        /// <param name="pMax">The max value.</param>
        /// <param name="pSteppingDelta">The step size.</param>
        /// <param name="pDefault">The deafult value. </param>
        /// <param name="pCapsFlags">Shows if it can be controlled automatically and/or manually.</param>
        ///
        /// <returns>Error code.</returns>
        ///
        [PreserveSig]
        int GetRange(
            [In] VideoProcAmpProperty Property,
            [Out] out int pMin,
            [Out] out int pMax,
            [Out] out int pSteppingDelta,
            [Out] out int pDefault,
            [Out] out VideoProcAmpFlags pCapsFlags
            );

        /// <summary>
        /// Set a specified property on the camera.
        /// </summary>
        ///
        /// <param name="Property">The property to set.</param>
        /// <param name="lValue">The new value of the property.</param>
        /// <param name="Flags">The auto or manual setting.</param>
        ///
        /// <returns>Error code.</returns>
        ///
        [PreserveSig]
        int Set(
            [In] VideoProcAmpProperty Property,
            [In] int lValue,
            [In] VideoProcAmpFlags Flags
            );

        /// <summary>
        /// Get the current setting of a camera property.
        /// </summary>
        ///
        /// <param name="Property">The property to retrieve.</param>
        /// <param name="lValue">The current value of the property.</param>
        /// <param name="Flags">Is it manual or automatic?</param>
        ///
        /// <returns>Error code.</returns>
        ///
        [PreserveSig]
        int Get(
            [In] VideoProcAmpProperty Property,
            [Out] out int lValue,
            [Out] out VideoProcAmpFlags Flags
            );
    }
}
