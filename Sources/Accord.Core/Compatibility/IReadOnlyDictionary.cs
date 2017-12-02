// Accord Core Library
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

#if NET35 || NET40
namespace Accord.Compat
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///   Minimum IReadOnlyDictionary implementation for .NET 3.5 to
    ///   make Accord.NET work. This is not a complete implementation.
    /// </summary>
    /// 
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {

        /// <summary>
        ///   Gets the keys.
        /// </summary>
        /// 
        IEnumerable<TKey> Keys { get; }

        /// <summary>
        ///   Gets the values.
        /// </summary>
        /// 
        IEnumerable<TValue> Values { get; }

        /// <summary>
        ///   Gets the value associated with the specified key.
        /// </summary>
        /// 
        TValue this[TKey key] { get; }

        /// <summary>
        ///   Determines whether the dictionary contains the specified key.
        /// </summary>
        /// 
        bool ContainsKey(TKey key);

        /// <summary>
        ///   Tries to get a value.
        /// </summary>
        /// 
        bool TryGetValue(TKey key, out TValue value);
    }
}
#endif
