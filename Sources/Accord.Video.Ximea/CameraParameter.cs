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
    /// Set of available configuration options for XIMEA cameras.
    /// </summary>
    /// 
    /// <remarks><para>The class defines list of parameters, which are available
    /// to set/get using corresponding methods of <see cref="XimeaCamera"/> and
    /// <see cref="XimeaVideoSource"/> classes.</para></remarks>
    /// 
    public static class CameraParameter
    {
        /// <summary>
        /// Get camera model name. Type string.
        /// </summary>
        public const string DeviceName = "device_name:info";

        /// <summary>
        /// Get device serial number in decimal format. Type string, integer, float
        /// </summary>
        public const string DeviceSerialNumber = "device_sn:info";

        /// <summary>
        /// Returns device type (1394, USB2.0, CURRERA…..). Type string.
        /// </summary>
        public const string DeviceType = "device_type:info";

        /// <summary>
        /// Set/Get exposure time in microseconds. Type integer.
        /// </summary>
        public const string Exposure = "exposure";

        /// <summary>
        /// Get longest possible exposure to be set on camera in microseconds. Type integer.
        /// </summary>
        public const string ExposureMax = "exposure:max";

        /// <summary>
        /// Get shortest possible exposure to be set on camera in microseconds. Type integer. 
        /// </summary>
        public const string ExposureMin = "exposure:min";

        /// <summary>
        /// Set/Get camera gain in dB. Type float. 
        /// </summary>
        public const string Gain = "gain";

        /// <summary>
        /// Get highest possible camera gain in dB. Type float.
        /// </summary>
        public const string GainMax = "gain:max";

        /// <summary>
        /// Get lowest possible camera gain in dB. Type float.
        /// </summary>
        public const string GainMin = "gain:min";

        /// <summary>
        /// Set/Get width of the image provided by the camera (in pixels). Type integer.
        /// </summary>
        public const string Width = "width";

        /// <summary>
        /// Get maximal image width provided by the camera (in pixels). Type integer.
        /// </summary>
        public const string WidthMax = "width:max";

        /// <summary>
        /// Get minimum image width provided by the camera (in pixels). Type integer.
        /// </summary>
        public const string WidthMin = "width:min";

        /// <summary>
        /// Set/Get height of the image provided by the camera (in pixels). Type integer.
        /// </summary>
        public const string Height = "height";

        /// <summary>
        /// Get maximal image height provided by the camera (in pixels). Type integer.
        /// </summary>
        public const string HeightMax = "height:max";

        /// <summary>
        /// Get minimum image height provided by the camera (in pixels). Type integer.
        /// </summary>
        public const string HeightMin = "height:min";

        /// <summary>
        /// Set/Get image resolution by binning or skipping. Type integer. 
        /// </summary>
        public const string Downsampling = "downsampling";

        /// <summary>
        /// Get highest value for binning or skipping. Type integer.
        /// </summary>
        public const string DownsamplingMax = "downsampling:max";

        /// <summary>
        /// Get lowest value for binning or skipping. Type integer.
        /// </summary>
        public const string DownsamplingMin = "downsampling:min";

        /// <summary>
        /// Get frames per second. Type float. 
        /// </summary>
        public const string Framerate = "framerate";

        /// <summary>
        /// Get highest possible framerate for current camera settings. Type float.
        /// </summary>
        public const string FramerateMax = "framerate:max";

        /// <summary>
        /// Get lowest framerate for current camera settings. Type float.
        /// </summary>
        public const string FramerateMin = "framerate:min";

        /// <summary>
        /// Set/Get horizontal offset from the origin to the area of interest (in pixels). Type integer. 
        /// </summary>
        public const string OffsetX = "offsetX";

        /// <summary>
        /// Get maximum horizontal offset from the origin to the area of interest (in pixels). Type integer. 
        /// </summary>
        public const string OffsetXMax = "offsetX:max";

        /// <summary>
        /// Get minimum horizontal offset from the origin to the area of interest (in pixels). Type integer. 
        /// </summary>
        public const string OffsetXMin = "offsetX:min";

        /// <summary>
        /// Set/Get vertical offset from the origin to the area of interest (in pixels). Type integer.
        /// </summary>
        public const string OffsetY = "offsetY";

        /// <summary>
        /// Get maximum vertical offset from the origin to the area of interest (in pixels). Type integer.
        /// </summary>
        public const string OffsetYMax = "offsetY:max";

        /// <summary>
        /// Get minimal vertical offset from the origin to the area of interest (in pixels). Type integer.
        /// </summary>
        public const string OffsetYMin = "offsetY:min";

        /// <summary>
        /// Set/Get white balance blue coefficient. Type float.
        /// </summary>
        public const string WhiteBalanceBlue = "wb_kb";

        /// <summary>
        /// Set/Get white balance red coefficient. Type float.
        /// </summary>
        public const string WhiteBalanceRed = "wb_kr";

        /// <summary>
        /// Set/Get white balance green coefficient. Type float.
        /// </summary>
        public const string WhiteBalanceGreen = "wb_kg";

        /// <summary>
        /// Set/Get sharpness strenght. Type float. 
        /// </summary>
        public const string Sharpness = "sharpness";

        /// <summary>
        /// Set/Get luminosity gamma value. Type float. By default 1.0.
        /// </summary>
        public const string GammaY = "gammaY";

        /// <summary>
        /// Set/Get chromaticity gamma value. Type float. By default 0.
        /// </summary>
        public const string GammaC = "gammaC";

        /// <summary>
        /// Set default color correction matrx. 
        /// </summary>
        public const string SetDefaultColorCorrectonMatrix = "defccMTX";

        /// <summary>
        /// Set/Get image format provided by the camera. Type integer. Use <see cref="ImageFormat"/>
        /// enumeraton for possible values.
        /// </summary>
        public const string ImageFormat = "imgdataformat";

        /// <summary>
        /// Set/Get camera's trigger mode. Type integer. Use <see cref="TriggerSource"/>
        /// enumeration for possible values.
        /// </summary>
        public const string Trigger = "trigger_source";

        /// <summary>
        /// Generates an internal trigger. <see cref="Trigger"/> must be set to <see cref="TriggerSource.Software"/>.
        /// Type integer.
        /// </summary>
        public const string SoftwareTrigger = "trigger_software";

        /// <summary>
        /// Calculates white balance. Takes white balance from image center (should be white/grey object
        /// in the center of scene). Type integer.
        /// </summary>
        public const string CalculateWhiteBalance = "manual_wb";

        /// <summary>
        /// Enable/disable automatic white balance. Type integer. By default 0.
        /// </summary>
        /// 
        /// <remarks><para>Set 0 to disable automatic white balance or 1 to enable.</para></remarks>
        /// 
        public const string AutoWhiteBalance = "auto_wb";

        /// <summary>
        /// Enable/disable bad pixels correction. Type integer. By default 0.
        /// </summary>
        /// 
        /// <remarks><para>Set 0 to disable bad pixels correction or 1 to enable.</para></remarks>
        /// 
        public const string BadPixelsCorrection = "bpc";

        /// <summary>
        /// Set/Get acquisition buffer size in bytes. Type integer. By default 53248000. 
        /// </summary>
        /// 
        /// <remarks><para>Defines acquisition buffer size in bytes. This buffer contains images'
        /// data from sensor. This parameter can be set only when acquisition is stopped.</para>
        /// 
        /// <para>See <see cref="BufferQueueSize"/> for additional information.</para>
        /// </remarks>
        /// 
        public const string AcquisitionBufferSize = "acq_buffer_size";

        /// <summary>
        /// Set/Get maximum number of images to store in queue. Type integer. By default 4.
        /// </summary>
        /// 
        /// <remarks><para><img src="img/video/HW_SW_buffers.png" width="500" height="317" />
        /// </para>
        /// 
        /// <para>See also <see cref="AcquisitionBufferSize"/> for additional information.</para>
        /// </remarks>
        /// 
        public const string BufferQueueSize = "buffers_queue_size";

        /// <summary>
        /// Set of configuration options to configure Automatic Exposure/Gain (AEAG) parameters.
        /// </summary>
        public static class AEAG
        {
            /// <summary>
            /// Enable/disable automatic exposure/gain control. Type integer. By default 0.
            /// </summary>
            /// 
            /// <remarks><para>Set 0 to disable automatic exposure/gain control or 1 to enable.</para></remarks>
            /// 
            public const string Enable = "aeag";

            /// <summary>
            /// Set/Get maximum limit of exposure in AEAG procedure. Type integer. By default 100. Units - ms.
            /// </summary>
            public const string ExposureMaxLimit = "ae_max_limit";

            /// <summary>
            /// Set/Get maximum limit of gain in AEAG procedure. Type float. Default depends on camera type. Units - dB.
            /// </summary>
            public const string GainMaxLimit = "ag_max_limit";

            /// <summary>
            /// Set/Get exposure priority, [0, 1]. Type float. By default 0.8.
            /// </summary>
            /// 
            /// <remarks><para>Setting the value to 0.5, for example, set exposure priority to 50%
            /// and gain priority to 50%.</para></remarks>
            /// 
            public const string ExposurePriority = "exp_priority";

            /// <summary>
            /// Set/Get average intensity of output signal AEAG should achieve (in %). Type float. By default 40.
            /// </summary>
            public const string Level = "aeag_level";
        }

        /// <summary>
        /// Set of configuration options to configure camera's LEDs. Currently supported only for Currera-R cameras.
        /// </summary>
        public static class LED
        {
            /// <summary>
            /// Selects camera LED to be used. Type integer.
            /// </summary>
            public const string Selector = "led_selector";

            /// <summary>
            /// Get highest LED number on camera. Type integer.
            /// </summary>
            public const string Max = "led_selector:max";

            /// <summary>
            /// Get lowest LED number on camera. Type integer.
            /// </summary>
            public const string Min = "led_selector:min";

            /// <summary>
            /// Set/Get LED functionality. Select LED by using <see cref="Selector"/> parameter.
            /// Use <see cref="LedMode"/> enumeration for possible parameter values. Type integer.
            /// </summary>
            public const string Mode = "led_mode";
        }

        /// <summary>
        /// Set of configuration options to configure GPO (General Purpose Output) ports.
        /// </summary>
        public static class GPO
        {
            /// <summary>
            /// Select camera GPO port. Type integer.
            /// </summary>
            public const string Selector = "gpo_selector";

            /// <summary>
            /// Get highest GPO port number on camera. Type integer.
            /// </summary>
            public const string Max = "gpo_selector:max";

            /// <summary>
            /// Get lowest GPO port number on camera. Type integer
            /// </summary>
            public const string Min = "gpo_selector:min";

            /// <summary>
            /// Set/Get GPO port functionality. Select port by using <see cref="Selector"/> parameter.
            /// Use <see cref="GpoMode"/> enumeration to set mode. Type integer.
            /// </summary>
            public const string Mode = "gpo_mode";
        }

        /// <summary>
        /// Set of configuration options to access/configure GPI (General Purpose Input) ports.
        /// </summary>
        public static class GPI
        {
            /// <summary>
            /// Select camera GPI port. Type integer.
            /// </summary>
            public const string Selector = "gpi_selector";

            /// <summary>
            /// Get highest GPI port number on camera. Type integer.
            /// </summary>
            public const string Max = "gpi_selector:max";

            /// <summary>
            /// Get lowest GPI port number on camera. Type integer
            /// </summary>
            public const string Min = "gpi_selector:min";

            /// <summary>
            /// Set/Get GPI port functionality. Select port by using <see cref="Selector"/> parameter.
            /// Use <see cref="GpiMode"/> enumeration to set mode. Type integer.
            /// </summary>
            public const string Mode = "gpi_mode";

            /// <summary>
            /// Get current GPI level. Type integer.
            /// </summary>
            public const string Level = "gpi_level";
        }

        /// <summary>
        /// Set of configuration options to configure camera's LUT - Look-Up-Table.
        /// Currently available only for Currera-R cameras.
        /// </summary>
        public static class LUT
        {
            /// <summary>
            /// Enable/Disable LUT. Type integer. Default 0.
            /// </summary>
            /// 
            /// <remarks><para>Set 0 to disable LUT - sensor pixels are transferred directly.
            /// Set 1 to enable LUT - sensor pixels are mapped through LUT.</para></remarks>
            /// 
            public static string Enable = "LUTEnable";

            /// <summary>
            /// Set/Get the index (offset) of the coefficient to access in the LUT. Type integer.
            /// </summary>
            public static string Index = "LUTIndex";

            /// <summary>
            /// Get lowest LUT index (offset) of the coefficient to access in the LUT. Type integer.
            /// </summary>
            public static string IndexMin = "LUTIndex:min";

            /// <summary>
            /// Get highest LUT index (offset) of the coefficient to access in the LUT. Type integer.
            /// </summary>
            public static string IndexMax = "LUTIndex:max";

            /// <summary>
            /// Set/Get value in the LUT. Index of the value must be selected using <see cref="Index"/>
            /// parameter. Type integer. 
            /// </summary>
            public static string Value = "LUTValue";

            /// <summary>
            /// Get highest value to be set in LUT. Type integer. 
            /// </summary>
            public static string ValueMin = "LUTValue:min";

            /// <summary>
            /// Get lowest value to be set in LUT. Type integer. 
            /// </summary>
            public static string ValueMax = "LUTValue:max";
        }

        /// <summary>
        /// Set of configuration options to access elements of Color Correction Matrix.
        /// </summary>
        /// 
        public static class ColorCorrectionMatrix
        {
            /// <summary>
            /// Set/Get color correction matrix element [0][0]. Type float. By default 1.0.
            /// </summary>
            public const string V00 = "ccMTX00";

            /// <summary>
            /// Set/Get color correction matrix element [0][1]. Type float. By default 0.0.
            /// </summary>
            public const string V01 = "ccMTX01";

            /// <summary>
            /// Set/Get color correction matrix element [0][2]. Type float. By default 0.0.
            /// </summary>
            public const string V02 = "ccMTX02";

            /// <summary>
            /// Set/Get color correction matrix element [0][3]. Type float. By default 0.0.
            /// </summary>
            public const string V03 = "ccMTX03";

            /// <summary>
            /// Set/Get color correction matrix element [1][0]. Type float. By default 0.0.
            /// </summary>
            public const string V10 = "ccMTX10";

            /// <summary>
            /// Set/Get color correction matrix element [1][1]. Type float. By default 1.0.
            /// </summary>
            public const string V11 = "ccMTX11";

            /// <summary>
            /// Set/Get color correction matrix element [1][2]. Type float. By default 0.0.
            /// </summary>
            public const string V12 = "ccMTX12";

            /// <summary>
            /// Set/Get color correction matrix element [1][3]. Type float. By default 0.0.
            /// </summary>
            public const string V13 = "ccMTX13";

            /// <summary>
            /// Set/Get color correction matrix element [2][0]. Type float. By default 0.0.
            /// </summary>
            public const string V20 = "ccMTX20";

            /// <summary>
            /// Set/Get color correction matrix element [2][1]. Type float. By default 0.0.
            /// </summary>
            public const string V21 = "ccMTX21";

            /// <summary>
            /// Set/Get color correction matrix element [2][2]. Type float. By default 1.0.
            /// </summary>
            public const string V22 = "ccMTX22";

            /// <summary>
            /// Set/Get color correction matrix element [2][3]. Type float. By default 0.0.
            /// </summary>
            public const string V23 = "ccMTX23";

            /// <summary>
            /// Set/Get color correction matrix element [3][0]. Type float. By default 0.0.
            /// </summary>
            public const string V30 = "ccMTX30";

            /// <summary>
            /// Set/Get color correction matrix element [3][1]. Type float. By default 0.0.
            /// </summary>
            public const string V31 = "ccMTX31";

            /// <summary>
            /// Set/Get color correction matrix element [3][2]. Type float. By default 0.0.
            /// </summary>
            public const string V32 = "ccMTX32";

            /// <summary>
            /// Set/Get color correction matrix element [3][3]. Type float. By default 1.0.
            /// </summary>
            public const string V33 = "ccMTX33";
        }
    }
}
