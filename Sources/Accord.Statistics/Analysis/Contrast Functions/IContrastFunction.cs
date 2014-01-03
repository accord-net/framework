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
    ///   Common interface for contrast functions.
    /// </summary>
    /// 
    /// <remarks>
    ///   Contrast functions are used as objective functions in
    ///   neg-entropy calculations.
    /// </remarks>
    /// 
    /// <seealso cref="IndependentComponentAnalysis"/>
    ///
    public interface IContrastFunction
    {
        /// <summary>
        ///   Contrast function.
        /// </summary>
        /// 
        /// <param name="x">The vector of observations.</param>
        /// <param name="output">At method's return, this parameter
        ///   should contain the evaluation of function over the vector
        ///   of observations <paramref name="x"/>.</param>
        /// <param name="derivative">At method's return, this parameter
        ///   should contain the evaluation of function derivative over 
        ///   the vector of observations <paramref name="x"/>.</param>
        ///   
        void Evaluate(double[] x, double[] output, double[] derivative);
    }
}
