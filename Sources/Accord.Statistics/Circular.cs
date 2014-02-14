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

namespace Accord.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///   Set of statistics functions operating over a circular space.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class represents collection of common functions used in
    ///   statistics. The values are handled as belonging to a distribution
    ///   defined over a circle, such as the <see cref="Accord.Statistics.Distributions.Univariate.VonMisesDistribution"/>.
    /// </remarks>
    /// 
    public static class Circular
    {

        #region Array Measures

        /// <summary>
        ///   Computes the Mean of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <returns>The mean of the given angles.</returns>
        /// 
        public static double Mean(double[] angles)
        {
            double N = angles.Length;

            double cos = 0, sin = 0;

            for (int i = 0; i < angles.Length; i++)
            {
                cos += Math.Cos(angles[i]);
                sin += Math.Sin(angles[i]);
            }

            return Math.Atan2(sin / N, cos / N);
        }

        /// <summary>
        ///   Computes the Variance of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <returns>The variance of the given angles.</returns>
        /// 
        public static double Variance(double[] angles)
        {
            double cos = 0, sin = 0;

            for (int i = 0; i < angles.Length; i++)
            {
                cos += Math.Cos(angles[i]);
                sin += Math.Sin(angles[i]);
            }

            double rho = Math.Sqrt(sin * sin + cos * cos);

            return 1.0 - rho / angles.Length;
        }

        /// <summary>
        ///   Computes the concentration (kappa) of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <returns>
        ///   The concentration (kappa) parameter of the <see cref="Accord.Statistics.Distributions.Univariate.VonMisesDistribution"/>
        ///   for the given data.
        /// </returns>
        /// 
        public static double Concentration(double[] angles)
        {
            return Concentration(angles, Mean(angles));
        }

        /// <summary>
        ///   Computes the concentration (kappa) of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <param name="mean">The mean of the angles, if already known.</param>
        /// <returns>
        ///   The concentration (kappa) parameter of the <see cref="Accord.Statistics.Distributions.Univariate.VonMisesDistribution"/>
        ///   for the given data.
        /// </returns>
        /// 
        public static double Concentration(double[] angles, double mean)
        {
            double cos = 0;

            for (int i = 0; i < angles.Length; i++)
                cos += Math.Cos(angles[i] - mean);

            return estimateKappa(cos / angles.Length);
        }
        #endregion

        /// <summary>
        ///   Computes the Weighted Mean of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <param name="weights">An unit vector containing the importance of each angle
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <returns>The mean of the given angles.</returns>
        /// 
        public static double WeightedMean(double[] angles, double[] weights)
        {
            double cos = 0, sin = 0;

            for (int i = 0; i < angles.Length; i++)
            {
                cos += Math.Cos(angles[i]) * weights[i];
                sin += Math.Sin(angles[i]) * weights[i];
            }

            return Math.Atan2(sin, cos);
        }

        /// <summary>
        ///   Computes the Weighted Concentration of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <param name="weights">An unit vector containing the importance of each angle
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <returns>The mean of the given angles.</returns>
        /// 
        public static double WeightedConcentration(double[] angles, double[] weights)
        {
            return WeightedConcentration(angles, weights, WeightedMean(angles, weights));
        }

        /// <summary>
        ///   Computes the Weighted Concentration of the given angles.
        /// </summary>
        /// 
        /// <param name="angles">A double array containing the angles in radians.</param>
        /// <param name="weights">An unit vector containing the importance of each angle
        /// in <see param="values"/>. The sum of this array elements should add up to 1.</param>
        /// <param name="mean">The mean of the angles, if already known.</param>
        /// <returns>The mean of the given angles.</returns>
        /// 
        public static double WeightedConcentration(double[] angles, double[] weights, double mean)
        {
            double cos = 0;

            for (int i = 0; i < angles.Length; i++)
                cos += Math.Cos(angles[i] - mean) * weights[i];

            return estimateKappa(cos);
        }

        /// <summary>
        ///   Computes the maximum likelihood estimate
        ///   of kappa given by Best and Fisher (1981).
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   This method implements the approximation to the Maximum Likelihood
        ///   Estimative of the kappa concentration parameter as suggested by Best
        ///   and Fisher (1981), cited by Zheng Sun (2006) and Hussin and Mohamed
        ///   (2008). Other useful approximations are given by Suvrit Sra (2009).</para>
        ///   
        /// <para>    
        ///   References:
        ///   <list type="bullet">
        ///     <item><description>
        ///       A.G. Hussin and I.B. Mohamed, 2008. Efficient Approximation for the von Mises Concentration Parameter.
        ///       Asian Journal of Mathematics &amp; Statistics, 1: 165-169. </description></item>
        ///     <item><description><a href="http://www.kyb.mpg.de/publications/attachments/vmfnote_7045%5B0%5D.pdf">
        ///       Suvrit Sra, "A short note on parameter approximation for von Mises-Fisher distributions:
        ///       and a fast implementation of $I_s(x)$". (revision of Apr. 2009). Computational Statistics (2011).
        ///       Available on: http://www.kyb.mpg.de/publications/attachments/vmfnote_7045%5B0%5D.pdf </a></description></item>
        ///     <item><description>
        ///       Zheng Sun. M.Sc. Comparing measures of fit for circular distributions. Master thesis, 2006.
        ///       Available on: https://dspace.library.uvic.ca:8443/bitstream/handle/1828/2698/zhengsun_master_thesis.pdf </description></item>
        ///   </list></para>
        /// </remarks>
        /// 
        private static double estimateKappa(double r)
        {
            // Best and Fisher (1981) gave a simple
            //   approximation to the MLE of kappa:

            double r3 = r * r * r;

            if (r < 0.53)
            {
                double r5 = r3 * r * r;
                return (2.0 * r) + (r3) + (5.0 * r5 / 6.0);
            }

            if (r < 0.85)
            {
                return -0.4 + 1.39 * r + 0.43 / (1.0 - r);
            }

            return 1.0 / (r3 - 4 * r * r + 3 * r);

            // However, Sun (2006) mentions this estimate of k
            // is not reliable when r is small, such as when r < 0.7.
        }
    }
}
