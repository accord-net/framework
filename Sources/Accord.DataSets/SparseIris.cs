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

    /// <summary>
    ///   Fisher's Iris flower data set.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Iris flower data set or Fisher's Iris data set is a multivariate data set 
    ///   introduced by Ronald Fisher in his 1936 paper The use of multiple measurements 
    ///   in taxonomic problems as an example of linear discriminant analysis. It is
    ///   sometimes called Anderson's Iris data set because Edgar Anderson collected the 
    ///   data to quantify the morphologic variation of Iris flowers of three related species.
    ///   Two of the three species were collected in the Gaspé Peninsula "all from the same
    ///   pasture, and picked on the same day and measured at the same time by the same person
    ///   with the same apparatus".</para>
    ///   
    /// <para>
    ///   The data set consists of 50 samples from each of three species of Iris(Iris setosa, 
    ///   Iris virginica and Iris versicolor). Four features were measured from each sample: 
    ///   the length and the width of the sepals and petals, in centimetres. Based on the 
    ///   combination of these four features, Fisher developed a linear discriminant model to
    ///   distinguish the species from each other.</para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Iris_flower_data_set">
    ///       Wikipedia contributors. "Iris flower data set." Wikipedia, The Free Encyclopedia. 
    ///       Wikipedia, The Free Encyclopedia, 14 Nov. 2016. Web. 14 Nov. 2016. </a>
    ///       </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="Accord.DataSets.MNIST" />
    /// <seealso cref="Accord.DataSets.Base.SparseDataSet" />
    /// 
    public class SparseIris : SparseDataSet
    {

        /// <summary>
        ///   Gets the data instances contained in Fisher's Iris flower dataset.
        /// </summary>
        /// 
        public Sparse<double>[] Instances { get; private set; }

        /// <summary>
        ///   Gets the class labels associated with each <see cref="Instances">instance</see>
        ///   in Fisher's Iris flower dataset.
        /// </summary>
        /// 
        public int[] ClassLabels { get; private set; }

        /// <summary>
        ///   Downloads and prepares the Iris dataset.
        /// </summary>
        /// 
        /// <param name="path">The path where datasets will be stored. If null or empty, the dataset
        /// will be saved on a subfolder called "data" in the current working directory.</param>
        /// 
        public SparseIris(string path = null)
            : base(path)
        {
            var tuples = Download("https://www.csie.ntu.edu.tw/~cjlin/libsvmtools/datasets/multiclass/iris.scale");

            Instances = tuples.Item1;

            ClassLabels = new int[tuples.Item2.Length];
            for (int i = 0; i < ClassLabels.Length; i++)
                ClassLabels[i] = (int)tuples.Item2[i];
        }

    }
}
