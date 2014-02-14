// Accord Statistics Library
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

namespace Accord.Statistics.Models.Fields.Learning
{
    using System;
    using System.ComponentModel;
    using Accord.Math.Optimization;

    /// <summary>
    ///   Conjugate Gradient learning algorithm for <see cref="HiddenConditionalRandomField{T}">
    ///   Hidden Conditional Hidden Fields</see>.
    /// </summary>
    /// 
    public class HiddenConjugateGradientLearning<T> : IHiddenConditionalRandomFieldLearning<T>,
        IConvergenceLearning, IDisposable
    {

        private ForwardBackwardGradient<T> calculator;
        private ConjugateGradient optimizer;

        /// <summary>
        ///   Gets or sets the model being trained.
        /// </summary>
        /// 
        public HiddenConditionalRandomField<T> Model { get; private set; }

        /// <summary>
        ///   Gets whether the model has converged
        ///   or if the line search has failed.
        /// </summary>
        /// 
        public bool Converged { get; private set; }

        /// <summary>
        ///   Gets the total number of iterations performed
        ///   by the conjugate gradient algorithm.
        /// </summary>
        /// 
        public int CurrentIteration
        {
            get { return optimizer.Iterations; }
        }

        /// <summary>
        ///   Gets or sets the maximum change in the average log-likelihood
        ///   after an iteration of the algorithm used to detect convergence.
        /// </summary>
        /// 
        /// <remarks>
        ///   This is the likelihood convergence limit L between two iterations of the algorithm. The
        ///   algorithm will stop when the change in the likelihood for two consecutive iterations
        ///   has not changed by more than L percent of the likelihood. If left as zero, the
        ///   algorithm will ignore this parameter and iterate over a number of fixed iterations
        ///   specified by the previous parameter.
        /// </remarks>
        /// 
        public double Tolerance
        {
            get { return optimizer.Tolerance; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Tolerance should be positive.");

                optimizer.Tolerance = value;
            }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the learning algorithm.
        /// </summary>
        /// 
        /// <remarks>
        ///   This is the maximum number of iterations to be performed by the learning algorithm. If
        ///   specified as zero, the algorithm will learn until convergence of the model average
        ///   likelihood respecting the desired limit.
        /// </remarks>
        /// 
        public int Iterations
        {
            get { return optimizer.MaxIterations; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "The maximum number of iterations should be positive.");

                optimizer.MaxIterations = value;
            }
        }

        /// <summary>
        ///   Occurs when the current learning progress has changed.
        /// </summary>
        /// 
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;



        /// <summary>
        ///   Constructs a new Conjugate Gradient learning algorithm.
        /// </summary>
        /// 
        public HiddenConjugateGradientLearning(HiddenConditionalRandomField<T> model)
        {
            Model = model;

            calculator = new ForwardBackwardGradient<T>(model);

            optimizer = new ConjugateGradient(model.Function.Weights.Length);
            optimizer.Progress += new EventHandler<OptimizationProgressEventArgs>(progressChanged);
            optimizer.Function = calculator.Objective;
            optimizer.Gradient = calculator.Gradient;
        }

        private void progressChanged(object sender, OptimizationProgressEventArgs e)
        {
            int percentage;

            double ratio = e.GradientNorm / e.SolutionNorm;
            if (Double.IsNaN(ratio))
                percentage = 100;
            else
                percentage = (int)Math.Max(0, Math.Min(100, (1.0 - ratio) * 100));

            if (ProgressChanged != null)
                ProgressChanged(this, new ProgressChangedEventArgs(percentage, e));
        }

        /// <summary>
        ///   Runs the learning algorithm with the specified input
        ///   training observations and corresponding output labels.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="outputs">The observation's labels.</param>
        /// 
        public double Run(T[][] observations, int[] outputs)
        {
            calculator.Inputs = observations;
            calculator.Outputs = outputs;

            Converged = true;
            optimizer.Tolerance = Tolerance;
            optimizer.MaxIterations = Iterations;

            try
            {
                optimizer.Minimize(Model.Function.Weights);
            }
            catch (LineSearchFailedException)
            {
                // TODO: Restructure CG to avoid exceptions.
                Converged = false;
            }

            Model.Function.Weights = optimizer.Solution;

            // Return negative log-likelihood as error function
            return -Model.LogLikelihood(observations, outputs);
        }

        /// <summary>
        ///   Online learning is not supported.
        /// </summary>
        ///   
        [Obsolete("Use Run(T[][], int[]) instead.")]
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
        ///   the <see cref="HiddenConjugateGradientLearning{T}"/> is reclaimed by garbage
        ///   collection.
        /// </summary>
        /// 
        ~HiddenConjugateGradientLearning()
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
