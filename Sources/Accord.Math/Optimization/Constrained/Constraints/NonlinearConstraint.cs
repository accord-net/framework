// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;
    using System.Text;
    using System.Collections.ObjectModel;

    /// <summary>
    ///   Constraint with only linear terms.
    /// </summary>
    /// 
    public class NonlinearConstraint : IConstraint, IFormattable
    {

        private const double DEFAULT_TOL = 1e-8;

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
        public Func<double[], double> Function { get; private set; }

        /// <summary>
        ///   Gets the gradient of the left hand
        ///   side of the constraint equation.
        /// </summary>
        /// 
        public Func<double[], double[]> Gradient { get; private set; }

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
        public double GetViolation(double[] input)
        {
            double fx = Function(input);

            switch (ShouldBe)
            {
                case ConstraintType.EqualTo:
                    return Math.Abs(fx - Value);

                case ConstraintType.GreaterThanOrEqualTo:
                    return fx - Value;

                case ConstraintType.LesserThanOrEqualTo:
                    return Value - fx;
            }

            throw new NotSupportedException();
        }

        /// <summary>
        ///   Gets whether this constraint is being violated 
        ///   (within the current tolerance threshold).
        /// </summary>
        /// 
        /// <param name="input">The function point.</param>
        /// 
        /// <returns>True if the constraint is being violated, false otherwise.</returns>
        /// 
        public bool IsViolated(double[] input)
        {
            return GetViolation(input) < -Tolerance;
        }

        /// <summary>
        ///   Gets the type of the constraint.
        /// </summary>
        /// 
        public ConstraintType ShouldBe { get; private set; }

        /// <summary>
        ///   Gets the value in the right hand side of 
        ///   the constraint equation. Default is 0.
        /// </summary>
        /// 
        public double Value { get; private set; }

        /// <summary>
        ///   Gets the violation tolerance for the constraint. Equality
        ///   constraints should set this to a small positive value. 
        ///   Default is 1e-8.
        /// </summary>
        /// 
        public double Tolerance { get; set; }

#if !NET35
        /// <summary>
        ///   Constructs a new nonlinear constraint.
        /// </summary>
        /// 
        /// <param name="objective">The objective function to which this constraint refers.</param>
        /// <param name="function">A lambda expression defining the left hand side of the constraint equation.</param>
        /// <param name="gradient">A lambda expression defining the gradient of the <paramref name="function">
        ///   left hand side of the constraint equation</paramref>.</param>
        /// <param name="shouldBe">How the left hand side of the constraint 
        ///   should be compared to the given <paramref name="value"/>.</param>
        /// <param name="value">The right hand side of the constraint equation.</param>
        /// <param name="withinTolerance">The tolerance for violations of the constraint. Equality
        ///   constraints should set this to a small positive value. Default is 1e-8.</param>
        /// 
        public NonlinearConstraint(IObjectiveFunction objective,
            Expression<Func<double>> function, ConstraintType shouldBe, double value,
            Expression<Func<double[]>> gradient = null, double withinTolerance = DEFAULT_TOL)
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

                if (g.Length != n)
                {
                    throw new DimensionMismatchException("gradient",
                    "The length of the gradient vector must match the number of variables in the objective function.");
                }
            }
        }
