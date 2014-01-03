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
    ///   Constraint with only linear terms.
    /// </summary>
    /// 
    public class NonlinearConstraint
    {

        /// <summary>
        ///   Gets the number of variables in the constraint.
        /// </summary>
        /// 
        public int NumberOfVariables { get; private set; }

        /// <summary>
        ///   Gets the left hand side of 
        ///   the constraint equation.
        /// </summary>
        /// 
        public Func<double[], double> Function
        {
            get;
            private set;
        }

        /// <summary>
        ///   Gets the gradient of the left hand
        ///   side of the constraint equation.
        /// </summary>
        /// 
        public Func<double[], double[]> Gradient
        {
            get;
            private set;
        }

        /// <summary>
        ///   Gets how much the constraint is being violated.
        /// </summary>
        /// 
        /// <param name="x">The function point.</param>
        /// 
        /// <returns>How much the constraint is being violated at the given point.</returns>
        /// 
        public double GetViolation(double[] x)
        {
            return Function(x) - Value;
        }

        /// <summary>
        ///   Gets the type of the constraint.
        /// </summary>
        /// 
        public ConstraintType ShouldBe { get; private set; }

        /// <summary>
        ///   Gets the value in the right hand
        ///   side of the constraint equation.
        /// </summary>
        /// 
        public double Value { get; private set; }

        /// <summary>
        ///   Gets the violation tolerance for the constraint. Equality
        ///   constraints should set this to a small positive value.
        /// </summary>
        /// 
        public double Tolerance { get; set; }
#if !NET35
        /// <summary>
        ///   Constructs a new nonlinear constraint.
        /// </summary>
        /// 
        /// <param name="objective">The objective function to which this constraint refers.</param>
        /// 
        /// <param name="function">A lambda expression defining the left hand side of the constraint equation.</param>
        /// <param name="gradient">A lambda expression defining the gradient of the <paramref name="function">
        /// left hand side of the constraint equation</paramref>.</param>
        /// <param name="shouldBe">How the left hand side of the constraint should be compared to the given <paramref name="value"/>.</param>
        /// <param name="value">The right hand side of the constraint equation.</param>
        /// <param name="withinTolerance">The tolerance for violations of the constraint. Equality
        ///   constraints should set this to a small positive value. Default is 0.</param>
        /// 
        /// 
        public NonlinearConstraint(IObjectiveFunction objective,
            Expression<Func<double>> function, ConstraintType shouldBe, double value,
            Expression<Func<double[]>> gradient = null, double withinTolerance = 0.0)
        {
            this.NumberOfVariables = objective.NumberOfVariables;
            this.ShouldBe = shouldBe;

            // Generate lambda functions
            var func = ExpressionParser.Replace(function, objective.Variables);
            this.Function = func.Compile();
            this.Value = value;
            this.Tolerance = withinTolerance;

            if (gradient != null)
            {
                var grad = ExpressionParser.Replace(gradient, objective.Variables);
                this.Gradient = grad.Compile();

                int n = NumberOfVariables;
                double[] probe = new double[n];
                double[] g = Gradient(probe);
                if (g.Length != n) throw new DimensionMismatchException("gradient",
                    "The length of the gradient vector must match the number of variables in the objective function.");
            }
        }
#endif

        /// <summary>
        ///   Constructs a new nonlinear constraint.
        /// </summary>
        /// 
        /// <param name="objective">The objective function to which this constraint refers.</param>
        /// 
        /// <param name="function">A lambda expression defining the left hand side of the constraint equation.</param>
        /// <param name="gradient">A lambda expression defining the gradient of the <paramref name="function">
        /// left hand side of the constraint equation</paramref>.</param>
        /// <param name="shouldBe">How the left hand side of the constraint should be compared to the given <paramref name="value"/>.</param>
        /// <param name="value">The right hand side of the constraint equation.</param>
        /// <param name="withinTolerance">The tolerance for violations of the constraint. Equality
        ///   constraints should set this to a small positive value. Default is 0.</param>
        /// 
        public NonlinearConstraint(IObjectiveFunction objective,
            Func<double[], double> function, ConstraintType shouldBe, double value,
            Func<double[], double[]> gradient = null, double withinTolerance = 0.0)
        {
            int n = objective.NumberOfVariables;

            if (gradient != null)
            {
                double[] probe = new double[n];
                double[] g = gradient(probe);
                if (g.Length != n) throw new DimensionMismatchException("gradient",
                    "The length of the gradient vector must match the number of variables in the objective function.");
            }

            this.Create(objective, function, shouldBe, value, gradient, withinTolerance);
        }

        /// <summary>
        ///   Creates an empty nonlinear constraint.
        /// </summary>
        /// 
        protected NonlinearConstraint()
        {
        }

        /// <summary>
        ///    Creates a nonlinear constraint.
        /// </summary>
        /// 
        protected void Create(IObjectiveFunction objective,
            Func<double[], double> function, ConstraintType shouldBe, double value,
            Func<double[], double[]> gradient, double tolerance)
        {
            this.NumberOfVariables = objective.NumberOfVariables;
            this.ShouldBe = shouldBe;
            this.Value = value;
            this.Tolerance = tolerance;

            this.Function = function;
            this.Gradient = gradient;
        }
    }
}
