// Accord Imaging Library
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

namespace Accord.Imaging
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;
    using AForge.Imaging;
    using AForge.Imaging.Filters;
    using System;

    /// <summary>
    ///   SURF Feature descriptor types.
    /// </summary>
    /// 
    public enum FastRetinaKeypointDescriptorType
    {
        /// <summary>
        ///   Do not compute descriptors.
        /// </summary>
        /// 
        None,

        /// <summary>
        ///   Compute standard 512-bit descriptors.
        /// </summary>
        /// 
        Standard,

        /// <summary>
        ///   Compute extended 1024-bit descriptors.
        /// </summary>
        /// 
        Extended,
    }

    /// <summary>
    ///   Fast Retina Keypoint (FREAK) detector.
    /// </summary>
    /// 
    [Serializable]
    public class FastRetinaKeypointDetector : IFeatureDetector<FastRetinaKeypoint, byte[]>
    {

        private FastRetinaKeypointDescriptorType featureType = FastRetinaKeypointDescriptorType.Standard;
        private float scale = 22.0f;
        private int octaves = 4;


        [NonSerialized]
        private IntegralImage integral;

        [NonSerialized]
        private UnmanagedImage grayImage;

        [NonSerialized]
        private FastRetinaKeypointPattern pattern;

        [NonSerialized]
        private FastRetinaKeypointDescriptor descriptor;



        /// <summary>
        ///   Gets the corners detector used to generate features.
        /// </summary>
        /// 
        public ICornersDetector Detector { get; private set; }


        /// <summary>
        ///   Gets or sets a value indicating whether all feature points
        ///   should have their descriptors computed after being detected.
        ///   Default is to compute standard descriptors.
        /// </summary>
        /// 
        /// <value><c>true</c> if to compute orientation; otherwise, <c>false</c>.</value>
        /// 
        public FastRetinaKeypointDescriptorType ComputeDescriptors
        {
            get { return featureType; }
            set { featureType = value; }
        }

        /// <summary>
        ///   Gets or sets the number of octaves to use when 
        ///   building the feature descriptor. Default is 4.
        /// </summary>
        /// 
        public int Octaves
        {
            get { return pattern.Octaves; }
            set
            {
                if (value != octaves)
                {
                    octaves = value;
                    pattern = null;
                }
            }
        }

        /// <summary>
        ///   Gets or sets the scale used when building 
        ///   the feature descriptor. Default is 0.22f.
        /// </summary>
        /// 
        public float Scale
        {
            get { return pattern.Scale; }
            set
            {
                if (value != scale)
                {
                    scale = value;
                    pattern = null;
                }
            }
        }




        /// <summary>
        ///   Initializes a new instance of the <see cref="FastRetinaKeypointDetector"/> class.
        /// </summary>
        /// 
        /// <param name="threshold">The detection threshold for the 
        /// <see cref="FastCornersDetector">FAST detector</see>.</param>
        /// 
        public FastRetinaKeypointDetector(int threshold)
        {
            init(new FastCornersDetector(threshold));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FastRetinaKeypointDetector"/> class.
        /// </summary>
        /// 
        public FastRetinaKeypointDetector()
        {
            init(new FastCornersDetector());
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FastRetinaKeypointDetector"/> class.
        /// </summary>
        /// 
        /// <param name="detector">A corners detector.</param>
        /// 
        public FastRetinaKeypointDetector(ICornersDetector detector)
        {
            init(detector);
        }

        private void init(ICornersDetector detector)
        {
            this.Detector = detector;
        }



        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found interest points.</returns>
        /// 
        public List<FastRetinaKeypoint> ProcessImage(UnmanagedImage image)
        {
            // check image format
            if (
                (image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (image.PixelFormat != PixelFormat.Format24bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppArgb)
                )
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }

            // make sure we have grayscale image
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                grayImage = image;
            }
            else
            {
                // create temporary grayscale image
                grayImage = Grayscale.CommonAlgorithms.BT709.Apply(image);
            }


            // 1. Extract corners points from the image.
            List<IntPoint> corners = Detector.ProcessImage(grayImage);

            List<FastRetinaKeypoint> features = new List<FastRetinaKeypoint>();
            for (int i = 0; i < corners.Count; i++)
                features.Add(new FastRetinaKeypoint(corners[i].X, corners[i].Y));


            // 2. Compute the integral for the given image
            integral = IntegralImage.FromBitmap(grayImage);


            // 3. Compute feature descriptors if required
            descriptor = null;
            if (featureType != FastRetinaKeypointDescriptorType.None)
            {
                descriptor = GetDescriptor();
                descriptor.Compute(features);
            }

            return features;
        }

        /// <summary>
        ///   Gets the <see cref="FastRetinaKeypointDescriptor">
        ///   feature descriptor</see> for the last processed image.
        /// </summary>
        /// 
        public FastRetinaKeypointDescriptor GetDescriptor()
        {
            if (descriptor == null || pattern == null)
            {
                if (pattern == null)
                    pattern = new FastRetinaKeypointPattern(octaves, scale);

                descriptor = new FastRetinaKeypointDescriptor(grayImage, integral, pattern);
                descriptor.Extended = featureType == FastRetinaKeypointDescriptorType.Extended;
            }

            return descriptor;
        }

        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found interest points.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public List<FastRetinaKeypoint> ProcessImage(Bitmap image)
        {
            // check image format
            if (
                (image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (image.PixelFormat != PixelFormat.Format24bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppArgb)
                )
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source");
            }

            // lock source image
            BitmapData imageData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            List<FastRetinaKeypoint> corners;

            try
            {
                // process the image
                corners = ProcessImage(new UnmanagedImage(imageData));
            }
            finally
            {
                // unlock image
                image.UnlockBits(imageData);
            }

            return corners;
        }

        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found interest points.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public List<FastRetinaKeypoint> ProcessImage(BitmapData imageData)
        {
            return ProcessImage(new UnmanagedImage(imageData));
        }

    }
}
