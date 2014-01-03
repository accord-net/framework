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

namespace Accord.DirectSound
{
    using System;
    using System.Threading;
    using Accord.Audio;
    using SlimDX.DirectSound;
    using SlimDX.Multimedia;
    using System.Collections.Generic;


    /// <summary>
    ///   Audio output device for local audio playback (i.e. a sound card port).
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>This <see cref="IAudioOutput">audio output</see> sends audio data
    ///   to a local output device such as a sound card. The audio is reproduced
    ///   using DirectSound through SlimDX.</para>
    ///   
    ///   <para>For instructions on how to list output devices, please see
    ///   the <see cref="AudioDeviceCollection"/> documentation page.</para>
    /// </remarks>
    /// 
    /// <example>
    ///   <para>Sample usage:</para>
    ///   
    ///   <code>
    ///   // To create an audio output device, DirectSound requires a handle to
    ///   // the parent form of the application (or other application handle). In
    ///   // Windows.Forms, this could be achieved by providing the Handle property
    ///   // of the currently displayed form.
    ///   
    ///   int sampleRate = 22000; // 22kHz
    ///   int channels = 2;       // stereo
    ///   
    ///   // Create the audio output device with the desired values
    ///   AudioOutputDevice output = new AudioOutputDevice(Handle, sampleRate, channels);
    ///   
    ///   // The output device works at real time, and as such, forms a queue of audio
    ///   // samples to be played (more specifically, a buffer). When this buffer starts
    ///   // to get empty, the output will ask the application for more samples of it
    ///   // should stop playing. To ask for more samples, the output device will fire
    ///   // an event which should be handled by the user:
    ///
    ///   output.NewFrameRequested += output_NewFrameRequested;
    ///   
    ///   // It is also possible to configure an event to be fired when the device
    ///   // has stopped playing and when it has just started playing a frame. Those
    ///   // are mainly used for reporting status to GUI controls.
    ///   output.Stopped += output_Stopped;
    ///   output.FramePlayingStarted += output_FramePlayingStarted;
    ///   
    ///   // Start playing
    ///   output.Play();
    ///   </code>
    ///   
    ///   <para>For more details regarding usage, please check one of 
    ///   the Audio sample applications accompanying the framework. </para>
    /// </example>
    /// 
    /// <seealso cref="AudioDeviceCollection"/>
    /// <seealso cref="AudioCaptureDevice"/>
    /// 
    public class AudioOutputDevice : IAudioOutput, IDisposable
    {
        private IntPtr owner;
        private Guid device = Guid.Empty;
        private SecondarySoundBuffer buffer;
        private int samplingRate;
        private int bufferSize;
        private int channels;

        private Thread thread;

        private NotificationPosition[] notifications;
        private WaitHandle[] waitHandles;

        private bool isPlaying;
        private bool stop;

        int firstHalfBufferIndex;
        int secondHalfBufferIndex;

        /// <summary>
        ///   Gets a value indicating whether this instance is playing audio.
        /// </summary>
        /// 
        /// <value>
        ///   <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool IsRunning
        {
            get { return isPlaying; }
        }

        /// <summary>
        ///   Gets the sampling rate for the current output device.
        /// </summary>
        /// 
        public int SamplingRate
        {
            get { return samplingRate; }
        }

        /// <summary>
        ///   Gets the number of channels for the current output device.
        /// </summary>
        /// 
        public int Channels
        {
            get { return channels; }
        }

        /// <summary>
        ///   Gets the parent owner form for the device.
        /// </summary>
        /// 
        public IntPtr Owner
        {
            get { return owner; }
        }

        /// <summary>
        ///   Audio output.
        /// </summary>
        /// 
        /// <remarks>Audio output is represented by Guid of audio output device.</remarks>
        /// 
        public virtual string Output
        {
            get { return device.ToString(); }
            set { device = new Guid(value); }
        }

        /// <summary>
        ///   Raised when a frame starts playing.
        /// </summary>
        /// 
        public event EventHandler<PlayFrameEventArgs> FramePlayingStarted;

        /// <summary>
        ///   Raised when a frame starts playing.
        /// </summary>
        /// 
        public event EventHandler<NewFrameRequestedEventArgs> NewFrameRequested;

        /// <summary>
        ///   Indicates all frames have been played and the audio finished.
        /// </summary>
        /// 
        public event EventHandler Stopped;

