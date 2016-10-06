// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using Accord.IO;
    using Accord.MachineLearning.DecisionTrees;
    using Accord.MachineLearning.DecisionTrees.Learning;
    using Accord.MachineLearning.DecisionTrees.Rules;
    using Accord.Math;
    using Accord.Statistics.Filters;
    using Accord.Tests.MachineLearning.Properties;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using Math.Optimization.Losses;

    [TestFixture]
    public class RandomForestTest
    {

        [Test, Ignore]
        public void constructor_test()
        {
            var times = ReadCSV(Properties.Resources.times);
            var features = ReadCSV(Properties.Resources.features);
            var didSolve = times.Select(list => list.Select(d => d < 5000).ToList()).ToList();
            var foldCount = 10;
            for (int i = 0; i < foldCount; i++)
            {
                var elementsPerFold = didSolve.Count / foldCount;
                var y_test = didSolve.Skip(i * elementsPerFold).Take(elementsPerFold);
                var y_train = didSolve.Except(y_test).ToList();
                var x_test = features.Skip(i * elementsPerFold).Take(elementsPerFold);
                var x_train = features.Except(x_test);

                var allSolverPredictions = new List<bool[]>();
                for (int j = 0; j < y_train.First().Count; j++)
                {
                    var y_train_current_solver = y_train.Select(list => list.Skip(j).First()).Select(b => b ? 1 : 0);
                    var randomForestLearning = new RandomForestLearning()
                    {
                        Trees = 10
                    };
                    var currentSolverPredictions = new List<bool>();

                    var randomForest = randomForestLearning.Learn(x_train.Select(list => list.ToArray()).ToArray(),
                        y_train_current_solver.ToArray());

                    foreach (var test_instance in x_test)
                    {
                        var compute = randomForest.Compute(test_instance.ToArray());
                        currentSolverPredictions.Add(compute != 0);
                    }
                    allSolverPredictions.Add(currentSolverPredictions.ToArray());
                }

                Assert.AreEqual(allSolverPredictions.Count, 29);
                foreach (var p in allSolverPredictions)
                    Assert.AreEqual(p.Length, 424);
            }
        }

        private static List<List<double>> ReadCSV(string text)
        {
            CsvReader reader = CsvReader.FromText(text, hasHeaders: true);

            var list = new List<List<double>>();

            foreach (string[] r in reader)
            {
                //Process row
                list.Add(r
                    .Skip(1)
                    .Select(x => double.Parse(x, System.Globalization.CultureInfo.InvariantCulture))
                    .ToList());
            }

            return list;
        }

        [Test]
        public void test_learn()
        {
            #region doc_iris
            // Fix random seed for reproducibility
            Accord.Math.Random.Generator.Seed = 1;

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

            // Create the forest learning algorithm
            var teacher = new RandomForestLearning()
            {
                NumberOfTrees = 10, // use 10 trees in the forest
            };

            // Finally, learn a random forest from data
            var forest = teacher.Learn(inputs, outputs);

            // We can estimate class labels using
            int[] predicted = forest.Decide(inputs);

            // And the classification error (0.0006) can be computed as 
            double error = new ZeroOneLoss(outputs).Loss(forest.Decide(inputs));
            #endregion

            Assert.AreEqual(0.0066666666666666671, error, 1e-10);
            //Assert.IsTrue(outputs.IsEqual(predicted, 1e-10));
        }

        [Test]
        public void LargeRunTest()
        {
            #region doc_nursery
            // Fix random seed for reproducibility
            Accord.Math.Random.Generator.Seed = 1;

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

            // From now on, we can start creating the decision tree.
            //
            var attributes = DecisionVariable.FromCodebook(codebook, inputColumns);

            // Now, let's create the forest learning algorithm
            var teacher = new RandomForestLearning(attributes)
            {
                NumberOfTrees = 1,
                SampleRatio = 1.0
            };

            // Finally, learn a random forest from data
            var forest = teacher.Learn(inputs, outputs);

            // We can estimate class labels using
            int[] predicted = forest.Decide(inputs);

            // And the classification error (0) can be computed as 
            double error = new ZeroOneLoss(outputs).Loss(forest.Decide(inputs));
            #endregion

            Assert.AreEqual(0, error, 1e-10);
            Assert.IsTrue(outputs.IsEqual(predicted));

            Assert.AreEqual(12960, lines.Length);
            Assert.AreEqual("usual,proper,complete,1,convenient,convenient,nonprob,recommended,recommend", lines[0]);
            Assert.AreEqual("great_pret,very_crit,foster,more,critical,inconv,problematic,not_recom,not_recom", lines[lines.Length - 1]);

            Assert.AreEqual(0, error);

            for (int i = 0; i < inputs.Length; i++)
            {
                int expected = outputs[i];
                int actual = forest.Compute(inputs[i]);

                Assert.AreEqual(expected, actual);
            }

        }
    }
}
