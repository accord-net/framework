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
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO.Compression;

    public static partial class NpzFormat
    {
        const CompressionLevel DEFAULT_COMPRESSION = CompressionLevel.Fastest;

        /// <summary>
        ///   Saves the specified arrays to an array of bytes.
        /// </summary>
        /// 
        /// <param name="arrays">The arrays to be saved to the array of bytes.</param>
        /// <param name="compression">The compression level to use when compressing the array.</param>
        /// 
        /// <returns>A byte array containig the saved arrays.</returns>
        /// 
        public static byte[] Save(Dictionary<string, Array> arrays, CompressionLevel compression = DEFAULT_COMPRESSION)
        {
            using (var stream = new MemoryStream())
            {
                Save(arrays, stream, compression, leaveOpen: true);
                return stream.ToArray();
            }
        }

        /// <summary>
        ///   Saves the specified array to an array of bytes.
        /// </summary>
        /// 
        /// <param name="array">The array to be saved to the array of bytes.</param>
        /// <param name="compression">The compression level to use when compressing the array.</param>
        /// 
        /// <returns>A byte array containig the saved array.</returns>
        /// 
        public static byte[] Save(Array array, CompressionLevel compression = DEFAULT_COMPRESSION)
        {
            using (var stream = new MemoryStream())
            {
                Save(array, stream, compression, leaveOpen: true);
                return stream.ToArray();
            }
        }




        /// <summary>
        ///   Saves the specified arrays to a file in the disk.
        /// </summary>
        /// 
        /// <param name="arrays">The arrays to be saved to disk.</param>
        /// <param name="path">The disk path under which the file will be saved.</param>
        /// <param name="compression">The compression level to use when compressing the array.</param>
        /// 
        public static void Save(Dictionary<string, Array> arrays, string path, CompressionLevel compression = DEFAULT_COMPRESSION)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                Save(arrays, stream, compression);
            }
        }

        /// <summary>
        ///   Saves the specified array to a file in the disk.
        /// </summary>
        /// 
        /// <param name="array">The array to be saved to disk.</param>
        /// <param name="path">The disk path under which the file will be saved.</param>
        /// <param name="compression">The compression level to use when compressing the array.</param>
        /// 
        public static void Save(Array array, string path, CompressionLevel compression = DEFAULT_COMPRESSION)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                Save(array, stream, compression);
            }
        }


        /// <summary>
        ///   Saves the specified arrays to a file in the disk.
        /// </summary>
        /// 
        /// <param name="arrays">The arrays to be saved to disk.</param>
        /// <param name="stream">The stream to which the file will be saved.</param>
        /// <param name="compression">The compression level to use when compressing the array.</param>
        /// <param name="leaveOpen">True to leave the stream opened after the file is saved; false otherwise.</param>
        /// 
        public static void Save(Dictionary<string, Array> arrays, Stream stream, CompressionLevel compression = DEFAULT_COMPRESSION, bool leaveOpen = false)
        {
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (KeyValuePair<string, Array> p in arrays)
                {
                    var entry = zip.CreateEntry(p.Key, compression);
                    NpyFormat.Save(p.Value, entry.Open());
                }
            }
        }

        /// <summary>
        ///   Saves the specified array to a stream.
        /// </summary>
        /// 
        /// <param name="array">The array to be saved to disk.</param>
        /// <param name="stream">The stream to which the file will be saved.</param>
        /// <param name="compression">The compression level to use when compressing the array.</param>
        /// <param name="leaveOpen">True to leave the stream opened after the file is saved; false otherwise.</param>
        /// 
        public static void Save(Array array, Stream stream, CompressionLevel compression = DEFAULT_COMPRESSION, bool leaveOpen = false)
        {
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: leaveOpen))
            {
                var entry = zip.CreateEntry("arr_0");
                NpyFormat.Save(array, entry.Open());
            }
        }


    }
}
