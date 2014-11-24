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
        /// Provides access to Qwerk's digital inputs.
        /// </summary>
        /// 
        /// <remarks><para>The class allows to retrieve state of Qwerk's digital inputs. The total
        /// number of available digital inputs equals to <see cref="DigitalIn.Count"/>.</para>
        /// 
        /// <para><note>The class is deprecated.</note></para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // get Qwerk's digital inputs service
        /// Qwerk.DigitalIn digitalIns = qwerk.GetDigitalInService( );
        /// // get state of 0th input
        /// bool input0 = digitalIns.GetInput( 0 );
        /// // get state of all inputs
        /// bool[] inputs = digitalIns.GetInputs( );
        /// </code>
        /// </remarks>
        /// 
        [Obsolete( "The class is deprecated." )]
        public class DigitalIn
        {
            // Qwerk's digital in controller
            private TeRKIceLib.DigitalInControllerPrx digitalInController = null;

            /// <summary>
            /// Number of available digital inputs, 4.
            /// </summary>
            public const int Count = 4;

            /// <summary>
            /// Initializes a new instance of the <see cref="Qwerk.DigitalIn"/> class.
            /// </summary>
            /// 
            /// <param name="qwerk">Reference to <see cref="Qwerk"/> object, which is connected to Qwerk board.</param>
            /// 
            /// <exception cref="NotConnectedException">The passed reference to <see cref="Qwerk"/> object is not connected to
            /// Qwerk board.</exception>
            /// <exception cref="ConnectionFailedException">Failed connecting to the requested service.</exception>
            /// <exception cref="ServiceAccessFailedException">Failed accessing to the requested service.</exception>
            /// 
            public DigitalIn( Qwerk qwerk )
            {
                string hostAddress = qwerk.HostAddress;

                // check if connection was established
                if ( hostAddress != null )
                {
                    try
                    {
                        Ice.ObjectPrx obj = qwerk.iceCommunicator.stringToProxy( "'::TeRK::DigitalInController':tcp -h " + hostAddress + " -p 10101" );
                        obj = obj.ice_timeout( Qwerk.TimeOut );
                        digitalInController = TeRKIceLib.DigitalInControllerPrxHelper.checkedCast( obj );
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

                    if ( digitalInController == null )
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
            /// Get state of the specified digital input.
            /// </summary>
            /// 
            /// <param name="input">Digital input to get state of, [0, <see cref="DigitalIn.Count"/>).</param>
            /// 
            /// <returns>Returns state of the requested input as boolean values - active/inactive.</returns>
            /// 
            /// <remarks><para>In the case if multiply inputs should be queried, it is much
            /// preferred to use <see cref="GetInputs"/> method, which retrieves state of all
            /// inputs at once.</para></remarks>
            /// 
            /// <exception cref="ArgumentOutOfRangeException">Invalid input is specified.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public bool GetInput( int input )
            {
                if ( ( input < 0 ) || ( input >= Count ) )
                {
                    throw new ArgumentOutOfRangeException( "Invalid input is specified." );
                } 
                
                return GetInputs( )[input];
            }

            /// <summary>
            /// Get state of all available digital inputs.
            /// </summary>
            /// 
            /// <returns>Returns state of all digital inputs as boolean values - active/inactive.</returns>
            /// 
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public bool[] GetInputs( )
            {
                try
                {
                    TeRKIceLib.DigitalInState state = digitalInController.getState( );
                    return state.digitalInStates;
                }
                catch
                {
                    throw new ConnectionLostException( "Connection is lost." );
                }
            }
        }
    }
}
