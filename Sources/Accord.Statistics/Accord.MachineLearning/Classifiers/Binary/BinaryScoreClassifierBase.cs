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

        // Main, overridable methods


        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and its most strongly
        /// associated class (as predicted by the classifier).
        /// </summary>
        /// <param name="input">The input vector.</param>
        public abstract double Score(TInput input);

        /// <summary>
        /// Predicts a class label for the input vector, returning a
        /// numerical score measuring the strength of association of the
        /// input vector to its most strongly related class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <returns></returns>
        public virtual double Score(TInput input, out bool decision)
        {
            double distance = Score(input);
            decision = Classes.Decide(distance);
            return distance;
        }


        double IMultilabelScoreClassifier<TInput>.Score(TInput input, int classIndex)
        {
            double d = Score(input);
            return classIndex == 0 ? d : -d;
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





        #region Distance

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
            double d = Score(input);
            result[0] = +d;
            result[1] = -d;
            return result;
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the result will be stored, 
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
        /// <param name="result">An array where the result will be stored, 
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public double[][] Scores(TInput[] input, double[][] result)
        {
            for (int i = 0; i < input.Length; i++)
            {
                double d = Score(input[i]);
                result[i][0] = +d;
                result[i][1] = -d;
            }
            return result;
        }



        // Input, decision


        double IMulticlassOutScoreClassifier<TInput, double>.Score(TInput input, out double decision)
        {
            bool value;
            double d = Score(input, out value);
            decision = Classes.ToZeroOne(value);
            return d;
        }


        double IMulticlassOutScoreClassifier<TInput, int>.Score(TInput input, out int decision)
        {
            bool value;
            double d = Score(input, out value);
            decision = Classes.ToZeroOne(value);
            return d;
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
            return Scores(input, out decision, new double[NumberOfOutputs]);
        }


        double[] IMultilabelOutScoreClassifier<TInput, int>.Scores(TInput input, out int decision)
        {
            return ToMultilabel().Scores(input, out decision, new double[NumberOfOutputs]);
        }


        double[] IMultilabelOutScoreClassifier<TInput, double>.Scores(TInput input, out double decision)
        {
            return ToMultilabel().Scores(input, out decision, new double[NumberOfOutputs]);
        }


        double[] IMultilabelRefScoreClassifier<TInput, bool[]>.Scores(TInput input, ref bool[] decision)
        {
            return ToMultilabel().Scores(input, ref decision, new double[NumberOfOutputs]);
        }


        double[] IMultilabelRefScoreClassifier<TInput, int[]>.Scores(TInput input, ref int[] decision)
        {
            return ToMultilabel().Scores(input, ref decision, new double[NumberOfOutputs]);
        }


        double[] IMultilabelRefScoreClassifier<TInput, double[]>.Scores(TInput input, ref double[] decision)
        {
            return ToMultilabel().Scores(input, ref decision, new double[NumberOfOutputs]);
        }


        //double IMulticlassRefDistanceClassifier<TInput, int[]>.Score(TInput input, ref int[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = ToMulticlass().Distance(input, out value);
        //    Vector.OneHot(value, decision);
        //    return result;
        //}


        //double IMulticlassRefDistanceClassifier<TInput, bool[]>.Score(TInput input, ref bool[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = ToMulticlass().Distance(input, out value);
        //    Vector.OneHot(value, decision);
        //    return result;
        //}

        //double IMulticlassRefDistanceClassifier<TInput, double[]>.Score(TInput input, ref double[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = ToMulticlass().Distance(input, out value);
        //    Vector.OneHot(value, decision);
        //    return result;
        //}



        




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
            double d = Score(input, out decision);
            result[0] = +d;
            result[1] = -d;
            return result;
        }

        double[] IMultilabelOutScoreClassifier<TInput, double>.Scores(TInput input, out double decision, double[] result)
        {
            bool value;
            double d = Score(input, out value);
            result[0] = +d;
            result[1] = -d;
            decision = Classes.ToZeroOne(value);
            return result;
        }


        double[] IMultilabelOutScoreClassifier<TInput, int>.Scores(TInput input, out int decision, double[] result)
        {
            bool value;
            double d = Score(input, out value);
            result[0] = +d;
            result[1] = -d;
            decision = Classes.ToZeroOne(value);
            return result;
        }

        double[] IMultilabelRefScoreClassifier<TInput, bool[]>.Scores(TInput input, ref bool[] decision, double[] result)
        {
            bool value;
            double d = Score(input, out value);
            result[0] = +d;
            result[1] = -d;
            Vector.OneHot(value, decision);
            return result;
        }


        double[] IMultilabelRefScoreClassifier<TInput, int[]>.Scores(TInput input, ref int[] decision, double[] result)
        {
            bool value;
            double d = Score(input, out value);
            result[0] = +d;
            result[1] = -d;
            Vector.OneHot(value, decision);
            return result;
        }

        double[] IMultilabelRefScoreClassifier<TInput, double[]>.Scores(TInput input, ref double[] decision, double[] result)
        {
            bool value;
            double d = Score(input, out value);
            result[0] = +d;
            result[1] = -d;
            Vector.OneHot(value, decision);
            return result;
        }

        
        // Input[], decision[]

        double[] IMulticlassScoreClassifier<TInput, int>.Score(TInput[] input, ref int[] decision)
        {
            return ToMulticlass().Score(input, ref decision, new double[input.Length]);
        }

        double[] IMulticlassScoreClassifier<TInput, double>.Score(TInput[] input, ref double[] decision)
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

        //double[] IMulticlassDistanceClassifier<TInput, int[]>.Score(TInput[] input, ref int[][] decision)
        //{
        //    return ToMulticlass().Distance(input, ref decision, new double[input.Length]);
        //}

        //double[] IMulticlassDistanceClassifier<TInput, bool[]>.Score(TInput[] input, ref bool[][] decision)
        //{
        //    return ToMulticlass().Distance(input, ref decision, new double[input.Length]);
        //}

        //double[] IMulticlassDistanceClassifier<TInput, double[]>.Score(TInput[] input, ref double[][] decision)
        //{
        //    return ToMulticlass().Distance(input, ref decision, new double[input.Length]);
        //}






        double[][] IMultilabelScoreClassifier<TInput, int>.Scores(TInput[] input, ref int[] decision)
        {
            return ToMultilabel().Scores(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelScoreClassifier<TInput, double>.Scores(TInput[] input, ref double[] decision)
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


        double[][] IMultilabelScoreClassifier<TInput, int[]>.Scores(TInput[] input, ref int[][] decision)
        {
            return ToMultilabel().Scores(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelScoreClassifier<TInput, bool[]>.Scores(TInput[] input, ref bool[][] decision)
        {
            return ToMultilabel().Scores(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelScoreClassifier<TInput, double[]>.Scores(TInput[] input, ref double[][] decision)
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
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = Score(input[i], out decision[i]);
            return result;
        }

        double[] IMulticlassScoreClassifier<TInput, int>.Score(TInput[] input, ref int[] decision, double[] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = ToMulticlass().Score(input[i], out decision[i]);
            return result;
        }

        double[] IMulticlassScoreClassifier<TInput, double>.Score(TInput[] input, ref double[] decision, double[] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = ToMulticlass().Score(input[i], out decision[i]);
            return result;
        }

        double[][] IMultilabelScoreClassifier<TInput, bool[]>.Scores(TInput[] input, ref bool[][] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                ToMultilabel().Scores(input[i], ref decision[i], result[i]);
            return result;
        }

        double[][] IMultilabelScoreClassifier<TInput, int[]>.Scores(TInput[] input, ref int[][] decision, double[][] result)
        {
            decision = create(input, decision);
            bool value;
            for (int i = 0; i < input.Length; i++)
            {
                Scores(input[i], out value, result[i]);
                decision[i] = Vector.OneHot<int>(value, decision[i]);
            }
            return result;
        }

        //double[] IMulticlassDistanceClassifier<TInput, double[]>.Score(TInput[] input, ref double[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    bool value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        result[i] = Distance(input[i], out value);
        //        Vector.OneHot(value, decision[i]);
        //    }
        //    return result;
        //}



        double[][] IMultilabelScoreClassifier<TInput, double[]>.Scores(TInput[] input, ref double[][] decision, double[][] result)
        {
            decision = create(input, decision);
            bool value;
            for (int i = 0; i < input.Length; i++)
            {
                Scores(input[i], out value, result[i]);
                decision[i] = Vector.OneHot<double>(value, decision[i]);
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
        /// <param name="result">An array where the result will be stored, 
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public double[][] Scores(TInput[] input, ref bool[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                Scores(input[i], out decision[i], result[i]);
            return result;
        }




        double[][] IMultilabelScoreClassifier<TInput, int>.Scores(TInput[] input, ref int[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                ToMultilabel().Scores(input[i], out decision[i], result[i]);
            return result;
        }


        double[][] IMultilabelScoreClassifier<TInput, double>.Scores(TInput[] input, ref double[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                ToMultilabel().Scores(input[i], out decision[i], result[i]);
            return result;
        }

        //double[] IMulticlassDistanceClassifier<TInput, bool[]>.Score(TInput[] input, ref bool[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    bool value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        result[i] = Distance(input[i], out value);
        //        Vector.OneHot<bool>(value, decision[i]);
        //    }
        //    return result;
        //}

        //double[] IMulticlassDistanceClassifier<TInput, int[]>.Score(TInput[] input, ref int[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    bool value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        result[i] = Distance(input[i], out value);
        //        Vector.OneHot<int>(value, decision[i]);
        //    }
        //    return result;
        //}

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

        IClassifier<TInput, int> IMultilabelScoreClassifier<TInput>.ToMulticlass()
        {
            return (IClassifier<TInput, int>)this;
        }
    }
}
