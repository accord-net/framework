// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using System;
    using Accord.Math.Differentiation;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Math;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Kernels;

    [TestClass()]
    public class ProbabilisticDualCoordinateDescentTest
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
            var dataset = SequentialMinimalOptimizationTest.yinyang;

            double[][] inputs = dataset.Submatrix(null, 0, 1).ToArray();
            int[] labels = dataset.GetColumn(2).ToInt32();

            Accord.Math.Tools.SetupGenerator(0);

            var svm = new SupportVectorMachine(inputs: 2);
            var teacher = new ProbabilisticDualCoordinateDescent(svm, inputs, labels);

            teacher.Tolerance = 1e-10;
            teacher.UseComplexityHeuristic = true;

            double error = teacher.Run();

            double[] weights = svm.ToWeights();

            Assert.AreEqual(0.13, error);
            Assert.AreEqual(3, weights.Length);
            Assert.AreEqual(-0.52913278486359605, weights[0], 1e-4);
            Assert.AreEqual(-1.6426069611746976, weights[1], 1e-4);
            Assert.AreEqual(-0.77766953652287762, weights[2], 1e-4);

            Assert.AreEqual(svm.Threshold, weights[0]);
        }

    }
}
