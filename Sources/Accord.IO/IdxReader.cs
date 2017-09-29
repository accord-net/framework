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
    using System.IO;
    using System.IO.Compression;
    using System.Runtime.InteropServices;

    /// <summary>
    ///   Data types which can be contained in a IDX data file.
    /// </summary>
    /// 
    /// <seealso cref="IdxReader"/>
    /// 
    public enum IdxDataType : byte
    {
        /// <summary>
        ///   byte (0x08)
        /// </summary>
        /// 
        UnsignedByte = 0x08,

        /// <summary>
        ///  sbyte (0x09)
        /// </summary>
        /// 
        SignedByte = 0x09,

        /// <summary>
        ///  short (0x0B)
        /// </summary>
        /// 
        Short = 0x0B,

        /// <summary>
        ///   int (0x0C)
        /// </summary>
        /// 
        Integer = 0x0C,

        /// <summary>
        ///   float (0x0D)
        /// </summary>
        /// 
        Float = 0x0D,

        /// <summary>
        ///   double (0x0E)
        /// </summary>
        /// 
        Double = 0x0E
    }

    /// <summary>
    ///   Reader for IDX files (such as MNIST's digit database).
    /// </summary>
    /// 
    public class IdxReader : IDisposable
    {

        private BinaryReader reader;

        /// <summary>
        ///   MNIST's magic number. See remarks for more details
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   The magic number is an integer (MSB first). The first 2 bytes 
        ///   are always 0. The third byte codes the type of the data. The 
        ///   4-th byte codes the number of dimensions of the vector/matrix:
        ///   1 for vectors, 2 for matrices.
        /// </para>
        /// 
        /// <list type="table">
        ///   <listheader>
        ///     <term>Code</term>
        ///     <description>Meaning</description>
        ///   </listheader>
        ///   <item>
        ///     <term>0x08</term><description>unsigned byte</description></item>
        ///   <item>
        ///     <term>0x09</term><description>signed byte</description></item>
        ///   <item>
        ///     <term>0x0B</term><description>short (2 bytes)</description></item>
        ///   <item>
        ///     <term>0x0C</term><description>int (4 bytes)</description></item>
        ///   <item>
        ///     <term>0x0D</term><description>float (4 bytes)</description></item>
        ///   <item>
        ///     <term>0x0E</term><description>double (8 bytes)</description></item>
        ///   </list>
        /// </remarks>
        /// 
        public int Magic { get; private set; }

        /// <summary>
        ///   Gets the type of the data stored in this file.
        /// </summary>
        /// 
        public IdxDataType DataType { get; private set; }

        /// <summary>
        ///   Gets the number of dimensions for the samples.
        /// </summary>
        /// 
        public int[] Dimensions { get; private set; }

        /// <summary>
        ///   Gets the number of samples stored in this file.
        /// </summary>
        /// 
        public int Samples { get; private set; }

        /// <summary>
        ///   Returns the underlying stream.
        /// </summary>
        /// 
        public Stream BaseStream
        {
            get { return reader.BaseStream; }
        }


        Type type;
        int dataSize;
        byte[] buffer;
        int arrayLength;



        /// <summary>
        ///   Creates a new <see cref="IdxReader"/>.
        /// </summary>
        /// 
        /// <param name="path">The path for the IDX file.</param>
        /// 
        public IdxReader(string path)
        {
            init(new FileStream(path, FileMode.Open, FileAccess.Read),
                path.EndsWith(".gz", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///   Creates a new <see cref="IdxReader"/>.
        /// </summary>
        /// 
        /// <param name="path">The path for the IDX file.</param>
        /// <param name="compressed">
        ///   Pass <c>true</c> if the stream contains 
        ///   a compressed (.gz) file. Default is true.</param>
        /// 
        public IdxReader(string path, bool compressed = true)
        {
            init(new FileStream(path, FileMode.Open, FileAccess.Read), compressed);
        }

        /// <summary>
        ///   Creates a new <see cref="IdxReader"/>.
        /// </summary>
        /// 
        /// <param name="file">The byte array representing the contents of the IDX file.</param>
        /// <param name="compressed">
        ///   Pass <c>true</c> if the stream contains 
        ///   a compressed (.gz) file. Default is true.</param>
        /// 
        public IdxReader(byte[] file, bool compressed = true)
        {
            init(new MemoryStream(file), compressed);
        }

        /// <summary>
        ///   Creates a new <see cref="IdxReader"/>.
        /// </summary>
        /// 
        /// <param name="input">The input stream containing the IDX file.</param>
        /// <param name="compressed">
        ///   Pass <c>true</c> if the stream contains 
        ///   a compressed (.gz) file. Default is true.</param>
        /// 
        public IdxReader(Stream input, bool compressed = true)
        {
            init(input, compressed);
        }

        private void init(Stream input, bool compressed)
        {
            if (compressed)
                reader = new BinaryReader(new GZipStream(input, CompressionMode.Decompress));
            else reader = new BinaryReader(input);

            Magic = reader.ReadInt32();

            byte[] bytes = BitConverter.GetBytes(Magic);

            short zero = BitConverter.ToInt16(bytes, 0);
            byte type = bytes[2];
            byte dimensions = bytes[3];

            if (zero != 0) // The first two bytes should be always zero
            {
                throw new FormatException("Magic number doesn't starts with zero."
                 + " If the file is compressed, please make sure to call this constructor"
                 + " with the 'compressed' argument set to 'true'.");
            }


            DataType = (IdxDataType)type;
            this.type = Translate(DataType);

            Samples = reverseBytes(reader.ReadInt32());

            Dimensions = new int[dimensions - 1];
            for (int i = 0; i < Dimensions.Length; i++)
                Dimensions[i] = reverseBytes(reader.ReadInt32());

            arrayLength = 1;
            for (int i = 0; i < Dimensions.Length; i++)
                arrayLength *= Dimensions[i];

            this.dataSize = Marshal.SizeOf(type);
            this.buffer = new byte[dataSize * arrayLength];
        }

        /// <summary>
        ///   Reads the next sample into the given array.
        /// </summary>
        /// 
        /// <param name="array">The array to contain the samples.</param>
        /// 
        /// <returns>How many bytes were read.</returns>
        /// 
        public int Read(Array array)
        {
            int readBytes = reader.Read(buffer, 0, buffer.Length);
            Buffer.BlockCopy(buffer, 0, array, 0, array.Length);

            return readBytes;
        }

        /// <summary>
        ///   Reads the next sample as a value.
        /// </summary>
        /// 
        /// <returns>A single number containing the sample.</returns>
        /// 
        public object ReadValue()
        {
            Array array = Array.CreateInstance(type, arrayLength);

            if (Read(array) == 0)
                return null;

            return array.GetValue(0);
        }

        /// <summary>
        ///   Reads the next sample as a vector.
        /// </summary>
        /// 
        /// <returns>A unidimensional array containing the sample.</returns>
        /// 
        public Array ReadVector()
        {
            Array array = Array.CreateInstance(type, arrayLength);

            if (Read(array) == 0)
                return null;

            return array;
        }

        /// <summary>
        ///   Reads the next sample as a matrix.
        /// </summary>
        /// 
        /// <returns>A multidimensional array containing the sample.</returns>
        /// 
        public Array ReadMatrix()
        {
            Array array = Array.CreateInstance(type, Dimensions);

            if (Read(array) == 0)
                return null;

            return array;
        }

        /// <summary>
        ///   Reads the next sample as a value.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type to be used.</typeparam>
        /// 
        /// <returns>A single number containing the sample.</returns>
        /// 
        public bool TryReadValue<T>(out T value)
        {
            value = default(T);

            object v = ReadValue();

            if (v == null)
                return false;

            if (type != typeof(T))
            {
                value = (T)Convert.ChangeType(v, typeof(T));
                return true;
            }

            value = (T)v;
            return true;
        }

        /// <summary>
        ///   Reads the next sample as a vector.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type to be used.</typeparam>
        /// 
        /// <returns>A unidimensional array containing the sample.</returns>
        /// 
        public T[] ReadVector<T>()
        {
            Array array = ReadVector();

            if (array == null)
                return null;

            if (type != typeof(T))
            {
                T[] result = new T[array.Length];

                for (int i = 0; i < result.Length; i++)
                    result[i] = (T)Convert.ChangeType(array.GetValue(i), typeof(T));

                return result;
            }

            return (T[])array;
        }

        /// <summary>
        ///   Reads the next sample as a matrix.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type to be used.</typeparam>
        /// 
        /// <returns>A multidimensional array containing the sample.</returns>
        /// 
        public T[,] ReadMatrix<T>()
        {
            Array array = ReadMatrix();

            if (array == null)
                return null;

            if (type != typeof(T))
            {
                int rows = array.GetLength(0);
                int cols = array.GetLength(1);
                T[,] result = new T[rows, cols];

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        result[i, j] = (T)Convert.ChangeType(array.GetValue(i, j), typeof(T));

                return result;
            }

            return (T[,])array;
        }

        /// <summary>
        ///   Reads all samples in the file, starting from the current position, as matrices.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type to be used.</typeparam>
        /// 
        /// <returns>
        ///   An array containing all samples from the current point until the end of the stream.
        /// </returns>
        /// 
        public T[][,] ReadToEndAsMatrices<T>()
        {
            List<T[,]> matrices = new List<T[,]>();

            T[,] current = ReadMatrix<T>();

            while (current != null)
            {
                matrices.Add(current);
                current = ReadMatrix<T>();
            }

            return matrices.ToArray();
        }

        /// <summary>
        ///   Reads all samples in the file, starting from the current position, as vectors.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type to be used.</typeparam>
        /// 
        /// <returns>
        ///   An array containing all samples from the current point until the end of the stream.
        /// </returns>
        /// 
        public T[][] ReadToEndAsVectors<T>()
        {
            List<T[]> vectors = new List<T[]>();

            T[] current = ReadVector<T>();

            while (current != null)
            {
                vectors.Add(current);
                current = ReadVector<T>();
            }

            return vectors.ToArray();
        }

        /// <summary>
        ///   Reads all samples in the file, starting from the current position, as vectors.
        /// </summary>
        /// 
        /// <typeparam name="T">The data type to be used.</typeparam>
        /// 
        /// <returns>
        ///   An array containing all samples from the current point until the end of the stream.
        /// </returns>
        /// 
        public T[] ReadToEndAsValues<T>()
        {
            List<T> vectors = new List<T>();

            T current;

            while (TryReadValue(out current))
            {
                vectors.Add(current);
            }

            return vectors.ToArray();
        }

        private static int reverseBytes(int val)
        {
            return (val & 0x000000FF) << 24 |
                   (val & 0x0000FF00) << 8 |
                   (val & 0x00FF0000) >> 8 |
             ((int)(val & 0xFF000000)) >> 24;
        }

        /// <summary>
        ///   Translates the given <see cref="IdxDataType"/> to a .NET <see cref="Type"/>.
        /// </summary>
        /// 
        /// <param name="type">The type to be translated.</param>
        /// 
        /// <returns>
        ///   A .NET <see cref="Type"/> that represents the <see cref="IdxDataType"/>.
        /// </returns>
        /// 
        public static Type Translate(IdxDataType type)
        {
            switch (type)
            {
                case IdxDataType.UnsignedByte:
                    return typeof(byte);
                case IdxDataType.SignedByte:
                    return typeof(sbyte);
                case IdxDataType.Short:
                    return typeof(short);
                case IdxDataType.Integer:
                    return typeof(int);
                case IdxDataType.Float:
                    return typeof(float);
                case IdxDataType.Double:
                    return typeof(double);
            }

            throw new ArgumentOutOfRangeException("type");
        }


        #region IDisposable members
        /// <summary>
        ///   Performs application-defined tasks associated with
        ///   freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// 
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged
        ///   resources; <c>false</c> to release only unmanaged resources.</param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (reader != null)
                {
#if !NETSTANDARD1_4
                    reader.Close();
#endif
                    reader = null;
                }
            }
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref="IdxReader"/> is reclaimed by garbage collection.
        /// </summary>
        /// 
        ~IdxReader()
        {
            Dispose(false);
        }
        #endregion


    }
}
