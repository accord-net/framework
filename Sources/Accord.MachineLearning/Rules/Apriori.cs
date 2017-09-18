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

namespace Accord.MachineLearning.Rules
{
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using Accord.Compat;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///   A-priori algorithm for association rule mining.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Anita Wasilewska, Lecture Notes. Available on 
    ///       http://www3.cs.stonybrook.edu/~cse634/lecture_notes/07apriori.pdf </description></item>
    ///    </list>></para>
    /// </remarks>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\AprioriTest.cs" region="doc_learn_1" lang="cs"/>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\AprioriTest.cs" region="doc_learn_2" lang="cs"/>
    /// </example>
    /// 
    /// <seealso cref="AssociationRule{T}"/>
    /// <seealso cref="AssociationRuleMatcher{T}"/>
    ///
    public class Apriori : Apriori<int>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="Apriori"/> class.
        /// </summary>
        /// <param name="threshold">The minimum number of times a rule should be detected (also known as its support) 
        ///   before it can be registered as a permanent in the learned classifier.</param>
        /// <param name="confidence">The minimum confidence in an association rule beore it is 
        ///   registered.</param>
        ///   
        public Apriori(int threshold, double confidence)
            : base(threshold, confidence)
        {
        }
    }

    /// <summary>
    ///   A-priori algorithm for association rule mining.
    /// </summary>
    /// 
    /// <typeparam name="T">The dataset item type. Default is int.</typeparam>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Anita Wasilewska, Lecture Notes. Available on 
    ///       http://www3.cs.stonybrook.edu/~cse634/lecture_notes/07apriori.pdf </description></item>
    ///    </list>></para>
    /// </remarks>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\AprioriTest.cs" region="doc_learn_1" lang="cs"/>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\AprioriTest.cs" region="doc_learn_2" lang="cs"/>
    /// </example>
    /// 
    /// <seealso cref="AssociationRule{T}"/>
    /// <seealso cref="AssociationRuleMatcher{T}"/>
    ///
    public class Apriori<T> :
        IUnsupervisedLearning<AssociationRuleMatcher<T>, SortedSet<T>, SortedSet<T>[]>,
        IUnsupervisedLearning<AssociationRuleMatcher<T>, T[], T[][]>
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        private int supportMin;
        private double confidence;
        private Dictionary<SortedSet<T>, int> frequent;

        /// <summary>
        ///   Gets the set of most frequent items and the respective 
        ///   number of times their appear in in the training dataset.
        /// </summary>
        /// 
        public Dictionary<SortedSet<T>, int> Frequent
        {
            get { return frequent; }
        }

        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        /// <value>The token.</value>
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Apriori{T}"/> class.
        /// </summary>
        /// 
        /// <param name="threshold">The minimum number of times a rule should be detected (also known as its support) 
        ///   before it can be registered as a permanent in the learned classifier.</param>
        /// <param name="confidence">The minimum confidence in an association rule beore it is 
        ///   registered.</param>
        ///   
        public Apriori(int threshold, double confidence)
        {
            this.supportMin = threshold;
            this.confidence = confidence;
            this.frequent = new Dictionary<SortedSet<T>, int>(new SetComparer());
        }


        /// <summary>
        ///   Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// 
        /// <returns>A model that has learned how to produce suitable outputs
        ///   given the input data <paramref name="x"/>.</returns>
        /// 
        public AssociationRuleMatcher<T> Learn(T[][] x, double[] weights = null)
        {
            return Learn(x.Apply(xi => new SortedSet<T>(xi)), weights);
        }

        /// <summary>
        ///   Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// 
        /// <returns>A model that has learned how to produce suitable outputs
        ///   given the input data <paramref name="x"/>.</returns>
        /// 
        public AssociationRuleMatcher<T> Learn(SortedSet<T>[] x, double[] weights = null)
        {
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            frequent.Clear();
            var L = new HashSet<SortedSet<T>>(new SetComparer());
            var counts = new Dictionary<SortedSet<T>, int>(new SetComparer());

            foreach (var t in x)
                foreach (var s in t)
                    L.Add(new SortedSet<T>() { s });
            int items = L.Count;

            for (int k = 1; L.Count != 0; k++)
            {
                var C = fold(L, k);
                counts.Clear();
                foreach (SortedSet<T> transaction in x)
                {
                    foreach (SortedSet<T> candidate in C)
                    {
                        if (candidate.IsSubsetOf(transaction))
                        {
                            int count;
                            if (counts.TryGetValue(candidate, out count))
                                counts[candidate] = count + 1;
                            else
                                counts[candidate] = 1;
                        }
                    }
                }

                L.Clear();
                foreach (var pair in counts)
                {
                    if (pair.Value >= supportMin)
                    {
                        L.Add(pair.Key);
                        frequent.Add(pair.Key, pair.Value);
                    }
                }
            }

            var rules = new List<AssociationRule<T>>();

            // Generate association rules from the most frequent itemsets
            foreach (var itemset in frequent.Keys)
            {
                double total = support(itemset, x);

                // generate all non-empty subsets of I
                foreach (var s in itemset.Subsets())
                {
                    double subtotal = support(s, x);
                    double sc = total / subtotal;
                    if (sc >= confidence && subtotal >= supportMin)
                    {
                        var y = new SortedSet<T>(itemset);
                        y.ExceptWith(s);
                        if (y.Count > 0)
                        {
                            rules.Add(new AssociationRule<T>()
                            {
                                Confidence = sc,
                                Support = total,
                                X = s,
                                Y = y,
                            });
                        }
                    }
                }
            }

            return new AssociationRuleMatcher<T>(items, rules.ToArray());
        }

        private int support(ISet<T> set, IList<SortedSet<T>> dataset)
        {
            int count = 0;
            foreach (var itemset in dataset)
                if (set.IsSubsetOf(itemset))
                    count++;

            return count;
        }


        // TODO: Move to a public class
        internal class SetComparer : IEqualityComparer<SortedSet<T>>
        {
            public bool Equals(SortedSet<T> x, SortedSet<T> y)
            {
                return x.SetEquals(y);
            }

            public int GetHashCode(SortedSet<T> obj)
            {
                // TODO: Change to something more efficient
                int sum = 0;
                foreach (var i in obj)
                    sum ^= i.GetHashCode();
                return sum;
            }
        }

        HashSet<SortedSet<T>> fold(HashSet<SortedSet<T>> L, int k)
        {
            var r = new HashSet<SortedSet<T>>(new SetComparer());

            foreach (var i in L)
            {
                foreach (var j in L)
                {
                    var t = new SortedSet<T>(i);
                    t.UnionWith(j);
                    if (t.Count == k)
                        r.Add(t);
                }
            }

            return r;
        }
    }
}
