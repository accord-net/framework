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
    ///   Constraint type.
    /// </summary>
    /// 
    public enum ConstraintType
    {
        /// <summary>
        ///   Equality constraint.
        /// </summary>
        /// 
        EqualTo = 0,

        /// <summary>
        ///   Inequality constraint specifying a greater than or equal to relationship.
        /// </summary>
        /// 
        GreaterThanOrEqualTo,

        /// <summary>
        ///   Inequality constraint specifying a lesser than or equal to relationship.
        /// </summary>
        /// 
        LesserThanOrEqualTo,
    }

    /// <summary>
    ///   Constraint with only linear terms.
    /// </summary>
    /// 
    public class LinearConstraint
    {
        private int[] indices;
        private double[] scalars;

        /// <summary>
        ///   Gets the number of variables in the constraint.
        /// </summary>
        /// 
        public int NumberOfVariables { get; private set; }

        /// <summary>
        ///   Gets the index of the variables (in respective to the
        ///   object function index) of the variables participating
        ///   in this constraint.
        /// </summary>
        /// 
        public int[] VariablesAtIndices
        {
            get { return indices; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.Length != NumberOfVariables)
                    throw new DimensionMismatchException("value");

                this.indices = value;
            }
        }

        /// <summary>
        ///   Gets the scalar coefficients combining the <see cref="VariablesAtIndices">
        ///   variables</see> specified by the constraints.
        /// </summary>
        public double[] CombinedAs
        {
            get { return scalars; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.Length != NumberOfVariables)
                    throw new DimensionMismatchException("value");

                this.scalars = value;
            }
        }

        /// <summary>
        ///   Gets the type of the constraint.
        /// </summary>
        /// 
        public ConstraintType ShouldBe { get; set; }

        /// <summary>
        ///   Gets the value to be compared to the combined values
        ///   of the variables.
        /// </summary>
        /// 
        public double Value { get; set; }

        /// <summary>
        ///   Constructs a new linear constraint.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of variables in the constraint.</param>
        /// 
        public LinearConstraint(int numberOfVariables)
        {
            this.NumberOfVariables = numberOfVariables;
            this.indices = Matrix.Indices(0, numberOfVariables);
            this.scalars = Matrix.Vector(numberOfVariables, 1.0);
            this.ShouldBe = ConstraintType.GreaterThanOrEqualTo;
        }

        /// <summary>
        ///   Constructs a new linear constraint.
        /// </summary>
        /// 
        /// <param name="coefficients">The scalar coefficients specifying 
        /// how variables should be combined in the constraint.</param>
        /// 
        public LinearConstraint(params double[] coefficients)
        {
            this.NumberOfVariables = coefficients.Length;
            this.indices = Matrix.Indices(0, coefficients.Length);
            this.CombinedAs = coefficients;
            this.ShouldBe = ConstraintType.GreaterThanOrEqualTo;
        }

        /// <summary>
        ///   Constructs a new linear constraint.
        /// </summary>
        /// 
        /// <param name="function">The objective function to which
        ///   this constraint refers to.</param>
        /// <param name="constraint">A <see cref="System.String"/> 
        ///   specifying this constraint, such as "ax + b = c".</param>
        /// 
        /// <remarks>
        ///   The constraint string is always parsed using 
        ///   <see cref="System.Globalization.CultureInfo.InvariantCulture"/>.
        ///   This means numbers should be written using the English format, 
        ///   using the dot (.) as the decimal separator.
        /// </remarks>
        /// 
        public LinearConstraint(IObjectiveFunction function, string constraint)
        {
            parseString(function, constraint);
        }

        /// <summary>
        ///   Constructs a new linear constraint.
        /// </summary>
        /// 
        /// <param name="function">The objective function to which this 
        ///   constraint refers to.</param>
        /// <param name="constraint">A <see cref="Expression{T}"/> specifying
        ///   this constraint in the form of a lambda expression.</param>
        /// 
        public LinearConstraint(IObjectiveFunction function, Expression<Func<bool>> constraint)
        {
            parseExpression(function, constraint);
        }

        /// <summary>
        ///   Attempts to create a <see cref="LinearConstraint"/>
        ///   from a <see cref="System.String"/> representation.
        /// </summary>
        /// 
        /// <param name="str">The string containing the constraint in textual form.</param>
        /// <param name="function">The objective function to which this constraint refers to.</param>
        /// <param name="constraint">The resulting constraint, if it could be parsed.</param>
        /// 
        /// <returns><c>true</c> if the function could be parsed
        ///   from the string, <c>false</c> otherwise.</returns>
        /// 
        public static bool TryParse(string str, IObjectiveFunction function,
            out LinearConstraint constraint)
        {
            // TODO: implement this method without the try-catch block.

            try
            {
                constraint = new LinearConstraint(function, str);
            }
            catch (FormatException)
            {
                constraint = null;
                return false;
            }

            return true;
        }

        private void parseString(IObjectiveFunction function, string constraint)
        {
            if (String.IsNullOrEmpty(constraint))
                throw new FormatException("Constraint is empty.");

            string f = constraint.Replace("*", String.Empty).Replace(" ", String.Empty);

            if (f[0] != '-' || f[0] != '+')
                f = f.Insert(0, "+");

            ConstraintType type;

            string lhs, rhs;

            if (f.Contains(">="))
                type = ConstraintType.GreaterThanOrEqualTo;
            else if (f.Contains("<="))
                type = ConstraintType.LesserThanOrEqualTo;
            else if (f.Contains("="))
                type = ConstraintType.EqualTo;
            else
                throw new FormatException("Invalid constraint type.");



            var terms = new Dictionary<string, double>();

            Regex r = new Regex(@"[\-\+][\s]*(\d*\.{0,1}\d+)?[\s]*[a-zA-Z]");
            Regex number = new Regex(@"\d*\.{0,1}\d+");
            Regex symbol = new Regex(@"[a-zA-Z]");
            Regex comp = new Regex(@"(>=|<=|=)");


            var sides = comp.Split(f);
            lhs = sides[0];
            rhs = sides[2];

            double value = Double.Parse(rhs, System.Globalization.CultureInfo.InvariantCulture);

            MatchCollection matches = r.Matches(lhs, 0);

            foreach (Match m in matches)
            {
                string term = m.Value;

                double scalar = (term[0] == '-') ? -1 : 1;

                // Extract value
                MatchCollection coeff = number.Matches(term);

                foreach (Match c in coeff)
                    scalar *= Double.Parse(c.Value, System.Globalization.CultureInfo.InvariantCulture);

                // Extract symbols
                MatchCollection symbols = symbol.Matches(term);

                if (symbols.Count == 1)
                    terms.Add(symbols[0].Value, scalar);
                else
                    throw new FormatException("Unexpected expression.");
            }

            List<int> indices = new List<int>();
            List<double> scalars = new List<double>();

            foreach (var term in terms)
            {
                indices.Add(function.Variables[term.Key]);
                scalars.Add(term.Value);
            }

            NumberOfVariables = indices.Count;
            VariablesAtIndices = indices.ToArray();
            CombinedAs = scalars.ToArray();
            ShouldBe = type;
            Value = value;
        }

        private void parseExpression(IObjectiveFunction function, Expression<Func<bool>> constraint)
        {
            ConstraintType type;

            switch (constraint.Body.NodeType)
            {
                case ExpressionType.Equal:
                    type = ConstraintType.EqualTo; break;

                case ExpressionType.LessThanOrEqual:
                    type = ConstraintType.LesserThanOrEqualTo; break;

                case ExpressionType.GreaterThanOrEqual:
                    type = ConstraintType.GreaterThanOrEqualTo; break;

                default:
                    throw new FormatException("Unexpected expression.");
            }

            BinaryExpression b = constraint.Body as BinaryExpression;
            var terms = new Dictionary<string, double>();
            parse(terms, b.Left);

            ConstantExpression c = b.Right as ConstantExpression;
            double value = (double)c.Value;

            List<int> indices = new List<int>();
            List<double> scalars = new List<double>();

            foreach (var term in terms)
            {
                indices.Add(function.Variables[term.Key]);
                scalars.Add(term.Value);
            }

            NumberOfVariables = indices.Count;
            VariablesAtIndices = indices.ToArray();
            CombinedAs = scalars.ToArray();
            ShouldBe = type;
            Value = value;
        }

        private static string parse(Dictionary<string, double> terms, Expression expr)
        {
            if (expr == null)
                return null;

            BinaryExpression eb = expr as BinaryExpression;
            MemberExpression em = expr as MemberExpression;
            UnaryExpression eu = expr as UnaryExpression;

            if (em != null) // member expression
            {
                string term = em.Member.Name;
                terms[term] = 1;
                return term;
            }
            else if (eb != null) // binary expression
            {
                if (expr.NodeType == ExpressionType.Multiply)
                {
                    // This could be either a constant times and expression or a quadratic member
                    ConstantExpression a = eb.Left as ConstantExpression ?? eb.Right as ConstantExpression;
                    MemberExpression m = eb.Left as MemberExpression ?? eb.Right as MemberExpression;
                    UnaryExpression u = eb.Left as UnaryExpression ?? eb.Right as UnaryExpression;

                    if (a != null)
                    {
                        // This is a constant times an expression
                        double scalar = (double)a.Value;

                        string term = parse(terms, (Expression)m ?? (Expression)u);
                        terms[term] = scalar;

                        return term;
                    }
                    else throw new FormatException("Unexpected expression.");
                }
                else if (expr.NodeType == ExpressionType.Add)
                {
                    // This could be an expression + term, a term + expression or an expression + expression
                    BinaryExpression lb = eb.Left as BinaryExpression;
                    MemberExpression lm = eb.Left as MemberExpression;
                    UnaryExpression lu = eb.Left as UnaryExpression;

                    BinaryExpression rb = eb.Right as BinaryExpression;
                    MemberExpression rm = eb.Right as MemberExpression;

                    if (lb != null)
                        parse(terms, lb);
                    else if (lm != null)
                        terms[lm.Member.Name] = 1;
                    else if (lu != null)
                        parse(terms, lu);
                    else throw new FormatException("Unexpected expression.");

                    if (rb != null)
                        parse(terms, rb);
                    else if (rm != null)
                        terms[lm.Member.Name] = 1;
                    else throw new FormatException("Unexpected expression.");
                }
                else if (expr.NodeType == ExpressionType.Subtract)
                {
                    // This could be an expression - term, a term - expression or an expression - expression
                    BinaryExpression lb = eb.Left as BinaryExpression;
                    MemberExpression lm = eb.Left as MemberExpression;
                    UnaryExpression lu = eb.Left as UnaryExpression;

                    BinaryExpression rb = eb.Right as BinaryExpression;
                    MemberExpression rm = eb.Right as MemberExpression;
                    UnaryExpression ru = eb.Right as UnaryExpression;

                    if (lb != null)
                        parse(terms, lb);
                    else if (lm != null)
                        terms[lm.Member.Name] = 1;
                    else if (lu != null)
                        parse(terms, lu);
                    else throw new FormatException("Unexpected expression.");

                    if (rb != null)
                    {
                        var term = parse(terms, rb);
                        terms[term] = -terms[term];
                    }
                    else if (rm != null)
                        terms[rm.Member.Name] = -1;
                    else throw new FormatException("Unexpected expression.");
                }
            }
            else if (eu != null) // unary expression
            {
                if (expr.NodeType == ExpressionType.UnaryPlus)
                {
                    BinaryExpression lb = eu.Operand as BinaryExpression;
                    MemberExpression lm = eu.Operand as MemberExpression;

                    if (lm != null)
                        terms[lm.Member.Name] = 1;
                    else if (lb != null)
                        parse(terms, lb);
                    else throw new FormatException("Unexpected expression.");
                }
                else if (expr.NodeType == ExpressionType.Negate)
                {
                    BinaryExpression lb = eu.Operand as BinaryExpression;
                    MemberExpression lm = eu.Operand as MemberExpression;

                    if (lm != null)
                        terms[lm.Member.Name] = -1;
                    else if (lb != null)
                    {
                        var term = parse(terms, lb);
                        terms[term] = -terms[term];
                    }
                    else throw new FormatException("Unexpected expression.");
                }
                else throw new FormatException("Unexpected expression.");
            }
            else throw new FormatException("Unexpected expression.");

            return null;
        }

    }
}
