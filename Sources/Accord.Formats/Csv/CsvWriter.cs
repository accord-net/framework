// Accord Formats Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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

namespace Accord.IO.Csv
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.IO;

    /// <summary>
    ///   Writer for CSV data.
    /// </summary>
    /// 
    public class CsvWriter : IDisposable
    {
        /// <summary>
        ///   Gets the writer.
        /// </summary>
        /// 
        /// <value>
        ///   The writer.
        /// </value>
        /// 
        public TextWriter Writer { get; private set; }

        /// <summary>
        ///   Gets or sets the comment character indicating that a line is commented out.
        /// </summary>
        /// 
        /// <value>The comment character indicating that a line is commented out.</value>
        /// 
        public string Comment { get; set; }

        /// <summary>
        ///   Gets or sets the escape character letting insert quotation characters inside a quoted field.
        /// </summary>
        /// 
        /// <value>The escape character letting insert quotation characters inside a quoted field.</value>
        /// 
        public string Escape { get; set; }

        /// <summary>
        ///   Gets or sets the delimiter character separating each field.
        /// </summary>
        /// 
        /// <value>The delimiter character separating each field.</value>
        /// 
        public string Delimiter { get; set; }

        /// <summary>
        ///   Gets or sets the quotation character wrapping every field.
        /// </summary>
        /// 
        /// <value>The quotation character wrapping every field.</value>
        /// 
        public string Quote { get; set; }

        /// <summary>
        ///   Gets or sets the format provider to use when converting 
        ///   data-types to text representations. Default is to use
        ///   CultureInfo.InvariantCulture.
        /// </summary>
        /// 
        /// <value>
        ///   The format provider.
        /// </value>
        /// 
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CsvWriter"/> class.
        /// </summary>
        /// 
        /// <param name="writer">A <see cref="T:TextWriter"/> pointing to the CSV file.</param>
        /// 
        public CsvWriter(TextWriter writer)
        {
            this.Writer = writer;
            this.Quote = CsvReader.DefaultQuote.ToString();
            this.Delimiter = CsvReader.DefaultDelimiter.ToString();
            this.Comment = CsvReader.DefaultComment.ToString();
            this.Escape = CsvReader.DefaultEscape.ToString();
            this.FormatProvider = System.Globalization.CultureInfo.InvariantCulture;
        }

        /// <summary>
        ///   Writes the column names of a data table as the headers of the CSV file.
        /// </summary>
        /// 
        /// <param name="table">A DataTable whose columns names will be written as headers.</param>
        /// 
        public void WriteHeaders(DataTable table)
        {
            var headers = new string[table.Columns.Count];
            for (int i = 0; i < headers.Length; i++)
                headers[i] = quote(table.Columns[i].ColumnName);

            write(headers, String.Empty);
        }


        /// <summary>
        ///   Writes the specified matrix in CSV format.
        /// </summary>
        /// 
        /// <typeparam name="T">The matrix data type.</typeparam>
        /// <param name="table">The table to be written.</param>
        /// 
        public void Write<T>(T[,] table)
        {
            int rows = table.GetLength(0);
            int cols = table.GetLength(1);

            string[] items = new string[cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < items.Length; j++)
                    items[j] = quote(table[i, j]);

                write(items, String.Empty);
            }

            Writer.Flush();
        }

        /// <summary>
        ///   Writes the specified matrix in CSV format.
        /// </summary>
        /// 
        /// <typeparam name="T">The matrix data type.</typeparam>
        /// <param name="table">The table to be written.</param>
        /// 
        public void Write<T>(T[][] table)
        {
            for (int i = 0; i < table.Length; i++)
            {
                string[] items = new string[table[i].Length];

                for (int j = 0; j < items.Length; j++)
                    items[j] = quote(table[i][j]);

                write(items, String.Empty);
            }

            Writer.Flush();
        }

        /// <summary>
        ///   Writes the specified table in a CSV format.
        /// </summary>
        /// 
        /// <param name="table">The data table to be written.</param>
        /// 
        public void Write(DataTable table)
        {
            string[] items = new string[table.Columns.Count];

            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < items.Length; i++)
                    items[i] = quote(row[i]);

                write(items, row.RowError);
            }

            Writer.Flush();
        }

        /// <summary>
        ///   Writes the specified fields in a CSV format.
        /// </summary>
        /// 
        /// <param name="fields">The fields to be written.</param>
        /// 
        public void WriteLine<T>(T[] fields)
        {
            WriteLine(fields, String.Empty);
        }

        /// <summary>
        ///   Writes the specified fields in a CSV format.
        /// </summary>
        /// 
        /// <param name="fields">The fields to be written.</param>
        /// <param name="comment">An optional comment for the line.</param>
        /// 
        public void WriteLine<T>(T[] fields, string comment)
        {
            string[] items = new string[fields.Length];

            for (int i = 0; i < items.Length; i++)
                items[i] = quote(items[i]);

            write(items, comment);
        }





        private void write(string[] fields, string comment)
        {
            Writer.Write(String.Join(Delimiter, fields));

            if (!String.IsNullOrEmpty(comment))
                Writer.Write(" {0} {1}", Comment, comment);

            Writer.WriteLine();
        }

        private string escape(object obj)
        {
            string text = String.Format(FormatProvider, "{0}", obj);

            text = text.Replace(Quote, Escape + Quote);

            return text;
        }

        private string quote(object obj)
        {
            string text = escape(obj);
            return String.Format(FormatProvider, "{0}{1}{0}", Quote, text);
        }





        /// <summary>
        ///   Performs application-defined tasks associated with 
        ///   freeing,  releasing, or resetting unmanaged resources.
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Finalizes an instance of the <see cref="CsvWriter"/> class.
        /// </summary>
        /// 
        ~CsvWriter()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// 
        /// <param name="disposing"><c>true</c> to release both managed and
        ///   unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (Writer != null)
                {
                    Writer.Dispose();
                    Writer = null;
                }
            }
        }

    }
}
