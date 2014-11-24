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
    /// Enumeration of commands supported by Lego Mindstorms RXT brick.
    /// </summary>
    internal enum RCXCommand
    {
        /// <summary>
        /// Check whether or not the RCX is alive.
        /// </summary>
        IsAlive = 0x10,

        /// <summary>
        /// Play one of defined sounds.
        /// </summary>
        PlaySound = 0x51,

        /// <summary>
        /// Play tone of specified frequency.
        /// </summary>
        PlayTone = 0x23,

        /// <summary>
        /// Get ROM and firmware versions.
        /// </summary>
        GetVersions = 0x15,

        /// <summary>
        /// Get battery power.
        /// </summary>
        GetBatteryPower = 0x30,

        /// <summary>
        /// Set time displayed on RCX brick.
        /// </summary>
        SetTime = 0x22,

        /// <summary>
        /// Turm off RCX brick.
        /// </summary>
        PowerOff = 0x60,

        /// <summary>
        /// Get value.
        /// </summary>
        GetValue = 0x12,

        /// <summary>
        /// Set sensor type.
        /// </summary>
        SetSensorType = 0x32,

        /// <summary>
        /// Set sensor mode.
        /// </summary>
        SetSensorMore = 0x42,

        /// <summary>
        /// Clear sensor value.
        /// </summary>
        ClearSensorValue = 0xD1,

        /// <summary>
        /// Set IR transmiter's range.
        /// </summary>
        SetTransmitterRange = 0x31,

        /// <summary>
        /// Turn on/off motor.
        /// </summary>
        SetMotorOnOff = 0x21,

        /// <summary>
        /// Set motor's power.
        /// </summary>
        SetMotorPower = 0x13,

        /// <summary>
        /// Set motor's direction
        /// </summary>
        SetMotorDirection = 0xE1,
    }
}
