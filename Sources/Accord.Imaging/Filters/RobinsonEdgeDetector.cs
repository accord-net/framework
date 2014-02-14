// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2013
// diego.catalano at live.com
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

namespace Accord.Imaging.Filters
{
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   Robinson's Edge Detector
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///   Robinson's edge detector is a variation of <see cref="KirschEdgeDetector">
    ///   Kirsch's detector</see> using different convolution masks. Both are examples
    ///   of <see cref="CompassConvolution">compass convolution filters</see>.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// Bitmap image = ... // Lena's picture
    /// 
    /// // Create a new Robinson's edge detector:
    /// var robinson = new RobinsonEdgeDetector();
    /// 
    /// // Compute the image edges
    /// Bitmap edges = robinson.Apply(image);
    /// 
    /// // Show on screen
    /// ImageBox.Show(edges);
    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below:</para>
    /// 
    ///   <img src="..\images\robinson.png" />
    /// 
    /// </example>
    /// 
    /// <seealso cref="KirschEdgeDetector"/>
    /// <seealso cref="CompassConvolution"/>
    /// 
    public class RobinsonEdgeDetector : BaseFilter
    {

        CompassConvolution convolution;

        /// <summary>
        ///   Initializes a new instance of the <see cref="RobinsonEdgeDetector"/> class.
        /// </summary>
        /// 
        public RobinsonEdgeDetector()
        {
            int[][,] sequence = 
            {
                North, Northwest, West, Southwest, South, Southeast, East, Northeast 
            };

            convolution = new CompassConvolution(sequence);
        }

        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return convolution.FormatTranslations; }
        }

        /// <summary>
        ///   Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            convolution.Apply(sourceData, destinationData);
        }


        /// <summary>
        ///   Gets the North direction Robinson kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] North = 
        {
            { -1,  0,  1 },
            { -2,  0,  2 },
            { -1,  0,  1 }
        };

        /// <summary>
        ///   Gets the Northwest direction Robinson kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] Northwest = 
        {
            {  0,  1,  2 },
            { -1,  0,  1 },
            { -2, -1,  0 }
        };

        /// <summary>
        ///   Gets the West direction Robinson kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] West = 
        {
            {  1,  2,  1 },
            {  0,  0,  0 },
            { -1, -2, -1 }
        };

        /// <summary>
        ///   Gets the Southwest direction Robinson kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] Southwest = 
        {
            {  2,  1,  0 },
            {  1,  0, -1 },
            {  0, -1, -2 }
        };

        /// <summary>
        ///   Gets the South direction Robinson kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] South = 
        {
            {  1,  0, -1 },
            {  2,  0, -2 },
            {  1,  0, -1 }
        };

        /// <summary>
        ///   Gets the Southeast direction Robinson kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] Southeast = 
        {
            {  0, -1, -2 },
            {  1,  0, -1 },
            {  2,  1,  0 }
        };

        /// <summary>
        ///   Gets the East direction Robinson kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] East = 
        {
            { -1, -2, -1 },
            {  0,  0,  0 },
            {  1,  2,  1 }
        };

        /// <summary>
        ///   Gets the Northeast direction Robinson kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] Northeast = 
        { 
            { -2, -1,  0 },
            { -1,  0,  1 },
            {  0,  1,  2 }
        };

    }
}
