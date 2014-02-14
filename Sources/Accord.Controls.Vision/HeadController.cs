// Accord Control Library
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

namespace Accord.Controls.Vision
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Windows.Forms;
    using Accord.Math;
    using Accord.Vision.Detection;
    using Accord.Vision.Detection.Cascades;
    using Accord.Vision.Tracking;
    using AForge;
    using AForge.Imaging;
    using AForge.Imaging.Filters;
    using AForge.Video;
    using AForge.Video.DirectShow;

    /// <summary>
    ///   Head-based tracking controller.
    /// </summary>
    /// 
    public class HeadController : Component,
        IBindableComponent, INotifyPropertyChanged, IVideoSource
    {

        private IntRange xaxisRange;
        private IntRange yaxisRange;
        private DoubleRange scaleRange;
        private DoubleRange angleRange;

        private IVideoSource videoSource;
        private HaarObjectDetector detector;
        private Camshift tracker;

        private ResizeNearestNeighbor resize = new ResizeNearestNeighbor(160, 120);

        private volatile bool requestedToStop;

        private PointF currentPosition;
        private float currentAngle;
        private float currentScale;

        private int skip = 0;

        private ISynchronizeInvoke synchronizingObject;



        #region Properties

        /// <summary>
        ///   Gets the <see cref="IObjectTracker"/> used to
        ///   track the head object in the video stream.
        /// </summary>
        /// 
        /// <value>The active object tracker.</value>
        /// 
        [Browsable(false)]
        public IObjectTracker Tracker
        {
            get { return tracker; }
        }

        /// <summary>
        ///   Gets the <see cref="IObjectDetector"/> used to
        ///   detect the head object in the video stream.
        /// </summary>
        /// 
        /// <value>The active object detector.</value>
        /// 
        [Browsable(false)]
        public IObjectDetector Detector
        {
            get { return detector; }
        }

        /// <summary>
        ///   Gets the current head position.
        /// </summary>
        /// 
        [Browsable(false)]
        public PointF HeadPosition
        {
            get { return currentPosition; }
        }

        /// <summary>
        ///   Gets the current head tilting angle.
        /// </summary>
        /// 
        [Browsable(false)]
        public float HeadAngle
        {
            get { return currentAngle; }
        }

        /// <summary>
        ///   Gets the current head scale.
        /// </summary>
        /// 
        [Browsable(false)]
        public float HeadScale
        {
            get { return currentScale; }
        }

        /// <summary>
        ///   Gets or sets the maximum position
        ///   for horizontal scale calibration.
        /// </summary>
        /// 
        [Category("Calibration"), DefaultValue(128)]
        public int XAxisMax
        {
            get { return xaxisRange.Max; }
            set
            {
                xaxisRange.Max = value;
                OnPropertyChanged("XAxisMax");
            }
        }

        /// <summary>
        ///   Gets or sets the minimum position
        ///   for horizontal scale calibration.
        /// </summary>
        /// 
        [Category("Calibration"), DefaultValue(180)]
        public int XAxisMin
        {
            get { return xaxisRange.Min; }
            set
            {
                xaxisRange.Min = value;
                OnPropertyChanged("XAxisMin");
            }
        }

        /// <summary>
        ///   Gets or sets the maximum position
        ///   for vertical scale calibration.
        /// </summary>
        /// 
        [Category("Calibration"), DefaultValue(100)]
        public int YAxisMax
        {
            get { return yaxisRange.Max; }
            set
            {
                yaxisRange.Max = value;
                OnPropertyChanged("YAxisMax");
            }
        }

        /// <summary>
        ///   Gets or sets the minimum position
        ///   for vertical scale calibration.
        /// </summary>
        /// 
        [Category("Calibration"), DefaultValue(150)]
        public int YAxisMin
        {
            get { return yaxisRange.Min; }
            set
            {
                yaxisRange.Min = value;
                OnPropertyChanged("YAxisMin");
            }
        }

        /// <summary>
        ///   Gets or sets the maximum area
        ///   for proximity scale calibration.
        /// </summary>
        /// 
        [Category("Calibration"), DefaultValue(140)]
        public double ScaleMax
        {
            get { return scaleRange.Max; }
            set
            {
                scaleRange.Max = value;
                OnPropertyChanged("ScaleMax");
            }
        }

        /// <summary>
        ///   Gets or sets the minimum area
        ///   for proximity scale calibration.
        /// </summary>
        /// 
        [Category("Calibration"), DefaultValue(90)]
        public double ScaleMin
        {
            get { return scaleRange.Min; }
            set
            {
                scaleRange.Min = value;
                OnPropertyChanged("ScaleMin");
            }
        }

        /// <summary>
        ///    Gets or sets the maximum angle
        ///    for head tilting calibration.
        /// </summary>
        /// 
        [Category("Calibration"), DefaultValue(1.3290820121765137)]
        public double AngleMax
        {
            get { return angleRange.Max; }
            set
            {
                angleRange.Max = value;
                OnPropertyChanged("AngleMax");
            }
        }

        /// <summary>
        ///    Gets or sets the minimum angle
        ///    for head tilting calibration.
        /// </summary>
        /// 
        [Category("Calibration"), DefaultValue(1.83104407787323)]
        public double AngleMin
        {
            get { return angleRange.Min; }
            set
            {
                angleRange.Min = value;
                OnPropertyChanged("AngleMin");
            }
        }


        /// <summary>
        ///   Gets or sets the object used to marshal event-handler calls that
        ///   are issued when the head object position has been updated.
        /// </summary>
        /// 
        /// <value>The <see cref="ISynchronizeInvoke"/> representing the object
        /// used to marshal the event-handler calls that are issued when the head
        /// position has been updated. The default is null.</value>
        /// 
        [Browsable(false), DefaultValue(null)]
        public ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                if (this.synchronizingObject == null && base.DesignMode)
                {
                    IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));

                    if (designerHost != null)
                    {
                        var rootComponent = designerHost.RootComponent as ISynchronizeInvoke;

                        if (rootComponent != null)
                        {
                            this.synchronizingObject = rootComponent;
                        }
                    }
                }
                return this.synchronizingObject;
            }
            set
            {
                this.synchronizingObject = value;
            }
        }

        /// <summary>
        ///   Gets or sets the video device used to track objects.
        /// </summary>
        /// 
        /// <value>The <see cref="VideoCaptureDevice"/> used to track objects.</value>
        /// 
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IVideoSource Device
        {
            get { return videoSource; }
            set
            {
                if (value != null)
                {
                    if (videoSource != null)
                    {
                        videoSource.NewFrame -= source_NewFrame;
                        videoSource.PlayingFinished -= videoSource_PlayingFinished;
                        videoSource.VideoSourceError -= videoSource_VideoSourceError;
                    }

                    videoSource = value;
                    videoSource.NewFrame += source_NewFrame;
                    videoSource.PlayingFinished += videoSource_PlayingFinished;
                    videoSource.VideoSourceError += videoSource_VideoSourceError;

                    VideoCaptureDevice captureDevice = videoSource as VideoCaptureDevice;

                    if (captureDevice != null)
                        captureDevice.VideoResolution = selectResolution(captureDevice);
                }
            }
        }

        private static VideoCapabilities selectResolution(VideoCaptureDevice device)
        {
            foreach (var cap in device.VideoCapabilities)
            {
                if (cap.FrameSize.Height == 240)
                    return cap;
                if (cap.FrameSize.Width == 320)
                    return cap;
            }

            return device.VideoCapabilities.Last();
        }

        /// <summary>
        ///   Gets a value indicating whether this instance is
        ///   attempting to detect faces in the video stream.
        /// </summary>
        /// 
        /// <value>
        /// 	<c>true</c> if this instance is detecting; otherwise, <c>false</c>.
        /// </value>
        /// 
        [Browsable(false)]
        public bool IsDetecting { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether this instance is
        ///   actually tracking an object in the video stream.
        /// </summary>
        /// 
        /// <value>
        /// 	<c>true</c> if this instance is tracking; otherwise, <c>false</c>.
        /// </value>
        /// 
        [Browsable(false)]
        public bool IsTracking { get; private set; }
        #endregion


        #region Events

        /// <summary>
        ///   Occurs when the head moves in the video scene.
        /// </summary>
        public event EventHandler<HeadEventArgs> HeadMove;

        /// <summary>
        ///   Occurs when a head enters the video scene.
        /// </summary>
        public event EventHandler<HeadEventArgs> HeadEnter;

        /// <summary>
        ///   Occurs when a head leaves the video scene.
        /// </summary>
        public event EventHandler<EventArgs> HeadLeave;



        #endregion


        /// <summary>
        ///   Initializes a new instance of the <see cref="HeadController"/> class.
        /// </summary>
        /// 
        public HeadController()
        {
            // Setup tracker
            tracker = new Camshift();
            tracker.Mode = CamshiftMode.RGB;
            tracker.Conservative = true;
            tracker.AspectRatio = 1.5f;

            // Setup detector
            detector = new HaarObjectDetector(new FaceHaarCascade());
            detector.MinSize = new Size(25, 25);
            detector.SearchMode = ObjectDetectorSearchMode.Single;
            detector.ScalingFactor = 1.2f;
            detector.ScalingMode = ObjectDetectorScalingMode.GreaterToSmaller;

            xaxisRange = new IntRange(0, 320);
            yaxisRange = new IntRange(0, 240);
            scaleRange = new DoubleRange(0, Math.Sqrt(320 * 240));
            angleRange = new DoubleRange(0, 2 * Math.PI);
        }


        #region Public methods

        /// <summary>
        ///   Resets the controller.
        /// </summary>
        /// 
        public void Reset()
        {
            if (IsTracking)
                OnHeadLeave(new HeadEventArgs(currentPosition, currentAngle, currentScale));

            IsTracking = false;
            IsDetecting = true;
        }

        /// <summary>
        ///   Starts processing the video source.
        /// </summary>
        /// 
        public void Start()
        {
            if (videoSource == null) return;

            requestedToStop = false;
            skip = 0;

            IsTracking = false;
            IsDetecting = true;

            videoSource.Start();
        }

        /// <summary>
        ///   Gets a value indicating whether this instance
        ///   is currently processing and sending events.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is processing
        /// 	and sending events; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool IsRunning
        {
            get { return (videoSource != null) ? videoSource.IsRunning : false; }
        }

        /// <summary>
        ///   Stops the video source.
        /// </summary>
        /// 
        public void Stop()
        {
            requestedToStop = true;

            if (videoSource != null)
            {
                videoSource.Stop();
            }
        }

        /// <summary>
        ///   Signal the video source to stop.
        /// </summary>
        /// 
        public void SignalToStop()
        {
            requestedToStop = true;

            if (videoSource != null)
            {
                videoSource.SignalToStop();
            }
        }

        /// <summary>
        ///   Waits until the video source has stopped.
        /// </summary>
        /// 
        public void WaitForStop()
        {
            if (!requestedToStop)
            {
                SignalToStop();
            }

            if (videoSource != null)
            {
                videoSource.WaitForStop();
            }
        }

        /// <summary>
        ///   Calibrates the specified movement using current positions.
        /// </summary>
        /// <param name="movement">The movement to be calibrated.</param>
        /// 
        public virtual void Calibrate(HeadMovement movement)
        {
            switch (movement)
            {
                case HeadMovement.Left:
                    XAxisMin = tracker.TrackingObject.Center.X;
                    break;

                case HeadMovement.Right:
                    XAxisMax = tracker.TrackingObject.Center.X;
                    break;

                case HeadMovement.Up:
                    YAxisMax = tracker.TrackingObject.Center.Y;
                    break;

                case HeadMovement.Down:
                    YAxisMin = tracker.TrackingObject.Center.Y;
                    break;

                case HeadMovement.Forward:
                    ScaleMax = Math.Sqrt(tracker.TrackingObject.Area);
                    break;

                case HeadMovement.Backward:
                    ScaleMin = Math.Sqrt(tracker.TrackingObject.Area);
                    break;

                case HeadMovement.TiltLeft:
                    AngleMin = tracker.TrackingObject.Angle;
                    break;

                case HeadMovement.TiltRight:
                    AngleMax = tracker.TrackingObject.Angle;
                    break;
            }
        }
        #endregion


        #region Private methods

        private readonly object syncObject = new Object();

        private void source_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (requestedToStop)
                return;

            if (!IsTracking && !IsDetecting)
                return;


            lock (syncObject)
            {
                // skip first frames during initialization
                if (skip < 10) { skip++; return; }

                Bitmap frame = eventArgs.Frame;

                int width = frame.Width;
                int height = frame.Height;

                BitmapData data = frame.LockBits(new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadWrite, frame.PixelFormat);

                UnmanagedImage image = new UnmanagedImage(data);

                if (IsDetecting)
                {
                    // Reduce frame size to process it faster
                    float xscale = (float)width / resize.NewWidth;
                    float yscale = (float)height / resize.NewHeight;

                    UnmanagedImage downsample = resize.Apply(image);

                    // Process the face detector in the downsampled image
                    Rectangle[] regions = detector.ProcessFrame(downsample);

                    // Check if the face has been steady 5 frames in a row
                    if (detector.Steady >= 5)
                    {
                        // Yes, so track the face
                        Rectangle face = regions[0];

                        // Reduce the face size to avoid tracking background
                        Rectangle window = new Rectangle(
                            (int)((face.X + face.Width / 2f) * xscale),
                            (int)((face.Y + face.Height / 2f) * yscale),
                            1, 1);

                        window.Inflate((int)(0.25f * face.Width * xscale),
                                       (int)(0.40f * face.Height * yscale));

                        // Re-initialize tracker
                        tracker.Reset();
                        tracker.SearchWindow = window;
                        tracker.ProcessFrame(image);

                        // Update initial position
                        computeCurrentPosition();

                        OnHeadEnter(new HeadEventArgs(currentPosition, currentAngle, currentScale));
                    }
                }

                else if (IsTracking)
                {
                    tracker.Extract = (NewFrame != null);

                    // Track the object
                    tracker.ProcessFrame(image);

                    // Get the object position
                    TrackingObject obj = tracker.TrackingObject;

                    // Update current position
                    computeCurrentPosition();

                    if (obj.IsEmpty || !tracker.IsSteady)
                    {
                        OnHeadLeave(EventArgs.Empty);
                    }

                    else
                    {
                        OnHeadMove(new HeadEventArgs(currentPosition, currentAngle, currentScale));

                        if (NewFrame != null && obj.Image != null)
                        {
                            Bitmap headFrame = obj.Image.ToManagedImage();
                            NewFrame(this, new NewFrameEventArgs(headFrame));
                        }
                    }
                }

                frame.UnlockBits(data);
            }
        }

        private void computeCurrentPosition()
        {
            TrackingObject obj = tracker.TrackingObject;

            DoubleRange unit = new DoubleRange(-1, 1);
            DoubleRange circle = new DoubleRange(Math.PI, 0);

            currentPosition.X = (float)Tools.Scale(xaxisRange, unit, obj.Center.X);
            currentPosition.Y = (float)Tools.Scale(yaxisRange, unit, obj.Center.Y);
            currentAngle = (float)Tools.Scale(angleRange, circle, obj.Angle);
            currentScale = (float)Tools.Scale(scaleRange, unit, Math.Sqrt(obj.Area));
        }


        #endregion


        #region Protected methods
        /// <summary>
        ///   Called when a head movement is detected.
        /// </summary>
        /// 
        protected virtual void OnHeadMove(HeadEventArgs args)
        {
            if (HeadMove != null)
            {
                if (SynchronizingObject != null &&
                    SynchronizingObject.InvokeRequired)
                {
                    SynchronizingObject.BeginInvoke(
                        HeadMove, new object[] { this, args });
                }
                else
                {
                    HeadMove(this, args);
                }
            }
        }



        /// <summary>
        ///   Called when the face being tracked leaves the scene.
        /// </summary>
        /// 
        protected virtual void OnHeadLeave(EventArgs args)
        {
            IsTracking = false;
            IsDetecting = true;

            if (HeadLeave != null)
            {
                if (SynchronizingObject != null &&
                    SynchronizingObject.InvokeRequired)
                {
                    SynchronizingObject.BeginInvoke(
                        HeadLeave, new object[] { this, args });
                }
                else
                {
                    HeadLeave(this, args);
                }
            }
        }

        /// <summary>
        ///   Called when a face enters the scene.
        /// </summary>
        /// 
        protected virtual void OnHeadEnter(HeadEventArgs args)
        {
            IsTracking = true;
            IsDetecting = false;

            if (HeadEnter != null)
            {
                if (SynchronizingObject != null &&
                    SynchronizingObject.InvokeRequired)
                {
                    SynchronizingObject.BeginInvoke(
                        HeadEnter, new object[] { this, args });
                }
                else
                {
                    HeadEnter(this, args);
                }
            }
        }
        #endregion



        #region IBindableComponent Implementation
        private BindingContext bindingContext;
        private ControlBindingsCollection dataBindings;

        /// <summary>
        /// Gets or sets the collection of currency managers for the <see cref="T:System.Windows.Forms.IBindableComponent"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The collection of <see cref="T:System.Windows.Forms.BindingManagerBase"/> objects for this <see cref="T:System.Windows.Forms.IBindableComponent"/>.
        /// </returns>
        [Browsable(false)]
        public BindingContext BindingContext
        {
            get
            {
                if (bindingContext == null)
                    bindingContext = new BindingContext();
                return bindingContext;
            }
            set { bindingContext = value; }
        }

        /// <summary>
        /// Gets the collection of data-binding objects for this <see cref="T:System.Windows.Forms.IBindableComponent"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.Windows.Forms.ControlBindingsCollection"/> for this <see cref="T:System.Windows.Forms.IBindableComponent"/>.
        /// </returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ControlBindingsCollection DataBindings
        {
            get
            {
                if (dataBindings == null)
                    dataBindings = new ControlBindingsCollection(this);
                return dataBindings;
            }
        }
        #endregion

        #region INotifyPropertyChanged Implementation

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region IVideoSource Members

        /// <summary>
        /// Received bytes count.
        /// </summary>
        /// <value></value>
        /// <remarks>Number of bytes the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        [Browsable(false)]
        public long BytesReceived
        {
            get { return videoSource.BytesReceived; }
        }

        /// <summary>
        /// Received frames count.
        /// </summary>
        /// <value></value>
        /// <remarks>Number of frames the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        [Browsable(false)]
        public int FramesReceived
        {
            get { return videoSource.FramesReceived; }
        }

        /// <summary>
        /// Video source.
        /// </summary>
        /// <value></value>
        /// <remarks>The meaning of the property depends on particular video source.
        /// Depending on video source it may be a file name, URL or any other string
        /// describing the video source.</remarks>
        /// 
        [Browsable(false)]
        public string Source
        {
            get { return "HeadController"; }
        }

        private void videoSource_VideoSourceError(object sender, VideoSourceErrorEventArgs eventArgs)
        {
            if (VideoSourceError != null)
            {
                if (SynchronizingObject != null &&
                    SynchronizingObject.InvokeRequired)
                {
                    SynchronizingObject.BeginInvoke(
                        VideoSourceError, new object[] { this, eventArgs });
                }
                else
                {
                    VideoSourceError(this, eventArgs);
                }
            }
        }

        private void videoSource_PlayingFinished(object sender, ReasonToFinishPlaying reason)
        {
            if (PlayingFinished != null)
            {
                if (SynchronizingObject != null &&
                    SynchronizingObject.InvokeRequired)
                {
                    SynchronizingObject.BeginInvoke(
                        PlayingFinished, new object[] { this, reason });
                }
                else
                {
                    PlayingFinished(this, reason);
                }
            }
        }

        /// <summary>
        /// Video playing finished event.
        /// </summary>
        /// <remarks>
        /// This event is used to notify clients that the video playing has finished.
        /// </remarks>
        /// 
        public event PlayingFinishedEventHandler PlayingFinished;

        /// <summary>
        /// Video source error event.
        /// </summary>
        /// <remarks>This event is used to notify clients about any type of errors occurred in
        /// video source object, for example internal exceptions.</remarks>
        /// 
        public event VideoSourceErrorEventHandler VideoSourceError;

        /// <summary>
        /// New frame event.
        /// </summary>
        /// <remarks><para>This event is used to notify clients about new available video frame.</para>
        /// 	<para><note>Since video source may have multiple clients, each client is responsible for
        /// making a copy (cloning) of the passed video frame, but video source is responsible for
        /// disposing its own original copy after notifying of clients.</note></para>
        /// </remarks>
        /// 
        public event NewFrameEventHandler NewFrame;

        #endregion


        /// <summary>
        ///   Releases the unmanaged resources used by the 
        ///   <see cref="T:System.ComponentModel.Component"/> 
        ///   and optionally releases the managed resources.
        /// </summary>
        /// 
        /// <param name="disposing">true to release both managed and unmanaged 
        ///   resources; false to release only unmanaged resources.</param>
        ///   
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
