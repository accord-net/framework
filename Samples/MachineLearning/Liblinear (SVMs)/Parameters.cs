using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.IO;

namespace Liblinear
{
    public class Parameters
    {
        public LibSvmSolverType Solver { get; set; }

        /* these are for training only */
        public double Tolerance { get; set; }
        public double Complexity { get; set; }

        public List<double> ClassWeights { get; set; }
        public List<int> ClassLabels { get; set; }

        /// <summary>
        ///   Epsilon in Support Vector Regression (SVR). Default is 0.1.
        /// </summary>
        /// 
        public double Epsilon { get; set; }

        public double Bias { get; set; }

        public bool CrossValidation { get; set; }

        public int ValidationFolds { get; set; }
    };
}
