// Accord Statistics Library
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

namespace Accord.Statistics.Filters
{
    using System;
    using System.Data;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    ///   Base abstract class for the Data Table preprocessing filters.
    /// </summary>
    /// <typeparam name="T">The column options type.</typeparam>
    /// 
    [Serializable]
    public abstract class BaseFilter<T> : IFilter where T : ColumnOptionsBase
    {
        /// <summary>
        ///   Gets or sets whether this filter is active. An inactive
        ///   filter will repass the input table as output unchanged.
        /// </summary>
        /// 
        [Description("Gets or sets whether this filter is active.")]
        public bool Active { get; set; }

        /// <summary>
        ///   Gets the collection of filter options.
        /// </summary>
        /// 
        [Description("Gets or sets processing options for the columns in the source DataTables")]
        public ColumnOptionCollection<T> Columns { get; private set; }

        /// <summary>
        ///   Creates a new DataTable Filter Base.
        /// </summary>
        /// 
        protected BaseFilter()
        {
            this.Columns = new ColumnOptionCollection<T>();
            this.Active = true;
        }

        /// <summary>
        ///   Applies the Filter to a <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// 
        /// <param name="data">The source <see cref="System.Data.DataTable"/>.</param>
        /// 
        /// <returns>The processed <see cref="System.Data.DataTable"/>.</returns>
        /// 
        public DataTable Apply(DataTable data)
        {
            // Initial argument checking
            if (data == null)
                throw new ArgumentNullException("data");

            if (Active)
            {
                return ProcessFilter(data);
            }

            return data;
        }


        /// <summary>
        ///   Processes the current filter.
        /// </summary>
        /// 
        protected abstract DataTable ProcessFilter(DataTable data);


        /// <summary>
        ///   Gets options associated with a given variable (data column).
        /// </summary>
        /// 
        /// <param name="columnName">The name of the variable.</param>
        /// 
        public T this[string columnName]
        {
            get { return Columns[columnName]; }
        }

        /// <summary>
        ///   Gets options associated with a given variable (data column).
        /// </summary>
        /// 
        /// <param name="index">The column's index for the variable.</param>
        /// 
        public T this[int index]
        {
            get { return Columns[index]; }
        }
    }

    /// <summary>
    ///   Column options for filter which have per-column settings.
    /// </summary>
    /// 
    [Serializable]
    public abstract class ColumnOptionsBase
    {
        /// <summary>
        ///   Gets or sets the name of the column that the options will apply to.
        /// </summary>
        /// 
        public String ColumnName { get; set; }

        /// <summary>
        ///   Constructs the base class for Column Options.
        /// </summary>
        /// 
        /// <param name="column">Column's name.</param>
        /// 
        protected ColumnOptionsBase(string column)
        {
            this.ColumnName = column;
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            return this.ColumnName;
        }
    }

    /// <summary>
    ///   Column option collection.
    /// </summary>
    /// 
    [Serializable]
    public class ColumnOptionCollection<T> : KeyedCollection<String, T>
        where T : ColumnOptionsBase
    {
        /// <summary>
        ///   Extracts the key from the specified element.
        /// </summary>
        /// 
        protected override string GetKeyForItem(T item)
        {
            return item.ColumnName;
        }
    }
}