        /// <summary>
        ///   Audio output error event.
        /// </summary>
        /// 
        /// <remarks>This event is used to notify clients about any type of errors occurred in
        /// audio output object, for example internal exceptions.</remarks>
        /// 
        public event EventHandler<AudioOutputErrorEventArgs> AudioOutputError;

        /// <summary>
        ///   Constructs a new Audio Output Device.
        /// </summary>
        /// 
        /// <param name="owner">The owner window handle.</param>
        /// <param name="samplingRate">The sampling rate of the device.</param>
        /// <param name="channels">The number of channels of the device.</param>
        /// 
        public AudioOutputDevice(IntPtr owner, int samplingRate, int channels)
            : this(Guid.Empty, owner, samplingRate, channels) { }

        /// <summary>
        ///   Constructs a new Audio Output Device.
        /// </summary>
        /// 
        /// <param name="device">Global identifier of the audio output device.</param>
        /// <param name="owner">The owner window handle.</param>
        /// <param name="samplingRate">The sampling rate of the device.</param>
        /// <param name="channels">The number of channels of the device.</param>
        /// 
        public AudioOutputDevice(Guid device, IntPtr owner, int samplingRate, int channels)
        {
            this.owner = owner;
            this.samplingRate = samplingRate;
            this.channels = channels;
            this.device = device;

            DirectSound ds = new DirectSound(device);
            ds.SetCooperativeLevel(owner, CooperativeLevel.Priority);


            // Set the output format
            WaveFormat waveFormat = new WaveFormat();
            waveFormat.FormatTag = WaveFormatTag.IeeeFloat;
            waveFormat.BitsPerSample = 32;
            waveFormat.BlockAlignment = (short)(waveFormat.BitsPerSample * channels / 8);
            waveFormat.Channels = (short)channels;
            waveFormat.SamplesPerSecond = samplingRate;
            waveFormat.AverageBytesPerSecond = waveFormat.SamplesPerSecond * waveFormat.BlockAlignment;

            bufferSize = 8 * waveFormat.AverageBytesPerSecond;


            // Setup the secondary buffer
            SoundBufferDescription desc2 = new SoundBufferDescription();
            desc2.Flags =
                BufferFlags.GlobalFocus |
                BufferFlags.ControlPositionNotify |
                BufferFlags.GetCurrentPosition2;
            desc2.SizeInBytes = bufferSize;
            desc2.Format = waveFormat;

            buffer = new SecondarySoundBuffer(ds, desc2);


            var list = new List<NotificationPosition>();
            int numberOfPositions = 32;

            // Set notification for buffer percentiles
            for (int i = 0; i < numberOfPositions; i++)
            {
                list.Add(new NotificationPosition()
                {
                    Event = new AutoResetEvent(false),
                    Offset = i * bufferSize / numberOfPositions + 1,
                });
            }

            // Set notification for end of buffer
            list.Add(new NotificationPosition()
            {
                Offset = bufferSize - 1,
                Event = new AutoResetEvent(false)
            });

            firstHalfBufferIndex = numberOfPositions / 2;
            secondHalfBufferIndex = numberOfPositions;

            notifications = list.ToArray();

            System.Diagnostics.Debug.Assert(notifications[firstHalfBufferIndex].Offset == bufferSize / 2 + 1);
            System.Diagnostics.Debug.Assert(notifications[secondHalfBufferIndex].Offset == bufferSize - 1);

            // Make a copy of the wait handles
            waitHandles = new WaitHandle[notifications.Length];
            for (int i = 0; i < notifications.Length; i++)
                waitHandles[i] = notifications[i].Event;

            // Store all notification positions
            buffer.SetNotificationPositions(notifications);
        }


        /// <summary>
        ///   Starts playing the current buffer.
        /// </summary>
        /// 
        public void Play(float[] samples)
        {
            if (isPlaying) return;

            // Start playing and exit.
            buffer.Write(samples, 0, LockFlags.None);

            new Thread(() =>
            {
                OnFramePlayingStarted(new PlayFrameEventArgs(0, samples.Length));

                buffer.Play(0, PlayFlags.None);

                waitHandles[secondHalfBufferIndex].WaitOne();

                OnStopped(EventArgs.Empty);

            }).Start();
        }

        /// <summary>
        ///   Starts playing the current buffer.
        /// </summary>
        /// 
        public void Play()
        {
            if (thread == null)
            {
                // check source
                if (device == null)
                    throw new ArgumentException("Audio output is not specified");

                isPlaying = true;

                // create events
                stop = false;

                // create and start new thread
                thread = new Thread(WorkerThread);
                thread.Start();
            }
        }

