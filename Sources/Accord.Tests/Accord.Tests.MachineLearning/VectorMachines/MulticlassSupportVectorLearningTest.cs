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

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class MulticlassSupportVectorLearningTest
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
            Accord.Math.Tools.SetupGenerator(0);

            // Sample data
            //   The following is a simple auto association function
            //   in which each input correspond to its own class. This
            //   problem should be easily solved using a Linear kernel.

            // Sample input data
            double[][] inputs =
            {
                new double[] { 0 },
                new double[] { 3 },
                new double[] { 1 },
                new double[] { 2 },
            };

            // Output for each of the inputs
            int[] outputs = { 0, 3, 1, 2 };


            // Create a new Linear kernel
            IKernel kernel = new Linear();

            // Create a new Multi-class Support Vector Machine for one input,
            //  using the linear kernel and four disjoint classes.
            var machine = new MulticlassSupportVectorMachine(1, kernel, 4);

            // Create the Multi-class learning algorithm for the machine
            var teacher = new MulticlassSupportVectorLearning(machine, inputs, outputs);

            // Configure the learning algorithm to use SMO to train the
            //  underlying SVMs in each of the binary class subproblems.
            teacher.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs);

            // Run the learning algorithm
            double error = teacher.Run();

            Assert.AreEqual(0, error);
            Assert.AreEqual(0, machine.Compute(inputs[0]));
            Assert.AreEqual(3, machine.Compute(inputs[1]));
            Assert.AreEqual(1, machine.Compute(inputs[2]));
            Assert.AreEqual(2, machine.Compute(inputs[3]));

        }

        [TestMethod()]
        public void RunTest2()
        {

            double[][] inputs =
            {
                new double[] { 0, 1, 1, 0 }, // 0
                new double[] { 0, 1, 0, 0 }, // 0
                new double[] { 0, 0, 1, 0 }, // 0
                new double[] { 0, 1, 1, 0 }, // 0
                new double[] { 0, 1, 0, 0 }, // 0
                new double[] { 1, 0, 0, 0 }, // 1
                new double[] { 1, 0, 0, 0 }, // 1
                new double[] { 1, 0, 0, 1 }, // 1
                new double[] { 0, 0, 0, 1 }, // 1
                new double[] { 0, 0, 0, 1 }, // 1
                new double[] { 1, 1, 1, 1 }, // 2
                new double[] { 1, 0, 1, 1 }, // 2
                new double[] { 1, 1, 0, 1 }, // 2
                new double[] { 0, 1, 1, 1 }, // 2
                new double[] { 1, 1, 1, 1 }, // 2
            };

            int[] outputs =
            {
                0, 0, 0, 0, 0,
                1, 1, 1, 1, 1,
                2, 2, 2, 2, 2,
            };

            IKernel kernel = new Linear();
            MulticlassSupportVectorMachine machine = new MulticlassSupportVectorMachine(4, kernel, 3);
            MulticlassSupportVectorLearning target = new MulticlassSupportVectorLearning(machine, inputs, outputs);

            target.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs);


            double actual = target.Run();
            double expected = 0;

            Assert.AreEqual(expected, actual);

            for (int i = 0; i < inputs.Length; i++)
            {
                actual = machine.Compute(inputs[i]);
                expected = outputs[i];
                Assert.AreEqual(expected, actual);
            }

        }

        [TestMethod()]
        public void RunTest3()
        {

            double[][] inputs =
            {
                // Tickets with the following structure should be assigned to location 0
                new double[] { 1, 4, 2, 0, 1 }, // should be assigned to location 0
                new double[] { 1, 3, 2, 0, 1 }, // should be assigned to location 0

                // Tickets with the following structure should be assigned to location 1
                new double[] { 3, 0, 1, 1, 1 }, // should be assigned to location 1
                new double[] { 3, 0, 1, 0, 1 }, // should be assigned to location 1

                // Tickets with the following structure should be assigned to location 2
                new double[] { 0, 5, 5, 5, 5 }, // should be assigned to location 2
                new double[] { 1, 5, 5, 5, 5 }, // should be assigned to location 2

                // Tickets with the following structure should be assigned to location 3
                new double[] { 1, 0, 0, 0, 0 }, // should be assigned to location 3
                new double[] { 1, 0, 0, 0, 0 }, // should be assigned to location 3
            };

            int[] outputs =
            {
                0, 0, // Those are the locations for the first two vectors above
                1, 1, // Those are the locations for the next two vectors above
                2, 2, // Those are the locations for the next two vectors above
                3, 3, // Those are the locations for the last two vectors above
            };

            // Since this is a simplification, a linear machine will suffice:
            IKernel kernel = new Linear();

            // Create the machine for feature vectors of length 5, for 4 possible locations
            MulticlassSupportVectorMachine machine = new MulticlassSupportVectorMachine(5, kernel, 4);

            // Create a new learning algorithm to train the machine
            MulticlassSupportVectorLearning target = new MulticlassSupportVectorLearning(machine, inputs, outputs);

            // Use the standard SMO algorithm
            target.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs);

            // Train the machines
            double actual = target.Run();


            // Compute the answer for all training samples
            for (int i = 0; i < inputs.Length; i++)
            {
                double[] answersWeights;

                double answer = machine.Compute(inputs[i], MulticlassComputeMethod.Voting, out answersWeights);

                // Assert it has been classified correctly
                Assert.AreEqual(outputs[i], answer);

                // Assert the most probable answer is indeed the correct one
                int imax; Matrix.Max(answersWeights, out imax);
                Assert.AreEqual(answer, imax);
            }

        }

    }
}
