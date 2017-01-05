// Accord Core Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Steven G. Johnson, 2008
// stevenj@alum.mit.edu
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
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//
//
// The source code presented in this file has been adapted from the
// Red Black tree implementation presented in the NLopt Numerical 
// Optimization Library. Original license details are given below.
//
//    Copyright (c) 2007-2011 Massachusetts Institute of Technology
//
//    Permission is hereby granted, free of charge, to any person obtaining
//    a copy of this software and associated documentation files (the
//    "Software"), to deal in the Software without restriction, including
//    without limitation the rights to use, copy, modify, merge, publish,
//    distribute, sublicense, and/or sell copies of the Software, and to
//    permit persons to whom the Software is furnished to do so, subject to
//    the following conditions:
// 
//    The above copyright notice and this permission notice shall be
//    included in all copies or substantial portions of the Software.
// 
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//

namespace Accord.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Red-black tree.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   A red–black tree is a data structure which is a type of self-balancing binary 
    ///   search tree. Balance is preserved by painting each node of the tree with one of
    ///   two colors (typically called 'red' and 'black') in a way that satisfies certain 
    ///   properties, which collectively constrain how unbalanced the tree can become in 
    ///   the worst case. When the tree is modified, the new tree is subsequently rearranged
    ///   and repainted to restore the coloring properties. The properties are designed in 
    ///   such a way that this rearranging and recoloring can be performed efficiently.</para>
    ///   
    /// <para>
    ///   The balancing of the tree is not perfect but it is good enough to allow it to 
    ///   guarantee searching in O(log n) time, where n is the total number of elements 
    ///   in the tree. The insertion and deletion operations, along with the tree rearrangement 
    ///   and recoloring, are also performed in O(log n) time. </para>
    ///   
    /// <para>
    ///   Tracking the color of each node requires only 1 bit of information per node because
    ///   there are only two colors. The tree does not contain any other data specific to its
    ///   being a red–black tree so its memory footprint is almost identical to a classic 
    ///   (uncolored) binary search tree. </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://ab-initio.mit.edu/nlopt">
    ///       Steven G. Johnson, The NLopt nonlinear-optimization package, 
    ///       http://ab-initio.mit.edu/nlopt </a></description></item>
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Red%E2%80%93black_tree">
    ///       Wikipedia, The Free Encyclopedia. Red-black tree. Available on:
    ///       http://en.wikipedia.org/wiki/Red%E2%80%93black_tree </a></description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    /// <typeparam name="T">The type of the value to be stored.</typeparam>
    /// 
    [Serializable]
    public class RedBlackTree<T> : BinaryTree<RedBlackTreeNode<T>>, 
        ICollection<RedBlackTreeNode<T>>, ICollection<T>
    {

        IComparer<T> compare;
        RedBlackTreeNode<T> root;
        int count;

        bool duplicates = false;

        /// <summary>
        ///   Constructs a new <see cref="RedBlackTree&lt;T&gt;"/> using the
        ///   default <see cref="IComparer{T}"/> for type <typeparamref name="T"/>.
        /// </summary>
        /// 
        public RedBlackTree()
        {
            this.compare = Comparer<T>.Default;
        }

        /// <summary>
        ///   Constructs a new <see cref="RedBlackTree&lt;T&gt;"/> using 
        ///   the provided <see cref="IComparer{T}"/> implementation.
        /// </summary>
        /// 
        /// <param name="comparer">
        ///   The element comparer to be used to order elements in the tree.</param>
        /// 
        public RedBlackTree(IComparer<T> comparer)
        {
            this.compare = comparer;
        }

        /// <summary>
        ///   Constructs a new <see cref="RedBlackTree&lt;T&gt;"/> using the
        ///   default <see cref="IComparer{T}"/> for type <typeparamref name="T"/>.
        /// </summary>
        /// 
        /// <param name="allowDuplicates">
        ///   Pass <c>true</c> to allow duplicate elements 
        ///   in the tree; <c>false</c> otherwise.</param>
        /// 
        public RedBlackTree(bool allowDuplicates)
        {
            this.compare = Comparer<T>.Default;
            this.duplicates = allowDuplicates;
        }

        /// <summary>
        ///   Constructs a new <see cref="RedBlackTree&lt;T&gt;"/> using 
        ///   the provided <see cref="IComparer{T}"/> implementation.
        /// </summary>
        /// 
        /// <param name="comparer">
        ///   The element comparer to be used to order elements in the tree.</param>
        /// <param name="allowDuplicates">
        ///   Pass <c>true</c> to allow duplicate elements 
        ///   in the tree; <c>false</c> otherwise.</param>
        /// 
        public RedBlackTree(IComparer<T> comparer, bool allowDuplicates)
        {
            this.compare = comparer;
            this.duplicates = allowDuplicates;
        }

        /// <summary>
        ///   Gets the number of nodes contained in this red-black tree.
        /// </summary>
        /// 
        public int Count { get { return count; } }

        /// <summary>
        ///   Gets the <see cref="IComparer{T}"/> for this red black tree.
        /// </summary>
        /// 
        public IComparer<T> Comparer
        {
            get { return compare; }
        }

        /// <summary>
        ///   Removes all nodes from the tree.
        /// </summary>
        /// 
        public void Clear()
        {
            this.root = null;
            this.count = 0;
        }

        /// <summary>
        ///   Adds a new item to the tree. If the element already
        ///   belongs to this tree, no new element will be added.
        /// </summary>
        /// 
        /// <param name="item">The item to be added.</param>
        /// 
        /// <returns>The node containing the added item.</returns>
        /// 
        public RedBlackTreeNode<T> Add(T item)
        {
            var n = new RedBlackTreeNode<T>(item);
            Add(n);
            return n;
        }

        void ICollection<T>.Add(T item)
        {
            Add(new RedBlackTreeNode<T>(item));
        }

        /// <summary>
        ///   Adds a new item to the tree. If the element already
        ///   belongs to this tree, no new element will be added.
        /// </summary>
        /// 
        /// <param name="item">The node to be added to the tree.</param>
        /// 
        public void Add(RedBlackTreeNode<T> item)
        {
            var k = item.Value;
            var p = this.root;

            item.Color = RedBlackTreeNodeType.Red;
            item.Parent = null;
            item.Left = null;
            item.Right = null;

            if (p == null)
            {
                this.root = item;
                item.Color = RedBlackTreeNodeType.Black;
                this.count++;
                return;
            }

            // insert (red) node into tree
            while (true)
            {
                int cmp = compare.Compare(k, p.Value);

                if (!duplicates && cmp == 0)
                {
                    p.Value = item.Value;
                    return;
                }
                else if (cmp <= 0)
                {
                    // k <= p.k
                    if (p.Left != null)
                        p = p.Left;
                    else
                    {
                        p.Left = item;
                        item.Parent = p;
                        break;
                    }
                }
                else
                {
                    if (p.Right != null)
                        p = p.Right;
                    else
                    {
                        p.Right = item;
                        item.Parent = p;
                        break;
                    }
                }
            }

        fixtree:

            if (item.Parent.Color == RedBlackTreeNodeType.Red)
            {
                // red cannot have red child
                var u = p == p.Parent.Left ? p.Parent.Right : p.Parent.Left;

                if (u != null && u.Color == RedBlackTreeNodeType.Red)
                {
                    p.Color = u.Color = RedBlackTreeNodeType.Black;
                    item = p.Parent;

                    if ((p = item.Parent) != null)
                    {
                        item.Color = RedBlackTreeNodeType.Red;
                        goto fixtree;
                    }
                }
                else
                {
                    if (item == p.Right && p == p.Parent.Left)
                    {
                        rotate_left(p);
                        p = item; item = item.Left;
                    }
                    else if (item == p.Left && p == p.Parent.Right)
                    {
                        rotate_right(p);
                        p = item;
                        item = item.Right;
                    }

                    p.Color = RedBlackTreeNodeType.Black;
                    p.Parent.Color = RedBlackTreeNodeType.Red;

                    if (item == p.Left && p == p.Parent.Left)
                        rotate_right(p.Parent);
                    else if (item == p.Right && p == p.Parent.Right)
                        rotate_left(p.Parent);
                }
            }

            this.count++;
        }

        /// <summary>
        ///   Attempts to remove an element from the tree.
        /// </summary>
        /// 
        /// <param name="item">The item to be removed.</param>
        /// 
        /// <returns>
        ///   <c>True</c> if the element was in the tree and was removed; false otherwise.
        /// </returns>
        /// 
        bool ICollection<T>.Remove(T item)
        {
            var node = Find(item);

            if (node == null)
                return false;

            return Remove(node) != null;
        }

        /// <summary>
        ///   Removes a node from the tree.
        /// </summary>
        /// 
        /// <param name="item">The node to be removed.</param>
        /// 
        /// <returns>
        ///   <c>True</c> if the element was in the tree and was removed; false otherwise.
        /// </returns>
        /// 
        bool ICollection<RedBlackTreeNode<T>>.Remove(RedBlackTreeNode<T> item)
        {
            return Remove(item) != null;
        }

        /// <summary>
        ///   Removes a node from the tree.
        /// </summary>
        /// 
        /// <param name="item">The key of the node to be removed.</param>
        /// 
        /// <returns>
        ///   A reference to the removed node, if the item was in the tree; otherwise, <c>null</c>.
        /// </returns>
        /// 
        public RedBlackTreeNode<T> Remove(T item)
        {
            var node = Find(item);

            if (node == null)
                return null;

            return Remove(node);
        }

       

       
        /// <summary>
        ///   Removes a node from the tree.
        /// </summary>
        /// 
        /// <param name="node">The node to be removed.</param>
        /// 
        /// <returns>
        ///   A reference to the removed node.
        /// </returns>
        /// 
        public RedBlackTreeNode<T> Remove(RedBlackTreeNode<T> node)
        {
            var k = node.Value;

            RedBlackTreeNode<T> m, mp;

            if (node.Left != null && node.Right != null)
            {
                var lmax = node.Left;

                while (lmax.Right != null)
                    lmax = lmax.Right;

                node.Value = lmax.Value;
                node = lmax;
            }

            m = node.Left != null ? node.Left : node.Right;

            if (node.Parent != null)
            {
                if (node.Parent.Right == node)
                    node.Parent.Right = m;
                else node.Parent.Left = m;
            }
            else
            {
                this.root = m;
            }

            mp = node.Parent;
            if (m != null)
                m.Parent = mp;

            if (node.Color == RedBlackTreeNodeType.Black)
            {
                if (m != null && m.Color == RedBlackTreeNodeType.Red)
                {
                    m.Color = RedBlackTreeNodeType.Black;
                }
                else
                {

                deleteblack:
                    if (mp != null)
                    {
                        var s = m == mp.Left ? mp.Right : mp.Left;

                        if (s.Color == RedBlackTreeNodeType.Red)
                        {
                            mp.Color = RedBlackTreeNodeType.Red;
                            s.Color = RedBlackTreeNodeType.Black;

                            if (m == mp.Left)
                                rotate_left(mp);
                            else rotate_right(mp);

                            s = m == mp.Left ? mp.Right : mp.Left;
                        }

                        if (mp.Color == RedBlackTreeNodeType.Black 
                          && s.Color == RedBlackTreeNodeType.Black 
                          && (s.Left == null || s.Left.Color == RedBlackTreeNodeType.Black) 
                          && (s.Right == null || s.Right.Color == RedBlackTreeNodeType.Black))
                        {
                            if (s != null)
                                s.Color = RedBlackTreeNodeType.Red;

                            m = mp;
                            mp = m.Parent;

                            goto deleteblack;
                        }
                        else if (mp.Color == RedBlackTreeNodeType.Red 
                            && s.Color == RedBlackTreeNodeType.Black 
                            && (s.Left == null || s.Left.Color == RedBlackTreeNodeType.Black) 
                            && (s.Right == null || s.Right.Color == RedBlackTreeNodeType.Black))
                        {
                            if (s != null)
                                s.Color = RedBlackTreeNodeType.Red;

                            mp.Color = RedBlackTreeNodeType.Black;
                        }
                        else
                        {
                            if (m == mp.Left && s.Color == RedBlackTreeNodeType.Black 
                                && (s.Left != null && s.Left.Color == RedBlackTreeNodeType.Red) 
                                && (s.Right == null || s.Right.Color == RedBlackTreeNodeType.Black))
                            {
                                s.Color = RedBlackTreeNodeType.Red;
                                s.Left.Color = RedBlackTreeNodeType.Black;

                                rotate_right(s);

                                s = m == mp.Left ? mp.Right : mp.Left;
                            }
                            else if (m == mp.Right && s.Color == RedBlackTreeNodeType.Black 
                                && (s.Right != null && s.Right.Color == RedBlackTreeNodeType.Red) 
                                && (s.Left == null || s.Left.Color == RedBlackTreeNodeType.Black))
                            {
                                s.Color = RedBlackTreeNodeType.Red;
                                s.Right.Color = RedBlackTreeNodeType.Black;
                                rotate_left(s);
                                s = m == mp.Left ? mp.Right : mp.Left;
                            }

                            s.Color = mp.Color;
                            mp.Color = RedBlackTreeNodeType.Black;

                            if (m == mp.Left)
                            {
                                s.Right.Color = RedBlackTreeNodeType.Black;
                                rotate_left(mp);
                            }
                            else
                            {
                                s.Left.Color = RedBlackTreeNodeType.Black;
                                rotate_right(mp);
                            }
                        }
                    }
                }
            }

            this.count--;
            node.Value = k; // n may have changed during remove
            return node; // the node that was deleted may be different from initial n
        }




        /// <summary>
        ///   Copies the nodes of this tree to an array, starting at a
        ///   particular <paramref name="arrayIndex">array index</paramref>.
        /// </summary>
        /// 
        /// <param name="array">
        ///   The one-dimensional array that is the destination of the elements
        ///   copied from this tree. The array must have zero-based indexing.
        /// </param>
        /// 
        /// <param name="arrayIndex">
        ///   The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// 
        public void CopyTo(RedBlackTreeNode<T>[] array, int arrayIndex)
        {
            foreach (var node in this)
                array[arrayIndex++] = node;
        }

        /// <summary>
        ///   Copies the elements of this tree to an array, starting at a
        ///   particular <paramref name="arrayIndex">array index</paramref>.
        /// </summary>
        /// 
        /// <param name="array">
        ///   The one-dimensional array that is the destination of the elements
        ///   copied from this tree. The array must have zero-based indexing.
        /// </param>
        /// 
        /// <param name="arrayIndex">
        ///   The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// 
        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var node in this)
                array[arrayIndex++] = node.Value;
        }

        /// <summary>
        ///   Gets a value indicating whether this instance is read only. 
        ///   In a <see cref="RedBlackTree{T}"/>, this returns false.
        /// </summary>
        /// 
        /// <value>
        ///    Returns <c>false</c>.
        /// </value>
        /// 
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through this tree in-order.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can
        ///   be used to traverse through this tree using in-order traversal.
        /// </returns>
        /// 
        public override IEnumerator<RedBlackTreeNode<T>> GetEnumerator()
        {
            RedBlackTreeNode<T> node = root;
            RedBlackTreeNode<T> lastNode = null;

            while (node != null)
            {
                if (lastNode == node.Parent)
                {
                    if (node.Left != null)
                    {
                        lastNode = node;
                        node = node.Left;
                        continue;
                    }
                    else
                    {
                        lastNode = null;
                    }
                }

                if (lastNode == node.Left)
                {
                    yield return node;

                    if (node.Right != null)
                    {
                        lastNode = node;
                        node = node.Right;
                        continue;
                    }
                    else
                    {
                        lastNode = null;
                    }
                }

                if (lastNode == node.Right)
                {
                    lastNode = node;
                    node = node.Parent;
                }
            }
        }
        
        /// <summary>
        ///   Returns an enumerator that iterates through this tree in-order.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can
        ///   be used to traverse through this tree using in-order traversal.
        /// </returns>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (RedBlackTreeNode<T> node in this)
                yield return node.Value;
        }

        /// <summary>
        ///   Determines whether this tree contains the specified item.
        /// </summary>
        /// 
        /// <param name="item">The item to be looked for.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the element was found inside the tree; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public bool Contains(T item)
        {
            return Find(item) != null;
        }

        /// <summary>
        ///   Determines whether this tree contains the specified item.
        /// </summary>
        /// 
        /// <param name="item">The item to be looked for.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the element was found inside the tree; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public bool Contains(RedBlackTreeNode<T> item)
        {
            return Find(item.Value) != null;
        }

        /// <summary>
        ///   Attempts to find a node that contains the specified key.
        /// </summary>
        /// 
        /// <param name="item">The key whose node is to be found.</param>
        /// 
        /// <returns>
        ///   A <see cref="RedBlackTreeNode{T}"/> containing the desired <paramref name="item"/>
        ///   if it is present in the dictionary; otherwise, returns <c>null</c>.
        /// </returns>
        /// 
        public RedBlackTreeNode<T> Find(T item)
        {
            var p = root;

            while (p != null)
            {
                int comp = compare.Compare(item, p.Value);

                if (comp == 0)
                    return p;

                p = comp <= 0 ? p.Left : p.Right;
            }

            return null;
        }

        /// <summary>
        ///  Finds the greatest point in the subtree rooted at <paramref name="node"/>
        ///  that is less than or equal to (&lt;=) <c>k</c>. In other words, finds either
        ///  <c>k</c> or a number immediately below it.
        /// </summary>
        /// 
        /// <param name="node">The subtree where search will take place.</param>
        /// <param name="value">A reference value to be found.</param>
        /// 
        /// <returns>
        ///   The node containing the given value <paramref name="value"/> or 
        ///   its immediately smaller neighboring number present in the tree.
        /// </returns>
        /// 
        public RedBlackTreeNode<T> FindLessThanOrEqualTo(RedBlackTreeNode<T> node, T value)
        {
            while (node != null)
            {
                if (compare.Compare(node.Value, value) <= 0)
                { 
                    // p.k <= k
                    var r = FindLessThanOrEqualTo(node.Right, value);

                    if (r != null)
                        return r;
                    return node;
                }
                else // if (p.k > k)
                {
                    node = node.Left;
                }
            }

            return null; // k < everything in subtree
        }

        /// <summary>
        ///  Finds the greatest point in the <see cref="RedBlackTree{T}">
        ///  tree</see> that is less than or equal to (&lt;=) <c>k</c>.
        ///  In other words, finds either <c>k</c> or a number immediately
        ///  below it.
        /// </summary>
        /// 
        /// <param name="value">A reference for the value to be found.</param>
        /// 
        /// <returns>
        ///   The node containing the given value <paramref name="value"/> or 
        ///   its immediately smaller neighboring number present in the tree.
        /// </returns>
        /// 
        public RedBlackTreeNode<T> FindLessThanOrEqualTo(T value)
        {
            return FindLessThanOrEqualTo(this.root, value);
        }

        /// <summary>
        ///  Finds the greatest point in the subtree rooted at <paramref name="node"/>
        ///  that is less than (&lt;) <c>k</c>. In other words, finds a number stored in
        ///  the tree that is immediately below <c>k</c>.
        /// </summary>
        /// 
        /// <param name="node">The subtree where search will take place.</param>
        /// <param name="value">A reference value to be found.</param>
        /// 
        /// <returns>
        ///   The node containing an element that is immediately below <paramref name="value"/>.
        /// </returns>
        /// 
        public RedBlackTreeNode<T> FindLessThan(RedBlackTreeNode<T> node, T value)
        {
            while (node != null)
            {
                if (compare.Compare(node.Value, value) < 0)
                {
                    // p.k < k
                    var r = FindLessThan(node.Right, value);

                    if (r != null)
                        return r;
                    return node;
                }
                else // if (p.k >= k)
                {
                    node = node.Left;
                }
            }
            return null; // k <= everything in subtree
        }

        /// <summary>
        ///  Finds the greatest point in the <see cref="RedBlackTree{T}">
        ///  tree</see> that is less than (&lt;) <c>k</c>. In other words, finds
        ///  a number stored in the tree that is immediately below <c>k</c>.
        /// </summary>
        /// 
        /// <param name="value">A reference value to be found.</param>
        /// 
        /// <returns>
        ///   The node containing an element that is immediately below <paramref name="value"/>.
        /// </returns>
        /// 
        public RedBlackTreeNode<T> FindLessThan(T value)
        {
            return FindLessThan(this.root, value);
        }

        /// <summary>
        ///  Finds the smallest point in the subtree rooted at <paramref name="node"/>
        ///  that is greater than (>) <c>k</c>. In other words, finds a number stored in
        ///  the tree that is immediately above <c>k</c>.
        /// </summary>
        /// 
        /// <param name="node">The subtree where search will take place.</param>
        /// <param name="value">A reference value to be found.</param>
        /// 
        /// <returns>
        ///   The node containing an element that is immediately below <paramref name="value"/>.
        /// </returns>
        /// 
        public RedBlackTreeNode<T> FindGreaterThan(RedBlackTreeNode<T> node, T value)
        {
            while (node != null)
            {
                if (compare.Compare(node.Value, value) > 0)
                {
                    // p.k > k
                    var l = FindGreaterThan(node.Left, value);

                    if (l != null)
                        return l;
                    return node;
                }
                else // if (p.k <= k)
                {
                    node = node.Right;
                }
            }

            return null; // k >= everything in subtree
        }

        /// <summary>
        ///  Finds the smallest point in the in the <see cref="RedBlackTree{T}">
        ///  tree</see> that is greater than (>) <c>k</c>. In other words, finds a
        ///  number stored in the tree that is immediately above <c>k</c>.
        /// </summary>
        /// 
        /// <param name="value">A reference value to be found.</param>
        /// 
        /// <returns>
        ///   The node containing an element that is immediately below <paramref name="value"/>.
        /// </returns>
        /// 
        public RedBlackTreeNode<T> FindGreaterThan(T value)
        {
            return FindGreaterThan(this.root, value);
        }

        /// <summary>
        ///   Finds the minimum element stored in the tree.
        /// </summary>
        /// 
        /// <returns>
        ///   The <see cref="RedBlackTreeNode{T}"/> that 
        ///   holds the minimum element in the tree.
        /// </returns>
        /// 
        public RedBlackTreeNode<T> Min()
        {
            var n = this.root;

            while (n != null && n.Left != null)
            {
                n = n.Left;
            }

            return n;
        }

        /// <summary>
        ///   Finds the maximum element stored in the tree.
        /// </summary>
        /// 
        /// <returns>
        ///   The <see cref="RedBlackTreeNode{T}"/> that
        ///   holds the maximum element in the tree.
        /// </returns>
        /// 
        public RedBlackTreeNode<T> Max()
        {
            var n = this.root;

            while (n != null && n.Right != null)
            {
                n = n.Right;
            }

            return n;
        }

        /// <summary>
        ///   Gets the node that contains the next in-order value coming 
        ///   after the value contained in the given <paramref name="node"/>.
        /// </summary>
        /// 
        /// <param name="node">The current node.</param>
        /// 
        /// <returns>
        ///   The node that contains a value that is immediately greater than
        ///   the current value contained in the given <paramref name="node"/>.
        /// </returns>
        /// 
        public RedBlackTreeNode<T> GetNextNode(RedBlackTreeNode<T> node)
        {
            if (node == null)
                return null;

            if (node.Right == null)
            {
                RedBlackTreeNode<T> prev;
                do
                {
                    prev = node;
                    node = node.Parent;
                } while (node != null && prev == node.Right);
            }
            else
            {
                node = node.Right;
                while (node.Left != null)
                    node = node.Left;
            }

            return node;
        }

        /// <summary>
        ///   Gets the node that contains the previous in-order value coming
        ///   before the value contained in the given <paramref name="node"/>.
        /// </summary>
        /// 
        /// <param name="node">The current node.</param>
        /// 
        /// <returns>
        ///   The node that contains a value that is immediately less than
        ///   the current value contained in the given <paramref name="node"/>.
        /// </returns>
        /// 
        public RedBlackTreeNode<T> GetPreviousNode(RedBlackTreeNode<T> node)
        {
            if (node == null)
                return null;

            if (node.Left == null)
            {
                RedBlackTreeNode<T> prev;

                do
                {
                    prev = node;
                    node = node.Parent;
                } while (node != null && prev == node.Left);
            }
            else
            {
                node = node.Left;
                while (node.Right != null)
                    node = node.Right;
            }

            return node;
        }

        /// <summary>
        ///   Forces a re-balance of the tree by removing and inserting the same node.
        /// </summary>
        /// 
        /// <param name="node">The node to be re-balanced.</param>
        /// 
        /// <returns>The same node, or a new one if it had to be recreated.</returns>
        /// 
        public RedBlackTreeNode<T> Resort(RedBlackTreeNode<T> node)
        {
            node = Remove(node);
            Add(node);
            return node;
        }



        private void rotate_left(RedBlackTreeNode<T> p)
        {
            var n = p.Right;
            p.Right = n.Left;
            n.Left = p;

            if (p.Parent != null)
            {
                if (p == p.Parent.Left)
                    p.Parent.Left = n;
                else p.Parent.Right = n;
            }
            else
            {
                this.root = n;
            }

            n.Parent = p.Parent;
            p.Parent = n;

            if (p.Right != null)
                p.Right.Parent = p;
        }

        private void rotate_right(RedBlackTreeNode<T> p)
        {
            var n = p.Left; /* must be non-null */

            p.Left = n.Right;
            n.Right = p;

            if (p.Parent != null)
            {
                if (p == p.Parent.Left)
                    p.Parent.Left = n;
                else p.Parent.Right = n;
            }
            else
            {
                this.root = n;
            }

            n.Parent = p.Parent;
            p.Parent = n;

            if (p.Left != null)
                p.Left.Parent = p;
        }



        internal bool check_node(RedBlackTreeNode<T> n, ref int nblack)
        {
            int nbl = 0;
            int nbr = 0;

            if (n == null)
            {
                nblack = 0;
                return true;
            }

            if (n.Right != null && n.Right.Parent != n)
                return false;

            if (n.Right != null && compare.Compare(n.Right.Value, n.Value) < 0)
                return false;

            if (n.Left != null && n.Left.Parent != n)
                return false;

            if (n.Left != null && compare.Compare(n.Left.Value, n.Value) > 0)
                return false;

            if (n.Color == RedBlackTreeNodeType.Red)
            {
                if (n.Right != null && n.Right.Color == RedBlackTreeNodeType.Red)
                    return false;

                if (n.Left != null && n.Left.Color == RedBlackTreeNodeType.Red)
                    return false;
            }

            if (!(check_node(n.Right, ref nbl) && check_node(n.Left, ref nbr)))
                return false;

            if (nbl != nbr)
                return false;

            nblack = nbl + (n.Color == RedBlackTreeNodeType.Black ? 1 : 0);

            return true;
        }

        internal bool check()
        {
            int nblack = 0;

            if (root == null)
                return true;

            if (root.Color != RedBlackTreeNodeType.Black)
                return false;

            return check_node(root, ref nblack);
        }


    }
}
