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
    using Accord.Statistics.Links;

    /// <summary>
    ///   Sparse Linear Support Vector Machine (SVM)
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Support vector machines (SVMs) are a set of related supervised learning methods
    ///   used for classification and regression. In simple words, given a set of training
    ///   examples, each marked as belonging to one of two categories, a SVM training algorithm
    ///   builds a model that predicts whether a new example falls into one category or the
    ///   other.</para>
    /// <para>
    ///   Intuitively, an SVM model is a representation of the examples as points in space,
    ///   mapped so that the examples of the separate categories are divided by a clear gap
    ///   that is as wide as possible. New examples are then mapped into that same space and
    ///   predicted to belong to a category based on which side of the gap they fall on.</para>
    ///   
    /// <para>
    ///   For the non-linear generalization of the Support Vector Machine using arbitrary 
    ///   kernel functions, please see the <see cref="KernelSupportVectorMachine"/>.
    /// </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Support_vector_machine">
    ///       http://en.wikipedia.org/wiki/Support_vector_machine </a></description></item>
    ///   </list></para>   
    /// </remarks>
    /// 
    /// <example>
    ///   <code>
    ///   // Example AND problem
    ///   double[][] inputs =
    ///   {
    ///       new double[] { 0, 0 }, // 0 and 0: 0 (label -1)
    ///       new double[] { 0, 1 }, // 0 and 1: 0 (label -1)
    ///       new double[] { 1, 0 }, // 1 and 0: 0 (label -1)
    ///       new double[] { 1, 1 }  // 1 and 1: 1 (label +1)
    ///   };
    ///   
    ///   // Dichotomy SVM outputs should be given as [-1;+1]
    ///   int[] labels =
    ///   {
    ///       // 0,  0,  0, 1
    ///         -1, -1, -1, 1
    ///   };
    ///   
    ///   // Create a Support Vector Machine for the given inputs
    ///   SupportVectorMachine machine = new SupportVectorMachine(inputs[0].Length);
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
    /// <seealso cref="KernelSupportVectorMachine"/>
    /// <seealso cref="MulticlassSupportVectorMachine"/>
    /// <seealso cref="MultilabelSupportVectorMachine"/>
    ///
    /// <seealso cref="Accord.MachineLearning.VectorMachines.Learning.SequentialMinimalOptimization"/>
    /// 
    [Serializable]
    public class SupportVectorMachine : ISupportVectorMachine
    {

        private int inputCount;
        private double[][] supportVectors;
        private double[] weights;
        private double threshold;

        private ILinkFunction linkFunction;

        /// <summary>
        ///   Gets or sets the <see cref="ILinkFunction">link
        ///   function</see> used by this machine, if any.
        /// </summary>
        /// 
        /// <value>The link function used to transform machine outputs.</value>
        /// 
        public ILinkFunction Link
        {
            get { return linkFunction; }
            set { linkFunction = value; }
        }

        /// <summary>
        ///   Gets a value indicating whether this machine produces probabilistic outputs.
        /// </summary>
        /// 
        /// <value>
        ///   <c>true</c> if this machine produces probabilistic outputs; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool IsProbabilistic
        {
            get { return linkFunction != null; }
        }

        /// <summary>
        ///   Creates a new Support Vector Machine
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs for the machine.</param>
        /// 
        public SupportVectorMachine(int inputs)
        {
            this.inputCount = inputs;
        }

        /// <summary>
        ///   Gets the number of inputs accepted by this machine.
        /// </summary>
        /// 
        /// <remarks>
        ///   If the number of inputs is zero, this means the machine
        ///   accepts a indefinite number of inputs. This is often the
        ///   case for kernel vector machines using a sequence kernel.
        /// </remarks>
        /// 
        public int Inputs
        {
            get { return inputCount; }
        }

        /// <summary>
        ///   Gets or sets the collection of support vectors used by this machine.
        /// </summary>
        /// 
        public double[][] SupportVectors
        {
            get { return supportVectors; }
            set { supportVectors = value; }
        }

        /// <summary>
        ///   Gets whether this machine is in compact mode. Compact
        ///   machines do not need to keep storing their support vectors.
        /// </summary>
        /// 
        public bool IsCompact
        {
            get { return supportVectors == null; }
        }

        /// <summary>
        ///   Gets or sets the collection of weights used by this machine.
        /// </summary>
        /// 
        public double[] Weights
        {
            get { return weights; }
            set { weights = value; }
        }

        /// <summary>
        ///   Gets or sets the threshold (bias) term for this machine.
        /// </summary>
        /// 
        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
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
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        public virtual int Compute(double[] inputs, out double output)
        {
            output = threshold;

            if (supportVectors == null)
            {
                for (int i = 0; i < weights.Length; i++)
                    output += weights[i] * inputs[i];
            }
            else
            {
                for (int i = 0; i < supportVectors.Length; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < inputs.Length; j++)
                        sum += supportVectors[i][j] * inputs[j];
                    output += weights[i] * sum;
                }
            }

            if (IsProbabilistic)
            {
                output = linkFunction.Inverse(output);
                return output >= 0.5 ? +1 : -1;
            }

            return output >= 0 ? +1 : -1;
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
        /// 
        /// <returns>The output for the given input. In a typical classification
        /// problem, the sign of this value should be considered as the class label.</returns>
        ///  
        public double Compute(double[] inputs)
        {
            double output;
            Compute(inputs, out output);
            return output;
        }


        /// <summary>
        ///   Saves the machine to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the machine is to be serialized.</param>
        /// 
        public virtual void Save(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
        }

        /// <summary>
        ///   Saves the machine to a stream.
        /// </summary>
        /// 
        /// <param name="path">The path to the file to which the machine is to be serialized.</param>
        /// 
        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                Save(fs);
            }
        }

        /// <summary>
        ///   Loads a machine from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        public static SupportVectorMachine Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (SupportVectorMachine)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a machine from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        public static SupportVectorMachine Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load(fs);
            }
        }

    }
}
