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
    using Accord.MachineLearning.Boosting;
    using Accord.MachineLearning.Boosting.Learners;
    using Accord.Math;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using NUnit.Framework;
    using Accord.Statistics.Analysis;
    using Accord.Statistics;
    using Accord.DataSets;
    using Accord.MachineLearning.DecisionTrees;
    using Accord.MachineLearning.DecisionTrees.Learning;

    [TestFixture]
    public class AdaBoostTest
    {

        [Test]
        public void learn_stump_classifier()
        {
            #region doc_learn
            // Let's say we want to classify the following 2-dimensional 
            // data samples into 2 possible classes, either true or false:
            double[][] inputs =
            {
                new double[] {  10, 42 },
                new double[] { 162, 96 },
                new double[] { 125, 20 },
                new double[] {  96,  6 },
                new double[] {   2, 73 },
                new double[] {  52, 51 },
                new double[] {  71, 49 },
            };

            // And those are their associated class labels
            bool[] outputs =
            {
                false, false, true, true, false, false, true
            };

            // We can create an AdaBoost algorithm as:
            var learner = new AdaBoost<DecisionStump>()
            {
                Learner = (p) => new ThresholdLearning(),

                // Train until:
                MaxIterations = 5,
                Tolerance = 1e-3
            };

            // Now, we can use the Learn method to learn a boosted classifier
            Boost<DecisionStump> classifier = learner.Learn(inputs, outputs);

            // And we can test its performance using (error should be 0):
            ConfusionMatrix cm = ConfusionMatrix.Estimate(classifier, inputs, outputs);

            double error = cm.Error;  // should be 0.0
            double acc = cm.Accuracy; // should be 1.0
            double kappa = cm.Kappa;  // should be 1.0

            // And compute a decision for a single data point using:
            bool y = classifier.Decide(inputs[0]); // result should false
            #endregion

            Assert.AreEqual(false, y);
            Assert.AreEqual(0, error);
            Assert.AreEqual(1, acc);
            Assert.AreEqual(1, kappa);

            Assert.AreEqual(5, classifier.Models.Count);
            Assert.AreEqual(0.16684734250395147, classifier.Models[0].Weight);
            Assert.AreEqual(0.22329026900109736, classifier.Models[1].Weight);
            Assert.AreEqual(0.28350372170582383, classifier.Models[2].Weight);
            Assert.AreEqual(0.16684734250395139, classifier.Models[3].Weight);
            Assert.AreEqual(0.15951132428517592, classifier.Models[4].Weight);

            int[] actual = new int[outputs.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = classifier.Compute(inputs[i]);

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(outputs[i] ? 1 : -1, actual[i]);
        }

        [Test]
        public void learn_logistic_regression()
        {
            #region doc_learn_lr
            // This example shows how to use AdaBoost to train more complex
            // models than a simple DecisionStump. For example, we will use
            // it to train a boosted Logistic Regression classifier.

            // Let's use some synthetic data for that: The Yin-Yang dataset is 
            // a simple 2D binary non-linear decision problem where the points 
            // belong to each of the classes interwine in a Yin-Yang shape:
            var dataset = new YinYang();
            double[][] inputs = dataset.Instances;
            int[] outputs = Classes.ToZeroOne(dataset.ClassLabels);

            // Create an AdaBoost for Logistic Regression as:
            var teacher = new AdaBoost<LogisticRegression>()
            {
                // Here we can specify how each regression should be learned:
                Learner = (param) => new IterativeReweightedLeastSquares<LogisticRegression>()
                {
                    ComputeStandardErrors = false,
                    MaxIterations = 50,
                    Tolerance = 0
                },

                // Train until:
                MaxIterations = 50,
                Tolerance = 1e-5,
            };

            // Now, we can use the Learn method to learn a boosted classifier
            Boost<LogisticRegression> classifier = teacher.Learn(inputs, outputs);

            // And we can test its performance using (error should be 0.11):
            ConfusionMatrix cm = ConfusionMatrix.Estimate(classifier, inputs, outputs);

            double error = cm.Error;  // should be 0.11
            double acc = cm.Accuracy; // should be 0.89
            double kappa = cm.Kappa;  // should be 0.78

            // And compute a decision for a single data point using:
            bool y = classifier.Decide(inputs[0]); // result should false
            #endregion

            Assert.AreEqual(false, y);
            Assert.AreEqual(0.11, error);
            Assert.AreEqual(0.89, acc);
            Assert.AreEqual(0.78, kappa);

            Assert.AreEqual(2, classifier.Models.Count);
            Assert.AreEqual(0.63576818449825168, classifier.Models[0].Weight);
            Assert.AreEqual(0.36423181550174832, classifier.Models[1].Weight);

            int[] actual = new int[outputs.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = classifier.Compute(inputs[i]);
        }

        [Test]
        public void learn_decision_trees()
        {
            #region doc_learn_dt
            // This example shows how to use AdaBoost to train more complex
            // models than a simple DecisionStump. For example, we will use
            // it to train a boosted Decision Trees.

            // Let's use some synthetic data for that: The Yin-Yang dataset is 
            // a simple 2D binary non-linear decision problem where the points 
            // belong to each of the classes interwine in a Yin-Yang shape:
            var dataset = new YinYang();
            double[][] inputs = dataset.Instances;
            int[] outputs = Classes.ToZeroOne(dataset.ClassLabels);

            // Create an AdaBoost for Logistic Regression as:
            var teacher = new AdaBoost<DecisionTree>()
            {
                // Here we can specify how each regression should be learned:
                Learner = (param) => new C45Learning()
                {
                    // i.e.
                    // MaxHeight = 
                    // MaxVariables = 
                },

                // Train until:
                MaxIterations = 50,
                Tolerance = 1e-5,
            };

            // Now, we can use the Learn method to learn a boosted classifier
            Boost<DecisionTree> classifier = teacher.Learn(inputs, outputs);

            // And we can test its performance using (error should be 0.11):
            double error = ConfusionMatrix.Estimate(classifier, inputs, outputs).Error;

            // And compute a decision for a single data point using:
            bool y = classifier.Decide(inputs[0]); // result should false
            #endregion

            Assert.AreEqual(false, y);
            Assert.AreEqual(0, error);

            Assert.AreEqual(22, classifier.Models.Count);
            Assert.AreEqual(0.063497989403001331, classifier.Models[0].Weight);
            Assert.AreEqual(0.081129615464770655, classifier.Models[1].Weight);
            Assert.AreEqual(0.083062765085567689, classifier.Models[2].Weight);
            Assert.AreEqual(0.050307480220333232, classifier.Models[3].Weight);
            Assert.AreEqual(0.044287142080877882, classifier.Models[4].Weight);
            Assert.AreEqual(0.042772219812778081, classifier.Models[5].Weight);

            int[] actual = new int[outputs.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = classifier.Compute(inputs[i]);
        }

        [Test]
        public void ConstructorTest()
        {
            // Let's say we want to classify the following 2-dimensional 
            // data samples into 2 possible classes, either true or false:
            double[][] inputs =
            {
                new double[] { 10, 42 },
                new double[] { 162, 96 },
                new double[] { 125, 20 },
                new double[] { 96, 6 },
                new double[] { 2, 73 },
                new double[] { 52, 51 },
                new double[] { 71, 49 },
            };

            // And those are their associated class labels
            int[] outputs =
            {
                -1, -1, +1, +1, -1, -1, +1
            };


            // First, we create a classsifier using:
            var classifier = new Boost<DecisionStump>();

            // Now, we can create a AdaBoost learning algorithm as:
            var teacher = new AdaBoost<DecisionStump>(classifier)
            {
                Creation = (weights) =>
                {
                    var stump = new DecisionStump(2);
                    stump.Learn(inputs, outputs, weights);
                    return stump;
                },

                // Train until:
                MaxIterations = 5,
                Tolerance = 1e-3
            };

            // Now, we can use the Run method to learn:
            double error = teacher.Run(inputs, outputs); // error should be zero.

            // Now, we can compute the model outputs for new samples using
            int y = classifier.Compute(new double[] { 71, 48 }); // should be 1
            Assert.AreEqual(1, y);

            Assert.AreEqual(0, error);

            Assert.AreEqual(5, classifier.Models.Count);
            Assert.AreEqual(0.16684734250395147, classifier.Models[0].Weight);
            Assert.AreEqual(0.22329026900109736, classifier.Models[1].Weight);
            Assert.AreEqual(0.28350372170582383, classifier.Models[2].Weight);
            Assert.AreEqual(0.16684734250395139, classifier.Models[3].Weight);
            Assert.AreEqual(0.15951132428517592, classifier.Models[4].Weight);

            int[] actual = new int[outputs.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = classifier.Compute(inputs[i]);

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(outputs[i], actual[i]);
        }

        [Test]
        public void ConstructorTest2()
        {
            var dataset = new YinYang();
            double[][] inputs = dataset.Instances;
            bool[] outputs2 = dataset.ClassLabels;

            int[] outputs = outputs2.Apply(x => x ? 1 : 0);

            var classifier = new Boost<Weak<LogisticRegression>>();

            var teacher = new AdaBoost<Weak<LogisticRegression>>(classifier)
            {
                Creation = (weights) =>
                {
                    LogisticRegression reg = new LogisticRegression(2, intercept: 1);

                    IterativeReweightedLeastSquares irls = new IterativeReweightedLeastSquares(reg)
                    {
                        ComputeStandardErrors = false
                    };

                    for (int i = 0; i < 50; i++)
                        irls.Run(inputs, outputs, weights);

                    return new Weak<LogisticRegression>(reg, (s, x) => Math.Sign(s.Compute(x) - 0.5));
                },

                Iterations = 50,
                Tolerance = 1e-5,
            };



            double error = teacher.Run(inputs, outputs);


            Assert.AreEqual(0.11, error);

            Assert.AreEqual(2, classifier.Models.Count);
            Assert.AreEqual(0.63576818449825168, classifier.Models[0].Weight);
            Assert.AreEqual(0.36423181550174832, classifier.Models[1].Weight);

            int[] actual = new int[outputs.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = classifier.Compute(inputs[i]);

            //for (int i = 0; i < actual.Length; i++)
            //    Assert.AreEqual(outputs[i], actual[i]);
        }


    }
}