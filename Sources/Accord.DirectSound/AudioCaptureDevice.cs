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
    ///   Audio source for local audio capture device (i.e. a microphone).
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>This <see cref="IAudioSource">audio source</see> captures audio data
    ///   obtained from a local audio capture device such as the microphone. The audio
    ///   is captured using DirectSound through SlimDX.</para>
    ///   
    ///   <para>For instructions on how to list capture devices, please see
    ///   the <see cref="AudioDeviceCollection"/> documentation page.</para>
    /// </remarks>
    /// 
    /// <example>
    ///   <para>Sample usage:</para>
    ///   
    ///   <code>
    ///   // Create default capture device
    ///   AudioCaptureDevice source = new AudioCaptureDevice();
    ///  
    ///   // Specify capturing options
    ///   source.DesiredFrameSize = 4096;
    ///   source.SampleRate = 22050;
    ///  
    ///   // Specify the callback function which will be
    ///   // called once a sample is completely available
    ///   source.NewFrame += source_NewFrame;
    ///
    ///   // Start capturing
    ///   source.Start();
    ///   
    ///   // ...
    ///   
    ///   // The callback function should determine what
    ///   // should be done with the samples being caught
    ///   private void source_NewFrame(object sender, NewFrameEventArgs eventArgs)
    ///   {
    ///       // Read current frame...
    ///       Signal s = eventArgs.Signal;
    ///
    ///       // Process/play/record it
    ///       // ...
    ///   }
    ///   </code>
    ///   
    ///   <para>For more details regarding usage, please check one of 
    ///   the Audio sample applications accompanying the framework. </para>
    /// </example>
    /// 
    /// <seealso cref="AudioDeviceCollection"/>
    /// <seealso cref="AudioOutputDevice"/>
    /// 
    public class AudioCaptureDevice : IAudioSource, IDisposable
    {

        // moniker string of audio capture device
        private Guid device = Guid.Empty;
        private string sourceName;

        // user data associated with the audio source
        private object userData = null;

        // received frames count
        private int framesReceived;

        // received byte count
        private int bytesReceived;

        // specifies desired capture frame size
        private int desiredCaptureSize = 4096;

        // specifies the sample rate used in the source
        private int sampleRate = 44100;

        private Thread thread = null;
        private ManualResetEvent stopEvent = null;

        private SampleFormat sampleFormat = SampleFormat.Format32BitIeeeFloat;

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
        ///   Audio source.
        /// </summary>
        /// 
        /// <remarks>Audio source is represented by Guid of audio capture device.</remarks>
        /// 
        public virtual string Source
        {
            get { return sourceName; }
            set { sourceName = value; }
        }

        /// <summary>
        ///   Gets or sets the sample format used by the device.
        /// </summary>
        /// 
        public SampleFormat Format
        {
            get { return sampleFormat; }
            set { sampleFormat = value; }
        }

        /// <summary>
        ///   Gets or sets the desired frame size.
        /// </summary>
        public int DesiredFrameSize
        {
            get { return desiredCaptureSize; }
            set { desiredCaptureSize = value; }
        }

        /// <summary>
        ///   Gets the number of audio channels captured by
        ///   the device. Currently, only a single channel 
        ///   is supported.
        /// </summary>
        /// 
        public int Channels
        {
            get { return 1; }
            set { }
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
        public object UserData
        {
            get { return userData; }
            set { userData = value; }
        }

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

                    // the thread is not running, free resources
                    Free();
                }
                return false;
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="AudioCaptureDevice"/> class.
        /// </summary>
        /// 
        public AudioCaptureDevice()
        {
            this.device = Guid.Empty;
            this.sourceName = "Default capture device";
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="AudioCaptureDevice"/> class.
        /// </summary>
        /// 
        public AudioCaptureDevice(AudioDeviceInfo device)
        {
            this.device = device.Guid;
            this.sourceName = device.Description;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="AudioCaptureDevice"/> class.
        /// </summary>
        /// 
        /// <param name="device">Global identifier of the audio capture device.</param>
        /// 
        public AudioCaptureDevice(Guid device)
        {
            this.device = device;
            this.sourceName = device.ToString();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="AudioCaptureDevice"/> class.
        /// </summary>
        /// 
        /// <param name="device">Global identifier of the audio capture device.</param>
        /// <param name="name">The device name or description string.</param>
        /// 
        public AudioCaptureDevice(Guid device, string name)
        {
            this.device = device;
            this.sourceName = name;
        }

        /// <summary>
        ///   Start audio source.
        /// </summary>
        /// 
        /// <remarks>Starts audio source and return execution to caller. audio source
        /// object creates background thread and notifies about new frames with the
        /// help of <see cref="NewFrame"/> event.</remarks>
        /// 
        public void Start()
        {
            if (thread == null)
            {
                // check source
                if (device == null)
                    throw new ArgumentException("Audio source is not specified");

                framesReceived = 0;
                bytesReceived = 0;

                // create events
                stopEvent = new ManualResetEvent(false);

                // create and start new thread
                thread = new Thread(WorkerThread);
                thread.Name = device.ToString();
                thread.Start();
            }
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
                stopEvent.Set();
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

                Free();
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
                thread.Abort();
                WaitForStop();
            }
        }

        /// <summary>
        ///   Free resource.
        /// </summary>
        /// 
        private void Free()
        {
            thread = null;

            // release events
            stopEvent.Close();
            stopEvent = null;
        }



        /// <summary>
        ///   Worker thread.
        /// </summary>
        /// 
        private void WorkerThread()
        {
            // Get the selected capture device
            DirectSoundCapture captureDevice = new DirectSoundCapture(device);


            // Set the capture format
            WaveFormat format = new WaveFormat();
            format.Channels = 1;
            format.SamplesPerSecond = sampleRate;
            format.FormatTag = sampleFormat.ToWaveFormat();
            format.BitsPerSample = (short)Signal.GetSampleSize(sampleFormat);
            format.BlockAlignment = (short)(format.BitsPerSample / 8);
            format.AverageBytesPerSecond = format.SamplesPerSecond * format.BlockAlignment;

            // Setup the capture buffer
            CaptureBufferDescription captureBufferDescription = new CaptureBufferDescription();
            captureBufferDescription.Format = format;
            captureBufferDescription.BufferBytes = 2 * desiredCaptureSize * format.BlockAlignment;
            captureBufferDescription.WaveMapped = true;
            captureBufferDescription.ControlEffects = false;

            CaptureBuffer captureBuffer = null;
            NotificationPosition[] notifications = new NotificationPosition[2];

            try
            {
                captureBuffer = new CaptureBuffer(captureDevice, captureBufferDescription);

                // Setup the notification positions
                int bufferPortionSize = captureBuffer.SizeInBytes / 2;
                notifications[0] = new NotificationPosition();
                notifications[0].Offset = bufferPortionSize - 1;
                notifications[0].Event = new AutoResetEvent(false);
                notifications[1] = new NotificationPosition();
                notifications[1].Offset = bufferPortionSize - 1 + bufferPortionSize;
                notifications[1].Event = new AutoResetEvent(false);
                captureBuffer.SetNotificationPositions(notifications);

                // Make a copy of the wait handles
                WaitHandle[] waitHandles = new WaitHandle[notifications.Length];
                for (int i = 0; i < notifications.Length; i++)
                    waitHandles[i] = notifications[i].Event;



                // Start capturing
                captureBuffer.Start(true);


                if (sampleFormat == SampleFormat.Format32BitIeeeFloat)
                {
                    float[] currentSample = new float[desiredCaptureSize];

                    while (!stopEvent.WaitOne(0, true))
                    {
                        int bufferPortionIndex = WaitHandle.WaitAny(waitHandles);
                        captureBuffer.Read(currentSample, 0, currentSample.Length, bufferPortionSize * bufferPortionIndex);
                        OnNewFrame(currentSample);
                    }
                }
                else if (sampleFormat == SampleFormat.Format16Bit)
                {
                    short[] currentSample = new short[desiredCaptureSize];

                    while (!stopEvent.WaitOne(0, true))
                    {
                        int bufferPortionIndex = WaitHandle.WaitAny(waitHandles);
                        captureBuffer.Read(currentSample, 0, currentSample.Length, bufferPortionSize * bufferPortionIndex);
                        OnNewFrame(currentSample);
                    }
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
                if (captureBuffer != null)
                {
                    captureBuffer.Stop();
                    captureBuffer.Dispose();
                }

                if (captureDevice != null)
                    captureDevice.Dispose();

                for (int i = 0; i < notifications.Length; i++)
                    if (notifications[i].Event != null)
                        notifications[i].Event.Close();
            }
        }



        /// <summary>
        ///   Notifies client about new block of frames.
        /// </summary>
        /// 
        /// <param name="frame">New frame's audio.</param>
        /// 
        protected void OnNewFrame(Array frame)
        {
            framesReceived++;

            if ((!stopEvent.WaitOne(0, true)) && (NewFrame != null))
            {
                NewFrame(this, new NewFrameEventArgs(Signal.FromArray(frame, sampleRate, sampleFormat)));
            }
        }


        /// <summary>
        ///   Gets whether this audio source supports seeking.
        /// </summary>
        /// 
        public bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        ///    This source does not support seeking.
        /// </summary>
        /// 
        public void Seek(int frameIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Gets or sets the desired sample rate for this capturing device.
        /// </summary>
        /// 
        public int SampleRate
        {
            get { return this.sampleRate; }
            set { this.sampleRate = value; }
        }


        #region IDisposable members
        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref="AudioCaptureDevice"/> is reclaimed by garbage collection.
        /// </summary>
        /// 
        ~AudioCaptureDevice()
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
                if (stopEvent != null)
                {
                    stopEvent.Close();
                    stopEvent = null;
                }
            }
        }
        #endregion

    }
}