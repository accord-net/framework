// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    ///   The MATLAB file format is fully documented at 
    ///   http://www.mathworks.com/help/pdf_doc/matlab/matfile_format.pdf
    /// </remarks>
    /// 
    public class MatReader : IDisposable
    {
        private BinaryReader reader;

        private Dictionary<string, MatNode> contents;

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
        {
            long startOffset = input.Position;
            reader = new BinaryReader(input);

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
                MatNode node = new MatNode(reader, offset, elementTag, true);

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
