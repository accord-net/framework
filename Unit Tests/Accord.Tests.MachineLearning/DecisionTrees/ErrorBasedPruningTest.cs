// Accord Unit Tests
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

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning.DecisionTrees.Pruning;
    using NUnit.Framework;
    using System;
    using Accord.MachineLearning.DecisionTrees;
    using System.Data;
    using Accord.Statistics.Filters;
    using Accord.Math;
    using Accord.MachineLearning.DecisionTrees.Learning;


    [TestFixture]
    public class ErrorBasedPruningTest
    {

#if !NO_DATA_TABLE
        [Test]
        public void RunTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

            double[][] inputs;
            int[] outputs;

            int trainingSamplesCount = 6000;
            DecisionTree tree = ReducedErrorPruningTest.createNurseryExample(out inputs, out outputs, trainingSamplesCount);

            int nodeCount = 0;
            foreach (var node in tree)
                nodeCount++;

            var pruningInputs = inputs.Submatrix(trainingSamplesCount, inputs.Length - 1);
            var pruningOutputs = outputs.Submatrix(trainingSamplesCount, inputs.Length - 1);
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

            Assert.AreEqual(0.28922413793103446, error, 5e-4);
            Assert.AreEqual(447, nodeCount);
            Assert.AreEqual(424, nodeCount2);
        }


        [Test]
        public void RunTest3()
        {
            Accord.Math.Random.Generator.Seed = 0;

            double[][] inputs;
            int[] outputs;

            int training = 6000;
            DecisionTree tree = ReducedErrorPruningTest.createNurseryExample(out inputs, out outputs, training);

            double[] actual = new double[10];

            for (int i = 0; i < actual.Length; i++)
            {
                int nodeCount2;
                repeat(inputs, outputs, tree, training, i * 0.1, out nodeCount2);

                actual[i] = nodeCount2;
            }

            double[] expected = { 447, 424, 410, 402, 376, 362, 354, 348, 336, 322 };

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }

        private static void repeat(double[][] inputs, int[] outputs,
            DecisionTree tree, int training, double threshold,
            out int nodeCount2)
        {
            int nodeCount = 0;
            foreach (var node in tree)
                nodeCount++;

            var pruningInputs = inputs.Submatrix(training, inputs.Length - 1);
            var pruningOutputs = outputs.Submatrix(training, inputs.Length - 1);
            ErrorBasedPruning prune = new ErrorBasedPruning(tree, pruningInputs, pruningOutputs);

            prune.Threshold = threshold;

            double lastError;
            double error = Double.PositiveInfinity;

            do
            {
                lastError = error;
                error = prune.Run();
            } while (error < lastError);

            nodeCount2 = 0;
            foreach (var node in tree)
                nodeCount2++;
        }
#endif
    }
}
