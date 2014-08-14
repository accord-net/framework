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

namespace Accord.Tests.Statistics
{
    using System;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class ReceiverOperatingCharacteristicTest
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
        public void ComputeTest()
        {
            // Example from
            // http://faculty.vassar.edu/lowry/roc1.html

            double[,] data = 
            { 
                { 4,  1 },                { 4,  1 },
                { 4,  1 },                { 4,  1 },
                { 4,  1 },                { 4,  1 },
                { 4,  1 },                { 4,  1 },
                { 4,  1 },                { 4,  1 },
                { 4,  1 },                { 4,  1 },
                { 4,  1 },                { 4,  1 },
                { 4,  1 },                { 4,  1 },
                { 4,  1 },                { 4,  1 }, // 18
                { 4,  0 },

                { 6,  1 },                 { 6,  1 }, 
                { 6,  1 },                 { 6,  1 }, 
                { 6,  1 },                 { 6,  1 }, 
                { 6,  1 }, // 7

                { 6,  0 },                 { 6,  0 },
                { 6,  0 },                 { 6,  0 },
                { 6,  0 },                 { 6,  0 },
                { 6,  0 },                 { 6,  0 },
                { 6,  0 },                 { 6,  0 },
                { 6,  0 },                 { 6,  0 },
                { 6,  0 },                 { 6,  0 },
                { 6,  0 },                 { 6,  0 },
                { 6,  0 }, // 17

                { 8,  1 },                { 8,  1 },
                { 8,  1 },                { 8,  1 }, // 4

                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 },
                { 8,  0 },                { 8,  0 }, // 36

                { 9, 1 },                 { 9, 1 },
                { 9, 1 }, // 3

                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 },
                { 9, 0 },                { 9, 0 }, 
                { 9, 0 },                { 9, 0 }, 
                { 9, 0 }, // 39
            };


            double[] measurement = data.GetColumn(1);
            double[] prediction = data.GetColumn(0);

            var roc = new ReceiverOperatingCharacteristic(measurement, prediction);
            double[] cutpoints = { 5, 7, 9, double.PositiveInfinity };

            roc.Compute(cutpoints);

            Assert.AreEqual(32, roc.Positives);
            Assert.AreEqual(93, roc.Negatives);

            Assert.AreEqual(4, roc.Points.Count);
            var p1 = roc.Points[0];
            var p2 = roc.Points[1];
            var p3 = roc.Points[2];
            var p4 = roc.Points[3];

            Assert.AreEqual(18, p1.FalseNegatives);
            Assert.AreEqual(18 + 7, p2.FalseNegatives);
            Assert.AreEqual(18 + 7 + 4, p3.FalseNegatives);
            Assert.AreEqual(18 + 7 + 4 + 3, p4.FalseNegatives);

            Assert.AreEqual(1, p1.TrueNegatives);
            Assert.AreEqual(1 + 17, p2.TrueNegatives);
            Assert.AreEqual(1 + 17 + 36, p3.TrueNegatives);
            Assert.AreEqual(1 + 17 + 36 + 39, p4.TrueNegatives);


            double area = roc.Area;
            double error = roc.StandardError;

            // Area should be near 0.87
            Assert.AreEqual(0.87, area, 0.011);
            Assert.IsFalse(Double.IsNaN(area));

            // Assert.AreEqual(0.043781206163219656, error); // HanleyMcNeil
            Assert.AreEqual(0.04485087617325112, error); // DeLong estimate
        }

        [TestMethod()]
        public void ReceiverOperatingCharacteristicConstructorTest2()
        {
            double[] measurement = { 0, 0, 0, 0, 0, 1, 1, 1 };
            double[] prediction = { 0, 0, 0.5, 0.5, 1, 1, 1, 1 };
            ReceiverOperatingCharacteristic target = new ReceiverOperatingCharacteristic(measurement, prediction);

            target.Compute(0.5, true);
            Assert.AreEqual(target.Points.Count, 4);
            var p1 = target.Points[0];
            var p2 = target.Points[1];
            var p3 = target.Points[2];
            var p4 = target.Points[3];

            Assert.AreEqual(p1.Sensitivity, 1);
            Assert.AreEqual(1 - p1.Specificity, 1);
            Assert.AreEqual(p4.Sensitivity, 0);
            Assert.AreEqual(1 - p4.Specificity, 0);

            target.Compute(0.5, false);
            Assert.AreEqual(target.Points.Count, 3);


            target.Compute(new double[] { 0.0, 0.4, 0.6, 1.0 });

            Assert.AreEqual(target.Points.Count, 4);
            Assert.AreEqual(target.Negatives, 5);
            Assert.AreEqual(target.Positives, 3);
            Assert.AreEqual(target.Observations, 8);

            foreach (var point in target.Points)
            {
                Assert.AreEqual(point.Samples, 8);
                Assert.AreEqual(point.ActualNegatives, 5);
                Assert.AreEqual(point.ActualPositives, 3);

                if (point.Cutoff == 0.0)
                {
                    Assert.AreEqual(point.PredictedNegatives, 0);
                    Assert.AreEqual(point.PredictedPositives, 8);
                }
                else if (point.Cutoff == 0.4)
                {
                    Assert.AreEqual(point.PredictedNegatives, 2);
                    Assert.AreEqual(point.PredictedPositives, 6);
                }
                else
                {
                    Assert.AreEqual(point.PredictedNegatives, 4);
                    Assert.AreEqual(point.PredictedPositives, 4);
                }

            }

            Assert.AreEqual(target.Area, 0.8);
            // Assert.AreEqual(target.StandardError, 0.1821680136170595); // HanleyMcNeil
            Assert.AreEqual(0.1, target.StandardError); // De Long

        }

        [TestMethod()]
        public void ReceiverOperatingCharacteristicConstructorTest3()
        {
            // This example shows how to measure the accuracy of a 
            // binary classifier using a ROC curve. For this example,
            // we will be creating a Support Vector Machine trained
            // on the following instances:

            double[][] inputs =
            {
                // Those are from class -1
                new double[] { 2, 4, 0 },
                new double[] { 5, 5, 1 },
                new double[] { 4, 5, 0 },
                new double[] { 2, 5, 5 },
                new double[] { 4, 5, 1 },
                new double[] { 4, 5, 0 },
                new double[] { 6, 2, 0 },
                new double[] { 4, 1, 0 },

                // Those are from class +1
                new double[] { 1, 4, 5 },
                new double[] { 7, 5, 1 },
                new double[] { 2, 6, 0 },
                new double[] { 7, 4, 7 },
                new double[] { 4, 5, 0 },
                new double[] { 6, 2, 9 },
                new double[] { 4, 1, 6 },
                new double[] { 7, 2, 9 },
            };

            int[] outputs =
            {
                -1, -1, -1, -1, -1, -1, -1, -1, // fist eight from class -1
                +1, +1, +1, +1, +1, +1, +1, +1  // last eight from class +1
            };

            // Create a linear Support Vector Machine with 4 inputs
            SupportVectorMachine machine = new SupportVectorMachine(inputs: 3);

            // Create the sequential minimal optimization teacher
            SequentialMinimalOptimization learn = new SequentialMinimalOptimization(machine, inputs, outputs);

            // Run the learning algorithm
            double error = learn.Run();

            // Extract the input labels predicted by the machine
            double[] predicted = new double[inputs.Length];
            for (int i = 0; i < predicted.Length; i++)
                predicted[i] = machine.Compute(inputs[i]);


            // Create a new ROC curve to assess the performance of the model
            var roc = new ReceiverOperatingCharacteristic(outputs, predicted);

            roc.Compute(100); // Compute a ROC curve with 100 points
            /*
                        // Generate a connected scatter plot for the ROC curve and show it on-screen
                        ScatterplotBox.Show(roc.GetScatterplot(includeRandom: true), nonBlocking: true)

                            .SetSymbolSize(0)      // do not display data points
                            .SetLinesVisible(true) // show lines connecting points
                            .SetScaleTight(true)   // tighten the scale to points
                            .WaitForClose();
            */

            Assert.AreEqual(0.7890625, roc.Area);
            // Assert.AreEqual(0.1174774, roc.StandardError, 1e-6); HanleyMcNeil
            Assert.AreEqual(0.11958120746409709, roc.StandardError, 1e-6);
        }

        [TestMethod()]
        public void DeLongVarianceTest()
        {
            // Example from Sampling Variability of Nonparametric Estimates of the
            // Areas under Receiver Operating Characteristic Curves: An Update

            bool yes = true;
            bool no = false;

            bool[] expected = 
            {
                /* 1*/ yes,
                /* 2*/ no,
                /* 3*/ yes,
                /* 4*/ no,
                /* 5*/ no,
                /* 6*/ yes,
                /* 7*/ yes,
                /* 8*/ no,
                /* 9*/ no,
                /*10*/ yes,
                /*11*/ no,
                /*12*/ no,
                /*13*/ yes,
                /*14*/ no,
                /*15*/ no
            };

            int[] actual = 
            {
                /* 1*/ 1,
                /* 2*/ 2,
                /* 3*/ 5,
                /* 4*/ 1,
                /* 5*/ 1,
                /* 6*/ 1,
                /* 7*/ 2,
                /* 8*/ 1,
                /* 9*/ 2,
                /*10*/ 2,
                /*11*/ 1,
                /*12*/ 1,
                /*13*/ 5,
                /*14*/ 1,
                /*15*/ 1
            };

            ReceiverOperatingCharacteristic curve = new ReceiverOperatingCharacteristic(expected, actual);

            curve.Compute(10);

            Assert.AreEqual(6, curve.PositiveResults.Length);
            Assert.AreEqual(1, curve.PositiveResults[0]);
            Assert.AreEqual(5, curve.PositiveResults[1]);
            Assert.AreEqual(1, curve.PositiveResults[2]);
            Assert.AreEqual(2, curve.PositiveResults[3]);
            Assert.AreEqual(2, curve.PositiveResults[4]);
            Assert.AreEqual(5, curve.PositiveResults[5]);

            Assert.AreEqual(9, curve.NegativeResults.Length);
            Assert.AreEqual(2, curve.NegativeResults[0]);
            Assert.AreEqual(1, curve.NegativeResults[1]);
            Assert.AreEqual(1, curve.NegativeResults[2]);
            Assert.AreEqual(1, curve.NegativeResults[3]);
            Assert.AreEqual(2, curve.NegativeResults[4]);
            Assert.AreEqual(1, curve.NegativeResults[5]);
            Assert.AreEqual(1, curve.NegativeResults[6]);
            Assert.AreEqual(1, curve.NegativeResults[7]);
            Assert.AreEqual(1, curve.NegativeResults[8]);


            Assert.AreEqual(6, curve.PositiveAccuracies.Length);
            Assert.AreEqual(0.3888, curve.PositiveAccuracies[0], 1e-4);
            Assert.AreEqual(1.0000, curve.PositiveAccuracies[1], 1e-4);
            Assert.AreEqual(0.3888, curve.PositiveAccuracies[2], 1e-4);
            Assert.AreEqual(0.8888, curve.PositiveAccuracies[3], 1e-4);
            Assert.AreEqual(0.8888, curve.PositiveAccuracies[4], 1e-4);
            Assert.AreEqual(1.0000, curve.PositiveAccuracies[5], 1e-4);

            Assert.AreEqual(9, curve.NegativeAccuracies.Length);
            Assert.AreEqual(0.5000, curve.NegativeAccuracies[0], 1e-4);
            Assert.AreEqual(0.8333, curve.NegativeAccuracies[1], 1e-4);
            Assert.AreEqual(0.8333, curve.NegativeAccuracies[2], 1e-4);
            Assert.AreEqual(0.8333, curve.NegativeAccuracies[3], 1e-4);
            Assert.AreEqual(0.5000, curve.NegativeAccuracies[4], 1e-4);
            Assert.AreEqual(0.8333, curve.NegativeAccuracies[5], 1e-4);
            Assert.AreEqual(0.8333, curve.NegativeAccuracies[6], 1e-4);
            Assert.AreEqual(0.8333, curve.NegativeAccuracies[7], 1e-4);
            Assert.AreEqual(0.8333, curve.NegativeAccuracies[8], 1e-4);

            Assert.IsFalse(curve.NegativeAccuracies.HasNaN());
            Assert.IsFalse(curve.PositiveAccuracies.HasNaN());

            Assert.AreEqual(0.1285, curve.StandardError, 1e-4);
            Assert.AreEqual(0.0165, curve.Variance, 1e-4);

            Assert.IsFalse(Double.IsNaN(curve.StandardError));
            Assert.IsFalse(Double.IsNaN(curve.Variance));
        }

        [TestMethod()]
        public void DeLongComparisonTest()
        {
            // Example from Sampling Variability of Nonparametric Estimates of the
            // Areas under Receiver Operating Characteristic Curves: An Update

            bool yes = true;
            bool no = false;

            bool[] expected = 
            {
                /* 1*/ yes,
                /* 2*/ no,
                /* 3*/ yes,
                /* 4*/ no,
                /* 5*/ no,
                /* 6*/ yes,
                /* 7*/ yes,
                /* 8*/ no,
                /* 9*/ no,
                /*10*/ yes,
                /*11*/ no,
                /*12*/ no,
                /*13*/ yes,
                /*14*/ no,
                /*15*/ no
            };

            int[] actual1 = 
            {
                /* 1*/ 1,
                /* 2*/ 2,
                /* 3*/ 5,
                /* 4*/ 1,
                /* 5*/ 1,
                /* 6*/ 1,
                /* 7*/ 2,
                /* 8*/ 1,
                /* 9*/ 2,
                /*10*/ 2,
                /*11*/ 1,
                /*12*/ 1,
                /*13*/ 5,
                /*14*/ 1,
                /*15*/ 1
            };

            int[] actual2 = 
            {
                /* 1*/ 1,
                /* 2*/ 1,
                /* 3*/ 5,
                /* 4*/ 1,
                /* 5*/ 1,
                /* 6*/ 1,
                /* 7*/ 4,
                /* 8*/ 1,
                /* 9*/ 2,
                /*10*/ 2,
                /*11*/ 1,
                /*12*/ 1,
                /*13*/ 5,
                /*14*/ 1,
                /*15*/ 1
            };

            ReceiverOperatingCharacteristic a = new ReceiverOperatingCharacteristic(expected, actual1);
            ReceiverOperatingCharacteristic b = new ReceiverOperatingCharacteristic(expected, actual2);

            a.Compute(10);
            b.Compute(10);

            TwoReceiverOperatingCurveTest test = new TwoReceiverOperatingCurveTest(a, b);

            Assert.AreEqual(-1.1351915229662422, test.Statistic);

        }
    }
}
