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
    using Accord.Math.Differentiation;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using NUnit.Framework;
    using Accord.Math;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Kernels;
    using Math.Optimization.Losses;
    using Accord.Statistics.Models.Regression.Linear;

    [TestFixture]
    public class ProbabilisticDualCoordinateDescentTest
    {

        [Test]
        public void RunTest()
        {
            var dataset = SequentialMinimalOptimizationTest.GetYingYang();

            double[][] inputs = dataset.Submatrix(null, 0, 1).ToJagged();
            int[] labels = dataset.GetColumn(2).ToInt32();

            Accord.Math.Random.Generator.Seed = 0;

            var svm = new SupportVectorMachine(inputs: 2);
            var teacher = new ProbabilisticDualCoordinateDescent(svm, inputs, labels);

            teacher.Tolerance = 1e-10;
            teacher.UseComplexityHeuristic = true;

            Assert.IsFalse(svm.IsProbabilistic);
            double error = teacher.Run();
            Assert.IsTrue(svm.IsProbabilistic);

            double[] weights = svm.ToWeights();

            Assert.AreEqual(0.13, error);
            Assert.AreEqual(3, weights.Length);
            Assert.AreEqual(-0.52913278486359605, weights[0], 1e-4);
            Assert.AreEqual(-1.6426069611746976, weights[1], 1e-4);
            Assert.AreEqual(-0.77766953652287762, weights[2], 1e-4);

            Assert.AreEqual(svm.Threshold, weights[0]);
        }

        [Test]
        public void logistic_regression_test2()
        {
            Accord.Math.Random.Generator.Seed = 0;

            #region doc_logreg
            // Declare some training data. This is exactly the same
            // data used in the LogisticRegression documentation page

            // Suppose we have the following data about some patients.
            // The first variable is continuous and represent patient
            // age. The second variable is dichotomic and give whether
            // they smoke or not (This is completely fictional data).

            // We also know if they have had lung cancer or not, and 
            // we would like to know whether smoking has any connection
            // with lung cancer (This is completely fictional data).

            double[][] input =
            {              // age, smokes?, had cancer?
                new double[] { 55,    0  }, // false - no cancer
                new double[] { 28,    0  }, // false
                new double[] { 65,    1  }, // false
                new double[] { 46,    0  }, // true  - had cancer
                new double[] { 86,    1  }, // true
                new double[] { 56,    1  }, // true
                new double[] { 85,    0  }, // false
                new double[] { 33,    0  }, // false
                new double[] { 21,    1  }, // false
                new double[] { 42,    1  }, // true
            };

            double[] output = // Whether each patient had lung cancer or not
            {
                0, 0, 0, 1, 1, 1, 0, 0, 0, 1
            };

            // Create the probabilistic-SVM learning algorithm
            var teacher = new ProbabilisticDualCoordinateDescent()
            {
                Tolerance = 1e-10,
                Complexity = 1e+10, // learn a hard-margin model
            };

            // Learn the support vector machine
            var svm = teacher.Learn(input, output);

            // Convert the svm to logistic regression
            var regression = (LogisticRegression)svm;

            // Compute the predicted outcome for inputs
            bool[] predicted = regression.Decide(input);

            // Compute probability scores for the outputs
            double[] scores = regression.Probability(input);

            // Compute odds-ratio as in the LogisticRegression example
            double ageOdds = regression.GetOddsRatio(1);   // 1.0430443799578411
            double smokeOdds = regression.GetOddsRatio(2); // 7.2414593749145508

            // Compute the classification error as in SVM example
            double error = new ZeroOneLoss(output).Loss(predicted);
            #endregion

            var rsvm = (SupportVectorMachine)regression;
            Assert.AreEqual(2, rsvm.NumberOfInputs);
            Assert.AreEqual(1, rsvm.NumberOfOutputs);
            Assert.AreEqual(2, rsvm.NumberOfClasses);
            double[] svmpred = svm.Probability(input);
            Assert.IsTrue(scores.IsEqual(svmpred, 1e-10));

            Assert.AreEqual(0.4, error);
            Assert.AreEqual(1.0430443799578411, ageOdds, 1e-4);
            Assert.AreEqual(7.2414593749145508, smokeOdds, 1e-4);

            Assert.AreEqual(-21.4120677536517, regression.Intercept, 1e-8);
            Assert.AreEqual(-21.4120677536517, regression.Coefficients[0], 1e-8);
            Assert.AreEqual(0.042143725408546939, regression.Coefficients[1], 1e-8);
            Assert.AreEqual(1.9798227572056906, regression.Coefficients[2], 1e-8);
        }

        [Test]
        public void logistic_regression_sparse_test()
        {
            Accord.Math.Random.Generator.Seed = 0;

            #region doc_logreg_sparse
            // Declare some training data. This is exactly the same
            // data used in the LogisticRegression documentation page

            // Suppose we have the following data about some patients.
            // The first variable is continuous and represent patient
            // age. The second variable is dichotomic and give whether
            // they smoke or not (This is completely fictional data).

            // We also know if they have had lung cancer or not, and 
            // we would like to know whether smoking has any connection
            // with lung cancer (This is completely fictional data).

            Sparse<double>[] input =
            {                               // age, smokes?, had cancer?
                Sparse.FromDense(new double[] { 55,    0  }), // false - no cancer
                Sparse.FromDense(new double[] { 28,    0  }), // false
                Sparse.FromDense(new double[] { 65,    1  }), // false
                Sparse.FromDense(new double[] { 46,    0  }), // true  - had cancer
                Sparse.FromDense(new double[] { 86,    1  }), // true
                Sparse.FromDense(new double[] { 56,    1  }), // true
                Sparse.FromDense(new double[] { 85,    0  }), // false
                Sparse.FromDense(new double[] { 33,    0  }), // false
                Sparse.FromDense(new double[] { 21,    1  }), // false
                Sparse.FromDense(new double[] { 42,    1  }), // true
            };

            double[] output = // Whether each patient had lung cancer or not
            {
                0, 0, 0, 1, 1, 1, 0, 0, 0, 1
            };

            // Create the probabilistic-SVM learning algorithm
            var teacher = new ProbabilisticDualCoordinateDescent<Linear, Sparse<double>>()
            {
                Tolerance = 1e-10,
                Complexity = 1e+10, // learn a hard-margin model
            };

            // Learn the support vector machine
            var svm = teacher.Learn(input, output);

            // Convert the svm to logistic regression
            var regression = (LogisticRegression)svm;

            // Compute the predicted outcome for inputs
            bool[] predicted = regression.Decide(input.ToDense(regression.NumberOfInputs));

            // Compute probability scores for the outputs
            double[] scores = regression.Probability(input.ToDense(regression.NumberOfInputs));

            // Compute odds-ratio as in the LogisticRegression example
            double ageOdds = regression.GetOddsRatio(1);   // 1.0430443799578411
            double smokeOdds = regression.GetOddsRatio(2); // 7.2414593749145508

            // Compute the classification error as in SVM example
            double error = new ZeroOneLoss(output).Loss(predicted);
            #endregion

            var rsvm = (SupportVectorMachine)regression;
            Assert.AreEqual(2, rsvm.NumberOfInputs);
            Assert.AreEqual(1, rsvm.NumberOfOutputs);
            Assert.AreEqual(2, rsvm.NumberOfClasses);
            double[] svmpred = svm.Probability(input);
            Assert.IsTrue(scores.IsEqual(svmpred, 1e-10));

            Assert.AreEqual(0.4, error);
            Assert.AreEqual(1.0430443799578411, ageOdds, 1e-4);
            Assert.AreEqual(7.2414593749145508, smokeOdds, 1e-4);

            Assert.AreEqual(-21.4120677536517, regression.Intercept, 1e-8);
            Assert.AreEqual(-21.4120677536517, regression.Coefficients[0], 1e-8);
            Assert.AreEqual(0.042143725408546939, regression.Coefficients[1], 1e-8);
            Assert.AreEqual(1.9798227572056906, regression.Coefficients[2], 1e-8);
        }

    }
}
