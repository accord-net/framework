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
    using System.Threading;
    using System.Threading.Tasks;
    using Accord.Math;

    /// <summary>
    ///   Stochastic Gradient Descent learning algorithm for <see cref="HiddenConditionalRandomField{T}">
    ///   Hidden Conditional Hidden Fields</see>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the observations.</typeparam>
    /// 
    /// <seealso cref="HiddenResilientGradientLearning{T}"/>
    /// 
    public class HiddenGradientDescentLearning<T> : IHiddenConditionalRandomFieldLearning<T>,
        IConvergenceLearning, IDisposable
    {
        private double learningRate = 100;
        private ISingleValueConvergence convergence;

        //private double decay = 0.9;
        //private double tau = 0.5;
        private double stepSize;

        private bool stochastic = true;
        private double[] gradient;

        private ForwardBackwardGradient<T> calculator;


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
        public int Iterations
        {
            get { return convergence.Iterations; }
            set { convergence.Iterations = value; }
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
        ///   Gets or sets the model being trained.
        /// </summary>
        /// 
        public HiddenConditionalRandomField<T> Model { get; private set; }

        /// <summary>
        ///   Occurs when the current learning progress has changed.
        /// </summary>
        /// 
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        ///   Initializes a new instance of the <see cref="HiddenGradientDescentLearning&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="model">The model to be trained.</param>
        /// 
        public HiddenGradientDescentLearning(HiddenConditionalRandomField<T> model)
        {
            Model = model;

            convergence = new RelativeConvergence();

            calculator = new ForwardBackwardGradient<T>(model);
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
#if SERIAL
                for (int i = 0; i < observations.Length; i++)
#else
                Parallel.For(0, observations.Length, i =>
#endif
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

                    System.Diagnostics.Debug.Assert(!gradient.HasNaN());
                }
#if !SERIAL
);
#endif

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

                System.Diagnostics.Debug.Assert(!Double.IsNaN(parameters[i]));
                System.Diagnostics.Debug.Assert(!Double.IsPositiveInfinity(parameters[i]));
            }


            return convergence.NewValue = error;
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
        public double Run(T[][] observations, int[] outputs)
        {
            convergence.Clear();

            do
            {
                RunEpoch(observations, outputs);
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
        /// <param name="args">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
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
