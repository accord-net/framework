using Accord.Controls;
using Accord.IO;
using Accord.MachineLearning.Bayes;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Neuro;
using Accord.Neuro.Learning;
using Accord.Statistics;
using Accord.Statistics.Analysis;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Kernels;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;
using System;
using System.Data;

namespace Tutorials.Classification.Binary
{
    class Program
    {
        static void Main(string[] args)
        {
            // The first examples show how to create Support Vector Machines for
            // classification through input and output matrices defined in code:
            {
                and();

                xor();
            }

            // There are many ways to load data into the framework. Using Excel is
            // one of them. We can read an Excel worksheet into a DataTable using:
            {
                DataTable table = new ExcelReader("examples.xls").GetWorksheet("Sheet1");

                // Convert the DataTable to input and output vectors
                double[][] inputs = table.ToJagged<double>("X", "Y");
                int[] outputs = table.Columns["G"].ToArray<int>();

                // Plot the data
                ScatterplotBox.Show("Yin-Yang", inputs, outputs).Hold();

                // Learn the data using Naive-Bayes
                naiveBayes(inputs, outputs);

                // Learn the data using a Decision Tree
                decisionTree(inputs, outputs);

                // Learn the data using a Linear SVM
                linearSvm(inputs, outputs);

                // Learn the data using a Kernel SVM
                kernelSvm(inputs, outputs);

                // Learn the data using Logistic Regression
                logisticRegression(inputs, outputs);

                // Learn the data using a neural network
                network(inputs, outputs);
            }


            // Other ways include reading from datasets stored in LibSVM format. In 
            // https://www.csie.ntu.edu.tw/~cjlin/libsvmtools/datasets/ you can find
            // a plentora of machine learning datasets for classification and regression.
            // All those datasets can be loaded into .NET using the SparseReader class:
            {
                // Create a new LibSVM sparse format data reader
                // to read the Wisconsin's Breast Cancer dataset
                //
                var reader = new SparseReader("examples-sparse.txt");

                // Read the sparse inputs and outputs from the file
                var results = reader.ReadSparseToEnd();
                Sparse<double>[] inputs = results.Item1;
                double[] outputs = results.Item2;

                // Note: this can be done more succintly in C# 7:
                // var (inputs, outputs) = reader.ReadSparseToEnd(); 

                // Learn the data using a sparse SVM
                sparseMachine(inputs, outputs);

                // Learn the data using a sparse Logistic Regression
                sparseLogistic(inputs, outputs);

                // Learn the data using s sparse SVM, but then
                // calibrate it to produce probabilistic outputs
                sparseMachineProbabilistic(inputs, outputs);
            }
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

        private static void naiveBayes(double[][] inputs, int[] outputs)
        {
            // In our problem, we have 2 classes (samples can be either
            // positive or negative), and 2 inputs (x and y coordinates).

            // Create a Naive Bayes learning algorithm
            var teacher = new NaiveBayesLearning<NormalDistribution>();

            // Use the learning algorithm to learn
            var nb = teacher.Learn(inputs, outputs);

            // At this point, the learning algorithm should have
            // figured important details about the problem itself:
            int numberOfClasses = nb.NumberOfClasses; // should be 2 (positive or negative)
            int nunmberOfInputs = nb.NumberOfInputs;  // should be 2 (x and y coordinates)

            // Classify the samples using the model
            int[] answers = nb.Decide(inputs);

            // Plot the results
            ScatterplotBox.Show("Expected results", inputs, outputs);
            ScatterplotBox.Show("Naive Bayes results", inputs, answers)
                .Hold();
        }

        private static void decisionTree(double[][] inputs, int[] outputs)
        {
            // In our problem, we have 2 classes (samples can be either
            // positive or negative), and 2 continuous-valued inputs.

            C45Learning teacher = new C45Learning(new[] {
                DecisionVariable.Continuous("X"),
                DecisionVariable.Continuous("Y")
            });

            // Use the learning algorithm to induce the tree
            DecisionTree tree = teacher.Learn(inputs, outputs);

            // Classify the samples using the model
            int[] answers = tree.Decide(inputs);

            // Plot the results
            ScatterplotBox.Show("Expected results", inputs, outputs);
            ScatterplotBox.Show("Decision Tree results", inputs, answers)
                .Hold();
        }

        private static void linearSvm(double[][] inputs, int[] outputs)
        {
            // Create a L2-regularized L2-loss optimization algorithm for
            // the dual form of the learning problem. This is *exactly* the
            // same method used by LIBLINEAR when specifying -s 1 in the 
            // command line (i.e. L2R_L2LOSS_SVC_DUAL).
            //
            var teacher = new LinearCoordinateDescent();

            // Teach the vector machine
            var svm = teacher.Learn(inputs, outputs);

            // Classify the samples using the model
            bool[] answers = svm.Decide(inputs);

            // Convert to Int32 so we can plot:
            int[] zeroOneAnswers = answers.ToZeroOne();

            // Plot the results
            ScatterplotBox.Show("Expected results", inputs, outputs);
            ScatterplotBox.Show("LinearSVM results", inputs, zeroOneAnswers);

            // Grab the index of multipliers higher than 0
            int[] idx = teacher.Lagrange.Find(x => x > 0);

            // Select the input vectors for those
            double[][] sv = inputs.Get(idx);

            // Plot the support vectors selected by the machine
            ScatterplotBox.Show("Support vectors", sv).Hold();
        }

        private static void kernelSvm(double[][] inputs, int[] outputs)
        {
            // Create a new Sequential Minimal Optimization (SMO) learning 
            // algorithm and estimate the complexity parameter C from data
            var teacher = new SequentialMinimalOptimization<Gaussian>()
            {
                UseComplexityHeuristic = true,
                UseKernelEstimation = true // estimate the kernel from the data
            };

            // Teach the vector machine
            var svm = teacher.Learn(inputs, outputs);

            // Classify the samples using the model
            bool[] answers = svm.Decide(inputs);

            // Convert to Int32 so we can plot:
            int[] zeroOneAnswers = answers.ToZeroOne();

            // Plot the results
            ScatterplotBox.Show("Expected results", inputs, outputs);
            ScatterplotBox.Show("GaussianSVM results", inputs, zeroOneAnswers);

            // Grab the index of multipliers higher than 0
            int[] idx = teacher.Lagrange.Find(x => x > 0);

            // Select the input vectors for those
            double[][] sv = inputs.Get(idx);

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
            var y = outputs.ToDouble().ToJagged();

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

        private static void logisticRegression(double[][] inputs, int[] outputs)
        {
            // Create iterative re-weighted least squares for logistic regressions
            var teacher = new IterativeReweightedLeastSquares<LogisticRegression>()
            {
                MaxIterations = 100,
                Regularization = 1e-6
            };

            // Use the teacher algorithm to learn the regression:
            LogisticRegression lr = teacher.Learn(inputs, outputs);

            // Classify the samples using the model
            bool[] answers = lr.Decide(inputs);

            // Convert to Int32 so we can plot:
            int[] zeroOneAnswers = answers.ToZeroOne();

            // Plot the results
            ScatterplotBox.Show("Expected results", inputs, outputs);
            ScatterplotBox.Show("Logistic Regression results", inputs, zeroOneAnswers)
                .Hold();
        }

        private static void sparseMachine(Sparse<double>[] inputs, double[] doubleOutputs)
        {
            // The dataset has output labels as 4 and 2. We have to convert them
            // into negative and positive labels so they can be properly processed.
            //
            bool[] outputs = doubleOutputs.Apply(x => x == 2.0 ? false : true);

            // Create a learning algorithm for Sparse data. The first generic argument
            // of the learning algorithm below is the chosen kernel function, and the
            // second is the type of inputs the machine should accept. Note that, using
            // those interfaces, it is possible to define custom kernel functions that
            // operate directly on double[], string[], graphs, trees or any object:
            var teacher = new LinearDualCoordinateDescent<Linear, Sparse<double>>()
            {
                Loss = Loss.L2,
                Complexity = 1000, // Create a hard-margin SVM
                Tolerance = 1e-5
            };

            // Use the learning algorithm to Learn 
            var svm = teacher.Learn(inputs, outputs);

            // Compute the machine's answers
            bool[] answers = svm.Decide(inputs);

            // Create a confusion matrix to show the machine's performance
            var m = new ConfusionMatrix(predicted: answers, expected: outputs);

            // Show it onscreen
            DataGridBox.Show(new ConfusionMatrixView(m)).Hold();
        }

        private static void sparseLogistic(Sparse<double>[] inputs, double[] doubleOutputs)
        {
            // The dataset has output labels as 4 and 2. We have to convert them
            // into negative and positive labels so they can be properly processed.
            //
            bool[] outputs = doubleOutputs.Apply(x => x == 2.0 ? false : true);

            // Create a probabilistic SVM that can output probabilities besides a decision
            var teacher = new ProbabilisticDualCoordinateDescent<Linear, Sparse<double>>()
            {
                Complexity = 1000,
                Tolerance = 1e-5
            };

            // Use the learning algorithm to Learn 
            var svm = teacher.Learn(inputs, outputs);

            // Transform the machine into a dense logistic regression:
            var lr = LogisticRegression.FromWeights(svm.ToWeights());

            // Compute the machine's answers
            bool[] svmAnswers = svm.Decide(inputs);

            // Compute the machine probability estimates:
            double[] svmProbability = svm.Probability(inputs);

            // Compute the logistic regression's answers:
            bool[] lrAnswers = lr.Decide(inputs.ToDense());

            // Compute the logistic regression probability estimates:
            double[] lrProbability = lr.Probability(inputs.ToDense());

            // They should be equal for both the SVM and the LR
        }

        private static void sparseMachineProbabilistic(Sparse<double>[] inputs, double[] doubleOutputs)
        {
            // The dataset has output labels as 4 and 2. We have to convert them
            // into negative and positive labels so they can be properly processed.
            //
            bool[] outputs = doubleOutputs.Apply(x => x == 2.0 ? false : true);

            // Create a learning algorithm for Sparse data. The first generic argument
            // of the learning algorithm below is the chosen kernel function, and the
            // second is the type of inputs the machine should accept. Note that, using
            // those interfaces, it is possible to define custom kernel functions that
            // operate directly on double[], string[], graphs, trees or any object:
            var teacher = new LinearDualCoordinateDescent<Linear, Sparse<double>>()
            {
                Loss = Loss.L2,
                Complexity = 1000, // Create a hard-margin SVM
                Tolerance = 1e-5
            };

            // Use the learning algorithm to Learn 
            var svm = teacher.Learn(inputs, outputs);

            // Create a probabilistic calibration algorithm based on Platt's method:
            var calibration = new ProbabilisticOutputCalibration<Linear, Sparse<double>>()
            {
                Model = svm
            };

            // Let's say that instead of having our data as bool[], we would
            // have received it as double[] containing the actual probabilities
            // associated with each sample:
            doubleOutputs.Apply(x => x == 2.0 ? 0.05 : 0.87, result: doubleOutputs);

            // Calibrate the SVM using Platt's method
            svm = calibration.Learn(inputs, doubleOutputs);

            // Compute the machine's answers
            bool[] answers = svm.Decide(inputs);

            // Compute the machine's probabilities
            double[] prob = svm.Probability(inputs);

            // Create a confusion matrix to show the machine's performance
            var m = new ConfusionMatrix(predicted: answers, expected: outputs);

            // Show it onscreen
            DataGridBox.Show(new ConfusionMatrixView(m)).Hold();
        }
    }
}
