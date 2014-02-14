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
    using System.Collections.Generic;
    using System.Data;
    using Accord.Math;

    /// <summary>
    ///   Branching filter.
    /// </summary>
    /// 
    [Serializable]
    public class Branching : BaseFilter<Branching.Options>
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="Branching"/> class.
        /// </summary>
        /// 
        public Branching()
            : base() { }


        /// <summary>
        ///   Initializes a new instance of the <see cref="Branching"/> class.
        /// </summary>
        /// 
        /// <param name="columns">The columns to use as filters.</param>
        /// 
        public Branching(params string[] columns)
        {
            foreach (String col in columns)
                Columns.Add(new Options(col));
        }

        /// <summary>
        ///   Processes the current filter.
        /// </summary>
        /// 
        protected override DataTable ProcessFilter(DataTable data)
        {
            // Copy the DataTable
            DataTable result = data.Clone();

            foreach (Options option in Columns)
            {
                foreach (int label in option.Filters.Keys)
                {
                    FiltersSequence filters = option.Filters[label];

                    // Get data subset
                    DataRow[] rows = data.Select("[" + option.ColumnName + "] = " + label);

                    DataTable branch = data.Clone();
                    foreach (DataRow row in rows)
                        branch.ImportRow(row);

                    DataTable branchResult = filters.Apply(branch);

                    foreach (DataRow row in branchResult.Rows)
                        result.ImportRow(row);
                }
            }

            return result;
        }


        /// <summary>
        ///   Column options for the branching filter.
        /// </summary>
        /// 
        [Serializable]
        public class Options : ColumnOptionsBase, IAutoConfigurableColumn
        {

            /// <summary>
            ///   Gets the collection of filters associated with a given label value.
            /// </summary>
            /// 
            public Dictionary<int, FiltersSequence> Filters { get; private set; }


            /// <summary>
            ///   Initializes a new instance of the <see cref="Options"/> class.
            /// </summary>
            /// 
            /// <param name="name">The column name.</param>
            /// 
            public Options(String name)
                : base(name)
            {
                Filters = new Dictionary<int, FiltersSequence>();
            }

            /// <summary>
            ///   Initializes a new instance of the <see cref="Options"/> class.
            /// </summary>
            /// 
            public Options()
                : this("New column") { }

            /// <summary>
            ///   Auto detects the column options by analyzing a given <see cref="System.Data.DataColumn"/>.
            /// </summary>
            /// 
            /// <param name="column">The column to analyze.</param>
            /// 
            public void Detect(DataColumn column)
            {
                int[] values = column.ToArray<int>().Distinct();

                foreach (int i in values)
                    Filters.Add(i, null);
            }

        }
    }
}
