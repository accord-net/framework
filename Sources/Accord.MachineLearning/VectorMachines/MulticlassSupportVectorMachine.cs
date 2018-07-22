// Accord Machine Learning Library
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

namespace Accord.MachineLearning.VectorMachines
{
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Obsolete. Please use <see cref="MulticlassSupportVectorMachine{TKernel}"/> instead.
    /// </summary>
    /// 
    [Serializable]
    [Obsolete("Please use MulticlassSupportVectorMachine<TKernel> instead.")]
#if !NETSTANDARD
    [SerializationBinder(typeof(MulticlassSupportVectorMachine.MulticlassSupportVectorMachineBinder))]
#endif
    public class MulticlassSupportVectorMachine :
        MulticlassSupportVectorMachine<IKernel<double[]>>, ICloneable
    {
#pragma warning disable 0618

        /// <summary>
        ///   Initializes a new instance of the <see cref="MulticlassSupportVectorMachine"/> class.
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs by the machine.</param>
        /// <param name="classes">The number of classes to be handled by the machine.</param>
        /// 
        public MulticlassSupportVectorMachine(int inputs, int classes)
            : base(classes, () => new KernelSupportVectorMachine(new Linear(), inputs))
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MulticlassSupportVectorMachine"/> class.
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs by the machine.</param>
        /// <param name="classes">The number of classes to be handled by the machine.</param>
        /// <param name="kernel">The kernel function to be used in the machine.</param>
        /// 
        public MulticlassSupportVectorMachine(int inputs, IKernel kernel, int classes)
            : base(classes, () => new KernelSupportVectorMachine(kernel, inputs))
        {
        }

        /// <summary>
        ///   Constructs a new Multi-class Kernel Support Vector Machine
        /// </summary>
        /// 
        /// <param name="machines">
        ///   The machines to be used in each of the pair-wise class subproblems.
        /// </param>
        /// 
        public MulticlassSupportVectorMachine(KernelSupportVectorMachine[][] machines)
            : base(machines.Length + 1, () => null)
        {
            NumberOfInputs = machines[0][0].NumberOfInputs;
            for (int i = 0; i < Models.Length; i++)
            {
                for (int j = 0; j < Models[i].Length; j++)
                {
                    Models[i][j] = machines[i][j];
                    if (machines[i][j].NumberOfInputs != NumberOfInputs)
                        throw new ArgumentException();
                }
            }
        }
#pragma warning restore 0618

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            var clone = new MulticlassSupportVectorMachine(NumberOfInputs, Kernel as IKernel, NumberOfOutputs);
            clone.Method = Method;
            for (int i = 0; i < Models.Length; i++)
                for (int j = 0; j < Models[i].Length; j++)
                    clone.Models[i][j] = (SupportVectorMachine<IKernel<double[]>>)Models[i][j].Clone();
            return clone;
        }

        #region Obsolete
        /// <summary>
        ///   Gets the total number of machines
        ///   in this multi-class classifier.
        /// </summary>
        /// 
        [Obsolete("Please use the Count property.")]
        public int MachinesCount
        {
            get { return base.Count; }
        }

        /// <summary>
        ///   Gets the number of classes.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfOutputs instead.")]
        public int Classes
        {
            get { return NumberOfOutputs; }
        }

        /// <summary>
        ///   Gets the number of inputs of the machines.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfInputs instead.")]
        public int Inputs
        {
            get { return NumberOfInputs; }
        }

