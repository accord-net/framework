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
    using Accord.DataSets;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Math.Distances;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;

    [TestFixture]
    public class TensorTest
    {

        [Test]
        public void learn()
        {
            #region doc_learn
            // Let's try to obtain a classifier for an 
            // example 2D binary classification dataset:
            var iris = new DataSets.YinYang();
            double[][] inputs = iris.Instances;
            bool[] outputs = iris.ClassLabels;

            // Create a learning algorithm with the tensor product kernel
            var smo = new SequentialMinimalOptimization<Tensor<IKernel>>()
            {
                // Combine multiple kernels using the tensor product
                Kernel = new Tensor<IKernel>(new Linear(), new Gaussian(0.01), new Gaussian(3.6))
            };

            // Use it to learn a new s.v. machine
            var svm = smo.Learn(inputs, outputs);

            // Now we can compute predicted values
            bool[] predicted = svm.Decide(inputs);

            // And check how far we are from the expected values
            double error = new ZeroOneLoss(outputs).Loss(predicted); // error will be 0.0
            #endregion

            Assert.AreEqual(0.0, error, 1e-6);
        }

    }
}
