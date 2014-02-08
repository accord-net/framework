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

    /// <summary>
    ///   Quadratic Kernel.
    /// </summary>
    /// 
    [Serializable]
    public sealed class Quadratic : KernelBase, IKernel,
        IDistance, IReverseDistance, ICloneable, IExpandable
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

            double sumx = 0, sumy = 0, sum = 0;

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
            double sumx = constant;
            double sumy = constant;
            double sum = constant;

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

        public double[] Expand(double[] input)
        {
            if (constant != 0)
                throw new NotSupportedException();

            int n = input.Length;

            double[] features = new double[n * n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    features[i * n + j] = input[i] * input[j];
                }
            }

            return features;
        }
    }
}
