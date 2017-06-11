using System;
using Accord.Controls;
using Accord.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;
using Accord.Statistics;

namespace ClassificationSample
{
    class Program
    {
        static void Main(string[] args)
        {
            and();

            xor();

            cancer();
        }

        private static void and()
        {
            // Create a simple binary AND
            // classification problem:

            double[][] problem =
            {
                //             a    b    a + b
                new double[] { 0,   0,     0    },
                new double[] { 0,   1,     0    },
                new double[] { 1,   0,     0    },
                new double[] { 1,   1,     1    },
            };

            // Get the two first columns as the problem
            // inputs and the last column as the output
            
            // input columns
            double[][] inputs = problem.GetColumns(0, 1);

            // output column
            int[] outputs = problem.GetColumn(2).ToInt32();

            // Plot the problem on screen
            ScatterplotBox.Show("AND", inputs, outputs).Hold();


            // Create a L2-regularized L2-loss support vector classification algorithm
            var teacher = new LinearDualCoordinateDescent()
            {
                Loss = Loss.L2,
                Complexity = 1000,
                Tolerance = 1e-5
            };

            // Use the algorithm to learn the machine
            var svm = teacher.Learn(inputs, outputs);

            // Compute the machine's answers for the learned inputs
            bool[] answers = svm.Decide(inputs);

            // Convert to Int32 so we can plot:
            int[] zeroOneAnswers = answers.ToZeroOne();

            // Plot the results
            ScatterplotBox.Show("SVM's answer", inputs, zeroOneAnswers)
                .Hold();
        }

        private static void xor()
        {
            // Create a simple binary XOR
            // classification problem:

            double[][] problem =
            {
                //             a    b    a XOR b
                new double[] { 0,   0,      0    },
                new double[] { 0,   1,      1    },
                new double[] { 1,   0,      1    },
                new double[] { 1,   1,      0    },
            };

            // Get the two first columns as the problem
            // inputs and the last column as the output

            // input columns
            double[][] inputs = problem.GetColumns(0, 1);

            // output column
            int[] outputs = problem.GetColumn(2).ToInt32();

            // Plot the problem on screen
            ScatterplotBox.Show("XOR", inputs, outputs).Hold();


            // However, SVMs expect the output value to be
            // either -1 or +1. As such, we have to convert
            // it so the vector contains { -1, -1, -1, +1 }:
            //
            outputs = outputs.Apply(x => x == 0 ? -1 : 1);

            // Create a L2-regularized L2-loss support vector classification
            var teacher = new LinearDualCoordinateDescent()
            {
                Loss = Loss.L2,
                Complexity = 1000,
                Tolerance = 1e-5
            };

            // Use the learning algorithm to Learn 
            var svm = teacher.Learn(inputs, outputs);

            // Compute the machine's answers:
            bool[] answers = svm.Decide(inputs);

            // Convert to Int32 so we can plot:
            int[] zeroOneAnswers = answers.ToZeroOne();

            // Plot the results
            ScatterplotBox.Show("SVM's answer", inputs, zeroOneAnswers).Hold();



            // Use an explicit kernel expansion to transform the 
            // non-linear classification problem into a linear one
            //
            // Create a quadratic kernel
            Quadratic quadratic = new Quadratic(constant: 1);
            
            // Project the inptus into a higher dimensionality space
            double[][] expansion = quadratic.Transform(inputs);

            // Create the same learning algorithm in the expanded input space
            teacher = new LinearDualCoordinateDescent()
            {
                Loss = Loss.L2,
                Complexity = 1000,
                Tolerance = 1e-5
            };

            // Use the learning algorithm to Learn 
            svm = teacher.Learn(inputs, outputs);

            // Compute the machine's answers for the learned inputs
            answers = svm.Decide(quadratic.Transform(inputs));

            // Convert to Int32 so we can plot:
            zeroOneAnswers = answers.ToZeroOne();

            // Plot the results
            ScatterplotBox.Show("SVM's answer", inputs, zeroOneAnswers).Hold();
        }

        private static void cancer()
        {
            // Create a new LibSVM sparse format data reader
            // to read the Wisconsin's Breast Cancer dataset
            //
            var reader = new SparseReader("examples-sparse.txt");

            // We will read the samples into those two variables:
            Sparse<double>[] inputs;
            double[] doubleOutputs;

            // Read them into the inputs and doubleOutputs:
            reader.ReadToEnd(out inputs, out doubleOutputs);

            // The dataset has output labels as 4 and 2. We have to convert them
            // into negative and positive labels so they can be properly processed.
            //
            bool[] outputs = doubleOutputs.Apply(x => x == 2.0 ? false : true);

            
            // Create a learning algorithm for the problem's dimensions
            var teacher = new LinearDualCoordinateDescent<Linear, Sparse<double>>()
            {
                Loss = Loss.L2,
                Complexity = 1000,
                Tolerance = 1e-5
            };

            // Use the learning algorithm to Learn 
            var svm = teacher.Learn(inputs, outputs);

            // Compute the machine's answers
            bool[] answers = svm.Decide(inputs);

            // Create a confusion matrix to show the machine's performance
            var m = new ConfusionMatrix(predicted: answers, expected: outputs);

            // Show it onscreen
            DataGridBox.Show(new ConfusionMatrixView(m));
        }

    }
}
