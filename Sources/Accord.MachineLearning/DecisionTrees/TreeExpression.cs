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

#if !NET35
namespace Accord.MachineLearning.DecisionTrees
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    ///   Decision Tree (Linq) Expression Creator.
    /// </summary>
    /// 
    internal class DecisionTreeExpressionCreator
    {

        DecisionTree tree;
        ParameterExpression inputs;
        LabelTarget label;

        /// <summary>
        ///   Initializes a new instance of the <see cref="DecisionTreeExpressionCreator"/> class.
        /// </summary>
        /// 
        /// <param name="tree">The decision tree.</param>
        /// 
        internal DecisionTreeExpressionCreator(DecisionTree tree)
        {
            this.tree = tree;
        }

        /// <summary>
        ///   Creates an expression for the tree.
        /// </summary>
        /// 
        /// <returns>A strongly typed lambda expression in the form
        /// of an <see cref="Expression{T}">expression</see> tree
        /// representing the <see cref="DecisionTree"/>.</returns>
        /// 
        public Expression<Func<double[], int>> Create()
        {
            inputs = Expression.Parameter(typeof(double[]), "input");
            label = Expression.Label(typeof(int), "return");

            var rootExpression = create(tree.Root);

            ConstructorInfo ex = typeof(ArgumentException).GetConstructor(
                new[] { typeof(string), typeof(string) });

            BlockExpression block = Expression.Block(typeof(int),
                rootExpression,
                Expression.Throw(Expression.New(ex,
                    Expression.Constant("Input contains a value outside of expected ranges."),
                    Expression.Constant("input"))),
                Expression.Label(label, Expression.Constant(0))
            );

            return Expression.Lambda<Func<double[], int>>(block, inputs);
        }

        private Expression create(DecisionNode node)
        {
            if (!node.IsLeaf)
            {
                int attributeIndex = node.Branches.AttributeIndex;

                // Create all comparison expressions
                BinaryExpression[] comparisons = new BinaryExpression[node.Branches.Count];
                Expression[] childExpression = new Expression[node.Branches.Count];

                for (int i = 0; i < comparisons.Length; i++)
                {
                    DecisionNode child = node.Branches[i];

                    var expr = Expression.ArrayIndex(inputs, Expression.Constant(attributeIndex));
                    var cons = Expression.Constant(child.Value);

                    switch (child.Comparison)
                    {
                        case ComparisonKind.Equal:
                            comparisons[i] = Expression.Equal(expr, cons); break;

                        case ComparisonKind.GreaterThan:
                            comparisons[i] = Expression.GreaterThan(expr, cons); break;

                        case ComparisonKind.GreaterThanOrEqual:
                            comparisons[i] = Expression.GreaterThanOrEqual(expr, cons); break;

                        case ComparisonKind.LessThan:
                            comparisons[i] = Expression.LessThan(expr, cons); break;

                        case ComparisonKind.LessThanOrEqual:
                            comparisons[i] = Expression.LessThanOrEqual(expr, cons); break;

                        case ComparisonKind.NotEqual:
                            comparisons[i] = Expression.NotEqual(expr, cons); break;

                        default:
                            throw new InvalidOperationException("Unexpected node comparison type.");
                    }


                    childExpression[i] = create(node.Branches[i]);
                }

                // Create expression for else expressions
                ConstructorInfo ex = typeof(ArgumentException).GetConstructor(new[] { typeof(string), typeof(string) });

                var lastElse = Expression.IfThenElse(comparisons[comparisons.Length - 1],
                    childExpression[comparisons.Length - 1],
                    Expression.Throw(Expression.New(ex,
                      Expression.Constant("Input contains a value outside of expected ranges."),
                      Expression.Constant("input"))));

                ConditionalExpression currentIf = null;
                for (int i = comparisons.Length - 2; i >= 0; i--)
                {
                    currentIf = Expression.IfThenElse(comparisons[i],
                            childExpression[i], lastElse);
                    lastElse = currentIf;
                }

                return currentIf;
            }

            else // node is a leaf
            {
                if (node.Output.HasValue)
                    return Expression.Return(label, Expression.Constant(node.Output.Value), typeof(int));
                return Expression.Return(label, Expression.Constant(-1), typeof(int));
            }

        }

    }
}
#endif