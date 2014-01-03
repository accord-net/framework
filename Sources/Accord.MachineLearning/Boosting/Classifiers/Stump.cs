// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Darko Jurić, 2013
// https://code.google.com/p/accord/issues/detail?id=27
//
// Copyright © César Souza, 2009-2014
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
    using Accord.Math.Comparers;
    using Accord.Math;

    /// <summary>
    ///   Simple classifier that based on decision margins that
    ///   are perpendicular to one of the space dimensions.
    /// </summary>
    /// 
    public class DecisionStump : IWeakClassifier
    {

        private int inputCount;

        private double threshold;
        private int featureIndex;
        private int sign;

        /// <summary>
        ///   Initializes a new instance of the <see cref="DecisionStump"/> class.
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs for this classifier.</param>
        /// 
        public DecisionStump(int inputs)
        {
            this.inputCount = inputs;
        }

        /// <summary>
        ///   Gets the decision threshold for this linear classifier.
        /// </summary>
        /// 
        public double Threshold { get { return threshold; } }

        /// <summary>
        ///   Gets the index of the attribute which this
        ///   classifier will use to compare against
        ///   <see cref="Threshold"/>.
        /// </summary>
        /// 
        public int Index { get { return featureIndex; } }

        /// <summary>
        ///   Gets the direction of the comparison 
        ///   (if greater than or less than).
        /// </summary>
        /// 
        public int Sign { get { return sign; } }


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
        public int Compute(double[] inputs)
        {
            return inputs[featureIndex] < threshold ? sign : -sign;
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
        public void Learn(double[][] inputs, int[] outputs, double[] weights)
        {
            ElementComparer comparer = new ElementComparer();

            double errorMinimum = Double.MaxValue;

            for (int i = 0; i < inputCount; i++)
            {
                comparer.Index = i;
                int[] indices = Matrix.Indices(0, inputs.Length);
                Array.Sort<int>(indices, (a, b) => inputs[a][i].CompareTo(inputs[b][i]));

                double error = 0.0;
                for (int j = 0; j < outputs.Length; j++)
                {
                    int idx = indices[j];
                    if (outputs[idx] > 0)
                        error += weights[idx];
                }

                for (int j = 0; j < outputs.Length - 1; j++)
                {
                    int idx = indices[j];
                    int nidx = indices[j + 1];

                    if (outputs[idx] < 0)
                        error += weights[idx];
                    else
                        error -= weights[idx];

                    double midpoint = (inputs[idx][i] + inputs[nidx][i]) / 2.0;

                    // Compare to current best
                    if (error < errorMinimum)
                    {
                        errorMinimum = error;
                        this.featureIndex = i;
                        this.threshold = midpoint;
                        this.sign = +1;
                    }
                    if ((1.0 - error) < errorMinimum)
                    {
                        errorMinimum = (1.0 - error);
                        this.featureIndex = i;
                        this.threshold = midpoint;
                        this.sign = -1;
                    }
                }
            }
        }



    }
}
