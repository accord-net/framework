// AForge TeRK Robotics Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2009
// andrew.kirillov@aforgenet.com
//

using TeRKIceLib = TeRK;

namespace AForge.Robotics.TeRK
{
    using System;
    using AForge;

    public partial class Qwerk
    {
        /// <summary>
        /// Provides access to Qwerk's motors' controllers.
        /// </summary>
        /// 
        /// <remarks><para>The class allows to manipulate Qwerk's motors. The total number
        /// of available motors equals to <see cref="Motors.Count"/>.</para>
        /// 
        /// <para><note>The class is deprecated.</note></para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // get Qwerk's motors service
        /// Qwerk.Motors motors = qwerk.GetMotorsService( );
        /// // stop all motors
        /// motors.StopMotors( );
        /// // run first motor
        /// motors.SetMotorVelocity( 0, 10000 );
        /// </code>
        /// </remarks>
        /// 
        [Obsolete( "The class is deprecated." )]
        public class Motors
        {
            // Qwerk's motor controller
            private TeRKIceLib.MotorControllerPrx motorController = null;

            /// <summary>
            /// Number of available motors' controllers, 4.
            /// </summary>
            public const int Count = 4;

            // default motors' acceleration
            private const int DefaultAcceleration = 16000;

            /// <summary>
            /// Initializes a new instance of the <see cref="Qwerk.Motors"/> class.
            /// </summary>
            /// 
            /// <param name="qwerk">Reference to <see cref="Qwerk"/> object, which is connected to Qwerk board.</param>
            /// 
            /// <exception cref="NotConnectedException">The passed reference to <see cref="Qwerk"/> object is not connected to
            /// Qwerk board.</exception>
            /// <exception cref="ConnectionFailedException">Failed connecting to the requested service.</exception>
            /// <exception cref="ServiceAccessFailedException">Failed accessing to the requested service.</exception>
            /// 
            public Motors( Qwerk qwerk )
            {
                string hostAddress = qwerk.HostAddress;

                // check if connection was established
                if ( hostAddress != null )
                {
                    try
                    {
                        Ice.ObjectPrx obj = qwerk.iceCommunicator.stringToProxy( "'::TeRK::MotorController':tcp -h " + hostAddress + " -p 10101" );
                        obj = obj.ice_timeout( Qwerk.TimeOut );
                        motorController = TeRKIceLib.MotorControllerPrxHelper.checkedCast( obj );
                    }
                    catch ( Ice.ObjectNotExistException )
                    {
                        // the object does not exist on the host
                        throw new ServiceAccessFailedException( "Failed accessing to the requested service." );
                    }
                    catch
                    {
                        throw new ConnectionFailedException( "Failed connecting to the requested service." );
                    }

                    if ( motorController == null )
                    {
                        throw new ServiceAccessFailedException( "Failed accessing to the requested cervice." );
                    }
                }
                else
                {
                    throw new NotConnectedException( "Qwerk object is not connected to a board." );
                }
            }

            /// <summary>
            /// Stop specified motor.
            /// </summary>
            /// 
            /// <param name="motor">Motor to stop, [0, <see cref="Motors.Count"/>).</param>
            /// 
            /// <exception cref="ArgumentOutOfRangeException">Invalid motor is specified.</exception>
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public void StopMotor( int motor )
            {
                if ( ( motor < 0 ) || ( motor >= Count ) )
                {
                    throw new ArgumentOutOfRangeException( "Invalid motor is specified." );
                }

                // prepare command
                TeRKIceLib.MotorCommand command = CreateCommand( );

                command.motorMask[motor]  = true;
                command.motorModes[motor] = TeRKIceLib.MotorMode.MotorOff;

                ExecuteCommand( command );
            }

            /// <summary>
            /// Stop all motors.
            /// </summary>
            /// 
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public void StopMotors( )
            {
                // prepare command
                TeRKIceLib.MotorCommand command = CreateCommand( );

                for ( int i = 0; i < Count; i++ )
                {
                    command.motorMask[i]  = true;
                    command.motorModes[i] = TeRKIceLib.MotorMode.MotorOff;
                }

                ExecuteCommand( command );
            }


            /// <summary>
            /// Set velocity of specified motor.
            /// </summary>
            /// 
            /// <param name="motor">Motor to set velocity for, [0, <see cref="Motors.Count"/>).</param>
            /// <param name="velocity">Velocity to set.</param>
            /// 
            /// <remarks><para>The method sets specified motor's velocity, which is measured in
            /// ticks per second. "Ticks" is a made-up term, which does not depend on specific motor,
            /// but is an unknown in distance and rotation (see Qwerk documentation for details).</para></remarks>
            /// 
            /// <exception cref="ArgumentOutOfRangeException">Invalid motor is specified.</exception>
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public void SetMotorVelocity( int motor, int velocity )
            {
                if ( ( motor < 0 ) || ( motor >= Count ) )
                {
                    throw new ArgumentOutOfRangeException( "Invalid motor is specified." );
                }

                // prepare command
                TeRKIceLib.MotorCommand command = CreateCommand( );

                command.motorMask[motor]          = true;
                command.motorModes[motor]         = TeRKIceLib.MotorMode.MotorSpeedControl;
                command.motorVelocities[motor]    = velocity;
                command.motorAccelerations[motor] = DefaultAcceleration;

                ExecuteCommand( command );
            }

            // Create motors' command
            private TeRKIceLib.MotorCommand CreateCommand( )
            {
                TeRKIceLib.MotorCommand command = new TeRKIceLib.MotorCommand( );

                command.motorMask          = new bool[Count];
                command.motorModes         = new TeRKIceLib.MotorMode[Count];
                command.motorPositions     = new int[Count];
                command.motorVelocities    = new int[Count];
                command.motorAccelerations = new int[Count];

                return command;
            }

            // Execute motors' command
            private void ExecuteCommand( TeRKIceLib.MotorCommand command )
            {
                // check controller
                if ( motorController == null )
                {
                    throw new NotConnectedException( "Qwerk's service is not connected." );
                }

                try
                {
                    // execute motors' command
                    motorController.execute( command );
                }
                catch
                {
                    throw new ConnectionLostException( "Connection is lost." );
                }
            }
        }
    }
}
