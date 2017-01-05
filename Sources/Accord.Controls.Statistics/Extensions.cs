// Accord Statistics Controls Library
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
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    ///   Extension methods for Windows Forms' controls.
    /// </summary>
    /// 
    public static class Extensions
    {

        /// <summary>
        ///   Enables the display of recursively nested properties 
        ///   in the Windows.Forms' DataGridView control.
        /// </summary>
        /// 
        /// <param name="dataGridView">The <see cref="DataGridView"/> to enable nested properties.</param>
        /// <param name="value">True to use nested properties, false otherwise.</param>
        /// 
        /// <remarks>
        ///   This method will register a custom cell formatting event in the DataGridView and
        ///   retrieve any nested property specified in the column's DataPropertyName property
        ///   using reflection. This method is based on th idea by Antonio Bello, originally 
        ///   shared in:
        ///   
        ///    http://www.developer-corner.com/2007/07/datagridview-how-to-bind-nested-objects_18.html
        ///    
        /// </remarks>
        /// 
        public static void AllowNestedProperties(this DataGridView dataGridView, bool value)
        {
            if (value)
                dataGridView.CellFormatting += dataGridView1_CellFormatting;
            else
                dataGridView.CellFormatting -= dataGridView1_CellFormatting;
        }

        private static void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // http://www.developer-corner.com/2007/07/datagridview-how-to-bind-nested-objects_18.html

            DataGridView dataGridView1 = sender as DataGridView;

            var row = dataGridView1.Rows[e.RowIndex];
            var col = dataGridView1.Columns[e.ColumnIndex];

            if ((row.DataBoundItem != null) && (col.DataPropertyName.Contains(".")))
            {
                e.Value = BindProperty(row.DataBoundItem, col.DataPropertyName,
                    col.DefaultCellStyle.Format, col.DefaultCellStyle.FormatProvider);
            }
        }

        private static string BindProperty(object property, string propertyName, string format, IFormatProvider provider)
        {
            string retValue = String.Empty;

            if (propertyName.Contains("."))
            {
                string leftPropertyName = propertyName.Substring(0, propertyName.IndexOf(".", StringComparison.Ordinal));
                PropertyInfo[] arrayProperties = property.GetType().GetProperties();

                foreach (PropertyInfo propertyInfo in arrayProperties)
                {
                    if (propertyInfo.Name == leftPropertyName)
                    {
                        retValue = BindProperty(propertyInfo.GetValue(property, null),
                            propertyName.Substring(propertyName.IndexOf(".", StringComparison.Ordinal) + 1),
                            format, provider);
                        break;
                    }
                }
            }
            else
            {
                Type propertyType;
                PropertyInfo propertyInfo;
                propertyType = property.GetType();
                propertyInfo = propertyType.GetProperty(propertyName);
                var obj = propertyInfo.GetValue(property, null);

                IFormattable fmt = obj as IFormattable;
                if (fmt != null)
                    retValue = fmt.ToString(format, provider);
                else
                    retValue = obj.ToString();
            }

            return retValue;
        }
    }
}
