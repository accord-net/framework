// Accord Machine Learning Library
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

namespace Accord.MachineLearning.DecisionTrees.Rules
{
    using System.Collections.Generic;
    using System.Text;
    using Accord.Statistics.Filters;
    using System;
    using System.Globalization;
    using Accord.Math;
    using Accord.MachineLearning.DecisionTrees.Learning;

    /// <summary>
    ///   Decision rule set.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Decision rule sets can be created from <see cref="DecisionTree"/>s using their
    ///   <see cref="DecisionTree.ToRules()"/> method. An example is shown below.</para>
    /// </remarks>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\C45LearningTest.cs" region="doc_iris" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\C45LearningTest.cs" region="doc_missing" />
    /// </example>
    /// 
    /// <seealso cref="DecisionTree"/>
    /// <seealso cref="C45Learning"/>
    /// 
    public class DecisionSet : MulticlassClassifierBase<double[]>, IEnumerable<DecisionRule>
    {

        HashSet<DecisionRule> rules;

        /// <summary>
        ///   Obsolete. Please use <see cref="ClassifierBase{TInput, TClasses}.NumberOfClasses"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfClasses instead.")]
        public int OutputClasses { get { return NumberOfClasses; } }


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
                NumberOfClasses = tree.NumberOfClasses,
                NumberOfOutputs = tree.NumberOfOutputs
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
            double r = Decide(input);
            return r == -1 ? (double?)null : r;
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
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            return toString(null, null, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(Codification<string> codebook, CultureInfo cultureInfo)
        {
            return toString(codebook, null, cultureInfo);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(Codification<string> codebook)
        {
            return ToString(codebook, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(Codification<string> codebook, string outputColumn, CultureInfo cultureInfo)
        {
            return toString(codebook, outputColumn, CultureInfo.CurrentUICulture);
        }

        private string toString(Codification<string> codebook, string outputColumn, CultureInfo cultureInfo)
        {
            var rulesArray = new DecisionRule[this.rules.Count];

            rules.CopyTo(rulesArray);
            Array.Sort(rulesArray);

            StringBuilder sb = new StringBuilder();
            if (codebook != null)
            {
                foreach (DecisionRule rule in rulesArray)
                    sb.AppendLine(rule.ToString(codebook, outputColumn, cultureInfo));
            }
            else
            {
                foreach (DecisionRule rule in rulesArray)
                    sb.AppendLine(rule.ToString(cultureInfo));
            }

            return sb.ToString();
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

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="P:Accord.MachineLearning.ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <returns>A class-label that best described <paramref name="input" /> according
        /// to this classifier.</returns>
        public override int Decide(double[] input)
        {
            if (input.HasNaN())
            {
                int[] outputs = new int[NumberOfClasses];
                foreach (DecisionRule rule in rules)
                {
                    if (rule.Match(input))
                        outputs[(int)rule.Output] += 1;
                }

                return outputs.ArgMax();
            }
            else
            {
                foreach (DecisionRule rule in rules)
                {
                    if (rule.Match(input))
                        return (int)rule.Output;
                }
            }

            return -1;
        }
    }
}
