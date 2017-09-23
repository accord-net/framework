// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2013
// diego.catalano at live.com
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
    using Accord.Math;
    using Accord.Statistics;
    using System.Runtime.Serialization;

    /// <summary>
    ///   Feature dictionary. Associates a set of Haralick features to a given degree
    ///   used to compute the originating <see cref="GrayLevelCooccurrenceMatrix">GLCM</see>.
    /// </summary>
    /// 
    /// <seealso cref="Haralick"/>
    /// <seealso cref="HaralickDescriptor"/>
    /// 
    [Serializable]
    public class HaralickDescriptorDictionary : SortedDictionary<CooccurrenceDegree, HaralickDescriptor>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="HaralickDescriptorDictionary"/> class.
        /// </summary>
        /// 
        public HaralickDescriptorDictionary()
        {
        }

        /// <summary>
        ///   Combines features generated from different <see cref="GrayLevelCooccurrenceMatrix">
        ///   GLCMs</see> computed using different <see cref="CooccurrenceDegree">angulations</see>
        ///   by concatenating them into a single vector.
        /// </summary>
        /// 
        /// <param name="features">The number of Haralick's original features to compute.</param>
        /// 
        /// <returns>A single vector containing all values computed from
        /// the different <see cref="HaralickDescriptor"/>s.</returns>
        /// 
        /// <remarks>
        ///   If there are <c>d</c> <see cref="CooccurrenceDegree">degrees</see> in this
        ///   collection, and <c>n</c> <paramref name="features"/> given to compute, the
        ///   generated vector will have size <c>d * n</c>. All features from different
        ///   degrees will be concatenated into this single result vector.
        /// </remarks>
        /// 
        public double[] Combine(int features)
        {
            int count = this.Count;
            double[] haralick = new double[features * count];

            int c = 0;
            foreach (KeyValuePair<CooccurrenceDegree, HaralickDescriptor> pair in this)
            {
                HaralickDescriptor descriptor = pair.Value;
                double[] vector = descriptor.GetVector(features);

                for (int i = 0; i < vector.Length; i++)
                    haralick[c++] = vector[i];
            }

            return haralick;
        }

        /// <summary>
        ///   Combines features generated from different <see cref="GrayLevelCooccurrenceMatrix">
        ///   GLCMs</see> computed using different <see cref="CooccurrenceDegree">angulations</see>
        ///   by averaging them into a single vector.
        /// </summary>
        /// 
        /// <param name="features">The number of Haralick's original features to compute.</param>
        /// 
        /// <returns>A single vector containing the average of the values
        ///   computed from the different <see cref="HaralickDescriptor"/>s.</returns>
        ///   
        /// <remarks>
        ///   If there are <c>d</c> <see cref="CooccurrenceDegree">degrees</see> in this
        ///   collection, and <c>n</c> <paramref name="features"/> given to compute, the
        ///   generated vector will have size <c>n</c>. All features from different
        ///   degrees will be averaged into this single result vector.
        /// </remarks>
        /// 
        public double[] Average(int features)
        {
            double[] haralick = new double[features];

            foreach (KeyValuePair<CooccurrenceDegree, HaralickDescriptor> pair in this)
            {
                HaralickDescriptor descriptor = pair.Value;
                double[] vector = descriptor.GetVector(features);

                for (int i = 0; i < vector.Length; i++)
                    haralick[i] += vector[i];
            }

            int count = this.Count;
            for (int i = 0; i < haralick.Length; i++)
                haralick[i] /= count;

            return haralick;
        }

        /// <summary>
        ///   Combines features generated from different <see cref="GrayLevelCooccurrenceMatrix">
        ///   GLCMs</see> computed using different <see cref="CooccurrenceDegree">angulations</see>
        ///   by averaging them into a single vector.
        /// </summary>
        /// 
        /// <param name="features">The number of Haralick's original features to compute.</param>
        /// 
        /// <returns>A single vector containing the average of the values
        ///   computed from the different <see cref="HaralickDescriptor"/>s.</returns>
        /// 
        /// <remarks>
        ///   If there are <c>d</c> <see cref="CooccurrenceDegree">degrees</see> in this
        ///   collection, and <c>n</c> <paramref name="features"/> given to compute, the
        ///   generated vector will have size <c>2*n*d</c>. Each even index will have
        ///   the average of a given feature, and the subsequent odd index will contain
        ///   the range of this feature.
        /// </remarks>
        /// 
        public double[] AverageWithRange(int features)
        {
            int degrees = this.Count;

            double[][] vectors = new double[features][];
            for (int i = 0; i < vectors.Length; i++)
                vectors[i] = new double[degrees];

            int c = 0;
            foreach (KeyValuePair<CooccurrenceDegree, HaralickDescriptor> pair in this)
            {
                HaralickDescriptor descriptor = pair.Value;
                double[] vector = descriptor.GetVector(features);

                for (int i = 0; i < vector.Length; i++)
                    vectors[i][c] = vector[i];
                c++;
            }

            double[] haralick = new double[features * 2];

            int j = 0;
            for (int i = 0; i < vectors.Length; i++)
            {
                haralick[j++] = vectors[i].Mean();
                haralick[j++] = vectors[i].GetRange().Length;
            }

            return haralick;
        }

        /// <summary>
        ///   Combines features generated from different <see cref="GrayLevelCooccurrenceMatrix">
        ///   GLCMs</see> computed using different <see cref="CooccurrenceDegree">angulations</see>
        ///   by averaging them into a single vector, normalizing them to be between -1 and 1.
        /// </summary>
        /// 
        /// <param name="features">The number of Haralick's original features to compute.</param>
        /// 
        /// <returns>A single vector containing the averaged and normalized values
        ///   computed from the different <see cref="HaralickDescriptor"/>s.</returns>
        /// 
        /// <remarks>
        ///   If there are <c>d</c> <see cref="CooccurrenceDegree">degrees</see> in this
        ///   collection, and <c>n</c> <paramref name="features"/> given to compute, the
        ///   generated vector will have size <c>n</c>. All features will be averaged, and
        ///   the mean will be scaled to be in a [-1,1] interval.
        /// </remarks>
        /// 
        public double[] Normalize(int features)
        {
            int degrees = this.Count;

            double[][] vectors = new double[features][];
            for (int i = 0; i < vectors.Length; i++)
                vectors[i] = new double[degrees];

            int c = 0;
            foreach (KeyValuePair<CooccurrenceDegree, HaralickDescriptor> pair in this)
            {
                HaralickDescriptor descriptor = pair.Value;
                double[] vector = descriptor.GetVector(features);

                for (int i = 0; i < vector.Length; i++)
                    vectors[i][c] = vector[i];
                c++;
            }

            double[] haralick = new double[features];

            for (int i = 0; i < vectors.Length; i++)
            {
                DoubleRange range = vectors[i].GetRange();
                double mean = vectors[i].Mean();

                if (mean != 0)
                    haralick[i] = Vector.Scale(mean, range.Min, range.Max, -1, 1);
            }

            return haralick;
        }
    }
}