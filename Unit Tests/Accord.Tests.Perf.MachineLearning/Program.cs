using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using System;
using System.Collections.Generic;
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
            var news20 = new Accord.Datasets.News20();
            Sparse<double>[] inputs = news20.Training.Item1;
            int[] outputs = news20.Training.Item2.ToInt32();

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

            var svm = learn.Learn(inputs, outputs);
        }
    }
}
