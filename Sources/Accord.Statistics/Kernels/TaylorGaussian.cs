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

namespace Accord.Statistics.Kernels
{
    using System;
    using Accord.Math;

    /// <summary>
    ///   Taylor approximation for the explicit Gaussian kernel.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///      Lin, Keng-Pei, and Ming-Syan Chen. "Efficient kernel approximation for large-scale support 
    ///      vector machine classification." Proceedings of the Eleventh SIAM International Conference on
    ///      Data Mining. 2011. Available on: http://epubs.siam.org/doi/pdf/10.1137/1.9781611972818.19 
    ///      </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public class TaylorGaussian : Gaussian, ITransform
    {

        private double[] coefficients;

        /// <summary>
        ///   Gets or sets the approximation degree 
        ///   for this kernel. Default is 1024.
        /// </summary>
        /// 
        public int Degree
        {
            get { return coefficients.Length; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value", "Value must be higher than zero.");
                createCoefficients(value);
            }
        }


        /// <summary>
        ///   Constructs a new <see cref="TaylorGaussian"/> kernel.
        /// </summary>
        /// 
        public TaylorGaussian()
            : base()
        {
            createCoefficients(1024);
        }

        /// <summary>
        ///   Constructs a new <see cref="TaylorGaussian"/> kernel with the given sigma.
        /// </summary>
        /// 
        /// <param name="sigma">The kernel's sigma parameter.</param>
        /// 
        public TaylorGaussian(double sigma)
            : base(sigma)
        {
            createCoefficients(1024);
        }

        /// <summary>
        ///   Constructs a new <see cref="TaylorGaussian"/> kernel with the given sigma.
        /// </summary>
        /// 
        /// <param name="sigma">The kernel's sigma parameter.</param>
        /// <param name="degree">The Gaussian approximation degree. Default is 1024.</param>
        /// 
        public TaylorGaussian(double sigma, int degree)
            : base(sigma)
        {
            createCoefficients(degree);
        }

        /// <summary>
        ///   Projects an input point into feature space.
        /// </summary>
        /// 
        /// <param name="input">The input point to be projected into feature space.</param>
        /// 
        /// <returns>
        ///   The feature space representation of the given <paramref name="input"/> point.
        /// </returns>
        /// 
        public double[] Transform(double[] input)
        {
            // http://epubs.siam.org/doi/pdf/10.1137/1.9781611972818.19 

            double[] features = new double[coefficients.Length];

            features[0] = 1;

            for (int index = 1, k = 0; index < coefficients.Length; k++)
            {
                double alpha = coefficients[k];

                foreach (int[] s in Combinatorics.Sequences(input.Length, k + 1))
                {
                    double prod = 1.0;
                    for (int i = 0; i < s.Length; i++)
                        prod *= input[s[i]];

                    features[index++] = alpha * prod;
                    if (index >= coefficients.Length)
                        break;
                }
            }

            double norm = Norm.SquareEuclidean(input);
            double constant = Math.Exp(-Gamma * norm);

            for (int i = 0; i < features.Length; i++)
                features[i] *= constant;

            return features;
        }




        private void createCoefficients(int degree)
        {
            coefficients = new double[degree];
            for (int i = 0; i < coefficients.Length; i++)
                coefficients[i] = Math.Sqrt(Math.Pow(2 * Gamma, i + 1) / Special.Factorial(i + 1));
        }

        /// <summary>
        ///   Called when the value for any of the
        ///   kernel's parameters has changed.
        /// </summary>
        /// 
        protected override void OnSigmaChanging()
        {
            if (coefficients == null)
                return;

            createCoefficients(Degree);
        }

    }
}
