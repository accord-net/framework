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

    /// <summary>
    /// Common interface for observation sequence taggers.
    /// </summary>
    /// 
    public interface IScoreTagger<in TInput> :
        ITagger<TInput> 
    {

        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequence"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        double[][] Scores(TInput[] sequence);

        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequence"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        double[][] Scores(TInput[] sequence, double[][] result);

        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequence"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        double[][] Scores(TInput[] sequence, ref int[] decision);

        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequence"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        double[][] Scores(TInput[] sequence, ref int[] decision, double[][] result);


        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequences"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        double[][][] Scores(TInput[][] sequences);

        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequences"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        double[][][] Scores(TInput[][] sequences, double[][][] result);

        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequences"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        double[][][] Scores(TInput[][] sequences, ref int[][] decision);

        /// <summary>
        ///   Computes numerical scores measuring the association between
        ///   each of the given <paramref name="sequences"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        double[][][] Scores(TInput[][] sequences, ref int[][] decision, double[][][] result);



    }
    
}
