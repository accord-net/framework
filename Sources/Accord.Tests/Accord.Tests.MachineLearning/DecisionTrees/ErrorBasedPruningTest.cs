// Accord Unit Tests
// The Accord.NET Framework
// http://accord.googlecode.com
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

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning.DecisionTrees.Pruning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.MachineLearning.DecisionTrees;
    using System.Data;
    using Accord.Statistics.Filters;
    using Accord.Math;
    using Accord.Tests.MachineLearning.Properties;
    using Accord.MachineLearning.DecisionTrees.Learning;
    
    
    [TestClass()]
    public class ErrorBasedpruningTest
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod()]
        public void RunTest()
        {
            double[][] inputs;
            int[] outputs;

            int training = 6000;
            DecisionTree tree = ReducedErrorPruningTest.createNurseryExample(out inputs, out outputs, training);

            int nodeCount = 0;
            foreach (var node in tree)
                nodeCount++;

            var pruningInputs = inputs.Submatrix(training, inputs.Length - 1);
            var pruningOutputs = outputs.Submatrix(training, inputs.Length - 1);
            ErrorBasedPruning prune = new ErrorBasedPruning(tree, pruningInputs, pruningOutputs);

            prune.Threshold = 0.1;

            double lastError, error = Double.PositiveInfinity;
            do
            {
                lastError = error;
                error = prune.Run();
            } while (error < lastError);

            int nodeCount2 = 0;
            foreach (var node in tree)
                nodeCount2++;

            Assert.AreEqual(0.25459770114942532, error);
            Assert.AreEqual(447, nodeCount);
            Assert.AreEqual(193, nodeCount2);
        }

    }
}
