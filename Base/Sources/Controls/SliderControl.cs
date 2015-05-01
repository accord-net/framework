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
    /// Slider control.
    /// </summary>
    ///
    /// <remarks>
    /// <para>The control represents a slider, which can be dragged in the [-1, 1] range.
    /// Default position of the slider is set 0, which corresponds to center of the control.<br />
    /// <img src="img/controls/slider_control.png" width="227" height="56" />
    /// </para>
    /// </remarks>
    ///
    public partial class SliderControl : Control
    {
        // horizontal or vertical configuration
        private bool isHorizontal = true;

        private bool resetPositionOnMouseRelease = true;

        // pens and brushes for drawing
        private Pen borderPen = new Pen( Color.Black );
        private SolidBrush positiveAreaBrush = new SolidBrush( Color.White );
        private SolidBrush negativeAreaBrush = new SolidBrush( Color.LightGray );
        private SolidBrush manipulatorBrush  = new SolidBrush( Color.LightSeaGreen );
        private SolidBrush disabledBrash     = new SolidBrush( Color.FromArgb( 240, 240, 240 ) );

        // manipulator's size
        private const int manipulatorWidth = 11;
        private const int manipulatorHeight = 20;

        // margins
        private int leftMargin;
        private int topMargin;

        // manipulator's position
        private float manipulatatorPosition = 0;

        // tracking information
        private bool tracking = false;

        // number of timer ticks before notifying user (-1 means no notification)
        private int ticksBeforeNotificiation = -1;

        /// <summary>
        /// Determines behaviour of manipulator, when mouse button is released.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>The property controls behaviour of manipulator on releasing mouse button. If
        /// the property is set to <see langword="true"/>, then position of manipulator is reset
        /// to 0, when mouse button is released. Otherwise manipulator stays on the place,
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
        /// Color used for drawing borders.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <see cref="Color.Black"/>.</para>
        /// </remarks>
        /// 
        [DefaultValue( typeof( Color ), "Black" )]
        [Description( "Color used for drawing borders." )]
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
        /// Background color used for filling area corresponding to positive values.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <see cref="Color.White"/>.</para>
        /// </remarks>
        /// 
        [DefaultValue( typeof( Color ), "White" )]
        [Description( "Background color used for filling area corresponding to positive values." )]
        public Color PositiveAreaBrush
        {
            get { return positiveAreaBrush.Color; }
            set
            {
                positiveAreaBrush = new SolidBrush( value );
                Invalidate( );
            }
        }

        /// <summary>
        /// Background color used for filling area corresponding to negative values.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <see cref="Color.LightGray"/>.</para>
        /// </remarks>
        /// 
        [DefaultValue( typeof( Color ), "LightGray" )]
        [Description( "Background color used for filling top right quarter of the control." )]
        public Color NegativeAreaBrush
        {
            get { return negativeAreaBrush.Color; }
            set
            {
                negativeAreaBrush = new SolidBrush( value );
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
        /// Defines if control has horizontal or vertical look.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <see langword="true"/>.</para>
        /// </remarks>
        /// 
        [DefaultValue( true )]
        [Description( "Defines if control has horizontal or vertical look." )]
        public bool IsHorizontal
        {
            get { return isHorizontal; }
            set
            {
                isHorizontal = value;

                if ( value )
                {
                    leftMargin = manipulatorWidth  / 2 + 2;
                    topMargin  = manipulatorHeight / 4;
                }
                else
                {
                    leftMargin = manipulatorHeight / 4;
                    topMargin  = manipulatorWidth  / 2 + 2;
                }

                Invalidate( );
            }
        }

        /// <summary>
        /// Current manipulator's position, [-1, 1].
        /// </summary>
        /// 
        /// <remarks><para>The property equals to current manipulator's position.</para>
        /// </remarks>
        /// 
        [Browsable( false )]
        public float ManipulatorPosition
        {
            get { return manipulatatorPosition; }
            set
            {
                manipulatatorPosition = Math.Max( -1.0f, Math.Min( 1.0f, value ) );
                Invalidate( );
                NotifyClients( );
            }
        }

        /// <summary>
        /// Delegate used for notification about manipulator's position changes.
        /// </summary>
        /// 
        /// <param name="sender">Event sender - object sending the event.</param>
        /// <param name="position">Current position of manipulator.</param>
        /// 
        public delegate void PositionChangedHandler( object sender, float position );

        /// <summary>
        /// Event used for notification about manipulator's position changes.
        /// </summary>
        [Description( "Occurs when manipulator's position is changed." )]
        public event PositionChangedHandler PositionChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="SliderControl"/> class.
        /// </summary>
        public SliderControl( )
        {
            InitializeComponent( );

            // update control style
            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true );

            IsHorizontal = true;
        }

        // Paint the control
        private void TurnControl_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;

            int clientWidth  = ClientRectangle.Width;
            int clientHeight = ClientRectangle.Height;

            if ( isHorizontal )
            {
                // draw area
                g.FillRectangle( ( this.Enabled ) ? negativeAreaBrush : disabledBrash, leftMargin, topMargin,
                   clientWidth / 2 - leftMargin, manipulatorHeight / 2 );
                g.FillRectangle( ( this.Enabled ) ? positiveAreaBrush : disabledBrash, clientWidth / 2, topMargin,
                   clientWidth / 2 - leftMargin, manipulatorHeight / 2 );
                g.DrawRectangle( borderPen, leftMargin, topMargin,
                   clientWidth - 1 - leftMargin * 2, manipulatorHeight / 2 );
                g.DrawLine( borderPen, clientWidth / 2, topMargin, clientWidth / 2, topMargin + manipulatorHeight / 2 );

                // calculate manipulator's center point
                int ctrlManipulatorX = (int) ( manipulatatorPosition * ( clientWidth / 2 - leftMargin ) + clientWidth / 2 );

                // draw manipulator
                g.FillRectangle( ( this.Enabled ) ? manipulatorBrush : disabledBrash, ctrlManipulatorX - manipulatorWidth / 2, 0,
                    manipulatorWidth, manipulatorHeight );
                g.DrawRectangle( borderPen, ctrlManipulatorX - manipulatorWidth / 2, 0,
                    manipulatorWidth, manipulatorHeight );
            }
            else
            {
                // draw area
                g.FillRectangle( ( this.Enabled ) ? positiveAreaBrush : disabledBrash, leftMargin, topMargin,
                    manipulatorHeight / 2, clientHeight / 2 - topMargin );
                g.FillRectangle( ( this.Enabled ) ? negativeAreaBrush : disabledBrash, leftMargin, clientHeight / 2,
                   manipulatorHeight / 2, clientHeight / 2 - topMargin );
                g.DrawRectangle( borderPen, leftMargin, topMargin,
                   manipulatorHeight / 2, clientHeight - 1 - topMargin * 2 );
                g.DrawLine( borderPen, leftMargin, clientHeight / 2, leftMargin + manipulatorHeight / 2, clientHeight / 2 );


                // calculate manipulator's center point
                int ctrlManipulatorY = (int) ( -manipulatatorPosition * ( clientHeight / 2 - topMargin ) + clientHeight / 2 );

                // draw manipulator
                g.FillRectangle( ( this.Enabled ) ? manipulatorBrush : disabledBrash, 0, ctrlManipulatorY - manipulatorWidth / 2,
                    manipulatorHeight, manipulatorWidth );
                g.DrawRectangle( borderPen, 0, ctrlManipulatorY - manipulatorWidth / 2,
                    manipulatorHeight, manipulatorWidth );
            }
        }

        // On mouse down event
        private void TurnControl_MouseDown( object sender, MouseEventArgs e )
        {
            if ( e.Button == MouseButtons.Left )
            {
                if ( isHorizontal )
                {
                    if (
                        ( e.X >= leftMargin ) &&
                        ( e.X < ClientRectangle.Width - leftMargin ) &&
                        ( e.Y >= topMargin ) &&
                        ( e.Y < ClientRectangle.Height - topMargin ) )
                    {
                        int cx = e.X - ClientRectangle.Width / 2;
                        manipulatatorPosition = (float) cx / ( ClientRectangle.Width / 2 - leftMargin );
                        tracking = true;
                    }
                }
                else
                {
                    if (
                        ( e.X >= leftMargin ) &&
                        ( e.X < ClientRectangle.Width - leftMargin ) &&
                        ( e.Y >= topMargin ) &&
                        ( e.Y < ClientRectangle.Height - topMargin ) )
                    {
                        int cy = ClientRectangle.Height / 2 - e.Y;
                        manipulatatorPosition = (float) cy / ( ClientRectangle.Height / 2 - topMargin );
                        tracking = true;
                    }
                }

                if ( tracking )
                {
                    this.Capture = true;
                    this.Cursor = Cursors.Hand;

                    NotifyClients( );
                    // start time, which is used to notify
                    // about manipulator's position change
                    ticksBeforeNotificiation = -1;
                    timer.Start( );
                }
            }
        }

        // On mouse up event
        private void TurnControl_MouseUp( object sender, MouseEventArgs e )
        {
            if ( ( e.Button == MouseButtons.Left ) && ( tracking ) )
            {
                tracking = false;
                this.Capture = false;
                this.Cursor = Cursors.Arrow;

                if ( resetPositionOnMouseRelease )
                {
                    manipulatatorPosition = 0;
                }

                Invalidate( );
                timer.Stop( );

                NotifyClients( );
            }
        }

        // On mouse move event
        private void TurnControl_MouseMove( object sender, MouseEventArgs e )
        {
            if ( tracking )
            {
                if ( isHorizontal )
                {
                    int cx = e.X - ClientRectangle.Width / 2;
                    manipulatatorPosition = (float) cx / ( ClientRectangle.Width / 2 - leftMargin );
                }
                else
                {
                    int cy = ClientRectangle.Height / 2 - e.Y;
                    manipulatatorPosition = (float) cy / ( ClientRectangle.Height / 2 - topMargin );
                }

                manipulatatorPosition = Math.Max( Math.Min( 1, manipulatatorPosition ), -1 );
                Invalidate( );

                // notify user after 10 timer ticks
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
                PositionChanged( this, manipulatatorPosition );
            }
        }

    }
}
