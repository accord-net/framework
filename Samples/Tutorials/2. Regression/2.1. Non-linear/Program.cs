using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Math.Optimization;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Kernels;
using System;

namespace ClassificationSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // In the previous section, we have seen how different models in the Accord.NET 
            // Framework could be used to solve linear regression problems. Here, we look at
            // how to address non-linear problems.

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

            // The task is to output a non-linear combination 
            // of those numbers: log(7.4x) / sqrt(1.1y + 42)
            double[] outputs =
            {
                Math.Log(7.4*3.0) / Math.Sqrt(1.1*1.0 + 42.0),
                Math.Log(7.4*7.0) / Math.Sqrt(1.1*1.0 + 42.0),
                Math.Log(7.4*3.0) / Math.Sqrt(1.1*1.0 + 42.0),
                Math.Log(7.4*3.0) / Math.Sqrt(1.1*2.0 + 42.0),
                Math.Log(7.4*6.0) / Math.Sqrt(1.1*1.0 + 42.0),
            };

            // Solve using a kernel SVM
            kernelSvm1(inputs, outputs);

            // Solve using a kernel SVM
            kernelSvm2(inputs, outputs);

            // Solve using non-linear, gradient-free optimization
            optimization(inputs, outputs);
        }

        private static void kernelSvm1(double[][] inputs, double[] outputs)
        {
            // Create a LibSVM-based support vector regression algorithm
            var teacher = new FanChenLinSupportVectorRegression<Gaussian>()
            {
                Tolerance = 1e-5,
                Complexity = 10000,
                Kernel = new Gaussian(0.1)
            };

            // Use the algorithm to learn the machine
            var svm = teacher.Learn(inputs, outputs);

            // Get machine's predictions for inputs
            double[] prediction = svm.Score(inputs);

            // Compute the error in the prediction (should be 0.0)
            double error = new SquareLoss(outputs).Loss(prediction);

            Console.WriteLine(error);
        }

        private static void kernelSvm2(double[][] inputs, double[] outputs)
        {
            // Create a new Sequential Minimal Optimization (SMO) learning 
            // algorithm and estimate the complexity parameter C from data
            var teacher = new SequentialMinimalOptimizationRegression<Gaussian>()
            {
                UseComplexityHeuristic = true,
                UseKernelEstimation = true // estimate the kernel from the data
            };

            // Teach the vector machine
            var svm = teacher.Learn(inputs, outputs);

            // Classify the samples using the model
            double[] answers = svm.Score(inputs);

            double error = new SquareLoss(outputs).Loss(answers); // should be
        }

        private static void optimization(double[][] inputs, double[] outputs)
        {
            // Non-linear regression can also be solved using arbitrary models
            // that can be defined by the user. For example, let's say we know
            // the overall model for the outputs but we do not know the value
            // of its parameters: log(w0  * x) / sqrt(w1 * y + w2)

            Func<double[], double[], double> model = (double[] x, double[] w)
                => Math.Log(w[0] * x[0]) / Math.Sqrt(w[1] * x[1] + w[2]);

            // Now that we have the model, we want to find which values we
            // can plug in its parameters such that the error when evaluating
            // in our data is as close to zero as possible. Mathematically, we
            // would like to find the best parameters w that minimizes:

            Func<double[], double> objective = (double[] w) =>
            {
                double sumOfSquares = 0.0;
                for (int i = 0; i < inputs.Length; i++)
                {
                    double expected = outputs[i];
                    double actual = model(inputs[i], w);
                    sumOfSquares += Math.Pow(expected - actual, 2);
                }
                return sumOfSquares;
            };

            // Now, let's use a gradient-free optimization algorithm to 
            // find the best parameters for our model's equations:
            var cobyla = new Cobyla(numberOfVariables: 3) // we have 3 parameters: w0, w1, and w2
            {
                Function = objective,
                MaxIterations = 100,
                Solution = new double[] { 1.0, 6.4, 100 } // start with some random values
            };

            bool success = cobyla.Minimize(); // should be true
            double[] solution = cobyla.Solution;

            // Get machine's predictions for inputs
            double[] prediction = inputs.Apply(x => model(x, solution));

            // Compute the error in the prediction (should be 0.0)
            double error = new SquareLoss(outputs).Loss(prediction);

            Console.WriteLine(error); // should be 0.000
        }
    }
}
