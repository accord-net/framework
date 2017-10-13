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
    /// Extension methods on the <see cref="IConstraint"/> interface.
    /// </summary>
    public static class ConstraintExtensions
    {

        /// <summary>
        /// Gets how much the constraint is being violated.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <param name="input">The function point.</param>
        /// <returns>
        /// How much the constraint is being violated at the given point. Positive
        /// value means the constraint is not being violated with the returned slack,
        /// while a negative value means the constraint is being violated by the returned
        /// amount.
        /// </returns>
        public static double GetViolation(this IConstraint constraint, double[] input)
        {
            double fx = constraint.Function(input);

            switch (constraint.ShouldBe)
            {
                case ConstraintType.EqualTo:
                    return Math.Abs(fx - constraint.Value);

                case ConstraintType.GreaterThanOrEqualTo:
                    return fx - constraint.Value;

                case ConstraintType.LesserThanOrEqualTo:
                    return constraint.Value - fx;
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets whether this constraint is being violated
        /// (within the current tolerance threshold).
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <param name="input">The function point.</param>
        /// <returns>
        /// True if the constraint is being violated, false otherwise.
        /// </returns>
        public static bool IsViolated(this IConstraint constraint, double[] input)
        {
            return constraint.GetViolation(input) + constraint.Tolerance < 0;
        }
    }
}
