// Accord Formats Library
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

namespace Accord.IO
{
#if !NETSTANDARD
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Globalization;
    using System.IO;
    using Accord.Compat;

    /// <summary>
    ///   Excel file reader using Microsoft Jet Database Engine.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class requires the Microsoft Access Database Engine
    ///   to work. The download is available from Microsoft under
    ///   the name "Microsoft Access Database Engine 2010 Redistributable",
    ///   available in both 32-bit (x86) and 64-bit (x64) versions.</para>
    ///   
    /// <para>
    ///   By default, the redistributable package will only install 
    ///   if it is the same as the current version of Microsoft Office
    ///   installed in the machine (i.e. ACE 32-bit can not be installed
    ///   with 64-bit office and vice-versa). To overcome this limitation
    ///   and install both versions of the ACE drivers, specify /passive
    ///   as a command line argument when installing the packages.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Create a new reader, opening a given path
    /// ExcelReader reader = new ExcelReader(path);
    /// 
    /// // Afterwards, we can query the file for all
    /// // worksheets within the specified workbook:
    /// string[] sheets = reader.GetWorksheetList();
    /// 
    /// // Finally, we can request an specific sheet:
    /// DataTable table = reader.GetWorksheet(sheets[1]);
    /// 
    /// // Now, we have loaded the Excel file into a DataTable. We
    /// // can go further and transform it into a matrix to start
    /// // running other algorithms on it: 
    /// 
    /// double[,] matrix = table.ToMatrix();
    /// 
    /// // We can also do it retrieving the name for each column:
    /// string[] columnNames; matrix = table.ToMatrix(out columnNames);
    /// 
    /// // Or we can extract specific columns into single arrays:
    /// double[] column = table.Columns[0].ToArray();
    /// 
    /// // PS: you might need to import the Accord.Math namespace in
    /// //   order to be able to call the ToMatrix extension methods. 
    /// </code>
    /// </example>
    /// 
    public class ExcelReader
    {

        private string strConnection;

        private string[] worksheets;

        /// <summary>
        ///   Creates a new spreadsheet reader.
        /// </summary>
        /// 
        /// <param name="stream">The stream containing the spreadsheet file.</param>
        /// <param name="xlsx">True if the file should be treated as .xlsx file, false otherwise. Default is true.</param>
        /// 
        public ExcelReader(Stream stream, bool xlsx)
            : this(stream, xlsx, true)
        {
        }

        /// <summary>
        ///   Creates a new spreadsheet reader.
        /// </summary>
        /// 
        /// <param name="stream">The stream containing the spreadsheet file.</param>
        /// <param name="xlsx">True if the file should be treated as .xlsx file, false otherwise. Default is true.</param>
        /// <param name="hasHeaders">True if the spreadsheet contains headers, false otherwise. Default is true.</param>
        /// 
        public ExcelReader(Stream stream, bool xlsx, bool hasHeaders)
            : this(stream, xlsx, hasHeaders, true)
        {
        }

