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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Runtime.InteropServices;
    using Accord.Math;
    using Accord.Compat;

    /// <summary>
    ///   Node object contained in <see cref="MatReader">.MAT file</see>. 
    ///   A node can contain a matrix object, a string, or another nodes.
    /// </summary>
    /// 
    public class MatNode : IEnumerable<MatNode>
    {
        long startOffset;
        long matrixOffset;

        Dictionary<string, MatNode> contents;

        private BinaryReader reader;
        private MatReader matReader;

        private bool compressed;
        private MatDataType matType;
        private int typeSize;
        private int length;
        private int bytes;
        private Type type;
        private object value;
        private int[] dimensions;

        private int readBytes;

        /// <summary>
        ///   Gets the name of this node.
        /// </summary>
        /// 
        public string Name { get; private set; }

        /// <summary>
        ///   Gets the child nodes contained at this node.
        /// </summary>
        /// 
        public Dictionary<string, MatNode> Fields
        {
            get { return contents; }
        }

        /// <summary>
        ///   Gets the object value contained at this node, if any. 
        ///   Its type can be known by checking the <see cref="Type"/>
        ///   property of this node.
        /// </summary>
        /// 
        public Object Value
        {
            get
            {
                if (value == null && type != null)
                    value = read();
                return value;
            }
        }

        /// <summary>
        ///   Gets the type of the object value contained in this node.
        /// </summary>
        /// 
        public Type ValueType
        {
            get { return Value.GetType(); }
        }

        /// <summary>
        ///   Gets the object value contained at this node, if any. 
        ///   Its type can be known by checking the <see cref="Type"/>
        ///   property of this node.
        /// </summary>
        /// 
        /// <typeparam name="T">The object type, if known.</typeparam>
        /// 
        /// <returns>The object stored at this node.</returns>
        /// 
        public T GetValue<T>()
        {
            if (Value is T)
                return (T)Value;

            if (typeof(T).IsArray)
            {
                var targetType = typeof(T).GetElementType();
                Array src = Value as Array;
                Array dst = Array.CreateInstance(targetType, dimensions);

                if (matReader.Transpose)
                    dst = dst.Transpose();

                foreach (int[] idx in src.GetIndices())
                    dst.SetValue(Convert.ChangeType(src.GetValue(idx), targetType), idx);

                return (T)Convert.ChangeType(dst, typeof(T));
            }

            throw new InvalidCastException();
        }

        /// <summary>
        ///   Gets the number of child objects contained in this node.
        /// </summary>
        /// 
        public int Count
        {
            get { return contents.Count; }
        }

        /// <summary>
        ///   Gets the child fields contained under the given name.
        /// </summary>
        /// 
        /// <param name="name">The name of the field to be retrieved.</param>
        /// 
        public MatNode this[string name]
        {
            get { return contents[name]; }
        }

        /// <summary>
        ///   Gets the child fields contained under the given name.
        /// </summary>
        /// 
        /// <param name="name">The name of the field to be retrieved.</param>
        /// 
        public MatNode this[int name]
        {
            get { return contents[name.ToString()]; }
        }


        internal unsafe MatNode(MatReader matReader, BinaryReader reader, long offset, MatDataTag tag, bool lazy)
        {
            // TODO: Completely refactor this method.
            this.matReader = matReader;

            // int originalBytes = tag.NumberOfBytes;
            contents = new Dictionary<string, MatNode>();

            this.startOffset = offset;
            this.reader = reader;

            if (tag.DataType == MatDataType.miCOMPRESSED)
            {
                compressed = true;

                // Read zlib's streams with Deflate using a little trick
                // http://george.chiramattel.com/blog/2007/09/deflatestream-block-length-does-not-match.html

                reader.ReadBytes(2); // ignore zlib headers

                reader = new BinaryReader(new DeflateStream(reader.BaseStream,
                    CompressionMode.Decompress, true));

                readBytes += 8;
                if (!reader.Read(out tag))
                    throw new NotSupportedException("Invalid reader at position " + readBytes + ".");
            }

            if (tag.DataType != MatDataType.miMATRIX)
                throw new NotSupportedException("Unexpected data type at position " + readBytes + ".");

            readBytes += 8;
            MatDataTag flagsTag;
            if (!reader.Read(out flagsTag))
                throw new NotSupportedException("Invalid flags tag at position " + readBytes + ".");

            if (flagsTag.DataType != MatDataType.miUINT32)
                throw new NotSupportedException("Unexpected flags data type at position " + readBytes + ".");

            readBytes += 8;

            ArrayFlags flagsElement;
            if (!reader.Read(out flagsElement))
                throw new NotSupportedException("Invalid flags element at position " + readBytes + ".");

            if (flagsElement.Class == MatArrayType.mxOBJECT_CLASS)
                throw new NotSupportedException("Unexpected object class flag at position " + readBytes + ".");



            readBytes += 8;
            MatDataTag dimensionsTag;
            if (!reader.Read(out dimensionsTag))
                throw new NotSupportedException("Invalid dimensions tag at position " + readBytes + ".");

            if (dimensionsTag.DataType != MatDataType.miINT32)
                throw new NotSupportedException("Invalid dimensions data type at position " + readBytes + ".");

            int numberOfDimensions = (int)dimensionsTag.NumberOfBytes / 4;
            dimensions = new int[numberOfDimensions];
            for (int i = dimensions.Length - 1; i >= 0; i--)
                dimensions[i] = reader.ReadInt32();

            readBytes += dimensions.Length * 4;

            readBytes += 8;
            MatDataTag nameTag;
            if (!reader.Read(out nameTag))
                throw new NotSupportedException("Invalid name tag at position " + readBytes + ".");

            if (nameTag.DataType != MatDataType.miINT8)
                throw new NotSupportedException("Invalid name data type at position " + readBytes + ".");

            if (nameTag.IsSmallFormat)
            {
#if NETSTANDARD1_4
                Name = new String((char*)nameTag.SmallData_Value, 0, nameTag.SmallData_NumberOfBytes);
#else
                Name = new String((sbyte*)nameTag.SmallData_Value, 0, nameTag.SmallData_NumberOfBytes);
#endif
            }
            else
            {
                readBytes += nameTag.NumberOfBytes;
                Name = new String(reader.ReadChars((int)nameTag.NumberOfBytes));
                align(reader, nameTag.NumberOfBytes);
            }

            Name = Name.Trim();

            if (flagsElement.Class == MatArrayType.mxSPARSE_CLASS)
            {
                readBytes += 8;
                MatDataTag irTag;
                if (!reader.Read(out irTag))
                    throw new NotSupportedException("Invalid sparse row tag at position " + readBytes + ".");

                // read ir
                int[] ir = new int[irTag.NumberOfBytes / 4];
                for (int i = 0; i < ir.Length; i++)
                    ir[i] = reader.ReadInt32();
                align(reader, irTag.NumberOfBytes);

                readBytes += 8;
                MatDataTag icTag;
                if (!reader.Read(out icTag))
                    throw new NotSupportedException("Invalid sparse column tag at position " + readBytes + ".");

                // read ic
                int[] ic = new int[icTag.NumberOfBytes / 4];
                for (int i = 0; i < ic.Length; i++)
                    ic[i] = reader.ReadInt32();
                align(reader, icTag.NumberOfBytes);


                // read values
                readBytes += 8;
                MatDataTag valuesTag;
                if (!reader.Read(out valuesTag))
                    throw new NotSupportedException("Invalid values tag at position " + readBytes + ".");

                MatDataType matType = valuesTag.DataType;
                type = MatReader.Translate(matType);
#pragma warning disable 618 // SizeOf would be Obsolete
                typeSize = Marshal.SizeOf(type);
#pragma warning restore 618 // SizeOf would be Obsolete
                length = valuesTag.NumberOfBytes / typeSize;
                bytes = valuesTag.NumberOfBytes;

                byte[] rawData = reader.ReadBytes(bytes);
                align(reader, rawData.Length);

                if (matType == MatDataType.miINT64 || matType == MatDataType.miUINT64)
                {
                    for (int i = 7; i < rawData.Length; i += 8)
                    {
                        byte b = rawData[i];
                        bool bit = (b & (1 << 6)) != 0;
                        if (bit)
                            rawData[i] |= 1 << 7;
                        else
                            rawData[i] = (byte)(b & ~(1 << 7));
                    }
                }

                Array array = Array.CreateInstance(type, length);
                Buffer.BlockCopy(rawData, 0, array, 0, rawData.Length);
                value = new MatSparse(ir, ic, array);
            }
            else if (flagsElement.Class == MatArrayType.mxCELL_CLASS)
            {
                int readBytes2 = 0;
                int toRead = tag.NumberOfBytes - readBytes;
                int cellI = 0;

                while (readBytes2 < toRead)
                {
                    // Read first MAT data element
                    MatDataTag elementTag;
                    if (!reader.Read(out elementTag))
                        throw new NotSupportedException("Invalid element tag at position " + readBytes + ".");

                    // Create a new node from the current position
                    MatNode node = new MatNode(matReader, reader, offset, elementTag, false);

                    node.Name = (cellI++).ToString();

                    contents.Add(node.Name, node);

                    readBytes2 += elementTag.NumberOfBytes + 8;
                }
            }
            else if (flagsElement.Class == MatArrayType.mxSTRUCT_CLASS)
            {
                MatDataTag fieldNameLengthTag;
                if (!reader.Read(out fieldNameLengthTag))
                    throw new NotSupportedException("Invalid struct field name length tag at position " + readBytes + ".");

                if (!fieldNameLengthTag.IsSmallFormat)
                    throw new NotSupportedException("Small format struct field name length is not supported at position " + readBytes + ".");

                int fieldNameLength = *(int*)fieldNameLengthTag.SmallData_Value;

                if (fieldNameLengthTag.DataType != MatDataType.miINT32)
                    throw new NotSupportedException("Unexpected struct field name length data type at position " + readBytes + ".");

                MatDataTag fieldNameTag;
                if (!reader.Read(out fieldNameTag))
                    throw new NotSupportedException("Invalid struct field name at position " + readBytes + ".");

                if (fieldNameTag.DataType != MatDataType.miINT8)
                    throw new NotSupportedException("Unexpected struct field name data type at position " + readBytes + ".");

                int fields = fieldNameTag.NumberOfBytes / fieldNameLength;
                string[] names = new string[fields];
                for (int i = 0; i < names.Length; i++)
                {
                    char[] charNames = reader.ReadChars(fieldNameLength);
                    int terminator = Array.IndexOf(charNames, '\0');
                    names[i] = new String(charNames, 0, terminator);
                }

                align(reader, fieldNameTag.NumberOfBytes);

                for (int i = 0; i < names.Length; i++)
                {
                    Debug.WriteLine("reading " + names[i]);

                    // Read first MAT data element
                    MatDataTag elementTag;
                    if (!reader.Read(out elementTag))
                        throw new NotSupportedException("Invalid struct element at position " + readBytes + ".");

                    if (elementTag.DataType == MatDataType.miINT32)
                        throw new NotSupportedException("Unexpected struct element data type at position " + readBytes + ".");

                    // Create a new node from the current position
                    MatNode node = new MatNode(matReader, reader, offset, elementTag, false);

                    node.Name = names[i];

                    contents.Add(node.Name, node);
                }
            }
            else
            {
                readBytes += 8;
                MatDataTag contentsTag;
                if (!reader.Read(out contentsTag))
                    throw new NotSupportedException("Invalid contents tag at position " + readBytes + ".");

                if (contentsTag.IsSmallFormat)
                {
                    matType = contentsTag.SmallData_Type;
                    if (matType == MatDataType.miUTF8)
                    {
#if NETSTANDARD1_4
                        value = new String((char*)contentsTag.SmallData_Value, 0, contentsTag.SmallData_NumberOfBytes);
#else
                        value = new String((sbyte*)contentsTag.SmallData_Value, 0, contentsTag.SmallData_NumberOfBytes);
#endif
                    }
                    else
                    {
                        type = MatReader.Translate(matType);
#pragma warning disable 618 // SizeOf would be Obsolete
                        typeSize = Marshal.SizeOf(type);
#pragma warning restore 618 // SizeOf would be Obsolete
                        length = 1;
                        for (int i = 0; i < dimensions.Length; i++)
                            length *= dimensions[i];
                        var array = Array.CreateInstance(type, dimensions);
                        byte[] rawData = new byte[4];
                        for (int i = 0; i < rawData.Length; i++)
                            rawData[i] = contentsTag.SmallData_Value[i];
                        Buffer.BlockCopy(rawData, 0, array, 0, length);

                        if (matReader.Transpose)
                            array = array.Transpose();

                        value = array;
                    }
                }
                else
                {
                    matType = contentsTag.DataType;
                    if (matType == MatDataType.miMATRIX)
                    {
                        // Create a new node from the current position
                        value = new MatNode(matReader, reader, offset, contentsTag, false);
                    }
                    else if (matType == MatDataType.miUTF8)
                    {
                        char[] utf8 = reader.ReadChars(contentsTag.NumberOfBytes);
                        value = new String(utf8);
                        align(reader, utf8.Length);
                    }
                    else
                    {
                        type = MatReader.Translate(matType);
#pragma warning disable 618 // SizeOf would be Obsolete
                        typeSize = Marshal.SizeOf(type);
#pragma warning restore 618 // SizeOf would be Obsolete
                        length = contentsTag.NumberOfBytes / typeSize;
                        bytes = contentsTag.NumberOfBytes;

                        if (!lazy)
                            value = read(reader);
                    }
                }
            }

            if (!compressed && lazy)
                matrixOffset = reader.BaseStream.Position;
        }

        private static void align(BinaryReader reader, int rreadBytes)
        {
            int mod = rreadBytes % 8;
            if (mod != 0) // need to be 8 bytes aligned
                reader.ReadBytes(8 - mod);
        }

        private object read()
        {
            var reader = this.reader;

            if (compressed)
            {
                reader.BaseStream.Seek(startOffset + 8 + 2, SeekOrigin.Begin);
                reader = new BinaryReader(new DeflateStream(reader.BaseStream,
                   CompressionMode.Decompress, true));
                reader.ReadBytes(readBytes);
            }
            else
            {
                reader.BaseStream.Seek(matrixOffset, SeekOrigin.Begin);
            }

            Array array = read(reader);

            return array;
        }

        private Array read(BinaryReader reader)
        {
            byte[] rawData = reader.ReadBytes(bytes);
            align(reader, rawData.Length);

            if (matType == MatDataType.miINT64 || matType == MatDataType.miUINT64)
            {
                for (int i = 7; i < rawData.Length; i += 8)
                {
                    byte b = rawData[i];
                    bool bit = (b & (1 << 6)) != 0;
                    if (bit)
                        rawData[i] |= 1 << 7;
                    else
                        rawData[i] = (byte)(b & ~(1 << 7));
                }
            }

            Array array = Array.CreateInstance(type, dimensions);
            Buffer.BlockCopy(rawData, 0, array, 0, rawData.Length);
            if (matReader.Transpose)
                array = array.Transpose();
            return array;
        }



        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<MatNode> GetEnumerator()
        {
            return contents.Values.GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return contents.Values.GetEnumerator();
        }

    }
}