        /// <summary>
        ///   Worker thread.
        /// </summary>
        /// 
        private void WorkerThread()
        {
            int samples = bufferSize / sizeof(float);

            try
            {

                // The first write should fill the entire buffer.
                var request = new NewFrameRequestedEventArgs(samples);
                NewFrameRequested.Invoke(this, request);

                buffer.Write(request.Buffer, 0, LockFlags.None);

                // The buffer starts playing.
                buffer.Play(0, PlayFlags.Looping);

                int framesPlayed = request.FrameIndex;
                int lastNotificationLocation = 0;
                bool requestedToStop = false;

                while (!stop)
                {
                    int positionIndex = WaitHandle.WaitAny(waitHandles);

                    if (stop) break;


                    if (positionIndex == firstHalfBufferIndex ||
                        positionIndex == secondHalfBufferIndex)
                    {
                        if (requestedToStop) break;

                        // When the first half of the buffer has finished
                        //  playing and we have just started playing the
                        //  second half, we will write the next samples in
                        //  the first half of the buffer again.

                        // When the second half of the buffer has finished
                        //  playing, the first half of the buffer will
                        //  start playing again (since this is a circular
                        //  buffer). At this time, we can write the next
                        //  samples in the second half of the buffer.

                        request.Frames = samples / 2;
                        NewFrameRequested(this, request);
                        requestedToStop = request.Stop;

                        int offset = (positionIndex == firstHalfBufferIndex) ? 0 : bufferSize / 2;

                        buffer.Write(request.Buffer, 0, request.Frames, offset, LockFlags.None);
                    }

                    if (positionIndex != secondHalfBufferIndex)
                    {
                        // Notify progress
                        int currentBufferLocation = notifications[positionIndex].Offset;

                        if (lastNotificationLocation >= currentBufferLocation)
                            lastNotificationLocation = -(bufferSize - lastNotificationLocation);

                        int delta = (currentBufferLocation - lastNotificationLocation);

                        framesPlayed += delta / sizeof(float);

                        lastNotificationLocation = currentBufferLocation;

                        OnFramePlayingStarted(new PlayFrameEventArgs(framesPlayed, delta));
                    }
                }
            }
            catch (Exception ex)
            {
                if (AudioOutputError != null)
                    AudioOutputError(this, new AudioOutputErrorEventArgs(ex.Message));
                else throw;
            }
            finally
            {
                buffer.Stop();

                OnStopped(EventArgs.Empty);
            }
        }

        private void OnFramePlayingStarted(PlayFrameEventArgs e)
        {
            isPlaying = true;

            if (FramePlayingStarted != null)
                FramePlayingStarted(this, e);
        }

        private void OnStopped(EventArgs e)
        {
            isPlaying = false;

            if (Stopped != null)
                Stopped(this, e);
        }

        /// <summary>
        ///   Stops playing the current buffer.
        /// </summary>
        /// 
        public void Stop()
        {
            if (this.IsRunning)
            {
                if (thread != null)
                    thread.Abort();
                WaitForStop();
            }
        }

        /// <summary>
        ///   Signals audio output to stop its work.
        /// </summary>
        /// 
        /// <remarks>Signals audio output to stop its background thread, stop to
        /// ask for new frames and free resources.</remarks>
        /// 
        public void SignalToStop()
        {
            // stop thread
            if (thread != null)
            {
                stop = true;

                for (int i = 0; i < notifications.Length; i++)
                    (notifications[i].Event as AutoResetEvent).Set();
            }
        }

        /// <summary>
        ///   Wait for audio output has stopped.
        /// </summary>
        /// 
        /// <remarks>Waits for output stopping after it was signaled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        /// 
        public void WaitForStop()
        {
            if (thread != null)
            {
                for (int i = 0; i < notifications.Length; i++)
                    (notifications[i].Event as AutoResetEvent).Set();

                // wait for thread stop
                thread.Join();

                thread = null;
            }
        }

        #region IDisposable members
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
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged
        /// resources; <c>false</c> to release only unmanaged resources.</param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (buffer != null)
                {
                    buffer.Dispose();
                    buffer = null;
                }
            }
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref="WaveFileAudioSource"/> is reclaimed by garbage collection.
        /// </summary>
        /// 
        ~AudioOutputDevice()
        {
            Dispose(false);
        }
        #endregion


    }
}
