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
    using System.ComponentModel;
    using System.Threading;
    using Accord.Math;
    using Accord.MachineLearning;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Stochastic Gradient Descent learning algorithm for <see cref="HiddenConditionalRandomField{T}">
    ///   Hidden Conditional Hidden Fields</see>.
    /// </summary>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Fields\HiddenConditionalRandomFieldTest.cs" region="doc_learn_1" />
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Fields\HiddenConditionalRandomFieldTest.cs" region="doc_learn_2" />
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Fields\HiddenConditionalRandomFieldTest.cs" region="doc_learn_3" />
    /// </example>
    /// 
    /// <seealso cref="HiddenQuasiNewtonLearning{T}" />
    /// <seealso cref="HiddenResilientGradientLearning{T}"/>
    /// 
    public class HiddenGradientDescentLearning<T> : BaseHiddenConditionalRandomFieldLearning<T>,
        ISupervisedLearning<HiddenConditionalRandomField<T>, T[], int>, IParallel,
        IHiddenConditionalRandomFieldLearning<T>, IConvergenceLearning, IDisposable
    {

        private double learningRate = 100;
        private ISingleValueConvergence convergence;

        //private double decay = 0.9;
        //private double tau = 0.5;
        private double stepSize;

        private bool stochastic = true;
        private double[] gradient;

        private ForwardBackwardGradient<T> calculator = new ForwardBackwardGradient<T>();


        private Object lockObj = new Object();



        /// <summary>
        ///   Gets or sets the learning rate to use as the gradient
        ///   descent step size. Default value is 1e-1.
        /// </summary>
        /// 
        public double LearningRate
        {
            get { return learningRate; }
            set { learningRate = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum change in the average log-likelihood
        ///   after an iteration of the algorithm used to detect convergence.
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return convergence.Tolerance; }
            set { convergence.Tolerance = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the learning algorithm.
        /// </summary>
        /// 
        public int MaxIterations
        {
            get { return convergence.MaxIterations; }
            set { convergence.MaxIterations = value; }
        }

        /// <summary>
        ///   Please use MaxIterations instead.
        /// </summary>
        /// 
        [Obsolete("Please use MaxIterations instead.")]
        public int Iterations
        {
            get { return MaxIterations; }
            set { MaxIterations = value; }
        }

        /// <summary>
        ///   Gets or sets the number of performed iterations.
        /// </summary>
        /// 
        public int CurrentIteration
        {
            get { return convergence.CurrentIteration; }
        }

        /// <summary>
        /// Gets or sets whether the algorithm has converged.
        /// </summary>
        /// <value><c>true</c> if this instance has converged; otherwise, <c>false</c>.</value>
        public bool HasConverged
        {
            get { return convergence.HasConverged; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this <see cref="HiddenGradientDescentLearning&lt;T&gt;"/>
        ///   should use stochastic gradient updates.
        /// </summary>
        /// 
        /// <value><c>true</c> for stochastic updates; otherwise, <c>false</c>.</value>
        /// 
        public bool Stochastic
        {
            get { return stochastic; }
            set { stochastic = value; }
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
        /// Gets or sets the parallelization options for this algorithm.
        /// </summary>
        /// <value>The parallel options.</value>
        public ParallelOptions ParallelOptions
        {
            get { return ((IParallel)calculator).ParallelOptions; }
            set { ((IParallel)calculator).ParallelOptions = value; }
        }



        /// <summary>
        ///   Occurs when the current learning progress has changed.
        /// </summary>
        /// 
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        ///   Initializes a new instance of the <see cref="HiddenGradientDescentLearning&lt;T&gt;"/> class.
        /// </summary>
        /// 

        public HiddenGradientDescentLearning()
        {
            convergence = new RelativeConvergence();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HiddenGradientDescentLearning&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="model">The model to be trained.</param>
        /// 
        public HiddenGradientDescentLearning(HiddenConditionalRandomField<T> model)
            : this()
        {
            Model = model;
            init();
        }

        private void init()
        {
            calculator.Model = Model;
            gradient = new double[Model.Function.Weights.Length];
        }

        /// <summary>
        ///   Resets the step size.
        /// </summary>
        /// 
        public void Reset()
        {
            convergence.Clear();
            stepSize = 0;
        }

        /// <summary>
        ///   Runs the learning algorithm with the specified input
        ///   training observations and corresponding output labels.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="outputs">The observation's labels.</param>
        /// 
        /// <returns>The error in the last iteration.</returns>
        /// 
        public double RunEpoch(T[][] observations, int[] outputs)
        {
            double error = 0;

            if (stochastic)
            {

                // In batch mode, we will use the average of the gradients
                // at each point as a better estimate of the true gradient.
                Array.Clear(gradient, 0, gradient.Length);

                int progress = 0;

                // For each training point
                if (ParallelOptions.MaxDegreeOfParallelism == 1)
                {
                    for (int i = 0; i < observations.Length; i++)
                        iterate(observations, outputs, i, ref error, ref progress);
                }
                else
                {
                    Parallel.For(0, observations.Length, ParallelOptions, i =>
                        iterate(observations, outputs, i, ref error, ref progress));
                }

                // Compute the average gradient
                for (int i = 0; i < gradient.Length; i++)
                    gradient[i] /= observations.Length;
            }
            else
            {
                calculator.Inputs = observations;
                calculator.Outputs = outputs;

                // Compute the true gradient
                gradient = calculator.Gradient();

                error = calculator.LastError;
            }

            double[] parameters = Model.Function.Weights;
            stepSize = learningRate / (convergence.CurrentIteration + 1);

            // Update the model using a dynamic step size
            for (int i = 0; i < parameters.Length; i++)
            {
                if (Double.IsInfinity(parameters[i])) continue;

                parameters[i] -= stepSize * gradient[i];

                Accord.Diagnostics.Debug.Assert(!Double.IsNaN(parameters[i]));
                Accord.Diagnostics.Debug.Assert(!Double.IsPositiveInfinity(parameters[i]));
            }


            return convergence.NewValue = error;
        }

        private void iterate(T[][] observations, int[] outputs, int i, ref double error, ref int progress)
        {
            calculator.Inputs = new[] { observations[i] };
            calculator.Outputs = new[] { outputs[i] };

            // Compute the estimated gradient
            double[] estimate = calculator.Gradient();

            lock (lockObj)
            {
                // Accumulate
                for (int j = 0; j < estimate.Length; j++)
                    gradient[j] += estimate[j];
                error += calculator.LastError;
            }

            int current = Interlocked.Increment(ref progress);
            double percent = current / (double)observations.Length * 100.0;
            OnProgressChanged(new ProgressChangedEventArgs((int)percent, i));

            Accord.Diagnostics.Debug.Assert(!gradient.HasNaN());
        }


        /// <summary>
        ///   Runs the learning algorithm with the specified input
        ///   training observations and corresponding output labels.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="outputs">The observation's labels.</param>
        /// 
        /// <returns>The error in the last iteration.</returns>
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
            convergence.Clear();

            do
            {
                RunEpoch(observations, outputs);
                if (Token.IsCancellationRequested)
                    break;
            }
            while (!convergence.HasConverged);

            return convergence.NewValue;
        }

        /// <summary>
        ///   Runs one iteration of the learning algorithm with the
        ///   specified input training observation and corresponding
        ///   output label.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="output">The observation's labels.</param>
        /// 
        /// <returns>The error in the last iteration.</returns>
        /// 
        public double Run(T[] observations, int output)
        {
            calculator.Inputs = new[] { observations };
            calculator.Outputs = new[] { output };

            double[] gradient = calculator.Gradient();
            double[] parameters = Model.Function.Weights;
            double stepSize = learningRate / convergence.CurrentIteration;

            // Update the model using a dynamic step size
            for (int i = 0; i < parameters.Length; i++)
            {
                if (Double.IsInfinity(parameters[i])) continue;

                parameters[i] -= stepSize * gradient[i];
            }

            return calculator.LastError;
        }


        /// <summary>
        ///   Raises the <see cref="E:ProgressChanged"/> event.
        /// </summary>
        /// 
        /// <param name="args">The ProgressChangedEventArgs instance containing the event data.</param>
        /// 
        protected void OnProgressChanged(ProgressChangedEventArgs args)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, args);
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
        ///   the <see cref="ForwardBackwardGradient{T}"/> is reclaimed by garbage
        ///   collection.
        /// </summary>
        /// 
        ~HiddenGradientDescentLearning()
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
