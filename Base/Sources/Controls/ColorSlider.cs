// AForge Controls Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;

namespace AForge.Controls
{
    using Point = System.Drawing.Point;

    /// <summary>
    /// Color slider control.
    /// </summary>
    /// 
    /// <remarks><para>The control represent a color slider, which allows selecting
    /// one or two values in the [0, 255] range. The application of this control
    /// includes mostly areas of image processing and computer vision, where it is required
    /// to select color threshold or ranges for different type of color filtering.</para>
    /// 
    /// <para>Depending on the control's <see cref="Type"/>, it has different look and may suite
    /// different tasks. See documentation to <see cref="ColorSliderType"/> for information
    /// about available type and possible control's looks.</para>
    /// </remarks>
    /// 
    public class ColorSlider : System.Windows.Forms.Control
    {
        private Pen blackPen = new Pen( Color.Black, 1 );
        private Color startColor = Color.Black;
        private Color endColor = Color.White;
        private Color fillColor = Color.Black;
        private ColorSliderType type = ColorSliderType.Gradient;
        private bool doubleArrow = true;
        private Bitmap arrow;
        private int min = 0, max = 255;
        private int width = 256, height = 10;
        private int trackMode = 0;
        private int dx;

        /// <summary>
        /// An event, to notify about changes of <see cref="Min"/> or <see cref="Max"/> properties.
        /// </summary>
        /// 
        /// <remarks><para>The event is fired after changes of <see cref="Min"/> or <see cref="Max"/> property,
        /// which is caused by user dragging the corresponding control’s arrow (slider).</para>
        /// </remarks>
        /// 
        public event EventHandler ValuesChanged;

        /// <summary>
        /// Enumeration of color slider types.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>The <see cref="ColorSliderType.Gradient"/> slider's type supposes the control's
        /// background filled with gradient startting from <see cref="StartColor"/> color and ending
        /// with <see cref="EndColor"/> color. The <see cref="FillColor"/> color does not have
        /// impact on control's look.</para>
        /// 
        /// <para>This type allows as one-arrow, as two-arrows control.</para>
        /// 
        /// <para><b>Sample control's look:</b></para>
        /// <img src="img/controls/slider_gradient.png" width="258" height="17" />    
        /// 
        /// <para>The <see cref="ColorSliderType.InnerGradient"/> slider's type supposes the control's
        /// background filled with gradient startting from <see cref="StartColor"/> color and ending
        /// with <see cref="EndColor"/> color. In addition the areas, which are outside of
        /// [<see cref="Min"/>, <see cref="Max"/>] range, are filled with <see cref="FillColor"/> color.</para>
        /// 
        /// <para>This type allows only two-arrows control.</para>
        /// 
        /// <para><b>Sample control's look:</b></para>
        /// <img src="img/controls/slider_inner_gradient.png" width="258" height="17" />
        /// 
        /// <para>The <see cref="ColorSliderType.OuterGradient"/> slider's type supposes the
        /// control's background filled with gradient startting from <see cref="StartColor"/> color
        /// and ending with <see cref="EndColor"/> color. In addition the area, which is inside of
        /// [<see cref="Min"/>, <see cref="Max"/>] range, is filled with <see cref="FillColor"/> color.</para>
        /// 
        /// <para>This type allows only two-arrows control.</para>
        /// 
        /// <para><b>Sample control's look:</b></para>
        /// <img src="img/controls/slider_outer_gradient.png" width="258" height="17" />
        /// 
        /// <para>The <see cref="ColorSliderType.Threshold"/> slider's type supposes filling areas
        /// outside of [<see cref="Min"/>, <see cref="Max"/>] range with <see cref="StartColor"/> and
        /// inside the range with <see cref="EndColor"/>. The <see cref="FillColor"/> color does not
        /// have impact on control's look.</para>
        /// 
        /// <para>This type allows as one-arrow, as two-arrows control.</para>
        /// 
        /// <para><b>Sample control's look:</b></para>
        /// <img src="img/controls/slider_threshold.png" width="258" height="17" />
        /// </remarks>
        ///
        public enum ColorSliderType
        {
            /// <summary>
            /// Gradient color slider type.
            /// </summary>
            Gradient,

