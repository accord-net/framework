// AForge Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

namespace Accord.Video
{
    using System;

    /// <summary>
    /// Delegate for new frame event handler.
    /// </summary>
    /// 
    /// <param name="sender">Sender object.</param>
    /// <param name="eventArgs">Event arguments.</param>
    /// 
    public delegate void NewFrameEventHandler(object sender, NewFrameEventArgs eventArgs);

    /// <summary>
    /// Delegate for video source error event handler.
    /// </summary>
    /// 
    /// <param name="sender">Sender object.</param>
    /// <param name="eventArgs">Event arguments.</param>
    /// 
    public delegate void VideoSourceErrorEventHandler(object sender, VideoSourceErrorEventArgs eventArgs);

    /// <summary>
    /// Delegate for playing finished event handler.
    /// </summary>
    /// 
    /// <param name="sender">Sender object.</param>
    /// <param name="reason">Reason of finishing video playing.</param>
    /// 
    public delegate void PlayingFinishedEventHandler(object sender, ReasonToFinishPlaying reason);

    /// <summary>
    /// Reason of finishing video playing.
    /// </summary>
    /// 
    /// <remarks><para>When video source class fire the <see cref="IVideoSource.PlayingFinished"/> event, they
    /// need to specify reason of finishing video playing. For example, it may be end of stream reached.</para></remarks>
    /// 
    public enum ReasonToFinishPlaying
    {
        /// <summary>
        /// Video playing has finished because it end was reached.
        /// </summary>
        EndOfStreamReached,
        /// <summary>
        /// Video playing has finished because it was stopped by user.
        /// </summary>
        StoppedByUser,
        /// <summary>
        /// Video playing has finished because the device was lost (unplugged).
        /// </summary>
        DeviceLost,
        /// <summary>
        /// Video playing has finished because of some error happened the video source (camera, stream, file, etc.).
        /// A error reporting event usually is fired to provide error information.
        /// </summary>
        VideoSourceError
    }

    /// <summary>
    /// Arguments for new frame event from video source.
    /// </summary>
    /// 
    public class NewFrameEventArgs : EventArgs
    {
        private System.Drawing.Bitmap frame;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewFrameEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="frame">New frame.</param>
        /// 
        public NewFrameEventArgs(System.Drawing.Bitmap frame)
        {
            this.frame = frame;
        }

        /// <summary>
        /// New frame from video source.
        /// </summary>
        /// 
        public System.Drawing.Bitmap Frame
        {
            get { return frame; }
        }
    }

    /// <summary>
    /// Arguments for video source error event from video source.
    /// </summary>
    /// 
    public class VideoSourceErrorEventArgs : EventArgs
    {
        private readonly string _description;
        private readonly Exception _exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoSourceErrorEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="description">Error description.</param>
        /// <param name="exception"></param>
        /// 
        public VideoSourceErrorEventArgs(string description, Exception exception = null)
        {
            _description = description;
            _exception = exception;
        }

        /// <summary>
        /// Video source error description.
        /// </summary>
        /// 
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Video source exception causing the error
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }
    }
}
