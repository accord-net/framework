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
    using AForge;
    using AForge.Imaging;
    using AForge.Imaging.Filters;
    using AForge.Math.Geometry;
    using System;

    /// <summary>
    ///   Filter to mark (highlight) lines in a image.
    /// </summary>
    /// 
    public class LineMarker : BaseInPlaceFilter
    {
        private int width = 3;
        private Color markerColor = Color.White;
        private Line line;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();


        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        ///   Color used to mark corners.
        /// </summary>
        public Color MarkerColor
        {
            get { return markerColor; }
            set { markerColor = value; }
        }

        /// <summary>
        ///   Gets or sets the set of points to mark.
        /// </summary>
        public Line Line
        {
            get { return line; }
            set { line = value; }
        }

        /// <summary>
        ///   Gets or sets the width of the points to be drawn.
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LineMarker"/> class.
        /// </summary>
        /// 
        public LineMarker(Line line)
            : this(line, Color.White, 3)
        {
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="LineMarker"/> class.
        /// </summary>
        /// 
        public LineMarker(Line line, Color markerColor)
            : this(line, markerColor, 3)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LineMarker"/> class.
        /// </summary>
        /// 
        public LineMarker(Line line, Color markerColor, int width)
        {
            this.line = line;
            this.markerColor = markerColor;
            this.width = width;

            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LineMarker"/> class.
        /// </summary>
        /// 
        public LineMarker(Color markerColor)
            : this(null, markerColor, 3)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LineMarker"/> class.
        /// </summary>
        /// 
        public LineMarker(Color markerColor, int width)
        {
            this.markerColor = markerColor;
            this.width = width;

            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        ///   Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        ///
        protected override unsafe void ProcessFilter(UnmanagedImage image)
        {
            // get border point which is still in the line

            IntPoint a = new IntPoint(0, 0);
            IntPoint b = new IntPoint(0, image.Height);
            IntPoint c = new IntPoint(image.Width, 0);
            IntPoint d = new IntPoint(image.Width, image.Height);

            LineSegment top = new LineSegment(a, c);
            LineSegment left = new LineSegment(a, b);
            LineSegment right = new LineSegment(c, d);
            LineSegment bottom = new LineSegment(b, d);

            AForge.Point?[] points = new AForge.Point?[4];
            points[0] = line.GetIntersectionWith(bottom);
            points[1] = line.GetIntersectionWith(left);
            points[2] = line.GetIntersectionWith(right);
            points[3] = line.GetIntersectionWith(top);

            IntPoint? p1 = null;
            IntPoint? p2 = null;
            foreach (var p in points)
            {
                if (p == null)
                    continue;

                if (p1 == null)
                {
                    p1 = p.Value.Round();
                    continue;
                }

                p2 = p.Value.Round();
                break;
            }

            IntPoint start = new IntPoint(image.Width - p1.Value.X, p1.Value.Y);
            IntPoint end = new IntPoint(image.Width - p2.Value.X, p2.Value.Y);

            Drawing.Line(image, start, end, markerColor);
        }
    }
}