        /// <summary>
        ///   Gets the subproblems classifiers.
        /// </summary>
        /// 
        [Obsolete("Please use the Models property instead.")]
        public KernelSupportVectorMachine[][] Machines
        {
            get { return Models.Apply((x, i, j) => (KernelSupportVectorMachine)x); }
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        ///
        [Obsolete("Please use the Decide, Distance or Probability methods")]
        public int Compute(params double[] inputs)
        {
            double output; // Compute using elimination method as default.
            return Compute(inputs, MulticlassComputeMethod.Elimination, out output);
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
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
        [Obsolete("Please use the Decide, Distance or Probability methods")]
        public int Compute(double[] inputs, out double output)
        {
            // Compute using elimination method as default.
            return Compute(inputs, MulticlassComputeMethod.Elimination, out output);
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// <param name="decisionPath">The decision path followed by the Decision
        /// Directed Acyclic Graph used by the <see cref="MulticlassComputeMethod.Elimination">
        /// elimination</see> method.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        /// 
        [Obsolete("Please use the Decide, Distance or Probability methods")]
        public int Compute(double[] inputs, out double output, out Tuple<int, int>[] decisionPath)
        {
            lock (Models)
            {
                var prev = Method;
                Method = MulticlassComputeMethod.Elimination;
                int decision;
                output = this.Probabilities(inputs, out decision)[decision];
                decisionPath = LastDecisionPath.Apply(x => x.Pair.ToTuple());
                Method = prev;
                return decision;
            }
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="responses">The model response for each class.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        [Obsolete("Please use the Decide, Distance or Probability methods")]
        public int Compute(double[] inputs, out double[] responses)
        {
            lock (Models)
            {
                var prev = Method;
                Method = MulticlassComputeMethod.Elimination;
                int decision;
                responses = this.Probabilities(inputs, out decision);
                Method = prev;
                return decision;
            }
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="method">The <see cref="MulticlassComputeMethod">
        ///   multi-class classification method</see> to use.</param>
        /// <param name="responses">The model response for each class.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        [Obsolete("Please use the Decide, Distance or Probability methods")]
        public int Compute(double[] inputs, MulticlassComputeMethod method, out double[] responses, out double output)
        {
            lock (this.Models)
            {
                var prev = this.Method;
                this.Method = method;
                int decision;
                responses = this.Probabilities(inputs, out decision);
                this.Method = prev;
                output = responses[decision];
                return decision;
            }
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="method">The <see cref="MulticlassComputeMethod">
        ///   multi-class classification method</see> to use.</param>
        /// <param name="responses">The model response for each class.</param>
        /// 
        /// <returns>The class decision for the given input.</returns>
        /// 
        [Obsolete("Please use the Decide, Distance or Probability methods")]
        public int Compute(double[] inputs, MulticlassComputeMethod method, out double[] responses)
        {
            lock (this.Models)
            {
                var prev = this.Method;
                this.Method = method;
                int decision;
                responses = this.Probabilities(inputs, out decision);
                this.Method = prev;
                return decision;
            }
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="method">The <see cref="MulticlassComputeMethod">
        ///   multi-class classification method</see> to use.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// 
        /// <returns>The class decision for the given input.</returns>
        ///
        [Obsolete("Please use the Decide, Distance or Probability methods")]
        public int Compute(double[] inputs, MulticlassComputeMethod method, out double output)
        {
            lock (this.Models)
            {
                var prev = this.Method;
                this.Method = method;
                int decision;
                double[] responses;
                responses = this.Probabilities(inputs, out decision);
                this.Method = prev;
                output = responses[decision];
                return decision;
            }
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="method">The <see cref="MulticlassComputeMethod">
        ///   multi-class classification method</see> to use.</param>
        /// 
        /// <returns>The class decision for the given input.</returns>
        ///
        [Obsolete("Please use the Decide, Distance or Probability methods")]
        public int Compute(double[] inputs, MulticlassComputeMethod method)
        {
            lock (this.Models)
            {
                var prev = this.Method;
                this.Method = method;
                int decision;
                this.Probabilities(inputs, out decision);
                this.Method = prev;
                return decision;
            }
        }

        /// <summary>
        ///   Gets whether this machine has been calibrated to
        ///   produce probabilistic outputs (through the Probability(TInput)
        ///   and Probabilities(TInput) methods).
        /// </summary>
        /// 
        public bool IsProbabilistic
        {
            get { return Models[0][0].IsProbabilistic; }
        }

#if !NETSTANDARD1_4
        /// <summary>
        ///   Saves the machine to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the machine is to be serialized.</param>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Save() instead (or use it as an extension method).")]
        public void Save(Stream stream)
        {
            Accord.IO.Serializer.Save(this, stream);
        }

        /// <summary>
        ///   Saves the machine to a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file to which the machine is to be serialized.</param>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Save() instead (or use it as an extension method).")]
        public void Save(string path)
        {
            Accord.IO.Serializer.Save(this, path);
        }

        /// <summary>
        ///   Loads a machine from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Load<MulticlassSupportVectorMachine>(stream) instead.")]
        public static MulticlassSupportVectorMachine Load(Stream stream)
        {
            return Accord.IO.Serializer.Load<MulticlassSupportVectorMachine>(stream);
        }

        /// <summary>
        ///   Loads a machine from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Load<MulticlassSupportVectorMachine>(path) instead.")]
        public static MulticlassSupportVectorMachine Load(string path)
        {
            return Accord.IO.Serializer.Load<MulticlassSupportVectorMachine>(path);
        }
#endif
        #endregion


        #region Serialization backwards compatibility
#if !NETSTANDARD
        internal class MulticlassSupportVectorMachineBinder : SerializationBinder
        {

            public override Type BindToType(string assemblyName, string typeName)
            {
                AssemblyName name = new AssemblyName(assemblyName);

                if (name.Version < new Version(3, 1, 0))
                {
#pragma warning disable 618
                    if (typeName == "Accord.MachineLearning.VectorMachines.MulticlassSupportVectorMachine")
                        return typeof(MulticlassSupportVectorMachine_2_13);
                    else if (typeName == "Accord.MachineLearning.VectorMachines.KernelSupportVectorMachine[]")
                        return typeof(KernelSupportVectorMachine.KernelSupportVectorMachine_2_13[]);
                    else if (typeName == "Accord.MachineLearning.VectorMachines.KernelSupportVectorMachine")
                        return typeof(KernelSupportVectorMachine.KernelSupportVectorMachine_2_13);
#pragma warning restore 618
                }

                return Type.GetType(typeName);
            }
        }

#pragma warning disable 0169
#pragma warning disable 0649
        [Serializable]
        internal class MulticlassSupportVectorMachine_2_13
        {
            public KernelSupportVectorMachine.KernelSupportVectorMachine_2_13[][] machines;

            public MulticlassSupportVectorMachine_2_13()
            {

            }

            public static implicit operator MulticlassSupportVectorMachine(MulticlassSupportVectorMachine_2_13 obj)
            {
#pragma warning disable 618
                var svms = obj.machines.Apply((x, i, j) => (KernelSupportVectorMachine)x);
#pragma warning restore 618
                var svm = new MulticlassSupportVectorMachine(svms);
                return svm;
            }
        }

#pragma warning restore 0169
#pragma warning restore 0649
#endif
        #endregion

    }

}
