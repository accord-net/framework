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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Models.Markov.Learning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Filters;
    using Accord.Math;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Statistics.Distributions.Univariate;

    [TestClass()]
    public class GenericViterbiLearningTest
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
        public void LearnTest5()
        {

            double[][][] sequences = new double[][][] 
            {
                new double[][] { new double[] { 0 }, new double[] { 3 }, new double[] { 1 } },
                new double[][] { new double[] { 0 }, new double[] { 2 } },
                new double[][] { new double[] { 1 }, new double[] { 0 }, new double[] { 3 } },
                new double[][] { new double[] { 3 }, new double[] { 4 } },
                new double[][] { new double[] { 0 }, new double[] { 1 }, new double[] { 3 }, new double[] { 5 } },
                new double[][] { new double[] { 0 }, new double[] { 3 }, new double[] { 4 } },
                new double[][] { new double[] { 0 }, new double[] { 1 }, new double[] { 3 }, new double[] { 5 } },
                new double[][] { new double[] { 0 }, new double[] { 1 }, new double[] { 3 }, new double[] { 5 } },
                new double[][] { new double[] { 0 }, new double[] { 1 }, new double[] { 3 }, new double[] { 4 }, new double[] { 5 } },
            };

            var hmm = HiddenMarkovModel.CreateGeneric(3, 6);

            var teacher = new ViterbiLearning<GeneralDiscreteDistribution>(hmm) { Iterations = 100, Tolerance = 0 };
            double ll = teacher.Run(sequences);

            double l0; hmm.Decode(sequences[0], out l0);
            double l1; hmm.Decode(sequences[1], out l1);
            double l2; hmm.Decode(sequences[2], out l2);

            double pl = System.Math.Exp(ll);
            double p0 = System.Math.Exp(l0);
            double p1 = System.Math.Exp(l1);
            double p2 = System.Math.Exp(l2);

            Assert.AreEqual(0.077427215162407442, pl, 1e-6);
            Assert.AreEqual(0.009958847736625515, p0, 1e-6);
            Assert.AreEqual(0.006790123456790126, p1, 1e-6);
            Assert.AreEqual(0.009958847736625515, p2, 1e-6);

            Assert.AreEqual(1, hmm.Dimension);




            double[][] sequences2 = new double[][] 
            {
                new double[] { 0, 3, 1 },
                new double[] { 0, 2 },
                new double[] { 1, 0, 3 },
                new double[] { 3, 4 },
                new double[] { 0, 1, 3, 5 },
                new double[] { 0, 3, 4 },
                new double[] { 0, 1, 3, 5 },
                new double[] { 0, 1, 3, 5 },
                new double[] { 0, 1, 3, 4, 5 },
            };

            hmm = HiddenMarkovModel.CreateGeneric(3, 6);

            teacher = new ViterbiLearning<GeneralDiscreteDistribution>(hmm) { Iterations = 100 };
            double ll2 = teacher.Run(sequences2);

            double l02; hmm.Decode(sequences2[0], out l02);
            double l12; hmm.Decode(sequences2[1], out l12);
            double l22; hmm.Decode(sequences2[2], out l22);

            Assert.AreEqual(ll, ll2);
            Assert.AreEqual(l0, l02);
            Assert.AreEqual(l1, l12);
            Assert.AreEqual(l2, l22);

            Assert.AreEqual(1, hmm.Dimension);
        }

        [TestMethod()]
        public void LearnTest3()
        {

            double[][] sequences = new double[][] 
            {
                new double[] { 0,1,1,1,1,0,1,1,1,1 },
                new double[] { 0,1,1,1,0,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1         },
                new double[] { 0,1,1,1,1,1,1       },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
            };

            // Creates a new Hidden Markov Model with 3 states
            var hmm = HiddenMarkovModel.CreateGeneric(new Forward(3), 2);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            var teacher = new ViterbiLearning<GeneralDiscreteDistribution>(hmm)
            {
                Tolerance = 0.0001,
                Iterations = 0,

                FittingOptions = new GeneralDiscreteOptions()
                {
                    UseLaplaceRule = true
                }
            };


            double ll = teacher.Run(sequences);

            // Calculate the probability that the given
            //  sequences originated from the model
            double l1; hmm.Decode(new double[] { 0, 1 }, out l1);        // 0.4999
            double l2; hmm.Decode(new double[] { 0, 1, 1, 1 }, out l2);  // 0.1145

            double l3; hmm.Decode(new double[] { 1, 1 }, out l3);        // 0.0000
            double l4; hmm.Decode(new double[] { 1, 0, 0, 0 }, out l4);  // 0.0000

            double l5; hmm.Decode(new double[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }, out l5); // 0.0002
            double l6; hmm.Decode(new double[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }, out l6); // 0.0002


            ll = System.Math.Exp(ll);
            l1 = System.Math.Exp(l1);
            l2 = System.Math.Exp(l2);
            l3 = System.Math.Exp(l3);
            l4 = System.Math.Exp(l4);
            l5 = System.Math.Exp(l5);
            l6 = System.Math.Exp(l6);

            Assert.AreEqual(1.754393540912413, ll, 1e-6);
            Assert.AreEqual(0.53946360153256712, l1, 1e-6);
            Assert.AreEqual(0.44850249229903377, l2, 1e-6);
            Assert.AreEqual(0.08646414524833077, l3, 1e-6);
            Assert.AreEqual(0.00041152263374485, l4, 1e-6);
            Assert.AreEqual(0.01541807695931400, l5, 1e-6);
            Assert.AreEqual(0.01541807695931400, l6, 1e-6);

            Assert.IsTrue(l1 > l3 && l1 > l4);
            Assert.IsTrue(l2 > l3 && l2 > l4);

            Assert.AreEqual(1, hmm.Dimension);
        }

        [TestMethod()]
        public void LearnTest6()
        {
            // Continuous Markov Models can operate using any
            // probability distribution, including discrete ones. 

            // In the following example, we will try to create a
            // Continuous Hidden Markov Model using a discrete
            // distribution to detect if a given sequence starts
            // with a zero and has any number of ones after that.

            double[][] sequences = new double[][] 
            {
                new double[] { 0,1,1,1,1,0,1,1,1,1 },
                new double[] { 0,1,1,1,0,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1         },
                new double[] { 0,1,1,1,1,1,1       },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
            };

            // Create a new Hidden Markov Model with 3 states and
            //  a generic discrete distribution with two symbols
            var hmm = HiddenMarkovModel.CreateGeneric(new Forward(3), 2);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            var teacher = new ViterbiLearning<GeneralDiscreteDistribution>(hmm)
            {
                Tolerance = 0.0001,
                Iterations = 0,

                FittingOptions = new GeneralDiscreteOptions()
                {
                    UseLaplaceRule = true
                }
            };

            double ll = teacher.Run(sequences);

            // Calculate the probability that the given
            //  sequences originated from the model
            double l1 = hmm.Evaluate(new double[] { 0, 1 });       // 0.613
            double l2 = hmm.Evaluate(new double[] { 0, 1, 1, 1 }); // 0.500

            // Sequences which do not start with zero have much lesser probability.
            double l3 = hmm.Evaluate(new double[] { 1, 1 });       // 0.186
            double l4 = hmm.Evaluate(new double[] { 1, 0, 0, 0 }); // 0.003

            // Sequences which contains few errors have higher probability
            //  than the ones which do not start with zero. This shows some
            //  of the temporal elasticity and error tolerance of the HMMs.
            double l5 = hmm.Evaluate(new double[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }); // 0.033
            double l6 = hmm.Evaluate(new double[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }); // 0.026


            double pl = System.Math.Exp(ll);
            double p1 = System.Math.Exp(l1);
            double p2 = System.Math.Exp(l2);
            double p3 = System.Math.Exp(l3);
            double p4 = System.Math.Exp(l4);
            double p5 = System.Math.Exp(l5);
            double p6 = System.Math.Exp(l6);

            Assert.AreEqual(1.754393540912413, pl, 1e-6);
            Assert.AreEqual(0.61368718756104801, p1, 1e-6);
            Assert.AreEqual(0.50049466955818356, p2, 1e-6);
            Assert.AreEqual(0.18643340385264684, p3, 1e-6);
            Assert.AreEqual(0.00300262431355424, p4, 1e-6);
            Assert.AreEqual(0.03338686211012481, p5, 1e-6);
            Assert.AreEqual(0.02659161933179825, p6, 1e-6);

            Assert.IsFalse(Double.IsNaN(ll));
            Assert.IsFalse(Double.IsNaN(l1));
            Assert.IsFalse(Double.IsNaN(l2));
            Assert.IsFalse(Double.IsNaN(l3));
            Assert.IsFalse(Double.IsNaN(l4));
            Assert.IsFalse(Double.IsNaN(l5));
            Assert.IsFalse(Double.IsNaN(l6));

            Assert.IsTrue(l1 > l3 && l1 > l4);
            Assert.IsTrue(l2 > l3 && l2 > l4);
        }

        [TestMethod()]
        public void LearnTest7()
        {
            // Create continuous sequences. In the sequences below, there
            //  seems to be two states, one for values between 0 and 1 and
            //  another for values between 5 and 7. The states seems to be
            //  switched on every observation.
            double[][] sequences = new double[][] 
            {
                new double[] { 0.1, 5.2, 0.3, 6.7, 0.1, 6.0 },
                new double[] { 0.2, 6.2, 0.3, 6.3, 0.1, 5.0 },
                new double[] { 0.1, 7.0, 0.1, 7.0, 0.2, 5.6 },
            };


            // Specify a initial normal distribution for the samples.
            var density = new NormalDistribution();

            // Creates a continuous hidden Markov Model with two states organized in a forward
            //  topology and an underlying univariate Normal distribution as probability density.
            var model = new HiddenMarkovModel<NormalDistribution>(new Forward(2), density);

            // Configure the learning algorithms to train the sequence classifier until the
            // difference in the average log-likelihood changes only by as little as 0.0001
            var teacher = new ViterbiLearning<NormalDistribution>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,
            };

            // Fit the model
            double logLikelihood = teacher.Run(sequences);

            // See the probability of the sequences learned
            double a1 = model.Evaluate(new[] { 0.1, 5.2, 0.3, 6.7, 0.1, 6.0 }); // 0.40
            double a2 = model.Evaluate(new[] { 0.2, 6.2, 0.3, 6.3, 0.1, 5.0 }); // 0.46

            // See the probability of an unrelated sequence
            double a3 = model.Evaluate(new[] { 1.1, 2.2, 1.3, 3.2, 4.2, 1.0 }); // 1.42

            double likelihood = Math.Exp(logLikelihood);
            a1 = Math.Exp(a1);
            a2 = Math.Exp(a2);
            a3 = Math.Exp(a3);

            Assert.AreEqual(1.5418305348314281, likelihood, 1e-10);
            Assert.AreEqual(0.4048936808991913, a1, 1e-10);
            Assert.AreEqual(0.4656014344844673, a2, 1e-10);
            Assert.AreEqual(1.4232710878429383E-48, a3, 1e-10);

            Assert.IsFalse(double.IsNaN(logLikelihood));
            Assert.IsFalse(double.IsNaN(a1));
            Assert.IsFalse(double.IsNaN(a2));
            Assert.IsFalse(double.IsNaN(a3));


            Assert.AreEqual(2, model.Emissions.Length);
            var state1 = (model.Emissions[0] as NormalDistribution);
            var state2 = (model.Emissions[1] as NormalDistribution);
            Assert.AreEqual(0.16666666666666, state1.Mean, 1e-10);
            Assert.AreEqual(6.11111111111111, state2.Mean, 1e-10);
            Assert.IsFalse(Double.IsNaN(state1.Mean));
            Assert.IsFalse(Double.IsNaN(state2.Mean));

            Assert.AreEqual(0.007499999999999, state1.Variance, 1e-10);
            Assert.AreEqual(0.538611111111111, state2.Variance, 1e-10);
            Assert.IsFalse(Double.IsNaN(state1.Variance));
            Assert.IsFalse(Double.IsNaN(state2.Variance));

            Assert.AreEqual(2, model.Transitions.GetLength(0));
            Assert.AreEqual(2, model.Transitions.GetLength(1));

            var A = Matrix.Exp(model.Transitions);
            Assert.AreEqual(0.090, A[0, 0], 1e-3);
            Assert.AreEqual(0.909, A[0, 1], 1e-3);
            Assert.AreEqual(0.875, A[1, 0], 1e-3);
            Assert.AreEqual(0.125, A[1, 1], 1e-3);

            Assert.IsFalse(A.HasNaN());
        }

        [TestMethod()]
        public void LearnTest8()
        {
            // Create continuous sequences. In the sequence below, there
            // seems to be two states, one for values equal to 1 and another
            // for values equal to 2.
            double[][] sequences = new double[][] 
            {
                new double[] { 1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2 }             
            };

            // Specify a initial normal distribution for the samples.
            var density = new NormalDistribution();

            // Creates a continuous hidden Markov Model with two states organized in a forward
            //  topology and an underlying univariate Normal distribution as probability density.
            var model = new HiddenMarkovModel<NormalDistribution>(new Forward(2), density);

            // Configure the learning algorithms to train the sequence classifier until the
            // difference in the average log-likelihood changes only by as little as 0.0001
            var teacher = new ViterbiLearning<NormalDistribution>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,

                // However, we will need to specify a regularization constant as the
                //  variance of each state will likely be zero (all values are equal)
                FittingOptions = new NormalOptions() { Regularization = double.Epsilon }
            };

            // Fit the model
            double likelihood = teacher.Run(sequences);


            // See the probability of the sequences learned
            double a1 = model.Evaluate(new double[] { 1, 2, 1, 2, 1, 2, 1, 2, 1 }); // exp(a1) = infinity
            double a2 = model.Evaluate(new double[] { 1, 2, 1, 2, 1 });             // exp(a2) = infinity

            // See the probability of an unrelated sequence
            double a3 = model.Evaluate(new double[] { 1, 2, 3, 2, 1, 2, 1 });          // exp(a3) = 0
            double a4 = model.Evaluate(new double[] { 1.1, 2.2, 1.3, 3.2, 4.2, 1.0 }); // exp(a4) = 0


            Assert.AreEqual(double.PositiveInfinity, System.Math.Exp(likelihood));
            Assert.AreEqual(3340.6878090199571, a1);
            Assert.AreEqual(1855.791720669667, a2);
            Assert.AreEqual(0.0, Math.Exp(a3));
            Assert.AreEqual(0.0, Math.Exp(a4));

            Assert.AreEqual(2, model.Emissions.Length);
            var state1 = (model.Emissions[0] as NormalDistribution);
            var state2 = (model.Emissions[1] as NormalDistribution);
            Assert.AreEqual(1.0, state1.Mean, 1e-10);
            Assert.AreEqual(2.0, state2.Mean, 1e-10);
            Assert.IsFalse(Double.IsNaN(state1.Mean));
            Assert.IsFalse(Double.IsNaN(state2.Mean));

            Assert.IsTrue(state1.Variance < 1e-30);
            Assert.IsTrue(state2.Variance < 1e-30);

            var A = Matrix.Exp(model.Transitions);
            Assert.AreEqual(2, A.GetLength(0));
            Assert.AreEqual(2, A.GetLength(1));
            Assert.AreEqual(0.0714285714285714, A[0, 0], 1e-6);
            Assert.AreEqual(0.9285714285714286, A[0, 1], 1e-6);
            Assert.AreEqual(0.9230769230769231, A[1, 0], 1e-6);
            Assert.AreEqual(0.0769230769230769, A[1, 1], 1e-6);
        }

        [TestMethod()]
        public void LearnTest9()
        {

            var observations = new double[][][]
            {
                #region example
                new double[][]
                {
                    new double[] {2.58825719356537, -6.10018078957452, -3.51826652951428,},
                    new double[] {1.5637531876564, -8.92844874836103, -9.09330631370717,},
                    new double[] {2.12242007255554, -14.8117769726059, -9.04211363915664,},
                    new double[] {0.39045587182045, -10.3548189544216, -7.69608701297759,},
                    new double[] {-0.553155690431595, -34.9185135663671, 14.6941023804174,},
                    new double[] {-0.923129916191101, -6.06337512248124, 8.28106954197084,},
                    new double[] {0.478342920541763, -4.93066650122859, 3.1120912556361,},
                },
                new double[][]
                {
                    new double[] {1.89824998378754, -8.21581113387553, -7.88790716806936,},
                    new double[] {2.24453508853912, -10.281886698766, -9.67846789539227,},
                    new double[] {0.946296751499176, -22.0276392511088, -6.52238763834787,},
                    new double[] {-0.251136720180511, -13.3010653290676, 8.47499524273859,},
                    new double[] {-2.35625505447388, -18.1542111199742, 6.25564428645639,},
                    new double[] {0.200483202934265, -5.48215328147925, 5.88811639894938,},
                },
                new double[][]
                {
                    new double[] {2.7240589261055, -3.71720542338046, -3.75092324997593,},
                    new double[] {2.19917744398117, -7.18434871865373, -4.92539999824263,},
                    new double[] {1.40723958611488, -11.5545592998714, -5.14780194932221,},
                    new double[] {1.61909088492393, -12.5262932665595, -6.34366687651826,},
                    new double[] {-2.54745036363602, -8.64924529565274, 4.15127988308386,},
                    new double[] {0.815489888191223, -33.8531051237431, 4.3954106953589,},
                    new double[] {-2.2090271115303, -7.17818258102413, 8.9117419130814,},
                    new double[] {-1.9000232219696, -2.4331659041997, 6.91224717766923,},
                },
                new double[][]
                {
                    new double[] {4.88746017217636, -4.36384651224969, -5.45526891285354,},
                    new double[] {1.07786506414413, -12.9399071692788, -5.88248026843442,},
                    new double[] {2.28888094425201, -15.4017823367163, -9.36490649113217,},
                    new double[] {-1.16468518972397, -35.4200913138333, 5.44735305966353,},
                    new double[] {-1.1483296751976, -13.5454911068913, 7.83577905727326,},
                    new double[] {-2.58188247680664, -1.10149600205281, 10.5928750605715,},
                    new double[] {-0.277529656887054, -6.96828661824016, 4.59381106840823,},
                },
                new double[][]
                {
                    new double[] {3.39118540287018, -2.9173207268871, -5.66795398530988,},
                    new double[] {1.44856870174408, -9.21319243840922, -5.74986260778932,},
                    new double[] {1.45215392112732, -10.3989582187704, -7.06932768129103,},
                    new double[] {0.640938431024551, -15.319525165245, -7.68866476960221,},
                    new double[] {-0.77500119805336, -20.8335910793105, -1.56702420087282,},
                    new double[] {-3.48337143659592, -18.0461677940976, 12.3393172987974,},
                    new double[] {-1.17014795541763, -5.59624373275155, 6.09176828712909,},
                },
                new double[][]
                {
                    new double[] {-3.984335064888, -6.2406475893692, -8.13815178201645,},
                    new double[] {-2.12110131978989, -5.60649378910647, -7.69551693188544,},
                    new double[] {-1.62762850522995, -24.1160212319193, -14.9683354815265,},
                    new double[] {-1.15231424570084, -17.1336790735458, -5.70731951079186,},
                    new double[] {0.00514835119247437, -35.4256585588532, 11.0357975880744,},
                    new double[] {0.247226655483246, -4.87705331087666, 8.47028869639136,},
                    new double[] {-1.28729045391083, -4.4684855254196, 4.45432778840328,},
                },
                new double[][]
                {
                    new double[] {-5.14926165342331, -14.4168633009146, -14.4808205022332,},
                    new double[] {-3.93681302666664, -13.6040611430423, -9.52852874304709,},
                    new double[] {-4.0200162678957, -17.9772444010218, -10.9145425003168,},
                    new double[] {2.99205146729946, -11.3995995445577, 10.0112700536762,},
                    new double[] {-1.80960297584534, -25.9626088707583, 3.84153700324761,},
                    new double[] {-0.47445073723793, -3.15995343875038, 3.81288679772555,},
                },
                new double[][]
                {
                    new double[] {-3.10730338096619, -4.90623566171983, -7.71155001801384,},
                    new double[] {-2.58265435695648, -12.8249488039327, -7.81701695282102,},
                    new double[] {-3.70455086231232, -10.9642675851383, -10.3474496036822,},
                    new double[] {2.34457105398178, -22.575668228196, -4.00681935468317,},
                    new double[] {-0.137023627758026, -22.8846781066673, 6.49448229892285,},
                    new double[] {-1.04487389326096, -10.8106353197974, 6.89123118904132,},
                    new double[] {-0.807777792215347, -6.72485967042486, 6.44026679233423,},
                    new double[] {-0.0864192843437195, -1.82784244477527, 5.21446167464657,},
                },
                new double[][]
                {
                    new double[] {-3.68375554680824, -8.91158395500054, -9.35894038244743,},
                    new double[] {-3.42774018645287, -8.90966793048099, -12.0502934183779,},
                    new double[] {-2.21796408295631, -20.1283824753482, -9.3404551995806,},
                    new double[] {0.275979936122894, -24.8898254667703, -1.95441472953041,},
                    new double[] {2.8757631778717, -25.5929744730134, 15.9213204397452,},
                    new double[] {-0.0532664358615875, -5.41014381829368, 7.0702071664098,},
                    new double[] {-0.523447245359421, -2.21351362388411, 5.47910029515575,},
                },
                new double[][]
                {
                    new double[] {-2.87790596485138, -4.67335526533981, -5.23215633615683,},
                    new double[] {-2.4156779050827, -3.99829080603495, -4.85576151355235,},
                    new double[] {-2.6987336575985, -7.76589206730162, -5.81054787011341,},
                    new double[] {-2.65482440590858, -10.5628263066491, -5.60468502395908,},
                    new double[] {-2.54620611667633, -13.0387387107748, -5.36223367466908,},
                    new double[] {-0.349991768598557, -6.54244110985515, -4.35843018634009,},
                    new double[] {1.43021196126938, -14.1423935327282, 11.3171592025544,},
                    new double[] {-0.248833745718002, -25.6880129237476, 3.6943247495434,},
                    new double[] {-0.191526114940643, -7.40986142342928, 5.01053017361167,},
                    new double[] {0.0262223184108734, -2.32355649224634, 5.02960958030255,},
                },
                new double[][]
                {
                    new double[] {-0.491838902235031, -6.14010393559236, 0.827477332024586,},
                    new double[] {-0.806065648794174, -7.15029676810841, -1.19623376104369,},
                    new double[] {-0.376655906438828, -8.79062775480082, -1.90518908829517,},
                    new double[] {0.0747844576835632, -8.78933441325732, -1.96265207353993,},
                    new double[] {-0.375023484230042, 3.89681155173501, 9.01643231817069,},
                    new double[] {-2.8106614947319, -11.460008093918, 2.27801912994775,},
                    new double[] {8.87353122234344, -36.8569805718597, 6.36432395690119,},
                    new double[] {2.17160433530808, -6.57312981892095, 6.99683358454453,},
                },
                new double[][]
                {
                    new double[] {-2.59969010949135, -3.67992698430228, 1.09594294144671,},
                    new double[] {-1.09673067927361, -5.84256216502719, -0.576662929456575,},
                    new double[] {-1.31642892956734, -7.75851355520771, -2.38379618379558,},
                    new double[] {-0.119869410991669, -8.5749576027529, -1.84393133510667,},
                    new double[] {1.6157403588295, -8.50491836461337, 1.75083250596366,},
                    new double[] {1.66225507855415, -26.4882911957686, 1.98153904369032,},
                    new double[] {2.55657434463501, -10.5098938623168, 11.632377227365,},
                    new double[] {1.91832333803177, -9.98753621777953, 7.38483383044985,},
                    new double[] {2.16058492660522, -2.7784029746222, 7.8378896386686,},
                },
#endregion
            };

            var density = new MultivariateNormalDistribution(3);
            var model = new HiddenMarkovModel<MultivariateNormalDistribution>(new Forward(5), density);

            var learning = new ViterbiLearning<MultivariateNormalDistribution>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,
                FittingOptions = new NormalOptions() { Regularization = 0.0001 }
            };

            double logLikelihood = learning.Run(observations);

            Assert.IsFalse(Double.IsNaN(logLikelihood));

            foreach (double value in model.Transitions)
                Assert.IsFalse(Double.IsNaN(value));

            foreach (double value in model.Probabilities)
                Assert.IsFalse(Double.IsNaN(value));
        }

    }
}
