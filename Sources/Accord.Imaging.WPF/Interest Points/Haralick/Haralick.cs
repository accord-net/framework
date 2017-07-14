﻿// Accord Imaging Library
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

namespace Accord.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Accord.Imaging;
    using Accord.Imaging.Filters;

    /// <summary>
    ///   <see cref="Haralick"/>'s operation modes.
    /// </summary>
    /// 
    public enum HaralickMode
    {
        /// <summary>
        ///   Features will be combined using 
        ///   <see cref="HaralickDescriptorDictionary.Average"/>.
        /// </summary>
        /// 
        Average,

        /// <summary>
        ///   Features will be combined using 
        ///   <see cref="HaralickDescriptorDictionary.AverageWithRange"/>.
        /// </summary>
        /// 
        AverageWithRange,

        /// <summary>
        ///   Features will be combined using 
        ///   <see cref="HaralickDescriptorDictionary.Combine"/>.
        /// </summary>
        /// 
        Combine,

        /// <summary>
        ///   Features will be combined using 
        ///   <see cref="HaralickDescriptorDictionary.Normalize"/>.
        /// </summary>
        /// 
        NormalizedAverage,
    }

    /// <summary>
    ///   Haralick textural feature extractor.
    /// </summary>
    /// 
    public class Haralick : IFeatureDetector<FeatureDescriptor>,
        IFeatureDetector<IFeatureDescriptor<double[]>>
    {
        int cellSize = 0;  // size of the cell, in number of pixels
        bool normalize = false;
        int distance = 1;
        bool autoGray = true;
        int featureCount = 13;

        HashSet<CooccurrenceDegree> degrees;
        HaralickDescriptorDictionary[,] features;
        GrayLevelCooccurrenceMatrix matrix;

        HaralickMode mode = HaralickMode.NormalizedAverage;

        /// <summary>
        ///   Gets the size of a cell, in pixels.
        /// </summary>
        /// 
        public int CellSize { get { return cellSize; } }

        /// <summary>
        ///   Gets the <see cref="CooccurrenceDegree"/>s which should
        ///   be computed by this Haralick textural feature extractor.
        ///   Default is <see cref="HaralickMode.NormalizedAverage"/>.
        /// </summary>
        /// 
        public HashSet<CooccurrenceDegree> Degrees
        {
            get { return degrees; }
        }

        /// <summary>
        ///   Gets or sets the mode of operation of this
        ///   <see cref="Haralick">Haralick's textural 
        ///   feature extractor</see>. 
        /// </summary>
        /// 
        /// <remarks>
        ///   The mode determines how the different features captured
        ///   by the <see cref="HaralickDescriptor"/> are combined.
        ///  </remarks>
        ///  
        /// <value>
        ///   A value from the <see cref="HaralickMode"/> enumeration
        ///   specifying how the different features should be combined.
        /// </value>
        /// 
        public HaralickMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        ///   Gets or sets the number of features to extract using
        ///   the <see cref="HaralickDescriptor"/>. By default, only
        ///   the first 13 original Haralick's features will be used.
        /// </summary>
        /// 
        public int Features
        {
            get { return featureCount; }
            set { featureCount = value; }
        }

        /// <summary>
        ///   Gets the set of local binary patterns computed for each
        ///   cell in the last call to <see cref="ProcessImage(Bitmap)"/>.
        /// </summary>
        /// 
        public HaralickDescriptorDictionary[,] Descriptors { get { return features; } }

        /// <summary>
        ///   Gets the <see cref="GrayLevelCooccurrenceMatrix">Gray-level
        ///   Co-occurrence Matrix (GLCM)</see> generated during the last
        ///   call to <see cref="ProcessImage(UnmanagedImage)"/>.
        /// </summary>
        /// 
        public GrayLevelCooccurrenceMatrix Matrix { get { return matrix; } }

        /// <summary>
        ///   Gets or sets whether to normalize final 
        ///   histogram feature vectors. Default is false.
        /// </summary>
        /// 
        public bool Normalize
        {
            get { return normalize; }
            set { normalize = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Haralick"/> class.
        /// </summary>
        /// 
        /// <param name="degrees">
        ///   The angulation degrees on which the <see cref="HaralickDescriptor">Haralick's
        ///   features</see> should be computed. Default is to use all directions.</param>
        /// 
        public Haralick(params CooccurrenceDegree[] degrees)
        {
            init(cellSize, normalize, degrees);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Haralick"/> class.
        /// </summary>
        /// 
        /// <param name="cellSize">
        ///   The size of a computing cell, measured in pixels.
        ///   Default is 0 (use whole image at once).</param>
        /// <param name="normalize">
        ///   Whether to normalize generated 
        ///   histograms. Default is false.</param>
        /// 
        public Haralick(int cellSize, bool normalize)
        {
            init(cellSize, normalize, null);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Haralick"/> class.
        /// </summary>
        /// 
        /// <param name="cellSize">
        ///   The size of a computing cell, measured in pixels.
        ///   Default is 0 (use whole image at once).</param>
        /// <param name="normalize">
        ///   Whether to normalize generated 
        ///   histograms. Default is true.</param>
        /// <param name="degrees">
        ///   The angulation degrees on which the <see cref="HaralickDescriptor">Haralick's
        ///   features</see> should be computed. Default is to use all directions.</param>
        /// 
        public Haralick(int cellSize, bool normalize, params CooccurrenceDegree[] degrees)
        {
            init(cellSize, normalize, degrees);
        }

        private void init(int size, bool norm, CooccurrenceDegree[] deg)
        {
            this.degrees = new HashSet<CooccurrenceDegree>();

            if (deg == null || deg.Length == 0)
            {
                this.degrees.Add(CooccurrenceDegree.Degree0);
                this.degrees.Add(CooccurrenceDegree.Degree45);
                this.degrees.Add(CooccurrenceDegree.Degree90);
                this.degrees.Add(CooccurrenceDegree.Degree135);
            }
            else
            {
                foreach (var degree in deg)
                    this.degrees.Add(degree);
            }

            this.cellSize = size;
            this.normalize = norm;
        }


        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found features points.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public List<double[]> ProcessImage(UnmanagedImage image)
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
            UnmanagedImage grayImage = null;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                grayImage = image;
            }
            else
            {
                // create temporary grayscale image
                grayImage = Grayscale.CommonAlgorithms.BT709.Apply(image);
            }


            // get source image size
            int width = grayImage.Width;
            int height = grayImage.Height;


            matrix = new GrayLevelCooccurrenceMatrix(distance,
                CooccurrenceDegree.Degree0, true, autoGray);

            if (cellSize > 0)
            {
                int cellCountX = (int)Math.Floor(width / (double)cellSize);
                int cellCountY = (int)Math.Floor(height / (double)cellSize);
                features = new HaralickDescriptorDictionary[cellCountX, cellCountY];

                // For each cell
                for (int i = 0; i < cellCountX; i++)
                {
                    for (int j = 0; j < cellCountY; j++)
                    {
                        var featureDict = new HaralickDescriptorDictionary();

                        Rectangle region = new Rectangle(
                            i * cellSize, j * cellSize, cellSize, cellSize);

                        foreach (CooccurrenceDegree degree in degrees)
                        {
                            matrix.Degree = degree;
                            double[,] glcm = matrix.Compute(grayImage, region);
                            featureDict[degree] = new HaralickDescriptor(glcm);
                        }

                        features[i, j] = featureDict;
                    }
                }
            }
            else
            {
                features = new HaralickDescriptorDictionary[1, 1];
                features[0, 0] = new HaralickDescriptorDictionary();
                foreach (CooccurrenceDegree degree in degrees)
                {
                    matrix.Degree = degree;
                    double[,] glcm = matrix.Compute(grayImage);
                    features[0, 0][degree] = new HaralickDescriptor(glcm);
                }
            }

            // Free some resources which wont be needed anymore
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                grayImage.Dispose();



            var blocks = new List<double[]>();

            switch (mode)
            {
                case HaralickMode.Average:
                    foreach (HaralickDescriptorDictionary feature in features)
                        blocks.Add(feature.Average(featureCount));
                    break;

                case HaralickMode.AverageWithRange:
                    foreach (HaralickDescriptorDictionary feature in features)
                        blocks.Add(feature.AverageWithRange(featureCount));
                    break;

                case HaralickMode.Combine:
                    foreach (HaralickDescriptorDictionary feature in features)
                        blocks.Add(feature.Combine(featureCount));
                    break;

                case HaralickMode.NormalizedAverage:
                    foreach (HaralickDescriptorDictionary feature in features)
                        blocks.Add(feature.Normalize(featureCount));
                    break;
            }

            if (normalize)
            {
                var sum = new double[featureCount];
                foreach (double[] block in blocks)
                    for (int i = 0; i < sum.Length; i++)
                        sum[i] += block[i];

                foreach (double[] block in blocks)
                    for (int i = 0; i < sum.Length; i++)
                        block[i] /= sum[i];
            }

            return blocks;
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
        public List<double[]> ProcessImage(BitmapData imageData)
        {
            return ProcessImage(new UnmanagedImage(imageData));
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
        public List<double[]> ProcessImage(Bitmap image)
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
            BitmapData imageData = image.LockBits(ImageLockMode.ReadOnly);

            try
            {
                // process the image
                return ProcessImage(new UnmanagedImage(imageData));
            }
            finally
            {
                // unlock image
                image.UnlockBits(imageData);
            }
        }


        IEnumerable<FeatureDescriptor> IFeatureDetector<FeatureDescriptor, double[]>.ProcessImage(Bitmap image)
        {
            return ProcessImage(image).ConvertAll(p => new FeatureDescriptor(p));
        }

        IEnumerable<FeatureDescriptor> IFeatureDetector<FeatureDescriptor, double[]>.ProcessImage(BitmapData imageData)
        {
            return ProcessImage(imageData).ConvertAll(p => new FeatureDescriptor(p));
        }

        IEnumerable<FeatureDescriptor> IFeatureDetector<FeatureDescriptor, double[]>.ProcessImage(UnmanagedImage image)
        {
            return ProcessImage(image).ConvertAll(p => new FeatureDescriptor(p));
        }

        IEnumerable<IFeatureDescriptor<double[]>> IFeatureDetector<IFeatureDescriptor<double[]>, double[]>.ProcessImage(Bitmap image)
        {
            return ProcessImage(image).ConvertAll(p => (IFeatureDescriptor<double[]>)new FeatureDescriptor(p));
        }

        IEnumerable<IFeatureDescriptor<double[]>> IFeatureDetector<IFeatureDescriptor<double[]>, double[]>.ProcessImage(BitmapData imageData)
        {
            return ProcessImage(imageData).ConvertAll(p => (IFeatureDescriptor<double[]>)new FeatureDescriptor(p));
        }

        IEnumerable<IFeatureDescriptor<double[]>> IFeatureDetector<IFeatureDescriptor<double[]>, double[]>.ProcessImage(UnmanagedImage image)
        {
            return ProcessImage(image).ConvertAll(p => (IFeatureDescriptor<double[]>)new FeatureDescriptor(p));
        }


        private Haralick()
        {
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            var clone = new Haralick();
            clone.autoGray = autoGray;
            clone.cellSize = cellSize;
            clone.degrees = degrees;
            clone.distance = distance;
            clone.featureCount = featureCount;
            clone.features = (HaralickDescriptorDictionary[,])features.Clone();
            clone.matrix = (GrayLevelCooccurrenceMatrix)matrix.Clone();
            clone.mode = mode;
            clone.normalize = normalize;
            return clone;
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, 
        ///   or resetting unmanaged resources.
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// 
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged
        ///   resources; <c>false</c> to release only unmanaged resources.</param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }

            // free native resources if there are any.
        }

    }
}
