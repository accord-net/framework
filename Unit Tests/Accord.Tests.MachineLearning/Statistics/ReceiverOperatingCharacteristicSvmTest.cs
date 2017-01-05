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

namespace Accord.Tests.Statistics
{
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Analysis;
    using NUnit.Framework;

    [TestFixture]
    public class ReceiverOperatingCharacteristicSvmTest
    {

        [Test]
        public void ReceiverOperatingCharacteristicConstructorTest3()
        {
            // This example shows how to measure the accuracy of a 
            // binary classifier using a ROC curve. For this example,
            // we will be creating a Support Vector Machine trained
            // on the following instances:

            double[][] inputs =
            {
                // Those are from class -1
                new double[] { 2, 4, 0 },
                new double[] { 5, 5, 1 },
                new double[] { 4, 5, 0 },
                new double[] { 2, 5, 5 },
                new double[] { 4, 5, 1 },
                new double[] { 4, 5, 0 },
                new double[] { 6, 2, 0 },
                new double[] { 4, 1, 0 },

                // Those are from class +1
                new double[] { 1, 4, 5 },
                new double[] { 7, 5, 1 },
                new double[] { 2, 6, 0 },
                new double[] { 7, 4, 7 },
                new double[] { 4, 5, 0 },
                new double[] { 6, 2, 9 },
                new double[] { 4, 1, 6 },
                new double[] { 7, 2, 9 },
            };

            int[] outputs =
            {
                -1, -1, -1, -1, -1, -1, -1, -1, // first eight from class -1
                +1, +1, +1, +1, +1, +1, +1, +1  // last  eight from class +1
            };

            // Create a linear Support Vector Machine with 3 inputs
            var machine = new SupportVectorMachine(inputs: 3);

            // Create the sequential minimal optimization teacher
            var learn = new SequentialMinimalOptimization(machine, inputs, outputs)
            {
                Complexity = 1
            };

            // Run the learning algorithm
            double error = learn.Run();

            // Extract the input labels predicted by the machine
            double[] predicted = new double[inputs.Length];
            for (int i = 0; i < predicted.Length; i++)
                predicted[i] = machine.Score(inputs[i]);


            // Create a new ROC curve to assess the performance of the model
            var roc = new ReceiverOperatingCharacteristic(outputs, predicted);

            roc.Compute(100); // Compute a ROC curve with 100 points
            /*
                        // Generate a connected scatter plot for the ROC curve and show it on-screen
                        ScatterplotBox.Show(roc.GetScatterplot(includeRandom: true), nonBlocking: true)

                            .SetSymbolSize(0)      // do not display data points
                            .SetLinesVisible(true) // show lines connecting points
                            .SetScaleTight(true)   // tighten the scale to points
                            .WaitForClose();
            */

            Assert.AreEqual(0.25, error);
            Assert.AreEqual(0.78125, roc.Area);
            // Assert.AreEqual(0.1174774, roc.StandardError, 1e-6); HanleyMcNeil
            // Assert.AreEqual(0.11958120746409709, roc.StandardError, 1e-6);
            Assert.AreEqual(0.132845321574701, roc.StandardError, 1e-6);
        }

    }
}
