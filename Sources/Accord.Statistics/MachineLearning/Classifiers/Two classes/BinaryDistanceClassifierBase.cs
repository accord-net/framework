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
    using Accord.Statistics;
    using Accord.MachineLearning;
    using System;

    /// <summary>
    /// Base class for <see cref="IBinaryDistanceClassifier{TInput}">
    /// score-based binary classifiers</see>.
    /// </summary>
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    [Serializable]
    public abstract class BinaryDistanceClassifierBase<TInput> :
        BinaryClassifierBase<TInput>,
        IBinaryDistanceClassifier<TInput>
    {

        // Main, overridable methods


        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and its most strongly
        /// associated class (as predicted by the classifier).
        /// </summary>
        /// <param name="input">The input vector.</param>
        public abstract double Distance(TInput input);

        /// <summary>
        /// Predicts a class label for the input vector, returning a
        /// numerical score measuring the strength of association of the
        /// input vector to its most strongly related class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="decision">The class label predicted by the classifier.</param>
        /// <returns></returns>
        public virtual double Distance(TInput input, out bool decision)
        {
            double distance = Distance(input);
            decision = Special.Decide(distance);
            return distance;
        }


        double IMultilabelDistanceClassifier<TInput>.Distance(TInput input, int classIndex)
        {
            double d = Distance(input);
            return classIndex == 0 ? d : -d;
        }


        double[] IMultilabelDistanceClassifier<TInput>.Distance(TInput[] input, int[] classIndex)
        {
            return ToMultilabel().Distance(input, classIndex, new double[input.Length]);
        }


        double[] IMultilabelDistanceClassifier<TInput>.Distance(TInput[] input, int classIndex)
        {
            return ToMultilabel().Distance(input, classIndex, new double[input.Length]);
        }


        double[] IMultilabelDistanceClassifier<TInput>.Distance(TInput[] input, int[] classIndex, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = Distances(input[i])[classIndex[i]];
            return result;
        }


        double[] IMultilabelDistanceClassifier<TInput>.Distance(TInput[] input, int classIndex, double[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = Distances(input[i])[classIndex];
            return result;
        }





        #region Distance

        // Input

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        public double[] Distances(TInput input)
        {
            return Distances(input, new double[NumberOfOutputs]);
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and its most strongly
        /// associated class (as predicted by the classifier).
        /// </summary>
        /// <param name="input">The input vector.</param>
        public double[] Distance(TInput[] input)
        {
            return Distance(input, new double[input.Length]);
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
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
        /// <param name="result">An array where the result will be stored, 
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public double[] Distances(TInput input, double[] result)
        {
            double d = Distance(input);
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
        /// <param name="result">An array where the result will be stored, 
        ///   avoiding unnecessary memory allocations.</param>
        ///
        public double[][] Distances(TInput[] input, double[][] result)
        {
            for (int i = 0; i < input.Length; i++)
            {
                double d = Distance(input[i]);
                result[i][0] = +d;
                result[i][1] = -d;
            }
            return result;
        }



        // Input, decision


        double IMulticlassOutDistanceClassifier<TInput, double>.Distance(TInput input, out double decision)
        {
            bool value;
            double d = Distance(input, out value);
            decision = Classes.ToZeroOne(value);
            return d;
        }


        double IMulticlassOutDistanceClassifier<TInput, int>.Distance(TInput input, out int decision)
        {
            bool value;
            double d = Distance(input, out value);
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
        public double[] Distances(TInput input, out bool decision)
        {
            return Distances(input, out decision, new double[NumberOfOutputs]);
        }


        double[] IMultilabelOutDistanceClassifier<TInput, int>.Distances(TInput input, out int decision)
        {
            return ToMultilabel().Distances(input, out decision, new double[NumberOfOutputs]);
        }


        double[] IMultilabelOutDistanceClassifier<TInput, double>.Distances(TInput input, out double decision)
        {
            return ToMultilabel().Distances(input, out decision, new double[NumberOfOutputs]);
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
        //    double result = ToMulticlass().Distance(input, out value);
        //    Vector.OneHot(value, decision);
        //    return result;
        //}


        //double IMulticlassRefDistanceClassifier<TInput, bool[]>.Distance(TInput input, ref bool[] decision)
        //{
        //    decision = create(input, decision);
        //    int value;
        //    double result = ToMulticlass().Distance(input, out value);
        //    Vector.OneHot(value, decision);
        //    return result;
        //}

        //double IMulticlassRefDistanceClassifier<TInput, double[]>.Distance(TInput input, ref double[] decision)
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
        public double[] Distances(TInput input, out bool decision, double[] result)
        {
            double d = Distance(input, out decision);
            result[0] = +d;
            result[1] = -d;
            return result;
        }

        double[] IMultilabelOutDistanceClassifier<TInput, double>.Distances(TInput input, out double decision, double[] result)
        {
            bool value;
            double d = Distance(input, out value);
            result[0] = +d;
            result[1] = -d;
            decision = Classes.ToZeroOne(value);
            return result;
        }


        double[] IMultilabelOutDistanceClassifier<TInput, int>.Distances(TInput input, out int decision, double[] result)
        {
            bool value;
            double d = Distance(input, out value);
            result[0] = +d;
            result[1] = -d;
            decision = Classes.ToZeroOne(value);
            return result;
        }

        double[] IMultilabelRefDistanceClassifier<TInput, bool[]>.Distances(TInput input, ref bool[] decision, double[] result)
        {
            bool value;
            double d = Distance(input, out value);
            result[0] = +d;
            result[1] = -d;
            Vector.OneHot(value, decision);
            return result;
        }


        double[] IMultilabelRefDistanceClassifier<TInput, int[]>.Distances(TInput input, ref int[] decision, double[] result)
        {
            bool value;
            double d = Distance(input, out value);
            result[0] = +d;
            result[1] = -d;
            Vector.OneHot(value, decision);
            return result;
        }

        double[] IMultilabelRefDistanceClassifier<TInput, double[]>.Distances(TInput input, ref double[] decision, double[] result)
        {
            bool value;
            double d = Distance(input, out value);
            result[0] = +d;
            result[1] = -d;
            Vector.OneHot(value, decision);
            return result;
        }

        
        // Input[], decision[]

        double[] IMulticlassDistanceClassifier<TInput, int>.Distance(TInput[] input, ref int[] decision)
        {
            return ToMulticlass().Distance(input, ref decision, new double[input.Length]);
        }

        double[] IMulticlassDistanceClassifier<TInput, double>.Distance(TInput[] input, ref double[] decision)
        {
            return ToMulticlass().Distance(input, ref decision, new double[input.Length]);
        }

        /// <summary>
        /// Predicts a class label for each input vector, returning a
        /// numerical score measuring the strength of association of the
        /// input vector to the most strongly related class.
        /// </summary>
        /// <param name="input">A set of input vectors.</param>
        /// <param name="decision">The class labels predicted for each input
        /// vector, as predicted by the classifier.</param>
        public double[] Distance(TInput[] input, ref bool[] decision)
        {
            return Distance(input, ref decision, new double[input.Length]);
        }

        //double[] IMulticlassDistanceClassifier<TInput, int[]>.Distance(TInput[] input, ref int[][] decision)
        //{
        //    return ToMulticlass().Distance(input, ref decision, new double[input.Length]);
        //}

        //double[] IMulticlassDistanceClassifier<TInput, bool[]>.Distance(TInput[] input, ref bool[][] decision)
        //{
        //    return ToMulticlass().Distance(input, ref decision, new double[input.Length]);
        //}

        //double[] IMulticlassDistanceClassifier<TInput, double[]>.Distance(TInput[] input, ref double[][] decision)
        //{
        //    return ToMulticlass().Distance(input, ref decision, new double[input.Length]);
        //}






        double[][] IMultilabelDistanceClassifier<TInput, int>.Distances(TInput[] input, ref int[] decision)
        {
            return ToMultilabel().Distances(input, ref decision, create<double>(input));
        }

        double[][] IMultilabelDistanceClassifier<TInput, double>.Distances(TInput[] input, ref double[] decision)
        {
            return ToMultilabel().Distances(input, ref decision, create<double>(input));
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
        public double[][] Distances(TInput[] input, ref bool[] decision)
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
        public double[] Distance(TInput[] input, ref bool[] decision, double[] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = Distance(input[i], out decision[i]);
            return result;
        }

        double[] IMulticlassDistanceClassifier<TInput, int>.Distance(TInput[] input, ref int[] decision, double[] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = ToMulticlass().Distance(input[i], out decision[i]);
            return result;
        }

        double[] IMulticlassDistanceClassifier<TInput, double>.Distance(TInput[] input, ref double[] decision, double[] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                result[i] = ToMulticlass().Distance(input[i], out decision[i]);
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
            bool value;
            for (int i = 0; i < input.Length; i++)
            {
                Distances(input[i], out value, result[i]);
                decision[i] = Vector.OneHot<int>(value, decision[i]);
            }
            return result;
        }

        //double[] IMulticlassDistanceClassifier<TInput, double[]>.Distance(TInput[] input, ref double[][] decision, double[] result)
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



        double[][] IMultilabelDistanceClassifier<TInput, double[]>.Distances(TInput[] input, ref double[][] decision, double[][] result)
        {
            decision = create(input, decision);
            bool value;
            for (int i = 0; i < input.Length; i++)
            {
                Distances(input[i], out value, result[i]);
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
        public double[][] Distances(TInput[] input, ref bool[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                Distances(input[i], out decision[i], result[i]);
            return result;
        }




        double[][] IMultilabelDistanceClassifier<TInput, int>.Distances(TInput[] input, ref int[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                ToMultilabel().Distances(input[i], out decision[i], result[i]);
            return result;
        }


        double[][] IMultilabelDistanceClassifier<TInput, double>.Distances(TInput[] input, ref double[] decision, double[][] result)
        {
            decision = create(input, decision);
            for (int i = 0; i < input.Length; i++)
                ToMultilabel().Distances(input[i], out decision[i], result[i]);
            return result;
        }

        //double[] IMulticlassDistanceClassifier<TInput, bool[]>.Distance(TInput[] input, ref bool[][] decision, double[] result)
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

        //double[] IMulticlassDistanceClassifier<TInput, int[]>.Distance(TInput[] input, ref int[][] decision, double[] result)
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
            return Distances(input, result);
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
            return Distances(input, result);
        }

        /// <summary>
        /// Views this instance as a multi-class distance classifier,
        /// giving access to more advanced methods, such as the prediction
        /// of integer labels.
        /// </summary>
        /// <returns>
        /// This instance seen as an <see cref="IMulticlassDistanceClassifier{TInput}" />.
        /// </returns>
        new public IMulticlassDistanceClassifier<TInput> ToMulticlass()
        {
            return (IMulticlassDistanceClassifier<TInput>)this;
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
