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
        /// Provides access to Qwerk's analog inputs.
        /// </summary>
        /// 
        /// <remarks><para>The class allows to retrieve state of Qwerk's analog inputs. The total
        /// number of available analog inputs equals to <see cref="AnalogIn.Count"/>.</para>
        /// 
        /// <para><note>The class is deprecated.</note></para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // get Qwerk's analog inputs service
        /// Qwerk.AnalogIn analogIns = qwerk.GetAnalogInService( );
        /// // get state of 0th input
        /// short input0 = analogIns.GetInput( 0 );
        /// // get state of all inputs
        /// short[] inputs = analogIns.GetInputs( );
        /// </code>
        /// </remarks>
        /// 
        [Obsolete( "The class is deprecated." )]
        public class AnalogIn
        {
            // Qwerk's analog in controller
            private TeRKIceLib.AnalogInControllerPrx analogInController = null;

            /// <summary>
            /// Number of available analog inputs, 8.
            /// </summary>
            public const int Count = 8;

            /// <summary>
            /// Initializes a new instance of the <see cref="Qwerk.AnalogIn"/> class.
            /// </summary>
            /// 
            /// <param name="qwerk">Reference to <see cref="Qwerk"/> object, which is connected to Qwerk board.</param>
            /// 
            /// <exception cref="NotConnectedException">The passed reference to <see cref="Qwerk"/> object is not connected to
            /// Qwerk board.</exception>
            /// <exception cref="ConnectionFailedException">Failed connecting to the requested service.</exception>
            /// <exception cref="ServiceAccessFailedException">Failed accessing to the requested service.</exception>
            /// 
            public AnalogIn( Qwerk qwerk )
            {
                string hostAddress = qwerk.HostAddress;

                // check if connection was established
                if ( hostAddress != null )
                {
                    try
                    {
                        Ice.ObjectPrx obj = qwerk.iceCommunicator.stringToProxy( "'::TeRK::AnalogInController':tcp -h " + hostAddress + " -p 10101" );
                        obj = obj.ice_timeout( Qwerk.TimeOut );
                        analogInController = TeRKIceLib.AnalogInControllerPrxHelper.checkedCast( obj );
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

                    if ( analogInController == null )
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
            /// Get state of the specified analog input.
            /// </summary>
            /// 
            /// <param name="input">Analog input to get state of, [0, <see cref="AnalogIn.Count"/>).</param>
            /// 
            /// <returns>Returns state of the requested input measured in milli volts.</returns>
            /// 
            /// <remarks><para>In the case if multiply inputs should be queried, it is much
            /// preferred to use <see cref="GetInputs"/> method, which retrieves state of all
            /// inputs at once.</para></remarks>
            /// 
            /// <exception cref="ArgumentOutOfRangeException">Invalid input is specified.</exception>
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public short GetInput( int input )
            {
                if ( ( input < 0 ) || ( input >= Count ) )
                {
                    throw new ArgumentOutOfRangeException( "Invalid input is specified." );
                } 
                
                return GetInputs( )[input];
            }

            /// <summary>
            /// Get state of all available analog inputs.
            /// </summary>
            /// 
            /// <returns>Returns state of all analog inputs measured in milli volts.</returns>
            /// 
            /// <exception cref="ConnectionLostException">Connestion to Qwerk is lost.</exception>
            /// 
            public short[] GetInputs( )
            {
                try
                {
                    TeRKIceLib.AnalogInState state = analogInController.getState( );
                    return state.analogInValues;
                }
                catch
                {
                    throw new ConnectionLostException( "Connection is lost." );
                }
            }
        }
    }
}
