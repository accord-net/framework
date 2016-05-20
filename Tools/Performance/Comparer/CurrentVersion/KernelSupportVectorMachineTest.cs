using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using BenchmarkDotNet.Attributes;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Math.Distances;
using Accord.Statistics.Distributions.Univariate;
using Accord;
using Shared;

namespace CurrentVersion
{
    [Config(typeof(FastAndDirtyConfig))]
    public class KernelSupportVectorMachineTest
    {
        private readonly double[][] inputs;
        private readonly int[] outputs;

        SupportVectorMachine<Polynomial> ksvm;
        SequentialMinimalOptimization<Polynomial> smo;

        public KernelSupportVectorMachineTest()
        {
            var data = Shared.Examples.YinYang();
            inputs = data.Training.Inputs;
            outputs = data.Training.Output;
        }

        [Setup]
        public void Setup()
        {
            ksvm = new SupportVectorMachine<Polynomial>(inputs: 2, kernel: new Polynomial(2));
            smo = new SequentialMinimalOptimization<Polynomial>()
            {
                Model = ksvm
            };
        }

        [Benchmark]
        public IntRange Accord_Core()
        {
            return new IntRange(0, 1);
        }

        [Benchmark]
        public Cosine Accord_Math()
        {
            return new Cosine();
        }

        [Benchmark]
        public NormalDistribution Accord_Statistics()
        {
            return new NormalDistribution();
        }

        [Benchmark]
        public SupportVectorMachine<Polynomial> v3_1_0()
        {
            ksvm = new SupportVectorMachine<Polynomial>(inputs: 2, kernel: new Polynomial(2));
            smo = new SequentialMinimalOptimization<Polynomial>()
            {
                Model = ksvm
            };
            smo.Learn(inputs, outputs);
            return ksvm;
        }

    }
}