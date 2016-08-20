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
    using Accord.Math;
    using Accord.Statistics;
    using System;

    // TODO: Rename to BinaryLikelihoodClassifierBase

    /// <summary>
    ///   Base class for <see cref="IBinaryLikelihoodClassifier{TInput}">
    ///   generative binary classifiers</see>.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// 
    [Serializable]
    public abstract class BinaryLikelihoodClassifierBase<TInput> :
        BinaryScoreClassifierBase<TInput>,
        IBinaryLikelihoodClassifier<TInput>
    {


        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and its most strongly
        /// associated class (as predicted by the classifier).
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <returns>System.Double.</returns>
        public override double Score(TInput input)
        {
            return LogLikelihood(input);
        }


        /// <summary>
        ///   Predicts a class label vector for the given input vector, returning the
        ///   log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        ///
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        ///
        public abstract double LogLikelihood(TInput input, out bool decision);

        /// <summary>
        ///   Predicts a class label for the given input vector, returning the
        ///   probability that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        ///
        public virtual double Probability(TInput input, out bool decision)
        {
            double exp = Math.Exp(LogLikelihood(input, out decision));
//#if DEBUG
//            if (exp < 0 || exp > 1)
//                throw new InvalidOperationException();
//#endif
            return exp;
        }


        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        public abstract double LogLikelihood(TInput input);


        /// <summary>
        /// Predicts a class label for the given input vector, returning the
        /// probability that the input vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        public double Probability(TInput input)
        {
            double exp = Math.Exp(LogLikelihood(input));
//#if DEBUG
//            if (exp < 0 || exp > 1)
//                throw new InvalidOperationException();
//#endif
            return exp;
        }



        #region LogLikelihoods

        // Input, classIndex
        double IMultilabelLikelihoodClassifier<TInput>.LogLikelihood(TInput input, int classIndex)
        {
            return ToMultilabel().LogLikelihoods(input)[classIndex];
        }

        double[] IMultilabelLikelihoodClassifier<TInput>.LogLikelihood(TInput[] input, int classIndex)
        {
            return ToMulticlass().LogLikelihood(input, classIndex, new double[input.Length]);
        }

        double[] IMultilabelLikelihoodClassifier<TInput>.LogLikelihood(TInput[] input, int classIndex, double[] result)
        {
            var temp = new double[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
            {
                ToMultilabel().LogLikelihoods(input[i], result: temp);
                result[i] = temp[classIndex];
            }

            return result;
        }


        // Input, classIndex[]
        double[] IMultilabelLikelihoodClassifier<TInput>.LogLikelihood(TInput[] input, int[] classIndex)
        {
            return ToMulticlass().LogLikelihood(input, classIndex, new double[input.Length]);
        }

        double[] IMultilabelLikelihoodClassifier<TInput>.LogLikelihood(TInput[] input, int[] classIndex, double[] result)
        {
            var temp = new double[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
            {
                ToMultilabel().LogLikelihoods(input[i], result: temp);
                result[i] = temp[classIndex[i]];
            }

            return result;
        }


        // Input

        double[] IMultilabelLikelihoodClassifier<TInput>.LogLikelihoods(TInput input)
        {
            return ToMultilabel().LogLikelihoods(input, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        public double[] LogLikelihood(TInput[] input)
        {
            return LogLikelihood(input, new double[input.Length]);
        }

        double[][] IMultilabelLikelihoodClassifier<TInput>.LogLikelihoods(TInput[] input)
        {
            return ToMultilabel().LogLikelihoods(input, create<double>(input));
        }


        // Input, result

        double[] IMultilabelLikelihoodClassifier<TInput>.LogLikelihoods(TInput input, double[] result)
        {
            double d = LogLikelihood(input);
            result[0] = +d;
            result[1] = 1 - d;
            return result;
        }


        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] LogLikelihood(TInput[] input, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = LogLikelihood(input[i]);
            return result;
        }

        double[][] IMultilabelLikelihoodClassifier<TInput>.LogLikelihoods(TInput[] input, double[][] result)
        {
            for (int i = 0; i < input.Length; i++)
            {
                double d = LogLikelihood(input[i]);
                result[i][0] = +d;
                result[i][1] = 1 - d;
            }
            return result;
        }



        // Input, decision

        double IMulticlassOutLikelihoodClassifier<TInput, double>.LogLikelihood(TInput input, out double decision)
        {
            bool value;
            double d = LogLikelihood(input, out value);
            decision = Classes.ToZeroOne(value);
            return d;
        }

        double IMulticlassOutLikelihoodClassifier<TInput, int>.LogLikelihood(TInput input, out int decision)
        {
            bool value;
            double d = LogLikelihood(input, out value);
            decision = Classes.ToZeroOne(value);
            return d;
        }



        double[] IMultilabelOutLikelihoodClassifier<TInput, bool>.LogLikelihoods(TInput input, out bool decision)
        {
            return LogLikelihoods(input, out decision, new double[NumberOfOutputs]);
        }

        double[] IMultilabelOutLikelihoodClassifier<TInput, int>.LogLikelihoods(TInput input, out int decision)
        {
            return ToMultilabel().LogLikelihoods(input, out decision, new double[NumberOfOutputs]);
        }

        double[] IMultilabelOutLikelihoodClassifier<TInput, double>.LogLikelihoods(TInput input, out double decision)
        {
            return ToMultilabel().LogLikelihoods(input, out decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        public double[] LogLikelihoods(TInput input, ref bool[] decision)
        {
            return LogLikelihoods(input, ref decision, new double[NumberOfOutputs]);
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, int[]>.LogLikelihoods(TInput input, ref int[] decision)
        {
            return ToMultilabel().LogLikelihoods(input, ref decision, new double[NumberOfOutputs]);
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, double[]>.LogLikelihoods(TInput input, ref double[] decision)
        {
            return ToMultilabel().LogLikelihoods(input, ref decision, new double[NumberOfOutputs]);
        }

        //double[] IMulticlassGenerativeClassifier<TInput, double[]>.LogLikelihood(TInput[] input, ref double[][] decision)
        //{
        //    return ToMulticlass().LogLikelihood(input, ref decision, new double[input.Length]);
        //}

        //double IMulticlassRefGenerativeClassifier<TInput, double[]>.LogLikelihood(TInput input, ref double[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = ToMulticlass().LogLikelihood(input, out value);
        //    Vector.OneHot(value, decision);
        //    return result;
        //}

        //public double LogLikelihood(TInput input, ref bool[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = ToMulticlass().LogLikelihood(input, out value);
        //    Vector.OneHot(value, decision);
        //    return result;
        //}

        //double IMulticlassRefGenerativeClassifier<TInput, int[]>.LogLikelihood(TInput input, ref int[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = ToMulticlass().LogLikelihood(input, out value);
        //    Vector.OneHot(value, decision);
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
        public double[] LogLikelihoods(TInput input, out bool decision, double[] result)
        {
            double d = LogLikelihood(input, out decision);
            result[0] = +d;
            result[1] = 1 - d;
            return result;
        }


        double[] IMultilabelOutLikelihoodClassifier<TInput, double>.LogLikelihoods(TInput input, out double decision, double[] result)
        {
            bool value;
            double d = LogLikelihood(input, out value);
            result[0] = +d;
            result[1] = 1 - d;
            decision = Classes.ToZeroOne(value);
            return result;
        }


        double[] IMultilabelOutLikelihoodClassifier<TInput, int>.LogLikelihoods(TInput input, out int decision, double[] result)
        {
            bool value;
            double d = LogLikelihood(input, out value);
            result[0] = +d;
            result[1] = 1 - d;
            decision = Classes.ToZeroOne(value);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] LogLikelihoods(TInput input, ref bool[] decision, double[] result)
        {
            bool value;
            double d = LogLikelihood(input, out value);
            result[0] = +d;
            result[1] = 1 - d;
            Vector.OneHot(value, decision);
            return result;
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, int[]>.LogLikelihoods(TInput input, ref int[] decision, double[] result)
        {
            bool value;
            double d = LogLikelihood(input, out value);
            result[0] = +d;
            result[1] = 1 - d;
            Vector.OneHot(value, decision);
            return result;
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, double[]>.LogLikelihoods(TInput input, ref double[] decision, double[] result)
        {
            bool value;
            double d = LogLikelihood(input, out value);
            result[0] = +d;
            result[1] = 1 - d;
            Vector.OneHot(value, decision);
            return result;
        }


        // Input[], decision[]

        double[] IMulticlassLikelihoodClassifier<TInput, int>.LogLikelihood(TInput[] input, ref int[] decision)
        {
            return ToMulticlass().LogLikelihood(input, ref decision, new double[input.Length]);
        }

        double[] IMulticlassLikelihoodClassifier<TInput, double>.LogLikelihood(TInput[] input, ref double[] decision)
        {
            return ToMulticlass().LogLikelihood(input, ref decision, new double[input.Length]);
        }

        /// <summary>
        /// Predicts a class label for each input vector, returning the
        /// log-likelihood that each vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[] LogLikelihood(TInput[] input, ref bool[] decision)
        {
            return LogLikelihood(input, ref decision, new double[input.Length]);
        }

        //double[] IMulticlassGenerativeClassifier<TInput, int[]>.LogLikelihood(TInput[] input, ref int[][] decision)
        //{
        //    return ToMulticlass().LogLikelihood(input, ref decision, new double[input.Length]);
        //}

        //public double[] LogLikelihood(TInput[] input, ref bool[][] decision)
        //{
        //    return ToMulticlass().LogLikelihood(input, ref decision, new double[input.Length]);
        //}








        double[][] IMultilabelLikelihoodClassifier<TInput, int>.LogLikelihoods(TInput[] input, ref int[] decision)
        {
            return ToMultilabel().LogLikelihoods(input, ref decision, create<double>(input));
        }


        double[][] IMultilabelLikelihoodClassifier<TInput, double>.LogLikelihoods(TInput[] input, ref double[] decision)
        {
            return ToMultilabel().LogLikelihoods(input, ref decision, create<double>(input));
        }


        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        public double[][] LogLikelihoods(TInput[] input, ref bool[] decision)
        {
            return LogLikelihoods(input, ref decision, create<double>(input));
        }


        double[][] IMultilabelLikelihoodClassifier<TInput, int[]>.LogLikelihoods(TInput[] input, ref int[][] decision)
        {
            return ToMultilabel().LogLikelihoods(input, ref decision, create<double>(input));
        }


        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// log-likelihoods of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        public double[][] LogLikelihoods(TInput[] input, ref bool[][] decision)
        {
            return ToMultilabel().LogLikelihoods(input, ref decision, create<double>(input));
        }


        double[][] IMultilabelLikelihoodClassifier<TInput, double[]>.LogLikelihoods(TInput[] input, ref double[][] decision)
        {
            return ToMultilabel().LogLikelihoods(input, ref decision, create<double>(input));
        }




        // Input[], decision[], result[]        

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
        public double[] LogLikelihood(TInput[] input, ref bool[] decision, double[] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = LogLikelihood(input[i], out decision[i]);
            return result;
        }

        double[] IMulticlassLikelihoodClassifier<TInput, int>.LogLikelihood(TInput[] input, ref int[] decision, double[] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = ToMulticlass().LogLikelihood(input[i], out decision[i]);
            return result;
        }

        double[] IMulticlassLikelihoodClassifier<TInput, double>.LogLikelihood(TInput[] input, ref double[] decision, double[] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = ToMulticlass().LogLikelihood(input[i], out decision[i]);
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
        public double[][] LogLikelihoods(TInput[] input, ref bool[][] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                LogLikelihoods(input[i], ref decision[i], result[i]);
            return result;
        }

        double[][] IMultilabelLikelihoodClassifier<TInput, int[]>.LogLikelihoods(TInput[] input, ref int[][] decision, double[][] result)
        {
            decision = create(input, decision);
            int value;
            for (int i = 0; i < input.Length; i++)
            {
                ToMultilabel().LogLikelihoods(input[i], out value, result[i]);
                decision[i] = Vector.OneHot<int>(value, decision[i]);
            }
            return result;
        }

        //double[] IMulticlassGenerativeClassifier<TInput, double[]>.LogLikelihood(TInput[] input, ref double[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        result[i] = ToMulticlass().LogLikelihood(input[i], out value);
        //        Vector.OneHot(value, decision[i]);
        //    }
        //    return result;
        //}



        double[][] IMultilabelLikelihoodClassifier<TInput, double[]>.LogLikelihoods(TInput[] input, ref double[][] decision, double[][] result)
        {
            decision = create(input, decision);
            int value;
            for (int i = 0; i < input.Length; i++)
            {
                ToMultilabel().LogLikelihoods(input[i], out value, result[i]);
                decision[i] = Vector.OneHot<double>(value, decision[i]);
            }
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
        public double[][] LogLikelihoods(TInput[] input, ref bool[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                LogLikelihoods(input[i], out decision[i], result[i]);
            return result;
        }




        double[][] IMultilabelLikelihoodClassifier<TInput, int>.LogLikelihoods(TInput[] input, ref int[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                ToMultilabel().LogLikelihoods(input[i], out decision[i], result[i]);
            return result;
        }


        double[][] IMultilabelLikelihoodClassifier<TInput, double>.LogLikelihoods(TInput[] input, ref double[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                ToMultilabel().LogLikelihoods(input[i], out decision[i], result[i]);
            return result;
        }

        //public double[] LogLikelihood(TInput[] input, ref bool[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    bool value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        result[i] = LogLikelihood(input[i], out value);
        //        Vector.OneHot<bool>(value, decision[i]);
        //    }
        //    return result;
        //}

        //double[] IMulticlassGenerativeClassifier<TInput, int[]>.LogLikelihood(TInput[] input, ref int[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    bool value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        result[i] = LogLikelihood(input[i], out value);
        //        Vector.OneHot<int>(value, decision[i]);
        //    }
        //    return result;
        //}

        #endregion





        #region Probability

        // Input, classIndex
        double IMultilabelLikelihoodClassifier<TInput>.Probability(TInput input, int classIndex)
        {
            return ToMultilabel().Probabilities(input)[classIndex];
        }

        double[] IMultilabelLikelihoodClassifier<TInput>.Probability(TInput[] input, int classIndex)
        {
            return ToMulticlass().Probability(input, classIndex, new double[input.Length]);
        }


        double[] IMultilabelLikelihoodClassifier<TInput>.Probability(TInput[] input, int classIndex, double[] result)
        {
            var temp = new double[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
            {
                ToMultilabel().Probabilities(input[i], result: temp);
                result[i] = temp[classIndex];
            }

            return result;
        }


        // Input, classIndex[]
        double[] IMultilabelLikelihoodClassifier<TInput>.Probability(TInput[] input, int[] classIndex)
        {
            return ToMulticlass().Probability(input, classIndex, new double[input.Length]);
        }

        double[] IMultilabelLikelihoodClassifier<TInput>.Probability(TInput[] input, int[] classIndex, double[] result)
        {
            var temp = new double[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
            {
                ToMultilabel().Probabilities(input[i], result: temp);
                result[i] = temp[classIndex[i]];
            }

            return result;
        }


        // Input

        double[] IMultilabelLikelihoodClassifier<TInput>.Probabilities(TInput input)
        {
            return ToMultilabel().Probabilities(input, new double[NumberOfOutputs]);
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

        double[][] IMultilabelLikelihoodClassifier<TInput>.Probabilities(TInput[] input)
        {
            return ToMultilabel().Probabilities(input, create<double>(input));
        }

        //double IMulticlassRefGenerativeClassifier<TInput, double[]>.Probability(TInput input, ref double[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = ToMulticlass().Probability(input, out value);
        //    Vector.OneHot(value, decision);
        //    return result;
        //}

        //public double Probability(TInput input, ref bool[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = ToMulticlass().Probability(input, out value);
        //    Vector.OneHot(value, decision);
        //    return result;
        //}

        //double IMulticlassRefGenerativeClassifier<TInput, int[]>.Probability(TInput input, ref int[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = ToMulticlass().Probability(input, out value);
        //    Vector.OneHot(value, decision);
        //    return result;
        //}




        // Input, result

        double[] IMultilabelLikelihoodClassifier<TInput>.Probabilities(TInput input, double[] result)
        {
            double d = Probability(input);
            result[0] = d;
            result[1] = 1 - d;
            return result;
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

        double[][] IMultilabelLikelihoodClassifier<TInput>.Probabilities(TInput[] input, double[][] result)
        {
            for (int i = 0; i < input.Length; i++)
            {
                double d = Probability(input[i]);
                result[i][0] = d;
                result[i][1] = 1 - d;
            }
            return result;
        }



        // Input, decision

        double IMulticlassOutLikelihoodClassifier<TInput, double>.Probability(TInput input, out double decision)
        {
            bool value;
            double d = Probability(input, out value);
            decision = Classes.ToZeroOne(value);
            return d;
        }

        double IMulticlassOutLikelihoodClassifier<TInput, int>.Probability(TInput input, out int decision)
        {
            bool value;
            double d = Probability(input, out value);
            decision = Classes.ToZeroOne(value);
            return d;
        }



        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[] Probabilities(TInput input, out bool decision)
        {
            return Probabilities(input, out decision, new double[NumberOfOutputs]);
        }

        double[] IMultilabelOutLikelihoodClassifier<TInput, int>.Probabilities(TInput input, out int decision)
        {
            return ToMultilabel().Probabilities(input, out decision, new double[NumberOfOutputs]);
        }

        double[] IMultilabelOutLikelihoodClassifier<TInput, double>.Probabilities(TInput input, out double decision)
        {
            return ToMultilabel().Probabilities(input, out decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        public double[] Probabilities(TInput input, ref bool[] decision)
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

        //double[] IMulticlassGenerativeClassifier<TInput, double[]>.Probability(TInput[] input, ref double[][] decision)
        //{
        //    return ToMulticlass().Probability(input, ref decision, new double[input.Length]);
        //}


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
        public double[] Probabilities(TInput input, out bool decision, double[] result)
        {
            double d = Probability(input, out decision);
            result[0] = d;
            result[1] = 1 - d;
            return result;
        }

        double[] IMultilabelOutLikelihoodClassifier<TInput, double>.Probabilities(TInput input, out double decision, double[] result)
        {
            bool value;
            double d = Probability(input, out value);
            result[0] = +d;
            result[1] = 1 - d;
            decision = Classes.ToZeroOne(value);
            return result;
        }

        double[] IMultilabelOutLikelihoodClassifier<TInput, int>.Probabilities(TInput input, out int decision, double[] result)
        {
            bool value;
            double d = Probability(input, out value);
            result[0] = +d;
            result[1] = 1 - d;
            decision = Classes.ToZeroOne(value);
            return result;
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, bool[]>.Probabilities(TInput input, ref bool[] decision, double[] result)
        {
            bool value;
            double d = Probability(input, out value);
            result[0] = +d;
            result[1] = 1 - d;
            Vector.OneHot(value, decision);
            return result;
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, int[]>.Probabilities(TInput input, ref int[] decision, double[] result)
        {
            bool value;
            double d = Probability(input, out value);
            result[0] = +d;
            result[1] = 1 - d;
            Vector.OneHot(value, decision);
            return result;
        }

        double[] IMultilabelRefLikelihoodClassifier<TInput, double[]>.Probabilities(TInput input, ref double[] decision, double[] result)
        {
            bool value;
            double d = Probability(input, out value);
            result[0] = +d;
            result[1] = 1 - d;
            Vector.OneHot(value, decision);
            return result;
        }


        // Input[], decision[]

        double[] IMulticlassLikelihoodClassifier<TInput, int>.Probability(TInput[] input, ref int[] decision)
        {
            return ToMulticlass().Probability(input, ref decision, new double[input.Length]);
        }

        double[] IMulticlassLikelihoodClassifier<TInput, double>.Probability(TInput[] input, ref double[] decision)
        {
            return ToMulticlass().Probability(input, ref decision, new double[input.Length]);
        }

        /// <summary>
        /// Predicts a class label for each input vector, returning the
        /// probability that each vector belongs to its predicted class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <returns></returns>
        public double[] Probability(TInput[] input, ref bool[] decision)
        {
            return Probability(input, ref decision, new double[input.Length]);
        }

        //double[] IMulticlassGenerativeClassifier<TInput, int[]>.Probability(TInput[] input, ref int[][] decision)
        //{
        //    return ToMulticlass().Probability(input, ref decision, new double[input.Length]);
        //}

        //double[] IMulticlassGenerativeClassifier<TInput, bool[]>.Probability(TInput[] input, ref bool[][] decision)
        //{
        //    return ToMulticlass().Probability(input, ref decision, new double[input.Length]);
        //}







        double[][] IMultilabelLikelihoodClassifier<TInput, int>.Probabilities(TInput[] input, ref int[] decision)
        {
            return ToMultilabel().Probabilities(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelLikelihoodClassifier<TInput, double>.Probabilities(TInput[] input, ref double[] decision)
        {
            return ToMultilabel().Probabilities(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning the
        /// probabilities of the input vector belonging to each possible class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The labels predicted by the classifier.</param>
        public double[][] Probabilities(TInput[] input, ref bool[] decision)
        {
            return Probabilities(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelLikelihoodClassifier<TInput, int[]>.Probabilities(TInput[] input, ref int[][] decision)
        {
            return ToMultilabel().Probabilities(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelLikelihoodClassifier<TInput, bool[]>.Probabilities(TInput[] input, ref bool[][] decision)
        {
            return ToMultilabel().Probabilities(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelLikelihoodClassifier<TInput, double[]>.Probabilities(TInput[] input, ref double[][] decision)
        {
            return ToMultilabel().Probabilities(input, ref decision, create<double>(input));
        }




        // Input[], decision[], result[]        

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
        public double[] Probability(TInput[] input, ref bool[] decision, double[] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = Probability(input[i], out decision[i]);
            return result;
        }

        double[] IMulticlassLikelihoodClassifier<TInput, int>.Probability(TInput[] input, ref int[] decision, double[] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = ToMulticlass().Probability(input[i], out decision[i]);
            return result;
        }

        double[] IMulticlassLikelihoodClassifier<TInput, double>.Probability(TInput[] input, ref double[] decision, double[] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = ToMulticlass().Probability(input[i], out decision[i]);
            return result;
        }

        double[][] IMultilabelLikelihoodClassifier<TInput, bool[]>.Probabilities(TInput[] input, ref bool[][] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                ToMultilabel().Probabilities(input[i], ref decision[i], result[i]);
            return result;
        }

        double[][] IMultilabelLikelihoodClassifier<TInput, int[]>.Probabilities(TInput[] input, ref int[][] decision, double[][] result)
        {
            decision = create(input, decision);
            int value;
            for (int i = 0; i < input.Length; i++)
            {
                ToMultilabel().Probabilities(input[i], out value, result[i]);
                decision[i] = Vector.OneHot<int>(value, decision[i]);
            }
            return result;
        }

        //double[] IMulticlassGenerativeClassifier<TInput, double[]>.Probability(TInput[] input, ref double[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        result[i] = ToMulticlass().Probability(input[i], out value);
        //        Vector.OneHot(value, decision[i]);
        //    }
        //    return result;
        //}



        double[][] IMultilabelLikelihoodClassifier<TInput, double[]>.Probabilities(TInput[] input, ref double[][] decision, double[][] result)
        {
            decision = create(input, decision);
            int value;
            for (int i = 0; i < input.Length; i++)
            {
                ToMultilabel().Probabilities(input[i], out value, result[i]);
                decision[i] = Vector.OneHot<double>(value, decision[i]);
            }
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
        public double[][] Probabilities(TInput[] input, ref bool[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                Probabilities(input[i], out decision[i], result[i]);
            return result;
        }



        double[][] IMultilabelLikelihoodClassifier<TInput, int>.Probabilities(TInput[] input, ref int[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                ToMultilabel().Probabilities(input[i], out decision[i], result[i]);
            return result;
        }

        double[][] IMultilabelLikelihoodClassifier<TInput, double>.Probabilities(TInput[] input, ref double[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                ToMultilabel().Probabilities(input[i], out decision[i], result[i]);
            return result;
        }

        //double[] IMulticlassGenerativeClassifier<TInput, bool[]>.Probability(TInput[] input, ref bool[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    bool value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        result[i] = Probability(input[i], out value);
        //        Vector.OneHot<bool>(value, decision[i]);
        //    }
        //    return result;
        //}

        //double[] IMulticlassGenerativeClassifier<TInput, int[]>.Probability(TInput[] input, ref int[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    bool value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        result[i] = Probability(input[i], out value);
        //        Vector.OneHot<int>(value, decision[i]);
        //    }
        //    return result;
        //}

        #endregion










        // Transform

        double ITransform<TInput, double>.Transform(TInput input)
        {
            return Probability(input);
        }

        /// <summary>
        /// Applies the transformation to a set of input vectors,
        /// producing an associated set of output vectors.
        /// </summary>
        /// <param name="input">The input data to which
        /// the transformation should be applied.</param>
        /// <param name="result">The location to where to store the
        /// result of this transformation.</param>
        /// <returns>
        /// The output generated by applying this
        /// transformation to the given input.
        /// </returns>
        public override double[] Transform(TInput input, double[] result)
        {
            return ToMultilabel().Probabilities(input, result);
        }

        /// <summary>
        /// Applies the transformation to a set of input vectors,
        /// producing an associated set of output vectors.
        /// </summary>
        /// <param name="input">The input data to which
        /// the transformation should be applied.</param>
        /// <param name="result">The location to where to store the
        /// result of this transformation.</param>
        /// <returns>
        /// The output generated by applying this
        /// transformation to the given input.
        /// </returns>
        public override double[][] Transform(TInput[] input, double[][] result)
        {
            return ToMultilabel().Probabilities(input, result);
        }


        /// <summary>
        /// Views this instance as a multi-class generative classifier,
        /// giving access to more advanced methods, such as the prediction
        /// of integer labels.
        /// </summary>
        /// <returns>
        /// This instance seen as an <see cref="IMulticlassLikelihoodClassifier{TInput}" />.
        /// </returns>
        new public IMulticlassLikelihoodClassifier<TInput> ToMulticlass()
        {
            return (IMulticlassLikelihoodClassifier<TInput>)this;
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

    }
}
