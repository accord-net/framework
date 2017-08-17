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
    using System.Collections;
    using System.Collections.Generic;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Base class for a data cluster.
    /// </summary>
    /// 
    [Serializable]
    public abstract class Cluster<TCollection, TData, TCluster>
        where TCollection : IClusterCollectionEx<TData, TCluster>
        where TCluster : Cluster<TCollection, TData, TCluster>, new()
    {
        private TCollection owner;
        private int index;

        /// <summary>
        ///   Gets the collection to which this cluster belongs to.
        /// </summary>
        /// 
        public TCollection Owner
        {
            get { return owner; }
        }

        /// <summary>
        ///   Gets the label for this cluster.
        /// </summary>
        /// 
        public int Index
        {
            get { return this.index; }
        }

        /// <summary>
        ///   Gets the proportion of samples contained in this cluster.
        /// </summary>
        /// 
        public double Proportion
        {
            get { return Owner.Proportions[Index]; }
        }

        [Serializable]
        internal class ClusterCollection : IClusterCollectionEx<TData, TCluster>
        {
            private TCollection collection;

            private double[] proportions;
            private TCluster[] clusters;


            /// <summary>
            ///   Initializes a new instance of the <see cref="KMeansClusterCollection"/> class.
            /// </summary>
            /// 
            /// <param name="collection">The collection that contains this instance as a field.</param>
            /// <param name="k">The number of clusters K.</param>
            /// 
            public ClusterCollection(TCollection collection, int k)
            {
                // To store centroids of the clusters
                this.proportions = new double[k];
                this.clusters = new TCluster[k];
                this.collection = collection;
                for (int i = 0; i < clusters.Length; i++)
                {
                    clusters[i] = new TCluster();
                    clusters[i].owner = collection;
                    clusters[i].index = i;
                }
            }

            protected TCollection Owner { get { return collection; } }


            public double[] Proportions
            {
                get { return proportions; }
            }

            /// <summary>
            ///   Gets the cluster definitions.
            /// </summary>
            /// 
            public TCluster[] Clusters { get { return clusters; } }

            /// <summary>
            ///   Gets the number of clusters in the collection.
            /// </summary>
            /// 
            public int Count
            {
                get { return Clusters.Length; }
            }

            /// <summary>
            ///   Gets the cluster at the given index.
            /// </summary>
            /// 
            /// <param name="index">The index of the cluster. This should also be the class label of the cluster.</param>
            /// 
            /// <returns>An object holding information about the selected cluster.</returns>
            /// 
            public TCluster this[int index]
            {
                get { return clusters[index]; }
            }


            /// <summary>
            ///   Returns an enumerator that iterates through a collection.
            /// </summary>
            /// 
            /// <returns>
            ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            /// 
            public IEnumerator<TCluster> GetEnumerator()
            {
                foreach (var cluster in clusters)
                    yield return cluster;
            }

            /// <summary>
            ///   Returns an enumerator that iterates through a collection.
            /// </summary>
            /// 
            /// <returns>
            ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            /// 
            IEnumerator IEnumerable.GetEnumerator()
            {
                return clusters.GetEnumerator();
            }
        }
    }
}
