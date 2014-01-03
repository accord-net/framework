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

    /// <summary>
    ///   Minimum ThreadLocal implementation for .NET 3.5 to make
    ///   Accord.NET work. This is not a complete implementation.
    /// </summary>
    /// 
    internal class ThreadLocal<T> : IDisposable
    {
        [ThreadStatic]
        private static Dictionary<object, T> lookupTable;

        private Func<T> init;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ThreadLocal&lt;T&gt;"/> class.
        /// </summary>
        /// 
        public ThreadLocal() : this(() => default(T)) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ThreadLocal&lt;T&gt;"/> class.
        /// </summary>
        /// 
        public ThreadLocal(Func<T> init)
        {
            this.init = init;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="ThreadLocal&lt;T&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        /// 
        ~ThreadLocal()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Gets or sets the value.
        /// </summary>
        /// 
        public T Value
        {
            get
            {
                T returnValue;

                if (lookupTable == null)
                {
                    lookupTable = new Dictionary<object, T>();
                    returnValue = lookupTable[this] = init();
                }
                else
                {
                    if (!lookupTable.TryGetValue(this, out returnValue))
                        returnValue = lookupTable[this] = init();
                }

                return returnValue;
            }
            set
            {
                if (lookupTable == null)
                    lookupTable = new Dictionary<object, T>();
                lookupTable[this] = value;
            }
        }

        /// <summary>
        ///   Performs application-defined tasks associated with 
        ///   freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// 
        public void Dispose()
        {
           Dispose(true);
           GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// 
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged
        ///   resources; <c>false</c> to release only unmanaged resources.</param>
        /// 
        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (lookupTable != null)
                {
                    if (lookupTable.ContainsKey(this)) 
                       lookupTable.Remove(this);
                }
            }
        }

    }
}
#endif