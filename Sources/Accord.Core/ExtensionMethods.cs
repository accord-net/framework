﻿// Accord Core Library
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
    using Accord.IO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    ///   Static class for utility extension methods.
    /// </summary>
    /// 
    public static class ExtensionMethods
    {

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
            FieldInfo fi = source.GetType().GetField(source.ToString());

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

            int size = Marshal.SizeOf(type);
            byte[] buffer = new byte[size];
            if (stream.Read(buffer, 0, buffer.Length) == 0)
            {
                structure = default(T);
                return false;
            }

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
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
            int size = Marshal.SizeOf(type);
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
            int size = Marshal.SizeOf(type);
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
            int size = Marshal.SizeOf(type);
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
            int size = Marshal.SizeOf(type);
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
            int size = Marshal.SizeOf(type);
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

#if NETSTANDARD2_0
            var type = reader.GetType().GetTypeInfo();
            char[] charBuffer = (char[])type.GetDeclaredField("_charBuffer").GetValue(reader);
            int charPos = (int)type.GetDeclaredField("_charPos").GetValue(reader);
            int byteLen = (int)type.GetDeclaredField("_byteLen").GetValue(reader);
#else
            // The current buffer of decoded characters
            char[] charBuffer = (char[])reader.GetType().InvokeMember("charBuffer",
                BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance
                | BindingFlags.GetField, null, reader, null, CultureInfo.InvariantCulture);

            // The current position in the buffer of decoded characters
            int charPos = (int)reader.GetType().InvokeMember("charPos",
                BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance
                | BindingFlags.GetField, null, reader, null, CultureInfo.InvariantCulture);

            // The number of encoded bytes that are in the current buffer
            int byteLen = (int)reader.GetType().InvokeMember("byteLen",
                BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance
                | BindingFlags.GetField, null, reader, null, CultureInfo.InvariantCulture);
#endif

            // The number of bytes that the already-read characters need when encoded.
            int numReadBytes = reader.CurrentEncoding.GetByteCount(charBuffer, 0, charPos);

            return reader.BaseStream.Position - byteLen + numReadBytes;
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
            if (value is IConvertible)
                return (T)System.Convert.ChangeType(value, typeof(T));

            Type type = value.GetType();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var m in methods)
            {
                if ((m.Name == "op_Implicit" || m.Name == "op_Explicit") && m.ReturnType == typeof(T))
                    return (T)m.Invoke(null, new[] { value });
            }

            return (T)System.Convert.ChangeType(value, typeof(T));
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
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
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
    }
}
