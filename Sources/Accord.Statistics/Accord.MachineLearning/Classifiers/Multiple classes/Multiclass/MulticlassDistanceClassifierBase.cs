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
    using System;

    /// <summary>
    /// Base class for <see cref="IMulticlassDistanceClassifier{TInput}">
    /// score-based multi-class classifiers</see>.
    /// </summary>
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    [Serializable]
    public abstract class MulticlassDistanceClassifierBase<TInput> :
        MulticlassClassifierBase<TInput>,
        IMulticlassDistanceClassifier<TInput>
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
        public virtual double[] Distances(TInput input, double[] result)
        {
            for (int i = 0; i < NumberOfOutputs; i++)
                result[i] = Distance(input, i);
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
        public virtual double[] Distances(TInput input, out int decision, double[] result)
        {
            Distances(input, result);
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
        public virtual double Distance(TInput input, int classIndex)
        {
            return Distances(input)[classIndex];
        }

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <returns>
        /// A class-label that best described <paramref name="input" /> according
        /// to this classifier.
        /// </returns>
        public override int Decide(TInput input)
        {
            double min = Double.PositiveInfinity;
            int imin = 0;
            for (int i = 0; i < NumberOfOutputs; i++)
            {
                double d = Distance(input, classIndex: i);
                if (d < min)
                {
                    min = d;
                    imin = i;
                }
            }

            return imin;
        }




        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and a given
        /// <paramref name="classIndex" />.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="classIndex">The index of the class whose score will be computed.</param>
        /// 
        public double[] Distance(TInput[] input, int[] classIndex)
        {
            return Distance(input, classIndex, new double[input.Length]);
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
        public double[] Distance(TInput[] input, int[] classIndex, double[] result)
        {
            double[] temp = new double[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
                result[i] = Distances(input[i], temp)[classIndex[i]];
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
        public double[] Distance(TInput[] input, int classIndex)
        {
            return Distance(input, classIndex, new double[input.Length]);
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
        public double[] Distance(TInput[] input, int classIndex, double[] result)
        {
            double[] temp = new double[NumberOfOutputs];
            for (int i = 0; i < input.Length; i++)
                result[i] = Distances(input[i], temp)[classIndex];
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
        public double Distance(TInput input)
        {
            int value;
            var result = Distances(input, out value);
            return result[value];
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and its most strongly
        /// associated class (as predicted by the classifier).
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// 
        public double[] Distance(TInput[] input)
        {
            return Distance(input, new double[input.Length]);
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
        public double[] Distance(TInput[] input, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = Distance(input[i]);
            return result;
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// 
        public double[] Distances(TInput input)
        {
            return Distances(input, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// 
        public double[][] Distances(TInput[] input)
        {
            return Distances(input, create<double>(input));
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
        public virtual double[][] Distances(TInput[] input, double[][] result)
        {
            for (int i = 0; i < input.Length; i++)
                Distances(input[i], result[i]);
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
        public double Distance(TInput input, out int decision)
        {
            double[] result = Distances(input, out decision);
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
        public double Distance(TInput input, out double decision)
        {
            int value;
            double[] result = Distances(input, out value);
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
        public double[] Distances(TInput input, out int decision)
        {
            return Distances(input, out decision, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Predicts a class label vector for the given input vector, returning a
        /// numerical score measuring the strength of association of the input vector
        /// to each of the possible classes.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// 
        public double[] Distances(TInput input, out double decision)
        {
            return Distances(input, out decision, new double[NumberOfOutputs]);
        }

        double[] IMultilabelRefDistanceClassifier<TInput, bool[]>.Distances(TInput input, ref bool[] decision)
        {
            return ToMultilabel().Distances(input, ref decision, new double[NumberOfOutputs]);
        }


        double[] IMultilabelRefDistanceClassifier<TInput, int[]>.Distances(TInput input, ref int[] decision)
        {
            return ToMultilabel().Distances(input, ref decision, new double[NumberOfOutputs]);
        }


        double[] IMultilabelRefDistanceClassifier<TInput, double[]>.Distances(TInput input, ref double[] decision)
        {
            return ToMultilabel().Distances(input, ref decision, new double[NumberOfOutputs]);
        }


        //double IMulticlassRefDistanceClassifier<TInput, int[]>.Distance(TInput input, ref int[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = Distance(input, out value);
        //    Vector.OneHot(value, decision);
        //    return result;
        //}

        //double IMulticlassRefDistanceClassifier<TInput, bool[]>.Distance(TInput input, ref bool[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = Distance(input, out value);
        //    Vector.OneHot(value, decision);
        //    return result;
        //}

        //double IMulticlassRefDistanceClassifier<TInput, double[]>.Distance(TInput input, ref double[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = Distance(input, out value);
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
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        /// 
        public double[] Distances(TInput input, out double decision, double[] result)
        {
            int value;
            Distances(input, out value, result);
            decision = value;
            return result;
        }

        double[] IMultilabelRefDistanceClassifier<TInput, bool[]>.Distances(TInput input, ref bool[] decision, double[] result)
        {
            decision = create(input, decision);
            int value;
            Distances(input, out value, result);
            Vector.OneHot<bool>(value, decision);
            return result;
        }

        double[] IMultilabelRefDistanceClassifier<TInput, int[]>.Distances(TInput input, ref int[] decision, double[] result)
        {
            decision = create(input, decision);
            int value;
            Distances(input, out value, result);
            Vector.OneHot<int>(value, decision);
            return result;
        }

        double[] IMultilabelRefDistanceClassifier<TInput, double[]>.Distances(TInput input, ref double[] decision, double[] result)
        {
            decision = create(input, decision);
            int value;
            Distances(input, out value, result);
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
        public double[] Distance(TInput[] input, ref int[] decision)
        {
            return Distance(input, ref decision, new double[input.Length]);
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
        public double[] Distance(TInput[] input, ref double[] decision)
        {
            return Distance(input, ref decision, new double[input.Length]);
        }

        //double[] IMulticlassDistanceClassifier<TInput, bool[]>.Distance(TInput[] input, ref bool[][] decision)
        //{
        //    return ToMulticlass().Distance(input, ref decision, new double[input.Length]);
        //}

        //double[] IMulticlassDistanceClassifier<TInput, int[]>.Distance(TInput[] input, ref int[][] decision)
        //{
        //    return ToMulticlass().Distance(input, ref decision, new double[input.Length]);
        //}

        //double[] IMultilabelDistanceClassifier<TInput, double[]>.Distance(TInput[] input, ref double[][] decision)
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
        public double[][] Distances(TInput[] input, ref int[] decision)
        {
            return Distances(input, ref decision, create<double>(input));
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
        public double[][] Distances(TInput[] input, ref double[] decision)
        {
            return Distances(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelDistanceClassifier<TInput, int[]>.Distances(TInput[] input, ref int[][] decision)
        {
            return ToMultilabel().Distances(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelDistanceClassifier<TInput, bool[]>.Distances(TInput[] input, ref bool[][] decision)
        {
            return ToMultilabel().Distances(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelDistanceClassifier<TInput, double[]>.Distances(TInput[] input, ref double[][] decision)
        {
            return ToMultilabel().Distances(input, ref decision, create<double>(input));
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
        public double[] Distance(TInput[] input, ref int[] decision, double[] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = Distance(input[i], out decision[i]);
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
        public double[] Distance(TInput[] input, ref double[] decision, double[] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = Distance(input[i], out decision[i]);
            return result;
        }

        //double[] IMulticlassDistanceClassifier<TInput, double[]>.Distance(TInput[] input, ref double[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    var temp = new double[NumberOfOutputs];
        //    int value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        Distances(input[i], out value, temp);
        //        Vector.OneHot(value, decision[i]);
        //        result[i] = temp[value];
        //    }
        //    return result;
        //}

        //double[] IMulticlassDistanceClassifier<TInput, int[]>.Distance(TInput[] input, ref int[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    var temp = new double[NumberOfOutputs];
        //    int value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        Distances(input[i], out value, temp);
        //        Vector.OneHot(value, decision[i]);
        //        result[i] = temp[value];
        //    }
        //    return result;
        //}

        //double[] IMulticlassDistanceClassifier<TInput, bool[]>.Distance(TInput[] input, ref bool[][] decision, double[] result)
        //{
        //    decision = create(input, decision);
        //    var temp = new double[NumberOfOutputs];
        //    int value;
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        Distances(input[i], out value, temp);
        //        Vector.OneHot(value, decision[i]);
        //        result[i] = temp[value];
        //    }
        //    return result;
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
        /// <param name="result">An array where the scores will be stored,
        ///   avoiding unnecessary memory allocations.</param>
        public double[][] Distances(TInput[] input, ref int[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                Distances(input[i], out decision[i], result[i]);
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
        public double[][] Distances(TInput[] input, ref double[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                Distances(input[i], out decision[i], result[i]);
            return result;
        }

        double[][] IMultilabelDistanceClassifier<TInput, bool[]>.Distances(TInput[] input, ref bool[][] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                ToMultilabel().Distances(input[i], ref decision[i], result[i]);
            return result;
        }

        double[][] IMultilabelDistanceClassifier<TInput, int[]>.Distances(TInput[] input, ref int[][] decision, double[][] result)
        {
            decision = create(input, decision);
            int value;
            for (int i = 0; i < input.Length; i++)
            {
                Distances(input[i], out value, result[i]);
                decision[i] = Vector.OneHot<int>(value, decision[i]);
            }
            return result;
        }

        double[][] IMultilabelDistanceClassifier<TInput, double[]>.Distances(TInput[] input, ref double[][] decision, double[][] result)
        {
            decision = create(input, decision);
            int value;
            for (int i = 0; i < input.Length; i++)
            {
                Distances(input[i], out value, result[i]);
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
            return Distances(input, result);
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
            return Distances(input, result);
        }

        /// <summary>
        /// Views this instance as a multi-label distance classifier,
        /// giving access to more advanced methods, such as the prediction
        /// of one-hot vectors.
        /// </summary>
        /// <returns>
        /// This instance seen as an <see cref="IMultilabelDistanceClassifier{TInput}" />.
        /// </returns>
        new public IMultilabelDistanceClassifier<TInput> ToMultilabel()
        {
            return (IMultilabelDistanceClassifier<TInput>)this;
        }
    }
}
