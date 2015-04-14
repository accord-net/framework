// Accord Formats Library
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

namespace Accord.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;

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
    /// <code>
    /// // Create a new MAT file reader
    /// var reader = new MatReader(file);
    /// 
    /// // Extract some basic information about the file:
    /// string description = reader.Description; // "MATLAB 5.0 MAT-file, Platform: PCWIN"
    /// int    version     = reader.Version;     // 256
    /// bool   bigEndian   = reader.BigEndian;   // false
    /// 
    /// 
    /// // Enumerate the fields in the file
    /// foreach (var field in reader.Fields)
    ///   Console.WriteLine(field.Key); // "structure"
    /// 
    /// // We have the single following field
    /// var structure = reader["structure"];
    /// 
    /// // Enumerate the fields in the structure
    /// foreach (var field in structure.Fields)
    ///   Console.WriteLine(field.Key); // "a", "string"
    /// 
    /// // Check the type for the field "a"
    /// var aType = structure["a"].Type; // byte[,]
    /// 
    /// // Retrieve the field "a" from the file
    /// var a = structure["a"].GetValue&lt;byte[,]>();
    /// 
    /// // We can also do directly if we know the type in advance
    /// var s = reader["structure"]["string"].GetValue&lt;string>();
    /// </code>
    /// </example>
    /// 
    public class MatReader : IDisposable
    {
        private BinaryReader reader;
        private bool autoTranspose;

        private Dictionary<string, MatNode> contents;

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
        /// <param name="input">The input stream containing the MAT file.</param>
        /// 
        public MatReader(Stream input)
            : this(input, true)
        {
        }

        /// <summary>
        ///   Creates a new <see cref="MatReader"/>.
        /// </summary>
        /// 
        /// <param name="input">The input stream containing the MAT file.</param>
        /// <param name="autoTranspose">Pass <c>true</c> to automatically transpose matrices if they 
        ///   have been stored differently from .NET's default row-major order. Default is <c>true</c>.</param>
        /// 
        public MatReader(Stream input, bool autoTranspose)
            : this(new BinaryReader(input), autoTranspose)
        {
        }

        /// <summary>
        ///   Creates a new <see cref="MatReader"/>.
        /// </summary>
        /// 
        /// <param name="reader">A reader for input stream containing the MAT file.</param>
        /// <param name="autoTranspose">Pass <c>true</c> to automatically transpose matrices if they 
        ///   have been stored differently from .NET's default row-major order. Default is <c>true</c>.</param>
        /// 
        public MatReader(BinaryReader reader, bool autoTranspose)
        {
            this.autoTranspose = autoTranspose;

            long startOffset = reader.BaseStream.Position;
            this.reader = reader;

            char[] title = reader.ReadChars(116);
            long subOffset = reader.ReadInt64();
            short version = reader.ReadInt16();
            char[] endian = reader.ReadChars(2);

            int terminator = Array.IndexOf(title, '\0');
            if (terminator < 0) terminator = title.Length;

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
                MatNode node = new MatNode(this, reader, offset, elementTag, true);

                // Advance the stream to the next element (might be removed in the future)
                reader.BaseStream.Seek(offset + elementTag.NumberOfBytes + 8, SeekOrigin.Begin);

                contents.Add(node.Name, node);
            }
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
                    reader.Close();
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
