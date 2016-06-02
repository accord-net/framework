﻿// Accord Unit Tests
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
    using Accord;
    using Accord.IO;
    using Accord.MachineLearning.Bayes;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Filters;
    using Accord.Tests.MachineLearning.Properties;
    using NUnit.Framework;
    using System.Data;
    using System.IO;


    [TestFixture]
    public class NaiveBayesGenericTest
    {

        [Test]
        public void NaiveBayesConstructorTest()
        {
            int classes = 2;
            int inputs = 0;
            bool thrown = false;

            try { new NaiveBayes<UniformDiscreteDistribution>(classes, inputs, new UniformDiscreteDistribution(0, 3)); }
            catch { thrown = true; }

            Assert.IsTrue(thrown);
        }


        [Test]
        public void NaiveBayesConstructorTest4()
        {
            int classes = 2;
            int inputCount = 3;
            double[] priors = { 0.4, 0.6 };
            UniformDiscreteDistribution initial = new UniformDiscreteDistribution(0, 3);

            var target = new NaiveBayes<UniformDiscreteDistribution>(classes, inputCount, initial, priors);

            Assert.AreEqual(classes, target.ClassCount);
            Assert.AreEqual(inputCount, target.InputCount);
            Assert.AreEqual(priors.Length, target.Priors.Length);
            Assert.AreEqual(0.4, target.Priors[0]);
            Assert.AreEqual(0.6, target.Priors[1]);

            Assert.AreEqual(2, target.Distributions.GetLength(0));
            Assert.AreEqual(3, target.Distributions.GetLength(1));

            for (int i = 0; i < classes; i++)
            {
                for (int j = 0; j < inputCount; j++)
                {
                    Assert.AreNotSame(initial, target.Distributions[i, j]);
                    Assert.AreEqual(0, target.Distributions[i, j].Minimum);
                    Assert.AreEqual(3, target.Distributions[i, j].Maximum);
                }
            }
        }

        [Test]
        public void NaiveBayesConstructorTest5()
        {
            const int classes = 2;
            const int inputCount = 3;
            double[] classPriors = { 0.4, 0.6 };
            var inputPriors = new[,]
            {
                {new UniformDiscreteDistribution(0,10), new UniformDiscreteDistribution(0,10), new UniformDiscreteDistribution(0,10)},
                {new UniformDiscreteDistribution(0,10), new UniformDiscreteDistribution(0,10), new UniformDiscreteDistribution(0,10)}
            };

            var target = new NaiveBayes<UniformDiscreteDistribution>(classes, inputCount, inputPriors, classPriors);

            Assert.AreEqual(classes, target.ClassCount);
            Assert.AreEqual(inputCount, target.InputCount);
            Assert.AreEqual(classPriors.Length, target.Priors.Length);
            Assert.AreEqual(0.4, target.Priors[0]);
            Assert.AreEqual(0.6, target.Priors[1]);

            Assert.AreEqual(2, target.Distributions.GetLength(0));
            Assert.AreEqual(3, target.Distributions.GetLength(1));
        }

        [Test]
        public void ComputeTest()
        {
            DataTable data = new DataTable("Mitchell's Tennis Example");

            data.Columns.Add("Day", "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");

            data.Rows.Add("D1", "Sunny", "Hot", "High", "Weak", "No");
            data.Rows.Add("D2", "Sunny", "Hot", "High", "Strong", "No");
            data.Rows.Add("D3", "Overcast", "Hot", "High", "Weak", "Yes");
            data.Rows.Add("D4", "Rain", "Mild", "High", "Weak", "Yes");
            data.Rows.Add("D5", "Rain", "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D6", "Rain", "Cool", "Normal", "Strong", "No");
            data.Rows.Add("D7", "Overcast", "Cool", "Normal", "Strong", "Yes");
            data.Rows.Add("D8", "Sunny", "Mild", "High", "Weak", "No");
            data.Rows.Add("D9", "Sunny", "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D10", "Rain", "Mild", "Normal", "Weak", "Yes");
            data.Rows.Add("D11", "Sunny", "Mild", "Normal", "Strong", "Yes");
            data.Rows.Add("D12", "Overcast", "Mild", "High", "Strong", "Yes");
            data.Rows.Add("D13", "Overcast", "Hot", "Normal", "Weak", "Yes");
            data.Rows.Add("D14", "Rain", "Mild", "High", "Strong", "No");

            // Create a new codification codebook to
            // convert strings into discrete symbols
            Codification codebook = new Codification(data,
                "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");

            int classCount = codebook["PlayTennis"].Symbols; // 2 possible values (yes, no)
            int inputCount = 4; // 4 variables (Outlook, Temperature, Humidity, Wind)

            GeneralDiscreteDistribution[] priors =
            {
                new GeneralDiscreteDistribution(codebook["Outlook"].Symbols),     // 3 possible values (Sunny, overcast, rain)
                new GeneralDiscreteDistribution(codebook["Temperature"].Symbols), // 3 possible values (Hot, mild, cool)
                new GeneralDiscreteDistribution(codebook["Humidity"].Symbols),    // 2 possible values (High, normal)
                new GeneralDiscreteDistribution(codebook["Wind"].Symbols)         // 2 possible values (Weak, strong)
            };

            // Create a new Naive Bayes classifiers for the two classes
            var target = new NaiveBayes<GeneralDiscreteDistribution>(classCount, inputCount, priors);

            // Extract symbols from data and train the classifier
            DataTable symbols = codebook.Apply(data);
            double[][] inputs = symbols.ToArray("Outlook", "Temperature", "Humidity", "Wind");
            int[] outputs = symbols.ToArray<int>("PlayTennis");

            // Compute the Naive Bayes model
            target.Estimate(inputs, outputs);


            double logLikelihood;
            double[] responses;

            // Compute the result for a sunny, cool, humid and windy day:
            double[] instance = codebook.Translate("Sunny", "Cool", "High", "Strong").ToDouble();

            int c = target.Compute(instance, out logLikelihood, out responses);

            string result = codebook.Translate("PlayTennis", c);

            Assert.AreEqual("No", result);
            Assert.AreEqual(0, c);
            Assert.AreEqual(0.795, responses[0], 1e-3);
            Assert.AreEqual(1, responses.Sum(), 1e-10);
            Assert.IsFalse(double.IsNaN(responses[0]));
            Assert.AreEqual(2, responses.Length);
        }

        [Test]
        public void ComputeTest2()
        {
            DataTable data = new DataTable("Mitchell's Tennis Example");

            data.Columns.Add("Day", "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");

            // We will set Temperature and Humidity to be continuous
            data.Columns["Temperature"].DataType = typeof(double);
            data.Columns["Humidity"].DataType = typeof(double);

            data.Rows.Add("D1", "Sunny", 38.0, 96.0, "Weak", "No");
            data.Rows.Add("D2", "Sunny", 39.0, 90.0, "Strong", "No");
            data.Rows.Add("D3", "Overcast", 38.0, 75.0, "Weak", "Yes");
            data.Rows.Add("D4", "Rain", 25.0, 87.0, "Weak", "Yes");
            data.Rows.Add("D5", "Rain", 12.0, 30.0, "Weak", "Yes");
            data.Rows.Add("D6", "Rain", 11.0, 35.0, "Strong", "No");
            data.Rows.Add("D7", "Overcast", 10.0, 40.0, "Strong", "Yes");
            data.Rows.Add("D8", "Sunny", 24.0, 90.0, "Weak", "No");
            data.Rows.Add("D9", "Sunny", 12.0, 26.0, "Weak", "Yes");
            data.Rows.Add("D10", "Rain", 25, 30.0, "Weak", "Yes");
            data.Rows.Add("D11", "Sunny", 26.0, 40.0, "Strong", "Yes");
            data.Rows.Add("D12", "Overcast", 27.0, 97.0, "Strong", "Yes");
            data.Rows.Add("D13", "Overcast", 39.0, 41.0, "Weak", "Yes");
            data.Rows.Add("D14", "Rain", 23.0, 98.0, "Strong", "No");

            // Create a new codification codebook to
            // convert strings into discrete symbols
            Codification codebook = new Codification(data);

            int classCount = codebook["PlayTennis"].Symbols; // 2 possible values (yes, no)
            int inputCount = 4; // 4 variables (Outlook, Temperature, Humidity, Wind)

            IUnivariateFittableDistribution[] priors =
            {
                new GeneralDiscreteDistribution(codebook["Outlook"].Symbols),   // 3 possible values (Sunny, overcast, rain)
                new NormalDistribution(),                                       // Continuous value (Celsius)
                new NormalDistribution(),                                       // Continuous value (percentage)
                new GeneralDiscreteDistribution(codebook["Wind"].Symbols)       // 2 possible values (Weak, strong)
            };

            // Create a new Naive Bayes classifiers for the two classes
            var target = new NaiveBayes<IUnivariateFittableDistribution>(classCount, inputCount, priors);

            // Extract symbols from data and train the classifier
            DataTable symbols = codebook.Apply(data);
            double[][] inputs = symbols.ToArray("Outlook", "Temperature", "Humidity", "Wind");
            int[] outputs = symbols.ToArray<int>("PlayTennis");

            // Compute the Naive Bayes model
            target.Estimate(inputs, outputs);


            double logLikelihood;
            double[] responses;

            // Compute the result for a sunny, cool, humid and windy day:
            double[] instance = new double[] 
            {
                codebook.Translate(columnName:"Outlook", value:"Sunny"), 
                12.0, 
                90.0,
                codebook.Translate(columnName:"Wind", value:"Strong")
            };

            int c = target.Compute(instance, out logLikelihood, out responses);

            string result = codebook.Translate("PlayTennis", c);

            Assert.AreEqual("No", result);
            Assert.AreEqual(0, c);
            Assert.AreEqual(0.840, responses[0], 1e-3);
            Assert.AreEqual(1, responses.Sum(), 1e-10);
            Assert.IsFalse(double.IsNaN(responses[0]));
            Assert.AreEqual(2, responses.Length);

            int c2 = target.Compute(instance, out logLikelihood);

            Assert.AreEqual(c, c2);
        }


        [Test]
        public void ComputeTest3()
        {
            // Let's say we have the following data to be classified
            // into three possible classes. Those are the samples:
            //
            double[][] inputs =
            {
                //               input         output
                new double[] { 0, 1, 1, 0 }, //  0 
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 0, 0, 1, 0 }, //  0
                new double[] { 0, 1, 1, 0 }, //  0
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 1, 0, 0, 0 }, //  1
                new double[] { 1, 0, 0, 0 }, //  1
                new double[] { 1, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 1, 1, 1, 1 }, //  2
                new double[] { 1, 0, 1, 1 }, //  2
                new double[] { 1, 1, 0, 1 }, //  2
                new double[] { 0, 1, 1, 1 }, //  2
                new double[] { 1, 1, 1, 1 }, //  2
            };

            int[] outputs = // those are the class labels
            {
                0, 0, 0, 0, 0,
                1, 1, 1, 1, 1,
                2, 2, 2, 2, 2,
            };

            // Create a new continuous naive Bayes model for 3 classes using 4-dimensional Gaussian distributions
            var bayes = new NaiveBayes<NormalDistribution>(inputs: 4, classes: 3, initial: NormalDistribution.Standard);

            // Teach the Naive Bayes model. The error should be zero:
            double error = bayes.Estimate(inputs, outputs, options: new NormalOptions
            {
                Regularization = 1e-5 // to avoid zero variances
            });

            // Now, let's test  the model output for the first input sample:
            int answer = bayes.Compute(new double[] { 0, 1, 1, 0 }); // should be 1


            Assert.AreEqual(0, error);
            for (int i = 0; i < inputs.Length; i++)
            {
                double actual = bayes.Compute(inputs[i]);
                double expected = outputs[i];
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void DistributionsTest()
        {
            int classes = 3;
            int[] symbols = { 2, 1 };
            NaiveBayes target = new NaiveBayes(classes, symbols);
            double[,][] actual = target.Distributions;

            Assert.IsNotNull(actual);
            Assert.AreEqual(classes, actual.GetLength(0));
            Assert.AreEqual(symbols.Length, actual.GetLength(1));
        }


        [Test]
        public void SerializationTest()
        {
            DataTable data = new DataTable("Mitchell's Tennis Example");

            data.Columns.Add("Day", "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");

            data.Rows.Add("D1", "Sunny", "Hot", "High", "Weak", "No");
            data.Rows.Add("D2", "Sunny", "Hot", "High", "Strong", "No");
            data.Rows.Add("D3", "Overcast", "Hot", "High", "Weak", "Yes");
            data.Rows.Add("D4", "Rain", "Mild", "High", "Weak", "Yes");
            data.Rows.Add("D5", "Rain", "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D6", "Rain", "Cool", "Normal", "Strong", "No");
            data.Rows.Add("D7", "Overcast", "Cool", "Normal", "Strong", "Yes");
            data.Rows.Add("D8", "Sunny", "Mild", "High", "Weak", "No");
            data.Rows.Add("D9", "Sunny", "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D10", "Rain", "Mild", "Normal", "Weak", "Yes");
            data.Rows.Add("D11", "Sunny", "Mild", "Normal", "Strong", "Yes");
            data.Rows.Add("D12", "Overcast", "Mild", "High", "Strong", "Yes");
            data.Rows.Add("D13", "Overcast", "Hot", "Normal", "Weak", "Yes");
            data.Rows.Add("D14", "Rain", "Mild", "High", "Strong", "No");

            // Create a new codification codebook to
            // convert strings into discrete symbols
            Codification codebook = new Codification(data,
                "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");

            var target = Serializer.Load<NaiveBayes<GeneralDiscreteDistribution>>(new MemoryStream(Resources.nb));

            Assert.AreEqual(target.InputCount, 4);
            Assert.AreEqual(target.ClassCount, 2);
            double logLikelihood;
            double[] responses;

            // Compute the result for a sunny, cool, humid and windy day:
            double[] instance = codebook.Translate("Sunny", "Cool", "High", "Strong").ToDouble();

            int c = target.Compute(instance, out logLikelihood, out responses);

            string result = codebook.Translate("PlayTennis", c);

            Assert.AreEqual("No", result);
            Assert.AreEqual(0, c);
            Assert.AreEqual(0.795, responses[0], 1e-3);
            Assert.AreEqual(1, responses.Sum(), 1e-10);
            Assert.IsFalse(double.IsNaN(responses[0]));
            Assert.AreEqual(2, responses.Length);
        }


    }
}
