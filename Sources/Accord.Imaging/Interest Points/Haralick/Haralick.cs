// Accord Imaging Library
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
    using Accord.Math;
    using Accord.Compat;

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
    /// <remarks>
    /// <para>
    ///   Haralick's texture features are based on measures derived from
    ///   <see cref="GrayLevelCooccurrenceMatrix">Gray-level Co-occurrence 
    ///   matrices (GLCM)</see>.</para>
    /// <para>
    ///   Whether considering the intensity or grayscale values of the image 
    ///   or various dimensions of color, the co-occurrence matrix can measure
    ///   the texture of the image. Because co-occurrence matrices are typically
    ///   large and sparse, various metrics of the matrix are often taken to get
    ///   a more useful set of features. Features generated using this technique
    ///   are usually called Haralick features, after R. M. Haralick, attributed to
    ///   his paper Textural features for image classification (1973).</para>
    ///   
    /// <para>
    ///   This class can extract <see cref="HaralickDescriptor"/>s from different
    ///   regions of an image using a pre-defined cell size. For more information
    ///   about which features are computed, please see documentation for the
    ///   <see cref="HaralickDescriptor"/> class.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia Contributors, "Co-occurrence matrix". Available at
    ///       http://en.wikipedia.org/wiki/Co-occurrence_matrix </description></item>
    ///     <item><description>
    ///       Robert M Haralick, K Shanmugam, Its'hak Dinstein; "Textural 
    ///       Features for Image Classification". IEEE Transactions on Systems, Man,
    ///       and Cybernetics. SMC-3 (6): 610–621, 1973. Available at:
    ///       <a href="http://www.makseq.com/materials/lib/Articles-Books/Filters/Texture/Co-occurrence/haralick73.pdf">
    ///       http://www.makseq.com/materials/lib/Articles-Books/Filters/Texture/Co-occurrence/haralick73.pdf </a>
    ///       </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The first example shows how to extract Haralick descriptors given an image.</para>
    ///   <code source="Unit Tests\Accord.Tests.Imaging\HaralickTest.cs" region="doc_apply" />
    ///   <para><b>Input image:</b></para>
    ///   <img src="..\images\imaging\wood_texture.jpg" width="320" height="240" />
    ///   
    /// <para>
    ///   The second example shows how to use the Haralick feature extractor as part of a
    ///   Bag-of-Words model in order to perform texture image classification:</para>
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_feature_haralick" />
    ///   <code source="Unit Tests\Accord.Tests.Vision\Imaging\BagOfVisualWordsTest.cs" region="doc_classification_feature_haralick" />
    /// </example>
    /// 
    /// <seealso cref="HaralickDescriptor"/>
    /// <seealso cref="GrayLevelCooccurrenceMatrix"/>
    /// <seealso cref="SpeededUpRobustFeaturesDetector"/>
    /// <seealso cref="HarrisCornersDetector"/>
    /// 
    public class Haralick : BaseFeatureExtractor<FeatureDescriptor>
    {
        int cellSize = 0;  // size of the cell, in number of pixels
        bool normalize = false;
        int distance = 1;
        bool autoGray = true;
        int featureCount = 13;

        double epsilon = 1e-10;

        HashSet<CooccurrenceDegree> degrees;
        HaralickDescriptorDictionary[,] features;
        GrayLevelCooccurrenceMatrix matrix;

        HaralickMode mode = HaralickMode.NormalizedAverage;

        /// <summary>
        ///   Gets the size of a cell, in pixels. A value of 0 means the 
        ///   cell will have the size of the image. Default is 0 (uses the 
        ///   entire image).
        /// </summary>
        /// 
        public int CellSize
        {
            get { return cellSize; }
            set { cellSize = value; }
        }

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
        ///   cell in the last call to <see cref="BaseFeatureExtractor{TPoint}.ProcessImage(Bitmap)"/>.
        /// </summary>
        /// 
        public HaralickDescriptorDictionary[,] Descriptors { get { return features; } }

        /// <summary>
        ///   Gets the <see cref="GrayLevelCooccurrenceMatrix">Gray-level
        ///   Co-occurrence Matrix (GLCM)</see> generated during the last
        ///   call to <see cref="BaseFeatureExtractor{TPoint}.Transform(UnmanagedImage)"/>.
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

            base.SupportedFormats.UnionWith(new[] {
                PixelFormat.Format8bppIndexed,
                PixelFormat.Format24bppRgb,
                PixelFormat.Format32bppRgb,
                PixelFormat.Format32bppArgb });
        }

        /// <summary>
        ///   This method should be implemented by inheriting classes to implement the 
        ///   actual feature extraction, transforming the input image into a list of features.
        /// </summary>
        /// 
        protected override IEnumerable<FeatureDescriptor> InnerTransform(UnmanagedImage image)
        {
            // TODO: Improve memory usage of this method by
            // caching into class variables whenever possible

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


            this.matrix = new GrayLevelCooccurrenceMatrix(distance,
                CooccurrenceDegree.Degree0, normalize: true, autoGray: autoGray);

            if (cellSize > 0)
            {
                int cellCountX = (int)Math.Floor(width / (double)cellSize);
                int cellCountY = (int)Math.Floor(height / (double)cellSize);
                this.features = new HaralickDescriptorDictionary[cellCountX, cellCountY];

                // For each cell
                for (int i = 0; i < cellCountX; i++)
                {
                    for (int j = 0; j < cellCountY; j++)
                    {
                        var dict = new HaralickDescriptorDictionary();

                        var region = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);

                        foreach (CooccurrenceDegree degree in degrees)
                        {
                            matrix.Degree = degree;
                            double[,] glcm = matrix.Compute(grayImage, region);
                            dict[degree] = new HaralickDescriptor(glcm);
                        }

                        this.features[i, j] = dict;
                    }
                }
            }
            else
            {
                var dict = new HaralickDescriptorDictionary();
                foreach (CooccurrenceDegree degree in degrees)
                {
                    matrix.Degree = degree;
                    double[,] glcm = matrix.Compute(grayImage);
                    dict[degree] = new HaralickDescriptor(glcm);
                }

                this.features = new HaralickDescriptorDictionary[,] { {  dict } };
            }

            // Free some resources which wont be needed anymore
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                grayImage.Dispose();



            var blocks = new List<FeatureDescriptor>();

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
                // TODO: Remove this block and instead propose a general architecture 
                //       for applying normalizations to descriptor blocks
                foreach (FeatureDescriptor block in blocks)
                    block.Descriptor.Divide(block.Descriptor.Euclidean() + epsilon, result: block.Descriptor);
            }

            return blocks;
        }


        private Haralick()
        {
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        protected override object Clone(ISet<PixelFormat> supportedFormats)
        {
            var clone = new Haralick();
            clone.SupportedFormats = supportedFormats;
            clone.autoGray = autoGray;
            clone.cellSize = cellSize;
            clone.degrees = degrees;
            clone.distance = distance;
            clone.featureCount = featureCount;
            clone.SupportedFormats = SupportedFormats;
            if (features != null)
                clone.features = (HaralickDescriptorDictionary[,])features.Clone();
            if (matrix != null)
                clone.matrix = (GrayLevelCooccurrenceMatrix)matrix.Clone();
            clone.mode = mode;
            clone.normalize = normalize;
            return clone;
        }

    }
}
