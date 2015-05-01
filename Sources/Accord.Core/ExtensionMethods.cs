// Accord Core Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using System.ComponentModel;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
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
        public static bool Read<T>(this BinaryReader stream, out T structure) where T : struct
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
            return (T)System.Convert.ChangeType(value, typeof(T));
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

            // The current buffer of decoded characters
            char[] charBuffer = (char[])reader.GetType().InvokeMember("charBuffer",
                BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance
                | BindingFlags.GetField, null, reader, null, CultureInfo.InvariantCulture);

            // The current position in the buffer of decoded characters
            int charPos = (int)reader.GetType().InvokeMember("charPos",
                BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance
                | BindingFlags.GetField, null, reader, null, CultureInfo.InvariantCulture);

            // The number of bytes that the already-read characters need when encoded.
            int numReadBytes = reader.CurrentEncoding.GetByteCount(charBuffer, 0, charPos);

            // The number of encoded bytes that are in the current buffer
            int byteLen = (int)reader.GetType().InvokeMember("byteLen",
                BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance
                | BindingFlags.GetField, null, reader, null, CultureInfo.InvariantCulture);

            return reader.BaseStream.Position - byteLen + numReadBytes;
        }


        private static readonly Object lockObj = new Object();

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
        public static object DeserializeAnyVersion(this BinaryFormatter formatter, Stream stream)
        {
            lock (lockObj)
            {
                try
                {
                    AppDomain.CurrentDomain.AssemblyResolve += resolve;
                    return formatter.Deserialize(stream);
                }
                finally
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= resolve;
                }
            }
        }

        private static Assembly resolve(object sender, ResolveEventArgs args)
        {
            var display = new AssemblyName(args.Name);

            if (display.Name == args.Name)
                return null;

            return ((AppDomain)sender).Load(display.Name);
        }
    }
}
