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
    using System.Collections.ObjectModel;
    /// <summary>
    ///   Column option collection.
    /// </summary>
    /// 
    [Serializable]
    public class ColumnOptionCollection<T> : KeyedCollection<String, T>
        where T : ColumnOptionsBase
    {
        /// <summary>
        ///   Extracts the key from the specified column options.
        /// </summary>
        /// 
        protected override string GetKeyForItem(T item)
        {
            return item.ColumnName;
        }

        /// <summary>
        ///   Adds a new column options definition to the collection.
        /// </summary>
        /// 
        /// <param name="options">The column options to be added.</param>
        /// 
        /// <returns>The added column options.</returns>
        /// 
        new public T Add(T options)
        {
            base.Add(options);
            return options;
        }

        /// <summary>
        ///   Gets the associated options for the given column name.
        /// </summary>
        /// 
        /// <param name="columnName">The name of the column whose options should be retrieved.</param>
        /// <param name="options">The retrieved options.</param>
        /// 
        /// <returns>True if the options was contained in the collection; false otherwise.</returns>
        /// 
        public bool TryGetValue(String columnName, out T options)
        {
            return base.Dictionary.TryGetValue(columnName, out options);
        }
    }
}
