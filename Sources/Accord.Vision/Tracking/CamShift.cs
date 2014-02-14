// Accord Vision Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
// Copyright © Benjamin Jung, 2009
//   This work is partially based on the original FaceIt library,
//   distributed under a MIT License. Details are listed below.
//
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

namespace Accord.Vision.Tracking
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Accord.Imaging.Moments;
    using Accord.Statistics.Moving;
    using AForge;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   Modes for the Camshift Tracker.
    /// </summary>
    /// 
    public enum CamshiftMode
    {
        /// <summary>
        ///   By choosing RGB, the tracker will process raw high-intensity RGB values.
        /// </summary>
        /// 
        RGB,

        /// <summary>
        ///   By choosing HSL, the tracker will perform a RGB-to-HSL conversion and use the Hue value instead.
        /// </summary>
        /// 
        HSL,

        /// <summary>
        ///   By choosing Mixed, the tracker will use HSL with some lightness information.
        /// </summary>
        /// 
        Mixed,
    }

    /// <summary>
    ///   Continuously Adaptive Mean Shift (Camshift) Object Tracker
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Camshift stands for "Continuously Adaptive Mean Shift". It combines the basic
    ///   Mean Shift algorithm with an adaptive region-sizing step. The kernel is a step
    ///   function applied to a probability map. The probability of each image pixel is
    ///   based on color using a method called histogram backprojection.</para>
    /// <para>
    ///   The implementation of this code has used Gary Bradski's original publication,
    ///   the OpenCV Library and the FaceIt implementation as references. The OpenCV
    ///   library is distributed under a BSD license. FaceIt is distributed under a MIT
    ///   license. The original licensing terms for FaceIt are described in the source
    ///   code and in the Copyright.txt file accompanying the framework.</para>  
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       G.R. Bradski, Computer video face tracking for use in a perceptual user interface,
    ///       Intel Technology Journal, Q2 1998. Available on:
    ///       <a href="ftp://download.intel.com/technology/itj/q21998/pdf/camshift.pdf">
    ///       ftp://download.intel.com/technology/itj/q21998/pdf/camshift.pdf </a></description></item>
    ///     <item><description>
    ///       R. Hewitt, Face tracking project description: Camshift Algorithm. Available on:
    ///       <a href="http://www.robinhewitt.com/research/track/camshift.html">
    ///       http://www.robinhewitt.com/research/track/camshift.html </a></description></item>
    ///     <item><description>
    ///       OpenCV Computer Vision Library. Available on:
    ///       <a href="http://sourceforge.net/projects/opencvlibrary/">
    ///       http://sourceforge.net/projects/opencvlibrary/ </a></description></item>
    ///     <item><description>
    ///       FaceIt object tracking in Flash AS3. Available on:
    ///       <a href="http://www.libspark.org/browser/as3/FaceIt">
    ///       http://www.libspark.org/browser/as3/FaceIt </a></description></item>
    ///  </list></para>  
    /// </remarks>
    /// 
    public class Camshift : IObjectTracker
    {

        private float[,] map; // object probability map

        // create histogram as float to avoid unnecessary casting
        //  when computing the histogram ratio for backprojection.
        private float[] originalHistogram;

        private Rectangle searchWindow;
        private TrackingObject trackingObject;
        private bool conservative = true;

        private bool smooth = true;
        private bool extract = false;
        private float aspectRatio = 0f;

        private CamshiftMode mode = CamshiftMode.RGB;

        // Parameters for HSV operation
        private DoubleRange hslSaturation = new DoubleRange(0.1, 0.9);
        private DoubleRange hslLuminance = new DoubleRange(0.1, 0.9);

        private int histogramSize = 4096;

        // max mean shift iterations
        private const int MAX_ITERATIONS = 10;

        private IMovingStatistics angleHistory;

        private IMovingStatistics yHistory;
        private IMovingStatistics xHistory;
        private IMovingStatistics widthHistory;
        private IMovingStatistics heightHistory;

        private int unstableCounter;

        private Object sync = new Object();


        #region Constructors

        /// <summary>
        ///   Constructs a new Camshift tracking algorithm.
        /// </summary>
        /// 
        public Camshift()
        {
            initialize(null, Rectangle.Empty, mode);
        }

        /// <summary>
        ///   Constructs a new Camshift tracking algorithm.
        /// </summary>
        /// 
        public Camshift(UnmanagedImage frame, Rectangle objectArea)
        {
            initialize(frame, objectArea, mode);
        }

        /// <summary>
        ///   Constructs a new Camshift tracking algorithm.
        /// </summary>
        /// 
        public Camshift(Rectangle objectArea)
        {
            initialize(null, objectArea, mode);
        }

        /// <summary>
        ///   Constructs a new Camshift tracking algorithm.
        /// </summary>
        /// 
        public Camshift(UnmanagedImage frame, Rectangle objectArea, CamshiftMode colorMode)
        {
            initialize(frame, objectArea, colorMode);
        }

        /// <summary>
        ///   Constructs a new Camshift tracking algorithm.
        /// </summary>
        /// 
        public Camshift(Rectangle objectArea, CamshiftMode colorMode)
        {
            initialize(null, objectArea, colorMode);
        }

        private void initialize(UnmanagedImage frame, Rectangle objectArea, CamshiftMode colorMode)
        {
            this.trackingObject = new TrackingObject();
            this.searchWindow = objectArea;
            this.mode = colorMode;

            this.angleHistory = new MovingCircularStatistics(4);

            this.widthHistory = new MovingNormalStatistics(15);
            this.heightHistory = new MovingNormalStatistics(15);
            this.yHistory = new MovingNormalStatistics(15);
            this.xHistory = new MovingNormalStatistics(15);

            if (frame != null && objectArea != Rectangle.Empty)
            {
                this.originalHistogram = createHistogram(frame, objectArea);
            }
        }
        #endregion


        #region Properties
        /// <summary>
        ///   Gets or sets the current search window.
        /// </summary>
        /// 
        public Rectangle SearchWindow
        {
            get { return searchWindow; }
            set { searchWindow = value; }
        }

        /// <summary>
        ///   Gets or sets the desired window aspect ratio.
        /// </summary>
        /// 
        public float AspectRatio
        {
            get { return aspectRatio; }
            set { aspectRatio = value; }
        }
        /// <summary>
        ///   Gets or sets the mode of operation for this tracker.
        /// </summary>
        /// 
        public CamshiftMode Mode
        {
            get { return this.mode; }
            set { this.mode = value; }
        }

        /// <summary>
        ///   If using HSL mode, specifies the operational saturation range for the tracker.
        /// </summary>
        /// 
        public DoubleRange HslSaturationRange
        {
            get { return hslSaturation; }
        }

        /// <summary>
        ///   If using HSL mode, specifies the operational lightness range for the tracker.
        /// </summary>
        /// 
        public DoubleRange HslLightnessRange
        {
            get { return hslLuminance; }
        }

        /// <summary>
        ///   Gets the location of the object being tracked.
        /// </summary>
        /// 
        public TrackingObject TrackingObject
        {
            get { return trackingObject; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the tracker
        ///   should extract the object image from the source. The
        ///   extracted image will be available in <see cref="Accord.Vision.Tracking.TrackingObject.Image"/>.
        /// </summary>
        /// 
        public bool Extract
        {
            get { return extract; }
            set { extract = value; }
        }

        /// <summary>
        ///   Probability map
        /// </summary>
        /// 
        public float[,] Map
        {
            get { return map; }
        }

        /// <summary>
        ///   Gets or sets whether the algorithm should scan only the
        ///   active window or the entire image for histogram ratio.
        /// </summary>
        /// 
        public bool Conservative
        {
            get { return conservative; }
            set { conservative = value; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the angular
        ///   movements should be smoothed using a moving average.
        /// </summary>
        /// <value><c>true</c> to smooth angular movements; otherwise, <c>false</c>.</value>
        /// 
        public bool Smooth
        {
            get { return smooth; }
            set { smooth = value; }
        }

        /// <summary>
        ///   Gets whether the tracking object is
        ///   showing little variation of fluctuation.
        /// </summary>
        /// <value><c>true</c> if the tracking object is steady; otherwise, <c>false</c>.</value>
        /// 
        public bool IsSteady { get; private set; }
        #endregion


        /// <summary>
        ///   Resets the object tracking algorithm.
        /// </summary>
        /// 
        public void Reset()
        {
            this.unstableCounter = 0;

            this.originalHistogram = null;
            this.IsSteady = false;
            this.trackingObject.Reset();

            this.angleHistory.Clear();
            this.heightHistory.Clear();
            this.widthHistory.Clear();
            this.xHistory.Clear();
            this.yHistory.Clear();
        }


        #region Backprojection generation
        /// <summary>
        ///   Generates a image of the histogram back projection
        /// </summary>
        public Bitmap GetBackprojection()
        {
            return GetBackprojection(PixelFormat.Format8bppIndexed);
        }

        /// <summary>
        ///   Generates a image of the histogram backprojection
        /// </summary>
        public unsafe Bitmap GetBackprojection(PixelFormat format)
        {
            return GetBackprojection(format, new Rectangle(0, 0,
                map.GetLength(1), map.GetLength(0)));
        }

        /// <summary>
        ///   Generates a image of the histogram backprojection
        /// </summary>
        public Bitmap GetBackprojection(PixelFormat format, Rectangle rectangle)
        {
            int height = rectangle.Height;
            int width = rectangle.Width;

            Bitmap bitmap;

            if (format == PixelFormat.Format8bppIndexed)
                bitmap = AForge.Imaging.Image.CreateGrayscaleImage(width, height);
            else
                bitmap = new Bitmap(width, height, format);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, format);

            GetBackprojection(new UnmanagedImage(data), rectangle);

            bitmap.UnlockBits(data);
            return bitmap;
        }

        /// <summary>
        ///   Generates a image of the histogram backprojection
        /// </summary>
        public unsafe void GetBackprojection(UnmanagedImage image, Rectangle rectangle)
        {
            lock (sync)
            {
                int height = rectangle.Height;
                int width = rectangle.Width;
                int startx = rectangle.Left;
                int starty = rectangle.Top;

                int srcOffset = map.GetLength(1) - rectangle.Width;
                PixelFormat format = image.PixelFormat;

                int pixelSize = Bitmap.GetPixelFormatSize(format) / 8;
                int stride = image.Stride;
                int offset = stride - width * pixelSize;


                // Do work
                fixed (float* map_ptr = &map[starty, startx])
                {
                    byte* dst = (byte*)image.ImageData.ToPointer();
                    float* src = map_ptr;

                    // Check if image is grayscale (8bpp)
                    if (format == PixelFormat.Format8bppIndexed)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++, dst++, src++)
                            {
                                // probability map contains values between 0 and 1
                                *dst = (byte)Math.Floor(255f * (*src));
                            }
                            dst += offset;
                            src += srcOffset;
                        }
                    }

                    else // Image is 24bpp
                    {
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++, dst += pixelSize, src++)
                            {
                                // probability map contains values between 0 and 1
                                byte value = (byte)Math.Floor(255f * (*src));
                                *(dst + 0) = value;
                                *(dst + 1) = value;
                                *(dst + 2) = value;
                            }
                            dst += offset;
                            src += srcOffset;
                        }
                    }
                }
            }
        }
        #endregion


        /// <summary>
        ///   Processes a new video frame.
        /// </summary>
        /// 
        public void ProcessFrame(UnmanagedImage frame)
        {
            // Check if the tracker has been initialized
            if (this.originalHistogram == null)
            {
                // This frame contains the object we are trying to track
                this.originalHistogram = createHistogram(frame, searchWindow);
            }
            else
            {
                // We are going to try to find the object in the frame
                this.camshift(frame);
            }
        }


        /// <summary>
        ///   Camshift algorithm
        /// </summary>
        /// 
        private void camshift(UnmanagedImage frame)
        {
            int width = frame.Width;
            int height = frame.Height;

            Rectangle area = new Rectangle(0, 0, width, height);

            // Compute tracking object center
            float objX = Math.Max(0, Math.Min(searchWindow.X + searchWindow.Width * 0.5f, width));
            float objY = Math.Max(0, Math.Min(searchWindow.Y + searchWindow.Height * 0.5f, height));
            float objAngle;


            // Compute mean shift
            CentralMoments moments = meanShift(frame);

            SizeF objSize = moments.GetSizeAndOrientation(out objAngle);


            if (Single.IsNaN(objSize.Width) || Single.IsNaN(objSize.Height) ||
                Single.IsNaN(objAngle) || objSize.Width < 1 || objSize.Height < 1)
            {
                Reset();
                return;
            }

            // Truncate to integer coordinates
            IntPoint center = new IntPoint((int)objX, (int)objY);

            Rectangle rec = new Rectangle((int)(objX - objSize.Width * 0.5f),
                                          (int)(objY - objSize.Height * 0.5f),
                                          (int)objSize.Width, (int)objSize.Height);

            angleHistory.Push(objAngle);

            // Create tracking object
            IsSteady = checkSteadiness();
            trackingObject.Rectangle = rec;
            trackingObject.Center = center;
            trackingObject.Angle = smooth ? (float)angleHistory.Mean : objAngle;

            if (extract)
            {
                Rectangle inner = rec;

                xHistory.Push(rec.X);
                yHistory.Push(rec.Y);
                widthHistory.Push(rec.Width);
                heightHistory.Push(rec.Height);

                inner.X = (int)xHistory.Mean;
                inner.Y = (int)yHistory.Mean;
                inner.Width = (int)widthHistory.Mean;
                inner.Height = (int)heightHistory.Mean;

                inner.Intersect(area);

                Crop crop = new Crop(inner);

                // TODO: Perform rotation of the extracted object
                //RotateNearestNeighbor rotate = new RotateNearestNeighbor((objAngle - Math.PI / 2) * 180f / Math.PI, true);
                //trackingObject.Image = rotate.Apply(crop.Apply(frame));

                trackingObject.Image = crop.Apply(frame);
            }

            // Compute a new search window size
            searchWindow.Width = (int)(1.1f * objSize.Width);
            searchWindow.Height = (int)((aspectRatio != 0) ?
                (aspectRatio * objSize.Width) : (1.1f * objSize.Height));
        }

        /// <summary>
        ///   Mean shift algorithm
        /// </summary>
        /// 
        private CentralMoments meanShift(UnmanagedImage frame)
        {
            // Variable initialization
            int width = frame.Width;
            int height = frame.Height;

            // (assume all frames have equal dimensions)
            if (map == null) map = new float[height, width];


            // Grab the current region of interest in the current frame
            Rectangle roi = conservative ? searchWindow : new Rectangle(0, 0, frame.Width, frame.Height);

            // Compute the histogram for the current frame
            float[] currentHistogram = createHistogram(frame, roi);

            // Use the previous histogram to compute a ratio histogram (storing in current)
            computeHistogramRatio(originalHistogram, currentHistogram, currentHistogram);

            // Compute the back-projection map using the ratio histogram
            lock (sync) generateBackprojectionMap(frame, currentHistogram);


            RawMoments moments = new RawMoments(1);

            // Mean shift with fixed number of iterations
            for (int i = 0; i < MAX_ITERATIONS - 1; i++)
            {
                // Locate first order moments
                moments.Compute(map, searchWindow);

                // Shift the mean (centroid)
                searchWindow.X += (int)(moments.CenterX - searchWindow.Width / 2f);
                searchWindow.Y += (int)(moments.CenterY - searchWindow.Height / 2f);
            }

            // Locate second order moments and perform final shift
            moments.Order = 2; moments.Compute(map, searchWindow);
            searchWindow.X += (int)(moments.CenterX - searchWindow.Width / 2f);
            searchWindow.Y += (int)(moments.CenterY - searchWindow.Height / 2f);

            // Keep the search window inside the image
            searchWindow.X = Math.Max(0, Math.Min(searchWindow.X, width));
            searchWindow.Y = Math.Max(0, Math.Min(searchWindow.Y, height));

            return new CentralMoments(moments); // moments to be used by Camshift
        }



        /// <summary>
        ///   Computes the ratio histogram between to histograms.
        /// </summary>
        /// 
        /// <remarks>
        ///   http://www.robinhewitt.com/research/track/backproject.html
        /// </remarks>
        /// 
        private static float computeHistogramRatio(float[] histogramM, float[] histogramI, float[] ratio)
        {
            float sum = 0;

            for (int i = 0; i < ratio.Length; i++)
            {
                sum += ratio[i] = (histogramI[i] != 0) ?
                    Math.Max(Math.Min(histogramM[i] / histogramI[i], 1f), 0f) : 0f;
            }

            return sum;

        }

        /// <summary>
        ///   Image histogram back-projection.
        /// </summary>
        /// 
        private unsafe void generateBackprojectionMap(UnmanagedImage frame, float[] ratioHistogram)
        {
            int width = frame.Width;
            int height = frame.Height;
            int stride = frame.Stride;
            int pixelSize = Bitmap.GetPixelFormatSize(frame.PixelFormat) / 8;
            int offset = stride - width * pixelSize;

            fixed (float* map_ptr = map)
            {
                byte* src = (byte*)frame.ImageData.ToPointer();
                float* dst = map_ptr;

                if (mode == CamshiftMode.HSL)
                {
                    // Process as HSL
                    HSL hsl = new HSL();
                    RGB rgb = new RGB();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src += pixelSize, dst++)
                        {
                            // RGB
                            rgb.Red = (*(src + RGB.R));
                            rgb.Green = (*(src + RGB.G));
                            rgb.Blue = (*(src + RGB.B));

                            // Transform into HSL
                            AForge.Imaging.HSL.FromRGB(rgb, hsl);

                            if ((hsl.Saturation >= hslSaturation.Min) && (hsl.Saturation <= hslSaturation.Max) &&
                                (hsl.Luminance >= hslLuminance.Min) && (hsl.Luminance <= hslLuminance.Max))
                                *dst = ratioHistogram[hsl.Hue];
                            else *dst = 0;
                        }
                        src += offset;
                    }
                }
                else if (mode == CamshiftMode.Mixed)
                {
                    // Process in mixed mode
                    HSL hsl = new HSL();
                    RGB rgb = new RGB();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src += pixelSize, dst++)
                        {
                            // RGB
                            rgb.Red = (*(src + RGB.R));
                            rgb.Green = (*(src + RGB.G));
                            rgb.Blue = (*(src + RGB.B));

                            // Transform into HSL
                            AForge.Imaging.HSL.FromRGB(rgb, hsl);

                            if ((hsl.Saturation >= hslSaturation.Min) && (hsl.Saturation <= hslSaturation.Max) &&
                                (hsl.Luminance >= hslLuminance.Min) && (hsl.Luminance <= hslLuminance.Max))
                                *dst = ratioHistogram[(int)(hsl.Hue * 10 + hsl.Saturation * 100 + hsl.Luminance * 10)];
                            else *dst = 0;
                        }
                        src += offset;
                    }
                }
                else
                {
                    // Process as RGB
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src += pixelSize, dst++)
                        {
                            // RGB
                            int r = (int)(*(src + RGB.R)) >> 4;
                            int g = (int)(*(src + RGB.G)) >> 4;
                            int b = (int)(*(src + RGB.B)) >> 4;
                            *dst = ratioHistogram[256 * r + 16 * g + b];
                        }
                        src += offset;
                    }
                }
            }
        }

        /// <summary>
        ///   Creates a color histogram discarding low intensity colors
        /// </summary>
        /// 
        private unsafe float[] createHistogram(UnmanagedImage frame, Rectangle area)
        {
            int width = frame.Width;
            int height = frame.Height;
            int stride = frame.Stride;
            int pixelSize = Bitmap.GetPixelFormatSize(frame.PixelFormat) / 8;
            int offset = stride - area.Width * pixelSize;
            float[] histogram = new float[histogramSize];

            // stay inside the image
            int areaX = Math.Max(area.X, 0);
            int areaY = Math.Max(area.Y, 0);
            int areaWidth = Math.Min(area.Width, width - areaX);
            int areaHeight = Math.Min(area.Height, height - areaY);


            if (mode == CamshiftMode.HSL)
            {
                // Process as HSL
                HSL hsl = new HSL();
                RGB rgb = new RGB();

                byte* src = (byte*)frame.ImageData.ToPointer() + areaX * pixelSize + areaY * stride;
                for (int y = 0; y < areaHeight; y++)
                {
                    for (int x = 0; x < areaWidth; x++, src += 3)
                    {
                        rgb.Red = (*(src + RGB.R));
                        rgb.Green = (*(src + RGB.G));
                        rgb.Blue = (*(src + RGB.B));

                        AForge.Imaging.HSL.FromRGB(rgb, hsl);

                        if ((hsl.Saturation >= hslSaturation.Min) && (hsl.Saturation <= hslSaturation.Max) &&
                            (hsl.Luminance >= hslLuminance.Min) && (hsl.Luminance <= hslLuminance.Max))
                            histogram[hsl.Hue] += 1;
                    }
                    src += offset;
                }
            }
            else if (mode == CamshiftMode.Mixed)
            {
                // Process in mixed mode
                HSL hsl = new HSL();
                RGB rgb = new RGB();

                byte* src = (byte*)frame.ImageData.ToPointer() + areaX * pixelSize + areaY * stride;
                for (int y = 0; y < areaHeight; y++)
                {
                    for (int x = 0; x < areaWidth; x++, src += 3)
                    {
                        rgb.Red = (*(src + RGB.R));
                        rgb.Green = (*(src + RGB.G));
                        rgb.Blue = (*(src + RGB.B));

                        AForge.Imaging.HSL.FromRGB(rgb, hsl);

                        if ((hsl.Saturation >= hslSaturation.Min) && (hsl.Saturation <= hslSaturation.Max) &&
                            (hsl.Luminance >= hslLuminance.Min) && (hsl.Luminance <= hslLuminance.Max))
                            histogram[(int)(hsl.Hue * 10 + hsl.Saturation * 100 + hsl.Luminance * 10)] += 1;
                    }
                    src += offset;
                }
            }
            else
            {
                // Process as RGB
                byte* src = (byte*)frame.ImageData.ToPointer() + areaX * pixelSize + areaY * stride;
                for (int y = 0; y < areaHeight; y++)
                {
                    for (int x = 0; x < areaWidth; x++, src += 3)
                    {
                        // (small values are discarded)
                        int r = (int)(*(src + RGB.R)) >> 4;
                        int g = (int)(*(src + RGB.G)) >> 4;
                        int b = (int)(*(src + RGB.B)) >> 4;
                        histogram[256 * r + 16 * g + b] += 1;
                    }
                    src += offset;
                }
            }

            return histogram;
        }

        /// <summary>
        ///   Checks for aberrant fluctuations in the tracking object.
        /// </summary>
        /// 
        private bool checkSteadiness()
        {
            if (angleHistory.StandardDeviation > 0.13)
                unstableCounter++;
            else unstableCounter--;

            unstableCounter = Math.Max(0, Math.Min(unstableCounter, angleHistory.Window));

            bool steady = unstableCounter < angleHistory.Window;

            return steady;
        }

    }
}