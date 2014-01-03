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
    using Accord;

    /// <summary>
    ///   Audio Output Interface
    /// </summary>
    /// 
    /// <remarks>This interface is implemented by objects which
    /// can reproduce sounds. Examples are sound card outputs, wave
    /// file encoders/writers and special purpose encoders.</remarks>
    /// 
    /// <seealso cref="IAudioSource"/>
    /// 
    public interface IAudioOutput
    {

        /// <summary>
        ///   Starts playing the buffer
        /// </summary>
        /// 
        void Play();

        /// <summary>
        ///   Stops playing the buffer
        /// </summary>
        /// 
        void Stop();

        /// <summary>
        ///   Audio output.
        /// </summary>
        /// 
        /// <remarks>
        ///   <para>
        ///   The meaning of the property depends on particular audio output.
        ///   Depending on audio source it may be a file name, driver guid, URL
        ///   or any other string describing the audio source.</para>
        /// </remarks>
        /// 
        string Output { get; set; }

        /// <summary>
        ///   Indicates a block of frames have started execution.
        /// </summary>
        /// 
        event EventHandler<PlayFrameEventArgs> FramePlayingStarted;

        /// <summary>
        ///   Indicates all frames have been played and the audio finished.
        /// </summary>
        /// 
        event EventHandler Stopped;

        /// <summary>
        ///   Indicates the audio output is requesting a new sample.
        /// </summary>
        /// 
        event EventHandler<NewFrameRequestedEventArgs> NewFrameRequested;


        /// <summary>
        ///   Gets a value indicating whether this instance is playing audio.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        /// 
        bool IsRunning { get; }

        /// <summary>
        ///   Signals audio output to stop its work.
        /// </summary>
        /// 
        /// <remarks>Signals audio output to stop its background thread, stop to
        /// request new frames and free resources.</remarks>
        /// 
        void SignalToStop();

        /// <summary>
        ///   Wait until audio output has stopped.
        /// </summary>
        /// 
        /// <remarks>Waits for audio output stopping after it was signaled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        /// 
        void WaitForStop();

        /// <summary>
        ///   Audio output error event.
        /// </summary>
        /// 
        /// <remarks>This event is used to notify clients about any type of errors occurred in
        /// audio output object, for example internal exceptions.</remarks>
        /// 
        event EventHandler<AudioOutputErrorEventArgs> AudioOutputError;

    }
}
