// Accord Video Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
//
// AForge Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2012
// contacts@aforgenet.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Video
{
    using System;
    using System.Drawing;

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
        /// <summary>
        /// Initializes a new instance of the <see cref="NewFrameEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="frame">New frame.</param>
        /// 
        public NewFrameEventArgs(Bitmap frame)
        {
            this.Frame = frame;
            this.FrameSize = frame.Size;
        }

        /// <summary>
        /// Gets the time when the frame started being captured.
        /// </summary>
        /// 
        public DateTime CaptureStarted { get; set; }

        /// <summary>
        /// Gets the time when the frame finished being captured.
        /// </summary>
        /// 
        public DateTime CaptureFinished { get; set; }

        /// <summary>
        /// New frame from video source.
        /// </summary>
        /// 
        public Bitmap Frame { get; set; }

        /// <summary>
        /// Gets the index of this frame since the capturing started.
        /// </summary>
        /// 
        public int FrameIndex { get; set; }

        /// <summary>
        /// Gets the region of the <see cref="Frame"/> which still contains 
        /// useful data (it could be that cropping operations have reduced this
        /// area to a smaller size, which should be reflected by this property).
        /// </summary>
        /// 
        public Size FrameSize { get; set; }
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
        /// 
        public VideoSourceErrorEventArgs(string description)
            : this(description, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoSourceErrorEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="exception">Error exception.</param>
        /// 
        public VideoSourceErrorEventArgs(Exception exception)
            : this(exception.Message, exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoSourceErrorEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="description">Error description.</param>
        /// <param name="exception">Error exception.</param>
        /// 
        public VideoSourceErrorEventArgs(string description, Exception exception)
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
        /// 
        public Exception Exception
        {
            get { return _exception; }
        }
    }
}
