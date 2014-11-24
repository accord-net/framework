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
        /// Enumeration of Qwerk's LEDs' states.
        /// </summary>
        public enum LedState
        {
            /// <summary>
            /// LED is off.
            /// </summary>
            Off,
            /// <summary>
            /// LED is on.
            /// </summary>
            On,
            /// <summary>
            /// LES is on and blinking.
            /// </summary>
            Blinking
        }

        /// <summary>
        /// Provides access to Qwerk's on-board LEDs.
        /// </summary>
        /// 
        /// <remarks><para>The class allows to manipulate Qwerk's on-boar LED's. The total number
        /// of available on-board LEDs equals to <see cref="Leds.Count"/>.</para>
        /// 
        /// <para><note>The class is deprecated.</note></para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // get Qwerk's LEDs service
        /// Qwerk.Leds leds = qwerk.GetLedsService( );
        /// // turn off all LEDs
        /// leds.SetLedsState( Qwerk.LedState.Off );
        /// // set zero LED to blinking state
        /// leds.SetLedState( 0, Qwerk.LedState.Blinking );
        /// // turn on 4th and 5th LEDs
        /// bool[] mask = new bool[10] {
        ///     false, false, false, false, true,
        ///     true, false, false, false, false };
        /// Qwerk.LedState[] states = new Qwerk.LedState[10] {
        ///     Qwerk.LedState.Off, Qwerk.LedState.Off, Qwerk.LedState.Off,
        ///     Qwerk.LedState.Off, Qwerk.LedState.On,  Qwerk.LedState.On,
        ///     Qwerk.LedState.Off, Qwerk.LedState.Off, Qwerk.LedState.Off,
        ///     Qwerk.LedState.Off };
        /// leds.SetLedsState( mask, states );
        /// </code>
        /// </remarks>
        /// 
        [Obsolete( "The class is deprecated." )]
        public class Leds
        {
            // Qwerk's LED controller
            private TeRKIceLib.LEDControllerPrx ledController = null;
            // mapping array to map LedState enumeration to TeRKIceLib.LEDMode enumeration
            private TeRKIceLib.LEDMode[] modesMapping = new TeRKIceLib.LEDMode[]
                { TeRKIceLib.LEDMode.LEDOff, TeRKIceLib.LEDMode.LEDOn, TeRKIceLib.LEDMode.LEDBlinking };

            /// <summary>
            /// Number of available on-board LEDs, 10.
            /// </summary>
            public const int Count = 10;

            /// <summary>
            /// Initializes a new instance of the <see cref="Qwerk.Leds"/> class.
            /// </summary>
            /// 
            /// <param name="qwerk">Reference to <see cref="Qwerk"/> object, which is connected to Qwerk board.</param>
            /// 
            /// <exception cref="NotConnectedException">The passed reference to <see cref="Qwerk"/> object is not connected to
            /// Qwerk board.</exception>
            /// <exception cref="ConnectionFailedException">Failed connecting to the requested service.</exception>
            /// <exception cref="ServiceAccessFailedException">Failed accessing to the requested service.</exception>
            /// 
            public Leds( Qwerk qwerk )
            {
                string hostAddress = qwerk.HostAddress;

                // check if connection was established
                if ( hostAddress != null )
                {
                    try
                    {
                        Ice.ObjectPrx obj = qwerk.iceCommunicator.stringToProxy( "'::TeRK::LEDController':tcp -h " + hostAddress + " -p 10101" );
                        obj = obj.ice_timeout( Qwerk.TimeOut );
                        ledController = TeRKIceLib.LEDControllerPrxHelper.checkedCast( obj );
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

                    if ( ledController == null )
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
            /// Set state of specified LED.
            /// </summary>
            /// 
            /// <param name="led">LED to set state for, [0, <see cref="Leds.Count"/>).</param>
            /// <param name="state">LED's state to set.</param>
            /// 
            /// <remarks><para>The method sets state of one of Qwerk's LEDs, which index is specified.</para></remarks>
            /// 
            /// <exception cref="ArgumentOutOfRangeException">Invalid LED is specified.</exception>
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public void SetLedState( int led, LedState state )
            {
                if ( ( led < 0 ) || ( led >= Count ) )
                {
                    throw new ArgumentOutOfRangeException( "Invalid LED is specified." );
                }

                bool[] ledsMask = new bool[Count];
                LedState[] states = new LedState[Count];

                ledsMask[led] = true;
                states[led] = state;

                SetLedsState( ledsMask, states );
            }

            /// <summary>
            /// Set state of all LEDs.
            /// </summary>
            /// 
            /// <param name="state">State, which will be set to all LEDs.</param>
            /// 
            /// <remarks><para>The method sets the same state to all Qwerk's on-board LEDs.</para></remarks>
            /// 
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public void SetLedsState( LedState state )
            {
                bool[] ledsMask = new bool[Count];
                LedState[] states = new LedState[Count];

                for ( int i = 0; i < Count; i++ )
                {
                    ledsMask[i] = true;
                    states[i] = state;
                }

                SetLedsState( ledsMask, states );
            }

            /// <summary>
            /// Set state of all LEDs.
            /// </summary>
            /// 
            /// <param name="ledsMask">Mask array specifying which LED's state need to be set.</param>
            /// <param name="states">Array of LEDs' states.</param>
            /// 
            /// <remarks><para>The <paramref name="ledsMask"/> and <paramref name="states"/> arrays specify
            /// which Qwerk's on-board LED's state should be updated. If value of the <paramref name="ledsMask"/>
            /// array is set to <see langword="true"/>, then corresponding LED's state is changed to the state,
            /// which is specified in <paramref name="states"/> array.</para>
            /// </remarks>
            /// 
            /// <exception cref="ArgumentException">Incorrect length of LEDs' masks or states array.</exception>
            /// <exception cref="NotConnectedException">No connection to Qwerk or its service.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public void SetLedsState( bool[] ledsMask, LedState[] states )
            {
                if ( ( ledsMask.Length != Count ) || ( states.Length != Count ) )
                {
                    throw new ArgumentException( "Incorrect length of leds' masks or states array." );
                }

                TeRKIceLib.LEDMode[] modes = new TeRKIceLib.LEDMode[Count];

                for ( int i = 0; i < Count; i++ )
                {
                    modes[i] = modesMapping[(int) states[i]];
                }

                // check controller
                if ( ledController == null )
                {
                    throw new NotConnectedException( "Qwerk's service is not connected." );
                }

                try
                {
                    // execute leds' command
                    ledController.execute( new TeRKIceLib.LEDCommand( ledsMask, modes ) );
                }
                catch
                {
                    throw new ConnectionLostException( "Connection is lost." );
                }
            }
        }
    }
}
