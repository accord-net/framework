// Accord Control Library
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
//
// Copyright © Mihail Stefanov, 2004
//   Adapted from original code from Mihail Stefanov <http://www.mommosoft.com>
//   Available in: http://www.codeproject.com/KB/database/BindArrayGrid.aspx
//   Distributed under the LGPL with permission of the original author.
//

namespace Accord.Controls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;


    /// <summary>
    ///   Array data type (i.e. jagged or multidimensional).
    /// </summary>
    /// 
    public enum ArrayDataType
    {
        /// <summary>
        ///   Simple array type (i.e. double[]).
        /// </summary>
        /// 
        Simple,

        /// <summary>
        ///  Jagged array type (i.e. double[][]).
        /// </summary>
        /// 
        Jagged,

        /// <summary>
        ///  Multidimensional array type (i.e. double[,])
        /// </summary>
        /// 
        Multidimensional
    }

    /// <summary>
    /// Represents a data bondable, customized view of two dimensional array
    /// </summary>
    /// 
    public class ArrayDataView : IBindingList
    {

        private Array data;

        private ArrayDataType type;

        private ArrayRowView[] rows;

        private int rowCount;
        private int colCount;

        private string[] columnNames;
        private string[] rowNames;


        /// <summary> Raised when the list changes. </summary>
        public event System.ComponentModel.ListChangedEventHandler ListChanged;


        #region Constructors

        /// <summary>
        ///   Initializes a new ArrayDataView from array.
        /// </summary>
        /// 
        /// <param name="array">array of data.</param>
        /// 
        public ArrayDataView(Array array)
        {
            if (array.Rank > 2)
                throw new ArgumentException("Supports only up to two dimensional arrays", "array");

            this.data = array;

            if (array.Rank == 2)
            {
                rowCount = array.GetLength(0);
                colCount = array.GetLength(1);
                type = ArrayDataType.Multidimensional;
            }
            else
            {
                if (array.GetValue(0) is Array)
                {
                    Array row = array.GetValue(0) as Array;

                    rowCount = array.GetLength(0);
                    colCount = row.GetLength(0);
                    type = ArrayDataType.Jagged;
                }
                else
                {
                    rowCount = 1;
                    colCount = array.GetLength(0);
                    type = ArrayDataType.Simple;
                }
            }

            rows = new ArrayRowView[rowCount];
            for (int i = 0; i < rows.Length; i++)
                rows[i] = new ArrayRowView(this, i);
        }

        /// <summary>
        ///   Initializes a new ArrayDataView from array with custom column names.
        /// </summary>
        /// 
        /// <param name="array">array of data.</param>
        /// <param name="columnNames">collection of column names.</param>
        /// 
        public ArrayDataView(Array array, string[] columnNames)
            : this(array)
        {
            if (columnNames != null)
            {
                if (columnNames.Length != colCount)
                    throw new ArgumentException("Column names must correspond to array columns.", "columnNames");

                this.columnNames = columnNames;
            }
        }

        /// <summary>
        ///   Initializes a new ArrayDataView from array with custom column names.
        /// </summary>
        /// 
        /// <param name="array">array of data.</param>
        /// <param name="columnNames">collection of column names.</param>
        /// 
        public ArrayDataView(Array array, object[] columnNames)
            : this(array)
        {
            if (columnNames != null)
            {
                if (columnNames.Length != colCount)
                    throw new ArgumentException("Column names must correspond to array columns.", "columnNames");

                this.columnNames = new string[columnNames.Length];
                for (int i = 0; i < columnNames.Length; i++)
                    this.columnNames[i] = columnNames[i].ToString();
            }
        }

        /// <summary>
        ///   Initializes a new ArrayDataView from array with custom column names.
        /// </summary>
        /// 
        /// <param name="array">Array of data.</param>
        /// <param name="columnNames">Collection of column names.</param>
        /// <param name="rowNames">Collection of row names.</param>
        /// 
        public ArrayDataView(Array array, string[] columnNames, string[] rowNames)
            : this(array, columnNames)
        {
            if (rowNames != null)
            {
                if (rowNames.Length != rowCount)
                    throw new ArgumentException("Row names must correspond to array rows.", "rowNames");
                this.rowNames = rowNames;
            }
        }

        /// <summary>
        ///   Initializes a new ArrayDataView from array with custom column names.
        /// </summary>
        /// 
        /// <param name="array">Array of data.</param>
        /// <param name="columnNames">Collection of column names.</param>
        /// <param name="rowNames">Collection of row names.</param>
        /// 
        public ArrayDataView(Array array, object[] columnNames, object[] rowNames)
            : this(array, columnNames)
        {
            if (rowNames != null)
            {
                if (rowNames.Length != rowCount)
                    throw new ArgumentException("Row names must correspond to array rows.", "rowNames");

                this.rowNames = new string[rowNames.Length];
                for (int i = 0; i < rowNames.Length; i++)
                    this.rowNames[i] = rowNames[i].ToString();
            }
        }


        #endregion // Constructors


        #region Properties

        /// <summary>
        ///   Gets the column names for the array currently bound.
        /// </summary>
        /// 
        public string[] ColumnNames
        {
            get
            {
                if (columnNames == null)
                {
                    columnNames = new string[colCount];
                    for (int i = 0; i < columnNames.Length; i++)
                        columnNames[i] = i.ToString(CultureInfo.CurrentCulture);
                }

                return columnNames;
            }
        }

        /// <summary>
        ///   Gets the row names for the array currently bound.
        /// </summary>
        /// 
        public string[] RowNames
        {
            get { return rowNames; }
        }

        /// <summary>
        ///   Gets or sets the array currently bound.
        /// </summary>
        /// 
        public Array ArrayData
        {
            get { return data; }
        }

        /// <summary>
        ///   Gets the type of the array currently bound.
        /// </summary>
        /// 
        public ArrayDataType ArrayType
        {
            get { return type; }
        }

        /// <summary>
        ///   Gets the number of rows in the data-bound array.
        /// </summary>
        /// 
        public int RowCount
        {
            get { return rowCount; }
        }

        /// <summary>
        ///   Gets the number of columns in the data-bound array.
        /// </summary>
        /// 
        public int ColumnsCount
        {
            get { return colCount; }
        }
        #endregion


        /// <summary>
        ///   Resets the data binding.
        /// </summary>
        /// 
        public void Reset()
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public void AddIndex(PropertyDescriptor property)
        {
        }

        /// <summary>
        ///   Arrays do not allow for member insertion.
        /// </summary>
        /// 
        public bool AllowNew
        {
            get { return false; }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public void ApplySort(PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
        {
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public PropertyDescriptor SortProperty
        {
            get { return null; }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public int Find(PropertyDescriptor property, object key)
        {
            return 0;
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public bool SupportsSorting
        {
            get { return false; }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public bool IsSorted
        {
            get { return false; }
        }

        /// <summary>
        ///   Arrays do not allow member removal.
        /// </summary>
        /// 
        public bool AllowRemove
        {
            get { return false; }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public bool SupportsSearching
        {
            get { return false; }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public ListSortDirection SortDirection
        {
            get { return new System.ComponentModel.ListSortDirection(); }
        }



        private void OnListChanged(ListChangedEventArgs e)
        {
            if (ListChanged != null)
            {
                ListChanged(this, e);
            }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public bool SupportsChangeNotification
        {
            get { return true; }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public void RemoveSort()
        {
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public object AddNew()
        {
            return null;
        }

        /// <summary>
        ///   Gets whether this view allows editing. Always true.
        /// </summary>
        /// 
        public bool AllowEdit
        {
            get { return true; }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public void RemoveIndex(PropertyDescriptor property)
        {
        }


        /// <summary>
        ///   This view is read only.
        /// </summary>
        /// 
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        ///   Gets a row from this view.
        /// </summary>
        /// 
        public object this[int index]
        {
            get { return rows[index]; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public void RemoveAt(int index)
        {
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public void Insert(int index, object value)
        {
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public void Remove(object value)
        {
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public bool Contains(object value)
        {
            return false;
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public void Clear()
        {
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public int IndexOf(object value)
        {
            return 0;
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public int Add(object value)
        {
            return 0;
        }

        /// <summary>
        ///   Arrays are always fixed size.
        /// </summary>
        /// 
        public bool IsFixedSize
        {
            get { return true; }
        }



        /// <summary>
        ///   Returns false.
        /// </summary>
        /// 
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        ///   Gets the length of the array.
        /// </summary>
        /// 
        public int Count
        {
            get { return rows.Length; }
        }

        /// <summary>
        ///   Multidimensional arrays do not support Array copying.
        /// </summary>
        /// 
        public void CopyTo(Array array, int index)
        {
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public object SyncRoot
        {
            get { return null; }
        }


        /// <summary>
        ///   Gets the array enumerator.
        /// </summary>
        /// 
        public System.Collections.IEnumerator GetEnumerator()
        {
            return rows.GetEnumerator();
        }

    }

}
