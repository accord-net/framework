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

    public class LbfgsbComparer
    {
        private List<OptimizationProgressEventArgs> actual;

        public double factr;
        public double[] l;
        public double[] u;
        public double pgtol;
        public int m = 5;
        public int max_iterations;

        public string ActualMessage;

        public LbfgsbComparer()
        {
            actual = new List<OptimizationProgressEventArgs>();
        }

        public Info[] Expected(Specification problem)
        {
            NativeCode = String.Empty;

            Function function = problem.Function.Invoke;
            Gradient gradient = problem.Gradient.Invoke;

            if (l == null)
            {
                l = new double[problem.Start.Length];
                for (int i = 0; i < l.Length; i++)
                    l[i] = Double.NegativeInfinity;
            }

            if (u == null)
            {
                u = new double[problem.Start.Length];
                for (int i = 0; i < l.Length; i++)
                    u[i] = Double.PositiveInfinity;
            }

            Param2 param = new Param2()
            {
                factr = factr,
                l = l,
                u = u,
                pgtol = pgtol,
                m = m,
                max_iterations = max_iterations
            };

            NativeCode = Wrapper.Lbfgsb3((double[])problem.Start.Clone(), function, gradient, param).Trim();

            return Wrapper.list.ToArray();
        }

        public string NativeCode { get; private set; }

        public OptimizationProgressEventArgs[] Actual(Specification problem)
        {
            ActualMessage = String.Empty;

            BoundedBroydenFletcherGoldfarbShanno target =
                new BoundedBroydenFletcherGoldfarbShanno(problem.Variables)
            {
                FunctionTolerance = factr,
                GradientTolerance = pgtol,
                Corrections = m,
                MaxIterations = max_iterations
            };

            for (int i = 0; i < target.LowerBounds.Length; i++)
            {
                if (l != null)
                    target.LowerBounds[i] = l[i];
                if (u != null)
                    target.UpperBounds[i] = u[i];
            }


            target.Function = problem.Function;
            target.Gradient = problem.Gradient;

            actual.Clear();
            target.Progress += new EventHandler<OptimizationProgressEventArgs>(target_Progress);

            target.Minimize((double[])problem.Start.Clone());

            if (target.Status == BoundedBroydenFletcherGoldfarbShannoStatus.GradientConvergence)
                ActualMessage = "CONVERGENCE: NORM_OF_PROJECTED_GRADIENT_<=_PGTOL";
            else if (target.Status == BoundedBroydenFletcherGoldfarbShannoStatus.FunctionConvergence)
                ActualMessage = "CONVERGENCE: REL_REDUCTION_OF_F_<=_FACTR*EPSMCH";
            else if (target.Status == BoundedBroydenFletcherGoldfarbShannoStatus.LineSearchFailed)
                ActualMessage = "ABNORMAL_TERMINATION_IN_LNSRCH";

            return actual.ToArray();
        }

        void target_Progress(object sender, OptimizationProgressEventArgs e)
        {
            actual.Add(e);
        }
    }
}
