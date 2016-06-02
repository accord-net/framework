using Accord.Statistics.Kernels;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrentVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<KernelSupportVectorMachineTest>();
            BenchmarkRunner.Run<MulticlassSupportVectorMachineTest>();
            //MulticlassSupportVectorMachineTest().v3_1_0();
            //Console.ReadLine();
        }

        
    }
}
