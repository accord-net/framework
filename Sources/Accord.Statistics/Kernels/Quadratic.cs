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

namespace Accord.Statistics.Kernels
{
    using System;
    using Accord.Math;
    using Accord.Math.Distances;
    using Accord.Compat;

    /// <summary>
    ///   Quadratic Kernel.
    /// </summary>
    /// 
    [Serializable]
    public sealed class Quadratic : KernelBase, IKernel,
        IDistance, IReverseDistance, ICloneable, ITransform
    {
        private double constant;

        /// <summary>
        ///   Constructs a new Quadratic kernel.
        /// </summary>
        /// 
        /// <param name="constant">The polynomial constant for this kernel. Default is 1.</param>
        /// 
        public Quadratic(double constant)
        {
            this.constant = constant;
        }

        /// <summary>
        ///   Constructs a new Quadratic kernel.
        /// </summary>
        /// 
        public Quadratic()
            : this(1.0) { }


        /// <summary>
        ///   Gets or sets the kernel's polynomial constant term.
        /// </summary>
        /// 
        public double Constant
        {
            get { return constant; }
            set { constant = value; }
        }


        /// <summary>
        ///   Quadratic kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// 
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public override double Function(double[] x, double[] y)
        {
            double sum = constant;
            for (int i = 0; i < x.Length; i++)
                sum += x[i] * y[i];

            return sum * sum;
        }

        /// <summary>
        ///   Quadratic kernel function.
        /// </summary>
        /// 
        /// <param name="z">Distance <c>z</c> in input space.</param>
        /// 
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double z)
        {
            double sum = constant + z;

            return sum * sum;
        }


        /// <summary>
        ///   Computes the squared distance in input space
        ///   between two points given in feature space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in feature (kernel) space.</param>
        /// <param name="y">Vector <c>y</c> in feature (kernel) space.</param>
        /// 
        /// <returns>Distance between <c>x</c> and <c>y</c> in input space.</returns>
        /// 
        public override double Distance(double[] x, double[] y)
        {
            if (x == y)
                return 0.0;

            double sumx = constant, sumy = constant, sum = constant;

            for (int i = 0; i < x.Length; i++)
            {
                sumx += x[i] * x[i];
                sumy += y[i] * y[i];
                sum += x[i] * y[i];
            }

            return sumx * sumx + sumy * sumy - 2 * sum * sum;
        }

        /// <summary>
        ///   Computes the distance in input space
        ///   between two points given in feature space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in feature (kernel) space.</param>
        /// <param name="y">Vector <c>y</c> in feature (kernel) space.</param>
        /// <returns>Distance between <c>x</c> and <c>y</c> in input space.</returns>
        /// 
        public double ReverseDistance(double[] x, double[] y)
        {
            double sumx = 0;
            double sumy = 0;
            double sum = 0;

            for (int i = 0; i < x.Length; i++)
            {
                sumx += x[i] * x[i];
                sumy += y[i] * y[i];
                sum += x[i] * y[i];
            }


            return Math.Sqrt(sumx) + Math.Sqrt(sumy) - 2.0 * Math.Sqrt(sum);
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            return MemberwiseClone();
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
            return Transform(input, constant);
        }

        /// <summary>
        ///   Projects a set of input points into feature space.
        /// </summary>
        /// 
        /// <param name="inputs">The input points to be projected into feature space.</param>
        /// 
        /// <returns>
        ///   The feature space representation of the given <paramref name="inputs"/> points.
        /// </returns>
        /// 
        public double[][] Transform(double[][] inputs)
        {
            double[][] r = new double[inputs.Length][];
            for (int i = 0; i < inputs.Length; i++)
                r[i] = Transform(inputs[i]);
            return r;
        }

        /// <summary>
        ///   Projects an input point into feature space.
        /// </summary>
        /// 
        /// <param name="input">The input point to be projected into feature space.</param>
        /// <param name="constant">The <see cref="Constant"/> parameter of the kernel.</param>
        /// 
        /// <returns>
        ///   The feature space representation of the given <paramref name="input"/> point.
        /// </returns>
        /// 
        public static double[] Transform(double[] input, double constant)
        {
            int n = input.Length;
            int m = (n * (n + 1)) / 2;

            double[] features = (constant == 0) ?
                new double[m] : new double[m + n + 1];

            for (int i = 0; i < input.Length; i++)
                features[i] = input[i] * input[i];

            int c = input.Length;
            for (int i = 0; i < input.Length; i++)
                for (int j = i + 1; j < input.Length; j++)
                    features[c++] = Constants.Sqrt2 * input[i] * input[j];

            if (constant != 0)
            {
                double sqrt2c = Math.Sqrt(2 * constant);

                for (int i = 0; i < input.Length; i++)
                    features[m + i] = input[i] * sqrt2c;

                features[features.Length - 1] = constant;
            }

            return features;
        }
    }
}
