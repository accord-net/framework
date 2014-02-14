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
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;
    using System.Text;


    /// <summary>
    ///   Constraint with only quadratic terms.
    /// </summary>
    /// 
    public class QuadraticConstraint : NonlinearConstraint
    {

        /// <summary>
        ///   Gets the matrix of <c>A</c> quadratic terms
        ///   for the constraint <c>x'Ax + x'b</c>.
        /// </summary>
        /// 
        public double[,] QuadraticTerms { get; private set; }

        /// <summary>
        ///   Gets the vector <c>b</c> of linear terms
        ///   for the constraint <c>x'Ax + x'b</c>.
        /// </summary>
        /// 
        public double[] LinearTerms { get; private set; }


        /// <summary>
        ///   Constructs a new quadratic constraint in the form <c>x'Ax + x'b</c>.
        /// </summary>
        /// 
        /// <param name="objective">The objective function to which this constraint refers.</param>
        /// <param name="quadraticTerms">The matrix of <c>A</c> quadratic terms.</param>
        /// <param name="linearTerms">The vector <c>b</c> of linear terms.</param>
        /// <param name="shouldBe">How the left hand side of the constraint should be compared to the given <paramref name="value"/>.</param>
        /// <param name="value">The right hand side of the constraint equation.</param>
        /// <param name="withinTolerance">The tolerance for violations of the constraint. Equality
        ///   constraints should set this to a small positive value. Default is 0.</param>
        ///
        public QuadraticConstraint(IObjectiveFunction objective,
            double[,] quadraticTerms, double[] linearTerms = null,
            ConstraintType shouldBe = ConstraintType.LesserThanOrEqualTo,
            double value = 0, double withinTolerance = 0.0)
        {
            int n = objective.NumberOfVariables;

            if (quadraticTerms == null)
                throw new ArgumentNullException("quadraticTerms");

            if (quadraticTerms.GetLength(0) != quadraticTerms.GetLength(1))
                throw new DimensionMismatchException("quadraticTerms", "Matrix must be square.");

            if (quadraticTerms.GetLength(0) != n)
                throw new DimensionMismatchException("quadraticTerms", 
                    "Matrix rows must match the number of variables in the objective function.");

            if (linearTerms != null)
            {
                if (linearTerms.Length != n)
                    throw new DimensionMismatchException("linearTerms",
                        "The length of the linear terms vector must match the "+
                        "number of variables in the objective function.");
            }
            else
            {
                linearTerms = new double[n];
            }

            this.QuadraticTerms = quadraticTerms;
            this.LinearTerms = linearTerms;

            Create(objective, function, shouldBe, value, gradient, withinTolerance);
        }


        private double function(double[] x)
        {
            return (x.Multiply(QuadraticTerms)).InnerProduct(x) + LinearTerms.InnerProduct(x);
        }

        private double[] gradient(double[] x)
        {
            double[] g = new double[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                // Calculate quadratic terms
                g[i] = 2.0 * x[i] * QuadraticTerms[i, i];
                for (int j = 0; j < x.Length; j++)
                    if (i != j) g[i] += x[j] * (QuadraticTerms[i, j] + QuadraticTerms[j, i]);

                // Calculate for linear terms
                g[i] += LinearTerms[i];
            }

            return g;
        }
    }
}
