// Accord Unit Tests
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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
    using Accord.Statistics.Running;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Math;
    using Accord.Statistics.Models.Markov.Topology;
    
    [TestClass()]
    public class RunningMarkovClassifierTest
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion



        [TestMethod()]
        public void PushTest()
        {
            double[][] sequences;
            
            var classifier = createClassifier(out sequences);
            var running = new RunningMarkovClassifier<NormalDistribution>(classifier);

            for (int i = 0; i < sequences.Length; i++)
            {
                double[] sequence = sequences[i];

                running.Clear();
                for (int j = 0; j < sequence.Length; j++)
                    running.Push(sequence[j]);

                double actualLikelihood;
                int actual = running.Classification;
                actualLikelihood = Math.Exp(running.Responses[actual]) / running.Responses.Exp().Sum();

                double expectedLikelihood;
                int expected = classifier.Compute(sequence, out expectedLikelihood);

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expectedLikelihood, actualLikelihood, 1e-8);
            }
        }

        private static HiddenMarkovClassifier<NormalDistribution> createClassifier(out double[][] sequences)
        {
            sequences = new double[][] 
            {
                new double[] { 0,1,2,3,4 }, 
                new double[] { 4,3,2,1,0 }, 
            };

            int[] labels = { 0, 1 };

            NormalDistribution density = new NormalDistribution();
            HiddenMarkovClassifier<NormalDistribution>  classifier =
                new HiddenMarkovClassifier<NormalDistribution>(2, new Ergodic(2), density);

            var teacher = new HiddenMarkovClassifierLearning<NormalDistribution>(classifier,

                modelIndex => new BaumWelchLearning<NormalDistribution>(classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0
                }
            );

            teacher.Run(sequences, labels);

            return classifier;
        }

    }
}
