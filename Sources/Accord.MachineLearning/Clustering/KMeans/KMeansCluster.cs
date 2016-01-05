// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using Accord.Math.Distances;
    using Accord.Statistics.Distributions.Univariate;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///   k-Means cluster collection.
    /// </summary>
    /// 
    /// <seealso cref="KMeans"/>
    /// 
    [Serializable]
    public class KMeansClusterCollection : ClusterCollection<double[], KMeansClusterCollection.KMeansCluster>
    {
        double[][,] covariances;

        /// <summary>
        ///   k-Means' cluster.
        /// </summary>
        /// 
        [Serializable]
        public class KMeansCluster : KMeansClusterCollection.Cluster<KMeansClusterCollection>
        {
            /// <summary>
            ///   Initializes a new instance of the <see cref="KMeansCluster"/> class.
            /// </summary>
            /// 
            /// <param name="owner">The owner collection.</param>
            /// <param name="index">The cluster index.</param>
            /// 
            public KMeansCluster(KMeansClusterCollection owner, int index)
                : base(owner, index)
            {
            }

            /// <summary>
            ///   Gets the covariance matrix for the samples in this cluster.
            /// </summary>
            /// 
            public double[,] Covariance
            {
                get { return Owner.covariances[Index]; } 
            }
        }

        /// <summary>
        ///   Gets the dimensionality of the data space.
        /// </summary>
        /// 
        public int Dimension
        {
            get { return Centroids[0].Length; }
        }

        /// <summary>
        ///   Gets the clusters' variance-covariance matrices.
        /// </summary>
        /// 
        /// <value>The clusters' variance-covariance matrices.</value>
        /// 
        public double[][,] Covariances
        {
            get { return covariances; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KMeansClusterCollection"/> class.
        /// </summary>
        /// 
        /// <param name="k">The number of clusters K.</param>
        /// <param name="distance">The distance metric to consider.</param>
        /// 
        public KMeansClusterCollection(int k, IDistance<double[]> distance)
            : base(k, distance)
        {
            covariances = new double[k][,];
            for (int i = 0; i < k; i++)
                Clusters[i] = new KMeansCluster(this, i);
        }

    }

}
