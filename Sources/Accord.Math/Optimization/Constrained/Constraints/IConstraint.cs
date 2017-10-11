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
    using System;

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
        ///   Gets the left hand side of 
        ///   the constraint equation.
        /// </summary>
        /// 
        Func<double[], double> Function { get; }

        /// <summary>
        ///   Gets the gradient of the left hand
        ///   side of the constraint equation.
        /// </summary>
        /// 
        Func<double[], double[]> Gradient { get; }
    }
}
