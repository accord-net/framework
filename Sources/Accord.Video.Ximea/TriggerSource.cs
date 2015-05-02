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
    /// Enumeration of camera's trigger modes.
    /// </summary>
    public enum TriggerSource : int
    {
        /// <summary>
        /// Camera works in free run mode.
        /// </summary>
        Off = 0,

        /// <summary>
        /// External trigger (rising edge).
        /// </summary>
        EdgeRising = 1,

        /// <summary>
        /// External trigger (falling edge).
        /// </summary>
        EdgeFalling = 2,

        /// <summary>
        /// Software (manual) trigger.
        /// </summary>
        Software = 3
    }
}
