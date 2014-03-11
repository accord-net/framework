using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AccordTestsMathCpp2;
using Accord.Math.Optimization;

namespace Accord.Tests.Math.Optimization
{
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
                Tolerance = factr,
                Precision = pgtol,
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

            double v = target.Minimize((double[])problem.Start.Clone());

            if (target.Status == BoundedBroydenFletcherGoldfarbShanno.Code.ConvergenceGradient)
                ActualMessage = "CONVERGENCE: NORM_OF_PROJECTED_GRADIENT_<=_PGTOL";
            else if (target.Status == BoundedBroydenFletcherGoldfarbShanno.Code.Convergence)
                ActualMessage = "CONVERGENCE: REL_REDUCTION_OF_F_<=_FACTR*EPSMCH";
            else if (target.Status == BoundedBroydenFletcherGoldfarbShanno.Code.ABNORMAL_TERMINATION_IN_LNSRCH)
                ActualMessage = "ABNORMAL_TERMINATION_IN_LNSRCH";

            return actual.ToArray();
        }

        void target_Progress(object sender, OptimizationProgressEventArgs e)
        {
            actual.Add(e);
        }
    }
}
