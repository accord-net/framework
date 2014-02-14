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

    [TestClass()]
    public class MaximumLikelihoodLearningTest
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
        public void RunTest()
        {
            // Example from
            // http://www.cs.columbia.edu/4761/notes07/chapter4.3-HMM.pdf

            

            int[][] observations = 
            {
                new int[] { 0,0,0,1,0,0 }, 
                new int[] { 1,0,0,1,0,0 },
                new int[] { 0,0,1,0,0,0 },
                new int[] { 0,0,0,0,1,0 },
                new int[] { 1,0,0,0,1,0 },
                new int[] { 0,0,0,1,1,0 },
                new int[] { 1,0,0,0,0,0 },
                new int[] { 1,0,1,0,0,0 },
            };

            int[][] paths =
            {
                new int[] { 0,0,1,0,1,0 },
                new int[] { 1,0,1,0,1,0 },
                new int[] { 1,0,0,1,1,0 },
                new int[] { 1,0,1,1,1,0 },
                new int[] { 1,0,0,1,0,1 },
                new int[] { 0,0,1,0,0,1 },
                new int[] { 0,0,1,1,0,1 },
                new int[] { 0,1,1,1,0,0 },
            };


            HiddenMarkovModel model = new HiddenMarkovModel(states: 2, symbols: 2);

            MaximumLikelihoodLearning target = new MaximumLikelihoodLearning(model);
            target.UseLaplaceRule = false;

            double logLikelihood = target.Run(observations, paths);

            var pi = Matrix.Exp(model.Probabilities);
            var A = Matrix.Exp(model.Transitions);
            var B = Matrix.Exp(model.Emissions);

            Assert.AreEqual(0.5, pi[0]);
            Assert.AreEqual(0.5, pi[1]);

            Assert.AreEqual(7 / 20.0, A[0, 0], 1e-5);
            Assert.AreEqual(13 / 20.0, A[0, 1], 1e-5);
            Assert.AreEqual(14 / 20.0, A[1, 0], 1e-5);
            Assert.AreEqual(6 / 20.0, A[1, 1], 1e-5);

            Assert.AreEqual(17 / 25.0, B[0, 0]);
            Assert.AreEqual(8 / 25.0, B[0, 1]);
            Assert.AreEqual(19 / 23.0, B[1, 0]);
            Assert.AreEqual(4 / 23.0, B[1, 1]);

            Assert.AreEqual(-1.1472359046136624, logLikelihood);
        }
    }
}
