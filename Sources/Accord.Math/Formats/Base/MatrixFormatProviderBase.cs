// Accord Math Library
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

namespace Accord.Math
{
    using System;
    using System.Globalization;

    /// <summary>
    ///   Base class for IMatrixFormatProvider implementers.
    /// </summary>
    /// 
    public abstract class MatrixFormatProviderBase : IMatrixFormatProvider
    {

        #region Formatting specification
        /// <summary>
        /// A string denoting the start of the matrix to be used in formatting.
        /// </summary>
        public string FormatMatrixStart { get; protected set; }

        /// <summary>
        /// A string denoting the end of the matrix to be used in formatting.
        /// </summary>
        public string FormatMatrixEnd { get; protected set; }

        /// <summary>
        /// A string denoting the start of a matrix row to be used in formatting.
        /// </summary>
        public string FormatRowStart { get; protected set; }
        /// <summary>
        /// A string denoting the end of a matrix row to be used in formatting.
        /// </summary>
        public string FormatRowEnd { get; protected set; }

        /// <summary>
        /// A string denoting the start of a matrix column to be used in formatting.
        /// </summary>
        public string FormatColStart { get; protected set; }

        /// <summary>
        /// A string denoting the end of a matrix column to be used in formatting.
        /// </summary>
        public string FormatColEnd { get; protected set; }

        /// <summary>
        /// A string containing the row delimiter for a matrix to be used in formatting.
        /// </summary>
        public string FormatRowDelimiter { get; protected set; }

        /// <summary>
        /// A string containing the column delimiter for a matrix to be used in formatting.
        /// </summary>
        public string FormatColDelimiter { get; protected set; }
        #endregion

        #region Parsing specification
        /// <summary>
        /// A string denoting the start of the matrix to be used in parsing.
        /// </summary>
        public string ParseMatrixStart { get; protected set; }

        /// <summary>
        /// A string denoting the end of the matrix to be used in parsing.
        /// </summary>
        public string ParseMatrixEnd { get; protected set; }

        /// <summary>
        /// A string denoting the start of a matrix row to be used in parsing.
        /// </summary>
        public string ParseRowStart { get; protected set; }

        /// <summary>
        /// A string denoting the end of a matrix row to be used in parsing.
        /// </summary>
        public string ParseRowEnd { get; protected set; }

        /// <summary>
        /// A string denoting the start of a matrix column to be used in parsing.
        /// </summary>
        public string ParseColStart { get; protected set; }

        /// <summary>
        /// A string denoting the end of a matrix column to be used in parsing.
        /// </summary>
        public string ParseColEnd { get; protected set; }

        /// <summary>
        /// A string containing the row delimiter for a matrix to be used in parsing.
        /// </summary>
        public string ParseRowDelimiter { get; protected set; }

        /// <summary>
        /// A string containing the column delimiter for a matrix to be used in parsing.
        /// </summary>
        public string ParseColDelimiter { get; protected set; }
        #endregion


        /// <summary>
        ///   Gets the culture specific formatting information
        ///   to be used during parsing or formatting.
        /// </summary>
        /// 
        public IFormatProvider InnerProvider { get; protected set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="MatrixFormatProviderBase"/> class.
        /// </summary>
        /// 
        /// <param name="innerProvider">The inner format provider.</param>
        /// 
        protected MatrixFormatProviderBase(IFormatProvider innerProvider)
        {
            this.InnerProvider = innerProvider;
        }

        /// <summary>
        ///   Returns an object that provides formatting services for the specified
        ///   type. Currently, only <see cref="IMatrixFormatProvider"/> is supported.
        /// </summary>
        /// <param name="formatType">
        ///   An object that specifies the type of format
        ///   object to return. </param>
        /// <returns>
        ///   An instance of the object specified by formatType, if the
        ///   <see cref="IFormatProvider">IFormatProvider</see> implementation
        ///   can supply that type of object; otherwise, null.</returns>
        ///   
        public object GetFormat(Type formatType)
        {
            // Determine whether custom formatting object is requested.

            if (formatType == typeof(ICustomFormatter))
            {
                return new MatrixFormatter();
            }

            return null;
        }

    }
}
