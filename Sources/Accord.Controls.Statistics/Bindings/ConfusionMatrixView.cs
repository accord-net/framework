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
    using Accord.Statistics.Analysis;


    /// <summary>
    ///   Represents a data bondable, customized view of a
    ///   <see cref="GeneralConfusionMatrix">confusion matrix</see>.
    /// </summary>
    /// 
    public class ConfusionMatrixView : IBindingList
    {

        /// <summary>
        ///   Gets the Confusion Matrix being shown.
        /// </summary>
        /// 
        public GeneralConfusionMatrix Matrix { get; private set; }

        /// <summary>
        ///   Gets or sets whether the control should
        ///   display proportions instead of counts.
        /// </summary>
        /// 
        public bool Proportions { get; set; }

        /// <summary>
        ///   Gets the names for the columns in the matrix.
        /// </summary>
        /// 
        public String[] ColumnNames { get; private set; }

        /// <summary>
        ///   Gets the names for the rows in the matrix.
        /// </summary>
        /// 
        public String[] RowNames { get; private set; }


        private ConfusionMatrixRowView[] rows;

        /// <summary>
        ///   Occurs when the list changes or an item in the list changes.
        /// </summary>
        /// 
        public event ListChangedEventHandler ListChanged;


        /// <summary>
        ///   Initializes a new instance of the <see cref="ConfusionMatrixView"/> class.
        /// </summary>
        /// 
        /// <param name="matrix">The confusion matrix.</param>
        /// 
        public ConfusionMatrixView(ConfusionMatrix matrix)
        {
            GeneralConfusionMatrix m = matrix.ToGeneralMatrix();

            init(m);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConfusionMatrixView"/> class.
        /// </summary>
        /// 
        /// <param name="matrix">The confusion matrix.</param>
        /// 
        public ConfusionMatrixView(GeneralConfusionMatrix matrix)
        {
            init(matrix);
        }

        private void init(GeneralConfusionMatrix matrix)
        {
            this.Matrix = matrix;

            int classes = matrix.NumberOfClasses;

            rows = new ConfusionMatrixRowView[classes + 1];
            RowNames = new String[classes + 1];
            ColumnNames = new String[classes + 1];

            for (int i = 0; i < classes; i++)
            {
                rows[i] = new ConfusionMatrixRowView(this, i);
                RowNames[i] = "Expected " + i;
                ColumnNames[i] = "Actual " + i;
            }

            rows[classes] = new ConfusionMatrixRowView(this, -1);

            RowNames[classes] = "Total";
            ColumnNames[classes] = "Total";
        }



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
        ///   Returns false.
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
        ///   Returns true.
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
            rows.CopyTo(array, index);
        }

        /// <summary>
        ///   Multidimensional arrays do not support Array copying.
        /// </summary>
        /// 
        public void CopyTo(ConfusionMatrixRowView[] array, int index)
        {
            rows.CopyTo(array, index);
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
