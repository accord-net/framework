using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreviousVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<KernelSupportVectorMachineTest>();
            BenchmarkRunner.Run<MulticlassSupportVectorMachineTest>();
            //new MulticlassSupportVectorMachineTest().v3_0_1();
            //Console.ReadLine();
        }
    }
}
