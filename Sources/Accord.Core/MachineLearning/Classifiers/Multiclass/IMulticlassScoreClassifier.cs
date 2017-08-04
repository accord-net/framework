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
    ///   Common interface for score-based multi-class classifiers. A multi-class
    ///   classifier can predict to which class an instance belongs based
    ///   on a decision score (a real number) that measures the association of the
    ///   input with each class.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int.</typeparam>
    /// 
    public interface IMulticlassScoreClassifierBase<in TInput, TClasses>
    {
        /// <summary>
        ///   Predicts a class label for each input vector, returning a
        ///   numerical score measuring the strength of association of the
        ///   input vector to the most strongly related class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels predicted for each input
        ///   vector, as predicted by the classifier.</param>
        /// 
        double[] Score(TInput[] input, ref TClasses[] decision);

        /// <summary>
        ///   Predicts a class label for each input vector, returning a
        ///   numerical score measuring the strength of association of the
        ///   input vector to the most strongly related class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels predicted for each input
        ///   vector, as predicted by the classifier.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] Score(TInput[] input, ref TClasses[] decision, double[] result);

    }

    /// <summary>
    ///   Common interface for score-based multi-class classifiers. A multi-class
    ///   classifier can predict to which class an instance belongs based
    ///   on a decision score (a real number) that measures the association of the
    ///   input with each class.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int.</typeparam>
    /// 
    public interface IMulticlassOutScoreClassifier<TInput, TClasses> :
        IMulticlassScoreClassifierBase<TInput, TClasses>,
        IMultilabelOutScoreClassifier<TInput, TClasses>
    {
        /// <summary>
        ///   Predicts a class label for the input vector, returning a
        ///   numerical score measuring the strength of association of the
        ///   input vector to its most strongly related class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// 
        double Score(TInput input, out TClasses decision);
    }

    /// <summary>
    ///   Common interface for score-based multi-class classifiers. A multi-class
    ///   classifier can predict to which class an instance belongs based
    ///   on a decision score (a real number) that measures the association of the
    ///   input with each class.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int.</typeparam>
    /// 
    public interface IMulticlassRefScoreClassifier<TInput, TClasses> :
        //IMulticlassDistanceClassifier<TInput, TClasses>,
        IMultilabelRefScoreClassifier<TInput, TClasses>
    {
        ///// <summary>
        /////   Predicts a class label for the input vector, returning a
        /////   numerical score measuring the strength of association of the
        /////   input vector to its most strongly related class.
        ///// </summary>
        ///// 
        ///// <param name="input">The input vector.</param>
        ///// <param name="decision">The class label predicted by the classifier.</param>
        ///// 
        //double Distance(TInput input, ref TClasses decision);
    }

    /// <summary>
    ///   Common interface for score-based multi-class classifiers. A multi-class
    ///   classifier can predict to which class an instance belongs based
    ///   on a decision score (a real number) that measures the association of the
    ///   input with each class.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int.</typeparam>
    /// 
    public interface IMulticlassScoreClassifier<TInput, TClasses>
        : IMulticlassOutScoreClassifier<TInput, TClasses>,
          IMulticlassRefScoreClassifier<TInput, TClasses[]>,
          IClassifier<TInput, TClasses>
    {
    }

    /// <summary>
    ///   Common interface for score-based multi-class classifiers. A multi-class
    ///   classifier can predict to which class an instance belongs based
    ///   on a decision score (a real number) that measures the association of the
    ///   input with each class.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// 
    public interface IMulticlassScoreClassifier<TInput> :
        IMulticlassScoreClassifier<TInput, int>,
        IMulticlassScoreClassifier<TInput, double>,
        IMulticlassRefScoreClassifier<TInput, bool[]>,
        IMultilabelScoreClassifier<TInput>,
        IMulticlassClassifier<TInput>
    {
        /// <summary>
        ///   Computes a class-label decision for a given <paramref name="input"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vector that should be classified into
        ///   one of the <see cref="ITransform.NumberOfOutputs"/> possible classes.</param>
        /// 
        /// <returns>A class-label that best described <paramref name="input"/> according
        /// to this classifier.</returns>
        /// 
        new int Decide(TInput input);

        /// <summary>
        ///   Computes class-label decisions for each vector in the given <paramref name="input"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vectors that should be classified into
        ///   one of the <see cref="ITransform.NumberOfOutputs"/> possible classes.</param>
        /// 
        /// <returns>The class-labels that best describe each <paramref name="input"/> 
        ///   vectors according to this classifier.</returns>
        /// 
        new int[] Decide(TInput[] input);

        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   the given <paramref name="input"/> vector and its most strongly
        ///   associated class (as predicted by the classifier).
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        double Score(TInput input);

        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   each of the given <paramref name="input"/> vectors and their
        ///   respective most strongly associated classes.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// 
        double[] Score(TInput[] input);

        /// <summary>
        ///   Computes a numerical score measuring the association between
        ///   each of the given <paramref name="input"/> vectors and their
        ///   respective most strongly associated classes.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="result">An array where the result will be stored, 
        ///   avoiding unnecessary memory allocations.</param>
        ///
        double[] Score(TInput[] input, double[] result);

        /// <summary>
        ///   Views this instance as a multi-label distance classifier,
        ///   giving access to more advanced methods, such as the prediction
        ///   of one-hot vectors.
        /// </summary>
        /// 
        /// <returns>This instance seen as an <see cref="IMultilabelScoreClassifier{TInput}"/>.</returns>
        /// 
        new IMultilabelScoreClassifier<TInput> ToMultilabel();

    }
}
