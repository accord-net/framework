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
    using Accord.MachineLearning;
    using NUnit.Framework;
    using Accord.MachineLearning.VectorMachines;
    using Accord.Statistics.Kernels;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math.Optimization.Losses;
    using System;
    using Accord.MachineLearning.Performance;
    using System.Threading;
    using System.Collections.Generic;
    using Accord.DataSets;
    using Accord.MachineLearning.DecisionTrees.Learning;
    using Accord.MachineLearning.DecisionTrees;

#if NET35
    using CancellationToken = Accord.Compat.CancellationToken;
#endif

    [TestFixture]
    public class GridsearchTest
    {

        [Test]
        public void GridsearchConstructorTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

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

#if DEBUG
            gridsearch.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif

            // Set the fitting function for the algorithm
            gridsearch.Fitting = delegate (GridSearchParameterCollection parameters, out double error)
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
            Assert.AreEqual(1, bestParameters["degree"].Value);
            Assert.AreEqual(1, bestParameters["constant"].Value);
            Assert.AreEqual(1e-8, bestParameters["complexity"].Value);

            // The minimum error should be zero because the problem is well-known.
            Assert.AreEqual(minError, 0.0);


            Assert.IsNotNull(bestModel);
            Assert.IsNotNull(bestParameters);
            Assert.AreEqual(bestParameters.Count, 3);
        }

        [Test]
        public void GridsearchConstructorTest2()
        {
            Accord.Math.Random.Generator.Seed = 0;

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

            gridsearch.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Set the fitting function for the algorithm
            gridsearch.Fitting = delegate (GridSearchParameterCollection parameters, out double error)
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



        [Test]
        public void learn_test()
        {
            #region doc_learn
            // Ensure results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

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

            // Instantiate a new Grid Search algorithm for Kernel Support Vector Machines
            var gridsearch = new GridSearch<SupportVectorMachine<Polynomial>, double[], int>()
            {
                // Here we can specify the range of the parameters to be included in the search
                ParameterRanges = new GridSearchRangeCollection()
                {
                    new GridSearchRange("complexity", new double[] { 0.00000001, 5.20, 0.30, 0.50 } ),
                    new GridSearchRange("degree",     new double[] { 1, 10, 2, 3, 4, 5 } ),
                    new GridSearchRange("constant",   new double[] { 0, 1, 2 } )
                },

                // Indicate how learning algorithms for the models should be created
                Learner = (p) => new SequentialMinimalOptimization<Polynomial>
                {
                    Complexity = p["complexity"],
                    Kernel = new Polynomial((int)p["degree"], p["constant"])
                },

                // Define how the performance of the models should be measured
                Loss = (actual, expected, m) => new ZeroOneLoss(expected).Loss(actual)
            };

            // If needed, control the degree of CPU parallelization
            gridsearch.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Search for the best model parameters
            var result = gridsearch.Learn(inputs, xor);

            // Get the best SVM found during the parameter search
            SupportVectorMachine<Polynomial> svm = result.BestModel;

            // Get an estimate for its error:
            double bestError = result.BestModelError;

            // Get the best values found for the model parameters:
            double bestC = result.BestParameters["complexity"].Value;
            double bestDegree = result.BestParameters["degree"].Value;
            double bestConstant = result.BestParameters["constant"].Value;
            #endregion

            Assert.IsNotNull(svm);
            Assert.AreEqual(1e-8, bestC, 1e-10);
            Assert.AreEqual(0, bestError, 1e-8);
            Assert.AreEqual(1, bestDegree, 1e-8);
            Assert.AreEqual(1, bestConstant, 1e-8);

            Assert.AreEqual(1, svm.Kernel.Degree);
            Assert.AreEqual(1, svm.Kernel.Constant);
        }


        [Test]
        public void create_test()
        {
            #region doc_create
            // Ensure results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

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

            // Instantiate a new Grid Search algorithm for Kernel Support Vector Machines
            var gridsearch = GridSearch<double[], int>.Create(

                ranges: new GridSearchRange[]
                {
                    new GridSearchRange("complexity", new double[] { 0.00000001, 5.20, 0.30, 0.50 } ),
                    new GridSearchRange("degree",     new double[] { 1, 10, 2, 3, 4, 5 } ),
                    new GridSearchRange("constant",   new double[] { 0, 1, 2 } )
                },

                learner: (p) => new SequentialMinimalOptimization<Polynomial>
                {
                    Complexity = p["complexity"],
                    Kernel = new Polynomial((int)p["degree"].Value, p["constant"])
                },

                // Define how the model should be learned, if needed
                fit: (teacher, x, y, w) => teacher.Learn(x, y, w),

                // Define how the performance of the models should be measured
                loss: (actual, expected, m) => new ZeroOneLoss(expected).Loss(actual)
            );

            // If needed, control the degree of CPU parallelization
            gridsearch.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Search for the best model parameters
            var result = gridsearch.Learn(inputs, xor);

            // Get the best SVM generated during the search
            SupportVectorMachine<Polynomial> svm = result.BestModel;

            // Get an estimate for its error:
            double bestError = result.BestModelError;

            // Get the best values for its parameters:
            double bestC = result.BestParameters["complexity"].Value;
            double bestDegree = result.BestParameters["degree"].Value;
            double bestConstant = result.BestParameters["constant"].Value;
            #endregion

            Assert.IsNotNull(svm);
            Assert.AreEqual(1e-8, bestC, 1e-10);
            Assert.AreEqual(0, bestError, 1e-8);
            Assert.AreEqual(1, bestDegree, 1e-8);
            Assert.AreEqual(1, bestConstant, 1e-8);
        }

#if !NET35
        [Test]
        public void learn_test_strongly_typed()
        {
            #region doc_learn_strongly_typed
            // Ensure results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

            // This is a sample code showing how to use Grid-Search in combination with 
            // Cross-Validation  to assess the performance of Support Vector Machines.

            // Consider the example binary data. We will be trying to learn a XOR 
            // problem and see how well does SVMs perform on this data.

            double[][] inputs =
            {
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
            };

            int[] xor = // result of xor for the sample input data
            {
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
            };

            // Create a new Grid-Search with Cross-Validation algorithm. Even though the
            // generic, strongly-typed approach used accross the framework is most of the
            // time easier to handle, meta-algorithms such as grid-search can be a bit hard
            // to setup. For this reason. the framework offers a specialized method for it:
            var gridsearch = GridSearch<double[], int>.Create(

                // Here we can specify the range of the parameters to be included in the search
                ranges: new
                {
                    Kernel = GridSearch.Values<IKernel>(new Linear(), new ChiSquare(), new Gaussian(), new Sigmoid()),
                    Complexity = GridSearch.Values(0.00000001, 5.20, 0.30, 0.50),
                    Tolerance = GridSearch.Range(1e-10, 1.0, stepSize: 0.05)
                },

                // Indicate how learning algorithms for the models should be created
                learner: (p) => new SequentialMinimalOptimization<IKernel>
                {
                    Complexity = p.Complexity,
                    Kernel = p.Kernel.Value,
                    Tolerance = p.Tolerance
                },

                // Define how the model should be learned, if needed
                fit: (teacher, x, y, w) => teacher.Learn(x, y, w),

                // Define how the performance of the models should be measured
                loss: (actual, expected, m) => new ZeroOneLoss(expected).Loss(actual)
            );

            // If needed, control the degree of CPU parallelization
            gridsearch.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Search for the best model parameters
            var result = gridsearch.Learn(inputs, xor);

            // Get the best SVM:
            SupportVectorMachine<IKernel> svm = result.BestModel;

            // Estimate its error:
            double bestError = result.BestModelError;

            // Get the best values for the parameters:
            double bestC = result.BestParameters.Complexity;
            double bestTolerance = result.BestParameters.Tolerance;
            IKernel bestKernel = result.BestParameters.Kernel.Value;
            #endregion

            Assert.IsNotNull(svm);
            Assert.AreEqual(1e-8, bestC, 1e-10);
            Assert.AreEqual(0, bestError, 1e-8);
            Assert.AreEqual(0, bestTolerance, 1e-8);
            Assert.AreEqual(typeof(Gaussian), bestKernel.GetType());
        }
#endif

        [Test]
        public void cross_validation_test()
        {
            #region doc_learn_cv
            // Ensure results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

            // This is a sample code showing how to use Grid-Search in combination with 
            // Cross-Validation  to assess the performance of Support Vector Machines.

            // Consider the example binary data. We will be trying to learn a XOR 
            // problem and see how well does SVMs perform on this data.

            double[][] inputs =
            {
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
            };

            int[] xor = // result of xor for the sample input data
            {
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
            };

            // Create a new Grid-Search with Cross-Validation algorithm. Even though the
            // generic, strongly-typed approach used accross the framework is most of the
            // time easier to handle, combining those both methods in a single call can be
            // difficult. For this reason. the framework offers a specialized method for
            // combining those two algorirthms:
            var gscv = GridSearch<double[], int>.CrossValidate(

                // Here we can specify the range of the parameters to be included in the search
                ranges: new
                {
                    Complexity = GridSearch.Values(0.00000001, 5.20, 0.30, 0.50),
                    Degree = GridSearch.Values(1, 10, 2, 3, 4, 5),
                    Constant = GridSearch.Values(0, 1, 2),
                },

                // Indicate how learning algorithms for the models should be created
                learner: (p, ss) => new SequentialMinimalOptimization<Polynomial>
                {
                    // Here, we can use the parameters we have specified above:
                    Complexity = p.Complexity,
                    Kernel = new Polynomial(p.Degree, p.Constant)
                },

                // Define how the model should be learned, if needed
                fit: (teacher, x, y, w) => teacher.Learn(x, y, w),

                // Define how the performance of the models should be measured
                loss: (actual, expected, r) => new ZeroOneLoss(expected).Loss(actual),

                folds: 3 // use k = 3 in k-fold cross validation
            );

            // If needed, control the parallelization degree
            gscv.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Search for the best vector machine
            var result = gscv.Learn(inputs, xor);

            // Get the best cross-validation result:
            var crossValidation = result.BestModel;

            // Estimate its error:
            double bestError = result.BestModelError;
            double trainError = result.BestModel.Training.Mean;
            double trainErrorVar = result.BestModel.Training.Variance;
            double valError = result.BestModel.Validation.Mean;
            double valErrorVar = result.BestModel.Validation.Variance;

            // Get the best values for the parameters:
            double bestC = result.BestParameters.Complexity;
            double bestDegree = result.BestParameters.Degree;
            double bestConstant = result.BestParameters.Constant;
            #endregion

            Assert.AreEqual(2, result.BestModel.NumberOfInputs);
            Assert.AreEqual(1, result.BestModel.NumberOfOutputs);
            Assert.AreEqual(16, result.BestModel.NumberOfSamples);
            Assert.AreEqual(5.333333333333333, result.BestModel.AverageNumberOfSamples);
            Assert.AreEqual(1e-8, bestC, 1e-10);
            Assert.AreEqual(0, bestError, 1e-8);
            Assert.AreEqual(10, bestDegree, 1e-8);
            Assert.AreEqual(0, bestConstant, 1e-8);
        }

#if !NET35
        [Test]
        [Category("Slow")]
        public void cross_validation_decision_tree()
        {
            #region doc_learn_tree_cv
            // Ensure results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

            // This is a sample code showing how to use Grid-Search in combination with 
            // Cross-Validation  to assess the performance of Decision Trees with C4.5.

            var parkinsons = new Parkinsons();
            double[][] input = parkinsons.Features;
            int[] output = parkinsons.ClassLabels;

            // Create a new Grid-Search with Cross-Validation algorithm. Even though the
            // generic, strongly-typed approach used accross the framework is most of the
            // time easier to handle, combining those both methods in a single call can be
            // difficult. For this reason. the framework offers a specialized method for
            // combining those two algorirthms:
            var gscv = GridSearch.CrossValidate(

                // Here we can specify the range of the parameters to be included in the search
                ranges: new
                {
                    Join = GridSearch.Range(fromInclusive: 1, toExclusive: 20),
                    MaxHeight = GridSearch.Range(fromInclusive: 1, toExclusive: 20),
                },

                // Indicate how learning algorithms for the models should be created
                learner: (p, ss) => new C45Learning
                {
                    // Here, we can use the parameters we have specified above:
                    Join = p.Join,
                    MaxHeight = p.MaxHeight,
                },

                // Define how the model should be learned, if needed
                fit: (teacher, x, y, w) => teacher.Learn(x, y, w),

                // Define how the performance of the models should be measured
                loss: (actual, expected, r) => new ZeroOneLoss(expected).Loss(actual),

                folds: 3, // use k = 3 in k-fold cross validation

                x: input, y: output // so the compiler can infer generic types
            );

            // If needed, control the parallelization degree
            gscv.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Search for the best decision tree
            var result = gscv.Learn(input, output);

            // Get the best cross-validation result:
            var crossValidation = result.BestModel;

            // Get an estimate of its error:
            double bestAverageError = result.BestModelError;

            double trainError = result.BestModel.Training.Mean;
            double trainErrorVar = result.BestModel.Training.Variance;
            double valError = result.BestModel.Validation.Mean;
            double valErrorVar = result.BestModel.Validation.Variance;

            // Get the best values for the parameters:
            int bestJoin = result.BestParameters.Join;
            int bestHeight = result.BestParameters.MaxHeight;

            // Use the best parameter values to create the final 
            // model using all the training and validation data:
            var bestTeacher = new C45Learning
            {
                Join = bestJoin,
                MaxHeight = bestHeight,
            };

            // Use the best parameters to create the final tree model:
            DecisionTree finalTree = bestTeacher.Learn(input, output);
            #endregion

            int height = finalTree.GetHeight();
            Assert.AreEqual(5, height);
            Assert.AreEqual(22, result.BestModel.NumberOfInputs);
            Assert.AreEqual(2, result.BestModel.NumberOfOutputs);
            Assert.AreEqual(195, result.BestModel.NumberOfSamples);
            Assert.AreEqual(65, result.BestModel.AverageNumberOfSamples);
            Assert.AreEqual(bestAverageError, valError);
            Assert.AreEqual(5, bestJoin, 1e-10);
            Assert.AreEqual(0.1076923076923077, bestAverageError, 1e-8);
            Assert.AreEqual(5, bestHeight, 1e-8);
        }
#endif


        class Mapper : TransformBase<string, string>
        {
            public string[] Inputs { get; set; }

            public string[] Outputs { get; set; }

            public double[] Weights { get; set; }

            public string Parameter1 { get; set; }

            public string Parameter2 { get; set; }

            public string Parameter3 { get; set; }

            public override string Transform(string input)
            {
                return String.Format("output for {} using {}, {}, {}.",
                    input, Parameter1, Parameter2, Parameter3);
            }
        }

        class MapperLearning : ISupervisedLearning<Mapper, string, string>
        {
            public CancellationToken Token { get; set; }

            public string Parameter1 { get; set; }
            public string Parameter2 { get; set; }
            public string Parameter3 { get; set; }

            public Mapper Learn(string[] x, string[] y, double[] weights = null)
            {
                if (Parameter1 == "parameter 12" && Parameter2 == "parameter 22" && Parameter3 == "parameter 31")
                    throw new ConvergenceException("Exception test");

                return new Mapper()
                {
                    Inputs = x,
                    Outputs = y,
                    Weights = weights,
                    Parameter1 = Parameter1,
                    Parameter2 = Parameter2,
                    Parameter3 = Parameter3,
                    NumberOfInputs = 4,
                    NumberOfOutputs = 2,
                };
            }
        }

        [Test]
        public void internals_test()
        {
            Accord.Math.Random.Generator.Seed = 0;

            string[] inputs =
            {
                "input 1",
                "input 2",
                "input 3",
                "input 4",
            };

            string[] outputs =
            {
                "output 1",
                "output 2",
                "output 3",
            };

            double[] weights =
            {
                1.0,
                2.0,
                3.0
            };

            var lossModels = new List<Mapper>();

            var ranges = new
            {
                Parameter1 = GridSearch.Values("parameter 11", "parameter 12"),
                Parameter2 = GridSearch.Values("parameter 21", "parameter 22", "parameter 23", "parameter 24"),
                Parameter3 = GridSearch.Values("parameter 31")
            };

            var gs = GridSearch.Create(

                ranges: ranges,

                learner: (p) => new MapperLearning
                {
                    Parameter1 = p.Parameter1,
                    Parameter2 = p.Parameter2,
                    Parameter3 = p.Parameter3,
                },

                fit: (teacher, x, y, w) => teacher.Learn(x, y, w),
                loss: (actual, expected, m) =>
                {
                    if (m.Parameter1 == "parameter 12" && m.Parameter2 == "parameter 21" && m.Parameter3 == "parameter 31")
                        return -42;

                    lock (lossModels)
                    {
                        lossModels.Add(m);
                    }

                    return Math.Abs(int.Parse(m.Parameter1.Replace("parameter ", ""))
                        + 100 * int.Parse(m.Parameter2.Replace("parameter ", ""))
                        + 10000 * int.Parse(m.Parameter3.Replace("parameter ", "")));
                },

                x: inputs,
                y: outputs
            );

            var result = gs.Learn(inputs, outputs, weights);

            Mapper bestModel = result.BestModel;
            Assert.AreEqual("parameter 12", bestModel.Parameter1);
            Assert.AreEqual("parameter 21", bestModel.Parameter2);
            Assert.AreEqual("parameter 31", bestModel.Parameter3);
            Assert.AreEqual(inputs, bestModel.Inputs);
            Assert.AreEqual(outputs, bestModel.Outputs);
            Assert.AreEqual(weights, bestModel.Weights);

            Assert.AreEqual(-42, result.BestModelError);
            Assert.AreEqual(4, result.BestModelIndex);

            var bestParameters = result.BestParameters;
            Assert.AreNotSame(ranges, bestParameters);
            Assert.AreEqual(1, bestParameters.Parameter1.Index);
            Assert.AreEqual(0, bestParameters.Parameter2.Index);
            Assert.AreEqual(0, bestParameters.Parameter3.Index);

            Assert.AreEqual("parameter 12", bestParameters.Parameter1.Value);
            Assert.AreEqual("parameter 21", bestParameters.Parameter2.Value);
            Assert.AreEqual("parameter 31", bestParameters.Parameter3.Value);

            Assert.AreEqual(8, result.Count);
            Assert.AreEqual(result.Errors, new double[] {
                312111, 312211, 312311, 312411,
                -42, Double.PositiveInfinity, 312312, 312412 });

            Exception[] exceptions = result.Exceptions;
            for (int i = 0; i < exceptions.Length; i++)
            {
                if (i != 5)
                    Assert.IsNull(exceptions[i]);
                else
                    Assert.AreEqual("Exception test", exceptions[i].Message);
            }


            Mapper[] models = result.Models;
            Assert.AreEqual(8, models.Length);
            Assert.AreEqual(6, lossModels.Count);

            int a = ranges.Parameter1.Length;
            int b = ranges.Parameter2.Length;
            int c = ranges.Parameter3.Length;

            Assert.AreEqual(2, a);
            Assert.AreEqual(4, b);
            Assert.AreEqual(1, c);

            for (int i = 0; i < models.Length; i++)
            {
                if (i == 5)
                {
                    Assert.IsNull(models[i]);
                }
                else
                {
                    Assert.AreEqual(inputs, models[i].Inputs);
                    Assert.AreEqual(outputs, models[i].Outputs);
                    Assert.AreEqual(weights, models[i].Weights);


                    Assert.AreEqual(4, models[i].NumberOfInputs);
                    Assert.AreEqual(2, models[i].NumberOfOutputs);
                    Assert.AreEqual(ranges.Parameter1.Values[((i / c) / b) % a], models[i].Parameter1);
                    Assert.AreEqual(ranges.Parameter2.Values[(i / c) % b], models[i].Parameter2);
                    Assert.AreEqual(ranges.Parameter3.Values[i % c], models[i].Parameter3);

                    if (i != 4)
                        Assert.IsTrue(lossModels.Contains(models[i]));
                }
            }

            Assert.AreEqual(4, result.NumberOfInputs);
            Assert.AreEqual(2, result.NumberOfOutputs);

            var parameters = result.Parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                for (int j = 0; j < parameters.Length; j++)
                {
                    if (i != j)
                    {
                        Assert.AreNotSame(parameters[i], parameters[j]);
                        Assert.AreNotEqual(parameters[i], parameters[j]);

                        Assert.AreNotEqual(parameters[i].Parameter1, parameters[j].Parameter1);
                        Assert.AreNotEqual(parameters[i].Parameter2, parameters[j].Parameter2);
                        Assert.AreNotEqual(parameters[i].Parameter3, parameters[j].Parameter3);
                    }

                    Assert.AreEqual(parameters[i].Parameter1.Values, parameters[j].Parameter1.Values);
                    Assert.AreEqual(parameters[i].Parameter2.Values, parameters[j].Parameter2.Values);
                    Assert.AreEqual(parameters[i].Parameter3.Values, parameters[j].Parameter3.Values);
                }
            }
        }

        [Test]
        public void learn_test_exception()
        {
            // https://github.com/accord-net/framework/issues/1052

            Accord.Math.Random.Generator.Seed = 0;

            double[][] inputs =
            {
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
            };

            int[] xor = // result of xor for the sample input data
            {
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
            };

            var gridsearch = GridSearch<double[], int>.Create(

                ranges: new
                {
                    Kernel = GridSearch.Values<IKernel>(new Linear()),
                    Complexity = GridSearch.Values(100000000000),
                    Tolerance = GridSearch.Range(1e-10, 1.0, stepSize: 0.05)
                },

                learner: (p) => new SequentialMinimalOptimization<IKernel>
                {
                    Complexity = p.Complexity,
                    Kernel = p.Kernel.Value,
                    Tolerance = p.Tolerance
                },

                fit: (teacher, x, y, w) =>
                {
                    try
                    {
                        return teacher.Learn(x, y, w);
                    }
                    finally
                    {
                        throw new Exception("abacaxi");
                    }
                },

                loss: (actual, expected, m) => new ZeroOneLoss(expected).Loss(actual)
            );

            var result = gridsearch.Learn(inputs, xor);

            SupportVectorMachine<IKernel> svm = result.BestModel;

            double bestError = result.BestModelError;

            double bestC = result.BestParameters.Complexity;
            double bestTolerance = result.BestParameters.Tolerance;
            IKernel bestKernel = result.BestParameters.Kernel.Value;

            Assert.IsNull(svm);
            Assert.AreEqual(20, result.Exceptions.Length);

            foreach (Exception ex in result.Exceptions)
                Assert.AreEqual("abacaxi", ex.Message);

            Assert.AreEqual(100000000000, bestC, 1e-10);
            Assert.AreEqual(Double.PositiveInfinity, bestError, 1e-8);
            Assert.AreEqual(1E-10, bestTolerance, 1e-8);
            Assert.AreEqual(typeof(Linear), bestKernel.GetType());
        }
    }
}