            /// <summary>
            /// Inner gradient color slider type.
            /// </summary>
            InnerGradient,

            /// <summary>
            /// Outer gradient color slider type.
            /// </summary>
            OuterGradient,

            /// <summary>
            /// Threshold color slider type.
            /// </summary>
            Threshold
        }

        /// <summary>
        /// Start color for gradient filling.
        /// </summary>
        ///
        /// <remarks>See documentation to <see cref="ColorSliderType"/> enumeration for information about
        /// the usage of this property.</remarks>
        ///
        [DefaultValue( typeof( Color ), "Black" )]
        public Color StartColor
        {
            get { return startColor; }
            set
            {
                startColor = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// End color for gradient filling.
        /// </summary>
        ///
        /// <remarks>See documentation to <see cref="ColorSliderType"/> enumeration for information about
        /// the usage of this property.</remarks>
        ///
        [DefaultValue( typeof( Color ), "White" )]
        public Color EndColor
        {
            get { return endColor; }
            set
            {
                endColor = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// Color to fill control's background in filtered zones.
        /// </summary>
        ///
        /// <remarks>See documentation to <see cref="ColorSliderType"/> enumeration for information about
        /// the usage of this property.</remarks>
        ///
        [DefaultValue( typeof( Color ), "Black" )]
        public Color FillColor
        {
            get { return fillColor; }
            set
            {
                fillColor = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// Specifies control's type.
        /// </summary>
        /// 
        /// <remarks>See documentation to <see cref="ColorSliderType"/> enumeration for information about
        /// the usage of this property.</remarks>
        ///
        [DefaultValue( ColorSliderType.Gradient )]
        public ColorSliderType Type
        {
            get { return type; }
            set
            {
                type = value;
                if ( ( type != ColorSliderType.Gradient ) && ( type != ColorSliderType.Threshold ) )
                    DoubleArrow = true;
                Invalidate( );
            }
        }

        /// <summary>
        /// Minimum selected value, [0, 255].
        /// </summary>
        /// 
        [DefaultValue( 0 )]
        public int Min
        {
            get { return min; }
            set
            {
                min = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// Maximum selected value, [0, 255].
        /// </summary>
        /// 
        [DefaultValue( 255 )]
        public int Max
        {
            get { return max; }
            set
            {
                max = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// Single or Double arrow slider control.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies if the slider has one or two selection arrows (sliders).</para>
        /// 
        /// <para>The single arrow allows only to specify one value, which is set by <see cref="Min"/>
        /// property. The single arrow slider is useful for applications, where it is required to select
        /// color threshold, for example.</para>
        /// 
        /// <para>The double arrow allows to specify two values, which are set by <see cref="Min"/>
        /// and <see cref="Max"/> properties. The double arrow slider is useful for applications, where it is
        /// required to select filtering color range, for example.</para>
        /// </remarks>
        /// 
        [DefaultValue( true )]
        public bool DoubleArrow
        {
            get { return doubleArrow; }
            set
            {
                doubleArrow = value;
                if ( ( !doubleArrow ) && ( type != ColorSliderType.Threshold ) )
                {
                    Type = ColorSliderType.Gradient;
                }
                Invalidate( );
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ColorSlider"/> class.
        /// </summary>
        /// 
        public ColorSlider( )
        {
            InitializeComponent( );

            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true );

            // load arrow bitmap
            Assembly assembly = this.GetType( ).Assembly;
            arrow = new Bitmap( assembly.GetManifestResourceStream( "AForge.Controls.Resources.arrow.bmp" ) );
            arrow.MakeTransparent( Color.FromArgb( 255, 255, 255 ) );
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        /// 
        /// <param name="disposing">Specifies if disposing was invoked by user's code.</param>
        /// 
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                blackPen.Dispose( );
                arrow.Dispose( );
            }
            base.Dispose( disposing );
        }

        // Init component
        private void InitializeComponent( )
        {
            this.SuspendLayout( );
            // 
            // ColorSlider
            // 
            this.Paint += new System.Windows.Forms.PaintEventHandler( this.ColorSlider_Paint );
            this.MouseMove += new System.Windows.Forms.MouseEventHandler( this.ColorSlider_MouseMove );
            this.MouseDown += new System.Windows.Forms.MouseEventHandler( this.ColorSlider_MouseDown );
            this.MouseUp += new System.Windows.Forms.MouseEventHandler( this.ColorSlider_MouseUp );
            this.ResumeLayout( false );

        }

        // Paint control
        private void ColorSlider_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;
            Rectangle rc = this.ClientRectangle;
            Brush brush;
            int x = ( rc.Right - width ) / 2;
            int y = 2;

            // draw rectangle around the control
            g.DrawRectangle( blackPen, x - 1, y - 1, width + 1, height + 1 );

            switch ( type )
            {
                case ColorSliderType.Gradient:
                case ColorSliderType.InnerGradient:
                case ColorSliderType.OuterGradient:

                    // create gradient brush
                    brush = new LinearGradientBrush( new Point( x, 0 ), new Point( x + width, 0 ), startColor, endColor );
                    g.FillRectangle( brush, x, y, width, height );
                    brush.Dispose( );

                    // check type
                    if ( type == ColorSliderType.InnerGradient )
                    {
                        // inner gradient
                        brush = new SolidBrush( fillColor );

                        if ( min != 0 )
                        {
                            g.FillRectangle( brush, x, y, min, height );
                        }
                        if ( max != 255 )
                        {
                            g.FillRectangle( brush, x + max + 1, y, 255 - max, height );
                        }
                        brush.Dispose( );
                    }
                    else if ( type == ColorSliderType.OuterGradient )
                    {
                        // outer gradient
                        brush = new SolidBrush( fillColor );
                        // fill space between min & max with color 3
                        g.FillRectangle( brush, x + min, y, max - min + 1, height );
                        brush.Dispose( );
                    }
                    break;
                case ColorSliderType.Threshold:
                    // 1 - fill with color 1
                    brush = new SolidBrush( startColor );
                    g.FillRectangle( brush, x, y, width, height );
                    brush.Dispose( );
                    // 2 - fill space between min & max with color 2
                    brush = new SolidBrush( endColor );
                    g.FillRectangle( brush, x + min, y, max - min + 1, height );
                    brush.Dispose( );
                    break;
            }


            // draw arrows
            x -= 4;
            y += 1 + height;

            g.DrawImage( arrow, x + min, y, 9, 6 );
            if ( doubleArrow )
                g.DrawImage( arrow, x + max, y, 9, 6 );
        }

        // On mouse down
        private void ColorSlider_MouseDown( object sender, System.Windows.Forms.MouseEventArgs e )
        {
            int x = ( ClientRectangle.Right - width ) / 2 - 4;
            int y = 3 + height;

            // check Y coordinate
            if ( ( e.Y >= y ) && ( e.Y < y + 6 ) )
            {
                // check X coordinate
                if ( ( e.X >= x + min ) && ( e.X < x + min + 9 ) )
                {
                    // left arrow
                    trackMode = 1;
                    dx = e.X - min;
                }
                if ( ( doubleArrow ) && ( e.X >= x + max ) && ( e.X < x + max + 9 ) )
                {
                    // right arrow
                    trackMode = 2;
                    dx = e.X - max;
                }

                if ( trackMode != 0 )
                    this.Capture = true;
            }
        }

        // On mouse up
        private void ColorSlider_MouseUp( object sender, System.Windows.Forms.MouseEventArgs e )
        {
            if ( trackMode != 0 )
            {
                // release capture
                this.Capture = false;
                trackMode = 0;

                // notify client
                if ( ValuesChanged != null )
                    ValuesChanged( this, new EventArgs( ) );
            }
        }

        // On mouse move
        private void ColorSlider_MouseMove( object sender, System.Windows.Forms.MouseEventArgs e )
        {
            if ( trackMode != 0 )
            {
                if ( trackMode == 1 )
                {
                    // left arrow tracking
                    min = e.X - dx;
                    min = Math.Max( min, 0 );
                    min = Math.Min( min, max );
                }
                if ( trackMode == 2 )
                {
                    // right arrow tracking
                    max = e.X - dx;
                    max = Math.Max( max, min );
                    max = Math.Min( max, 255 );
                }

                // repaint control
                Invalidate( );
            }
        }
    }
}
