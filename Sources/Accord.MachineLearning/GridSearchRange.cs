// Accord Machine Learning Library
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

namespace Accord.MachineLearning
{
    using System;
    using System.Collections.ObjectModel;
    using Accord.Math;

    /// <summary>
    ///   Range of parameters to be tested in a grid search.
    /// </summary>
    /// 
    [Serializable]
    public class GridSearchRange
    {

        /// <summary>
        ///   Gets or sets the name of the parameter from which the range belongs to.
        /// </summary>
        /// 
        public string Name { get; set; }

        /// <summary>
        ///   Gets or sets the range of values that should be tested for this parameter.
        /// </summary>
        /// 
        public double[] Values { get; set; }

        /// <summary>
        ///   Constructs a new GridsearchRange object.
        /// </summary>
        /// 
        /// <param name="name">The name for this parameter.</param>
        /// <param name="start">The starting value for this range.</param>
        /// <param name="end">The end value for this range.</param>
        /// <param name="step">The step size for this range.</param>
        /// 
        public GridSearchRange(string name, double start, double end, double step)
        {
            this.Name = name;
            this.Values = Matrix.Interval(start, end, step);
        }

        /// <summary>
        ///   Constructs a new GridSearchRange object.
        /// </summary>
        /// 
        /// <param name="name">The name for this parameter.</param>
        /// <param name="values">The array of values to try.</param>
        /// 
        public GridSearchRange(string name, double[] values)
        {
            this.Name = name;
            this.Values = values;
        }

        /// <summary>
        ///   Gets the array of GridSearchParameters to try.
        /// </summary>
        /// 
        public GridSearchParameter[] GetParameters()
        {
            GridSearchParameter[] parameters = new GridSearchParameter[Values.Length];
            for (int i = 0; i < Values.Length; i++)
                parameters[i] = new GridSearchParameter(Name, Values[i]);
            return parameters;
        }

    }

    /// <summary>
    ///   GridSearchRange collection.
    /// </summary>
    /// 
    [Serializable]
    public class GridSearchRangeCollection : KeyedCollection<string, GridSearchRange>
    {
        /// <summary>
        ///   Constructs a new collection of GridsearchRange objects.
        /// </summary>
        /// 
        public GridSearchRangeCollection(params GridSearchRange[] ranges)
        {
            foreach (var range in ranges)
                this.Add(range);
        }

        /// <summary>
        ///   Returns the identifying value for an item on this collection.
        /// </summary>
        /// 
        protected override string GetKeyForItem(GridSearchRange item)
        {
            return item.Name;
        }

        /// <summary>
        ///   Adds a parameter range to the end of the GridsearchRangeCollection.
        /// </summary>
        /// 
        public void Add(string name, params double[] values)
        {
            this.Add(new GridSearchRange(name, values));
        }
    }

}
