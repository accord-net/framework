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
    using Accord.IO;
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Base class for sparse datasets that can be downloaded from LIBSVM website.
    /// </summary>
    /// 
    [Serializable]
    public abstract class SparseDataSet : WebDataSet
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="SparseDataSet"/> class.
        /// </summary>
        /// 
        /// <param name="path">The path where datasets will be stored. If null or empty, the dataset
        ///   will be saved on a subfolder called "data" in the current working directory.</param>
        /// 
        protected SparseDataSet(string path)
            : base(path)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseDataSet" /> class.
        /// </summary>
        /// 
        protected SparseDataSet()
        {
        }

        /// <summary>
        /// Downloads the dataset from a specified URL, saving it to disk, and returning
        /// it as a set of <see cref="Sparse{T}">sparse vectors</see>>. If the dataset
        /// already exists in the disk, it will not be redownloaded again.
        /// </summary>
        /// 
        /// <param name="url">The URL where the dataset resides.</param>
        /// 
        /// <returns>System.Tuple&lt;Accord.Math.Sparse&lt;System.Double&gt;[], System.Double[]&gt;.</returns>
        /// 
        protected new Tuple<Sparse<double>[], double[]> Download(string url)
        {
            string uncompressedFileName;
            Download(url, Path, out uncompressedFileName);

            using (var reader = new SparseReader(uncompressedFileName))
                return reader.ReadSparseToEnd();
        }

    }
}
