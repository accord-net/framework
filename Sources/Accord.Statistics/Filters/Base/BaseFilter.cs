// Accord Statistics Library
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

namespace Accord.Statistics.Filters
{
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Collections;
    using Accord.Compat;
    using System.Threading;
    using System.Data;

    /// <summary>
    ///   Base abstract class for the Data Table preprocessing filters.
    /// </summary>
    /// 
    /// <typeparam name="TOptions">The column options type.</typeparam>
    /// <typeparam name="TFilter">The filter type to whom these options should belong to.</typeparam>
    /// 
    [Serializable]
    public abstract class BaseFilter<TOptions, TFilter> : IFilter, IEnumerable<TOptions>
        where TOptions : ColumnOptionsBase<TFilter>, new()
        where TFilter : BaseFilter<TOptions, TFilter>
    {
        [NonSerialized]
        private CancellationToken token;

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
        public ColumnOptionCollection<TOptions, TFilter> Columns { get; private set; }

        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        /// 
        /// <value>The token.</value>
        /// 
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }

        /// <summary>
        ///   Creates a new DataTable Filter Base.
        /// </summary>
        /// 
        protected BaseFilter()
        {
            this.Columns = new ColumnOptionCollection<TOptions, TFilter>();
            this.Columns.AddingNew += Columns_AddingNew;
            this.Active = true;
        }

#if !NETSTANDARD1_4
        /// <summary>
        ///   Applies the Filter to a <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// 
        /// <param name="data">The source <see cref="System.Data.DataTable"/>.</param>
        /// <param name="columnNames">The name of the columns that should be processed.</param>
        /// 
        /// <returns>The processed <see cref="System.Data.DataTable"/>.</returns>
        /// 
        public DataTable Apply(DataTable data, params string[] columnNames)
        {
            // Initial argument checking
            if (data == null)
                throw new ArgumentNullException("data");

            if (Active)
            {
                return ProcessFilter(data.DefaultView.ToTable(false, columnNames));
            }

            return data;
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
#endif

        /// <summary>
        ///   Gets options associated with a given variable (data column).
        /// </summary>
        /// 
        /// <param name="columnName">The name of the variable.</param>
        /// 
        public TOptions this[string columnName]
        {
            get
            {
                if (!Columns.Contains(columnName))
                {
                    this.Columns.Add(new TOptions()
                    {
                        ColumnName = columnName,
                        Owner = (TFilter)this
                    });
                }

                return Columns[columnName];
            }
        }

        /// <summary>
        ///   Gets options associated with a given variable (data column).
        /// </summary>
        /// 
        /// <param name="index">The column's index for the variable.</param>
        /// 
        public TOptions this[int index]
        {
            get
            {
                for (int i = Columns.Count; i <= index; i++)
                {
                    this.Columns.Add(new TOptions()
                    {
                        ColumnName = i.ToString(),
                        Owner = (TFilter)this
                    });
                }

                return Columns[index];
            }
        }

        /// <summary>
        /// Gets the number of inputs accepted by the model.
        /// </summary>
        /// 
        /// <value>The number of inputs.</value>
        /// 
        public int NumberOfInputs
        {
            get { return this.Columns.Count; }
            set { throw new InvalidOperationException("This property is read-only."); }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<TOptions> GetEnumerator()
        {
            return Columns.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Columns.GetEnumerator();
        }

        /// <summary>
        ///   Add a new column options definition to the collection.
        /// </summary>
        /// 
        public void Add(TOptions options)
        {
            this.Columns.Add(options);
        }

        private void Columns_AddingNew(object sender, TOptions e)
        {
            OnAddingOptions(e);
        }

        /// <summary>
        ///   Called when a new column options definition is being added.
        ///   Can be used to validate or modify these options beforehand.
        /// </summary>
        /// 
        /// <param name="options">The column options being added.</param>
        /// 
        protected virtual void OnAddingOptions(TOptions options)
        {
        }
    }
}
