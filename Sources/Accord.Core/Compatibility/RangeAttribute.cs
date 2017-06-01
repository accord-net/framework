// Accord Core Library
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

#if NETSTANDARD2_0
namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    ///   Specifies the numeric range constraints for the value of a data field.
    /// </summary>
    /// 
    /// <seealso cref="System.Attribute" />
    /// 
    public class RangeAttribute : Attribute
    {
        /// <summary>
        ///   Gets the minimum allowed field value.
        /// </summary>
        public object Minimum { get; private set; }

        /// <summary>
        ///   Gets the maximum allowed field value.
        /// </summary>
        public object Maximum { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RangeAttribute"/> class by using the specified minimum and maximum values.
        /// </summary>
        /// 
        /// <param name="minimum">Specifies the minimum value allowed for the data field value.</param>
        /// <param name="maximum">TSpecifies the maximum value allowed for the data field value.</param>
        /// 
        public RangeAttribute(object minimum, object maximum)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
        }
    }
}
#endif
