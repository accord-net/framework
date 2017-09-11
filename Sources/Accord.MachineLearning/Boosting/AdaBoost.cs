// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Darko Jurić, 2013
// https://code.google.com/p/accord/issues/detail?id=27
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

namespace Accord.MachineLearning.Boosting
{
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Math;
    using System;
    using System.Threading;
    using Accord.Statistics.Analysis;
    using Accord.Statistics;
    using Accord.Math.Optimization.Losses;
    using Accord.Compat;

    /// <summary>
    ///   Model construction (fitting) delegate.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the model to be created.</typeparam>
    /// <param name="weights">The current weights for the input samples.</param>
    /// <returns>A model trained over the weighted samples.</returns>
    /// 
    [Obsolete("Please use the .Learner property instead.")]
    public delegate TModel ModelConstructor<TModel>(double[] weights);

    /// <summary>
    ///   Extra parameters that can be passed to AdaBoost's model learning function.
    /// </summary>
    /// 
    public class AdaBoostParameters
    {
    }

    /// <summary>
    ///   AdaBoost learning algorithm.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the model to be trained.</typeparam>
    /// 
    /// <remarks>
    /// <para>
    ///   AdaBoost, short for "Adaptive Boosting", is a machine learning meta-algorithm
    ///   formulated by Yoav Freund and Robert Schapire who won the Gödel Prize in 2003
    ///   for their work. It can be used in conjunction with many other types of learning
    ///   algorithms to improve their performance. The output of the other learning algorithms 
    ///   ('weak learners') is combined into a weighted sum that represents the final output of
    ///   the boosted classifier. AdaBoost is adaptive in the sense that subsequent weak learners
    ///   are tweaked in favor of those instances misclassified by previous classifiers. AdaBoost
    ///   is sensitive to noisy data and outliers. In some problems it can be less susceptible to
    ///   the overfitting problem than other learning algorithms. The individual learners can be 
    ///   weak, but as long as the performance of each one is slightly better than random guessing 
    ///   (e.g., their error rate is smaller than 0.5 for binary classification), the final model
    ///   can be proven to converge to a strong learner.</para>
    /// <para>
    ///   Every learning algorithm will tend to suit some problem types better than others, and will 
    ///   typically have many different parameters and configurations to be adjusted before achieving
    ///   optimal performance on a dataset, AdaBoost(with decision trees as the weak learners) is often
    ///   referred to as the best out-of-the-box classifier. When used with decision tree learning,
    ///   information gathered at each stage of the AdaBoost algorithm about the relative 'hardness' 
    ///   of each training sample is fed into the tree growing algorithm such that later trees tend 
    ///   to focus on harder-to-classify examples.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       <a href="https://en.wikipedia.org/wiki/AdaBoost">
    ///        Wikipedia contributors. "AdaBoost." Wikipedia, The Free Encyclopedia. Wikipedia, The 
    ///        Free Encyclopedia, 10 Aug. 2017. Web. 7 Sep. 2017 </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Boosting\AdaBoostTest.cs" region="doc_learn" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Boosting\AdaBoostTest.cs" region="doc_learn_lr" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Boosting\AdaBoostTest.cs" region="doc_learn_dt" />
    /// </example>
    /// 
    public class AdaBoost<TModel> : ISupervisedBinaryLearning<Boost<TModel>, double[]>
        where TModel : IClassifier<double[], int>
    {

        RelativeConvergence convergence = new RelativeConvergence();

        double threshold = 0.5;


        /// <summary>
        ///   Initializes a new instance of the <see cref="AdaBoost&lt;TModel&gt;"/> class.
        /// </summary>
        /// 
        public AdaBoost()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="AdaBoost&lt;TModel&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="model">The model to be learned.</param>
        /// 
        public AdaBoost(Boost<TModel> model)
        {
            this.Model = model;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="AdaBoost&lt;TModel&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="model">The model to be learned.</param>
        /// <param name="creationFunction">The model fitting function.</param>
        /// 
        [Obsolete("Please use the parameterless constructor instead.")]
        public AdaBoost(Boost<TModel> model, ModelConstructor<TModel> creationFunction)
        {
            this.Model = model;
            this.Creation = creationFunction;
        }

        /// <summary>
        ///   Gets or sets the model being trained.
        /// </summary>
        /// 
        public Boost<TModel> Model { get; set; }

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
            get { return convergence.MaxIterations; }
            set { convergence.MaxIterations = value; }
        }

        /// <summary>
        ///   Gets or sets the relative tolerance used to 
        ///   detect convergence of the learning algorithm.
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return convergence.Tolerance; }
            set { convergence.Tolerance = value; }
        }

        /// <summary>
        ///   Gets or sets the error limit before learning stops. Default is 0.5.
        /// </summary>
        /// 
        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        /// <summary>
        ///   Gets or sets the fitting function which creates
        ///   and trains a model given a weighted data set.
        /// </summary>
        /// 
        [Obsolete("Please use the Learner property instead.")]
        public ModelConstructor<TModel> Creation { get; set; }

        /// <summary>
        ///   Gets or sets a function that takes a set of parameters and creates
        ///   a learning algorithm for learning each stage of the boosted classsifier.
        /// </summary>
        /// 
        public Func<AdaBoostParameters, ISupervisedLearning<TModel, double[], int>> Learner { get; set; }

        /// <summary>
        ///   Gets or sets a cancellation token that can be used to 
        ///   stop the learning algorithm while it is running.
        /// </summary>
        /// 
        public CancellationToken Token { get; set; }

        /// <summary>
        ///   Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// 
        /// <returns>A model that has learned how to produce <paramref name="y"/> given <paramref name="x"/>.</returns>
        /// 
        public Boost<TModel> Learn(double[][] x, int[][] y, double[] weights = null)
        {
            return Learn(x, y.ArgMax(dimension: 0), weights);
        }

        /// <summary>
        ///   Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// 
        /// <returns>A model that has learned how to produce <paramref name="y"/> given <paramref name="x"/>.</returns>
        /// 
        public Boost<TModel> Learn(double[][] x, bool[][] y, double[] weights = null)
        {
            return Learn(x, y.ArgMax(dimension: 0), weights);
        }

        /// <summary>
        ///   Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// 
        /// <returns>A model that has learned how to produce <paramref name="y"/> given <paramref name="x"/>.</returns>
        /// 
        public Boost<TModel> Learn(double[][] x, bool[] y, double[] weights = null)
        {
            return Learn(x, Classes.ToZeroOne(y), weights);
        }

        /// <summary>
        ///   Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// 
        /// <returns>A model that has learned how to produce <paramref name="y"/> given <paramref name="x"/>.</returns>
        /// 
        public Boost<TModel> Learn(double[][] x, int[] y, double[] weights = null)
        {
            if (weights == null)
                weights = Vector.Create(x.Length, 1.0 / x.Length);

            if (Model == null)
                Model = new Boost<TModel>();

            double error = 0;
            double weightSum = 0;

            var predicted = new int[y.Length];
            var parameters = new AdaBoostParameters();
            TModel model;

            do
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (Creation != null)
                {
                    model = Creation(weights);
                }
#pragma warning restore CS0618 // Type or member is obsolete
                else
                {
                    // Create and train a classifier
                    var learner = Learner(parameters);
                    model = learner.Learn(x, y, weights);
                }

                if (model == null)
                    break;

                // Determine its current accuracy
                model.Decide(x, result: predicted);

                error = 0;
                for (int i = 0; i < predicted.Length; i++)
                    if (predicted[i] != y[i])
                        error += weights[i];

                if (error >= threshold)
                    break;


                // AdaBoost
                double w = 0.5 * System.Math.Log((1.0 - error) / error);

                double sum = 0;
                for (int i = 0; i < weights.Length; i++)
                {
                    double d;
                    if (y[i] == predicted[i])
                        d = Math.Exp(-w);
                    else
                        d = Math.Exp(+w);

                    weights[i] *= d;
                    sum += weights[i];
                }

                // Update sample weights
                for (int i = 0; i < weights.Length; i++)
                    weights[i] /= sum;

                Model.Add(w, model);

                weightSum += w;

                convergence.NewValue = error;

            } while (!convergence.HasConverged);


            // Normalize weights for confidence calculation
            for (int i = 0; i < Model.Models.Count; i++)
                Model.Models[i].Weight /= weightSum;

            return Model;
        }

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <param name="inputs">The input samples.</param>
        /// <param name="outputs">The corresponding output labels.</param>
        /// 
        /// <returns>The classifier error.</returns>
        /// 
        [Obsolete("Please use the Learn(x, y) method instead.")]
        public double Run(double[][] inputs, int[] outputs)
        {
            double[] weights = new double[inputs.Length];
            for (int i = 0; i < weights.Length; i++)
                weights[i] = 1.0 / weights.Length;
            return Run(inputs, outputs, weights);
        }

        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// 
        /// <param name="inputs">The input samples.</param>
        /// <param name="outputs">The corresponding output labels.</param>
        /// <param name="sampleWeights">The weights for each of the samples.</param>
        /// 
        /// <returns>The classifier error.</returns>
        /// 
        [Obsolete("Please use the Learn(x, y) method instead.")]
        public double Run(double[][] inputs, int[] outputs, double[] sampleWeights)
        {
            Learn(inputs, Classes.Decide(outputs), (double[])sampleWeights.Clone());
            return ComputeError(inputs, outputs);
        }

        /// <summary>
        ///   Computes the error ratio, the number of
        ///   misclassifications divided by the total
        ///   number of samples in a dataset.
        /// </summary>
        /// 
        public double ComputeError(double[][] inputs, int[] outputs)
        {
            return new ZeroOneLoss(outputs).Loss(Model.Decide(inputs));
        }


    }
}
