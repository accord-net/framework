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
    using Accord.MachineLearning.DecisionTrees;
    using Accord.MachineLearning.DecisionTrees.Learning;
    using Accord.Math;
    using Accord.Statistics.Filters;
    using AForge;
    using Math.Optimization.Losses;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Data;

    [TestFixture]
    public class ID3LearningTest
    {
#if !NO_DATA_TABLE
        public static void CreateMitchellExample(out DecisionTree tree, out int[][] inputs, out int[] outputs)
        {
            DataTable data = new DataTable("Mitchell's Tennis Example");

            data.Columns.Add("Day", "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");

            data.Rows.Add("D1", "Sunny", "Hot", "High", "Weak", "No");
            data.Rows.Add("D2", "Sunny", "Hot", "High", "Strong", "No");
            data.Rows.Add("D3", "Overcast", "Hot", "High", "Weak", "Yes");
            data.Rows.Add("D4", "Rain", "Mild", "High", "Weak", "Yes");
            data.Rows.Add("D5", "Rain", "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D6", "Rain", "Cool", "Normal", "Strong", "No");
            data.Rows.Add("D7", "Overcast", "Cool", "Normal", "Strong", "Yes");
            data.Rows.Add("D8", "Sunny", "Mild", "High", "Weak", "No");
            data.Rows.Add("D9", "Sunny", "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D10", "Rain", "Mild", "Normal", "Weak", "Yes");
            data.Rows.Add("D11", "Sunny", "Mild", "Normal", "Strong", "Yes");
            data.Rows.Add("D12", "Overcast", "Mild", "High", "Strong", "Yes");
            data.Rows.Add("D13", "Overcast", "Hot", "Normal", "Weak", "Yes");
            data.Rows.Add("D14", "Rain", "Mild", "High", "Strong", "No");

            // Create a new codification codebook to
            // convert strings into integer symbols
            Codification codebook = new Codification(data,
                "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");

            DecisionVariable[] attributes =
            {
               new DecisionVariable("Outlook",     codebook["Outlook"].Symbols),     // 3 possible values (Sunny, overcast, rain)
               new DecisionVariable("Temperature", codebook["Temperature"].Symbols), // 3 possible values (Hot, mild, cool)
               new DecisionVariable("Humidity",    codebook["Humidity"].Symbols),    // 2 possible values (High, normal)
               new DecisionVariable("Wind",        codebook["Wind"].Symbols)         // 2 possible values (Weak, strong)
            };

            int classCount = codebook["PlayTennis"].Symbols; // 2 possible values (yes, no)

            tree = new DecisionTree(attributes, classCount);
            ID3Learning id3 = new ID3Learning(tree);

            // Extract symbols from data and train the classifier
            DataTable symbols = codebook.Apply(data);
            inputs = symbols.ToArray<int>("Outlook", "Temperature", "Humidity", "Wind");
            outputs = symbols.ToArray<int>("PlayTennis");

            double error = id3.Run(inputs, outputs);
            Assert.AreEqual(0, error);


            {
                int[] query = codebook.Translate("Sunny", "Hot", "High", "Strong");

                int output = tree.Compute(query);

                string answer = codebook.Translate("PlayTennis", output);

                Assert.AreEqual("No", answer);
            }


            foreach (DataRow row in data.Rows)
            {
                var x = codebook.Translate(row, "Outlook", "Temperature", "Humidity", "Wind");

                int y = tree.Compute(x);

                string actual = codebook.Translate("PlayTennis", y);
                string expected = row["PlayTennis"] as string;

                Assert.AreEqual(expected, actual);
            }

            {
                string answer = codebook.Translate("PlayTennis",
                    tree.Compute(codebook.Translate("Sunny", "Hot", "High", "Strong")));

                Assert.AreEqual("No", answer);
            }
        }
#endif

        public static void CreateXORExample(out DecisionTree tree, out int[][] inputs, out int[] outputs)
        {
            inputs = new int[][]
            {
                new int[] { 1, 0, 0, 1 },
                new int[] { 0, 1, 0, 0 },
                new int[] { 0, 0, 0, 0 },
                new int[] { 1, 1, 0, 0 },
                new int[] { 0, 1, 1, 1 },
                new int[] { 0, 0, 1, 1 },
                new int[] { 1, 0, 1, 1 }
            };

            outputs = new int[]
            {
                1, 1, 0, 0, 1, 0, 1
            };

            DecisionVariable[] attributes =
            {
               new DecisionVariable("a1", 2),
               new DecisionVariable("a2", 2),
               new DecisionVariable("a3", 2),
               new DecisionVariable("a4", 2)
            };

            int classCount = 2;

            tree = new DecisionTree(attributes, classCount);
            ID3Learning id3 = new ID3Learning(tree);


            double error = id3.Run(inputs, outputs);

            Assert.AreEqual(0, error);
        }

        [Test]
        public void RunTest()
        {
            int[][] inputs =
            {
                new int[] { 0, 0 },
                new int[] { 0, 1 },
                new int[] { 1, 0 },
                new int[] { 1, 1 },
            };

            int[] outputs = // xor
            {
                0,
                1,
                1,
                0
            };

            DecisionVariable[] attributes =
            {
                new DecisionVariable("x", DecisionVariableKind.Discrete),
                new DecisionVariable("y", DecisionVariableKind.Discrete),
            };


            DecisionTree tree = new DecisionTree(attributes, 2);

            ID3Learning teacher = new ID3Learning(tree);

            double error = teacher.Run(inputs, outputs);

            Assert.AreEqual(0, error);

            Assert.AreEqual(0, tree.Root.Branches.AttributeIndex); // x
            Assert.AreEqual(2, tree.Root.Branches.Count);
            Assert.IsNull(tree.Root.Value);
            Assert.IsNull(tree.Root.Value);

            Assert.AreEqual(0.0, tree.Root.Branches[0].Value); // x = [0]
            Assert.AreEqual(1.0, tree.Root.Branches[1].Value); // x = [1]

            Assert.AreEqual(tree.Root, tree.Root.Branches[0].Parent);
            Assert.AreEqual(tree.Root, tree.Root.Branches[1].Parent);

            Assert.AreEqual(2, tree.Root.Branches[0].Branches.Count);
            Assert.AreEqual(2, tree.Root.Branches[1].Branches.Count);

            Assert.IsTrue(tree.Root.Branches[0].Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[0].Branches[1].IsLeaf);

            Assert.IsTrue(tree.Root.Branches[1].Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[1].Branches[1].IsLeaf);

            Assert.AreEqual(0.0, tree.Root.Branches[0].Branches[0].Value); // y = [0]
            Assert.AreEqual(1.0, tree.Root.Branches[0].Branches[1].Value); // y = [1]

            Assert.AreEqual(0.0, tree.Root.Branches[1].Branches[0].Value); // y = [0]
            Assert.AreEqual(1.0, tree.Root.Branches[1].Branches[1].Value); // y = [1]

            Assert.AreEqual(0, tree.Root.Branches[0].Branches[0].Output); // 0 ^ 0 = 0
            Assert.AreEqual(1, tree.Root.Branches[0].Branches[1].Output); // 0 ^ 1 = 1
            Assert.AreEqual(1, tree.Root.Branches[1].Branches[0].Output); // 1 ^ 0 = 1
            Assert.AreEqual(0, tree.Root.Branches[1].Branches[1].Output); // 1 ^ 1 = 0
        }

        [Test]
        public void learn_test()
        {
            int[][] inputs =
            {
                new int[] { 0, 0 },
                new int[] { 0, 1 },
                new int[] { 1, 0 },
                new int[] { 1, 1 },
            };

            int[] outputs = // xor
            {
                0,
                1,
                1,
                0
            };

            DecisionVariable[] attributes =
            {
                new DecisionVariable("x", DecisionVariableKind.Discrete),
                new DecisionVariable("y", DecisionVariableKind.Discrete),
            };


            ID3Learning teacher = new ID3Learning(attributes);

            var tree = teacher.Learn(inputs, outputs);

            double error = teacher.ComputeError(inputs, outputs);

            Assert.AreEqual(0, error);

            Assert.AreEqual(0, tree.Root.Branches.AttributeIndex); // x
            Assert.AreEqual(2, tree.Root.Branches.Count);
            Assert.IsNull(tree.Root.Value);
            Assert.IsNull(tree.Root.Value);

            Assert.AreEqual(0.0, tree.Root.Branches[0].Value); // x = [0]
            Assert.AreEqual(1.0, tree.Root.Branches[1].Value); // x = [1]

            Assert.AreEqual(tree.Root, tree.Root.Branches[0].Parent);
            Assert.AreEqual(tree.Root, tree.Root.Branches[1].Parent);

            Assert.AreEqual(2, tree.Root.Branches[0].Branches.Count);
            Assert.AreEqual(2, tree.Root.Branches[1].Branches.Count);

            Assert.IsTrue(tree.Root.Branches[0].Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[0].Branches[1].IsLeaf);

            Assert.IsTrue(tree.Root.Branches[1].Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[1].Branches[1].IsLeaf);

            Assert.AreEqual(0.0, tree.Root.Branches[0].Branches[0].Value); // y = [0]
            Assert.AreEqual(1.0, tree.Root.Branches[0].Branches[1].Value); // y = [1]

            Assert.AreEqual(0.0, tree.Root.Branches[1].Branches[0].Value); // y = [0]
            Assert.AreEqual(1.0, tree.Root.Branches[1].Branches[1].Value); // y = [1]

            Assert.AreEqual(0, tree.Root.Branches[0].Branches[0].Output); // 0 ^ 0 = 0
            Assert.AreEqual(1, tree.Root.Branches[0].Branches[1].Output); // 0 ^ 1 = 1
            Assert.AreEqual(1, tree.Root.Branches[1].Branches[0].Output); // 1 ^ 0 = 1
            Assert.AreEqual(0, tree.Root.Branches[1].Branches[1].Output); // 1 ^ 1 = 0
        }

        [Test]
        public void learn_test_automatic()
        {
            int[][] inputs =
            {
                new int[] { 0, 0 },
                new int[] { 0, 1 },
                new int[] { 1, 0 },
                new int[] { 1, 1 },
            };

            int[] outputs = // xor
            {
                0,
                1,
                1,
                0
            };

            ID3Learning teacher = new ID3Learning();

            var tree = teacher.Learn(inputs, outputs);

            double error = teacher.ComputeError(inputs, outputs);

            Assert.AreEqual(0, error);

            Assert.AreEqual(0, tree.Root.Branches.AttributeIndex); // x
            Assert.AreEqual(2, tree.Root.Branches.Count);
            Assert.IsNull(tree.Root.Value);
            Assert.IsNull(tree.Root.Value);

            Assert.AreEqual(0.0, tree.Root.Branches[0].Value); // x = [0]
            Assert.AreEqual(1.0, tree.Root.Branches[1].Value); // x = [1]

            Assert.AreEqual(tree.Root, tree.Root.Branches[0].Parent);
            Assert.AreEqual(tree.Root, tree.Root.Branches[1].Parent);

            Assert.AreEqual(2, tree.Root.Branches[0].Branches.Count);
            Assert.AreEqual(2, tree.Root.Branches[1].Branches.Count);

            Assert.IsTrue(tree.Root.Branches[0].Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[0].Branches[1].IsLeaf);

            Assert.IsTrue(tree.Root.Branches[1].Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[1].Branches[1].IsLeaf);

            Assert.AreEqual(0.0, tree.Root.Branches[0].Branches[0].Value); // y = [0]
            Assert.AreEqual(1.0, tree.Root.Branches[0].Branches[1].Value); // y = [1]

            Assert.AreEqual(0.0, tree.Root.Branches[1].Branches[0].Value); // y = [0]
            Assert.AreEqual(1.0, tree.Root.Branches[1].Branches[1].Value); // y = [1]

            Assert.AreEqual(0, tree.Root.Branches[0].Branches[0].Output); // 0 ^ 0 = 0
            Assert.AreEqual(1, tree.Root.Branches[0].Branches[1].Output); // 0 ^ 1 = 1
            Assert.AreEqual(1, tree.Root.Branches[1].Branches[0].Output); // 1 ^ 0 = 1
            Assert.AreEqual(0, tree.Root.Branches[1].Branches[1].Output); // 1 ^ 1 = 0
        }


#if !NO_DATA_TABLE
        [Test]
        public void RunTest2()
        {
            DecisionTree tree;
            int[][] inputs;
            int[] outputs;

            CreateMitchellExample(out tree, out inputs, out outputs);

            Assert.AreEqual(0, tree.Root.Branches.AttributeIndex); // Outlook
            Assert.AreEqual(3, tree.Root.Branches.Count);
            Assert.IsNull(tree.Root.Output);
            Assert.IsNull(tree.Root.Value);

            Assert.AreEqual(0, tree.Root.Branches[0].Value); // Outlook = Sunny
            Assert.AreEqual(2, tree.Root.Branches[0].Branches.AttributeIndex); // Decide over Humidity
            Assert.AreEqual(2, tree.Root.Branches[0].Branches.Count);
            Assert.IsTrue(tree.Root.Branches[0].Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[0].Branches[1].IsLeaf);

            Assert.AreEqual(1, tree.Root.Branches[1].Value); // Outlook = Overcast
            Assert.IsNotNull(tree.Root.Branches[1].Branches);
            Assert.AreEqual(0, tree.Root.Branches[1].Branches.Count);
            Assert.IsTrue(tree.Root.Branches[1].IsLeaf);

            Assert.AreEqual(2, tree.Root.Branches[2].Value); // Outlook = Rain
            Assert.AreEqual(3, tree.Root.Branches[2].Branches.AttributeIndex); // Decide over Wind
            Assert.AreEqual(2, tree.Root.Branches[2].Branches.Count);
            Assert.IsTrue(tree.Root.Branches[2].Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[2].Branches[1].IsLeaf);

            Assert.AreEqual(0, tree.Root.Branches[0].Branches[0].Value); // Humidity = High
            Assert.IsTrue(tree.Root.Branches[0].Branches[0].IsLeaf);

            Assert.AreEqual(1, tree.Root.Branches[0].Branches[1].Value); // Humidity = Normal
            Assert.IsTrue(tree.Root.Branches[0].Branches[1].IsLeaf);

            Assert.AreEqual(0, tree.Root.Branches[2].Branches[0].Value); // Wind = Weak
            Assert.IsTrue(tree.Root.Branches[2].Branches[0].IsLeaf);

            Assert.AreEqual(1, tree.Root.Branches[2].Branches[1].Value); // Wind = Strong
            Assert.IsTrue(tree.Root.Branches[2].Branches[1].IsLeaf);
        }
#endif

        [Test]
        public void RunTest3()
        {
            DecisionTree tree;
            int[][] inputs;
            int[] outputs;

            CreateXORExample(out tree, out inputs, out outputs);

            Assert.AreEqual(3, tree.Root.Branches.AttributeIndex); // a4
            Assert.AreEqual(2, tree.Root.Branches.Count);
            Assert.IsNull(tree.Root.Output);
            Assert.IsNull(tree.Root.Value);

            Assert.AreEqual(0, tree.Root.Branches[0].Value); // a4 = 0
            Assert.AreEqual(0, tree.Root.Branches[0].Branches.AttributeIndex); // Decide over a1
            Assert.AreEqual(2, tree.Root.Branches[0].Branches.Count);
            Assert.IsFalse(tree.Root.Branches[0].Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[0].Branches[1].IsLeaf);
            Assert.AreEqual(0, tree.Root.Branches[0].Branches[1].Output);

            Assert.AreEqual(1, tree.Root.Branches[1].Value); // a4 = 1
            Assert.AreEqual(0, tree.Root.Branches[1].Branches.AttributeIndex); // Decide over a1
            Assert.AreEqual(2, tree.Root.Branches[1].Branches.Count);
            Assert.IsFalse(tree.Root.Branches[1].Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[1].Branches[1].IsLeaf);
            Assert.AreEqual(1, tree.Root.Branches[1].Branches[1].Output);

            Assert.AreEqual(0, tree.Root.Branches[0].Branches[0].Value); // a1 = 0
            Assert.AreEqual(1, tree.Root.Branches[0].Branches[0].Branches.AttributeIndex); // Decide over a2
            Assert.AreEqual(2, tree.Root.Branches[0].Branches[0].Branches.Count);
            Assert.IsTrue(tree.Root.Branches[0].Branches[0].Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[0].Branches[0].Branches[1].IsLeaf);
        }

#if !NO_DATA_TABLE
        [Test]
        public void ConstantDiscreteVariableTest()
        {
            DecisionTree tree;
            int[][] inputs;
            int[] outputs;

            DataTable data = new DataTable("Degenerated Tennis Example");

            data.Columns.Add("Day", "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");

            data.Rows.Add("D1", "Sunny", "Hot", "High", "Weak", "No");
            data.Rows.Add("D2", "Sunny", "Hot", "High", "Strong", "No");
            data.Rows.Add("D3", "Overcast", "Hot", "High", "Weak", "Yes");
            data.Rows.Add("D4", "Rain", "Hot", "High", "Weak", "Yes");
            data.Rows.Add("D5", "Rain", "Hot", "Normal", "Weak", "Yes");
            data.Rows.Add("D6", "Rain", "Hot", "Normal", "Strong", "No");
            data.Rows.Add("D7", "Overcast", "Hot", "Normal", "Strong", "Yes");
            data.Rows.Add("D8", "Sunny", "Hot", "High", "Weak", "No");
            data.Rows.Add("D9", "Sunny", "Hot", "Normal", "Weak", "Yes");
            data.Rows.Add("D10", "Rain", "Hot", "Normal", "Weak", "Yes");
            data.Rows.Add("D11", "Sunny", "Hot", "Normal", "Strong", "Yes");
            data.Rows.Add("D12", "Overcast", "Hot", "High", "Strong", "Yes");
            data.Rows.Add("D13", "Overcast", "Hot", "Normal", "Weak", "Yes");
            data.Rows.Add("D14", "Rain", "Hot", "High", "Strong", "No");

            // Create a new codification codebook to
            // convert strings into integer symbols
            Codification codebook = new Codification(data);

            DecisionVariable[] attributes =
            {
               new DecisionVariable("Outlook",     codebook["Outlook"].Symbols),     // 3 possible values (Sunny, overcast, rain)
               new DecisionVariable("Temperature", codebook["Temperature"].Symbols), // 1 constant value (Hot)
               new DecisionVariable("Humidity",    codebook["Humidity"].Symbols),    // 2 possible values (High, normal)
               new DecisionVariable("Wind",        codebook["Wind"].Symbols)         // 2 possible values (Weak, strong)
            };

            int classCount = codebook["PlayTennis"].Symbols; // 2 possible values (yes, no)


            bool thrown = false;
            try
            {
                tree = new DecisionTree(attributes, classCount);
            }
            catch
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);


            attributes[1] = new DecisionVariable("Temperature", 2);
            tree = new DecisionTree(attributes, classCount);
            ID3Learning id3 = new ID3Learning(tree);

            // Extract symbols from data and train the classifier
            DataTable symbols = codebook.Apply(data);
            inputs = symbols.ToArray<int>("Outlook", "Temperature", "Humidity", "Wind");
            outputs = symbols.ToArray<int>("PlayTennis");

            double error = id3.Run(inputs, outputs);

            Assert.AreEqual(0, error);

            for (int i = 0; i < inputs.Length; i++)
            {
                int y = tree.Compute(inputs[i]);
                Assert.AreEqual(outputs[i], y);
            }
        }

        [Test]
        public void IncompleteDiscreteVariableTest()
        {
            DecisionTree tree;
            int[][] inputs;
            int[] outputs;

            DataTable data = new DataTable("Degenerated Tennis Example");

            data.Columns.Add("Day", "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");

            data.Rows.Add("D1", "Sunny", "Hot", "High", "Weak", "No");
            data.Rows.Add("D2", "Sunny", "Hot", "High", "Strong", "No");
            data.Rows.Add("D3", "Overcast", "Hot", "High", "Weak", "Yes");
            data.Rows.Add("D4", "Rain", "Mild", "High", "Weak", "Yes");
            data.Rows.Add("D5", "Rain", "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D6", "Rain", "Cool", "Normal", "Strong", "No");
            data.Rows.Add("D7", "Overcast", "Cool", "Normal", "Strong", "Yes");
            data.Rows.Add("D8", "Sunny", "Mild", "High", "Weak", "No");
            data.Rows.Add("D9", "Sunny", "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D10", "Rain", "Mild", "Normal", "Weak", "Yes");
            data.Rows.Add("D11", "Sunny", "Mild", "Normal", "Strong", "Yes");
            data.Rows.Add("D12", "Overcast", "Mild", "High", "Strong", "Yes");
            data.Rows.Add("D13", "Overcast", "Hot", "Normal", "Weak", "Yes");
            data.Rows.Add("D14", "Rain", "Mild", "High", "Strong", "No");

            // Create a new codification codebook to
            // convert strings into integer symbols
            Codification codebook = new Codification(data);

            DecisionVariable[] attributes =
            {
               new DecisionVariable("Outlook",     codebook["Outlook"].Symbols+200), // 203 possible values, 200 undefined
               new DecisionVariable("Temperature", codebook["Temperature"].Symbols), // 3 possible values (Hot, mild, cool)
               new DecisionVariable("Humidity",    codebook["Humidity"].Symbols),    // 2 possible values (High, normal)
               new DecisionVariable("Wind",        codebook["Wind"].Symbols)         // 2 possible values (Weak, strong)
            };

            int classCount = codebook["PlayTennis"].Symbols; // 2 possible values (yes, no)

            tree = new DecisionTree(attributes, classCount);
            ID3Learning id3 = new ID3Learning(tree);

            // Extract symbols from data and train the classifier
            DataTable symbols = codebook.Apply(data);
            inputs = symbols.ToArray<int>("Outlook", "Temperature", "Humidity", "Wind");
            outputs = symbols.ToArray<int>("PlayTennis");

            double error = id3.Run(inputs, outputs);

            Assert.AreEqual(0, error);

            Assert.AreEqual(203, tree.Root.Branches.Count);
            Assert.IsTrue(tree.Root.Branches[100].IsLeaf);
            Assert.IsNull(tree.Root.Branches[100].Output);

            for (int i = 0; i < inputs.Length; i++)
            {
                int y = tree.Compute(inputs[i]);
                Assert.AreEqual(outputs[i], y);
            }
        }
