// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
// cesarsouza at gmail.com
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
// 

namespace Accord.Tests.MachineLearning.GPL
{
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Kernels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Audio.Generators;
    using System;
    using Accord.Math;
    using Accord.Audio;
    using Accord.Statistics.Filters;
    using Accord.Audio.Windows;

    [TestClass]
    public class TemporalRegressionTest
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
        public void TrainTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // Generate 1000 samples from a sine signal
            Signal signal = new SineGenerator().Generate(1000);

            // Convert to a time-series sequence
            double[] sequence = signal.ToDouble();

            // Create a time-series segmentation algorithm that will
            // create a database of samples in which the previous 5
            // samples will be used to predict the next one:
            //
            var segmenter = new Windowing(windowSize: 5);


            double[] outputs;
            double[][] inputs;

            // Extract the inputs and outputs from the signal
            inputs = segmenter.Apply(sequence, out outputs);


            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            var machine = new KernelSupportVectorMachine(new Polynomial(2), inputs: 5);

            // Create the sequential minimal optimization teacher
            var learn = new SequentialMinimalOptimizationRegression(machine, inputs, outputs)
            {
                Complexity = 100
            };

            // Run the learning algorithm
            double error = learn.Run();

            // Check for correct answers
            double[] answers = new double[inputs.Length];
            for (int i = 0; i < answers.Length; i++)
                answers[i] = machine.Compute(inputs[i]);

            for (int i = 0; i < outputs.Length; i++)
                Assert.AreEqual(outputs[i], answers[i], 1e-2);
        }

    }
}
