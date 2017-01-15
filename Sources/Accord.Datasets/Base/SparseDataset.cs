// Accord Datasets Library
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

namespace Accord.DataSets.Base
{
    using Accord.Math;
    using ICSharpCode.SharpZipLib.BZip2;
    using Accord.IO;
    using System;
    using System.IO;
    using System.Net;

    /// <summary>
    ///   Base class for sparse datasets that can be downloaded from LIBSVM website.
    /// </summary>
    public abstract class SparseDataSet
    {
        /// <summary>
        /// Gets the path to the directory where the datasets will be stored.
        /// </summary>
        /// 
        /// <value>The path to a directory.</value>
        /// 
        public string Path { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseDataSet"/> class.
        /// </summary>
        /// 
        /// <param name="path">The path where datasets will be stored. If null or empty, the dataset
        ///   will be saved on a subfolder called "data" in the current working directory.</param>
        /// 
        public SparseDataSet(string path)
        {
            this.Path = path;

            if (String.IsNullOrEmpty(path))
                this.Path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "data");
        }

        /// <summary>
        /// Downloads the dataset from a specified URL, saving it to disk, and returning
        /// it as a set of <see cref="Sparse{T}">sparse vectors</see>>. If the dataset
        /// already exists in the disk, it will not be redownloaded again.
        /// </summary>
        /// 
        /// <param name="url">The URL where the dataset resides.</param>
        /// <param name="name">The name of the file to store the dataset.</param>
        /// 
        /// <returns>System.Tuple&lt;Accord.Math.Sparse&lt;System.Double&gt;[], System.Double[]&gt;.</returns>
        /// 
        protected Tuple<Sparse<double>[], double[]> Download(string url, string name)
        {
            string destination = System.IO.Path.Combine(Path, name);

            if (!File.Exists(destination))
            {
                Directory.CreateDirectory(Path);

                using (var client = new WebClient())
                    client.DownloadFile(url, destination);
            }


            if (url.EndsWith(".bz2"))
            {
                using (FileStream compressedFile = new FileStream(destination, FileMode.Open))
                using (Stream stream = new MemoryStream())
                {
                    BZip2.Decompress(compressedFile, stream, false);
                    stream.Seek(0, SeekOrigin.Begin);
                    using (var reader = new SparseReader(stream))
                        return reader.ReadSparseToEnd();
                }
            }
            else if (url.EndsWith(".gz"))
            {
                using (var compressedFile = new FileStream(destination, FileMode.Open))
                using (var reader = new SparseReader(compressedFile))
                    return reader.ReadSparseToEnd();
            }

            using (var reader = new SparseReader(destination))
                return reader.ReadSparseToEnd();
        }
    }
}
