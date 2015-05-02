// AForge Controls Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2010
// andrew.kirillov@aforgenet.com
//

using System;
using System.Collections.Generic;
using AForge;

namespace AForge.Controls
{
    /// <summary>
    /// The class provides simple API for enumerating available joysticks and checking their
    /// current status.
    /// </summary>
    /// 
    /// <remarks><para>The class provides simple access to joysticks (game controllers) through using
    /// Win32 API, which allows to enumerate available devices and query their status (state of all buttons,
    /// axes, etc).</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // enumerate available devices
    /// List&lt;Joystick.DeviceInfo&gt; devices = Joystick.GetAvailableDevices( );
    /// 
    /// foreach ( Joystick.DeviceInfo di in devices )
    /// {
    ///     System.Diagnostics.Debug.WriteLine(
    ///         string.Format( "{0} : {1} ({2} axes, {3} buttons)",
    ///             di.ID, di.Name, di.Axes, di.Buttons ) );
    /// }
    /// 
    /// 
    /// // create new joystick and initialize it
    /// Joystick joystick = new Joystick( 0 );
    /// // get its current status
    /// Joystick.Status status = joystick.GetCurrentStatus( );
    /// // check if 1st button is pressed
    /// if ( status.IsButtonPressed( Joystick.Buttons.Button1 ) )
    /// {
    ///     // 1st button is pressed
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class Joystick
    {
        // information of the initialized joystick
        private DeviceInfo info = null;

        /// <summary>
        /// Information about initialized joystick.
        /// </summary>
        ///
        /// <remarks><para>The property keeps information about joystick, which was
        /// initialized using <see cref="Init"/> method. If no joystick was initialized,
        /// then accessing this property will generate <see cref="ApplicationException"/>
        /// exception.</para></remarks>
        /// 
        /// <exception cref="ApplicationException">Joystick was not initialized.</exception>
        ///
        public DeviceInfo Info
        {
            get
            {
                if ( info == null )
                    throw new ApplicationException( "Joystick was not initialized." );

                return info;
            }
        }

        /// <summary>
        /// Information about joystick connected to the system.
        /// </summary>
        /// 
        public class DeviceInfo
        {
            /// <summary>
            /// Joystick ID, [0..15].
            /// </summary>
            public readonly int ID;

            internal readonly JoystickAPI.JOYCAPS capabilities;

            /// <summary>
            /// Joystick name.
            /// </summary>
            public string Name
            {
                get { return capabilities.name; }
            }

            /// <summary>
            /// Number of joystick axes.
            /// </summary>
            public int Axes
            {
                get { return capabilities.axesNumber; }
            }

            /// <summary>
            /// Number of joystick buttons.
            /// </summary>
            public int Buttons
            {
                get { return capabilities.buttonsNumber; }
            }

            internal DeviceInfo( int id, JoystickAPI.JOYCAPS joyCaps )
            {
                ID = id;
                capabilities = joyCaps;
            }
        }

        /// <summary>
        /// Get list of available joysticks connected to the system.
        /// </summary>
        /// 
        /// <returns>Returns list containing information about available joysticks connected to
        /// the system.</returns>
        /// 
        public static List<DeviceInfo> GetAvailableDevices( )
        {
            List<DeviceInfo> devices = new List<DeviceInfo>( );
            int joyCapsSize = System.Runtime.InteropServices.Marshal.SizeOf( typeof( JoystickAPI.JOYCAPS ) );

            // get number of devices
            int devicesCount = JoystickAPI.joyGetNumDevs( );
            // check all devices
            for ( int i = 0; i < devicesCount; i++ )
            {
                JoystickAPI.JOYCAPS joyCaps = new JoystickAPI.JOYCAPS( );

                if ( JoystickAPI.joyGetDevCapsW( i, joyCaps, joyCapsSize ) == JoystickAPI.ResultCode.NoError )
                {
                    devices.Add( new DeviceInfo( i, joyCaps ) );
                }
            }

            return devices;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Joystick"/> class.
        /// </summary>
        /// 
        /// <remarks><para>This constructor does not make initialization of any joystick
        /// device, so <see cref="Init"/> method should be used before querying joystick
        /// status or properties.</para></remarks>
        ///
        public Joystick( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Joystick"/> class.
        /// </summary>
        /// 
        /// <param name="id">Joystick ID to initialize, [0, 15].</param>
        /// 
        /// <remarks><para>This constructor initializes joystick with specified ID using
        /// <see cref="Init"/> method, so the object becomes ready for querying joystick's
        /// status.</para></remarks>
        /// 
        public Joystick( int id )
        {
            Init( id );
        }

        /// <summary>
        /// Initialize joystick with the specified ID.
        /// </summary>
        /// 
        /// <param name="id">Joystick's ID to initialize, [0, 15].</param>
        /// 
        /// <remarks><para></para></remarks>
        /// 
        /// <exception cref="ArgumentException">Invalid joystick ID was specified. It must be in [0, 15] range.</exception>
        /// <exception cref="NotConnectedException">The requested joystick is not connected to the system.</exception>
        /// 
        public void Init( int id )
        {
            if ( ( id < 0 ) || ( id > 15 ) )
            {
                throw new ArgumentException( "Invalid joystick ID was specified." );
            }

            JoystickAPI.JOYCAPS joyCaps = new JoystickAPI.JOYCAPS( ); 
            
            if ( JoystickAPI.joyGetDevCapsW( id, joyCaps,
                System.Runtime.InteropServices.Marshal.SizeOf( joyCaps ) ) != JoystickAPI.ResultCode.NoError )
            {
                throw new NotConnectedException( "The requested joystick is not connected to the system." );
            }

            info = new DeviceInfo( id, joyCaps );
        }

        private static JoystickAPI.JoyPosFlags[] requestFlags = new JoystickAPI.JoyPosFlags[]
        {
            JoystickAPI.JoyPosFlags.ReturnPov | JoystickAPI.JoyPosFlags.ReturnButtons,
            JoystickAPI.JoyPosFlags.ReturnPov | JoystickAPI.JoyPosFlags.ReturnButtons | JoystickAPI.JoyPosFlags.ReturnX,
            JoystickAPI.JoyPosFlags.ReturnPov | JoystickAPI.JoyPosFlags.ReturnButtons | JoystickAPI.JoyPosFlags.ReturnXY,
            JoystickAPI.JoyPosFlags.ReturnPov | JoystickAPI.JoyPosFlags.ReturnButtons | JoystickAPI.JoyPosFlags.ReturnXYZ,
            JoystickAPI.JoyPosFlags.ReturnPov | JoystickAPI.JoyPosFlags.ReturnButtons | JoystickAPI.JoyPosFlags.ReturnXYZR,
            JoystickAPI.JoyPosFlags.ReturnPov | JoystickAPI.JoyPosFlags.ReturnButtons | JoystickAPI.JoyPosFlags.ReturnXYZRU,
        };

        /// <summary>
        /// Get joystick's status.
        /// </summary>
        /// 
        /// <returns>Returns current status of initialized joystick, which provides information
        /// about current state of all axes, buttons and point of view.</returns>
        /// 
        /// <remarks><para><note>Before using this method the joystick object needs to be initialized
        /// using <see cref="Init"/> method or <see cref="Joystick(int)"/> constructor.</note></para></remarks>
        /// 
        /// <exception cref="NotConnectedException">The requested joystick is not connected to the system.</exception>
        /// <exception cref="ApplicationException">Joystick was not initialized.</exception>
        ///
        public Status GetCurrentStatus( )
        {
            JoystickAPI.JOYINFOEX ji = new JoystickAPI.JOYINFOEX( );

            ji.size = System.Runtime.InteropServices.Marshal.SizeOf( ji );
            ji.flags = ( Info.capabilities.axesNumber > 5 ) ? JoystickAPI.JoyPosFlags.ReturnAll :
                requestFlags[Info.capabilities.axesNumber];

            if ( JoystickAPI.joyGetPosEx( Info.ID, ji ) != JoystickAPI.ResultCode.NoError )
            {
                throw new NotConnectedException( "The requested joystick is not connected to the system." );
            }

            return new Status( ji, Info.capabilities );
        }

        /// <summary>
        /// Class describing current joystick's status.
        /// </summary>
        ///
        /// <remarks><para><note>All joystick axes' positions are measured in [-1, 1] range, where
        /// 0 corresponds to center position - axis is not deflected (directed) to any side.</note></para></remarks>
        ///
        public class Status
        {
            private JoystickAPI.JOYINFOEX status;
            private JoystickAPI.JOYCAPS capabilities;

            internal Status( JoystickAPI.JOYINFOEX status, JoystickAPI.JOYCAPS capabilities )
            {
                this.status = status;
                this.capabilities = capabilities;
            }

            /// <summary>
            /// Position of X axis, [-1, 1].
            /// </summary>
            public float XAxis
            {
                get
                {
                    return ( ( ( status.flags & JoystickAPI.JoyPosFlags.ReturnX ) == 0 ) ? 0 :
                        (float) ( status.xPos - capabilities.xMin ) / capabilities.xMax * 2 - 1 );
                }
            }

            /// <summary>
            /// Position of Y axis, [-1, 1].
            /// </summary>
            public float YAxis
            {
                get
                {
                    return ( ( ( status.flags & JoystickAPI.JoyPosFlags.ReturnY ) == 0 ) ? 0 :
                        (float) ( status.yPos - capabilities.yMin ) / capabilities.yMax * 2 - 1 );
                }
            }

            /// <summary>
            /// Position of Z axis, [-1, 1].
            /// </summary>
            public float ZAxis
            {
                get
                {
                    return ( ( ( status.flags & JoystickAPI.JoyPosFlags.ReturnZ ) == 0 ) ? 0 :
                        (float) ( status.zPos - capabilities.zMin ) / capabilities.zMax * 2 - 1 );
                }
            }

            /// <summary>
            /// Position of R axis - 4th joystick's axes, [-1, 1].
            /// </summary>
            public float RAxis
            {
                get
                {
                    return ( ( ( status.flags & JoystickAPI.JoyPosFlags.ReturnR ) == 0 ) ? 0 :
                        (float) ( status.rPos - capabilities.rMin ) / capabilities.rMax * 2 - 1 );
                }
            }

            /// <summary>
            /// Position of U axis - 5th joystick's axes, [-1, 1].
            /// </summary>
            public float UAxis
            {
                get
                {
                    return ( ( ( status.flags & JoystickAPI.JoyPosFlags.ReturnU ) == 0 ) ? 0 :
                        (float) ( status.uPos - capabilities.uMin ) / capabilities.uMax * 2 - 1 );
                }
            }

            /// <summary>
            /// Position of V axis - 6th joystick's axes, [-1, 1].
            /// </summary>
            public float VAxis
            {
                get
                {
                    return ( ( ( status.flags & JoystickAPI.JoyPosFlags.ReturnV ) == 0 ) ? 0 :
                        (float) ( status.vPos - capabilities.vMin ) / capabilities.vMax * 2 - 1 );
                }
            }

            /// <summary>
            /// Joystick buttons' state.
            /// </summary>
            public Buttons Buttons
            {
                get { return (Buttons) status.buttons; }
            }

            /// <summary>
            /// Current point of view state, [0, 359].
            /// </summary>
            /// 
            public float PointOfView
            {
                get { return ( status.pov > 35900) ? -1 : (float) status.pov / 100 ; }
            }

            /// <summary>
            /// Check if certain button (or combination of buttons) is pressed.
            /// </summary>
            /// 
            /// <param name="button">Button to check state of.</param>
            /// 
            /// <returns>Returns <see langword="true"/> if the specified button is pressed or
            /// <see langword="false"/> otherwise.</returns>
            ///
            public bool IsButtonPressed( Buttons button )
            {
                return ( ( ( (Buttons) status.buttons ) & button ) != 0 );
            }
        }

        /// <summary>
        /// Flags enumeration of joystick buttons.
        /// </summary>
        [Flags]
        public enum Buttons
        {
            /// <summary>
            /// 1st button.
            /// </summary>
            Button1  = 0x0001,
            /// <summary>
            /// 2nd button.
            /// </summary>
            Button2 = 0x0002,
            /// <summary>
            /// 3rd button.
            /// </summary>
            Button3  = 0x0004,
            /// <summary>
            /// 4th button.
            /// </summary>
            Button4  = 0x0008,
            /// <summary>
            /// 5th button.
            /// </summary>
            Button5 = 0x0010,
            /// <summary>
            /// 6th button.
            /// </summary>
            Button6 = 0x0020,
            /// <summary>
            /// 7th button.
            /// </summary>
            Button7 = 0x0040,
            /// <summary>
            /// 8th button.
            /// </summary>
            Button8 = 0x0080,
            /// <summary>
            /// 9th button.
            /// </summary>
            Button9 = 0x0100,
            /// <summary>
            /// 10th button.
            /// </summary>
            Button10 = 0x0200,
            /// <summary>
            /// 11th button.
            /// </summary>
            Button11 = 0x0400,
            /// <summary>
            /// 12th button.
            /// </summary>
            Button12 = 0x0800,
            /// <summary>
            /// 13th button.
            /// </summary>
            Button13 = 0x1000,
            /// <summary>
            /// 14th button.
            /// </summary>
            Button14 = 0x2000,
            /// <summary>
            /// 15th button.
            /// </summary>
            Button15 = 0x4000,
            /// <summary>
            /// 16th button.
            /// </summary>
            Button16 = 0x8000,
        }
    }
}
