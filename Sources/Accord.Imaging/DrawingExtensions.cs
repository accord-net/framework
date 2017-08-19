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
    using Accord.Math;
    using System;
    using System.Drawing;

    /// <summary>
    ///   Extension methods for drawwing structures.
    /// </summary>
    /// 
    public static class DrawingExtensions
    {

        /// <summary>
        ///   Convert the given hyperrectangle in to a System.Drawing.Rectangle.
        /// </summary>
        /// 
        public static Rectangle ToRectangle(this Hyperrectangle rect)
        {
            if (rect.NumberOfDimensions != 2)
                throw new ArgumentException("rect");

            return Rectangle.FromLTRB(
                left: (int)rect.Min[0],
                top: (int)rect.Min[1],
                right: (int)rect.Max[0],
                bottom: (int)rect.Max[1]);
        }

        /// <summary>
        ///   Convert the given hyperrectangle in to a System.Drawing.RectangleF.
        /// </summary>
        /// 
        public static RectangleF ToRectangleF(this Hyperrectangle rect)
        {
            if (rect.NumberOfDimensions != 2)
                throw new ArgumentException("rect");

            return RectangleF.FromLTRB(
                left: (float)rect.Min[0],
                top: (float)rect.Min[1],
                right: (float)rect.Max[0],
                bottom: (float)rect.Max[1]);
        }

        /// <summary>
        ///   Convert the given System.Drawing.Rectangle to a <see cref="Hyperrectangle"/>.
        /// </summary>
        /// 
        public static Hyperrectangle ToHyperrectangle(this Rectangle rect)
        {
            return new Hyperrectangle(new double[] { rect.Left, rect.Top }, new double[] { rect.Right, rect.Bottom });
        }

        /// <summary>
        ///   Convert the given System.Drawing.RectangleF to a <see cref="Hyperrectangle"/>.
        /// </summary>
        /// 
        public static Hyperrectangle ToHyperrectangle(this RectangleF rect)
        {
            return new Hyperrectangle(new double[] { rect.Left, rect.Top }, new double[] { rect.Right, rect.Bottom });
        }
    }
}
