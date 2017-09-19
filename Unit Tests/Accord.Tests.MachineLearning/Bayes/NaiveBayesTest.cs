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

#if !MONO

namespace Accord.Tests.MachineLearning
{
    using Accord;
    using Accord.IO;
    using Accord.MachineLearning;
    using Accord.MachineLearning.Bayes;
    using Accord.Math;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Filters;
    using NUnit.Framework;
    using System;
    using System.Data;
    using System.IO;
    using System.Text;

    [TestFixture]
    public class NaiveBayesTest
    {


        [Test]
        public void NaiveBayesConstructorTest()
        {
            int classes = 0;
            int[] symbols = new int[0];
            bool thrown = false;

            try { new NaiveBayes(classes, symbols); }
            catch { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [Test]
        public void NaiveBayesConstructorTest2()
        {
            int classes = 2;
            int[] symbols = null;
            bool thrown = false;

            try { new NaiveBayes(classes, symbols); }
            catch { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [Test]
        public void NaiveBayesConstructorTest3()
        {
            int classes = 2;
            int[] symbols = new int[2];
            bool thrown = false;

            try { new NaiveBayes(classes, null, symbols); }
            catch { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [Test]
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

#if !NO_DATA_TABLE
        [Test]
        public void ComputeTest_Obsolete()
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

        [Test]
        public void ComputeTest()
        {
            #region doc_mitchell
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
            #endregion

            #region doc_codebook
            // Create a new codification codebook to
            // convert strings into discrete symbols
            Codification codebook = new Codification(data,
                "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");

            // Extract input and output pairs to train
            DataTable symbols = codebook.Apply(data);
            int[][] inputs = symbols.ToArray<int>("Outlook", "Temperature", "Humidity", "Wind");
            int[] outputs = symbols.ToArray<int>("PlayTennis");
            #endregion

            #region doc_learn
            // Create a new Naive Bayes learning
            var learner = new NaiveBayesLearning();

            // Learn a Naive Bayes model from the examples
            NaiveBayes nb = learner.Learn(inputs, outputs);
            #endregion


            #region doc_test
            // Consider we would like to know whether one should play tennis at a
            // sunny, cool, humid and windy day. Let us first encode this instance
            int[] instance = codebook.Translate("Sunny", "Cool", "High", "Strong");

            // Let us obtain the numeric output that represents the answer
            int c = nb.Decide(instance); // answer will be 0

            // Now let us convert the numeric output to an actual "Yes" or "No" answer
            string result = codebook.Translate("PlayTennis", c); // answer will be "No"

            // We can also extract the probabilities for each possible answer
            double[] probs = nb.Probabilities(instance); // { 0.795, 0.205 }
            #endregion

            Assert.AreEqual("No", result);
            Assert.AreEqual(0, c);
            Assert.AreEqual(0.795, probs[0], 1e-3);
            Assert.AreEqual(0.205, probs[1], 1e-3);
            Assert.AreEqual(1, probs.Sum(), 1e-10);
            Assert.IsFalse(double.IsNaN(probs[0]));
            Assert.AreEqual(2, probs.Length);
        }
#endif

        [Test]
        public void learn_no_datatable()
        {
            #region doc_mitchell_no_datatable
            string[] columnNames = { "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis" };

            string[][] data =
            {
                new string[] { "Sunny", "Hot", "High", "Weak", "No" },
                new string[] { "Sunny", "Hot", "High", "Strong", "No" },
                new string[] { "Overcast", "Hot", "High", "Weak", "Yes" },
                new string[] { "Rain", "Mild", "High", "Weak", "Yes" },
                new string[] { "Rain", "Cool", "Normal", "Weak", "Yes" },
                new string[] { "Rain", "Cool", "Normal", "Strong", "No" },
                new string[] { "Overcast", "Cool", "Normal", "Strong", "Yes" },
                new string[] { "Sunny", "Mild", "High", "Weak", "No" },
                new string[] { "Sunny", "Cool", "Normal", "Weak", "Yes" },
                new string[] {  "Rain", "Mild", "Normal", "Weak", "Yes" },
                new string[] {  "Sunny", "Mild", "Normal", "Strong", "Yes" },
                new string[] {  "Overcast", "Mild", "High", "Strong", "Yes" },
                new string[] {  "Overcast", "Hot", "Normal", "Weak", "Yes" },
                new string[] {  "Rain", "Mild", "High", "Strong", "No" },
            };

            // Create a new codification codebook to
            // convert strings into discrete symbols
            Codification codebook = new Codification(columnNames, data);

            // Extract input and output pairs to train
            int[][] symbols = codebook.Transform(data);
            int[][] inputs = symbols.Get(null, 0, -1); // Gets all rows, from 0 to the last (but not the last)
            int[] outputs = symbols.GetColumn(-1);     // Gets only the last column

            // Create a new Naive Bayes learning
            var learner = new NaiveBayesLearning();

            NaiveBayes nb = learner.Learn(inputs, outputs);

            // Consider we would like to know whether one should play tennis at a
            // sunny, cool, humid and windy day. Let us first encode this instance
            int[] instance = codebook.Translate("Sunny", "Cool", "High", "Strong");

            // Let us obtain the numeric output that represents the answer
            int c = nb.Decide(instance); // answer will be 0

            // Now let us convert the numeric output to an actual "Yes" or "No" answer
            string result = codebook.Translate("PlayTennis", c); // answer will be "No"

            // We can also extract the probabilities for each possible answer
            double[] probs = nb.Probabilities(instance); // { 0.795, 0.205 }
            #endregion

            Assert.AreEqual("No", result);
            Assert.AreEqual(0, c);
            Assert.AreEqual(0.795, probs[0], 1e-3);
            Assert.AreEqual(0.205, probs[1], 1e-3);
            Assert.AreEqual(1, probs.Sum(), 1e-10);
            Assert.IsFalse(double.IsNaN(probs[0]));
            Assert.AreEqual(2, probs.Length);
        }

        [Test]
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

        [Test]
        public void ComputeTest3_Obsolete()
        {
            // Let's say we have the following data to be classified
            // into three possible classes. Those are the samples:
            //
            int[][] inputs =
            {
                //               input      output
                new int[] { 0, 1, 1, 0 }, //  0 
                new int[] { 0, 1, 0, 0 }, //  0
                new int[] { 0, 0, 1, 0 }, //  0
                new int[] { 0, 1, 1, 0 }, //  0
                new int[] { 0, 1, 0, 0 }, //  0
                new int[] { 1, 0, 0, 0 }, //  1
                new int[] { 1, 0, 0, 0 }, //  1
                new int[] { 1, 0, 0, 1 }, //  1
                new int[] { 0, 0, 0, 1 }, //  1
                new int[] { 0, 0, 0, 1 }, //  1
                new int[] { 1, 1, 1, 1 }, //  2
                new int[] { 1, 0, 1, 1 }, //  2
                new int[] { 1, 1, 0, 1 }, //  2
                new int[] { 0, 1, 1, 1 }, //  2
                new int[] { 1, 1, 1, 1 }, //  2
            };

            int[] outputs = // those are the class labels
            {
                0, 0, 0, 0, 0,
                1, 1, 1, 1, 1,
                2, 2, 2, 2, 2,
            };

            // Create a discrete naive Bayes model for 3 classes and 4 binary inputs
            int[] symbols = new int[] { 2, 2, 2, 2 };
            var bayes = new NaiveBayes(3, symbols);

            // Teach the model. The error should be zero:
            double error = bayes.Estimate(inputs, outputs);

            // Now, let's test  the model output for the first input sample:
            int answer = bayes.Compute(new int[] { 0, 1, 1, 0 }); // should be 1


            Assert.AreEqual(0, error);
            for (int i = 0; i < inputs.Length; i++)
            {
                error = bayes.Compute(inputs[i]);
                double expected = outputs[i];
                Assert.AreEqual(expected, error);
            }
        }

        [Test]
        public void laplace_smoothing_missing_sample()
        {
            #region doc_laplace
            // To test the effectiveness of the Laplace rule for when
            // an example of a symbol is not present in the training set,
            // lets create dataset where the second column could contain
            // values 0, 1 or 2 but only actually contains examples with
            // containing 1 and 2:

            int[][] inputs =
            {
                //      input     output
                new [] { 0, 1 }, //  0 
                new [] { 0, 2 }, //  0
                new [] { 0, 1 }, //  0
                new [] { 1, 2 }, //  1
                new [] { 0, 2 }, //  1
                new [] { 0, 2 }, //  1
                new [] { 1, 1 }, //  2
                new [] { 0, 1 }, //  2
                new [] { 1, 1 }, //  2
            };

            int[] outputs = // those are the class labels
            {
                0, 0, 0, 1, 1, 1, 2, 2, 2,
            };

            // Since the data is not enough to determine which symbols we are
            // expecting in our model, we will have to specify the model by
            // hand. The first column can assume 2 different values, whereas
            // the third column can assume 3:
            var bayes = new NaiveBayes(classes: 3, symbols: new[] { 2, 3 });

            // Now we can create a learning algorithm
            var learning = new NaiveBayesLearning()
            {
                Model = bayes
            };

            // Enable the use of the Laplace rule
            learning.Options.InnerOption.UseLaplaceRule = true;

            // Learn the Naive Bayes model
            learning.Learn(inputs, outputs);

            // Estimate a sample with 0 in the second col
            int answer = bayes.Decide(new int[] { 0, 1 });
            #endregion

            Assert.AreEqual(0, answer);

            double prob = bayes.Probability(new int[] { 0, 1 }, out answer);
            Assert.AreEqual(0, answer);
            Assert.AreEqual(0.52173913043478259, prob, 1e-10);

            double error = new ZeroOneLoss(outputs)
            {
                Mean = true
            }.Loss(bayes.Decide(inputs));

            Assert.AreEqual(2 / 9.0, error);
        }

        [Test]
        public void ComputeTest3()
        {
            #region doc_multiclass
            // Let's say we have the following data to be classified
            // into three possible classes. Those are the samples:
            //
            int[][] inputs =
            {
                //               input      output
                new int[] { 0, 1, 1, 0 }, //  0 
                new int[] { 0, 1, 0, 0 }, //  0
                new int[] { 0, 0, 1, 0 }, //  0
                new int[] { 0, 1, 1, 0 }, //  0
                new int[] { 0, 1, 0, 0 }, //  0
                new int[] { 1, 0, 0, 0 }, //  1
                new int[] { 1, 0, 0, 0 }, //  1
                new int[] { 1, 0, 0, 1 }, //  1
                new int[] { 0, 0, 0, 1 }, //  1
                new int[] { 0, 0, 0, 1 }, //  1
                new int[] { 1, 1, 1, 1 }, //  2
                new int[] { 1, 0, 1, 1 }, //  2
                new int[] { 1, 1, 0, 1 }, //  2
                new int[] { 0, 1, 1, 1 }, //  2
                new int[] { 1, 1, 1, 1 }, //  2
            };

            int[] outputs = // those are the class labels
            {
                0, 0, 0, 0, 0,
                1, 1, 1, 1, 1,
                2, 2, 2, 2, 2,
            };

            // Let us create a learning algorithm
            var learner = new NaiveBayesLearning();

            // and teach a model on the data examples
            NaiveBayes nb = learner.Learn(inputs, outputs);

            // Now, let's test  the model output for the first input sample:
            int answer = nb.Decide(new int[] { 0, 1, 1, 0 }); // should be 1
            #endregion

            double error = new ZeroOneLoss(outputs).Loss(nb.Decide(inputs));
            Assert.AreEqual(0, error);

            for (int i = 0; i < inputs.Length; i++)
            {
                error = nb.Compute(inputs[i]);
                double expected = outputs[i];
                Assert.AreEqual(expected, error);
            }
        }


        [Test]
        public void DistributionsTest()
        {
            int classes = 3;
            int[] symbols = { 2, 1 };
            NaiveBayes target = new NaiveBayes(classes, symbols);
            var actual = target.Distributions;

            Assert.IsNotNull(actual);
            Assert.AreEqual(classes, actual.GetLength(0));
            Assert.AreEqual(symbols.Length, actual.GetLength(1));
        }

#if !NO_BINARY_SERIALIZATION
        [Test]
        public void SerializeTest()
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

            //target.Save(@"C:\Projects\Accord.NET\framework\nb2.bin");
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "nb2.bin");
            target = Serializer.Load<NaiveBayes>(fileName);

            Assert.AreEqual(2, target.NumberOfOutputs);
            Assert.AreEqual(3, target.NumberOfInputs);
            Assert.AreEqual(2, target.Priors.Length);
            Assert.AreEqual(0.4, target.Priors[0]);
            Assert.AreEqual(0.6, target.Priors[1]);
        }
#endif

        [Test]
        public void no_sample_test()
        {
            // Declare some boolean data
            bool[,] source =
            {
                // v1,v2,v3,v4,v5,v6,v7,v8,result
                { true,  true,  false, true,  true,  false, false, false, false },
                { true,  true,  true,  true,  true,  false, false, false, false },
                { true,  false, true,  true,  true,  false, false, true,  false },
                { true,  true,  true,  true,  true,  false, false, true,  false },
                { false, false, true,  true,  true,  false, false, true,  false },
                { true,  true,  true,  true,  false, false, false, false, false },
                { false, true,  true,  false, true,  false, false, false, false },
                { true,  true,  true,  false, true,  false, false, false, false },
                { false, true,  true,  false, true,  false, false, true,  false },
                { false, true,  true,  true,  true,  false, false, true,  false },
                { false, true,  true,  false, false, false, false, false, false },
                { true,  false, false, true,  false, false, false, true,  true  },
                { true,  true,  false, true,  false, false, false, true,  true  },
                { true,  true,  true,  true,  false, false, false, true,  true  },
                { false, true,  true,  true,  false, true,  true,  true,  true  },
                { true,  true,  false, false, false, true,  true,  true,  true  },
                { false, true,  false, false, false, true,  true,  true,  true  },
                { true,  true,  true,  true,  false, true,  true,  true,  true  },
                { false, false, false, false, false, true,  true,  true,  true  },
                { true,  true,  false, true,  false, true,  true,  true,  true  },
                { false, true,  false, true,  false, true,  true,  true,  true  },
                { false, true,  true,  false, false, true,  true,  true,  true  },
            };

            // Evaluation of a single point
            int[] sp = new[] { false, false, false, false, true, true, true, true }.ToInt32();


            // Transform to integers, then to jagged (matrix with [][] instead of [,])
            int[][] data = source.ToInt32().ToJagged();

            // Classification setup
            var inputs = data.Get(null, 0, 8); // select all rows, with cols 0 to 8
            var outputs = data.GetColumn(8);   // select last column

            var learner2 = new NaiveBayesLearning<GeneralDiscreteDistribution, GeneralDiscreteOptions, int>();
            learner2.Options.InnerOption.UseLaplaceRule = true;
            learner2.Distribution = (i, j) => new GeneralDiscreteDistribution(symbols: 2);
            learner2.ParallelOptions.MaxDegreeOfParallelism = 1;
            var nb2 = learner2.Learn(inputs, outputs);

            test(nb2, inputs, sp);


            var learner1 = new NaiveBayesLearning();
            learner1.Options.InnerOption.UseLaplaceRule = true;
            learner2.ParallelOptions.MaxDegreeOfParallelism = 1;
            var nb1 = learner1.Learn(inputs, outputs);

            test(nb1, inputs, sp);
        }

        private static void test(NaiveBayes<GeneralDiscreteDistribution, int> nb, int[][] inputs, int[] sp)
        {
            int c = nb.Decide(sp); // 1
            double[] p = nb.Probabilities(sp); // 0.015, 0.985

            // Evaluation of all points
            int[] actual = nb.Decide(inputs);

            Assert.AreEqual(1, c);
            Assert.AreEqual(0.015197568389057824, p[0], 1e-10);
            Assert.AreEqual(0.98480243161094227, p[1], 1e-10);

            Assert.AreEqual(nb.Distributions[0].Components[0].Frequencies[0], 0.46153846153846156);
            Assert.AreEqual(nb.Distributions[0].Components[1].Frequencies[0], 0.23076923076923078);
            Assert.AreEqual(nb.Distributions[0].Components[2].Frequencies[0], 0.15384615384615385);
            Assert.AreEqual(nb.Distributions[0].Components[3].Frequencies[0], 0.38461538461538464);
            Assert.AreEqual(nb.Distributions[0].Components[4].Frequencies[0], 0.23076923076923078);
            Assert.AreEqual(nb.Distributions[0].Components[5].Frequencies[0], 0.92307692307692313);
            Assert.AreEqual(nb.Distributions[0].Components[6].Frequencies[0], 0.92307692307692313);
            Assert.AreEqual(nb.Distributions[0].Components[7].Frequencies[0], 0.53846153846153844);

            Assert.AreEqual(nb.Distributions[1].Components[0].Frequencies[0], 0.46153846153846156);
            Assert.AreEqual(nb.Distributions[1].Components[1].Frequencies[0], 0.23076923076923078);
            Assert.AreEqual(nb.Distributions[1].Components[2].Frequencies[0], 0.61538461538461542);
            Assert.AreEqual(nb.Distributions[1].Components[3].Frequencies[0], 0.38461538461538464);
            Assert.AreEqual(nb.Distributions[1].Components[4].Frequencies[0], 0.92307692307692313);
            Assert.AreEqual(nb.Distributions[1].Components[5].Frequencies[0], 0.30769230769230771);
            Assert.AreEqual(nb.Distributions[1].Components[6].Frequencies[0], 0.30769230769230771);
            Assert.AreEqual(nb.Distributions[1].Components[7].Frequencies[0], 0.076923076923076927);

            int[] last = actual.Get(new[] { 11, 12 }.Concatenate(Vector.Range(14, 22)));
            int[] others = actual.Get(Vector.Range(0, 10).Concatenate(13));
            Assert.IsTrue(1.IsEqual(last));
            Assert.IsTrue(0.IsEqual(others));
        }

        [Test]
        public void issue_168()
        {
            // Text naive bayes classification gives wrong results #168
            // https://github.com/accord-net/framework/issues/168
            // Some sample texts
            string[] spamTokens = Tokenize(@"I decided to sign up for the Disney Half Marathon. Half of a marathon is 13.1 miles. A full marathon is 26.2 miles. You may wonder why the strange number of miles. “26.2” is certainly not an even number. And after running 26 miles who cares about the point two? You might think that 26.2 miles is a whole number of kilometers. It isn’t. In fact, it is even worse in kilometers – 42.1648128. I bet you don’t see many t-shirts in England with that number printed on the front.");
            string[] loremTokens = Tokenize(@"Lorem ipsum dolor sit amet,  Nulla nec tortor. Donec id elit quis purus consectetur consequat. Nam congue semper tellus. Sed erat dolor, dapibus sit amet, venenatis ornare, ultrices ut, nisi. Aliquam ante. Suspendisse scelerisque dui nec velit. Duis augue augue, gravida euismod, vulputate ac, facilisis id, sem. Morbi in orci. Nulla purus lacus, pulvinar vel, malesuada ac, mattis nec, quam. Nam molestie scelerisque quam. Nullam feugiat cursus lacus.orem ipsum dolor sit amet.");

            // Their respective classes
            string[] classes = { "spam", "lorem" };


            // Create a new Bag-of-Words for the texts
            BagOfWords bow = new BagOfWords()
            {
                // Limit the maximum number of occurences in 
                // the feature vector to a single instance
                MaximumOccurance = 1
            };

            bow.Learn(new[] { spamTokens, loremTokens });

            string word = bow.CodeToString[52];
            Assert.AreEqual("in", word);

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

            // Create the naïve bayes model
            var teacher = new NaiveBayesLearning()
            {
                Empirical = true,
                Options = new IndependentOptions<GeneralDiscreteOptions>()
                {
                    InnerOption = new GeneralDiscreteOptions()
                    {
                        //UseLaplaceRule = true
                    }
                }
            };

            // The following line is only needed to ensure reproducible results. Please remove it to enable full parallelization
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1; // (Remove, comment, or change this line to enable full parallelism)


            // Estimate the model
            var nb = teacher.Learn(inputs, outputs);


            double[][] spamDist = nb.Distributions.GetRow(0);
            double[][] loremDist = nb.Distributions.GetRow(1);

            for (int i = 0; i < spamDist.Length; i++)
            {
                if (i == 52)
                {
                    Assert.AreEqual(spamDist[i][0], 0.0, 1e-8);
                    Assert.AreEqual(spamDist[i][1], 1.0, 1e-8);
                    Assert.AreEqual(loremDist[i][0], 0.0, 1e-8);
                    Assert.AreEqual(loremDist[i][1], 1.0, 1e-8);
                }
                else
                {
                    if (i < 68)
                    {
                        Assert.AreEqual(spamDist[i][0], 0.0, 1e-8);
                        Assert.AreEqual(spamDist[i][1], 1.0, 1e-8);
                        Assert.AreEqual(loremDist[i][0], 1.0, 1e-8);
                        Assert.AreEqual(loremDist[i][1], 0.0, 1e-8);
                    }
                    else
                    {
                        Assert.AreEqual(spamDist[i][0], 1.0, 1e-8);
                        Assert.AreEqual(spamDist[i][1], 0.0, 1e-8);
                        Assert.AreEqual(loremDist[i][0], 0.0, 1e-8);
                        Assert.AreEqual(loremDist[i][1], 1.0, 1e-8);
                    }
                }
            }

            // Consume the model
            {
                // This classifies as spam
                string text = @"I decided to sign up for";
                int[] input = bow.GetFeatureVector(Tokenize(text));
                int answer = nb.Decide(input);
                string result = classes[answer];
                Assert.AreEqual("lorem", result);
            }

            {
                // This classifies as spam
                string text = @"I decided to sign up for the";
                int[] input = bow.GetFeatureVector(Tokenize(text));
                int answer = nb.Decide(input);
                string result = classes[answer];
                Assert.AreEqual("spam", result);
            }

            {
                // This classifies as lorem
                string text = @"I decided to lorem ipsum nulla nec tortor purus sit amet";
                int[] input = bow.GetFeatureVector(Tokenize(text));
                int answer = nb.Decide(input);
                string result = classes[answer];
                Assert.AreEqual("lorem", result);
            }
        }

        [Test]
        public void no_samples_for_class_simple()
        {
            int[][] inputs =
            {
                new [] { 1, 1 }, // 0
                new [] { 1, 1 }, // 0
                new [] { 1, 1 }, // 2
            };

            int[] outputs =
            {
                0, 0, 2
            };


            var teacher = new NaiveBayesLearning();

            Assert.Throws<ArgumentException>(() => teacher.Learn(inputs, outputs),
               "There are no samples for class label {0}. Please make sure that class " +
               "labels are contiguous and there is at least one training sample for each label.", 1);
        }

        [Test]
        public void no_samples_for_class_double()
        {
            double[][] inputs =
            {
                new double[] { 1, 1 }, // 0
                new double[] { 1, 1 }, // 0
                new double[] { 1, 1 }, // 2
            };

            int[] outputs =
            {
                0, 0, 2
            };


            var teacher = new NaiveBayesLearning<NormalDistribution>();

            Assert.Throws<ArgumentException>(() => teacher.Learn(inputs, outputs),
               "There are no samples for class label {0}. Please make sure that class " +
               "labels are contiguous and there is at least one training sample for each label.", 1);
        }

        [Test]
        public void gh_758()
        {
            // Let's say we have the following data to be classified into three 
            // non -mutually-exclusive possible classes. Those are the samples:
            //
            int[][] inputs =
            {
                //               input         output
                new int[] { 0, 1, 1, 0 }, //  0 
                new int[] { 0, 1, 0, 0 }, //  0
                new int[] { 0, 0, 1, 0 }, //  0
                new int[] { 0, 1, 1, 0 }, //  0, 1
                new int[] { 0, 1, 0, 0 }, //  0, 1
                new int[] { 1, 0, 0, 0 }, //     1
                new int[] { 1, 0, 0, 0 }, //     1
                new int[] { 1, 0, 0, 1 }, //     1, 2
                new int[] { 0, 0, 0, 1 }, //     1, 2
                new int[] { 0, 0, 0, 1 }, //     1, 2
                new int[] { 1, 1, 1, 1 }, //        2
                new int[] { 1, 0, 1, 1 }, //        2
                new int[] { 1, 1, 0, 1 }, //        2
                new int[] { 0, 1, 1, 1 }, //        2
                new int[] { 1, 1, 1, 1 }, //        2
            };

            int[][] outputs = // those are the class labels
            {
                new[] { 1, 0, 0 },
                new[] { 1, 0, 0 },
                new[] { 1, 0, 0 },
                new[] { 1, 1, 0 },
                new[] { 1, 1, 0 },
                new[] { 0, 1, 0 },
                new[] { 0, 1, 0 },
                new[] { 0, 1, 1 },
                new[] { 0, 1, 1 },
                new[] { 0, 1, 1 },
                new[] { 0, 0, 1 },
                new[] { 0, 0, 1 },
                new[] { 0, 0, 1 },
                new[] { 0, 0, 1 },
                new[] { 0, 0, 1 },
            };

            // Create a new Naive Bayes teacher 
            var teacher = new NaiveBayesLearning();

            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;

            var bayes = teacher.Learn(inputs, outputs);

            double[][] prediction = bayes.Probabilities(inputs);

            // Teach the Naive Bayes model. The error should be zero:
            double error = new BinaryCrossEntropyLoss(outputs).Loss(prediction);

            Assert.AreEqual(11.566909963298386, error, 1e-8);

            Assert.IsTrue(teacher.optimized);
        }

        [Test]
        public void CrossValidationTest()
        {
            #region doc_cross_validation
            // Ensure we have reproducible results
            Accord.Math.Random.Generator.Seed = 0;

            // Let's say we have the following data to be classified
            // into three possible classes. Those are the samples:
            //
            int[][] inputs =
            {
                //               input      output
                new int[] { 0, 1, 1, 0 }, //  0 
                new int[] { 0, 1, 0, 0 }, //  0
                new int[] { 0, 0, 1, 0 }, //  0
                new int[] { 0, 1, 1, 0 }, //  0
                new int[] { 0, 1, 0, 0 }, //  0
                new int[] { 1, 0, 0, 0 }, //  1
                new int[] { 1, 0, 0, 0 }, //  1
                new int[] { 1, 0, 0, 1 }, //  1
                new int[] { 0, 0, 0, 1 }, //  1
                new int[] { 0, 0, 0, 1 }, //  1
                new int[] { 1, 1, 1, 1 }, //  2
                new int[] { 1, 0, 1, 1 }, //  2
                new int[] { 1, 1, 0, 1 }, //  2
                new int[] { 0, 1, 1, 1 }, //  2
                new int[] { 1, 1, 1, 1 }, //  2
            };

            int[] outputs = // those are the class labels
            {
                0, 0, 0, 0, 0,
                1, 1, 1, 1, 1,
                2, 2, 2, 2, 2,
            };

            // Let's say we want to measure the cross-validation 
            // performance of Naive Bayes on the above data set:
            var cv = CrossValidation.Create(

                k: 10, // We will be using 10-fold cross validation

                // First we define the learning algorithm:
                learner: (p) => new NaiveBayesLearning(),

                // Now we have to specify how the n.b. performance should be measured:
                loss: (actual, expected, p) => new ZeroOneLoss(expected).Loss(actual),

                // This function can be used to perform any special
                // operations before the actual learning is done, but
                // here we will just leave it as simple as it can be:
                fit: (teacher, x, y, w) => teacher.Learn(x, y, w),

                // Finally, we have to pass the input and output data
                // that will be used in cross-validation. 
                x: inputs, y: outputs
            );

            // After the cross-validation object has been created,
            // we can call its .Learn method with the input and 
            // output data that will be partitioned into the folds:
            var result = cv.Learn(inputs, outputs);

            // We can grab some information about the problem:
            int numberOfSamples = result.NumberOfSamples; // should be 15
            int numberOfInputs = result.NumberOfInputs;   // should be 4
            int numberOfOutputs = result.NumberOfOutputs; // should be 3

            double trainingError = result.Training.Mean; // should be 0
            double validationError = result.Validation.Mean; // should be 0.15 (+/- var. 0.11388888888888887)

            // If desired, compute an aggregate confusion matrix for the validation sets:
            GeneralConfusionMatrix gcm = result.ToConfusionMatrix(inputs, outputs);
            #endregion

            Assert.AreEqual(15, numberOfSamples);
            Assert.AreEqual(4, numberOfInputs);
            Assert.AreEqual(3, numberOfOutputs);

            Assert.AreEqual(10, cv.K);
            Assert.AreEqual(0, result.Training.Mean, 1e-10);
            Assert.AreEqual(0.15, result.Validation.Mean, 1e-10);

            Assert.AreEqual(0, result.Training.Variance, 1e-10);
            Assert.AreEqual(0.11388888888888887, result.Validation.Variance, 1e-10);

            Assert.AreEqual(10, cv.Folds.Length);
            Assert.AreEqual(10, result.Models.Length);

            Assert.AreEqual(15, gcm.Samples);
            Assert.AreEqual(0.8666666666666667, gcm.Accuracy);
            Assert.AreEqual(0.1333333333333333, gcm.Error);
            Assert.AreEqual(3, gcm.Classes);
        }


        [Test]
        public void gh_802()
        {
            int[][] inputs = new int[][]
            {
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 0, 0, 1 },
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 1 },
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 1 },
                new int[] { 1, 1, 3, 3 },
                new int[] { 1, 1, 3, 2 },
                new int[] { 1, 1, 3, 4 },
                new int[] { 1, 1, 3, 8 },
                new int[] { 1, 1, 3, 6 },
                new int[] { 1, 1, 3, 3 },
                new int[] { 1, 1, 3, 2 },
                new int[] { 1, 1, 3, 4 },
                new int[] { 1, 1, 3, 8 },
                new int[] { 1, 1, 3, 11 },
                new int[] { 1, 1, 2, 3 },
                new int[] { 1, 1, 2, 2 },
                new int[] { 1, 1, 2, 4 },
                new int[] { 1, 1, 2, 5 },
                new int[] { 1, 1, 2, 6 },
                new int[] { 1, 1, 3, 3 },
                new int[] { 1, 1, 3, 2 },
                new int[] { 1, 1, 3, 4 },
                new int[] { 1, 1, 3, 8 },
                new int[] { 1, 1, 3, 6 },
                new int[] { 1, 1, 4, 2 },
                new int[] { 1, 1, 4, 5 },
                new int[] { 1, 1, 3, 3 },
                new int[] { 1, 1, 3, 2 },
                new int[] { 1, 1, 3, 4 },
                new int[] { 1, 1, 3, 8 },
                new int[] { 1, 1, 3, 6 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 1, 3, 5, 12 },
                new int[] { 1, 3, 5, 13 },
                new int[] { 1, 3, 5, 13 },
                new int[] { 0, 0, 0, 1 },
                new int[] { 1, 1, 2, 3 },
                new int[] { 1, 1, 2, 5 },
                new int[] { 1, 1, 2, 6 },
                new int[] { 1, 1, 2, 9 },
                new int[] { 1, 1, 2, 3 },
                new int[] { 1, 1, 2, 2 },
                new int[] { 1, 1, 2, 4 },
                new int[] { 1, 1, 2, 5 },
                new int[] { 1, 1, 2, 6 },
                new int[] { 1, 1, 3, 3 },
                new int[] { 1, 1, 3, 2 },
                new int[] { 1, 1, 3, 4 },
                new int[] { 1, 1, 3, 8 },
                new int[] { 1, 1, 3, 6 },
                new int[] { 1, 1, 3, 3 },
                new int[] { 1, 1, 3, 2 },
                new int[] { 1, 1, 3, 4 },
                new int[] { 1, 1, 3, 8 },
                new int[] { 1, 1, 3, 6 },
                new int[] { 1, 1, 2, 3 },
                new int[] { 1, 1, 2, 2 },
                new int[] { 1, 1, 2, 4 },
                new int[] { 1, 1, 2, 5 },
                new int[] { 1, 1, 2, 6 },
                new int[] { 1, 1, 3, 3 },
                new int[] { 1, 1, 3, 2 },
                new int[] { 1, 1, 3, 4 },
                new int[] { 1, 1, 3, 8 },
                new int[] { 1, 1, 3, 6 },
                new int[] { 1, 1, 3, 3 },
                new int[] { 1, 1, 3, 2 },
                new int[] { 1, 1, 3, 4 },
                new int[] { 1, 1, 3, 8 },
                new int[] { 1, 1, 3, 6 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 14 },
                new int[] { 0, 4, 6, 15 },
                new int[] { 0, 4, 6, 15 },
                new int[] { 0, 4, 6, 15 },
                new int[] { 0, 5, 0, 13 },
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 1 },
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 1 },
                new int[] { 1, 1, 3, 6 },
                new int[] { 1, 1, 1, 3 },
                new int[] { 1, 1, 1, 2 },
                new int[] { 1, 1, 1, 8 },
                new int[] { 1, 1, 1, 5 },
                new int[] { 1, 1, 1, 6 },
                new int[] { 1, 1, 1, 3 },
                new int[] { 1, 1, 1, 2 },
                new int[] { 1, 1, 1, 8 },
                new int[] { 1, 1, 1, 5 },
                new int[] { 1, 1, 1, 6 },
                new int[] { 1, 1, 4, 2 },
                new int[] { 1, 1, 4, 4 },
                new int[] { 1, 1, 4, 8 },
                new int[] { 1, 1, 4, 5 },
                new int[] { 1, 1, 4, 6 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 6, 0, 16 },
                new int[] { 0, 4, 6, 15 },
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 1 },
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 1 },
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 1 },
                new int[] { 1, 1, 3, 6 },
                new int[] { 1, 1, 3, 6 },
                new int[] { 1, 1, 4, 2 },
                new int[] { 1, 1, 4, 4 },
                new int[] { 1, 1, 4, 8 },
                new int[] { 1, 1, 4, 5 },
                new int[] { 1, 1, 4, 6 },
                new int[] { 1, 1, 3, 2 },
                new int[] { 1, 1, 3, 4 },
                new int[] { 1, 1, 3, 6 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 1, 2, 0, 10 },
                new int[] { 1, 2, 0, 10 },
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 1 },
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 1 },
                new int[] { 1, 1, 7, 5 },
                new int[] { 1, 1, 1, 3 },
                new int[] { 1, 1, 1, 2 },
                new int[] { 1, 1, 1, 8 },
                new int[] { 1, 1, 1, 5 },
                new int[] { 1, 1, 1, 6 },
                new int[] { 0, 2, 0, 10 },
                new int[] { 1, 2, 0, 10 },
                new int[] { 0, 6, 0, 16 },
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 1 },
                new int[] { 1, 1, 1, 3 },
                new int[] { 1, 1, 1, 2 },
                new int[] { 1, 1, 1, 8 },
                new int[] { 1, 1, 1, 5 },
                new int[] { 1, 1, 1, 6 },
                new int[] { 0, 2, 0, 10 }
            };


            int[] outputs = new int[] { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1 };

            // Create a new Naive Bayes teacher 
            var teacher = new NaiveBayesLearning()
            {
                Model = new NaiveBayes(2, 2, 7, 8, 17)
            };

            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;

            var bayes = teacher.Learn(inputs, outputs);

            double[][] prediction = bayes.Probabilities(inputs);

            // Teach the Naive Bayes model. The error should be zero:
            double error = new BinaryCrossEntropyLoss(outputs).Loss(prediction);

            Assert.AreEqual(53.346248364202573, error, 1e-8);

        }



        [Test]
        public void gh_811()
        {
            // https://github.com/accord-net/framework/issues/811

            Assert.That(() => new NaiveBayesLearning().Learn(new int[][] { new int[] { 1, 1, 1, 5 }, new int[] { 0, 2, 0, 10 } }, new int[] { 1, 1 }),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("There are no samples for class label 0. Please make sure that class labels are contiguous and there is at least one training sample for each label."));

            Assert.That(() => new NaiveBayesLearning().Learn(new int[][] { new int[] { 1, 1, 1, 5 }, new int[] { 0, 2, 0, 10 } }, new int[] { 0, 0 }),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Message.EqualTo(@"The number of classses should be higher than 1 (currently detected: 1).
Parameter name: classes"));

            Assert.That(() => new NaiveBayesLearning().Learn(new int[][] { new int[] { 1, 1, 1, 5 }, new int[] { 0, 2, 0, 10 } }, new int[] { 1, 2 }),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("There are no samples for class label 0. Please make sure that class labels are contiguous and there is at least one training sample for each label."));

            Assert.That(() => new NaiveBayesLearning().Learn(new int[][] { new int[] { 1, 1, 1, 5 }, new int[] { 0, 2, 0, 10 } }, new int[] { 0, 0 }),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Message.EqualTo(@"The number of classses should be higher than 1 (currently detected: 1).
Parameter name: classes"));

            Assert.That(() => new NaiveBayesLearning().Learn(new int[][] { new int[] { 1, 1, 1, 5 }, new int[] { 0, 2, 0, 10 } }, new int[] { 0, 3 }),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("There are no samples for class label 1. Please make sure that class labels are contiguous and there is at least one training sample for each label."));

            Assert.That(() => new NaiveBayesLearning().Learn(new int[][] { new int[] { 1, -1, 1, 5 }, new int[] { 0, 2, 0, 10 } }, new int[] { 1, 1 }),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("The input vector ad index 0 contains a negative value at position 1, which is not supported by this algorithm."));

            Assert.That(() => new NaiveBayesLearning().Learn(new int[][] { new int[] { 1, 1, -42, 5 }, new int[] { 0, 2, 0, 10 } }, new int[] { 0, 1 }),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("The input vector ad index 0 contains a negative value at position 2, which is not supported by this algorithm."));
        }

    }
}
#endif
