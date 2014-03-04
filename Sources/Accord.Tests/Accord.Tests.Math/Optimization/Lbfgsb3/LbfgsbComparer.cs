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

        public BoundSpec bound;
        public double factr;
        public double[] l;
        public double[] u;
        public double pgtol;
        public int m;
        public int max_iterations;

        public string ActualMessage;

        public LbfgsbComparer()
        {
            actual = new List<OptimizationProgressEventArgs>();
        }

        public Info[] Expected(Specification problem)
        {
            Function function = problem.Function.Invoke;
            Gradient gradient = problem.Gradient.Invoke;

            Param2 param = new Param2()
            {
                bound = bound,
                factr = factr,
                l = l,
                u = u,
                pgtol = pgtol,
                m = m,
                max_iterations = max_iterations
            };

            NativeCode = Wrapper.Lbfgsb3((double[])problem.Start.Clone(), function, gradient, param);

            return Wrapper.list.ToArray();
        }

        public string NativeCode { get; private set; }

        public OptimizationProgressEventArgs[] Actual(Specification problem)
        {
            BoundedBroydenFletcherGoldfarbShanno target = new BoundedBroydenFletcherGoldfarbShanno(problem.Variables)
            {
                Tolerance = factr,
                Precision = pgtol,
                Corrections = 5,
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


            return actual.ToArray();
        }

        void target_Progress(object sender, OptimizationProgressEventArgs e)
        {
            actual.Add(e);
        }
    }
}
