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
        /// Provides access to Qwerk's servos' controllers.
        /// </summary>
        /// 
        /// <remarks><para>The class allows to manipulate Qwerk's servos. The total number
        /// of available servos equals to <see cref="Servos.Count"/>.</para>
        /// 
        /// <para>Each servo has logical and physical positions, which may or may not be equal.
        /// Both positions may vary in the [0, 255] range. Physical and logical positions are
        /// equal, when servo's physical moving bound (range) is set to [0, 255] (see <see cref="SetBound"/>).
        /// In this case setting servo's logical position with <see cref="SetPosition"/> method results
        /// in servo's physical moving in the [0, 255] range. However, of physical bound is set
        /// to [10, 20], for example, then physically the servo will move only in this range. But logical
        /// position is still may be set in [0, 255] range, which is mapped to physical range.</para>
        /// 
        /// <para><note>The class is deprecated.</note></para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // get Qwerk's servos service
        /// Qwerk.Servos servos = qwerk.GetServosService( );
        /// // stop all servos
        /// servos.StopServos( );
        /// // set 0th servo's bounds
        /// servos.SetBound( 0, new IntRange( 10, 100 ) );
        /// // set servo's position
        /// servos.SetPosition( 0, 50 );
        /// // ...
        /// 
        /// // get 0th servo's position
        /// int currentPosition = servos.GetPosition( 0 );
        /// </code>
        /// </remarks>
        /// 
        [Obsolete( "The class is deprecated." )]
        public class Servos
        {
            // Qwerk's servo controller
            private TeRKIceLib.ServoControllerPrx servoController = null;

            /// <summary>
            /// Number of available servos' controllers, 16.
            /// </summary>
            public const int Count = 16;

            // default servos' speed
            private const int DefaultSpeed = 1000;

            /// <summary>
            /// Initializes a new instance of the <see cref="Qwerk.Servos"/> class.
            /// </summary>
            /// 
            /// <param name="qwerk">Reference to <see cref="Qwerk"/> object, which is connected to Qwerk board.</param>
            /// 
            /// <exception cref="NotConnectedException">The passed reference to <see cref="Qwerk"/> object is not connected to
            /// Qwerk board.</exception>
            /// <exception cref="ConnectionFailedException">Failed connecting to the requested service.</exception>
            /// <exception cref="ServiceAccessFailedException">Failed accessing to the requested service.</exception>
            /// 
            public Servos( Qwerk qwerk )
            {
                string hostAddress = qwerk.HostAddress;

                // check if connection was established
                if ( hostAddress != null )
                {
                    try
                    {
                        Ice.ObjectPrx obj = qwerk.iceCommunicator.stringToProxy( "'::TeRK::ServoController':tcp -h " + hostAddress + " -p 10101" );
                        obj = obj.ice_timeout( Qwerk.TimeOut );
                        servoController = TeRKIceLib.ServoControllerPrxHelper.checkedCast( obj );
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

                    if ( servoController == null )
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
            /// Stop specified servo.
            /// </summary>
            /// 
            /// <param name="servo">Servo to stop, [0, <see cref="Servos.Count"/>).</param>
            /// 
            /// <returns>Returns current position of the specified servo.</returns>
            /// 
            /// <exception cref="ArgumentOutOfRangeException">Invalid servo is specified.</exception>
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public int StopServo( int servo )
            {
                if ( ( servo < 0 ) || ( servo >= Count ) )
                {
                    throw new ArgumentOutOfRangeException( "Invalid servo is specified." );
                } 
                
                bool[] mask = new bool[Count];
                mask[servo] = true;

                return StopServos( mask )[servo];
            }

            /// <summary>
            /// Stop all servos.
            /// </summary>
            /// 
            /// <returns>Returns array of current servos' positions.</returns>
            ///
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            ///
            public int[] StopServos( )
            {
                bool[] mask = new bool[Count];

                for ( int i = 0; i < Count; i++ )
                {
                    mask[i] = true;
                }

                return StopServos( mask );
            }

            /// <summary>
            /// Stop specified servos.
            /// </summary>
            /// 
            /// <param name="mask">Mask array specifying which servos need to stop.</param>
            /// 
            /// <returns>Returns array of current servos' positions.</returns>
            ///
            /// <remarks><para>The <paramref name="mask"/> array specifies
            /// which Qwerk's servo need to be stopped. If value of the <paramref name="mask"/>
            /// array is set to <see langword="true"/>, then corresponding servo is stopped.</para>
            /// </remarks>
            ///
            /// <exception cref="ArgumentException">Incorrect length of <paramref name="mask"/> array.</exception>
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            ///
            public int[] StopServos( bool[] mask )
            {
                if ( mask.Length != Count )
                {
                    throw new ArgumentException( "Incorrect length of mask array." );
                }

                // check controller
                if ( servoController == null )
                {
                    throw new NotConnectedException( "Qwerk's service is not connected." );
                }

                try
                {
                    TeRKIceLib.ServoCommand command = CreateCommand( );

                    for ( int i = 0; i < Count; i++ )
                    {
                        command.servoMask[i]      = mask[i];
                        command.servoModes[i]     = TeRKIceLib.ServoMode.ServoOff;
                        command.servoPositions[i] = 0;
                        command.servoSpeeds[i]    = 0;
                    }

                    // execute servos' command
                    TeRKIceLib.ServoState state = servoController.execute( command );
                    return state.servoPositions;
                }
                catch
                {
                    throw new ConnectionLostException( "Connection is lost." );
                }
            }

            /// <summary>
            /// Set position of a single servo.
            /// </summary>
            /// 
            /// <param name="servo">Servo to set position for, [0, <see cref="Servos.Count"/>).</param>
            /// <param name="position">Position to set for the specified servo, [0, 255].</param>
            /// 
            /// <returns>Returns current position of the specified servo.</returns>
            /// 
            /// <remarks><para>The method sets position of single Qwerk's servo, which index is
            /// specified. It is preferred to use <see cref="SetPositions"/> for setting positions
            /// of multiple servos, which does it at once.</para></remarks>
            /// 
            /// <exception cref="ArgumentOutOfRangeException">Invalid servo is specified.</exception>
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public int SetPosition( int servo, int position )
            {
                if ( ( servo < 0 ) || ( servo >= Count ) )
                {
                    throw new ArgumentOutOfRangeException( "Invalid servo is specified." );
                }

                bool[] mask = new bool[Count];
                int[] positions = new int[Count];

                mask[servo] = true;
                positions[servo] = position;

                return SetPositions( mask, positions )[servo];
            }

            /// <summary>
            /// Set positions of specified servos.
            /// </summary>
            /// 
            /// <param name="mask">Mask array specifying which servos need to be set.</param>
            /// <param name="positions">Array of servos' positions to set. Each position is in [0, 255] range.</param>
            /// 
            /// <returns>Returns array of current servos' positions.</returns>
            ///
            /// <remarks><para>The <paramref name="mask"/> and <paramref name="positions"/> arrays specify
            /// which Qwerk's servo's state should be updated. If value of the <paramref name="mask"/>
            /// array is set to <see langword="true"/>, then corresponding servo's state is changed to the state,
            /// which is specified in <paramref name="positions"/> array.</para>
            /// </remarks>
            ///
            /// <exception cref="ArgumentException">Incorrect length of <paramref name="mask"/> or
            /// <paramref name="positions"/> array.</exception>
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            ///
            public int[] SetPositions( bool[] mask, int[] positions )
            {
                if ( ( mask.Length != Count ) || ( positions.Length != Count ) )
                {
                    throw new ArgumentException( "Incorrect length of mask or positions array." );
                }

                // check controller
                if ( servoController == null )
                {
                    throw new NotConnectedException( "Qwerk's service is not connected." );
                }

                try
                {
                    TeRKIceLib.ServoCommand command = CreateCommand( );

                    for ( int i = 0; i < Count; i++ )
                    {
                        command.servoMask[i]      = mask[i];
                        command.servoModes[i]     = TeRKIceLib.ServoMode.ServoMotorPositionControl;
                        command.servoPositions[i] = positions[i];
                        command.servoSpeeds[i]    = DefaultSpeed;
                    }

                    // execute servos' command
                    TeRKIceLib.ServoState state = servoController.execute( command );
                    return state.servoPositions;
                }
                catch
                {
                    throw new ConnectionLostException( "Connection is lost." );
                }
            }

            /// <summary>
            /// Get current position of a single servo.
            /// </summary>
            /// 
            /// <param name="servo">Servo to get position for, [0, <see cref="Servos.Count"/>).</param>
            /// 
            /// <returns>Returns current position of the specified servo.</returns>
            /// 
            /// <exception cref="ArgumentOutOfRangeException">Invalid servo is specified.</exception>
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public int GetPosition( int servo )
            {
                if ( ( servo < 0 ) || ( servo >= Count ) )
                {
                    throw new ArgumentOutOfRangeException( "Invalid servo is specified." );
                }

                return GetPositions( )[servo];
            }

            /// <summary>
            /// Get current positions of all servos.
            /// </summary>
            /// 
            /// <returns>Returns array of current servos' positions.</returns>
            /// 
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public int[] GetPositions( )
            {
                return SetPositions( new bool[Count], new int[Count] ); 
            }

            /// <summary>
            /// Set bounds for the specified servo.
            /// </summary>
            /// 
            /// <param name="servo">Servo to set bounds for, [0, <see cref="Servos.Count"/>).</param>
            /// <param name="bound">Bounds to set for the specified servo.</param>
            /// 
            /// <remarks><para>The method sets servo's physical bounds in which it may move.
            /// See documentation to <see cref="Qwerk.Servos"/> for clarification.</para></remarks>
            /// 
            /// <exception cref="ArgumentOutOfRangeException">Invalid servo is specified.</exception>
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public void SetBound( int servo, IntRange bound )
            {
                if ( ( servo < 0 ) || ( servo >= Count ) )
                {
                    throw new ArgumentOutOfRangeException( "Invalid servo is specified." );
                }

                bool[] mask = new bool[Count];
                IntRange[] bounds = new IntRange[Count];

                mask[servo] = true;
                bounds[servo] = bound;

                SetBounds( mask, bounds );
            }

            /// <summary>
            /// Set bounds for specified servos.
            /// </summary>
            /// 
            /// <param name="mask">Mask array specifying which servos need to be set.</param>
            /// <param name="bounds">Array of servos' bounds. Each bound may be in [0, 255] range.</param>
            ///
            /// <remarks><para>The method sets servos' physical bounds in which they may move.
            /// See documentation to <see cref="Qwerk.Servos"/> for clarification.</para></remarks>
            /// 
            /// <exception cref="ArgumentException">Incorrect length of <paramref name="mask"/>,
            /// or <paramref name="bounds"/> array.</exception>
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public void SetBounds( bool[] mask, IntRange[] bounds )
            {
                if ( ( mask.Length != Count ) || ( bounds.Length != Count ) )
                {
                    throw new ArgumentException( "Incorrect length of mask or positions array." );
                }

                // check controller
                if ( servoController == null )
                {
                    throw new NotConnectedException( "Qwerk's service is not connected." );
                }

                try
                {
                    TeRKIceLib.Bounds[] nativeBounds = new TeRKIceLib.Bounds[Count];

                    for ( int i = 0; i < Count; i++ )
                    {
                        if ( mask[i] )
                        {
                            nativeBounds[i].min = bounds[i].Min;
                            nativeBounds[i].max = bounds[i].Max;
                        }
                    }

                    // set servos' bounds
                    servoController.setBounds( mask, nativeBounds );
                }
                catch
                {
                    throw new ConnectionLostException( "Connection is lost." );
                }
            }

            /// <summary>
            /// Get bounds for the specified servo.
            /// </summary>
            /// 
            /// <param name="servo">Servo to get bounds for, [0, <see cref="Servos.Count"/>).</param>
            /// 
            /// <returns>Returns configured bounds of the specified servo.</returns>
            /// 
            /// <exception cref="ArgumentOutOfRangeException">Invalid servo is specified.</exception>
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public IntRange GetBound( int servo )
            {
                if ( ( servo < 0 ) || ( servo >= Count ) )
                {
                    throw new ArgumentOutOfRangeException( "Invalid servo is specified." );
                }

                return GetBounds( )[servo];
            }

            /// <summary>
            /// Get bounds of all servos.
            /// </summary>
            /// 
            /// <returns>Returns array of configured servos' bounds.</returns>
            /// 
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public IntRange[] GetBounds( )
            {
                // check controller
                if ( servoController == null )
                {
                    throw new NotConnectedException( "Qwerk's service is not connected." );
                }

                try
                {
                    // get servos' bounds
                    TeRKIceLib.Bounds[] nativeBounds = servoController.getBounds( );

                    IntRange[] bounds = new IntRange[Count];

                    for ( int i = 0; i < Count; i++ )
                    {
                        bounds[i] = new IntRange( nativeBounds[i].min, nativeBounds[i].max );
                    }

                    return bounds;
                }
                catch
                {
                    throw new ConnectionLostException( "Connection is lost." );
                }
            }


            // Create servos' command
            private TeRKIceLib.ServoCommand CreateCommand( )
            {
                TeRKIceLib.ServoCommand command = new TeRKIceLib.ServoCommand( );

                command.servoMask      = new bool[Count];
                command.servoModes     = new TeRKIceLib.ServoMode[Count];
                command.servoPositions = new int[Count];
                command.servoSpeeds    = new int[Count];

                return command;
            }
        }
    }
}
