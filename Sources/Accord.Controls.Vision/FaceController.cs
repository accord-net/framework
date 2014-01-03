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
    using System.Windows.Forms;
    using Accord.Math;
    using Accord.Statistics.Moving;
    using Accord.Vision.Detection;
    using Accord.Vision.Detection.Cascades;
    using Accord.Vision.Tracking;
    using AForge;
    using AForge.Imaging;
    using AForge.Video;

    /// <summary>
    ///   Face-based tracking controller.
    /// </summary>
    /// 
    public class FaceController : Component,
        IBindableComponent, INotifyPropertyChanged
    {

        private PointF rawPosition;
        private PointF currentPosition;

        private DoubleRange xaxisRange = new DoubleRange(+0.65, -0.65);
        private DoubleRange yaxisRange = new DoubleRange(+0.35, -0.10);

        private IVideoSource videoSource;
        private MatchingTracker tracker;
        private HaarObjectDetector detector;

        private ISynchronizeInvoke synchronizingObject;

        private MovingNormalStatistics xsmooth = new MovingNormalStatistics(5);
        private MovingNormalStatistics ysmooth = new MovingNormalStatistics(5);


        #region Properties

        /// <summary>
        ///   Gets or sets the object used to marshal event-handler calls that
        ///   are issued when the head object position has been updated.
        /// </summary>
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
        /// <value>The <see cref="IVideoSource"/> used to track objects.</value>
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
                        videoSource.NewFrame -= source_NewFrame;

                    Reset();

                    videoSource = value;
                    videoSource.NewFrame += source_NewFrame;
                }
            }
        }

        /// <summary>
        ///   Gets the <see cref="IObjectTracker"/> used to
        ///   track the head object in the video stream.
        /// </summary>
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
        /// <value>The active object detector.</value>
        /// 
        [Browsable(false)]
        public IObjectDetector Detector
        {
            get { return detector; }
        }

        /// <summary>
        ///   Gets the current face's center point.
        /// </summary>
        /// 
        [Browsable(false)]
        public PointF FacePosition
        {
            get { return currentPosition; }
        }


        /// <summary>
        ///   Gets or sets the maximum position
        ///   for horizontal scale calibration.
        /// </summary>
        /// 
        [Category("Calibration"), DefaultValue(-0.65)]
        public double XAxisMax
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
        [Category("Calibration"), DefaultValue(0.65)]
        public double XAxisMin
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
        [Category("Calibration"), DefaultValue(-0.10)]
        public double YAxisMax
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
        [Category("Calibration"), DefaultValue(0.35)]
        public double YAxisMin
        {
            get { return yaxisRange.Min; }
            set
            {
                yaxisRange.Min = value;
                OnPropertyChanged("YAxisMin");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether this instance is
        ///   attempting to detect faces in the video stream.
        /// </summary>
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
        /// <value>
        /// 	<c>true</c> if this instance is tracking; otherwise, <c>false</c>.
        /// </value>
        /// 
        [Browsable(false)]
        public bool IsTracking { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether this instance
        ///   is currently processing and sending events.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is processing
        /// 	and sending events; otherwise, <c>false</c>.
        /// </value>
        /// 
        [Browsable(false)]
        public bool IsRunning { get; private set; }
        #endregion


        #region Events
        /// <summary>
        ///   Occurs when the face moves in the video scene.
        /// </summary>
        public event EventHandler<FaceEventArgs> FaceMove;

        /// <summary>
        ///   Occurs when a face enters the video scene.
        /// </summary>
        public event EventHandler<FaceEventArgs> FaceEnter;

        /// <summary>
        ///   Occurs when a face leaves the video scene.
        /// </summary>
        public event EventHandler<EventArgs> FaceLeave;
        #endregion



        /// <summary>
        ///   Initializes a new instance of the <see cref="FaceController"/> class.
        /// </summary>
        /// 
        public FaceController()
        {
            // Setup tracker
            tracker = new MatchingTracker();
            tracker.Extract = false;

            // Setup detector
            detector = new HaarObjectDetector(new NoseHaarCascade());
            detector.ScalingFactor = 1.1f;
            detector.SearchMode = ObjectDetectorSearchMode.Single;
            detector.ScalingMode = ObjectDetectorScalingMode.SmallerToGreater;
            detector.MinSize = new Size(2, 5);
            //detector.MaxSize = new Size(15, 18);
        }



        #region Public methods

        /// <summary>
        ///   Calibrates the specified movement using current positions.
        /// </summary>
        /// <param name="movement">The movement to be calibrated.</param>
        public virtual void Calibrate(FaceMovement movement)
        {
            switch (movement)
            {
                case FaceMovement.Left:
                    XAxisMin = rawPosition.X;
                    break;

                case FaceMovement.Right:
                    XAxisMax = rawPosition.X;
                    break;

                case FaceMovement.Up:
                    YAxisMax = rawPosition.Y;
                    break;

                case FaceMovement.Down:
                    YAxisMin = rawPosition.Y;
                    break;
            }
        }

        /// <summary>
        ///   Starts processing the source 
        ///   stream and sending events.
        /// </summary>
        /// 
        public void Start()
        {
            IsRunning = true;
        }

        /// <summary>
        ///   Stops sending events.
        /// </summary>
        /// 
        public void Stop()
        {
            IsRunning = false;
            Reset();
        }

        /// <summary>
        ///   Resets the controller.
        /// </summary>
        /// 
        public void Reset()
        {
            if (IsTracking)
                OnFaceLeave(EventArgs.Empty);

            IsTracking = false;
            IsDetecting = true;
        }
        #endregion


        #region Private methods
        private Object syncObject = new Object();

        private void source_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            if (!IsRunning || (!IsTracking && !IsDetecting))
                return;


            lock (syncObject)
            {
                Bitmap frame = eventArgs.Frame;

                int width = frame.Width;
                int height = frame.Height;

                BitmapData data = frame.LockBits(new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadWrite, frame.PixelFormat);

                UnmanagedImage head = new UnmanagedImage(data);


                if (IsDetecting)
                {
                    IsDetecting = false;

                    // Process the nose detector in the head image
                    Rectangle[] regions = detector.ProcessFrame(head);

                    if (regions.Length >= 1)
                    {
                        // Re-initialize tracker
                        tracker.Reset();
                        tracker.SearchWindow = regions[0];
                        tracker.ProcessFrame(head);

                        // Update initial position
                        computeCurrentPosition(width, height);

                        OnFaceEnter(new FaceEventArgs(currentPosition));
                    }
                    else
                    {
                        IsDetecting = true;
                    }
                }

                else if (IsTracking)
                {
                    // Track the object
                    tracker.ProcessFrame(head);

                    // Get the object position
                    TrackingObject obj = tracker.TrackingObject;

                    // Update current position
                    computeCurrentPosition(width, height);

                    if (obj.IsEmpty)
                    {
                        OnFaceLeave(EventArgs.Empty);
                    }

                    else
                    {
                        OnFaceMove(new FaceEventArgs(currentPosition));
                    }
                }

                frame.UnlockBits(data);
            }
        }

        private void computeCurrentPosition(int width, int height)
        {
            TrackingObject obj = tracker.TrackingObject;

            DoubleRange scaleX = new DoubleRange(0, width);
            DoubleRange scaleY = new DoubleRange(0, height);
            DoubleRange unit = new DoubleRange(-1, 1);

            rawPosition.X = (float)Tools.Scale(scaleX, unit, obj.Center.X);
            rawPosition.Y = (float)Tools.Scale(scaleY, unit, obj.Center.Y);

            double newPositionX = Tools.Scale(xaxisRange, unit, rawPosition.X);
            double newPositionY = Tools.Scale(yaxisRange, unit, rawPosition.Y);

            xsmooth.Push(newPositionX);
            ysmooth.Push(newPositionY);

            newPositionX = xsmooth.Mean;
            newPositionY = ysmooth.Mean;

            currentPosition.X = (float)(Math.Round(newPositionX, 1));
            currentPosition.Y = (float)(Math.Round(newPositionY, 1));
        }
        #endregion


        #region Protected methods
        /// <summary>
        ///   Called when the face being tracked leaves the scene.
        /// </summary>
        protected virtual void OnFaceLeave(EventArgs args)
        {
            IsTracking = false;
            IsDetecting = true;

            if (FaceLeave != null)
            {
                if (SynchronizingObject != null &&
                    SynchronizingObject.InvokeRequired)
                {
                    SynchronizingObject.BeginInvoke(
                        FaceLeave, new object[] { this, args });
                }
                else
                {
                    FaceLeave(this, args);
                }
            }
        }

        /// <summary>
        ///   Called when a face enters the scene.
        /// </summary>
        protected virtual void OnFaceEnter(FaceEventArgs args)
        {
            IsTracking = true;
            IsDetecting = false;

            if (FaceEnter != null)
            {
                if (SynchronizingObject != null &&
                    SynchronizingObject.InvokeRequired)
                {
                    SynchronizingObject.BeginInvoke(
                        FaceEnter, new object[] { this, args });
                }
                else
                {
                    FaceEnter(this, args);
                }
            }
        }

        /// <summary>
        ///   Called when a head movement is detected.
        /// </summary>
        protected virtual void OnFaceMove(FaceEventArgs args)
        {
            if (FaceMove != null)
            {
                if (SynchronizingObject != null &&
                    SynchronizingObject.InvokeRequired)
                {
                    SynchronizingObject.BeginInvoke(
                        FaceMove, new object[] { this, args });
                }
                else
                {
                    FaceMove(this, args);
                }
            }
        }
        #endregion


        #region IBindableComponent Implementation
        private BindingContext bindingContext;
        private ControlBindingsCollection dataBindings;

        /// <summary>
        ///   Gets or sets the collection of currency managers for the <see cref="T:System.Windows.Forms.IBindableComponent"/>.
        /// </summary>
        /// 
        /// <returns>
        ///   The collection of <see cref="T:System.Windows.Forms.BindingManagerBase"/>
        ///   objects for this <see cref="T:System.Windows.Forms.IBindableComponent"/>.
        /// </returns>
        /// 
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
        ///   Gets the collection of data-binding objects for this <see cref="T:System.Windows.Forms.IBindableComponent"/>.
        /// </summary>
        /// 
        /// <returns>
        ///   The <see cref="T:System.Windows.Forms.ControlBindingsCollection"/> for this <see cref="T:System.Windows.Forms.IBindableComponent"/>.
        /// </returns>
        /// 
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
        /// 
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        /// 
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
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
