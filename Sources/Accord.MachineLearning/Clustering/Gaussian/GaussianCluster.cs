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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Accord.Math;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Math.Distances;
    using Accord.Statistics.Distributions.Univariate;
    using System.Collections;
    using Accord.Compat;

    using Centroid = Statistics.Distributions.Multivariate.IMixtureComponent<Statistics.Distributions.Multivariate.MultivariateNormalDistribution>;

    /// <summary>
    ///   Gaussian Mixture Model Cluster Collection.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class contains information about all <see cref="GaussianCluster">
    ///   Gaussian clusters</see> found during a <see cref="GaussianMixtureModel"/> 
    ///   estimation. </para>
    /// <para>
    ///   Given a new sample, this class can be used to find the nearest cluster related
    ///   to this sample through the Nearest method. </para>
    /// </remarks>
    /// 
    /// <seealso cref="GaussianMixtureModel"/>
    /// <seealso cref="GaussianCluster"/>
    /// 
    [Serializable]
    public class GaussianClusterCollection : MulticlassLikelihoodClassifierBase<double[]>,
        ICentroidClusterCollection<double[], Centroid, GaussianClusterCollection.GaussianCluster>,
#pragma warning disable 0618
        IClusterCollection<double[]>
#pragma warning restore 0618
    {
        GaussianCluster.ClusterCollection collection;

        MultivariateMixture<MultivariateNormalDistribution> model;


        /// <summary>
        ///   Gaussian Mixture Model cluster.
        /// </summary>
        /// 
        /// <remarks>
        ///   This class contains information about a Gaussian cluster found
        ///   during a <see cref="GaussianMixtureModel"/> estimation. Clusters
        ///   are often contained within a <see cref="GaussianClusterCollection"/>.
        /// </remarks>
        /// 
        /// <seealso cref="GaussianMixtureModel"/>
        /// <seealso cref="GaussianClusterCollection"/>
        /// 
        [Serializable]
        public class GaussianCluster : CentroidCluster<GaussianClusterCollection, double[], Centroid, GaussianCluster>, Centroid
        {
            /// <summary>
            ///   Gets the probability density function of the
            ///   underlying Gaussian probability distribution 
            ///   evaluated in point <c>x</c>.
            /// </summary>
            /// 
            /// <param name="x">An observation.</param>
            /// 
            /// <returns>
            ///   The log-probability of <c>x</c> occurring
            ///   in the weighted Gaussian distribution.
            /// </returns>
            /// 
            public double LogLikelihood(double[] x)
            {
                if (Owner.Model == null)
                    return Double.NegativeInfinity;
                return Owner.Model.LogProbabilityDensityFunction(Index, x);
            }

            /// <summary>
            ///   Gets the probability density function of the
            ///   underlying Gaussian probability distribution 
            ///   evaluated in point <c>x</c>.
            /// </summary>
            /// 
            /// <param name="x">An observation.</param>
            /// 
            /// <returns>
            ///   The probability of <c>x</c> occurring
            ///   in the weighted Gaussian distribution.
            /// </returns>
            /// 
            public double Likelihood(double[] x)
            {
                if (Owner.Model == null)
                    return 0;
                return Owner.Model.ProbabilityDensityFunction(Index, x);
            }

            /// <summary>
            ///   Gets a copy of the normal distribution associated with this cluster.
            /// </summary>
            /// 
            public MultivariateNormalDistribution ToDistribution()
            {
                return (MultivariateNormalDistribution)Owner.Model.Components[Index].Clone();
            }

            /// <summary>
            ///   Gets the deviance of the points in relation to the cluster.
            /// </summary>
            /// 
            /// <param name="points">The input points.</param>
            /// 
            /// <returns>The deviance, measured as -2 * the log-likelihood
            /// of the input points in this cluster.</returns>
            /// 
            public double Deviance(double[] points)
            {
                if (Owner.Model == null)
                    return Double.PositiveInfinity;
                return -2 * Owner.Model.LogProbabilityDensityFunction(Index, points);
            }

            /// <summary>
            ///   Gets the mean vector associated with this cluster.
            /// </summary>
            /// 
            public double[] Mean
            {
                get
                {
                    if (Owner.Model == null)
                        return null;
                    return Owner.Model.Components[Index].Mean;
                }
            }
            /// <summary>
            ///   Gets the variance vector associated with this cluster.
            /// </summary>
            /// 
            public double[] Variance
            {
                get
                {
                    if (Owner.Model == null)
                        return null;
                    return Owner.Model.Components[Index].Variance;
                }
            }

            /// <summary>
            ///   Gets the clusters' variance-covariance matrices.
            /// </summary>
            /// 
            /// <value>The clusters' variance-covariance matrices.</value>
            /// 
            public double[,] Covariance
            {
                get
                {
                    if (Owner.Model == null)
                        return null;
                    return Owner.Model.Components[Index].Covariance;
                }
            }

            /// <summary>
            ///   Gets the cluster's coefficient component.
            /// </summary>
            /// 
            public double Coefficient
            {
                get
                {
                    if (Owner.Model == null)
                        return 0;
                    return Owner.Model.Coefficients[Index];
                }
            }

            /// <summary>
            ///   Gets the component distribution.
            /// </summary>

            public MultivariateNormalDistribution Component
            {
                get { return Owner.Model.Components[Index]; }
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GaussianClusterCollection"/> class.
        /// </summary>
        /// 
        /// <param name="components">The number of components in the mixture.</param>
        /// 
        public GaussianClusterCollection(int components)
        {
            this.collection = new GaussianCluster.ClusterCollection(this, components, new LogLikelihood<MultivariateNormalDistribution>());
        }

        /// <summary>
        /// Gets the proportion of samples in each cluster.
        /// </summary>
        /// 
        public double[] Proportions
        {
            get
            {
                if (model == null)
                    return null;
                return model.Coefficients;
            }
        }

        /// <summary>
        ///   Gets the mean vectors for the clusters.
        /// </summary>
        /// 
        public double[][] Means
        {
            get
            {
                if (model == null)
                    return null;
                return model.Components.Apply(x => x.Mean);
            }
        }

        /// <summary>
        ///   Gets the variance for each of the clusters.
        /// </summary>
        /// 
        public double[][] Variance
        {
            get
            {
                if (model == null)
                    return null;
                return model.Components.Apply(x => x.Variance);
            }
        }

        /// <summary>
        ///   Gets the covariance matrices for each of the clusters.
        /// </summary>
        /// 
        public double[][,] Covariance
        {
            get
            {
                if (model == null)
                    return null;
                return model.Components.Apply(x => x.Covariance);
            }
        }

        /// <summary>
        ///   Gets the mixture model represented by this clustering.
        /// </summary>
        /// 
        public MultivariateMixture<MultivariateNormalDistribution> Model
        {
            get { return model; }
        }


#if DEBUG
        public override double Score(double[] input, int classIndex)
        {
            double score = base.Score(input, classIndex);
            double dist = Distance.Distance(input, Centroids[classIndex]);
            if (!score.IsEqual(-dist, rtol: 1e-5))
                throw new Exception();
            return score;
        }
#endif

        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <returns>System.Double.</returns>
        public override double LogLikelihood(double[] input, int classIndex)
        {
            double log = model.LogProbabilityDensityFunction(classIndex, input);
#if DEBUG
            double dist = Distance.Distance(input, Centroids[classIndex]);
            if (!log.IsEqual(-dist, rtol: 1e-5))
                throw new Exception();
#endif
            return log;
        }

        /// <summary>
        ///   Gets the deviance of the points in relation to the model.
        /// </summary>
        /// 
        /// <param name="points">The input points.</param>
        /// 
        /// <returns>The deviance, measured as -2 * the log-likelihood of the input points.</returns>
        /// 
        public double Deviance(double[][] points)
        {
            return -2 * model.LogLikelihood(points);
        }

        /// <summary>
        ///   Gets a copy of the mixture distribution modeled by this Gaussian Mixture Model.
        /// </summary>
        /// 
        public MultivariateMixture<MultivariateNormalDistribution> ToMixtureDistribution()
        {
            return (MultivariateMixture<MultivariateNormalDistribution>)model.Clone();
        }

        /// <summary>
        ///   Initializes the model with initial values obtained 
        ///   through a run of the K-Means clustering algorithm.
        /// </summary>
        /// 
        public void Initialize(KMeans kmeans)
        {
            if (kmeans.K != Count)
                throw new ArgumentException("The number of clusters does not match.", "kmeans");

            this.NumberOfInputs = kmeans.Clusters.NumberOfInputs;

            // Initialize the Mixture Model with data from K-Means
            var proportions = kmeans.Clusters.Proportions;
            var distributions = new MultivariateNormalDistribution[Count];

            for (int i = 0; i < Count; i++)
            {
                double[] mean = kmeans.Clusters.Centroids[i];
                double[][] covariance = kmeans.Clusters.Covariances[i];

                if (covariance == null || !covariance.IsPositiveDefinite())
                    covariance = Jagged.Identity(kmeans.Dimension);

                distributions[i] = new MultivariateNormalDistribution(mean, covariance);
            }

            this.model = new MultivariateMixture<MultivariateNormalDistribution>(proportions, distributions);
        }

        /// <summary>
        ///   Initializes the model with initial values.
        /// </summary>
        /// 
        public void Initialize(MultivariateNormalDistribution[] distributions)
        {
            if (distributions.Length != Count)
                throw new ArgumentException("The number of distributions and clusters does not match.", "distributions");

            this.model = new MultivariateMixture<MultivariateNormalDistribution>(distributions);
        }

        /// <summary>
        ///   Initializes the model with initial values.
        /// </summary>
        /// 
        public void Initialize(NormalDistribution[] distributions)
        {
            if (distributions.Length != Count)
                throw new ArgumentException("The number of distributions and clusters does not match.", "distributions");

            var normals = new MultivariateNormalDistribution[distributions.Length];
            for (int i = 0; i < normals.Length; i++)
                normals[i] = distributions[i].ToMultivariateDistribution();

            this.model = new MultivariateMixture<MultivariateNormalDistribution>(normals);
        }

        /// <summary>
        ///   Initializes the model with initial values.
        /// </summary>
        /// 
        public void Initialize(double[] coefficients, MultivariateNormalDistribution[] distributions)
        {
            if (distributions.Length != Count)
                throw new ArgumentException("The number of distributions and clusters does not match.", "distributions");

            if (coefficients.Length != Count)
                throw new ArgumentException("The number of coefficients and clusters does not match.", "coefficients");

            this.model = new MultivariateMixture<MultivariateNormalDistribution>(coefficients, distributions);
        }

        /// <summary>
        ///   Initializes the model with initial values.
        /// </summary>
        /// 
        public void Initialize(double[] coefficients, NormalDistribution[] distributions)
        {
            var normals = new MultivariateNormalDistribution[distributions.Length];
            for (int i = 0; i < normals.Length; i++)
                normals[i] = distributions[i].ToMultivariateDistribution();

            Initialize(coefficients, normals);
        }

        /// <summary>
        ///   Initializes the model with initial values.
        /// </summary>
        /// 
        public void Initialize(MultivariateMixture<MultivariateNormalDistribution> mixture)
        {
            Initialize(mixture.Coefficients, mixture.Components);
        }

        /// <summary>
        ///   Initializes the model with initial values.
        /// </summary>
        /// 
        public void Initialize(Mixture<NormalDistribution> mixture)
        {
            var normals = new MultivariateNormalDistribution[mixture.Components.Length];
            for (int i = 0; i < normals.Length; i++)
                normals[i] = mixture.Components[i].ToMultivariateDistribution();

            Initialize(mixture.Coefficients, normals);
        }



        // Using composition over inheritance to achieve the closest as possible effect to a Mixin
        // in C# - unfortunately needs a lot a boilerplate code to rewrire the interface methods to
        // where their actual implementation is

        /// <summary>
        /// Gets or sets the clusters' centroids.
        /// </summary>
        /// <value>The clusters' centroids.</value>
        public IMixtureComponent<MultivariateNormalDistribution>[] Centroids
        {
            get { return collection.Centroids; }
            set { collection.Centroids = value; }
        }

        /// <summary>
        /// Gets the collection of clusters currently modeled by the clustering algorithm.
        /// </summary>
        /// <value>The clusters.</value>
        public GaussianCluster[] Clusters
        {
            get { return collection.Clusters; }
        }

        /// <summary>
        /// Gets or sets the distance function used to measure the distance
        /// between a point and the cluster centroid in this clustering definition.
        /// </summary>
        /// <value>The distance.</value>
        public IDistance<double[], Centroid> Distance
        {
            get { return collection.Distance; }
            set { collection.Distance = value; }
        }

        /// <summary>
        /// Gets the number of clusters in the collection.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get { return collection.Count; } }

        /// <summary>
        /// Gets the <see cref="GaussianCluster"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>GaussianCluster.</returns>
        public GaussianCluster this[int index]
        {
            get { return collection[index]; }
        }

        /// <summary>
        /// Calculates the average square distance from the data points
        /// to the nearest clusters' centroids.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="labels">The labels.</param>
        /// <param name="weights">The weights.</param>
        /// <returns>The average square distance from the data points to the nearest
        /// clusters' centroids.</returns>
        /// <remarks>The average distance from centroids can be used as a measure
        /// of the "goodness" of the clustering. The more the data are
        /// aggregated around the centroids, the less the average distance.</remarks>
        public double Distortion(double[][] data, int[] labels = null, double[] weights = null)
        {
            return collection.Distortion(data, labels, weights);
        }

        /// <summary>
        /// Transform data points into feature vectors containing the
        /// distance between each point and each of the clusters.
        /// </summary>
        /// <param name="points">The input points.</param>
        /// <param name="weights">The weight associated with each point.</param>
        /// <param name="result">An optional matrix to store the computed transformation.</param>
        /// <returns>A vector containing the distance between the input points and the clusters.</returns>
        public double[][] Transform(double[][] points, double[] weights = null, double[][] result = null)
        {
            return collection.Transform(points, weights, result);
        }

        /// <summary>
        /// Transform data points into feature vectors containing the
        /// distance between each point and each of the clusters.
        /// </summary>
        /// <param name="points">The input points.</param>
        /// <param name="labels">The label of each input point.</param>
        /// <param name="weights">The weight associated with each point.</param>
        /// <param name="result">An optional matrix to store the computed transformation.</param>
        /// <returns>A vector containing the distance between the input points and the clusters.</returns>
        public double[] Transform(double[][] points, int[] labels, double[] weights = null, double[] result = null)
        {
            return collection.Transform(points, labels, weights, result);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<GaussianCluster> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }
    }
}
