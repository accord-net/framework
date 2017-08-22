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
    ///   Common interface for generative multi-class classifiers. A multi-class
    ///   classifier can predicts a class label based on an input instance vector.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int.</typeparam>
    /// 
    public interface IMulticlassLikelihoodClassifierBase<TInput, TClasses> :
        IMulticlassScoreClassifierBase<TInput, TClasses>
    {
        /// <summary>
        ///   Predicts a class label for each input vector, returning the
        ///   probability that each vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        ///   vector, as predicted by the classifier. If passed as null, the classifier
        ///   will create a new array.</param>
        /// 
        double[] Probability(TInput[] input, ref TClasses[] decision);

        /// <summary>
        ///   Predicts a class label for each input vector, returning the
        ///   probability that each vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        ///   vector, as predicted by the classifier. If passed as null, the classifier
        ///   will create a new array.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] Probability(TInput[] input, ref TClasses[] decision, double[] result);


        /// <summary>
        ///   Predicts a class label for each input vector, returning the
        ///   log-likelihood that each vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        ///   vector, as predicted by the classifier. If passed as null, the classifier
        ///   will create a new array.</param>
        /// 
        double[] LogLikelihood(TInput[] input, ref TClasses[] decision);

        /// <summary>
        ///   Predicts a class label for each input vector, returning the
        ///   log-likelihood that each vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        ///   vector, as predicted by the classifier. If passed as null, the classifier
        ///   will create a new array.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] LogLikelihood(TInput[] input, ref TClasses[] decision, double[] result);
    }

    /// <summary>
    ///   Common interface for generative multi-class classifiers. A multi-class
    ///   classifier can predicts a class label based on an input instance vector.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int.</typeparam>
    /// 
    public interface IMulticlassOutLikelihoodClassifier<TInput, TClasses> :
        IMulticlassOutScoreClassifier<TInput, TClasses>,
        IMultilabelOutLikelihoodClassifier<TInput, TClasses>,
        IMulticlassLikelihoodClassifierBase<TInput, TClasses>
    {
        /// <summary>
        ///   Predicts a class label for the given input vector, returning the
        ///   probability that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        ///
        double Probability(TInput input, out TClasses decision);

        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning the
        ///   log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        ///
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        ///
        double LogLikelihood(TInput input, out TClasses decision);

    }

    /// <summary>
    ///   Common interface for generative multi-class classifiers. A multi-class
    ///   classifier can predicts a class label based on an input instance vector.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int.</typeparam>
    /// 
    public interface IMulticlassRefLikelihoodClassifier<TInput, TClasses> :
        IMulticlassRefScoreClassifier<TInput, TClasses>,
        IMultilabelRefLikelihoodClassifier<TInput, TClasses>
        //IMulticlassGenerativeClassifier<TInput, TClasses>
    {
        ///// <summary>
        /////   Predicts a class label for the given input vector, returning the
        /////   probability that the input vector belongs to its predicted class.
        ///// </summary>
        ///// 
        ///// <param name="input">The input vector.</param>
        ///// <param name="decision">The class label predicted by the classifier.</param>
        ///// 
        //double Probability(TInput input, ref TClasses decision);

        ///// <summary>
        /////   Predicts a class label vector for the given input vector, returning the
        /////   log-likelihood that the input vector belongs to its predicted class.
        ///// </summary>
        ///// 
        ///// <param name="input">The input vector.</param>
        ///// <param name="decision">The class label predicted by the classifier.</param>
        /////
        //double LogLikelihood(TInput input, ref TClasses decision);
    }

    /// <summary>
    ///   Common interface for generative multi-class classifiers. A multi-class
    ///   classifier can predicts a class label based on an input instance vector.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int.</typeparam>
    /// 
    public interface IMulticlassLikelihoodClassifier<TInput, TClasses> :
        IMulticlassOutLikelihoodClassifier<TInput, TClasses>,
        IMulticlassRefLikelihoodClassifier<TInput, TClasses[]>,
        IMulticlassScoreClassifier<TInput, TClasses>
    {
    }

    /// <summary>
    ///   Common interface for generative multi-class classifiers. A multi-class
    ///   classifier can predicts a class label based on an input instance vector.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// 
    public interface IMulticlassLikelihoodClassifier<TInput> :
        IMulticlassLikelihoodClassifier<TInput, int>,
        IMulticlassLikelihoodClassifier<TInput, double>,
        IMulticlassRefLikelihoodClassifier<TInput, bool[]>,
        IMulticlassScoreClassifier<TInput>,
        IMultilabelLikelihoodClassifier<TInput>
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
        ///   Predicts a class label for the given input vector, returning the
        ///   probability that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        ///
        double Probability(TInput input);

        /// <summary>
        ///   Predicts a class label for the given input vectors, returning the
        ///   probability that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        ///
        double[] Probability(TInput[] input);

        /// <summary>
        ///   Predicts a class label for the given input vectors, returning the
        ///   probability that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        double[] Probability(TInput[] input, double[] result);

        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning the
        ///   log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        ///
        /// <param name="input">The input vector.</param>
        ///
        double LogLikelihood(TInput input);

        /// <summary>
        ///   Predicts a class label vector for the given input vectors, returning the
        ///   log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        ///
        /// <param name="input">The input vector.</param>
        ///
        double[] LogLikelihood(TInput[] input);

        /// <summary>
        ///   Predicts a class label vector for the given input vectors, returning the
        ///   log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        ///
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///
        double[] LogLikelihood(TInput[] input, double[] result);


        /// <summary>
        ///   Views this instance as a multi-label generative classifier,
        ///   giving access to more advanced methods, such as the prediction
        ///   of one-hot vectors.
        /// </summary>
        /// 
        /// <returns>This instance seen as an <see cref="IMultilabelLikelihoodClassifier{TInput}"/>.</returns>
        /// 
        new IMultilabelLikelihoodClassifier<TInput> ToMultilabel();

    }
}
