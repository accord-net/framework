// Accord Math Library
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

namespace Accord.Math.Optimization
{
    /// <summary>
    /// Defines an interface for an optimization constraint.
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        ///   Gets the type of the constraint.
        /// </summary>
        /// 
        ConstraintType ShouldBe { get; }

        /// <summary>
        ///   Gets the value in the right hand
        ///   side of the constraint equation.
        /// </summary>
        /// 
        double Value { get; }

        /// <summary>
        ///   Gets the violation tolerance for the constraint.
        /// </summary>
        /// 
        double Tolerance { get; }

        /// <summary>
        ///   Gets the number of variables in the constraint.
        /// </summary>
        /// 
        int NumberOfVariables { get; }

        /// <summary>
        /// Calculates the left hand side of the constraint
        /// equation given a vector x.
        /// </summary>
        /// <param name="x">The vector.</param>
        /// <returns>
        /// The left hand side of the constraint equation as evaluated at x.
        /// </returns>
        double Function(double[] x);

        /// <summary>
        /// Calculates the gradient of the constraint
        /// equation given a vector x
        /// </summary>
        /// <param name="x">The vector.</param>
        /// <returns>The gradient of the constraint as evaluated at x.</returns>
        double[] Gradient(double[] x);
    }
}
