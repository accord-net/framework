// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Darko Jurić, 2013
// https://code.google.com/p/accord/issues/detail?id=27
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

namespace Accord.MachineLearning.Boosting
{
    /// <summary>
    ///   Common interface for Weak classifiers
    ///   used in Boosting mechanisms.
    /// </summary>
    /// 
    /// <seealso cref="AdaBoost{TModel}"/>
    /// 
    public interface IWeakClassifier
    {
        /// <summary>
        ///   Computes the output class label for a given input.
        /// </summary>
        /// 
        /// <param name="inputs">The input vector.</param>
        /// 
        /// <returns>The most likely class label for the given input.</returns>
        /// 
        int Compute(double[] inputs);
    }
}
