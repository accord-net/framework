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
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;

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
    /// <example>
    /// <para>
    ///   Linear constraints are commonly used in optimisation routines.
    ///   The framework provides support for linear constraints to be specified
    ///   using a <see cref="string"/> representation, an <see cref="Expression"/> 
    ///   or using a vector of constraint values.
    /// </para>
    /// 
    /// <code source="Unit Tests\Accord.Tests.Math\Optimization\LinearConstraintTest.cs" region="doc_example" />
    /// 
    /// </example>
    /// 
    /// <seealso cref="LinearConstraintCollection"/>
    ///

    public class LinearConstraint : IConstraint
    {
        /// <summary>
        ///   Gets the default constant violation tolerance (1e-12).
        /// </summary>
        /// 
        public const double DefaultTolerance = 1e-12;

        private int[] indices;
        private double[] combinedAs;
        private double[] grad;

        /// <summary>
        ///   Gets the number of variables in the constraint.
        /// </summary>
        /// 
        public int NumberOfVariables { get; private set; }

        /// <summary>
        ///   Gets the index of the variables (in respective to the
        ///   objective function) of the variables participating
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
                this.grad = null;
            }
        }

        /// <summary>
        ///   Gets the scalar coefficients combining the <see cref="VariablesAtIndices">
        ///   variables</see> specified by the constraints.
        /// </summary>
        public double[] CombinedAs
        {
            get { return combinedAs; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.Length != NumberOfVariables)
                    throw new DimensionMismatchException("value");

                this.combinedAs = value;
                this.grad = null;
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
        ///   Gets the violation tolerance for the constraint. Equality
        ///   constraints should set this to a small positive value.
        /// </summary>
        /// 
        public double Tolerance { get; set; }

        private LinearConstraint()
        {
            this.Tolerance = DefaultTolerance;
        }

        /// <summary>
        ///   Constructs a new linear constraint.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of variables in the constraint.</param>
        /// 
        public LinearConstraint(int numberOfVariables)
            : this()
        {
            this.NumberOfVariables = numberOfVariables;
            this.indices = Vector.Range(numberOfVariables);
            this.combinedAs = Vector.Ones(numberOfVariables);
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
            : this()
        {
            this.NumberOfVariables = coefficients.Length;
            this.indices = Vector.Range(0, coefficients.Length);
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
        /// <param name="format">The culture information specifying how
        ///   numbers written in the <paramref name="constraint"/> should
        ///   be parsed. Default is CultureInfo.InvariantCulture.</param>
        /// 
        public LinearConstraint(IObjectiveFunction function, string constraint, CultureInfo format)
            : this()
        {
            parseString(function, constraint, format);
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
        public LinearConstraint(IObjectiveFunction function, string constraint)
            : this(function, constraint, CultureInfo.InvariantCulture)
        {
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
            : this()
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
        public static bool TryParse(string str,
            IObjectiveFunction function, out LinearConstraint constraint)
        {
            return TryParse(str, CultureInfo.InvariantCulture, function, out constraint);
        }

        /// <summary>
        ///   Attempts to create a <see cref="LinearConstraint"/>
        ///   from a <see cref="System.String"/> representation.
        /// </summary>
        /// 
        /// <param name="str">The string containing the constraint in textual form.</param>
        /// <param name="function">The objective function to which this constraint refers to.</param>
        /// <param name="constraint">The resulting constraint, if it could be parsed.</param>
        /// <param name="culture">The culture information specifying how
        ///   numbers written in the <paramref name="constraint"/> should
        ///   be parsed. Default is CultureInfo.InvariantCulture.</param>
        /// 
        /// <returns><c>true</c> if the function could be parsed
        ///   from the string, <c>false</c> otherwise.</returns>
        /// 
        public static bool TryParse(string str, CultureInfo culture,
            IObjectiveFunction function, out LinearConstraint constraint)
        {
            // TODO: implement this method without the try-catch block.

            try
            {
                constraint = new LinearConstraint(function, str, culture);
            }
            catch (FormatException)
            {
                constraint = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates the left hand side of the constraint
        /// equation given a vector x.
        /// </summary>
        /// <param name="x">The vector.</param>
        /// <returns>
        /// The left hand side of the constraint equation as evaluated at x.
        /// </returns>
        public double Function(double[] x)
        {
            double sum = 0;

            for (int i = 0; i < indices.Length; i++)
            {
                int index = indices[i];
                double val = x[index];
                double a = CombinedAs[i];

                sum += val * a;
            }

            return sum;
        }

        /// <summary>
        /// Calculates the gradient of the constraint.
        /// </summary>
        /// <param name="x">The vector.</param>
        /// <returns>The gradient of the constraint.</returns>
        public double[] Gradient(double[] x)
        {
            if (grad == null)
            {
                if (x.Length == indices.Length && indices.IsEqual(Vector.Range(x.Length)))
                {
                    grad = combinedAs;
                }
                else
                {
                    var tmp = new double[x.Length];

                    for (int i = 0; i < indices.Length; i++)
                    {
                        int index = indices[i];
                        tmp[index] = CombinedAs[i];
                    }

                    grad = tmp;
                }
                
            }

            return grad;
        }

        private void parseString(IObjectiveFunction function, string constraint, CultureInfo culture)
        {
            if (String.IsNullOrEmpty(constraint))
                throw new FormatException("Constraint is empty.");

            string f = constraint.Replace("*", String.Empty).Replace(" ", String.Empty);

            if (f[0] != '-' && f[0] != '+')
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

            string separator = culture.NumberFormat.NumberDecimalSeparator;

            Regex r = new Regex(@"[\-\+][\s]*(\d*\" + separator + @"{0,1}\d+)?[\s]*([a-zA-Z])*");
            Regex number = new Regex(@"\d*\" + separator + @"{0,1}\d+");
            Regex symbol = new Regex(@"[a-zA-Z]");
            Regex comp = new Regex(@"(>=|<=|=)");


            var sides = comp.Split(f);
            lhs = sides[0];
            rhs = sides[2];

            double value = Double.Parse(rhs, culture);

            MatchCollection matches = r.Matches(lhs, 0);

            foreach (Match m in matches)
            {
                string term = m.Value;

                double scalar = (term[0] == '-') ? -1 : 1;

                // Extract value
                MatchCollection coeff = number.Matches(term);

                foreach (Match c in coeff)
                    scalar *= Double.Parse(c.Value, culture);

                // Extract symbols
                MatchCollection symbols = symbol.Matches(term);

                if (symbols.Count == 1)
                {
                    terms.Add(symbols[0].Value, scalar);
                }
                else
                {
                    if (coeff.Count == 1)
                        value -= scalar;
                    else if (term != "+")
                        throw new FormatException("Unexpected expression.");
                }
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
            double value = 0;
            parse(terms, b.Left, ref value);

            ConstantExpression c = b.Right as ConstantExpression;
            value = (double)c.Value - value;

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

        private static string parse(Dictionary<string, double> terms, Expression expr, ref double value)
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

                        string term = parse(terms, (Expression)m ?? (Expression)u, ref value);
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
                    ConstantExpression lc = eb.Left as ConstantExpression;

                    BinaryExpression rb = eb.Right as BinaryExpression;
                    MemberExpression rm = eb.Right as MemberExpression;
                    ConstantExpression rc = eb.Right as ConstantExpression;

                    if (lb != null)
                    {
                        parse(terms, lb, ref value);
                    }
                    else if (lm != null)
                    {
                        terms[lm.Member.Name] = 1;
                    }
                    else if (lu != null)
                    {
                        parse(terms, lu, ref value);
                    }
                    else if (lc != null)
                    {
                        value += (double)lc.Value;
                    }
                    else throw new FormatException("Unexpected expression.");

                    if (rb != null)
                    {
                        parse(terms, rb, ref value);
                    }
                    else if (rm != null)
                    {
                        terms[rm.Member.Name] = 1;
                    }
                    else if (rc != null)
                    {
                        value += (double)rc.Value;
                    }
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
                    ConstantExpression rc = eb.Right as ConstantExpression;

                    if (lb != null)
                        parse(terms, lb, ref value);
                    else if (lm != null)
                        terms[lm.Member.Name] = 1;
                    else if (lu != null)
                        parse(terms, lu, ref value);
                    else throw new FormatException("Unexpected expression.");

                    if (rb != null)
                    {
                        var term = parse(terms, rb, ref value);
                        terms[term] = -terms[term];
                    }
                    else if (rm != null)
                    {
                        terms[rm.Member.Name] = -1;
                    }
                    else if (rc != null)
                    {
                        value -= (double)rc.Value;
                    }
                    else throw new FormatException("Unexpected expression.");
                }
            }
            else if (eu != null) // unary expression
            {
                if (expr.NodeType == ExpressionType.UnaryPlus)
                {
                    BinaryExpression lb = eu.Operand as BinaryExpression;
                    MemberExpression lm = eu.Operand as MemberExpression;
                    ConstantExpression lc = eu.Operand as ConstantExpression;

                    if (lm != null)
                    {
                        terms[lm.Member.Name] = 1;
                    }
                    else if (lb != null)
                    {
                        parse(terms, lb, ref value);
                    }
                    else if (lc != null)
                    {
                        value += (double)lc.Value;
                    }
                    else throw new FormatException("Unexpected expression.");
                }
                else if (expr.NodeType == ExpressionType.Negate)
                {
                    BinaryExpression lb = eu.Operand as BinaryExpression;
                    MemberExpression lm = eu.Operand as MemberExpression;
                    ConstantExpression lc = eu.Operand as ConstantExpression;

                    if (lm != null)
                    {
                        terms[lm.Member.Name] = -1;
                    }
                    else if (lb != null)
                    {
                        var term = parse(terms, lb, ref value);
                        terms[term] = -terms[term];
                    }
                    else if (lc != null)
                    {
                        value -= (double)lc.Value;
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
