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
    using Accord.DataSets;
    using Accord.MachineLearning.DecisionTrees;
    using Accord.MachineLearning.DecisionTrees.Learning;
    using Accord.MachineLearning.DecisionTrees.Rules;
    using Accord.Math;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics.Filters;
    using Accord.Tests.MachineLearning.Properties;
    using NUnit.Framework;
    using System;
    using System.Data;
    using System.Globalization;
#if NO_CULTURE
    using CultureInfo = Accord.Compat.CultureInfoEx;
#endif

    [TestFixture]
    public class C45LearningTest
    {
#if !NO_DATA_TABLE
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


        [Test]
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
            Assert.IsFalse(tree.Root.Branches[0].Branches[2].IsLeaf);

            Assert.AreEqual(84, tree.Root.Branches[1].Value); // Temperature > 84.0
            Assert.AreEqual(0, tree.Root.Branches[1].Output.Value); // Output is "No"
            Assert.AreEqual(ComparisonKind.GreaterThan, tree.Root.Branches[1].Comparison);
            Assert.IsNotNull(tree.Root.Branches[1].Branches);
            Assert.AreEqual(0, tree.Root.Branches[1].Branches.Count);
            Assert.IsTrue(tree.Root.Branches[1].IsLeaf);

            Assert.AreEqual(0, tree.Root.Branches[0].Branches[0].Value); // Temperature <= 84.0 && Outlook == 0
            Assert.AreEqual(ComparisonKind.Equal, tree.Root.Branches[0].Branches[0].Comparison);
            Assert.AreEqual(2, tree.Root.Branches[0].Branches[0].Branches.AttributeIndex); // Decide over Humidity
            Assert.AreEqual(72.5, tree.Root.Branches[0].Branches[0].Branches[0].Value);
            Assert.AreEqual(72.5, tree.Root.Branches[0].Branches[0].Branches[1].Value);
            Assert.IsTrue(tree.Root.Branches[0].Branches[0].Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[0].Branches[0].Branches[1].IsLeaf);
            Assert.AreEqual(ComparisonKind.LessThanOrEqual, tree.Root.Branches[0].Branches[0].Branches[0].Comparison);
            Assert.AreEqual(ComparisonKind.GreaterThan, tree.Root.Branches[0].Branches[0].Branches[1].Comparison);

            Assert.AreEqual(2, tree.Root.Branches[0].Branches[2].Value); // Temperature <= 84.0 && Outlook == 2
            Assert.AreEqual(ComparisonKind.Equal, tree.Root.Branches[0].Branches[2].Comparison);
            Assert.AreEqual(3, tree.Root.Branches[0].Branches[2].Branches.AttributeIndex); // Decide over Wind
            Assert.AreEqual(0, tree.Root.Branches[0].Branches[2].Branches[0].Value);
            Assert.AreEqual(1, tree.Root.Branches[0].Branches[2].Branches[1].Value);
            Assert.IsTrue(tree.Root.Branches[0].Branches[2].Branches[0].IsLeaf);
            Assert.IsTrue(tree.Root.Branches[0].Branches[2].Branches[1].IsLeaf);
            Assert.AreEqual(ComparisonKind.Equal, tree.Root.Branches[0].Branches[2].Branches[0].Comparison);
            Assert.AreEqual(ComparisonKind.Equal, tree.Root.Branches[0].Branches[2].Branches[1].Comparison);
        }

        [Test]
        public void LargeRunTest()
        {
            #region doc_nursery
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
                new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

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

            // We can either specify the decision attributes we want
            // manually, or we can ask the codebook to do it for us:
            DecisionVariable[] attributes = DecisionVariable.FromCodebook(codebook, inputColumns);

            // Now, let's create the C4.5 algorithm:
            C45Learning c45 = new C45Learning(attributes);

            // and induce a decision tree from the data:
            DecisionTree tree = c45.Learn(inputs, outputs);

            // To get the estimated class labels, we can use
            int[] predicted = tree.Decide(inputs);

            // And the classification error (of 0.0) can be computed as 
            double error = new ZeroOneLoss(outputs).Loss(tree.Decide(inputs));

            // To compute a decision for one of the input points,
            //   such as the 25-th example in the set, we can use
            //
            int y = tree.Decide(inputs[25]); // should be 1
            #endregion

            Assert.AreEqual(12960, lines.Length);
            Assert.AreEqual("usual,proper,complete,1,convenient,convenient,nonprob,recommended,recommend", lines[0]);
            Assert.AreEqual("great_pret,very_crit,foster,more,critical,inconv,problematic,not_recom,not_recom", lines[lines.Length - 1]);

            Assert.AreEqual(0, error);
            Assert.AreEqual(1, y);

            for (int i = 0; i < inputs.Length; i++)
            {
                int expected = outputs[i];
                int actual = tree.Compute(inputs[i]);

                Assert.AreEqual(expected, actual);
            }

#if !NET35
            #region doc_nursery_native
            // Finally, we can also convert our tree to a native
            // function, improving efficiency considerably, with
            //
            Func<double[], int> func = tree.ToExpression().Compile();

            // Again, to compute a new decision, we can just use
            //
            int z = func(inputs[25]);
            #endregion

            Assert.AreEqual(z, y);

            for (int i = 0; i < inputs.Length; i++)
            {
                int expected = outputs[i];
                int actual = func(inputs[i]);

                Assert.AreEqual(expected, actual);
            }
#endif
        }


        [Test]
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
               new DecisionVariable("Temperature", DecisionVariableKind.Continuous),  // constant continuous value
               new DecisionVariable("Humidity",    DecisionVariableKind.Continuous),  // continuous values
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


        [Test]
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

        [Test]
        public void missing_values_test()
        {
            #region doc_missing
            // In this example, we will be using a modified version of the famous Play Tennis 
            // example by Tom Mitchell (1998), where some values have been replaced by missing 
            // values. We will use NaN double values to represent values missing from the data.

            // Note: this example uses DataTables to represent the input data, 
            // but this is not required. The same could be performed using plain
            // double[][] matrices and vectors instead.
            DataTable data = new DataTable("Tennis Example with Missing Values");

            data.Columns.Add("Day", typeof(string));
            data.Columns.Add("Outlook", typeof(string));
            data.Columns.Add("Temperature", typeof(string));
            data.Columns.Add("Humidity", typeof(string));
            data.Columns.Add("Wind", typeof(string));
            data.Columns.Add("PlayTennis", typeof(string));

            data.Rows.Add("D1", "Sunny", "Hot", "High", "Weak", "No");
            data.Rows.Add("D2", null, "Hot", "High", "Strong", "No");
            data.Rows.Add("D3", null, null, "High", null, "Yes");
            data.Rows.Add("D4", "Rain", "Mild", "High", "Weak", "Yes");
            data.Rows.Add("D5", "Rain", "Cool", null, "Weak", "Yes");
            data.Rows.Add("D6", "Rain", "Cool", "Normal", "Strong", "No");
            data.Rows.Add("D7", "Overcast", "Cool", "Normal", "Strong", "Yes");
            data.Rows.Add("D8", null, "Mild", "High", null, "No");
            data.Rows.Add("D9", null, "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D10", null, null, "Normal", null, "Yes");
            data.Rows.Add("D11", null, "Mild", "Normal", null, "Yes");
            data.Rows.Add("D12", "Overcast", "Mild", null, "Strong", "Yes");
            data.Rows.Add("D13", "Overcast", "Hot", null, "Weak", "Yes");
            data.Rows.Add("D14", "Rain", "Mild", "High", "Strong", "No");

            // Create a new codification codebook to convert 
            // the strings above into numeric, integer labels:
            var codebook = new Codification()
            {
                DefaultMissingValueReplacement = Double.NaN
            };

            // Learn the codebook
            codebook.Learn(data);

            // Use the codebook to convert all the data
            DataTable symbols = codebook.Apply(data);

            // Grab the training input and output instances:
            string[] inputNames = new[] { "Outlook", "Temperature", "Humidity", "Wind" };
            double[][] inputs = symbols.ToJagged(inputNames);
            int[] outputs = symbols.ToArray<int>("PlayTennis");

            // Create a new learning algorithm
            var teacher = new C45Learning()
            {
                Attributes = DecisionVariable.FromCodebook(codebook, inputNames)
            };

            // Use the learning algorithm to induce a new tree:
            DecisionTree tree = teacher.Learn(inputs, outputs);

            // To get the estimated class labels, we can use
            int[] predicted = tree.Decide(inputs);

            // The classification error (~0.214) can be computed as 
            double error = new ZeroOneLoss(outputs).Loss(predicted);

            // Moreover, we may decide to convert our tree to a set of rules:
            DecisionSet rules = tree.ToRules();

            // And using the codebook, we can inspect the tree reasoning:
            string ruleText = rules.ToString(codebook, "PlayTennis",
                System.Globalization.CultureInfo.InvariantCulture);

            // The output should be:
            string expected = @"No =: (Outlook == Sunny)
No =: (Outlook == Rain) && (Wind == Strong)
Yes =: (Outlook == Overcast)
Yes =: (Outlook == Rain) && (Wind == Weak)
";
            #endregion

            expected = expected.Replace("\r\n", Environment.NewLine);
            Assert.AreEqual(expected, ruleText);

            Assert.AreEqual(14, codebook["Day"].NumberOfSymbols);
            Assert.AreEqual(3, codebook["Outlook"].NumberOfSymbols);
            Assert.AreEqual(3, codebook["Temperature"].NumberOfSymbols);
            Assert.AreEqual(2, codebook["Humidity"].NumberOfSymbols);
            Assert.AreEqual(2, codebook["Wind"].NumberOfSymbols);
            Assert.AreEqual(2, codebook["PlayTennis"].NumberOfSymbols);

            foreach (var col in codebook)
            {
                Assert.AreEqual(Double.NaN, col.MissingValueReplacement);
                Assert.AreEqual(CodificationVariable.Ordinal, col.VariableType);
            }

            Assert.AreEqual(0.21428571428571427, error, 1e-10);
            Assert.AreEqual(4, tree.NumberOfInputs);
            Assert.AreEqual(2, tree.NumberOfOutputs);

            double newError = ComputeError(rules, inputs, outputs);
            Assert.AreEqual(0.21428571428571427, newError, 1e-10);
        }

        [Test]
        public void missing_values_thresholds_test()
        {
            #region doc_missing_thresholds
            // In this example, we will be using a modified version of the famous Play Tennis 
            // example by Tom Mitchell (1998), where some values have been replaced by missing 
            // values. We will use NaN double values to represent values missing from the data.

            // Note: this example uses DataTables to represent the input data, 
            // but this is not required. The same could be performed using plain
            // double[][] matrices and vectors instead.
            DataTable data = new DataTable("Tennis Example with Missing Values");

            data.Columns.Add("Day", typeof(string));
            data.Columns.Add("Outlook", typeof(string));
            data.Columns.Add("Temperature", typeof(int));
            data.Columns.Add("Humidity", typeof(string));
            data.Columns.Add("Wind", typeof(string));
            data.Columns.Add("PlayTennis", typeof(string));

            data.Rows.Add("D1", "Sunny", 35, "High", "Weak", "No");
            data.Rows.Add("D2", null, 32, "High", "Strong", "No");
            data.Rows.Add("D3", null, null, "High", null, "Yes");
            data.Rows.Add("D4", "Rain", 25, "High", "Weak", "Yes");
            data.Rows.Add("D5", "Rain", 16, null, "Weak", "Yes");
            data.Rows.Add("D6", "Rain", 12, "Normal", "Strong", "No");
            data.Rows.Add("D7", "Overcast", "18", "Normal", "Strong", "Yes");
            data.Rows.Add("D8", null, 27, "High", null, "No");
            data.Rows.Add("D9", null, 17, "Normal", "Weak", "Yes");
            data.Rows.Add("D10", null, null, "Normal", null, "Yes");
            data.Rows.Add("D11", null, 23, "Normal", null, "Yes");
            data.Rows.Add("D12", "Overcast", 25, null, "Strong", "Yes");
            data.Rows.Add("D13", "Overcast", 33, null, "Weak", "Yes");
            data.Rows.Add("D14", "Rain", 24, "High", "Strong", "No");

            string[] inputNames = new[] { "Outlook", "Temperature", "Humidity", "Wind" };

            // Create a new discretization codebook to convert 
            // the numbers above into discrete, string labels:
            var discretization = new Discretization<double, string>()
            {
                { "Temperature", x => x >= 30 && x < 50, "Hot" },
                { "Temperature", x => x >= 20 && x < 30, "Mild" },
                { "Temperature", x => x >= 00 && x < 20, "Cool" },
            };

            // Use the discretization to convert all the data
            DataTable discrete = discretization.Apply(data);

            // Create a new codification codebook to convert 
            // the strings above into numeric, integer labels:
            var codebook = new Codification()
            {
                DefaultMissingValueReplacement = Double.NaN
            };

            // Use the codebook to convert all the data
            DataTable symbols = codebook.Apply(discrete);

            // Grab the training input and output instances:
            double[][] inputs = symbols.ToJagged(inputNames);
            int[] outputs = symbols.ToArray<int>("PlayTennis");

            // Create a new learning algorithm
            var teacher = new C45Learning()
            {
                Attributes = DecisionVariable.FromCodebook(codebook, inputNames)
            };

            // Use the learning algorithm to induce a new tree:
            DecisionTree tree = teacher.Learn(inputs, outputs);

            // To get the estimated class labels, we can use
            int[] predicted = tree.Decide(inputs);

            // The classification error (~0.214) can be computed as 
            double error = new ZeroOneLoss(outputs).Loss(predicted);

            // Moreover, we may decide to convert our tree to a set of rules:
            DecisionSet rules = tree.ToRules();

            // And using the codebook, we can inspect the tree reasoning:
            string ruleText = rules.ToString(codebook, "PlayTennis",
                System.Globalization.CultureInfo.InvariantCulture);

            // The output should be:
            string expected = @"No =: (Outlook == Sunny)
No =: (Outlook == Rain) && (Wind == Strong)
Yes =: (Outlook == Overcast)
Yes =: (Outlook == Rain) && (Wind == Weak)
";
            #endregion

            expected = expected.Replace("\r\n", Environment.NewLine);
            Assert.AreEqual(expected, ruleText);

            Assert.AreEqual(14, codebook["Day"].NumberOfSymbols);
            Assert.AreEqual(3, codebook["Outlook"].NumberOfSymbols);
            Assert.AreEqual(3, codebook["Temperature"].NumberOfSymbols);
            Assert.AreEqual(2, codebook["Humidity"].NumberOfSymbols);
            Assert.AreEqual(2, codebook["Wind"].NumberOfSymbols);
            Assert.AreEqual(2, codebook["PlayTennis"].NumberOfSymbols);

            foreach (var col in codebook)
            {
                Assert.AreEqual(Double.NaN, col.MissingValueReplacement);
                Assert.AreEqual(CodificationVariable.Ordinal, col.VariableType);
            }

            Assert.AreEqual(0.21428571428571427, error, 1e-10);
            Assert.AreEqual(4, tree.NumberOfInputs);
            Assert.AreEqual(2, tree.NumberOfOutputs);

            double newError = ComputeError(rules, inputs, outputs);
            Assert.AreEqual(0.21428571428571427, newError, 1e-10);
        }
