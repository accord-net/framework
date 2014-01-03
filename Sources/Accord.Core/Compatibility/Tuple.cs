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

#if NET35
namespace Accord
{
    using System.Collections.Generic;

    /// <summary>
    ///   Minimum Tuple implementation for .NET 3.5 to
    ///   make Accord.NET work. This is not a complete implementation.
    /// </summary>
    /// 
    public class Tuple<T1, T2>
    {

        /// <summary>
        ///   Gets or sets the item 1.
        /// </summary>
        /// 
        public T1 Item1 { get; set; }

        /// <summary>
        ///   Gets or sets the item 2.
        /// </summary>
        /// 
        public T2 Item2 { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Tuple&lt;T1, T2&gt;"/> class.
        /// </summary>
        /// 
        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        ///   </exception>
        public override bool Equals(object obj)
        {
            var other = obj as Tuple<T1, T2>;
            if (other == null) return false;

            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(Item2, other.Item2);
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// 
        public override int GetHashCode()
        {
            return (Item1 == null ? 1 : Item1.GetHashCode())
                 ^ (Item2 == null ? 2 : Item2.GetHashCode());
        }
    }

    /// <summary>
    ///   Minimum Tuple implementation for .NET 3.5 to
    ///   make Accord.NET work. This is not a complete implementation.
    /// </summary>
    /// 
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public class Tuple<T1, T2, T3>
    {
        /// <summary>
        ///   Gets or sets the item 1.
        /// </summary>
        public T1 Item1 { get; set; }

        /// <summary>
        ///   Gets or sets the item 2.
        /// </summary>
        /// 
        public T2 Item2 { get; set; }

        /// <summary>
        ///   Gets or sets the item 3.
        /// </summary>
        /// 
        public T3 Item3 { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Tuple&lt;T1, T2, T3&gt;"/> class.
        /// </summary>
        /// 
        public Tuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        ///   </exception>
        ///   
        public override bool Equals(object obj)
        {
            var other = obj as Tuple<T1, T2, T3>;
            if (other == null) return false;

            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(Item3, other.Item3);
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// 
        public override int GetHashCode()
        {
            return (Item1 == null ? 1 : Item1.GetHashCode())
                 ^ (Item2 == null ? 2 : Item2.GetHashCode())
                 ^ (Item3 == null ? 3 : Item3.GetHashCode());
        }
    }


    /// <summary>
    ///   Minimum Tuple implementation for .NET 3.5 to
    ///   make Accord.NET work. This is not a complete implementation.
    /// </summary>
    /// 
    public static class Tuple
    {
        /// <summary>
        ///   Creates the specified tuple.
        /// </summary>
        /// 
        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            var tuple = new Tuple<T1, T2>(item1, item2);
            return tuple;
        }

        /// <summary>
        ///   Creates the specified tuple.
        /// </summary>
        /// 
        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
        {
            var tuple = new Tuple<T1, T2, T3>(item1, item2, item3);
            return tuple;
        }
    }
}
#endif