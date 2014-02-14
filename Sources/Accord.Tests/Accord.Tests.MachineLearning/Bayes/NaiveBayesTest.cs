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

namespace Accord.Tests.MachineLearning
{
    using System.Data;
    using System.Text;
    using Accord;
    using Accord.MachineLearning;
    using Accord.MachineLearning.Bayes;
    using Accord.Math;
    using Accord.Statistics.Filters;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class NaiveBayesTest
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
        public void NaiveBayesConstructorTest()
        {
            int classes = 0;
            int[] symbols = new int[0];
            bool thrown = false;

            try { new NaiveBayes(classes, symbols); }
            catch { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        public void NaiveBayesConstructorTest2()
        {
            int classes = 2;
            int[] symbols = null;
            bool thrown = false;

            try { new NaiveBayes(classes, symbols); }
            catch { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        public void NaiveBayesConstructorTest3()
        {
            int classes = 2;
            int[] symbols = new int[2];
            bool thrown = false;

            try { new NaiveBayes(classes, null, symbols); }
            catch { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        public void NaiveBayesConstructorTest4()
        {
            int classes = 2;
            int[] symbols = { 2, 3, 1 };
            double[] priors = { 0.4, 0.6 };
            NaiveBayes target = new NaiveBayes(classes, priors, symbols);

            Assert.AreEqual(2, target.ClassCount);
            Assert.AreEqual(3, target.InputCount);
            Assert.AreEqual(2, target.Priors.Length);
            Assert.AreEqual(0.4, target.Priors[0]);
            Assert.AreEqual(0.6, target.Priors[1]);
        }


        [TestMethod()]
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
            Codification codebook = new Codification(data);

            int[] symbolCounts =
            {
                codebook["Outlook"].Symbols,     // 3 possible values (Sunny, overcast, rain)
                codebook["Temperature"].Symbols, // 3 possible values (Hot, mild, cool)
                codebook["Humidity"].Symbols,    // 2 possible values (High, normal)
                codebook["Wind"].Symbols         // 2 possible values (Weak, strong)
            };

            int classCount = codebook["PlayTennis"].Symbols; // 2 possible values (yes, no)


            // Create a new Naive Bayes classifiers for the two classes
            NaiveBayes target = new NaiveBayes(classCount, symbolCounts);

            // Extract symbols from data and train the classifier
            DataTable symbols = codebook.Apply(data);
            int[][] inputs = symbols.ToArray<int>("Outlook", "Temperature", "Humidity", "Wind");
            int[] outputs = symbols.ToArray<int>("PlayTennis");

            // Compute the Naive Bayes model
            target.Estimate(inputs, outputs);


            double logLikelihood;
            double[] responses;

            // Compute the result for a sunny, cool, humid and windy day:
            int[] instance = codebook.Translate("Sunny", "Cool", "High", "Strong");

            int c = target.Compute(instance, out logLikelihood, out responses);

            string result = codebook.Translate("PlayTennis", c);

            Assert.AreEqual("No", result);
            Assert.AreEqual(0, c);
            Assert.AreEqual(0.795, responses[0], 1e-3);
            Assert.AreEqual(1, responses.Sum(), 1e-10);
            Assert.IsFalse(double.IsNaN(responses[0]));
            Assert.AreEqual(2, responses.Length);
        }

        [TestMethod()]
        public void ComputeTest2()
        {

            // Some sample texts
            string[] spamTokens = Tokenize(@"I decided to sign up for the Disney Half Marathon. Half of a marathon is 13.1 miles. A full marathon is 26.2 miles. You may wonder why the strange number of miles. “26.2” is certainly not an even number. And after running 26 miles who cares about the point two? You might think that 26.2 miles is a whole number of kilometers. It isn’t. In fact, it is even worse in kilometers – 42.1648128. I bet you don’t see many t-shirts in England with that number printed on the front.");

            string[] loremTokens = Tokenize(@"Lorem ipsum dolor sit amet,  Nulla nec tortor. Donec id elit quis purus consectetur consequat. Nam congue semper tellus. Sed erat dolor, dapibus sit amet, venenatis ornare, ultrices ut, nisi. Aliquam ante. Suspendisse scelerisque dui nec velit. Duis augue augue, gravida euismod, vulputate ac, facilisis id, sem. Morbi in orci. Nulla purus lacus, pulvinar vel, malesuada ac, mattis nec, quam. Nam molestie scelerisque quam. Nullam feugiat cursus lacus.orem ipsum dolor sit amet.");

            // Their respective classes
            string[] classes = { "spam", "lorem" };


            // Create a new Bag-of-Words for the texts
            BagOfWords bow = new BagOfWords(spamTokens, loremTokens)
            {
                // Limit the maximum number of occurrences in 
                // the feature vector to a single instance
                MaximumOccurance = 1
            };

            // Define the symbols for the Naïve Bayes
            int[] symbols = new int[bow.NumberOfWords];
            for (int i = 0; i < symbols.Length; i++)
                symbols[i] = bow.MaximumOccurance + 1;

            // Create input and outputs for training
            int[][] inputs =
            {
                bow.GetFeatureVector(spamTokens),
                bow.GetFeatureVector(loremTokens)
            };

            int[] outputs =
            {
                0, // spam
                1, // lorem
            };

            // Create the naïve Bayes model
            NaiveBayes bayes = new NaiveBayes(2, symbols);

            for (int i = 0; i < bayes.ClassCount; i++)
                for (int j = 0; j < bayes.SymbolCount.Length; j++)
                    for (int k = 0; k < bayes.SymbolCount[j]; k++)
                        bayes.Distributions[i, j][k] = 1e-10;

            // Estimate the model
            bayes.Estimate(inputs, outputs);


            // Initialize with prior probabilities
            for (int i = 0; i < bayes.ClassCount; i++)
                for (int j = 0; j < bayes.SymbolCount.Length; j++)
                {
                    double sum = bayes.Distributions[i, j].Sum();
                    Assert.AreEqual(1, sum, 1e-5);
                }

            // Consume the model
            {
                // First an example to classify as lorem
                int[] input = bow.GetFeatureVector(loremTokens);
                int answer = bayes.Compute(input);
                string result = classes[answer];

                Assert.AreEqual("lorem", result);
            }

            {
                // Then an example to classify as spam
                int[] input = bow.GetFeatureVector(spamTokens);
                int answer = bayes.Compute(input);
                string result = classes[answer];

                Assert.AreEqual("spam", result);
            }

        }

        public static string[] Tokenize(string text)
        {
            StringBuilder sb = new StringBuilder(text);

            char[] invalid = "!-;':'\",.?\n\r\t".ToCharArray();

            for (int i = 0; i < invalid.Length; i++)
                sb.Replace(invalid[i], ' ');

            return sb.ToString().Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        }

        [TestMethod()]
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

    }
}
