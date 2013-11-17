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

    public class DecisionList : IList<DecisionRule>
    {
        List<DecisionRule> rules;



        public DecisionList()
        {
            this.rules = new List<DecisionRule>();
        }

        public DecisionList(IEnumerable<DecisionRule> rules)
        {
            this.rules = new List<DecisionRule>(rules);
        }

        public static DecisionList FromDecisionTree(DecisionTree tree)
        {
            var rules = new List<DecisionRule>();

            foreach (var node in tree)
            {
                if (node.IsLeaf && !node.IsRoot && node.Output.HasValue)
                {
                    rules.Add(DecisionRule.FromNode(node));
                }
            }

            return new DecisionList(rules);
        }



        public double? Compute(double[] input)
        {
            foreach (DecisionRule rule in rules)
            {
                if (rule.Match(input))
                    return rule.Output;
            }

            return null;
        }



        #region ICollection implementation

        public void Add(DecisionRule item)
        {
            rules.Add(item);
        }

        public void Clear()
        {
            rules.Clear();
        }

        public bool Contains(DecisionRule item)
        {
            return rules.Contains(item);
        }

        public void CopyTo(DecisionRule[] array, int arrayIndex)
        {
            rules.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return rules.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DecisionRule item)
        {
            return rules.Remove(item);
        }

        public IEnumerator<DecisionRule> GetEnumerator()
        {
            return rules.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return rules.GetEnumerator();
        }

        public int IndexOf(DecisionRule item)
        {
            return rules.IndexOf(item);
        }

        public void Insert(int index, DecisionRule item)
        {
            rules.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            rules.RemoveAt(index);
        }

        public DecisionRule this[int index]
        {
            get { return rules[index]; }
            set { rules[index] = value; }
        }

        #endregion
    }
}
