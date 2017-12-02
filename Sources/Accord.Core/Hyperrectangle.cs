// Accord Math Library
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

namespace Accord.Math
{
    using System.Globalization;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Accord.Compat;
    using System.Collections.Concurrent;

    /// <summary>
    ///   Hyperrectangle structure.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///   In geometry, an n-orthotope (also called a hyperrectangle or a box) is the generalization of a rectangle for higher 
    ///   dimensions, formally defined as the Cartesian product of intervals.</para>
    ///   
    /// <para>
    ///    References:
    ///    <list type="bullet">
    ///      <item><description>
    ///        Wikipedia contributors, "Hyperrectangle," Wikipedia, The Free Encyclopedia, 
    ///        https://en.wikipedia.org/w/index.php?title=Hyperrectangle </description></item>
    ///     </list></para>     
    /// </remarks>
    /// 
    /// <seealso cref="System.IFormattable" />
    /// 
    public struct Hyperrectangle : ICloneable, IEquatable<Hyperrectangle>, IFormattable
    {

        double[] min;
        double[] max;

        /// <summary>
        /// Gets the minimum point defining the lower bound of the hyperrectangle.
        /// </summary>
        /// 
        public double[] Min
        {
            get { return min; }
        }

        /// <summary>
        /// Gets the maximum point defining the upper bound of the hyperrectangle.
        /// </summary>
        /// 
        public double[] Max
        {
            get { return max; }
        }

        /// <summary>
        ///   Gets the number of dimensions of the hyperrectangle.
        /// </summary>
        /// 
        public int NumberOfDimensions
        {
            get { return min.Length; }
        }

        /// <summary>
        ///   Gets the length of each dimension. The length of the first dimension
        ///   can be referred as the width, the second as the height, and so on.
        /// </summary>
        /// 
        public double[] GetLength()
        {
            double[] length = new double[min.Length];
            for (int i = 0; i < length.Length; i++)
                length[i] = max[i] - min[i];
            return length;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Hyperrectangle"/> struct.
        /// </summary>
        /// 
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle..</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// 
        public Hyperrectangle(double x, double y, double width, double height)
        {
            this.min = new double[] { x, y };
            this.max = new double[] { x + width, y + height };
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Hyperrectangle"/> struct.
        /// </summary>
        /// 
        /// <param name="min">The minimum point in the hyperrectangle (the lower bound).</param>
        /// <param name="max">The maximum point in the hyperrectangle (the upper bound).</param>
        /// <param name="copy">Whether the passed vectors should be copied into this instance
        ///   or used as-is. Default is true (elements from the given vectors will be copied
        ///   into new array instances).</param>
        /// 
        public Hyperrectangle(double[] min, double[] max, bool copy = true)
        {
            if (min.Length != max.Length)
                throw new DimensionMismatchException("max");

            if (copy)
            {
                this.min = (double[])min.Clone();
                this.max = (double[])max.Clone();
            }
            else
            {
                this.min = min;
                this.max = max;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hyperrectangle"/> struct from minimum and maximum values.
        /// </summary>
        /// 
        /// <param name="min">The minimum point in the hyperrectangle (the lower bound).</param>
        /// <param name="max">The maximum point in the hyperrectangle (the upper bound).</param>
        /// <param name="copy">Whether the passed vectors should be copied into this instance
        ///   or used as-is. Default is true (elements from the given vectors will be copied
        ///   into new array instances).</param>
        /// 
        public static Hyperrectangle FromMinAndMax(double[] min, double[] max, bool copy = true)
        {
            return new Hyperrectangle(min, max, copy: copy);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hyperrectangle"/> struct from a minimum value and a size.
        /// </summary>
        /// 
        /// <param name="min">The minimum point in the hyperrectangle (the lower bound).</param>
        /// <param name="size">The size of each dimension (i.e. width, height, and so on).</param>
        /// <param name="copy">Whether the passed vectors should be copied into this instance
        ///   or used as-is. Default is true (elements from the given vectors will be copied
        ///   into new array instances).</param>
        /// 
        public static Hyperrectangle FromMinAndLength(double[] min, double[] size, bool copy = true)
        {
            if (copy)
            {
                min = (double[])min.Clone();
                size = (double[])size.Clone();
            }

            for (int i = 0; i < size.Length; i++)
                size[i] = min[i] + size[i];

            return new Hyperrectangle(min, size, copy: false);
        }


        /// <summary>
        ///   Determines if this rectangle intersects with rect.
        /// </summary>
        /// 
        public bool IntersectsWith(Hyperrectangle rect)
        {
            for (int i = 0; i < min.Length; i++)
            {
                double amini = min[i];
                double amaxi = max[i];

                double bmini = rect.min[i];
                double bmaxi = rect.max[i];

                if (amini >= bmaxi || amaxi < bmini)
                    return false;
            }

            return true;
        }

        /// <summary>
        ///   Determines if the specified point is contained within this Hyperrectangle structure.
        /// </summary>
        /// 
        public bool Contains(params double[] point)
        {
            for (int i = 0; i < point.Length; i++)
            {
                double mini = min[i];
                double maxi = max[i];

                double pointi = point[i];

                if (pointi < mini || pointi >= maxi)
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(Hyperrectangle other)
        {
            if (this.min.Length != other.min.Length)
                return false;

            for (int i = 0; i < min.Length; i++)
            {
                if (this.min[i] != other.min[i])
                    return false;
                if (this.max[i] != other.max[i])
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new Hyperrectangle((double[])min.Clone(), (double[])max.Clone());
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return ToString("G", null);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format to use.-or- A null reference (Nothing in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation.</param>
        /// <param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            if (formatProvider == null)
                formatProvider = System.Globalization.CultureInfo.CurrentCulture;

            if (NumberOfDimensions == 2)
            {
                return String.Format(formatProvider, format, 
                    "X = {0} Y = {1} Width = {2} Height = {3}",
                    min[0], min[1], max[0] - min[0], max[1] - min[1]);
            }

            return String.Format(formatProvider, format,
                "Min = {0} Max = {1} (Length = {2})", 
                min, max, GetLength());
        }
    }
}