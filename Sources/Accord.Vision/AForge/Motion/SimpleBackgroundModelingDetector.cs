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
    /// Motion detector based on simple background modeling.
    /// </summary>
    /// 
    /// <remarks><para>The class implements motion detection algorithm, which is based on
    /// difference of current video frame with modeled background frame.
    /// The <see cref="MotionFrame">difference frame</see> is thresholded and the
    /// <see cref="MotionLevel">amount of difference pixels</see> is calculated.
    /// To suppress stand-alone noisy pixels erosion morphological operator may be applied, which
    /// is controlled by <see cref="SuppressNoise"/> property.</para>
    /// 
    /// <para><note>In the case if precise motion area's borders are required (for example,
    /// for further motion post processing), then <see cref="KeepObjectsEdges"/> property
    /// may be used to restore borders after noise suppression.</note></para>
    /// 
    /// <para>As the first approximation of background frame, the first frame of video stream is taken.
    /// During further video processing the background frame is constantly updated, so it
    /// changes in the direction to decrease difference with current video frame (the background
    /// frame is moved towards current frame). See <see cref="FramesPerBackgroundUpdate"/>
    /// <see cref="MillisecondsPerBackgroundUpdate"/> properties, which control the rate of
    /// background frame update.</para>
    /// 
    /// <para>Unlike <see cref="TwoFramesDifferenceDetector"/> motion detection algorithm, this algorithm
    /// allows to identify quite clearly all objects, which are not part of the background (scene) -
    /// most likely moving objects. And unlike <see cref="CustomFrameDifferenceDetector"/> motion
    /// detection algorithm, this algorithm includes background adaptation feature, which allows it
    /// to update its modeled background frame in order to take scene changes into account.</para>
    /// 
    /// <para><note>Because of the adaptation feature of the algorithm, it may adopt
    /// to background changes, what <see cref="CustomFrameDifferenceDetector"/> algorithm can not do.
    /// However, if moving object stays on the scene for a while (so algorithm adopts to it and does
    /// not treat it as a new moving object any more) and then starts to move again, the algorithm may
    /// find two moving objects - the true one, which is really moving, and the false one, which does not (the
    /// place, where the object stayed for a while).</note></para>
    /// 
    /// <para><note>The algorithm is not applicable to such cases, when moving object resides
    /// in camera's view most of the time (laptops camera monitoring a person sitting in front of it,
    /// for example). The algorithm is mostly supposed for cases, when camera monitors some sort
    /// of static scene, where moving objects appear from time to time - street, road, corridor, etc.
    /// </note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create motion detector
    /// MotionDetector detector = new MotionDetector(
    ///     new SimpleBackgroundModelingDetector( ),
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
    public class SimpleBackgroundModelingDetector : IMotionDetector
    {
        // frame's dimension
        private int width;
        private int height;
        private int frameSize;

        // previous frame of video stream
        private UnmanagedImage backgroundFrame;
        // current frame of video sream
        private UnmanagedImage motionFrame;
        // temporary buffer used for suppressing noise
        private UnmanagedImage tempFrame;
        // number of pixels changed in the new frame of video stream
        private int pixelsChanged;

        // suppress noise
        private bool suppressNoise   = true;
        private bool keepObjectEdges = false;

        // threshold values
        private int differenceThreshold    =  15;
        private int differenceThresholdNeg = -15;

        private int framesPerBackgroundUpdate = 2;
        private int framesCounter = 0;

        private int millisecondsPerBackgroundUpdate = 0;
        private int millisecondsLeftUnprocessed = 0;
        private DateTime lastTimeMeasurment;

        // binary erosion filter
        private BinaryErosion3x3 erosionFilter = new BinaryErosion3x3( );
        // binary dilatation filter
        private BinaryDilatation3x3 dilatationFilter = new BinaryDilatation3x3( );

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
        /// with modeled background frame.</para>
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
        /// filter. See <see cref="KeepObjectsEdges"/> property, if it is required to restore
        /// edges of objects, which are not noise.</para>
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
        /// Restore objects edges after noise suppression or not.
        /// </summary>
        /// 
        /// <remarks><para>The value specifies if additional filtering should be done
        /// to restore objects' edges after noise suppression by applying 3x3 dilatation
        /// image processing filter.</para>
        /// 
        /// <para>Default value is set to <see langword="false"/>.</para>
        /// 
        /// <para><note>Turning the value on leads to more processing time of video frame.</note></para>
        /// </remarks>
        /// 
        public bool KeepObjectsEdges
        {
            get { return keepObjectEdges; }
            set
            {
                lock ( sync )
                {
                    keepObjectEdges = value;
                }
            }
        }

        /// <summary>
        /// Frames per background update, [1, 50].
        /// </summary>
        /// 
        /// <remarks><para>The value controls the speed of modeled background adaptation to
        /// scene changes. After each specified amount of frames the background frame is updated
        /// in the direction to decrease difference with current processing frame.</para>
        /// 
        /// <para>Default value is set to <b>2</b>.</para>
        /// 
        /// <para><note>The property has effect only in the case if <see cref="MillisecondsPerBackgroundUpdate"/>
        /// property is set to <b>0</b>. Otherwise it does not have effect and background
        /// update is managed according to the <see cref="MillisecondsPerBackgroundUpdate"/>
        /// property settings.</note></para>
        /// </remarks>
        /// 
        public int FramesPerBackgroundUpdate
        {
            get { return framesPerBackgroundUpdate; }
            set { framesPerBackgroundUpdate = Math.Max( 1, Math.Min( 50, value ) ); }
        }

        /// <summary>
        /// Milliseconds per background update, [0, 5000].
        /// </summary>
        /// 
        /// <remarks><para>The value represents alternate way of controlling the speed of modeled
        /// background adaptation to scene changes. The value sets number of milliseconds, which
        /// should elapse between two consequent video frames to result in background update
        /// for one intensity level. For example, if this value is set to 100 milliseconds and
        /// the amount of time elapsed between two last video frames equals to 350, then background
        /// frame will be update for 3 intensity levels in the direction to decrease difference
        /// with current video frame (the remained 50 milliseconds will be added to time difference
        /// between two next consequent frames, so the accuracy is preserved).</para>
        /// 
        /// <para>Unlike background update method controlled using <see cref="FramesPerBackgroundUpdate"/>
        /// method, the method guided by this property is not affected by changes
        /// in frame rates. If, for some reasons, a video source starts to provide delays between
        /// frames (frame rate drops down), the amount of background update still stays consistent.
        /// When background update is controlled by this property, it is always possible to estimate
        /// amount of time required to change, for example, absolutely black background (0 intensity
        /// values) into absolutely white background (255 intensity values). If value of this
        /// property is set to 100, then it will take approximately 25.5 seconds for such update
        /// regardless of frame rate.</para>
        /// 
        /// <para><note>Background update controlled by this property is slightly slower then
        /// background update controlled by <see cref="FramesPerBackgroundUpdate"/> property,
        /// so it has a bit greater impact on performance.</note></para>
        /// 
        /// <para><note>If this property is set to 0, then corresponding background updating
        /// method is not used (turned off), but background update guided by
        /// <see cref="FramesPerBackgroundUpdate"/> property is used.</note></para>
        /// 
        /// <para>Default value is set to <b>0</b>.</para>
        /// </remarks>
        /// 
        public int MillisecondsPerBackgroundUpdate
        {
            get { return millisecondsPerBackgroundUpdate; }
            set { millisecondsPerBackgroundUpdate = Math.Max( 0, Math.Min( 5000, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleBackgroundModelingDetector"/> class.
        /// </summary>
        public SimpleBackgroundModelingDetector( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleBackgroundModelingDetector"/> class.
        /// </summary>
        /// 
        /// <param name="suppressNoise">Suppress noise in video frames or not (see <see cref="SuppressNoise"/> property).</param>
        /// 
        public SimpleBackgroundModelingDetector( bool suppressNoise )
        {
            this.suppressNoise = suppressNoise;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleBackgroundModelingDetector"/> class.
        /// </summary>
        /// 
        /// <param name="suppressNoise">Suppress noise in video frames or not (see <see cref="SuppressNoise"/> property).</param>
        /// <param name="keepObjectEdges">Restore objects edges after noise suppression or not (see <see cref="KeepObjectsEdges"/> property).</param>
        /// 
        public SimpleBackgroundModelingDetector( bool suppressNoise, bool keepObjectEdges )
        {
            this.suppressNoise   = suppressNoise;
            this.keepObjectEdges = keepObjectEdges;
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
                // check background frame
                if ( backgroundFrame == null )
                {
                    lastTimeMeasurment = DateTime.Now;

                    // save image dimension
                    width  = videoFrame.Width;
                    height = videoFrame.Height;

                    // alocate memory for previous and current frames
                    backgroundFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );
                    motionFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );

                    frameSize = motionFrame.Stride * height;

                    // temporary buffer
                    if ( suppressNoise )
                    {
                        tempFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );
                    }

                    // convert source frame to grayscale
                    Tools.ConvertToGrayscale( videoFrame, backgroundFrame );

                    return;
                }

                // check image dimension
                if ( ( videoFrame.Width != width ) || ( videoFrame.Height != height ) )
                    return;

                // convert current image to grayscale
                Tools.ConvertToGrayscale( videoFrame, motionFrame );

                // pointers to background and current frames
                byte* backFrame;
                byte* currFrame;
                int diff;

                // update background frame
                if ( millisecondsPerBackgroundUpdate == 0 )
                {
                    // update background frame using frame counter as a base
                    if ( ++framesCounter == framesPerBackgroundUpdate )
                    {
                        framesCounter = 0;

                        backFrame = (byte*) backgroundFrame.ImageData.ToPointer( );
                        currFrame = (byte*) motionFrame.ImageData.ToPointer( );

                        for ( int i = 0; i < frameSize; i++, backFrame++, currFrame++ )
                        {
                            diff = *currFrame - *backFrame;
                            if ( diff > 0 )
                            {
                                ( *backFrame )++;
                            }
                            else if ( diff < 0 )
                            {
                                ( *backFrame )--;
                            }
                        }
                    }
                }
                else
                {
                    // update background frame using timer as a base

                    // get current time and calculate difference
                    DateTime currentTime = DateTime.Now;
                    TimeSpan timeDff = currentTime - lastTimeMeasurment;
                    // save current time as the last measurment
                    lastTimeMeasurment = currentTime;

                    int millisonds = (int) timeDff.TotalMilliseconds + millisecondsLeftUnprocessed;

                    // save remainder so it could be taken into account in the future
                    millisecondsLeftUnprocessed = millisonds % millisecondsPerBackgroundUpdate;
                    // get amount for background update 
                    int updateAmount = (int) ( millisonds / millisecondsPerBackgroundUpdate );

                    backFrame = (byte*) backgroundFrame.ImageData.ToPointer( );
                    currFrame = (byte*) motionFrame.ImageData.ToPointer( );

                    for ( int i = 0; i < frameSize; i++, backFrame++, currFrame++ )
                    {
                        diff = *currFrame - *backFrame;
                        if ( diff > 0 )
                        {
                            ( *backFrame ) += (byte) ( (  diff < updateAmount ) ? diff :  updateAmount );
                        }
                        else if ( diff < 0 )
                        {
                            ( *backFrame ) += (byte) ( ( -diff < updateAmount ) ? diff : -updateAmount );
                        }
                    }
                }

                backFrame = (byte*) backgroundFrame.ImageData.ToPointer( );
                currFrame = (byte*) motionFrame.ImageData.ToPointer( );

                // 1 - get difference between frames
                // 2 - threshold the difference
                for ( int i = 0; i < frameSize; i++, backFrame++, currFrame++ )
                {
                    // difference
                    diff = (int) *currFrame - (int) *backFrame;
                    // treshold
                    *currFrame = ( ( diff >= differenceThreshold ) || ( diff <= differenceThresholdNeg ) ) ? (byte) 255 : (byte) 0;
                }

                if ( suppressNoise )
                {
                    // suppress noise and calculate motion amount
                    AForge.SystemTools.CopyUnmanagedMemory( tempFrame.ImageData, motionFrame.ImageData, frameSize );
                    erosionFilter.Apply( tempFrame, motionFrame );

                    if ( keepObjectEdges )
                    {
                        AForge.SystemTools.CopyUnmanagedMemory( tempFrame.ImageData, motionFrame.ImageData, frameSize );
                        dilatationFilter.Apply( tempFrame, motionFrame );
                    }
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
                if ( backgroundFrame != null )
                {
                    backgroundFrame.Dispose( );
                    backgroundFrame = null;
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

                framesCounter = 0;
            }
        }
    }
}
