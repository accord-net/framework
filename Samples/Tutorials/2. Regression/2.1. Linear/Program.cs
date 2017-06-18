using System;
using System.Data;
using Accord.Controls;
using Accord.IO;
using Accord.MachineLearning.Bayes;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Neuro.Learning;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Kernels;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Fields.Functions;
using Accord.Statistics.Models.Fields;
using Accord.Statistics.Models.Fields.Learning;
using Accord.Neuro;
using Accord.Statistics;
using Accord.Statistics.Models.Regression.Linear;
using Accord.Math.Optimization.Losses;

namespace Tutorials.Regression.Linear
{
    // (Please nevermind this line, as it is just addressing a namespace clash
    // between this application namespace and one of the framework's namespaces)
    using Linear = Accord.Statistics.Kernels.Linear;

    class Program
    {
        static void Main(string[] args)
        {
            // In the previous chapter, we have seen how the many models in the Accord.NET Framework 
            // could be used used to solve classification problems. In this chapter, we will focus into
            // another related, but different problem: regression. 

            // In a regression problem, we would typically have some input vectors x and some desired 
            // output values y. Note that, differently from classification problems, here the output 
            // values y are not restricted to be class labels, but can rather be continuous variables 
            // or vectors.

            linearRegression();

            multivariateLinear();

            multipleLinearRegression();

            linearSvm1();

            linearSvm2();

            coxProportionalHazards();
        }

        private static void linearRegression()
        {
            // Declare some sample test data.
            double[] inputs = { 80, 60, 10, 20, 30 };
            double[] outputs = { 20, 40, 30, 50, 60 };

            // Use Ordinary Least Squares to learn the regression
            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();

            // Use OLS to learn the simple linear regression
            SimpleLinearRegression regression = ols.Learn(inputs, outputs);

            // Compute the output for a given input:
            double y = regression.Transform(85); // The answer will be 28.088

            // We can also extract the slope and the intercept term
            // for the line. Those will be -0.26 and 50.5, respectively.
            double s = regression.Slope;     // -0.264706
            double c = regression.Intercept; // 50.588235
        }

        private static void multivariateLinear()
        {
            double[][] inputs =
            {
                // variables:  x1  x2  x3
                new double[] {  1,  1,  1 }, // input sample 1
                new double[] {  2,  1,  1 }, // input sample 2
                new double[] {  3,  1,  1 }, // input sample 3
            };

            double[][] outputs =
            {
                // variables:  y1  y2
                new double[] {  2,  3 }, // corresponding output to sample 1
                new double[] {  4,  6 }, // corresponding output to sample 2
                new double[] {  6,  9 }, // corresponding output to sample 3
            };

            // Use Ordinary Least Squares to create the regression
            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();

            // Now, compute the multivariate linear regression:
            MultivariateLinearRegression regression = ols.Learn(inputs, outputs);

            // We can obtain predictions using
            double[][] predictions = regression.Transform(inputs);

            // The prediction error is
            double error = new SquareLoss(outputs).Loss(predictions); // 0
        }

        private static void multipleLinearRegression()
        {
            // Now suppose you have some points
            double[][] inputs =
            {
                new double[] { 1, 1 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 0, 0 },
            };

            // located in the same Z (z = 1)
            double[] outputs = { 1, 1, 1, 1 };

            // We will use Ordinary Least Squares to create a
            // linear regression model with an intercept term
            var ols = new OrdinaryLeastSquares()
            {
                UseIntercept = true
            };

            // Use Ordinary Least Squares to estimate a regression model
            MultipleLinearRegression regression = ols.Learn(inputs, outputs);

            // As result, we will be given the following:
            double a = regression.Weights[0]; // a = 0
            double b = regression.Weights[1]; // b = 0
            double c = regression.Intercept;  // c = 1

            // This is the plane described by the equation
            // ax + by + c = z => 0x + 0y + 1 = z => 1 = z.

            // We can compute the predicted points using
            double[] predicted = regression.Transform(inputs);

            // And the squared error loss using 
            double error = new SquareLoss(outputs).Loss(predicted);
        }

