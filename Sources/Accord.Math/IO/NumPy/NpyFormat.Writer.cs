// Accord Formats Library
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

namespace Accord.IO
{
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.IO.Compression;
    using Accord.Compat;
    using System.Threading.Tasks;

    public static partial class NpyFormat
    {
        /// <summary>
        ///   Saves the specified array to an array of bytes.
        /// </summary>
        /// 
        /// <param name="array">The array to be saved to the array of bytes.</param>
        /// 
        /// <returns>A byte array containig the saved array.</returns>
        /// 
        public static byte[] Save(Array array)
        {
            using (var stream = new MemoryStream())
            {
                Save(array, stream);
                return stream.ToArray();
            }
        }

        /// <summary>
        ///   Saves the specified array to the disk using the npy format.
        /// </summary>
        /// 
        /// <param name="array">The array to be saved to disk.</param>
        /// <param name="path">The disk path under which the file will be saved.</param>
        /// 
        /// <returns>The number of bytes written when saving the file to disk.</returns>
        /// 
        public static ulong Save(Array array, string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
                return Save(array, stream);
        }

        /// <summary>
        ///   Saves the specified array to a stream using the npy format.
        /// </summary>
        /// 
        /// <param name="array">The array to be saved to disk.</param>
        /// <param name="stream">The stream to which the file will be saved.</param>
        /// 
        /// <returns>The number of bytes written when saving the file to disk.</returns>
        /// 
        public static ulong Save(Array array, Stream stream)
        {
            using (var writer = new BinaryWriter(stream
#if !NET35 && !NET40
                , System.Text.Encoding.ASCII, leaveOpen: true
#endif
                ))
            {
                Type type;
                int maxLength;
                string dtype = GetDtypeFromType(array, out type, out maxLength);

                int[] shape = array.GetLength(max: true);

                ulong bytesWritten = (ulong)writeHeader(writer, dtype, shape);

                if (array.IsJagged())
                {
                    if (type == typeof(String))
                        return bytesWritten + writeStringMatrix(writer, array, maxLength, shape);
                    return bytesWritten + writeValueJagged(writer, array, maxLength, shape);
                }
                else
                {
                    if (type == typeof(String))
                        return bytesWritten + writeStringMatrix(writer, array, maxLength, shape);
                    return bytesWritten + writeValueMatrix(writer, array, maxLength, shape);
                }
            }
        }

        private static ulong writeValueMatrix(BinaryWriter reader, Array matrix, int bytes, int[] shape)
        {
            int total = 1;
            for (int i = 0; i < shape.Length; i++)
                total *= shape[i];
            var buffer = new byte[bytes * total];

            Buffer.BlockCopy(matrix, 0, buffer, 0, buffer.Length);
            reader.Write(buffer, 0, buffer.Length);

#if NETSTANDARD1_4
            return (ulong)buffer.Length;
#else
            return (ulong)buffer.LongLength;
#endif
        }

        private static ulong writeValueJagged(BinaryWriter reader, Array matrix, int bytes, int[] shape)
        {
            int last = shape[shape.Length - 1];
            byte[] buffer = new byte[bytes * last];
            int[] first = shape.Get(0, -1);

            ulong writtenBytes = 0;
            foreach (Array arr in Jagged.Enumerate<Array>(matrix, first))
            {
                Array.Clear(buffer, arr.Length, buffer.Length - buffer.Length);
                Buffer.BlockCopy(arr, 0, buffer, 0, buffer.Length);
                reader.Write(buffer, 0, buffer.Length);
#if NETSTANDARD1_4
                writtenBytes += (ulong)buffer.Length;
#else
                writtenBytes += (ulong)buffer.LongLength;
#endif
            }

            return writtenBytes;
        }

        private static ulong writeStringMatrix(BinaryWriter reader, Array matrix, int bytes, int[] shape)
        {
            var buffer = new byte[bytes];
            var empty = new byte[bytes];
            empty[0] = byte.MinValue;
            for (int i = 1; i < empty.Length; i++)
                empty[i] = byte.MaxValue;

            ulong writtenBytes = 0;

            unsafe
            {
                fixed (byte* b = buffer)
                {
                    foreach (String s in Jagged.Enumerate<String>(matrix, shape))
                    {
                        if (s != null)
                        {
                            int c = 0;
                            for (int i = 0; i < s.Length; i++)
                                b[c++] = (byte)s[i];
                            for (; c < buffer.Length; c++)
                                b[c] = byte.MinValue;

                            reader.Write(buffer, 0, bytes);
                        }
                        else
                        {
                            reader.Write(empty, 0, bytes);
                        }

#if NETSTANDARD1_4
                        writtenBytes += (ulong)buffer.Length;
#else
                        writtenBytes += (ulong)buffer.LongLength;
#endif
                    }
                }
            }

            return writtenBytes;
        }



        private static int writeHeader(BinaryWriter writer, string dtype, int[] shape)
        {
            // The first 6 bytes are a magic string: exactly "x93NUMPY"

            char[] magic = { 'N', 'U', 'M', 'P', 'Y' };
            writer.Write((byte)147);
            writer.Write(magic);
            writer.Write((byte)1); // major
            writer.Write((byte)0); // minor;

            string tuple = String.Join(", ", shape.Select(i => i.ToString()).ToArray());
            string header = "{{'descr': '{0}', 'fortran_order': False, 'shape': ({1}), }}";
            header = String.Format(header, dtype, tuple);
            int preamble = 10; // magic string (6) + 4

            int len = header.Length + 1; // the 1 is to account for the missing \n at the end
            int headerSize = len + preamble;

            int pad = 16 - (headerSize % 16);
            header = header.PadRight(header.Length + pad);
            header += "\n";
            headerSize = header.Length + preamble;

            if (headerSize % 16 != 0)
                throw new Exception();

            writer.Write((ushort)header.Length);
            for (int i = 0; i < header.Length; i++)
                writer.Write((byte)header[i]);

            return headerSize;
        }

        private static string GetDtypeFromType(Array array, out Type type, out int bytes)
        {
            type = array.GetInnerMostType();

            bytes = 1;

            if (type == typeof(String))
            {
                foreach (String s in Jagged.Enumerate<String>(array))
                {
                    if (s.Length > bytes)
                        bytes = s.Length;
                }
            }
            else if (type == typeof(bool))
            {
                bytes = 1;
            }
            else
            {
#pragma warning disable 618 // SizeOf would be Obsolete
                bytes = Marshal.SizeOf(type);
#pragma warning restore 618 // SizeOf would be Obsolete
            }

            if (type == typeof(bool))
                return "|b1";
            if (type == typeof(Byte))
                return "|i1";
            if (type == typeof(Int16))
                return "<i2";
            if (type == typeof(Int32))
                return "<i4";
            if (type == typeof(Int64))
                return "<i8";
            if (type == typeof(UInt16))
                return "<u2";
            if (type == typeof(UInt32))
                return "<u4";
            if (type == typeof(UInt64))
                return "<u8";
            if (type == typeof(Single))
                return "<f4";
            if (type == typeof(Double))
                return "<f8";
            if (type == typeof(String))
                return "|S" + bytes;

            throw new NotSupportedException();
        }


    }
}
