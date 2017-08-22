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

namespace Accord.Tests.Statistics.Models.Fields
{
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Fields;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Fields.Learning;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Statistics.Models.Markov.Topology;
    using NUnit.Framework;
    using System.IO;

    [TestFixture]
    public class HiddenConditionalRandomFieldTest
    {


        [Test]
        public void HiddenConditionalRandomFieldConstructorTest()
        {
            HiddenMarkovClassifier hmm = DiscreteHiddenMarkovClassifierPotentialFunctionTest.CreateModel1();

            var function = new MarkovDiscreteFunction(hmm);
            var target = new HiddenConditionalRandomField<int>(function);

            Assert.AreEqual(function, target.Function);
            Assert.AreEqual(2, target.Function.Factors[0].States);
        }

        [Test]
        public void ComputeTest()
        {
            HiddenMarkovClassifier hmm = DiscreteHiddenMarkovClassifierPotentialFunctionTest.CreateModel1();

            // Declare some testing data
            int[][] inputs = new int[][]
            {
                new int[] { 0,1,1,0 },   // Class 0
                new int[] { 0,0,1,0 },   // Class 0
                new int[] { 0,1,1,1,0 }, // Class 0
                new int[] { 0,1,0 },     // Class 0

                new int[] { 1,0,0,1 },   // Class 1
                new int[] { 1,1,0,1 },   // Class 1
                new int[] { 1,0,0,0,1 }, // Class 1
                new int[] { 1,0,1 },     // Class 1
            };

            int[] outputs = new int[]
            {
                0,0,0,0, // First four sequences are of class 0
                1,1,1,1, // Last four sequences are of class 1
            };


            var function = new MarkovDiscreteFunction(hmm);
            var target = new HiddenConditionalRandomField<int>(function);


            for (int i = 0; i < inputs.Length; i++)
            {
                int expected = hmm.Compute(inputs[i]);

                int actual = target.Compute(inputs[i]);

                double h0 = hmm.LogLikelihood(inputs[i], 0);
                double h1 = hmm.LogLikelihood(inputs[i], 1);

                double c0 = target.LogLikelihood(inputs[i], 0);
                double c1 = target.LogLikelihood(inputs[i], 1);

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(h0, c0, 1e-10);
                Assert.AreEqual(h1, c1, 1e-10);

                Assert.IsFalse(double.IsNaN(c0));
                Assert.IsFalse(double.IsNaN(c1));
            }
        }



