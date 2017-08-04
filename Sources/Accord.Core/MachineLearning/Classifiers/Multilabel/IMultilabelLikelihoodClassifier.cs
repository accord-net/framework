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
    ///   Common interface for generative multi-label classifiers. A multi-label
    ///   classifier can predict the occurrence of multiple class labels at once.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int[].</typeparam>
    /// 
    public interface IMultilabelLikelihoodClassifierBase<TInput, TClasses> :
        IMultilabelScoreClassifierBase<TInput, TClasses>
    {
        /// <summary>
        ///   Predicts a class label vector for each input vector, returning the
        ///   probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        ///   
        double[][] Probabilities(TInput[] input, ref TClasses[] decision);

        /// <summary>
        ///   Predicts a class label vector for each input vector, returning the
        ///   probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///   
        double[][] Probabilities(TInput[] input, ref TClasses[] decision, double[][] result);

        /// <summary>
        ///   Predicts a class label vector for each input vector, returning the
        ///   log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        ///   
        double[][] LogLikelihoods(TInput[] input, ref TClasses[] decision);

        /// <summary>
        ///   Predicts a class label vector for each input vector, returning the
        ///   log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///   
        double[][] LogLikelihoods(TInput[] input, ref TClasses[] decision, double[][] result);
    }

    /// <summary>
    ///   Common interface for generative multi-label classifiers. A multi-label
    ///   classifier can predict the occurrence of multiple class labels at once,
    ///   as well as their probabilities.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int[].</typeparam>
    /// 
    public interface IMultilabelOutLikelihoodClassifier<TInput, TClasses> :
        IMultilabelLikelihoodClassifierBase<TInput, TClasses>,
        IMultilabelOutScoreClassifier<TInput, TClasses>
    {
        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning the
        ///   probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        ///   vector, as predicted by the classifier. If passed as null, the classifier
        ///   will create a new array.</param>
        /// 
        double[] Probabilities(TInput input, out TClasses decision);

        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning the
        ///   probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        ///   vector, as predicted by the classifier. If passed as null, the classifier
        ///   will create a new array.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] Probabilities(TInput input, out TClasses decision, double[] result);


        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning the
        ///   log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        ///   vector, as predicted by the classifier. If passed as null, the classifier
        ///   will create a new array.</param>
        /// 
        double[] LogLikelihoods(TInput input, out TClasses decision);

        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning the
        ///   log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        ///   vector, as predicted by the classifier. If passed as null, the classifier
        ///   will create a new array.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] LogLikelihoods(TInput input, out TClasses decision, double[] result);

    }

    /// <summary>
    ///   Common interface for generative multi-label classifiers. A multi-label
    ///   classifier can predict the occurrence of multiple class labels at once,
    ///   as well as their probabilities.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int[].</typeparam>
    /// 
    public interface IMultilabelRefLikelihoodClassifier<TInput, TClasses> :
        IMultilabelLikelihoodClassifierBase<TInput, TClasses>,
        IMultilabelRefScoreClassifier<TInput, TClasses>
    {
        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning the
        ///   probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        ///
        double[] Probabilities(TInput input, ref TClasses decision);

        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning the
        ///   probabilities of the input vector belonging to each possible class.
        /// </summary>
        ///
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] Probabilities(TInput input, ref TClasses decision, double[] result);


        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning the
        ///   log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        ///
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        ///
        double[] LogLikelihoods(TInput input, ref TClasses decision);

        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning the
        ///   log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        ///
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] LogLikelihoods(TInput input, ref TClasses decision, double[] result);

    }

    /// <summary>
    ///   Common interface for generative multi-label classifiers. A multi-label
    ///   classifier can predict the occurrence of multiple class labels at once.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int[].</typeparam>
    /// 
    public interface IMultilabelLikelihoodClassifier<TInput, TClasses> :
        IMultilabelOutLikelihoodClassifier<TInput, TClasses>,
        IMultilabelRefLikelihoodClassifier<TInput, TClasses[]>
    {
    }

    /// <summary>
    ///   Common interface for generative multi-label classifiers. A multi-label
    ///   classifier can predict the occurrence of multiple class labels at once,
    ///   as well as their probabilities.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// 
    public interface IMultilabelLikelihoodClassifier<TInput> :
        IMultilabelLikelihoodClassifier<TInput, int>,
        IMultilabelLikelihoodClassifier<TInput, double>,
        IMultilabelRefLikelihoodClassifier<TInput, bool[]>,
        IMultilabelScoreClassifier<TInput>
    {

        /// <summary>
        ///   Computes the probability that the given input vector
        ///   belongs to the specified <paramref name="classIndex"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// 
        double Probability(TInput input, int classIndex);

        /// <summary>
        ///   Computes the probability that the given input vectors
        ///   belongs to each class specified in <paramref name="classIndex"/>.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="classIndex">The class indices associated with each
        ///   input vector whose scores will be computed.</param>
        /// 
        double[] Probability(TInput[] input, int[] classIndex);

        /// <summary>
        ///   Computes the probability that the given input vectors
        ///   belongs to each class specified in <paramref name="classIndex"/>.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="classIndex">The class indices associated with each
        ///   input vector whose scores will be computed.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] Probability(TInput[] input, int[] classIndex, double[] result);

        /// <summary>
        ///   Computes the probability that the given input vectors
        ///   belongs to each class specified in <paramref name="classIndex"/>.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="classIndex">The class indices associated with each
        ///   input vector whose scores will be computed.</param>
        /// 
        double[] Probability(TInput[] input, int classIndex);

        /// <summary>
        ///   Computes the probability that the given input vectors
        ///   belongs to each class specified in <paramref name="classIndex"/>.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="classIndex">The class indices associated with each
        ///   input vector whose scores will be computed.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] Probability(TInput[] input, int classIndex, double[] result);


        /// <summary>
        ///   Computes the log-likelihood that the given input vector
        ///   belongs to the specified <paramref name="classIndex"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// 
        double LogLikelihood(TInput input, int classIndex);

        /// <summary>
        ///   Computes the log-likelihood that the given input vectors
        ///   belongs to each class specified in <paramref name="classIndex"/>.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="classIndex">The class indices associated with each
        ///   input vector whose scores will be computed.</param>
        /// 
        double[] LogLikelihood(TInput[] input, int[] classIndex);

        /// <summary>
        ///   Computes the log-likelihood that the given input vectors
        ///   belongs to each class specified in <paramref name="classIndex"/>.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="classIndex">The class indices associated with each
        ///   input vector whose scores will be computed.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] LogLikelihood(TInput[] input, int[] classIndex, double[] result);

        /// <summary>
        ///   Computes the log-likelihood that the given input vectors
        ///   belongs to each class specified in <paramref name="classIndex"/>.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="classIndex">The class indices associated with each
        ///   input vector whose scores will be computed.</param>
        /// 
        double[] LogLikelihood(TInput[] input, int classIndex);

        /// <summary>
        ///   Computes the log-likelihood that the given input vectors
        ///   belongs to each class specified in <paramref name="classIndex"/>.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="classIndex">The class indices associated with each
        ///   input vector whose scores will be computed.</param>
        /// <param name="result">An array where the distances will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] LogLikelihood(TInput[] input, int classIndex, double[] result);




        /// <summary>
        ///   Computes the log-likelihood that the given input 
        ///   vector belongs to each of the possible classes.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        double[] LogLikelihoods(TInput input);

        /// <summary>
        ///   Computes the log-likelihood that the given input 
        ///   vector belongs to each of the possible classes.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] LogLikelihoods(TInput input, double[] result);

        /// <summary>
        ///   Computes the log-likelihoods that the given input 
        ///   vectors belongs to each of the possible classes.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// 
        double[][] LogLikelihoods(TInput[] input);

        /// <summary>
        ///   Computes the log-likelihoods that the given input 
        ///   vectors belongs to each of the possible classes.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[][] LogLikelihoods(TInput[] input, double[][] result);


        /// <summary>
        ///   Computes the probabilities that the given input 
        ///   vector belongs to each of the possible classes.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        double[] Probabilities(TInput input);

        /// <summary>
        ///   Computes the probabilities that the given input 
        ///   vector belongs to each of the possible classes.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[] Probabilities(TInput input, double[] result);

        /// <summary>
        ///   Computes the probabilities that the given input 
        ///   vectors belongs to each of the possible classes.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// 
        double[][] Probabilities(TInput[] input);

        /// <summary>
        ///   Computes the probabilities that the given input 
        ///   vectors belongs to each of the possible classes.
        /// </summary>
        /// 
        /// <param name="input">A set of input vectors.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        double[][] Probabilities(TInput[] input, double[][] result);


        /// <summary>
        /// Views this instance as a multi-class generative classifier.
        /// </summary>
        /// 
        /// <returns>
        /// This instance seen as an <see cref="IMulticlassLikelihoodClassifier{TInput}" />.
        /// </returns>
        /// 
        new IMulticlassLikelihoodClassifier<TInput> ToMulticlass();

        /// <summary>
        /// Views this instance as a multi-class generative classifier.
        /// </summary>
        /// 
        /// <returns>
        /// This instance seen as an <see cref="IMulticlassLikelihoodClassifier{TInput}" />.
        /// </returns>
        /// 
        new IMulticlassLikelihoodClassifier<TInput, T> ToMulticlass<T>();
    }
}
