// Accord Imaging Library
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

namespace Accord.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Math.Distances;

    /// <summary>
    ///   Nearest neighbor feature point matching algorithm.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///     This class matches feature points using a <see cref="KNearestNeighbors">
    ///     k-Nearest Neighbors</see> algorithm.</para>
    /// </remarks>
    ///
    /// <seealso cref="CorrelationMatching"/>
    /// <seealso cref="RansacHomographyEstimator"/>
    /// 
    public class KNearestNeighborMatching<T>
    {

        /// <summary>
        ///   Gets or sets the number k of nearest neighbors.
        /// </summary>
        /// 
        public int K { get; set; }

        /// <summary>
        ///   Gets or sets the distance function used
        ///   as a distance metric between data points.
        /// </summary>
        /// 
        public IDistance<T> Distance { get; set; }

        /// <summary>
        ///   Gets or sets a minimum relevance threshold
        ///   used to find matching pairs. Default is 0.
        /// </summary>
        /// 
        public double Threshold { get; set; }



        /// <summary>
        ///   Constructs a new <see cref="KNearestNeighbors">
        ///   K-Nearest Neighbors matching</see> algorithm.
        /// </summary>
        /// 
        /// <param name="k">The number of neighbors to use when matching points.</param>
        /// <param name="distance">The distance function to consider between points.</param>
        /// 
        [Obsolete("Please specify the distance function using classes instead of lambda functions.")]
        public KNearestNeighborMatching(int k, Func<T, T, double> distance)
            : this(k, Accord.Math.Distance.GetDistance(distance))
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="KNearestNeighbors">
        ///   K-Nearest Neighbors matching</see> algorithm.
        /// </summary>
        /// 
        /// <param name="k">The number of neighbors to use when matching points.</param>
        /// <param name="distance">The distance function to consider between points.</param>
        /// 
        public KNearestNeighborMatching(int k, IDistance<T> distance)
        {
            this.K = k;
            this.Distance = distance;
        }

#if NET35
        /// <summary>
        ///   Matches two sets of feature points.
        /// </summary>
        /// 
        public IntPoint[][] Match<TFeaturePoint>(IEnumerable<TFeaturePoint> points1, IEnumerable<TFeaturePoint> points2)
            where TFeaturePoint : IFeaturePoint<T>
        {
            // This overload exists to maintain compatibility with .NET 3.5 and
            // is redundant when generics covariance/contravariance is available
            //
            return match(points1.Cast<IFeaturePoint<T>>().ToArray(), 
                         points2.Cast<IFeaturePoint<T>>().ToArray());
        }
#else
        /// <summary>
        ///   Matches two sets of feature points.
        /// </summary>
        /// 
        public IntPoint[][] Match(IEnumerable<IFeaturePoint<T>> points1, IEnumerable<IFeaturePoint<T>> points2)
        {
            return match(points1.ToArray(), points2.ToArray());
        }

        /// <summary>
        ///   Matches two sets of feature points.
        /// </summary>
        /// 
        public IntPoint[][] Match(IFeaturePoint<T>[] points1, IFeaturePoint<T>[] points2)
        {
            return match(points1, points2);
        }
#endif

        /// <summary>
        ///   Matches two sets of feature points.
        /// </summary>
        /// 
        private IntPoint[][] match(IFeaturePoint<T>[] points1, IFeaturePoint<T>[] points2)
        {
            if (points1.Length == 0 || points2.Length == 0)
                throw new ArgumentException("Insufficient number of points to produce a matching.");


            bool swap = false;

            // We should build the classifiers with the highest number
            // of training points. Thus, if we have more points in the
            // second image than in the first, we'll have to swap them

            if (points2.Length > points1.Length)
            {
                var aux = points1;
                points1 = points2;
                points2 = aux;
                swap = true;
            }


            // Get the descriptors associated with each feature point
            T[] features1 = new T[points1.Length];
            for (int i = 0; i < features1.Length; i++)
                features1[i] = points1[i].Descriptor;

            T[] features2 = new T[points2.Length];
            for (int i = 0; i < features2.Length; i++)
                features2[i] = points2[i].Descriptor;

            var knn = CreateNeighbors(features1);

            double[] scores = new double[features2.Length];
            int[] labels = new int[features2.Length];
            knn.Score(features2, ref labels, result: scores);

            int[] bestMatch = new int[points1.Length];
            double[] bestScore = new double[points1.Length];
            for (int i = 0; i < bestScore.Length; i++)
                bestScore[i] = Double.PositiveInfinity;


            // Get all points matching with this point    
            for (int j = 0; j < labels.Length; j++)
            {
                int i = labels[j];

                if (scores[j] > Threshold)
                {
                    if (scores[j] < bestScore[i])
                    {
                        bestScore[i] = scores[j];
                        bestMatch[i] = j;
                    }
                }
            }


            var p1 = new List<IntPoint>(bestScore.Length);
            var p2 = new List<IntPoint>(bestScore.Length);

            // Get the two sets of points
            for (int i = 0; i < bestScore.Length; i++)
            {
                IFeaturePoint<T> pi = points1[i];

                if (bestScore[i] != Double.PositiveInfinity)
                {
                    int j = bestMatch[i];
                    IFeaturePoint<T> pj = points2[j];
                    p1.Add(new IntPoint((int)pi.X, (int)pi.Y));
                    p2.Add(new IntPoint((int)pj.X, (int)pj.Y));
                }
            }

            IntPoint[] m1 = p1.ToArray();
            IntPoint[] m2 = p2.ToArray();

            // Create matching point pairs

            if (swap)
                return new IntPoint[][] { m2, m1 };
            return new IntPoint[][] { m1, m2 };
        }


        /// <summary>
        ///   Creates a nearest neighbor algorithm with the feature points as
        ///   inputs and their respective indices a the corresponding output.
        /// </summary>
        /// 
        protected virtual IMulticlassScoreClassifier<T> CreateNeighbors(T[] features)
        {
            int[] outputs = Vector.Range(0, features.Length);

            // Create a k-Nearest Neighbor classifier to classify points
            // in the second image to nearest points in the first image
            return new KNearestNeighbors<T>(K, Distance).Learn(features, outputs);
        }

    }
}
