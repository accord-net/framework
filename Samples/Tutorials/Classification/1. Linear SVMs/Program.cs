using System;
using Accord.Controls;
using Accord.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;

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


            // However, SVMs expect the output value to be
            // either -1 or +1. As such, we have to convert
            // it so the vector contains { -1, -1, -1, +1 }:
            //
            outputs = outputs.Apply(x => x == 0 ? -1 : 1);


            // Create a new linear-SVM for two inputs (a and b)
            SupportVectorMachine svm = new SupportVectorMachine(inputs: 2);

            // Create a L2-regularized L2-loss support vector classification
            var teacher = new LinearDualCoordinateDescent(svm, inputs, outputs)
            {
                Loss = Loss.L2,
                Complexity = 1000,
                Tolerance = 1e-5
            };

            // Learn the machine
            double error = teacher.Run(computeError: true);

            // Compute the machine's answers for the learned inputs
            int[] answers = inputs.Apply(x => Math.Sign(svm.Compute(x)));

            // Plot the results
            ScatterplotBox.Show("SVM's answer", inputs, answers).Hold();
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


            // Create a new linear-SVM for two inputs (a and b)
            SupportVectorMachine svm = new SupportVectorMachine(inputs: 2);

            // Create a L2-regularized L2-loss support vector classification
            var teacher = new LinearDualCoordinateDescent(svm, inputs, outputs)
            {
                Loss = Loss.L2,
                Complexity = 1000,
                Tolerance = 1e-5
            };

            // Learn the machine
            double error = teacher.Run(computeError: true);

            // Compute the machine's answers for the learned inputs
            int[] answers = inputs.Apply(x => Math.Sign(svm.Compute(x)));

            // Plot the results
            ScatterplotBox.Show("SVM's answer", inputs, answers).Hold();

            // Use an explicit kernel expansion to transform the 
            // non-linear classification problem into a linear one
            //
            // Create a quadratic kernel
            Quadratic quadratic = new Quadratic(constant: 1);
            
            // Project the inptus into a higher dimensionality space
            double[][] expansion = inputs.Apply(quadratic.Transform);


            
            // Create a new linear-SVM for the transformed input space
            svm = new SupportVectorMachine(inputs: expansion[0].Length);

            // Create the same learning algorithm in the expanded input space
            teacher = new LinearDualCoordinateDescent(svm, expansion, outputs)
            {
                Loss = Loss.L2,
                Complexity = 1000,
                Tolerance = 1e-5
            };

            // Learn the machine
            error = teacher.Run(computeError: true); 

            // Compute the machine's answers for the learned inputs
            answers = expansion.Apply(x => Math.Sign(svm.Compute(x)));

            // Plot the results
            ScatterplotBox.Show("SVM's answer", inputs, answers).Hold();
        }

        private static void cancer()
        {
            // Create a new LibSVM sparse format data reader
            // to read the Wisconsin's Breast Cancer dataset
            //
            var reader = new SparseReader("examples-sparse.txt");

            int[] outputs; // Read the classification problem into dense memory
            double[][] inputs = reader.ReadToEnd(sparse: false, labels: out outputs);

            // The dataset has output labels as 4 and 2. We have to convert them
            // into negative and positive labels so they can be properly processed.
            //
            outputs = outputs.Apply(x => x == 2 ? -1 : +1);

            // Create a new linear-SVM for the problem dimensions
            var svm = new SupportVectorMachine(inputs: reader.Dimensions);

            // Create a learning algorithm for the problem's dimensions
            var teacher = new LinearDualCoordinateDescent(svm, inputs, outputs)
            {
                Loss = Loss.L2,
                Complexity = 1000,
                Tolerance = 1e-5
            };

            // Learn the classification
            double error = teacher.Run();

            // Compute the machine's answers for the learned inputs
            int[] answers = inputs.Apply(x => Math.Sign(svm.Compute(x)));

            // Create a confusion matrix to show the machine's performance
            var m = new ConfusionMatrix(predicted: answers, expected: outputs);

            // Show it onscreen
            DataGridBox.Show(new ConfusionMatrixView(m));
        }

    }
}
