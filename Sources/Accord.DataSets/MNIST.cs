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
    ///   MNIST Dataset of handwritten digits.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The MNIST database (Mixed National Institute of Standards and Technology database) is a large database
    ///   of handwritten digits that is commonly used for training various image processing systems. The database 
    ///   is also widely used for training and testing in the field of machine learning. It was created by
    ///   "re-mixing" the samples from NIST's original datasets. The creators felt that since NIST's training 
    ///   dataset was taken from American Census Bureau employees, while the testing dataset was taken from 
    ///   American high school students, it was not well-suited for machine learning experiments. Furthermore, 
    ///   the black and white images from NIST were normalized to fit into a 20x20 pixel bounding box and 
    ///   anti-aliased, which introduced grayscale levels.</para>
    ///   
    /// <para>
    ///   The MNIST database contains 60,000 training images and 10,000 testing images. Half of the training set 
    ///   and half of the test set were taken from NIST's training dataset, while the other half of the training
    ///   set and the other half of the test set were taken from NIST's testing dataset. There have been a number 
    ///   of scientific papers on attempts to achieve the lowest error rate; one paper, using a hierarchical system 
    ///   of convolutional neural networks, manages to get an error rate on the MNIST database of 0.23 percent. The 
    ///   original creators of the database keep a list of some of the methods tested on it. In their original paper,
    ///   they use a support vector machine to get an error rate of 0.8 percent.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/MNIST_database">
    ///       Wikipedia contributors. "MNIST database." Wikipedia, The Free Encyclopedia. 
    ///       Wikipedia, The Free Encyclopedia, 14 Nov. 2016. Web. 14 Nov. 2016. </a>
    ///       </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to download and instantiate the MNIST dataset using the <see cref="MNIST"/> 
    ///   class, and then how to use it to learn a learn multi-label (one-vs-rest) support vector machine. Being only 
    ///   a linear machine, it achieves an error rate of ~0.85%.</para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachine\MultilabelSupportVectorLearningTest.cs" region="doc_learn_mnist" />
    /// </example>   
    /// 
    /// <seealso cref="Accord.DataSets.Base.SparseDataSet" />
    /// 
    public class MNIST : SparseDataSet
    {
        /// <summary>
        /// Gets the training set of the MNIST dataset.
        /// </summary>
        /// 
        public Tuple<Sparse<double>[], double[]> Training { get; private set; }

        /// <summary>
        /// Gets the testing set of the MNIST dataset.
        /// </summary>
        /// 
        public Tuple<Sparse<double>[], double[]> Testing { get; private set; }

        /// <summary>
        ///   Downloads and prepares the MNIST dataset.
        /// </summary>
        /// 
        /// <param name="path">The path where datasets will be stored. If null or empty, the dataset
        /// will be saved on a subfolder called "data" in the current working directory.</param>
        /// 
        public MNIST(string path = null)
            : base(path)
        {
            Training = Download("https://www.csie.ntu.edu.tw/~cjlin/libsvmtools/datasets/multiclass/mnist.bz2");
            Testing = Download("https://www.csie.ntu.edu.tw/~cjlin/libsvmtools/datasets/multiclass/mnist.t.bz2");
        }
        
    }
}
