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

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using Accord.Math;
    using Accord.Statistics;
    using Accord.Statistics.Kernels;
    using Math.Optimization.Losses;
    using System;
    using System.Threading;
    using Accord.Compat;

    /// <summary>
    ///   Exact support vector reduction through linear dependency elimination.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///   The following example shows how to reduce the number of support vectors in
    ///   a SVM by removing vectors which are linearly dependent between themselves.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\SupportVectorReductionTest.cs" region="doc_learn" />
    /// </example>
    /// 
    //[Obsolete("Please use SupportVectorReduction<TKernel> instead.")]
    public class SupportVectorReduction
        : SupportVectorReductionBase<SupportVectorMachine<IKernel<double[]>, double[]>,
            IKernel<double[]>, double[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportVectorReduction"/> class.
        /// </summary>
        /// <param name="machine">The machine to be reduced.</param>
        ///
        public SupportVectorReduction(ISupportVectorMachine<double[]> machine)
            : base(machine)
        {
        }
    }

    /// <summary>
    ///   Exact support vector reduction through linear dependency elimination.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///   The following example shows how to reduce the number of support vectors in
    ///   a SVM by removing vectors which are linearly dependent between themselves.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\SupportVectorReductionTest.cs" region="doc_learn" />
    /// </example>
    /// 
    public class SupportVectorReduction<TKernel>
        : SupportVectorReductionBase<SupportVectorMachine<TKernel, double[]>, TKernel, double[]>
        where TKernel : IKernel<double[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportVectorReduction"/> class.
        /// </summary>
        /// <param name="machine">The machine to be reduced.</param>
        ///
        public SupportVectorReduction(ISupportVectorMachine<double[]> machine)
            : base(machine)
        {
        }
    }

    /// <summary>
    ///   Exact support vector reduction through linear dependency elimination.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///   The following example shows how to reduce the number of support vectors in
    ///   a SVM by removing vectors which are linearly dependent between themselves.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\SupportVectorReductionTest.cs" region="doc_learn" />
    /// </example>
    /// 
    public class SupportVectorReduction<TKernel, TInput>
        : SupportVectorReductionBase<SupportVectorMachine<TKernel, TInput>, TKernel, TInput>
        where TKernel : IKernel<TInput>
        where TInput : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportVectorReduction"/> class.
        /// </summary>
        /// <param name="machine">The machine to be reduced.</param>
        ///
        public SupportVectorReduction(ISupportVectorMachine<TInput> machine)
            : base(machine)
        {
        }
    }

    /// <summary>
    ///   Exact support vector reduction through linear dependency elimination.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///   The following example shows how to reduce the number of support vectors in
    ///   a SVM by removing vectors which are linearly dependent between themselves.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\SupportVectorReductionTest.cs" region="doc_learn" />
    /// </example>
    /// 
    public class SupportVectorReductionBase<TModel, TKernel, TInput>
        : BaseSupportVectorCalibration<TModel, TKernel, TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>
        where TKernel : IKernel<TInput>
#if !NETSTANDARD1_4
        where TInput : ICloneable
#endif
    {
        private double threshold = 1e-12;

        /// <summary>
        /// Gets or sets the minimum threshold that is used to determine
        /// whether a weight will be kept in the machine or not. Default
        /// is 1e-12.
        /// </summary>
        /// 
        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        /// <summary>
        ///   Creates a new <see cref="SupportVectorReduction"/> algorithm.
        /// </summary>
        /// 
        /// <param name="machine">The machine to be reduced.</param>
        /// 
        public SupportVectorReductionBase(TModel machine)
            : base(machine)
        {
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// 
        public TModel Learn()
        {
            return base.Learn(Model.SupportVectors, Classes.Decide(Model.Weights));
        }

        /// <summary>
        /// Runs the learning algorithm.
        /// </summary>
        protected override void InnerRun()
        {
            var supportVectors = Model.SupportVectors;
            var alpha = (double[])Model.Weights.Clone();
            var kernel = Model.Kernel;

            this.Input = supportVectors;
            this.Output = Classes.Decide(alpha);

            int n = supportVectors.Length;

            // Create Gram matrix
            var K = kernel.ToJagged(supportVectors);

            // Reduce to Echelon form to detect linear dependence
            var ech = new JaggedReducedRowEchelonForm(K);

            double[][] rref = ech.Result;
            int[] pivot = ech.Pivot;


            // For each support vector
            for (int i = 0; i < supportVectors.Length; i++)
            {
                // Get its corresponding row
                int row = ech.Pivot[i];

                // Check if it can be expressed as a
                // linear combination of other vectors

                if (row > supportVectors.Length - ech.FreeVariables - 1)
                {
                    double c = alpha[row];
                    for (int j = 0; j < rref.Length; j++)
                        alpha[j] = alpha[j] + c * rref[j][row];

                    alpha[row] = 0;
                }
            }

            // Retain only multipliers which are not zero
            int[] idx = alpha.Find(a => Math.Abs(a) > threshold);
            Model.Weights = alpha.Get(idx);
            Model.SupportVectors = supportVectors.Get(idx);
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please use Learn() instead.")]
        public override double Run()
        {
            var classifier = Learn();
            return new ZeroOneLoss(Output).Loss(classifier.Decide(Input));
        }

        /// <summary>
        ///   Creates a new <see cref="SupportVectorReduction"/> algorithm.
        /// </summary>
        /// 
        /// <param name="machine">The machine to be reduced.</param>
        /// 
        public SupportVectorReductionBase(ISupportVectorMachine<TInput> machine)
            : base(machine)
        {
        }


    }
}
