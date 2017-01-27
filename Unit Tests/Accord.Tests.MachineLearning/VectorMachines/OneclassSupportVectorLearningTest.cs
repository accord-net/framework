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

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Kernels;
    using Math.Optimization.Losses;
    using NUnit.Framework;

    [TestFixture]
    public class OneclassSupportVectorLearningTest
    {

        [Test]
        public void RunTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            var dist = NormalDistribution.Standard;

            double[] x =
            {
                +1.0312479734420776,
                +0.99444115161895752,
                +0.21835240721702576,
                +0.47197291254997253,
                +0.68701112270355225,
                -0.58556461334228516,
                -0.64154046773910522,
                -0.66485315561294556,
                +0.37940266728401184,
                -0.61046308279037476
            };

            double[][] inputs = Jagged.ColumnVector(x);

            IKernel kernel = new Linear();

            var machine = new KernelSupportVectorMachine(kernel, inputs: 1);

            var teacher = new OneclassSupportVectorLearning(machine, inputs)
            {
                Nu = 0.1
            };

            // Run the learning algorithm
            double error = teacher.Run();

            Assert.AreEqual(2, machine.Weights.Length);
            Assert.AreEqual(0.39198910030993617, machine.Weights[0]);
            Assert.AreEqual(0.60801089969006383, machine.Weights[1]);
            Assert.AreEqual(inputs[0][0], machine.SupportVectors[0][0]);
            Assert.AreEqual(inputs[7][0], machine.SupportVectors[1][0]);
            Assert.AreEqual(0.0, error, 1e-10);
        }

        [Test]
        public void learn_test()
        {
            #region doc_learn
            // Ensure that results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

            // Generate some data to be learned
            double[][] inputs =
            {
                new double[] { +1.0312479734420776  },
                new double[] { +0.99444115161895752 },
                new double[] { +0.21835240721702576 },
                new double[] { +0.47197291254997253 },
                new double[] { +0.68701112270355225 },
                new double[] { -0.58556461334228516 },
                new double[] { -0.64154046773910522 },
                new double[] { -0.66485315561294556 },
                new double[] { +0.37940266728401184 },
                new double[] { -0.61046308279037476 }
            };


            // Create a new One-class SVM learning algorithm
            var teacher = new OneclassSupportVectorLearning<Linear>()
            {
                Kernel = new Linear(), // or, for example, 'new Gaussian(0.9)'
                Nu = 0.1
            };

            // Learn a support vector machine
            var svm = teacher.Learn(inputs);

            // Test the machine
            double[] prediction = svm.Score(inputs);

            // Compute the log-likelihood of the answer
            double ll = new LogLikelihoodLoss().Loss(prediction);
            #endregion

            Assert.AreEqual(-1.6653345369377348E-16, ll, 1e-10);
            Assert.AreEqual(2, svm.Weights.Length);
            Assert.AreEqual(0.39198910030993617, svm.Weights[0], 1e-10);
            Assert.AreEqual(0.60801089969006383, svm.Weights[1], 1e-10);
            Assert.AreEqual(inputs[0][0], svm.SupportVectors[0][0], 1e-10);
            Assert.AreEqual(inputs[7][0], svm.SupportVectors[1][0], 1e-10);

        }

    }
}
