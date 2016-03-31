// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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

    /// <summary>
    ///   Base class for generative multi-class classifiers.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// 
    [Serializable]
    public abstract class GenerativeClassifierBase<TInput> : ClassifierBase<TInput, double>, 
        IGenerativeClassifier<TInput, double>, ITransform<TInput, int>
    {

        /// <summary>
        ///   Computes the log-likelihood for one specific the class label
        ///   for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The input vector that should be classified into
        ///   any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs"/> possible classes.</param>
        /// <param name="classIndex">The index for the class whose log-likelihood should be computed.</param>
        /// 
        /// <returns>The log-likelihood for the specified <paramref name="classIndex">class label</paramref>.</returns>
        /// 
        public abstract double LogLikelihood(TInput input, int classIndex);



        /// <summary>
        ///   Computes a class-label decision for a given <paramref name="input"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vector that should be classified into
        ///   one of the <see cref="ITransform{TInput, TScores}.NumberOfOutputs"/> possible classes.</param>
        /// 
        /// <returns>A class-label that best described <paramref name="input"/> according
        /// to this classifier.</returns>
        /// 
        public override int Decide(TInput input)
        {
            int imax = 0;
            double max = Double.NegativeInfinity;
            for (int i = 0; i < NumberOfOutputs; i++)
            {
                double output = LogLikelihood(input, i);
                if (output > max)
                {
                    max = output;
                    imax = i;
                }
            }

            return imax;
        }

        /// <summary>
        ///   Computes the individual probabilities for each of the class
        ///   labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The input vector that should be classified into
        ///   any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs"/> possible classes.</param>
        /// 
        /// <returns>The probabilities of each possible class label.</returns>
        /// 
        public virtual double[] Probabilities(TInput input)
        {
            return Probabilities(input, new double[NumberOfOutputs]);
        }

        /// <summary>
        ///   Computes the individual probabilities for each of the class
        ///   labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The input vector that should be classified into
        ///   any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs"/> possible classes.</param>
        /// <param name="result">The location to store the class-specific probabilities.</param>
        ///
        /// <returns>The probabilities of each possible class label.</returns>
        /// 
        public virtual double[] Probabilities(TInput input, double[] result)
        {
            LogLikelihood(input, result);
            return Special.Softmax(result, result);
        }

        /// <summary>
        ///   Computes the individual probabilities for each of the class
        ///   labels for a given set of <paramref name="input">input vectors</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The set of input vectors that should be classified into
        ///   any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs"/> possible
        ///   classes each.</param>
        /// 
        /// <returns>The probabilities of each possible class label for each input in the set.</returns>
        /// 
        public virtual double[][] Probabilities(TInput[] input)
        {
            return Probabilities(input, Jagged.Create<double>(input.Length, NumberOfOutputs));
        }

        /// <summary>
        ///   Computes the individual probabilities for each of the class
        ///   labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The input vector that should be classified into
        ///   any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs"/> possible classes.</param>
        /// <param name="result">The location to store the class-specific probabilities.</param>
        /// 
        /// <returns>The probabilities of each possible class label.</returns>
        /// 
        public virtual double[][] Probabilities(TInput[] input, double[][] result)
        {
            for (int i = 0; i < input.Length; i++)
                Probabilities(input[i], result: result[i]);
            return result;
        }

        /// <summary>
        ///   Computes the probability for one specific the class label
        ///   for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The input vector that should be classified into
        ///   any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs"/> possible classes.</param>
        /// <param name="classIndex">The index for the class whose probabilities should be computed.</param>
        /// 
        /// <returns>The probabilities for the specified <paramref name="classIndex">class label</paramref>.</returns>
        /// 
        public virtual double Probability(TInput input, int classIndex)
        {
            return Probabilities(input)[classIndex];
        }

        /// <summary>
        ///   Computes the probability for one specific the class label
        ///   for the given <paramref name="input">input vectors</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The input vectors that should be classified into
        ///   any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs"/> possible classes.</param>
        /// <param name="classIndex">The index for the class whose probabilities should be computed.</param>
        /// 
        /// <returns>The probabilities for the specified <paramref name="classIndex">class label</paramref>.</returns>
        /// 
        public virtual double[] Probability(TInput[] input, int classIndex)
        {
            return Probabilities(input).GetColumn(classIndex);
        }

        /// <summary>
        ///   Computes the probability for one specific the class label
        ///   for the given <paramref name="input">input vectors</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The input vectors that should be classified into
        ///   any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs"/> possible classes.</param>
        /// <param name="classIndex">The index for the class whose probabilities should be computed.</param>
        /// <param name="result">The location to store the class-specific probabilities.</param>
        /// 
        /// <returns>The probabilities for the specified <paramref name="classIndex">class label</paramref>.</returns>
        /// 
        public virtual double[] Probability(TInput[] input, int classIndex, double[] result)
        {
            return Probabilities(input).GetColumn(classIndex, result: result);
        }



        /// <summary>
        ///   Computes the individual log-likelihood for each of the class
        ///   labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The input vector that should be classified into
        ///   any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs"/> possible classes.</param>
        /// 
        /// <returns>The probabilities of each possible class label.</returns>
        /// 
        public virtual double[] LogLikelihood(TInput input)
        {
            return LogLikelihood(input, new double[NumberOfOutputs]);
        }

        /// <summary>
        ///   Computes the individual log-likelihood for each of the class
        ///   labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The input vector that should be classified into
        ///   any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs"/> possible classes.</param>
        /// <param name="result">The location to store the class-specific log-likelihood.</param>
        /// 
        /// <returns>The probabilities of each possible class label.</returns>
        /// 
        public virtual double[] LogLikelihood(TInput input, double[] result)
        {
            for (int i = 0; i < result.Length; i++)
                result[i] = LogLikelihood(input, i);
            return result;
        }

        /// <summary>
        ///   Computes the individual log-likelihood for each of the class
        ///   labels for a set of <paramref name="input">input vectors</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The input vectors that should be classified into
        ///   any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs"/> possible classes.</param>
        /// 
        /// <returns>The probabilities of each possible class label, for each vector.</returns>
        /// 
        public virtual double[][] LogLikelihood(TInput[] input)
        {
            return LogLikelihood(input, Jagged.Create<double>(input.Length, NumberOfOutputs));
        }

        /// <summary>
        ///   Computes the individual log-likelihood for each of the class
        ///   labels for a set of <paramref name="input">input vectors</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The input vectors that should be classified into
        ///   any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs"/> possible classes.</param>
        /// <param name="result">The location to store the class-specific log-likelihood
        ///   for each vector.</param>
        /// 
        /// <returns>The probabilities of each possible class label, for each vector.</returns>
        /// 
        public virtual double[][] LogLikelihood(TInput[] input, double[][] result)
        {
            for (int i = 0; i < input.Length; i++)
                LogLikelihood(input[i], result: result[i]);
            return result;
        }

        /// <summary>
        ///   Computes the log-likelihood for one specific the class label
        ///   for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The input vector that should be classified into
        ///   any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs"/> possible classes.</param>
        /// <param name="classIndex">The index for the class whose log-likelihood should be computed.</param>
        /// 
        /// <returns>The log-likelihood for the specified <paramref name="classIndex">class label</paramref>.</returns>
        /// 

        public virtual double[] LogLikelihood(TInput[] input, int classIndex)
        {
            return LogLikelihood(input, classIndex, new double[input.Length]);
        }

        /// <summary>
        ///   Computes the log-likelihood for one specific the class label
        ///   for the given <paramref name="input">input vectors</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The input vectors that should be classified into
        ///   any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs"/> possible classes.</param>
        /// <param name="classIndex">The index for the class whose probabilities should be computed.</param>
        /// <param name="result">The location to store the class-specific log-likelihood.</param>
        /// 
        /// <returns>The log-likelihoods for the specified <paramref name="classIndex">class label</paramref>.</returns>
        /// 
        public virtual double[] LogLikelihood(TInput[] input, int classIndex, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = LogLikelihood(input[i], classIndex);
            return result;
        }



        /// <summary>
        ///   Applies the transformation to a set of input vectors,
        ///   producing an associated set of output vectors.
        /// </summary>
        /// 
        /// <param name="input">The input data to which
        ///   the transformation should be applied.</param>
        /// 
        /// <returns>The output generated by applying this
        ///   transformation to the given input.</returns>
        /// 
        public override double[] Transform(TInput input)
        {
            return LogLikelihood(input);
        }

        /// <summary>
        ///   Applies the transformation to a set of input vectors,
        ///   producing an associated set of output vectors.
        /// </summary>
        /// 
        /// <param name="input">The input data to which
        ///   the transformation should be applied.</param>
        /// <param name="result">The location to where to store the
        ///   result of this transformation.</param>
        /// 
        /// <returns>The output generated by applying this
        ///   transformation to the given input.</returns>
        /// 
        public override double[] Transform(TInput input, double[] result)
        {
            return LogLikelihood(input, result);
        }

        /// <summary>
        ///   Applies the transformation to a set of input vectors,
        ///   producing an associated set of output vectors.
        /// </summary>
        /// 
        /// <param name="input">The input data to which
        ///   the transformation should be applied.</param>
        /// 
        /// <returns>The output generated by applying this
        ///   transformation to the given input.</returns>
        /// 
        public override double[][] Transform(TInput[] input)
        {
            return LogLikelihood(input);
        }

        /// <summary>
        ///   Applies the transformation to a set of input vectors,
        ///   producing an associated set of output vectors.
        /// </summary>
        /// 
        /// <param name="input">The input data to which
        ///   the transformation should be applied.</param>
        /// <param name="result">The location to where to store the
        ///   result of this transformation.</param>
        /// 
        /// <returns>The output generated by applying this
        ///   transformation to the given input.</returns>
        /// 
        public override double[][] Transform(TInput[] input, double[][] result)
        {
            return LogLikelihood(input, result);
        }

        /// <summary>
        ///   Applies the transformation to a set of input vectors,
        ///   producing an associated set of output vectors.
        /// </summary>
        /// 
        /// <param name="input">The input data to which
        ///   the transformation should be applied.</param>
        /// <param name="result">The location to where to store the
        ///   result of this transformation.</param>
        /// 
        /// <returns>The output generated by applying this
        ///   transformation to the given input.</returns>
        /// 
        public int[] Transform(TInput[] input, int[] result)
        {
            return Decide(input, result);
        }

        int ITransform<TInput, int>.Transform(TInput input)
        {
            return Decide(input);
        }

        int[] ITransform<TInput, int>.Transform(TInput[] input)
        {
            return Decide(input);
        }
    }

    /// <summary>
    ///   Base class for generative multi-class classifiers.
    /// </summary>
    /// 
    [Serializable]
    public abstract class GenerativeClassifierBase : GenerativeClassifierBase<double[]>,
        IGenerativeClassifier<int[]>, ITransform<int[], int>
    {


        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public virtual double[] Transform(int[] input)
        {
            return Transform(input.ToDouble());
        }

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public virtual double[][] Transform(int[][] input)
        {
            return Transform(input.ToDouble());
        }

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <param name="result"></param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public virtual double[][] Transform(int[][] input, double[][] result)
        {
            return Transform(input.ToDouble(), result);
        }

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="ITransform{TInput, TScores}.NumberOfOutputs" /> possible classes.</param>
        /// <returns>
        /// A class-label that best described <paramref name="input" /> according
        /// to this classifier.
        /// </returns>
        public int Decide(int[] input)
        {
            return Decide(input.ToDouble());
        }

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="ITransform{TInput, TScores}.NumberOfOutputs" /> possible classes.</param>
        /// <returns>
        /// A class-label that best described <paramref name="input" /> according
        /// to this classifier.
        /// </returns>
        public virtual int[] Decide(int[][] input)
        {
            return Decide(input.ToDouble());
        }

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="ITransform{TInput, TScores}.NumberOfOutputs" /> possible classes.</param>
        /// <param name="result"></param>
        /// <returns>
        /// A class-label that best described <paramref name="input" /> according
        /// to this classifier.
        /// </returns>
        public virtual int[] Decide(int[][] input, int[] result)
        {
            return Decide(input.ToDouble(), result);
        }

        /// <summary>
        /// Computes the individual probabilities for each of the class
        /// labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs" /> possible classes.</param>
        /// <returns>
        /// The probabilities of each possible class label.
        /// </returns>
        public virtual double[] Probabilities(int[] input)
        {
            return Probabilities(input.ToDouble());
        }

        /// <summary>
        /// Computes the individual probabilities for each of the class
        /// labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs" /> possible classes.</param>
        /// <param name="result"></param>
        /// <returns>
        /// The probabilities of each possible class label.
        /// </returns>
        public virtual double[] Probabilities(int[] input, double[] result)
        {
            return Probabilities(input.ToDouble(), result);
        }

        /// <summary>
        /// Computes the individual probabilities for each of the class
        /// labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs" /> possible classes.</param>
        /// <returns>
        /// The probabilities of each possible class label.
        /// </returns>
        public double[][] Probabilities(int[][] input)
        {
            return Probabilities(input.ToDouble());
        }

        /// <summary>
        /// Computes the individual probabilities for each of the class
        /// labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs" /> possible classes.</param>
        /// <param name="result"></param>
        /// <returns>
        /// The probabilities of each possible class label.
        /// </returns>
        public virtual double[][] Probabilities(int[][] input, double[][] result)
        {
            return Probabilities(input.ToDouble(), result);
        }

        /// <summary>
        /// Computes the probability for one specific the class label
        /// for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs" /> possible classes.</param>
        /// <param name="classIndex">The index for the class whose probabilities should be computed.</param>
        /// <returns>
        /// The probabilities for the specified <paramref name="classIndex">class label</paramref>.
        /// </returns>
        public virtual double Probability(int[] input, int classIndex)
        {
            return Probability(input.ToDouble(), classIndex);
        }

        /// <summary>
        /// Computes the probability for one specific the class label
        /// for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs" /> possible classes.</param>
        /// <param name="classIndex">The index for the class whose probabilities should be computed.</param>
        /// <returns>
        /// The probabilities for the specified <paramref name="classIndex">class label</paramref>.
        /// </returns>
        public virtual double[] Probability(int[][] input, int classIndex)
        {
            return Probability(input.ToDouble(), classIndex);
        }

        /// <summary>
        /// Computes the probability for one specific the class label
        /// for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs" /> possible classes.</param>
        /// <param name="classIndex">The index for the class whose probabilities should be computed.</param>
        /// <param name="result"></param>
        /// <returns>
        /// The probabilities for the specified <paramref name="classIndex">class label</paramref>.
        /// </returns>
        public virtual double[] Probability(int[][] input, int classIndex, double[] result)
        {
            return Probability(input.ToDouble(), classIndex, result);
        }

        /// <summary>
        /// Computes the individual log-likelihood for each of the class
        /// labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs" /> possible classes.</param>
        /// <returns>
        /// The probabilities of each possible class label.
        /// </returns>
        public virtual double[] LogLikelihood(int[] input)
        {
            return LogLikelihood(input.ToDouble());
        }

        /// <summary>
        /// Computes the individual log-likelihood for each of the class
        /// labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs" /> possible classes.</param>
        /// <param name="result"></param>
        /// <returns>
        /// The probabilities of each possible class label.
        /// </returns>
        public virtual double[] LogLikelihood(int[] input, double[] result)
        {
            return LogLikelihood(input.ToDouble(), result);
        }

        /// <summary>
        /// Computes the individual log-likelihood for each of the class
        /// labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs" /> possible classes.</param>
        /// <returns>
        /// The probabilities of each possible class label.
        /// </returns>
        public virtual double[][] LogLikelihood(int[][] input)
        {
            return LogLikelihood(input.ToDouble());
        }

        /// <summary>
        /// Computes the individual log-likelihood for each of the class
        /// labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs" /> possible classes.</param>
        /// <param name="result"></param>
        /// <returns>
        /// The probabilities of each possible class label.
        /// </returns>
        public virtual double[][] LogLikelihood(int[][] input, double[][] result)
        {
            return LogLikelihood(input.ToDouble(), result);
        }

        /// <summary>
        /// Computes the individual log-likelihood for each of the class
        /// labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs" /> possible classes.</param>
        /// <param name="classIndex"></param>
        /// <returns>
        /// The probabilities of each possible class label.
        /// </returns>
        public virtual double LogLikelihood(int[] input, int classIndex)
        {
            return LogLikelihood(input.ToDouble(), classIndex);
        }

        /// <summary>
        /// Computes the individual log-likelihood for each of the class
        /// labels for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs" /> possible classes.</param>
        /// <param name="classIndex"></param>
        /// <param name="result"></param>
        /// <returns>
        /// The probabilities of each possible class label.
        /// </returns>
        public virtual double[] LogLikelihood(int[][] input, int classIndex, double[] result)
        {
            return LogLikelihood(input.ToDouble(), classIndex, result);
        }




        int ITransform<int[], int>.Transform(int[] input)
        {
            return Decide(input);
        }

        int[] ITransform<int[], int>.Transform(int[][] input)
        {
            return Decide(input);
        }

        int[] ITransform<int[], int>.Transform(int[][] input, int[] result)
        {
            return Decide(input, result);
        }
    }
}
