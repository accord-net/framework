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
    using System;
    using Accord.Compat;

    /// <summary>
    /// Base class for generative multi-class classifiers.
    /// </summary>
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    [Serializable]
    public abstract class MulticlassLikelihoodClassifierBase<TInput> :
        MulticlassScoreClassifierBase<TInput>,
        IMulticlassLikelihoodClassifier<TInput>
    {


        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <returns>System.Double.</returns>
        public override double Score(TInput input, int classIndex)
        {
            return LogLikelihood(input, classIndex);
        }

        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// 
        public virtual double LogLikelihood(TInput input, int classIndex)
        {
            return LogLikelihoods(input)[classIndex];
        }


        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public virtual double[] LogLikelihoods(TInput input, out int decision, double[] result)
        {
            LogLikelihoods(input, result);
            decision = result.ArgMax();
            return result;
        }


        /// <summary>
        /// Computes the log-likelihood that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///   
        public virtual double[] LogLikelihoods(TInput input, double[] result)
        {
            for (int i = 0; i < NumberOfOutputs; i++)
                result[i] = LogLikelihood(input, classIndex: i);
            return result;
        }






        // Input, classIndex
        /// <summary>
        /// Computes the probability that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// 
        public double Probability(TInput input, int classIndex)
        {
            return Probabilities(input)[classIndex];
        }

        /// <summary>
        /// Computes the probability that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        public double[] Probability(TInput[] input, int classIndex)
        {
            return Probability(input, classIndex, new double[input.Length]);
        }


        /// <summary>
        /// Computes the probability that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///   
        public double[] Probability(TInput[] input, int classIndex, double[] result)
        {
            var temp = new double[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
            {
                Probabilities(input[i], result: temp);
                result[i] = temp[classIndex];
            }

            return result;
        }


        // Input, classIndex[]
        /// <summary>
        /// Computes the probability that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// 
        public double[] Probability(TInput[] input, int[] classIndex)
        {
            return Probability(input, classIndex, new double[input.Length]);
        }

        /// <summary>
        /// Computes the probability that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[] Probability(TInput[] input, int[] classIndex, double[] result)
        {
            var temp = new double[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
            {
                Probabilities(input[i], result: temp);
                result[i] = temp[classIndex[i]];
            }

            return result;
        }



        /// <summary>
        /// Computes the probabilities that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///   
        public virtual double[] Probabilities(TInput input, double[] result)
        {
            LogLikelihoods(input, result);
            Special.Softmax(result, result);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public virtual double[] Probabilities(TInput input, out int decision, double[] result)
        {
            result = Probabilities(input, result);
            decision = result.ArgMax();
            return result;
        }






        // LogLikelihood, input, classIndex

        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// 
        public double[] LogLikelihood(TInput[] input, int[] classIndex)
        {
            return LogLikelihood(input, classIndex, new double[input.Length]);
        }

        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///   
        public virtual double[] LogLikelihood(TInput[] input, int[] classIndex, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = LogLikelihood(input[i], classIndex[i]);
            return result;
        }

        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        ///   
        public double[] LogLikelihoods(TInput[] input, int classIndex)
        {
            return LogLikelihoods(input, classIndex, new double[input.Length]);
        }

        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///   
        public double[] LogLikelihoods(TInput[] input, int classIndex, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = LogLikelihood(input[i], classIndex);
            return result;
        }

        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        ///   
        public double[] LogLikelihood(TInput[] input, int classIndex)
        {
            return LogLikelihood(input, classIndex, new double[input.Length]);
        }

        /// <summary>
        /// Computes the log-likelihood that the given input vector
        /// belongs to the specified <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///   
        public double[] LogLikelihood(TInput[] input, int classIndex, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = LogLikelihood(input[i], classIndex);
            return result;
        }







        // Same as Distance methods in IMultilabelDistanceClassifier, 
        // but replaced with LogLikelihood

        #region LogLikelihood

        // Input

        /// <summary>
        /// Computes the log-likelihood that the given input
        /// vector belongs to its most plausible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// 
        public virtual double LogLikelihood(TInput input)
        {
            int value;
            var result = LogLikelihoods(input, out value);
            return result[value];
        }

        /// <summary>
        /// Computes the log-likelihood that the given input
        /// vectors belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// 
        public double[] LogLikelihood(TInput[] input)
        {
            return LogLikelihood(input, new double[input.Length]);
        }

        /// <summary>
        /// Computes the log-likelihood that the given input
        /// vectors belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[] LogLikelihood(TInput[] input, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = LogLikelihood(input[i]);
            return result;
        }

        /// <summary>
        /// Computes the log-likelihood that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// 
        public double[] LogLikelihoods(TInput input)
        {
            return LogLikelihoods(input, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Computes the log-likelihood that the given input
        /// vectors belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// 
        public double[][] LogLikelihoods(TInput[] input)
        {
            return LogLikelihoods(input, create<double>(input));
        }


        // Input, result

        /// <summary>
        /// Computes the log-likelihood that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[][] LogLikelihoods(TInput[] input, double[][] result)
        {
            for (int i = 0; i < input.Length; i++)
                LogLikelihoods(input[i], result[i]);
            return result;
        }


        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// 
        public virtual double LogLikelihood(TInput input, out int decision)
        {
            double[] result = LogLikelihoods(input, out decision);
            return result[decision];
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// 
        public double LogLikelihood(TInput input, out double decision)
        {
            int value;
            double[] result = LogLikelihoods(input, out value);
            decision = value;
            return result[value];
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// 
        public double[] LogLikelihoods(TInput input, out int decision)
        {
            return LogLikelihoods(input, out decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// 
        public double[] LogLikelihoods(TInput input, out double decision)
        {
            return LogLikelihoods(input, out decision, new double[NumberOfOutputs]);
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, bool[]>.LogLikelihoods(TInput input, ref bool[] decision)
        {
            return ToMultilabel().LogLikelihoods(input, ref decision, new double[NumberOfOutputs]);
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, int[]>.LogLikelihoods(TInput input, ref int[] decision)
        {
            return ToMultilabel().LogLikelihoods(input, ref decision, new double[NumberOfOutputs]);
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, double[]>.LogLikelihoods(TInput input, ref double[] decision)
        {
            return ToMultilabel().LogLikelihoods(input, ref decision, new double[NumberOfOutputs]);
        }

        //double LogLikelihood(TInput input, ref double[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = LogLikelihood(input, out value);
        //    Vector.OneHot<double>(value, decision);
        //    return result;
        //}

        //double LogLikelihood(TInput input, ref bool[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = LogLikelihood(input, out value);
        //    Vector.OneHot<bool>(value, decision);
        //    return result;
        //}

        //double LogLikelihood(TInput input, ref int[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = LogLikelihood(input, out value);
        //    Vector.OneHot<int>(value, decision);
        //    return result;
        //}



        // Input, decision, result

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[] LogLikelihoods(TInput input, out double decision, double[] result)
        {
            int value;
            LogLikelihoods(input, out value, result);
            decision = value;
            return result;
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, bool[]>.LogLikelihoods(TInput input, ref bool[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            LogLikelihoods(input, out value, result);
            Vector.OneHot<bool>(value, decision);
            return result;
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, int[]>.LogLikelihoods(TInput input, ref int[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            LogLikelihoods(input, out value, result);
            Vector.OneHot<int>(value, decision);
            return result;
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, double[]>.LogLikelihoods(TInput input, ref double[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            LogLikelihoods(input, out value, result);
            Vector.OneHot<double>(value, decision);
            return result;
        }



        // Input[], decision[]

        /// <summary>
        /// Predicts a class label for each input vector, returning the
        /// log-likelihood that each vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[] LogLikelihood(TInput[] input, ref int[] decision)
        {
            return LogLikelihood(input, ref decision, new double[input.Length]);
        }

        /// <summary>
        /// Predicts a class label for each input vector, returning the
        /// log-likelihood that each vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>

        public double[] LogLikelihood(TInput[] input, ref double[] decision)
        {
            return LogLikelihood(input, ref decision, new double[input.Length]);
        }

        //double[] LogLikelihood(TInput[] input, ref bool[][] decision)
        //{
        //    return LogLikelihood(input, ref decision, new double[input.Length]);
        //}

        //double[] LogLikelihood(TInput[] input, ref int[][] decision)
        //{
        //    return LogLikelihood(input, ref decision, new double[input.Length]);
        //}

        //double[] LogLikelihood(TInput[] input, ref double[][] decision)
        //{
        //    return LogLikelihood(input, ref decision, new double[input.Length]);
        //}

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// 
        public double[][] LogLikelihoods(TInput[] input, ref int[] decision)
        {
            return LogLikelihoods(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// 
        public double[][] LogLikelihoods(TInput[] input, ref double[] decision)
        {
            return LogLikelihoods(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelLikelihoodClassifierBase<TInput, int[]>.LogLikelihoods(TInput[] input, ref int[][] decision)
        {
            return ToMultilabel().LogLikelihoods(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelLikelihoodClassifierBase<TInput, bool[]>.LogLikelihoods(TInput[] input, ref bool[][] decision)
        {
            return ToMultilabel().LogLikelihoods(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelLikelihoodClassifierBase<TInput, double[]>.LogLikelihoods(TInput[] input, ref double[][] decision)
        {
            return ToMultilabel().LogLikelihoods(input, ref decision, create<double>(input));
        }




        // Input, decision, result        
        /// <summary>
        /// Predicts a class label for each input vector, returning the
        /// log-likelihood that each vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[] LogLikelihood(TInput[] input, ref int[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = LogLikelihood(input[i], out decision[i]);
            return result;
        }

        /// <summary>
        /// Predicts a class label for each input vector, returning the
        /// log-likelihood that each vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[] LogLikelihood(TInput[] input, ref double[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = LogLikelihood(input[i], out decision[i]);
            return result;
        }

        //double[] LogLikelihood(TInput[] input, ref double[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    var temp = new double[NumberOfOutputs];
        //    int value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        LogLikelihoods(input[i], out value, temp);
        //        Vector.OneHot(value, decision[i]);
        //        result[i] = temp[value];
        //    }
        //    return result;
        //}

        //double[] LogLikelihood(TInput[] input, ref int[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    var temp = new double[NumberOfOutputs];
        //    int value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        LogLikelihoods(input[i], out value, temp);
        //        Vector.OneHot(value, decision[i]);
        //        result[i] = temp[value];
        //    }
        //    return result;
        //}

        //double[] LogLikelihood(TInput[] input, ref bool[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    var temp = new double[NumberOfOutputs];
        //    int value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        LogLikelihoods(input[i], out value, temp);
        //        Vector.OneHot(value, decision[i]);
        //        result[i] = temp[value];
        //    }
        //    return result;
        //}

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[][] LogLikelihoods(TInput[] input, ref int[] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                LogLikelihoods(input[i], out decision[i], result[i]);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[][] LogLikelihoods(TInput[] input, ref double[] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                LogLikelihoods(input[i], out decision[i], result[i]);
            return result;
        }

        double[][] IMultilabelLikelihoodClassifierBase<TInput, bool[]>.LogLikelihoods(TInput[] input, ref bool[][] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                ToMultilabel().LogLikelihoods(input[i], ref decision[i], result[i]);
            return result;
        }


        double[][] IMultilabelLikelihoodClassifierBase<TInput, int[]>.LogLikelihoods(TInput[] input, ref int[][] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            for (int i = 0; i < input.Length; i++)
            {
                LogLikelihoods(input[i], out value, result[i]);
                decision[i] = Vector.OneHot<int>(value, decision[i]);
            }
            return result;
        }

        double[][] IMultilabelLikelihoodClassifierBase<TInput, double[]>.LogLikelihoods(TInput[] input, ref double[][] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            for (int i = 0; i < input.Length; i++)
            {
                LogLikelihoods(input[i], out value, result[i]);
                decision[i] = Vector.OneHot<double>(value, decision[i]);
            }
            return result;
        }


        #endregion































        // Same as Distance methods in IMultilabelDistanceClassifier, 
        // but replaced with Probability

        #region Probability

        // Input

        /// <summary>
        /// Predicts a class label for the given input vector, returning the
        /// probability that the input vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        public virtual double Probability(TInput input)
        {
            int value;
            var result = Probabilities(input, out value);
            return result[value];
        }

        /// <summary>
        /// Predicts a class label for the given input vector, returning the
        /// probability that the input vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        public double[] Probability(TInput[] input)
        {
            return Probability(input, new double[input.Length]);
        }

        /// <summary>
        /// Predicts a class label for the given input vector, returning the
        /// probability that the input vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] Probability(TInput[] input, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = Probability(input[i]);
            return result;
        }

        /// <summary>
        /// Computes the probabilities that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// 
        public double[] Probabilities(TInput input)
        {
            return Probabilities(input, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Computes the probabilities that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// 
        public double[][] Probabilities(TInput[] input)
        {
            return Probabilities(input, create<double>(input));
        }


        // Input, result

        /// <summary>
        /// Computes the probabilities that the given input
        /// vector belongs to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[][] Probabilities(TInput[] input, double[][] result)
        {
            for (int i = 0; i < input.Length; i++)
                Probabilities(input[i], result[i]);
            return result;
        }




        // Input, decision

        /// <summary>
        /// Predicts a class label for the given input vector, returning the
        /// probability that the input vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// 
        public virtual double Probability(TInput input, out int decision)
        {
            double[] result = Probabilities(input, out decision);
            return result[decision];
        }

        /// <summary>
        /// Predicts a class label for the given input vector, returning the
        /// probability that the input vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// 
        public double Probability(TInput input, out double decision)
        {
            int value;
            double result = Probability(input, out value);
            decision = value;
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// 
        public double[] Probabilities(TInput input, out int decision)
        {
            return Probabilities(input, out decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// 
        public double[] Probabilities(TInput input, out double decision)
        {
            return Probabilities(input, out decision, new double[NumberOfOutputs]);
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, bool[]>.Probabilities(TInput input, ref bool[] decision)
        {
            return ToMultilabel().Probabilities(input, ref decision, new double[NumberOfOutputs]);
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, int[]>.Probabilities(TInput input, ref int[] decision)
        {
            return ToMultilabel().Probabilities(input, ref decision, new double[NumberOfOutputs]);
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, double[]>.Probabilities(TInput input, ref double[] decision)
        {
            return ToMultilabel().Probabilities(input, ref decision, new double[NumberOfOutputs]);
        }

        double Probability(TInput input, ref double[] decision)
        {
            decision = createOrReuse(input, decision);
            int value;
            double result = Probability(input, out value);
            Vector.OneHot<double>(value, decision);
            return result;
        }

        double Probability(TInput input, ref bool[] decision)
        {
            decision = createOrReuse(input, decision);
            int value;
            double result = Probability(input, out value);
            Vector.OneHot<bool>(value, decision);
            return result;
        }

        double Probability(TInput input, ref int[] decision)
        {
            decision = createOrReuse(input, decision);
            int value;
            double result = Probability(input, out value);
            Vector.OneHot<int>(value, decision);
            return result;
        }




        // Input, decision, result

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[] Probabilities(TInput input, out double decision, double[] result)
        {
            int value;
            Probabilities(input, out value, result);
            decision = value;
            return result;
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, bool[]>.Probabilities(TInput input, ref bool[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            Probabilities(input, out value, result);
            Vector.OneHot<bool>(value, decision);
            return result;
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, int[]>.Probabilities(TInput input, ref int[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            Probabilities(input, out value, result);
            Vector.OneHot<int>(value, decision);
            return result;
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, double[]>.Probabilities(TInput input, ref double[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            Probabilities(input, out value, result);
            Vector.OneHot<double>(value, decision);
            return result;
        }



        // Input[], decision[]

        /// <summary>
        /// Predicts a class label for each input vector, returning the
        /// probability that each vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[] Probability(TInput[] input, ref int[] decision)
        {
            return Probability(input, ref decision, new double[input.Length]);
        }

        /// <summary>
        /// Predicts a class label for each input vector, returning the
        /// probability that each vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[] Probability(TInput[] input, ref double[] decision)
        {
            return Probability(input, ref decision, new double[input.Length]);
        }

        //double[] IMultilabelGenerativeClassifier<TInput, bool[]>.Probability(TInput[] input, ref bool[][] decision)
        //{
        //    return Probability(input, ref decision, new double[input.Length]);
        //}

        //double[] IMulticlassGenerativeClassifier<TInput, int[]>.Probability(TInput[] input, ref int[][] decision)
        //{
        //    return Probability(input, ref decision, new double[input.Length]);
        //}

        //double[] IMulticlassGenerativeClassifier<TInput, double[]>.Probability(TInput[] input, ref double[][] decision)
        //{
        //    return Probability(input, ref decision, new double[input.Length]);
        //}

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// 
        public double[][] Probabilities(TInput[] input, ref int[] decision)
        {
            return Probabilities(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// 
        public double[][] Probabilities(TInput[] input, ref double[] decision)
        {
            return Probabilities(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelLikelihoodClassifierBase<TInput, int[]>.Probabilities(TInput[] input, ref int[][] decision)
        {
            return ToMultilabel().Probabilities(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelLikelihoodClassifierBase<TInput, bool[]>.Probabilities(TInput[] input, ref bool[][] decision)
        {
            return ToMultilabel().Probabilities(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelLikelihoodClassifierBase<TInput, double[]>.Probabilities(TInput[] input, ref double[][] decision)
        {
            return ToMultilabel().Probabilities(input, ref decision, create<double>(input));
        }




        // Input, decision, result        

        /// <summary>
        /// Predicts a class label for each input vector, returning the
        /// probability that each vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///   
        public double[] Probability(TInput[] input, ref int[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = Probability(input[i], out decision[i]);
            return result;
        }

        /// <summary>
        /// Predicts a class label for each input vector, returning the
        /// probability that each vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///   
        public double[] Probability(TInput[] input, ref double[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = Probability(input[i], out decision[i]);
            return result;
        }

        //double[] IMulticlassGenerativeClassifier<TInput, double[]>.Probability(TInput[] input, ref double[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    var temp = new double[NumberOfOutputs];
        //    int value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        Probabilities(input[i], out value, temp);
        //        Vector.OneHot(value, decision[i]);
        //        result[i] = temp[value];
        //    }
        //    return result;
        //}

        //double[] IMulticlassGenerativeClassifier<TInput, int[]>.Probability(TInput[] input, ref int[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    var temp = new double[NumberOfOutputs];
        //    int value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        Probabilities(input[i], out value, temp);
        //        Vector.OneHot(value, decision[i]);
        //        result[i] = temp[value];
        //    }
        //    return result;
        //}

        //double[] IMulticlassGenerativeClassifier<TInput, bool[]>.Probability(TInput[] input, ref bool[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    var temp = new double[NumberOfOutputs];
        //    int value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        Probabilities(input[i], out value, temp);
        //        Vector.OneHot(value, decision[i]);
        //        result[i] = temp[value];
        //    }
        //    return result;
        //}

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[][] Probabilities(TInput[] input, ref int[] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                Probabilities(input[i], out decision[i], result[i]);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        /// <param name="result">An array where the probabilities will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[][] Probabilities(TInput[] input, ref double[] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                Probabilities(input[i], out decision[i], result[i]);
            return result;
        }

        double[][] IMultilabelLikelihoodClassifierBase<TInput, bool[]>.Probabilities(TInput[] input, ref bool[][] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                ToMultilabel().Probabilities(input[i], ref decision[i], result[i]);
            return result;
        }

        double[][] IMultilabelLikelihoodClassifierBase<TInput, int[]>.Probabilities(TInput[] input, ref int[][] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            for (int i = 0; i < input.Length; i++)
            {
                Probabilities(input[i], out value, result[i]);
                decision[i] = Vector.OneHot<int>(value, decision[i]);
            }
            return result;
        }

        double[][] IMultilabelLikelihoodClassifierBase<TInput, double[]>.Probabilities(TInput[] input, ref double[][] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            for (int i = 0; i < input.Length; i++)
            {
                Probabilities(input[i], out value, result[i]);
                decision[i] = Vector.OneHot<double>(value, decision[i]);
            }
            return result;
        }

        #endregion


        // Transform
        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <param name="result">A location to store the output, avoiding unnecessary memory allocations.</param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public override double[] Transform(TInput input, double[] result)
        {
            return Probabilities(input, result);
        }

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <param name="result">A location to store the output, avoiding unnecessary memory allocations.</param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public override double[][] Transform(TInput[] input, double[][] result)
        {
            return Probabilities(input, result);
        }

        /// <summary>
        /// Views this instance as a multi-label generative classifier,
        /// giving access to more advanced methods, such as the prediction
        /// of one-hot vectors.
        /// </summary>
        /// <returns>
        /// This instance seen as an <see cref="IMultilabelLikelihoodClassifier{TInput}" />.
        /// </returns>
        new public IMultilabelLikelihoodClassifier<TInput> ToMultilabel()
        {
            return (IMultilabelLikelihoodClassifier<TInput>)this;
        }

        /// <summary>
        /// Views this instance as a multi-class generative classifier.
        /// </summary>
        /// <returns>
        /// This instance seen as an <see cref="IMulticlassLikelihoodClassifier{TInput}" />.
        /// </returns>
        new public IMulticlassLikelihoodClassifier<TInput> ToMulticlass()
        {
            return (IMulticlassLikelihoodClassifier<TInput>)this;
        }

        /// <summary>
        /// Views this instance as a multi-class generative classifier.
        /// </summary>
        /// <returns>
        /// This instance seen as an <see cref="IMulticlassLikelihoodClassifier{TInput}" />.
        /// </returns>
        new public IMulticlassLikelihoodClassifier<TInput, T> ToMulticlass<T>()
        {
            return (IMulticlassLikelihoodClassifier<TInput, T>)this;
        }

    }
}
