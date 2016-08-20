// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

namespace Accord.MachineLearning.VectorMachines
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Statistics.Kernels;
    using Accord.Statistics.Links;
    using Accord.Statistics.Models.Regression;
    using Accord.Math;
    using Accord.Statistics;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    ///  Sparse Kernel Support Vector Machine (kSVM)
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   The original optimal hyperplane algorithm (SVM) proposed by Vladimir Vapnik in 1963 was a
    ///   linear classifier. However, in 1992, Bernhard Boser, Isabelle Guyon and Vapnik suggested
    ///   a way to create non-linear classifiers by applying the kernel trick (originally proposed
    ///   by Aizerman et al.) to maximum-margin hyperplanes. The resulting algorithm is formally
    ///   similar, except that every dot product is replaced by a non-linear kernel function.</para>
    /// <para>
    ///   This allows the algorithm to fit the maximum-margin hyperplane in a transformed feature space.
    ///   The transformation may be non-linear and the transformed space high dimensional; thus though
    ///   the classifier is a hyperplane in the high-dimensional feature space, it may be non-linear in
    ///   the original input space.</para>
    ///
    /// <para>
    ///   The machines are also able to learn sequence classification problems in which the input vectors
    ///   can have arbitrary length. For an example on how to do that, please see the documentation page
    ///   for the <see cref="DynamicTimeWarping">DynamicTimeWarping kernel</see>.</para>
    ///
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Support_vector_machine">
    ///       http://en.wikipedia.org/wiki/Support_vector_machine </a></description></item>
    ///     <item><description><a href="http://www.kernel-machines.org/">
    ///       http://www.kernel-machines.org/ </a></description></item>
    ///   </list></para>
    /// </remarks>
    ///
    /// <example>
    ///   <code>
    ///   // Example XOR problem
    ///   double[][] inputs =
    ///   {
    ///       new double[] { 0, 0 }, // 0 xor 0: 1 (label +1)
    ///       new double[] { 0, 1 }, // 0 xor 1: 0 (label -1)
    ///       new double[] { 1, 0 }, // 1 xor 0: 0 (label -1)
    ///       new double[] { 1, 1 }  // 1 xor 1: 1 (label +1)
    ///   };
    ///
    ///   // Dichotomy SVM outputs should be given as [-1;+1]
    ///   int[] labels =
    ///   {
    ///       // 1,  0,  0, 1
    ///          1, -1, -1, 1
    ///   };
    ///
    ///   // Create a Kernel Support Vector Machine for the given inputs
    ///   KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Gaussian(0.1), inputs[0].Length);
    ///
    ///   // Instantiate a new learning algorithm for SVMs
    ///   SequentialMinimalOptimization smo = new SequentialMinimalOptimization(machine, inputs, labels);
    ///
    ///   // Set up the learning algorithm
    ///   smo.Complexity = 1.0;
    ///
    ///   // Run the learning algorithm
    ///   double error = smo.Run();
    ///
    ///   // Compute the decision output for one of the input vectors
    ///   int decision = System.Math.Sign(machine.Compute(inputs[0]));
    ///   </code>
    /// </example>
    ///
    /// <seealso cref="Accord.Statistics.Kernels"/>
    /// <seealso cref="KernelSupportVectorMachine"/>
    /// <seealso cref="MulticlassSupportVectorMachine"/>
    /// <seealso cref="MultilabelSupportVectorMachine"/>
    ///
    /// <seealso cref="Accord.MachineLearning.VectorMachines.Learning.SequentialMinimalOptimization"/>
    ///
    [Serializable]
    public class SupportVectorMachine<TKernel> : SupportVectorMachine<TKernel, double[]>
        where TKernel : IKernel<double[]>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportVectorMachine{TKernel}"/> class.
        /// </summary>
        ///
        /// <param name="inputs">The number of inputs for this machine.</param>
        /// <param name="kernel">The kernel function to be used.</param>
        ///
        public SupportVectorMachine(int inputs, TKernel kernel)
            : base(inputs, kernel)
        {

        }

        /// <summary>
        ///   Converts a <see cref="Linear"/>-kernel machine into an array of
        ///   linear coefficients. The first position in the array is the
        ///   <see cref="SupportVectorMachine{TKernel, TInput}.Threshold"/> value. If this
        ///   machine is not linear, an exception will be thrown.
        /// </summary>
        ///
        /// <returns>
        ///   An array of linear coefficients representing this machine.
        /// </returns>
        ///
        /// <exception cref="InvalidOperationException">
        ///   Thrown if the <see cref="SupportVectorMachine{TKernel, TInput}.Kernel">kernel function</see> is not <see cref="Linear"/>.
        /// </exception>
        ///
        public override double[] ToWeights()
        {
            if (!(Kernel is Linear))
                throw new InvalidOperationException();

            var w = new double[NumberOfInputs + 1];

            for (int i = 0; i < SupportVectors.Length; i++)
                for (int j = 0; j < SupportVectors[i].Length; j++)
                    w[j + 1] += SupportVectors[i][j];
            w[0] = Threshold;

            return w;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            var clone = new SupportVectorMachine<TKernel>(NumberOfInputs, Kernel);
            clone.SupportVectors = SupportVectors.MemberwiseClone();
            clone.Weights = (double[])Weights.Clone();
            clone.IsProbabilistic = IsProbabilistic;
            clone.Threshold = Threshold;
            return clone;
        }
    }
}