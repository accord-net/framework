// AForge Core Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
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
