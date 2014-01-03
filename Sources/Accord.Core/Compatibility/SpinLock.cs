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
    using System.Threading;

    /// <summary>
    ///   Minimum SpinLock implementation for .NET 3.5 to make
    ///   Accord.NET work. This is not a complete implementation.
    /// </summary>
    /// 
    internal struct SpinLock
    {
        private int lockObj;

        /// <summary>
        ///   Gets whether the lock is currently held by any thread.
        /// </summary>
        /// 
        public bool IsHeld
        {
            get { return this.lockObj != 0; }
        }

        /// <summary>
        ///   Acquires the lock.
        /// </summary>
        /// 
        public void Enter(ref bool taken)
        {
            if (Interlocked.CompareExchange(ref lockObj, 1, 0) != 0)
            {
                int count = 0;
                while (Interlocked.CompareExchange(ref lockObj, 1, 0) != 0)
                {
                    count++;

                    if (Environment.ProcessorCount > 1 && count <= 5)
                        Thread.SpinWait(25);
                    else
                        Thread.Sleep(0);
                }
            }

            taken = true;
        }

        /// <summary>
        ///   Releases the lock.
        /// </summary>
        /// 
        public void Exit()
        {
            this.lockObj = 0;
        }
    }
}
#endif
