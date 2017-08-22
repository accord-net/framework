// Accord Statistics Library
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
    using Accord.Math;
    using Accord.Statistics;
    using Accord.MachineLearning;
    using System;
    using Accord.Compat;

    /// <summary>
    /// Common base class for observation sequence taggers.
    /// </summary>
    /// 
    [Serializable]
    public abstract class ScoreTaggerBase<TInput> :
        TaggerBase<TInput>,
        IScoreTagger<TInput>
    {

        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequence"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        public double[][] Scores(TInput[] sequence)
        {
            return Scores(new[] { sequence })[0];
        }

        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequence"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        public double[][] Scores(TInput[] sequence, double[][] result)
        {
            return Scores(new[] { sequence }, new[] { result })[0];

        }

        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequence"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        public double[][] Scores(TInput[] sequence, ref int[] decision)
        {
            var d = new[] { decision };
            return Scores(new[] { sequence }, ref d)[0];
        }

        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequence"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        public double[][] Scores(TInput[] sequence, ref int[] decision, double[][] result)
        {
            return Scores(new[] { sequence }, new[] { result })[0];
        }

        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequences"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        public double[][][] Scores(TInput[][] sequences)
        {
            return Scores(sequences, create(sequences));
        }

        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequences"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        public double[][][] Scores(TInput[][] sequences, ref int[][] decision)
        {
            return Scores(sequences, ref decision, create(sequences));
        }


        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequences"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        public abstract double[][][] Scores(TInput[][] sequences, double[][][] result);

        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequences"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        public abstract double[][][] Scores(TInput[][] sequences, ref int[][] decision, double[][][] result);

    }
}
