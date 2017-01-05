// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Tests.Interop.Math
{
    using Accord.Math.Optimization;
    using AccordTestsMathCpp2;
    using System;
    using System.Collections.Generic;

    public class Specification
    {
        public Specification(int n, Func<double[], double> function, 
            Func<double[], double[]> gradient, double[] start)
        {
            this.Variables = n;
            this.Function = function;
            this.Gradient = gradient;
            this.Start = start;

            if (start == null)
                this.Start = new double[n];
        }

        public int Variables;
        public Func<double[], double> Function;
        public Func<double[], double[]> Gradient;
        public double[] Start;
    }

    public class LBFGSComparer
    {
        private List<OptimizationProgressEventArgs> actual;

        public int m = 6;
        public double epsilon = 1e-5;
        public int past = 0;
        public double delta = 1e-5;
        public int max_iterations = 0;
        public LineSearch linesearch = LineSearch.Default;
        public int max_linesearch = 40;
        public double min_step = 1e-20;
        public double max_step = 1e20;
        public double ftol = 1e-4;
        public double wolfe = 0.9;
        public double gtol = 0.9;
        public double xtol = 1.0e-16;
        public double orthantwise_c = 0;
        public int orthantwise_start = 0;
        public int orthantwise_end = -1;

        public string ActualMessage;

        public LBFGSComparer()
        {
            actual = new List<OptimizationProgressEventArgs>();
        }

        public Info[] Expected(Specification problem)
        {
            Function function = problem.Function.Invoke;
            Gradient gradient = problem.Gradient.Invoke;

            Param param = new Param()
            {
                m = m,
                epsilon = epsilon,
                past = past,
                delta = delta,
                max_iterations = max_iterations,
                linesearch = (int)linesearch,
                max_linesearch = max_linesearch,
                min_step = min_step,
                max_step = max_step,
                ftol = ftol,
                wolfe = wolfe,
                gtol = gtol,
                xtol = xtol,
                orthantwise_c = orthantwise_c,
                orthantwise_start = orthantwise_start,
                orthantwise_end = orthantwise_end
            };

            NativeCode = Wrapper.Libbfgs((double[])problem.Start.Clone(), function, gradient, param);

            // Convergence and success have the same
            // enumeration value in the original code
            if (NativeCode == "LBFGS_CONVERGENCE")
                NativeCode = "LBFGS_SUCCESS";

            return Wrapper.list.ToArray();
        }

        public string NativeCode { get; private set; }

        public OptimizationProgressEventArgs[] Actual(Specification problem)
        {
            BroydenFletcherGoldfarbShanno target = new BroydenFletcherGoldfarbShanno(problem.Variables)
            {
                Corrections = m,
                Epsilon = epsilon,
                Past = past,
                Delta = delta,
                MaxIterations = max_iterations,
                LineSearch = (LineSearch)linesearch,
                MaxLineSearch = max_linesearch,
                MinStep = min_step,
                MaxStep = max_step,
                ParameterTolerance = ftol,
                Wolfe = wolfe,
                GradientTolerance = gtol,
                FunctionTolerance = xtol,
                OrthantwiseC = orthantwise_c,
                OrthantwiseStart = orthantwise_start,
                OrthantwiseEnd = orthantwise_end
            };

            target.Function = problem.Function;
            target.Gradient = problem.Gradient;

            actual.Clear();
            target.Progress += new EventHandler<OptimizationProgressEventArgs>(target_Progress);

            target.Minimize((double[])problem.Start.Clone());

            ActualMessage = target.Status.GetDescription();


            return actual.ToArray();
        }

        void target_Progress(object sender, OptimizationProgressEventArgs e)
        {
            actual.Add(e);
        }
    }
}
