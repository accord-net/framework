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

namespace Accord
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Compat;
    using Accord.IO;
    using System.Data;
    using Accord.Collections;
    using System.Runtime.CompilerServices;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///   Static class for utility extension methods.
    /// </summary>
    /// 
    public static class ExtensionMethods
    {
#if !NETSTANDARD1_4
        /// <summary>
        ///   Creates and adds multiple <see cref="System.Data.DataColumn"/>
        ///   objects with the given names at once.
        /// </summary>
        /// 
        /// <param name="collection">The <see cref="System.Data.DataColumnCollection"/>
        /// to add in.</param>
        /// <param name="columnNames">The names of the <see cref="System.Data.DataColumn"/> to
        /// be created and added.</param>
        /// 
        /// <example>
        ///   <code>
        ///   DataTable table = new DataTable();
        ///   
        ///   // Add multiple columns at once:
        ///   table.Columns.Add("columnName1", "columnName2");
        ///   </code>
        /// </example>
        /// 
        public static void Add(this DataColumnCollection collection, params string[] columnNames)
        {
            for (int i = 0; i < columnNames.Length; i++)
                collection.Add(columnNames[i]);
        }

        /// <summary>
        ///   Creates and adds multiple <see cref="System.Data.DataColumn"/>
        ///   objects with the given names at once.
        /// </summary>
        /// 
        /// <param name="collection">The <see cref="System.Data.DataColumnCollection"/>
        ///   to add in.</param>
        /// <param name="columns">The names of the <see cref="System.Data.DataColumn"/>s to
        ///   be created and added, alongside with their types.</param>
        /// 
        /// <example>
        ///   <code>
        ///   DataTable table = new DataTable();
        ///   
        ///   // Add multiple columns at once:
        ///   table.Columns.Add(new OrderedDictionary&gt;String, Type&lt;()
        ///   {
        ///       { "columnName1", typeof(int)    },
        ///       { "columnName2", typeof(double) },
        ///   });
        ///   </code>
        /// </example>
        /// 
        public static void Add(this DataColumnCollection collection, OrderedDictionary<string, Type> columns)
        {
            foreach (var pair in columns)
                collection.Add(pair.Key, pair.Value);
        }
#endif

        /// <summary>
        ///   Gets a the value of a <see cref="DescriptionAttribute"/>
        ///   associated with a particular enumeration value.
        /// </summary>
        /// 
        /// <typeparam name="T">The enumeration type.</typeparam>
        /// <param name="source">The enumeration value.</param>
        /// 
        /// <returns>The string value stored in the value's description attribute.</returns>
        /// 
        public static string GetDescription<T>(this T source)
        {
#if NETSTANDARD1_4
            FieldInfo fi = source.GetType().GetRuntimeField(source.ToString());
#else
            FieldInfo fi = source.GetType().GetField(source.ToString());
#endif

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;

            return source.ToString();
        }

        /// <summary>
        ///   Reads a <c>struct</c> from a stream.
        /// </summary>
        /// 
        public static bool Read<T>(this BinaryReader stream, out T structure)
            where T : struct
        {
            var type = typeof(T);

#pragma warning disable CS0618 // Type or member is obsolete
            int size = Marshal.SizeOf(type);
#pragma warning restore CS0618 // Type or member is obsolete
            byte[] buffer = new byte[size];
            if (stream.Read(buffer, 0, buffer.Length) == 0)
            {
                structure = default(T);
                return false;
            }

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
#pragma warning disable CS0618 // Type or member is obsolete
            structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
#pragma warning restore CS0618 // Type or member is obsolete
            handle.Free();

            return true;
        }


        /// <summary>
        ///   Reads a <c>struct</c> from a stream.
        /// </summary>
        /// 
        public static bool Write<T>(this BinaryWriter stream, T[] array)
            where T : struct
        {
            var type = typeof(T);
#pragma warning disable CS0618 // Type or member is obsolete
            int size = Marshal.SizeOf(type);
#pragma warning restore CS0618 // Type or member is obsolete
            byte[] buffer = new byte[size * array.Length];

            Buffer.BlockCopy(array, 0, buffer, 0, buffer.Length);
            stream.Write(buffer, 0, buffer.Length);

            return true;
        }

        /// <summary>
        ///   Reads a <c>struct</c> from a stream.
        /// </summary>
        /// 
        public static bool Write<T>(this BinaryWriter stream, T[][] array)
            where T : struct
        {
            var type = typeof(T);
#pragma warning disable CS0618 // Type or member is obsolete
            int size = Marshal.SizeOf(type);
#pragma warning restore CS0618 // Type or member is obsolete
            byte[] buffer = new byte[size * array[0].Length];

            for (int i = 0; i < array.Length; i++)
            {
                Buffer.BlockCopy(array[i], 0, buffer, 0, buffer.Length);
                stream.Write(buffer, 0, buffer.Length);
            }

            return true;
        }

        /// <summary>
        ///   Reads a <c>struct</c> from a stream.
        /// </summary>
        /// 
        public static bool Write<T>(this BinaryWriter stream, T[,] array)
            where T : struct
        {
            var type = typeof(T);
#pragma warning disable CS0618 // Type or member is obsolete
            int size = Marshal.SizeOf(type);
#pragma warning restore CS0618 // Type or member is obsolete
            byte[] buffer = new byte[size * array.Length];

            Buffer.BlockCopy(array, 0, buffer, 0, buffer.Length);
            stream.Write(buffer, 0, buffer.Length);

            return true;
        }

        /// <summary>
        ///   Reads a <c>struct</c> from a stream.
        /// </summary>
        /// 
        public static T[][] ReadJagged<T>(this BinaryReader stream, int rows, int columns)
            where T : struct
        {
            var type = typeof(T);
#pragma warning disable CS0618 // Type or member is obsolete
            int size = Marshal.SizeOf(type);
#pragma warning restore CS0618 // Type or member is obsolete
            var buffer = new byte[size * columns];
            T[][] matrix = new T[rows][];
            for (int i = 0; i < matrix.Length; i++)
                matrix[i] = new T[columns];

            for (int i = 0; i < matrix.Length; i++)
            {
                stream.Read(buffer, 0, buffer.Length);
                Buffer.BlockCopy(buffer, 0, matrix[i], 0, buffer.Length);
            }

            return matrix;
        }

        /// <summary>
        ///   Reads a <c>struct</c> from a stream.
        /// </summary>
        /// 
        public static T[,] ReadMatrix<T>(this BinaryReader stream, int rows, int columns)
            where T : struct
        {
            return (T[,])ReadMatrix(stream, typeof(T), rows, columns);
        }

        /// <summary>
        ///   Reads a <c>struct</c> from a stream.
        /// </summary>
        /// 
        public static Array ReadMatrix(this BinaryReader stream, Type type, params int[] lengths)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            int size = Marshal.SizeOf(type);
#pragma warning restore CS0618 // Type or member is obsolete
            int total = 1;
            for (int i = 0; i < lengths.Length; i++)
                total *= lengths[i];
            var buffer = new byte[size * total];
            var matrix = Array.CreateInstance(type, lengths);

            stream.Read(buffer, 0, buffer.Length);
            Buffer.BlockCopy(buffer, 0, matrix, 0, buffer.Length);

            return matrix;
        }

        /// <summary>
        ///   Gets the underlying buffer position for a StreamReader.
        /// </summary>
        /// 
        /// <param name="reader">A StreamReader whose position will be retrieved.</param>
        /// 
        /// <returns>The current offset from the beginning of the 
        ///   file that the StreamReader is currently located into.</returns>
        /// 
        public static long GetPosition(this StreamReader reader)
        {
            // http://stackoverflow.com/a/17457085/262032

#if NETSTANDARD1_4 || NETSTANDARD2_0
            var type = typeof(StreamReader).GetTypeInfo();
            char[] charBuffer = (char[])type.GetDeclaredField("_charBuffer").GetValue(reader);
            int charPos = (int)type.GetDeclaredField("_charPos").GetValue(reader);
            int byteLen = (int)type.GetDeclaredField("_byteLen").GetValue(reader);
#else
            var type = typeof(StreamReader);

            char[] charBuffer;
            int charPos;
            int byteLen;

            if (SystemTools.IsRunningOnMono() && type.GetField("decoded_buffer") != null)
            {
                // Mono's StreamReader source code is at: https://searchcode.com/codesearch/view/26576619/

                // The current buffer of decoded characters
                charBuffer = (char[])GetField(reader, "decoded_buffer");

                // The current position in the buffer of decoded characters
                charPos = (int)GetField(reader, "decoded_count");

                // The number of encoded bytes that are in the current buffer
                byteLen = (int)GetField(reader, "buffer_size");
            }
            else
            {
                // The current buffer of decoded characters
                charBuffer = (char[])GetField(reader, "charBuffer");

                // The current position in the buffer of decoded characters
                charPos = (int)GetField(reader, "charPos");

                // The number of encoded bytes that are in the current buffer
                byteLen = (int)GetField(reader, "byteLen");
            }
#endif

            // The number of bytes that the already-read characters need when encoded.
            int numReadBytes = reader.CurrentEncoding.GetByteCount(charBuffer, 0, charPos);

            return reader.BaseStream.Position - byteLen + numReadBytes;
        }

