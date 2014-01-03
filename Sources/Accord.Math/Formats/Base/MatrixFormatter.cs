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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    ///   Defines how matrices are formatted and displayed, depending on the
    ///   chosen format representation.
    /// </summary>
    /// 
    public class MatrixFormatter : ICustomFormatter
    {

        /// <summary>
        ///   Converts the value of a specified object to an equivalent string
        ///   representation using specified formatting information.
        /// </summary>
        /// <param name="format">A format string containing formatting specifications.</param>
        /// <param name="arg">An object to format.</param>
        /// <param name="formatProvider">
        ///   An <see cref="T:System.IFormatProvider"/> object that supplies
        ///   format information about the current instance.</param>
        /// <returns>
        ///   The string representation of the value of <paramref name="arg"/>,
        ///   formatted as specified by <paramref name="format"/> and
        ///   <paramref name="formatProvider"/>.
        /// </returns>
        /// 
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            IMatrixFormatProvider provider = formatProvider as IMatrixFormatProvider;
            Array obj = arg as Array;

            // Check if the user has provided the correct format provider
            // for a matrix or if the argument is indeed an array (matrix)
            if (provider != null && obj != null)
            {
                return Format(format, obj, provider);
            }
            else
            {
                // No, it has not. Try to provide standard formatting for the type
                return handleOtherFormats(format, arg, CultureInfo.CurrentCulture);
            }
        }



        #region Static methods for output formatting
        /// <summary>
        ///   Converts a jagged or multidimensional array into a <a cref="System.String">System.String</a> representation.
        /// </summary>
        /// 
        public static string Format(string format, Array matrix, IMatrixFormatProvider formatProvider)
        {
            // Initial argument checking
            if (matrix.Rank > 2)
            {
                throw new NotSupportedException("Matrices with more than two dimensions are not supported.");
            }

            // Try to parse the format options passed through the "format" argument
            string newline, elementFormat;
            if (!parseOptions(format, out newline, out elementFormat))
            {
                throw new FormatException(String.Format("The format of '{0}' is invalid.", format));
            }

            IFormatProvider culture = formatProvider.InnerProvider;


            // Retrieve matrix dimensions. If the matrix is a jagged array,
            //  we will compute the columns for each of the rows.
            int rows = matrix.GetLength(0);
            int cols = (matrix.Rank == 2) ? matrix.GetLength(1) : 0;


            // Initialize the matrix construction
            StringBuilder sb = new StringBuilder();
            sb.Append(formatProvider.FormatMatrixStart);


            // For each row
            for (int i = 0; i < rows; i++)
            {
                // Start constructing the row
                sb.Append(formatProvider.FormatRowStart);

                // Construct the columns for the row
                if (matrix.Rank == 1)
                {
                    Object obj = matrix.GetValue(i);
                    Array row = obj as Array;

                    if (row == null)
                    {
                        sb.Append(handleOtherFormats(elementFormat, obj, culture));
                    }
                    else
                    {
                        #region Process row for jagged arrays
                        cols = row.Length;

                        // For each column
                        for (int j = 0; j < cols; j++)
                        {
                            sb.Append(handleOtherFormats(elementFormat, row.GetValue(j), culture));

                            if (j < cols - 1) sb.Append(formatProvider.FormatColDelimiter);
                        }
                        #endregion
                    }
                }
                else
                {
                    #region Process row for multidimensional arrays

                    // For each column
                    for (int j = 0; j < cols; j++)
                    {
                        sb.Append(handleOtherFormats(elementFormat, matrix.GetValue(i, j), culture));

                        if (j < cols - 1) sb.Append(formatProvider.FormatColDelimiter);
                    }
                    #endregion
                }

                // Finalize constructing the row
                sb.Append(formatProvider.FormatRowEnd);

                // Check if we are still in the middle of the row
                if (i < rows - 1) sb.Append(formatProvider.FormatRowDelimiter);
            }

            // Finalize constructing the matrix
            sb.Append(formatProvider.FormatMatrixEnd);


            // Finally, perform post-processing such as replacing user
            // selected newlines or presenting the output in just one line.
            String str = sb.ToString();
            str = str.Replace("\n", newline);

            if (String.IsNullOrEmpty(newline))
                str = Regex.Replace(str, " +", " ");

            return str;
        }

        /// <summary>
        ///   Parses a format string containing the format options for the matrix representation.
        /// </summary>
        private static bool parseOptions(string format, out string newline, out string elementFormat)
        {
            // "{0,g}"      -> multiline with system new line character and number format g
            // "{0:Mn,g}"   -> multiline with \n new line character and number format g
            // "{0:Mnr,g}"  -> multiline with \n\r new line character and number format g
            // "{0:Ms,g}"   -> single line and number format g

            if (String.IsNullOrEmpty(format))
            {
                newline = Environment.NewLine;
                elementFormat = String.Empty;
                return true;
            }

            string[] options = format.Split(',');

            if (options.Length == 1)
            {
                elementFormat = String.Empty;

                switch (options[0])
                {
                    case "Mn":
                        newline = "\n";
                        return true;

                    case "Mnr":
                        newline = "\n\r";
                        return true;

                    case "Ms":
                        newline = String.Empty;
                        return true;

                    default:
                        newline = Environment.NewLine;
                        elementFormat = format;
                        return true;
                }
            }

            if (options.Length == 2)
            {
                elementFormat = options[1];

                switch (options[0])
                {
                    case "Mn":
                        newline = "\n"; break;

                    case "Mnr":
                        newline = "\n\r"; break;

                    case "Ms":
                        newline = String.Empty; break;

                    default:
                        newline = String.Empty;
                        return false;
                }

                return true;
            }

            newline = String.Empty;
            elementFormat = format;
            return false;
        }

        /// <summary>
        ///   Handles formatting for objects other than matrices.
        /// </summary>
        private static string handleOtherFormats(string format, object arg, IFormatProvider culture)
        {
            try
            {
                IFormattable obj = arg as IFormattable;
                if (obj != null)
                {
                    return obj.ToString(format, culture);
                }
                else if (arg != null)
                {
                    return arg.ToString();
                }
            }
            catch (FormatException e)
            {
                throw new FormatException(String.Format("The format of '{0}' is invalid.", format), e);
            }

            return String.Empty;
        }

        #endregion



        #region Static methods for input parsing
        /// <summary>
        ///   Converts a matrix represented in a System.String into a jagged array.
        /// </summary>
        /// 
        public static double[][] ParseJagged(string str, IMatrixFormatProvider provider)
        {
            // remove excess spaces
            str = Regex.Replace(str, @" +", " ");

            // First remove starting and trailing tokens
            str = str.Remove(0, provider.ParseMatrixStart.Length);
            str = str.Remove(str.Length - provider.ParseMatrixEnd.Length, provider.ParseMatrixEnd.Length);

            // Now split rows
            string[] strRows = str.Split(new string[] { provider.ParseRowDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            List<double[]> rows = new List<double[]>();

            foreach (string strRow in strRows)
            {
                string row = strRow.Trim();

                // Remove starting and trailing tokens
                if (row.StartsWith(provider.ParseRowStart, StringComparison.Ordinal))
                    row = row.Remove(0, provider.ParseRowStart.Length);
                if (row.EndsWith(provider.ParseRowEnd, StringComparison.Ordinal))
                    row = row.Remove(row.Length - provider.ParseRowEnd.Length, provider.ParseRowEnd.Length);

                // Now split rows values
                string[] strCols = row.Split(new string[] { provider.ParseColDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                List<double> values = new List<double>();

                foreach (string strCol in strCols)
                {
                    string col = Regex.Replace(strCol, @"\s", String.Empty);

                    // Remove starting and trailing tokens
                    if (col.StartsWith(provider.ParseColStart, StringComparison.Ordinal))
                        col = col.Remove(0, provider.ParseColStart.Length);
                    if (col.EndsWith(provider.ParseColEnd, StringComparison.Ordinal))
                        col = col.Remove(col.Length - provider.ParseColEnd.Length, provider.ParseColEnd.Length);

                    // finally, parse the value and store
                    values.Add(Double.Parse(col, provider.InnerProvider));
                }

                rows.Add(values.ToArray());
            }

            return rows.ToArray();
        }

        /// <summary>
        ///   Converts a matrix represented in a System.String into a multi-dimensional array.
        /// </summary>
        /// 
        public static double[,] ParseMultidimensional(string str, IMatrixFormatProvider provider)
        {
            return Matrix.ToMatrix(ParseJagged(str, provider));
        }
       
        #endregion


    }
}
