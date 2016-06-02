// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    ///   Common interface for score-based multi-label classifiers. A multi-label
    ///   classifier can predict the occurrence of multiple class labels at once
    ///   based on a decision score (a real number) computed for each class.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int[].</typeparam>
    /// 
    public interface IMultilabelDistanceClassifier<in TInput, TClasses> :
        IClassifier<TInput, TClasses> 
    {
        /// <summary>
        ///   Predicts a class label vector for each input vector, returning a
        ///   numerical score measuring the strength of association of the input vector
        ///   to each of the possible classes.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        ///   vector, as predicted by the classifier. If passed as null, the classifier
        ///   will create a new array.</param>
        /// 
        double[][] Distances(TInput[] input, ref TClasses[] decision);

        /// <summary>
        ///   Predicts a class label vector for each input vector, returning a
        ///   numerical score measuring the strength of association of the input vector
        ///   to each of the possible classes.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        ///   vector, as predicted by the classifier. If passed as null, the classifier
        ///   will create a new array.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[][] Distances(TInput[] input, ref TClasses[] decision, double[][] result);
    }

    /// <summary>
    ///   Common interface for score-based multi-label classifiers. A multi-label
    ///   classifier can predict the occurrence of multiple class labels at once
    ///   based on a decision score (a real number) computed for each class.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int[].</typeparam>
    /// 
    public interface IMultilabelRefDistanceClassifier<TInput, TClasses> :
        IMultilabelDistanceClassifier<TInput, TClasses>
    {
        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning a
        ///   numerical score measuring the strength of association of the input vector
        ///   to each of the possible classes.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        ///   vector, as predicted by the classifier. If passed as null, the classifier
        ///   will create a new array.</param>
        /// 
        double[] Distances(TInput input, ref TClasses decision);

        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning a
        ///   numerical score measuring the strength of association of the input vector
        ///   to each of the possible classes.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        ///   vector, as predicted by the classifier. If passed as null, the classifier
        ///   will create a new array.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] Distances(TInput input, ref TClasses decision, double[] result);

    }

    /// <summary>
    ///   Common interface for score-based multi-label classifiers. A multi-label
    ///   classifier can predict the occurrence of multiple class labels at once
    ///   based on a decision score (a real number) computed for each class.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int[].</typeparam>
    /// 
    public interface IMultilabelOutDistanceClassifier<TInput, TClasses> :
        IMultilabelDistanceClassifier<TInput, TClasses>
    {
        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning a
        ///   numerical score measuring the strength of association of the input vector
        ///   to each of the possible classes.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        ///
        double[] Distances(TInput input, out TClasses decision);

        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning a
        ///   numerical score measuring the strength of association of the input vector
        ///   to each of the possible classes.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] Distances(TInput input, out TClasses decision, double[] result);
    }

    /// <summary>
    ///   Common interface for score-based multi-label classifiers. A multi-label
    ///   classifier can predict the occurrence of multiple class labels at once
    ///   based on a decision score (a real number) computed for each class.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// 
    public interface IMultilabelDistanceClassifier<TInput> :
        IMultilabelOutDistanceClassifier<TInput, int>,
        IMultilabelOutDistanceClassifier<TInput, double>,
        IMultilabelRefDistanceClassifier<TInput, int[]>,
        IMultilabelRefDistanceClassifier<TInput, bool[]>,
        IMultilabelRefDistanceClassifier<TInput, double[]>,
        IMultilabelClassifier<TInput>
    {

        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   the given <paramref name="input"/> vector and a given 
        ///   <paramref name="classIndex"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// 
        double Distance(TInput input, int classIndex);

        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   each of the given <paramref name="input"/> vectors and the 
        ///   given <paramref name="classIndex">class indices</paramref>.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="classIndex">The class indices associated with each
        ///   input vector whose scores will be computed.</param>
        /// 
        double[] Distance(TInput[] input, int[] classIndex);

        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   each of the given <paramref name="input"/> vectors and the 
        ///   given <paramref name="classIndex">class indices</paramref>.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="classIndex">The class indices associated with each
        ///   input vector whose scores will be computed.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] Distance(TInput[] input, int[] classIndex, double[] result);


        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   each of the given <paramref name="input"/> vectors and the 
        ///   given <paramref name="classIndex">class indices</paramref>.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// 
        double[] Distance(TInput[] input, int classIndex);

        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   each of the given <paramref name="input"/> vectors and the 
        ///   given <paramref name="classIndex">class indices</paramref>.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] Distance(TInput[] input, int classIndex, double[] result);



        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   the given <paramref name="input"/> vector and each class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        double[] Distances(TInput input);

        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   the given <paramref name="input"/> vector and each class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///   
        double[] Distances(TInput input, double[] result);

        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   each of the given <paramref name="input"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// 
        double[][] Distances(TInput[] input);

        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   each of the given <paramref name="input"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[][] Distances(TInput[] input, double[][] result);

    }
}
