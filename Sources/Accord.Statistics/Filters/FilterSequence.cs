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
    using System.Collections;
    using System;
    using System.Data;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;

    /// <summary>
    ///   Sequence of table processing filters.
    /// </summary>
    /// 
    [Serializable]
    public class FiltersSequence : Collection<IFilter>, IFilter
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="FiltersSequence"/> class.
        /// </summary>
        /// 
        public FiltersSequence() { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FiltersSequence"/> class.
        /// </summary>
        /// 
        /// <param name="filters">Sequence of filters to apply.</param>
        /// 
        public FiltersSequence(params IFilter[] filters) 
            :base(new List<IFilter>(filters))
        {
        }


        /// <summary>
        ///   Applies the sequence of filters to a given table.
        /// </summary>
        public DataTable Apply(DataTable data)
        {
            if (data == null)
                throw new ArgumentNullException("data");


            DataTable result = data;

            foreach (IFilter filter in this)
            {
                result = filter.Apply(result);
            }

            return result;
        }
    }
}