        [Test]
        public void SimpleGestureRecognitionTest()
        {
            // Let's say we would like to do a very simple mechanism for
            // gesture recognition. In this example, we will be trying to
            // create a classifier that can distinguish between the words
            // "hello", "car", and "wardrobe". 

            // Let's say we decided to acquire some data, and we asked some
            // people to perform those words in front of a Kinect camera, and,
            // using Microsoft's SDK, we were able to captured the x and y
            // coordinates of each hand while the word was being performed.

            // Let's say we decided to represent our frames as:
            // 
            //    double[] frame = { leftHandX, leftHandY, rightHandX, rightHandY };
            //
            // Since we captured words, this means we captured sequences of
            // frames as we described above. Let's write some of those as 
            // rough examples to explain how gesture recognition can be done:

            double[][] hello =
            {
                new double[] { 1.0, 0.1, 0.0, 0.0 }, // let's say the word
                new double[] { 0.0, 1.0, 0.1, 0.1 }, // hello took 6 frames
                new double[] { 0.0, 1.0, 0.1, 0.1 }, // to be recorded.
                new double[] { 0.0, 0.0, 1.0, 0.0 },
                new double[] { 0.0, 0.0, 1.0, 0.0 },
                new double[] { 0.0, 0.0, 0.1, 1.1 },
            };

            double[][] car =
            {
                new double[] { 0.0, 0.0, 0.0, 1.0 }, // the car word
                new double[] { 0.1, 0.0, 1.0, 0.1 }, // took only 4.
                new double[] { 0.0, 0.0, 0.1, 0.0 },
                new double[] { 1.0, 0.0, 0.0, 0.0 },
            };

            double[][] wardrobe =
            {
                new double[] { 0.0, 0.0, 1.0, 0.0 }, // same for the
                new double[] { 0.1, 0.0, 1.0, 0.1 }, // wardrobe word.
                new double[] { 0.0, 0.1, 1.0, 0.0 },
                new double[] { 0.1, 0.0, 1.0, 0.1 },
            };

            // Here, please note that a real-world example would involve *lots*
            // of samples for each word. Here, we are considering just one from
            // each class which is clearly sub-optimal and should _never_ be done
            // on practice. For example purposes, however, please disregard this.

            // Those are the words we have in our vocabulary:
            //
            double[][][] words = { hello, car, wardrobe };

            // Now, let's associate integer labels with them. This is needed
            // for the case where there are multiple samples for each word.
            //
            int[] labels = { 0, 1, 2 };


            // We will create our classifiers assuming an independent
            // Gaussian distribution for each component in our feature
            // vectors (like assuming a Naive Bayes assumption).

            var initial = new Independent<NormalDistribution>
            (
                new NormalDistribution(0, 1),
                new NormalDistribution(0, 1),
                new NormalDistribution(0, 1),
                new NormalDistribution(0, 1)
            );


            // Now, we can proceed and create our classifier. 
            //
            int numberOfWords = 3;  // we are trying to distinguish between 3 words
            int numberOfStates = 5; // this value can be found by trial-and-error

            var hmm = new HiddenMarkovClassifier<Independent<NormalDistribution>>
            (
                classes: numberOfWords,
                topology: new Forward(numberOfStates), // word classifiers should use a forward topology
                initial: initial
            );

            // Create a new learning algorithm to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<Independent<NormalDistribution>>(hmm,

                // Train each model until the log-likelihood changes less than 0.001
                modelIndex => new BaumWelchLearning<Independent<NormalDistribution>>(hmm.Models[modelIndex])
                {
                    Tolerance = 0.001,
                    Iterations = 100,

                    // This is necessary so the code doesn't blow up when it realize
                    // there is only one sample per word class. But this could also be
                    // needed in normal situations as well.
                    //
                    FittingOptions = new IndependentOptions()
                    {
                        InnerOption = new NormalOptions() { Regularization = 1e-5 }
                    }
                }
            );

            // Finally, we can run the learning algorithm!
            double logLikelihood = teacher.Run(words, labels);

            // At this point, the classifier should be successfully 
            // able to distinguish between our three word classes:
            //
            int tc1 = hmm.Compute(hello);
            int tc2 = hmm.Compute(car);
            int tc3 = hmm.Compute(wardrobe);

            Assert.AreEqual(0, tc1);
            Assert.AreEqual(1, tc2);
            Assert.AreEqual(2, tc3);

            // Now, we can use the Markov classifier to initialize a HCRF
            var function = new MarkovMultivariateFunction(hmm);
            var hcrf = new HiddenConditionalRandomField<double[]>(function);


            // We can check that both are equivalent, although they have
            // formulations that can be learned with different methods
            //
            for (int i = 0; i < words.Length; i++)
            {
                // Should be the same
                int expected = hmm.Compute(words[i]);
                int actual = hcrf.Compute(words[i]);

                // Should be the same
                double h0 = hmm.LogLikelihood(words[i], 0);
                double c0 = hcrf.LogLikelihood(words[i], 0);

                double h1 = hmm.LogLikelihood(words[i], 1);
                double c1 = hcrf.LogLikelihood(words[i], 1);

                double h2 = hmm.LogLikelihood(words[i], 2);
                double c2 = hcrf.LogLikelihood(words[i], 2);

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(h0, c0, 1e-10);
                Assert.IsTrue(h1.IsRelativelyEqual(c1, 1e-10));
                Assert.IsTrue(h2.IsRelativelyEqual(c2, 1e-10));

                Assert.IsFalse(double.IsNaN(c0));
                Assert.IsFalse(double.IsNaN(c1));
                Assert.IsFalse(double.IsNaN(c2));
            }


            // Now we can learn the HCRF using one of the best learning
            // algorithms available, Resilient Backpropagation learning:

            // Create a learning algorithm
            var rprop = new HiddenResilientGradientLearning<double[]>(hcrf)
            {
                Iterations = 50,
                Tolerance = 1e-5
            };

            // Run the algorithm and learn the models
            double error = rprop.Run(words, labels);

