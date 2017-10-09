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
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.MachineLearning;
    using Accord.Math;
    using NUnit.Framework;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using System.Linq;
    using System.Collections.Generic;
    using Accord.DataSets;
    using Accord.Statistics.Filters;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Models.Fields.Learning;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Fields;

    [TestFixture]
    public class POSTest
    {
        [Test]
        public void chunking_dataset_markov()
        {
            Chunking chunking = new Chunking(path: Path.Combine(TestContext.CurrentContext.WorkDirectory, "tmp"));

            // Learn a mapping between each word to an integer class label:
            var wordMap = new Codification().Learn(chunking.Words);

            // Learn a mapping between each tag to an integer class labels:
            var tagMap = new Codification().Learn(chunking.Tags);

            // Convert the training and testing sets into integer labels:
            int[][] trainX = wordMap.Transform(chunking.Training.Item1);
            int[][] testX = wordMap.Transform(chunking.Testing.Item1);

            // Convert the training and testing tags into integer labels:
            int[][] trainY = tagMap.Transform(chunking.Training.Item2);
            int[][] testY = tagMap.Transform(chunking.Testing.Item2);


            // Learn one Markov model using the training data
            var teacher = new MaximumLikelihoodLearning()
            {
                UseLaplaceRule = true,
                UseWeights = true
            };

            // Use the teacher to learn a Markov model
            var markov = teacher.Learn(trainX, trainY);

            // Use the model to predict instances:
            int[][] predY = markov.Decide(testX);

            // Check the accuracy of the model:
            var cm = new GeneralConfusionMatrix(
                predicted: predY.Concatenate(),
                expected: testY.Concatenate());

            double acc = cm.Accuracy;
#if NET35
            Assert.AreEqual(0.51725520822339954d, acc, 1e-10);
#else
            Assert.AreEqual(0.43987588914452158, acc, 1e-10);
#endif
        }


        [Test, Ignore("Computer intensive")]
        public void chunking_dataset_crf()
        {
            Chunking chunking = new Chunking(path: Path.GetTempPath());

            // Learn a mapping between each word to an integer class label:
            var wordMap = new Codification().Learn(chunking.Words);

            // Learn a mapping between each tag to an integer class labels:
            var tagMap = new Codification().Learn(chunking.Tags);

            // Convert the training and testing sets into integer labels:
            int[][] trainX = wordMap.Transform(chunking.Training.Item1);
            int[][] testX = wordMap.Transform(chunking.Testing.Item1);

            // Convert the training and testing tags into integer labels:
            int[][] trainY = tagMap.Transform(chunking.Training.Item2);
            int[][] testY = tagMap.Transform(chunking.Testing.Item2);


            int numberOfClasses = chunking.Tags.Length;
            int numberOfSymbols = chunking.Words.Length;

            // Learn one Markov model using the training data
            var teacher = new QuasiNewtonLearning<int>()
            {
                Function = new MarkovDiscreteFunction(states: numberOfClasses, symbols: numberOfSymbols)
            };

            // Use the teacher to learn a Conditional Random Field model
            ConditionalRandomField<int> crf = teacher.Learn(trainX, trainY);

            // Use the crf to predict instances:
            int[][] predY = crf.Decide(testX);

            // Check the accuracy of the model:
            var cm = new ConfusionMatrix(
                predicted: predY.Concatenate(),
                expected: testY.Concatenate());

            double acc = cm.Accuracy;

            Assert.AreEqual(0.99983114169322662, acc, 1e-10);
        }
    }
}
