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
    using System.Diagnostics;
    using System.ComponentModel;
    using Accord.Statistics.Analysis;

    /// <summary>
    ///   Provides an abstraction of the confusion matrix values.
    /// </summary>
    /// 
    public class ConfusionMatrixPropertyDescriptor : PropertyDescriptor
    {

        /// <summary>
        ///   Gets the index of the column being represented.
        /// </summary>
        /// 
        public int ColumnIndex { get; private set; }


        private String name;


        /// <summary>
        ///   Initializes a new instance of the <see cref="ConfusionMatrixPropertyDescriptor"/> class.
        /// </summary>
        /// 
        /// <param name="name">The name for the column.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// 
        public ConfusionMatrixPropertyDescriptor(String name, int columnIndex)
            : base(name, null)
        {
            this.ColumnIndex = columnIndex;
            this.name = name;
        }

        /// <summary>
        ///   Returns the name of the array.
        /// </summary>
        /// 
        public override string DisplayName
        {
            get { return name; }
        }

        /// <summary>
        ///   Returns the type of ArrayRowView.
        /// </summary>
        /// 
        public override Type ComponentType
        {
            get { return typeof(ConfusionMatrixRowView); }
        }

        /// <summary>
        ///   Returns true.
        /// </summary>
        /// 
        public override bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        ///   Returns System.Double.
        /// </summary>
        /// 
        public override Type PropertyType
        {
            get { return typeof(double); }
        }

        /// <summary>
        ///   Gets a value from the array.
        /// </summary>
        /// 
        public override object GetValue(object component)
        {
            try
            {
                ConfusionMatrixRowView rowView = component as ConfusionMatrixRowView;
                return rowView.GetValue(ColumnIndex);
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine(e);
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override void SetValue(object component, object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Returns false.
        /// </summary>
        /// 
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        public override void ResetValue(object component)
        {
        }

        /// <summary>
        ///   Returns false.
        /// </summary>
        /// 
        /// <param name="component"></param>
        /// 
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
