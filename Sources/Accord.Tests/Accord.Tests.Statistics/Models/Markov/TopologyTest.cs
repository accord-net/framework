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

using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Math;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;
namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for HiddenMarkovModelTest and is intended
    ///to contain all HiddenMarkovModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TopologyTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        public void UniformTest()
        {
            // Create a new Ergodic hidden Markov model with three
            //   fully-connected states and four sequence symbols.
            var model = new HiddenMarkovModel(new Ergodic(3), 4);

            var expected = new double[,] 
            {
                { 0.33, 0.33, 0.33 },
                { 0.33, 0.33, 0.33 },
                { 0.33, 0.33, 0.33 },
            };

            var A = Matrix.Exp(model.Transitions);

            Assert.AreEqual(model.States, 3);
            Assert.IsTrue(A.IsEqual(expected, 0.01));
        }

        [TestMethod()]
        public void ForwardTest()
        {
            // Create a new Forward-only hidden Markov model with
            // three forward-only states and four sequence symbols.
            var model = new HiddenMarkovModel(new Forward(3), 4);

            Assert.AreEqual(model.States, 3);
            var actual = Matrix.Exp(model.Transitions);
            var expected = new double[,] 
            {
                { 0.33, 0.33, 0.33 },
                { 0.00, 0.50, 0.50 },
                { 0.00, 0.00, 1.00 },
            };

            Assert.IsTrue(actual.IsEqual(expected, 0.01));
        }

        [TestMethod()]
        public void ForwardTest2()
        {
            var topology = new Forward(3, 2);

            Assert.AreEqual(topology.States, 3);
            Assert.AreEqual(topology.Deepness, 2);

            double[,] actual;
            double[] pi;
            int states = topology.Create(false, out actual, out pi);
            var expected = new double[,] 
            {
                { 0.50, 0.50, 0.00 },
                { 0.00, 0.50, 0.50 },
                { 0.00, 0.00, 1.00 },
            };

            Assert.IsTrue(actual.IsEqual(expected, 0.01));
            Assert.AreEqual(states, 3);
        }

        [TestMethod()]
        public void ForwardTest3()
        {
            var topology = new Forward(states: 3, deepness: 2);

            double[,] actualA;
            double[] actualPi;

            double[,] expectedA;
            double[] expectedPi;

            int actualStates = topology.Create(true, out actualA, out actualPi);
            int expectedStates = topology.Create(false, out expectedA, out expectedPi);

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Assert.AreEqual(actualA[i, j], System.Math.Log(expectedA[i, j]));

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Assert.AreEqual(actualPi[i], System.Math.Log(expectedPi[i]));

            Assert.AreEqual(actualStates, expectedStates);
            Assert.AreEqual(actualStates, 3);
        }
    }

}