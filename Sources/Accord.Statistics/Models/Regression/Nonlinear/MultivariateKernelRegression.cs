// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

namespace Accord.Statistics.Models.Regression
{
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class MultivariateKernelRegression<TKernel> : MultipleTransformBase<double[], double>
        where TKernel : IKernel<double[]>
    {
        public TKernel Kernel { get; set; }

        public double[][] Weights { get; set; }

        public double[][] Inputs { get ;set ;}

        public override double[] Transform(double[] input, double[] result)
        {
            return Transform(new[] { input }, new[] { result })[0];
        }

        public override double[][] Transform(double[][] input, double[][] result)
        {
            var newK = Kernel.ToJagged2(x: input, y: Inputs);

            // Project into the kernel principal components
            return Matrix.DotWithTransposed(newK, Weights, result: result);
        }
    }

    public class MultivariateKernelRegression : MultivariateKernelRegression<IKernel>
    {
        
    }
}
