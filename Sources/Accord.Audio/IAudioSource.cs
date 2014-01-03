// Accord Audio Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
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

namespace Accord.Audio
{
    using System;

    /// <summary>
    ///   Audio Source interface.
    /// </summary>
    /// 
    /// <remarks>This interface is implemented by objects which can
    /// generate or capture sounds. Examples are sound card capture
    /// ports, microphones, wave file decoders and others.</remarks>
    /// 
    /// <seealso cref="IAudioOutput"/>
    /// 
    public interface IAudioSource : IDisposable
    {
        /// <summary>
        ///   New frame block event.
        /// </summary>
        /// 
        /// <remarks><para>This event is used to notify clients about new available audio frame.</para>
        /// 
        /// <para><note>Since audio source may have multiple clients, each client is responsible for
        /// making a copy (cloning) of the passed audio frame, but audio source is responsible for
        /// disposing its own original copy after notifying of clients.</note></para>
        /// </remarks>
        /// 
        event EventHandler<NewFrameEventArgs> NewFrame;

        /// <summary>
        ///   Audio source error event.
        /// </summary>
        /// 
        /// <remarks>This event is used to notify clients about any type of errors occurred in
        /// audio source object, for example internal exceptions.</remarks>
        /// 
        event EventHandler<AudioSourceErrorEventArgs> AudioSourceError;

        /// <summary>
        ///   Audio source.
        /// </summary>
        /// 
        /// <remarks>The meaning of the property depends on particular audio source.
        /// Depending on audio source it may be a file name, driver guid, URL or any
        /// other string describing the audio source.</remarks>
        /// 
        string Source { get; set; }

        /// <summary>
        ///   Amount of samples to be read on each frame.
        /// </summary>
        /// 
        int DesiredFrameSize { get; set; }

        /// <summary>
        ///   Gets the number of audio channels in the source.
        /// </summary>
        /// 
        int Channels { get; set; }

        /// <summary>
        ///   Seeks a frame.
        /// </summary>
        /// 
        /// <remarks>
        ///   This method may throw an NotSupportedException if the source
        ///   does not allow repositioning.
        /// </remarks>
        /// 
        void Seek(int frameIndex);

        /// <summary>
        ///   Gets or sets the sample rate for the source.
        /// </summary>
        /// 
        /// <remarks>
        ///   Changing this property may throw an NotSupportedException if
        ///   the underlying source does not allow resampling.
        /// </remarks>
        /// 
        int SampleRate { get; set; }

        /// <summary>
        ///   Gets a Boolean value indicating if the source allows repositioning.
        /// </summary>
        /// 
        bool CanSeek { get; }

        /// <summary>
        ///   Received frames count.
        /// </summary>
        /// 
        /// <remarks>Number of frames the audio source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        int FramesReceived { get; }

        /// <summary>
        ///   Received bytes count.
        /// </summary>
        /// 
        /// <remarks>Number of bytes the audio source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        int BytesReceived { get; }

        /// <summary>
        ///   User data.
        /// </summary>
        /// 
        /// <remarks>The property allows to associate user data with audio source object.</remarks>
        /// 
        object UserData { get; set; }

        /// <summary>
        /// State of the audio source.
        /// </summary>
        /// 
        /// <remarks>Current state of audio source object - running or not.</remarks>
        /// 
        bool IsRunning { get; }

        /// <summary>
        ///   Start audio source.
        /// </summary>
        /// 
        /// <remarks>Starts audio source and return execution to caller. Audio source
        /// object creates background thread and notifies about new frames with the
        /// help of <see cref="NewFrame"/> event.</remarks>
        /// 
        void Start();

        /// <summary>
        ///   Signals audio source to stop its work.
        /// </summary>
        /// 
        /// <remarks>Signals audio source to stop its background thread, stop to
        /// provide new frames and free resources.</remarks>
        /// 
        void SignalToStop();

        /// <summary>
        ///   Wait until audio source has stopped.
        /// </summary>
        /// 
        /// <remarks>Waits for audio source stopping after it was signaled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        /// 
        void WaitForStop();

        /// <summary>
        ///   Stop audio source.
        /// </summary>
        /// 
        /// <remarks>Stops audio source aborting its thread.</remarks>
        /// 
        void Stop();

    }
}