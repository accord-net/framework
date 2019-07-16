// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2013
// contacts@aforgenet.com
//

namespace Accord.Video.DirectShow
{
    using System;


    /// <summary>
    /// The enumeration specifies a setting on a video stream.
    /// </summary>
    public enum VideoProcAmpProperty
    {
        /// <summary>
        /// Brightness control.
        /// </summary>
        Brightness = 0,
        /// <summary>
        /// Contrast control.
        /// </summary>
        Contrast,
        /// <summary>
        /// Hue control.
        /// </summary>
        Hue,
        /// <summary>
        /// Saturation control.
        /// </summary>
        Saturation,
        /// <summary>
        /// Sharpness control.
        /// </summary>
        Sharpness,
        /// <summary>
        /// Gamma control.
        /// </summary>
        Gamma,
        /// <summary>
        /// ColorEnable control.
        /// </summary>
        ColorEnable,
        /// <summary>
        /// WhiteBalance control.
        /// </summary>
        WhiteBalance,
        /// <summary>
        /// BacklightCompensation control.
        /// </summary>
        BacklightCompensation,
        /// <summary>
        /// Gain control.
        /// </summary>
        Gain,
        /// <summary>
        /// amount of digital zoom to apply control.
        /// </summary>
        DigitalMultiplier,
        /// <summary>
        /// upper limit for the amount of digital zoom that can be applied control.
        /// </summary>
        DigitalMultiplierLimit,
        /// <summary>
        /// white balance setting in blue and red values control.
        /// </summary>
        WhiteBalanceComponent,
        /// <summary>
        /// local power line frequency control.
        /// </summary>
        PowerlineFrequency
    }

    /// <summary>
    /// The enumeration defines whether a setting is controlled manually or automatically.
    /// </summary>
    [Flags]
    public enum VideoProcAmpFlags
    {
        /// <summary>
        /// No control flag.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Auto control Flag.
        /// </summary>
        Auto = 0x0001,
        /// <summary>
        /// Manual control Flag.
        /// </summary>
        Manual = 0x0002
    }
}
