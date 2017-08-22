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
    ///   Common interface for score-based multi-label classifiers. A multi-label
    ///   classifier can predict the occurrence of multiple class labels at once
    ///   based on a decision score (a real number) computed for each class.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int[].</typeparam>
    /// 
    public interface IMultilabelScoreClassifierBase<in TInput, TClasses>
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
        double[][] Scores(TInput[] input, ref TClasses[] decision);

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
        double[][] Scores(TInput[] input, ref TClasses[] decision, double[][] result);
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
    public interface IMultilabelRefScoreClassifier<TInput, TClasses> :
        IMultilabelScoreClassifierBase<TInput, TClasses>
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
        double[] Scores(TInput input, ref TClasses decision);

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
        double[] Scores(TInput input, ref TClasses decision, double[] result);

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
    public interface IMultilabelOutScoreClassifier<TInput, TClasses> :
        IMultilabelScoreClassifierBase<TInput, TClasses>
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
        double[] Scores(TInput input, out TClasses decision);

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
        double[] Scores(TInput input, out TClasses decision, double[] result);
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
    public interface IMultilabelScoreClassifier<TInput, TClasses> :
        IMultilabelOutScoreClassifier<TInput, TClasses>,
        IMultilabelRefScoreClassifier<TInput, TClasses[]>,
        IClassifier<TInput, TClasses>
    { 
    }

    /// <summary>
    ///   Common interface for score-based multi-label classifiers. A multi-label
    ///   classifier can predict the occurrence of multiple class labels at once
    ///   based on a decision score (a real number) computed for each class.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// 
    public interface IMultilabelScoreClassifier<TInput> :
        IMultilabelScoreClassifier<TInput, int>,
        IMultilabelScoreClassifier<TInput, double>,
        IMultilabelRefScoreClassifier<TInput, bool[]>,
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
        double Score(TInput input, int classIndex);

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
        double[] Score(TInput[] input, int[] classIndex);

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
        double[] Score(TInput[] input, int[] classIndex, double[] result);


        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   each of the given <paramref name="input"/> vectors and the 
        ///   given <paramref name="classIndex">class indices</paramref>.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// 
        double[] Score(TInput[] input, int classIndex);

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
        double[] Score(TInput[] input, int classIndex, double[] result);



        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   the given <paramref name="input"/> vector and each class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        double[] Scores(TInput input);

        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   the given <paramref name="input"/> vector and each class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///   
        double[] Scores(TInput input, double[] result);

        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   each of the given <paramref name="input"/> vectors and each
        ///   possible class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// 
        double[][] Scores(TInput[] input);

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
        double[][] Scores(TInput[] input, double[][] result);


        /// <summary>
        /// Views this instance as a multi-class score-based classifier.
        /// </summary>
        /// 
        /// <returns>
        /// This instance seen as an <see cref="IMulticlassScoreClassifierBase{TInput, TClasses}" />.
        /// </returns>
        /// 
        IMulticlassScoreClassifier<TInput> ToMulticlass();

        /// <summary>
        /// Views this instance as a multi-class score-based classifier.
        /// </summary>
        /// 
        /// <returns>
        /// This instance seen as an <see cref="IMulticlassScoreClassifierBase{TInput, TClasses}" />.
        /// </returns>
        /// 
        IMulticlassScoreClassifier<TInput, T> ToMulticlass<T>();
    }
}
