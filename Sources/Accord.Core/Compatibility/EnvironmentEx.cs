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
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    ///   Set of methods necessary to make Accord.NET work in .NET 3.5 and .NET 4.0. 
    ///   Those methods are not available in other versions of the framework and should
    ///   not be called by user code.
    /// </summary>
    /// 
    public static class EnvironmentEx
    {
        private static int CSIDL_SYSTEM = 0x0025;
        private static int CSIDL_SYSTEMX86 = 0x0029;

        [DllImport("shell32.dll")]
        static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out]StringBuilder lpszPath, int nFolder, bool fCreate);

        /// <summary>
        ///   Gets the 32-bit system path.
        /// </summary>
        public static string GetWindowsSystemDirectory32()
        {
            StringBuilder sb = new StringBuilder(2048);
            SHGetSpecialFolderPath(IntPtr.Zero, sb, CSIDL_SYSTEM, false);
            return sb.ToString();

        }

        /// <summary>
        ///   Gets the 64-bit system path.
        /// </summary>
        public static string GetWindowsSystemDirectory64()
        {
            StringBuilder sb = new StringBuilder(2048);
            SHGetSpecialFolderPath(IntPtr.Zero, sb, CSIDL_SYSTEMX86, false);
            return sb.ToString();

        }

        /// <summary>
        ///   Gets wether the process (not the system) is running in 64-bits.
        /// </summary>
        public static readonly bool Is64BitProcess = IntPtr.Size == 4;

    }
}
#endif