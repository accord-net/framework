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
    using Accord.Math.Distances;
    using Accord.Statistics.Distributions.Univariate;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///   k-Modes cluster collection.
    /// </summary>
    /// 
    /// <seealso cref="KModes{T}"/>
    /// 
    [Serializable]
    public class KModesClusterCollection<T> : ClusterCollection<T[], KModesClusterCollection<T>.KModesCluster>
    {
        /// <summary>
        ///   k-Modes' cluster.
        /// </summary>
        /// 
        /// <seealso cref="KModes{T}"/>
        /// <seealso cref="ClusterCollection{T, U}"/>
        /// 
        [Serializable]
        public class KModesCluster : Cluster<KModesClusterCollection<T>>
        {
            /// <summary>
            ///   Initializes a new instance of the <see cref="KModesCluster"/> class.
            /// </summary>
            /// 
            /// <param name="owner">The owner collection.</param>
            /// <param name="index">The cluster index.</param>
            /// 
            public KModesCluster(KModesClusterCollection<T> owner, int index)
                : base(owner, index)
            {
            }
        }


        /// <summary>
        ///   Gets the dimensionality of the data space.
        /// </summary>
        /// 
        public int Dimension { get { return Centroids[0].Length; } }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ClusterCollection{T,U}"/> class.
        /// </summary>
        /// 
        /// <param name="k">The number of clusters K.</param>
        /// <param name="distance">The distance metric to use.</param>
        /// 
        public KModesClusterCollection(int k, IDistance<T[], T[]> distance)
            : base(k, distance)
        {
            for (int i = 0; i < k; i++)
                Clusters[i] = new KModesCluster(this, i);
        }

    }
}