            // At this point, the HCRF should be successfully 
            // able to distinguish between our three word classes:
            //
            int hc1 = hcrf.Compute(hello);
            int hc2 = hcrf.Compute(car);
            int hc3 = hcrf.Compute(wardrobe);

            Assert.AreEqual(0, hc1);
            Assert.AreEqual(1, hc2);
            Assert.AreEqual(2, hc3);
        }


        [Test]
        public void learn_test()
        {
            Accord.Math.Random.Generator.Seed = 0;

            #region doc_learn_1
            // Let's say we would like to do a very simple mechanism for gesture recognition. 
            // In this example, we will be trying to create a classifier that can distinguish 
            // between the words "hello", "car", and "wardrobe". 

            // Let's say we decided to acquire some data, and we asked some people to perform 
            // those words in front of a Kinect camera, and, using Microsoft's SDK, we were able 
            // to captured the x and y coordinates of each hand while the word was being performed.

            // Let's say we decided to represent our frames as:
            // 
            //    double[] frame = { leftHandX, leftHandY, rightHandX, rightHandY }; // 4 dimensions
            //
            // Since we captured words, this means we captured sequences of frames as we described 
            // above. Let's write some of those as rough examples to explain how gesture recognition 
            // can be done:

            double[][] hello =
            {
                new double[] { 1.0, 0.1, 0.0, 0.0 }, // let's say the word
                new double[] { 0.0, 1.0, 0.1, 0.1 }, // hello took 6 frames
                new double[] { 0.0, 1.0, 0.1, 0.1 }, // to be recorded.
                new double[] { 0.0, 0.0, 1.0, 0.0 },
                new double[] { 0.0, 0.0, 1.0, 0.0 },
                new double[] { 0.0, 0.0, 0.1, 1.1 },
            };

            double[][] car =
            {
                new double[] { 0.0, 0.0, 0.0, 1.0 }, // the car word
                new double[] { 0.1, 0.0, 1.0, 0.1 }, // took only 4.
                new double[] { 0.0, 0.0, 0.1, 0.0 },
                new double[] { 1.0, 0.0, 0.0, 0.0 },
            };

            double[][] wardrobe =
            {
                new double[] { 0.0, 0.0, 1.0, 0.0 }, // same for the
                new double[] { 0.1, 0.0, 1.0, 0.1 }, // wardrobe word.
                new double[] { 0.0, 0.1, 1.0, 0.0 },
                new double[] { 0.1, 0.0, 1.0, 0.1 },
            };

            // Please note that a real-world example would involve *lots* of samples for each word. 
            // Here, we are considering just one from each class which is clearly sub-optimal and 
            // should _never_ be done on practice. Please keep in mind that we are doing like this
            // only to simplify this example on how to create and use HCRFs.

            // These are the words we have in our vocabulary:
            double[][][] words = { hello, car, wardrobe };

            // Now, let's associate integer labels with them. This is needed
            // for the case where there are multiple samples for each word.
            int[] labels = { 0, 1, 2 };

            // Create a new learning algorithm to train the hidden Markov model sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<Independent<NormalDistribution>, double[]>()
            {
                // Train each model until the log-likelihood changes less than 0.001
                Learner = (i) => new BaumWelchLearning<Independent<NormalDistribution>, double[]>()
                {
                    Topology = new Forward(5), // this value can be found by trial-and-error

                    // We will create our classifiers assuming an independent Gaussian distribution 
                    // for each component in our feature vectors (assuming a Naive Bayes assumption).
                    Emissions = (s) => new Independent<NormalDistribution>(dimensions: 4), // 4 dimensions

                    Tolerance = 0.001,
                    Iterations = 100,

                    // This is necessary so the code doesn't blow up when it realizes there is only one 
                    // sample per word class. But this could also be needed in normal situations as well:
                    FittingOptions = new IndependentOptions()
                    {
                        InnerOption = new NormalOptions() { Regularization = 1e-5 }
                    }
                }
            };

            // PS: In case you find exceptions trying to configure your model, you might want 
            //     to try disabling parallel processing to get more descriptive error messages:
            // teacher.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Finally, we can run the learning algorithm!
            var hmm = teacher.Learn(words, labels);
            double logLikelihood = teacher.LogLikelihood;

            // At this point, the classifier should be successfully 
            // able to distinguish between our three word classes:
            //
            int tc1 = hmm.Decide(hello);    // should be 0
            int tc2 = hmm.Decide(car);      // should be 1
            int tc3 = hmm.Decide(wardrobe); // should be 2
            #endregion 

            Assert.AreEqual(0, tc1);
            Assert.AreEqual(1, tc2);
            Assert.AreEqual(2, tc3);

            #region doc_learn_2
            // Now, we can use the Markov classifier to initialize a HCRF
            var baseline = HiddenConditionalRandomField.FromHiddenMarkov(hmm);

            // We can check that both are equivalent, although they have
            // formulations that can be learned with different methods:
            int[] predictedLabels = baseline.Decide(words);

            #endregion 

            // We can check that both are equivalent, although they have
            // formulations that can be learned with different methods
            //
            for (int i = 0; i < words.Length; i++)
            {
                // Should be the same
                int expected = hmm.Decide(words[i]);
                int actual = baseline.Decide(words[i]);

                // Should be the same
                double h0 = hmm.LogLikelihood(words[i], 0);
                double c0 = baseline.LogLikelihood(words[i], 0);

                double h1 = hmm.LogLikelihood(words[i], 1);
                double c1 = baseline.LogLikelihood(words[i], 1);

                double h2 = hmm.LogLikelihood(words[i], 2);
                double c2 = baseline.LogLikelihood(words[i], 2);

                Assert.AreEqual(expected, predictedLabels[i]);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(h0, c0, 1e-10);
                Assert.IsTrue(h1.IsRelativelyEqual(c1, 1e-10));
                Assert.IsTrue(h2.IsRelativelyEqual(c2, 1e-10));
            }

            Accord.Math.Random.Generator.Seed = 0;

            #region doc_learn_3
            // Now we can learn the HCRF using one of the best learning
            // algorithms available, Resilient Backpropagation learning:

            // Create the Resilient Backpropagation learning algorithm
            var rprop = new HiddenResilientGradientLearning<double[]>()
            {
                Function = baseline.Function, // use the same HMM function

                Iterations = 50,
                Tolerance = 1e-5
            };

            // Run the algorithm and learn the models
            var hcrf = rprop.Learn(words, labels);

            // At this point, the HCRF should be successfully 
            // able to distinguish between our three word classes:
            //
            int hc1 = hcrf.Decide(hello);    // should be 0
            int hc2 = hcrf.Decide(car);      // should be 1
            int hc3 = hcrf.Decide(wardrobe); // should be 2
            #endregion

            Assert.AreEqual(0, hc1); 
            Assert.AreEqual(1, hc2); 
            Assert.AreEqual(2, hc3); 
        }



