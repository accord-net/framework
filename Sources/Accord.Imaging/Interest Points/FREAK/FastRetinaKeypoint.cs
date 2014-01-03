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
    using System.Text;
    using System.Drawing;

    /// <summary>
    ///   Fast Retina Keypoint (FREAK) point.
    /// </summary>
    /// 
    /// <seealso cref="FastRetinaKeypointDescriptor"/>
    /// <seealso cref="FastRetinaKeypointDescriptor"/>
    /// 
    [Serializable]
    public class FastRetinaKeypoint : IFeaturePoint<byte[]>
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="FastRetinaKeypoint"/> class.
        /// </summary>
        /// 
        /// <param name="x">The x-coordinate of the point in the image.</param>
        /// <param name="y">The y-coordinate of the point in the image.</param>
        /// 
        public FastRetinaKeypoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
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
        ///   Gets or sets the orientation of this point in angles.
        /// </summary>
        /// 
        public double Orientation { get; set; }


        /// <summary>
        ///   Gets or sets the descriptor vector
        ///   associated with this point.
        /// </summary>
        /// 
        public byte[] Descriptor { get; set; }


        /// <summary>
        ///   Converts the binary descriptor to 
        ///   string of hexadecimal values.
        /// </summary>
        /// 
        /// <returns>A string containing an hexadecimal
        /// value representing this point's descriptor.</returns>
        /// 
        public string ToHex()
        {
            if (Descriptor == null)
                return String.Empty;

            StringBuilder hex = new StringBuilder(Descriptor.Length * 2);
            foreach (byte b in Descriptor)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        /// <summary>
        ///   Converts the binary descriptor
        ///   to a string of binary values.
        /// </summary>
        /// 
        /// <returns>A string containing a binary value
        /// representing this point's descriptor.</returns>
        /// 
        public string ToBinary()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Descriptor.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bool set = (Descriptor[i] & (1 << j)) != 0;
                    sb.Append(set ? "1" : "0");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///   Converts the binary descriptor to base64.
        /// </summary>
        /// 
        /// <returns>A string containing the base64 
        /// representation of the descriptor.</returns>
        /// 
        public string ToBase64()
        {
            if (Descriptor == null)
                return String.Empty;

            return Convert.ToBase64String(Descriptor);
        }




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
        ///   Performs an implicit conversion from <see cref="Accord.Imaging.FastRetinaKeypoint"/>
        ///   to <see cref="System.Drawing.Point"/>.
        /// </summary>
        /// 
        /// <param name="point">The point to be converted.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public static implicit operator Point(FastRetinaKeypoint point)
        {
            return point.ToPoint();
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="Accord.Imaging.FastRetinaKeypoint"/>
        ///   to <see cref="System.Drawing.PointF"/>.
        /// </summary>
        /// 
        /// <param name="point">The point to be converted.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public static implicit operator PointF(FastRetinaKeypoint point)
        {
            return point.ToPointF();
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="Accord.Imaging.FastRetinaKeypoint"/>
        ///   to <see cref="AForge.IntPoint"/>.
        /// </summary>
        /// 
        /// <param name="point">The point to be converted.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public static implicit operator AForge.IntPoint(FastRetinaKeypoint point)
        {
            return point.ToIntPoint();
        }
    }
}
