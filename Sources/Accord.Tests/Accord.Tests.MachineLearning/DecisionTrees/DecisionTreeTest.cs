// Accord Unit Tests
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

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning.DecisionTrees;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using AForge;
    using Accord.Statistics.Filters;
    using System.Data;
    using Accord.Math;

    [TestClass()]
    public class DecisionTreeTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod()]
        public void ComputeTest()
        {

            DecisionTree tree;
            int[][] inputs;
            int[] outputs;

            ID3LearningTest.CreateMitchellExample(out tree, out inputs, out outputs);

            Assert.AreEqual(4, tree.InputCount);
            Assert.AreEqual(2, tree.OutputClasses);


            for (int i = 0; i < inputs.Length; i++)
            {
                int y = tree.Compute(inputs[i].ToDouble());
                Assert.AreEqual(outputs[i], y);
            }

        }

        [TestMethod()]
        public void EnumerateTest()
        {
            DecisionTree tree;
            int[][] inputs;
            int[] outputs;

            ID3LearningTest.CreateMitchellExample(out tree, out inputs, out outputs);


            DecisionNode[] expected = 
            {
                tree.Root,
                tree.Root.Branches[0], // Outlook = 0
                tree.Root.Branches[0].Branches[0], // Humidity = 0
                tree.Root.Branches[0].Branches[1], // Humidity = 1
                tree.Root.Branches[1], // Outlook = 1
                tree.Root.Branches[2], // Outlook = 2
                tree.Root.Branches[2].Branches[0], // Wind = 0
                tree.Root.Branches[2].Branches[1], // Wind = 1
            };

            int i = 0;
            foreach (var node in tree)
            {
                Assert.AreEqual(expected[i++], node);
            }

            Assert.AreEqual(expected.Length, i);
        }

        [TestMethod()]
        public void TraverseTest()
        {
            DecisionTree tree;
            int[][] inputs;
            int[] outputs;

            ID3LearningTest.CreateMitchellExample(out tree, out inputs, out outputs);


            {
                DecisionNode[] expected = 
                {
                    tree.Root,
                    tree.Root.Branches[0], // Outlook = 0
                    tree.Root.Branches[1], // Outlook = 1
                    tree.Root.Branches[2], // Outlook = 2
                    tree.Root.Branches[0].Branches[0], // Humidity = 0
                    tree.Root.Branches[0].Branches[1], // Humidity = 1
                    tree.Root.Branches[2].Branches[0], // Wind = 0
                    tree.Root.Branches[2].Branches[1], // Wind = 1
                };

                int i = 0;
                foreach (var node in tree.Traverse(DecisionTreeTraversal.BreadthFirst))
                {
                    Assert.AreEqual(expected[i++], node);
                }
                Assert.AreEqual(expected.Length, i);
            }

            {
                DecisionNode[] expected = 
                {
                    tree.Root,
                    tree.Root.Branches[0], // Outlook = 0
                    tree.Root.Branches[0].Branches[0], // Humidity = 0
                    tree.Root.Branches[0].Branches[1], // Humidity = 1
                    tree.Root.Branches[1], // Outlook = 1
                    tree.Root.Branches[2], // Outlook = 2
                    tree.Root.Branches[2].Branches[0], // Wind = 0
                    tree.Root.Branches[2].Branches[1], // Wind = 1
                };

                int i = 0;
                foreach (var node in tree.Traverse(DecisionTreeTraversal.DepthFirst))
                {
                    Assert.AreEqual(expected[i++], node);
                }
                Assert.AreEqual(expected.Length, i);
            }

            {
                DecisionNode[] expected = 
                {
                    tree.Root.Branches[0].Branches[0], // Humidity = 0
                    tree.Root.Branches[0].Branches[1], // Humidity = 1
                    tree.Root.Branches[0], // Outlook = 0
                    tree.Root.Branches[1], // Outlook = 1
                    tree.Root.Branches[2].Branches[0], // Wind = 0
                    tree.Root.Branches[2].Branches[1], // Wind = 1
                    tree.Root.Branches[2], // Outlook = 2
                    tree.Root,
                };

                int i = 0;
                foreach (var node in tree.Traverse(DecisionTreeTraversal.PostOrder))
                {
                    Assert.AreEqual(expected[i++], node);
                }
                Assert.AreEqual(expected.Length, i);
            }
        }

    }
}
