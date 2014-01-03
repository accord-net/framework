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

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using Accord.Math;
    using Accord.Statistics.Kernels;

    /// <summary>
    ///   Exact support vector reduction through 
    ///   linear dependency elimination.
    /// </summary>
    /// 
    public class SupportVectorReduction : ISupportVectorMachineLearning
    {

        KernelSupportVectorMachine machine;
        private IKernel kernel;
        double[] alpha;

        double[][] supportVectors;
        int[] outputs;


        /// <summary>
        ///   Creates a new <see cref="SupportVectorReduction"/> algorithm.
        /// </summary>
        /// 
        /// <param name="machine">The machine to be reduced.</param>
        /// 
        public SupportVectorReduction(KernelSupportVectorMachine machine)
        {
            this.machine = machine;
            this.supportVectors = machine.SupportVectors;
            this.outputs = machine.Weights.Sign().ToInt32();
            this.alpha = (double[])machine.Weights.Clone();
            this.kernel = machine.Kernel;
        }

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <param name="computeError">True to compute error after the training
        /// process completes, false otherwise.</param>
        /// 
        public double Run(bool computeError)
        {
            int n = supportVectors.Length;

            
            // Create Gram matrix
            double[,] K = new double[n, n];
            for (int i = 0; i < supportVectors.Length; i++)
                for (int j = 0; j < supportVectors.Length; j++)
                    K[i, j] = machine.Kernel.Function(supportVectors[i], supportVectors[j]);

            // Reduce to Echelon form to detect linear dependence
            ReducedRowEchelonForm ech = new ReducedRowEchelonForm(K);

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
                    for (int j = 0; j < supportVectors.Length; j++)
                        alpha[j] = alpha[j] + c * rref[j, row];

                    alpha[row] = 0;
                }

            }

            // Retain only multipliers which are not zero
            int[] idx = alpha.Find(a => a != 0);
            machine.Weights = alpha.Submatrix(idx);
            machine.SupportVectors = supportVectors.Submatrix(idx);

            if (computeError)
                return ComputeError(supportVectors, outputs);

            return 0;
        }

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        public double Run()
        {
            return Run(true);
        }

        /// <summary>
        ///   Computes the error rate for a given set of input and outputs.
        /// </summary>
        /// 
        public double ComputeError(double[][] inputs, int[] expectedOutputs)
        {
            // Compute errors
            int count = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                bool actual = compute(inputs[i]) >= 0;
                bool expected = expectedOutputs[i] >= 0;

                if (actual != expected) count++;
            }

            // Return misclassification error ratio
            return count / (double)inputs.Length;
        }

        private double compute(double[] point)
        {
            double sum = machine.Threshold;
            for (int i = 0; i < alpha.Length; i++)
                sum += alpha[i] * kernel.Function(supportVectors[i], point);

            return sum;
        }

    }
}
