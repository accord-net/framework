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
    using Accord.Statistics;
    using Accord.MachineLearning;
    using System;
    using Accord.Compat;

    /// <summary>
    /// Base class for <see cref="IBinaryScoreClassifier{TInput}">
    /// score-based binary classifiers</see>.
    /// </summary>
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    [Serializable]
    public abstract class BinaryScoreClassifierBase<TInput> :
        BinaryClassifierBase<TInput>,
        IBinaryScoreClassifier<TInput>
    {

        internal const double SCORE_DECISION_THRESHOLD = 0;
        internal const int CLASS_POSITIVE = 1;
        internal const int CLASS_NEGATIVE = 0;

        // Main, overridable methods

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the result will be stored, 
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public abstract double[] Score(TInput[] input, double[] result);


        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="P:Accord.MachineLearning.ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <param name="result">The location where to store the class-labels.</param>
        /// <returns>A class-label that best described <paramref name="input" /> according
        /// to this classifier.</returns>
        public override bool[] Decide(TInput[] input, bool[] result)
        {
            Score(input, ref result, result: new double[input.Length]);
            return result;
        }

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="P:Accord.MachineLearning.ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <returns>A class-label that best described <paramref name="input" /> according
        /// to this classifier.</returns>
        public override bool Decide(TInput input)
        {
            return Decide(new[] { input }, result: new bool[1])[0];
        }

        

        // with class index

        double IMultilabelScoreClassifier<TInput>.Score(TInput input, int classIndex)
        {
            double d = Score(input);
            return classIndex == 0 ? -d : +d;
        }


        double[] IMultilabelScoreClassifier<TInput>.Score(TInput[] input, int[] classIndex)
        {
            return ToMultilabel().Score(input, classIndex, new double[input.Length]);
        }


        double[] IMultilabelScoreClassifier<TInput>.Score(TInput[] input, int classIndex)
        {
            return ToMultilabel().Score(input, classIndex, new double[input.Length]);
        }


        double[] IMultilabelScoreClassifier<TInput>.Score(TInput[] input, int[] classIndex, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = Scores(input[i])[classIndex[i]];
            return result;
        }


        double[] IMultilabelScoreClassifier<TInput>.Score(TInput[] input, int classIndex, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = Scores(input[i])[classIndex];
            return result;
        }





        #region Score

        // Input

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and its most strongly
        /// associated class (as predicted by the classifier).
        /// </summary>
        /// <param name="input">The input vector.</param>
        public double Score(TInput input)
        {
            return Score(new TInput[] { input })[0];
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        public double[] Scores(TInput input)
        {
            return Scores(input, new double[NumberOfClasses]);
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and its most strongly
        /// associated class (as predicted by the classifier).
        /// </summary>
        /// <param name="input">The input vector.</param>
        public double[] Score(TInput[] input)
        {
            return Score(input, new double[input.Length]);
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
        /// <param name="result">An array where the result will be stored, 
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public double[] Scores(TInput input, double[] result)
        {
            return Scores(new[] { input }, new[] { result })[0];
        }



        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the result will be stored, 
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public double[][] Scores(TInput[] input, double[][] result)
        {
            TInput[] p = new TInput[1];
            double[] o = new double[1];

            for (int i = 0; i < input.Length; i++)
            {
                p[0] = input[i];
                double d = Score(p, result: o)[0];
                result[i][CLASS_NEGATIVE] = -d;
                result[i][CLASS_POSITIVE] = +d;
            }
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
        /// <returns></returns>
        public double Score(TInput input, out bool decision)
        {
            bool[] d = new bool[1];
            double s = Score(new TInput[] { input }, ref d)[0];
            decision = d[0];
            return s;
        }

        double IMulticlassOutScoreClassifier<TInput, double>.Score(TInput input, out double decision)
        {
            bool[] d = new bool[1];
            double s = Score(new TInput[] { input }, ref d)[0];
            decision = Classes.ToZeroOne(d[0]);
            return s;
        }


        double IMulticlassOutScoreClassifier<TInput, int>.Score(TInput input, out int decision)
        {
            bool[] d = new bool[1];
            double s = Score(new TInput[] { input }, ref d)[0];
            decision = Classes.ToZeroOne(d[0]);
            return s;
        }



        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        public double[] Scores(TInput input, out bool decision)
        {
            return Scores(input, out decision, new double[NumberOfClasses]);
        }


        double[] IMultilabelOutScoreClassifier<TInput, int>.Scores(TInput input, out int decision)
        {
            return ToMultilabel().Scores(input, out decision, new double[NumberOfClasses]);
        }


        double[] IMultilabelOutScoreClassifier<TInput, double>.Scores(TInput input, out double decision)
        {
            return ToMultilabel().Scores(input, out decision, new double[NumberOfClasses]);
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
        /// <param name="result">An array where the result will be stored, 
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public double[] Scores(TInput input, out bool decision, double[] result)
        {
            bool[] d = new bool[1];
            double[][] r = new[] { result };
            double[] s = Scores(new TInput[] { input }, ref d, r)[0];
            decision = d[0];
            return s;
        }

        double[] IMultilabelOutScoreClassifier<TInput, double>.Scores(TInput input, out double decision, double[] result)
        {
            double[] d = new double[1];
            double[][] r = new[] { result };
            double[] s = Scores(new TInput[] { input }, ref d, r)[0];
            decision = d[0];
            return s;
        }


        double[] IMultilabelOutScoreClassifier<TInput, int>.Scores(TInput input, out int decision, double[] result)
        {
            int[] d = new int[1];
            double[][] r = new[] { result };
            double[] s = Scores(new TInput[] { input }, ref d, r)[0];
            decision = d[0];
            return s;
        }

        double[] IMultilabelRefScoreClassifier<TInput, bool[]>.Scores(TInput input, ref bool[] decision, double[] result)
        {
            bool[] d = new bool[1];
            double[][] r = new[] { result };
            double[] s = Scores(new TInput[] { input }, ref d, r)[0];
            Vector.OneHot<bool>(d[0], decision);
            return s;
        }

        double[] IMultilabelRefScoreClassifier<TInput, int[]>.Scores(TInput input, ref int[] decision, double[] result)
        {
            bool[] d = new bool[1];
            double[][] r = new[] { result };
            double[] s = Scores(new TInput[] { input }, ref d, r)[0];
            Vector.OneHot<int>(d[0], decision);
            return s;
        }

        double[] IMultilabelRefScoreClassifier<TInput, double[]>.Scores(TInput input, ref double[] decision, double[] result)
        {
            bool[] d = new bool[1];
            double[][] r = new[] { result };
            double[] s = Scores(new TInput[] { input }, ref d, r)[0];
            Vector.OneHot<double>(d[0], decision);
            return s;
        }


        // Input[], decision[]

        double[] IMulticlassScoreClassifierBase<TInput, int>.Score(TInput[] input, ref int[] decision)
        {
            return ToMulticlass().Score(input, ref decision, new double[input.Length]);
        }

        double[] IMulticlassScoreClassifierBase<TInput, double>.Score(TInput[] input, ref double[] decision)
        {
            return ToMulticlass().Score(input, ref decision, new double[input.Length]);
        }

        /// <summary>
        /// Predicts a class label for each input vector, returning a
        /// numerical score measuring the strength of association of the
        /// input vector to the most strongly related class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels predicted for each input
        /// vector, as predicted by the classifier.</param>
        public double[] Score(TInput[] input, ref bool[] decision)
        {
            return Score(input, ref decision, new double[input.Length]);
        }








        double[][] IMultilabelScoreClassifierBase<TInput, int>.Scores(TInput[] input, ref int[] decision)
        {
            return ToMultilabel().Scores(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelScoreClassifierBase<TInput, double>.Scores(TInput[] input, ref double[] decision)
        {
            return ToMultilabel().Scores(input, ref decision, create<double>(input));
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
        public double[][] Scores(TInput[] input, ref bool[] decision)
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




        // Input[], decision[], result[]        

        /// <summary>
        /// Predicts a class label for each input vector, returning a
        /// numerical score measuring the strength of association of the
        /// input vector to the most strongly related class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels predicted for each input
        /// vector, as predicted by the classifier.</param>
        /// <param name="result">An array where the result will be stored, 
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public double[] Score(TInput[] input, ref bool[] decision, double[] result)
        {
            Score(input, result);
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                decision[i] = Classes.Decide(result[i] - SCORE_DECISION_THRESHOLD);
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
        /// <param name="result">An array where the distances will be stored,
        /// avoiding unnecessary memory allocations.</param>
        /// <returns>System.Double[].</returns>
        public double[] Score(TInput[] input, ref int[] decision, double[] result)
        {
            Score(input, result);
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                decision[i] = Classes.ToZeroOne(result[i] - SCORE_DECISION_THRESHOLD);
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
        /// <param name="result">An array where the distances will be stored,
        /// avoiding unnecessary memory allocations.</param>
        /// <returns>System.Double[].</returns>
        public double[] Score(TInput[] input, ref double[] decision, double[] result)
        {
            Score(input, result);
            decision = createOrReuse(input, decision);
            for (int i = 0; i < input.Length; i++)
                decision[i] = Classes.ToZeroOne(result[i] - SCORE_DECISION_THRESHOLD);
            return result;
        }

        double[][] IMultilabelScoreClassifierBase<TInput, bool[]>.Scores(TInput[] input, ref bool[][] decision, double[][] result)
        {
            ToMultilabel().Scores(input, result);
            decision = createOrReuse(input, decision);
            for (int i = 0; i < result.Length; i++)
                Vector.OneHot<bool>(Classes.ToZeroOne(result[i][CLASS_POSITIVE] - SCORE_DECISION_THRESHOLD), result: decision[i]);
            return result;
        }

        double[][] IMultilabelScoreClassifierBase<TInput, int[]>.Scores(TInput[] input, ref int[][] decision, double[][] result)
        {
            ToMultilabel().Scores(input, result);
            decision = createOrReuse(input, decision);
            for (int i = 0; i < result.Length; i++)
                Vector.OneHot<int>(Classes.ToZeroOne(result[i][CLASS_POSITIVE] - SCORE_DECISION_THRESHOLD), result: decision[i]);
            return result;
        }

        double[][] IMultilabelScoreClassifierBase<TInput, double[]>.Scores(TInput[] input, ref double[][] decision, double[][] result)
        {
            ToMultilabel().Scores(input, result);
            decision = createOrReuse(input, decision);
            for (int i = 0; i < result.Length; i++)
                Vector.OneHot<double>(Classes.ToZeroOne(result[i][CLASS_POSITIVE] - SCORE_DECISION_THRESHOLD), result: decision[i]);
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
        /// <param name="result">An array where the result will be stored, 
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public double[][] Scores(TInput[] input, ref bool[] decision, double[][] result)
        {
            ToMultilabel().Scores(input, result);
            decision = createOrReuse(input, decision);
            for (int i = 0; i < result.Length; i++)
                decision[i] = Classes.Decide(result[i][CLASS_POSITIVE] - SCORE_DECISION_THRESHOLD);
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
        /// <param name="result">An array where the distances will be stored,
        /// avoiding unnecessary memory allocations.</param>
        /// <returns>System.Double[][].</returns>
        public double[][] Scores(TInput[] input, ref int[] decision, double[][] result)
        {
            ToMultilabel().Scores(input, result);
            decision = createOrReuse(input, decision);
            for (int i = 0; i < result.Length; i++)
                decision[i] = Classes.ToZeroOne(result[i][CLASS_POSITIVE] - SCORE_DECISION_THRESHOLD);
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
        /// <param name="result">An array where the distances will be stored,
        /// avoiding unnecessary memory allocations.</param>
        /// <returns>System.Double[][].</returns>
        public double[][] Scores(TInput[] input, ref double[] decision, double[][] result)
        {
            ToMultilabel().Scores(input, result);
            decision = createOrReuse(input, decision);
            for (int i = 0; i < result.Length; i++)
                decision[i] = Classes.ToZeroOne(result[i][CLASS_POSITIVE] - SCORE_DECISION_THRESHOLD);
            return result;
        }

        #endregion





        // Transform

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
            return Scores(input, result);
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
            return Scores(input, result);
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
        public override double[] Transform(TInput[] input, double[] result)
        {
            return Score(input, result);
        }


        int IMulticlassScoreClassifier<TInput>.Decide(TInput input)
        {
            return ((IClassifier<TInput, int>)this).Decide(input);
        }

        int[] IMulticlassScoreClassifier<TInput>.Decide(TInput[] input)
        {
            return ((IClassifier<TInput, int>)this).Decide(input);
        }


        /// <summary>
        /// Views this instance as a multi-class distance classifier,
        /// giving access to more advanced methods, such as the prediction
        /// of integer labels.
        /// </summary>
        /// <returns>
        /// This instance seen as an <see cref="IMulticlassScoreClassifier{TInput}" />.
        /// </returns>
        new public IMulticlassScoreClassifier<TInput> ToMulticlass()
        {
            return (IMulticlassScoreClassifier<TInput>)this;
        }

        /// <summary>
        /// Views this instance as a multi-class distance classifier,
        /// giving access to more advanced methods, such as the prediction
        /// of integer labels.
        /// </summary>
        /// <returns>
        /// This instance seen as an <see cref="IMulticlassScoreClassifier{TInput}" />.
        /// </returns>
        new public IMulticlassScoreClassifier<TInput, T> ToMulticlass<T>()
        {
            return (IMulticlassScoreClassifier<TInput, T>)this;
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

    }
}
