// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
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

namespace Accord.Statistics.Testing
{
    using System;
    using Accord.Statistics.Analysis;

    /// <summary>
    ///   Hypothesis test for a single ROC curve.
    /// </summary>
    /// 
    [Serializable]
    public class ReceiverOperatingCurveTest : ZTest
    {

        /// <summary>
        ///   Gets the ROC curve being tested.
        /// </summary>
        /// 
        public ReceiverOperatingCharacteristic Curve { get; private set; }



        /// <summary>
        ///   Creates a new <see cref="ReceiverOperatingCurveTest"/>.
        /// </summary>
        /// 
        /// <param name="curve">The curve to be tested.</param>
        /// <param name="hypothesizedValue">The hypothesized value for the ROC area.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        ///
        public ReceiverOperatingCurveTest(ReceiverOperatingCharacteristic curve, double hypothesizedValue = 0.5,
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            this.Curve = curve;

            Compute(curve.Area, hypothesizedValue, curve.StandardError, alternate);
        }

        /// <summary>
        ///   Calculates the standard error of an area calculation for a
        ///   curve with the given number of positive and negatives instances
        /// </summary>
        /// 
        public static double HanleyMcNealVariance(double area, int positiveCount, int negativeCount)
        {
            double A = area;

            // real positive cases
            int Na = positiveCount;

            // real negative cases
            int Nn = negativeCount;

            double Q1 = A / (2.0 - A);
            double Q2 = 2 * A * A / (1.0 + A);

            return (A * (1.0 - A) +
                (Na - 1.0) * (Q1 - A * A) +
                (Nn - 1.0) * (Q2 - A * A)) / (Na * Nn);
        }

        /// <summary>
        ///   Calculates the standard error of an area calculation for a
        ///   curve with the given number of positive and negatives instances
        /// </summary>
        /// 
        public static double DeLongVariance(double[] positiveAccuracies, double[] negativeAccuracies)
        {
            double[] Vx = positiveAccuracies;
            double[] Vy = negativeAccuracies;

            double meanVx = Statistics.Tools.Mean(Vx);
            double meanVy = Statistics.Tools.Mean(Vy);

            double varVx = Statistics.Tools.Variance(Vx);
            double varVy = Statistics.Tools.Variance(Vy);

            return varVx / Vx.Length + varVy / Vy.Length;
        }

      
    }
}
