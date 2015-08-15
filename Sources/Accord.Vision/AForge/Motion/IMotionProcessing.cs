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
    /// Interface of motion processing algorithm.
    /// </summary>
    ///
    /// <remarks><para>The interface specifies methods, which should be implemented
    /// by all motion processng algorithms - algorithm which perform further post processing
    /// of detected motion, which is done by motion detection algorithms (see <see cref="IMotionDetector"/>).
    /// </para></remarks>
    /// 
    /// <seealso cref="MotionDetector"/>
    /// <seealso cref="IMotionDetector"/>
    ///
    public interface IMotionProcessing
    {
        /// <summary>
        /// Process video and motion frames doing further post processing after
        /// performed motion detection.
        /// </summary>
        /// 
        /// <param name="videoFrame">Original video frame.</param>
        /// <param name="motionFrame">Motion frame provided by motion detection
        /// algorithm (see <see cref="IMotionDetector"/>).</param>
        /// 
        /// <remarks><para>The method does father post processing of detected motion.
        /// Type of motion post processing is specified by specific implementation
        /// of the <see cref="IMotionProcessing"/> interface - it may motion
        /// area highlighting, motion objects counting, etc.</para></remarks>
        /// 
        void ProcessFrame( UnmanagedImage videoFrame, UnmanagedImage motionFrame );

        /// <summary>
        /// Reset internal state of motion processing algorithm.
        /// </summary>
        /// 
        /// <remarks><para>The method allows to reset internal state of motion processing
        /// algorithm and prepare it for processing of next video stream or to restart
        /// the algorithm.</para>
        /// 
        /// <para><note>Some motion processing algorithms may not have any stored internal
        /// states and may just process provided video frames without relying on any motion
        /// history etc. In this case such algorithms provide empty implementation of this method.</note></para>
        /// </remarks>
        ///
        void Reset( );
    }
}
