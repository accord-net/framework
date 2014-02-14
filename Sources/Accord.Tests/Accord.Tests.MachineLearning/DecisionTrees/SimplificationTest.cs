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
    using System;
    using System.Data;
    using Accord.MachineLearning.DecisionTrees;
    using Accord.MachineLearning.DecisionTrees.Learning;
    using Accord.MachineLearning.DecisionTrees.Rules;
    using Accord.Math;
    using Accord.Statistics.Filters;
    using Accord.Tests.MachineLearning.Properties;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class SimplificationTest
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
        public void LargeRunTest()
        {

            double[][] inputs;
            int[] outputs;
            DecisionTree tree = createTree(out inputs, out outputs);

            var rules = DecisionSet.FromDecisionTree(tree);

            Simplification simpl = new Simplification(rules);
            double error = simpl.ComputeError(inputs, outputs);

            Assert.AreEqual(0, error);

            double newError = simpl.Compute(inputs, outputs);

            Assert.AreEqual(0.067515432098765427, newError);
        }

        [TestMethod]
        public void LargeRunTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            int[,] random = Matrix.Random(1000, 10, 0, 10).ToInt32();

            int[][] samples = random.ToArray();
            int[] outputs = new int[1000];

            for (int i = 0; i < samples.Length; i++)
            {
                if (samples[i][0] > 5 || Tools.Random.NextDouble() > 0.85)
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

        [TestMethod()]
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
                new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in lines)
                table.Rows.Add(line.Split(','));


            Codification codebook = new Codification(table);


            DataTable symbols = codebook.Apply(table);
            inputs = symbols.ToArray(inputColumns);
            outputs = symbols.ToArray<int>(outputColumn);


            var attributes = DecisionVariable.FromCodebook(codebook, inputColumns);
            var tree = new DecisionTree(attributes, outputClasses: 5);


            C45Learning c45 = new C45Learning(tree);

            c45.Run(inputs, outputs);

            return tree;
        }

    }
}
