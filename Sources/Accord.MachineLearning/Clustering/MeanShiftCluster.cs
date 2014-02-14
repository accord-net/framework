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
    using System.Collections;
    using System.Collections.Generic;
    using Accord.MachineLearning.Structures;

    /// <summary>
    ///   Mean shift cluster collection.
    /// </summary>
    /// 
    /// <seealso cref="MeanShift"/>
    /// 
    [Serializable]
    public class MeanShiftClusterCollection : IClusterCollection<double[], MeanShiftCluster>
    {

        private List<MeanShiftCluster> clusters;

        private double[][] modes;
        private KDTree<int> tree;


        /// <summary>
        ///   Initializes a new instance of the <see cref="MeanShiftClusterCollection"/> class.
        /// </summary>
        /// 
        public MeanShiftClusterCollection(KDTree<int> tree, double[][] modes)
        {
            this.modes = modes;
            this.tree = tree;

            this.clusters = new List<MeanShiftCluster>();
            for (int i = 0; i < modes.Length; i++)
                clusters.Add(new MeanShiftCluster(this, i));
        }


        /// <summary>
        ///   Gets the number of clusters in the collection.
        /// </summary>
        /// 
        public int Count
        {
            get { return clusters.Count; }
        }

        /// <summary>
        ///   Gets the modes of the clusters.
        /// </summary>
        /// 
        public double[][] Modes
        {
            get { return modes; }
        }


        /// <summary>
        ///   Gets the cluster at the given index.
        /// </summary>
        /// 
        /// <param name="index">The index of the cluster. This should also be the class label of the cluster.</param>
        /// 
        /// <returns>An object holding information about the selected cluster.</returns>
        /// 
        public MeanShiftCluster this[int index]
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
        public IEnumerator<MeanShiftCluster> GetEnumerator()
        {
            return clusters.GetEnumerator();
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
            KDTreeNodeCollection<int> result = tree.Nearest(point, 1);

            if (result.Count > 0)
                return result.Nearest.Value;

            return -1;
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
        public int[] Nearest(double[][] point)
        {
            int[] labels = new int[point.Length];
            for (int i = 0; i < labels.Length; i++)
                labels[i] = Nearest(point[i]);
            return labels;
        }
    }

    /// <summary>
    ///   Mean shift cluster.
    /// </summary>
    /// 
    /// <seealso cref="MeanShift"/>
    /// <seealso cref="MeanShiftClusterCollection"/>
    /// 
    [Serializable]
    public class MeanShiftCluster
    {
        private MeanShiftClusterCollection owner;
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
        ///   Initializes a new instance of the <see cref="MeanShiftCluster"/> class.
        /// </summary>
        /// 
        /// <param name="owner">The owner.</param>
        /// <param name="index">The cluster index.</param>
        /// 
        public MeanShiftCluster(MeanShiftClusterCollection owner, int index)
        {
            this.owner = owner;
            this.index = index;
        }

        /// <summary>
        ///   Gets the mode of the cluster.
        /// </summary>
        /// 
        public double[] Mode
        {
            get { return owner.Modes[index]; }
        }
    }
}
