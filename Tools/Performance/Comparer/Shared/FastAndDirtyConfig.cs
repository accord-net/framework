using System;
using System.Linq;
using System.Collections.Generic;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace Shared
{
    public class FastAndDirtyConfig : ManualConfig
    {
        public FastAndDirtyConfig()
        {
            Add(Job.Default
                .WithLaunchCount(1)      
                .WithIterationTime(100) 
                .WithWarmupCount(100)
                .WithTargetCount(100)    
            );
        }
    }
}