// Motion Detection sample application
// AForge.NET Framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MotionDetectorSample
{
    // Drawing modes
    public enum DrawingMode
    {
        None,
        Rectangular
    }

    // Delegate to notify about new rectangle
    public delegate void NewRectangleHandler( object sender, Rectangle rect );

    // Control to define motion regions
    public partial class DefineRegionsControl : System.Windows.Forms.Control
    {
        // border's color
        private Color borderColor = Color.Black;
        // backgound's color
        private Color backColor = Color.FromArgb( 96, 96, 96 );
        // selection rectangle's color
        private Color selectionColor = Color.Yellow;
        // rectangles' color
        private Color rectsColor = Color.FromArgb( 0, 255, 0 );
        // backgroun image
        private Bitmap backImage = null;

        // collection of rectangles
        private List<Rectangle> rectangles = new List<Rectangle>( );

        private DrawingMode drawingMode = DrawingMode.None;
        private bool dragging = false;

        private Point startPoint;
        private Point endPoint;

        // Background image
        public new Bitmap BackgroundImage
        {
            get { return backImage; }
            set { backImage = value; }
        }

        // Drawing mode
        public DrawingMode DrawingMode
        {
            get { return drawingMode; }
            set
            {
                drawingMode = value;

                this.Cursor = ( drawingMode == DrawingMode.None ) ? Cursors.Default : Cursors.Cross;
            }
        }

        // Rectangles array
        public Rectangle[] Rectangles
        {
            get
            {
                Rectangle[] rects = new Rectangle[rectangles.Count];
                rectangles.CopyTo( rects );
                return rects;
            }
            set
            {
                rectangles.Clear( );

                if ( value != null )
                {
                    rectangles.AddRange( value );
                }
                Invalidate( );
            }
        }

        // Event to notify about new rectangle
        public event NewRectangleHandler OnNewRectangle;
        
        // Class constructor
        public DefineRegionsControl( )
        {
            InitializeComponent( );

            // update style of the control
            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer |
                ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true );
        }

        // Remove all regions
        public void RemoveAllRegions( )
        {
            rectangles.RemoveRange( 0, rectangles.Count );
            this.Invalidate( );
        }

		// Paint the control
        protected override void OnPaint( PaintEventArgs pe )
        {
            Graphics  g = pe.Graphics;
            Rectangle rect = this.ClientRectangle;

            // draw rectangle
            using ( Pen pen = new Pen( borderColor, 1 ) )
            {
                g.DrawRectangle( pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1 );
            }

            // draw background
            if ( backImage != null )
            {
                g.DrawImage( backImage, 1, 1, rect.Width - 2, rect.Height - 2 );
            }
            else
            {
                using ( Brush backBrush = new SolidBrush( backColor ) )
                {
                    g.FillRectangle( backBrush, 1, 1, rect.Width - 2, rect.Height - 2 );
                }
            }

            // draw rectangles
            using ( Pen pen = new Pen( rectsColor, 1 ) )
            {
                foreach ( Rectangle r in rectangles )
                {
                    g.DrawRectangle( pen, r.X + 1, r.Y + 1, r.Width - 1, r.Height - 1 );
                }
            }

            base.OnPaint( pe );
        }

        // On mouse down
        protected override void OnMouseDown( MouseEventArgs e )
        {
            // check drawing mode
            if ( drawingMode == DrawingMode.Rectangular )
            {
                // check button type
                if ( e.Button == MouseButtons.Left )
                {
                    // switch to dragging mode
                    dragging = true;
                    this.Capture = true;

                    // set initial starting and ending points
                    startPoint.X = endPoint.X = e.X;
                    startPoint.Y = endPoint.Y = e.Y;
                    // draw selection rectangle
                    ControlPaint.DrawReversibleFrame( new Rectangle( e.X, e.Y, 1, 1 ), Color.Green, FrameStyle.Dashed );
                }
                else if ( e.Button == MouseButtons.Right )
                {
                }
            }
        }

        // On mouse up
        protected override void OnMouseUp( MouseEventArgs e )
        {
            // check drawing and dragging mode
            if ( ( drawingMode == DrawingMode.Rectangular ) && ( dragging == true ) )
            {
                // stop dragging mode
                dragging = false;
                this.Capture = false;

                // reset drawing mode
                drawingMode = DrawingMode.None;
                this.Cursor = Cursors.Default;

                // erase selection rectangle
                DrawSelectionRectangle( );

                // get normalized points
                NormalizePoints( ref startPoint, ref endPoint );

                // check that points are inside of the control
                CheckPointsInClient( ref startPoint );
                CheckPointsInClient( ref endPoint );

                // add rectangle to collection
                Rectangle rect = new Rectangle( startPoint.X - 1, startPoint.Y - 1, endPoint.X - startPoint.X + 1, endPoint.Y - startPoint.Y + 1 );
                rectangles.Add( rect );

                // notify clients about new available rectangle
                if ( OnNewRectangle != null )
                {
                    OnNewRectangle( this, rect );
                }

                // redraw the control
                this.Invalidate( );
            }
        }

        // On mouse move
        protected override void OnMouseMove( MouseEventArgs e )
        {
            if ( dragging == true )
            {
                // erase old rectangle
                DrawSelectionRectangle( );

                endPoint.X = e.X;
                endPoint.Y = e.Y;

                // draw new rectangle
                DrawSelectionRectangle( );
            }
        }

        // Draw reversable selection rectangle
        private void DrawSelectionRectangle( )
        {
            Point start = startPoint;
            Point end   = endPoint;

            // do normalization
            NormalizePoints( ref start, ref end );

            // check that points are inside of the control
            CheckPointsInClient( ref start );
            CheckPointsInClient( ref end );

            // convert client coordinates to screen coordinates
            Point screenStartPoint = this.PointToScreen( start);
            Point screenEndPoint   = this.PointToScreen( end );

            // draw rectangle
            ControlPaint.DrawReversibleFrame(
                new Rectangle(
                    screenStartPoint.X, screenStartPoint.Y,
                    screenEndPoint.X - screenStartPoint.X + 1, screenEndPoint.Y - screenStartPoint.Y + 1 ),
                selectionColor, FrameStyle.Dashed );
        }

        // Normalize points, so the first point will keep smaller coordinates
        private void NormalizePoints( ref Point point1, ref Point point2 )
        {
            Point t1 = point1;
            Point t2 = point2;

            point1.X = Math.Min( t1.X, t2.X );
            point1.Y = Math.Min( t1.Y, t2.Y );
            point2.X = Math.Max( t1.X, t2.X );
            point2.Y = Math.Max( t1.Y, t2.Y );
        }

        // Ensure the point is in client area
        private void CheckPointsInClient( ref Point point )
        {
            if ( point.X < 1 )
            {
                point.X = 1;
            }
            if ( point.Y < 1 )
            {
                point.Y = 1;
            }
            if ( point.X >= ClientRectangle.Width - 1 )
            {
                point.X = ClientRectangle.Width - 2;
            }
            if ( point.Y >= ClientRectangle.Height - 1 )
            {
                point.Y = ClientRectangle.Height - 2;
            }
        }
    }
}
