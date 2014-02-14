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
    using System.Runtime.Serialization;

    /// <summary>
    ///   Grouping filter.
    /// </summary>
    /// 
    [Serializable]
    public class Grouping : BaseFilter<Grouping.Options>
    {

        [OptionalField]
        private bool lockGroups;

        [OptionalField]
        private int[] groupIndices;


        /// <summary>
        ///   Gets or sets a value indicating whether the group labels
        ///   are locked and should not be randomly re-selected.
        /// </summary>
        /// 
        /// <value><c>true</c> to lock groups; otherwise, <c>false</c>.</value>
        /// 
        public bool Lock
        {
            get { return lockGroups; }
            set { lockGroups = value; }
        }


        /// <summary>
        ///   Gets or sets the group index labels.
        /// </summary>
        /// 
        /// <value>The group indices.</value>
        /// 
        public int[] GroupIndices
        {
            get { return groupIndices; }
            set { groupIndices = value; }
        }

        /// <summary>
        ///   Gets or sets the two-group proportions.
        /// </summary>
        /// 
        public double Proportion { get; set; }

        /// <summary>
        ///   Gets or sets the name of the indicator
        ///   column which will be used to distinguish
        ///   samples from either group.
        /// </summary>
        /// 
        public string GroupIndicatorColumnName { get; set; }

        /// <summary>
        ///   Creates a new Grouping filter with equal group
        ///   proportions and default Group indicator column.
        /// </summary>
        /// 
        public Grouping()
        {
            Proportion = 0.5;
            GroupIndicatorColumnName = "Group";
        }

        /// <summary>
        ///   Creates a new Grouping filter.
        /// </summary>
        /// 
        public Grouping(string column)
        {
            Columns.Add(new Options(column));
        }


        /// <summary>
        ///   Processes the current filter.
        /// </summary>
        /// 
        protected override DataTable ProcessFilter(DataTable data)
        {

            if (!lockGroups)
            {
                // Check if we should balance label proportions
                if (Columns.Count == 0)
                {
                    // No. Just generate assign groups at random
                    groupIndices = Accord.Statistics.Tools.RandomGroups(data.Rows.Count, Proportion);
                }

                else
                {
                    // Yes, we must balance the occurrences in a data column
                    groupIndices = balancedGroups(data);
                }
            }

            return apply(data);
        }

        private int[] balancedGroups(DataTable data)
        {
            // Works with only one column and for the binary case
            // TODO: Expand to multiple columns and multi-classes

            int[] classes = Columns[0].Classes;
            string column = Columns[0].ColumnName;
            int groupCount = 2;

            // Get subsets with 0 and 1
            List<DataRow>[] subsets = new List<DataRow>[classes.Length];
            for (int i = 0; i < subsets.Length; i++)
                subsets[i] = new List<DataRow>(data.Select("[" + column + "] = " + classes[i]));

            List<DataRow>[] groups = new List<DataRow>[groupCount];
            for (int i = 0; i < groups.Length; i++)
                groups[i] = new List<DataRow>();

            int totalPositives = subsets[0].Count;
            int totalNegatives = subsets[1].Count;


            int firstGroupPositives = (int)((subsets[0].Count / (double)groupCount) * 2 * Proportion);
            int firstGroupNegatives = (int)((subsets[1].Count / (double)groupCount) * 2 * Proportion);


            int[] groupIndices = new int[data.Rows.Count];

            // Put positives and negatives into first group
            for (int j = 0; j < firstGroupPositives; j++)
            {
                // Get first positive row
                DataRow row = subsets[0][0];
                subsets[0].Remove(row);
                groups[0].Add(row);
                groupIndices[row.Table.Rows.IndexOf(row)] = 0;
            }

            for (int j = 0; j < firstGroupNegatives; j++)
            {
                // Get first negative row
                DataRow row = subsets[1][0];
                subsets[1].Remove(row);
                groups[0].Add(row);
                groupIndices[row.Table.Rows.IndexOf(row)] = 0;
            }

            // Put positives and negatives into second group
            for (int j = 0; j < subsets[0].Count; j++)
            {
                // Get first positive row
                DataRow row = subsets[0][j];
                groups[1].Add(row);
                groupIndices[row.Table.Rows.IndexOf(row)] = 1;
            }

            for (int j = 0; j < subsets[1].Count; j++)
            {
                // Get first negative row
                DataRow row = subsets[1][j];
                groups[1].Add(row);
                groupIndices[row.Table.Rows.IndexOf(row)] = 1;
            }

            return groupIndices;
        }

        private DataTable apply(DataTable data)
        {
            DataTable result = data.Copy();
            if (!result.Columns.Contains(GroupIndicatorColumnName))
                result.Columns.Add(GroupIndicatorColumnName, typeof(int));

            for (int i = 0; i < result.Rows.Count; i++)
                result.Rows[i][GroupIndicatorColumnName] = groupIndices[i];

            return result;
        }

        /// <summary>
        ///   Options for the grouping filter.
        /// </summary>
        /// 
        [Serializable]
        public class Options : ColumnOptionsBase
        {
            /// <summary>
            ///   Gets or sets the labels used for each class contained in the column.
            /// </summary>
            /// 
            public int[] Classes { get; set; }

            /// <summary>
            ///   Constructs a new Options object for the given column.
            /// </summary>
            /// 
            /// <param name="name">
            ///   The name of the column to create this options for.
            /// </param>
            /// 
            public Options(String name)
                : base(name)
            {
                Classes = new int[] { 0, 1 };
            }

            /// <summary>
            ///   Constructs a new Options object.
            /// </summary>
            /// 
            public Options()
                : this("New column")
            {

            }
        }

    }
}
