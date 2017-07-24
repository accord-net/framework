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
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Linq;

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
            /// <param name="parallelOptions">The parallelization options for this procedure.
            /// Only relevant for the <see cref="Seeding.PamBuild"/>. </param>
            /// 
            public virtual void Randomize(TData[] points, Seeding strategy = Seeding.KMeansPlusPlus, 
                ParallelOptions parallelOptions = null)
            {
                if (points == null)
                    throw new ArgumentNullException("points");

                // If number of points equal to required number of centroids,
                // then no need any seeding algorithm, just include them all.
                if (points.Length == Centroids.Length)
                {
                    for (int i = 0; i < points.Length; ++i)
                        Centroids[i] = (TData)points[i].Clone();
                    return;
                }

                // Otherwise perform chosen algorithm
                switch (strategy)
                {
                    case Seeding.Fixed:
                    {
                        DoFixedSeeding(points);
                        break;
                    }

                    case Seeding.Uniform:
                    {
                        DoUniformSeeding(points);
                        break;
                    }

                    case Seeding.KMeansPlusPlus:
                    {
                        DoKMeansPlusPlusSeeding(points);
                        break;
                    }

                    case Seeding.PamBuild:
                    {
                        if (parallelOptions == null)
                        {
                            parallelOptions = new ParallelOptions();
                            parallelOptions.MaxDegreeOfParallelism = 1;
                        }
                        DoPamBuildSeeding(points, parallelOptions);
                        break;
                    }
                } // switch

                Owner.NumberOfInputs = Tools.GetNumberOfInputs(points);
            } // Randomize()

            private void DoFixedSeeding(TData[] points)
            {
                int[] idx = Vector.Sample(points.Length);
                for (int i = 0; i < Centroids.Length; i++)
                {
                    if (Centroids[i] != null)
                        Centroids[i] = (TData)points[idx[i]].Clone();
                }
            }

            private void DoUniformSeeding(TData[] points)
            {
                Centroids = Vector.Sample(points, Centroids.Length);
                for (int i = 0; i < Centroids.Length; i++)
                    Centroids[i] = (TData)Centroids[i].Clone();
            }

            private void DoKMeansPlusPlusSeeding(TData[] points)
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

                        try
                        {
                            // Sample randomly using the probabilities
                            idx = GeneralDiscreteDistribution.Random(D);
                        }
                        catch (InvalidOperationException)
                        {
                            // Degenerate case: numerical inaccuracy when normalizing 
                            // the point-centroid distances to become probabilities
                            idx = r.Next(0, points.Length);

                            Trace.TraceWarning("Could not convert distances into probabilities that sum up to one during KMeans++ initialization.");
                        }
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

            private void DoPamBuildSeeding(TData[] points, ParallelOptions parallelOptions)
            {
                int firstMedoidIndex = SelectFirstMedoid(points);
                if (Centroids.Length > 1)
                {
                    var medoids = new HashSet<int>();
                    medoids.Add(firstMedoidIndex);
                    var pointGains = new double[points.Length];
                    while (medoids.Count < Centroids.Length)
                    {
                        Parallel.For(0, points.Length, parallelOptions, i =>
                        {
                            double gain = 0.0;
                            if (!medoids.Contains(i))
                            {
                                Parallel.For(0, points.Length, parallelOptions, j =>
                                {
                                    // Skip selected points and point #I
                                    if (j == i || medoids.Contains(j)) return;
                                    double dj = medoids.Min(n => Distance.Distance(points[j], points[n]));
                                    double dij = Distance.Distance(points[i], points[j]);
                                    if (dj > dij)
                                        InterlockedEx.Add(ref gain, dj - dij);
                                });
                            }
                            pointGains[i] = gain;
                        });

                        int maxGainPointIndex = 0;
                        while (medoids.Contains(maxGainPointIndex) && maxGainPointIndex < pointGains.Length)
                            ++maxGainPointIndex;
                        for (int i = maxGainPointIndex + 1; i < pointGains.Length; i++)
                        {
                            if (!medoids.Contains(i) && pointGains[i] < pointGains[maxGainPointIndex])
                                maxGainPointIndex = i;
                        }
                        if (maxGainPointIndex < pointGains.Length)
                            medoids.Add(maxGainPointIndex);
                        else
                        {
                            // Should never happen.
                            throw new Exception("Can't select medoids!");
                        }
                    }

                    int index = 0;
                    foreach (int medoidPointIndex in medoids)
                        Centroids[index++] = (TData)points[medoidPointIndex].Clone();
                }
                else
                {
                    Centroids[0] = (TData)points[firstMedoidIndex].Clone();
                }
            }

            private int SelectFirstMedoid(TData[] points)
            {
                var costs = new double[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    double cost = 0.0;
                    for (int j = 0; j < points.Length; j++)
                        cost += Distance.Distance(points[i], points[j]);
                    costs[i] = cost;
                }

                int minCostPointIndex = 0;
                for (int i = 1; i < costs.Length; i++)
                {
                    if (costs[i] < costs[minCostPointIndex])
                        minCostPointIndex = i;
                }

                return minCostPointIndex;
            }
        }
    }
}
