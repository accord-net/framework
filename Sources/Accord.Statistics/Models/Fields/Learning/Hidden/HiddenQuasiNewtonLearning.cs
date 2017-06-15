// Accord Statistics Library
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

#pragma warning disable 612, 618

namespace Accord.Statistics.Models.Fields.Learning
{
    using System;
    using Accord.Math.Optimization;
    using Accord.MachineLearning;
    using System.Threading;

    /// <summary>
    ///   Quasi-Newton (L-BFGS) learning algorithm for <see cref="HiddenConditionalRandomField{T}">
    ///   Hidden Conditional Hidden Fields</see>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the observations.</typeparam>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Fields\HiddenConditionalRandomFieldTest.cs" region="doc_learn_1" />
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Fields\HiddenConditionalRandomFieldTest.cs" region="doc_learn_2" />
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Fields\HiddenConditionalRandomFieldTest.cs" region="doc_learn_3" />
    ///   
    ///   <para>
    ///   The next example shows how to use the learning algorithms in a real-world dataset,
    ///   including training and testing in separate sets and evaluating its performance:</para>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Fields\Learning\NormalQuasiNewtonHiddenLearningTest.cs" region="doc_learn_pendigits" />
    /// </example>
    /// 
    /// <seealso cref="HiddenGradientDescentLearning{T}"/>
    /// <seealso cref="HiddenResilientGradientLearning{T}"/>
    /// 
    public class HiddenQuasiNewtonLearning<T> : BaseHiddenConditionalRandomFieldLearning<T>,
        ISupervisedLearning<HiddenConditionalRandomField<T>, T[], int>,
        IHiddenConditionalRandomFieldLearning<T>,
        IConvergenceLearning,
        IDisposable
    {

        private BoundedBroydenFletcherGoldfarbShanno lbfgs;
        private ForwardBackwardGradient<T> calculator;


        /// <summary>
        ///   Gets or sets the amount of the parameter weights
        ///   which should be included in the objective function.
        ///   Default is 0 (do not include regularization).
        /// </summary>
        /// 
        public double Regularization
        {
            get { return calculator.Regularization; }
            set { calculator.Regularization = value; }
        }

        /// <summary>
        /// Gets or sets the tolerance value used to determine
        /// whether the algorithm has converged.
        /// </summary>
        /// <value>The tolerance.</value>
        public double Tolerance { get; set; }

        int IConvergenceLearning.Iterations
        {
            get { return lbfgs.Iterations; }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the maximum number of iterations
        /// performed by the learning algorithm.
        /// </summary>
        /// <value>The maximum iterations.</value>
        public int MaxIterations { get; set; }

        /// <summary>
        /// Gets the current iteration number.
        /// </summary>
        /// <value>The current iteration.</value>
        public int CurrentIteration
        {
            get
            {
                if (lbfgs != null)
                    return lbfgs.Iterations;
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets whether the algorithm has converged.
        /// </summary>
        /// <value><c>true</c> if this instance has converged; otherwise, <c>false</c>.</value>
        public bool HasConverged
        {
            get
            {
                return lbfgs.Status == BoundedBroydenFletcherGoldfarbShannoStatus.FunctionConvergence
                    || lbfgs.Status == BoundedBroydenFletcherGoldfarbShannoStatus.GradientConvergence;
            }
        }

        /// <summary>
        ///   Constructs a new L-BFGS learning algorithm.
        /// </summary>
        /// 
        public HiddenQuasiNewtonLearning()
        {
        }

        /// <summary>
        ///   Constructs a new L-BFGS learning algorithm.
        /// </summary>
        /// 
        public HiddenQuasiNewtonLearning(HiddenConditionalRandomField<T> model)
        {
            Model = model;
            init();
        }

        private void init()
        {
            calculator = new ForwardBackwardGradient<T>(Model);

            lbfgs = new BoundedBroydenFletcherGoldfarbShanno(Model.Function.Weights.Length);
            lbfgs.FunctionTolerance = Tolerance;
            lbfgs.MaxIterations = MaxIterations;
            lbfgs.Function = calculator.Objective;
            lbfgs.Gradient = calculator.Gradient;

            for (int i = 0; i < lbfgs.UpperBounds.Length; i++)
            {
                lbfgs.UpperBounds[i] = 1e10;
                lbfgs.LowerBounds[i] = -1e100;
            }
        }

        /// <summary>
        ///   Runs the learning algorithm with the specified input
        ///   training observations and corresponding output labels.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="outputs">The observation's labels.</param>
        /// 
        [Obsolete("Please use Learn(x, y) instead.")]
        public double Run(T[][] observations, int[] outputs)
        {
            return InnerRun(observations, outputs);
        }

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        protected override double InnerRun(T[][] observations, int[] outputs)
        {
            init();

            calculator.Inputs = observations;
            calculator.Outputs = outputs;

            lbfgs.Token = Token;
            lbfgs.Minimize(Model.Function.Weights);

            Model.Function.Weights = lbfgs.Solution;

            // Return negative log-likelihood as error function
            return -Model.LogLikelihood(observations, outputs);
        }

        /// <summary>
        ///   Online learning is not supported.
        /// </summary>
        ///   
        public double Run(T[] observations, int output)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Online learning is not supported.
        /// </summary>
        ///   
        [Obsolete("Use Run(T[][], int[]) instead.")]
        public double RunEpoch(T[][] observations, int[] output)
        {
            throw new NotSupportedException();
        }


        #region IDisposable Members

        /// <summary>
        ///   Performs application-defined tasks associated with freeing,
        ///   releasing, or resetting unmanaged resources.
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before
        ///   the <see cref="HiddenQuasiNewtonLearning{T}"/> is reclaimed by garbage
        ///   collection.
        /// </summary>
        /// 
        ~HiddenQuasiNewtonLearning()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// 
        /// <param name="disposing"><c>true</c> to release both managed 
        /// and unmanaged resources; <c>false</c> to release only unmanaged
        /// resources.</param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (calculator != null)
                {
                    calculator.Dispose();
                    calculator = null;
                }
            }
        }

        #endregion

    }
}
