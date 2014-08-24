using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liblinear
{
    public class Problem
    {
        public int Dimensions { get; set; }
        public double[] Outputs { get; set; }
        public double[][] Inputs { get; set; }
    }
}
