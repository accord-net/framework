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
    public class MulticlassSupportVectorMachineTest
    {
        private readonly Problem problem;

        MulticlassSupportVectorMachine<Polynomial> ksvm;
        MulticlassSupportVectorLearning<Polynomial> smo;

        public MulticlassSupportVectorMachineTest()
        {
            problem = Shared.Examples.KaggleDigits();
        }

        [Setup]
        public void Setup()
        {
            ksvm = new MulticlassSupportVectorMachine<Polynomial>(
                inputs: 2, kernel: new Polynomial(2), classes: 10);

            smo = new MulticlassSupportVectorLearning<Polynomial>()
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
        public MulticlassSupportVectorMachine<Polynomial> v3_1_0()
        {
            ksvm = new MulticlassSupportVectorMachine<Polynomial>(
                inputs: 2, kernel: new Polynomial(2), classes: 10);

            smo = new MulticlassSupportVectorLearning<Polynomial>()
            {
                Model = ksvm
            };

            smo.Learn(problem.Training.Inputs, problem.Testing.Output);
            return ksvm;
        }

    }
}