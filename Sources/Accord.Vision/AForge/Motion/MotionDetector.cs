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
    /// Motion detection wrapper class, which performs motion detection and processing.
    /// </summary>
    ///
    /// <remarks><para>The class serves as a wrapper class for
    /// <see cref="IMotionDetector">motion detection</see> and
    /// <see cref="IMotionProcessing">motion processing</see> algorithms, allowing to call them with
    /// single call. Unlike motion detection and motion processing interfaces, the class also
    /// provides additional methods for convenience, so the algorithms could be applied not
    /// only to <see cref="AForge.Imaging.UnmanagedImage"/>, but to .NET's <see cref="Bitmap"/> class
    /// as well.</para>
    /// 
    /// <para>In addition to wrapping of motion detection and processing algorthms, the class provides
    /// some additional functionality. Using <see cref="MotionZones"/> property it is possible to specify
    /// set of rectangular zones to observe - only motion in these zones is counted and post procesed.</para>
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
    public class MotionDetector
    {
        private IMotionDetector   detector;
        private IMotionProcessing processor;

        // motion detectoin zones
        private Rectangle[] motionZones = null;
        // image of motion zones
        private UnmanagedImage zonesFrame;
        // size of video frame
        private int videoWidth, videoHeight;

        // dummy object to lock for synchronization
        private object sync = new object( );

        /// <summary>
        /// Motion detection algorithm to apply to each video frame.
        /// </summary>
        ///
        /// <remarks><para>The property sets motion detection algorithm, which is used by
        /// <see cref="ProcessFrame(UnmanagedImage)"/> method in order to calculate
        /// <see cref="IMotionDetector.MotionLevel">motion level</see> and
        /// <see cref="IMotionDetector.MotionFrame">motion frame</see>.
        /// </para></remarks>
        ///
        public IMotionDetector MotionDetectionAlgorithm
        {
            get { return detector; }
            set
            {
                lock ( sync )
                {
                    detector = value;
                }
            }
        }

        /// <summary>
        /// Motion processing algorithm to apply to each video frame after
        /// motion detection is done.
        /// </summary>
        /// 
        /// <remarks><para>The property sets motion processing algorithm, which is used by
        /// <see cref="ProcessFrame(UnmanagedImage)"/> method after motion detection in order to do further
        /// post processing of motion frames. The aim of further post processing depends on
        /// actual implementation of the specified motion processing algorithm - it can be
        /// highlighting of motion area, objects counting, etc.
        /// </para></remarks>
        /// 
        public IMotionProcessing MotionProcessingAlgorithm
        {
            get { return processor; }
            set
            {
                lock ( sync )
                {
                    processor = value;
                }
            }
        }

        /// <summary>
        /// Set of zones to detect motion in.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps array of rectangular zones, which are observed for motion detection.
        /// Motion outside of these zones is ignored.</para>
        /// 
        /// <para>In the case if this property is set, the <see cref="ProcessFrame(UnmanagedImage)"/> method
        /// will filter out all motion witch was detected by motion detection algorithm, but is not
        /// located in the specified zones.</para>
        /// </remarks>
        /// 
        public Rectangle[] MotionZones
        {
            get { return motionZones; }
            set
            {
                motionZones = value;
                CreateMotionZonesFrame( );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionDetector"/> class.
        /// </summary>
        /// 
        /// <param name="detector">Motion detection algorithm to apply to each video frame.</param>
        /// 
        public MotionDetector( IMotionDetector detector ) : this( detector, null ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionDetector"/> class.
        /// </summary>
        /// 
        /// <param name="detector">Motion detection algorithm to apply to each video frame.</param>
        /// <param name="processor">Motion processing algorithm to apply to each video frame after
        /// motion detection is done.</param>
        /// 
        public MotionDetector( IMotionDetector detector, IMotionProcessing processor )
        {
            this.detector  = detector;
            this.processor = processor;
        }

        /// <summary>
        /// Process new video frame.
        /// </summary>
        /// 
        /// <param name="videoFrame">Video frame to process (detect motion in).</param>
        /// 
        /// <returns>Returns amount of motion, which is provided <see cref="IMotionDetector.MotionLevel"/>
        /// property of the <see cref="MotionDetectionAlgorithm">motion detection algorithm in use</see>.</returns>
        /// 
        /// <remarks><para>See <see cref="ProcessFrame(UnmanagedImage)"/> for additional details.</para>
        /// </remarks>
        /// 
        public float ProcessFrame( Bitmap videoFrame )
        {
            float motionLevel = 0;

            BitmapData videoData = videoFrame.LockBits(
                new Rectangle( 0, 0, videoFrame.Width, videoFrame.Height ),
                ImageLockMode.ReadWrite, videoFrame.PixelFormat );

            try
            {
                motionLevel = ProcessFrame( new UnmanagedImage( videoData ) );
            }
            finally
            {
                videoFrame.UnlockBits( videoData );
            }

            return motionLevel;
        }

        /// <summary>
        /// Process new video frame.
        /// </summary>
        /// 
        /// <param name="videoFrame">Video frame to process (detect motion in).</param>
        /// 
        /// <returns>Returns amount of motion, which is provided <see cref="IMotionDetector.MotionLevel"/>
        /// property of the <see cref="MotionDetectionAlgorithm">motion detection algorithm in use</see>.</returns>
        /// 
        /// <remarks><para>See <see cref="ProcessFrame(UnmanagedImage)"/> for additional details.</para>
        /// </remarks>
        ///
        public float ProcessFrame( BitmapData videoFrame )
        {
            return ProcessFrame( new UnmanagedImage( videoFrame ) );
        }

        /// <summary>
        /// Process new video frame.
        /// </summary>
        /// 
        /// <param name="videoFrame">Video frame to process (detect motion in).</param>
        /// 
        /// <returns>Returns amount of motion, which is provided <see cref="IMotionDetector.MotionLevel"/>
        /// property of the <see cref="MotionDetectionAlgorithm">motion detection algorithm in use</see>.</returns>
        /// 
        /// <remarks><para>The method first of all applies motion detection algorithm to the specified video
        /// frame to calculate <see cref="IMotionDetector.MotionLevel">motion level</see> and
        /// <see cref="IMotionDetector.MotionFrame">motion frame</see>. After this it applies motion processing algorithm
        /// (if it was set) to do further post processing, like highlighting motion areas, counting moving
        /// objects, etc.</para>
        /// 
        /// <para><note>In the case if <see cref="MotionZones"/> property is set, this method will perform
        /// motion filtering right after motion algorithm is done and before passing motion frame to motion
        /// processing algorithm. The method does filtering right on the motion frame, which is produced
        /// by motion detection algorithm. At the same time the method recalculates motion level and returns
        /// new value, which takes motion zones into account (but the new value is not set back to motion detection
        /// algorithm' <see cref="IMotionDetector.MotionLevel"/> property).
        /// </note></para>
        /// </remarks>
        /// 
        public float ProcessFrame( UnmanagedImage videoFrame )
        {
            lock ( sync )
            {
                if ( detector == null )
                    return 0;

                videoWidth  = videoFrame.Width;
                videoHeight = videoFrame.Height;

                float motionLevel = 0;
                // call motion detection
                detector.ProcessFrame( videoFrame );
                motionLevel = detector.MotionLevel;

                // check if motion zones are specified
                if ( motionZones != null )
                {
                    if ( zonesFrame == null )
                    {
                        CreateMotionZonesFrame( );
                    }

                    if ( ( videoWidth == zonesFrame.Width ) && ( videoHeight == zonesFrame.Height ) )
                    {
                        unsafe
                        {
                            // pointers to background and current frames
                            byte* zonesPtr  = (byte*) zonesFrame.ImageData.ToPointer( );
                            byte* motionPtr = (byte*) detector.MotionFrame.ImageData.ToPointer( );

                            motionLevel = 0;

                            for ( int i = 0, frameSize = zonesFrame.Stride * videoHeight; i < frameSize; i++, zonesPtr++, motionPtr++ )
                            {
                                *motionPtr &= *zonesPtr;
                                motionLevel += ( *motionPtr & 1 );
                            }
                            motionLevel /= ( videoWidth * videoHeight );
                        }
                    }
                }

                // call motion post processing
                if ( ( processor != null ) && ( detector.MotionFrame != null ) )
                {
                    processor.ProcessFrame( videoFrame, detector.MotionFrame );
                }

                return motionLevel;
            }
        }

        /// <summary>
        /// Reset motion detector to initial state.
        /// </summary>
        /// 
        /// <remarks><para>The method resets motion detection and motion processing algotithms by calling
        /// their <see cref="IMotionDetector.Reset"/> and <see cref="IMotionProcessing.Reset"/> methods.</para>
        /// </remarks>
        /// 
        public void Reset( )
        {
            lock ( sync )
            {
                if ( detector != null )
                {
                    detector.Reset( );
                }
                if ( processor != null )
                {
                    processor.Reset( );
                }

                videoWidth  = 0;
                videoHeight = 0;

                if ( zonesFrame != null )
                {
                    zonesFrame.Dispose( );
                    zonesFrame = null;
                }
            }
        }

        // Create motion zones' image
        private unsafe void CreateMotionZonesFrame( )
        {
            lock ( sync )
            {
                // free previous motion zones frame
                if ( zonesFrame != null )
                {
                    zonesFrame.Dispose( );
                    zonesFrame = null;
                }

                // create motion zones frame only in the case if the algorithm has processed at least one frame
                if ( ( motionZones != null ) && ( motionZones.Length != 0 ) && ( videoWidth != 0 ) )
                {
                    zonesFrame = UnmanagedImage.Create( videoWidth, videoHeight, PixelFormat.Format8bppIndexed );

                    Rectangle imageRect = new Rectangle( 0, 0, videoWidth, videoHeight );

                    // draw all motion zones on motion frame
                    foreach ( Rectangle rect in motionZones )
                    {
                        rect.Intersect( imageRect );

                        // rectangle's dimenstion
                        int rectWidth  = rect.Width;
                        int rectHeight = rect.Height;

                        // start pointer
                        int stride = zonesFrame.Stride;
                        byte* ptr = (byte*) zonesFrame.ImageData.ToPointer( ) + rect.Y * stride + rect.X;

                        for ( int y = 0; y < rectHeight; y++ )
                        {
                            AForge.SystemTools.SetUnmanagedMemory( ptr, 255, rectWidth );
                            ptr += stride;
                        }
                    }
                }
            }
        }
    }
}
