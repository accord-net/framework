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
    public class KernelSupportVectorMachineTest
    {
        private readonly double[][] inputs;
        private readonly int[] outputs;

        public KernelSupportVectorMachineTest()
        {
            var data = Shared.Examples.YinYang();
            inputs = data.Training.Inputs;
            outputs = data.Training.Output;
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
            var ksvm = new KernelSupportVectorMachine(new Polynomial(2), 2);
            var smo = new SequentialMinimalOptimization(ksvm, inputs, outputs);

            return smo.Run(computeError: false);
        }

    }
}