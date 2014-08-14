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
    using Accord.MachineLearning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.MachineLearning.VectorMachines;
    using Accord.Statistics.Kernels;
    using Accord.MachineLearning.VectorMachines.Learning;
    using System;

    [TestClass()]
    public class GridsearchTest
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
        public void GridsearchConstructorTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // Example binary data
            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] xor = // xor labels
            {
                -1, 1, 1, -1
            };

            // Declare the parameters and ranges to be searched
            GridSearchRange[] ranges = 
            {
                new GridSearchRange("complexity", new double[] { 0.00000001, 5.20, 0.30, 0.50 } ),
                new GridSearchRange("degree",     new double[] { 1, 10, 2, 3, 4, 5 } ),
                new GridSearchRange("constant",   new double[] { 0, 1, 2 } )
            };


            // Instantiate a new Grid Search algorithm for Kernel Support Vector Machines
            var gridsearch = new GridSearch<KernelSupportVectorMachine>(ranges);

            // Set the fitting function for the algorithm
            gridsearch.Fitting = delegate(GridSearchParameterCollection parameters, out double error)
            {
                // The parameters to be tried will be passed as a function parameter.
                int degree = (int)parameters["degree"].Value;
                double constant = parameters["constant"].Value;
                double complexity = parameters["complexity"].Value;

                // Use the parameters to build the SVM model
                Polynomial kernel = new Polynomial(degree, constant);
                KernelSupportVectorMachine ksvm = new KernelSupportVectorMachine(kernel, 2);

                // Create a new learning algorithm for SVMs
                SequentialMinimalOptimization smo = new SequentialMinimalOptimization(ksvm, inputs, xor);
                smo.Complexity = complexity;

                // Measure the model performance to return as an out parameter
                error = smo.Run();

                return ksvm; // Return the current model
            };


            // Declare some out variables to pass to the grid search algorithm
            GridSearchParameterCollection bestParameters; double minError;

            // Compute the grid search to find the best Support Vector Machine
            KernelSupportVectorMachine bestModel = gridsearch.Compute(out bestParameters, out minError);


            // A linear kernel can't solve the xor problem.
            Assert.AreNotEqual((int)bestParameters["degree"].Value, 1);

            // The minimum error should be zero because the problem is well-known.
            Assert.AreEqual(minError, 0.0);


            Assert.IsNotNull(bestModel);
            Assert.IsNotNull(bestParameters);
            Assert.AreEqual(bestParameters.Count, 3);
        }

        [TestMethod()]
        public void GridsearchConstructorTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // Example binary data
            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] xor = // xor labels
            {
                -1, 1, 1, -1
            };

            // Declare the parameters and ranges to be searched
            GridSearchRange[] ranges = 
            {
                new GridSearchRange("complexity", new double[] { 0.00000001, 5.20, 0.30, 1000000, 0.50 } ),
            };


            // Instantiate a new Grid Search algorithm for Kernel Support Vector Machines
            var gridsearch = new GridSearch<SupportVectorMachine>(ranges);

            // Set the fitting function for the algorithm
            gridsearch.Fitting = delegate(GridSearchParameterCollection parameters, out double error)
            {
                // The parameters to be tried will be passed as a function parameter.
                double complexity = parameters["complexity"].Value;

                // Use the parameters to build the SVM model
                SupportVectorMachine svm = new SupportVectorMachine(2);

                // Create a new learning algorithm for SVMs
                SequentialMinimalOptimization smo = new SequentialMinimalOptimization(svm, inputs, xor);
                smo.Complexity = complexity;

                // Measure the model performance to return as an out parameter
                error = smo.Run();

                return svm; // Return the current model
            };


            {
                // Declare some out variables to pass to the grid search algorithm
                GridSearchParameterCollection bestParameters; double minError;

                // Compute the grid search to find the best Support Vector Machine
                SupportVectorMachine bestModel = gridsearch.Compute(out bestParameters, out minError);


                // The minimum error should be zero because the problem is well-known.
                Assert.AreEqual(minError, 0.5);


                Assert.IsNotNull(bestModel);
                Assert.IsNotNull(bestParameters);
                Assert.AreEqual(bestParameters.Count, 1);
            }

            {
                // Compute the grid search to find the best Support Vector Machine
                var result = gridsearch.Compute();


                // The minimum error should be zero because the problem is well-known.
                Assert.AreEqual(result.Error, 0.5);

                Assert.IsNotNull(result.Model);
                Assert.AreEqual(5, result.Errors.Length);
                Assert.AreEqual(5, result.Models.Length);
            }
        }
    }
}