        [Test]
        public void NumberOfFeaturesTest()
        {
            Independent initial = new Independent(
                      new GeneralDiscreteDistribution(46), // output,
                      new NormalDistribution(0, 1),        // headAngle, 
                      new NormalDistribution(0, 1),        // handAngle, 
                      new NormalDistribution(0, 1),        // relHandX, 
                      new NormalDistribution(0, 1)         // relHandY, 
            );



            var model = new HiddenMarkovClassifier<Independent>(
                classes: 13, topology: new Forward(5), initial: initial);

            var function = new MarkovMultivariateFunction(model);
            var field = new HiddenConditionalRandomField<double[]>(function);



            int discreteDistributions = 1;
            int continuousDistributions = 4;
            int symbols = 46;
            int states = 5;
            int classes = 13;
            int expected = classes *
                (1 + states + states * states +
                states * discreteDistributions * symbols +
                states * continuousDistributions * 3);

            Assert.AreEqual(expected, field.Function.Features.Length);
            Assert.AreEqual(expected, field.Function.Weights.Length);
            Assert.AreEqual(4173, expected);
        }

#if !NO_BINARY_SERIALIZATION
        [Test]
        public void SaveLoadTest()
        {
            double[][] hello =
            {
                new double[] { 1.0, 0.1, 0.0, 0.0 }, // let's say the word
                new double[] { 0.0, 1.0, 0.1, 0.1 }, // hello took 6 frames
                new double[] { 0.0, 1.0, 0.1, 0.1 }, // to be recorded.
                new double[] { 0.0, 0.0, 1.0, 0.0 },
                new double[] { 0.0, 0.0, 1.0, 0.0 },
                new double[] { 0.0, 0.0, 0.1, 1.1 },
            };

            double[][] car =
            {
                new double[] { 0.0, 0.0, 0.0, 1.0 }, // the car word
                new double[] { 0.1, 0.0, 1.0, 0.1 }, // took only 4.
                new double[] { 0.0, 0.0, 0.1, 0.0 },
                new double[] { 1.0, 0.0, 0.0, 0.0 },
            };

            double[][] wardrobe =
            {
                new double[] { 0.0, 0.0, 1.0, 0.0 }, // same for the
                new double[] { 0.1, 0.0, 1.0, 0.1 }, // wardrobe word.
                new double[] { 0.0, 0.1, 1.0, 0.0 },
                new double[] { 0.1, 0.0, 1.0, 0.1 },
            };

            double[][][] words = { hello, car, wardrobe };

            int[] labels = { 0, 1, 2 };

            var initial = new Independent
            (
                new NormalDistribution(0, 1),
                new NormalDistribution(0, 1),
                new NormalDistribution(0, 1),
                new NormalDistribution(0, 1)
            );

            int numberOfWords = 3;
            int numberOfStates = 5;

            var classifier = new HiddenMarkovClassifier<Independent>
            (
               classes: numberOfWords,
               topology: new Forward(numberOfStates),
               initial: initial
            );

            var teacher = new HiddenMarkovClassifierLearning<Independent>(classifier,
                modelIndex => new BaumWelchLearning<Independent>(classifier.Models[modelIndex])
                {
                    Tolerance = 0.001,
                    Iterations = 100,
                    FittingOptions = new IndependentOptions()
                    {
                        InnerOption = new NormalOptions() { Regularization = 1e-5 }
                    }
                }
            );

            double logLikelihood = teacher.Run(words, labels);

            var function = new MarkovMultivariateFunction(classifier);
            var hcrf = new HiddenConditionalRandomField<double[]>(function);


            MemoryStream stream = new MemoryStream();

            hcrf.Save(stream);

            stream.Seek(0, SeekOrigin.Begin);

            var target = HiddenConditionalRandomField<double[]>.Load(stream);

            Assert.AreEqual(hcrf.Function.Factors.Length, target.Function.Factors.Length);
            for (int i = 0; i < hcrf.Function.Factors.Length; i++)
            {
                var e = hcrf.Function.Factors[i];
                var a = target.Function.Factors[i];
                Assert.AreEqual(e.Index, target.Function.Factors[i].Index);
                Assert.AreEqual(e.States, target.Function.Factors[i].States);

                Assert.AreEqual(e.EdgeParameters.Count, a.EdgeParameters.Count);
                Assert.AreEqual(e.EdgeParameters.Offset, a.EdgeParameters.Offset);
                Assert.AreEqual(e.FactorParameters.Count, a.FactorParameters.Count);
                Assert.AreEqual(e.FactorParameters.Offset, a.FactorParameters.Offset);

                Assert.AreEqual(e.OutputParameters.Count, a.OutputParameters.Count);
                Assert.AreEqual(e.OutputParameters.Offset, a.OutputParameters.Offset);
                Assert.AreEqual(e.StateParameters.Count, a.StateParameters.Count);
                Assert.AreEqual(e.StateParameters.Offset, a.StateParameters.Offset);

                Assert.AreEqual(target.Function, a.Owner);
                Assert.AreEqual(hcrf.Function, e.Owner);
            }

            Assert.AreEqual(hcrf.Function.Features.Length, target.Function.Features.Length);
            for (int i = 0; i < hcrf.Function.Factors.Length; i++)
                Assert.AreEqual(hcrf.Function.Features[i].GetType(), target.Function.Features[i].GetType());

            Assert.AreEqual(hcrf.Function.Outputs, target.Function.Outputs);

            for (int i = 0; i < hcrf.Function.Weights.Length; i++)
                Assert.AreEqual(hcrf.Function.Weights[i], target.Function.Weights[i]);
        }
#endif
    }
}
