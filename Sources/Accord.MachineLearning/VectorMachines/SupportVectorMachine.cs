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
    using System;
    using System.IO;
    using Accord.IO;
    using Accord.Math;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Statistics.Kernels;
    using Accord.Statistics.Links;
    using Accord.Statistics.Models.Regression;
    using Accord.MachineLearning.VectorMachines.Learning;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using Statistics.Models.Regression.Linear;

    /// <summary>
    ///  Linear Support Vector Machine (SVM).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class implements a linear support vector machine classifier. For its kernel
    ///   counterpart, which can produce non-linear decision boundaries, please check 
    ///   <see cref="SupportVectorMachine{TKernel}"/> and <see cref="SupportVectorMachine{TKernel, TData}"/>.</para>
    ///   
    /// <para>
    ///   Note: a linear SVM model can be converted to <see cref="MultipleLinearRegression"/> and 
    ///   <see cref="LogisticRegression"/>. This means that linear and logistic regressions
    ///   can be created using any of the highly optimized LIBLINEAR learning algorithms
    ///   such as <see cref="LinearCoordinateDescent"/>, <see cref="LinearDualCoordinateDescent"/>,
    ///   <see cref="ProbabilisticCoordinateDescent"/> and <see cref="ProbabilisticDualCoordinateDescent"/>.
    ///   </para>
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
    ///   <para>
    ///   The first example shows how to learn a linear SVM. However, since the
    ///   problem being learned is not linearly separable, the classifier will
    ///   not be able to produce a perfect decision boundary.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\SequentialMinimalOptimizationTest.cs" region="doc_xor_linear" />
    ///   
    ///   <para>
    ///   The second example shows how to learn an SVM using a  standard kernel 
    ///   that operates on vectors of doubles. With kernels, it is possible to
    ///   produce non-linear boundaries that perfectly separate the data.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\SequentialMinimalOptimizationTest.cs" region="doc_xor_normal" />
    ///   
    ///   <para>
    ///   The third example shows how to learn an SVM using a Sparse kernel that 
    ///   operates on sparse vectors.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\SequentialMinimalOptimizationTest.cs" region="doc_xor_sparse" />
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
    [SerializationBinder(typeof(SupportVectorMachine.SupportVectorMachineBinder))]
    public class SupportVectorMachine : SupportVectorMachine<Linear>,
        IBinaryClassifier<double[]>, ISupportVectorMachine<double[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportVectorMachine"/> class.
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs for this machine.</param>
        /// 
        public SupportVectorMachine(int inputs)
            : base(inputs, new Linear())
        {
        }


        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            var clone = new SupportVectorMachine(NumberOfInputs) { Kernel = Kernel };
            clone.SupportVectors = SupportVectors.MemberwiseClone();
            clone.IsProbabilistic = IsProbabilistic;
            clone.Weights = (double[])Weights.Clone();
            clone.Threshold = Threshold;
            return clone;
        }

        #region Serialization backwards compatibility

        internal class SupportVectorMachineBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                AssemblyName name = new AssemblyName(assemblyName);

                if (name.Version < new Version(3, 1, 0))
                {
                    if (typeName == "Accord.MachineLearning.VectorMachines.SupportVectorMachine")
                        return typeof(SupportVectorMachine_2_13);
                }

                return null;
            }
        }

#pragma warning disable 0169
#pragma warning disable 0649

        [Serializable]
        internal class SupportVectorMachine_2_13 : ISerializable
        {
            public int inputCount;
            public double[][] supportVectors;
            public double[] weights;
            public double threshold;
            public ILinkFunction linkFunction;


            [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
            }

            [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
            private SupportVectorMachine_2_13(SerializationInfo info, StreamingContext context)
            {
                info.GetValue("inputCount", out inputCount);
                info.GetValue("linkFunction", out linkFunction);
                info.GetValue("supportVectors", out supportVectors);
                info.GetValue("threshold", out threshold);
                info.GetValue("weights", out weights);
            }


            public static implicit operator SupportVectorMachine(SupportVectorMachine_2_13 obj)
            {
                var svm = new SupportVectorMachine(obj.inputCount)
                {
                    SupportVectors = obj.supportVectors,
                    Weights = obj.weights,
                    Threshold = obj.threshold
                };

                var fn = obj.linkFunction as LogLinkFunction;
                if (fn != null)
                {
                    svm.Weights.Multiply(fn.B, result: svm.Weights);
                    svm.Threshold = svm.Threshold * fn.B + fn.A;
                    svm.IsProbabilistic = true;
                }

                return svm;
            }
        }

#pragma warning restore 0169
#pragma warning restore 0649

        #endregion


        /// <summary>
        /// Performs an explicit conversion from <see cref="SupportVectorMachine"/> to <see cref="MultipleLinearRegression"/>.
        /// </summary>
        /// 
        /// <param name="svm">The <see cref="SupportVectorMachine">linear Support Vector Machine</see> to be converted.</param>
        /// 
        /// <returns>The result of the conversion.</returns>
        /// 
        public static explicit operator MultipleLinearRegression(SupportVectorMachine svm)
        {
            double[] w = svm.ToWeights();
            return new MultipleLinearRegression()
            {
                Weights = w.Get(1, 0),
                Intercept = svm.Threshold
            };
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="SupportVectorMachine"/> to <see cref="LogisticRegression"/>.
        /// </summary>
        /// 
        /// <param name="svm">The <see cref="SupportVectorMachine">linear Support Vector Machine</see> to be converted.</param>
        /// 
        /// <returns>The result of the conversion.</returns>
        /// 
        public static explicit operator LogisticRegression(SupportVectorMachine svm)
        {
            double[] w = svm.ToWeights();
            return new LogisticRegression()
            {
                Weights = w.Get(1, 0),
                Intercept = svm.Threshold
            };
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="MultipleLinearRegression"/> to <see cref="SupportVectorMachine"/>.
        /// </summary>
        /// 
        /// <param name="regression">The linear regression to be converted.</param>
        /// 
        /// <returns>The result of the conversion.</returns>
        /// 
        public static explicit operator SupportVectorMachine(MultipleLinearRegression regression)
        {
            return new SupportVectorMachine(regression.NumberOfInputs)
            {
                Weights = new[] { 1.0 },
                SupportVectors = new[] { regression.Weights },
                Threshold = regression.Intercept,
            };
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="MultipleLinearRegression"/> to <see cref="SupportVectorMachine"/>.
        /// </summary>
        /// 
        /// <param name="regression">The linear regression to be converted.</param>
        /// 
        /// <returns>The result of the conversion.</returns>
        /// 
        public static explicit operator SupportVectorMachine(LogisticRegression regression)
        {
            return new SupportVectorMachine(regression.NumberOfInputs)
            {
                Weights = new[] { 1.0 },
                SupportVectors = new[] { regression.Weights },
                Threshold = regression.Intercept,
                IsProbabilistic = true
            };
        }
    }
}
