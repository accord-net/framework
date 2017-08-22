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
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.MachineLearning.VectorMachines;
    using Math.Optimization.Losses;
    using Accord.Statistics.Kernels;

    [TestFixture]
    public class ProbabilisticNewtonMethodTest
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
            var teacher = new ProbabilisticNewtonMethod(svm, input, labels);

            teacher.Tolerance = 1e-10;
            teacher.Complexity = 1e+10;

            Assert.IsFalse(svm.IsProbabilistic);
            double error = teacher.Run();
            Assert.IsTrue(svm.IsProbabilistic);
            Assert.AreEqual(0.2, error);

            Assert.AreEqual(0.02064511826338301, svm.SupportVectors[0][0], 1e-10);
            Assert.AreEqual(1.767889310996118, svm.SupportVectors[0][1], 1e-10);
            Assert.AreEqual(-2.4577464317497455, svm.Threshold, 1e-10);

            var regression = LogisticRegression.FromWeights(svm.ToWeights());

            double[] actual = new double[output.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = regression.Compute(input[i]);

            double ageOdds = regression.GetOddsRatio(1); // 1.0208597028836701
            double smokeOdds = regression.GetOddsRatio(2); // 5.8584748789881331
            

            Assert.AreEqual(1.0208597028836701, ageOdds, 1e-4);
            Assert.AreEqual(5.8584748789881331, smokeOdds, 1e-4);

            Assert.AreEqual(-2.4577464307294092, regression.Intercept, 1e-8);
            Assert.AreEqual(-2.4577464307294092, regression.Coefficients[0], 1e-8);
            Assert.AreEqual(0.020645118265359252, regression.Coefficients[1], 1e-8);
            Assert.AreEqual(1.7678893101571855, regression.Coefficients[2], 1e-8);
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

            // Create the probabilistic-SVM learning algorithm
            var teacher = new ProbabilisticNewtonMethod()
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
            double[] scores = regression.Score(input);

            // Compute odds-ratio as in the LogisticRegression example
            double ageOdds = regression.GetOddsRatio(1);   // 1.0208597028836701
            double smokeOdds = regression.GetOddsRatio(2); // 5.8584748789881331

            // Compute the classification error as in SVM example
            double error = new ZeroOneLoss(output).Loss(predicted);
            #endregion

            Assert.AreEqual(0.2, error);
            Assert.AreEqual(1.0208597028836701, ageOdds, 1e-4);
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

            // Create the probabilistic-SVM learning algorithm
            var teacher = new ProbabilisticNewtonMethod<Linear, Sparse<double>>()
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
            double[] scores = regression.Score(input.ToDense(regression.NumberOfInputs));

            // Compute odds-ratio as in the LogisticRegression example
            double ageOdds = regression.GetOddsRatio(1);   // 1.0208597028836701
            double smokeOdds = regression.GetOddsRatio(2); // 5.8584748789881331

            // Compute the classification error as in SVM example
            double error = new ZeroOneLoss(output).Loss(predicted);
            #endregion

            Assert.AreEqual(0.2, error);
            Assert.AreEqual(1.0208597028836701, ageOdds, 1e-4);
            Assert.AreEqual(5.8584748789881331, smokeOdds, 1e-4);

            Assert.AreEqual(-2.4577464307294092, regression.Intercept, 1e-8);
            Assert.AreEqual(-2.4577464307294092, regression.Coefficients[0], 1e-8);
            Assert.AreEqual(0.020645118265359252, regression.Coefficients[1], 1e-8);
            Assert.AreEqual(1.7678893101571855, regression.Coefficients[2], 1e-8);
        }

    }
}
