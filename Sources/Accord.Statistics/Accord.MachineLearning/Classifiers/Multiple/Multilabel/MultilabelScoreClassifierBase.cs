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
    ///   Base class for <see cref="IMultilabelScoreClassifier{TInput}">
    ///   score-based multi-label classifiers</see>.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// 
    [Serializable]
    public abstract class MultilabelScoreClassifierBase<TInput> :
        MultilabelClassifierBase<TInput>,
        IMultilabelScoreClassifier<TInput>,
        IMulticlassScoreClassifier<TInput>
    {


        // Main, overridable methods

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="decision">The class label associated with the input
        /// vector, as predicted by the classifier.</param>
        public abstract double Score(TInput input, int classIndex, out bool decision);

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        public virtual double Score(TInput input, int classIndex)
        {
            bool decision;
            return Score(input, classIndex, out decision);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public virtual double[] Scores(TInput input, ref bool[] decision, double[] result)
        {
            for (int i = 0; i < result.Length; i++)
                result[i] = Score(input, i, out decision[i]);
            return result;
        }


        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        public double[] Score(TInput[] input, int[] classIndex)
        {
            return Score(input, classIndex, new double[input.Length]);
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] Score(TInput[] input, int[] classIndex, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = Score(input[i], classIndex[i]);
            return result;
        }





        // Input[], classIndex

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        public double[] Score(TInput[] input, int classIndex)
        {
            return Score(input, classIndex, new double[input.Length]);
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] Score(TInput[] input, int classIndex, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = Score(input[i], classIndex);
            return result;
        }



        // Input

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        public double[] Scores(TInput input)
        {
            return Scores(input, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        public double[][] Scores(TInput[] input)
        {
            return Scores(input, create<double>(input));
        }


        // Input, result

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] Scores(TInput[] input, double[][] result)
        {
            bool[] decision = null;
            for (int i = 0; i < input.Length; i++)
                Scores(input[i], ref decision, result[i]);
            return result;
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] Scores(TInput input, double[] result)
        {
            bool[] decision = null;
            return Scores(input, ref decision, result);
        }





        // Input, decision

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[] Scores(TInput input, ref bool[] decision)
        {
            return Scores(input, ref decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[] Scores(TInput input, ref int[] decision)
        {
            return Scores(input, ref decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// 
        public double[] Scores(TInput input, ref double[] decision)
        {
            return Scores(input, ref decision, new double[NumberOfOutputs]);
        }


        // Input, decision, result

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///   
        public double[] Scores(TInput input, ref int[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            var mask = new bool[NumberOfOutputs];
            Scores(input, ref mask, result);
            Vector.KHot<int>(mask, decision);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] Scores(TInput input, ref double[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            var mask = new bool[NumberOfOutputs];
            Scores(input, ref mask, result);
            Vector.KHot<double>(mask, decision);
            return result;
        }



        // Input[], decision[]

        /// <summary>
        /// Predicts a class label vector for each input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[][] Scores(TInput[] input, ref int[][] decision)
        {
            return Scores(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[][] Scores(TInput[] input, ref bool[][] decision)
        {
            return Scores(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[][] Scores(TInput[] input, ref double[][] decision)
        {
            return Scores(input, ref decision, create<double>(input));
        }




        // Input, decision, result        

        /// <summary>
        /// Predicts a class label vector for each input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] Scores(TInput[] input, ref bool[][] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                Scores(input[i], ref decision[i], result[i]);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] Scores(TInput[] input, ref int[][] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            var mask = new bool[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
            {
                Scores(input[i], ref mask, result[i]);
                decision[i] = Vector.KHot<int>(mask, decision[i]);
            }
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] Scores(TInput[] input, ref double[][] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            var mask = new bool[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
            {
                Scores(input[i], ref mask, result[i]);
                decision[i] = Vector.KHot<double>(mask, decision[i]);
            }
            return result;
        }




        // Special case for int: because this is distance-based classifier, we can
        // recover the most likely class (among all simultaneously possible classes)

        int IClassifier<TInput, int>.Decide(TInput input)
        {
            int imax = 0;
            double max = Double.NegativeInfinity;
            for (int i = 0; i < NumberOfOutputs; i++)
            {
                double output = Score(input, i);
                if (output > max)
                {
                    max = output;
                    imax = i;
                }
            }

            return imax;
        }

        int[] IClassifier<TInput, int>.Decide(TInput[] input)
        {
            return Decide(input, new int[input.Length]);
        }

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// <returns>
        /// A class-label that best described <paramref name="input" /> according
        /// to this classifier.
        /// </returns>
        public virtual int[] Decide(TInput[] input, int[] result)
        {
            var t = (IClassifier<TInput, int>)this;
            for (int i = 0; i < input.Length; i++)
                result[i] = t.Decide(input[i]);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        public double[] Scores(TInput input, out int decision)
        {
            return Scores(input, out decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] Scores(TInput input, out int decision, double[] result)
        {
            Scores(input, result);
            decision = result.ArgMax();
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[][] Scores(TInput[] input, ref int[] decision)
        {
            return Scores(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] Scores(TInput[] input, ref int[] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                Scores(input[i], out decision[i], result[i]);
            return result;
        }




        double IClassifier<TInput, double>.Decide(TInput input)
        {
            return ((IClassifier<TInput, int>)this).Decide(input);
        }

        double[] IClassifier<TInput, double>.Decide(TInput[] input)
        {
            return Decide(input, new double[input.Length]);
        }

        double[] IClassifier<TInput, double[]>.Decide(TInput input)
        {
            return Decide(input, new double[NumberOfOutputs]);
        }

        double[][] IClassifier<TInput, double[]>.Decide(TInput[] input)
        {
            return Decide(input, create<double>(input));
        }

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// <returns>
        /// A class-label that best described <paramref name="input" /> according
        /// to this classifier.
        /// </returns>
        public double[] Decide(TInput[] input, double[] result)
        {
            var t = (IClassifier<TInput, int>)this;
            for (int i = 0; i < input.Length; i++)
                result[i] = t.Decide(input[i]);
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        public double[] Scores(TInput input, out double decision)
        {
            return Scores(input, out decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[] Scores(TInput input, out double decision, double[] result)
        {
            Scores(input, result);
            decision = result.ArgMax();
            return result;
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        public double[][] Scores(TInput[] input, ref double[] decision)
        {
            return Scores(input, ref decision, create<double>(input));
        }

        /// <summary>
        /// Predicts a class label vector for each input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] Scores(TInput[] input, ref double[] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                Scores(input[i], out decision[i], result[i]);
            return result;
        }




        // Transform

        int ICovariantTransform<TInput, int>.Transform(TInput input)
        {
            var t = (IClassifier<TInput, int>)this;
            return t.Decide(input);
        }

        int[] ICovariantTransform<TInput, int>.Transform(TInput[] input)
        {
            return Transform(input, new int[NumberOfOutputs]);
        }

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <param name="result">A location to store the output, avoiding unnecessary memory allocations.</param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public virtual int[] Transform(TInput[] input, int[] result)
        {
            return Decide(input, result);
        }

        double ICovariantTransform<TInput, double>.Transform(TInput input)
        {
            var t = (IClassifier<TInput, double>)this;
            return t.Decide(input);
        }

        double[] ICovariantTransform<TInput, double>.Transform(TInput[] input)
        {
            return Transform(input, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <param name="result">A location to store the output, avoiding unnecessary memory allocations.</param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public virtual double[] Transform(TInput[] input, double[] result)
        {
            return Decide(input, result);
        }

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
            return Scores(input, result);
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
            return Scores(input, result);
        }


        /// <summary>
        /// Views this instance as a multi-class generative classifier.
        /// </summary>
        /// <returns>
        /// This instance seen as an <see cref="IMulticlassLikelihoodClassifier{TInput}" />.
        /// </returns>
        public IMulticlassScoreClassifier<TInput> ToMulticlass()
        {
            return (IMulticlassScoreClassifier<TInput>)this;
        }

        /// <summary>
        /// Views this instance as a multi-class generative classifier.
        /// </summary>
        /// <returns>
        /// This instance seen as an <see cref="IMulticlassLikelihoodClassifier{TInput}" />.
        /// </returns>
        public IMulticlassScoreClassifier<TInput, T> ToMulticlass<T>()
        {
            return (IMulticlassScoreClassifier<TInput, T>)this;
        }




        int IMulticlassScoreClassifier<TInput>.Decide(TInput input)
        {
            return ((IClassifier<TInput, int>)this).Decide(input);
        }

        int[] IMulticlassScoreClassifier<TInput>.Decide(TInput[] input)
        {
            return ((IClassifier<TInput, int>)this).Decide(input);
        }

        double IMulticlassScoreClassifier<TInput>.Score(TInput input)
        {
            int decision;
            return Scores(input, out decision)[decision];
        }

        double[] IMulticlassScoreClassifier<TInput>.Score(TInput[] input)
        {
            return ((IMulticlassScoreClassifier<TInput>)this).Score(input, new double[input.Length]);
        }

        double[] IMulticlassScoreClassifier<TInput>.Score(TInput[] input, double[] result)
        {
            int d;
            for (int i = 0; i < input.Length; i++)
                result[i] = Scores(input[i], out d)[d];
            return result;
        }

         IMultilabelScoreClassifier<TInput> IMulticlassScoreClassifier<TInput>.ToMultilabel()
        {
            return this;
        }

        double IMulticlassOutScoreClassifier<TInput, int>.Score(TInput input, out int decision)
        {
            return Scores(input, out decision)[decision];
        }

        double[] IMulticlassScoreClassifierBase<TInput, int>.Score(TInput[] input, ref int[] decision)
        {
            return ((IMulticlassScoreClassifier<TInput>)this).Score(input, ref decision, new double[input.Length]);
        }

        double[] IMulticlassScoreClassifierBase<TInput, int>.Score(TInput[] input, ref int[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);

            int d;
            for (int i = 0; i < input.Length; i++)
            {
                result[i] = Scores(input[i], out d)[d];
                decision[i] = d;
            }
            return result;
        }

        double IMulticlassOutScoreClassifier<TInput, double>.Score(TInput input, out double decision)
        {
            int d;
            double result = Scores(input, out d)[d];
            decision = d;
            return result;
        }

        double[] IMulticlassScoreClassifierBase<TInput, double>.Score(TInput[] input, ref double[] decision)
        {
            return ((IMulticlassScoreClassifier<TInput>)this).Score(input, ref decision);
        }

        double[] IMulticlassScoreClassifierBase<TInput, double>.Score(TInput[] input, ref double[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);

            int d;
            for (int i = 0; i < input.Length; i++)
            {
                result[i] = Scores(input[i], out d)[d];
                decision[i] = d;
            }
            return result;
        }

        int IMulticlassClassifier<TInput>.Decide(TInput input)
        {
            return ((IClassifier<TInput, int>)this).Decide(input);
        }

        int[] IMulticlassClassifier<TInput>.Decide(TInput[] input)
        {
            return ((IClassifier<TInput, int>)this).Decide(input);
        }

        IMultilabelClassifier<TInput> IMulticlassClassifier<TInput>.ToMultilabel()
        {
            return this;
        }
    }
}
