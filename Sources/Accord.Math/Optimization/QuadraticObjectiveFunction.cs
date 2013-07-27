// Accord Math Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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
    ///   Quadractic objective function.
    /// </summary>
    /// 
    public class QuadraticObjectiveFunction : IObjectiveFunction
    {

        private Dictionary<string, double> linear;
        private Dictionary<Tuple<string, string>, double> quadratic;

        private Dictionary<string, int> variables;
        private Dictionary<int, string> indices;

        private double[,] Q;
        private double[] d;


        /// <summary>
        ///   Gets input variable's labels for the function.
        /// </summary>
        /// 
        public IDictionary<string, int> Variables
        {
            get { return new ReadOnlyDictionary<string, int>(variables); }
        }
        /// <summary>
        ///   Gets the index of each input variable in the function.
        /// </summary>
        /// 
        public IDictionary<int, string> Indices
        {
            get { return new ReadOnlyDictionary<int, string>(indices); }
        }

        /// <summary>
        ///   Gets the number of input variables for the function.
        /// </summary>
        /// 
        public int NumberOfVariables { get { return variables.Count; } }

        /// <summary>
        ///   Gets the objective function.
        /// </summary>
        /// 
        public Func<double[], double> Function
        {
            get { return function; }
        }

        /// <summary>
        ///   Creates a new objective function specified through a string.
        /// </summary>
        /// 
        /// <param name="f">A <see cref="System.String"/> containing the function in the form similar to "ax²+b".</param>
        /// 
        public QuadraticObjectiveFunction(string f)
        {
            var terms = parseString(f);

            initialize(terms);
        }

        /// <summary>
        ///   Creates a new objective function specified through a string.
        /// </summary>
        /// 
        /// <param name="f">A <see cref="Expression{T}"/> containing the function in the form of a lambda expression.</param>
        /// 
        public QuadraticObjectiveFunction(Expression<Func<double>> f)
        {
            var terms = new Dictionary<Tuple<string, string>, double>();
            double scalar;
            parseExpression(terms, f.Body, out scalar);

            initialize(terms);
        }

        private void initialize(Dictionary<Tuple<string, string>, double> terms)
        {
            variables = new Dictionary<string, int>();
            indices = new Dictionary<int, string>();

            linear = new Dictionary<string, double>();
            quadratic = new Dictionary<Tuple<string, string>, double>();


            var list = new SortedSet<string>();

            foreach (var term in terms)
            {
                if (term.Key.Item2 != null)
                {
                    list.Add(term.Key.Item1);
                    list.Add(term.Key.Item2);

                    quadratic.Add(term.Key, term.Value);
                }
                else
                {
                    list.Add(term.Key.Item1);

                    linear.Add(term.Key.Item1, term.Value);
                }
            }

            int i = 0;
            foreach (var variable in list)
            {
                variables.Add(variable, i);
                indices.Add(i, variable);
                i++;
            }

            this.Q = createQuadraticTermsMatrix();
            this.d = createLinearTermsVector();
        }

        private double[,] createQuadraticTermsMatrix()
        {
            int n = variables.Count;

            double[,] Q = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                var x = indices[i];
                for (int j = 0; j < n; j++)
                {
                    var y = indices[j];
                    var k = Tuple.Create(x, y);

                    if (quadratic.ContainsKey(k))
                    {
                        double s = quadratic[k];
                        Q[i, j] = s;
                        Q[j, i] = s;
                    }
                }
            }

            for (int i = 0; i < n; i++)
                Q[i, i] *= 2;

            return Q;
        }

        private double[] createLinearTermsVector()
        {
            int n = variables.Count;
            double[] d = new double[n];

            for (int i = 0; i < indices.Count; i++)
            {
                if (linear.ContainsKey(indices[i]))
                    d[i] = linear[indices[i]];
            }

            return d;
        }



        /// <summary>
        ///   Gets the Hessian matrix of quadratic terms.
        /// </summary>
        /// 
        public double[,] GetQuadraticTermsMatrix()
        {
            return (double[,])Q.Clone();
        }

        /// <summary>
        ///   Gets the vector of linear terms.
        /// </summary>
        /// 
        public double[] GetLinearTermsVector()
        {
            return (double[])d.Clone();
        }





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
            StringBuilder sb = new StringBuilder();

            foreach (var term in quadratic)
                sb.AppendFormat("{0:+#;-#}{1}{2} ", term.Value, term.Key.Item1, term.Key.Item2);

            foreach (var term in linear)
                sb.AppendFormat("{0:+#;-#}{1} ", term.Value, term.Key);

            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }



        private double function(double[] x)
        {
            return x.Multiply(Q).InnerProduct(x) + x.InnerProduct(d);
        }


        private static Dictionary<Tuple<string, string>, double> parseString(string f)
        {
            f = f.Replace("*", String.Empty).Replace(" ", String.Empty);

            var terms = new Dictionary<Tuple<string, string>, double>();

            Regex replaceQuad = new Regex(@"([a-zA-Z])(²)");
            f = replaceQuad.Replace(f, "$1$1");


            Regex r = new Regex(@"[\-\+]?[\s]*((\d*\.{0,1}\d+)|[a-zA-Z][²]?)+");
            Regex number = new Regex(@"\d*\.{0,1}\d+");
            Regex symbol = new Regex(@"[a-zA-Z]");


            MatchCollection matches = r.Matches(f, 0);

            foreach (Match m in matches)
            {
                string term = m.Value;

                double scalar = (term[0] == '-') ? -1 : 1;

                // Extract value
                MatchCollection coeff = number.Matches(term);

                foreach (Match c in coeff)
                    scalar *= Double.Parse(c.Value);

                // Extract symbols
                MatchCollection symbols = symbol.Matches(term);

                if (symbols.Count == 1)
                    terms.Add(Tuple.Create(symbols[0].Value, (string)null), scalar);
                else if (symbols.Count == 2)
                    terms.Add(Tuple.Create(symbols[0].Value, symbols[1].Value), scalar);
                else throw new FormatException("Unexpected expression.");
            }

            return terms;
        }

        private static Tuple<string, string> parseExpression(
          Dictionary<Tuple<string, string>, double> terms,
          Expression expr, out double scalar, bool dontAdd = false)
        {
            scalar = 0;

            if (expr == null)
                return null;

            BinaryExpression eb = expr as BinaryExpression;
            MemberExpression em = expr as MemberExpression;
            UnaryExpression eu = expr as UnaryExpression;

            if (em != null) // member expression
            {
                var term = Tuple.Create(em.Member.Name, (string)null);
                terms[term] = 1;
                return term;
            }
            else if (eb != null) // binary expression
            {
                if (expr.NodeType == ExpressionType.Multiply)
                {
                    // This could be either a constant*expression, expression*constant or expression*expression
                    ConstantExpression c = eb.Left as ConstantExpression ?? eb.Right as ConstantExpression;

                    MemberExpression lm = eb.Left as MemberExpression;
                    BinaryExpression lb = eb.Left as BinaryExpression;
                    UnaryExpression lu = eb.Left as UnaryExpression;

                    MemberExpression rm = eb.Right as MemberExpression;
                    BinaryExpression rb = eb.Right as BinaryExpression;
                    UnaryExpression ru = eb.Right as UnaryExpression;


                    if (c != null)
                    {
                        // This is constant*expression or expression*constant
                        scalar = (double)c.Value;

                        if ((lm ?? rm) != null)
                        {
                            var term = Tuple.Create((lm ?? rm).Member.Name, (string)null);
                            if (!dontAdd) terms[term] = scalar;
                            return term;
                        }
                        else if ((lb ?? rb ?? (Expression)lm ?? lu) != null)
                        {
                            double n;
                            var term = parseExpression(terms, lb ?? lu ?? (Expression)rb ?? ru, out n);
                            if (!dontAdd) terms[term] = scalar;
                            return term;
                        }
                        else throw new FormatException("Unexpected expression.");
                    }
                    else
                    {
                        // This is x * x
                        if (lm != null && rm != null)
                        {
                            scalar = 1;
                            return addTuple(terms, scalar, lm.Member.Name, rm.Member.Name);
                        }
                        else if ((lb ?? rb ?? lu ?? (Expression)ru) != null && (lm ?? rm) != null)
                        {
                            // This is expression * x
                            var term = parseExpression(terms, (lb ?? rb ?? lu ?? (Expression)ru), out scalar, dontAdd: true);
                            return addTuple(terms, scalar, term.Item1, (lm ?? rm).Member.Name);
                        }
                        else throw new FormatException("Unexpected expression.");
                    }
                }
                else if (expr.NodeType == ExpressionType.Add)
                {
                    // This could be an expression + term, a term + expression or an expression + expression
                    BinaryExpression lb = eb.Left as BinaryExpression;
                    MemberExpression lm = eb.Left as MemberExpression;
                    UnaryExpression lu = eb.Left as UnaryExpression;

                    BinaryExpression rb = eb.Right as BinaryExpression;
                    MemberExpression rm = eb.Right as MemberExpression;

                    scalar = 1;
                    if (lb != null)
                    {
                        parseExpression(terms, lb, out scalar);
                    }
                    else if (lm != null)
                    {
                        var term = Tuple.Create(lm.Member.Name, (string)null);
                        if (!dontAdd) terms[term] = scalar;
                    }
                    else if (lu != null)
                    {
                        parseExpression(terms, lu, out scalar);
                    }
                    else throw new FormatException("Unexpected expression.");

                    if (rb != null)
                    {
                        parseExpression(terms, rb, out scalar);
                    }
                    else if (rm != null)
                    {
                        var term = Tuple.Create(lm.Member.Name, (string)null);
                        if (!dontAdd) terms[term] = scalar;
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
                    UnaryExpression ru = eb.Right as UnaryExpression;


                    if (lb != null)
                    {
                        parseExpression(terms, lb, out scalar);
                    }
                    else if (lm != null)
                    {
                        scalar = 1;
                        var term = Tuple.Create(lm.Member.Name, (string)null);
                        if (!dontAdd) terms[term] = scalar;
                    }
                    else if (lu != null)
                    {
                        parseExpression(terms, lu, out scalar);
                    }
                    else throw new FormatException("Unexpected expression.");

                    if (rb != null)
                    {
                        var term = parseExpression(terms, rb, out scalar);
                        terms[term] = -scalar;
                    }
                    else if (rm != null)
                    {
                        scalar = -1;
                        var term = Tuple.Create(rm.Member.Name, (string)null);
                        if (!dontAdd) terms[term] = scalar;
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

                    if (lm != null)
                    {
                        scalar = 1;
                        var term = Tuple.Create(lm.Member.Name, (string)null);
                        if (!dontAdd) terms[term] = scalar;
                        return term;
                    }
                    else if (lb != null)
                    {
                        var term = parseExpression(terms, lb, out scalar);
                        if (!dontAdd) terms[term] = scalar;
                    }
                    else throw new FormatException("Unexpected expression.");
                }
                else if (expr.NodeType == ExpressionType.Negate)
                {
                    BinaryExpression lb = eu.Operand as BinaryExpression;
                    MemberExpression lm = eu.Operand as MemberExpression;

                    if (lm != null)
                    {
                        scalar = -1;
                        var term = Tuple.Create(lm.Member.Name, (string)null);
                        if (!dontAdd) terms[term] = scalar;
                        return term;
                    }
                    else if (lb != null)
                    {
                        var term = parseExpression(terms, lb, out scalar);
                        terms[term] = -scalar;
                    }
                    else throw new FormatException("Unexpected expression.");
                }
                else throw new FormatException("Unexpected expression.");
            }
            else throw new FormatException("Unexpected expression.");

            return null;
        }

        private static Tuple<string, string> addTuple(Dictionary<Tuple<string, string>, double> terms, double v, string v1, string v2)
        {

            var t1 = Tuple.Create(v1, v2);
            var t2 = Tuple.Create(v2, v1);

            if (!terms.ContainsKey(t1) && !terms.ContainsKey(t2))
                terms[t1] = v;
            return t1;
        }

    }
}
