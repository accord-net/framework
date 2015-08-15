// AForge Vision Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Vision.Motion
{
    using System;
    using AForge.Imaging;

    /// <summary>
    /// Interface of motion detector algorithm.
    /// </summary>
    /// 
    /// <remarks><para>The interface specifies methods, which should be implemented
    /// by all motion detection algorithms - algorithms which perform processing of video
    /// frames in order to detect motion. Amount of detected motion may be checked using
    /// <see cref="MotionLevel"/> property. Also <see cref="MotionFrame"/> property may
    /// be used in order to see all the detected motion areas. For example, the <see cref="MotionFrame"/> property
    /// is used by motion processing algorithms for further motion post processing, like
    /// highlighting motion areas, counting number of detected moving object, etc.
    /// </para></remarks>
    /// 
    /// <seealso cref="MotionDetector"/>
    /// <seealso cref="IMotionProcessing"/>
    ///
    ///
    public interface IMotionDetector
    {
        /// <summary>
        /// Motion level value, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>Amount of changes in the last processed frame. For example, if value of
        /// this property equals to 0.1, then it means that last processed frame has 10% of changes
        /// (however it is up to specific implementation to decide how to compare specified frame).</para>
        /// </remarks>
        /// 
        float MotionLevel { get; }

        /// <summary>
        /// Motion frame containing detected areas of motion.
        /// </summary>
        /// 
        /// <remarks><para>Motion frame is a grayscale image, which shows areas of detected motion.
        /// All black pixels in the motion frame correspond to areas, where no motion is
        /// detected. But white pixels correspond to areas, where motion is detected.</para></remarks>
        /// 
        UnmanagedImage MotionFrame { get; }

        /// <summary>
        /// Process new video frame.
        /// </summary>
        /// 
        /// <param name="videoFrame">Video frame to process (detect motion in).</param>
        /// 
        /// <remarks><para>Processes new frame from video source and detects motion in it.</para></remarks>
        /// 
        void ProcessFrame( UnmanagedImage videoFrame );

        /// <summary>
        /// Reset motion detector to initial state.
        /// </summary>
        /// 
        /// <remarks><para>Resets internal state and variables of motion detection algorithm.
        /// Usually this is required to be done before processing new video source, but
        /// may be also done at any time to restart motion detection algorithm.</para>
        /// </remarks>
        /// 
        void Reset( );
    }
}
