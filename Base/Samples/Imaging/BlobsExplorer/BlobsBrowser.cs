// Blobs Browser sample application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Text;
using System.Windows.Forms;

using AForge;
using AForge.Imaging;
using AForge.Math.Geometry;

namespace BlobsExplorer
{
    public delegate void BlobSelectionHandler( object sender, Blob blob );

    public partial class BlobsBrowser : Control
    {
        private Bitmap image = null;
        private int imageWidth, imageHeight;
        private Control parent = null;

        private BlobCounter blobCounter = new BlobCounter( );
        private Blob[] blobs;
        private int selectedBlobID;

        Dictionary<int, List<IntPoint>> leftEdges   = new Dictionary<int, List<IntPoint>>( );
        Dictionary<int, List<IntPoint>> rightEdges  = new Dictionary<int, List<IntPoint>>( );
        Dictionary<int, List<IntPoint>> topEdges    = new Dictionary<int, List<IntPoint>>( );
        Dictionary<int, List<IntPoint>> bottomEdges = new Dictionary<int, List<IntPoint>>( );

        Dictionary<int, List<IntPoint>> hulls = new Dictionary<int, List<IntPoint>>( );
        Dictionary<int, List<IntPoint>> quadrilaterals = new Dictionary<int, List<IntPoint>>( );

        // Event to notify about selected blob
        public event BlobSelectionHandler BlobSelected;

        // Blobs' highlight types enumeration
        public enum HightlightType
        {
            ConvexHull,
            LeftAndRightEdges,
            TopAndBottomEdges,
            Quadrilateral
        }

        private HightlightType highlighting = HightlightType.Quadrilateral;
        private bool showRectangleAroundSelection = false;

        // Blobs' highlight type
        public HightlightType Highlighting
        {
            get { return highlighting; }
            set
            {
                highlighting = value;
                Invalidate( );
            }
        }

        // Show rectangle around selection or not
        public bool ShowRectangleAroundSelection
        {
            get { return showRectangleAroundSelection; }
            set
            {
                showRectangleAroundSelection = value;
                Invalidate( );
            }
        }

        public BlobsBrowser( )
        {
            InitializeComponent( );

            // update control style
            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true );
        }

        // Set image to display by the control
        public int SetImage( Bitmap image )
        {
            leftEdges.Clear( );
            rightEdges.Clear( );
            topEdges.Clear( );
            bottomEdges.Clear( );
            hulls.Clear( );
            quadrilaterals.Clear( );

            selectedBlobID = 0;

            this.image  = AForge.Imaging.Image.Clone( image, PixelFormat.Format24bppRgb );
            imageWidth  = this.image.Width;
            imageHeight = this.image.Height;

            blobCounter.ProcessImage( this.image );
            blobs = blobCounter.GetObjectsInformation( );

            GrahamConvexHull grahamScan = new GrahamConvexHull( );

            foreach ( Blob blob in blobs )
            {
                List<IntPoint> leftEdge   = new List<IntPoint>( );
                List<IntPoint> rightEdge  = new List<IntPoint>( );
                List<IntPoint> topEdge    = new List<IntPoint>( );
                List<IntPoint> bottomEdge = new List<IntPoint>( );

                // collect edge points
                blobCounter.GetBlobsLeftAndRightEdges( blob, out leftEdge, out rightEdge );
                blobCounter.GetBlobsTopAndBottomEdges( blob, out topEdge, out bottomEdge );

                leftEdges.Add( blob.ID, leftEdge );
                rightEdges.Add( blob.ID, rightEdge );
                topEdges.Add( blob.ID, topEdge );
                bottomEdges.Add( blob.ID, bottomEdge );

                // find convex hull
                List<IntPoint> edgePoints = new List<IntPoint>( );
                edgePoints.AddRange( leftEdge );
                edgePoints.AddRange( rightEdge );

                List<IntPoint> hull = grahamScan.FindHull( edgePoints );
                hulls.Add( blob.ID, hull );

                List<IntPoint> quadrilateral = null;

                // find quadrilateral
                if ( hull.Count < 4 )
                {
                    quadrilateral = new List<IntPoint>( hull );
                }
                else
                {
                    quadrilateral = PointsCloud.FindQuadrilateralCorners( hull );
                }
                quadrilaterals.Add( blob.ID, quadrilateral );

                // shift all points for vizualization
                IntPoint shift = new IntPoint( 1, 1 );

                PointsCloud.Shift( leftEdge, shift );
                PointsCloud.Shift( rightEdge, shift );
                PointsCloud.Shift( topEdge, shift );
                PointsCloud.Shift( bottomEdge, shift );
                PointsCloud.Shift( hull, shift );
                PointsCloud.Shift( quadrilateral, shift );
            }

            UpdatePosition( );
            Invalidate( );

            return blobs.Length;
        }

