using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using Accord.Statistics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math.Optimization.Losses;

namespace Accord.Performance.MachineLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestSparseKernelSVM();
            //TestPredictSparseSVM();
            //TestSparseSVMComplete();
            TestPredictSparseMulticlassSVM();
        }

        private static void TestSparseKernelSVM()
        {
            Console.WriteLine("Downloading dataset");
            var news20 = new Accord.DataSets.News20(@"C:\Temp\");
            Sparse<double>[] inputs = news20.Training.Item1.Get(0, 2000);
            int[] outputs = news20.Training.Item2.ToMulticlass().Get(0, 2000);

            var learn = new MultilabelSupportVectorLearning<Linear, Sparse<double>>()
            {
                // using LIBLINEAR's L2-loss SVC dual for each SVM
                Learner = (p) => new LinearDualCoordinateDescent<Linear, Sparse<double>>()
                {
                    Loss = Loss.L2,
                    Complexity = 1.0,
                    Tolerance = 1e-4
                }
            };

            Console.WriteLine("Learning");
            Stopwatch sw = Stopwatch.StartNew();
            var svm = learn.Learn(inputs, outputs);
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine("Predicting");
            sw = Stopwatch.StartNew();
            int[] predicted = svm.ToMulticlass().Decide(inputs);
            Console.WriteLine(sw.Elapsed);
        }

        private static void TestPredictSparseSVM()
        {
            Console.WriteLine("Downloading dataset");
            var news20 = new Accord.DataSets.News20(@"C:\Temp\");
            Sparse<double>[] inputs = news20.Training.Item1;
            int[] outputs = news20.Training.Item2.ToMulticlass();

            var learn = new MultilabelSupportVectorLearning<Linear, Sparse<double>>()
            {
                // using LIBLINEAR's L2-loss SVC dual for each SVM
                Learner = (p) => new LinearDualCoordinateDescent<Linear, Sparse<double>>()
                {
                    Loss = Loss.L2,
                    Complexity = 1.0,
                    Tolerance = 1e-4
                }
            };

            Console.WriteLine("Learning");
            Stopwatch sw = Stopwatch.StartNew();
            var svm = learn.Learn(inputs.Get(0, 100), outputs.Get(0, 100));
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine("Predicting");
            sw = Stopwatch.StartNew();
            int[] predicted = svm.ToMulticlass().Decide(inputs);
            Console.WriteLine(sw.Elapsed);
        }

        private static void TestPredictSparseMulticlassSVM()
        {
            Console.WriteLine("Downloading dataset");
            var news20 = new Accord.DataSets.News20(@"C:\Temp\");
            Sparse<double>[] inputs = news20.Training.Item1;
            int[] outputs = news20.Training.Item2.ToMulticlass();

            var learn = new MulticlassSupportVectorLearning<Linear, Sparse<double>>()
            {
                // using LIBLINEAR's L2-loss SVC dual for each SVM
                Learner = (p) => new LinearDualCoordinateDescent<Linear, Sparse<double>>()
                {
                    Loss = Loss.L2,
                    Complexity = 1.0,
                    Tolerance = 1e-4
                }
            };

            Console.WriteLine("Learning");
            Stopwatch sw = Stopwatch.StartNew();
            var svm = learn.Learn(inputs.Get(0, 1000), outputs.Get(0, 1000));
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine("Predicting");
            sw = Stopwatch.StartNew();
            int[] predicted = svm.Decide(inputs);
            Console.WriteLine(sw.Elapsed);
        }

        private static void TestSparseSVMComplete()
        {
            #region doc_learn_news20
            Console.WriteLine("Downloading dataset:");
            var news20 = new Accord.DataSets.News20(@"C:\Temp\");
            var trainInputs = news20.Training.Item1;
            var trainOutputs = news20.Training.Item2.ToMulticlass();
            var testInputs = news20.Testing.Item1;
            var testOutputs = news20.Testing.Item2.ToMulticlass();

            Console.WriteLine(" - Training samples: {0}", trainInputs.Rows());
            Console.WriteLine(" - Testing samples: {0}", testInputs.Rows());
            Console.WriteLine(" - Dimensions: {0}", trainInputs.Columns());
            Console.WriteLine(" - Classes: {0}", trainOutputs.DistinctCount());
            Console.WriteLine();


            // Create and use the learning algorithm to train a sparse linear SVM
            var learn = new MultilabelSupportVectorLearning<Linear, Sparse<double>>()
            {
                // using LIBLINEAR's L2-loss SVC dual for each SVM
                Learner = (p) => new LinearDualCoordinateDescent<Linear, Sparse<double>>()
                {
                    Loss = Loss.L2,
                    Tolerance = 1e-4
                },
            };

            // Display progress in the console
            learn.SubproblemFinished += (sender, e) =>
            {
                Console.WriteLine(" - {0} / {1} ({2:00.0%})", e.Progress, e.Maximum, e.Progress / (double)e.Maximum);
            };

            // Start the learning algorithm
            Console.WriteLine("Learning");
            Stopwatch sw = Stopwatch.StartNew();
            var svm = learn.Learn(trainInputs, trainOutputs);
            Console.WriteLine("Done in {0}", sw.Elapsed);
            Console.WriteLine();


            // Compute accuracy in the training set
            Console.WriteLine("Predicting training set");
            sw = Stopwatch.StartNew();
            int[] trainPredicted = svm.ToMulticlass().Decide(trainInputs);
            Console.WriteLine("Done in {0}", sw.Elapsed);

            double trainError = new ZeroOneLoss(trainOutputs).Loss(trainPredicted);
            Console.WriteLine("Training error: {0}", trainError);
            Console.WriteLine();


            // Compute accuracy in the testing set
            Console.WriteLine("Predicting testing set");
            sw = Stopwatch.StartNew();
            int[] testPredicted = svm.ToMulticlass().Decide(testInputs);
            Console.WriteLine("Done in {0}", sw.Elapsed);

            double testError = new ZeroOneLoss(testOutputs).Loss(testPredicted);
            Console.WriteLine("Testing error: {0}", testError);
            #endregion
        }

    }
}
