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
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    /// <summary>
    ///   Provides static methods to save and load files saved in LibSVM's sparse format. 
    /// </summary>
    /// 
    public static class SparseFormat
    {
        const SerializerCompression DEFAULT_COMPRESSION = SerializerCompression.None;

        /// <summary>
        ///   Loads an array of sparse samples from a file in the disk.
        /// </summary>
        /// 
        /// <param name="path">The path to the file containing the samples to be loaded.</param>
        /// <param name="samples">The samples that have been read from the file.</param>
        /// <param name="outputs">The output labels associated with each sample in <paramref name="samples"/>.</param>
        /// <param name="compression">The type of compression to use. Default is None.</param>
        /// 
        public static void Load(string path, out Sparse<double>[] samples, out bool[] outputs, SerializerCompression compression = DEFAULT_COMPRESSION)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
                Load(stream, out samples, out outputs, compression);
        }

        /// <summary>
        ///   Loads an array of sparse samples from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from where the file will be loaded.</param>
        /// <param name="samples">The samples that have been read from the file.</param>
        /// <param name="outputs">The output labels associated with each sample in <paramref name="samples"/>.</param>
        /// <param name="compression">The type of compression to use. Default is None.</param>
        /// 
        public static void Load(Stream stream, out Sparse<double>[] samples, out bool[] outputs, SerializerCompression compression = DEFAULT_COMPRESSION)
        {
            if (compression == SerializerCompression.GZip)
            {
                using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
                {
                    Load(gzip, out samples, out outputs, compression = SerializerCompression.None);
                    return;
                }
            }

            using (SparseReader reader = new SparseReader(stream))
                reader.ReadToEnd(out samples, out outputs);
        }






        /// <summary>
        ///   Saves an array of sparse samples to a file in the disk.
        /// </summary>
        /// 
        /// <param name="path">The disk path under which the file will be saved.</param>
        /// <param name="samples">The samples that have been read from the file.</param>
        /// <param name="outputs">The output labels associated with each sample in <paramref name="samples"/>.</param>
        /// <param name="compression">The type of compression to use. Default is None.</param>
        /// 
        public static void Save(Sparse<double>[] samples, bool[] outputs, string path, SerializerCompression compression = DEFAULT_COMPRESSION)
        {
            using (Stream stream = new FileStream(path, FileMode.Create))
                Save(samples, outputs, stream, compression);
        }

        /// <summary>
        ///   Saves an array of dense samples to a file in the disk.
        /// </summary>
        /// 
        /// <param name="path">The disk path under which the file will be saved.</param>
        /// <param name="samples">The samples that have been read from the file.</param>
        /// <param name="outputs">The output labels associated with each sample in <paramref name="samples"/>.</param>
        /// <param name="compression">The type of compression to use. Default is None.</param>
        /// 
        public static void Save(double[][] samples, bool[] outputs, string path, SerializerCompression compression = DEFAULT_COMPRESSION)
        {
            using (Stream stream = new FileStream(path, FileMode.Create))
                Save(samples, outputs, stream, compression);
        }




        /// <summary>
        ///   Saves an array of sparse samples to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream where the file will be saved.</param>
        /// <param name="samples">The samples that have been read from the file.</param>
        /// <param name="outputs">The output labels associated with each sample in <paramref name="samples"/>.</param>
        /// <param name="compression">The type of compression to use. Default is None.</param>
        /// 
        public static void Save(Sparse<double>[] samples, bool[] outputs, Stream stream, SerializerCompression compression = DEFAULT_COMPRESSION)
        {
            if (compression == SerializerCompression.GZip)
            {
                using (var gzip = new GZipStream(stream, CompressionMode.Compress))
                {
                    Save(samples, outputs, gzip, SerializerCompression.None);
                    return;
                }
            }

            using (SparseWriter writer = new SparseWriter(stream))
                writer.Write(samples, outputs);
        }

        /// <summary>
        ///   Saves an array of dense samples to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream where the file will be saved.</param>
        /// <param name="samples">The samples that have been read from the file.</param>
        /// <param name="outputs">The output labels associated with each sample in <paramref name="samples"/>.</param>
        /// <param name="compression">The type of compression to use. Default is None.</param>
        /// 
        public static void Save(double[][] samples, bool[] outputs, Stream stream, SerializerCompression compression = DEFAULT_COMPRESSION)
        {
            if (compression == SerializerCompression.GZip)
            {
                using (var gzip = new GZipStream(stream, CompressionMode.Compress))
                {
                    Save(samples, outputs, gzip, SerializerCompression.None);
                    return;
                }
            }

            using (SparseWriter writer = new SparseWriter(stream))
                writer.Write(samples, outputs);
        }
    }
}
