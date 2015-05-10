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
    using System.Drawing.Imaging;
    
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    /// Motion detector based on two continues frames difference.
    /// </summary>
    /// 
    /// <remarks><para>The class implements the simplest motion detection algorithm, which is
    /// based on difference of two continues frames. The <see cref="MotionFrame">difference frame</see>
    /// is thresholded and the <see cref="MotionLevel">amount of difference pixels</see> is calculated.
    /// To suppress stand-alone noisy pixels erosion morphological operator may be applied, which
    /// is controlled by <see cref="SuppressNoise"/> property.</para>
    /// 
    /// <para>Although the class may be used on its own to perform motion detection, it is preferred
    /// to use it in conjunction with <see cref="MotionDetector"/> class, which provides additional
    /// features and allows to use moton post processing algorithms.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create motion detector
    /// MotionDetector detector = new MotionDetector(
    ///     new TwoFramesDifferenceDetector( ),
    ///     new MotionAreaHighlighting( ) );
    /// 
    /// // continuously feed video frames to motion detector
    /// while ( ... )
    /// {
    ///     // process new video frame and check motion level
    ///     if ( detector.ProcessFrame( videoFrame ) > 0.02 )
    ///     {
    ///         // ring alarm or do somethng else
    ///     }
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="MotionDetector"/>
    /// 
    public class TwoFramesDifferenceDetector : IMotionDetector
    {
        // frame's dimension
        private int width;
        private int height;
        private int frameSize;

        // previous frame of video stream
        private UnmanagedImage previousFrame;
        // current frame of video sream
        private UnmanagedImage motionFrame;
        // temporary buffer used for suppressing noise
        private UnmanagedImage tempFrame;
        // number of pixels changed in the new frame of video stream
        private int pixelsChanged;

        // suppress noise
        private bool suppressNoise = true;

        // threshold values
        private int differenceThreshold    =  15;
        private int differenceThresholdNeg = -15;

        // binary erosion filter
        private BinaryErosion3x3 erosionFilter = new BinaryErosion3x3( );

        // dummy object to lock for synchronization
        private object sync = new object( );

        /// <summary>
        /// Difference threshold value, [1, 255].
        /// </summary>
        /// 
        /// <remarks><para>The value specifies the amount off difference between pixels, which is treated
        /// as motion pixel.</para>
        /// 
        /// <para>Default value is set to <b>15</b>.</para>
        /// </remarks>
        /// 
        public int DifferenceThreshold
        {
            get { return differenceThreshold; }
            set
            {
                lock ( sync )
                {
                    differenceThreshold = Math.Max( 1, Math.Min( 255, value ) );
                    differenceThresholdNeg = -differenceThreshold;
                }
            }
        }

        /// <summary>
        /// Motion level value, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>Amount of changes in the last processed frame. For example, if value of
        /// this property equals to 0.1, then it means that last processed frame has 10% difference
        /// with previous frame.</para>
        /// </remarks>
        /// 
        public float MotionLevel
        {
            get
            {
                lock ( sync )
                {
                    return (float) pixelsChanged / ( width * height );
                }
            }
        }

        /// <summary>
        /// Motion frame containing detected areas of motion.
        /// </summary>
        /// 
        /// <remarks><para>Motion frame is a grayscale image, which shows areas of detected motion.
        /// All black pixels in the motion frame correspond to areas, where no motion is
        /// detected. But white pixels correspond to areas, where motion is detected.</para>
        /// 
        /// <para><note>The property is set to <see langword="null"/> after processing of the first
        /// video frame by the algorithm.</note></para>
        /// </remarks>
        ///
        public UnmanagedImage MotionFrame
        {
            get
            {
                lock ( sync )
                {
                    return motionFrame;
                }
            }
        }

        /// <summary>
        /// Suppress noise in video frames or not.
        /// </summary>
        /// 
        /// <remarks><para>The value specifies if additional filtering should be
        /// done to suppress standalone noisy pixels by applying 3x3 erosion image processing
        /// filter.</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para>
        /// 
        /// <para><note>Turning the value on leads to more processing time of video frame.</note></para>
        /// </remarks>
        /// 
        public bool SuppressNoise
        {
            get { return suppressNoise; }
            set
            {
                lock ( sync )
                {
                    suppressNoise = value;

                    // allocate temporary frame if required
                    if ( ( suppressNoise ) && ( tempFrame == null ) && ( motionFrame != null ) )
                    {
                        tempFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );
                    }

                    // check if temporary frame is not required
                    if ( ( !suppressNoise ) && ( tempFrame != null ) )
                    {
                        tempFrame.Dispose( );
                        tempFrame = null;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoFramesDifferenceDetector"/> class.
        /// </summary>
        /// 
        public TwoFramesDifferenceDetector( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoFramesDifferenceDetector"/> class.
        /// </summary>
        /// 
        /// <param name="suppressNoise">Suppress noise in video frames or not (see <see cref="SuppressNoise"/> property).</param>
        /// 
        public TwoFramesDifferenceDetector( bool suppressNoise )
        {
            this.suppressNoise = suppressNoise;
        }

        /// <summary>
        /// Process new video frame.
        /// </summary>
        /// 
        /// <param name="videoFrame">Video frame to process (detect motion in).</param>
        /// 
        /// <remarks><para>Processes new frame from video source and detects motion in it.</para>
        /// 
        /// <para>Check <see cref="MotionLevel"/> property to get information about amount of motion
        /// (changes) in the processed frame.</para>
        /// </remarks>
        /// 
        public unsafe void ProcessFrame( UnmanagedImage videoFrame )
        {
            lock ( sync )
            {
                // check previous frame
                if ( previousFrame == null )
                {
                    // save image dimension
                    width = videoFrame.Width;
                    height = videoFrame.Height;

                    // alocate memory for previous and current frames
                    previousFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );
                    motionFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );

                    frameSize = motionFrame.Stride * height;

                    // temporary buffer
                    if ( suppressNoise )
                    {
                        tempFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );
                    }

                    // convert source frame to grayscale
                    Tools.ConvertToGrayscale( videoFrame, previousFrame );

                    return;
                }

                // check image dimension
                if ( ( videoFrame.Width != width ) || ( videoFrame.Height != height ) )
                    return;

                // convert current image to grayscale
                Tools.ConvertToGrayscale( videoFrame, motionFrame );

                // pointers to previous and current frames
                byte* prevFrame = (byte*) previousFrame.ImageData.ToPointer( );
                byte* currFrame = (byte*) motionFrame.ImageData.ToPointer( );
                // difference value
                int diff;

                // 1 - get difference between frames
                // 2 - threshold the difference
                // 3 - copy current frame to previous frame
                for ( int i = 0; i < frameSize; i++, prevFrame++, currFrame++ )
                {
                    // difference
                    diff = (int) *currFrame - (int) *prevFrame;
                    // copy current frame to previous
                    *prevFrame = *currFrame;
                    // treshold
                    *currFrame = ( ( diff >= differenceThreshold ) || ( diff <= differenceThresholdNeg ) ) ? (byte) 255 : (byte) 0;
                }

                if ( suppressNoise )
                {
                    // suppress noise and calculate motion amount
                    AForge.SystemTools.CopyUnmanagedMemory( tempFrame.ImageData, motionFrame.ImageData, frameSize );
                    erosionFilter.Apply( tempFrame, motionFrame );
                }

                // calculate amount of motion pixels
                pixelsChanged = 0;
                byte* motion = (byte*) motionFrame.ImageData.ToPointer( );

                for ( int i = 0; i < frameSize; i++, motion++ )
                {
                    pixelsChanged += ( *motion & 1 );
                }
            }
        }

        /// <summary>
        /// Reset motion detector to initial state.
        /// </summary>
        /// 
        /// <remarks><para>Resets internal state and variables of motion detection algorithm.
        /// Usually this is required to be done before processing new video source, but
        /// may be also done at any time to restart motion detection algorithm.</para>
        /// </remarks>
        /// 
        public void Reset( )
        {
            lock ( sync )
            {
                if ( previousFrame != null )
                {
                    previousFrame.Dispose( );
                    previousFrame = null;
                }

                if ( motionFrame != null )
                {
                    motionFrame.Dispose( );
                    motionFrame = null;
                }

                if ( tempFrame != null )
                {
                    tempFrame.Dispose( );
                    tempFrame = null;
                }
            }
        }
    }
}