#if !NETSTANDARD1_4

        private static object GetField(StreamReader reader, string name)
        {
            return typeof(StreamReader).InvokeMember(name,
                BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance |
                BindingFlags.GetField, null, reader, null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///   Deserializes the specified stream into an object graph, but locates
        ///   types by searching all loaded assemblies and ignoring their versions.
        /// </summary>
        /// 
        /// <param name="formatter">The binary formatter.</param>
        /// <param name="stream">The stream from which to deserialize the object graph.</param>
        /// 
        /// <returns>The top (root) of the object graph.</returns>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Load<T>() instead.")]
        public static T DeserializeAnyVersion<T>(this BinaryFormatter formatter, Stream stream)
        {
            return Serializer.Load<T>(stream);
        }
#endif

        /// <summary>
        ///   Converts an object into another type, irrespective of whether
        ///   the conversion can be done at compile time or not. This can be
        ///   used to convert generic types to numeric types during runtime.
        /// </summary>
        /// 
        /// <typeparam name="T">The destination type.</typeparam>
        /// 
        /// <param name="value">The value to be converted.</param>
        /// 
        /// <returns>The result of the conversion.</returns>
        /// 
        public static T To<T>(this object value)
        {
            return (T)To(value, typeof(T));
        }

        /// <summary>
        ///   Converts an object into another type, irrespective of whether
        ///   the conversion can be done at compile time or not. This can be
        ///   used to convert generic types to numeric types during runtime.
        /// </summary>
        /// 
        /// <param name="value">The value to be converted.</param>
        /// <param name="type">The type that the value should be converted to.</param>
        /// 
        /// <returns>The result of the conversion.</returns>
        /// 
        public static object To(this object value, Type type)
        {
            if (value == null)
                return System.Convert.ChangeType(null, type);

            if (type.IsInstanceOfType(value))
                return value;

#if NETSTANDARD
            if (type.GetTypeInfo().IsEnum)
#else
            if (type.IsEnum)
#endif
                return Enum.ToObject(type, (int)System.Convert.ChangeType(value, typeof(int)));

            Type inputType = value.GetType();

#if NETSTANDARD
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
#else
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
#endif
            {
                MethodInfo setter = type.GetMethod("op_Implicit", new[] { inputType });
                return setter.Invoke(null, new object[] { value });
            }

            var methods = new List<MethodInfo>();
            methods.AddRange(inputType.GetMethods(BindingFlags.Public | BindingFlags.Static));
            methods.AddRange(type.GetMethods(BindingFlags.Public | BindingFlags.Static));

            foreach (MethodInfo m in methods)
            {
                if (m.IsPublic && m.IsStatic)
                {
                    if ((m.Name == "op_Implicit" || m.Name == "op_Explicit") && m.ReturnType == type)
                    {
                        ParameterInfo[] p = m.GetParameters();
                        if (p.Length == 1 && p[0].ParameterType.IsInstanceOfType(value))
                            return m.Invoke(null, new[] { value });
                    }
                }
            }

            //if (value is IConvertible)
            //    return System.Convert.ChangeType(value, type);

            return System.Convert.ChangeType(value, type);
        }

        /// <summary>
        /// Gets the type of the element in a jagged or multi-dimensional matrix.
        /// </summary>
        /// 
        /// <param name="type">The array type whose element type should be computed.</param>
        /// 
        public static Type GetInnerMostType(this Type type)
        {
            while (type.IsArray)
                type = type.GetElementType();

            return type;
        }

        /// <summary>
        ///   Determines whether the given type has a public default (parameterless) constructor.
        /// </summary>
        /// 
        /// <param name="t">The type to check.</param>
        /// 
        /// <returns>True if the type has a public parameterless constructor; false otherwise.</returns>
        /// 
        public static bool HasDefaultConstructor(this Type t)
        {
#if NETSTANDARD1_4
            var info = t.GetTypeInfo();
            if (info.IsValueType)
                return true;

            ConstructorInfo ctors = info.DeclaredConstructors.Where(x => x.GetParameters().Length == 0).FirstOrDefault();
            return ctors != null;
#else
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
#endif
        }


        /// <summary>
        ///   Replaces the format item in a specified string with the string
        ///   representation of a corresponding object in a specified array.
        /// </summary>
        /// 
        /// <param name="str">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// 
        /// <returns>
        ///   A copy of str in which the format items have been replaced by
        ///   the string representation of the corresponding objects in args.
        /// </returns>
        /// 
        public static string Format(this string str, params object[] args)
        {
            return String.Format(str, args);
        }

#if !NETSTANDARD1_4

        /// <summary>
        ///   Checks whether two dictionaries have the same contents.
        /// </summary>
        /// 
        public static bool IsEqual<TKey, TValue>(this IDictionary<TKey, TValue> a, IDictionary<TKey, TValue> b)
        {
            if (a == b)
                return true;

            if (a.Count != b.Count)
                return false;

            var aKeys = new HashSet<TKey>(a.Keys);
            var bKeys = new HashSet<TKey>(b.Keys);
            if (!aKeys.SetEquals(bKeys))
                return false;

            if (HasMethod<TValue>("SetEquals"))
            {
                var setEquals = typeof(TValue).GetMethod("SetEquals");
                foreach (var k in aKeys)
                {
                    if (!(bool)setEquals.Invoke(a[k], new object[] { b[k] }))
                        return false;
                }
            }
            else
            {
                foreach (var k in aKeys)
                    if (!a[k].Equals(b[k]))
                        return false;
            }
            return true;
        }

        /// <summary>
        ///   Checks whether an object implements a method with the given name.
        /// </summary>
        /// 
        public static bool HasMethod(this object obj, string methodName)
        {
            try
            {
                var type = obj.GetType();
                return type.GetMethod(methodName) != null;
            }
            catch (AmbiguousMatchException)
            {
                return true;
            }
        }

        /// <summary>
        ///   Checks whether a type implements a method with the given name.
        /// </summary>
        /// 
        public static bool HasMethod<T>(string methodName)
        {
            try
            {
                var type = typeof(T);
                return type.GetMethod(methodName) != null;
            }
            catch (AmbiguousMatchException)
            {
                return true;
            }
        }
#endif


        /// <summary>
        ///   Determines whether <c>a > b</c>.
        /// </summary>
        /// 
        public static bool IsGreaterThan<T>(this T a, object b)
            where T : IComparable
        {
            return a.CompareTo(b) > 0;
        }

        /// <summary>
        ///   Determines whether <c>a >= b</c>.
        /// </summary>
        /// 
        public static bool IsGreaterThanOrEqual<T>(this T a, object b)
            where T : IComparable
        {
            return a.CompareTo(b) >= 0;
        }

        /// <summary>
        ///   Determines whether <c>a &lt; b</c>.
        /// </summary>
        /// 
        public static bool IsLessThan<T>(this T a, object b)
            where T : IComparable
        {
            return a.CompareTo(b) < 0;
        }

        /// <summary>
        ///   Determines whether <c>a &lt;= b</c>.
        /// </summary>
        /// 
        public static bool IsLessThanOrEqual<T>(this T a, object b)
            where T : IComparable
        {
            return a.CompareTo(b) <= 0;
        }

#if !NETSTANDARD1_4
        /// <summary>
        ///   Gets the default value for a type. This method should serve as
        ///   a programmatic equivalent to <c>default(T)</c>.
        /// </summary>
        /// 
        /// <param name="type">The type whose default value should be retrieved.</param>
        /// 
        public static object GetDefaultValue(this Type type)
        {
#if NETSTANDARD2_0
            if (type.GetTypeInfo().IsValueType)
#else
            if (type.IsValueType)
#endif
                return Activator.CreateInstance(type);
            return null;
        }
#endif

#if !NETSTANDARD1_4
        /// <summary>
        ///   Retrieves the memory address of a generic value type.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the object whose address needs to be retrieved.</typeparam>
        /// <param name="t">The object those address needs to be retrieved.</param>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static System.IntPtr AddressOf<T>(this T t)
        {
            unsafe
            {
                System.TypedReference reference = __makeref(t);
                return *(System.IntPtr*)(&reference);
            }
        }

        /// <summary>
        ///   Retrieves the memory address of a generic reference type.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the object whose address needs to be retrieved.</typeparam>
        /// <param name="t">The object those address needs to be retrieved.</param>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        static System.IntPtr AddressOfRef<T>(ref T t)
        {
            unsafe
            {
                System.TypedReference reference = __makeref(t);
                System.TypedReference* pRef = &reference;
                return (System.IntPtr)pRef; //(&pRef)
            }
        }
#endif

        // TODO: Move this method to a more appropriate location
        internal static WebClient NewWebClient()
        {
            var webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) (Accord.NET Framework)");
            return webClient;
        }

        /// <summary>
        ///   Attempts to download a file from the web multiple times before giving up.
        /// </summary>
        /// 
        /// <param name="client">The web client to use.</param>
        /// <param name="url">The URL of the file to be downloaded.</param>
        /// <param name="fileName">The disk location where the file should be stored.</param>
        /// <param name="maxAttempts">The maximum number of attempts.</param>
        /// <param name="overwrite">Do not overwrite <paramref name="fileName"/> if it already exists.</param>
        /// 
        internal static void DownloadFileWithRetry(this WebClient client, string url, string fileName, int maxAttempts = 3, bool overwrite = false)
        {
            if (!overwrite && File.Exists(fileName))
                return;

            for (int numberOfAttempts = 1; numberOfAttempts <= maxAttempts; numberOfAttempts++)
            {
                Console.WriteLine("Downloading {0} (#{1})", url, numberOfAttempts);
                try
                {
                    client.DownloadFile(url, fileName);
                    break;
                }
                catch (WebException)
                {
                    int milliseconds = numberOfAttempts * 2000;
#if NETSTANDARD1_4
                    Task.Delay(milliseconds).Wait();
#else
                    Thread.Sleep(milliseconds);
#endif
                    if (numberOfAttempts == maxAttempts)
                        throw;
                }
            }
        }



        /// <summary>
        ///  Serializes (converts) a structure to a byte array.
        /// </summary>
        /// 
        /// <param name="value">The structure to be serialized.</param>
        /// <returns>The byte array containing the serialized structure.</returns>
        /// 
        public static byte[] ToByteArray<T>(this T value)
            where T : struct
        {
            int rawsize = Marshal.SizeOf(value);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            Marshal.StructureToPtr(value, buffer, false);
            handle.Free();
            return rawdata;
        }

        /// <summary>
        ///   Deserializes (converts) a byte array to a given structure type.
        /// </summary>
        /// 
        /// <remarks>
        ///  This is a potentiality unsafe operation.
        /// </remarks>
        /// 
        /// <param name="rawData">The byte array containing the serialized object.</param>
        /// <param name="position">The starting position in the rawData array where the object is located.</param>
        /// <returns>The object stored in the byte array.</returns>
        /// 
        public static T ToStruct<T>(this byte[] rawData, int position = 0)
            where T : struct
        {
            Type type = typeof(T);

#pragma warning disable CS0618 // Type or member is obsolete
            int rawsize = Marshal.SizeOf(type);
#pragma warning restore CS0618 // Type or member is obsolete

            if (rawsize > (rawData.Length - position))
                throw new ArgumentException("The given array is smaller than the object size.");

            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawData, position, buffer, rawsize);
#pragma warning disable CS0618 // Type or member is obsolete
            T obj = (T)Marshal.PtrToStructure(buffer, type);
#pragma warning restore CS0618 // Type or member is obsolete
            Marshal.FreeHGlobal(buffer);
            return obj;
        }

        /// <summary>
        ///   Returns a type object representing an array of the current type, with the specified number of dimensions.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="rank">The rank.</param>
        /// <param name="jagged">Whether to return a type for a jagged array of the given rank, or a 
        /// multdimensional array. Default is false (default is to return multidimensional array types).</param>
        /// 
        public static Type MakeArrayType(this Type elementType, int rank, bool jagged)
        {
            if (!jagged)
                return elementType.MakeArrayType(rank);

            if (rank == 0)
                return elementType;

            return elementType.MakeArrayType(rank: rank - 1, jagged: true).MakeArrayType();
        }
    }
}
