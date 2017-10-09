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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    ///   Linear Constraint Collection.
    /// </summary>
    /// 
    public class LinearConstraintCollection : Collection<LinearConstraint>, IEnumerable<LinearConstraint>
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
            double[] tolerances;
            return CreateMatrix(numberOfVariables, out b, out tolerances, out equalities);
        }

        /// <summary>
        ///   Creates a matrix of linear constraints in canonical form.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of variables in the objective function.</param>
        /// <param name="b">The vector of independent terms (the right hand side of the constraints).</param>
        /// <param name="tolerances">The amount each constraint can be violated before the answer is declared close enough.</param>
        /// <param name="equalities">The number of equalities in the matrix.</param>
        /// <returns>The matrix <c>A</c> of linear constraints.</returns>
        /// 
        public double[,] CreateMatrix(int numberOfVariables, out double[] b,
            out double[] tolerances, out int equalities)
        {
            // First of all, separate the equality constraints from the inequalities.
            LinearConstraint[] constraintArray = this.ToArray();
            constraintArray.Sort((c1, c2) => c1.ShouldBe.CompareTo(c2.ShouldBe), stable: true);

            int numberOfConstraints = constraintArray.Length;
            double[,] A = new double[numberOfConstraints, numberOfVariables];
            b = new double[numberOfConstraints];
            tolerances = new double[numberOfConstraints];
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
                        tolerances[i] = constraint.Tolerance;
                    }
                    else if (constraint.ShouldBe == ConstraintType.LesserThanOrEqualTo)
                    {
                        A[i, k] = -constraint.CombinedAs[j];
                        b[i] = -constraint.Value;
                        tolerances[i] = constraint.Tolerance;
                    }
                    else
                    {
                        throw new ArgumentException("The provided constraint type is not supported.");
                    }
                }

                if (constraint.ShouldBe == ConstraintType.EqualTo)
                    equalities++;
            }

            return A;
        }

        /// <summary>
        ///   Creates a <see cref="LinearConstraintCollection"/> from a matrix
        ///   specifying the constraint variables and a vector specifying their
        ///   expected value.
        /// </summary>
        /// 
        /// <param name="a">The constraint matrix.</param>
        /// <param name="b">The constraint values.</param>
        /// <param name="meq">The number of equalities at the start of the 
        /// matrix <paramref name="a"/>. Contraints thereafter are taken to be
        /// less than or equal to the constraint values.</param>
        /// 
        [Obsolete("This method is obsolete because the convention is inconsistent with GoldfarbIdnani. Use Create(...) instead.")]
        public static LinearConstraintCollection FromMatrix(double[,] a, double[] b, int meq)
        {
            int numberOfVariables = a.GetLength(1);
            int numberOfConstraints = a.GetLength(0);

            var constraints = new LinearConstraint[numberOfConstraints];
            for (int i = 0; i < constraints.Length; i++)
            {
                constraints[i] = new LinearConstraint(numberOfVariables);
                a.GetRow(i, result: constraints[i].CombinedAs);
                if (i < meq)
                    constraints[i].ShouldBe = ConstraintType.EqualTo;
                else
                    constraints[i].ShouldBe = ConstraintType.LesserThanOrEqualTo;
                constraints[i].Value = b[i];
            }

            return new LinearConstraintCollection(constraints);
        }

        /// <summary>
        ///   Creates a <see cref="LinearConstraintCollection"/> from a matrix
        ///   specifying the constraint variables and a vector specifying their
        ///   expected value.
        /// </summary>
        /// 
        /// <param name="a">The constraint matrix.</param>
        /// <param name="b">The constraint values.</param>
        /// <param name="meq">The number of equalities at the start of the 
        /// matrix <paramref name="a"/>. Contraints thereafter are taken to be
        /// greater than or equal to the constraint values.</param>
        /// 
        public static LinearConstraintCollection Create(double[,] a, double[] b, int meq)
        {
            int numberOfVariables = a.GetLength(1);
            int numberOfConstraints = a.GetLength(0);

            var constraints = new LinearConstraint[numberOfConstraints];
            for (int i = 0; i < constraints.Length; i++)
            {
                constraints[i] = new LinearConstraint(numberOfVariables);
                a.GetRow(i, result: constraints[i].CombinedAs);
                if (i < meq)
                    constraints[i].ShouldBe = ConstraintType.EqualTo;
                else
                    constraints[i].ShouldBe = ConstraintType.GreaterThanOrEqualTo;
                constraints[i].Value = b[i];
            }

            return new LinearConstraintCollection(constraints);
        }
    }
}
