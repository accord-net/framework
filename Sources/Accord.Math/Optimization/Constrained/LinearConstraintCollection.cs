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
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    ///   Linear Constraint Collection.
    /// </summary>
    /// 
    public class LinearConstraintCollection : Collection<LinearConstraint>
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="LinearConstraintCollection"/> class.
        /// </summary>
        /// 
        public LinearConstraintCollection()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LinearConstraintCollection"/> class.
        /// </summary>
        /// 
        public LinearConstraintCollection(IEnumerable<LinearConstraint> list)
        {
            foreach (var c in list)
                this.Add(c);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LinearConstraintCollection"/> class.
        /// </summary>
        /// 
        public LinearConstraintCollection(IList<LinearConstraint> list)
            : base(list)
        {
        }

        /// <summary>
        ///   Creates a matrix of linear constraints in canonical form.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of variables in the objective function.</param>
        /// <param name="b">The vector of independent terms (the right hand side of the constraints).</param>
        /// <param name="equalities">The number of equalities in the matrix.</param>
        /// <returns>The matrix <c>A</c> of linear constraints.</returns>
        /// 
        public double[,] CreateMatrix(int numberOfVariables, out double[] b, out int equalities)
        {
            // First of all, separate the equality constraints from the inequalities.
            LinearConstraint[] constraintArray = this.ToArray();
            constraintArray.StableSort((c1, c2) => c1.ShouldBe.CompareTo(c2.ShouldBe));

            int numberOfConstraints = constraintArray.Length;
            double[,] A = new double[numberOfConstraints, numberOfVariables];
            b = new double[numberOfConstraints];
            equalities = 0;

            for (int i = 0; i < constraintArray.Length; i++)
            {
                LinearConstraint constraint = constraintArray[i];

                if (constraint.NumberOfVariables > numberOfVariables)
                    throw new ArgumentException("The number of variables in the constraint exceeds the number of variables for the problem.");

                for (int j = 0; j < constraint.VariablesAtIndices.Length; j++)
                {
                    int k = constraint.VariablesAtIndices[j];

                    if (k >= numberOfVariables)
                        throw new ArgumentException("The constraint refers to a variable index which is not present on the objective function.");

                    if (constraint.ShouldBe == ConstraintType.GreaterThanOrEqualTo ||
                        constraint.ShouldBe == ConstraintType.EqualTo)
                    {
                        A[i, k] = constraint.CombinedAs[j];
                        b[i] = constraint.Value;
                    }
                    else if (constraint.ShouldBe == ConstraintType.LesserThanOrEqualTo)
                    {
                        A[i, k] = -constraint.CombinedAs[j];
                        b[i] = -constraint.Value;
                    }
                    else
                        throw new ArgumentException("The provided constraint type is not supported.");
                }

                if (constraint.ShouldBe == ConstraintType.EqualTo)
                    equalities++;
            }

            return A;
        }

    }
}