#endif

        [Test]
        public void ArgumentCheck1()
        {
            int[][] samples =
            {
                new [] { 0, 2, 4 },
                new [] { 1, 5, 2 },
                null,
                new [] { 1, 5, 6 },
            };

            int[] outputs =
            {
                1, 1, 0, 0
            };

            DecisionVariable[] vars = new DecisionVariable[3];
            for (int i = 0; i < vars.Length; i++)
                vars[i] = DecisionVariable.Discrete(i.ToString(), new IntRange(0, 10));

            DecisionTree tree = new DecisionTree(vars, 2);
            ID3Learning teacher = new ID3Learning(tree);

            bool thrown = false;

            try { double error = teacher.Run(samples, outputs); }
            catch (ArgumentNullException) { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [Test]
        public void ConsistencyTest1()
        {
            int[,] random = Matrix.Random(1000, 10, 0.0, 10.0).ToInt32();

            int[][] samples = random.ToJagged();
            int[] outputs = new int[1000];

            for (int i = 0; i < samples.Length; i++)
            {
                if (samples[i][0] > 8)
                    outputs[i] = 1;
            }

            DecisionVariable[] vars = new DecisionVariable[10];
            for (int i = 0; i < vars.Length; i++)
                vars[i] = new DecisionVariable(i.ToString(), new IntRange(0, 10));

            DecisionTree tree = new DecisionTree(vars, 2);

            ID3Learning teacher = new ID3Learning(tree);

            double error = teacher.Run(samples, outputs);

            Assert.AreEqual(0, error);

            Assert.AreEqual(11, tree.Root.Branches.Count);
            for (int i = 0; i < tree.Root.Branches.Count; i++)
                Assert.IsTrue(tree.Root.Branches[i].IsLeaf);
        }

        [Test]
        public void ConsistencyTest1_automatic()
        {
            int n = 10000;
            int[,] random = Matrix.Random(n, 10, 0.0, 11.0).ToInt32();

            int[][] samples = random.ToJagged();
            int[] outputs = new int[n];

            for (int i = 0; i < samples.Length; i++)
            {
                if (samples[i][0] > 8)
                    outputs[i] = 1;
            }

            ID3Learning teacher = new ID3Learning();

            var tree = teacher.Learn(samples, outputs);

            double error = teacher.ComputeError(samples, outputs);

            Assert.AreEqual(0, error);

            Assert.AreEqual(11, tree.Root.Branches.Count);
            for (int i = 0; i < tree.Root.Branches.Count; i++)
                Assert.IsTrue(tree.Root.Branches[i].IsLeaf);
        }


        [Test]
        public void LargeSampleTest1()
        {
            Accord.Math.Tools.SetupGenerator(0);

            int[][] dataSamples = Matrix.Random(500, 3, 0.0, 10.0).ToInt32().ToJagged();
            int[] target = Matrix.Random(500, 1, 0.0, 2.0).ToInt32().GetColumn(0);
            DecisionVariable[] features =
            {
                new DecisionVariable("Outlook",      10),
                new DecisionVariable("Temperature",  10),
                new DecisionVariable("Humidity",     10),
            };


            DecisionTree tree = new DecisionTree(features, 2);
            ID3Learning id3Learning = new ID3Learning(tree);

            double error = id3Learning.Run(dataSamples, target);

            Assert.IsTrue(error < 0.2);

            var code = tree.ToCode("MyTree");


            Assert.IsNotNull(code);
            Assert.IsTrue(code.Length > 0);
        }

        [Test]
        public void LargeSampleTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            int[][] dataSamples = Matrix.Random(500, 3, 0.0, 10.0).ToInt32().ToJagged();
            int[] target = Matrix.Random(500, 1, 0.0, 2.0).ToInt32().GetColumn(0);
            DecisionVariable[] features =
            {
                new DecisionVariable("Outlook",      10),
                new DecisionVariable("Temperature",  10),
                new DecisionVariable("Humidity",     10),
            };


            DecisionTree tree = new DecisionTree(features, 2);
            ID3Learning id3Learning = new ID3Learning(tree)
            {
                Rejection = false
            };

            double error = id3Learning.Run(dataSamples, target);

            int height = tree.GetHeight();
            Assert.AreEqual(3, height);

            foreach (var node in tree)
            {
                if (node.IsLeaf)
                    Assert.IsNotNull(node.Output);
            }

            Assert.IsTrue(error < 0.15);
        }

        [Test]
        public void LargeSampleTest_WithRepetition()
        {
            Accord.Math.Random.Generator.Seed = 0;

            int[][] dataSamples = Matrix.Random(500, 3, 0, 10).ToJagged();
            int[] target = Matrix.Random(500, 1, 0, 2).GetColumn(0);
            DecisionVariable[] features =
            {
                new DecisionVariable("Outlook",      10),
                new DecisionVariable("Temperature",  10),
                new DecisionVariable("Humidity",     10),
            };


            DecisionTree tree = new DecisionTree(features, 2);
            ID3Learning id3Learning = new ID3Learning(tree)
            {
                Rejection = false,
                Join = 2 // every variable can join two times
            };

            double error = id3Learning.Run(dataSamples, target);

            int height = tree.GetHeight();
            Assert.AreEqual(6, height);

            foreach (var node in tree)
            {
                if (node.IsLeaf)
                    Assert.IsNotNull(node.Output);
            }

            Assert.IsTrue(error < 0.15);
        }

        [Test]
        public void learn_doc()
        {
            #region doc_learn_simplest
            // In this example, we will learn a decision tree directly from integer
            // matrices that define the inputs and outputs of our learning problem.

            int[][] inputs =
            {
                new int[] { 0, 0 },
                new int[] { 0, 1 },
                new int[] { 1, 0 },
                new int[] { 1, 1 },
            };

            int[] outputs = // xor between inputs[0] and inputs[1]
            {
                0, 1, 1, 0
            };

            // Create an ID3 learning algorithm
            ID3Learning teacher = new ID3Learning();

            // Learn a decision tree for the XOR problem
            var tree = teacher.Learn(inputs, outputs);

            // Compute the error in the learning
            double error = new ZeroOneLoss(outputs).Loss(tree.Decide(inputs));

            // The tree can now be queried for new examples:
            int[] predicted = tree.Decide(inputs); // should be { 0, 1, 1, 0 }
            #endregion

            Assert.AreEqual(0, error);
            Assert.AreEqual(0, predicted[0]);
            Assert.AreEqual(1, predicted[1]);
            Assert.AreEqual(1, predicted[2]);
            Assert.AreEqual(0, predicted[3]);
        }

#if !NO_DATA_TABLE
        [Test]
        public void learn_doc2()
        {
            #region doc_learn_mitchell
            // In this example, we will be using the famous Play Tennis example by Tom Mitchell (1998).
            // In Mitchell's example, one would like to infer if a person would play tennis or not
            // based solely on four input variables. Those variables are all categorical, meaning that
            // there is no order between the possible values for the variable (i.e. there is no order
            // relationship between Sunny and Rain, one is not bigger nor smaller than the other, but are 
            // just distinct). Moreover, the rows, or instances presented above represent days on which the
            // behavior of the person has been registered and annotated, pretty much building our set of 
            // observation instances for learning:

            // Note: this example uses DataTables to represent the input data , but this is not required.
            DataTable data = new DataTable("Mitchell's Tennis Example");

            data.Columns.Add("Day", "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");
            data.Rows.Add("D1", "Sunny", "Hot", "High", "Weak", "No");
            data.Rows.Add("D2", "Sunny", "Hot", "High", "Strong", "No");
            data.Rows.Add("D3", "Overcast", "Hot", "High", "Weak", "Yes");
            data.Rows.Add("D4", "Rain", "Mild", "High", "Weak", "Yes");
            data.Rows.Add("D5", "Rain", "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D6", "Rain", "Cool", "Normal", "Strong", "No");
            data.Rows.Add("D7", "Overcast", "Cool", "Normal", "Strong", "Yes");
            data.Rows.Add("D8", "Sunny", "Mild", "High", "Weak", "No");
            data.Rows.Add("D9", "Sunny", "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D10", "Rain", "Mild", "Normal", "Weak", "Yes");
            data.Rows.Add("D11", "Sunny", "Mild", "Normal", "Strong", "Yes");
            data.Rows.Add("D12", "Overcast", "Mild", "High", "Strong", "Yes");
            data.Rows.Add("D13", "Overcast", "Hot", "Normal", "Weak", "Yes");
            data.Rows.Add("D14", "Rain", "Mild", "High", "Strong", "No");

            // In order to try to learn a decision tree, we will first convert this problem to a more simpler
            // representation. Since all variables are categories, it does not matter if they are represented
            // as strings, or numbers, since both are just symbols for the event they represent. Since numbers
            // are more easily representable than text string, we will convert the problem to use a discrete 
            // alphabet through the use of a Accord.Statistics.Filters.Codification codebook.</para>

            // A codebook effectively transforms any distinct possible value for a variable into an integer 
            // symbol. For example, “Sunny” could as well be represented by the integer label 0, “Overcast” 
            // by “1”, Rain by “2”, and the same goes by for the other variables. So:</para>

            // Create a new codification codebook to 
            // convert strings into integer symbols
            var codebook = new Codification(data);

            // Translate our training data into integer symbols using our codebook:
            DataTable symbols = codebook.Apply(data);
            int[][] inputs = symbols.ToArray<int>("Outlook", "Temperature", "Humidity", "Wind");
            int[] outputs = symbols.ToArray<int>("PlayTennis");

            // For this task, in which we have only categorical variables, the simplest choice 
            // to induce a decision tree is to use the ID3 algorithm by Quinlan. Let’s do it:

            // Create a teacher ID3 algorithm
            var id3learning = new ID3Learning()
            {
                // Now that we already have our learning input/ouput pairs, we should specify our
                // decision tree. We will be trying to build a tree to predict the last column, entitled
                // “PlayTennis”. For this, we will be using the “Outlook”, “Temperature”, “Humidity” and
                // “Wind” as predictors (variables which will we will use for our decision). Since those
                // are categorical, we must specify, at the moment of creation of our tree, the
                // characteristics of each of those variables. So:

                new DecisionVariable("Outlook",     3), // 3 possible values (Sunny, overcast, rain)
                new DecisionVariable("Temperature", 3), // 3 possible values (Hot, mild, cool)  
                new DecisionVariable("Humidity",    2), // 2 possible values (High, normal)    
                new DecisionVariable("Wind",        2)  // 2 possible values (Weak, strong) 

                // Note: It is also possible to create a DecisionVariable[] from a codebook:
                // DecisionVariable[] attributes = DecisionVariable.FromCodebook(codebook);
            };

            // Learn the training instances!
            DecisionTree tree = id3learning.Learn(inputs, outputs);

            // Compute the training error when predicting training instances
            double error = new ZeroOneLoss(outputs).Loss(tree.Decide(inputs));

            // The tree can now be queried for new examples through 
            // its decide method. For example, we can create a query

            int[] query = codebook.Transform(new[,]
            {
                { "Outlook",     "Sunny"  },
                { "Temperature", "Hot"    },
                { "Humidity",    "High"   },
                { "Wind",        "Strong" }
            });

            // And then predict the label using
            int predicted = tree.Decide(query);  // result will be 0

            // We can translate it back to strings using
            string answer = codebook.Revert("PlayTennis", predicted); // Answer will be: "No"
            #endregion

            Assert.AreEqual(0, predicted);
            Assert.AreEqual("No", answer);
            Assert.AreEqual(0, error);
        }
#endif
    }
}
