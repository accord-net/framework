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
    ///   Hypothesis test for two Receiver-Operating 
    ///   Characteristic (ROC) curve areas (ROC-AUC).
    /// </summary>
    /// 
    [Serializable]
    public class TwoReceiverOperatingCurveTest : TwoSampleZTest
    {

        /// <summary>
        ///   First Receiver-Operating Characteristic curve.
        /// </summary>
        /// 
        public ReceiverOperatingCharacteristic Curve1 { get; private set; }

        /// <summary>
        ///   First Receiver-Operating Characteristic curve.
        /// </summary>
        /// 
        public ReceiverOperatingCharacteristic Curve2 { get; private set; }

        /// <summary>
        ///   Gets the summed Kappa variance
        ///   for the two contingency tables.
        /// </summary>
        /// 
        public double OverallVariance { get; private set; }

        /// <summary>
        ///   Gets the variance for the first Kappa value.
        /// </summary>
        /// 
        public double Variance1 { get; private set; }

        /// <summary>
        ///   Gets the variance for the second Kappa value.
        /// </summary>
        /// 
        public double Variance2 { get; private set; }

        /// <summary>
        ///   Creates a new test for two ROC curves.
        /// </summary>
        /// 
        /// <param name="curve1">The first ROC curve.</param>
        /// <param name="curve2">The second ROC curve.</param>
        /// <param name="hypothesizedDifference">The hypothesized difference between the two areas.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        ///
        public TwoReceiverOperatingCurveTest(ReceiverOperatingCharacteristic curve1, ReceiverOperatingCharacteristic curve2,
            double hypothesizedDifference = 0, TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            this.Curve1 = curve1;
            this.Curve2 = curve2;

            double[] Vx1 = curve1.NegativeAccuracies;
            double[] Vy1 = curve1.PositiveAccuracies;

            double[] Vx2 = curve2.NegativeAccuracies;
            double[] Vy2 = curve2.PositiveAccuracies;

            double covx = Statistics.Tools.Covariance(Vx1, Vx2);
            double covy = Statistics.Tools.Covariance(Vy1, Vy2);
            double cov = covx / Vx1.Length + covy / Vy1.Length;

            this.EstimatedValue1 = curve1.Area;
            this.EstimatedValue2 = curve2.Area;
            this.ObservedDifference = EstimatedValue1 - EstimatedValue2;
            this.HypothesizedDifference = hypothesizedDifference;

            this.Variance1 = curve1.Variance;
            this.Variance2 = curve2.Variance;

            this.OverallVariance = Variance1 + Variance2 - 2 * cov;
            this.StandardError = System.Math.Sqrt(OverallVariance);

            // Compute Z statistic
            double z = (ObservedDifference - HypothesizedDifference) / StandardError;

            Compute(z, alternate);
        }
    }
}
