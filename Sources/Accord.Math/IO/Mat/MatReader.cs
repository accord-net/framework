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
    using System.Linq;

    /// <summary>
    ///   Reader for .mat files (such as the ones created by Matlab and Octave).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   MAT files are binary files containing variables and structures from mathematical 
    ///   processing programs, such as MATLAB or Octave. In MATLAB, .mat files can be created
    ///   using its <c>save</c> and <c>load</c> functions. For the moment, this reader supports
    ///   only version 5 MAT files (Matlab 5 MAT-file).</para>
    ///   
    /// <para>
    ///   The MATLAB file format is documented at 
    ///   <a href="http://www.mathworks.com/help/pdf_doc/matlab/matfile_format.pdf">
    ///   http://www.mathworks.com/help/pdf_doc/matlab/matfile_format.pdf </a></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   All the examples below involve loading files from the following URL: 
    ///   https://github.com/accord-net/framework/blob/development/Unit%20Tests/Accord.Tests.Math/Resources/mat/ </para>
    /// <para>
    ///   The first example shows how to read a simple .MAT file containing a single matrix of 
    ///   integer numbers. It also shows how to discover the names of the variables stored in
    ///   the file and how to discover their types:</para>
    /// <code source="Unit Tests\Accord.Tests.Math\IO\MatReaderTest.cs" region="doc_matrix_int32" />
    /// 
    /// <para>
    ///   The second example shows how to read a simple .MAT file containing a single matrix of 
    ///   8-bpp integer values (signed bytes):</para>
    /// <code source="Unit Tests\Accord.Tests.Math\IO\MatReaderTest.cs" region="doc_matrix_byte" />
    /// 
    /// <para>
    ///   The third example shows how to read a more complex .MAT file containing a structure. Structures
    ///   can hold complex types such as collections of matrices, lists, and strings in a nested hierarchy:</para>
    /// <code source="Unit Tests\Accord.Tests.Math\IO\MatReaderTest.cs" region="doc_structure" />
    /// 
    /// <para>
    ///   The <see cref="MatReader"/> class can also read the more complex cell array structures. However, 
    ///   there is no example of this functionality right now, 
    ///   <a href="https://github.com/accord-net/framework/blob/4b41bcdd96daf5c428081e579f37b995e600c18a/Unit%20Tests/Accord.Tests.Math/IO/MatReaderTest.cs#L355">
    ///   except for those unit tests currently in the project repository</a>. If you would like examples for this 
    ///   feature, please open a new issue at the <a href="https://github.com/accord-net/framework/issues">project's
    ///   issue tracker</a>.</para>
    /// </example>
    /// 
    public class MatReader : IDisposable
    {
        private BinaryReader reader;
        private bool autoTranspose;

        private Dictionary<string, MatNode> contents;

        /// <summary>
        ///   Gets the name of the variables contained in this file. Those
        ///   can be used as keys to the <see cref="Fields"/> property to
        ///   retrieve a variable or navigate the variable hierarchy.
        /// </summary>
        /// 
        public string[] FieldNames
        {
            get { return contents.Keys.OrderBy(x => x).ToArray(); }
        }

        /// <summary>
        ///   Gets the child nodes contained in this file.
        /// </summary>
        /// 
        public Dictionary<string, MatNode> Fields
        {
            get { return contents; }
        }

        /// <summary>
        ///   Gets the human readable description of the MAT file.
        /// </summary>
        /// 
        /// <example>
        ///   An example header description could be given by
        ///   <c>"MATLAB 5.0 MAT-file, Platform: PCWIN, Created on: Thu Feb 22 03:12:25 2007"</c>.
        /// </example>
        /// 
        public string Description { get; private set; }

        /// <summary>
        ///   Gets the version information about the file. 
        ///   This field should always contain 256.
        /// </summary>
        /// 
        public int Version { get; private set; }

        /// <summary>
        ///   Gets whether the MAT file uses the Big-Endian
        ///   standard for bit-order. Currently, only little
        ///   endian is supported.
        /// </summary>
        /// 
        public bool BigEndian { get; private set; }

        /// <summary>
        ///   Gets whether matrices will be auto-transposed 
        ///   to .NET row and column format if necessary.
        /// </summary>
        /// 
        public bool Transpose { get { return autoTranspose; } }

        /// <summary>
        ///   Returns the underlying stream.
        /// </summary>
        /// 
        public Stream BaseStream
        {
            get { return reader.BaseStream; }
        }

        /// <summary>
        ///   Gets a child object contained in this node.
        /// </summary>
        /// 
        /// <param name="key">The field name or index.</param>
        /// 
        public MatNode this[string key]
        {
            get { return contents[key]; }
        }

        /// <summary>
        ///   Gets a child object contained in this node.
        /// </summary>
        /// 
        /// <param name="key">The field index.</param>
        /// 
        public MatNode this[int key]
        {
            get { return contents[key.ToString()]; }
        }

        /// <summary>
        ///   Creates a new <see cref="MatReader"/>.
        /// </summary>
        /// 
        /// <param name="fileName">A relative or absolute path for the .MAT file.</param>
        /// <param name="autoTranspose">Pass <c>true</c> to automatically transpose matrices if they 
        ///   have been stored differently from .NET's default row-major order. Default is <c>true</c>.</param>
        /// <param name="lazy">Whether matrices should be read lazily (if set to true, only
        ///   matrices that have explicitly been asked for will be loaded).</param>
        /// 
        public MatReader(string fileName, bool autoTranspose = true, bool lazy = true)
        {
            init(new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read)), autoTranspose, lazy);
        }

        /// <summary>
        ///   Creates a new <see cref="MatReader"/>.
        /// </summary>
        /// 
        /// <param name="input">The input stream containing the MAT file.</param>
        /// <param name="autoTranspose">Pass <c>true</c> to automatically transpose matrices if they 
        ///   have been stored differently from .NET's default row-major order. Default is <c>true</c>.</param>
        /// <param name="lazy">Whether matrices should be read lazily (if set to true, only
        ///   matrices that have explicitly been asked for will be loaded).</param>
        /// 
        public MatReader(Stream input, bool autoTranspose = true, bool lazy = true)
        {
            init(new BinaryReader(input), autoTranspose, lazy);
        }

        /// <summary>
        ///   Creates a new <see cref="MatReader"/>.
        /// </summary>
        /// 
        /// <param name="input">The input stream containing the MAT file.</param>
        /// <param name="autoTranspose">Pass <c>true</c> to automatically transpose matrices if they 
        ///   have been stored differently from .NET's default row-major order. Default is <c>true</c>.</param>
        /// <param name="lazy">Whether matrices should be read lazily (if set to true, only
        ///   matrices that have explicitly been asked for will be loaded).</param>
        /// 
        public MatReader(byte[] input, bool autoTranspose = true, bool lazy = true)
        {
            init(new BinaryReader(new MemoryStream(input)), autoTranspose, lazy);
        }

        /// <summary>
        ///   Creates a new <see cref="MatReader"/>.
        /// </summary>
        /// 
        /// <param name="reader">A reader for input stream containing the MAT file.</param>
        /// <param name="autoTranspose">Pass <c>true</c> to automatically transpose matrices if they 
        ///   have been stored differently from .NET's default row-major order. Default is <c>true</c>.</param>
        /// <param name="lazy">Whether matrices should be read lazily (if set to true, only
        ///   matrices that have explicitly been asked for will be loaded).</param>
        /// 
        public MatReader(BinaryReader reader, bool autoTranspose = true, bool lazy = true)
        {
            init(reader, autoTranspose, lazy);
        }

        private void init(BinaryReader reader, bool autoTranspose, bool lazy)
        {
            this.autoTranspose = autoTranspose;

            long startOffset = reader.BaseStream.Position;
            this.reader = reader;

            char[] title = reader.ReadChars(116);
            reader.ReadInt64(); // long subOffset
            short version = reader.ReadInt16();
            char[] endian = reader.ReadChars(2);

            int terminator = Array.IndexOf(title, '\0');
            if (terminator < 0)
                terminator = title.Length;

            Description = new String(title, 0, terminator).Trim();
            Version = version;
            BigEndian = endian[0] == 'M';


            if (BitConverter.IsLittleEndian && BigEndian)
                throw new NotSupportedException("The file bit ordering differs from the system architecture.");

            contents = new Dictionary<string, MatNode>();

            while (true)
            {
                long offset = reader.BaseStream.Position;

                // Read first MAT data element
                MatDataTag elementTag;
                if (!reader.Read(out elementTag))
                    return;

                // Create a new node from the current position
                MatNode node = new MatNode(this, reader, offset, elementTag, lazy: lazy);

                // Advance the stream to the next element (might be removed in the future)
                reader.BaseStream.Seek(offset + elementTag.NumberOfBytes + 8, SeekOrigin.Begin);

                contents.Add(node.Name, node);
            }
        }

        /// <summary>
        ///   Reads an object from a given key.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the object to be read.</typeparam>
        /// <param name="key">The name of the object.</param>
        /// 
        public T Read<T>(string key)
        {
            return (T)Fields[key].Value;
        }

        /// <summary>
        ///   Reads an object from a given key.
        /// </summary>
        /// 
        /// <param name="key">The name of the object.</param>
        /// 
        public object Read(string key)
        {
            return Fields[key].Value;
        }


        internal static Type Translate(MatDataType type)
        {
            switch (type)
            {
                case MatDataType.miINT8:
                    return typeof(sbyte);
                case MatDataType.miUINT8:
                    return typeof(byte);
                case MatDataType.miINT16:
                    return typeof(short);
                case MatDataType.miUINT16:
                    return typeof(ushort);
                case MatDataType.miINT32:
                    return typeof(int);
                case MatDataType.miUINT32:
                    return typeof(uint);
                case MatDataType.miSINGLE:
                    return typeof(float);
                case MatDataType.miDOUBLE:
                    return typeof(double);
                case MatDataType.miINT64:
                    return typeof(long);
                case MatDataType.miUINT64:
                    return typeof(ulong);
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
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
        ///   <see cref="MatReader"/> is reclaimed by garbage collection.
        /// </summary>
        /// 
        ~MatReader()
        {
            Dispose(false);
        }
        #endregion

    }
}
