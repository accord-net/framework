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
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Imaging.Filters;
    using System.Collections.Generic;
    using AForge.Imaging;

    /// <summary>
    ///   Filter to mark (highlight) feature points in a image.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The filter highlights feature points on the image using a given set of points.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale and 24 color images for processing.</para>
    /// </remarks>
    /// 
    public class FeaturesMarker : BaseFilter
    {

        private IEnumerable<SpeededUpRobustFeaturePoint> points;
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
        ///   Gets or sets the set of points to mark.
        /// </summary>
        /// 
        public IEnumerable<SpeededUpRobustFeaturePoint> Points
        {
            get { return points; }
            set { points = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FeaturesMarker"/> class.
        /// </summary>
        /// 
        public FeaturesMarker()
            : this(new SpeededUpRobustFeaturePoint[0])
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FeaturesMarker"/> class.
        /// </summary>
        /// 
        public FeaturesMarker(IEnumerable<SpeededUpRobustFeaturePoint> points)
        {
            this.points = points;

            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        private GrayscaleToRGB toRGB = new GrayscaleToRGB();

        /// <summary>
        ///   Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        ///
        protected override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            if (sourceData.PixelFormat == PixelFormat.Format8bppIndexed)
                sourceData = toRGB.Apply(sourceData);
            
            // Copy image contents
            sourceData.Copy(destinationData);

            Bitmap managedImage = destinationData.ToManagedImage(makeCopy: false);

            using (Graphics g = Graphics.FromImage(managedImage))
            using (Pen positive = new Pen(Color.Red))
            using (Pen negative = new Pen(Color.Blue))
            using (Pen line = new Pen(Color.FromArgb(0, 255, 0)))
            {
                // mark all points
                foreach (SpeededUpRobustFeaturePoint p in points)
                {
                    int S = 2 * (int)(2.5f * p.Scale);
                    int R = (int)(S / 2f);

                    Point pt = new Point((int)p.X, (int)p.Y);
                    Point ptR = new Point((int)(R * System.Math.Cos(p.Orientation)),
                                          (int)(R * System.Math.Sin(p.Orientation)));

                    Pen myPen = (p.Laplacian > 0 ? negative : positive);

                    g.DrawEllipse(myPen, pt.X - R, pt.Y - R, S, S);
                    g.DrawLine(line, new Point(pt.X, pt.Y), new Point(pt.X + ptR.X, pt.Y + ptR.Y));
                }
            }

        }
    }
}