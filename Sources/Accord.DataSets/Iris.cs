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
    /// <example>
    /// <para>
    ///   The following example shows how to download and train a classifier (SVM) in the Iris dataset:</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn_iris_confusion_matrix" />
    ///   
    /// <para>
    ///   The next example shows how to learn a mini-batch classifier for the Iris dataset:</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\AveragedStochasticGradientDescentTest.cs" region="doc_learn_multiclass" />
    /// </example>
    /// 
    /// <seealso cref="Accord.DataSets.SparseIris" />
    /// 
    public class Iris
    {
        /// <summary>
        ///   Gets the data instances contained in Fisher's Iris flower dataset.
        /// </summary>
        /// 
        public double[][] Instances { get; private set; }

        /// <summary>
        ///   Gets the class labels associated with each <see cref="Instances">instance</see>
        ///   in Fisher's Iris flower dataset.
        /// </summary>
        /// 
        public int[] ClassLabels { get; private set; }

        /// <summary>
        ///   Gets the class labels in Fisher's Iris flower dataset:
        ///   "Iris - setosa", "Iris - versicolor", and "Iris - virginica".
        /// </summary>
        /// 
        public string[] ClassNames { get; private set; }

        /// <summary>
        ///   Gets the variable names in Fisher's Iris flower dataset: 
        ///   "Sepal length", "Sepal width", "Petal length", and "Petal width".
        /// </summary>
        /// 
        public string[] VariableNames { get; private set; }

        /// <summary>
        ///   Prepares the Iris dataset.
        /// </summary>
        /// 
        public Iris()
        {
            double[,] data =
            {
                { 5.1,3.5,1.4,0.2,0 },
                { 4.9,3.0,1.4,0.2,0 },
                { 4.7,3.2,1.3,0.2,0 },
                { 4.6,3.1,1.5,0.2,0 },
                { 5.0,3.6,1.4,0.2,0 },
                { 5.4,3.9,1.7,0.4,0 },
                { 4.6,3.4,1.4,0.3,0 },
                { 5.0,3.4,1.5,0.2,0 },
                { 4.4,2.9,1.4,0.2,0 },
                { 4.9,3.1,1.5,0.1,0 },
                { 5.4,3.7,1.5,0.2,0 },
                { 4.8,3.4,1.6,0.2,0 },
                { 4.8,3.0,1.4,0.1,0 },
                { 4.3,3.0,1.1,0.1,0 },
                { 5.8,4.0,1.2,0.2,0 },
                { 5.7,4.4,1.5,0.4,0 },
                { 5.4,3.9,1.3,0.4,0 },
                { 5.1,3.5,1.4,0.3,0 },
                { 5.7,3.8,1.7,0.3,0 },
                { 5.1,3.8,1.5,0.3,0 },
                { 5.4,3.4,1.7,0.2,0 },
                { 5.1,3.7,1.5,0.4,0 },
                { 4.6,3.6,1.0,0.2,0 },
                { 5.1,3.3,1.7,0.5,0 },
                { 4.8,3.4,1.9,0.2,0 },
                { 5.0,3.0,1.6,0.2,0 },
                { 5.0,3.4,1.6,0.4,0 },
                { 5.2,3.5,1.5,0.2,0 },
                { 5.2,3.4,1.4,0.2,0 },
                { 4.7,3.2,1.6,0.2,0 },
                { 4.8,3.1,1.6,0.2,0 },
                { 5.4,3.4,1.5,0.4,0 },
                { 5.2,4.1,1.5,0.1,0 },
                { 5.5,4.2,1.4,0.2,0 },
                { 4.9,3.1,1.5,0.1,0 },
                { 5.0,3.2,1.2,0.2,0 },
                { 5.5,3.5,1.3,0.2,0 },
                { 4.9,3.1,1.5,0.1,0 },
                { 4.4,3.0,1.3,0.2,0 },
                { 5.1,3.4,1.5,0.2,0 },
                { 5.0,3.5,1.3,0.3,0 },
                { 4.5,2.3,1.3,0.3,0 },
                { 4.4,3.2,1.3,0.2,0 },
                { 5.0,3.5,1.6,0.6,0 },
                { 5.1,3.8,1.9,0.4,0 },
                { 4.8,3.0,1.4,0.3,0 },
                { 5.1,3.8,1.6,0.2,0 },
                { 4.6,3.2,1.4,0.2,0 },
                { 5.3,3.7,1.5,0.2,0 },
                { 5.0,3.3,1.4,0.2,0 },
                { 7.0,3.2,4.7,1.4,1 },
                { 6.4,3.2,4.5,1.5,1 },
                { 6.9,3.1,4.9,1.5,1 },
                { 5.5,2.3,4.0,1.3,1 },
                { 6.5,2.8,4.6,1.5,1 },
                { 5.7,2.8,4.5,1.3,1 },
                { 6.3,3.3,4.7,1.6,1 },
                { 4.9,2.4,3.3,1.0,1 },
                { 6.6,2.9,4.6,1.3,1 },
                { 5.2,2.7,3.9,1.4,1 },
                { 5.0,2.0,3.5,1.0,1 },
                { 5.9,3.0,4.2,1.5,1 },
                { 6.0,2.2,4.0,1.0,1 },
                { 6.1,2.9,4.7,1.4,1 },
                { 5.6,2.9,3.6,1.3,1 },
                { 6.7,3.1,4.4,1.4,1 },
                { 5.6,3.0,4.5,1.5,1 },
                { 5.8,2.7,4.1,1.0,1 },
                { 6.2,2.2,4.5,1.5,1 },
                { 5.6,2.5,3.9,1.1,1 },
                { 5.9,3.2,4.8,1.8,1 },
                { 6.1,2.8,4.0,1.3,1 },
                { 6.3,2.5,4.9,1.5,1 },
                { 6.1,2.8,4.7,1.2,1 },
                { 6.4,2.9,4.3,1.3,1 },
                { 6.6,3.0,4.4,1.4,1 },
                { 6.8,2.8,4.8,1.4,1 },
                { 6.7,3.0,5.0,1.7,1 },
                { 6.0,2.9,4.5,1.5,1 },
                { 5.7,2.6,3.5,1.0,1 },
                { 5.5,2.4,3.8,1.1,1 },
                { 5.5,2.4,3.7,1.0,1 },
                { 5.8,2.7,3.9,1.2,1 },
                { 6.0,2.7,5.1,1.6,1 },
                { 5.4,3.0,4.5,1.5,1 },
                { 6.0,3.4,4.5,1.6,1 },
                { 6.7,3.1,4.7,1.5,1 },
                { 6.3,2.3,4.4,1.3,1 },
                { 5.6,3.0,4.1,1.3,1 },
                { 5.5,2.5,4.0,1.3,1 },
                { 5.5,2.6,4.4,1.2,1 },
                { 6.1,3.0,4.6,1.4,1 },
                { 5.8,2.6,4.0,1.2,1 },
                { 5.0,2.3,3.3,1.0,1 },
                { 5.6,2.7,4.2,1.3,1 },
                { 5.7,3.0,4.2,1.2,1 },
                { 5.7,2.9,4.2,1.3,1 },
                { 6.2,2.9,4.3,1.3,1 },
                { 5.1,2.5,3.0,1.1,1 },
                { 5.7,2.8,4.1,1.3,1 },
                { 6.3,3.3,6.0,2.5,2 },
                { 5.8,2.7,5.1,1.9,2 },
                { 7.1,3.0,5.9,2.1,2 },
                { 6.3,2.9,5.6,1.8,2 },
                { 6.5,3.0,5.8,2.2,2 },
                { 7.6,3.0,6.6,2.1,2 },
                { 4.9,2.5,4.5,1.7,2 },
                { 7.3,2.9,6.3,1.8,2 },
                { 6.7,2.5,5.8,1.8,2 },
                { 7.2,3.6,6.1,2.5,2 },
                { 6.5,3.2,5.1,2.0,2 },
                { 6.4,2.7,5.3,1.9,2 },
                { 6.8,3.0,5.5,2.1,2 },
                { 5.7,2.5,5.0,2.0,2 },
                { 5.8,2.8,5.1,2.4,2 },
                { 6.4,3.2,5.3,2.3,2 },
                { 6.5,3.0,5.5,1.8,2 },
                { 7.7,3.8,6.7,2.2,2 },
                { 7.7,2.6,6.9,2.3,2 },
                { 6.0,2.2,5.0,1.5,2 },
                { 6.9,3.2,5.7,2.3,2 },
                { 5.6,2.8,4.9,2.0,2 },
                { 7.7,2.8,6.7,2.0,2 },
                { 6.3,2.7,4.9,1.8,2 },
                { 6.7,3.3,5.7,2.1,2 },
                { 7.2,3.2,6.0,1.8,2 },
                { 6.2,2.8,4.8,1.8,2 },
                { 6.1,3.0,4.9,1.8,2 },
                { 6.4,2.8,5.6,2.1,2 },
                { 7.2,3.0,5.8,1.6,2 },
                { 7.4,2.8,6.1,1.9,2 },
                { 7.9,3.8,6.4,2.0,2 },
                { 6.4,2.8,5.6,2.2,2 },
                { 6.3,2.8,5.1,1.5,2 },
                { 6.1,2.6,5.6,1.4,2 },
                { 7.7,3.0,6.1,2.3,2 },
                { 6.3,3.4,5.6,2.4,2 },
                { 6.4,3.1,5.5,1.8,2 },
                { 6.0,3.0,4.8,1.8,2 },
                { 6.9,3.1,5.4,2.1,2 },
                { 6.7,3.1,5.6,2.4,2 },
                { 6.9,3.1,5.1,2.3,2 },
                { 5.8,2.7,5.1,1.9,2 },
                { 6.8,3.2,5.9,2.3,2 },
                { 6.7,3.3,5.7,2.5,2 },
                { 6.7,3.0,5.2,2.3,2 },
                { 6.3,2.5,5.0,1.9,2 },
                { 6.5,3.0,5.2,2.0,2 },
                { 6.2,3.4,5.4,2.3,2 },
                { 5.9,3.0,5.1,1.8,2 },
            };

            ClassNames = new[] { "Iris - setosa", "Iris - versicolor", "Iris - virginica" };
            VariableNames = new[] { "Sepal length", "Sepal width", "Petal length", "Petal width" };

            int n = data.GetLength(0);
            int d = data.GetLength(1) - 1;
            Instances = new double[n][];
            ClassLabels = new int[n];
            for (int i = 0; i < Instances.Length; i++)
            {
                Instances[i] = new double[d];
                for (int j = 0; j < d; j++)
                    Instances[i][j] = data[i, j];
                ClassLabels[i] = (int)data[i, d];
            }

        }

    }
}
