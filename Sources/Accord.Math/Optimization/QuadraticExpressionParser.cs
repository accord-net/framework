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
    using Accord.Compat;

    internal static class QuadraticExpressionParser
    {
        public static string ToVariable(this int num, char prefix = 'x')
        {
            string number = num.ToString();
            char[] chars = new char[number.Length + 1];
            chars[0] = prefix;

            for (int i = 0; i < number.Length; i++)
            {
                chars[i + 1] = (char)(number[i] + (0x2080 - '0'));
            }

            return new string(chars);
        }

        public static Dictionary<Tuple<string, string>, double> ParseString(string f, CultureInfo culture)
        {
            f = f.Replace("*", String.Empty).Replace(" ", String.Empty);

            var terms = new Dictionary<Tuple<string, string>, double>();

            Regex replaceQuad = new Regex(@"([a-zA-Z])(²)");
            f = replaceQuad.Replace(f, "$1$1");


            string separator = culture.NumberFormat.NumberDecimalSeparator;

            Regex r = new Regex(@"[\-\+]?[\s]*((\d*\" + separator + @"{0,1}\d+)|[a-zA-Z][²]?)+");
            Regex number = new Regex(@"\d*\" + separator + @"{0,1}\d+");
            Regex symbol = new Regex(@"[a-zA-Z]");


            MatchCollection matches = r.Matches(f, 0);

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
                    terms.Add(Tuple.Create(symbols[0].Value, (string)null), scalar);
                else if (symbols.Count == 2)
                    terms.Add(Tuple.Create(symbols[0].Value, symbols[1].Value), scalar);
                else
                    terms.Add(Tuple.Create((string)null, (string)null), scalar);
            }

            return terms;
        }

        public static Tuple<string, string> ParseExpression(
            Dictionary<Tuple<string, string>, double> terms,
            Expression expr, out double scalar, 
            bool dontAdd = false)
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
                            var term = ParseExpression(terms, lb ?? lu ?? (Expression)rb ?? ru, out n);
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
                            var term = ParseExpression(terms, (lb ?? rb ?? lu ?? (Expression)ru), out scalar, dontAdd: true);
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
                    ConstantExpression rc = eb.Right as ConstantExpression;

                    scalar = 1;
                    if (lb != null)
                    {
                        ParseExpression(terms, lb, out scalar);
                    }
                    else if (lm != null)
                    {
                        var term = Tuple.Create(lm.Member.Name, (string)null);
                        if (!dontAdd)
                            terms[term] = scalar;
                    }
                    else if (lu != null)
                    {
                        ParseExpression(terms, lu, out scalar);
                    }
                    else throw new FormatException("Unexpected expression.");

                    scalar = 1;
                    if (rb != null)
                    {
                        ParseExpression(terms, rb, out scalar);
                    }
                    else if (rm != null)
                    {
                        var term = Tuple.Create(rm.Member.Name, (string)null);
                        if (!dontAdd)
                            terms[term] = scalar;
                    }
                    else if (rc != null)
                    {
                        scalar = (double)rc.Value;
                        var term = Tuple.Create((string)null, (string)null);
                        if (!dontAdd)
                            terms[term] = scalar;
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
                    {
                        ParseExpression(terms, lb, out scalar);
                    }
                    else if (lm != null)
                    {
                        scalar = 1;
                        var term = Tuple.Create(lm.Member.Name, (string)null);
                        if (!dontAdd) terms[term] = scalar;
                    }
                    else if (lu != null)
                    {
                        ParseExpression(terms, lu, out scalar);
                    }
                    else throw new FormatException("Unexpected expression.");

                    if (rb != null)
                    {
                        var term = ParseExpression(terms, rb, out scalar);
                        terms[term] = -scalar;
                    }
                    else if (rm != null)
                    {
                        scalar = -1;
                        var term = Tuple.Create(rm.Member.Name, (string)null);
                        if (!dontAdd) terms[term] = scalar;
                    }
                    else if (rc != null)
                    {
                        scalar = (double)rc.Value;
                        var term = Tuple.Create((string)null, (string)null);
                        terms[term] = -scalar;
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
                        var term = ParseExpression(terms, lb, out scalar);
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
                        var term = ParseExpression(terms, lb, out scalar);
                        terms[term] = -scalar;
                    }
                    else throw new FormatException("Unexpected expression.");
                }
                else throw new FormatException("Unexpected expression.");
            }
            else throw new FormatException("Unexpected expression.");

            return null;
        }

        private static Tuple<string, string> addTuple(Dictionary<Tuple<string, string>,
            double> terms, double v, string v1, string v2)
        {

            var t1 = Tuple.Create(v1, v2);
            var t2 = Tuple.Create(v2, v1);

            if (!terms.ContainsKey(t1) && !terms.ContainsKey(t2))
                terms[t1] = v;
            return t1;
        }
    }
}
