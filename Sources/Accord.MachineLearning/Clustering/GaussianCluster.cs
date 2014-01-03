// Accord Machine Learning Library
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

namespace Accord.MachineLearning
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Accord.Math;
    using Accord.Statistics.Distributions.Multivariate;
    using System.Threading.Tasks;

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
    public class GaussianCluster
    {
        private GaussianMixtureModel owner;
        private int index;

        /// <summary>
        ///   Gets the label for this cluster.
        /// </summary>
        /// 
        public int Index
        {
            get { return this.index; }
        }

        /// <summary>
        ///   Gets the cluster's mean.
        /// </summary>
        /// 
        public double[] Mean
        {
            get
            {
                if (owner.model == null) 
                    return null;

                return owner.model.Components[index].Mean;
            }
        }

        /// <summary>
        ///   Gets the cluster's variance-covariance matrix.
        /// </summary>
        /// 
        public double[,] Covariance
        {
            get
            {
                if (owner.model == null)
                    return null;

                return owner.model.Components[index].Covariance;
            }
        }

        /// <summary>
        ///   Gets the mixture coefficient for the cluster distribution.
        /// </summary>
        /// 
        public double Proportion
        {
            get
            {
                if (owner.model == null)
                    return 0;

                return owner.model.Coefficients[index];
            }
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
        public double LogLikelihood(double[] x)
        {
            if (owner.model == null)
                return 0;

            return owner.model.LogProbabilityDensityFunction(index, x);
        }

        /// <summary>
        ///   Gets a copy of the normal distribution associated with this cluster.
        /// </summary>
        /// 
        public MultivariateNormalDistribution ToDistribution()
        {
            if (owner.model == null)
                throw new InvalidOperationException("The model has not been initialized.");

            return (MultivariateNormalDistribution)owner.model.Components[index].Clone();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GaussianCluster"/> class.
        /// </summary>
        /// 
        /// <param name="owner">The owner collection.</param>
        /// <param name="index">The cluster index.</param>
        /// 
        public GaussianCluster(GaussianMixtureModel owner, int index)
        {
            this.owner = owner;
            this.index = index;
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
            if (owner.model == null) return 1;
            return -2 * owner.model.LogProbabilityDensityFunction(index, points);
        }
    }

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
    ///   to this sample through the <see cref="Nearest(double[])"/> method. </para>
    /// </remarks>
    /// 
    /// <seealso cref="GaussianMixtureModel"/>
    /// <seealso cref="GaussianCluster"/>
    /// 
    [Serializable]
    public class GaussianClusterCollection : ReadOnlyCollection<GaussianCluster>, IClusterCollection<double[]>
    {
        GaussianMixtureModel owner;

        /// <summary>
        ///   Gets the mean vectors for the clusters.
        /// </summary>
        /// 
        public double[][] Means { get { return owner.model.Components.Apply(x => x.Mean); } }

        /// <summary>
        ///   Gets the variance for each of the clusters.
        /// </summary>
        /// 
        public double[][] Variance { get { return owner.model.Components.Apply(x => x.Variance); } }

        /// <summary>
        ///   Gets the covariance matrices for each of the clusters.
        /// </summary>
        /// 
        public double[][,] Covariance { get { return owner.model.Components.Apply(x => x.Covariance); } }

        /// <summary>
        ///   Gets the mixture coefficients for each cluster.
        /// </summary>
        /// 
        public double[] Coefficients { get { return owner.model.Coefficients; } }


        /// <summary>
        ///   Initializes a new instance of the <see cref="GaussianClusterCollection"/> class.
        /// </summary>
        /// 
        /// <param name="owner">The owner collection.</param>
        /// <param name="list">The list.</param>
        /// 
        public GaussianClusterCollection(GaussianMixtureModel owner, IList<GaussianCluster> list)
            : base(list)
        {
            this.owner = owner;
        }

        /// <summary>
        ///   Returns the closest cluster to an input point.
        /// </summary>
        /// 
        /// <param name="point">The input vector.</param>
        /// <returns>
        ///   The index of the nearest cluster
        ///   to the given data point. </returns>
        ///   
        public int Nearest(double[] point)
        {
            if (point == null)
                throw new ArgumentNullException("point");

            var model = owner.model;

            int imax = 0;
            double max = model.ProbabilityDensityFunction(0, point);

            for (int i = 1; i < model.Components.Length; i++)
            {
                double p = model.ProbabilityDensityFunction(i, point);

                if (p > max)
                {
                    max = p;
                    imax = i;
                }
            }

            return imax;
        }

        /// <summary>
        ///   Returns the closest cluster to an input point.
        /// </summary>
        /// 
        /// <param name="point">The input vector.</param>
        /// <param name="response">A value between 0 and 1 representing
        ///   the confidence in the generated classification.</param>
        /// 
        /// <returns>
        ///   The index of the nearest cluster
        ///   to the given data point. </returns>
        ///   
        public int Nearest(double[] point, out double response)
        {
            if (point == null)
                throw new ArgumentNullException("point");

            double[] responses;
            int index = Nearest(point, out responses);

            double sum = responses.Sum();
            response = responses[index] / sum;
            return index;
        }

        /// <summary>
        ///   Returns the closest cluster to an input point.
        /// </summary>
        /// 
        /// <param name="point">The input vector.</param>
        /// <param name="responses">The likelihood for each of the classes.</param>
        /// 
        /// <returns>
        ///   The index of the nearest cluster
        ///   to the given data point. </returns>
        ///   
        public int Nearest(double[] point, out double[] responses)
        {
            if (point == null)
                throw new ArgumentNullException("point");

            var model = owner.model;

            responses = new double[model.Components.Length];

            for (int i = 0; i < responses.Length; i++)
                responses[i] = model.ProbabilityDensityFunction(i, point);

            int imax;
            responses.Max(out imax);
            return imax;
        }

        /// <summary>
        ///   Returns the closest cluster to an input point.
        /// </summary>
        /// 
        /// <param name="points">The input vectors.</param>
        /// <returns>
        ///   The index of the nearest cluster
        ///   to the given data point. </returns>
        ///   
        public int[] Nearest(double[][] points)
        {
            int[] labels = new int[points.Length];

            Parallel.For(0, points.Length, i =>
            {
                labels[i] = Nearest(points[i]);
            });

            return labels;
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
            return -2 * owner.model.LogLikelihood(points);
        }

    }
}
