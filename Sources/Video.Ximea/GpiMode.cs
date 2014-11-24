// AForge XIMEA Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Video.Ximea
{
    using System;

    /// <summary>
    /// XIMEA camera's GPI port modes.
    /// </summary>
    public enum GpiMode : int
    {
        /// <summary>
        /// Input is off.
        /// </summary>
        Off = 0,

        /// <summary>
        /// Trigger input.
        /// </summary>
        Trigger = 1,

        /// <summary>
        /// External signal input.
        /// </summary>
        ExternalEvent = 2
    }
}