        // Paint the control
        private void BlobsBrowser_Paint( object sender, PaintEventArgs e )
        {
            Graphics  g = e.Graphics;
            Rectangle rect = this.ClientRectangle;

            Pen borderPen = new Pen( Color.FromArgb( 64, 64, 64 ), 1 );
            Pen highlightPen = new Pen( Color.Red );
            Pen highlightPenBold = new Pen( Color.FromArgb( 0, 255, 0 ), 3 );
            Pen rectPen = new Pen( Color.Blue );

            // draw rectangle
            g.DrawRectangle( borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1 );

            if ( image != null )
            {
                g.DrawImage( image, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2 );

                foreach ( Blob blob in blobs )
                {
                    Pen pen = ( blob.ID == selectedBlobID ) ? highlightPenBold : highlightPen;

                    if ( ( showRectangleAroundSelection ) && ( blob.ID == selectedBlobID ) )
                    {
                        g.DrawRectangle( rectPen, blob.Rectangle );
                    }

                    switch ( highlighting )
                    {
                        case HightlightType.ConvexHull:
                            g.DrawPolygon( pen, PointsListToArray( hulls[blob.ID] ) );
                            break;
                        case HightlightType.LeftAndRightEdges:
                            DrawEdge( g, pen, leftEdges[blob.ID] );
                            DrawEdge( g, pen, rightEdges[blob.ID] );
                            break;
                        case HightlightType.TopAndBottomEdges:
                            DrawEdge( g, pen, topEdges[blob.ID] );
                            DrawEdge( g, pen, bottomEdges[blob.ID] );
                            break;
                        case HightlightType.Quadrilateral:
                            g.DrawPolygon( pen, PointsListToArray( quadrilaterals[blob.ID] ) );
                            break;
                    }
                }
            }
            else
            {
                g.FillRectangle( new SolidBrush( Color.FromArgb( 128, 128, 128 ) ),
                    rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2 );
            }
        }

        // Update controls size and position
        private void UpdatePosition( )
        {
            if ( this.Parent != null )
            {
                Rectangle rc = this.Parent.ClientRectangle;
                int width  = 320;
                int height = 240;

                if ( image != null )
                {
                    // get frame size
                    width  = imageWidth;
                    height = imageHeight;
                }

                // update controls size and location
                this.SuspendLayout( );
                this.Location = new System.Drawing.Point( ( rc.Width - width - 2 ) / 2, ( rc.Height - height - 2 ) / 2 );
                this.Size = new Size( width + 2, height + 2 );
                this.ResumeLayout( );
            }
        }

        // Parent of the control has changed
        private void BlobsBrowser_ParentChanged( object sender, EventArgs e )
        {
            if ( parent != null )
            {
                parent.SizeChanged -= new EventHandler( parent_SizeChanged );
            }

            parent = this.Parent;

            // set handler for Size Changed parent's event
            if ( parent != null )
            {
                parent.SizeChanged += new EventHandler( parent_SizeChanged );
            }
        }

        // Parent control has changed its size
        private void parent_SizeChanged( object sender, EventArgs e )
        {
            UpdatePosition( );
        }

        // On mouse moving - update cursor
        private void BlobsBrowser_MouseMove( object sender, MouseEventArgs e )
        {
            int x = e.X - 1;
            int y = e.Y - 1;

            if ( ( image != null ) && ( x >= 0 ) && ( y >= 0 ) &&
                 ( x < imageWidth ) && ( y < imageHeight ) &&
                 ( blobCounter.ObjectLabels[y * imageWidth + x] != 0 ) )
            {
                this.Cursor = Cursors.Hand;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        // On mouse click - notify user if blob was clicked
        private void BlobsBrowser_MouseClick( object sender, MouseEventArgs e )
        {
            int x = e.X - 1;
            int y = e.Y - 1;

            if ( ( image != null ) && ( x >= 0 ) && ( y >= 0 ) &&
                 ( x < imageWidth ) && ( y < imageHeight ) )
            {
                int blobID = blobCounter.ObjectLabels[y * imageWidth + x];

                if ( blobID != 0 )
                {
                    selectedBlobID = blobID;
                    Invalidate( );

                    if ( BlobSelected != null )
                    {
                        for ( int i = 0; i < blobs.Length; i++ )
                        {
                            if ( blobs[i].ID == blobID )
                            {
                                BlobSelected( this, blobs[i] );
                            }
                        }
                    }
                }
            }
        }

        // Convert list of AForge.NET's IntPoint to array of .NET's Point
        private static System.Drawing.Point[] PointsListToArray( List<IntPoint> list )
        {
            System.Drawing.Point[] array = new System.Drawing.Point[list.Count];

            for ( int i = 0, n = list.Count; i < n; i++ )
            {
                array[i] = new System.Drawing.Point( list[i].X, list[i].Y );
            }

            return array;
        }

        // Draw object's edge
        private static void DrawEdge( Graphics g, Pen pen, List<IntPoint> edge )
        {
            System.Drawing.Point[] points = PointsListToArray( edge );

            if ( points.Length > 1 )
            {
                g.DrawLines( pen, points );
            }
            else
            {
                g.DrawLine( pen, points[0], points[0] );
            }
        }
    }
}
