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
    using Accord.Compat;

    /// <summary>
    ///   Column option collection.
    /// </summary>
    /// 
    /// <typeparam name="TFilter">The type of the filter that this collection belongs to.</typeparam>
    /// <typeparam name="TOptions">The type of the column options that will be used by the 
    ///   <typeparamref name="TFilter"/> to determine how to process a particular column.</typeparam>
    /// 
    [Serializable]
    public class ColumnOptionCollection<TOptions, TFilter> : KeyedCollection<String, TOptions>
        where TOptions : ColumnOptionsBase<TFilter>
    {

        /// <summary>
        ///   Occurs when a new <typeparamref name="TOptions"/> is being 
        ///   added to the collection. Handlers of this event can prevent a column
        ///   options from being added by throwing an exception.
        /// </summary>
        /// 
#if !NET35 && !NET40
        public event EventHandler<TOptions> AddingNew;
#else
        public event ColumnOptionsEventHandler AddingNew;

        /// <summary>
        ///   Compatibility event args for the <see cref="AddingNew"/> event. This
        ///   is only required and used for the .NET 3.5 version of the framework.
        /// </summary>
        /// 
        public delegate void ColumnOptionsEventHandler(object sender, TOptions options);
#endif

        /// <summary>
        ///   Extracts the key from the specified column options.
        /// </summary>
        /// 
        protected override string GetKeyForItem(TOptions item)
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
        new public TOptions Add(TOptions options)
        {
            if (AddingNew != null)
                AddingNew(this, options);

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
        public bool TryGetValue(String columnName, out TOptions options)
        {
            return base.Dictionary.TryGetValue(columnName, out options);
        }
    }
}
