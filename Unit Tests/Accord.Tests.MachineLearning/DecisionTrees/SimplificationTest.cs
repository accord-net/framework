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
    using System;
    using System.Data;
    using Accord.MachineLearning.DecisionTrees;
    using Accord.MachineLearning.DecisionTrees.Learning;
    using Accord.MachineLearning.DecisionTrees.Rules;
    using Accord.Math;
    using Accord.Statistics.Filters;
    using NUnit.Framework;
    using Accord.Tests.MachineLearning.Properties;

    [TestFixture]
    public class SimplificationTest
    {
#if !NO_DATA_TABLE
        [Test]
        public void LargeRunTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

            double[][] inputs;
            int[] outputs;
            DecisionTree tree = createTree(out inputs, out outputs);

            var rules = DecisionSet.FromDecisionTree(tree);

            Simplification simpl = new Simplification(rules);
            double error = simpl.ComputeError(inputs, outputs);

            Assert.AreEqual(0, error);

            double newError = simpl.Compute(inputs, outputs);

            Assert.AreEqual(0.067515432098765427, newError, 1e-6);
        }
#endif

        [Test]
        public void LargeRunTest2()
        {
            Accord.Math.Random.Generator.Seed = 0;

            int[,] random = Matrix.Random(1000, 10, 0.0, 10.0).ToInt32();

            int[][] samples = random.ToJagged();
            int[] outputs = new int[1000];

            for (int i = 0; i < samples.Length; i++)
            {
                if (samples[i][0] > 5 || Accord.Math.Tools.Random.NextDouble() > 0.85)
                    outputs[i] = 1;
            }

            DecisionVariable[] vars = new DecisionVariable[10];
            for (int i = 0; i < vars.Length; i++)
                vars[i] = new DecisionVariable("x" + i, 10);

            DecisionTree tree = new DecisionTree(vars, 2);

            var teacher = new ID3Learning(tree);

            double error = teacher.Run(samples, outputs);

            Assert.AreEqual(0, error);

            var rules = DecisionSet.FromDecisionTree(tree);

            Simplification simpl = new Simplification(rules)
            {
                Alpha = 0.05
            };

            error = simpl.ComputeError(samples.ToDouble(), outputs);
            Assert.AreEqual(0, error);

            double newError = simpl.Compute(samples.ToDouble(), outputs);

            Assert.AreEqual(0.097, newError);
        }

        [Test]
        public void HypothesisTest()
        {
            bool[] actual =
            {
                true, true, true, true, // 4
                true, true, true, true, // 8
                true, true, true, true, // 12
                true, true, true, true, // 16
                true, true, true, true, // 20
                true, true, true, true, // 24
                true, true, true, true, // 28
                true, true, true, true, // 32

                false, false, false, false, // 4
                false, false, false, false, // 8
                false, false, false, false, // 12
                false, false, false, false, // 16
                false, false, false, false, // 20
                false, false, false, false, // 24
                false, false, false, false, // 28
                false, false, false, false, // 32
            };

            bool[] expected =
            {
                true, true, true, true, // 4
                true, true, true, true, // 8
                true, true, true, true, // 12
                true, true, true, true, // 16
                false, false, false, false, // 4
                false, false, false, false, // 8
                false, false, false, false, // 12
                false, false, false, false, // 16

                false, false, false, false, // 4
                false, false, false, false, // 8
                false, false, false, false, // 12
                false, false, false, false, // 16
                false, false, false, false, // 20
                true, true, true, true, // 4
                true, true, true, true, // 8
                true, true, true, true, // 12
            };

            Assert.IsTrue(Simplification.CanEliminate(actual, expected, 0.05));
            Assert.IsFalse(Simplification.CanEliminate(expected, expected, 0.05));
            Assert.IsFalse(Simplification.CanEliminate(actual, actual, 0.05));
        }


#if !NO_DATA_TABLE
        private static DecisionTree createTree(out double[][] inputs, out int[] outputs)
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

            foreach (var line in lines)
                table.Rows.Add(line.Split(','));

            Assert.AreEqual(12960, lines.Length);
            Assert.AreEqual("usual,proper,complete,1,convenient,convenient,nonprob,recommended,recommend", lines[0]);
            Assert.AreEqual("great_pret,very_crit,foster,more,critical,inconv,problematic,not_recom,not_recom", lines[lines.Length - 1]);


            Codification codebook = new Codification(table);


            DataTable symbols = codebook.Apply(table);
            inputs = symbols.ToArray(inputColumns);
            outputs = symbols.ToArray<int>(outputColumn);

            Assert.AreEqual(12960, inputs.Rows());
            Assert.AreEqual(8, inputs.Columns());
            Assert.AreEqual(12960, outputs.Length);
            Assert.AreEqual(4, outputs.Max());
            Assert.AreEqual(0, outputs.Min());
            Assert.AreEqual(5, outputs.DistinctCount());


            var attributes = DecisionVariable.FromCodebook(codebook, inputColumns);
            var tree = new DecisionTree(attributes, classes: 5);

            Assert.AreEqual(8, tree.NumberOfInputs);
            Assert.AreEqual(5, tree.NumberOfOutputs);

            C45Learning c45 = new C45Learning(tree);

            double error = c45.Run(inputs, outputs);

            Assert.AreEqual(8, tree.Attributes.Count);
            for (int i = 0; i < tree.Attributes.Count; i++)
            { 
                Assert.AreEqual(tree.Attributes[i].Nature, DecisionVariableKind.Discrete);
                Assert.AreEqual(tree.Attributes[i].Range.Min, 0);
            }

            Assert.AreEqual(tree.Attributes[0].Name, "parents");
            Assert.AreEqual(tree.Attributes[0].Range.Max, 2);
            Assert.AreEqual(tree.Attributes[1].Name, "has_nurs");
            Assert.AreEqual(tree.Attributes[1].Range.Max, 4);
            Assert.AreEqual(tree.Attributes[2].Name, "form");
            Assert.AreEqual(tree.Attributes[2].Range.Max, 3);
            Assert.AreEqual(tree.Attributes[3].Name, "children");
            Assert.AreEqual(tree.Attributes[3].Range.Max, 3);
            Assert.AreEqual(tree.Attributes[4].Name, "housing");
            Assert.AreEqual(tree.Attributes[4].Range.Max, 2);
            Assert.AreEqual(tree.Attributes[5].Name, "finance");
            Assert.AreEqual(tree.Attributes[5].Range.Max, 1);
            Assert.AreEqual(tree.Attributes[6].Name, "social");
            Assert.AreEqual(tree.Attributes[6].Range.Max, 2);
            Assert.AreEqual(tree.Attributes[7].Name, "health");
            Assert.AreEqual(tree.Attributes[7].Range.Max, 2);


            Assert.AreEqual(8, tree.NumberOfInputs);
            Assert.AreEqual(5, tree.NumberOfOutputs);
            Assert.AreEqual(0, error);

            return tree;
        }
#endif
    }
}
