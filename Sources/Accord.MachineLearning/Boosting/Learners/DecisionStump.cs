// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Darko Jurić, 2013
// https://code.google.com/p/accord/issues/detail?id=27
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

namespace Accord.MachineLearning.Boosting.Learners
{
    using System;
    using Accord.Compat;
    using Accord.MachineLearning.DecisionTrees;

    /// <summary>
    ///   Simple classifier that based on decision margins that
    ///   are perpendicular to one of the space dimensions.
    /// </summary>
    /// 
    /// <seealso cref="ThresholdLearning"/>
    /// <seealso cref="AdaBoost{TModel}"/>
    /// 
    /// <example>
    /// <para>
    ///   The <see cref="DecisionStump"/> classifier is mostly intended to be used as a weak classifier
    ///   in the context of an <see cref="AdaBoost{TModel}"/> learning algorithm. Please refer to the
    ///   <see cref="AdaBoost{TModel}"/> class for more examples on using the classifier in this scenario.
    ///   A simple example is shown below:</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Boosting\AdaBoostTest.cs" region="doc_learn" />
    ///   
    /// <para>
    ///   It is also possible to use the <see cref="DecisionStump"/> as a standalone classifier through
    ///   the <see cref="ThresholdLearning"/> algorithm. An example is given below:</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Boosting\DecisionStumpTest.cs" region="doc_learn" />
    /// </example>
    /// 
    [Serializable]
    public class DecisionStump : BinaryClassifierBase<double[]>
    {
        // TODO: Consider merging this class with DecisionNode or KDTreeNode

        private double threshold;
        private int featureIndex;
        private ComparisonKind comparison;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecisionStump"/> class.
        /// </summary>
        public DecisionStump()
        {

        }

        /// <summary>
        ///   Gets the decision threshold for this linear classifier.
        /// </summary>
        /// 
        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        /// <summary>
        ///   Gets the index of the attribute which this
        ///   classifier will use to compare against
        ///   <see cref="Threshold"/>.
        /// </summary>
        /// 
        public int Index
        {
            get { return featureIndex; }
            set { featureIndex = value; }
        }

        /// <summary>
        ///   Gets or sets the comparison to be performed.
        /// </summary>
        /// 
        public ComparisonKind Comparison
        {
            get { return comparison; }
            set { comparison = value; }
        }

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="P:Accord.MachineLearning.ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <returns>A class-label that best described <paramref name="input" /> according
        /// to this classifier.</returns>
        public override bool Decide(double[] input)
        {
            double value = input[featureIndex];

            switch (comparison)
            {
                case ComparisonKind.Equal:
                    return value == threshold;
                case ComparisonKind.NotEqual:
                    return value != threshold;
                case ComparisonKind.GreaterThanOrEqual:
                    return value >= threshold;
                case ComparisonKind.GreaterThan:
                    return value >= threshold;
                case ComparisonKind.LessThan:
                    return value < threshold;
                case ComparisonKind.LessThanOrEqual:
                    return value <= threshold;
                default:
                    throw new InvalidOperationException();
            }
        }




        #region Obsolete
        /// <summary>
        ///   Initializes a new instance of the <see cref="DecisionStump"/> class.
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs for this classifier.</param>
        /// 
        [Obsolete("Please use the default constructor instead.")]
        public DecisionStump(int inputs)
        {
        }

        /// <summary>
        ///   Gets the direction of the comparison 
        ///   (if greater than or less than).
        /// </summary>
        /// 
        [Obsolete("Please use the Comparison property instead.")]
        public int Sign
        {
            get
            {
                switch (Comparison)
                {
                    case ComparisonKind.Equal:
                        return 0;
                    case ComparisonKind.GreaterThanOrEqual:
                    case ComparisonKind.GreaterThan:
                        return 1;
                    case ComparisonKind.LessThan:
                    case ComparisonKind.LessThanOrEqual:
                        return -1;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        ///   Computes the output class label for a given input.
        /// </summary>
        /// 
        /// <param name="inputs">The input vector.</param>
        /// 
        /// <returns>
        ///   The most likely class label for the given input.
        /// </returns>
        /// 
        [Obsolete("Please use the Decide() method instead.")]
        public int Compute(double[] inputs)
        {
            return Decide(inputs) ? +1 : -1;
        }

        /// <summary>
        ///   Teaches the stump classifier to recognize
        ///   the class labels of the given input samples.
        /// </summary>
        /// 
        /// <param name="inputs">The input vectors.</param>
        /// <param name="outputs">The class labels corresponding to each input vector.</param>
        /// <param name="weights">The weights associated with each input vector.</param>
        /// 
        [Obsolete("Please use the ThresholdLearning class instead.")]
        public void Learn(double[][] inputs, int[] outputs, double[] weights)
        {
            new ThresholdLearning()
            {
                Model = this
            }.Learn(inputs, outputs, weights);
        }

        #endregion
    }
}
