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
using AForge.Neuro;

namespace ClassificationSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read the Excel worksheet into a DataTable
            DataTable table = new ExcelReader("examples.xls").GetWorksheet("Sheet1");

            // Convert the DataTable to input and output vectors
            double[][] inputs = table.ToArray<double>("X", "Y");
            int[] outputs = table.Columns["G"].ToArray<int>();

            // Plot the data
            ScatterplotBox.Show("Yin-Yang", inputs, outputs).Hold();

            naiveBayes(inputs, outputs);

            decisionTree(inputs, outputs);

            linearSvm(inputs, outputs);

            kernelSvm(inputs, outputs);

            logistic(inputs, outputs);

            network(inputs, outputs);
        }

        private static void naiveBayes(double[][] inputs, int[] outputs)
        {
            // In our problem, we have 2 classes (samples can be either
            // positive or negative), and 2 inputs (x and y coordinates).

            var nb = new NaiveBayes<NormalDistribution>(classes: 2,
                inputs: 2, initial: new NormalDistribution());

            // The Naive Bayes expects the class labels to 
            // range from 0 to k, so we convert -1 to be 0:
            //
            outputs = outputs.Apply(x => x < 0 ? 0 : x);

            // Estimate the Naive Bayes
            double error = nb.Estimate(inputs, outputs);

            // Classify the samples using the model
            int[] answers = inputs.Apply(nb.Compute);

            // Plot the results
            ScatterplotBox.Show("Expected results", inputs, outputs);
            ScatterplotBox.Show("Naive Bayes results", inputs, answers)
                .Hold();
        }

        private static void decisionTree(double[][] inputs, int[] outputs)
        {
            // In our problem, we have 2 classes (samples can be either
            // positive or negative), and 2 continuous-valued inputs.
            DecisionTree tree = new DecisionTree(inputs: new[] 
            {
                DecisionVariable.Continuous("X"),
                DecisionVariable.Continuous("Y")
            }, classes: 2);

            C45Learning teacher = new C45Learning(tree);

            // The C4.5 algorithm expects the class labels to
            // range from 0 to k, so we convert -1 to be zero:
            //
            outputs = outputs.Apply(x => x < 0 ? 0 : x);

            double error = teacher.Run(inputs, outputs);

            // Classify the samples using the model
            int[] answers = inputs.Apply(tree.Compute);

            // Plot the results
            ScatterplotBox.Show("Expected results", inputs, outputs);
            ScatterplotBox.Show("Decision Tree results", inputs, answers)
                .Hold();
        }

        private static void linearSvm(double[][] inputs, int[] outputs)
        {
            // Create a linear binary machine with 2 inputs
            var svm = new SupportVectorMachine(inputs: 2);

            // Create a L2-regularized L2-loss optimization algorithm for
            // the dual form of the learning problem. This is *exactly* the
            // same method used by LIBLINEAR when specifying -s 1 in the 
            // command line (i.e. L2R_L2LOSS_SVC_DUAL).
            //
            var teacher = new LinearCoordinateDescent(svm, inputs, outputs);

            // Teach the vector machine
            double error = teacher.Run();

            // Classify the samples using the model
            int[] answers = inputs.Apply(svm.Compute).Apply(System.Math.Sign);

            // Plot the results
            ScatterplotBox.Show("Expected results", inputs, outputs);
            ScatterplotBox.Show("LinearSVM results", inputs, answers);

            // Grab the index of multipliers higher than 0
            int[] idx = teacher.Lagrange.Find(x => x > 0);

            // Select the input vectors for those
            double[][] sv = inputs.Submatrix(idx);

            // Plot the support vectors selected by the machine
            ScatterplotBox.Show("Support vectors", sv).Hold();
        }

        private static void kernelSvm(double[][] inputs, int[] outputs)
        {
            // Estimate the kernel from the data
            var gaussian = Gaussian.Estimate(inputs);

            // Create a Gaussian binary support machine with 2 inputs
            var svm = new KernelSupportVectorMachine(gaussian, inputs: 2);

            // Create a new Sequential Minimal Optimization (SMO) learning 
            // algorithm and estimate the complexity parameter C from data
            var teacher = new SequentialMinimalOptimization(svm, inputs, outputs)
            {
                UseComplexityHeuristic = true
            };

            // Teach the vector machine
            double error = teacher.Run();

            // Classify the samples using the model
            int[] answers = inputs.Apply(svm.Compute).Apply(System.Math.Sign);

            // Plot the results
            ScatterplotBox.Show("Expected results", inputs, outputs);
            ScatterplotBox.Show("GaussianSVM results", inputs, answers);

            // Grab the index of multipliers higher than 0
            int[] idx = teacher.Lagrange.Find(x => x > 0);

            // Select the input vectors for those
            double[][] sv = inputs.Submatrix(idx);

            // Plot the support vectors selected by the machine
            ScatterplotBox.Show("Support vectors", sv).Hold();
        }

        private static void network(double[][] inputs, int[] outputs)
        {
            // Since we would like to learn binary outputs in the form
            // [-1,+1], we can use a bipolar sigmoid activation function
            IActivationFunction function = new BipolarSigmoidFunction();

            // In our problem, we have 2 inputs (x, y pairs), and we will 
            // be creating a network with 5 hidden neurons and 1 output:
            //
            var network = new ActivationNetwork(function,
                inputsCount: 2, neuronsCount: new[] { 5, 1 });

            // Create a Levenberg-Marquardt algorithm
            var teacher = new LevenbergMarquardtLearning(network)
            {
                UseRegularization = true
            };


            // Because the network is expecting multiple outputs,
            // we have to convert our single variable into arrays
            //
            var y = outputs.ToDouble().ToArray();

            // Iterate until stop criteria is met
            double error = double.PositiveInfinity;
            double previous;

            do
            {
                previous = error;

                // Compute one learning iteration
                error = teacher.RunEpoch(inputs, y);

            } while (Math.Abs(previous - error) < 1e-10 * previous);


            // Classify the samples using the model
            int[] answers = inputs.Apply(network.Compute).GetColumn(0).Apply(System.Math.Sign);

            // Plot the results
            ScatterplotBox.Show("Expected results", inputs, outputs);
            ScatterplotBox.Show("Network results", inputs, answers)
                .Hold();
        }

        private static void logistic(double[][] inputs, int[] outputs)
        {
            // In our problem, we have 2 inputs (x, y pairs)
            var logistic = new LogisticRegression(inputs: 2);

            // Create a iterative re-weighted least squares algorithm
            var teacher = new IterativeReweightedLeastSquares(logistic);


            // Logistic Regression expects the output labels 
            // to range from 0 to k, so we convert -1 to be 0:
            //
            outputs = outputs.Apply(x => x < 0 ? 0 : x);


            // Iterate until stop criteria is met
            double error = double.PositiveInfinity;
            double previous;

            do
            {
                previous = error;

                // Compute one learning iteration
                error = teacher.Run(inputs, outputs);

            } while (Math.Abs(previous - error) < 1e-10 * previous);


            // Classify the samples using the model
            int[] answers = inputs.Apply(logistic.Compute).Apply(Math.Round).ToInt32();

            // Plot the results
            ScatterplotBox.Show("Expected results", inputs, outputs);
            ScatterplotBox.Show("Logistic Regression results", inputs, answers)
                .Hold();
        }

    }
}
