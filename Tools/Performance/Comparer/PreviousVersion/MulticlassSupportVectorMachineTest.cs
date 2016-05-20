using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Kernels;
using BenchmarkDotNet.Attributes;
using Shared;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PreviousVersion
{
    [Config(typeof(FastAndDirtyConfig))]
    public class MulticlassSupportVectorMachineTest
    {
        private readonly Problem problem;

        public MulticlassSupportVectorMachineTest()
        {
            problem = Shared.Examples.KaggleDigits();
        }

        [Benchmark]
        public Plane Accord_Math()
        {
            return new Accord.Math.Plane(0, 1, 2);
        }

        [Benchmark]
        public NormalDistribution Accord_Statistics()
        {
            return new NormalDistribution();
        }

        [Benchmark]
        public double v3_0_1()
        {
            var ksvm = new MulticlassSupportVectorMachine(784, new Polynomial(2), 10);
            var smo = new MulticlassSupportVectorLearning(ksvm, problem.Training.Inputs, problem.Training.Output);
            smo.Algorithm = (svm, x, y, i, j) => new SequentialMinimalOptimization(svm, x, y);

            return smo.Run(computeError: false);
        }

    }
}