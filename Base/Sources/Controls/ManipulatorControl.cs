// AForge Controls Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2010
// andrew.kirillov@aforgenet.com
//

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AForge.Controls
{
    /// <summary>
    /// Manipulator control.
    /// </summary>
    ///
    /// <remarks>
    /// <para>The manipulator control can be used to mimic behaviour of analogue joystick using
    /// regular mouse. By dragging manipulator away from control's centre, it fires <see cref="PositionChanged"/>
    /// event notifying about its X/Y coordinates (or about R/Theta coordinates in Polar coordinates system).
    /// </para>
    /// 
    /// <para>For example, in robotics applications the control can be used to drive robots. If user drags manipulator
    /// further from centre (increasing distance between centre and manipulator), then higher power (speed) should be
    /// set for robot's motors. But dragging it in different directions away from centre should result in changing
    /// robot's direction: straight forward, backward, turning right or left, etc.<br />
    /// <img src="img/controls/manipulator_round.png" width="215" height="215" />
    /// </para>
    /// 
    /// <para>Another possible application of the control is to control position of some device, etc.
    /// For example, the control could be used with pan-tilt camera - by dragging control away from centre,
    /// the camera may rotate in one of the directions.<br />
    /// <img src="img/controls/manipulator_square.png" width="215" height="215" />
    /// </para>
    /// </remarks>
    ///
    public partial class ManipulatorControl : Control
    {
        private bool isSquareLook = false;
        private bool drawHorizontalAxis = true;
        private bool drawVerticalAxis   = false;

        private bool resetPositionOnMouseRelease = true;

        // pens and brushes for drawing
        private Pen borderPen = new Pen( Color.Black );
        private SolidBrush topLeftBackgroundBrush     = new SolidBrush( Color.White );
        private SolidBrush topRightBackgroundBrush    = new SolidBrush( Color.White );
        private SolidBrush bottomLeftBackgroundBrush  = new SolidBrush( Color.LightGray );
        private SolidBrush bottomRightBackgroundBrush = new SolidBrush( Color.LightGray );
        private SolidBrush manipulatorBrush           = new SolidBrush( Color.LightSeaGreen );
        private SolidBrush disabledBrash              = new SolidBrush( Color.FromArgb( 240, 240, 240 ) );

        // manipulator's position
        private float manipulatatorX = 0;
        private float manipulatatorY = 0;

        // manipulator's size
        private const int manipulatorSize = 21;
        private const int manipulatorRadius = manipulatorSize / 2;

        // size (width and height) of manipulator's area
        private int areaSize = 0;
        private int areaRadius = 0;
        private int areaMargin = manipulatorSize / 2 + 2;

        // tracking information
        private bool tracking = false;

        // number of timer ticks before notifying user (-1 means no notification)
        private int ticksBeforeNotificiation = -1;

        /// <summary>
        /// Determines if the control has square or round look.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>The control has a square look if the property is set to <see langword="true"/>,
        /// otherwise it has round look.</para>
        /// 
        /// <para>Default value is set to <see langword="false"/>.</para>
        /// </remarks>
        /// 
        [DefaultValue( false )]
        [Description( "Determines if the control has square or round look." )]
        public bool IsSquareLook
        {
            get { return isSquareLook; }
            set
            {
                isSquareLook = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// Determines if horizontal axis should be drawn or not.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <see langword="true"/>.</para>
        /// </remarks>
        /// 
        [DefaultValue( true )]
        [Description( "Determines if horizontal axis should be drawn or not." )]
        public bool DrawHorizontalAxis
        {
            get { return drawHorizontalAxis; }
            set
            {
                drawHorizontalAxis = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// Determines if vertical axis should be drawn or not.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <see langword="false"/>.</para>
        /// </remarks>
        /// 
        [DefaultValue( false )]
        [Description( "Determines if vertical axis should be drawn or not." )]
        public bool DrawVerticalAxis
        {
            get { return drawVerticalAxis; }
            set
            {
                drawVerticalAxis = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// Determines behaviour of manipulator, when mouse button is released.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>The property controls behaviour of manipulator on releasing mouse button. If
        /// the property is set to <see langword="true"/>, then position of manipulator is reset
        /// to (0, 0), when mouse button is released. Otherwise manipulator stays on the place,
        /// where it was left.</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para>
        /// </remarks>
        /// 
        [DefaultValue( true )]
        [Description( "Determines behaviour of manipulator, when mouse button is released." )]
        public bool ResetPositionOnMouseRelease
        {
            get { return resetPositionOnMouseRelease; }
            set { resetPositionOnMouseRelease = value; }
        }

        /// <summary>
        /// Color used for drawing borders and axis's.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <see cref="Color.Black"/>.</para>
        /// </remarks>
        /// 
        [DefaultValue( typeof( Color ), "Black" )]
        [Description( "Color used for drawing borders and axis's." )]
        public Color BorderColor
        {
            get { return borderPen.Color; }
            set
            {
                borderPen = new Pen( value );
                Invalidate( );
            }
        }

        /// <summary>
        /// Background color used for filling top left quarter of the control.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <see cref="Color.White"/>.</para>
        /// </remarks>
        /// 
        [DefaultValue( typeof( Color ), "White" )]
        [Description( "Background color used for filling top left quarter of the control." )]
        public Color TopLeftBackgroundColor
        {
            get { return topLeftBackgroundBrush.Color; }
            set
            {
                topLeftBackgroundBrush = new SolidBrush( value );
                Invalidate( );
            }
        }

        /// <summary>
        /// Background color used for filling top right quarter of the control.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <see cref="Color.White"/>.</para>
        /// </remarks>
        /// 
        [DefaultValue( typeof( Color ), "White" )]
        [Description( "Background color used for filling top right quarter of the control." )]
        public Color TopRightBackgroundColor
        {
            get { return topRightBackgroundBrush.Color; }
            set
            {
                topRightBackgroundBrush = new SolidBrush( value );
                Invalidate( );
            }
        }

        /// <summary>
        /// Background color used for filling bottom left quarter of the control.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <see cref="Color.LightGray"/>.</para>
        /// </remarks>
        /// 
        [DefaultValue( typeof( Color ), "LightGray" )]
        [Description( "Background color used for filling bottom left quarter of the control." )]
        public Color BottomLeftBackgroundColor
        {
            get { return bottomLeftBackgroundBrush.Color; }
            set
            {
                bottomLeftBackgroundBrush = new SolidBrush( value );
                Invalidate( );
            }
        }

        /// <summary>
        /// Background color used for filling bottom right quarter of the control.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <see cref="Color.LightGray"/>.</para>
        /// </remarks>
        /// 
        [DefaultValue( typeof( Color ), "LightGray" )]
        [Description( "Background color used for filling bottom right quarter of the control." )]
        public Color BottomRightBackgroundColor
        {
            get { return bottomRightBackgroundBrush.Color; }
            set
            {
                bottomRightBackgroundBrush = new SolidBrush( value );
                Invalidate( );
            }
        }

        /// <summary>
        /// Color used for filling manipulator.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <see cref="Color.LightSeaGreen"/>.</para>
        /// </remarks>
        /// 
        [DefaultValue( typeof( Color ), "LightSeaGreen" )]
        [Description( "Color used for filling manipulator." )]
        public Color ManipulatorColor
        {
            get { return manipulatorBrush.Color; }
            set
            {
                manipulatorBrush = new SolidBrush( value );
                Invalidate( );
            }
        }

        /// <summary>
        /// Class, which summarizes arguments of manipulator's position change event.
        /// </summary>
        ///
        /// <remarks><para>Properties of this class allow to get:
        /// <list type="bullet">
        /// <item>X/Y coordinates of manipulator in
        /// <a href="http://en.wikipedia.org/wiki/Cartesian_coordinate_system">Cartesian coordinate system</a>,
        /// where X axis is directed from center of the control to the right and Y axis is directed from
        /// center to the top. Both coordinates are measured in [-1, 1] range.</item>
        /// <item>Theta and R coordinates of manipulator in
        /// <a href="http://en.wikipedia.org/wiki/Polar_coordinate_system">Polar coordinate system</a>.</item>
        /// </list>
        /// </para></remarks>
        ///
        public class PositionEventArgs : EventArgs
        {
            private float x, y;

            /// <summary>
            /// X coordinate of manipulator, [-1, 1].
            /// </summary>
            public float X
            {
                get { return x; }
            }

            /// <summary>
            /// Y coordinate of manipulator, [-1, 1].
            /// </summary>
            public float Y
            {
                get { return y; }
            }

            /// <summary>
            /// Theta coordinate of manipulator in Polar coordinate system, [0, 359].
            /// </summary>
            public float Theta
            {
                get
                {
                    if ( x != 0 )
                    {
                        double t = Math.Atan( y / x );
                        t = t / Math.PI * 180;

                        if ( t < 0 )
                        {
                            t += 180;
                        }
                        if ( y < 0 )
                        {
                            t += 180;
                        }

                        return (float) t;
                    }

                    return ( ( y > 0 ) ? 90 : ( ( y < 0 ) ? 270 : 0 ) );
                }
            }

            /// <summary>
            /// R (radius) coordinate of manipulator in Polar coordinate system, [0, 1].
            /// </summary>
            public float R
            {
                get { return (float) Math.Sqrt( x * x + y * y ); }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="PositionEventArgs"/> class.
            /// </summary>
            /// 
            /// <param name="x">X coordinate of manipulator, [-1, 1].</param>
            /// <param name="y">Y coordinate of manipulator, [-1, 1].</param>
            /// 
            public PositionEventArgs( float x, float y )
            {
                this.x = x;
                this.y = y;
            }
        }

        /// <summary>
        /// Current manipulator's position.
        /// </summary>
        /// 
        /// <remarks><para>The property equals to current manipulator's position. Both X and Y values
        /// are in the [-1, 1] range and represented in
        /// <a href="http://en.wikipedia.org/wiki/Cartesian_coordinate_system">Cartesian coordinate system</a>.</para>
        /// </remarks>
        /// 
        [Browsable( false )]
        public PointF ManipulatorPosition
        {
            get { return new PointF( manipulatatorX, manipulatatorY ); }
            set
            {
                manipulatatorX = Math.Max( -1.0f, Math.Min( 1.0f, value.X ) );
                manipulatatorY = Math.Max( -1.0f, Math.Min( 1.0f, value.Y ) );

                if ( !isSquareLook )
                {
                    // get distance from center
                    double cR = Math.Sqrt( manipulatatorX * manipulatatorX + manipulatatorY * manipulatatorY );

                    // correct point if it is too far away
                    if ( cR > 1 )
                    {
                        double coef = (double) 1 / cR;
                        manipulatatorX = (float) ( coef * manipulatatorX );
                        manipulatatorY = (float) ( coef * manipulatatorY );
                    }
                }
                Invalidate( );
                NotifyClients( );
            }
        }

        /// <summary>
        /// Delegate used for notification about manipulator's position changes.
        /// </summary>
        /// 
        /// <param name="sender">Event sender - object sending the event.</param>
        /// <param name="eventArgs">Event arguments - current manipulator's position.</param>
        /// 
        public delegate void PositionChangedHandler( object sender, PositionEventArgs eventArgs );

        /// <summary>
        /// Event used for notification about manipulator's position changes.
        /// </summary>
        [Description( "Occurs when manipulator's position is changed." )]
        public event PositionChangedHandler PositionChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManipulatorControl"/> class.
        /// </summary>
        public ManipulatorControl( )
        {
            InitializeComponent( );

            // update control style
            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true );
        }

        // Paint the control
        private void ManipulatorControl_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;

            // calculate size of the manipulator's ares
            areaSize = Math.Min( ClientRectangle.Width, ClientRectangle.Height ) - areaMargin * 2;
            areaRadius = areaSize / 2;

            // draw area
            if ( areaSize > 1 )
            {
                if ( isSquareLook )
                {
                    // square looking control
                    int halfAreaSize = areaSize / 2;

                    g.FillRectangle( ( !this.Enabled ) ? disabledBrash : topLeftBackgroundBrush, areaMargin, areaMargin, halfAreaSize, halfAreaSize );
                    g.FillRectangle( ( !this.Enabled ) ? disabledBrash : topRightBackgroundBrush, areaMargin + halfAreaSize, areaMargin, areaSize - halfAreaSize, halfAreaSize );
                    g.FillRectangle( ( !this.Enabled ) ? disabledBrash : bottomLeftBackgroundBrush, areaMargin, areaMargin + halfAreaSize, halfAreaSize, areaSize - halfAreaSize );
                    g.FillRectangle( ( !this.Enabled ) ? disabledBrash : bottomRightBackgroundBrush, areaMargin + halfAreaSize, areaMargin + halfAreaSize,
                        areaSize - halfAreaSize, areaSize - halfAreaSize );

                    g.DrawRectangle( borderPen, areaMargin, areaMargin, areaSize - 1, areaSize - 1 );
                }
                else
                {
                    // round looking control
                    g.FillPie( ( this.Enabled ) ? topRightBackgroundBrush : disabledBrash, areaMargin, areaMargin, areaSize - 1, areaSize - 1, 0, -90 );
                    g.FillPie( ( this.Enabled ) ? topLeftBackgroundBrush : disabledBrash, areaMargin, areaMargin, areaSize - 1, areaSize - 1, -90, -90 );
                    g.FillPie( ( this.Enabled ) ? bottomRightBackgroundBrush : disabledBrash, areaMargin, areaMargin, areaSize - 1, areaSize - 1, 0, 90 );
                    g.FillPie( ( this.Enabled ) ? bottomLeftBackgroundBrush : disabledBrash, areaMargin, areaMargin, areaSize - 1, areaSize - 1, 90, 90 );
                    g.DrawEllipse( borderPen, areaMargin, areaMargin, areaSize - 1, areaSize - 1 );
                }
            }

            // draw axis or not ?
            if ( drawHorizontalAxis )
            {
                g.DrawLine( borderPen, areaMargin, areaMargin + areaRadius, areaMargin + areaSize - 1, areaMargin + areaRadius );
            }
            if ( drawVerticalAxis )
            {
                g.DrawLine( borderPen, areaMargin + areaRadius, areaMargin, areaMargin + areaRadius, areaMargin + areaSize - 1 );
            }

            // calculate manipulator's center point
            int ctrlManipulatorX = (int) (  manipulatatorX * areaRadius ) + areaMargin + areaRadius;
            int ctrlManipulatorY = (int) ( -manipulatatorY * areaRadius ) + areaMargin + areaRadius;

            // draw manipulator
            g.FillEllipse ( ( this.Enabled ) ? manipulatorBrush : disabledBrash,
                ctrlManipulatorX - manipulatorRadius, ctrlManipulatorY - manipulatorRadius,
                manipulatorSize, manipulatorSize );
            g.DrawEllipse( borderPen,
                ctrlManipulatorX - manipulatorRadius, ctrlManipulatorY - manipulatorRadius,
                manipulatorSize, manipulatorSize );
        }

        // On mouse down event
        private void ManipulatorControl_MouseDown( object sender, MouseEventArgs e )
        {
            if ( e.Button == MouseButtons.Left )
            {
                // get click point relatively to manipulation area's center
                int cX = e.X - areaMargin - areaRadius;
                int cY = e.Y - areaMargin - areaRadius;

                if ( isSquareLook )
                {
                    if (
                        ( cX <= areaRadius ) && ( cX >= -areaRadius ) &&
                        ( cY <= areaRadius ) && ( cY >= -areaRadius ) )
                    {
                        tracking = true;
                    }
                }
                else
                {
                    // check if the point is inside of manipulator
                    if ( Math.Sqrt( cX * cX + cY * cY ) <= areaRadius )
                    {
                        tracking = true;
                    }
                }

                if ( tracking )
                {
                    manipulatatorX = (float)  cX / areaRadius;
                    manipulatatorY = (float) -cY / areaRadius;

                    this.Capture = true;
                    this.Cursor = Cursors.Hand;

                    NotifyClients( );
                    // start timer, which is used to notify
                    // about manipulator's position change
                    ticksBeforeNotificiation = -1;
                    timer.Start( );
                }
            }
        }

        // On mouse up event
        private void ManipulatorControl_MouseUp( object sender, MouseEventArgs e )
        {
            if ( ( e.Button == MouseButtons.Left ) && ( tracking ) )
            {
                tracking = false;
                this.Capture = false;
                this.Cursor = Cursors.Arrow;

                timer.Stop( );

                if ( resetPositionOnMouseRelease )
                {
                    manipulatatorX = 0;
                    manipulatatorY = 0;
                }

                NotifyClients( );

                Invalidate( );
            }
        }

        // On mouse move event
        private void ManipulatorControl_MouseMove( object sender, MouseEventArgs e )
        {
            if ( tracking )
            {
                // get mouse point relatively to manipulation area's center
                int cX = e.X - areaMargin - areaRadius;
                int cY = e.Y - areaMargin - areaRadius;

                if ( isSquareLook )
                {
                    cX = Math.Min( areaRadius, Math.Max( -areaRadius, cX ) );
                    cY = Math.Min( areaRadius, Math.Max( -areaRadius, cY ) );
                }
                else
                {
                    // get distance from center
                    int cR = (int) Math.Sqrt( cX * cX + cY * cY );

                    // correct point if it is too far away
                    if ( cR > areaRadius )
                    {
                        double coef = (double) areaRadius / cR;
                        cX = (int) ( coef * cX );
                        cY = (int) ( coef * cY );
                    }
                }

                manipulatatorX = (float)  cX / areaRadius;
                manipulatatorY = (float) -cY / areaRadius;

                Invalidate( );

                // notify user after 5 timer ticks
                ticksBeforeNotificiation = 5;
            }
        }

        // Timer handler, which is used to notify clients about manipulator's changes
        private void timer_Tick( object sender, EventArgs e )
        {
            if ( ticksBeforeNotificiation != -1 )
            {
                // time to notify
                if ( ticksBeforeNotificiation == 0 )
                {
                    // notify users
                    NotifyClients( );
                }

                ticksBeforeNotificiation--;
            }
        }

        // Notify client about changes of manipulator's position
        private void NotifyClients( )
        {
            if ( PositionChanged != null )
            {
                PositionChanged( this, new PositionEventArgs( manipulatatorX, manipulatatorY ) );
            }
        }
    }
}
