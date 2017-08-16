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

#if !NET35 && !NET40
namespace Accord.IO
{
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO.Compression;
    using System.Collections;

    /// <summary>
    ///   Provides static methods to save and load files saved in NumPy's .npz format. 
    /// </summary>
    /// 
    /// <seealso cref="NpyFormat"/>
    /// 
    public static partial class NpzFormat
    {

        /// <summary>
        ///   Loads an array of the specified type from an array of bytes.
        /// </summary>
        /// 
        /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
        /// <param name="bytes">The bytes that contain the matrix to be loaded.</param>
        /// <param name="value">The object to be read. This parameter can be used to avoid the
        ///   need of specifying a generic argument to this function.</param>
        /// 
        /// <returns>The array to be returned.</returns>
        /// 
        public static void Load<T>(byte[] bytes, out T value)
            where T : class,
#if !NETSTANDARD1_4
            ICloneable,
#endif
            IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
        {
            using (var dict = Load<T>(bytes))
            {
                value = dict.Values.First();
            }
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
        public static void Load<T>(string path, out T value)
            where T : class,
#if !NETSTANDARD1_4
            ICloneable,
#endif
            IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
        {
            using (var dict = Load<T>(path))
            {
                value = dict.Values.First();
            }
        }

        /// <summary>
        ///   Loads an array of the specified type from a file in the disk.
        /// </summary>
        /// 
        /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
        /// <param name="stream">The stream containing the matrix to be loaded.</param>
        /// <param name="value">The object to be read. This parameter can be used to avoid the
        ///   need of specifying a generic argument to this function.</param>
        /// 
        /// <returns>The array to be returned.</returns>
        /// 
        public static void Load<T>(Stream stream, out T value)
            where T : class,
#if !NETSTANDARD1_4
            ICloneable,
#endif
            IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
        {
            using (var dict = Load<T>(stream))
            {
                value = dict.Values.First();
            }
        }


        /// <summary>
        ///   Gets a lazily-instantiated array that can be used to load 
        ///   matrices of the specified type from an array of bytes.
        /// </summary>
        /// 
        /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
        /// <param name="bytes">The bytes that contain the matrix to be loaded.</param>
        /// 
        /// <returns>The array to be returned.</returns>
        /// 
        public static NpzDictionary<T> Load<T>(byte[] bytes)
            where T : class,
#if !NETSTANDARD1_4
            ICloneable,
#endif
            IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
        {
            return Load<T>(new MemoryStream(bytes));
        }

        /// <summary>
        ///   Gets a lazily-instantiated array that can be used to load 
        ///   matrices of the specified type from a file in the disk.
        /// </summary>
        /// 
        /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
        /// <param name="path">The path to the file containing the matrix to be loaded.</param>
        /// <param name="value">The object to be read. This parameter can be used to avoid the
        ///   need of specifying a generic argument to this function.</param>
        /// 
        /// <returns>The array to be returned.</returns>
        /// 
        public static NpzDictionary<T> Load<T>(string path, out NpzDictionary<T> value)
            where T : class,
#if !NETSTANDARD1_4
            ICloneable,
#endif
            IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
        {
            return value = Load<T>(new FileStream(path, FileMode.Open));
        }

        /// <summary>
        ///   Gets a lazily-instantiated array that can be used to load 
        ///   matrices of the specified type from a stream.
        /// </summary>
        /// 
        /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
        /// <param name="stream">The stream containing the matrix to be loaded.</param>
        /// <param name="value">The object to be read. This parameter can be used to avoid the
        ///   need of specifying a generic argument to this function.</param>
        /// 
        /// <returns>The array to be returned.</returns>
        /// 
        public static NpzDictionary<T> Load<T>(Stream stream, out NpzDictionary<T> value)
            where T : class,
#if !NETSTANDARD1_4
            ICloneable,
#endif
            IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
        {
            return value = Load<T>(stream);
        }


        /// <summary>
        ///   Gets a lazily-instantiated array that can be used to load 
        ///   matrices of the specified type from a file in the disk.
        /// </summary>
        /// 
        /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
        /// <param name="path">The path to the file containing the matrix to be loaded.</param>
        /// 
        /// <returns>The array to be returned.</returns>
        /// 
        public static NpzDictionary<T> Load<T>(string path)
            where T : class,
#if !NETSTANDARD1_4
            ICloneable,
#endif
            IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
        {
            return Load<T>(new FileStream(path, FileMode.Open));
        }

        /// <summary>
        ///   Gets a lazily-instantiated array that can be used to load 
        ///   matrices of the specified type from a stream.
        /// </summary>
        /// 
        /// <typeparam name="T">The type to be loaded from the npy-formatted file.</typeparam>
        /// <param name="stream">The stream containing the matrix to be loaded.</param>
        /// 
        /// <returns>The array to be returned.</returns>
        /// 
        public static NpzDictionary<T> Load<T>(Stream stream)
            where T : class,
#if !NETSTANDARD1_4
            ICloneable,
#endif
            IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
        {
            return new NpzDictionary<T>(stream);
        }


        /// <summary>
        ///   Gets a lazily-instantiated array that can be used to load 
        ///   multi-dimensional matrices from an array of bytes.
        /// </summary>
        /// 
        /// <param name="bytes">The bytes that contain the matrix to be loaded.</param>
        /// 
        /// <returns>A collection of arrays.</returns>
        /// 
        public static NpzDictionary<Array> LoadMatrix(byte[] bytes)
        {
            return LoadMatrix(new MemoryStream(bytes));
        }

        /// <summary>
        ///   Gets a lazily-instantiated array that can be used to load 
        ///   multi-dimensional matrices from a file in the disk.
        /// </summary>
        /// 
        /// <param name="path">The path to the file containing the matrix to be loaded.</param>
        /// 
        /// <returns>A collection of arrays.</returns>
        /// 
        public static NpzDictionary<Array> LoadMatrix(string path)
        {
            return LoadMatrix(new FileStream(path, FileMode.Open));
        }

        /// <summary>
        ///   Gets a lazily-instantiated array that can be used to load 
        ///   multi-dimensional matrices from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream containing the matrix to be loaded.</param>
        /// 
        /// <returns>A collection of arrays.</returns>
        /// 
        public static NpzDictionary<Array> LoadMatrix(Stream stream)
        {
            return new NpzDictionary(stream, jagged: false);
        }


        /// <summary>
        ///   Gets a lazily-instantiated array that can be used to load 
        ///   jagged matrices from an array of bytes.
        /// </summary>
        /// 
        /// <param name="bytes">The bytes that contain the matrix to be loaded.</param>
        /// 
        /// <returns>A collection of arrays.</returns>
        /// 
        public static NpzDictionary<Array> LoadJagged(byte[] bytes)
        {
            return LoadJagged(new MemoryStream(bytes));
        }

        /// <summary>
        ///   Gets a lazily-instantiated array that can be used to load 
        ///   jagged matrices from a file in the disk.
        /// </summary>
        /// 
        /// <param name="path">The path to the file containing the matrix to be loaded.</param>
        /// 
        /// <returns>A collection of arrays.</returns>
        /// 
        public static NpzDictionary<Array> LoadJagged(string path)
        {
            return LoadJagged(new FileStream(path, FileMode.Open));
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
        public static NpzDictionary<Array> LoadJagged(Stream stream, bool trim = true)
        {
            return new NpzDictionary(stream, jagged: true);
        }

    }
}
#endif