﻿// Accord Unit Tests
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
    using Accord.Tests.MachineLearning.Properties;
    using Accord.MachineLearning.DecisionTrees.Learning;


    [TestFixture]
    public class ReducedErrorPruningTest
    {
#if !NO_DATA_TABLE
        [Test]
        public void RunTest()
        {
            double[][] inputs;
            int[] outputs;

            int training = 6000;
            DecisionTree tree = createNurseryExample(out inputs, out outputs, training);

            int nodeCount = 0;
            foreach (var node in tree)
                nodeCount++;

            var pruningInputs = inputs.Submatrix(training, inputs.Length - 1);
            var pruningOutputs = outputs.Submatrix(training, inputs.Length - 1);
            var prune = new ReducedErrorPruning(tree, pruningInputs, pruningOutputs);

            double lastError, error = Double.PositiveInfinity;
            do
            {
                lastError = error;
                error = prune.Run();
            } while (error <= lastError);

            int nodeCount2 = 0;
            foreach (var node in tree)
                nodeCount2++;

            Assert.AreEqual(0.19454022988505748, error, 5e-4);
            Assert.AreEqual(447, nodeCount);
            Assert.AreEqual(4, nodeCount2);
        }

        public static DecisionTree createNurseryExample(out double[][] inputs, out int[] outputs, int first)
        {
            string nurseryData = Resources.nursery;

            string[] inputColumns = 
            {
                "parents", "has_nurs", "form", "children",
                "housing", "finance", "social", "health"
            };

            string outputColumn = "output";

            DataTable table = new DataTable("Nursery");
            table.Columns.Add(inputColumns);
            table.Columns.Add(outputColumn);

            string[] lines = nurseryData.Split(
                new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            Assert.AreEqual(12960, lines.Length);
            Assert.AreEqual("usual,proper,complete,1,convenient,convenient,nonprob,recommended,recommend", lines[0]);
            Assert.AreEqual("great_pret,very_crit,foster,more,critical,inconv,problematic,not_recom,not_recom", lines[lines.Length - 1]);

            foreach (var line in lines)
                table.Rows.Add(line.Split(','));

            Codification codebook = new Codification(table);
            DataTable symbols = codebook.Apply(table);
            inputs = symbols.ToArray(inputColumns);
            outputs = symbols.ToArray<int>(outputColumn);

            var attributes = DecisionVariable.FromCodebook(codebook, inputColumns);
            var tree = new DecisionTree(attributes, classes: 5);

            C45Learning c45 = new C45Learning(tree);
            double error = c45.Run(inputs.First(first), outputs.First(first));

            Assert.AreEqual(0, error);

            return tree;
        }
#endif
    }
}
