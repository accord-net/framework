// Accord Core Library
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

#if !NET45
namespace System.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///   Minimum IReadOnlyCollection implementation for .NET 4.0 to
    ///   make Accord.NET work. This is not a complete implementation.
    /// </summary>
    /// 
    public interface IReadOnlyCollection<
#if !NET35
        out
#endif
        T> : IEnumerable<T>
    {
        /// <summary>
        ///   Gets the item at the specified index.
        /// </summary>
        /// 
        T this[int index] { get; }

        /// <summary>
        ///   Gets the number of elements in the collection.
        /// </summary>
        /// 
        int Count { get; }
    }

}
#endif
