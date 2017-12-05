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

namespace Accord.Imaging.Filters
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Accord.Math;
    using AForge;
    using Accord.Imaging;
    using Accord.Imaging.Filters;
    using System.Linq;

    /// <summary>
    ///   Filter to mark (highlight) points in a image.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The filter highlights points on the image using a given set of points.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale, 24 and 32 bpp color images for processing.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>Sample usage:
    /// <code>
    /// // Create a blob contour's instance
    /// BlobCounter bc = new BlobCounter(image);
    /// 
    /// // Extract blobs
    /// Blob[] blobs = bc.GetObjectsInformation();
    /// bc.ExtractBlobsImage(bmp, blobs[0], true);
    /// 
    /// // Extract blob's edge points
    /// List&lt;IntPoint&gt; contour = bc.GetBlobsEdgePoints(blobs[0]);
    /// 
    /// // Create a green, 2 pixel width points marker's instance
    /// PointsMarker marker = new PointsMarker(contour, Color.Green, 2);
    /// 
    /// // Apply the filter in a given color image
    /// marker.ApplyInPlace(colorBlob);
    /// </code>
    /// </para>
    /// </example>
    //
    public class PointsMarker : BaseInPlaceFilter
    {
        private int width = 3;
        private Color markerColor = Color.White;
        private IEnumerable<IntPoint> points;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
        private bool connect;

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
        /// 
        public Color MarkerColor
        {
            get { return markerColor; }
            set { markerColor = value; }
        }

        /// <summary>
        ///   Gets or sets the set of points to mark.
        /// </summary>
        /// 
        public IEnumerable<IntPoint> Points
        {
            get { return points; }
            set { points = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the points 
        /// should be connected by line segments. Default is false.
        /// </summary>
        /// 
        public bool Connect
        {
            get { return connect; }
            set { connect = value; }
        }

        /// <summary>
        ///   Gets or sets the width of the points to be drawn.
        /// </summary>
        /// 
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PointsMarker"/> class.
        /// </summary>
        /// 
        public PointsMarker(IEnumerable<IFeaturePoint> points)
            : this(points, Color.White, 3)
        {
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="PointsMarker"/> class.
        /// </summary>
        /// 
        public PointsMarker(IEnumerable<IFeaturePoint> points, Color markerColor)
            : this(points, markerColor, 3)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PointsMarker"/> class.
        /// </summary>
        /// 
        public PointsMarker(IEnumerable<IFeaturePoint> points, Color markerColor, int width)
        {
            markerColor = init(points.Select(x => new IntPoint((int)x.X, (int)x.Y)), markerColor, width);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PointsMarker"/> class.
        /// </summary>
        /// 
        public PointsMarker(IEnumerable<IntPoint> points)
            : this(points, Color.White, 3)
        {
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="PointsMarker"/> class.
        /// </summary>
        /// 
        public PointsMarker(IEnumerable<IntPoint> points, Color markerColor)
            : this(points, markerColor, 3)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PointsMarker"/> class.
        /// </summary>
        /// 
        public PointsMarker(IEnumerable<IntPoint> points, Color markerColor, int width)
        {
            markerColor = init(points, markerColor, width);
        }

        private Color init(IEnumerable<IntPoint> points, Color markerColor, int width)
        {
            this.points = points;
            this.markerColor = markerColor;
            this.width = width;

            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            return markerColor;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PointsMarker"/> class.
        /// </summary>
        /// 
        public PointsMarker(Color markerColor)
            : this((IEnumerable<IntPoint>)null, markerColor, 3)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PointsMarker"/> class.
        /// </summary>
        /// 
        public PointsMarker(Color markerColor, int width)
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
            if (Connect)
            {
                Drawing.Polygon(image, points.ToList(), markerColor);
            }
            else
            {
                // mark all points
                foreach (IntPoint p in points)
                {
                    Drawing.FillRectangle(image, new Rectangle(p.X - width / 2, p.Y - width / 2, width, width), markerColor);
                }
            }
        }
    }
}