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
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using Accord.Compat;
    using System.Threading.Tasks;

#if !NET35 && !NET40
    /// <summary>
    ///   Provides static methods to save and load files saved in NumPy's .npy format. 
    /// </summary>
    /// 
    /// <seealso cref="NpzFormat"/>
#else
    /// <summary>
    ///   Provides static methods to save and load files saved in NumPy's .npy format. 
    /// </summary>
    /// 
#endif
    public static partial class NpyFormat
    {
        /// <summary>
        ///   Loads an array of the specified type from a byte array.
        /// </summary>
        /// 
        /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
        /// <param name="bytes">The bytes that contain the matrix to be loaded.</param>
        /// 
        /// <returns>The array to be returned.</returns>
        /// 
        public static T Load<T>(byte[] bytes)
            where T : class,
#if !NETSTANDARD1_4
            ICloneable,
#endif
            IList, ICollection, IEnumerable
#if !NET35
            , IStructuralComparable, IStructuralEquatable
#endif
        {
            if (typeof(T).IsJagged())
                return LoadJagged(bytes).To<T>();
            return LoadMatrix(bytes).To<T>();
        }

        /// <summary>
        ///   Loads an array of the specified type from a file in the disk.
        /// </summary>
        /// 
        /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
        /// <param name="bytes">The bytes that contain the matrix to be loaded.</param>
        /// <param name="value">The object to be read. This parameter can be used to avoid the
        ///   need of specifying a generic argument to this function.</param>
        /// 
        /// <returns>The array to be returned.</returns>
        /// 
        public static T Load<T>(byte[] bytes, out T value)
            where T : class,
#if !NETSTANDARD1_4
            ICloneable,
#endif
IList, ICollection, IEnumerable
#if !NET35
            , IStructuralComparable, IStructuralEquatable
#endif
        {
            return value = Load<T>(bytes);
        }

        /// <summary>
        ///   Loads an array of the specified type from a file in the disk.
        /// </summary>
        /// 
        /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
        /// <param name="path">The path to the file containing the matrix to be loaded.</param>
        /// <param name="value">The object to be read. This parameter can be used to avoid the
        ///   need of specifying a generic argument to this function.</param>
        /// 
        /// <returns>The array to be returned.</returns>
        /// 
        public static T Load<T>(string path, out T value)
            where T : class,
#if !NETSTANDARD1_4
            ICloneable,
#endif
            IList, ICollection, IEnumerable
#if !NET35
            , IStructuralComparable, IStructuralEquatable
#endif
        {
            return value = Load<T>(path);
        }

        /// <summary>
        ///   Loads an array of the specified type from a stream.
        /// </summary>
        /// 
        /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
        /// <param name="stream">The stream containing the matrix to be loaded.</param>
        /// <param name="value">The object to be read. This parameter can be used to avoid the
        ///   need of specifying a generic argument to this function.</param>
        /// 
        /// <returns>The array to be returned.</returns>
        /// 
        public static T Load<T>(Stream stream, out T value)
            where T : class,
#if !NETSTANDARD1_4
            ICloneable,
#endif
            IList, ICollection, IEnumerable
#if !NET35
            , IStructuralComparable, IStructuralEquatable
#endif
        {
            return value = Load<T>(stream);
        }

        /// <summary>
        ///   Loads an array of the specified type from a file in the disk.
        /// </summary>
        /// 
        /// <param name="path">The path to the file containing the matrix to be loaded.</param>
        /// 
        /// <returns>The array to be returned.</returns>
        /// 
        public static T Load<T>(string path)
            where T : class,
#if !NETSTANDARD1_4
            ICloneable,
#endif
            IList, ICollection, IEnumerable
#if !NET35
            , IStructuralComparable, IStructuralEquatable
#endif
        {
            using (var stream = new FileStream(path, FileMode.Open))
                return Load<T>(stream);
        }

        /// <summary>
        ///   Loads an array of the specified type from a stream.
        /// </summary>
        /// 
        /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
        /// <param name="stream">The stream containing the matrix to be loaded.</param>
        /// 
        /// <returns>The array to be returned.</returns>
        /// 
        public static T Load<T>(Stream stream)
            where T : class,
#if !NETSTANDARD1_4
            ICloneable,
#endif
            IList, ICollection, IEnumerable
#if !NET35
            , IStructuralComparable, IStructuralEquatable
#endif
        {
            if (typeof(T).IsJagged())
                return LoadJagged(stream).To<T>();
            return LoadMatrix(stream).To<T>();
        }



        /// <summary>
        ///   Loads a multi-dimensional array from an array of bytes.
        /// </summary>
        /// 
        /// <param name="bytes">The bytes that contain the matrix to be loaded.</param>
        /// 
        /// <returns>A multi-dimensional array containing the values available in the given stream.</returns>
        /// 
        public static Array LoadMatrix(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
                return LoadMatrix(stream);
        }

        /// <summary>
        ///   Loads a multi-dimensional array from a file in the disk.
        /// </summary>
        /// 
        /// <param name="path">The path to the file containing the matrix to be loaded.</param>
        /// 
        /// <returns>A multi-dimensional array containing the values available in the given stream.</returns>
        /// 
        public static Array LoadMatrix(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
                return LoadMatrix(stream);
        }

        /// <summary>
        ///   Loads a jagged array from an array of bytes.
        /// </summary>
        /// 
        /// <param name="bytes">The bytes that contain the matrix to be loaded.</param>
        /// 
        /// <returns>A jagged array containing the values available in the given stream.</returns>
        /// 
        public static Array LoadJagged(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
                return LoadJagged(stream);
        }

        /// <summary>
        ///   Loads a jagged array from a file in the disk.
        /// </summary>
        /// 
        /// <param name="path">The path to the file containing the matrix to be loaded.</param>
        /// 
        /// <returns>A jagged array containing the values available in the given stream.</returns>
        /// 
        public static Array LoadJagged(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
                return LoadJagged(stream);
        }




        /// <summary>
        ///   Loads a multi-dimensional array from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream containing the matrix to be loaded.</param>
        /// 
        /// <returns>A multi-dimensional array containing the values available in the given stream.</returns>
        /// 
        public static Array LoadMatrix(Stream stream)
        {
            using (var reader = new BinaryReader(stream, System.Text.Encoding.ASCII
#if !NET35 && !NET40
            , leaveOpen: true
#endif
            ))
            {
                int bytes;
                Type type;
                int[] shape;
                if (!parseReader(reader, out bytes, out type, out shape))
                    throw new FormatException();

                Array matrix = Matrix.Zeros(type, shape);

                if (type == typeof(String))
                    return readStringMatrix(reader, matrix, bytes, type, shape);
                return readValueMatrix(reader, matrix, bytes, type, shape);
            }
        }

        /// <summary>
        ///   Loads a jagged array from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream containing the matrix to be loaded.</param>
        /// <param name="trim">Pass true to remove null or empty elements from the loaded array.</param>
        /// 
        /// <returns>A jagged array containing the values available in the given stream.</returns>
        /// 
        public static Array LoadJagged(Stream stream, bool trim = true)
        {
            using (var reader = new BinaryReader(stream, System.Text.Encoding.ASCII
#if !NET35 && !NET40
            , leaveOpen: true
#endif
            ))
            {
                int bytes;
                Type type;
                int[] shape;
                if (!parseReader(reader, out bytes, out type, out shape))
                    throw new FormatException();

                Array matrix = Jagged.Zeros(type, shape);

                if (type == typeof(String))
                {
                    Array result = readStringMatrix(reader, matrix, bytes, type, shape);

                    if (trim)
                        return result.Trim();
                    return result;
                }

                return readValueJagged(reader, matrix, bytes, type, shape);
            }
        }









        private static Array readValueMatrix(BinaryReader reader, Array matrix, int bytes, Type type, int[] shape)
        {
            int total = 1;
            for (int i = 0; i < shape.Length; i++)
                total *= shape[i];
            var buffer = new byte[bytes * total];

            reader.Read(buffer, 0, buffer.Length);
            Buffer.BlockCopy(buffer, 0, matrix, 0, buffer.Length);

            return matrix;
        }

        private static Array readValueJagged(BinaryReader reader, Array matrix, int bytes, Type type, int[] shape)
        {
            int last = shape[shape.Length - 1];
            byte[] buffer = new byte[bytes * last];

            int[] firsts = new int[shape.Length - 1];
            for (int i = 0; i < firsts.Length; i++)
                firsts[i] = -1;

            foreach (var p in matrix.GetIndices(deep: true))
            {
                bool changed = false;
                for (int i = 0; i < firsts.Length; i++)
                {
                    if (firsts[i] != p[i])
                    {
                        firsts[i] = p[i];
                        changed = true;
                    }
                }

                if (!changed)
                    continue;

                Array arr = (Array)matrix.GetValue(deep: true, indices: firsts);

                reader.Read(buffer, 0, buffer.Length);
                Buffer.BlockCopy(buffer, 0, arr, 0, buffer.Length);
            }
            return matrix;
        }

        private static Array readStringMatrix(BinaryReader reader, Array matrix, int bytes, Type type, int[] shape)
        {
            var buffer = new byte[bytes];

            unsafe
            {
                fixed (byte* b = buffer)
                {
                    foreach (var p in matrix.GetIndices(deep: true))
                    {
                        reader.Read(buffer, 0, bytes);
                        if (buffer[0] == byte.MinValue)
                        {
                            bool isNull = true;
                            for (int i = 1; i < buffer.Length; i++)
                            {
                                if (buffer[i] != byte.MaxValue)
                                {
                                    isNull = false;
                                    break;
                                }
                            }

                            if (isNull)
                            {
                                matrix.SetValue(value: null, deep: true, indices: p);
                                continue;
                            }
                        }

#if NETSTANDARD1_4
                        String s = new String((char*)b);
#else
                        String s = new String((sbyte*)b);
#endif
                        matrix.SetValue(value: s, deep: true, indices: p);
                    }
                }
            }

            return matrix;
        }



        private static bool parseReader(BinaryReader reader, out int bytes, out Type t, out int[] shape)
        {
            bytes = 0;
            t = null;
            shape = null;

            // The first 6 bytes are a magic string: exactly "x93NUMPY"
            if (reader.ReadChar() != 63) return false;
            if (reader.ReadChar() != 'N') return false;
            if (reader.ReadChar() != 'U') return false;
            if (reader.ReadChar() != 'M') return false;
            if (reader.ReadChar() != 'P') return false;
            if (reader.ReadChar() != 'Y') return false;

            byte major = reader.ReadByte(); // 1
            byte minor = reader.ReadByte(); // 0

            if (major != 1 || minor != 0)
                throw new NotSupportedException();

            ushort len = reader.ReadUInt16();

            string header = new String(reader.ReadChars(len));
            string mark = "'descr': '";
            int s = header.IndexOf(mark) + mark.Length;
            int e = header.IndexOf("'", s + 1);
            string type = header.Substring(s, e - s);
            bool? isLittleEndian;
            t = GetType(type, out bytes, out isLittleEndian);

            if (isLittleEndian.HasValue && isLittleEndian.Value == false)
                throw new Exception();

            mark = "'fortran_order': ";
            s = header.IndexOf(mark) + mark.Length;
            e = header.IndexOf(",", s + 1);
            bool fortran = bool.Parse(header.Substring(s, e - s));

            if (fortran)
                throw new Exception();

            mark = "'shape': (";
            s = header.IndexOf(mark) + mark.Length;
            e = header.IndexOf(")", s + 1);
            shape = header.Substring(s, e - s).Split(',').Select(Int32.Parse).ToArray();

            return true;
        }

        private static Type GetType(string dtype, out int bytes, out bool? isLittleEndian)
        {
            isLittleEndian = IsLittleEndian(dtype);
            bytes = Int32.Parse(dtype.Substring(2));

            string typeCode = dtype.Substring(1);

            if (typeCode == "b1")
                return typeof(bool);
            if (typeCode == "i1")
                return typeof(SByte);
            if (typeCode == "i2")
                return typeof(Int16);
            if (typeCode == "i4")
                return typeof(Int32);
            if (typeCode == "i8")
                return typeof(Int64);
            if (typeCode == "u1")
                return typeof(Byte);
            if (typeCode == "u2")
                return typeof(UInt16);
            if (typeCode == "u4")
                return typeof(UInt32);
            if (typeCode == "u8")
                return typeof(UInt64);
            if (typeCode == "f4")
                return typeof(Single);
            if (typeCode == "f8")
                return typeof(Double);
            if (typeCode.StartsWith("S"))
                return typeof(String);

            throw new NotSupportedException();
        }

        private static bool? IsLittleEndian(string type)
        {
            bool? littleEndian = null;

            switch (type[0])
            {
                case '<':
                    littleEndian = true;
                    break;
                case '>':
                    littleEndian = false;
                    break;
                case '|':
                    littleEndian = null;
                    break;
                default:
                    throw new Exception();
            }

            return littleEndian;
        }
    }
}
