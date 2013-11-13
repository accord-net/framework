// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
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

namespace Accord.MachineLearning.DecisionTrees
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Accord.Statistics.Filters;

    public class DecisionRule
    {
        public List<DecisionExpression> Expressions { get; private set; }
        public double Output { get; private set; }

        public DecisionRule(List<DecisionExpression> expressions, double output)
        {
            this.Expressions = expressions;
            this.Output = output;
        }

        public bool Match(double[] input)
        {
            foreach (var expr in Expressions)
            {
                if (!expr.Match(input))
                    return false;
            }

            return true;
        }

     

        public static DecisionRule FromNode(DecisionNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (!node.IsLeaf || node.IsRoot || !node.Value.HasValue)
                throw new InvalidOperationException("Only leaf nodes that have a parent can be converted to rules.");

            var expressions = new List<DecisionExpression>();

            DecisionNode current = node;
            DecisionTree owner = current.Owner;
            double output = current.Output.Value;

            while (current.Parent != null)
            {
                int index = current.Parent.Branches.AttributeIndex;
                DecisionVariable variable = owner.Attributes[index];
                ComparisonKind comparison = current.Comparison;
                double value = current.Value.Value;

                expressions.Add(new DecisionExpression(variable, index, comparison, value));

                current = current.Parent;
            }

            return new DecisionRule(expressions, output);
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Expressions.Count - 1; i++)
                sb.AppendFormat("({0}) && ", Expressions[i]);

            sb.AppendFormat("({0}) == {1}", Expressions.Last(), Output);

            return sb.ToString();
        }
    }

    public class DecisionExpression
    {
        DecisionVariable Variable { get; set; }
        int Index { get; set; }
        ComparisonKind Comparison { get; set; }
        double Value { get; set; }

        public bool Match(double[] input)
        {
            double x = input[Index];

            switch (Comparison)
            {
                case ComparisonKind.Equal:
                    return (x == Value);

                case ComparisonKind.GreaterThan:
                    return (x > Value);

                case ComparisonKind.GreaterThanOrEqual:
                    return (x >= Value);

                case ComparisonKind.LessThan:
                    return (x < Value);

                case ComparisonKind.LessThanOrEqual:
                    return (x <= Value);

                case ComparisonKind.NotEqual:
                    return (x != Value);

                default:
                    throw new InvalidOperationException();
            }
        }

        public DecisionExpression(DecisionVariable variable, int index, ComparisonKind comparison, double value)
        {
            this.Variable = variable;
            this.Index = index;
            this.Comparison = comparison;
            this.Value = value;
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
            return toString(null);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(Codification codebook)
        {
            return toString(codebook);
        }

        private string toString(Codification codebook)
        {
            String name = Variable.Name;

            if (String.IsNullOrEmpty(name))
                name = "x" + Index;

            String op = ComparisonExtensions.ToString(Comparison);

            String value;
            if (codebook != null && codebook.Columns.Contains(name))
                value = codebook.Translate(name, (int)Value);

            else value = Value.ToString();

            return String.Format("{0} {1} {2}", name, op, value);
        }
    }
}
