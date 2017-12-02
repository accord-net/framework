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

#if !NETSTANDARD1_4
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
    /// <remarks>
    ///   The branching filter allows for different filter sequences to be
    ///   applied to different subsets of a data table. For instance, consider
    ///   a data table whose first column, "IsStudent", is an indicator variable:
    ///   a value of 1 indicates the row contains information about a student, and
    ///   a value of 0 indicates the row contains information about someone who is
    ///   not currently a student. Using the branching filter, it becomes possible
    ///   to apply a different set of filters for the rows that represent students
    ///   and different filters for rows that represent non-students.
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   Suppose we have the following data table. In this table, each row represents
    ///   a person, an indicator variable tell us whether this person is a smoker, and
    ///   the last column indicates the age of each person. Let's say we would like to
    ///   convert the age of smokers to a scale from -1 to 0, and the age of non-smokers
    ///   to a scale from 0 to 1.</para>
    ///   
    /// <code>
    /// object[,] data = 
    /// {
    ///     { "Id",  "IsSmoker", "Age" },
    ///     {   0,       1,        10  },
    ///     {   1,       1,        15  },
    ///     {   2,       0,        40  },
    ///     {   3,       1,        20  },
    ///     {   4,       0,        70  },
    ///     {   5,       0,        55  },
    /// };
    /// 
    /// // Create a DataTable from data
    /// DataTable input = data.ToTable();
    /// 
    /// // We will create two filters, one to operate on the smoking
    /// // branch of the data, and other in the non-smoking subjects.
    /// //
    /// var smoker = new LinearScaling();
    /// var common = new LinearScaling();
    /// 
    /// // for the smokers, we will convert the age to [-1; 0]
    /// smoker.Columns.Add(new LinearScaling.Options("Age")
    /// {
    ///     SourceRange = new DoubleRange(10, 20),
    ///     OutputRange = new DoubleRange(-1, 0)
    /// });
    /// 
    /// // for non-smokers, we will convert the age to [0; +1]
    /// common.Columns.Add(new LinearScaling.Options("Age")
    /// {
    ///     SourceRange = new DoubleRange(40, 70),
    ///     OutputRange = new DoubleRange(0, 1)
    /// });
    /// 
    /// // We now configure and create the branch filter
    /// var settings = new Branching.Options("IsSmoker");
    /// settings.Filters.Add(1, smoker);
    /// settings.Filters.Add(0, common);
    /// 
    /// Branching branching = new Branching(settings);
    /// 
    /// 
    /// // Finally, we can process the input data:
    /// DataTable actual = branching.Apply(input);
    /// 
    /// // As result, the generated table will
    /// // then contain the following entries:
    /// 
    /// //  { "Id",  "IsSmoker", "Age" },
    /// //  {   0,       1,      -1.0  },
    /// //  {   1,       1,      -0.5  },
    /// //  {   2,       0,       0.0  },
    /// //  {   3,       1,       0.0  },
    /// //  {   4,       0,       1.0  },
    /// //  {   5,       0,       0.5  },
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class Branching : BaseFilter<Branching.Options, Branching>
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
        ///   Initializes a new instance of the <see cref="Branching"/> class.
        /// </summary>
        /// 
        /// <param name="columns">The columns to use as filters.</param>
        /// 
        public Branching(params Options[] columns)
        {
            foreach (Options col in columns)
                Columns.Add(col);
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
                var name = option.ColumnName;

                if (!data.Columns.Contains(name))
                    continue;

                foreach (int label in option.Filters.Keys)
                {
                    var filter = option.Filters[label];

                    // Get data subset
                    DataRow[] rows = data.Select("[" + option.ColumnName + "] = " + label);

                    DataTable branch = data.Clone();
                    foreach (DataRow row in rows)
                        branch.ImportRow(row);

                    DataTable branchResult = filter.Apply(branch);

                    if (result.Rows.Count == 0)
                    {
                        foreach (DataColumn col in branchResult.Columns)
                        {
                            if (result.Columns.Contains(col.ColumnName))
                                result.Columns[col.ColumnName].DataType = col.DataType;
                        }
                    }

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
        public class Options : ColumnOptionsBase<Branching>, IAutoConfigurableColumn
        {

            /// <summary>
            ///   Gets the collection of filters associated with a given label value.
            /// </summary>
            /// 
            public Dictionary<int, IFilter> Filters { get; private set; }


            /// <summary>
            ///   Initializes a new instance of the <see cref="Options"/> class.
            /// </summary>
            /// 
            /// <param name="name">The column name.</param>
            /// 
            public Options(String name)
                : base(name)
            {
                Filters = new Dictionary<int, IFilter>();
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
#endif