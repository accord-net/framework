// Accord Core Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Based on the original classes from: 
//
// AForge Core Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
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

namespace Accord
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Set of systems tools.
    /// </summary>
    /// 
    /// <remarks><para>The class is a container of different system tools, which are used
    /// across the framework. Some of these tools are platform specific, so their
    /// implementation is different on different platform, like .NET and Mono.</para>
    /// </remarks>
    /// 
    public static class SystemTools
    {
        /// <summary>
        /// Determines whether the code is running under Mono. 
        /// </summary>
        /// 
        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        /// <summary>
        /// Copy block of unmanaged memory.
        /// </summary>
        /// 
        /// <param name="dst">Destination pointer.</param>
        /// <param name="src">Source pointer.</param>
        /// <param name="count">Memory block's length to copy.</param>
        /// 
        /// <returns>Return's value of <paramref name="dst"/> - pointer to destination.</returns>
        /// 
        /// <remarks><para>This function is required because of the fact that .NET does
        /// not provide any way to copy unmanaged blocks, but provides only methods to
        /// copy from unmanaged memory to managed memory and vise versa.</para></remarks>
        ///
        public static IntPtr CopyUnmanagedMemory(IntPtr dst, IntPtr src, int count)
        {
            unsafe
            {
                CopyUnmanagedMemory((byte*)dst.ToPointer(), (byte*)src.ToPointer(), count);
            }
            return dst;
        }

        /// <summary>
        /// Copy block of unmanaged memory.
        /// </summary>
        /// 
        /// <param name="dst">Destination pointer.</param>
        /// <param name="src">Source pointer.</param>
        /// <param name="count">Memory block's length to copy.</param>
        /// 
        /// <returns>Return's value of <paramref name="dst"/> - pointer to destination.</returns>
        /// 
        /// <remarks><para>This function is required because of the fact that .NET does
        /// not provide any way to copy unmanaged blocks, but provides only methods to
        /// copy from unmanaged memory to managed memory and vise versa.</para></remarks>
        /// 
        public static unsafe byte* CopyUnmanagedMemory(byte* dst, byte* src, int count)
        {
            return memcpy(dst, src, count);
        }

        /// <summary>
        /// Fill memory region with specified value.
        /// </summary>
        /// 
        /// <param name="dst">Destination pointer.</param>
        /// <param name="filler">Filler byte's value.</param>
        /// <param name="count">Memory block's length to fill.</param>
        /// 
        /// <returns>Return's value of <paramref name="dst"/> - pointer to destination.</returns>
        /// 
        public static IntPtr SetUnmanagedMemory(IntPtr dst, int filler, int count)
        {
            unsafe
            {
                SetUnmanagedMemory((byte*)dst.ToPointer(), filler, count);
            }
            return dst;
        }

        /// <summary>
        /// Fill memory region with specified value.
        /// </summary>
        /// 
        /// <param name="dst">Destination pointer.</param>
        /// <param name="filler">Filler byte's value.</param>
        /// <param name="count">Memory block's length to fill.</param>
        /// 
        /// <returns>Return's value of <paramref name="dst"/> - pointer to destination.</returns>
        /// 
        public static unsafe byte* SetUnmanagedMemory(byte* dst, int filler, int count)
        {
            return memset(dst, filler, count);
        }


        // Win32 memory copy function
        [DllImport("ntdll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static unsafe extern byte* memcpy(byte* dst, byte* src, int count);

        // Win32 memory set function
        [DllImport("ntdll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static unsafe extern byte* memset(byte* dst, int filler, int count);

    }
}
