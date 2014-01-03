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
    ///   Common interface for Matrix format providers.
    /// </summary>
    /// 
    public interface IMatrixFormatProvider : IFormatProvider
    {

        #region Formatting specification
        /// <summary>A string denoting the start of the matrix to be used in formatting.</summary>
        string FormatMatrixStart { get; }

        /// <summary>A string denoting the end of the matrix to be used in formatting.</summary>
        string FormatMatrixEnd { get; }

        /// <summary>A string denoting the start of a matrix row to be used in formatting.</summary>
        string FormatRowStart { get; }

        /// <summary>A string denoting the end of a matrix row to be used in formatting.</summary>
        string FormatRowEnd { get; }

        /// <summary>A string denoting the start of a matrix column to be used in formatting.</summary>
        string FormatColStart { get; }

        /// <summary>A string denoting the end of a matrix column to be used in formatting.</summary>
        string FormatColEnd { get; }

        /// <summary>A string containing the row delimiter for a matrix to be used in formatting.</summary>
        string FormatRowDelimiter { get; }

        /// <summary>A string containing the column delimiter for a matrix to be used in formatting.</summary>
        string FormatColDelimiter { get; }
        #endregion


        #region Parsing specification
        /// <summary>A string denoting the start of the matrix to be used in parsing.</summary>
        string ParseMatrixStart { get; }

        /// <summary>A string denoting the end of the matrix to be used in parsing.</summary>
        string ParseMatrixEnd { get; }

        /// <summary>A string denoting the start of a matrix row to be used in parsing.</summary>
        string ParseRowStart { get; }

        /// <summary>A string denoting the end of a matrix row to be used in parsing.</summary>
        string ParseRowEnd { get; }

        /// <summary>A string denoting the start of a matrix column to be used in parsing.</summary>
        string ParseColStart { get; }

        /// <summary>A string denoting the end of a matrix column to be used in parsing.</summary>
        string ParseColEnd { get; }

        /// <summary>A string containing the row delimiter for a matrix to be used in parsing.</summary>
        string ParseRowDelimiter { get; }

        /// <summary>A string containing the column delimiter for a matrix to be used in parsing.</summary>
        string ParseColDelimiter { get; }
        #endregion


        /// <summary>
        ///   Gets the culture specific formatting information
        ///   to be used during parsing or formatting.
        /// </summary>
        IFormatProvider InnerProvider { get; }
        
    }


}
