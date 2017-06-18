using Accord.DataSets;
using Accord.MachineLearning;
using Accord.MachineLearning.Bayes;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Math.Distances;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Analysis;
using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Kernels;
using Accord.Statistics.Models.Fields.Functions;
using Accord.Statistics.Models.Fields.Learning;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;

namespace Tutorials.Classification.Multiclass
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                // In the previous sections we have seen how to import data into .NET using
                // ExcelReader and SparseReader. Yet another way to get a machine learning
                // dataset into .NET is to see if it is not already available in Accord.NET's
                // DataSets project.

                // The Accord.DataSets gives access to many popular machine learning datasets
                // that you can use right away to test your algorithms, code, and knowledge.

                // We can start by loading the famous Ronald Fisher's Iris flower dataset:
                Iris iris = new Iris(); // https://en.wikipedia.org/wiki/Iris_flower_data_set

                // Get input and output pairs from it:
                double[][] inputs = iris.Instances;
                int[] outputs = iris.ClassLabels;

                // Let's check how many classes we have in this dataset:
                int classes = outputs.Distinct().Length; // should be 3

                // Learn using K-Nearest Neighbors
                knn(inputs, outputs);

                // Learn using Naive-Bayes
                naiveBayes(inputs, outputs);

                // Learn using a decision tree
                decisionTree(inputs, outputs);

                // Learn using a random forest
                randomForest(inputs, outputs);

                // Learn using a multi-class (one-vs-all) SVM
                multiclassSvm(inputs, outputs);

                // Learn using multi-label (one-vs-rest) SVM
                multilabelSvm(inputs, outputs);

                // Learn using Multinomial Logistic Regression
                multinomial(inputs, outputs);
            }

            {
                // Until now, in every example we have seen the input vectors always
                // had a constant size. In other words, the number of variables in
                // the same classification problem would never change and be fixed
                // at a single value, such as 5.

                // However, there are cases in which we would like to classify entities
                // which do not have a fixed size, for example, sequences of observations.

                // For example, let's consider the example below. Each row in the array
                // represents a different sequence of observations. The observations are
                // represented by the integers 0, 1 or 2. The first four observations in
                // the top correspond to the first class (class label 0), the next four 
                // to the second class (class label 1), and the last three to the third
                // class (class label 2).

                int[][] inputs = new int[][]
                {
                    new int[] { 0, 1, 1, 2    },    // Class 0
                    new int[] { 0, 0, 1, 2    },    // Class 0
                    new int[] { 0, 1, 1, 1, 2 },    // Class 0
                    new int[] { 0, 1, 2       },    // Class 0
                                         
                    new int[] { 1, 2, 0, 1    },    // Class 1
                    new int[] { 1, 1, 0, 1    },    // Class 1
                    new int[] { 1, 2, 0, 0, 1 },    // Class 1
                    new int[] { 1, 2, 1       },    // Class 1

                    new int[] { 1, 1          },    // Class 2
                    new int[] { 1, 1, 1, 1    },    // Class 2
                    new int[] { 1, 1, 1       },    // Class 2
                };

                int[] outputs = new int[]
                {
                    0,0,0,0, // First four sequences are of class 0
                    1,1,1,1, // Next  four sequences are of class 1
                    2,2,2    // Last  four sequences are of class 2
                };

                // Learn using a Dynamic Time Warping SVM
                dtw(inputs, outputs);

                // Learn using a Hidden Markov Classifier
                hmmc(inputs, outputs);

                // Learn using a Hidden Conditional Random Field
                hcrf(inputs, outputs);

                // Learn using Bag-of-Words
                bagOfWords(inputs, outputs);
            }
        }


        private static void knn(double[][] inputs, int[] outputs)
        {
            // Create a new k-NN algorithm:
            var knn = new KNearestNeighbors()
            {
                K = 3, // base a decision on the class labels of the three nearest neighbors of the query point
                Distance = new Euclidean() // actually the default
            };

            // Learn a k-NN classifier
            knn = knn.Learn(inputs, outputs);

            // Get predictions according to kNN
            int[] predicted = knn.Decide(inputs);

            // Create a confusion matrix to check the quality of the predictions:
            var cm = new ConfusionMatrix(predicted: predicted, expected: outputs);

            // Check the accuracy measure:
            double accuracy = cm.Accuracy; // (should be 1.0 or 100%)
        }

        private static void naiveBayes(double[][] inputs, int[] outputs)
        {
            // Create a new Naive-Bayes teaching algorithm with normal distributions.
            // Note: the generic parameters are optional, but they help increasing the
            // type safety of the teacher. For example, by specifying the NormalOptions
            // argument, we will be able to configure settings that are very specific
            // to normal distributions in the NaiveBayes teacher:
            var teacher = new NaiveBayesLearning<NormalDistribution, NormalOptions>()
            {
                Empirical = true // Estimate class priors from the data
            };

            // As mentioned above, because we specified the NormalOptions generic
            // parameter, now we can configure settings such as Gaussian regularization,
            // robustness and constraints (note: setting those is completely optional).
            teacher.Options.InnerOption.Regularization = 1e-10;
            teacher.Options.InnerOption.Diagonal = false;

            // Learn the Naive Bayes classifier
            var nb = teacher.Learn(inputs, outputs);

            // Get predictions according to kNN
            int[] predicted = nb.Decide(inputs);

            // Create a confusion matrix to check the quality of the predictions:
            var cm = new ConfusionMatrix(predicted: predicted, expected: outputs);

            // Check the accuracy measure:
            double accuracy = cm.Accuracy; // (should be 1.0 or 100%)
        }

        private static void decisionTree(double[][] inputs, int[] outputs)
        {
            var teacher = new C45Learning()
            {
                // Note: all the settings below are optional
                Join = 5, // Variables can participate 5 times in a decision path
                MaxVariables = 0, // No limit on the number of variables to consider
                MaxHeight = 10 // The tree can have a maximum height of 10
            };

            // Use the learning algorithm to induce the tree
            DecisionTree tree = teacher.Learn(inputs, outputs);

            // Classify the samples using the model
            int[] predicted = tree.Decide(inputs);

            // Create a confusion matrix to check the quality of the predictions:
            var cm = new ConfusionMatrix(predicted: predicted, expected: outputs);

            // Check the accuracy measure:
            double accuracy = cm.Accuracy; // (should be 1.0 or 100%)
        }

        private static void randomForest(double[][] inputs, int[] outputs)
        {
            var teacher = new RandomForestLearning()
            {
                NumberOfTrees = 100, // Use 100 decision trees to cover this problem
            };

            // Use the learning algorithm to induce the tree
            RandomForest rf = teacher.Learn(inputs, outputs);

            // Classify the samples using the RF
            int[] predicted = rf.Decide(inputs);

            // Create a confusion matrix to check the quality of the predictions:
            var cm = new ConfusionMatrix(predicted: predicted, expected: outputs);

            // Check the accuracy measure:
            double accuracy = cm.Accuracy; // (should be 1.0 or 100%)
        }

        private static void multiclassSvm(double[][] inputs, int[] outputs)
        {
            // Support vector machines are by definition binary classifiers. As such, in order
            // to apply them for classification problems with more than 2 classes we need to
            // use some tricks. There are two most famous ways to achieve a multi-class model
            // from a set of binary ones: one-vs-one and one-vs-the rest.

            // One-vs-one classifiers are true multi-class classifiers in the sense that they
            // can only predict one single class label for each sample. However, One-vs-rest
            // classifiers can predict either one class label or many, depending on how it is
            // used.

            // Create the multi-class learning algorithm as one-vs-one
            var teacher = new MulticlassSupportVectorLearning<Linear>()
            {
                Learner = (p) => new SequentialMinimalOptimization<Linear>()
                {
                    Complexity = 10000.0 // Create a hard SVM
                }
            };

            // Learn a multi-class SVM using the teacher
            var svm = teacher.Learn(inputs, outputs);

            // Get the predictions for the inputs
            int[] predicted = svm.Decide(inputs);

            // Create a confusion matrix to check the quality of the predictions:
            var cm = new ConfusionMatrix(predicted: predicted, expected: outputs);

            // Check the accuracy measure:
            double accuracy = cm.Accuracy; // (should be 1.0 or 100%)
        }

        private static void multilabelSvm(double[][] inputs, int[] outputs)
        {
            // Create the multi-label learning algorithm as one-vs-rest
            var teacher = new MultilabelSupportVectorLearning<Linear>()
            {
                Learner = (p) => new SequentialMinimalOptimization<Linear>()
                {
                    Complexity = 10000.0 // Create a hard SVM
                }
            };

            // Learn a multi-label SVM using the teacher
            var svm = teacher.Learn(inputs, outputs);

            // Get the predictions for the inputs
            bool[][] predicted = svm.Decide(inputs);

            // Use the machine as if it were a multi-class machine
            // instead of a multi-label, identifying the strongest
            // class among the multi-label predictions:
            int[] classLabels = svm.ToMulticlass().Decide(inputs);
        }

        private static void multinomial(double[][] inputs, int[] outputs)
        {
            var lbnr = new LowerBoundNewtonRaphson()
            {
                MaxIterations = 100,
                Tolerance = 1e-6
            };

            // Learn a multinomial logistic regression using the teacher:
            MultinomialLogisticRegression mlr = lbnr.Learn(inputs, outputs);

            // We can compute the model answers
            int[] answers = mlr.Decide(inputs);

            // And also the probability of each of the answers
            double[][] probabilities = mlr.Probabilities(inputs);

            // Now we can check how good our model is at predicting
            double error = new AccuracyLoss(outputs).Loss(answers);

            // We can also verify the classes with highest 
            // probability are the ones being decided for:
            int[] argmax = probabilities.ArgMax(dimension: 1); // should be same as 'answers'
        }




        private static void dtw(int[][] inputs, int[] outputs)
        {
            // One way to perform sequence classification with an SVM is to use
            // a kernel defined over sequences, such as DynamicTimeWarping.

            // Create the multi-class learning algorithm as one-vs-one with DTW:
            var teacher = new MulticlassSupportVectorLearning<DynamicTimeWarping<Dirac<int>, int>, int[]>()
            {
                Learner = (p) => new SequentialMinimalOptimization<DynamicTimeWarping<Dirac<int>, int>, int[]>()
                {
                    Complexity = 10000.0 // Create a hard SVM
                }
            };

            // Learn a multi-label SVM using the teacher
            var svm = teacher.Learn(inputs, outputs);

            // Get the predictions for the inputs
            int[] predicted = svm.Decide(inputs);

            // Create a confusion matrix to check the quality of the predictions:
            var cm = new ConfusionMatrix(predicted: predicted, expected: outputs);

            // Check the accuracy measure:
            double accuracy = cm.Accuracy; // (should be 1.0 or 100%)
        }

        private static void hmmc(int[][] inputs, int[] outputs)
        {
            // Create a new learning algorithm to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<GeneralDiscreteDistribution, int>()
            {
                Learner = (i) => new BaumWelchLearning<GeneralDiscreteDistribution, int, GeneralDiscreteOptions>()
                {
                    Tolerance = 0.001,
                    MaxIterations = 0
                }
            };

            // Train the sequence classifier using the algorithm
            var hmmClassifier = teacher.Learn(inputs, outputs);

            // Compute the classifier answers for the given inputs
            int[] answers = hmmClassifier.Decide(inputs);
        }

        private static void hcrf(int[][] inputs, int[] outputs)
        {
            // Create a learning algorithm
            var teacher = new HiddenResilientGradientLearning<int>()
            {
                Function = new MarkovDiscreteFunction(states: 3, symbols: 4, outputClasses: 3),
                MaxIterations = 50
            };

            // Run the algorithm and learn the models
            var hcrf = teacher.Learn(inputs, outputs);

            // Compute the classifier answers for the given inputs
            int[] answers = hcrf.Decide(inputs);
        }

        private static void bagOfWords(int[][] inputs, int[] outputs)
        {
            var bow = new BagOfWords<int>();

            var quantizer = bow.Learn(inputs);

            double[][] histograms = quantizer.Transform(inputs);

            // One way to perform sequence classification with an SVM is to use
            // a kernel defined over sequences, such as DynamicTimeWarping.

            // Create the multi-class learning algorithm as one-vs-one with DTW:
            var teacher = new MulticlassSupportVectorLearning<ChiSquare, double[]>()
            {
                Learner = (p) => new SequentialMinimalOptimization<ChiSquare, double[]>()
                {
                    Complexity = 10000.0 // Create a hard SVM
                }
            };

            // Learn a multi-label SVM using the teacher
            var svm = teacher.Learn(histograms, outputs);

            // Get the predictions for the inputs
            int[] predicted = svm.Decide(histograms);

            // Create a confusion matrix to check the quality of the predictions:
            var cm = new ConfusionMatrix(predicted: predicted, expected: outputs);

            // Check the accuracy measure:
            double accuracy = cm.Accuracy; 
        }

    }
}
