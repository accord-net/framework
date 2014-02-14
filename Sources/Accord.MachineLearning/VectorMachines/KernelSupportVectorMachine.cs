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
            if (kernel == null) throw new ArgumentNullException("kernel");

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
