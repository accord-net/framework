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
    /// XIMEA camera's GPO port modes.
    /// </summary>
    public enum GpoMode : int
    {
        /// <summary>
        /// Output off.
        /// </summary>
        Off = 0,

        /// <summary>
        /// Logical level.
        /// </summary>
        On = 1,

        /// <summary>
        /// High during exposure (integration) time + readout time + data transfer time.
        /// </summary>
        FrameActive = 2,

        /// <summary>
        /// Low during exposure (integration) time + readout time + data trasnfer time. 
        /// </summary>
        FrameActiveNew = 3,

        /// <summary>
        /// High during exposure(integration) time.
        /// </summary>
        ExposureActive = 4,

        /// <summary>
        /// Low during exposure(integration) time.
        /// </summary>
        ExposureActiveNeg = 5
    }
}
