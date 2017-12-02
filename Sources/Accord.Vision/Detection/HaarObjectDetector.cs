﻿// Accord Vision Library
// The Accord.NET Framework (LGPL) 
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Copyright © Masakazu Ohtsuka, 2008
//   This work is partially based on the original Project Marilena code,
//   distributed under a 2-clause BSD License. Details are listed below.
//
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//   * Redistribution's of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//
//   * Redistribution's in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//
// This software is provided by the copyright holders and contributors "as is" and
// any express or implied warranties, including, but not limited to, the implied
// warranties of merchantability and fitness for a particular purpose are disclaimed.
// In no event shall the Intel Corporation or contributors be liable for any direct,
// indirect, incidental, special, exemplary, or consequential damages
// (including, but not limited to, procurement of substitute goods or services;
// loss of use, data, or profits; or business interruption) however caused
// and on any theory of liability, whether in contract, strict liability,
// or tort (including negligence or otherwise) arising in any way out of
// the use of this software, even if advised of the possibility of such damage.
//

namespace Accord.Vision.Detection
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Accord.Imaging;
    using System.Drawing.Imaging;
    using Cascades;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Object detector options for the search procedure.
    /// </summary>
    /// 
    public enum ObjectDetectorSearchMode
    {
        /// <summary>
        ///   Entire image will be scanned.
        /// </summary>
        /// 
        Default = 0,

        /// <summary>
        ///   Only a single object will be retrieved.
        /// </summary>
        /// 
        Single,

        /// <summary>
        ///   If a object has already been detected inside an area,
        ///   it will not be scanned twice for inner or overlapping
        ///   objects, saving computation time.
        /// </summary>
        /// 
        NoOverlap,

        /// <summary>
        ///   If several objects are located within one another, 
        ///   they will be averaged. Additionally, objects which
        ///   have not been detected sufficient times may be dropped
        ///   by setting <see cref="HaarObjectDetector.Suppression"/>.
        /// </summary>
        /// 
        Average,
    }

    /// <summary>
    ///   Object detector options for window scaling.
    /// </summary>
    /// 
    public enum ObjectDetectorScalingMode
    {
        /// <summary>
        ///   Will start with a big search window and
        ///   gradually scale into smaller ones.
        /// </summary>
        /// 
        GreaterToSmaller,

        /// <summary>
        ///   Will start with small search windows and
        ///   gradually scale into greater ones.
        /// </summary>
        /// 
        SmallerToGreater,
    }

    /// <summary>
    ///   Viola-Jones Object Detector based on Haar-like features.
    /// </summary>
    /// <remarks>
    /// 
    /// <para>
    ///   The Viola-Jones object detection framework is the first object detection framework
    ///   to provide competitive object detection rates in real-time proposed in 2001 by Paul
    ///   Viola and Michael Jones. Although it can be trained to detect a variety of object
    ///   classes, it was motivated primarily by the problem of face detection.</para>
    ///   
    /// <para>
    ///   The implementation of this code has used Viola and Jones' original publication, the
    ///   OpenCV Library and the Marilena Project as reference. OpenCV is released under a BSD
    ///   license, it is free for both academic and commercial use. Please be aware that some
    ///   particular versions of the Haar object detection framework are patented by Viola and
    ///   Jones and may be subject to restrictions for use in commercial applications. The code
    ///   has been implemented with full support for tilted Haar features from the ground up.</para>
    ///   
    ///  <para>
    ///     References:
    ///     <list type="bullet">
    ///       <item><description>
    ///         <a href="http://www.cs.utexas.edu/~grauman/courses/spring2007/395T/papers/viola_cvpr2001.pdf">
    ///         Viola, P. and Jones, M. (2001). Rapid Object Detection using a Boosted Cascade
    ///         of Simple Features.</a></description></item>
    ///       <item><description>
    ///         <a href="http://en.wikipedia.org/wiki/Viola-Jones_object_detection_framework">
    ///         http://en.wikipedia.org/wiki/Viola-Jones_object_detection_framework </a>
    ///       </description></item>
    ///     </list>
    ///   </para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The first example shows how to detect faces from a single image using the detector.</para>
    ///   <code source="Unit Tests\Accord.Tests.Vision\ObjectDetectorTest.cs" region="doc_example"/>
    /// 
    /// <para>
    ///   The second example shows how to process an entire video using FileVideoReader class,
    ///   detecting faces from each frame, and saving those detections back to disk in the form
    ///   of individual frames and as a .mp4 file (using FileVideoWriter).</para>
    ///   <code source="Sources\Extras\Accord.Tests.Video.FFMPEG\ObjectDetectorTest.cs" region="doc_video"/>
    ///   <img src="..\images\video\haar_frame_24.png" />
    /// <para>
    ///   The <a href="https://1drv.ms/v/s!AoiTwBxoR4OAoLJhPozzixD25XcbiQ">generated video file can be found here</a>.</para>
    /// </example>
    /// 
    /// <seealso cref="FaceHaarCascade"/> 
    /// <seealso cref="NoseHaarCascade"/> 
    /// 
    public class HaarObjectDetector : IObjectDetector
    {

        private List<Rectangle> detectedObjects;
        private HaarClassifier classifier;

        private ObjectDetectorSearchMode searchMode = ObjectDetectorSearchMode.NoOverlap;
        private ObjectDetectorScalingMode scalingMode = ObjectDetectorScalingMode.GreaterToSmaller;

        // TODO: Support ROI
        //  private Rectangle searchWindow;

        private Size minSize = new Size(15, 15);
        private Size maxSize = new Size(500, 500);
        private float factor = 1.2f;
        private int channel = RGB.R;
        private bool parallel;

        private Rectangle[] lastObjects;
        private int steadyThreshold = 2;

        private int baseWidth;
        private int baseHeight;

        private int lastWidth;
        private int lastHeight;
        private float[] steps;

        private GroupMatching match;

        [NonSerialized]
        private IntegralImage2 integralImage;


        #region Constructors

        /// <summary>
        ///   Constructs a new Haar object detector.
        /// </summary>
        /// 
        /// <param name="cascade">
        ///   The <see cref="HaarCascade"/> to use in the detector's classifier.
        ///   For the default face cascade, please take a look on
        ///   <see cref="Cascades.FaceHaarCascade"/>.
        /// </param>
        /// 
        public HaarObjectDetector(HaarCascade cascade)
            : this(cascade, 15) { }

        /// <summary>
        ///   Constructs a new Haar object detector.
        /// </summary>
        /// 
        /// <param name="cascade">
        ///   The <see cref="HaarCascade"/> to use in the detector's classifier.
        ///   For the default face cascade, please take a look on
        ///   <see cref="Cascades.FaceHaarCascade"/>.</param>
        /// <param name="minSize">
        ///   Minimum window size to consider when searching for 
        ///   objects. Default value is <c>15</c>.</param>
        /// 
        public HaarObjectDetector(HaarCascade cascade, int minSize)
            : this(cascade, minSize, ObjectDetectorSearchMode.NoOverlap) { }

        /// <summary>
        ///   Constructs a new Haar object detector.
        /// </summary>
        /// 
        /// <param name="cascade">
        ///   The <see cref="HaarCascade"/> to use in the detector's classifier.
        ///   For the default face cascade, please take a look on
        ///   <see cref="Cascades.FaceHaarCascade"/>.
        /// </param>
        /// <param name="minSize">
        ///   Minimum window size to consider when searching for
        ///   objects. Default value is <c>15</c>.</param>
        /// <param name="searchMode">The <see cref="ObjectDetectorSearchMode"/> to use
        ///   during search. Please see documentation of <see cref="ObjectDetectorSearchMode"/>
        ///   for details. Default value is <see cref="ObjectDetectorSearchMode.NoOverlap"/></param>
        /// 
        public HaarObjectDetector(HaarCascade cascade, int minSize, ObjectDetectorSearchMode searchMode)
            : this(cascade, minSize, searchMode, 1.2f) { }

        /// <summary>
        ///   Constructs a new Haar object detector.
        /// </summary>
        /// 
        /// <param name="cascade">
        ///   The <see cref="HaarCascade"/> to use in the detector's classifier.
        ///   For the default face cascade, please take a look on
        ///   <see cref="Cascades.FaceHaarCascade"/>.</param>
        /// <param name="minSize">
        ///   Minimum window size to consider when searching for
        ///   objects. Default value is <c>15</c>.</param>
        /// <param name="searchMode">
        ///   The <see cref="ObjectDetectorSearchMode"/> to use
        ///   during search. Please see documentation of <see cref="ObjectDetectorSearchMode"/>
        ///   for details. Default value is <see cref="ObjectDetectorSearchMode.NoOverlap"/></param>
        /// <param name="scaleFactor">The re-scaling factor to use when re-scaling the window during search.</param>
        /// 
        public HaarObjectDetector(HaarCascade cascade, int minSize,
            ObjectDetectorSearchMode searchMode, float scaleFactor)
            : this(cascade, minSize, searchMode, scaleFactor, ObjectDetectorScalingMode.SmallerToGreater) { }

        /// <summary>
        ///   Constructs a new Haar object detector.
        /// </summary>
        /// 
        /// <param name="cascade">
        ///   The <see cref="HaarCascade"/> to use in the detector's classifier.
        ///   For the default face cascade, please take a look on
        ///   <see cref="Cascades.FaceHaarCascade"/>. </param>
        /// <param name="minSize">
        ///   Minimum window size to consider when searching for
        ///   objects. Default value is <c>15</c>.</param>
        /// <param name="searchMode">The <see cref="ObjectDetectorSearchMode"/> to use
        ///   during search. Please see documentation of <see cref="ObjectDetectorSearchMode"/>
        ///   for details. Default is <see cref="ObjectDetectorSearchMode.NoOverlap"/>.</param>
        /// <param name="scaleFactor">The scaling factor to rescale the window
        ///   during search. Default value is <c>1.2f</c>.</param>
        /// <param name="scalingMode">The <see cref="ObjectDetectorScalingMode"/> to use
        ///   when re-scaling the search window during search. Default is
        ///   <see cref="ObjectDetectorScalingMode.SmallerToGreater"/>.</param>
        /// 
        public HaarObjectDetector(HaarCascade cascade, int minSize,
            ObjectDetectorSearchMode searchMode, float scaleFactor,
            ObjectDetectorScalingMode scalingMode)
        {
            this.classifier = new HaarClassifier(cascade);
            this.minSize = new Size(minSize, minSize);
            this.searchMode = searchMode;
            this.ScalingMode = scalingMode;
            this.factor = scaleFactor;
            this.detectedObjects = new List<Rectangle>();

            this.baseWidth = cascade.Width;
            this.baseHeight = cascade.Height;

            this.match = new GroupMatching(0, 0.2);

#if NET35
            this.parallel = false;
#else
            this.parallel = true;
#endif
        }
        #endregion

        #region Properties


        /// <summary>
        ///   Gets or sets a value indicating whether this <see cref="HaarObjectDetector"/>
        ///   should scan the image using multiple threads. This setting can only be changed
        ///   to true on .NET version which support the Parallel Tasks framework (4.0+).
        /// </summary>
        /// 
        /// <value><c>true</c> to use multiple threads; otherwise, <c>false</c>.</value>
        /// 
        public bool UseParallelProcessing
        {
            get { return parallel; }
            set
            {
#if !NET35
                parallel = value;
#endif
            }
        }

        /// <summary>
        ///   Minimum window size to consider when searching objects.
        /// </summary>
        /// 
        public Size MinSize
        {
            get { return minSize; }
            set { minSize = value; }
        }

        /// <summary>
        ///   Maximum window size to consider when searching objects.
        /// </summary>
        /// 
        public Size MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; }
        }

        /// <summary>
        ///   Gets or sets the color channel to use when processing color images. 
        /// </summary>
        /// 
        public int Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        /// <summary>
        ///   Gets or sets the scaling factor to rescale the window during search.
        /// </summary>
        /// 
        public float ScalingFactor
        {
            get { return factor; }
            set
            {
                if (value != factor)
                {
                    factor = value;
                    steps = null;
                }
            }
        }

        /// <summary>
        ///   Gets or sets the desired searching method.
        /// </summary>
        /// 
        public ObjectDetectorSearchMode SearchMode
        {
            get { return searchMode; }
            set { searchMode = value; }
        }

        /// <summary>
        ///   Gets or sets the desired scaling method.
        /// </summary>
        /// 
        public ObjectDetectorScalingMode ScalingMode
        {
            get { return scalingMode; }
            set
            {
                if (value != scalingMode)
                {
                    scalingMode = value;
                    steps = null;
                }
            }
        }

        /// <summary>
        ///   Gets or sets the minimum threshold used to suppress rectangles which
        ///   have not been detected sufficient number of times. This property only
        ///   has effect when <see cref="SearchMode"/> is set to <see cref="ObjectDetectorSearchMode.Average"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   The value of this property represents the minimum amount of detections
        ///   made inside a region to report this region as an actual detection. For
        ///   example, setting this property to two will discard all regions which 
        ///   had not achieved at least two detected rectangles within it.</para>
        ///   
        /// <para>
        ///   Setting this property to a value higher than zero may decrease the
        ///   number of false positives.</para>
        /// </remarks>
        /// 
        public int Suppression
        {
            get { return match.MinimumNeighbors; }
            set { match.MinimumNeighbors = value; }
        }

        /// <summary>
        ///   Gets the detected objects bounding boxes.
        /// </summary>
        /// 
        public Rectangle[] DetectedObjects
        {
            get { return detectedObjects.ToArray(); }
        }

        /// <summary>
        ///   Gets the internal Cascade Classifier used by this detector.
        /// </summary>
        /// 
        public HaarClassifier Classifier
        {
            get { return classifier; }
        }

        /// <summary>
        ///   Gets how many frames the object has
        ///   been detected in a steady position.
        /// </summary>
        /// <value>
        ///   The number of frames the detected object
        ///   has been in a steady position.</value>
        ///   
        public int Steady { get; private set; }

        #endregion


        /// <summary>
        ///   Performs object detection on the given frame.
        /// </summary>
        /// 
        public Rectangle[] ProcessFrame(Bitmap frame)
        {
            return frame.LockBits(ImageLockMode.ReadOnly, (ui) => ProcessFrame(ui));
        }

        /// <summary>
        ///   Performs object detection on the given frame.
        /// </summary>
        /// 
        public Rectangle[] ProcessFrame(UnmanagedImage image)
        {
            int colorChannel =
              image.PixelFormat == PixelFormat.Format8bppIndexed ? 0 : channel;

            // Creates an integral image representation of the frame
            if (integralImage == null || integralImage.Width != image.Width || integralImage.Height != image.Height)
            {
                integralImage = IntegralImage2.FromBitmap(image, colorChannel, classifier.Cascade.HasTiltedFeatures);
            }
            else
            {
                integralImage.Update(image, colorChannel);
            }

            // Creates a new list of detected objects.
            this.detectedObjects.Clear();

            int width = integralImage.Width;
            int height = integralImage.Height;

            // Update parameters only if different size
            if (steps == null || width != lastWidth || height != lastHeight)
                update(width, height);


            Rectangle window = Rectangle.Empty;

            // For each scaling step
            for (int i = 0; i < steps.Length; i++)
            {
                float scaling = steps[i];

                // Set the classifier window scale
                classifier.Scale = scaling;

                // Get the scaled window size
                window.Width = (int)(baseWidth * scaling);
                window.Height = (int)(baseHeight * scaling);

                // Check if the window is lesser than the minimum size
                if (window.Width < minSize.Width || window.Height < minSize.Height)
                {
                    // If we are searching in greater to smaller mode,
                    if (scalingMode == ObjectDetectorScalingMode.GreaterToSmaller)
                    {
                        goto EXIT; // it won't get bigger, so we should stop.
                    }
                    else continue; // continue until it gets greater.
                }

                // Check if the window is greater than the maximum size
                else if (window.Width > maxSize.Width || window.Height > maxSize.Height)
                {
                    // If we are searching in greater to smaller mode,
                    if (scalingMode == ObjectDetectorScalingMode.GreaterToSmaller)
                    {
                        continue; // continue until it gets smaller.
                    }
                    else goto EXIT; // it won't get smaller, so we should stop.                    }
                }

                // Grab some scan loop parameters
                int xStep = window.Width >> 3;
                int yStep = window.Height >> 3;

                int xEnd = width - window.Width;
                int yEnd = height - window.Height;


                if (!parallel)  // Check if we should run in parallel
                {
                    // Sequential mode. Scan the integral image searching
                    // for objects in the window without parallelization.

                    // For every pixel in the window column
                    for (int y = 0; y < yEnd; y += yStep)
                    {
                        window.Y = y;

                        // For every pixel in the window row
                        for (int x = 0; x < xEnd; x += xStep)
                        {
                            window.X = x;

                            if (searchMode == ObjectDetectorSearchMode.NoOverlap && overlaps(window))
                                continue; // We have already detected something here, moving along.

                            // Try to detect an object inside the window
                            if (classifier.Compute(integralImage, window))
                            {
                                // object has been detected
                                detectedObjects.Add(window);

                                if (searchMode == ObjectDetectorSearchMode.Single)
                                    goto EXIT; // stop on first object found
                            }
                        }
                    }
                }

#if !NET35
                else // use parallel processing
                {
                    // Parallel mode. Scan the integral image searching
                    // for objects in the window with parallelization.
                    var bag = new System.Collections.Concurrent.ConcurrentBag<Rectangle>();

                    int numSteps = (int)Math.Ceiling((double)yEnd / yStep);

                    // For each pixel in the window column
                    Parallel.For(0, numSteps, (j, options) =>
                    {
                        int y = j * yStep;

                        // Create a local window reference
                        Rectangle localWindow = window;

                        localWindow.Y = y;

                        // For each pixel in the window row
                        for (int x = 0; x < xEnd; x += xStep)
                        {
                            if (options.ShouldExitCurrentIteration)
                                return;

                            localWindow.X = x;

                            // Try to detect and object inside the window
                            if (classifier.Compute(integralImage, localWindow))
                            {
                                // an object has been detected
                                bag.Add(localWindow);

                                if (searchMode == ObjectDetectorSearchMode.Single)
                                    options.Stop();
                            }
                        }
                    });

                    // If required, avoid adding overlapping objects at
                    // the expense of extra computation. Otherwise, only
                    // add objects to the detected objects collection.
                    if (searchMode == ObjectDetectorSearchMode.NoOverlap)
                    {
                        foreach (Rectangle obj in bag)
                            if (!overlaps(obj))
                                detectedObjects.Add(obj);
                    }
                    else if (searchMode == ObjectDetectorSearchMode.Single)
                    {
                        if (bag.TryPeek(out window))
                        {
                            detectedObjects.Add(window);
                            goto EXIT;
                        }
                    }
                    else
                    {
                        foreach (Rectangle obj in bag)
                            detectedObjects.Add(obj);
                    }
                }
#endif
            }


        EXIT:

            Rectangle[] objects = detectedObjects.ToArray();

            if (searchMode == ObjectDetectorSearchMode.Average)
                objects = match.Group(objects);

            checkSteadiness(objects);
            lastObjects = objects;

            return objects; // Returns the array of detected objects.
        }

        private void update(int width, int height)
        {
            List<float> listSteps = new List<float>();

            // Set initial parameters according to scaling mode
            if (scalingMode == ObjectDetectorScalingMode.SmallerToGreater)
            {
                float start = 1f;
                float stop = Math.Min(width / (float)baseWidth, height / (float)baseHeight);
                float step = factor;

                for (float f = start; f < stop; f *= step)
                    listSteps.Add(f);
            }
            else
            {
                float start = Math.Min(width / (float)baseWidth, height / (float)baseHeight);
                float stop = 1f;
                float step = 1f / factor;

                for (float f = start; f > stop; f *= step)
                    listSteps.Add(f);
            }

            steps = listSteps.ToArray();

            lastWidth = width;
            lastHeight = height;
        }

        private void checkSteadiness(Rectangle[] rectangles)
        {
            if (lastObjects == null ||
                rectangles == null ||
                rectangles.Length == 0)
            {
                Steady = 0;
                return;
            }

            bool equals = true;
            foreach (Rectangle current in rectangles)
            {
                bool found = false;
                foreach (Rectangle last in lastObjects)
                {
                    if (current.IsEqual(last, steadyThreshold))
                    {
                        found = true;
                        continue;
                    }
                }

                if (!found)
                {
                    equals = false;
                    break;
                }
            }

            if (equals)
                Steady++;

            else
                Steady = 0;
        }

        private bool overlaps(Rectangle rect)
        {
            foreach (Rectangle r in detectedObjects)
            {
                if (rect.IntersectsWith(r))
                    return true;
            }
            return false;
        }


    }
}
