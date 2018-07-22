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
    using Accord.MachineLearning;
    using NUnit.Framework;
    using System;
    using Accord.Math;
    using Accord.Math.Distances;
    using Accord.Statistics.Analysis;
    using Accord.IO;
    using System.IO;

    [TestFixture]
    public class KNearestNeighborTest
    {

        [Test]
        public void KNearestNeighborConstructorTest()
        {
            double[][] inputs =
            {
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },

                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },

                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            int[] outputs =
            {
                0, 0,
                1, 1, 1, 1,
                2, 2, 2
            };

            int k = 3;

            KNearestNeighbors target = new KNearestNeighbors(k, inputs, outputs);

            for (int i = 0; i < inputs.Length; i++)
            {
                int actual = target.Compute(inputs[i]);
                int expected = outputs[i];

                Assert.AreEqual(expected, actual);
            }

            double[][] test =
            {
                new double[] { -4, -3, -1 },
                new double[] { -5, -4, -4 },

                new double[] {  5,  3,  4 },
                new double[] {  3,  1,  6 },

                new double[] { 10,  5,  4 },
                new double[] { 13,  4,  5 },
            };

            int[] expectedOutputs =
            {
                0, 0,
                1, 1,
                2, 2,
            };

            for (int i = 0; i < test.Length; i++)
            {
                int actual = target.Compute(test[i]);
                int expected = expectedOutputs[i];

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void KNearestNeighborConstructorTest2()
        {
            // Create some sample learning data. In this data,
            // the first two instances belong to a class, the
            // four next belong to another class and the last
            // three to yet another.

            double[][] inputs =
            {
                // The first two are from class 0
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },

                // The next four are from class 1
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },

                // The last three are from class 2
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            int[] outputs =
            {
                0, 0,        // First two from class 0
                1, 1, 1, 1,  // Next four from class 1
                2, 2, 2      // Last three from class 2
            };


            // Now we will create the K-Nearest Neighbors algorithm. For this
            // example, we will be choosing k = 4. This means that, for a given
            // instance, its nearest 4 neighbors will be used to cast a decision.
            KNearestNeighbors knn = new KNearestNeighbors(k: 4, classes: 3,
                inputs: inputs, outputs: outputs);


            // After the algorithm has been created, we can classify a new instance:
            int answer = knn.Compute(new double[] { 11, 5, 4 }); // answer will be 2.

            Assert.AreEqual(2, answer);
        }

        [Test]
        public void learn_test1()
        {
            string basePath = NUnit.Framework.TestContext.CurrentContext.TestDirectory;

            #region doc_learn
            // Create some sample learning data. In this data,
            // the first two instances belong to a class, the
            // four next belong to another class and the last
            // three to yet another.

            double[][] inputs =
            {
                // The first two are from class 0
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },

                // The next four are from class 1
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },

                // The last three are from class 2
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            int[] outputs =
            {
                0, 0,        // First two from class 0
                1, 1, 1, 1,  // Next four from class 1
                2, 2, 2      // Last three from class 2
            };


            // Now we will create the K-Nearest Neighbors algorithm. For this
            // example, we will be choosing k = 4. This means that, for a given
            // instance, its nearest 4 neighbors will be used to cast a decision.
            var knn = new KNearestNeighbors(k: 4);

            // We learn the algorithm:
            knn.Learn(inputs, outputs);

            // After the algorithm has been created, we can classify a new instance:
            int answer = knn.Decide(new double[] { 11, 5, 4 }); // answer will be 2.

            // Let's say we would like to compute the error matrix for the classifier:
            var cm = GeneralConfusionMatrix.Estimate(knn, inputs, outputs);

            // We can use it to estimate measures such as 
            double error = cm.Error;  // should be 
            double acc = cm.Accuracy; // should be 
            double kappa = cm.Kappa;  // should be 
            #endregion

            Assert.AreEqual(2, answer);
            Assert.AreEqual(0, error);
            Assert.AreEqual(1, acc);
            Assert.AreEqual(1, kappa);

#if !NO_BINARY_SERIALIZATION
            #region doc_serialization
            // After we have created and learned our model, let's say we would 
            // like to save it to disk. For this, we can import the Accord.IO 
            // namespace at the top of our source file namespace, and then use 
            // Serializer's extension method Save:

            // Save to a file called "knn.bin" in the basePath directory:
            knn.Save(Path.Combine(basePath, "knn.bin"));

            // To load it back from the disk, we might need to use the Serializer class directly:
            var loaded_knn = Serializer.Load<KNearestNeighbors>(Path.Combine(basePath, "knn.bin"));

            // At this point, knn and loaded_knn should be 
            // two different instances of identical objects.
            #endregion

            // Make sure the loaded classifier is still working
            Assert.AreEqual(2, loaded_knn.Decide(new double[] { 11, 5, 4 }));
            cm = GeneralConfusionMatrix.Estimate(loaded_knn, inputs, outputs);
            Assert.AreEqual(0, cm.Error);
            Assert.AreEqual(1, cm.Accuracy);
            Assert.AreEqual(1, cm.Kappa);

            Assert.AreEqual(knn.ClassCount, loaded_knn.ClassCount);
            Assert.AreEqual(knn.Distance, loaded_knn.Distance);
            Assert.AreEqual(knn.K, loaded_knn.K);
            Assert.AreEqual(knn.NumberOfClasses, loaded_knn.NumberOfClasses);
            Assert.AreEqual(knn.NumberOfInputs, loaded_knn.NumberOfInputs);
            Assert.AreEqual(knn.NumberOfOutputs, loaded_knn.NumberOfOutputs);
            Assert.AreEqual(knn.Outputs, loaded_knn.Outputs);
            Assert.AreEqual(knn.Token, loaded_knn.Token);
#endif
        }

        [Test]
        public void KNearestNeighborGenericConstructorTest()
        {
            double[][] inputs =
            {
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },

                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },

                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            int[] outputs =
            {
                0, 0,
                1, 1, 1, 1,
                2, 2, 2
            };

            int k = 3;

            var target = new KNearestNeighbors<double[]>(k, inputs, outputs, new SquareEuclidean());

            for (int i = 0; i < inputs.Length; i++)
            {
                int actual = target.Compute(inputs[i]);
                int expected = outputs[i];

                Assert.AreEqual(expected, actual);
            }

            double[][] test =
            {
                new double[] { -4, -3, -1 },
                new double[] { -5, -4, -4 },

                new double[] {  5,  3,  4 },
                new double[] {  3,  1,  6 },

                new double[] { 10,  5,  4 },
                new double[] { 13,  4,  5 },
            };

            int[] expectedOutputs =
            {
                0, 0,
                1, 1,
                2, 2,
            };

            for (int i = 0; i < test.Length; i++)
            {
                int actual = target.Compute(test[i]);
                int expected = expectedOutputs[i];

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void KNearestNeighborGenericConstructorTest2()
        {
            // Create some sample learning data. In this data,
            // the first two instances belong to a class, the
            // four next belong to another class and the last
            // three to yet another.

            double[][] inputs =
            {
                // The first two are from class 0
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },

                // The next four are from class 1
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },

                // The last three are from class 2
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            int[] outputs =
            {
                0, 0,        // First two from class 0
                1, 1, 1, 1,  // Next four from class 1
                2, 2, 2      // Last three from class 2
            };


            // Now we will create the K-Nearest Neighbors algorithm. For this
            // example, we will be choosing k = 4. This means that, for a given
            // instance, its nearest 4 neighbors will be used to cast a decision.
            var knn = new KNearestNeighbors<double[]>(k: 4, classes: 3,
                inputs: inputs, outputs: outputs, distance: new SquareEuclidean());


            // After the algorithm has been created, we can classify a new instance:
            int answer = knn.Compute(new double[] { 11, 5, 4 }); // answer will be 2.


            Assert.AreEqual(2, answer);
        }

        [Test]
        public void learn_test()
        {
            #region doc_learn_distance
            // Create some sample learning data. In this data,
            // the first two instances belong to a class, the
            // four next belong to another class and the last
            // three to yet another.

            double[][] inputs =
            {
                // The first two are from class 0
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },

                // The next four are from class 1
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },

                // The last three are from class 2
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            int[] outputs =
            {
                0, 0,        // First two from class 0
                1, 1, 1, 1,  // Next four from class 1
                2, 2, 2      // Last three from class 2
            };


            // Now we will create the K-Nearest Neighbors algorithm. For this
            // example, we will be choosing k = 4. This means that, for a given
            // instance, its nearest 4 neighbors will be used to cast a decision.
            var knn = new KNearestNeighbors<double[]>(k: 4, distance: new SquareEuclidean());

            // We learn the algorithm:
            knn.Learn(inputs, outputs);

            // After the algorithm has been created, we can classify a new instance:
            int answer = knn.Decide(new double[] { 11, 5, 4 }); // answer will be 2.

            // Let's say we would like to compute the error matrix for the classifier:
            var cm = GeneralConfusionMatrix.Estimate(knn, inputs, outputs);

            // We can use it to estimate measures such as 
            double error = cm.Error;  // should be 0
            double acc = cm.Accuracy; // should be 1
            double kappa = cm.Kappa;  // should be 1
            #endregion

            Assert.AreEqual(2, answer);
            Assert.AreEqual(0, error);
            Assert.AreEqual(1, acc);
            Assert.AreEqual(1, kappa);
        }

        [Test]
        public void KNearestNeighborConstructorTest3()
        {
            // The k-Nearest Neighbors algorithm can be used with
            // any kind of data. In this example, we will see how
            // it can be used to compare, for example, Strings.

            string[] inputs =
            {
                "Car",     // class 0
                "Bar",     // class 0
                "Jar",     // class 0

                "Charm",   // class 1
                "Chair"    // class 1
            };

            int[] outputs =
            {
                0, 0, 0,  // First three are from class 0
                1, 1,     // And next two are from class 1
            };


            // Now we will create the K-Nearest Neighbors algorithm. For this
            // example, we will be choosing k = 1. This means that, for a given
            // instance, only its nearest neighbor will be used to cast a new
            // decision. 

            // In order to compare strings, we will be using Levenshtein's string distance
            KNearestNeighbors<string> knn = new KNearestNeighbors<string>(k: 1, classes: 2,
                inputs: inputs, outputs: outputs, distance: new Levenshtein());

            // After the algorithm has been created, we can use it:
            int answer = knn.Compute("Chars"); // answer should be 1.

            Assert.AreEqual(1, answer);
        }

        [Test]
        public void learn_string()
        {
            string basePath = NUnit.Framework.TestContext.CurrentContext.TestDirectory;

            #region doc_learn_text
            // The k-Nearest Neighbors algorithm can be used with
            // any kind of data. In this example, we will see how
            // it can be used to compare, for example, Strings.

            string[] inputs =
            {
                "Car",     // class 0
                "Bar",     // class 0
                "Jar",     // class 0

                "Charm",   // class 1
                "Chair"    // class 1
            };

            int[] outputs =
            {
                0, 0, 0,  // First three are from class 0
                1, 1,     // And next two are from class 1
            };


            // Now we will create the K-Nearest Neighbors algorithm. For this
            // example, we will be choosing k = 1. This means that, for a given
            // instance, only its nearest neighbor will be used to cast a new
            // decision. 

            // In order to compare strings, we will be using Levenshtein's string distance
            var knn = new KNearestNeighbors<string>(k: 1, distance: new Levenshtein());

            // We learn the algorithm:
            knn.Learn(inputs, outputs);

            // After the algorithm has been created, we can use it:
            int answer = knn.Decide("Chars"); // answer should be 1.

            // Let's say we would like to compute the error matrix for the classifier:
            var cm = ConfusionMatrix.Estimate(knn, inputs, outputs);

            // We can use it to estimate measures such as 
            double error = cm.Error;  // should be 0
            double acc = cm.Accuracy; // should be 1
            double kappa = cm.Kappa;  // should be 1
            #endregion

            Assert.AreEqual(1, answer);
            Assert.AreEqual(0, error);
            Assert.AreEqual(1, acc);
            Assert.AreEqual(1, kappa);

#if !NO_BINARY_SERIALIZATION
            knn.Save(Path.Combine(basePath, "string_knn.bin"));

            var loaded_knn = Serializer.Load<KNearestNeighbors<string>>(Path.Combine(basePath, "string_knn.bin"));

            Assert.AreEqual(1, loaded_knn.Decide("Chars"));
            cm = ConfusionMatrix.Estimate(loaded_knn, inputs, outputs);
            Assert.AreEqual(0, cm.Error);
            Assert.AreEqual(1, cm.Accuracy);
            Assert.AreEqual(1, cm.Kappa);

            Assert.AreEqual(knn.ClassCount, loaded_knn.ClassCount);
            Assert.AreEqual(knn.Distance, loaded_knn.Distance);
            Assert.AreEqual(knn.K, loaded_knn.K);
            Assert.AreEqual(knn.NumberOfClasses, loaded_knn.NumberOfClasses);
            Assert.AreEqual(knn.NumberOfInputs, loaded_knn.NumberOfInputs);
            Assert.AreEqual(knn.NumberOfOutputs, loaded_knn.NumberOfOutputs);
            Assert.AreEqual(knn.Outputs, loaded_knn.Outputs);
            Assert.AreEqual(knn.Token, loaded_knn.Token);
#endif
        }

        [Test]
        public void weights_test_1()
        {
            KNearestNeighbors<string> a;
            KNearestNeighbors<string> b;

            {
                string[] inputs = { "Car", "Bar", "Bar", "Bar", "Jar", "Charm", "Charm", "Chair" };
                int[] outputs = { 0, 0, 0, 1, 1, 2, 2, 2 };
                double[] weights = { 1, 1, 0, 0, 1, 1, 0, 1 };
                var knn = new KNearestNeighbors<string>(k: inputs.Length, distance: new Levenshtein());
                a = knn.Learn(inputs, outputs, weights);
            }

            {
                string[] inputs = { "Car", "Bar", "Jar", "Charm", "Chair" };
                int[] outputs = { 0, 0, 1, 2, 2 };
                var knn = new KNearestNeighbors<string>(k: inputs.Length, distance: new Levenshtein());
                b = knn.Learn(inputs, outputs);
            }

            string[] x = new[] { "Car", "Bar", "Jar", "Charm", "Chair" };
            Assert.AreEqual(a.Scores(x), b.Scores(x));
        }

        [Test]
        public void weights_test_2()
        {
            KNearestNeighbors<string> a;
            KNearestNeighbors<string> b;

            {
                string[] inputs = { "Car", "Bar", "Bar", "Bar", "Jar", "Charm", "Charm", "Chair" };
                int[] outputs = { 0, 0, 0, 1, 1, 2, 2, 2 };
                var knn = new KNearestNeighbors<string>(k: inputs.Length, distance: new Levenshtein());
                a = knn.Learn(inputs, outputs);
            }

            {
                string[] inputs = { "Car", "Bar", "Bar", "Jar", "Charm", "Chair" };
                int[] outputs = { 0, 0, 1, 1, 2, 2 };
                double[] weights = { 1, 2, 1, 1, 2, 1 };
                var knn = new KNearestNeighbors<string>(k: inputs.Length, distance: new Levenshtein());
                b = knn.Learn(inputs, outputs, weights);
            }

            {
                string x = "Bar";
                double[] expected = a.Scores(x);
                double[] actual = b.Scores(x);
                Assert.AreEqual(expected, actual);
            }
            {
                string[] x = new[] { "Car", "Bar", "Jar", "Charm", "Chair" };
                double[][] expected = a.Scores(x);
                double[][] actual = b.Scores(x);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void weights_test_tree_1()
        {
            KNearestNeighbors a;
            KNearestNeighbors b;

            {
                double[][] inputs = Jagged.ColumnVector(4.2, 0.7, 0.7, 0.7, 1.3, 9.4, 9.4, 12);
                int[] outputs = { 0, 0, 0, 1, 1, 2, 2, 2 };
                double[] weights = { 1, 1, 0, 0, 1, 1, 0, 1 };
                var knn = new KNearestNeighbors(k: inputs.Length);
                a = knn.Learn(inputs, outputs, weights);
            }

            {
                double[][] inputs = Jagged.ColumnVector(4.2, 0.7, 1.3, 9.4, 12);
                int[] outputs = { 0, 0, 1, 2, 2 };
                var knn = new KNearestNeighbors(k: inputs.Length);
                b = knn.Learn(inputs, outputs);
            }

            double[][] x = Jagged.ColumnVector(4.2, 0.7, 1.3, 9.4, 12);
            Assert.AreEqual(a.Scores(x), b.Scores(x));
        }

        [Test]
        public void weights_test_tree_2()
        {
            KNearestNeighbors a;
            KNearestNeighbors b;

            {
                double[][] inputs = Jagged.ColumnVector(4.2, 0.7, 0.7, 0.7, 1.3, 9.4, 9.4, 12);
                int[] outputs = { 0, 0, 0, 1, 1, 2, 2, 2 };
                var knn = new KNearestNeighbors(k: inputs.Length);
                a = knn.Learn(inputs, outputs);
            }

            {
                double[][] inputs = Jagged.ColumnVector(4.2, 0.7, 0.7, 1.3, 9.4, 12);
                int[] outputs = { 0, 0, 1, 1, 2, 2 };
                double[] weights = { 1, 2, 1, 1, 2, 1 };
                var knn = new KNearestNeighbors(k: inputs.Length);
                b = knn.Learn(inputs, outputs, weights);
            }

            {
                double[] x = { 9.4 };
                double[] expected = a.Scores(x);
                double[] actual = b.Scores(x);
                Assert.IsTrue(expected.IsEqual(actual, 1e-4));
            }
            {
                double[][] x = Jagged.ColumnVector(4.2, 0.7, 1.3, 9.4, 12);
                double[][] expected = a.Scores(x);
                double[][] actual = b.Scores(x);
                Assert.IsTrue(expected.IsEqual(actual, 1e-4));
            }
        }

        [Test]
        public void KNearestNeighbor_CrossValidation()
        {
            // Create some sample learning data. In this data,
            // the first two instances belong to a class, the
            // four next belong to another class and the last
            // three to yet another.

            double[][] inputs =
            {
                // The first two are from class 0
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },

                // The next four are from class 1
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },

                // The last three are from class 2
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            int[] outputs =
            {
                0, 0,        // First two from class 0
                1, 1, 1, 1,  // Next four from class 1
                2, 2, 2      // Last three from class 2
            };



            // Create a new Cross-validation algorithm passing the data set size and the number of folds
            var crossvalidation = new CrossValidation(size: inputs.Length, folds: 3);

            // Define a fitting function using Support Vector Machines. The objective of this
            // function is to learn a SVM in the subset of the data indicated by cross-validation.

            crossvalidation.Fitting = delegate (int k, int[] indicesTrain, int[] indicesValidation)
            {
                // The fitting function is passing the indices of the original set which
                // should be considered training data and the indices of the original set
                // which should be considered validation data.

                // Lets now grab the training data:
                var trainingInputs = inputs.Submatrix(indicesTrain);
                var trainingOutputs = outputs.Submatrix(indicesTrain);

                // And now the validation data:
                var validationInputs = inputs.Submatrix(indicesValidation);
                var validationOutputs = outputs.Submatrix(indicesValidation);

                // Now we will create the K-Nearest Neighbors algorithm. For this
                // example, we will be choosing k = 4. This means that, for a given
                // instance, its nearest 4 neighbors will be used to cast a decision.
                KNearestNeighbors knn = new KNearestNeighbors(k: 4, classes: 3,
            inputs: inputs, outputs: outputs);


                // After the algorithm has been created, we can classify instances:
                int[] train_predicted = trainingInputs.Apply(knn.Compute);
                int[] test_predicted = validationInputs.Apply(knn.Compute);

                // Compute classification error
                var cmTrain = new GeneralConfusionMatrix(3, train_predicted, trainingOutputs);
                double trainingAcc = cmTrain.Accuracy;

                // Now we can compute the validation error on the validation data:
                var cmTest = new GeneralConfusionMatrix(3, test_predicted, validationOutputs);
                double validationAcc = cmTest.Accuracy;

                // Return a new information structure containing the model and the errors achieved.
                return new CrossValidationValues(knn, trainingAcc, validationAcc);
            };


            // Compute the cross-validation
            var result = crossvalidation.Compute();

            // Finally, access the measured performance.
            double trainingAccs = result.Training.Mean;
            double validationAccs = result.Validation.Mean;


            Assert.AreEqual(1, trainingAccs);
            Assert.AreEqual(1, validationAccs);
        }
    }
}
