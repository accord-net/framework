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

namespace Accord.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    ///   Temporary internal framework class for handling debug assertions
    ///   while inside unit tests. Will be removed in a future release.
    /// </summary>
    /// 
    public static class Trace
    {
        /// <summary>
        ///   Throws an exception if a condition is false.
        /// </summary>
        /// 
        [Conditional("TRACE")]
        public static void Assert(bool condition, string message = "Internal framework error.")
        {
            if (!condition)
                throw new Exception(message);
        }

        /// <summary>
        ///   Compatibility method for .NET Standard 4.1.
        /// </summary>
        /// 
        public static void WriteLine(string v)
        {
#if NETSTANDARD1_4
            Console.WriteLine(v);
#else
            System.Diagnostics.Trace.WriteLine(v);
#endif
        }

        /// <summary>
        ///   Compatibility method for .NET Standard 4.1.
        /// </summary>
        /// 
        public static void TraceWarning(string v)
        {
#if NETSTANDARD1_4
            Console.WriteLine(v);
#else
            System.Diagnostics.Trace.TraceWarning(v);
#endif
        }

        /// <summary>
        ///   Compatibility method for .NET Standard 4.1.
        /// </summary>
        /// 
        public static void Write(string v)
        {
#if NETSTANDARD1_4
            Console.Write(v);
#else
            System.Diagnostics.Trace.Write(v);
#endif
        }

        /// <summary>
        ///   Compatibility method for .NET Standard 4.1.
        /// </summary>
        /// 
        public static void TraceWarning(string v, params object[] p)
        {
#if NETSTANDARD1_4
            Console.Write(String.Format(v, p));
#else
            System.Diagnostics.Trace.TraceWarning(v, p);
#endif
        }
    }
}
