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

    public class TaylorGaussian : Gaussian, ITransform
    {

        private double[] coefficients;

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


        public TaylorGaussian()
            : base()
        {
            createCoefficients(1024);
        }

        public TaylorGaussian(double sigma)
            : base(sigma)
        {
            createCoefficients(1024);
        }




        public double[] Transform(double[] input)
        {
            //http://epubs.siam.org/doi/pdf/10.1137/1.9781611972818.19 

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

        protected override void OnSigmaChanging()
        {
            createCoefficients(Degree);
        }

    }
}
