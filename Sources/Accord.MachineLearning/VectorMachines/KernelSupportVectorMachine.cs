// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Statistics.Kernels;
    using Accord.Statistics.Links;
    using Accord.Statistics.Models.Regression;

    /// <summary>
    ///  Sparse Kernel Support Vector Machine (kSVM)
    /// </summary>
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
    ///   int decision = System.Math.Sign(svm.Compute(inputs[0]));
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
    public class KernelSupportVectorMachine : SupportVectorMachine
    {

        private IKernel kernel;


        /// <summary>
        ///   Creates a new Kernel Support Vector Machine.
        /// </summary>
        /// 
        /// <param name="kernel">The chosen kernel for the machine.</param>
        /// <param name="inputs">The number of inputs for the machine.</param>
        /// 
        /// <remarks>
        ///   If the number of inputs is zero, this means the machine
        ///   accepts a indefinite number of inputs. This is often the
        ///   case for kernel vector machines using a sequence kernel.
        /// </remarks>
        /// 
        public KernelSupportVectorMachine(IKernel kernel, int inputs)
            : base(inputs)
        {
            if (kernel == null)
                throw new ArgumentNullException("kernel");

            this.kernel = kernel;
        }

        /// <summary>
        ///   Gets or sets the kernel used by this machine.
        /// </summary>
        /// 
        public IKernel Kernel
        {
            get { return kernel; }
            set { kernel = value; }
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <remarks>
        ///   For a binary decision problem, the decision for the negative
        ///   or positive class is typically computed by taking the sign of
        ///   the machine's output.
        /// </remarks>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="SupportVectorMachine.IsProbabilistic">probabilistic
        ///   </see> machine, the output is the probability of the positive
        ///   class. If this is a standard machine, the output is the distance
        ///   to the decision hyperplane in feature space.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        public override int Compute(double[] inputs, out double output)
        {
            output = Threshold;

            if (IsCompact)
            {
                for (int i = 0; i < Weights.Length; i++)
                    output += Weights[i] * inputs[i];
            }
            else
            {
                for (int i = 0; i < SupportVectors.Length; i++)
                    output += Weights[i] * kernel.Function(SupportVectors[i], inputs);
            }

            if (IsProbabilistic)
            {
                output = Link.Inverse(output);
                return output >= 0.5 ? +1 : -1;
            }

            return output >= 0 ? +1 : -1;
        }

        /// <summary>
        ///   Creates a new <see cref="SupportVectorMachine"/> that is
        ///   completely equivalent to a <see cref="LogisticRegression"/>.
        /// </summary>
        /// 
        /// <param name="regression">The <see cref="LogisticRegression"/> to be converted.</param>
        /// 
        /// <returns>
        ///   A <see cref="KernelSupportVectorMachine"/> whose linear weights 
        ///   are equivalent to the given <see cref="LogisticRegression"/>'s
        ///   <see cref="GeneralizedLinearRegression.Coefficients"> linear 
        ///   coefficients</see>, properly configured with a <see cref="LogLinkFunction"/>. 
        /// </returns>
        /// 
        public new static KernelSupportVectorMachine FromLogisticRegression(LogisticRegression regression)
        {
            double[] weights = regression.Coefficients;
            var svm = new KernelSupportVectorMachine(new Linear(), regression.Inputs);
            for (int i = 0; i < svm.Weights.Length; i++)
                svm.Weights[i] = weights[i + 1];

            svm.Threshold = regression.Intercept;
            svm.Link = new LogitLinkFunction(1, 0);

            return svm;
        }

        /// <summary>
        ///   Creates a new linear <see cref="SupportVectorMachine"/> 
        ///   with the given set of linear <paramref name="weights"/>.
        /// </summary>
        /// 
        /// <param name="weights">The machine's linear coefficients.</param>
        /// 
        /// <returns>
        ///   A <see cref="SupportVectorMachine"/> whose linear coefficients
        ///   are defined by the given <paramref name="weights"/> vector.
        /// </returns>
        /// 
        public new static KernelSupportVectorMachine FromWeights(double[] weights)
        {
            var svm = new KernelSupportVectorMachine(new Linear(0), weights.Length - 1);
            for (int i = 0; i < svm.Weights.Length; i++)
                svm.Weights[i] = weights[i + 1];
            svm.Threshold = weights[0];

            return svm;
        }

        /// <summary>
        ///   Converts a <see cref="Linear"/>-kernel machine into an array of 
        ///   linear coefficients. The first position in the array is the 
        ///   <see cref="SupportVectorMachine.Threshold"/> value. If this 
        ///   machine is not linear, an exception will be thrown.
        /// </summary>
        /// 
        /// <returns>
        ///   An array of linear coefficients representing this machine.
        /// </returns>
        /// 
        /// <exception cref="InvalidOperationException">
        ///   Thrown if the <see cref="Kernel">kernel function</see> is not <see cref="Linear"/>.
        /// </exception>
        /// 
        public override double[] ToWeights()
        {
            if (!(kernel is Linear))
                throw new InvalidOperationException();

            double[] w = new double[Weights.Length + 1];

            if (SupportVectors != null)
            {
                for (int i = 0; i < SupportVectors.Length; i++)
                    for (int j = 0; j < SupportVectors[i].Length; j++)
                        w[j + 1] += SupportVectors[i][j];
                w[0] = Threshold;
            }
            else
            {
                for (int i = 0; i < Weights.Length; i++)
                    w[i + 1] = Weights[i];
                w[0] = Threshold;
            }

            return w;
        }

        /// <summary>
        ///   Saves the machine to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the machine is to be serialized.</param>
        /// 
        public override void Save(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
        }

        /// <summary>
        ///   Loads a machine from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        public static new KernelSupportVectorMachine Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (KernelSupportVectorMachine)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a machine from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        public static new KernelSupportVectorMachine Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load(fs);
            }
        }

    }
}
