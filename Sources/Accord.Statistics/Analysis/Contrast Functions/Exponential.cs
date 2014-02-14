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

namespace Accord.Statistics.Analysis.ContrastFunctions
{
    using System;

    /// <summary>
    ///   Exponential contrast function.
    /// </summary>
    /// 
    /// <remarks>
    ///   According to Hyvärinen, the Exponential contrast function may be
    ///   used when the independent components are highly super-Gaussian or
    ///   when robustness is very important.
    /// </remarks>
    ///
    /// <seealso cref="IndependentComponentAnalysis"/>
    ///
    [Serializable]
    public class Exponential : IContrastFunction
    {
        double alpha = 1;

        /// <summary>
        ///   Initializes a new instance of the <see cref="Exponential"/> class.
        /// </summary>
        /// <param name="alpha">The exponential alpha constant. Default is 1.</param>
        /// 
        public Exponential(double alpha)
        {
            this.alpha = alpha;
        }

        /// <summary>
        ///   Gets the exponential alpha constant.
        /// </summary>
        /// 
        public double Alpha { get { return alpha; } }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Exponential"/> class.
        /// </summary>
        /// 
        public Exponential() { }

        /// <summary>
        ///   Contrast function.
        /// </summary>
        /// 
        /// <param name="x">The vector of observations.</param>
        /// <param name="output">At method's return, this parameter
        /// should contain the evaluation of function over the vector
        /// of observations <paramref name="x"/>.</param>
        /// <param name="derivative">At method's return, this parameter
        /// should contain the evaluation of function derivative over
        /// the vector of observations <paramref name="x"/>.</param>
        /// 
        public void Evaluate(double[] x, double[] output, double[] derivative)
        {
            // Exponential contrast function and its derivative, as given
            //  in original Hyvärinen's paper. See main references for the
            //  Independent Component Analysis class for details.

            for (int j = 0; j < x.Length; j++)
            {
                double w = x[j];
                double e = System.Math.Exp(-alpha * (w * w) / 2.0);

                // g(w*x) = wx * exp(-(wx^2)/2)
                output[j] = w * e;

                // g'(w*x) = (1 - wx^2) * exp(-(wx^2)/2)
                derivative[j] = (1.0 - alpha * w * w) * e;
            }
        }
    }

       
}
