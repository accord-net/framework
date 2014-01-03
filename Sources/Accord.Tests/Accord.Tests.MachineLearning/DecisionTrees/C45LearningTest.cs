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
    using Accord.Math;
    using Accord.Statistics.Filters;
    using Accord.Tests.MachineLearning.Properties;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.MachineLearning.DecisionTrees.Rules;

    [TestClass()]
    public class C45LearningTest
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



        public static void CreateMitchellExample(out DecisionTree tree, out double[][] inputs, out int[] outputs)
        {
            DataTable data = new DataTable("Mitchell's Tennis Example");

            data.Columns.Add("Day", typeof(string));
            data.Columns.Add("Outlook", typeof(string));
            data.Columns.Add("Temperature", typeof(double));
            data.Columns.Add("Humidity", typeof(double));
            data.Columns.Add("Wind", typeof(string));
            data.Columns.Add("PlayTennis", typeof(string));

            data.Rows.Add("D1", "Sunny", 85, 85, "Weak", "No");
            data.Rows.Add("D2", "Sunny", 80, 90, "Strong", "No");
            data.Rows.Add("D3", "Overcast", 83, 78, "Weak", "Yes");
            data.Rows.Add("D4", "Rain", 70, 96, "Weak", "Yes");
            data.Rows.Add("D5", "Rain", 68, 80, "Weak", "Yes");
            data.Rows.Add("D6", "Rain", 65, 70, "Strong", "No");
            data.Rows.Add("D7", "Overcast", 64, 65, "Strong", "Yes");
            data.Rows.Add("D8", "Sunny", 72, 95, "Weak", "No");
            data.Rows.Add("D9", "Sunny", 69, 70, "Weak", "Yes");
            data.Rows.Add("D10", "Rain", 75, 80, "Weak", "Yes");
            data.Rows.Add("D11", "Sunny", 75, 70, "Strong", "Yes");
            data.Rows.Add("D12", "Overcast", 72, 90, "Strong", "Yes");
            data.Rows.Add("D13", "Overcast", 81, 75, "Weak", "Yes");
            data.Rows.Add("D14", "Rain", 71, 80, "Strong", "No");

            // Create a new codification codebook to
            // convert strings into integer symbols
            Codification codebook = new Codification(data);

            DecisionVariable[] attributes =
            {
               new DecisionVariable("Outlook",     codebook["Outlook"].Symbols),      // 3 possible values (Sunny, overcast, rain)
               new DecisionVariable("Temperature", DecisionVariableKind.Continuous), // continuous values
               new DecisionVariable("Humidity",    DecisionVariableKind.Continuous), // continuous values
               new DecisionVariable("Wind",        codebook["Wind"].Symbols)          // 2 possible values (Weak, strong)
            };

            int classCount = codebook["PlayTennis"].Symbols; // 2 possible values (yes, no)

            tree = new DecisionTree(attributes, classCount);
            C45Learning c45 = new C45Learning(tree);

            // Extract symbols from data and train the classifier
            DataTable symbols = codebook.Apply(data);
            inputs = symbols.ToArray("Outlook", "Temperature", "Humidity", "Wind");
            outputs = symbols.ToArray<int>("PlayTennis");

            double error = c45.Run(inputs, outputs);
        }


        [TestMethod()]
        public void RunTest()
        {
            DecisionTree tree;
            double[][] inputs;
            int[] outputs;

            CreateMitchellExample(out tree, out inputs, out outputs);

            Assert.AreEqual(1, tree.Root.Branches.AttributeIndex); // Temperature
            Assert.AreEqual(2, tree.Root.Branches.Count);
            Assert.IsNull(tree.Root.Output);
            Assert.IsNull(tree.Root.Value);

            Assert.AreEqual(84, tree.Root.Branches[0].Value); // Temperature <= 84.0
            Assert.AreEqual(0, tree.Root.Branches[0].Branches.AttributeIndex); // Decide over Outlook
            Assert.AreEqual(ComparisonKind.LessThanOrEqual, tree.Root.Branches[0].Comparison);
            Assert.AreEqual(3, tree.Root.Branches[0].Branches.Count);
            Assert.IsFalse(tree.Root.Branches[0].Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[0].Branches[1].IsLeaf);

            Assert.AreEqual(84, tree.Root.Branches[1].Value); // Temperature > 84.0
            Assert.AreEqual(0, tree.Root.Branches[1].Output.Value); // Output is "No"
            Assert.AreEqual(ComparisonKind.GreaterThan, tree.Root.Branches[1].Comparison);
            Assert.IsNotNull(tree.Root.Branches[1].Branches);
            Assert.AreEqual(0, tree.Root.Branches[1].Branches.Count);
            Assert.IsTrue(tree.Root.Branches[1].IsLeaf);

            Assert.AreEqual(0, tree.Root.Branches[0].Branches[0].Value); // Outlook <= 0
            Assert.AreEqual(ComparisonKind.Equal, tree.Root.Branches[0].Branches[0].Comparison);
            Assert.AreEqual(3, tree.Root.Branches[0].Branches.Count);
            Assert.AreEqual(2, tree.Root.Branches[0].Branches[0].Branches.AttributeIndex); // Decide over Humidity
            Assert.AreEqual(70, tree.Root.Branches[0].Branches[0].Branches[0].Value);
            Assert.AreEqual(ComparisonKind.LessThanOrEqual, tree.Root.Branches[0].Branches[0].Branches[0].Comparison);
            Assert.AreEqual(ComparisonKind.GreaterThan, tree.Root.Branches[0].Branches[0].Branches[1].Comparison);
        }


        [TestMethod()]
        public void LargeRunTest()
        {
            // This example uses the Nursery Database available from the University of
            // California Irvine repository of machine learning databases, available at
            //
            //   http://archive.ics.uci.edu/ml/machine-learning-databases/nursery/nursery.names
            //
            // The description paragraph is listed as follows.
            //
            //   Nursery Database was derived from a hierarchical decision model
            //   originally developed to rank applications for nursery schools. It
            //   was used during several years in 1980's when there was excessive
            //   enrollment to these schools in Ljubljana, Slovenia, and the
            //   rejected applications frequently needed an objective
            //   explanation. The final decision depended on three subproblems:
            //   occupation of parents and child's nursery, family structure and
            //   financial standing, and social and health picture of the family.
            //   The model was developed within expert system shell for decision
            //   making DEX (M. Bohanec, V. Rajkovic: Expert system for decision
            //   making. Sistemica 1(1), pp. 145-157, 1990.).
            //

            // Let's begin by loading the raw data. This string variable contains
            // the contents of the nursery.data file as a single, continuous text.
            //
            string nurseryData = Resources.nursery;

            // Those are the input columns available in the data
            //
            string[] inputColumns = 
            {
                "parents", "has_nurs", "form", "children",
                "housing", "finance", "social", "health"
            };

            // And this is the output, the last column of the data.
            //
            string outputColumn = "output";


            // Let's populate a data table with this information.
            //
            DataTable table = new DataTable("Nursery");
            table.Columns.Add(inputColumns);
            table.Columns.Add(outputColumn);

            string[] lines = nurseryData.Split(
                new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in lines)
                table.Rows.Add(line.Split(','));


            // Now, we have to convert the textual, categorical data found
            // in the table to a more manageable discrete representation.
            //
            // For this, we will create a codebook to translate text to
            // discrete integer symbols:
            //
            Codification codebook = new Codification(table);

            // And then convert all data into symbols
            //
            DataTable symbols = codebook.Apply(table);
            double[][] inputs = symbols.ToArray(inputColumns);
            int[] outputs = symbols.ToArray<int>(outputColumn);

            // From now on, we can start creating the decision tree.
            //
            var attributes = DecisionVariable.FromCodebook(codebook, inputColumns);
            DecisionTree tree = new DecisionTree(attributes, outputClasses: 5);


            // Now, let's create the C4.5 algorithm
            C45Learning c45 = new C45Learning(tree);

            // and learn a decision tree. The value of
            //   the error variable below should be 0.
            //
            double error = c45.Run(inputs, outputs);


            // To compute a decision for one of the input points,
            //   such as the 25-th example in the set, we can use
            //
            int y = tree.Compute(inputs[25]);

            Assert.AreEqual(0, error);

            for (int i = 0; i < inputs.Length; i++)
            {
                int expected = outputs[i];
                int actual = tree.Compute(inputs[i]);

                Assert.AreEqual(expected, actual);
            }


#if !NET35

            // Finally, we can also convert our tree to a native
            // function, improving efficiency considerably, with
            //
            Func<double[], int> func = tree.ToExpression().Compile();

            // Again, to compute a new decision, we can just use
            //
            int z = func(inputs[25]);


            for (int i = 0; i < inputs.Length; i++)
            {
                int expected = outputs[i];
                int actual = func(inputs[i]);

                Assert.AreEqual(expected, actual);
            }
#endif
        }


        [TestMethod()]
        public void ConstantContinuousVariableTest()
        {
            DecisionTree tree;
            double[][] inputs;
            int[] outputs;

            DataTable data = new DataTable("Degenerated Tennis Example");

            data.Columns.Add("Day", typeof(string));
            data.Columns.Add("Outlook", typeof(string));
            data.Columns.Add("Temperature", typeof(double));
            data.Columns.Add("Humidity", typeof(double));
            data.Columns.Add("Wind", typeof(string));
            data.Columns.Add("PlayTennis", typeof(string));

            data.Rows.Add("D1", "Sunny", 100, 85, "Weak", "No");
            data.Rows.Add("D2", "Sunny", 100, 90, "Weak", "No");
            data.Rows.Add("D3", "Overcast", 60, 78, "Weak", "Yes");
            data.Rows.Add("D4", "Rain", 100, 64, "Weak", "Yes");
            data.Rows.Add("D5", "Rain", 100, 62, "Weak", "Yes");
            data.Rows.Add("D6", "Rain", 100, 90, "Weak", "No");
            data.Rows.Add("D7", "Overcast", 100, 65, "Weak", "Yes");
            data.Rows.Add("D8", "Sunny", 100, 95, "Weak", "No");
            data.Rows.Add("D9", "Sunny", 100, 56, "Weak", "Yes");
            data.Rows.Add("D10", "Rain", 100, 74, "Weak", "Yes");
            data.Rows.Add("D11", "Sunny", 100, 44, "Weak", "Yes");
            data.Rows.Add("D12", "Overcast", 100, 64, "Weak", "Yes");
            data.Rows.Add("D13", "Overcast", 100, 65, "Weak", "Yes");
            data.Rows.Add("D14", "Rain", 100, 80, "Weak", "No");

            // Create a new codification codebook to
            // convert strings into integer symbols
            Codification codebook = new Codification(data);

            DecisionVariable[] attributes =
            {
               new DecisionVariable("Outlook",     codebook["Outlook"].Symbols),      // 3 possible values (Sunny, overcast, rain)
               new DecisionVariable("Temperature", DecisionVariableKind.Continuous), // constant continuous value
               new DecisionVariable("Humidity",    DecisionVariableKind.Continuous), // continuous values
               new DecisionVariable("Wind",        codebook["Wind"].Symbols + 1)      // 1 possible value (Weak)
            };

            int classCount = codebook["PlayTennis"].Symbols; // 2 possible values (yes, no)

            tree = new DecisionTree(attributes, classCount);
            C45Learning c45 = new C45Learning(tree);

            // Extract symbols from data and train the classifier
            DataTable symbols = codebook.Apply(data);
            inputs = symbols.ToArray("Outlook", "Temperature", "Humidity", "Wind");
            outputs = symbols.ToArray<int>("PlayTennis");

            double error = c45.Run(inputs, outputs);

            for (int i = 0; i < inputs.Length; i++)
            {
                int y = tree.Compute(inputs[i]);
                Assert.AreEqual(outputs[i], y);
            }
        }


        [TestMethod()]
        public void ConstantDiscreteVariableTest()
        {
            DecisionTree tree;
            double[][] inputs;
            int[] outputs;

            DataTable data = new DataTable("Degenerated Tennis Example");

            data.Columns.Add("Day", typeof(string));
            data.Columns.Add("Outlook", typeof(string));
            data.Columns.Add("Temperature", typeof(double));
            data.Columns.Add("Humidity", typeof(double));
            data.Columns.Add("Wind", typeof(string));
            data.Columns.Add("PlayTennis", typeof(string));

            data.Rows.Add("D1", "Sunny", 50, 85, "Weak", "No");
            data.Rows.Add("D2", "Sunny", 50, 90, "Weak", "No");
            data.Rows.Add("D3", "Overcast", 83, 78, "Weak", "Yes");
            data.Rows.Add("D4", "Rain", 70, 96, "Weak", "Yes");
            data.Rows.Add("D5", "Rain", 68, 80, "Weak", "Yes");
            data.Rows.Add("D6", "Rain", 65, 70, "Weak", "No");
            data.Rows.Add("D7", "Overcast", 64, 65, "Weak", "Yes");
            data.Rows.Add("D8", "Sunny", 50, 95, "Weak", "No");
            data.Rows.Add("D9", "Sunny", 69, 70, "Weak", "Yes");
            data.Rows.Add("D10", "Rain", 75, 80, "Weak", "Yes");
            data.Rows.Add("D11", "Sunny", 75, 70, "Weak", "Yes");
            data.Rows.Add("D12", "Overcast", 72, 90, "Weak", "Yes");
            data.Rows.Add("D13", "Overcast", 81, 75, "Weak", "Yes");
            data.Rows.Add("D14", "Rain", 50, 80, "Weak", "No");

            // Create a new codification codebook to
            // convert strings into integer symbols
            Codification codebook = new Codification(data);

            DecisionVariable[] attributes =
            {
               new DecisionVariable("Outlook",     codebook["Outlook"].Symbols),      // 3 possible values (Sunny, overcast, rain)
               new DecisionVariable("Temperature", DecisionVariableKind.Continuous), // continuous values
               new DecisionVariable("Humidity",    DecisionVariableKind.Continuous), // continuous values
               new DecisionVariable("Wind",        codebook["Wind"].Symbols + 1)      // 1 possible value (Weak)
            };

            int classCount = codebook["PlayTennis"].Symbols; // 2 possible values (yes, no)

            tree = new DecisionTree(attributes, classCount);
            C45Learning c45 = new C45Learning(tree);

            // Extract symbols from data and train the classifier
            DataTable symbols = codebook.Apply(data);
            inputs = symbols.ToArray("Outlook", "Temperature", "Humidity", "Wind");
            outputs = symbols.ToArray<int>("PlayTennis");

            double error = c45.Run(inputs, outputs);

            for (int i = 0; i < inputs.Length; i++)
            {
                int y = tree.Compute(inputs[i]);
                Assert.AreEqual(outputs[i], y);
            }
        }


        [TestMethod]
        public void ConsistencyTest1()
        {
            double[,] random = Matrix.Random(1000, 10, 0.0, 1.0);

            double[][] samples = random.ToArray();
            int[] outputs = new int[1000];

            for (int i = 0; i < samples.Length; i++)
            {
                if (samples[i][0] > 0.8)
                    outputs[i] = 1;
            }

            DecisionVariable[] vars = new DecisionVariable[10];
            for (int i = 0; i < vars.Length; i++)
                vars[i] = new DecisionVariable(i.ToString(), DecisionVariableKind.Continuous);

            DecisionTree tree = new DecisionTree(vars, 2);

            C45Learning teacher = new C45Learning(tree);

            double error = teacher.Run(samples, outputs);

            Assert.AreEqual(0, error);

            Assert.AreEqual(2, tree.Root.Branches.Count);
            Assert.IsTrue(tree.Root.Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[1].IsLeaf);
        }

        [TestMethod]
        public void LargeSampleTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[][] dataSamples = Matrix.Random(500, 3, 0, 10).ToArray();
            int[] target = Matrix.Random(500, 1, 0, 2).ToInt32().GetColumn(0);

            DecisionVariable[] features =
            {
                new DecisionVariable("Outlook", DecisionVariableKind.Continuous), 
                new DecisionVariable("Temperature", DecisionVariableKind.Continuous), 
                new DecisionVariable("Humidity", DecisionVariableKind.Continuous), 
            };


            DecisionTree tree = new DecisionTree(features, 2);
            C45Learning teacher = new C45Learning(tree);

            double error = teacher.Run(dataSamples, target);

            foreach (var node in tree)
            {
                if (node.IsLeaf)
                    Assert.IsNotNull(node.Output);
            }

            Assert.IsTrue(error < 0.50);
        }
    }
}
