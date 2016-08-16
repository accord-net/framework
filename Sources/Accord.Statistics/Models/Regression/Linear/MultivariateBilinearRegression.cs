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

namespace Accord.Statistics.Models.Regression.Linear
{
    using System;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using Accord.MachineLearning;

    /// <summary>
    ///   Multivariate Bilinear Regression.
    /// </summary>
    /// 
    [Serializable]
    public class MultivariateBilinearRegression : ITransform<double[], double[]>
    {
        private MultivariateLinearRegression a;
        private MultivariateLinearRegression b;

        public MultivariateBilinearRegression()
        {
        }

        public MultivariateLinearRegression InputTransform
        {
            get { return a; }
        }

        public MultivariateLinearRegression OutputTransform
        {
            get { return b; }
        }

        public MultivariateBilinearRegression Inverse()
        {
            return new MultivariateBilinearRegression()
            {
                a = b.Inverse(),
                b = a.Inverse()
            };
        }


        public double[] Transform(double[] input)
        {
            throw new NotImplementedException();
        }

        public double[][] Transform(double[][] input)
        {
            throw new NotImplementedException();
        }

        public double[][] Transform(double[][] input, double[][] result)
        {
            throw new NotImplementedException();
        }

        public int NumberOfInputs
        {
            get { throw new NotImplementedException(); }
        }

        public int NumberOfOutputs
        {
            get { throw new NotImplementedException(); }
        }
    }
}
