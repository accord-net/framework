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
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   Column options for filter which have per-column settings.
    /// </summary>
    /// 
    [Serializable]
    public abstract class ColumnOptionsBase<TFilter>
    {
        [NonSerialized]
        private CancellationToken token;

        /// <summary>
        ///   Gets or sets the filter to which these options belong to.
        /// </summary>
        /// 
        /// <value>The owner filter.</value>
        /// 
        public TFilter Owner { get; set; }

        /// <summary>
        ///   Gets or sets the name of the column that the options will apply to.
        /// </summary>
        /// 
        public String ColumnName { get; internal set; }

        /// <summary>
        ///   Gets or sets a user-determined object associated with this column.
        /// </summary>
        /// 
        public object Tag { get; set; }

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
}