#endif

        /// <summary>
        ///   Constructs a new nonlinear constraint.
        /// </summary>
        /// 
        /// <param name="objective">The objective function to which this constraint refers.</param>
        /// <param name="function">A lambda expression defining the left hand side of the 
        ///   constraint equation.</param>
        /// <param name="shouldBe">How the left hand side of the constraint should be 
        ///   compared to the given <paramref name="value"/>.</param>
        /// <param name="value">The right hand side of the constraint equation. Default is 0.</param>
        /// 
        public NonlinearConstraint(IObjectiveFunction objective,
            Func<double[], double> function, ConstraintType shouldBe, double value)
        {
            int n = objective.NumberOfVariables;

            this.Create(objective.NumberOfVariables, function, shouldBe, value, null, DEFAULT_TOL);
        }

        /// <summary>
        ///   Constructs a new nonlinear constraint.
        /// </summary>
        /// 
        /// <param name="objective">The objective function to which this constraint refers.</param>
        /// <param name="function">A lambda expression defining the left hand side of the 
        ///   constraint equation.</param>
        /// <param name="gradient">A lambda expression defining the gradient of the <paramref name="function">
        ///   left hand side of the constraint equation</paramref>.</param>
        /// <param name="shouldBe">How the left hand side of the constraint should be 
        ///   compared to the given <paramref name="value"/>.</param>
        /// <param name="value">The right hand side of the constraint equation. Default is 0.</param>
        /// 
        public NonlinearConstraint(IObjectiveFunction objective,
            Func<double[], double> function, ConstraintType shouldBe, double value,
            Func<double[], double[]> gradient)
        {
            int n = objective.NumberOfVariables;

            this.Create(objective.NumberOfVariables, function, shouldBe, value, gradient, DEFAULT_TOL);
        }

        /// <summary>
        ///   Constructs a new nonlinear constraint.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of variables in the constraint.</param>
        /// <param name="function">A lambda expression defining the left hand side of the constraint equation.</param>
        /// <param name="shouldBe">How the left hand side of the constraint should be compared to 
        ///   the given <paramref name="value"/>. Default is <see cref="ConstraintType.GreaterThanOrEqualTo"/>.</param>
        /// <param name="value">The right hand side of the constraint equation. Default is 0.</param>
        /// 
        public NonlinearConstraint(int numberOfVariables,
            Func<double[], double> function, ConstraintType shouldBe, double value)
        {
            this.Create(numberOfVariables, function, shouldBe, value, null, DEFAULT_TOL);
        }

        /// <summary>
        ///   Constructs a new nonlinear constraint.
        /// </summary>
        /// 
        /// <param name="objective">The objective function to which this constraint refers.</param>
        /// <param name="constraint">A boolean lambda expression expressing the constraint. Please 
        ///   see examples for details.</param>
        /// 
        public NonlinearConstraint(IObjectiveFunction objective, 
            Expression<Func<double[], bool>> constraint)
        {
            int n = objective.NumberOfVariables;

            Func<double[], double> function;
            ConstraintType shouldBe;
            double value;

            parse(constraint, out function, out shouldBe, out value);

            this.Create(objective.NumberOfVariables, function, shouldBe, value, null, DEFAULT_TOL);
        }

        /// <summary>
        ///   Constructs a new nonlinear constraint.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of variables in the constraint.</param>
        /// <param name="constraint">A boolean lambda expression expressing the constraint. Please 
        ///   see examples for details.</param>
        /// 
        public NonlinearConstraint(int numberOfVariables, Expression<Func<double[], bool>> constraint)
        {
            Func<double[], double> function;
            ConstraintType shouldBe;
            double value;

            parse(constraint, out function, out shouldBe, out value);

            this.Create(numberOfVariables, function, shouldBe, value, null, DEFAULT_TOL);
        }

        /// <summary>
        ///   Constructs a new nonlinear constraint.
        /// </summary>
        /// 
        /// <param name="objective">The objective function to which this constraint refers.</param>
        /// <param name="function">A lambda expression defining the left hand side of the constraint.</param>
        /// 
        public NonlinearConstraint(IObjectiveFunction objective, Func<double[], double> function)
        {
            int n = objective.NumberOfVariables;

            this.Create(objective.NumberOfVariables, function, ConstraintType.GreaterThanOrEqualTo, 0.0, null, DEFAULT_TOL);
        }

        /// <summary>
        ///   Constructs a new nonlinear constraint.
        /// </summary>
        /// 
        /// <param name="objective">The objective function to which this constraint refers.</param>
        /// <param name="function">A lambda expression defining the left hand side of the 
        ///   constraint equation.</param>
        /// <param name="gradient">A lambda expression defining the gradient of the <paramref name="function">
        ///   left hand side of the constraint equation.</paramref>.</param>
        /// 
        public NonlinearConstraint(IObjectiveFunction objective,
            Func<double[], double> function,
            Func<double[], double[]> gradient)
        {
            int n = objective.NumberOfVariables;

            this.Create(objective.NumberOfVariables, function,
                ConstraintType.GreaterThanOrEqualTo, 0.0, gradient, DEFAULT_TOL);
        }

        /// <summary>
        ///   Constructs a new nonlinear constraint.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of variables in the constraint.</param>
        /// <param name="function">A lambda expression defining the left hand side of the 
        ///   constraint equation.</param>
        /// <param name="gradient">A lambda expression defining the gradient of the <paramref name="function">
        ///   left hand side of the constraint equation</paramref>.</param>
        /// <param name="shouldBe">How the left hand side of the constraint should be 
        ///   compared to the given <paramref name="value"/>.</param>
        /// <param name="value">The right hand side of the constraint equation. Default is 0.</param>
        /// <param name="withinTolerance">The tolerance for violations of the constraint. Equality
        ///   constraints should set this to a small positive value. Default is 1e-8.</param>
        /// 
        public NonlinearConstraint(int numberOfVariables,
            Func<double[], double> function,
            Func<double[], double[]> gradient,
            ConstraintType shouldBe = ConstraintType.GreaterThanOrEqualTo,
            double value = 0, double withinTolerance = DEFAULT_TOL)
        {
            this.Create(numberOfVariables, function,
                shouldBe, value, gradient, withinTolerance);
        }

        /// <summary>
        ///   Constructs a new nonlinear constraint.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of variables in the constraint.</param>
        /// <param name="function">A lambda expression defining the left hand side of the 
        ///   constraint equation.</param>
        ///   
        public NonlinearConstraint(int numberOfVariables, Func<double[], double> function)
        {
            this.Create(numberOfVariables, function, ConstraintType.GreaterThanOrEqualTo, 0.0, null, DEFAULT_TOL);
        }

        /// <summary>
        ///   Constructs a new nonlinear constraint.
        /// </summary>
        /// 
        /// <param name="objective">The objective function to which this constraint refers.</param>
        /// <param name="function">A lambda expression defining the left hand side of the constraint equation.</param>
        /// <param name="gradient">A lambda expression defining the gradient of the <paramref name="function">
        /// left hand side of the constraint equation</paramref>.</param>
        /// <param name="shouldBe">How the left hand side of the constraint should be compared to the given <paramref name="value"/>.</param>
        /// <param name="value">The right hand side of the constraint equation. Default is 0.</param>
        /// <param name="withinTolerance">The tolerance for violations of the constraint. Equality
        ///   constraints should set this to a small positive value. Default is 1e-8.</param>
        /// 
        public NonlinearConstraint(IObjectiveFunction objective,
            Func<double[], double> function, ConstraintType shouldBe, double value,
            Func<double[], double[]> gradient, double withinTolerance = DEFAULT_TOL)
        {
            this.Create(objective.NumberOfVariables, function, shouldBe, value, gradient, withinTolerance);
        }


        /// <summary>
        ///   Constructs a new nonlinear constraint.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of variables in the constraint.</param>
        /// <param name="function">A lambda expression defining the left hand side of the constraint equation.</param>
        /// <param name="gradient">A lambda expression defining the gradient of the <paramref name="function">
        /// left hand side of the constraint equation</paramref>.</param>
        /// <param name="shouldBe">How the left hand side of the constraint should be compared to the given <paramref name="value"/>.</param>
        /// <param name="value">The right hand side of the constraint equation. Default is 0.</param>
        /// <param name="withinTolerance">The tolerance for violations of the constraint. Equality
        ///   constraints should set this to a small positive value. Default is 1e-8.</param>
        /// 
        public NonlinearConstraint(int numberOfVariables,
            Func<double[], double> function, ConstraintType shouldBe, double value,
            Func<double[], double[]> gradient, double withinTolerance = DEFAULT_TOL)
        {
            this.Create(numberOfVariables, function, shouldBe, value, gradient, withinTolerance);
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
        protected void Create(int numberOfVariables,
            Func<double[], double> function, ConstraintType shouldBe, double value,
            Func<double[], double[]> gradient, double tolerance)
        {

            if (gradient != null)
            {
                double[] probe = new double[numberOfVariables];
                double[] g = gradient(probe);

                if (g.Length != numberOfVariables)
                    throw new DimensionMismatchException("gradient",
                    "The length of the gradient vector must match the number of variables in the objective function.");
            }

            this.NumberOfVariables = numberOfVariables;
            this.ShouldBe = shouldBe;
            this.Value = value;
            this.Tolerance = tolerance;

            this.Function = function;
            this.Gradient = gradient;
        }



        private static void parse(Expression<Func<double[], bool>> constraint,
            out Func<double[], double> function, out ConstraintType shouldBe, out double value)
        {
            var expression = constraint.Body as BinaryExpression;

            var comparisonType = expression.NodeType;

            switch (comparisonType)
            {
                case ExpressionType.LessThanOrEqual:
                    shouldBe = ConstraintType.LesserThanOrEqualTo;
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    shouldBe = ConstraintType.GreaterThanOrEqualTo;
                    break;

                case ExpressionType.Equal:
                    shouldBe = ConstraintType.EqualTo;
                    break;

                default:
                    throw new NotSupportedException(comparisonType + " is not supported.");
            }

            var left = expression.Left;
            var right = expression.Right as ConstantExpression;

            var functionExpression = Expression.Lambda(left, constraint.Parameters.ToArray());

            function = functionExpression.Compile() as Func<double[], double>;

            value = (Double)right.Value;
        }


        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            return ToString("G", System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public string ToString(string format, IFormatProvider formatProvider)
        {
            string v = Value.ToString(format, formatProvider);
            string t = Tolerance.ToString(format, formatProvider);

            switch (ShouldBe)
            {
                case ConstraintType.EqualTo:
                    return String.Format(formatProvider, "g(x) == {0} (± {1})", v, t);

                case ConstraintType.GreaterThanOrEqualTo:
                    return String.Format(formatProvider, "g(x) >= {0} (± {1})", v, t);

                case ConstraintType.LesserThanOrEqualTo:
                    return String.Format("g(x) <= {0} (± {1})", v, t);
            }

            return "g(x)";
        }
    }
}
