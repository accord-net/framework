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

namespace Accord.Imaging.Filters
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   Filter to mark (highlight) rectangles in a image.
    /// </summary>
    /// 
    public class RectanglesMarker : BaseInPlaceFilter
    {
        private Color markerColor = Color.White;
        private Color fillColor = Color.Transparent;
        private IEnumerable<Rectangle> rectangles;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();


        /// <summary>
        ///   Color used to mark pairs.
        /// </summary>
        /// 
        public Color MarkerColor
        {
            get { return markerColor; }
            set { markerColor = value; }
        }

        /// <summary>
        ///   Gets or sets the color used to fill
        ///   rectangles. Default is Transparent.
        /// </summary>
        /// 
        public Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }
            

        /// <summary>
        ///   The set of rectangles.
        /// </summary>
        /// 
        public IEnumerable<Rectangle> Rectangles
        {
            get { return rectangles; }
            set { rectangles = value; }
        }


        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="RectanglesMarker"/> class.
        /// </summary>
        /// 
        /// <param name="markerColor">The color to use to drawn the rectangles.</param>
        /// 
        public RectanglesMarker(Color markerColor)
            : this(null, markerColor)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RectanglesMarker"/> class.
        /// </summary>
        /// 
        /// <param name="rectangles">Set of rectangles to be drawn.</param>
        /// 
        public RectanglesMarker(params Rectangle[] rectangles)
            : this(rectangles, Color.White)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RectanglesMarker"/> class.
        /// </summary>
        /// 
        /// <param name="rectangles">Set of rectangles to be drawn.</param>
        /// 
        public RectanglesMarker(IEnumerable<Rectangle> rectangles)
            : this(rectangles, Color.White)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RectanglesMarker"/> class.
        /// </summary>
        /// 
        /// <param name="rectangles">Set of rectangles to be drawn.</param>
        /// <param name="markerColor">The color to use to drawn the rectangles.</param>
        /// 
        public RectanglesMarker(IEnumerable<Rectangle> rectangles, Color markerColor)
        {
            this.rectangles = rectangles;
            this.markerColor = markerColor;

            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        ///   Applies the filter to the image.
        /// </summary>
        protected override void ProcessFilter(UnmanagedImage image)
        {
            // mark all rectangular regions
            foreach (Rectangle rectangle in rectangles)
            {
                Drawing.Rectangle(image, rectangle, markerColor);
                if (fillColor != Color.Transparent)
                    Drawing.FillRectangle(image, rectangle, fillColor);
            }
        }
    }
}