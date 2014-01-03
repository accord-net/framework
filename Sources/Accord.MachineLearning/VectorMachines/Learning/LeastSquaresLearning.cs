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
    using System;
    using Accord.Math;
    using Accord.Statistics.Kernels;

    /// <summary>
    ///   Least Squares SVM (LS-SVM) learning algorithm.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       <a href="http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.43.6438">
    ///       Suykens, J. A. K., et al. "Least squares support vector machine classifiers: a large scale 
    ///       algorithm." European Conference on Circuit Theory and Design, ECCTD. Vol. 99. 1999. Available on:
    ///       http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.43.6438 </a>
    ///       </description></item>
    ///     </list></para>  
    /// </remarks>
    /// 
    public class LeastSquaresLearning : ISupportVectorMachineLearning
    {

        private double[][] inputs;
        private int[] outputs;

        private double[] diagonal;
        private double gamma = 0.01;
        private double tolerance = 1e-6;

        private SupportVectorMachine machine;
        private IKernel kernel;

        private int cacheSize;
        private KernelFunctionCache cache;

        private int[] ones;


        /// <summary>
        ///   Initializes a new instance of the Least Squares SVM (LS-SVM) learning algorithm.
        /// </summary>
        /// 
        /// <param name="machine">A Support Vector Machine.</param>
        /// <param name="inputs">The input data points as row vectors.</param>
        /// <param name="outputs">The output label for each input point. Values must be either -1 or +1.</param>
        /// 
        public LeastSquaresLearning(SupportVectorMachine machine, double[][] inputs, int[] outputs)
        {
            SupportVectorLearningHelper.CheckArgs(machine, inputs, outputs);

            // Set the machine
            this.machine = machine;

            // Grab the machine kernel
            KernelSupportVectorMachine ksvm = machine as KernelSupportVectorMachine;
            this.kernel = (ksvm == null) ? new Linear() : ksvm.Kernel;

            // Kernel cache
            this.cacheSize = inputs.Length;

            // Get learning data
            this.inputs = inputs;
            this.outputs = outputs;

            this.ones = Matrix.Vector(outputs.Length, 1);
        }



        /// <summary>
        ///   Complexity (cost) parameter C. Increasing the value of C forces 
        ///   the creation of a more accurate model that may not generalize well. 
        /// </summary>
        /// 
        /// <remarks>
        ///   The cost parameter C controls the trade off between allowing training
        ///   errors and forcing rigid margins. It creates a soft margin that permits
        ///   some misclassifications. Increasing the value of C increases the cost of
        ///   misclassifying points and forces the creation of a more accurate model
        ///   that may not generalize well.
        /// </remarks>
        /// 
        public double Complexity
        {
            get { return 1.0 / this.gamma; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                this.gamma = 1.0 / value;
            }
        }

        /// <summary>
        ///   Convergence tolerance. Default value is 1e-6.
        /// </summary>
        /// 
        /// <remarks>
        ///   The criterion for completing the model training process. The default is 1e-6.
        /// </remarks>
        /// 
        public double Tolerance
        {
            get { return this.tolerance; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                this.tolerance = value;
            }
        }

        /// <summary>
        ///   Gets or sets the cache size to partially
        ///   stored the kernel matrix. Default is the
        ///   same number of input vectors.
        /// </summary>
        /// 
        public int CacheSize
        {
            get { return cacheSize; }
            set
            {
                if (cacheSize < 0)
                    throw new ArgumentOutOfRangeException("value");
                this.cacheSize = value;
            }
        }


        /// <summary>
        ///   Runs the LS-SVM algorithm.
        /// </summary>
        /// 
        /// <returns>
        ///   The misclassification error rate of
        ///   the resulting support vector machine.
        /// </returns>
        /// 
        public double Run()
        {
            return Run(false);
        }

        /// <summary>
        ///   Runs the LS-SVM algorithm.
        /// </summary>
        /// 
        /// <param name="computeError">
        ///   True to compute error after the training
        ///   process completes, false otherwise. Default is true.
        /// </param>
        /// 
        /// <returns>
        ///   The misclassification error rate of
        ///   the resulting support vector machine.
        /// </returns>
        /// 
        public double Run(bool computeError)
        {
            // Create kernel function cache
            cache = new KernelFunctionCache(kernel, inputs);
            diagonal = new double[inputs.Length];
            for (int i = 0; i < diagonal.Length; i++)
                diagonal[i] = kernel.Function(inputs[i], inputs[i]) + gamma;


            // 1. Solve to find nu and eta
            double[] eta = conjugateGradient(outputs);
            double[] nu = conjugateGradient(ones);


            // 2. Compute  s = Y' eta
            double s = 0;
            for (int i = 0; i < outputs.Length; i++)
                s += outputs[i] * eta[i];


            // 3. Find solution
            double b = 0;
            for (int i = 0; i < eta.Length; i++)
                b += eta[i];
            b /= s;

            double[] alpha = new double[nu.Length];
            for (int i = 0; i < alpha.Length; i++)
                alpha[i] = (nu[i] - eta[i] * b) * outputs[i];

            machine.SupportVectors = inputs;
            machine.Weights = alpha;
            machine.Threshold = b;

            // Compute error if required.
            return (computeError) ? ComputeError(inputs, outputs) : 0.0;
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
                bool actual = machine.Compute(inputs[i]) >= 0;
                bool expected = expectedOutputs[i] >= 0;

                if (actual != expected) count++;
            }

            // Return misclassification error ratio
            return count / (double)inputs.Length;
        }



        private double[] conjugateGradient(int[] B)
        {
            int[] y = outputs;
            double[] x = new double[B.Length];
            double[] r = new double[B.Length];
            double[] p = new double[B.Length]; 
            double[] H = new double[p.Length];
            double[] col = new double[inputs.Length];

            // Initialization
            for (int i = 0; i < B.Length; i++)
                p[i] = r[i] = B[i];

            double norm = 0;
            for (int i = 0; i < r.Length; i++)
                norm += r[i] * r[i];

            double beta = 1;
            int iteration = 0;

            // Repeat to convergence
            while (norm > Tolerance)
            {
                iteration++;

                if (iteration > B.Length)
                    break;

                for (int i = 0; i < p.Length; i++)
                    p[i] = r[i] + beta * p[i];


                for (int i = 0; i < p.Length; i++)
                {
                    for (int j = 0; j < col.Length; j++)
                        col[j] = cache.GetOrCompute(i, j) * y[j] * p[j];

                    double s = 0;
                    for (int j = 0; j < i; j++)              s += col[j];
                    for (int j = i + 1; j < col.Length; j++) s += col[j];

                    H[i] = (y[i] < 0) ? -s : s;
                }

                for (int i = 0; i < p.Length; i++)
                    H[i] += diagonal[i] * p[i];


                double sum = 0;
                for (int i = 0; i < p.Length; i++)
                    sum += p[i] * H[i];

                // Update the solution
                double lambda = norm / sum;
                for (int i = 0; i < p.Length; i++)
                    x[i] += lambda * p[i];

                for (int i = 0; i < p.Length; i++)
                    r[i] -= lambda * H[i];

                // Prepare for a next iteration
                double newNorm = 0;
                for (int i = 0; i < r.Length; i++)
                    newNorm += r[i] * r[i];

                beta = newNorm / norm;
                norm = newNorm;
            }

            return x;
        }
    }
}
