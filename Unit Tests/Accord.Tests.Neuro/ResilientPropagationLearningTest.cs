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

namespace Accord.Tests.Neuro
{
    using System;
    using Accord.Neuro.Learning;
    using AForge;
    using AForge.Neuro;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class ResilientBackPropagationLearningTest
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



#if NET35
        [TestMethod()]
        public void RunEpochTest1()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[][] input = 
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            double[][] output =
            {
                new double[] { -1 },
                new double[] {  1 },
                new double[] {  1 },
                new double[] { -1 }
            };

            Neuron.RandGenerator = new ThreadSafeRandom(0);
            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 2, 1);

            var teacher = new ParallelResilientBackpropagationLearning(network);

            double error = 1.0;
            while (error > 1e-5)
                error = teacher.RunEpoch(input, output);

            for (int i = 0; i < input.Length; i++)
            {
                double actual = network.Compute(input[i])[0];
                double expected = output[i][0];

                Assert.AreEqual(expected, actual, 0.01);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }
#endif


    }
}
