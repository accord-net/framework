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
    public class DecisionStumpTest
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

            // We create a learning algorithm as:
            var teacher = new ThresholdLearning();

            // Now, we can use the Learn method to learn a classifier:
            DecisionStump classifier = teacher.Learn(inputs, outputs);

            // Now, we can check how good it is using a confusion matrix:
            var cm = ConfusionMatrix.Estimate(classifier, inputs, outputs);

            double error = cm.Error; // should be ~0.14

            // We can also compute the model outputs for new samples using
            bool y = classifier.Decide(new double[] { 71, 48 }); // should be false
            #endregion

            Assert.AreEqual(false, y);
            Assert.AreEqual(0.14285714285714285, error, 1e-10);
        }

    }
}