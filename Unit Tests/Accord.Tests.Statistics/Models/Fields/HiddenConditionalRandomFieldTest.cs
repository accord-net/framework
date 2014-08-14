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

namespace Accord.Tests.Statistics.Models.Fields
{
    using Accord.Statistics.Models.Fields;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Math;
    using System;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions.Fitting;

    [TestClass()]
    public class HiddenConditionalRandomFieldTest
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
        public void HiddenConditionalRandomFieldConstructorTest()
        {
            HiddenMarkovClassifier hmm = HiddenMarkovClassifierPotentialFunctionTest.CreateModel1();

            var function = new MarkovDiscreteFunction(hmm);
            var target = new HiddenConditionalRandomField<int>(function);

            Assert.AreEqual(function, target.Function);
            Assert.AreEqual(2, target.Function.Factors[0].States);
        }

        [TestMethod()]
        public void ComputeTest()
        {
            HiddenMarkovClassifier hmm = HiddenMarkovClassifierPotentialFunctionTest.CreateModel1();

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



        [TestMethod()]
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

            Independent initial = new Independent
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

            var classifier = new HiddenMarkovClassifier<Independent>
            (
               classes: numberOfWords, 
               topology: new Forward(numberOfStates), // word classifiers should use a forward topology
               initial: initial
            );

            // Create a new learning algorithm to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<Independent>(classifier,

                // Train each model until the log-likelihood changes less than 0.001
                modelIndex => new BaumWelchLearning<Independent>(classifier.Models[modelIndex])
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
            int c1 = classifier.Compute(hello);
            int c2 = classifier.Compute(car);
            int c3 = classifier.Compute(wardrobe);

            Assert.AreEqual(0, c1);
            Assert.AreEqual(1, c2);
            Assert.AreEqual(2, c3);
        }




        [TestMethod()]
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
    }
}