#endif

        [Test]
        public void ConsistencyTest1()
        {
            double[,] random = Matrix.Random(1000, 10, 0.0, 1.0);

            double[][] samples = random.ToJagged();
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

        [Test]
        public void LargeSampleTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[][] dataSamples = Matrix.Random(500, 3, 0.0, 10.0).ToJagged();
            int[] target = Matrix.Random(500, 1, 0.0, 2.0).ToInt32().GetColumn(0);

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

        [Test]
        public void ArgumentCheck1()
        {
            double[][] samples =
            {
                new [] { 0, 2, 4.0 },
                new [] { 1, 5, 2.0 },
                null,
                new [] { 1, 5, 6.0 },
            };

            int[] outputs =
            {
                1, 1, 0, 0
            };

            DecisionVariable[] vars = new DecisionVariable[3];
            for (int i = 0; i < vars.Length; i++)
                vars[i] = DecisionVariable.Continuous(i.ToString());

            DecisionTree tree = new DecisionTree(vars, 2);
            var teacher = new C45Learning(tree);

            bool thrown = false;

            try { double error = teacher.Run(samples, outputs); }
            catch (ArgumentNullException) { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [Test]
        public void weight_test_1()
        {

            DecisionTree reference;

            {
                // Learn with weights
                double[][] samples =
                {
                    new [] { 0, 2, 4.0 },
                    new [] { 1, 5, 2.0 },
                    new [] { 1, 5, 6.0 },
                };

                double[] weights = { 1, 1, 2 };

                int[] outputs = { 1, 1, 0 };

                // Learn without weights
                reference = new C45Learning().Learn(samples, outputs, weights);
            }

            {
                // Learn equivalent without weights
                double[][] samples =
                {
                    new [] { 0, 2, 4.0 },
                    new [] { 1, 5, 2.0 },
                    new [] { 1, 5, 6.0 },
                    new [] { 1, 5, 6.0 },
                };

                int[] outputs = { 1, 1, 0, 0 };

                // Learn without weights
                var target = new C45Learning().Learn(samples, outputs);
                AreEqual(reference.Root, target.Root);
            }

            {
                // Learn equivalent with weights
                double[][] samples =
                {
                    new [] { 0, 2, 4.0 },
                    new [] { 1, 5, 2.0 },
                    new [] { 1, 5, 6.0 },
                    new [] { 1, 5, 6.0 },
                };

                int[] outputs = { 1, 1, 0, 0 };

                double[] weights = { 0.1, 0.1, 0.1, 0.0 };

                // Learn without weights
                var target = new C45Learning().Learn(samples, outputs, weights);
                AreEqual(reference.Root, target.Root);
            }

            {
                // Learn equivalent with weights
                double[][] samples =
                {
                    new [] { 0, 2, 4.0 },
                    new [] { 1, 5, 2.0 },
                    new [] { 1, 5, 6.0 },
                    new [] { 1, 5, 6.0 },
                    new [] { 3, 5, 6.0 },
                    new [] { 10, 4, 10.0 },
                    new [] { -4, 7, 4.0 },
                    new [] { 7, 5, 42.0 },
                };

                int[] outputs = { 1, 1, 0, 0, 3, 5, 4, 2 };

                double[] weights = { 1, 1, 1, 1, 0, 0, 0, 0 };

                // Learn with weights
                var target = new C45Learning().Learn(samples, outputs, weights);
                AreEqual(reference.Root, target.Root);
            }
        }

        private static void AreEqual(DecisionNode a, DecisionNode b)
        {
            Assert.AreEqual(b.Comparison, a.Comparison);
            Assert.AreEqual(b.IsLeaf, a.IsLeaf);
            Assert.AreEqual(b.IsRoot, a.IsRoot);
            Assert.AreEqual(b.Output, a.Output);
            Assert.AreEqual(b.Value, a.Value);

            Assert.AreEqual(a.Branches.Count, b.Branches.Count);
            for (int i = 0; i < a.Branches.Count; i++)
                AreEqual(b.Branches[i], b.Branches[i]);
        }

        [Test]
        public void IrisDatasetTest()
        {
            #region doc_iris
            // In this example, we will process the famous Fisher's Iris dataset in 
            // which the task is to classify weather the features of an Iris flower 
            // belongs to an Iris setosa, an Iris versicolor, or an Iris virginica:
            //
            //  - https://en.wikipedia.org/wiki/Iris_flower_data_set
            //

            // First, let's load the dataset into an array of text that we can process
            string[][] text = Resources.iris_data.Split(new[] { "\r\n" },
                StringSplitOptions.RemoveEmptyEntries).Apply(x => x.Split(','));

            // The first four columns contain the flower features
            double[][] inputs = text.GetColumns(0, 1, 2, 3).To<double[][]>();

            // The last column contains the expected flower type
            string[] labels = text.GetColumn(4);

            // Since the labels are represented as text, the first step is to convert
            // those text labels into integer class labels, so we can process them
            // more easily. For this, we will create a codebook to encode class labels:
            //
            var codebook = new Codification("Output", labels);

            // With the codebook, we can convert the labels:
            int[] outputs = codebook.Translate("Output", labels);

            // Create a teaching algorithm:
            var teacher = new C45Learning()
            {
                new DecisionVariable("sepal length", DecisionVariableKind.Continuous),
                new DecisionVariable("sepal width", DecisionVariableKind.Continuous),
                new DecisionVariable("petal length", DecisionVariableKind.Continuous),
                new DecisionVariable("petal width", DecisionVariableKind.Continuous),
            };

            // Use the learning algorithm to induce a new tree:
            DecisionTree tree = teacher.Learn(inputs, outputs);

            // To get the estimated class labels, we can use
            int[] predicted = tree.Decide(inputs);

            // The classification error (0.0266) can be computed as 
            double error = new ZeroOneLoss(outputs).Loss(predicted);

            // Moreover, we may decide to convert our tree to a set of rules:
            DecisionSet rules = tree.ToRules();

            // And using the codebook, we can inspect the tree reasoning:
            string ruleText = rules.ToString(codebook, "Output",
                System.Globalization.CultureInfo.InvariantCulture);

            // The output is:
            string expected = @"Iris-setosa =: (petal length <= 2.45)
Iris-versicolor =: (petal length > 2.45) && (petal width <= 1.75) && (sepal length <= 7.05) && (sepal width <= 2.85)
Iris-versicolor =: (petal length > 2.45) && (petal width <= 1.75) && (sepal length <= 7.05) && (sepal width > 2.85)
Iris-versicolor =: (petal length > 2.45) && (petal width > 1.75) && (sepal length <= 5.95) && (sepal width > 3.05)
Iris-virginica =: (petal length > 2.45) && (petal width <= 1.75) && (sepal length > 7.05)
Iris-virginica =: (petal length > 2.45) && (petal width > 1.75) && (sepal length > 5.95)
Iris-virginica =: (petal length > 2.45) && (petal width > 1.75) && (sepal length <= 5.95) && (sepal width <= 3.05)
";
            #endregion
            expected = expected.Replace("\r\n", Environment.NewLine);

            Assert.AreEqual(0.026666666666666668, error, 1e-10);
            Assert.AreEqual(4, tree.NumberOfInputs);
            Assert.AreEqual(3, tree.NumberOfOutputs);

            double newError = ComputeError(rules, inputs, outputs);
            Assert.AreEqual(0.026666666666666668, newError, 1e-10);
            Assert.AreEqual(expected, ruleText);
        }

        [Test]
        public void iris_new_method_create_tree()
        {
            string[][] text = Resources.iris_data.Split(new[] { "\r\n" },
                StringSplitOptions.RemoveEmptyEntries).Apply(x => x.Split(','));

            double[][] inputs = text.GetColumns(0, 1, 2, 3).To<double[][]>();

            string[] labels = text.GetColumn(4);

            var codebook = new Codification("Output", labels);

            int[] outputs = codebook.Translate("Output", labels);

            DecisionVariable[] features =
            {
                new DecisionVariable("sepal length", DecisionVariableKind.Continuous),
                new DecisionVariable("sepal width", DecisionVariableKind.Continuous),
                new DecisionVariable("petal length", DecisionVariableKind.Continuous),
                new DecisionVariable("petal width", DecisionVariableKind.Continuous),
            };

            var teacher = new C45Learning(features);

            var tree = teacher.Learn(inputs, outputs);
            Assert.AreEqual(4, tree.NumberOfInputs);
            Assert.AreEqual(3, tree.NumberOfOutputs);


            // To get the estimated class labels, we can use
            int[] predicted = tree.Decide(inputs);

            // And the classification error can be computed as 
            double error = new ZeroOneLoss(outputs) // 0.0266
            {
                Mean = true
            }.Loss(tree.Decide(inputs));

            // Moreover, we may decide to convert our tree to a set of rules:
            DecisionSet rules = tree.ToRules();

            // And using the codebook, we can inspect the tree reasoning:
            string ruleText = rules.ToString(codebook, "Output",
                System.Globalization.CultureInfo.InvariantCulture);

            // The output is:
            string expected = @"Iris-setosa =: (petal length <= 2.45)
Iris-versicolor =: (petal length > 2.45) && (petal width <= 1.75) && (sepal length <= 7.05) && (sepal width <= 2.85)
Iris-versicolor =: (petal length > 2.45) && (petal width <= 1.75) && (sepal length <= 7.05) && (sepal width > 2.85)
Iris-versicolor =: (petal length > 2.45) && (petal width > 1.75) && (sepal length <= 5.95) && (sepal width > 3.05)
Iris-virginica =: (petal length > 2.45) && (petal width <= 1.75) && (sepal length > 7.05)
Iris-virginica =: (petal length > 2.45) && (petal width > 1.75) && (sepal length > 5.95)
Iris-virginica =: (petal length > 2.45) && (petal width > 1.75) && (sepal length <= 5.95) && (sepal width <= 3.05)
";
            expected = expected.Replace("\r\n", Environment.NewLine);

            Assert.AreEqual(0.026666666666666668, error, 1e-10);

            double newError = ComputeError(rules, inputs, outputs);
            Assert.AreEqual(0.026666666666666668, newError, 1e-10);
            Assert.AreEqual(expected, ruleText);
        }

        [Test]
        public void new_method_create_tree()
        {
            #region doc_simplest
            // In this example, we will process the famous Fisher's Iris dataset in 
            // which the task is to classify weather the features of an Iris flower 
            // belongs to an Iris setosa, an Iris versicolor, or an Iris virginica:
            //
            //  - https://en.wikipedia.org/wiki/Iris_flower_data_set
            //

            // First, let's load the dataset into an array of text that we can process
            string[][] text = Resources.iris_data.Split(new[] { "\r\n" },
                StringSplitOptions.RemoveEmptyEntries).Apply(x => x.Split(','));

            // The first four columns contain the flower features
            double[][] inputs = text.GetColumns(0, 1, 2, 3).To<double[][]>();

            // The last column contains the expected flower type
            string[] labels = text.GetColumn(4);

            // Since the labels are represented as text, the first step is to convert
            // those text labels into integer class labels, so we can process them
            // more easily. For this, we will create a codebook to encode class labels:
            //
            var codebook = new Codification("Output", labels);

            // With the codebook, we can convert the labels:
            int[] outputs = codebook.Translate("Output", labels);

            // And we can use the C4.5 for learning:
            C45Learning teacher = new C45Learning();

            // Finally induce the tree from the data:
            var tree = teacher.Learn(inputs, outputs);

            // To get the estimated class labels, we can use
            int[] predicted = tree.Decide(inputs);

            // The classification error (0.0266) can be computed as 
            double error = new ZeroOneLoss(outputs).Loss(predicted);

            // Moreover, we may decide to convert our tree to a set of rules:
            DecisionSet rules = tree.ToRules();

            // And using the codebook, we can inspect the tree reasoning:
            string ruleText = rules.ToString(codebook, "Output",
                System.Globalization.CultureInfo.InvariantCulture);

            // The output is:
            string expected = @"Iris-setosa =: (2 <= 2.45)
Iris-versicolor =: (2 > 2.45) && (3 <= 1.75) && (0 <= 7.05) && (1 <= 2.85)
Iris-versicolor =: (2 > 2.45) && (3 <= 1.75) && (0 <= 7.05) && (1 > 2.85)
Iris-versicolor =: (2 > 2.45) && (3 > 1.75) && (0 <= 5.95) && (1 > 3.05)
Iris-virginica =: (2 > 2.45) && (3 <= 1.75) && (0 > 7.05)
Iris-virginica =: (2 > 2.45) && (3 > 1.75) && (0 > 5.95)
Iris-virginica =: (2 > 2.45) && (3 > 1.75) && (0 <= 5.95) && (1 <= 3.05)
";
            #endregion

            expected = expected.Replace("\r\n", Environment.NewLine);

            Assert.AreEqual(0.026666666666666668, error, 1e-10);

            double newError = ComputeError(rules, inputs, outputs);
            Assert.AreEqual(0.026666666666666668, newError, 1e-10);
            Assert.AreEqual(expected, ruleText);
        }

        [Test]
        [Category("Random")]
        public void AttributeReuseTest1()
        {
            string[][] text = Resources.iris_data
                .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Apply(x => x.Split(','));

            Assert.AreEqual(150, text.Rows());
            Assert.AreEqual(5, text.Columns());
            Assert.AreEqual("Iris-setosa", text[0].Get(-1));
            Assert.AreEqual("Iris-virginica", text.Get(-1).Get(-1));

            double[][] inputs = new double[text.Length][];
            for (int i = 0; i < inputs.Length; i++)
                inputs[i] = text[i].First(4).Apply(s => Double.Parse(s, CultureInfo.InvariantCulture));

            string[] labels = text.GetColumn(4);

            Codification codebook = new Codification("Label", labels);

            int[] outputs = codebook.Translate("Label", labels);


            DecisionVariable[] features =
            {
                new DecisionVariable("sepal length", DecisionVariableKind.Continuous),
                new DecisionVariable("sepal width", DecisionVariableKind.Continuous),
                new DecisionVariable("petal length", DecisionVariableKind.Continuous),
                new DecisionVariable("petal width", DecisionVariableKind.Continuous),
            };


            DecisionTree tree = new DecisionTree(features, codebook.Columns[0].Symbols);

            C45Learning teacher = new C45Learning(tree);

            teacher.Join = 3;

            double error = teacher.Run(inputs, outputs);
            Assert.AreEqual(0.00, error, 1e-10);

            DecisionSet rules = tree.ToRules();

            double newError = ComputeError(rules, inputs, outputs);
            Assert.AreEqual(0.00, newError, 1e-10);

            string ruleText = rules.ToString(codebook,
                System.Globalization.CultureInfo.InvariantCulture);

            string expected = @"0 =: (petal length <= 2.45)
1 =: (petal length > 2.45) && (petal width > 1.75) && (petal length <= 4.85) && (sepal length <= 5.95)
1 =: (petal length > 2.45) && (petal width <= 1.75) && (sepal length <= 7.05) && (petal length <= 4.95) && (petal width <= 1.65)
1 =: (petal length > 2.45) && (petal width <= 1.75) && (sepal length <= 7.05) && (petal length > 4.95) && (petal width > 1.55)
2 =: (petal length > 2.45) && (petal width <= 1.75) && (sepal length > 7.05)
2 =: (petal length > 2.45) && (petal width > 1.75) && (petal length > 4.85)
2 =: (petal length > 2.45) && (petal width > 1.75) && (petal length <= 4.85) && (sepal length > 5.95)
2 =: (petal length > 2.45) && (petal width <= 1.75) && (sepal length <= 7.05) && (petal length <= 4.95) && (petal width > 1.65)
2 =: (petal length > 2.45) && (petal width <= 1.75) && (sepal length <= 7.05) && (petal length > 4.95) && (petal width <= 1.55)
";
            expected = expected.Replace("\r\n", Environment.NewLine);

            Assert.AreEqual(expected, ruleText);
        }

        public double ComputeError(DecisionSet rules, double[][] inputs, int[] outputs)
        {
            int miss = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                if (rules.Compute(inputs[i]) != outputs[i])
                    miss++;
            }

            return (double)miss / inputs.Length;
        }

        [Test]
        public void same_input_different_output_minimal()
        {
            double[][] inputs = new double[][] {
                new double[] { 0 },
                new double[] { 0 }
            };

            int[] outputs = new int[] {
                1,
                0
            };


            DecisionVariable[] variables = { new DecisionVariable("x", DecisionVariableKind.Continuous) };

            DecisionTree decisionTree = new DecisionTree(variables, 2);
            C45Learning c45Learning = new C45Learning(decisionTree);
            c45Learning.ParallelOptions.MaxDegreeOfParallelism = 1;
            c45Learning.Run(inputs, outputs); // System.AggregateException thrown here

            Assert.AreEqual(decisionTree.Decide(new[] { 0 }), 0);
        }

        [Test]
        public void same_input_different_output()
        {
            double[][] inputs = new double[][] {
                new double[] { 1 },
                new double[] { 0 },
                new double[] { 2 },
                new double[] { 3 },
                new double[] { 0 },
            };

            int[] outputs = new int[] {
                11,
                00,
                22,
                33,
                01
            };


            DecisionVariable[] variables = { new DecisionVariable("x", DecisionVariableKind.Continuous) };

            DecisionTree decisionTree = new DecisionTree(variables, 34);
            C45Learning c45Learning = new C45Learning(decisionTree)
            {
                Join = 10,
                MaxHeight = 10
            };
            c45Learning.Run(inputs, outputs); // System.AggregateException thrown here

            int[] actual = decisionTree.Decide(inputs);

            Assert.AreEqual(11, actual[0]);
            Assert.AreEqual(00, actual[1]);
            Assert.AreEqual(22, actual[2]);
            Assert.AreEqual(33, actual[3]);
            Assert.AreEqual(00, actual[4]);
        }

        [Test]
        public void max_height()
        {
            double[][] inputs =
           {
                new double[] { 1, 4 },
                new double[] { 0, 2 },
                new double[] { 2, 1 },
                new double[] { 3, 7 },
                new double[] { 0, 1 },
            };

            int[] outputs = { 1, 1, 2, 3, 1 };

            var target = new C45Learning()
            {
                MaxHeight = 1
            };

            var tree = target.Learn(inputs, outputs);

            int height = tree.GetHeight();
            Assert.AreEqual(1, height);
        }

        [Test]
        public void missing_values()
        {
            var dataset = new WisconsinOriginalBreastCancer();
            int?[][] inputs = dataset.Features;
            int[] outputs = dataset.ClassLabels;

            var c45 = new C45Learning()
            {
            };

            var tree = c45.Learn(inputs, outputs);

            int height = tree.GetHeight();

            Assert.AreEqual(4, height);
            int[] predicted = tree.Decide(inputs);

            double error = new ZeroOneLoss(outputs).Loss(predicted);
            Assert.AreEqual(0.0028612303290414878, error, 1e-8);
        }
    }
}
