﻿// Accord Audio Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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
    using System.Threading;
    using System.Collections.Generic;
    using Accord.Compat;
    using System.Linq;
    using System.Data;

    /// <summary>
    ///   Software mixer for <see cref="IAudioSource">audio sources</see>.
    /// </summary>
    /// 
    public class AudioSourceMixer : IAudioSource, IDisposable
    {
        private Thread thread = null;

        private int channels;
        private int sampleRate;
        private int frameSize;
        private int framesReceived;
        private int bytesReceived;
        private bool needToStop;

        private Signal[] signals;
        private IAudioSource[] sources;
        private ManualResetEventSlim[] inputReady;
        private ManualResetEventSlim outputReady;


        /// <summary>
        ///   New frame event.
        /// </summary>
        /// 
        /// <remarks><para>Notifies clients about new available frame from audio source.</para>
        /// 
        /// <para><note>Since audio source may have multiple clients, each client is responsible for
        /// making a copy (cloning) of the passed audio frame, because the audio source disposes its
        /// own original copy after notifying of clients.</note></para>
        /// </remarks>
        /// 
        public event EventHandler<NewFrameEventArgs> NewFrame;


        /// <summary>
        ///   Audio source error event.
        /// </summary>
        /// 
        /// <remarks>This event is used to notify clients about any type of errors occurred in
        /// audio source object, for example internal exceptions.</remarks>
        /// 
        public event EventHandler<AudioSourceErrorEventArgs> AudioSourceError;


        /// <summary>
        ///   Initializes a new instance of the <see cref="AudioSourceMixer"/> class.
        /// </summary>
        /// 
        /// <param name="sources">The audio sources to be mixed.</param>
        /// 
        public AudioSourceMixer(IEnumerable<IAudioSource> sources)
        {
            this.sources = sources.ToArray();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="AudioSourceMixer"/> class.
        /// </summary>
        /// 
        /// <param name="sources">The audio sources to be mixed.</param>
        /// 
        public AudioSourceMixer(params IAudioSource[] sources)
        {
            this.sources = sources;
        }


        /// <summary>
        ///   Gets a string representing this instance.
        /// </summary>
        /// 
        public string Source
        {
            get { return "Software Audio Mixer"; }
            set { }
        }

        /// <summary>
        ///   Gets the audio sources being mixed.
        /// </summary>
        /// 
        public IAudioSource[] Sources
        {
            get { return sources; }
        }


        /// <summary>
        ///   Amount of samples to be read on each frame.
        /// </summary>
        /// 
        public int DesiredFrameSize
        {
            get { return frameSize; }
            set { frameSize = value; }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public void Seek(int frameIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Gets the sample rate for the source.
        /// </summary>
        /// 
        public int SampleRate
        {
            get { return sampleRate; }
            set { throw new ReadOnlyException(); }
        }

        /// <summary>
        /// Obsolete. Please use <see cref="NumberOfChannels"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfChannels instead.")]
        public int Channels
        {
            get { return channels; }
            set { throw new ReadOnlyException(); }
        }

        /// <summary>
        ///   Gets the number of audio channels in the source.
        /// </summary>
        /// 
        public int NumberOfChannels
        {
            get { return channels; }
            set { throw new ReadOnlyException(); }
        }

        /// <summary>
        ///   Returns false, as this source doesn't allows repositioning.
        /// </summary>
        /// 
        public bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        ///   Received frames count.
        /// </summary>
        /// 
        /// <remarks>Number of frames the audio source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public int FramesReceived
        {
            get
            {
                int frames = framesReceived;
                framesReceived = 0;
                return frames;
            }
        }

        /// <summary>
        ///   Received bytes count.
        /// </summary>
        /// 
        /// <remarks>Number of bytes the audio source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public int BytesReceived
        {
            get
            {
                int bytes = bytesReceived;
                bytesReceived = 0;
                return bytes;
            }
        }


        /// <summary>
        ///   User data.
        /// </summary>
        /// 
        /// <remarks>The property allows to associate user data with audio source object.</remarks>
        /// 
        public object UserData { get; set; }

        /// <summary>
        ///   State of the audio source.
        /// </summary>
        /// 
        /// <remarks>Current state of audio source object - running or not.</remarks>
        /// 
        public bool IsRunning
        {
            get
            {
                if (thread != null)
                {
                    // check thread status
                    if (thread.Join(0) == false)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        ///   Start audio source.
        /// </summary>
        /// 
        /// <remarks>Starts audio source and return execution to caller. Audio source
        /// object creates background thread and notifies about new frames with the
        /// help of <see cref="NewFrame"/> event.</remarks>
        /// 
        public void Start()
        {
            if (thread != null)
                throw new InvalidOperationException("Thread is already running.");

            // check source
            for (int i = 0; i < sources.Length; i++)
            {
                IAudioSource source = sources[i];

                if (source == null)
                    throw new ArgumentException("Audio source is not specified");

                source.UserData = i;

                source.NewFrame += source_NewFrame;
                channels = Math.Max(channels, source.NumberOfChannels);
                if (sampleRate == 0)
                    sampleRate = source.SampleRate;
                else if (source.SampleRate != sampleRate)
                    throw new InvalidSignalPropertiesException(
                        "The sample rates of all audio sources must match");

                if (frameSize == 0)
                    frameSize = source.DesiredFrameSize;
                else if (source.DesiredFrameSize != frameSize)
                    throw new InvalidSignalPropertiesException(
                        "The sample rates of all audio sources must match");
            }

            if (channels > 2)
                throw new ArgumentException("Only a maximum of 2 channels is supported.");

            framesReceived = 0;
            bytesReceived = 0;

            // create events
            signals = new Signal[sources.Length];
            inputReady = new ManualResetEventSlim[sources.Length];
            for (int i = 0; i < inputReady.Length; i++)
                inputReady[i] = new ManualResetEventSlim(false);
            outputReady = new ManualResetEventSlim(false);

            // create and start new thread
            thread = new Thread(WorkerThread);
            thread.Name = "Software Audio Mixer";
            thread.Start();
        }

        /// <summary>
        ///   Signals audio source to stop its work.
        /// </summary>
        /// 
        /// <remarks>Signals audio source to stop its background thread, stop to
        /// provide new frames and free resources.</remarks>
        /// 
        public void SignalToStop()
        {
            // stop thread
            if (thread != null)
            {
                // signal to stop
                needToStop = true;

                // unblock all wait operations
                for (int i = 0; i < inputReady.Length; i++)
                    inputReady[i].Set();
            }
        }

        /// <summary>
        ///   Wait for audio source has stopped.
        /// </summary>
        /// 
        /// <remarks>Waits for source stopping after it was signaled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        /// 
        public void WaitForStop()
        {
            if (thread != null)
            {
                // wait for thread stop
                thread.Join();
                thread = null;
            }
        }

        /// <summary>
        ///   Stop audio source.
        /// </summary>
        /// 
        /// <remarks><para>Stops audio source aborting its thread.</para>
        /// 
        /// <para><note>Since the method aborts background thread, its usage is highly not preferred
        /// and should be done only if there are no other options. The correct way of stopping camera
        /// is <see cref="SignalToStop">signaling it stop</see> and then
        /// <see cref="WaitForStop">waiting</see> for background thread's completion.</note></para>
        /// </remarks>
        /// 
        public void Stop()
        {
            if (this.IsRunning)
            {
                SignalToStop();
                WaitForStop();
            }
        }

        /// <summary>
        ///   Worker thread.
        /// </summary>
        /// 
        private unsafe void WorkerThread()
        {
            try
            {
                needToStop = false;

                // Rearm the auto reset events
                for (int i = 0; i < inputReady.Length; i++)
                    inputReady[i].Reset();

                short[] buffer = new short[frameSize * channels];
                Signal signal = Signal.FromArray(buffer, channels, sampleRate, SampleFormat.Format16Bit);

                // Multiple sources
                while (!needToStop)
                {
                    mix(buffer, signal);
                }
            }
            catch (Exception ex)
            {
                if (AudioSourceError != null)
                    AudioSourceError(this, new AudioSourceErrorEventArgs(ex.Message));
                else throw;
            }
            finally
            {

            }
        }

        private unsafe void mix(short[] dst, Signal s)
        {
            for (int i = 0; i < signals.Length; i++)
            {
                inputReady[i].Wait();
                inputReady[i].Reset();
                outputReady.Reset();

                if (needToStop)
                    return;

                Signal inputSignal = signals[i];

                short* src = (short*)inputSignal.Data.ToPointer();

                if (inputSignal.NumberOfChannels < channels)
                {
                    for (int j = 0; j < frameSize; j += 2, src++)
                    {
                        dst[j] = mix(dst[j], *src);
                        dst[j + 1] = mix(dst[j + 1], *src);
                    }
                }
                else
                {
                    for (int j = 0; j < dst.Length; j++, src++)
                    {
                        dst[j] = mix(dst[j], *src);
                    }
                }
            }

            outputReady.Set();

            OnNewFrame(new NewFrameEventArgs(s));
        }

        unsafe private static short mix(short a, short b)
        {
            // From: http://atastypixel.com/blog/how-to-mix-audio-samples-properly-on-ios/

            if (a < 0 && b < 0)
                // If both samples are negative, mixed signal must have
                // an amplitude between the lesser of A and B, and the 
                // minimum permissible negative amplitude
                return (short)(((int)a + (int)b) - (((int)a * (int)b) / Int16.MinValue));

            else if (a > 0 && b > 0)
                // If both samples are positive, mixed signal must 
                // have an amplitude between the greater of A and B,
                // and the maximum permissible positive amplitude
                return (short)(((int)a + (int)b) - (((int)a * (int)b) / Int16.MaxValue));

            else
                // If samples are on opposite sides of the 0-crossing, 
                // mixed signal should reflect that samples cancel each
                // other out somewhat
                return (short)(a + b);
        }

        private void source_NewFrame(object sender, NewFrameEventArgs e)
        {
            if (needToStop)
                return;

            if (sources.Length == 1)
            {
                // Just relay the frame down to listeners
                OnNewFrame(e);
            }
            else
            {
                // Wait until we've read one frame from each source,
                // signal the mixer that the frame is ready, then wait
                // until the mixer has received and processed all frames
                // and signalled that the output is ready
                var source = sender as IAudioSource;
                int index = (int)source.UserData;

                signals[index] = e.Signal;

                inputReady[index].Set();
                outputReady.Wait();
            }
        }


        /// <summary>
        ///   Notifies client about new block of frames.
        /// </summary>
        /// 
        protected void OnNewFrame(NewFrameEventArgs args)
        {
            framesReceived++;

            if (NewFrame != null)
                NewFrame(this, args);
        }



        #region IDisposable members
        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref="AudioSourceMixer"/> is reclaimed by garbage collection.
        /// </summary>
        /// 
        ~AudioSourceMixer()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Performs application-defined tasks associated with
        ///   freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// 
        /// <param name="disposing">
        ///   <c>true</c> to release both managed and unmanaged resources;
        ///   <c>false</c> to release only unmanaged resources.</param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (inputReady != null)
                {
                    for (int i = 0; i < inputReady.Length; i++)
                    {
                        inputReady[i].Dispose();
                        inputReady[i] = null;
                    }
                }

                if (outputReady != null)
                    outputReady.Dispose();
            }

            outputReady = null;
            inputReady = null;
        }
        #endregion

    }
}
