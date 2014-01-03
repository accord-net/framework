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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    ///   Minimum Lazy implementation for .NET 3.5 to make
    ///   Accord.NET work. This is not a complete implementation.
    /// </summary>
    /// 
    internal class Lazy<T>
    {
        private readonly Func<T> valueFactory;
        private readonly object lockObj = new Object();

        private bool isValueCreated;
        private T value;


        /// <summary>
        ///   Initializes a new instance of the <see cref="Lazy&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="valueFactory">A function which creates the instance value on first access.</param>
        /// 
        public Lazy(Func<T> valueFactory)
            : this(valueFactory, true) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Lazy&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="valueFactory">A function which creates the instance value on first access.</param>
        /// <param name="isThreadSafe">Needs to be true.</param>
        /// 
        public Lazy(Func<T> valueFactory, bool isThreadSafe)
        {
            if (valueFactory == null)
                throw new ArgumentNullException("valueFactory");

            if (isThreadSafe == false)
                throw new ArgumentException("This implementation only supports thread-safe instances.",
                    "isThreadSafe");

            this.valueFactory = valueFactory;
        }

        /// <summary>
        ///   Gets the lazily initialized value for this instance.
        /// </summary>
        /// 
        public T Value
        {
            get
            {
                if (!isValueCreated)
                {
                    lock (lockObj)
                    {
                        if (!isValueCreated)
                        {
                            value = valueFactory();

                            Thread.MemoryBarrier();
                            isValueCreated = true;
                        }
                    }
                }

                Thread.MemoryBarrier();
                return value;
            }
        }

        /// <summary>
        ///   Gets a value that indicates whether a value has been created for this Lazy{T} instance.
        /// </summary>
        /// 
        public bool IsValueCreated
        {
            get
            {
                lock (lockObj)
                {
                    return isValueCreated;
                }
            }
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
            return Value.ToString();
        }
    }
}
#endif