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

    /// <summary>
    ///   Decision Rule.
    /// </summary>
    /// 
    public class DecisionRule : ICloneable, IEnumerable<Antecedent>,
        IEquatable<DecisionRule>, IComparable, IComparable<DecisionRule>
    {
        private SortedSet<Antecedent> expressions;
        private DecisionVariableCollection variables;
        private double output;


        /// <summary>
        ///   Gets the decision variables handled by this rule.
        /// </summary>
        /// 
        public DecisionVariableCollection Variables { get { return variables; } }

        /// <summary>
        ///   Gets the <see cref="Antecedent"/> expressions that
        ///   must be fulfilled in order for this rule to be applicable.
        /// </summary>
        /// 
        public ISet<Antecedent> Antecedents { get { return expressions; } }

        /// <summary>
        ///   Gets the output of this decision rule, given when all
        ///   <see cref="Antecedent"/> conditions are met.
        /// </summary>
        /// 
        public double Output { get { return output; } }





        /// <summary>
        ///   Initializes a new instance of the <see cref="DecisionRule"/> class.
        /// </summary>
        /// 
        /// <param name="output">The output.</param>
        /// 
        public DecisionRule(double output)
        {
            this.expressions = new SortedSet<Antecedent>();
            this.output = output;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DecisionRule"/> class.
        /// </summary>
        /// 
        /// <param name="expressions">The antecedent expressions.</param>
        /// <param name="output">The output.</param>
        /// 
        public DecisionRule(IEnumerable<Antecedent> expressions, double output)
        {
            this.expressions = new SortedSet<Antecedent>(expressions);
            this.output = output;
        }

        /// <summary>
        ///   Gets the number of antecedents contained 
        ///   in this <see cref="DecisionRule"/>.
        /// </summary>
        /// 
        public int Count
        {
            get { return Antecedents.Count; }
        }

        /// <summary>
        ///   Checks whether a the rule applies to a given input vector.
        /// </summary>
        /// 
        /// <param name="input">An input vector.</param>
        /// 
        /// <returns>True, if the input matches the rule 
        ///   <see cref="Antecedents"/>; otherwise, false.
        /// </returns>
        ///   
        public bool Match(double[] input)
        {
            foreach (var expr in Antecedents)
            {
                if (!expr.Match(input))
                    return false;
            }

            return true;
        }


        /// <summary>
        ///   Creates a new <see cref="DecisionRule"/> from a <see cref="DecisionTree"/>'s
        ///   <see cref="DecisionNode"/>. This node must be a leaf, cannot be the root, and
        ///   should have one output value.
        /// </summary>
        /// 
        /// <param name="node">A <see cref="DecisionNode"/> from a <see cref="DecisionTree"/>.</param>
        /// 
        /// <returns>A <see cref="DecisionRule"/> representing the given <paramref name="node"/>.</returns>
        /// 
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
            rule.variables = node.Owner.Attributes;

            while (current.Parent != null)
            {
                int index = current.Parent.Branches.AttributeIndex;
                ComparisonKind comparison = current.Comparison;
                double value = current.Value.Value;

                Antecedent expression = new Antecedent(rule, index, comparison, value);

                rule.Antecedents.Add(expression);

                current = current.Parent;
            }

            return rule;
        }

        /// <summary>
        ///   Gets whether this rule and another rule have
        ///   the same antecedents but different outputs.
        /// </summary>
        /// 
        /// <param name="rule"></param>
        /// 
        /// <returns>True if the two rules are contradictory; 
        ///   false otherwise.</returns>
        /// 
        public bool IsInconsistentWith(DecisionRule rule)
        {
            return Antecedents.SetEquals(rule.Antecedents) && Output != rule.Output;
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

            var expr = expressions.ToArray();

            for (int i = 0; i < expr.Length - 1; i++)
                sb.AppendFormat("({0}) && ", expr[i]);

            sb.AppendFormat("({0}) := {1}", expr[expr.Length - 1], Output);

            return sb.ToString();
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            return new DecisionRule(Antecedents, Output);
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that 
        ///   can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<Antecedent> GetEnumerator()
        {
            return expressions.GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that 
        ///   can be used to iterate through the collection.
        /// </returns>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return expressions.GetEnumerator();
        }

        public override int GetHashCode()
        {
            return this.Output.GetHashCode() +
                13 * this.Variables.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DecisionRule);
        }

        public bool Equals(DecisionRule other)
        {
            if (other == null)
                return false;

            return this.Antecedents.SetEquals(other.Antecedents) &&
                this.Output == other.Output &&
                this.Variables == other.Variables;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as DecisionRule);
        }

        public int CompareTo(DecisionRule other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            int count = Count.CompareTo(other.Count);

            if (count != 0)
                return count;

            return Output.CompareTo(other.Output);
        }
    }

}
