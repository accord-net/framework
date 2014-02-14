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
    ///   Gets the matrix representation used in C# jagged arrays.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class can be used to convert to and from C#
    ///   arrays and their string representation. Please 
    ///   see the example for details.
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   Converting from a jagged matrix to a string representation:</para>
    ///   
    /// <code>
    ///   // Declare a number array
    ///   double[][] x = 
    ///   {
    ///      new double[] { 1, 2, 3, 4 },
    ///      new double[] { 5, 6, 7, 8 },
    ///   };
    ///   
    ///   // Convert the aforementioned array to a string representation:
    ///   string str = x.ToString(CSharpJaggedMatrixFormatProvider.CurrentCulture);
    ///   
    ///   // the final result will be equivalent to
    ///   "double[][] x =                  " +
    ///   "{                               " +
    ///   "   new double[] { 1, 2, 3, 4 }, " +
    ///   "   new double[] { 5, 6, 7, 8 }, " +
    ///   "}"
    /// </code>
    /// 
    /// <para>
    ///   Converting from strings to actual arrays:</para>
    /// 
    /// <code>
    ///   // Declare an input string
    ///   string str = "double[][] x =     " +
    ///   "{                               " +
    ///   "   new double[] { 1, 2, 3, 4 }, " +
    ///   "   new double[] { 5, 6, 7, 8 }, " +
    ///   "}";
    ///   
    ///   // Convert the string representation to an actual number array:
    ///   double[][] array = Matrix.Parse(str, CSharpJaggedMatrixFormatProvider.InvariantCulture);
    ///   
    ///   // array will now contain the actual jagged 
    ///   // array representation of the given string.
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Accord.Math.Matrix"/>
    /// <seealso cref="CSharpMatrixFormatProvider"/>
    /// 
    /// <seealso cref="CSharpJaggedMatrixFormatProvider"/>
    /// <seealso cref="CSharpArrayFormatProvider"/>
    /// 
    /// <seealso cref="OctaveMatrixFormatProvider"/>
    /// <seealso cref="OctaveArrayFormatProvider"/>
    /// 
    public sealed class CSharpJaggedMatrixFormatProvider : MatrixFormatProviderBase
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="CSharpJaggedMatrixFormatProvider"/> class.
        /// </summary>
        /// 
        public CSharpJaggedMatrixFormatProvider(IFormatProvider innerProvider)
            : base(innerProvider)
        {
            FormatMatrixStart = "new double[][] {\n";
            FormatMatrixEnd = " \n};";
            FormatRowStart = "    new double[] { ";
            FormatRowEnd = " }";
            FormatColStart = ", ";
            FormatColEnd = ", ";
            FormatRowDelimiter = ",\n";
            FormatColDelimiter = ", ";

            ParseMatrixStart = "new double[][] {";
            ParseMatrixEnd = "};";
            ParseRowStart = "new double[] {";
            ParseRowEnd = "}";
            ParseColStart = ",";
            ParseColEnd = ",";
            ParseRowDelimiter = "},";
            ParseColDelimiter = ",";
        }

        /// <summary>
        ///   Gets the IMatrixFormatProvider which uses the CultureInfo used by the current thread.
        /// </summary>
        /// 
        public static CSharpJaggedMatrixFormatProvider CurrentCulture
        {
            get { return currentCulture; }
        }

        /// <summary>
        ///   Gets the IMatrixFormatProvider which uses the invariant system culture.
        /// </summary>
        /// 
        public static CSharpJaggedMatrixFormatProvider InvariantCulture
        {
            get { return invariantCulture; }
        }

        private static readonly CSharpJaggedMatrixFormatProvider currentCulture =
            new CSharpJaggedMatrixFormatProvider(CultureInfo.CurrentCulture);

        private static readonly CSharpJaggedMatrixFormatProvider invariantCulture =
            new CSharpJaggedMatrixFormatProvider(CultureInfo.InvariantCulture);

    }
}
