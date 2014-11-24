// AForge Lego Robotics Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Robotics.Lego.Internals
{
    using System;

    /// <summary>
    /// Enumeration of command types supported by Lego Mindstorms NXT brick.
    /// </summary>
    /// 
    internal enum NXTCommandType
    {
        /// <summary>
        /// Direct command, which requires reply.
        /// </summary>
        DirectCommand = 0x00,

        /// <summary>
        /// System command, which requires reply.
        /// </summary>
        SystemCommand = 0x01,

        /// <summary>
        /// Reply command received from NXT brick.
        /// </summary>
        ReplyCommand = 0x02,

        /// <summary>
        /// Direct command, which does not require reply.
        /// </summary>
        DirectCommandWithoutReply = 0x80,

        /// <summary>
        /// System command, which does not require reply.
        /// </summary>
        SystemCommandWithoutReply = 0x81
    }

    /// <summary>
    /// Enumeration of system commands supported by Lego Mindstorms NXT brick.
    /// </summary>
    /// 
    internal enum NXTSystemCommand
    {
        /// <summary>
        /// Get firmware version of NXT brick.
        /// </summary>
        GetFirmwareVersion = 0x88,

        /// <summary>
        /// Set NXT brick name.
        /// </summary>
        SetBrickName = 0x98,

        /// <summary>
        /// Get device information.
        /// </summary>
        GetDeviceInfo = 0x9B
    }

    /// <summary>
    /// Enumeration of direct commands supported by Lego Mindstorms NXT brick.
    /// </summary>
    /// 
    internal enum NXTDirectCommand
    {
        /// <summary>
        /// Keep NXT brick alive.
        /// </summary>
        KeepAlive = 0x0D,

        /// <summary>
        /// Play tone of specified frequency.
        /// </summary>
        PlayTone = 0x03,

        /// <summary>
        /// Get battery level.
        /// </summary>
        GetBatteryLevel = 0x0B,

        /// <summary>
        /// Set output state.
        /// </summary>
        SetOutputState = 0x04,

        /// <summary>
        /// Get output state.
        /// </summary>
        GetOutputState = 0x06,

        /// <summary>
        /// Reset motor position.
        /// </summary>
        ResetMotorPosition = 0x0A,

        /// <summary>
        /// Set input mode.
        /// </summary>
        SetInputMode = 0x05,

        /// <summary>
        /// Get input values.
        /// </summary>
        GetInputValues = 0x07,

        /// <summary>
        /// Get status of the Low Speed bus.
        /// </summary>
        LsGetStatus = 0x0E,

        /// <summary>
        /// Write to the Low Speed bus.
        /// </summary>
        LsWrite = 0x0F,

        /// <summary>
        /// Read from the Low Speed bus.
        /// </summary>
        LsRead = 0x10,

        /// <summary>
        /// Reset input scaled value.
        /// </summary>
        ResetInputScaledValue = 0x08
    }
}
