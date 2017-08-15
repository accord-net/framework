// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2015
// diego.catalano at live.com
//
// Copyright 2015 Haifeng Li
// haifeng.hli at gmail.com
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

namespace Accord.Statistics.Kernels
{
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Thin Spline Plate Kernel.
    /// </summary>
    /// 
    /// <remarks>
    ///   Thin plate splines (TPS) are a spline-based technique for data interpolation and smoothing.
    /// </remarks>
    /// 
    [Serializable]
    public sealed class ThinSplinePlate : KernelBase, IKernel, ICloneable
    {
        private double sigma;

        /// <summary>
        ///   Gets or sets the sigma constant for this kernel.
        /// </summary>
        /// 
        public double Sigma
        {
            get { return sigma; }
            set { sigma = Math.Max(value,1); }
        }

        /// <summary>
        ///   Constructs a new ThinSplinePlate Kernel.
        /// </summary>
        /// 
        /// <param name="sigma">The value for sigma.</param>
        /// 
        public ThinSplinePlate(double sigma)
        {
            Sigma = sigma;
        }

        /// <summary>
        ///   Thin Spline Plate Kernel Function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public override double Function(double[] x, double[] y)
        {
            double r = 0;
            for (int i = 0; i < x.Length; i++)
            {
                double dxy = x[i] - y[i];
                r += dxy * dxy;
            }

            return r / (sigma * sigma) * Math.Log(Math.Sqrt(r) / sigma);
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

    }
}
