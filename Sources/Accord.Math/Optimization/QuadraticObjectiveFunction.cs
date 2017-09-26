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
    using System.Text;
    using Accord.Compat;
    using System.Linq;

    /// <summary>
    ///   Quadratic objective function.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In mathematics, a quadratic function, a quadratic polynomial, a polynomial 
    ///   of degree 2, or simply a quadratic, is a polynomial function in one or more 
    ///   variables in which the highest-degree term is of the second degree. For example,
    ///   a quadratic function in three variables x, y, and z contains exclusively terms
    ///   x², y², z², xy, xz, yz, x, y, z, and a constant:
    /// </para>
    /// 
    /// <code>
    ///   f(x,y,z) = ax² + by² +cz² + dxy + exz + fyz + gx + hy + iz + j
    /// </code>
    /// 
    /// <para>
    ///   Please note that the function's constructor expects the function
    ///   expression to be given on this form. Scalar values must be located
    ///   on the left of the variables, and no term should be duplicated in
    ///   the quadratic expression. Please take a look on the examples section
    ///   of this page for some examples of expected functions.</para>
    /// 
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Quadratic_function">
    ///       Wikipedia, The Free Encyclopedia. Quadratic Function. Available on:
    ///       https://en.wikipedia.org/wiki/Quadratic_function </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// 
    /// <example>
    /// <para>
    ///   Examples of valid quadratic functions are:</para>
    ///   
    /// <code>
    ///   var f1 = new QuadraticObjectiveFunction("x² + 1");
    ///   var f2 = new QuadraticObjectiveFunction("-x*y + y*z");
    ///   var f3 = new QuadraticObjectiveFunction("-2x² + xy - y² - 10xz + z²");
    ///   var f4 = new QuadraticObjectiveFunction("-2x² + xy - y² + 5y");
    /// </code>
    /// 
    /// <para>
    ///   It is also possible to specify quadratic functions using lambda expressions.
    ///   In this case, it is first necessary to create some dummy symbol variables to
    ///   act as placeholders in the quadratic expressions. Their value is not important,
    ///   as they will only be used to parse the form of the expression, not its value.
    /// </para>
    /// 
    /// <code>
    ///   // Declare symbol variables
    ///   double x = 0, y = 0, z = 0;
    /// 
    ///   var g1 = new QuadraticObjectiveFunction(() => x * x + 1);
    ///   var g2 = new QuadraticObjectiveFunction(() => -x * y + y * z);
    ///   var g3 = new QuadraticObjectiveFunction(() => -2 * x * x + x * y - y * y - 10 * x * z + z * z);
    ///   var g4 = new QuadraticObjectiveFunction(() => -2 * x * x + x * y - y * y + 5 * y);
    /// </code>
    /// 
    /// <para>
    ///   Finally, for large problems, it is usually best to declare quadratic
    ///   problems using matrices and vectors. Without loss of generality, the
    ///   quadratic matrix can always be taken to be symmetric (as the anti-
    ///   symmetric part has no contribution to the function). For efficiency
    ///   reasons, the quadratic matrix must be symmetric.
    /// </para>
    /// 
    /// <code>
    ///   var h1 = new QuadraticObjectiveFunction("3x² - 4y² + 6xy + 3x + 2y");
    /// 
    ///   double[,] Q = { { 6, 6 }, { 6, -8 } }; // Note the factor of 2
    ///   double[] d = { 3, 2 };
    /// 
    ///   // Equivalently this can be written:
    ///   var h2 = new QuadraticObjectiveFunction(Q, d);
    /// </code>
    /// 
    /// <para>
    ///   After those functions are created, you can either query their values
    ///   using</para>
    ///   
    /// <code>
    ///   f1.Function(new [] { 5.0 }); // x*x+1 = x² + 1 = 25 + 1 = 26
    /// </code>
    /// 
    /// <para>
    ///   Or you can pass it to a quadratic optimization method such
    ///   as Goldfarb-Idnani to explore its minimum or maximal points:</para>
    /// 
    /// <code>
    ///   // Declare symbol variables
    ///   double x = 0, y = 0, z = 0;
    /// 
    ///   // Create the function to be optimized
    ///   var f = new QuadraticObjectiveFunction(() => x * x - 2 * x * y + 3 * y * y + z * z - 4 * x - 5 * y - z);
    /// 
    ///   // Create some constraints for the solution
    ///   var constraints = new List&lt;LinearConstraint>();
    ///   constraints.Add(new LinearConstraint(f, () => 6 * x - 7 * y &lt;= 8));
    ///   constraints.Add(new LinearConstraint(f, () => 9 * x + 1 * y &lt;= 11));
    ///   constraints.Add(new LinearConstraint(f, () => 9 * x - y &lt;= 11));
    ///   constraints.Add(new LinearConstraint(f, () => -z - y == 12));
    /// 
    ///   // Create the Quadratic Programming solver
    ///   GoldfarbIdnani solver = new GoldfarbIdnani(f, constraints);
    /// 
    ///   // Minimize the function
    ///   bool success = solver.Minimize();
    ///   
    ///   double value = solver.Value;
    ///   double[] solutions = solver.Solution;
    /// </code>
    /// 
    /// <para>
    ///   Sometimes it is easiest to compose quadratic functions as linear
    ///   combinations of other quadratic functions. The <see cref="QuadraticObjectiveFunction" />
    ///   supports this by overloading the addition and multiplication operators.
    /// </para>
    /// 
    /// <code source="Unit Tests\Accord.Tests.Math\Optimization\QuadraticObjectiveFunctionTest.cs" region="doc_example" />
    /// 
    /// </example>
    /// 
    /// <seealso cref="GoldfarbIdnani"/>
    /// 
    public class QuadraticObjectiveFunction : NonlinearObjectiveFunction, IObjectiveFunction
    {
        private double[,] Q;
        private double[] d;

        /// <summary>
        ///   Gets the quadratic terms of the quadratic function.
        /// </summary>
        /// 
        public double[,] QuadraticTerms { get { return Q; } }

        /// <summary>
        ///   Gets the vector of linear terms of the quadratic function.
        /// </summary>
        /// 
        public double[] LinearTerms { get { return d; } }

        /// <summary>
        ///   Gets the constant term in the quadratic function.
        /// </summary>
        /// 
        public double ConstantTerm { get; set; }

        /// <summary>
        ///   Creates a new objective function specified through a string.
        /// </summary>
        /// 
        /// <param name="quadraticTerms">A Hessian matrix of quadratic terms defining the quadratic objective function.</param>
        /// <param name="linearTerms">The vector of linear terms associated with <paramref name="quadraticTerms"/>.</param>
        /// <param name="variables">The name for each variable in the problem.</param>
        /// 
        public QuadraticObjectiveFunction(double[,] quadraticTerms, double[] linearTerms, params string[] variables)
        {
            if (!quadraticTerms.IsSquare())
                throw new DimensionMismatchException("quadraticTerms", "The matrix must be square.");

            if (!quadraticTerms.IsSymmetric(rtol: 1e-6))
                throw new NonSymmetricMatrixException("The matrix must be symmetric.");

            if (quadraticTerms.Rows() != linearTerms.Length)
                throw new DimensionMismatchException("linearTerms",
                    "The vector of linear terms must have the same length as the Hessian matrix side.");

            if (variables.Length == 0)
            {
                variables = new string[linearTerms.Length];
                for (int i = 0; i < variables.Length; i++)
                    variables[i] = i.ToVariable();
            }
            else if (variables.Length != linearTerms.Length)
            {
                throw new DimensionMismatchException("variables",
                    "The vector of variable names must have the same length as the vector of linear terms.");
            }

            for (int i = 0; i < variables.Length; i++)
            {
                string var = variables[i];
                this.InnerVariables[var] = i;
                this.InnerIndices[i] = var;
            }

            this.Q = quadraticTerms;
            this.d = linearTerms;
            base.NumberOfVariables = d.Length;

            this.Function = function;
            this.Gradient = gradient;
        }

        /// <summary>
        ///   Creates a new objective function specified through a string.
        /// </summary>
        /// 
        /// <param name="function">A <see cref="System.String"/> containing
        /// the function in the form similar to "ax²+b".</param>
        /// 
        public QuadraticObjectiveFunction(string function)
            : this(function, CultureInfo.InvariantCulture)
        {
        }

        /// <summary>
        ///   Creates a new objective function specified through a string.
        /// </summary>
        /// 
        /// <param name="function">A <see cref="System.String"/> containing
        ///   the function in the form similar to "ax²+b".</param>
        /// <param name="culture">The culture information specifying how
        ///   numbers written in the <paramref name="function"/> should
        ///   be parsed. Default is CultureInfo.InvariantCulture.</param>
        /// 
        public QuadraticObjectiveFunction(string function, CultureInfo culture)
        {
            var terms = QuadraticExpressionParser.ParseString(function, culture);

            initialize(terms);
        }

        /// <summary>
        ///   Creates a new objective function specified through a string.
        /// </summary>
        /// 
        /// <param name="function">A <see cref="Expression{T}"/> containing 
        /// the function in the form of a lambda expression.</param>
        /// 
        public QuadraticObjectiveFunction(Expression<Func<double>> function)
        {
            var terms = new Dictionary<Tuple<string, string>, double>();
            double scalar;
            QuadraticExpressionParser.ParseExpression(terms, function.Body, out scalar);

            initialize(terms);
        }

        private void initialize(Dictionary<Tuple<string, string>, double> terms)
        {
            var linear = new Dictionary<string, double>();
            var quadratic = new Dictionary<Tuple<string, string>, double>();

            var list = new SortedSet<string>();

            foreach (var term in terms)
            {
                if (term.Key.Item2 != null)
                {
                    list.Add(term.Key.Item1);
                    list.Add(term.Key.Item2);

                    quadratic.Add(term.Key, term.Value);
                }
                else if (term.Key.Item1 != null)
                {
                    list.Add(term.Key.Item1);

                    linear.Add(term.Key.Item1, term.Value);
                }
                else
                {
                    ConstantTerm = term.Value;
                }
            }

            int i = 0;
            foreach (var variable in list)
            {
                InnerVariables.Add(variable, i);
                InnerIndices.Add(i, variable);
                i++;
            }

            NumberOfVariables = Variables.Count;
            this.Q = createQuadraticTermsMatrix(quadratic);
            this.d = createLinearTermsVector(linear);

            this.Function = function;
            this.Gradient = gradient;
        }

        private double[,] createQuadraticTermsMatrix(Dictionary<Tuple<string, string>, double> quadratic)
        {
            int n = Variables.Count;

            double[,] Q = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                var x = Indices[i];
                for (int j = 0; j < n; j++)
                {
                    var y = Indices[j];
                    var k = Tuple.Create(x, y);

                    if (quadratic.ContainsKey(k))
                    {
                        double s = quadratic[k];
                        Q[i, j] += s;
                        Q[j, i] += s;
                    }
                }
            }

            return Q;
        }

        private double[] createLinearTermsVector(Dictionary<string, double> linear)
        {
            int n = Variables.Count;
            double[] d = new double[n];

            for (int i = 0; i < Indices.Count; i++)
            {
                var x = Indices[i];
                if (linear.ContainsKey(x))
                    d[i] += linear[x];
            }

            return d;
        }

        private double function(double[] input)
        {
            double a = 0.5 * input.DotAndDot(Q, input);
            double b = input.Dot(d);
            return a + b + ConstantTerm;
        }

        private double[] gradient(double[] input)
        {
            double[] g = Q.Dot(input);
            g.Add(d, g);

            return g;
        }

        #region Operator Overloads

        /// <summary>
        /// Multiplies each term in the <see cref="QuadraticObjectiveFunction"/> by the specified scalar.
        /// </summary>
        /// 
        /// <param name="scalar">Scalar to multiply the individual terms by.</param>
        /// <param name="a">A <see cref="QuadraticObjectiveFunction"/> instance.</param>
        /// 
        /// <returns>Returns a new <see cref="QuadraticObjectiveFunction"/> instance 
        /// with all terms multiplied by the specified scalar.</returns>
        ///
        public static QuadraticObjectiveFunction operator *(double scalar, QuadraticObjectiveFunction a)
        {
            double[] linearTerms = a.LinearTerms.Multiply(scalar);
            double[,] quadraticTerms = a.QuadraticTerms.Multiply(scalar);
            double constantTerm = a.ConstantTerm * scalar;

            string[] variables = new string[a.NumberOfVariables];
            for (int i = 0; i < variables.Length; i++)
                variables[i] = a.InnerIndices[i];

            return new QuadraticObjectiveFunction(quadraticTerms, linearTerms, variables)
            {
                ConstantTerm = constantTerm,
            };
        }

        /// <summary>
        /// Multiplies each term in the <see cref="QuadraticObjectiveFunction"/> by the specified scalar.
        /// </summary>
        /// 
        /// <param name="a">A <see cref="QuadraticObjectiveFunction"/> instance.</param>
        /// <param name="scalar">Scalar to multiply the individual terms by.</param>
        /// 
        /// <returns>Returns a new <see cref="QuadraticObjectiveFunction"/> instance 
        /// with all terms multiplied by the specified scalar.</returns>
        ///
        public static QuadraticObjectiveFunction operator *(QuadraticObjectiveFunction a, double scalar)
        {
            return scalar * a;
        }

        /// <summary>
        /// Divides each term in the <see cref="QuadraticObjectiveFunction"/> by the specified scalar.
        /// </summary>
        /// 
        /// <param name="a">A <see cref="QuadraticObjectiveFunction"/> instance.</param>
        /// <param name="scalar">Scalar to divide the individual terms by.</param>
        /// 
        /// <returns>Returns a new <see cref="QuadraticObjectiveFunction"/> instance 
        /// with all terms divided by the specified scalar.</returns>
        ///
        public static QuadraticObjectiveFunction operator /(QuadraticObjectiveFunction a, double scalar)
        {
            if (scalar == 0)
            {
                throw new DivideByZeroException("Cannot divide objective function by zero");
            }

            return a * (1 / scalar);
        }

        /// <summary>
        /// Negates the quadratic objective function.
        /// </summary>
        /// 
        /// <param name="a">A <see cref="QuadraticObjectiveFunction"/> instance.</param>
        /// 
        /// <returns>Returns a new <see cref="QuadraticObjectiveFunction"/> instance where 
        /// each term has been negated.</returns>
        /// 
        public static QuadraticObjectiveFunction operator -(QuadraticObjectiveFunction a)
        {
            return -1 * a;
        }

        /// <summary>
        /// Adds two quadratic objective functions together by linearly combining 
        /// the individual terms.
        /// </summary>
        /// 
        /// <param name="a">The first quadratic objective function.</param>
        /// <param name="b">The second quadratic objective function.</param>
        /// 
        /// <returns>Returns a new <see cref="QuadraticObjectiveFunction"/> where the resultant
        /// terms are the sum of the individuals.</returns>
        /// 
        public static QuadraticObjectiveFunction operator +(QuadraticObjectiveFunction a, QuadraticObjectiveFunction b)
        {
            if (a.NumberOfVariables != b.NumberOfVariables)
                throw new DimensionMismatchException("NumberOfVariables",
                    "The quadratic objective functions must have the same number of variables.");

            double[] linearTerms = a.LinearTerms.Add(b.LinearTerms);
            double[,] quadraticTerms = a.QuadraticTerms.Add(b.QuadraticTerms);
            double constantTerm = a.ConstantTerm + b.ConstantTerm;

            string[] variables;
            if (a.InnerIndices.All(kvp => kvp.Value == b.InnerIndices[kvp.Key]))
            {
                variables = new string[a.NumberOfVariables];
                for (int i = 0; i < variables.Length; i++)
                    variables[i] = a.InnerIndices[i];
            }
            else
            {
                variables = new string[0];
            }

            return new QuadraticObjectiveFunction(quadraticTerms, linearTerms, variables)
            {
                ConstantTerm = constantTerm,
            };
        }

        /// <summary>
        /// Subtracts a quadratic objective function together by linearly subtracting 
        /// the individual terms.
        /// </summary>
        /// 
        /// <param name="a">The first quadratic objective function.</param>
        /// <param name="b">The second quadratic objective function.</param>
        /// 
        /// <returns>Returns a new <see cref="QuadraticObjectiveFunction"/> where each of the resultant
        /// terms are taken to be the difference of the corresponding individual terms.</returns>
        /// 
        public static QuadraticObjectiveFunction operator -(QuadraticObjectiveFunction a, QuadraticObjectiveFunction b)
        {
            if (a.NumberOfVariables != b.NumberOfVariables)
                throw new DimensionMismatchException("NumberOfVariables",
                    "The quadratic objective functions must have the same number of variables.");

            double[] linearTerms = a.LinearTerms.Subtract(b.LinearTerms);
            double[,] quadraticTerms = a.QuadraticTerms.Subtract(b.QuadraticTerms);
            double constantTerm = a.ConstantTerm - b.ConstantTerm;

            string[] variables;
            if (a.InnerIndices.All(kvp => kvp.Value == b.InnerIndices[kvp.Key]))
            {
                variables = new string[a.NumberOfVariables];
                for (int i = 0; i < variables.Length; i++)
                    variables[i] = a.InnerIndices[i];
            }
            else
            {
                variables = new string[0];
            }

            return new QuadraticObjectiveFunction(quadraticTerms, linearTerms, variables)
            {
                ConstantTerm = constantTerm,
            };
        }

        #endregion

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            var simpleString = string.Format("{0}-dimensional quadratic objective function", NumberOfVariables);

            const int MaxTerms = 15;
            int terms = 0;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < NumberOfVariables; i++)
            {
                if (Q[i, i] == 0)
                    continue;

                if (++terms > MaxTerms)
                    return simpleString;

                sb.AppendFormat("{0:+#;-#}{1}² ", 0.5 * Q[i, i], InnerIndices[i]);
            }

            for (int i = 0; i < NumberOfVariables; i++)
            {
                for (int j = i + 1; j < NumberOfVariables; j++)
                {
                    if (Q[i, j] == 0)
                        continue;

                    if (++terms > MaxTerms)
                        return simpleString;

                    sb.AppendFormat("{0:+#;-#}{1}{2} ", Q[i, j], InnerIndices[i], InnerIndices[j]);
                }
            }

            for (int i = 0; i < NumberOfVariables; i++)
            {
                if (d[i] == 0)
                    continue;

                if (++terms > MaxTerms)
                    return simpleString;

                sb.AppendFormat("{0:+#;-#}{1} ", d[i], InnerIndices[i]);
            }


            if (ConstantTerm != 0)
                sb.AppendFormat("{0:+#;-#} ", ConstantTerm);

            if (sb.Length == 0)
                return "0";

            sb.Remove(sb.Length - 1, 1);

            if (sb[0] == '+')
            {
                sb.Remove(0, 1);
            }

            return sb.ToString();
        }

        /// <summary>
        ///   Attempts to create a <see cref="QuadraticObjectiveFunction"/>
        ///   from a <see cref="System.String"/> representation.
        /// </summary>
        /// 
        /// <param name="str">The string containing the function in textual form.</param>
        /// <param name="function">The resulting function, if it could be parsed.</param>
        /// 
        /// <returns><c>true</c> if the function could be parsed
        ///   from the string, <c>false</c> otherwise.</returns>
        /// 
        public static bool TryParse(string str, out QuadraticObjectiveFunction function)
        {
            return TryParse(str, CultureInfo.InvariantCulture, out function);
        }

        /// <summary>
        ///   Attempts to create a <see cref="QuadraticObjectiveFunction"/>
        ///   from a <see cref="System.String"/> representation.
        /// </summary>
        /// 
        /// <param name="str">The string containing the function in textual form.</param>
        /// <param name="function">The resulting function, if it could be parsed.</param>
        /// <param name="culture">The culture information specifying how
        ///   numbers written in the <paramref name="function"/> should
        ///   be parsed. Default is CultureInfo.InvariantCulture.</param>
        ///   
        /// <returns><c>true</c> if the function could be parsed
        ///   from the string, <c>false</c> otherwise.</returns>
        /// 
        public static bool TryParse(string str, CultureInfo culture, out QuadraticObjectiveFunction function)
        {
            // TODO: implement this method without the try-catch block.

            try
            {
                function = new QuadraticObjectiveFunction(str, culture);
            }
            catch (FormatException)
            {
                function = null;
                return false;
            }

            return true;
        }
    }
}