        /// <summary>
        ///   Creates a new spreadsheet reader.
        /// </summary>
        /// 
        /// <param name="stream">The stream containing the spreadsheet file.</param>
        /// <param name="xlsx">True if the file should be treated as .xlsx file, false otherwise. Default is true.</param>
        /// <param name="hasHeaders">True if the spreadsheet contains headers, false otherwise. Default is true.</param>
        /// <param name="hasMixedData">True to read "intermixed" data columns as text, false otherwise. Default is true.</param>
        /// 
        public ExcelReader(Stream stream, bool xlsx, bool hasHeaders, bool hasMixedData)
        {
            string tempFileName = Path.GetTempFileName();
            string withExtension = Path.ChangeExtension(tempFileName, xlsx ? ".xlsx" : ".xls");

            File.Move(tempFileName, withExtension);
            tempFileName = withExtension;

            using (FileStream file = new FileStream(tempFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                stream.CopyTo(file);
            }

            initialize(tempFileName, hasHeaders, hasMixedData);
        }

        /// <summary>
        ///   Creates a new spreadsheet reader.
        /// </summary>
        /// 
        /// <param name="path">The path of for the spreadsheet file.</param>
        /// 
        public ExcelReader(string path)
            : this(path, true, true)
        {
        }

        /// <summary>
        ///   Creates a new spreadsheet reader.
        /// </summary>
        /// 
        /// <param name="path">The path of for the spreadsheet file.</param>
        /// <param name="hasHeaders">True if the spreadsheet contains headers, false otherwise. Default is true.</param>
        /// 
        public ExcelReader(string path, bool hasHeaders)
            : this(path, hasHeaders, true)
        {
        }

        /// <summary>
        ///   Creates a new spreadsheet reader.
        /// </summary>
        /// 
        /// <param name="path">The path of for the spreadsheet file.</param>
        /// <param name="hasHeaders">True if the spreadsheet contains headers, false otherwise. Default is true.</param>
        /// <param name="hasMixedData">True to read "intermixed" data columns as text, false otherwise. Default is true.</param>
        /// 
        public ExcelReader(string path, bool hasHeaders, bool hasMixedData)
        {
            initialize(path, hasHeaders, hasMixedData);
        }

        private void initialize(string path, bool hasHeaders, bool hasMixedData)
        {
            string fullPath = Path.GetFullPath(path);
            string extension = Path.GetExtension(path);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException(String.Format("File could not be found: {0}", fullPath), fullPath);

            string tempFileName = Path.GetTempFileName();
            File.Copy(fullPath, tempFileName, true);

            // Reader Settings
            HasHeaders = hasHeaders;
            HasMixedData = hasMixedData;

            switch (extension)
            {
                case ".xls": Version = "Excel 8.0"; break; // Excel 95-2003
                case ".xlsx": Version = "Excel 12.0"; break; // Excel 2007+
                default: throw new ArgumentException("File type could not be determined by file extension.", "path");
            }

            if (IntPtr.Size == 4 && extension == ".xls")
                Provider = "Microsoft.Jet.OLEDB.4.0";   // for x86/95-2003
            else Provider = "Microsoft.ACE.OLEDB.12.0"; // for x64/95-2007+

            var strBuilder = new DbConnectionStringBuilder();

            strBuilder.Add("Provider", Provider);
            strBuilder.Add("Data Source", tempFileName);
            strBuilder.Add("Extended Properties", Version + ";" +
                "HDR=" + (HasHeaders ? "Yes" : "No") + ';' +
                "Imex=" + (HasMixedData ? "2" : "0") + ';');
            strConnection = strBuilder.ToString();
        }

        /// <summary>
        ///   Gets the data provider used by the reader.
        /// </summary>
        /// 
        public string Provider { get; private set; }

        /// <summary>
        ///   Gets the Excel version used by the reader.
        /// </summary>
        /// 
        public String Version { get; private set; }

        /// <summary>
        ///   Gets whether the workbook has column headers.
        /// </summary>
        /// 
        public bool HasHeaders { get; private set; }

        /// <summary>
        ///   Gets whether the data contains mixed string and numeric data.
        /// </summary>
        /// 
        public bool HasMixedData { get; private set; }

        /// <summary>
        ///   Gets the names of the distinct sheets
        ///   that are contained in the Excel file.
        /// </summary>
        /// 
        public string[] WorksheetNames
        {
            get
            {
                if (worksheets == null)
                    GetWorksheetList();
                return worksheets;
            }
        }

        /// <summary>
        ///   Gets the list of worksheets in the spreadsheet.
        /// </summary>
        /// 
        public string[] GetWorksheetList()
        {
            var set = new HashSet<string>();

            using (var connection = new OleDbConnection(strConnection))
            {
                connection.Open();

                var table = connection.GetSchema("Tables");

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string name = (string)table.Rows[i]["TABLE_NAME"];

                    // removes the trailing $ and other characters appended in the table name
                    while (name.EndsWith("$", StringComparison.Ordinal)
                        || name.EndsWith("$\"", StringComparison.Ordinal)
                        || name.EndsWith("$\'", StringComparison.Ordinal)
                        || name.EndsWith("$\"\'", StringComparison.Ordinal)
                        || name.EndsWith("$\'\"", StringComparison.Ordinal))
                        name = name.Remove(name.Length - 1).Trim('"', '\'');

                    set.Add(name);
                }
            }

            this.worksheets = new List<string>(set).ToArray();

            return worksheets;
        }

        /// <summary>
        ///   Gets the list of columns in a worksheet.
        /// </summary>
        /// 
        public string[] GetColumnsList(string worksheet)
        {
            string[] columns;

            OleDbConnection connection = new OleDbConnection(strConnection);
            connection.Open();
            DataTable tableColumns = connection.GetSchema("Columns", new string[] { null, null, worksheet + '$', null });
            connection.Close();

            columns = new string[tableColumns.Rows.Count];

            for (int i = 0; i < columns.Length; i++)
                columns[i] = (string)tableColumns.Rows[i]["COLUMN_NAME"];

            return columns;
        }

        /// <summary>
        ///   Gets an worksheet as a data table.
        /// </summary>
        /// 
        public DataTable GetWorksheet(string worksheet)
        {
            DataTable ws;

            using (OleDbConnection connection = new OleDbConnection(strConnection))
            {
                OleDbCommand command = new OleDbCommand("SELECT * FROM [" + worksheet + "$]", connection);

                using (OleDbDataAdapter adaptor = new OleDbDataAdapter(command))
                {
                    ws = new DataTable(worksheet);
                    ws.Locale = CultureInfo.InvariantCulture;
                    adaptor.FillSchema(ws, SchemaType.Source);
                    adaptor.Fill(ws);
                }
            }

            return ws;
        }

        /// <summary>
        ///   Gets an worksheet as a data table.
        /// </summary>
        /// 
        public DataTable GetWorksheet(int worksheetIndex)
        {
            string name = WorksheetNames[worksheetIndex];

            return GetWorksheet(name);
        }

        /// <summary>
        ///   Gets the entire worksheet as a data set.
        /// </summary>
        /// 
        public DataSet GetWorksheet()
        {
            DataSet dataset = new DataSet("Excel Workbook");
            dataset.Locale = CultureInfo.InvariantCulture;

            foreach (string sheet in WorksheetNames)
            {
                DataTable table = GetWorksheet(sheet);
                dataset.Tables.Add(table);
            }

            return dataset;
        }
    }
#endif
}
