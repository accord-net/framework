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

namespace Accord.Statistics.Models.Fields
{

    /// <summary>
    ///   Common interface for gradient evaluators for <see cref="HiddenConditionalRandomField{T}">
    ///   Hidden Conditional Random Fields </see>.
    /// </summary>
    /// 
    public interface IHiddenRandomFieldGradient
    {

        /// <summary>
        ///   Computes the gradient using the 
        ///   input/outputs stored in this object.
        /// </summary>
        /// 
        /// <returns>The value of the gradient vector for the given parameters.</returns>
        /// 
        double[] Gradient();

        /// <summary>
        ///   Computes the objective (cost) function for the Hidden
        ///   Conditional Random Field (negative log-likelihood) using
        ///   the input/outputs stored in this object.
        /// </summary>
        /// 
        double Objective();

    }
}
