// Accord Math Library
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

namespace Accord.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    interface IConstraint
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

        /// <summary>
        ///   Gets how much the constraint is being violated.
        /// </summary>
        /// 
        /// <param name="input">The function point.</param>
        /// 
        /// <returns>
        ///   How much the constraint is being violated at the given point. Positive
        ///   value means the constraint is not being violated with the returned slack, 
        ///   while a negative value means the constraint is being violated by the returned
        ///   amount.
        /// </returns>
        /// 
        double GetViolation(double[] input);
    }
}
