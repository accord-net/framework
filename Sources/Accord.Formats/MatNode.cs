using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Accord.IO
{
    public class MatNode
    {
        long startOffset;
        long matrixOffset;

        Dictionary<string, MatNode> contents;

        private BinaryReader reader;

        private bool compressed;
        private MatDataType matType;
        private int typeSize;
        private int length;
        private int bytes;
        private Type type;
        private object value;
        private int[] dimensions;

        private int readBytes;

        public Dictionary<string, MatNode> Values { get { return contents; } }

        public string Name { get; private set; }

        public Object Value
        {
            get
            {
                if (value == null && type != null)
                    value = read();
                return value;
            }
        }


        internal unsafe MatNode(BinaryReader reader, long offset)
        {
            contents = new Dictionary<string, MatNode>();
            this.startOffset = offset;
        }

        internal unsafe MatNode(BinaryReader reader, long offset, MatDataTag tag, bool lazy)
        {
            int originalBytes = tag.NumberOfBytes;
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
                    throw new Exception();
            }

            if (tag.DataType != MatDataType.miMATRIX)
                throw new Exception();

            readBytes += 8;
            MatDataTag flagsTag;
            if (!reader.Read(out flagsTag))
                throw new Exception();

            if (flagsTag.DataType != MatDataType.miUINT32)
                throw new Exception();

            readBytes += 8;

            ArrayFlags flagsElement;
            if (!reader.Read(out flagsElement))
                throw new Exception();

            if (flagsElement.NonZeroElements != 0)
            {
                // Sparse matrices will not be supported right now
                //reader.ReadBytes(tag.NumberOfBytes - readBytes);
                //return;
            }

            if (flagsElement.Class == MatArrayType.mxOBJECT_CLASS)
                throw new Exception();



            readBytes += 8;
            MatDataTag dimensionsTag;
            if (!reader.Read(out dimensionsTag))
                throw new Exception();

            if (dimensionsTag.DataType != MatDataType.miINT32)
                throw new Exception();

            int numberOfDimensions = (int)dimensionsTag.NumberOfBytes / 4;
            dimensions = new int[numberOfDimensions];
            for (int i = dimensions.Length - 1; i >= 0; i--)
                dimensions[i] = reader.ReadInt32();

            readBytes += dimensions.Length * 4;

            readBytes += 8;
            MatDataTag nameTag;
            if (!reader.Read(out nameTag))
                throw new Exception();

            if (nameTag.DataType != MatDataType.miINT8)
                throw new Exception();

            if (nameTag.IsSmallFormat)
            {
                Name = new String((sbyte*)nameTag.SmallData_Value, 0, nameTag.SmallData_NumberOfBytes);
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
                    throw new Exception();

                // read ir
                int[] ir = new int[irTag.NumberOfBytes / 4];
                for (int i = 0; i < ir.Length; i++)
                    ir[i] = reader.ReadInt32();
                align(reader, irTag.NumberOfBytes);

                readBytes += 8;
                MatDataTag icTag;
                if (!reader.Read(out icTag))
                    throw new Exception();

                // read ic
                int[] ic = new int[icTag.NumberOfBytes / 4];
                for (int i = 0; i < ic.Length; i++)
                    ic[i] = reader.ReadInt32();
                align(reader, icTag.NumberOfBytes);


                // read values
                readBytes += 8;
                MatDataTag valuesTag;
                if (!reader.Read(out valuesTag))
                    throw new Exception();

                MatDataType matType = valuesTag.DataType;
                type = MatReader.Translate(matType);
                typeSize = Marshal.SizeOf(type);
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
                value = Tuple.Create(ir, ic, array);
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
                        throw new Exception();

                    // Create a new node from the current position
                    MatNode node = new MatNode(reader, offset, elementTag, false);

                    node.Name = (cellI++).ToString();

                    contents.Add(node.Name, node);

                    readBytes2 += elementTag.NumberOfBytes + 8;
                }
            }
            else if (flagsElement.Class == MatArrayType.mxSTRUCT_CLASS)
            {
                MatDataTag fieldNameLengthTag;
                if (!reader.Read(out fieldNameLengthTag))
                    throw new Exception();

                if (!fieldNameLengthTag.IsSmallFormat)
                    throw new Exception();

                int fieldNameLength = *(int*)fieldNameLengthTag.SmallData_Value;

                if (fieldNameLengthTag.DataType != MatDataType.miINT32)
                    throw new Exception();

                MatDataTag fieldNameTag;
                if (!reader.Read(out fieldNameTag))
                    throw new Exception();

                if (fieldNameTag.DataType != MatDataType.miINT8)
                    throw new Exception();

                int fields = fieldNameTag.NumberOfBytes / fieldNameLength;
                string[] names = new string[fields];
                for (int i = 0; i < names.Length; i++)
                {
                    char[] charNames = reader.ReadChars(fieldNameLength);
                    int terminator = Array.IndexOf(charNames, '\0');
                    names[i] = new String(charNames, 0, terminator);
                }

                align(reader, fieldNameTag.NumberOfBytes);
                // matrixOffset = reader.BaseStream.Position;

                for (int i = 0; i < names.Length; i++)
                {
                    Debug.WriteLine("reading " + names[i]);

                    // Read first MAT data element
                    MatDataTag elementTag;
                    if (!reader.Read(out elementTag))
                        throw new Exception();

                    if (elementTag.DataType == MatDataType.miINT32)
                        throw new Exception();

                    // Create a new node from the current position
                    MatNode node = new MatNode(reader, offset, elementTag, false);

                    node.Name = names[i];

                    contents.Add(node.Name, node);
                }
            }
            else
            {
                readBytes += 8;
                MatDataTag contentsTag;
                if (!reader.Read(out contentsTag))
                    throw new Exception();

                if (contentsTag.IsSmallFormat)
                {
                    matType = contentsTag.SmallData_Type;
                    if (matType == MatDataType.miUTF8)
                    {
                        value = new String((sbyte*)contentsTag.SmallData_Value,
                            0, contentsTag.SmallData_NumberOfBytes);
                    }
                    else
                    {
                        type = MatReader.Translate(matType);
                        typeSize = Marshal.SizeOf(type);
                        length = 1;
                        for (int i = 0; i < dimensions.Length; i++)
                            length *= dimensions[i];
                        var array = Array.CreateInstance(type, dimensions);
                        byte[] rawData = new byte[4];
                        for (int i = 0; i < rawData.Length; i++)
                            rawData[i] = contentsTag.SmallData_Value[i];
                        Buffer.BlockCopy(rawData, 0, array, 0, length);
                        value = array;
                    }
                }
                else
                {
                    matType = contentsTag.DataType;
                    if (matType == MatDataType.miMATRIX)
                    {
                        // Create a new node from the current position
                        MatNode node = new MatNode(reader, offset, contentsTag, false);
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
                        typeSize = Marshal.SizeOf(type);
                        length = contentsTag.NumberOfBytes / typeSize;
                        bytes = contentsTag.NumberOfBytes;

                        if (!lazy)
                        {
                            value = read(reader);
                        }
                    }
                    //byte[] rawData = reader.ReadBytes(bytes);
                }
            }

            if (!compressed && lazy)
                matrixOffset = reader.BaseStream.Position;
        }

        unsafe private static void align(BinaryReader reader, int rreadBytes)
        {
            int mod = rreadBytes % 8;
            if (mod != 0) // need to be 8 bytes aligned
                reader.ReadBytes(8 - mod);
        }

        public MatNode this[string name]
        {
            get { return contents[name]; }
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
            return array;
        }

    }
}
