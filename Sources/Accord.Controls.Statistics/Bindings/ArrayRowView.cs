// Accord Control Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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

    /// <summary>
    ///   Represents a row from array view.
    /// </summary>
    /// 
    public class ArrayRowView : ICustomTypeDescriptor, IEditableObject, IDataErrorInfo
    {

        private ArrayDataView owner;
        private int rowIndex;
        private string error;


        /// <summary>
        ///   Initializes a new instance of the <see cref="ArrayRowView"/> class.
        /// </summary>
        internal ArrayRowView(ArrayDataView owner, int index)
        {
            this.owner = owner;
            this.rowIndex = index;
            this.error = String.Empty;
        }

        internal string GetName()
        {
            return owner.RowNames[rowIndex];
        }

        /// <summary>
        ///   Gets the value at the specified position of this row.
        /// </summary>
        /// <param name="index">The column index of the element to get.</param>
        internal object GetColumn(int index)
        {
            if (owner.ArrayData.Rank == 2)
            {
                return owner.ArrayData.GetValue(this.rowIndex, index);
            }
            else
            {
                return owner.ArrayData.GetValue(index);
            }
        }

        /// <summary>
        ///   Sets a value to the element at the specified position of this row.
        /// </summary>
        /// <param name="index">The index of the element to set.</param>
        /// <param name="value">The new value for the specified element.</param>
        internal void SetColumnValue(int index, object value)
        {
            try
            {
                if (owner.ArrayData.Rank == 2)
                {
                    owner.ArrayData.SetValue(value, this.rowIndex, index);
                }
                else
                {
                    owner.ArrayData.SetValue(value, index);
                }
            }
            catch (ArgumentException e)
            {
                error = e.ToString();
            }
            catch (InvalidCastException e)
            {
                error = e.ToString();
            }
            catch (IndexOutOfRangeException e)
            {
                error = e.ToString();
            }
        }

        #region ICustomTypeDescriptor Members

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
            return owner;
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
            if (owner.ArrayData.Rank == 2)
            {
                // Multidimensional array of rank 2
                int col = owner.ArrayData.GetLength(1);
                Type type = owner.ArrayData.GetType().GetElementType();
                PropertyDescriptor[] prop;

                if (owner.RowNames != null)
                {
                    prop = new PropertyDescriptor[col+1];
                    prop[0] = new RowNamePropertyDescriptor("Row");
                    for (int i = 0; i < col; i++)
                        prop[i+1] = new ArrayPropertyDescriptor(owner.ColumnNames[i], type, i);
                }
                else
                {
                    prop = new PropertyDescriptor[col];
                    for (int i = 0; i < col; i++)
                        prop[i] = new ArrayPropertyDescriptor(owner.ColumnNames[i], type, i);
                }
                return new PropertyDescriptorCollection(prop);
            }
            else
            {
                if (owner.ArrayType == ArrayDataType.Simple)
                {
                    int col = owner.ArrayData.GetLength(0);
                    Type type = owner.ArrayData.GetType().GetElementType();
                    PropertyDescriptor[] prop = new PropertyDescriptor[col];

                    for (int i = 0; i < prop.Length; i++)
                        prop[i] = new ArrayPropertyDescriptor(owner.ColumnNames[i], type, i);

                    return new PropertyDescriptorCollection(prop);
                }
                else
                {
                    Type type = owner.ArrayData.GetType().GetElementType();
                    PropertyDescriptor[] prop = new PropertyDescriptor[owner.ColumnsCount];

                    for (int i = 0; i < prop.Length; i++)
                        prop[i] = new ArrayPropertyDescriptor(owner.ColumnNames[i], type, i);

                    return new PropertyDescriptorCollection(prop);
                }
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

        #endregion

        #region IEditableObject Members

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public void EndEdit()
        {
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public void CancelEdit()
        {
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public void BeginEdit()
        {
        }

        #endregion

        #region IDataErrorInfo Members
        /// <summary>
        ///   Gets the error message for the property with the given name.
        /// </summary>
        public string this[string columnName]
        {
            get { return String.Empty; }
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <returns>
        /// An error message indicating what is wrong with this object. The default is an empty string ("").
        /// </returns>
        public string Error
        {
            get { return error; }
        }

        #endregion

    }
}
