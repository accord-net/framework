// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using Accord.Statistics.Filters;
    using System;
    using System.Runtime.Serialization;
    using System.Collections.Generic;

    /// <summary>
    ///   Decision Tree (DT) Node.
    /// </summary>
    /// 
    /// <remarks>
    ///   Each node of a decision tree can play two roles. When a node is not a leaf, it
    ///   contains a <see cref="DecisionBranchNodeCollection"/> with a collection of child nodes. The
    ///   branch specifies an attribute index, indicating which column from the data set
    ///   (the attribute) should be compared against its children values. The type of the
    ///   comparison is specified by each of the children. When a node is a leaf, it will
    ///   contain the output value which should be decided for when the node is reached.
    /// </remarks>
    /// 
    /// <seealso cref="DecisionTree"/>
    /// 
    [Serializable]
    public class DecisionNode : IEnumerable<DecisionNode>
    {

        [NonSerialized]
        private DecisionTree owner;

        [NonSerialized]
        private DecisionNode parent;


        /// <summary>
        ///   Gets or sets the value this node responds to
        ///   whenever this node acts as a child node. This
        ///   value is set only when the node has a parent.
        /// </summary>
        /// 
        public double? Value { get; set; }

        /// <summary>
        ///   Gets or sets the type of the comparison which
        ///   should be done against <see cref="Value"/>.
        /// </summary>
        /// 
        public ComparisonKind Comparison { get; set; }

        /// <summary>
        ///   If this is a leaf node, gets or sets the output
        ///   value to be decided when this node is reached.
        /// </summary>
        /// 
        public int? Output { get; set; }

        /// <summary>
        ///   If this is not a leaf node, gets or sets the collection
        ///   of child nodes for this node, together with the attribute
        ///   determining the reasoning process for those children.
        /// </summary>
        /// 
        public DecisionBranchNodeCollection Branches { get; set; }

        /// <summary>
        ///   Gets or sets the parent of this node. If this is a root
        ///   node, the parent is <c>null</c>.
        /// </summary>
        /// 
        public DecisionNode Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>
        ///   Gets the <see cref="DecisionTree"/> containing this node.
        /// </summary>
        /// 
        public DecisionTree Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        /// <summary>
        ///   Creates a new decision node.
        /// </summary>
        /// 
        /// <param name="owner">The owner tree for this node.</param>
        /// 
        public DecisionNode(DecisionTree owner)
        {
            Owner = owner;
            Comparison = ComparisonKind.None;
            Branches = new DecisionBranchNodeCollection(this);
        }

        /// <summary>
        ///   Gets a value indicating whether this instance is a root node (has no parent).
        /// </summary>
        /// 
        /// <value><c>true</c> if this instance is a root; otherwise, <c>false</c>.</value>
        /// 
        public bool IsRoot
        {
            get { return Parent == null; }
        }

        /// <summary>
        ///   Gets a value indicating whether this instance is a leaf (has no children).
        /// </summary>
        /// 
        /// <value><c>true</c> if this instance is a leaf; otherwise, <c>false</c>.</value>
        /// 
        public bool IsLeaf
        {
            get { return Branches == null || Branches.Count == 0; }
        }



        /// <summary>
        ///   Computes whether a value satisfies
        ///   the condition imposed by this node.
        /// </summary>
        /// 
        /// <param name="x">The value x.</param>
        /// 
        /// <returns><c>true</c> if the value satisfies this node's
        /// condition; otherwise, <c>false</c>.</returns>
        /// 
        public bool Compute(double x)
        {
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
            if (IsRoot)
                return "Root";

            String name = Owner.Attributes[Parent.Branches.AttributeIndex].Name;

            if (String.IsNullOrEmpty(name))
                name = "x" + Parent.Branches.AttributeIndex;

            String op = ComparisonExtensions.ToString(Comparison);

            String value;
            if (codebook != null && Value.HasValue && codebook.Columns.Contains(name))
                value = codebook.Translate(name, (int)Value.Value);

            else value = Value.ToString();


            return String.Format("{0} {1} {2}", name, op, value);
        }

        /// <summary>
        ///   Computes the height of the node, defined as the
        ///   distance (in number of links) between the tree's
        ///   root node and this node.
        /// </summary>
        /// 
        /// <returns>The node's height.</returns>
        /// 
        public int GetHeight()
        {
            int height = 0;
            var parent = Parent;

            while (parent != null)
            {
                height++;
                parent = parent.parent;
            }

            return height;
        }




        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (Branches != null)
            {
                Branches.Owner = this;

                foreach (DecisionNode node in Branches)
                {
                    node.Parent = this;
                }
            }
        }


        /// <summary>
        ///   Returns an enumerator that iterates through the node's subtree.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<DecisionNode> GetEnumerator()
        {
            var stack = new Stack<DecisionNode>(new[] { this });

            while (stack.Count != 0)
            {
                DecisionNode current = stack.Pop();

                yield return current;

                if (current.Branches != null)
                    for (int i = current.Branches.Count - 1; i >= 0; i--)
                        stack.Push(current.Branches[i]);
            }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the node's subtree.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
