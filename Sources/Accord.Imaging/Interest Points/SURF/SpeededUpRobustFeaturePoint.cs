// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Christopher Evans, 2009-2011
// http://www.chrisevansdev.com/
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
    using System;
    using System.Drawing;

    /// <summary>
    ///   Speeded-Up Robust Feature (SURF) Point.
    /// </summary>
    /// 
    /// <seealso cref="SpeededUpRobustFeaturesDetector"/>
    /// <seealso cref="SpeededUpRobustFeaturesDescriptor"/>
    /// 
    [Serializable]
    public class SpeededUpRobustFeaturePoint : IFeaturePoint
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="SpeededUpRobustFeaturePoint"/> class.
        /// </summary>
        /// 
        /// <param name="x">The x-coordinate of the point in the image.</param>
        /// <param name="y">The y-coordinate of the point in the image.</param>
        /// <param name="scale">The point's scale.</param>
        /// <param name="laplacian">The point's laplacian value.</param>
        /// 
        public SpeededUpRobustFeaturePoint(double x, double y, double scale, int laplacian)
        {
            this.X = x;
            this.Y = y;
            this.Scale = scale;
            this.Laplacian = laplacian;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SpeededUpRobustFeaturePoint"/> class.
        /// </summary>
        /// 
        /// <param name="x">The x-coordinate of the point in the image.</param>
        /// <param name="y">The y-coordinate of the point in the image.</param>
        /// <param name="scale">The point's scale.</param>
        /// <param name="laplacian">The point's laplacian value.</param>
        /// <param name="orientation">The point's orientation angle.</param>
        /// <param name="response">The point's response value.</param>
        /// 
        public SpeededUpRobustFeaturePoint(double x, double y, double scale, int laplacian,
            double orientation, double response)
        {
            this.X = x;
            this.Y = y;
            this.Scale = scale;
            this.Laplacian = laplacian;
            this.Orientation = orientation;
            this.Response = response;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SpeededUpRobustFeaturePoint"/> class.
        /// </summary>
        /// 
        /// <param name="x">The x-coordinate of the point in the image.</param>
        /// <param name="y">The y-coordinate of the point in the image.</param>
        /// <param name="scale">The point's scale.</param>
        /// <param name="laplacian">The point's Laplacian value.</param>
        /// <param name="descriptor">The SURF point descriptor.</param>
        /// <param name="orientation">The point's orientation angle.</param>
        /// <param name="response">The point's response value.</param>
        /// 
        public SpeededUpRobustFeaturePoint(double x, double y, double scale, int laplacian,
            double orientation, double response, double[] descriptor)
        {
            this.X = x;
            this.Y = y;
            this.Scale = scale;
            this.Laplacian = laplacian;
            this.Orientation = orientation;
            this.Response = response;
            this.Descriptor = descriptor;
        }


        /// <summary>
        ///   Gets or sets the x-coordinate of this point.
        /// </summary>
        /// 
        public double X { get; set; }

        /// <summary>
        ///   Gets or sets the y-coordinate of this point.
        /// </summary>
        /// 
        public double Y { get; set; }

        /// <summary>
        ///   Gets or sets the scale of the point.
        /// </summary>
        /// 
        public double Scale { get; set; }

        /// <summary>
        ///   Gets or sets the response of the detected feature (strength).
        /// </summary>
        /// 
        public double Response { get; set; }

        /// <summary>
        ///   Gets or sets the orientation of this point
        ///   measured anti-clockwise from the x-axis.
        /// </summary>
        /// 
        public double Orientation { get; set; }

        /// <summary>
        ///   Gets or sets the sign of laplacian for this point
        ///   (which may be useful for fast matching purposes).
        /// </summary>
        /// 
        public int Laplacian { get; set; }

        /// <summary>
        ///   Gets or sets the descriptor vector
        ///   associated with this point.
        /// </summary>
        /// 
        public double[] Descriptor { get; set; }



        /// <summary>
        ///   Converts the feature point to a <see cref="AForge.IntPoint"/>.
        /// </summary>
        /// 
        public AForge.IntPoint ToIntPoint()
        {
            return new AForge.IntPoint((int)X, (int)Y);
        }

        /// <summary>
        ///   Converts this object into a <see cref="AForge.IntPoint"/>.
        /// </summary>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public Point ToPoint()
        {
            return new Point((int)X, (int)Y);
        }

        /// <summary>
        ///   Converts this object into a <see cref="System.Drawing.PointF"/>.
        /// </summary>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public PointF ToPointF()
        {
            return new PointF((float)X, (float)Y);
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="Accord.Imaging.SpeededUpRobustFeaturePoint"/>
        ///   to <see cref="System.Drawing.Point"/>.
        /// </summary>
        /// 
        /// <param name="point">The point to be converted.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public static implicit operator Point(SpeededUpRobustFeaturePoint point)
        {
            return point.ToPoint();
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="Accord.Imaging.SpeededUpRobustFeaturePoint"/>
        ///   to <see cref="System.Drawing.PointF"/>.
        /// </summary>
        /// 
        /// <param name="point">The point to be converted.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public static implicit operator PointF(SpeededUpRobustFeaturePoint point)
        {
            return point.ToPointF();
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="Accord.Imaging.SpeededUpRobustFeaturePoint"/>
        ///   to <see cref="AForge.IntPoint"/>.
        /// </summary>
        /// 
        /// <param name="point">The point to be converted.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public static implicit operator AForge.IntPoint(SpeededUpRobustFeaturePoint point)
        {
            return point.ToIntPoint();
        }
    }
}
