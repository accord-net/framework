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

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using Accord.Math;
    using Accord.Statistics;
    using Accord.Statistics.Kernels;
    using System;
    using System.Threading;

    /// <summary>
    ///   Exact support vector reduction through linear dependency elimination.
    /// </summary>
    /// 
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
    public class SupportVectorReductionBase<TModel, TKernel, TInput>
        : BaseSupportVectorCalibration<TModel, TKernel, TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>
        where TKernel : IKernel<TInput>
        where TInput : ICloneable
    {

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

            var rref = ech.Result;
            var pivot = ech.Pivot;


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
            int[] idx = alpha.Find(a => a != 0);
            Model.Weights = alpha.Get(idx);
            Model.SupportVectors = supportVectors.Get(idx);
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
