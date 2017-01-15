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

namespace Accord.Performance.MachineLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            TestSparseKernelSVM();
        }

        private static void TestSparseKernelSVM()
        {
            var news20 = new Accord.DataSets.News20(@"C:\Temp\");
            Sparse<double>[] inputs = news20.Training.Item1.Get(0, 100);
            int[] outputs = news20.Training.Item2.ToMulticlass().Get(0, 100);

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

            Stopwatch sw = Stopwatch.StartNew();
            var svm = learn.Learn(inputs, outputs);
            Console.WriteLine(sw.Elapsed);

            sw = Stopwatch.StartNew();
            int[] predicted = svm.ToMulticlass().Decide(inputs);
            Console.WriteLine(sw.Elapsed);
        }
    }
}
