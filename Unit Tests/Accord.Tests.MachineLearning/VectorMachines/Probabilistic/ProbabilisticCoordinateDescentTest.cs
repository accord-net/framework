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
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Models.Regression;
    using NUnit.Framework;
    using Accord.Statistics.Kernels;
    using Math.Optimization.Losses;

    [TestFixture]
    public class ProbabilisticCoordinateDescentTest
    {

        [Test]
        public void RunTest()
        {
            double[][] input =
            {
                new double[] { 55, 0 }, // 0 - no cancer
                new double[] { 28, 0 }, // 0
                new double[] { 65, 1 }, // 0
                new double[] { 46, 0 }, // 1 - have cancer
                new double[] { 86, 1 }, // 1
                new double[] { 56, 1 }, // 1
                new double[] { 85, 0 }, // 0
                new double[] { 33, 0 }, // 0
                new double[] { 21, 1 }, // 0
                new double[] { 42, 1 }, // 1
            };

            double[] output =
            {
                0, 0, 0, 1, 1, 1, 0, 0, 0, 1
            };

            int[] labels = output.Apply(x => x > 0 ? +1 : -1);

            var svm = new SupportVectorMachine(inputs: 2);
            Assert.AreEqual(2, svm.NumberOfInputs);
            Assert.AreEqual(1, svm.NumberOfOutputs);
            Assert.AreEqual(2, svm.NumberOfClasses);
            var teacher = new ProbabilisticCoordinateDescent(svm, input, labels);

            teacher.Tolerance = 1e-10;
            teacher.Complexity = 1e+10;

            Assert.IsFalse(svm.IsProbabilistic);
            double error = teacher.Run();
            Assert.IsTrue(svm.IsProbabilistic);

            var regression = LogisticRegression.FromWeights(svm.ToWeights());

            double[] actual = new double[output.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = regression.Compute(input[i]);

            double ageOdds = regression.GetOddsRatio(1); // 1.0208597028836701
            double smokeOdds = regression.GetOddsRatio(2); // 5.8584748789881331

            Assert.AreEqual(0.2, error);
            Assert.AreEqual(1.0208597028836701, ageOdds, 1e-4);
            Assert.AreEqual(5.8584748789881331, smokeOdds, 1e-4);

            Assert.IsFalse(Double.IsNaN(ageOdds));
            Assert.IsFalse(Double.IsNaN(smokeOdds));

            Assert.AreEqual(-2.4577464307294092, regression.Intercept, 1e-8);
            Assert.AreEqual(-2.4577464307294092, regression.Coefficients[0], 1e-8);
            Assert.AreEqual(0.020645118265359252, regression.Coefficients[1], 1e-8);
            Assert.AreEqual(1.7678893101571855, regression.Coefficients[2], 1e-8);
        }

        [Test]
        public void RunTest2()
        {
            var dataset = SequentialMinimalOptimizationTest.GetYingYang();

            double[][] inputs = dataset.Submatrix(null, 0, 1).ToJagged();
            int[] labels = dataset.GetColumn(2).ToInt32();

            var svm = new SupportVectorMachine(inputs: 2);
            var teacher = new ProbabilisticCoordinateDescent(svm, inputs, labels);

            teacher.Tolerance = 1e-10;
            teacher.Complexity = 1e+10;

            double error = teacher.Run();

            double[] weights = svm.ToWeights();

            Assert.AreEqual(0.11, error);
            Assert.AreEqual(3, weights.Length);
            Assert.AreEqual(-1.3231203367770932, weights[0], 1e-8);
            Assert.AreEqual(-3.0227742288788493, weights[1], 1e-8);
            Assert.AreEqual(-0.73074823290553259, weights[2], 1e-8);

            Assert.AreEqual(svm.Threshold, weights[0]);
        }

        [Test]
        public void KernelTest1()
        {
            var dataset = SequentialMinimalOptimizationTest.GetYingYang();
            double[][] inputs = dataset.Submatrix(null, 0, 1).ToJagged();
            int[] labels = dataset.GetColumn(2).ToInt32();

            double e1, e2;
            double[] w1, w2;

            {
                Accord.Math.Random.Generator.Seed = 0;

                var svm = new SupportVectorMachine(inputs: 2);
                var teacher = new ProbabilisticCoordinateDescent(svm, inputs, labels);

                teacher.Tolerance = 1e-10;
                teacher.Complexity = 1e+10;

                e1 = teacher.Run();
                w1 = svm.ToWeights();
            }

            {
                Accord.Math.Random.Generator.Seed = 0;

                var svm = new KernelSupportVectorMachine(new Linear(0), inputs: 2);
                var teacher = new ProbabilisticCoordinateDescent(svm, inputs, labels);

                teacher.Tolerance = 1e-10;
                teacher.Complexity = 1e+10;

                e2 = teacher.Run();
                w2 = svm.ToWeights();
            }

            Assert.AreEqual(e1, e2);
            Assert.AreEqual(w1.Length, w2.Length);
            Assert.AreEqual(w1[0], w2[0], 1e-8);
            Assert.AreEqual(w1[1], w2[1], 1e-8);
            Assert.AreEqual(w1[2], w2[2], 1e-8);
        }

        [Test]
        public void KernelTest2()
        {
            var dataset = SequentialMinimalOptimizationTest.GetYingYang();
            var inputs = dataset.Submatrix(null, 0, 1).ToJagged();
            var labels = dataset.GetColumn(2).ToInt32();

            var svm = new KernelSupportVectorMachine(new Linear(1), inputs: 2);

            var p = new ProbabilisticCoordinateDescent(svm, inputs, labels);

            Assert.NotNull(p);
        }


        [Test]
        public void logistic_regression_test()
        {
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

            // Create the L1-regularization learning algorithm
            var teacher = new ProbabilisticCoordinateDescent()
            {
                Tolerance = 1e-10,
                Complexity = 1e+10, // learn a hard-margin model
            };

            // Learn the L1-regularized machine
            var svm = teacher.Learn(input, output);

            // Convert the svm to logistic regression
            var regression = (LogisticRegression)svm;

            // Compute the predicted outcome for inputs
            bool[] predicted = regression.Decide(input);

            // Compute log-likelihood scores for the outputs
            double[] scores = regression.Score(input);

            // Compute odds-ratio as in the LogisticRegression example
            double ageOdds = regression.GetOddsRatio(1);   // 1.0208597029158772
            double smokeOdds = regression.GetOddsRatio(2); // 5.8584748789881331

            // Compute the classification error as in SVM example
            double error = new ZeroOneLoss(output).Loss(predicted);
            #endregion

            Assert.AreEqual(2, regression.NumberOfInputs);
            Assert.AreEqual(1, regression.NumberOfOutputs); 
            Assert.AreEqual(2, regression.NumberOfClasses); 

            var rsvm = (SupportVectorMachine)regression;
            Assert.AreEqual(2, rsvm.NumberOfInputs);
            Assert.AreEqual(1, rsvm.NumberOfOutputs);
            Assert.AreEqual(2, rsvm.NumberOfClasses);

            double[] svmpred = svm.Score(input);
            Assert.IsTrue(scores.IsEqual(svmpred, 1e-10));

            Assert.AreEqual(0.2, error);
            Assert.AreEqual(1.0208597029158772, ageOdds, 1e-4);
            Assert.AreEqual(5.8584748789881331, smokeOdds, 1e-4);

            Assert.AreEqual(-2.4577464307294092, regression.Intercept, 1e-8);
            Assert.AreEqual(-2.4577464307294092, regression.Coefficients[0], 1e-8);
            Assert.AreEqual(0.020645118265359252, regression.Coefficients[1], 1e-8);
            Assert.AreEqual(1.7678893101571855, regression.Coefficients[2], 1e-8);
        }

        [Test]
        public void logistic_regression_sparse_test()
        {
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

            // Create the L1-regularization learning algorithm
            var teacher = new ProbabilisticCoordinateDescent<Linear, Sparse<double>>()
            {
                Tolerance = 1e-10,
                Complexity = 1e+10, // learn a hard-margin model
            };

            // Learn the L1-regularized machine
            var svm = teacher.Learn(input, output);

            // Convert the svm to logistic regression
            var regression = (LogisticRegression)svm;

            // Compute the predicted outcome for inputs
            bool[] predicted = regression.Decide(input.ToDense(regression.NumberOfInputs));

            // Compute log-likelihood scores for the outputs
            double[] scores = regression.Score(input.ToDense(regression.NumberOfInputs));

            // Compute odds-ratio as in the LogisticRegression example
            double ageOdds = regression.GetOddsRatio(1);   // 1.0208597029158772
            double smokeOdds = regression.GetOddsRatio(2); // 5.8584748789881331

            // Compute the classification error as in SVM example
            double error = new ZeroOneLoss(output).Loss(predicted);
            #endregion

            var rsvm = (SupportVectorMachine)regression;
            Assert.AreEqual(2, rsvm.NumberOfInputs);
            Assert.AreEqual(1, rsvm.NumberOfOutputs); 
            Assert.AreEqual(2, rsvm.NumberOfClasses); 
            double[] svmpred = svm.Score(input);
            Assert.IsTrue(scores.IsEqual(svmpred, 1e-10));

            Assert.AreEqual(0.2, error);
            Assert.AreEqual(1.0208597029158772, ageOdds, 1e-4);
            Assert.AreEqual(5.8584748789881331, smokeOdds, 1e-4);

            Assert.AreEqual(-2.4577464307294092, regression.Intercept, 1e-8);
            Assert.AreEqual(-2.4577464307294092, regression.Coefficients[0], 1e-8);
            Assert.AreEqual(0.020645118265359252, regression.Coefficients[1], 1e-8);
            Assert.AreEqual(1.7678893101571855, regression.Coefficients[2], 1e-8);
        }

    }
}
