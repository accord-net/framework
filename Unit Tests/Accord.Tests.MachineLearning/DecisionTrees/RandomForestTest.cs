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

    [TestFixture]
    public class RandomForestTest
    {

        [Test]
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
    }
}
