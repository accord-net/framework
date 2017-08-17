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
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Base class for Hidden Conditional Random Fields learning algorithms based
    ///   on <see cref="IGradientOptimizationMethod">gradient optimization algorithms</see>.
    /// </summary>
    /// 
    public abstract class BaseHiddenGradientOptimizationLearning<TData, TOptimizer> : BaseHiddenConditionalRandomFieldLearning<TData>,
        ISupervisedLearning<HiddenConditionalRandomField<TData>, TData[], int>, IParallel,
        IHiddenConditionalRandomFieldLearning<TData>, IDisposable
        where TOptimizer : IGradientOptimizationMethod, ISupportsCancellation
    {
        private TOptimizer optimizer;
        private ForwardBackwardGradient<TData> calculator = new ForwardBackwardGradient<TData>();

        /// <summary>
        ///   Gets the optimization algorithm being used.
        /// </summary>
        /// 
        public TOptimizer Optimizer
        {
            get { return optimizer; }
        }

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

        /// <summary>
        /// Gets or sets the maximum number of iterations
        /// performed by the learning algorithm.
        /// </summary>
        /// <value>The maximum iterations.</value>
        public int MaxIterations { get; set; }

        /// <summary>
        /// Gets or sets whether the algorithm has converged.
        /// </summary>
        /// <value><c>true</c> if this instance has converged; otherwise, <c>false</c>.</value>
        public bool HasConverged { get; private set; }

        /// <summary>
        /// Gets or sets the parallelization options for this algorithm.
        /// </summary>
        /// <value>The parallel options.</value>
        public ParallelOptions ParallelOptions
        {
            get { return ((IParallel)calculator).ParallelOptions; }
            set { ((IParallel)calculator).ParallelOptions = value; }
        }

        /// <summary>
        ///   Constructs a new L-BFGS learning algorithm.
        /// </summary>
        /// 
        public BaseHiddenGradientOptimizationLearning()
        {
        }

        /// <summary>
        ///   Inheritors of this class should create the optimization algorithm in this
        ///   method, using the current <see cref="MaxIterations"/> and <see cref="Tolerance"/>
        ///   settings.
        /// </summary>
        /// 
        protected abstract TOptimizer CreateOptimizer();

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        protected override double InnerRun(TData[][] observations, int[] outputs)
        {
            calculator.Model = Model;
            this.optimizer = CreateOptimizer();
            this.optimizer.Function = calculator.Objective;
            this.optimizer.Gradient = calculator.Gradient;
            this.optimizer.Token = Token;

            calculator.Inputs = observations;
            calculator.Outputs = outputs;

            optimizer.Solution = Model.Function.Weights;
            HasConverged = optimizer.Minimize();
            Model.Function.Weights = optimizer.Solution;

            // Return negative log-likelihood as error function
            return -Model.LogLikelihood(observations, outputs);
        }

        /// <summary>
        ///   Online learning is not supported.
        /// </summary>
        ///   
        public double Run(TData[] observations, int output)
        {
            throw new NotSupportedException("Online learning is not supported.");
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
        public double Run(TData[][] observations, int[] outputs)
        {
            return InnerRun(observations, outputs);
        }

        /// <summary>
        ///   Online learning is not supported.
        /// </summary>
        ///   
        [Obsolete("Use Run(T[][], int[]) instead.")]
        public double RunEpoch(TData[][] observations, int[] output)
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
        ~BaseHiddenGradientOptimizationLearning()
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
