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
    /// Base class for <see cref="IMulticlassScoreClassifier{TInput}">
    /// score-based multi-class classifiers</see>.
    /// </summary>
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    [Serializable]
    public abstract class MulticlassScoreClassifierBase<TInput> :
        MulticlassClassifierBase<TInput>,
        IMulticlassScoreClassifier<TInput>
    {

      

        // Main, overridable methods


        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the result will be stored, 
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public virtual double[] Scores(TInput input, double[] result)
        {
            for (int i = 0; i < NumberOfClasses; i++)
                result[i] = Score(input, i);
            return result;
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
        /// 
        public virtual double[] Scores(TInput input, out int decision, double[] result)
        {
            Scores(input, result);
            decision = result.ArgMax();
            return result;
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// 
        public abstract double Score(TInput input, int classIndex);

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="IClassifier.NumberOfClasses" /> possible classes.</param>
        /// <returns>
        /// A class-label that best described <paramref name="input" /> according
        /// to this classifier.
        /// </returns>
        public override int Decide(TInput input)
        {
            double max = Score(input, classIndex: 0);
            int imax = 0;
            for (int i = 1; i < NumberOfClasses; i++)
            {
                double d = Score(input, classIndex: i);
                if (d > max)
                {
                    max = d;
                    imax = i;
                }
            }

            return imax;
        }




        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// 
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
        /// 
        public double[] Score(TInput[] input, int[] classIndex, double[] result)
        {
            double[] temp = new double[NumberOfClasses];
            for (int i = 0; i < input.Length; i++)
                result[i] = Scores(input[i], temp)[classIndex[i]];
            return result;
        }


        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// 
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
        /// 
        public double[] Score(TInput[] input, int classIndex, double[] result)
        {
            double[] temp = new double[NumberOfClasses];
            for (int i = 0; i < input.Length; i++)
                result[i] = Scores(input[i], temp)[classIndex];
            return result;
        }






        #region Distances

        // Input

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and its most strongly
        /// associated class (as predicted by the classifier).
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// 
        public double Score(TInput input)
        {
            int value;
            var result = Scores(input, out value);
            return result[value];
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and its most strongly
        /// associated class (as predicted by the classifier).
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// 
        public double[] Score(TInput[] input)
        {
            return Score(input, new double[input.Length]);
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and its most strongly
        /// associated class (as predicted by the classifier).
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[] Score(TInput[] input, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = Score(input[i]);
            return result;
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// 
        public double[] Scores(TInput input)
        {
            return Scores(input, new double[NumberOfClasses]);
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// 
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
        /// 
        public virtual double[][] Scores(TInput[] input, double[][] result)
        {
            for (int i = 0; i < input.Length; i++)
                Scores(input[i], result[i]);
            return result;
        }




        // Input, decision

        /// <summary>
        /// Predicts a class label for the input vector, returning a
        /// numerical score measuring the strength of association of the
        /// input vector to its most strongly related class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// 
        public double Score(TInput input, out int decision)
        {
            double[] result = Scores(input, out decision);
            return result[decision];
        }

        /// <summary>
        /// Predicts a class label for the input vector, returning a
        /// numerical score measuring the strength of association of the
        /// input vector to its most strongly related class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// 
        public double Score(TInput input, out double decision)
        {
            int value;
            double[] result = Scores(input, out value);
            decision = value;
            return result[value];
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// 
        public double[] Scores(TInput input, out int decision)
        {
            return Scores(input, out decision, new double[NumberOfClasses]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// 
        public double[] Scores(TInput input, out double decision)
        {
            return Scores(input, out decision, new double[NumberOfClasses]);
        }

        double[] IMultilabelRefScoreClassifier<TInput, bool[]>.Scores(TInput input, ref bool[] decision)
        {
            return ToMultilabel().Scores(input, ref decision, new double[NumberOfClasses]);
        }


        double[] IMultilabelRefScoreClassifier<TInput, int[]>.Scores(TInput input, ref int[] decision)
        {
            return ToMultilabel().Scores(input, ref decision, new double[NumberOfClasses]);
        }


        double[] IMultilabelRefScoreClassifier<TInput, double[]>.Scores(TInput input, ref double[] decision)
        {
            return ToMultilabel().Scores(input, ref decision, new double[NumberOfClasses]);
        }



        // Input, decision, result

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[] Scores(TInput input, out double decision, double[] result)
        {
            int value;
            Scores(input, out value, result);
            decision = value;
            return result;
        }

        double[] IMultilabelRefScoreClassifier<TInput, bool[]>.Scores(TInput input, ref bool[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            Scores(input, out value, result);
            Vector.OneHot<bool>(value, decision);
            return result;
        }

        double[] IMultilabelRefScoreClassifier<TInput, int[]>.Scores(TInput input, ref int[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            Scores(input, out value, result);
            Vector.OneHot<int>(value, decision);
            return result;
        }

        double[] IMultilabelRefScoreClassifier<TInput, double[]>.Scores(TInput input, ref double[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            Scores(input, out value, result);
            Vector.OneHot<double>(value, decision);
            return result;
        }



        // Input[], decision[]

        /// <summary>
        /// Predicts a class label for each input vector, returning a
        /// numerical score measuring the strength of association of the
        /// input vector to the most strongly related class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels predicted for each input
        /// vector, as predicted by the classifier.</param>
        /// 
        public double[] Score(TInput[] input, ref int[] decision)
        {
            return Score(input, ref decision, new double[input.Length]);
        }

        /// <summary>
        /// Predicts a class label for each input vector, returning a
        /// numerical score measuring the strength of association of the
        /// input vector to the most strongly related class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels predicted for each input
        /// vector, as predicted by the classifier.</param>
        /// 
        public double[] Score(TInput[] input, ref double[] decision)
        {
            return Score(input, ref decision, new double[input.Length]);
        }

        //double[] IMulticlassDistanceClassifier<TInput, bool[]>.Score(TInput[] input, ref bool[][] decision)
        //{
        //    return ToMulticlass().Distance(input, ref decision, new double[input.Length]);
        //}

        //double[] IMulticlassDistanceClassifier<TInput, int[]>.Score(TInput[] input, ref int[][] decision)
        //{
        //    return ToMulticlass().Distance(input, ref decision, new double[input.Length]);
        //}

        //double[] IMultilabelDistanceClassifier<TInput, double[]>.Score(TInput[] input, ref double[][] decision)
        //{
        //    return ToMulticlass().Distance(input, ref decision, new double[input.Length]);
        //}

        /// <summary>
        /// Predicts a class label vector for each input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels associated with each input
        /// vector, as predicted by the classifier. If passed as null, the classifier
        /// will create a new array.</param>
        /// 
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
        /// 
        public double[][] Scores(TInput[] input, ref double[] decision)
        {
            return Scores(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelScoreClassifierBase<TInput, int[]>.Scores(TInput[] input, ref int[][] decision)
        {
            return ToMultilabel().Scores(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelScoreClassifierBase<TInput, bool[]>.Scores(TInput[] input, ref bool[][] decision)
        {
            return ToMultilabel().Scores(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelScoreClassifierBase<TInput, double[]>.Scores(TInput[] input, ref double[][] decision)
        {
            return ToMultilabel().Scores(input, ref decision, create<double>(input));
        }




        // Input, decision, result        

        /// <summary>
        /// Predicts a class label for each input vector, returning a
        /// numerical score measuring the strength of association of the
        /// input vector to the most strongly related class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels predicted for each input
        /// vector, as predicted by the classifier.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[] Score(TInput[] input, ref int[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = Score(input[i], out decision[i]);
            return result;
        }

        /// <summary>
        /// Predicts a class label for each input vector, returning a
        /// numerical score measuring the strength of association of the
        /// input vector to the most strongly related class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels predicted for each input
        /// vector, as predicted by the classifier.</param>
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        ///   
        public double[] Score(TInput[] input, ref double[] decision, double[] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = Score(input[i], out decision[i]);
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
        public double[][] Scores(TInput[] input, ref int[] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                Scores(input[i], out decision[i], result[i]);
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
        /// 
        public double[][] Scores(TInput[] input, ref double[] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                Scores(input[i], out decision[i], result[i]);
            return result;
        }

        double[][] IMultilabelScoreClassifierBase<TInput, bool[]>.Scores(TInput[] input, ref bool[][] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                ToMultilabel().Scores(input[i], ref decision[i], result[i]);
            return result;
        }

        double[][] IMultilabelScoreClassifierBase<TInput, int[]>.Scores(TInput[] input, ref int[][] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            for (int i = 0; i < input.Length; i++)
            {
                Scores(input[i], out value, result[i]);
                decision[i] = Vector.OneHot<int>(value, decision[i]);
            }
            return result;
        }

        double[][] IMultilabelScoreClassifierBase<TInput, double[]>.Scores(TInput[] input, ref double[][] decision, double[][] result)
        {
            decision = createOrReuse(input, decision);
            int value;
            for (int i = 0; i < input.Length; i++)
            {
                Scores(input[i], out value, result[i]);
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
        /// Views this instance as a multi-label distance classifier,
        /// giving access to more advanced methods, such as the prediction
        /// of one-hot vectors.
        /// </summary>
        /// <returns>
        /// This instance seen as an <see cref="IMultilabelScoreClassifier{TInput}" />.
        /// </returns>
        new public IMultilabelScoreClassifier<TInput> ToMultilabel()
        {
            return (IMultilabelScoreClassifier<TInput>)this;
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
    }
}
