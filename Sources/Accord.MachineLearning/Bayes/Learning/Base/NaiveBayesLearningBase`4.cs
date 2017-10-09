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

namespace Accord.MachineLearning.Bayes
{
#if !MONO
    using System;
    using System.Linq;
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Compat;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///   Base class for Naive Bayes learning algorithms.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type for the Naive Bayes model to be learned.</typeparam>
    /// <typeparam name="TDistribution">The univariate distribution to be used as components in the Naive Bayes distribution.</typeparam>
    /// <typeparam name="TInput">The type for the samples modeled by the distribution.</typeparam>
    /// <typeparam name="TOptions">The fitting options for the independent distribution.</typeparam>
    /// 
    public abstract class NaiveBayesLearningBase<TModel, TDistribution, TInput, TOptions> :
        ISupervisedLearning<TModel, TInput[], double[]>,
        ISupervisedLearning<TModel, TInput[], int>,
        IParallel
        where TDistribution : IFittableDistribution<TInput>,
                              IUnivariateDistribution<TInput>,
                              IUnivariateDistribution
        where TOptions : IndependentOptions, new()
        where TModel : NaiveBayes<TDistribution, TInput>
    {
        internal bool optimized = false;

        /// <summary>
        /// Gets or sets the parallelization options for this algorithm.
        /// </summary>
        public ParallelOptions ParallelOptions { get; set; }

        /// <summary>
        ///   Gets or sets the model being learned.
        /// </summary>
        /// 
        public TModel Model { get; set; }

        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        public CancellationToken Token
        {
            get { return ParallelOptions.CancellationToken; }
            set { ParallelOptions.CancellationToken = value; }
        }

        /// <summary>
        /// Gets or sets whether the class priors should be estimated
        /// from the data.
        /// </summary>
        /// 
        public bool Empirical { get; set; }

        /// <summary>
        ///   Gets or sets the fitting options to use when
        ///   estimating the class-specific distributions.
        /// </summary>
        ///
        public TOptions Options { get; set; }

        /// <summary>
        ///   Gets or sets the distribution creation function. This function can
        ///   be used to specify how the initial distributions of the model should
        ///   be created. By default, this function attempts to call the empty
        ///   constructor of the distribution using <c>Activator.CreateInstance()</c>.
        /// </summary>
        /// 
        public Func<int, int, TDistribution> Distribution { get; set; }

        /// <summary>
        ///   Constructs a new Naïve Bayes learning algorithm.
        /// </summary>
        ///
        public NaiveBayesLearningBase()
        {
            this.Empirical = true;
            this.ParallelOptions = new ParallelOptions();
            this.Options = new TOptions();
            this.Options.Transposed = true;
            this.Distribution = (classIndex, variableIndex) =>
            {
                try
                {
                    return Activator.CreateInstance<TDistribution>();
                }
                catch
                {
                    throw new InvalidOperationException("Please set the Distribution property to specify how the initial distributions should be created.");
                }
            };
        }

        /// <summary>
        ///   Creates an instance of the model to be learned. Inheritors
        ///   of this abstract class must define this method so new models
        ///   can be created from the training data.
        /// </summary>
        /// 
        protected abstract TModel Create(TInput[][] x, int y);

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        ///
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weight">The weight of importance for each input-output pair.</param>
        ///
        /// <returns>
        ///   A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        ///
        public virtual TModel Learn(TInput[][] x, int[] y, double[] weight = null)
        {
            Accord.MachineLearning.Tools.CheckArgs(x, y, weight, () =>
            {
                if (Model == null)
                    Model = Create(x, y.Max() + 1);
                return Model;
            });

            if (ParallelOptions.MaxDegreeOfParallelism == 1)
            {
                for (int i = 0; i < Model.NumberOfOutputs; i++)
                    InnerLearn(x, y, weight, i);
            }
            else
            {
                // For each class
                Parallel.For(0, Model.NumberOfOutputs, ParallelOptions, i =>
                    InnerLearn(x, y, weight, i));
            }

            return Model;
        }

        private void InnerLearn(TInput[][] x, int[] y, double[] weight, int classIndex)
        {
            // Estimate conditional distributions
            // Get variables values in class i
            int[] sampleIndicesInClass = y.Find(y_i => y_i == classIndex);
            TInput[][] samplesInClass = x.Get(sampleIndicesInClass, transpose: true);

            if (Empirical)
                Model.Priors[classIndex] = sampleIndicesInClass.Length / (double)x.Length;

            Fit(classIndex, values: samplesInClass, weights: weight, transposed: true);
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        ///
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weight">The weight of importance for each input-output pair.</param>
        /// 
        /// <returns>
        ///   A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        ///
        public virtual TModel Learn(TInput[][] x, int[][] y, double[] weight = null)
        {
            return Learn(x, y.ToDouble(), weight);
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        ///
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weight">The weight of importance for each input-output pair.</param>
        /// 
        /// <returns>
        ///   A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        ///
        public virtual TModel Learn(TInput[][] x, double[][] y, double[] weight = null)
        {
            Accord.MachineLearning.Tools.CheckArgs(x, y, weight, () =>
            {
                if (Model == null)
                    Model = Create(x, y.Columns());
                return Model;
            });

            // For efficiency
            x = x.Transpose();

            if (ParallelOptions.MaxDegreeOfParallelism == 1)
            {
                // For each class
                for (int i = 0; i < Model.NumberOfOutputs; i++)
                {
                    InnerLearn(x, y, weight, i);
                }
            }
            else
            {
                // For each class
                Parallel.For(0, Model.NumberOfOutputs, ParallelOptions, i =>
                    InnerLearn(x, y, weight, i));
            }

            return Model;
        }

        private void InnerLearn(TInput[][] x, double[][] y, double[] weight, int classIndex)
        {
            // Estimate conditional distributions
            // Get variables values in class i
            double[] target = y.GetColumn(classIndex);

            if (weight != null)
                target.Multiply(weight, result: target);

            if (Empirical)
                Model.Priors[classIndex] = target.Sum() / x.Length;

            Fit(classIndex, values: x, weights: target, transposed: true);
        }

        /// <summary>
        ///    Fits one of the distributions in the naive bayes model.
        /// </summary>
        /// 
        protected virtual void Fit(int i, TInput[][] values, double[] weights, bool transposed)
        {
            Options.Transposed = transposed;

            var fit = Model.Distributions[i] as IFittableDistribution<TInput[], TOptions>;
            if (fit != null)
            {
                // Use a more efficient call if available
                fit.Fit(values, weights, Options);
                this.optimized = true;
            }
            else
            {
                // Use a generic call (which might need type casting)
                Model.Distributions[i].Fit(values, weights, Options);
                this.optimized = false;
            }
        }

    }
#endif
}