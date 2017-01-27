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
    using System.Threading.Tasks;
    using Accord.Math;
    using Accord.MachineLearning;

    /// <summary>
    ///   Resilient Gradient Learning.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the observations being modeled.</typeparam>
    ///
    /// <example>
    /// <code>
    /// // Suppose we would like to learn how to classify the
    /// // following set of sequences among three class labels: 
    /// 
    /// int[][] inputSequences =
    /// {
    ///     // First class of sequences: starts and
    ///     // ends with zeros, ones in the middle:
    ///     new[] { 0, 1, 1, 1, 0 },        
    ///     new[] { 0, 0, 1, 1, 0, 0 },     
    ///     new[] { 0, 1, 1, 1, 1, 0 },     
    ///  
    ///     // Second class of sequences: starts with
    ///     // twos and switches to ones until the end.
    ///     new[] { 2, 2, 2, 2, 1, 1, 1, 1, 1 },
    ///     new[] { 2, 2, 1, 2, 1, 1, 1, 1, 1 },
    ///     new[] { 2, 2, 2, 2, 2, 1, 1, 1, 1 },
    ///  
    ///     // Third class of sequences: can start
    ///     // with any symbols, but ends with three.
    ///     new[] { 0, 0, 1, 1, 3, 3, 3, 3 },
    ///     new[] { 0, 0, 0, 3, 3, 3, 3 },
    ///     new[] { 1, 0, 1, 2, 2, 2, 3, 3 },
    ///     new[] { 1, 1, 2, 3, 3, 3, 3 },
    ///     new[] { 0, 0, 1, 1, 3, 3, 3, 3 },
    ///     new[] { 2, 2, 0, 3, 3, 3, 3 },
    ///     new[] { 1, 0, 1, 2, 3, 3, 3, 3 },
    ///     new[] { 1, 1, 2, 3, 3, 3, 3 },
    /// };
    /// 
    /// // Now consider their respective class labels
    /// int[] outputLabels =
    /// {
    ///     /* Sequences  1-3 are from class 0: */ 0, 0, 0,
    ///     /* Sequences  4-6 are from class 1: */ 1, 1, 1,
    ///     /* Sequences 7-14 are from class 2: */ 2, 2, 2, 2, 2, 2, 2, 2
    /// };
    /// 
    /// 
    /// // Create the Hidden Conditional Random Field using a set of discrete features
    /// var function = new MarkovDiscreteFunction(states: 3, symbols: 4, outputClasses: 3);
    /// var classifier = new HiddenConditionalRandomField&lt;int>(function);
    /// 
    /// // Create a learning algorithm
    /// var teacher = new HiddenResilientGradientLearning&lt;int>(classifier)
    /// {
    ///     Iterations = 50
    /// };
    /// 
    /// // Run the algorithm and learn the models
    /// teacher.Run(inputSequences, outputLabels);
    /// 
    /// 
    /// // After training has finished, we can check the 
    /// // output classification label for some sequences. 
    /// 
    /// int y1 = classifier.Compute(new[] { 0, 1, 1, 1, 0 });    // output is y1 = 0
    /// int y2 = classifier.Compute(new[] { 0, 0, 1, 1, 0, 0 }); // output is y1 = 0
    /// 
    /// int y3 = classifier.Compute(new[] { 2, 2, 2, 2, 1, 1 }); // output is y2 = 1
    /// int y4 = classifier.Compute(new[] { 2, 2, 1, 1 });       // output is y2 = 1
    /// 
    /// int y5 = classifier.Compute(new[] { 0, 0, 1, 3, 3, 3 }); // output is y3 = 2
    /// int y6 = classifier.Compute(new[] { 2, 0, 2, 2, 3, 3 }); // output is y3 = 2
    /// </code>
    /// </example>
    /// 
    public class HiddenResilientGradientLearning<T> :
        ISupervisedLearning<HiddenConditionalRandomField<T>, T[], int>,
        IHiddenConditionalRandomFieldLearning<T>,
        IConvergenceLearning, IDisposable
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        private ForwardBackwardGradient<T> calculator;
        private ISingleValueConvergence convergence;

        private double initialStep = 0.0125;
        private double deltaMax = 50.0;
        private double deltaMin = 1e-6;

        private double etaMinus = 0.5;
        private double etaPlus = 1.2;
        private bool stochastic = true;

        private double[] gradient;
        private double[] previousGradient;

        private Object lockObj = new Object();

        // update values, also known as deltas
        private double[] weightsUpdates;

        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }

        /// <summary>
        ///   Gets or sets the model being trained.
        /// </summary>
        /// 
        public HiddenConditionalRandomField<T> Model { get; private set; }

        /// <summary>
        ///   Gets or sets a value indicating whether this <see cref="HiddenGradientDescentLearning&lt;T&gt;"/>
        ///   should use stochastic gradient updates. Default is true.
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
        ///   Occurs when the current learning progress has changed.
        /// </summary>
        /// 
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        ///   Gets or sets the maximum possible update step,
        ///   also referred as delta min. Default is 50.
        /// </summary>
        /// 
        public double UpdateUpperBound
        {
            get { return deltaMax; }
            set { deltaMax = value; }
        }

        /// <summary>
        ///   Gets or sets the minimum possible update step,
        ///   also referred as delta max. Default is 1e-6.
        /// </summary>
        /// 
        public double UpdateLowerBound
        {
            get { return deltaMin; }
            set { deltaMin = value; }
        }

        /// <summary>
        ///   Gets the decrease parameter, also 
        ///   referred as eta minus. Default is 0.5.
        /// </summary>
        /// 
        public double DecreaseFactor
        {
            get { return etaMinus; }
            set
            {
                if (value <= 0 || value >= 1)
                    throw new ArgumentOutOfRangeException("value", "Value should be between 0 and 1.");
                etaMinus = value;
            }
        }

        /// <summary>
        ///   Gets the increase parameter, also
        ///   referred as eta plus. Default is 1.2.
        /// </summary>
        /// 
        public double IncreaseFactor
        {
            get { return etaPlus; }
            set
            {
                if (value <= 1)
                    throw new ArgumentOutOfRangeException("value", "Value should be higher than 1.");
                etaPlus = value;
            }
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
        ///   Initializes a new instance of the <see cref="HiddenResilientGradientLearning{T}"/> class.
        /// </summary>
        /// 
        /// <param name="model">Model to teach.</param>
        /// 
        public HiddenResilientGradientLearning(HiddenConditionalRandomField<T> model)
        {
            Model = model;

            calculator = new ForwardBackwardGradient<T>(model);
            convergence = new RelativeConvergence(iterations: 100, tolerance: 0, checks: 3);

            int parameters = Model.Function.Weights.Length;
            gradient = new double[parameters];
            previousGradient = new double[parameters];
            weightsUpdates = new double[parameters];

            // Initialize steps
            Reset(initialStep);
        }



        /// <summary>
        ///   Runs one iteration of the learning algorithm with the
        ///   specified input training observation and corresponding
        ///   output label.
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
            return run(observations, outputs);
        }

        private double run(T[][] observations, int[] outputs)
        {
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


#if SERIAL      // For each training point
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

                    Accord.Diagnostics.Debug.Assert(!gradient.HasNaN());
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

            // Do the Resilient Backpropagation parameter update
            for (int k = 0; k < calculator.Parameters.Length; k++)
            {
                if (Double.IsInfinity(parameters[k])) continue;

                double S = previousGradient[k] * gradient[k];

                if (S > 0.0)
                {
                    weightsUpdates[k] = Math.Min(weightsUpdates[k] * etaPlus, deltaMax);
                    parameters[k] -= Math.Sign(gradient[k]) * weightsUpdates[k];
                    previousGradient[k] = gradient[k];
                }
                else if (S < 0.0)
                {
                    weightsUpdates[k] = Math.Max(weightsUpdates[k] * etaMinus, deltaMin);
                    previousGradient[k] = 0.0;
                }
                else
                {
                    parameters[k] -= Math.Sign(gradient[k]) * weightsUpdates[k];
                    previousGradient[k] = gradient[k];
                }
            }

            Accord.Diagnostics.Debug.Assert(!Model.Function.Weights.HasNaN());

            return convergence.NewValue = error;
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
            return RunEpoch(new[] { observations }, new[] { output });
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

        /// <summary>
        ///   Resets the current update steps using the given learning rate.
        /// </summary>
        /// 
        public void Reset(double rate)
        {
            convergence.Clear();

            Parallel.For(0, weightsUpdates.Length, i =>
            {
                for (int j = 0; j < weightsUpdates.Length; j++)
                    weightsUpdates[i] = rate;
            });
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair.</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public HiddenConditionalRandomField<T> Learn(T[][] x, int[] y, double[] weights = null)
        {
            run(x, y);
            return Model;
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
        ///   the <see cref="HiddenResilientGradientLearning{T}"/> is reclaimed by garbage
        ///   collection.
        /// </summary>
        /// 
        ~HiddenResilientGradientLearning()
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