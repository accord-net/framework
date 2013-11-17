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

namespace Accord.MachineLearning.DecisionTrees.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Accord.Statistics.Filters;

    public class DecisionRule : ICloneable
    {
        public DecisionVariableCollection Variables { get; private set; }

        public List<DecisionExpression> Expressions { get; private set; }
        public double Output { get; private set; }

        public DecisionRule(double output)
        {
            this.Expressions = new List<DecisionExpression>();
            this.Output = output;
        }

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

            DecisionNode current = node;
            DecisionTree owner = current.Owner;
            double output = current.Output.Value;

            DecisionRule rule = new DecisionRule(output);

            while (current.Parent != null)
            {
                int index = current.Parent.Branches.AttributeIndex;
                ComparisonKind comparison = current.Comparison;
                double value = current.Value.Value;

                DecisionExpression expression = new DecisionExpression(rule, index, comparison, value);

                rule.Expressions.Add(expression);

                current = current.Parent;
            }

            return rule;
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Expressions.Count - 1; i++)
                sb.AppendFormat("({0}) && ", Expressions[i]);

            sb.AppendFormat("({0}) == {1}", Expressions.Last(), Output);

            return sb.ToString();
        }

        public object Clone()
        {
            return new DecisionRule(new List<DecisionExpression>(Expressions), Output);
        }
    }

}
