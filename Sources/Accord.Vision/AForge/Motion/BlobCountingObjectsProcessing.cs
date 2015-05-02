// AForge Vision Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Vision.Motion
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    using AForge.Imaging;

    /// <summary>
    /// Motion processing algorithm, which counts separate moving objects and highlights them.
    /// </summary>
    /// 
    /// <remarks><para>The aim of this motion processing algorithm is to count separate objects
    /// in the motion frame, which is provided by <see cref="IMotionDetector">motion detection algorithm</see>.
    /// In the case if <see cref="HighlightMotionRegions"/> property is set to <see langword="true"/>,
    /// found objects are also highlighted on the original video frame. The algorithm
    /// counts and highlights only those objects, which size satisfies <see cref="MinObjectsWidth"/>
    /// and <see cref="MinObjectsHeight"/> properties.</para>
    /// 
    /// <para><note>The motion processing algorithm is supposed to be used only with motion detection
    /// algorithms, which are based on finding difference with background frame
    /// (see <see cref="SimpleBackgroundModelingDetector"/> and <see cref="CustomFrameDifferenceDetector"/>
    /// as simple implementations) and allow extract moving objects clearly.</note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create instance of motion detection algorithm
    /// IMotionDetector motionDetector = new ... ;
    /// // create instance of motion processing algorithm
    /// BlobCountingObjectsProcessing motionProcessing = new BlobCountingObjectsProcessing( );
    /// // create motion detector
    /// MotionDetector detector = new MotionDetector( motionDetector, motionProcessing );
    /// 
    /// // continuously feed video frames to motion detector
    /// while ( ... )
    /// {
    ///     // process new video frame and check motion level
    ///     if ( detector.ProcessFrame( videoFrame ) > 0.02 )
    ///     {
    ///         // check number of detected objects
    ///         if ( motionProcessing.ObjectsCount > 1 )
    ///         {
    ///             // ...
    ///         }
    ///     }
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="MotionDetector"/>
    /// <seealso cref="IMotionDetector"/>
    /// 
    public class BlobCountingObjectsProcessing : IMotionProcessing
    {
        // blob counter to detect separate blobs
        private BlobCounter blobCounter = new BlobCounter( );
        // color used for blobs highlighting
        private Color highlightColor = Color.Red;
        // highlight motion regions or not
        private bool highlightMotionRegions = true;

        /// <summary>
        /// Highlight motion regions or not.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies if detected moving objects should be highlighted
        /// with rectangle or not.</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para>
        ///
        /// <para><note>Turning the value on leads to additional processing time of video frame.</note></para>
        /// </remarks>
        /// 
        public bool HighlightMotionRegions
        {
            get { return highlightMotionRegions; }
            set { highlightMotionRegions = value; }
        }

        /// <summary>
        /// Color used to highlight motion regions.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <b>red</b> color.</para>
        /// </remarks>
        /// 
        public Color HighlightColor
        {
            get { return highlightColor; }
            set { highlightColor = value; }
        }

        /// <summary>
        /// Minimum width of acceptable object.
        /// </summary>
        /// 
        /// <remarks><para>The property sets minimum width of an object to count and highlight. If
        /// objects have smaller width, they are not counted and are not highlighted.</para>
        /// 
        /// <para>Default value is set to <b>10</b>.</para>
        /// </remarks>
        /// 
        public int MinObjectsWidth
        {
            get { return blobCounter.MinWidth; }
            set
            {
                lock ( blobCounter )
                {
                    blobCounter.MinWidth = value;
                }
            }
        }

        /// <summary>
        /// Minimum height of acceptable object.
        /// </summary>
        /// 
        /// <remarks><para>The property sets minimum height of an object to count and highlight. If
        /// objects have smaller height, they are not counted and are not highlighted.</para>
        /// 
        /// <para>Default value is set to <b>10</b>.</para>
        /// </remarks>
        /// 
        public int MinObjectsHeight
        {
            get { return blobCounter.MinHeight; }
            set
            {
                lock ( blobCounter )
                {
                    blobCounter.MinHeight = value;
                }
            }
        }

        /// <summary>
        /// Number of detected objects.
        /// </summary>
        /// 
        /// <remarks><para>The property provides number of moving objects detected by
        /// the last call of <see cref="ProcessFrame"/> method.</para></remarks>
        /// 
        public int ObjectsCount
        {
            get
            {
                lock ( blobCounter )
                {
                    return blobCounter.ObjectsCount;
                }
            }
        }

        /// <summary>
        /// Rectangles of moving objects.
        /// </summary>
        /// 
        /// <remarks><para>The property provides array of moving objects' rectangles
        /// detected by the last call of <see cref="ProcessFrame"/> method.</para></remarks>
        /// 
        public Rectangle[] ObjectRectangles
        {
            get
            {
                lock ( blobCounter )
                {
                    return blobCounter.GetObjectsRectangles( );
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobCountingObjectsProcessing"/> class.
        /// </summary>
        /// 
        public BlobCountingObjectsProcessing( ) : this( 10, 10 ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobCountingObjectsProcessing"/> class.
        /// </summary>
        /// 
        /// <param name="highlightMotionRegions">Highlight motion regions or not (see <see cref="HighlightMotionRegions"/> property).</param>
        /// 
        public BlobCountingObjectsProcessing( bool highlightMotionRegions ) : this( 10, 10, highlightMotionRegions ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobCountingObjectsProcessing"/> class.
        /// </summary>
        /// 
        /// <param name="minWidth">Minimum width of acceptable object (see <see cref="MinObjectsWidth"/> property).</param>
        /// <param name="minHeight">Minimum height of acceptable object (see <see cref="MinObjectsHeight"/> property).</param>
        /// 
        public BlobCountingObjectsProcessing( int minWidth, int minHeight ) :
            this( minWidth, minHeight, Color.Red ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobCountingObjectsProcessing"/> class.
        /// </summary>
        /// 
        /// <param name="minWidth">Minimum width of acceptable object (see <see cref="MinObjectsWidth"/> property).</param>
        /// <param name="minHeight">Minimum height of acceptable object (see <see cref="MinObjectsHeight"/> property).</param>
        /// <param name="highlightColor">Color used to highlight motion regions.</param>
        /// 
        public BlobCountingObjectsProcessing( int minWidth, int minHeight, Color highlightColor )
        {
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight   = minHeight;
            blobCounter.MinWidth    = minWidth;

            this.highlightColor     = highlightColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobCountingObjectsProcessing"/> class.
        /// </summary>
        /// 
        /// <param name="minWidth">Minimum width of acceptable object (see <see cref="MinObjectsWidth"/> property).</param>
        /// <param name="minHeight">Minimum height of acceptable object (see <see cref="MinObjectsHeight"/> property).</param>
        /// <param name="highlightMotionRegions">Highlight motion regions or not (see <see cref="HighlightMotionRegions"/> property).</param>
        /// 
        public BlobCountingObjectsProcessing( int minWidth, int minHeight, bool highlightMotionRegions )
            : this( minWidth, minHeight )
        {
            this.highlightMotionRegions = highlightMotionRegions;
        }

        /// <summary>
        /// Process video and motion frames doing further post processing after
        /// performed motion detection.
        /// </summary>
        /// 
        /// <param name="videoFrame">Original video frame.</param>
        /// <param name="motionFrame">Motion frame provided by motion detection
        /// algorithm (see <see cref="IMotionDetector"/>).</param>
        /// 
        /// <remarks><para>Processes provided motion frame and counts number of separate
        /// objects, which size satisfies <see cref="MinObjectsWidth"/> and <see cref="MinObjectsHeight"/>
        /// properties. In the case if <see cref="HighlightMotionRegions"/> property is
        /// set to <see langword="true"/>, the found object are also highlighted on the
        /// original video frame.
        /// </para></remarks>
        /// 
        /// <exception cref="InvalidImagePropertiesException">Motion frame is not 8 bpp image, but it must be so.</exception>
        /// <exception cref="UnsupportedImageFormatException">Video frame must be 8 bpp grayscale image or 24/32 bpp color image.</exception>
        /// 
        public unsafe void ProcessFrame( UnmanagedImage videoFrame, UnmanagedImage motionFrame )
        {
            if ( motionFrame.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                throw new InvalidImagePropertiesException( "Motion frame must be 8 bpp image." );
            }

            if ( ( videoFrame.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                 ( videoFrame.PixelFormat != PixelFormat.Format24bppRgb ) &&
                 ( videoFrame.PixelFormat != PixelFormat.Format32bppRgb ) &&
                 ( videoFrame.PixelFormat != PixelFormat.Format32bppArgb ) )
            {
                throw new UnsupportedImageFormatException( "Video frame must be 8 bpp grayscale image or 24/32 bpp color image." );
            } 

            int width  = videoFrame.Width;
            int height = videoFrame.Height;

            if ( ( motionFrame.Width != width ) || ( motionFrame.Height != height ) )
                return;

            lock ( blobCounter )
            {
                blobCounter.ProcessImage( motionFrame );
            }

            if ( highlightMotionRegions )
            {
                // highlight each moving object
                Rectangle[] rects = blobCounter.GetObjectsRectangles( );

                foreach ( Rectangle rect in rects )
                {
                    Drawing.Rectangle( videoFrame, rect, highlightColor );
                }
            }
        }

        /// <summary>
        /// Reset internal state of motion processing algorithm.
        /// </summary>
        /// 
        /// <remarks><para>The method allows to reset internal state of motion processing
        /// algorithm and prepare it for processing of next video stream or to restart
        /// the algorithm.</para></remarks>
        ///
        public void Reset( )
        {
        }
    }
}
