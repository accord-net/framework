// Accord Machine Learning Library
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

namespace Accord.MachineLearning.DecisionTrees.Rules
{
    using System.Collections.Generic;

    /// <summary>
    ///   Decision rule set.
    /// </summary>
    /// 
    public class DecisionSet : IEnumerable<DecisionRule>
    {

        HashSet<DecisionRule> rules;

        /// <summary>
        ///   Gets the number of possible output 
        ///   classes covered by this decision set.
        /// </summary>
        /// 
        public int OutputClasses { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="DecisionSet"/> class.
        /// </summary>
        /// 
        public DecisionSet()
        {
            this.rules = new HashSet<DecisionRule>();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DecisionSet"/> class.
        /// </summary>
        /// 
        /// <param name="rules">A set of decision rules.</param>
        /// 
        public DecisionSet(IEnumerable<DecisionRule> rules)
        {
            this.rules = new HashSet<DecisionRule>(rules);
        }

        /// <summary>
        ///   Creates a new <see cref="DecisionSet"/> from a <see cref="DecisionTree"/>.
        /// </summary>
        /// 
        /// <param name="tree">A <see cref="DecisionTree"/>.</param>
        /// 
        /// <returns>A <see cref="DecisionSet"/> that is completely 
        /// equivalent to the given <paramref name="tree"/></returns>
        /// 
        public static DecisionSet FromDecisionTree(DecisionTree tree)
        {
            var rules = new List<DecisionRule>();

            foreach (var node in tree)
            {
                if (node.IsLeaf && !node.IsRoot && node.Output.HasValue)
                {
                    rules.Add(DecisionRule.FromNode(node));
                }
            }

            return new DecisionSet(rules)
            {
                OutputClasses = tree.OutputClasses
            };
        }


        /// <summary>
        ///   Computes the decision output for a given input.
        /// </summary>
        /// 
        /// <param name="input">An input vector.</param>
        /// 
        /// <returns>The decision output for the given 
        ///   <paramref name="input"/>.</returns>
        /// 
        public double? Compute(double[] input)
        {
            foreach (DecisionRule rule in rules)
            {
                if (rule.Match(input))
                    return rule.Output;
            }

            return null;
        }


        /// <summary>
        ///   Adds a new <see cref="DecisionRule"/> to the set.
        /// </summary>
        /// 
        /// <param name="item">The <see cref="DecisionRule"/> to be added.</param>
        /// 
        public void Add(DecisionRule item)
        {
            rules.Add(item);
        }

        /// <summary>
        ///   Adds a collection of new <see cref="DecisionRule"/>s to the set.
        /// </summary>
        /// 
        /// <param name="items">The collection of <see cref="DecisionRule"/>s to be added.</param>
        /// 
        public void AddRange(IEnumerable<DecisionRule> items)
        {
            foreach (var rule in items)
                rules.Add(rule);
        }

        /// <summary>
        ///   Removes all rules from this set.
        /// </summary>
        /// 
        public void Clear()
        {
            rules.Clear();
        }

        /// <summary>
        ///   Gets the number of rules in this set.
        /// </summary>
        /// 
        public int Count
        {
            get { return rules.Count; }
        }

        /// <summary>
        ///   Removes a given rule from the set.
        /// </summary>
        /// 
        /// <param name="item">The <see cref="DecisionRule"/> to be removed.</param>
        /// 
        /// <returns>True if the rule was removed; false otherwise.</returns>
        /// 
        public bool Remove(DecisionRule item)
        {
            return rules.Remove(item);
        }



        /// <summary>
        ///    Returns an enumerator that iterates through a collection.
        /// </summary>
        /// 
        /// <returns>
        ///    An <see cref="T:System.Collections.IEnumerator"/> object 
        ///    that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<DecisionRule> GetEnumerator()
        {
            return rules.GetEnumerator();
        }

        /// <summary>
        ///    Returns an enumerator that iterates through a collection.
        /// </summary>
        /// 
        /// <returns>
        ///    An <see cref="T:System.Collections.IEnumerator"/> object 
        ///    that can be used to iterate through the collection.
        /// </returns>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return rules.GetEnumerator();
        }

    }
}
