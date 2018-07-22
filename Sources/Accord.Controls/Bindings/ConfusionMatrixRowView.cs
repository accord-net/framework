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

namespace Accord.Controls
{
    using System;
    using System.ComponentModel;
    using Accord.Statistics.Analysis;

    /// <summary>
    ///   Represents a row from a <see cref="ConfusionMatrixView"/>.
    /// </summary>
    /// 
    public class ConfusionMatrixRowView : ICustomTypeDescriptor
    {

        /// <summary>
        ///   Gets the owner of this row.
        /// </summary>
        /// 
        public ConfusionMatrixView Owner { get; private set; }

        /// <summary>
        ///   Gets the index for this row.
        /// </summary>
        /// 
        public int RowIndex { get; private set; }


        internal ConfusionMatrixRowView(ConfusionMatrixView owner, int rowIndex)
        {
            this.Owner = owner;
            this.RowIndex = rowIndex;
        }

        /// <summary>
        ///   Gets the row's header.
        /// </summary>
        /// 
        public string Header
        {
            get
            {
                if (RowIndex == -1) 
                    return Owner.RowNames[Owner.Matrix.NumberOfClasses];
                return Owner.RowNames[RowIndex];
            }
        }

        /// <summary>
        ///   Returns null.
        /// </summary>
        /// 
        public TypeConverter GetConverter()
        {
            return null;
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return EventDescriptorCollection.Empty;
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return EventDescriptorCollection.Empty;
        }

        /// <summary>
        ///   Returns null.
        /// </summary>
        /// 
        public string GetComponentName()
        {
            return null;
        }

        /// <summary>
        ///   Gets the owner ArrayDataView.
        /// </summary>
        /// 
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return Owner;
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public AttributeCollection GetAttributes()
        {
            return AttributeCollection.Empty;
        }

        /// <summary>
        ///   Gets the values of the multidimensional array as properties.
        /// </summary>
        /// 
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            GeneralConfusionMatrix matrix = Owner.Matrix;
            int classes = matrix.NumberOfClasses;

            PropertyDescriptor[] columns = new PropertyDescriptor[classes + 1];

            for (int i = 0; i < classes; i++)
                columns[i] = new ConfusionMatrixPropertyDescriptor(Owner.ColumnNames[i], i);
            columns[classes] = new ConfusionMatrixPropertyDescriptor(Owner.ColumnNames[classes], -1);

            return new PropertyDescriptorCollection(columns);
        }

        /// <summary>
        ///   Gets the value for a given element in this row.
        /// </summary>
        /// 
        /// <param name="columnIndex">The column index of an element.</param>
        /// 
        /// <returns>The element at this row located at position <paramref name="columnIndex"/>.</returns>
        /// 
        public double GetValue(int columnIndex)
        {
            if (RowIndex == -1 && columnIndex == -1)
            {
                if (Owner.Proportions)
                    return 1;
                else return Owner.Matrix.NumberOfSamples;
            }

            if (RowIndex == -1)
            {
                if (Owner.Proportions)
                    return Owner.Matrix.ColumnProportions[columnIndex];
                else return Owner.Matrix.ColumnTotals[columnIndex];
            }
            else if (columnIndex == -1)
            {
                if (Owner.Proportions)
                    return Owner.Matrix.RowProportions[RowIndex];
                else return Owner.Matrix.RowTotals[RowIndex];
            }
            else
            {
                if (Owner.Proportions)
                    return Owner.Matrix.ProportionMatrix[RowIndex, columnIndex];
                else return Owner.Matrix.Matrix[RowIndex, columnIndex];
            }
        }

        PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return GetProperties(null);
        }

        /// <summary>
        ///   Returns null.
        /// </summary>
        public object GetEditor(Type editorBaseType)
        {
            return null;
        }

        /// <summary>
        ///   Returns null.
        /// </summary>
        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        /// <summary>
        ///   Returns null.
        /// </summary>
        public EventDescriptor GetDefaultEvent()
        {
            return null;
        }

        /// <summary>
        ///   Gets the name of this class.
        /// </summary>
        public string GetClassName()
        {
            return this.GetType().Name;
        }




    }
}
