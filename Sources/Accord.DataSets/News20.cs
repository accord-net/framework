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

namespace Accord.DataSets
{
    using Accord.DataSets.Base;
    using Accord.Math;
    using System;
    using Accord.Compat;

    /// <summary>
    ///   20 Newsgroups dataset.
    /// </summary>
    /// 
    /// <remarks>
    /// The 20 Newsgroups data set is a collection of approximately 20,000 newsgroup documents,
    /// partitioned (nearly) evenly across 20 different newsgroups. 
    /// 
    /// http://qwone.com/~jason/20Newsgroups/
    /// </remarks>
    /// 
    /// <seealso cref="Accord.DataSets.MNIST" />
    /// <seealso cref="Accord.DataSets.SparseIris" />
    /// <seealso cref="Accord.DataSets.Base.SparseDataSet" />
    /// 
    public class News20 : SparseDataSet
    {
        /// <summary>
        /// Gets the training set of the News20 dataset.
        /// </summary>
        public Tuple<Sparse<double>[], double[]> Training { get; private set; }

        /// <summary>
        /// Gets the testing set of the News20 dataset.
        /// </summary>
        public Tuple<Sparse<double>[], double[]> Testing { get; private set; }

        /// <summary>
        ///   Downloads and prepares the News20 dataset.
        /// </summary>
        /// 
        /// <param name="path">The path where datasets will be stored. If null or empty, the dataset
        /// will be saved on a subfolder called "data" in the current working directory.</param>
        /// 
        public News20(string path = null)
            : base(path)
        {
            Training = Download("https://www.csie.ntu.edu.tw/~cjlin/libsvmtools/datasets/multiclass/news20.bz2");
            Testing = Download("https://www.csie.ntu.edu.tw/~cjlin/libsvmtools/datasets/multiclass/news20.t.bz2");
        }
        
    }
}
