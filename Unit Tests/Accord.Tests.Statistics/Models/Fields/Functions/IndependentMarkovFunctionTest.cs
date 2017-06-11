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
    using Accord.Math.Differentiation;
    using Accord.Math.Optimization.Losses;
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
    using System;

    [TestFixture]
    public class IndependentMarkovFunctionTest
    {

        public static HiddenMarkovClassifier<Independent> CreateModel1()
        {
            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a multivariate sequence and the same sequence backwards.
            double[][][] sequences = new double[][][]
            {
                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 0 },
                    new double[] { 1 },
                    new double[] { 2 },
                    new double[] { 3 },
                    new double[] { 4 },
                }, 

                new double[][]
                {
                     // This is the second sequence with label = 1
                    new double[] { 4 },
                    new double[] { 3 },
                    new double[] { 2 },
                    new double[] { 1 },
                    new double[] { 0 },
                }
            };

            // Labels for the sequences
            int[] labels = { 0, 1 };

            // Creates a sequence classifier containing 2 hidden Markov Models
            //  with 2 states and an underlying Normal distribution as density.
            NormalDistribution component = new NormalDistribution();
            Independent density = new Independent(component);
            var classifier = new HiddenMarkovClassifier<Independent>(2, new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<Independent>(classifier,

                // Train each model until the log-likelihood changes less than 0.001
                modelIndex => new BaumWelchLearning<Independent>(classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);

            Assert.AreEqual(-13.271981026832929d, logLikelihood, 1e-10);

            return classifier;
        }

        public static HiddenMarkovClassifier<Independent> CreateModel2(out double[][][] sequences, out int[] labels)
        {
            sequences = new double[][][]
            {
                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 0, 1.1 },
                    new double[] { 1, 2.5 },
                    new double[] { 1, 3.4 },
                    new double[] { 1, 4.7 },
                    new double[] { 2, 5.8 },
                }, 

                new double[][]
                {
                        // This is the second sequence with label = 1
                    new double[] { 2,  3.2 },
                    new double[] { 2,  2.6 },
                    new double[] { 1,  1.2 },
                    new double[] { 1,  0.8 },
                    new double[] { 0,  1.1 },
                }
            };

            labels = new[] { 0, 1 };

            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a multivariate sequence and the same sequence backwards.
            var comp1 = new GeneralDiscreteDistribution(3);
            var comp2 = new NormalDistribution(1);
            var density = new Independent(comp1, comp2);

            // Creates a sequence classifier containing 2 hidden Markov Models with 2 states
            // and an underlying multivariate mixture of Normal distributions as density.
            var classifier = new HiddenMarkovClassifier<Independent>(
                2, new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<Independent>(
                classifier,

                // Train each model until the log-likelihood changes less than 0.0001
                modelIndex => new BaumWelchLearning<Independent>(
                    classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0,
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);

            Assert.AreEqual(double.NegativeInfinity, logLikelihood); // only one training sample per class

            return classifier;
        }

        public static HiddenMarkovClassifier<Independent, double[]> CreateModel2_learn(out double[][][] sequences, out int[] labels)
        {
            sequences = new double[][][]
            {
                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 0, 1.1 },
                    new double[] { 1, 2.5 },
                    new double[] { 1, 3.4 },
                    new double[] { 1, 4.7 },
                    new double[] { 2, 5.8 },
                }, 

                new double[][]
                {
                        // This is the second sequence with label = 1
                    new double[] { 2,  3.2 },
                    new double[] { 2,  2.6 },
                    new double[] { 1,  1.2 },
                    new double[] { 1,  0.8 },
                    new double[] { 0,  1.1 },
                }
            };

            labels = new[] { 0, 1 };

            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a multivariate sequence and the same sequence backwards.
            var comp1 = new GeneralDiscreteDistribution(3);
            var comp2 = new NormalDistribution(1);
            var density = new Independent(comp1, comp2);

            // Creates a sequence classifier containing 2 hidden Markov Models with 2 states
            // and an underlying multivariate mixture of Normal distributions as density.
            var classifier = new HiddenMarkovClassifier<Independent, double[]>(
                2, new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<Independent, double[]>(classifier)
            {

                // Train each model until the log-likelihood changes less than 0.0001
                Learner = modelIndex => new BaumWelchLearning<Independent, double[]>(classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0,
                }
            };

            // Train the sequence classifier using the algorithm
            //double logLikelihood = teacher.Run(sequences, labels);
            var model = teacher.Learn(sequences, labels);
            Assert.AreSame(model, classifier);

            return classifier;
        }


        public static HiddenMarkovClassifier<Independent> CreateModel3(out double[][][] sequences2, out int[] labels2)
        {
            sequences2 = new double[][][]
            {
                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 1, 1.12, 2.41, 1.17, 9.3 },
                    new double[] { 1, 2.54, 1.45, 0.16, 4.5 },
                    new double[] { 1, 3.46, 2.63, 1.15, 9.2 },
                    new double[] { 1, 4.73, 0.41, 1.54, 5.5 },
                    new double[] { 2, 5.81, 2.42, 1.13, 9.1 },
                }, 

                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 0, 1.49, 2.48, 1.18, 9.37 },
                    new double[] { 1, 2.18, 1.44, 2.19, 1.56 },
                    new double[] { 1, 3.77, 2.62, 1.10, 9.25 },
                    new double[] { 2, 4.76, 5.44, 3.58, 5.54 },
                    new double[] { 2, 5.85, 2.46, 1.16, 5.13 },
                    new double[] { 2, 4.84, 5.44, 3.54, 5.52 },
                    new double[] { 2, 5.83, 3.41, 1.22, 5.11 },
                }, 

                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 2, 1.11, 2.41, 1.12, 2.31 },
                    new double[] { 1, 2.52, 3.73, 0.12, 4.50 },
                    new double[] { 1, 3.43, 2.61, 1.24, 9.29 },
                    new double[] { 1, 4.74, 2.42, 2.55, 6.57 },
                    new double[] { 2, 5.85, 2.43, 1.16, 9.16 },
                }, 

                new double[][]
                {
                        // This is the second sequence with label = 1
                    new double[] { 0,  1.26, 5.44, 1.56, 9.55 },
                    new double[] { 2,  2.67, 5.45, 4.27, 1.54 },
                    new double[] { 1,  1.28, 3.46, 2.18, 4.13 },
                    new double[] { 1,  5.89, 2.57, 1.79, 5.02 },
                    new double[] { 0,  1.40, 2.48, 2.10, 6.41 },
                },

                new double[][]
                {
                        // This is the second sequence with label = 1
                    new double[] { 2,  3.21, 2.49, 1.54, 9.17 },
                    new double[] { 2,  2.62, 5.40, 4.25, 1.54 },
                    new double[] { 1,  1.53, 6.49, 2.17, 4.52 },
                    new double[] { 1,  2.84, 2.58, 1.73, 6.04 },
                    new double[] { 1,  1.45, 2.47, 2.28, 5.42 },
                    new double[] { 1,  1.46, 2.46, 2.35, 5.41 },
                },

                new double[][]
                {
                        // This is the second sequence with label = 1
                    new double[] { 1,  5.27, 5.45, 1.4, 9.5 },
                    new double[] { 2,  2.68, 2.54, 3.2, 2.2 },
                    new double[] { 1,  2.89, 3.83, 2.6, 4.1 },
                    new double[] { 1,  1.80, 1.32, 1.2, 4.2 },
                    new double[] { 0,  1.41, 2.41, 2.1, 6.4 },
                }
            };

            labels2 = new[] { 0, 0, 0, 1, 1, 1 };

            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a multivariate sequence and the same sequence backwards.
            var comp1 = new GeneralDiscreteDistribution(3);
            var comp2 = new NormalDistribution(1);
            var comp3 = new NormalDistribution(2);
            var comp4 = new NormalDistribution(3);
            var comp5 = new NormalDistribution(4);
            var density = new Independent(comp1, comp2, comp3, comp4, comp5);

            // Creates a sequence classifier containing 2 hidden Markov Models with 2 states
            // and an underlying multivariate mixture of Normal distributions as density.
            var classifier = new HiddenMarkovClassifier<Independent>(
                2, new Forward(5), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<Independent>(
                classifier,

                // Train each model until the log-likelihood changes less than 0.0001
                modelIndex => new BaumWelchLearning<Independent>(
                    classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0,
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences2, labels2);

            Assert.AreEqual(-3.0493028798326081d, logLikelihood, 1e-10);

            return classifier;
        }

        public static HiddenMarkovClassifier<Independent<NormalDistribution>> CreateModel4(out double[][][] words, out int[] labels, bool usePriors)
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

            double[][] wardrobe2 =
            {
                new double[] { 0.0, 0.0, 1.0, 0.0 }, // same for the
                new double[] { 0.2, 0.0, 1.0, 0.1 }, // wardrobe word.
                new double[] { 0.0, 0.1, 1.0, 0.0 },
                new double[] { 0.1, 0.0, 1.0, 0.2 },
            };

            words = new double[][][] { hello, car, wardrobe, wardrobe2 };

            labels = new [] { 0, 1, 2, 2 };

            var initial = new Independent<NormalDistribution>
            (
                new NormalDistribution(0, 1),
                new NormalDistribution(0, 1),
                new NormalDistribution(0, 1),
                new NormalDistribution(0, 1)
            );


            int numberOfWords = 3;
            int numberOfStates = 5;

            var classifier = new HiddenMarkovClassifier<Independent<NormalDistribution>>
            (
                classes: numberOfWords,
                topology: new Forward(numberOfStates),
                initial: initial
            );

            var teacher = new HiddenMarkovClassifierLearning<Independent<NormalDistribution>>(classifier,

                modelIndex => new BaumWelchLearning<Independent<NormalDistribution>>(classifier.Models[modelIndex])
                {
                    Tolerance = 0.001,
                    Iterations = 100,

                    FittingOptions = new IndependentOptions()
                    {
                        InnerOption = new NormalOptions() { Regularization = 1e-5 }
                    }
                }
            );

            if (usePriors)
                teacher.Empirical = true;

            double logLikelihood = teacher.Run(words, labels);

            Assert.AreEqual(208.38345600145777d, logLikelihood);

            return classifier;
        }



        [Test]
        public void ComputeTest()
        {
            var model = CreateModel1();

            var target = new MarkovMultivariateFunction(model);

            var hcrf = new HiddenConditionalRandomField<double[]>(target);

            double actual;
            double expected;

            double[][] x = { new double[] { 0 }, new double[] { 1 } };

            for (int c = 0; c < model.Classes; c++)
            {
                for (int i = 0; i < model[c].States; i++)
                {
                    // Check initial state transitions
                    expected = model.Priors[c] * Math.Exp(model[c].Probabilities[i]) * model[c].Emissions[i].ProbabilityDensityFunction(x[0]);
                    actual = Math.Exp(target.Factors[c].Compute(-1, i, x, 0, c));
                    Assert.AreEqual(expected, actual, 1e-6);
                    Assert.IsFalse(double.IsNaN(actual));
                }

                for (int t = 1; t < x.Length; t++)
                {
                    // Check normal state transitions
                    for (int i = 0; i < model[c].States; i++)
                    {
                        for (int j = 0; j < model[c].States; j++)
                        {
                            double xb = Math.Exp(model[c].Transitions[i, j]);
                            double xc = model[c].Emissions[j].ProbabilityDensityFunction(x[t]);
                            expected = xb * xc;
                            actual = Math.Exp(target.Factors[c].Compute(i, j, x, t, c));
                            Assert.AreEqual(expected, actual, 1e-6);
                            Assert.IsFalse(double.IsNaN(actual));
                        }
                    }
                }

                actual = model.LogLikelihood(x, c);
                expected = hcrf.LogLikelihood(x, c);
                Assert.AreEqual(expected, actual, 1e-8);
                Assert.IsFalse(double.IsNaN(actual));
            }
        }

        [Test]
        public void ComputeTest2()
        {
            double[][][] sequences;
            int[] labels;
            var model = CreateModel2(out sequences, out labels);

            var target = new MarkovMultivariateFunction(model);
            var hcrf = new HiddenConditionalRandomField<double[]>(target);


            double actual;
            double expected;

            double[][] x = { new double[] { 0, 1.7 }, new double[] { 2, 2.1 } };

            for (int c = 0; c < model.Classes; c++)
            {
                for (int i = 0; i < model[c].States; i++)
                {
                    // Check initial state transitions
                    expected = model.Priors[c] * Math.Exp(model[c].Probabilities[i]) * model[c].Emissions[i].ProbabilityDensityFunction(x[0]);
                    actual = Math.Exp(target.Factors[c].Compute(-1, i, x, 0, c));
                    Assert.AreEqual(expected, actual, 1e-6);
                    Assert.IsFalse(double.IsNaN(actual));
                }

                for (int t = 1; t < x.Length; t++)
                {
                    // Check normal state transitions
                    for (int i = 0; i < model[c].States; i++)
                    {
                        for (int j = 0; j < model[c].States; j++)
                        {
                            double xb = Math.Exp(model[c].Transitions[i, j]);
                            double xc = model[c].Emissions[j].ProbabilityDensityFunction(x[t]);
                            expected = xb * xc;
                            actual = Math.Exp(target.Factors[c].Compute(i, j, x, t, c));
                            Assert.AreEqual(expected, actual, 1e-6);
                            Assert.IsFalse(double.IsNaN(actual));
                        }
                    }
                }

                actual = model.LogLikelihood(x, c);
                expected = hcrf.LogLikelihood(x, c);
                Assert.AreEqual(expected, actual);
                Assert.IsFalse(double.IsNaN(actual));
            }
        }

        [Test]
        public void learn_test()
        {
            double[][][] sequences;
            int[] labels;
            HiddenMarkovClassifier<Independent, double[]> model = 
                CreateModel2_learn(out sequences, out labels);

            var target = new MarkovMultivariateFunction(model);
            var hcrf = new HiddenConditionalRandomField<double[]>(target);


            double actual;
            double expected;

            double[][] x = { new double[] { 0, 1.7 }, new double[] { 2, 2.1 } };

            for (int c = 0; c < model.Classes; c++)
            {
                for (int i = 0; i < model[c].States; i++)
                {
                    // Check initial state transitions
                    expected = model.Priors[c] * Math.Exp(model[c].LogInitial[i]) * model[c].Emissions[i].ProbabilityDensityFunction(x[0]);
                    actual = Math.Exp(target.Factors[c].Compute(-1, i, x, 0, c));
                    Assert.AreEqual(expected, actual, 1e-6);
                    Assert.IsFalse(double.IsNaN(actual));
                }

                for (int t = 1; t < x.Length; t++)
                {
                    // Check normal state transitions
                    for (int i = 0; i < model[c].States; i++)
                    {
                        for (int j = 0; j < model[c].States; j++)
                        {
                            double xb = Math.Exp(model[c].LogTransitions[i][j]);
                            double xc = model[c].Emissions[j].ProbabilityDensityFunction(x[t]);
                            expected = xb * xc;
                            actual = Math.Exp(target.Factors[c].Compute(i, j, x, t, c));
                            Assert.AreEqual(expected, actual, 1e-6);
                            Assert.IsFalse(double.IsNaN(actual));
                        }
                    }
                }

                actual = model.LogLikelihood(x, c);
                expected = hcrf.LogLikelihood(x, c);
                Assert.AreEqual(expected, actual, 1e-8);
                Assert.IsFalse(double.IsNaN(actual));
            }
        }


        [Test]
        public void ComputeTest3()
        {
            double[][][] sequences2;
            int[] labels2;
            var model = CreateModel3(out sequences2, out labels2);

            var target = new MarkovMultivariateFunction(model);

            var hcrf = new HiddenConditionalRandomField<double[]>(target);


            double actual;
            double expected;

            for (int k = 0; k < 5; k++)
            {
                foreach (var x in sequences2)
                {
                    for (int c = 0; c < model.Classes; c++)
                    {
                        for (int i = 0; i < model[c].States; i++)
                        {
                            // Check initial state transitions
                            double xa = model.Priors[c];
                            double xb = Math.Exp(model[c].Probabilities[i]);
                            double xc = model[c].Emissions[i].ProbabilityDensityFunction(x[0]);
                            expected = xa * xb * xc;
                            actual = Math.Exp(target.Factors[c].Compute(-1, i, x, 0, c));
                            Assert.AreEqual(expected, actual, 1e-6);
                            Assert.IsFalse(double.IsNaN(actual));
                        }

                        for (int t = 1; t < x.Length; t++)
                        {
                            // Check normal state transitions
                            for (int i = 0; i < model[c].States; i++)
                            {
                                for (int j = 0; j < model[c].States; j++)
                                {
                                    double xb = Math.Exp(model[c].Transitions[i, j]);
                                    double xc = model[c].Emissions[j].ProbabilityDensityFunction(x[t]);
                                    expected = xb * xc;
                                    actual = Math.Exp(target.Factors[c].Compute(i, j, x, t, c));
                                    Assert.AreEqual(expected, actual, 1e-6);
                                    Assert.IsFalse(double.IsNaN(actual));
                                }
                            }
                        }

                        actual = Math.Exp(model.LogLikelihood(x, c));
                        expected = Math.Exp(hcrf.LogLikelihood(x, c));
                        Assert.AreEqual(expected, actual, 1e-10);
                        Assert.IsFalse(double.IsNaN(actual));

                        actual = model.Compute(x);
                        expected = hcrf.Compute(x);
                        Assert.AreEqual(expected, actual);
                        Assert.IsFalse(double.IsNaN(actual));
                    }
                }
            }
        }

        [Test]
        public void ComputeTest4()
        {
            int[] labels;
            double[][][] words;
            HiddenMarkovClassifier<Independent<NormalDistribution>> model =
                CreateModel4(out words, out labels, false);

            var target = new MarkovMultivariateFunction(model);

            var hcrf = new HiddenConditionalRandomField<double[]>(target);


            Assert.AreEqual(3, model.Priors.Length);
            Assert.AreEqual(1 / 3.0, model.Priors[0]);
            Assert.AreEqual(1 / 3.0, model.Priors[1]);
            Assert.AreEqual(1 / 3.0, model.Priors[2]);

            check4(words, model, target, hcrf);
        }

        [Test]
        public void ComputeDeoptimizeTest3()
        {
            double[][][] sequences;
            int[] labels;
            var model = CreateModel3(out sequences, out labels);

            var target = new MarkovMultivariateFunction(model);

#pragma warning disable 0618
            target.Deoptimize();
#pragma warning restore 0618

            var hcrf = new HiddenConditionalRandomField<double[]>(target);


            Assert.AreEqual(2, model.Priors.Length);
            Assert.AreEqual(1 / 2.0, model.Priors[0]);
            Assert.AreEqual(1 / 2.0, model.Priors[1]);

            check4(sequences, model, target, hcrf);
        }

        [Test]
        public void ComputeDeoptimizeTest4()
        {
            int[] labels;
            double[][][] words;
            var model = CreateModel4(out words, out labels, false);

            var target = new MarkovMultivariateFunction(model);

#pragma warning disable 0618
            target.Deoptimize();
#pragma warning restore 0618

            var hcrf = new HiddenConditionalRandomField<double[]>(target);


            Assert.AreEqual(3, model.Priors.Length);
            Assert.AreEqual(1 / 3.0, model.Priors[0]);
            Assert.AreEqual(1 / 3.0, model.Priors[1]);
            Assert.AreEqual(1 / 3.0, model.Priors[2]);

            check4(words, model, target, hcrf);
        }

        private static void check4(double[][][] words, HiddenMarkovClassifier<Independent<NormalDistribution>> model, MarkovMultivariateFunction target, HiddenConditionalRandomField<double[]> hcrf)
        {
            double actual;
            double expected;

            foreach (var x in words)
            {
                for (int c = 0; c < model.Classes; c++)
                {
                    for (int i = 0; i < model[c].States; i++)
                    {
                        // Check initial state transitions
                        double xa = model.Priors[c];
                        double xb = Math.Exp(model[c].Probabilities[i]);
                        double xc = model[c].Emissions[i].ProbabilityDensityFunction(x[0]);
                        expected = xa * xb * xc;
                        actual = Math.Exp(target.Factors[c].Compute(-1, i, x, 0, c));
                        Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-10));
                        Assert.IsFalse(double.IsNaN(actual));
                    }

                    for (int t = 0; t < x.Length; t++)
                    {
                        // Check normal state transitions
                        for (int i = 0; i < model[c].States; i++)
                        {
                            for (int j = 0; j < model[c].States; j++)
                            {
                                double xb = Math.Exp(model[c].Transitions[i, j]);
                                double xc = model[c].Emissions[j].ProbabilityDensityFunction(x[t]);
                                expected = xb * xc;
                                actual = Math.Exp(target.Factors[c].Compute(i, j, x, t, c));
                                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-10));
                                Assert.IsFalse(double.IsNaN(actual));
                            }
                        }
                    }

                    actual = Math.Exp(model.LogLikelihood(x, c));
                    expected = Math.Exp(hcrf.LogLikelihood(x, c));
                    Assert.AreEqual(expected, actual, 1e-10);
                    Assert.IsFalse(double.IsNaN(actual));

                    actual = model.Compute(x);
                    expected = hcrf.Compute(x);
                    Assert.AreEqual(expected, actual);
                    Assert.IsFalse(double.IsNaN(actual));
                }
            }
        }

        private static void check4(double[][][] words, HiddenMarkovClassifier<Independent> model, MarkovMultivariateFunction target, HiddenConditionalRandomField<double[]> hcrf)
        {
            double actual;
            double expected;

            foreach (var x in words)
            {
                for (int c = 0; c < model.Classes; c++)
                {
                    for (int i = 0; i < model[c].States; i++)
                    {
                        // Check initial state transitions
                        double xa = model.Priors[c];
                        double xb = Math.Exp(model[c].Probabilities[i]);
                        double xc = model[c].Emissions[i].ProbabilityDensityFunction(x[0]);
                        expected = xa * xb * xc;
                        actual = Math.Exp(target.Factors[c].Compute(-1, i, x, 0, c));
                        Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-10));
                        Assert.IsFalse(double.IsNaN(actual));
                    }

                    for (int t = 1; t < x.Length; t++)
                    {
                        // Check normal state transitions
                        for (int i = 0; i < model[c].States; i++)
                        {
                            for (int j = 0; j < model[c].States; j++)
                            {
                                double xb = Math.Exp(model[c].Transitions[i, j]);
                                double xc = model[c].Emissions[j].ProbabilityDensityFunction(x[t]);
                                expected = xb * xc;
                                actual = Math.Exp(target.Factors[c].Compute(i, j, x, t, c));
                                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-10));
                                Assert.IsFalse(double.IsNaN(actual));
                            }
                        }
                    }

                    actual = Math.Exp(model.LogLikelihood(x, c));
                    expected = Math.Exp(hcrf.LogLikelihood(x, c));
                    Assert.AreEqual(expected, actual, 1e-10);
                    Assert.IsFalse(double.IsNaN(actual));

                    actual = model.Compute(x);
                    expected = hcrf.Compute(x);
                    Assert.AreEqual(expected, actual);
                    Assert.IsFalse(double.IsNaN(actual));
                }
            }
        }

        [Test]
        public void ComputeTestPriors4()
        {
            int[] labels;
            double[][][] words;
            var model = CreateModel4(out words, out labels, true);

            var target = new MarkovMultivariateFunction(model);

            var hcrf = new HiddenConditionalRandomField<double[]>(target);

            double actual;
            double expected;

            foreach (var x in words)
            {
                for (int c = 0; c < model.Classes; c++)
                {
                    for (int i = 0; i < model[c].States; i++)
                    {
                        // Check initial state transitions
                        double xa = model.Priors[c];
                        double xb = Math.Exp(model[c].Probabilities[i]);
                        double xc = model[c].Emissions[i].ProbabilityDensityFunction(x[0]);
                        expected = xa * xb * xc;
                        actual = Math.Exp(target.Factors[c].Compute(-1, i, x, 0, c));
                        Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-5));
                        Assert.IsFalse(double.IsNaN(actual));
                    }

                    for (int t = 1; t < x.Length; t++)
                    {
                        // Check normal state transitions
                        for (int i = 0; i < model[c].States; i++)
                        {
                            for (int j = 0; j < model[c].States; j++)
                            {
                                double xb = Math.Exp(model[c].Transitions[i, j]);
                                double xc = model[c].Emissions[j].ProbabilityDensityFunction(x[t]);
                                expected = xb * xc;
                                actual = Math.Exp(target.Factors[c].Compute(i, j, x, t, c));
                                Assert.IsTrue(expected.IsRelativelyEqual(actual, 1e-6));
                                Assert.IsFalse(double.IsNaN(actual));
                            }
                        }
                    }

                    actual = Math.Exp(model.LogLikelihood(x, c));
                    expected = Math.Exp(hcrf.LogLikelihood(x, c));
                    Assert.AreEqual(expected, actual, 1e-10);
                    Assert.IsFalse(double.IsNaN(actual));

                    actual = model.Compute(x);
                    expected = hcrf.Compute(x);
                    Assert.AreEqual(expected, actual);
                    Assert.IsFalse(double.IsNaN(actual));
                }
            }
        }



        [Test]
        public void GradientTest2()
        {
            double[][][] sequences2;
            int[] labels2;

            var hmm = CreateModel3(out sequences2, out labels2);
            var function = new MarkovMultivariateFunction(hmm);

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new ForwardBackwardGradient<double[]>(model);

            var inputs = sequences2;
            var outputs = labels2;

            double[] actual = target.Gradient(function.Weights, inputs, outputs);

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);
            diff.Function = parameters => func(model, parameters, inputs, outputs);
            double[] expected = diff.Compute(function.Weights);


            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-3);
                Assert.IsFalse(double.IsNaN(actual[i]));
                Assert.IsFalse(double.IsNaN(expected[i]));
            }
        }

        [Test]
        public void GradientTest3()
        {
            double[][][] sequences2;
            int[] labels2;
            var hmm = CreateModel3(out sequences2, out labels2);
            var function = new MarkovMultivariateFunction(hmm);

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new ForwardBackwardGradient<double[]>(model);
            target.Regularization = 2;

            var inputs = sequences2;
            var outputs = labels2;


            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            diff.Function = parameters => func(model, parameters, inputs, outputs, target.Regularization);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, inputs, outputs);


            for (int i = 0; i < actual.Length; i++)
            {
                double e = expected[i];
                double a = actual[i];
                Assert.AreEqual(e, a, 1e-3);

                Assert.IsFalse(double.IsNaN(actual[i]));
                Assert.IsFalse(double.IsNaN(expected[i]));
            }
        }


        private double func(HiddenConditionalRandomField<double[]> model, double[] parameters, double[][][] inputs, int[] outputs)
        {
            model.Function.Weights = parameters;
            return -model.LogLikelihood(inputs, outputs);
        }

        private double func(HiddenConditionalRandomField<double[]> model, double[] parameters, double[][][] inputs, int[] outputs, double beta)
        {
            model.Function.Weights = parameters;

            // Regularization
            double sumSquaredWeights = 0;
            if (beta != 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                    if (!(Double.IsInfinity(parameters[i]) || Double.IsNaN(parameters[i])))
                        sumSquaredWeights += parameters[i] * parameters[i];
                sumSquaredWeights = sumSquaredWeights * 0.5 / beta;
            }

            double logLikelihood = model.LogLikelihood(inputs, outputs);

            // Maximize the log-likelihood and minimize the sum of squared weights
            return -logLikelihood + sumSquaredWeights;
        }





        [Test]
        public void GradientDeoptimizeTest2()
        {
            double[][][] sequences2;
            int[] labels2;

            var hmm = CreateModel3(out sequences2, out labels2);
            var function = new MarkovMultivariateFunction(hmm);

#pragma warning disable 0618
            function.Deoptimize();
#pragma warning restore 0618

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new ForwardBackwardGradient<double[]>(model);

            var inputs = sequences2;
            var outputs = labels2;

            double[] actual = target.Gradient(function.Weights, inputs, outputs);

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);
            diff.Function = parameters => func(model, parameters, inputs, outputs);
            double[] expected = diff.Compute(function.Weights);


            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-3);
                Assert.IsFalse(double.IsNaN(actual[i]));
                Assert.IsFalse(double.IsNaN(expected[i]));
            }
        }

        [Test]
        public void GradientDeoptimizeTest3()
        {
            double[][][] sequences2;
            int[] labels2;
            var hmm = CreateModel3(out sequences2, out labels2);
            var function = new MarkovMultivariateFunction(hmm);

#pragma warning disable 0618
            function.Deoptimize();
#pragma warning restore 0618

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new ForwardBackwardGradient<double[]>(model);
            target.Regularization = 2;

            var inputs = sequences2;
            var outputs = labels2;


            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            diff.Function = parameters => func(model, parameters, inputs, outputs, target.Regularization);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, inputs, outputs);


            for (int i = 0; i < actual.Length; i++)
            {
                double e = expected[i];
                double a = actual[i];
                Assert.AreEqual(e, a, 1e-3);

                Assert.IsFalse(double.IsNaN(actual[i]));
                Assert.IsFalse(double.IsNaN(expected[i]));
            }
        }

    }
}