        private static void linearSvm1()
        {
            // Declare a very simple regression problem 
            // with only 2 input variables (x and y):
            double[][] inputs =
            {
                new[] { 3.0, 1.0 },
                new[] { 7.0, 1.0 },
                new[] { 3.0, 1.0 },
                new[] { 3.0, 2.0 },
                new[] { 6.0, 1.0 },
            };

            // The task is to output a weighted sum of those numbers 
            // plus an independent constant term: 7.4x + 1.1y + 42
            double[] outputs =
            {
                7.4*3.0 + 1.1*1.0 + 42.0,
                7.4*7.0 + 1.1*1.0 + 42.0,
                7.4*3.0 + 1.1*1.0 + 42.0,
                7.4*3.0 + 1.1*2.0 + 42.0,
                7.4*6.0 + 1.1*1.0 + 42.0,
            };

            // Create a new Sequential Minimal Optimization (SMO) learning 
            // algorithm and estimate the complexity parameter C from data
            var teacher = new SequentialMinimalOptimizationRegression<Linear>()
            {
                UseComplexityHeuristic = true,
                Complexity = 100000.0 // Note: do not do this in an actual application!
                // Setting the Complexity property to a very high value forces the SVM
                // to "believe literally" in whatever the data says. Normally, the SVM
                // would be more cautions under the (valid) assumption that the data 
                // might actually contain noise and/or incorrect measurements.
            };

            // Teach the vector machine
            var svm = teacher.Learn(inputs, outputs);

            // Classify the samples using the model
            double[] answers = svm.Score(inputs);

            double error = new SquareLoss(outputs).Loss(answers); // should be 0.0
        }

        private static void linearSvm2()
        {
            // Declare a very simple regression problem 
            // with only 2 input variables (x and y):
            double[][] inputs =
            {
                new[] { 3.0, 1.0 },
                new[] { 7.0, 1.0 },
                new[] { 3.0, 1.0 },
                new[] { 3.0, 2.0 },
                new[] { 6.0, 1.0 },
            };

            // The task is to output a weighted sum of those numbers 
            // plus an independent constant term: 7.4x + 1.1y + 42
            double[] outputs =
            {
                7.4*3.0 + 1.1*1.0 + 42.0,
                7.4*7.0 + 1.1*1.0 + 42.0,
                7.4*3.0 + 1.1*1.0 + 42.0,
                7.4*3.0 + 1.1*2.0 + 42.0,
                7.4*6.0 + 1.1*1.0 + 42.0,
            };

            // Create Newton-based support vector regression 
            var teacher = new LinearRegressionNewtonMethod()
            {
                Tolerance = 1e-5,
                Complexity = 10000
            };

            // Use the algorithm to learn the machine
            var svm = teacher.Learn(inputs, outputs);

            // Get machine's predictions for inputs
            double[] prediction = svm.Score(inputs);

            // Compute the error in the prediction (should be 0.0)
            double error = new SquareLoss(outputs).Loss(prediction);

            Console.WriteLine(error);
        }

        private static void coxProportionalHazards()
        {
            // Let's say we have the following survival problem. Each row in the table below 
            // represents a patient under care in a hospital. The first colum represents their 
            // age (a single feature, but there could have been many like age, height, weight, 
            // etc), the time until an event has happened (like, for example, unfortunatey death) 
            // and the event outcome (i.e. what has exactly happened after this amount of time,
            // has the patient died or did he simply leave the hospital and we couldn't get more 
            // data about him?)

            object[,] data =
            {
                //    input         time until           outcome 
                // (features)     event happened     (what happened?)
                {       50,              1,         SurvivalOutcome.Censored  },
                {       70,              2,         SurvivalOutcome.Failed    },
                {       45,              3,         SurvivalOutcome.Censored  },
                {       35,              5,         SurvivalOutcome.Censored  },
                {       62,              7,         SurvivalOutcome.Failed    },
                {       50,             11,         SurvivalOutcome.Censored  },
                {       45,              4,         SurvivalOutcome.Censored  },
                {       57,              6,         SurvivalOutcome.Censored  },
                {       32,              8,         SurvivalOutcome.Censored  },
                {       57,              9,         SurvivalOutcome.Failed    },
                {       60,             10,         SurvivalOutcome.Failed    },
            }; // Note: Censored means that we stopped recording data for that person,
               // so we do not know what actually happened to them, except that things
               // were going fine until the point in time appointed by "time to event"

            // Parse the data above
            double[][] inputs = data.GetColumn(0).ToDouble().ToJagged();
            double[] time = data.GetColumn(1).ToDouble();
            SurvivalOutcome[] output = data.GetColumn(2).To<SurvivalOutcome[]>();

            // Create a new PH Newton-Raphson learning algorithm
            var teacher = new ProportionalHazardsNewtonRaphson()
            {
                ComputeBaselineFunction = true,
                ComputeStandardErrors = true,
                MaxIterations = 100
            };

            // Use the learning algorithm to infer a Proportional Hazards model
            ProportionalHazards regression = teacher.Learn(inputs, time, output);

            // Use the regression to make predictions (problematic)
            SurvivalOutcome[] prediction = regression.Decide(inputs);

            // Use the regression to make score estimates 
            double[] score = regression.Score(inputs);

            // Use the regression to make probability estimates 
            double[] probability = regression.Probability(inputs);
        }
    }
}
