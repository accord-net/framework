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

namespace Accord.MachineLearning.DecisionTrees.Learning
{
    using System;
    using System.Collections.Generic;
    using Accord.MachineLearning;
    using System.Collections;
    using Accord.Compat;
    using System.Threading.Tasks;
    using System.Linq;

    /// <summary>
    ///   Base class for tree inducing (learning) algorithms.
    /// </summary>
    /// 
    [Serializable]
    public class DecisionTreeLearningBase : ParallelLearningBase, IEnumerable<DecisionVariable>
    {

        private DecisionTree tree;

        private int maxHeight;
        private int maxVariables;

        private int join = 1;

        private IList<DecisionVariable> attributes;
        private int[] attributeUsageCount;

        /// <summary>
        ///   Gets or sets the maximum allowed height when learning a tree. If 
        ///   set to zero, the tree can have an arbitrary length. Default is 0.
        /// </summary>
        /// 
        public int MaxHeight
        {
            get { return maxHeight; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value",
                        "The height must be greater than or equal to zero.");
                }

                maxHeight = value;
            }
        }

        /// <summary>
        ///   Gets or sets the maximum number of variables that
        ///   can enter the tree. A value of zero indicates there
        ///   is no limit. Default is 0 (there is no limit on the
        ///   number of variables).
        /// </summary>
        /// 
        public int MaxVariables
        {
            get { return maxVariables; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value",
                        "The height must be greater than or equal to zero.");
                }

                maxVariables = value;
            }
        }

        /// <summary>
        ///   Gets or sets the collection of attributes to 
        ///   be processed by the induced decision tree.
        /// </summary>
        /// 
        public IList<DecisionVariable> Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }

        /// <summary>
        ///   Gets or sets how many times one single variable can be integrated into the decision process. In the original
        ///   ID3 algorithm, a variable can join only one time per decision path (path from the root to a leaf). If set to
        ///   zero, a single variable can participate as many times as needed. Default is 1.
        /// </summary>
        /// 
        public int Join
        {
            get { return join; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value",
                        "The number of times must be greater than or equal to zero.");
                }

                join = value;
            }
        }

        /// <summary>
        ///   Gets or sets the decision trees being learned.
        /// </summary>
        /// 
        public DecisionTree Model
        {
            get { return tree; }
            set { tree = value; }
        }

        /// <summary>
        ///   Gets how many times each attribute has already been used in the current path.
        ///   In the original C4.5 and ID3 algorithms, attributes could be re-used only once,
        ///   but in the framework implementation this behaviour can be adjusted by setting 
        ///   the <see cref="Join"/> property.
        /// </summary>
        /// 
        protected int[] AttributeUsageCount
        {
            get { return attributeUsageCount; }
            set { attributeUsageCount = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecisionTreeLearningBase"/> class.
        /// </summary>
        public DecisionTreeLearningBase()
        {
            this.ParallelOptions = new ParallelOptions();
            this.attributes = new List<DecisionVariable>();
        }

        /// <param name="attributes">The attributes to be processed by the induced tree.</param>
        //
        public DecisionTreeLearningBase(DecisionVariable[] attributes)
        {
            this.attributes = new List<DecisionVariable>(attributes);
        }

        /// <summary>
        ///   Adds the specified variable to the list of <see cref="Attribute"/>s.
        /// </summary>
        /// 
        public void Add(DecisionVariable variable)
        {
            this.attributes.Add(variable);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<DecisionVariable> GetEnumerator()
        {
            return attributes.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return attributes.GetEnumerator();
        }

        /// <summary>
        ///   Computes the split information measure.
        /// </summary>
        /// 
        /// <param name="samples">The total number of samples.</param>
        /// <param name="partitions">The partitioning.</param>
        /// <param name="missing">An extra partition containing only missing values.</param>
        /// 
        /// <returns>The split information for the given partitions.</returns>
        /// 
        public static double SplitInformation(int samples, IList<int>[] partitions, List<int> missing = null)
        {
            double info = 0;

            for (int i = 0; i < partitions.Length; i++)
            {
                if (partitions[i] == null)
                    continue;

                double p = partitions[i].Count / (double)samples;

                if (p != 0)
                    info -= p * Math.Log(p, 2);
            }

            if (missing != null)
            {
                double p = missing.Count / (double)samples;

                if (p != 0)
                    info -= p * Math.Log(p, 2);
            }

#if DEBUG
            int totalSum = partitions.Sum(x => x == null ? 0 : x.Count);
            int missingSum = missing == null ? 0 : missing.Count;
            Accord.Diagnostics.Debug.Assert(samples == (totalSum + missingSum));
#endif

            return info;
        }
    }
}
