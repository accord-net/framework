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
    using Accord.IO;
    using Accord.Math;
    using Accord.Math.Distances;
    using Accord.Statistics.Distributions.Univariate;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    ///   Data cluster.
    /// </summary>
    /// 
    [Serializable]
    public class CentroidCluster<TCollection, TData, TCluster> : CentroidCluster<TCollection, TData, TData, TCluster>
        where TCollection : ICentroidClusterCollection<TData, TCluster>, IMulticlassScoreClassifier<TData, int>
        where TCluster : CentroidCluster<TCollection, TData, TCluster>, new()
        where TData : ICloneable
    {

        [Serializable]
        new internal class ClusterCollection : CentroidCluster<TCollection, TData, TData, TCluster>.ClusterCollection,
            ICentroidClusterCollection<TData, TCluster>
        {
            /// <summary>
            ///   Initializes a new instance of the <see cref="KMeansClusterCollection"/> class.
            /// </summary>
            /// 
            /// <param name="collection">The collection that contains this instance as a field.</param>
            /// <param name="k">The number of clusters K.</param>
            /// <param name="distance">The distance metric to consider.</param>
            /// 
            public ClusterCollection(TCollection collection, int k, IDistance<TData> distance)
                : base(collection, k, distance)
            {
            }

            /// <summary>
            ///   Randomizes the clusters inside a dataset.
            /// </summary>
            /// 
            /// <param name="points">The data to randomize the algorithm.</param>
            /// <param name="strategy">The seeding strategy to be used. Default is <see cref="Seeding.KMeansPlusPlus"/>.</param>
            /// 
            public virtual void Randomize(TData[] points, Seeding strategy = Seeding.KMeansPlusPlus)
            {
                if (points == null)
                    throw new ArgumentNullException("points");

                if (strategy == Seeding.Fixed)
                {
                    int[] idx = Vector.Sample(points.Length);
                    for (int i = 0; i < Centroids.Length; i++)
                    {
                        if (Centroids[i] != null)
                            Centroids[i] = (TData)points[idx[i]].Clone();
                    }
                }
                else if (strategy == Seeding.Uniform)
                {
                    Centroids = Vector.Sample(points, Centroids.Length);
                    for (int i = 0; i < Centroids.Length; i++)
                        Centroids[i] = (TData)Centroids[i].Clone();
                }
                else if (strategy == Seeding.KMeansPlusPlus)
                {
                    // Initialize using K-Means++
                    // http://en.wikipedia.org/wiki/K-means%2B%2B

                    var r = Accord.Math.Random.Generator.Random;

                    // 1. Choose one center uniformly at random from among the data points.
                    int idx = r.Next(0, points.Length);
                    Centroids[0] = (TData)points[idx].Clone();

                    for (int c = 1; c < Centroids.Length; c++)
                    {
                        // 2. For each data point x, compute D(x), the distance between
                        //    x and the nearest center that has already been chosen.

                        double sum = 0;
                        var D = new double[points.Length];
                        for (int i = 0; i < D.Length; i++)
                        {
                            var x = points[i];

                            double min = Distance.Distance(x, Centroids[0]);
                            for (int j = 1; j < c; j++)
                            {
                                double d = Distance.Distance(x, Centroids[j]);

                                if (d < min)
                                    min = d;
                            }

                            D[i] = min;
                            sum += min;
                        }

                        // Note: the following checks could have been avoided if we added
                        // a small value to each distance, but is kept as this to avoid 
                        // breaking the random pattern in existing code.

                        if (sum == 0)
                        {
                            // Degenerate case: all points are the same, chose any of them
                            idx = r.Next(0, points.Length);
                        }
                        else
                        {
                            // Convert to probabilities
                            for (int i = 0; i < D.Length; i++)
                                D[i] /= sum;

                            // Sample randomly using the probabilities
                            idx = GeneralDiscreteDistribution.Random(D);
                        }

                        // 3. Choose one new data point at random as a new center, using a weighted
                        //    probability distribution where a point x is chosen with probability 
                        //    proportional to D(x)^2.					
                        Centroids[c] = (TData)points[idx].Clone();
                    }

#if DEBUG
                    // Make sure that centroids are not datapoints
                    for (int i = 0; i < Centroids.Length; i++)
                        for (int j = 0; j < points.Length; j++)
                            if (Object.Equals(Centroids[i], points[j]))
                                throw new Exception();
#endif
                }

                this.Owner.NumberOfInputs = Tools.GetNumberOfInputs(points);
            }

        }
    }
}
