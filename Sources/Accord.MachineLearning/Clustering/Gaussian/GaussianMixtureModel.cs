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

namespace Accord.MachineLearning
{
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using System;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Gaussian mixture model clustering.
    /// </summary>
    /// 
    /// <remarks>
    ///   Gaussian Mixture Models are one of most widely used model-based 
    ///   clustering methods. This specialized class provides a wrap-up
    ///   around the
    ///   <see cref="Statistics.Distributions.Multivariate.MultivariateMixture{Normal}">
    ///   Mixture&lt;NormalDistribution&gt;</see> distribution and provides
    ///   mixture initialization using the K-Means clustering algorithm.
    /// </remarks>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Clustering\GaussianMixtureModelTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="KMeans"/> 
    /// <seealso cref="MeanShift"/> 
    /// 
    [Serializable]
    public class GaussianMixtureModel : ParallelLearningBase,
        IUnsupervisedLearning<GaussianClusterCollection, double[], int>,
#pragma warning disable 0618
        IClusteringAlgorithm<double[], double>
#pragma warning restore 0618
    {
        private GaussianClusterCollection clusters;

        /// <summary>
        ///   Gets or sets the maximum number of iterations to
        ///   be performed by the method. If set to zero, no
        ///   iteration limit will be imposed. Default is 0.
        /// </summary>
        /// 
        public int MaxIterations { get; set; }

        /// <summary>
        ///   Gets or sets the convergence criterion for the
        ///   Expectation-Maximization algorithm. Default is 1e-3.
        /// </summary>
        /// 
        /// <value>The convergence threshold.</value>
        /// 
        public double Tolerance { get; set; }

        /// <summary>
        ///   Gets or sets whether cluster labels should be computed
        ///   at the end of the learning iteration. Setting to <c>False</c>
        ///   might save a few computations in case they are not necessary.
        /// </summary>
        /// 
        public bool ComputeLabels { get; set; }

        /// <summary>
        ///   Gets or sets whether the log-likelihood should be computed
        ///   at the end of the learning iteration. Setting to <c>False</c>
        ///   might save a few computations in case they are not necessary.
        /// </summary>
        /// 
        public bool ComputeLogLikelihood { get; set; }

        /// <summary>
        ///   Gets the log-likelihood of the model at the last iteration.
        /// </summary>
        /// 
        public double LogLikelihood { get; private set; }

        /// <summary>
        ///   Gets or sets how many random initializations to try. 
        ///   Default is 3.
        /// </summary>
        /// 
        public int Initializations { get; set; }

        /// <summary>
        ///   Gets how many iterations have been performed in the last call
        ///   to <see cref="Compute(double[][])"/>.
        /// </summary>
        /// 
        public int Iterations { get; private set; }

        /// <summary>
        ///   Gets or sets whether to make computations using the log
        ///   -domain. This might improve accuracy on large datasets.
        /// </summary>
        /// 
        public bool UseLogarithm { get; set; }

        /// <summary>
        ///   Gets or sets the fitting options for the component
        ///   Gaussian distributions of the mixture model.
        /// </summary>
        /// 
        /// <value>The fitting options for inner Gaussian distributions.</value>
        /// 
        public NormalOptions Options { get; set; }

        /// <summary>
        ///   Gets the Gaussian components of the mixture model.
        /// </summary>
        /// 
        public GaussianClusterCollection Gaussians
        {
            get { return clusters; }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="GaussianMixtureModel"/> class.
        /// </summary>
        /// 
        /// <param name="components">
        ///   The number of clusters in the clustering problem. This will be
        ///   used to set the number of components in the mixture model.</param>
        ///   
        public GaussianMixtureModel(int components)
        {
            if (components <= 0)
            {
                throw new ArgumentOutOfRangeException("components",
                    "The number of components should be greater than zero.");
            }

            constructor(components);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GaussianMixtureModel"/> class.
        /// </summary>
        /// 
        /// <param name="kmeans">
        ///   The initial solution as a K-Means clustering.</param>
        ///   
        public GaussianMixtureModel(KMeans kmeans)
        {
            if (kmeans == null)
                throw new ArgumentNullException("kmeans");

            constructor(kmeans.K);

            clusters.Initialize(kmeans);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GaussianMixtureModel"/> class.
        /// </summary>
        /// 
        /// <param name="mixture">
        ///   The initial solution as a mixture of normal distributions.</param>
        ///   
        public GaussianMixtureModel(Mixture<NormalDistribution> mixture)
        {
            if (mixture == null)
                throw new ArgumentNullException("mixture");

            constructor(mixture.Components.Length);

            clusters.Initialize(mixture);
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="GaussianMixtureModel"/> class.
        /// </summary>
        /// 
        /// <param name="mixture">
        ///   The initial solution as a mixture of normal distributions.</param>
        ///   
        public GaussianMixtureModel(MultivariateMixture<MultivariateNormalDistribution> mixture)
        {
            if (mixture == null)
                throw new ArgumentNullException("mixture");

            constructor(mixture.Components.Length);

            clusters.Initialize(mixture);
        }

        private void constructor(int components)
        {
            ComputeLabels = true;
            ComputeLogLikelihood = true;
            Tolerance = 1e-3;
            Initializations = 3;
            MaxIterations = 0;
            UseLogarithm = true;
            Options = new Statistics.Distributions.Fitting.NormalOptions();
            ParallelOptions = new ParallelOptions();

            // Initialize the model using the created objects.
            this.clusters = new GaussianClusterCollection(components);
        }

        /// <summary>
        ///   Divides the input data into K clusters modeling each
        ///   cluster as a multivariate Gaussian distribution. 
        /// </summary>     
        /// 
        [Obsolete("Please use Learn(x) instead.")]
        public int[] Compute(double[][] data)
        {
            return Learn(data).Decide(data);
        }

        /// <summary>
        ///   Divides the input data into K clusters modeling each
        ///   cluster as a multivariate Gaussian distribution. 
        /// </summary>     
        /// 
        [Obsolete("Please use Learn(x) instead.")]
        public int[] Compute(double[][] data, double[] weights)
        {
            return Learn(data, weights).Decide(data);
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// <returns>A model that has learned how to produce suitable outputs
        /// given the input data <paramref name="x" />.</returns>
        public GaussianClusterCollection Learn(double[][] x, double[] weights = null)
        {
            if (clusters.Model == null)
                Initialize(x, weights);

            // Create the mixture options
            var mixtureOptions = new MixtureOptions()
            {
                Threshold = this.Tolerance,
                InnerOptions = this.Options,
                MaxIterations = this.MaxIterations,
                Logarithm = this.UseLogarithm,
                ParallelOptions = ParallelOptions,
            };

            MultivariateMixture<MultivariateNormalDistribution> model = clusters.Model;

            // Fit a multivariate Gaussian distribution
            model.Fit(x, weights, mixtureOptions);
#pragma warning disable 612, 618
            this.Iterations = mixtureOptions.Iterations;
#pragma warning restore 612, 618

            for (int i = 0; i < clusters.Model.Components.Length; i++)
                clusters.Centroids[i] = new MixtureComponent<MultivariateNormalDistribution>(model, i);

            if (ComputeLogLikelihood)
                LogLikelihood = model.LogLikelihood(x);

            Accord.Diagnostics.Debug.Assert(clusters.NumberOfClasses == clusters.Model.Components.Length);
            Accord.Diagnostics.Debug.Assert(clusters.NumberOfOutputs == clusters.Model.Components.Length);
            Accord.Diagnostics.Debug.Assert(clusters.NumberOfInputs == x[0].Length);

            return clusters;
        }

        /// <summary>
        ///   Initializes the model with initial values obtained 
        ///   through a run of the K-Means clustering algorithm.
        /// </summary>
        /// 
        public void Initialize(double[][] data, double[] weights)
        {
            var kmeans = new KMeans[Initializations];
            double[] errors = new double[Initializations];

            Parallel.For(0, kmeans.Length, ParallelOptions, i =>
            {
                // Create a new K-Means algorithm
                kmeans[i] = new KMeans(clusters.Count)
                {
                    ComputeCovariances = true,
                    ComputeError = true,
                    UseSeeding = Seeding.KMeansPlusPlus,
                    Tolerance = Tolerance
                };

                // Compute the K-Means
                kmeans[i].Learn(data, weights);

                errors[i] = kmeans[i].Error;
            });

            int index = errors.ArgMin();

            // Initialize the model with K-Means
            clusters.Initialize(kmeans[index]);
        }

        /// <summary>
        ///   Initializes the model with initial values obtained 
        ///   through a run of the K-Means clustering algorithm.
        /// </summary>
        /// 
        public void Initialize(KMeans kmeans)
        {
            clusters.Initialize(kmeans);
        }

        /// <summary>
        ///   Initializes the model with initial values.
        /// </summary>
        /// 
        public void Initialize(MultivariateNormalDistribution[] distributions)
        {
            clusters.Initialize(distributions);
        }

        /// <summary>
        ///   Initializes the model with initial values.
        /// </summary>
        /// 
        public void Initialize(NormalDistribution[] distributions)
        {
            clusters.Initialize(distributions);
        }

        /// <summary>
        ///   Initializes the model with initial values.
        /// </summary>
        /// 
        public void Initialize(double[] coefficients, MultivariateNormalDistribution[] distributions)
        {
            clusters.Initialize(coefficients, distributions);
        }

        /// <summary>
        ///   Initializes the model with initial values.
        /// </summary>
        /// 
        public void Initialize(double[] coefficients, NormalDistribution[] distributions)
        {
            clusters.Initialize(coefficients, distributions);
        }

        /// <summary>
        ///   Initializes the model with initial values.
        /// </summary>
        /// 
        public void Initialize(MultivariateMixture<MultivariateNormalDistribution> mixture)
        {
            clusters.Initialize(mixture);
        }

        /// <summary>
        ///   Initializes the model with initial values.
        /// </summary>
        /// 
        public void Initialize(Mixture<NormalDistribution> mixture)
        {
            clusters.Initialize(mixture);
        }




        /// <summary>
        ///   Gets a copy of the mixture distribution modeled by this Gaussian Mixture Model.
        /// </summary>
        /// 
        public MultivariateMixture<MultivariateNormalDistribution> ToMixtureDistribution()
        {
            return clusters.ToMixtureDistribution();
        }



        # region Deprecated
        /// <summary>
        ///   Divides the input data into K clusters modeling each
        ///   cluster as a multivariate Gaussian distribution. 
        /// </summary>     
        /// 
        [Obsolete("Please set the properties of this class directly and use Learn(double[]) instead.")]
        public double Compute(double[][] data, GaussianMixtureModelOptions options)
        {
            this.Options = options.NormalOptions;
            this.MaxIterations = options.Iterations;
            this.UseLogarithm = options.Logarithm;
            this.Tolerance = options.Threshold;
            this.ParallelOptions = options.ParallelOptions;

            Compute(data, options.Weights);

            return LogLikelihood;
        }

        /// <summary>
        ///   Divides the input data into K clusters modeling each
        ///   cluster as a multivariate Gaussian distribution. 
        /// </summary>     
        /// 
        [Obsolete("Please set the tolerance threshold using the Tolerance property and use Compute(double[]) instead.")]
        public double Compute(double[][] data, double threshold)
        {
            this.Tolerance = threshold;
            Learn(data);
            return LogLikelihood;
        }
        #endregion


#pragma warning disable 0618
        /// <summary>
        ///   Gets the collection of clusters currently modeled by the
        ///   clustering algorithm.
        /// </summary>
        /// 
        [Obsolete("Please use Gaussians instead")]
        public IClusterCollection<double[]> Clusters
        {
            get { return (IClusterCollection<double[]>)Gaussians; }
        }

        IClusterCollection<double[]> IUnsupervisedLearning<IClusterCollection<double[]>, double[], int>.Learn(double[][] x, double[] weights)
        {
            return Learn(x);
        }
#pragma warning restore 0618
    }

    /// <summary>
    ///   Options for Gaussian Mixture Model fitting.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class provides different options that can be passed to a 
    ///   <see cref="GaussianMixtureModel"/> object when calling its
    ///   <see cref="GaussianMixtureModel.Compute(double[][], GaussianMixtureModelOptions)"/>
    ///   method.
    /// </remarks>
    /// 
    /// <seealso cref="GaussianMixtureModel"/>
    /// 
    [Obsolete("This class will be removed.")]
    public class GaussianMixtureModelOptions
    {

        /// <summary>
        ///   Gets or sets the convergence criterion for the
        ///   Expectation-Maximization algorithm. Default is 1e-3.
        /// </summary>
        /// 
        /// <value>The convergence threshold.</value>
        /// 
        public double Threshold { get; set; }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   to be performed by the Expectation-Maximization
        ///   algorithm. Default is zero (iterate until convergence).
        /// </summary>
        /// 
        public int Iterations { get; set; }

        /// <summary>
        ///   Gets or sets whether to make computations using the log
        ///   -domain. This might improve accuracy on large datasets.
        /// </summary>
        /// 
        public bool Logarithm { get; set; }

        /// <summary>
        ///   Gets or sets the sample weights. If set to null,
        ///   the data will be assumed equal weights. Default
        ///   is null.
        /// </summary>
        /// 
        public double[] Weights { get; set; }

        /// <summary>
        ///   Gets or sets the fitting options for the component
        ///   Gaussian distributions of the mixture model.
        /// </summary>
        /// 
        /// <value>The fitting options for inner Gaussian distributions.</value>
        /// 
        public NormalOptions NormalOptions { get; set; }

        /// <summary>
        ///   Gets or sets parallelization options.
        /// </summary>
        /// 
        public ParallelOptions ParallelOptions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianMixtureModelOptions"/> class.
        /// </summary>
        /// 
        public GaussianMixtureModelOptions()
        {
            Threshold = 1e-3;
            ParallelOptions = new ParallelOptions();
        }

    }
}
