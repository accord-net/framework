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
    using System;
    using System.IO;
    using System.Net;
    using ICSharpCode.SharpZipLib.BZip2;
    using ICSharpCode.SharpZipLib.GZip;
    using Accord.Compat;

    /// <summary>
    ///   Base class for sparse datasets that can be downloaded from LIBSVM website.
    /// </summary>
    /// 
    [Serializable]
    public abstract class WebDataSet
    {
        /// <summary>
        /// Gets the path to the directory where the datasets will be stored.
        /// </summary>
        /// 
        /// <value>The path to a directory.</value>
        /// 
        public string Path { get; protected set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="WebDataSet"/> class.
        /// </summary>
        /// 
        /// <param name="path">The path where datasets will be stored. If null or empty, the dataset
        ///   will be saved on a subfolder called "data" in the current working directory.</param>
        /// 
        protected WebDataSet(string path)
        {
            this.Path = path;

            if (String.IsNullOrEmpty(path))
                this.Path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "data");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDataSet" /> class.
        /// </summary>
        /// 
        protected WebDataSet()
        {

        }

        /// <summary>
        ///   Downloads a file from the specified <paramref name="url"/>, 
        ///   storing in <paramref name="path"/>, under name <paramref name="uncompressedFileName"/>.
        /// </summary>
        /// 
        /// <param name="url">The URL where the file should be downloaded from.</param>
        /// <param name="path">The path where the file will be stored localy.</param>
        /// <param name="uncompressedFileName">The generated name of the uncompressed file.</param>
        /// 
        /// <returns><c>true</c> if the download succeeded, <c>false</c> otherwise.</returns>
        /// 
        public static bool Download(string url, string path, out string uncompressedFileName)
        {
            string name = System.IO.Path.GetFileName(url);
            string downloadedFileName = System.IO.Path.Combine(path, name);

            if (!File.Exists(downloadedFileName))
            {
                Directory.CreateDirectory(path);

                using (var client = new WebClient())
                    client.DownloadFile(url, downloadedFileName);
            }


            // If the file is compressed, decompress it to disk
            if (endsWith(downloadedFileName, ".bz2"))
            {
                uncompressedFileName = downloadedFileName.Remove(downloadedFileName.Length - 4);
                if (!File.Exists(uncompressedFileName))
                {
                    using (var compressedFile = new FileStream(downloadedFileName, FileMode.Open))
                    using (var uncompressedFile = new FileStream(uncompressedFileName, FileMode.CreateNew))
                    {
                        BZip2.Decompress(compressedFile, uncompressedFile, false);
                    }
                }
            }
            else if (endsWith(downloadedFileName, ".gz"))
            {
                uncompressedFileName = downloadedFileName.Remove(downloadedFileName.Length - 3);
                if (!File.Exists(uncompressedFileName))
                {
                    using (var compressedFile = new FileStream(downloadedFileName, FileMode.Open))
                    using (var decompressedFile = new GZipInputStream(compressedFile))
                    using (var uncompressedFile = new FileStream(uncompressedFileName, FileMode.CreateNew))
                    {
                        decompressedFile.CopyTo(uncompressedFile);
                    }
                }
            }
            else if (endsWith(downloadedFileName, ".Z"))
            {
                uncompressedFileName = downloadedFileName.Remove(downloadedFileName.Length - 2);
                if (!File.Exists(uncompressedFileName))
                {
                    using (var compressedFile = new FileStream(downloadedFileName, FileMode.Open))
                    using (var decompressedFile = new Accord.IO.Compression.LzwInputStream(compressedFile))
                    using (var uncompressedFile = new FileStream(uncompressedFileName, FileMode.CreateNew))
                    {
                        decompressedFile.CopyTo(uncompressedFile);
                    }
                }
            }
            else
            {
                uncompressedFileName = downloadedFileName;
            }

            return true;
        }

        private static bool endsWith(string str, string value)
        {
#if NETSTANDARD1_4
            return str.EndsWith(value, StringComparison.OrdinalIgnoreCase);
#else
            return str.EndsWith(value, StringComparison.InvariantCultureIgnoreCase);
#endif
        }
    }
}
