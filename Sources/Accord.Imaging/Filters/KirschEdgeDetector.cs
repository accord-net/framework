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
    ///   Kirsch's Edge Detector
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///   The Kirsch operator or Kirsch <see cref="CompassConvolution">compass kernel</see>
    ///   is a non-linear edge detector that finds the maximum edge strength in a few 
    ///   predetermined directions. It is named after the computer scientist Russell 
    ///   A. Kirsch.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia contributors. "Kirsch operator." Wikipedia, The Free Encyclopedia. Wikipedia,
    ///       The Free Encyclopedia. Available at http://en.wikipedia.org/wiki/Kirsch_operator </description></item>
    ///   </list>
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// Bitmap image = ... // Lena's picture
    /// 
    /// // Create a new Kirsch's edge detector:
    /// var kirsch = new KirschEdgeDetector();
    /// 
    /// // Compute the image edges
    /// Bitmap edges = kirsch.Apply(image);
    /// 
    /// // Show on screen
    /// ImageBox.Show(edges);
    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below:</para>
    /// 
    ///   <img src="..\images\kirsch.png" />
    /// 
    /// </example>
    /// 
    /// <seealso cref="RobinsonEdgeDetector"/>
    /// <seealso cref="CompassConvolution"/>
    /// 
    public class KirschEdgeDetector : BaseFilter
    {
        CompassConvolution convolution;

        /// <summary>
        ///   Initializes a new instance of the <see cref="KirschEdgeDetector"/> class.
        /// </summary>
        /// 
        public KirschEdgeDetector()
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
        ///   Gets the North direction Kirsch kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] North = 
        {
            { -3, -3, 5 },
            { -3,  0, 5 },
            { -3, -3, 5 }
        };

        /// <summary>
        ///   Gets the Northwest direction Kirsch kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] Northwest = 
        {
            { -3,  5,  5 },
            { -3,  0,  5 },
            { -3, -3, -3 }
        };

        /// <summary>
        ///   Gets the West direction Kirsch kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] West = 
        {
            {  5,  5,  5 },
            { -3,  0, -3 },
            { -3, -3, -3 }
        };

        /// <summary>
        ///   Gets the Southwest direction Kirsch kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] Southwest = 
        {
            {  5,  5, -3 },
            {  5,  0, -3 },
            { -3, -3, -3 }
        };

        /// <summary>
        ///   Gets the South direction Kirsch kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] South = 
        {
            {  5, -3, -3 },
            {  5,  0, -3 },
            {  5, -3, -3 }
        };

        /// <summary>
        ///   Gets the Southeast direction Kirsch kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] Southeast = 
        {
            { -3, -3, -3 },
            {  5,  0, -3 },
            {  5,  5, -3 }
        };

        /// <summary>
        ///   Gets the East direction Kirsch kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] East = 
        {
            { -3, -3, -3 },
            { -3,  0, -3 },
            {  5,  5,  5 }
        };

        /// <summary>
        ///   Gets the Northeast direction Kirsch kernel mask.
        /// </summary>
        /// 
        public static readonly int[,] Northeast = 
        {
            { -3, -3, -3 },
            { -3,  0,  5 },
            { -3,  5,  5 }
        };

    }
}
