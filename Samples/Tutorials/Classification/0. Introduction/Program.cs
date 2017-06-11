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

namespace ClassificationSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read the Excel worksheet into a DataTable
            DataTable table = new ExcelReader("examples.xls").GetWorksheet("Sheet1");

            // Convert the DataTable to input and output vectors
            double[][] inputs = table.ToJagged<double>("X", "Y");
            int[] outputs = table.Columns["G"].ToArray<int>();

            // Plot the data
            ScatterplotBox.Show("Yin-Yang", inputs, outputs).Hold();

            naiveBayes(inputs, outputs);

            decisionTree(inputs, outputs);

            linearSvm(inputs, outputs);

            kernelSvm(inputs, outputs);

            logistic(inputs, outputs);

            network(inputs, outputs);

            multilabelsvm();

            sequenceClassification();

            resilientGradientHiddenLearning();
        }

        private static void naiveBayes(double[][] inputs, int[] outputs)
        {
            // In our problem, we have 2 classes (samples can be either
            // positive or negative), and 2 inputs (x and y coordinates).

            // The Naive Bayes expects the class labels to 
            // range from 0 to k, so we convert -1 to be 0:
            //
            outputs = outputs.Apply(x => x < 0 ? 0 : x);

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

            // The C4.5 algorithm expects the class labels to
            // range from 0 to k, so we convert -1 to be zero:
            //
            outputs = outputs.Apply(x => x < 0 ? 0 : x);

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

        private static void logistic(double[][] inputs, int[] outputs)
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

        private static void multilabelsvm()
        {
            // Sample data
            // The following is simple auto association function
            // where each input correspond to its own class. This
            // problem should be easily solved by a Linear kernel.

            // Sample input data
            double[][] inputs =
            {
                new double[] { 0 },
                new double[] { 3 },
                new double[] { 1 },
                new double[] { 2 },
            };

            // Outputs for each of the inputs
            int[][] outputs =
            {
                new[] { -1,  1, -1 },
                new[] { -1, -1,  1 },
                new[] {  1,  1, -1 },
                new[] { -1, -1, -1 },
            };


            // Create the Multi-label learning algorithm for the machine
            var teacher = new MultilabelSupportVectorLearning<Linear>()
            {
                Learner = (p) => new SequentialMinimalOptimization<Linear>()
                {
                    Complexity = 10000.0 // Create a hard SVM
                }
            };

            // Learn a multi-label SVM using the teacher
            var svm = teacher.Learn(inputs, outputs);

            // Compute the machine answers for the inputs
            bool[][] answers = svm.Decide(inputs);

            // Use the machine as if it were a multi-class machine
            // instead of a multi-label, identifying the strongest
            // class among the multi-label predictions:
            int[] maxAnswers = svm.ToMulticlass().Decide(inputs);
        }

        private static void sequenceClassification()
        {
            // Declare some training data
            int[][] inputs = new int[][]
            {
                new int[] { 0,1,1,0 },   // Class 0
                new int[] { 0,0,1,0 },   // Class 0
                new int[] { 0,1,1,1,0 }, // Class 0
                new int[] { 0,1,0 },     // Class 0

                new int[] { 1,0,0,1 },   // Class 1
                new int[] { 1,1,0,1 },   // Class 1
                new int[] { 1,0,0,0,1 }, // Class 1
                new int[] { 1,0,1 },     // Class 1
            };

            int[] outputs = new int[]
            {
                0,0,0,0, // First four sequences are of class 0
                1,1,1,1, // Last four sequences are of class 1
            };


            // We are trying to predict two different classes
            int classes = 2;

            // Each sequence may have up to two symbols (0 or 1)
            int symbols = 2;

            // Nested models will have two states each
            int[] states = new int[] { 2, 2 };

            // Creates a new Hidden Markov Model Classifier with the given parameters
            HiddenMarkovClassifier classifier = new HiddenMarkovClassifier(classes, states, symbols);

            // Create a new learning algorithm to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning(classifier,

                // Train each model until the log-likelihood changes less than 0.001
                modelIndex => new BaumWelchLearning(classifier.Models[modelIndex])
                {
                    Tolerance = 0.001,
                    MaxIterations = 0
                }
            );

            // Train the sequence classifier using the algorithm
            teacher.Learn(inputs, outputs);

            // Compute the classifier answers for the given inputs
            int[] answers = classifier.Decide(inputs);
        }

        private static void resilientGradientHiddenLearning()
        {
            // Suppose we would like to learn how to classify the
            // following set of sequences among three class labels: 
            int[][] inputs =
            {
                // First class of sequences: starts and
                // ends with zeros, ones in the middle:
                new[] { 0, 1, 1, 1, 0 },
                new[] { 0, 0, 1, 1, 0, 0 },
                new[] { 0, 1, 1, 1, 1, 0 },     
 
                // Second class of sequences: starts with
                // twos and switches to ones until the end.
                new[] { 2, 2, 2, 2, 1, 1, 1, 1, 1 },
                new[] { 2, 2, 1, 2, 1, 1, 1, 1, 1 },
                new[] { 2, 2, 2, 2, 2, 1, 1, 1, 1 },
 
                // Third class of sequences: can start
                // with any symbols, but ends with three.
                new[] { 0, 0, 1, 1, 3, 3, 3, 3 },
                new[] { 0, 0, 0, 3, 3, 3, 3 },
                new[] { 1, 0, 1, 2, 2, 2, 3, 3 },
                new[] { 1, 1, 2, 3, 3, 3, 3 },
                new[] { 0, 0, 1, 1, 3, 3, 3, 3 },
                new[] { 2, 2, 0, 3, 3, 3, 3 },
                new[] { 1, 0, 1, 2, 3, 3, 3, 3 },
                new[] { 1, 1, 2, 3, 3, 3, 3 },
            };

            // Now consider their respective class labels
            int[] outputs =
            {
                /* Sequences  1-3 are from class 0: */ 0, 0, 0,
                /* Sequences  4-6 are from class 1: */ 1, 1, 1,
                /* Sequences 7-14 are from class 2: */ 2, 2, 2, 2, 2, 2, 2, 2
            };


            // Create the Hidden Conditional Random Field using a set of discrete features
            var function = new MarkovDiscreteFunction(states: 3, symbols: 4, outputClasses: 3);
            var classifier = new HiddenConditionalRandomField<int>(function);

            // Create a learning algorithm
            var teacher = new HiddenResilientGradientLearning<int>(classifier)
            {
                MaxIterations = 50
            };

            // Run the algorithm and learn the models
            teacher.Learn(inputs, outputs);

            // Compute the classifier answers for the given inputs
            int[] answers = classifier.Decide(inputs);
        }

    }
}
