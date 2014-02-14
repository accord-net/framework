// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Steven G. Johnson, 2008
// stevenj@alum.mit.edu
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
//
// The source code presented in this file has been adapted from the
// Augmented Lagrangian method implementation presented in the NLopt
// Numerical Optimization Library. This file is thus licensed under
// the same MIT license as the original. Details are given below.
//
//    Copyright (c) 2007-2011 Massachusetts Institute of Technology
//
//    Permission is hereby granted, free of charge, to any person obtaining
//    a copy of this software and associated documentation files (the
//    "Software"), to deal in the Software without restriction, including
//    without limitation the rights to use, copy, modify, merge, publish,
//    distribute, sublicense, and/or sell copies of the Software, and to
//    permit persons to whom the Software is furnished to do so, subject to
//    the following conditions:
// 
//    The above copyright notice and this permission notice shall be
//    included in all copies or substantial portions of the Software.
// 
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//

namespace Accord.Math.Optimization
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Augmented Lagrangian method for constrained non-linear optimization.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://ab-initio.mit.edu/nlopt">
    ///       Steven G. Johnson, The NLopt nonlinear-optimization package, http://ab-initio.mit.edu/nlopt </a></description></item>
    ///     <item><description><a href="http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.72.6121">
    ///       E. G. Birgin and J. M. Martinez, "Improving ultimate convergence of an augmented Lagrangian
    ///       method," Optimization Methods and Software vol. 23, no. 2, p. 177-195 (2008). </a></description></item>
    ///   </list>
    /// </para>   
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   In this framework, it is possible to state a non-linear programming problem
    ///   using either symbolic processing or vector-valued functions. The following 
    ///   example demonstrates the former.</para>
    ///   
    /// <code>
    /// // Suppose we would like to minimize the following function:
    /// //
    /// //    f(x,y) = min 100(y-x²)²+(1-x)²
    /// //
    /// // Subject to the constraints
    /// //
    /// //    x >= 0  (x must be positive)
    /// //    y >= 0  (y must be positive)
    /// //
    ///
    /// // In this example we will be using some symbolic processing. 
    /// // The following variables could be initialized to any value.
    ///
    /// double x = 0, y = 0;
    ///
    ///
    /// // First, we create our objective function
    /// var f = new NonlinearObjectiveFunction(
    ///
    ///     // This is the objective function:  f(x,y) = min 100(y-x²)²+(1-x)²
    ///     function: () => 100 * Math.Pow(y - x * x, 2) + Math.Pow(1 - x, 2),
    ///
    ///     // The gradient vector:
    ///     gradient: () => new[] 
    ///     {
    ///         2 * (200 * Math.Pow(x, 3) - 200 * x * y + x - 1), // df/dx = 2(200x³-200xy+x-1)
    ///         200 * (y - x*x)                                   // df/dy = 200(y-x²)
    ///     }
    ///
    /// );
    ///
    ///
    /// // Now we can start stating the constraints
    /// var constraints = new List&lt;NonlinearConstraint>();
    ///
    /// // Add the non-negativity constraint for x
    /// constraints.Add(new NonlinearConstraint(f,
    ///
    ///     // 1st constraint: x should be greater than or equal to 0
    ///     function: () => x, shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0,
    ///
    ///     gradient: () => new[] { 1.0, 0.0 }
    /// ));
    ///
    /// // Add the non-negativity constraint for y
    /// constraints.Add(new NonlinearConstraint(f,
    ///
    ///     // 2nd constraint: y should be greater than or equal to 0
    ///     function: () => y, shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0,
    ///
    ///     gradient: () => new[] { 0.0, 1.0 }
    /// ));
    ///
    ///
    /// // Finally, we create the non-linear programming solver
    /// var solver = new AugmentedLagrangianSolver(2, constraints);
    ///
    /// // And attempt to solve the problem
    /// double minValue = solver.Minimize(f);
    /// </code>
    /// </example>
    /// 
    public class AugmentedLagrangianSolver
    {

        IGradientOptimizationMethod dualSolver;

        NonlinearConstraint[] lesserThanConstraints;
        NonlinearConstraint[] greaterThanConstraints;
        NonlinearConstraint[] equalityConstraints;


        private double rho;
        private double rhoMax = 1e+1;
        private double rhoMin = 1e-6;

        private double[] lambda; // equality multipliers
        private double[] mu;     // "lesser than"  inequality multipliers
        private double[] nu;     // "greater than" inequality multipliers

        private int numberOfVariables;


        // Stopping criteria
        private double ftol_abs = 0;
        private double ftol_rel = 1e-10;
        private double xtol_rel = 1e-10;

        private int functionEvaluations;
        private int maxEvaluations;
        private int iterations;


        /// <summary>
        ///   Gets the number of variables (free parameters)
        ///   in the optimization problem.
        /// </summary>
        /// 
        /// <value>The number of parameters.</value>
        /// 
        public int Parameters
        {
            get { return numberOfVariables; }
        }

        /// <summary>
        ///   Gets the number of iterations performed in the last
        ///   call to <see cref="Minimize(NonlinearObjectiveFunction)"/>.
        /// </summary>
        /// 
        /// <value>
        ///   The number of iterations performed
        ///   in the previous optimization.</value>
        ///   
        public int Iterations
        {
            get { return iterations; }
        }

        /// <summary>
        ///   Gets the number of function evaluations performed
        ///   in the last call to <see cref="Minimize(NonlinearObjectiveFunction)"/>.
        /// </summary>
        /// 
        /// <value>
        ///   The number of evaluations performed
        ///   in the previous optimization.</value>
        ///   
        public int Evaluations
        {
            get { return functionEvaluations; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of evaluations
        ///   to be performed during optimization. Default
        ///   is 0 (evaluate until convergence).
        /// </summary>
        /// 
        public int MaxEvaluations
        {
            get { return maxEvaluations; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("value");
                maxEvaluations = value;
            }
        }

        /// <summary>
        ///   Gets or sets the function to be optimized.
        /// </summary>
        /// 
        /// <value>The function to be optimized.</value>
        /// 
        public Func<double[], double> Function { get; set; }

        /// <summary>
        ///   Gets or sets a function returning the gradient
        ///   vector of the function to be optimized for a
        ///   given value of its free parameters.
        /// </summary>
        /// 
        /// <value>The gradient function.</value>
        /// 
        public Func<double[], double[]> Gradient { get; set; }

        /// <summary>
        ///   Gets the solution found, the values of the
        ///   parameters which optimizes the function.
        /// </summary>
        /// 
        public double[] Solution { get; private set; }


        /// <summary>
        ///   Creates a new instance of the Augmented Lagrangian algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
        /// <param name="constraints">The <see cref="NonlinearConstraint"/>s to which the solution must be subjected.</param>
        /// 
        public AugmentedLagrangianSolver(int numberOfVariables,
            IEnumerable<NonlinearConstraint> constraints)
        {
            var innerSolver = new BroydenFletcherGoldfarbShanno(numberOfVariables);
            init(numberOfVariables, constraints, innerSolver);
        }

        /// <summary>
        ///   Creates a new instance of the Augmented Lagrangian algorithm.
        /// </summary>
        /// 
        /// <param name="innerSolver">The <see cref="IGradientOptimizationMethod">unconstrained optimization
        ///   method</see> used internally to solve the dual of this optimization problem.</param>
        /// <param name="constraints">The <see cref="NonlinearConstraint"/>s to which the solution must be subjected.</param>
        /// 
        public AugmentedLagrangianSolver(IGradientOptimizationMethod innerSolver,
            IEnumerable<NonlinearConstraint> constraints)
        {
            int numberOfVariables = innerSolver.Parameters;
            init(numberOfVariables, constraints, innerSolver);
        }

        private void init(int numberOfVariables, IEnumerable<NonlinearConstraint> constraints, IGradientOptimizationMethod innerSolver)
        {
            this.numberOfVariables = numberOfVariables;

            List<NonlinearConstraint> equality = new List<NonlinearConstraint>();
            List<NonlinearConstraint> lesserThan = new List<NonlinearConstraint>();
            List<NonlinearConstraint> greaterThan = new List<NonlinearConstraint>();

            foreach (var c in constraints)
            {
                switch (c.ShouldBe)
                {
                    case ConstraintType.EqualTo:
                        equality.Add(c); break;

                    case ConstraintType.GreaterThanOrEqualTo:
                        greaterThan.Add(c); break;

                    case ConstraintType.LesserThanOrEqualTo:
                        lesserThan.Add(c); break;

                    default:
                        throw new ArgumentException("Unknown constraint type.", "constraints");
                }
            }

            this.lesserThanConstraints = lesserThan.ToArray();
            this.greaterThanConstraints = greaterThan.ToArray();
            this.equalityConstraints = equality.ToArray();

            this.Solution = new double[numberOfVariables];

            this.dualSolver = innerSolver;
            dualSolver.Function = objectiveFunction;
            dualSolver.Gradient = objectiveGradient;

            for (int i = 0; i < Solution.Length; i++)
                Solution[i] = Accord.Math.Tools.Random.NextDouble() * 2 - 1;
        }

        /// <summary>
        ///   Minimizes the given function. 
        /// </summary>
        /// 
        /// <param name="function">The function to be minimized.</param>
        /// 
        /// <returns>The minimum value found at the <see cref="Solution"/>.</returns>
        /// 
        public double Minimize(NonlinearObjectiveFunction function)
        {
            if (function.NumberOfVariables != numberOfVariables)
                throw new ArgumentOutOfRangeException("function",
                    "Incorrect number of variables in the objective function. " +
                    "The number of variables must match the number of variables set in the solver.");

            this.Function = function.Function;
            this.Gradient = function.Gradient;

            minimize();

            return Function(Solution);
        }

        /// <summary>
        ///   Minimizes the defined function. 
        /// </summary>
        /// 
        /// <param name="function">The function to be minimized.</param>
        /// <param name="gradient">The gradient of the given <paramref name="function"/>.</param>
        /// 
        /// <returns>The minimum value found at the <see cref="Solution"/>.</returns>
        /// 
        public double Minimize(Func<double[], double> function, Func<double[], double[]> gradient)
        {
            this.Function = function;
            this.Gradient = gradient;

            minimize();

            return Function(Solution);
        }

        /// <summary>
        ///   Maximizes the given function. 
        /// </summary>
        /// 
        /// <param name="function">The function to be maximized.</param>
        /// 
        /// <returns>The maximum value found at the <see cref="Solution"/>.</returns>
        /// 
        public double Maximize(NonlinearObjectiveFunction function)
        {
            if (function.NumberOfVariables != numberOfVariables)
                throw new ArgumentOutOfRangeException("function",
                    "Incorrect number of variables in the objective function. " +
                    "The number of variables must match the number of variables set in the solver.");

            this.Function = x => -function.Function(x);
            this.Gradient = x => function.Gradient(x).Multiply(-1);

            minimize();

            return -Function(Solution);
        }

        /// <summary>
        ///   maximizes the defined function. 
        /// </summary>
        /// 
        /// <param name="function">The function to be maximized.</param>
        /// <param name="gradient">The gradient of the given <paramref name="function"/>.</param>
        /// 
        /// <returns>The maximum value found at the <see cref="Solution"/>.</returns>
        /// 
        public double Maximize(Func<double[], double> function, Func<double[], double[]> gradient)
        {
            this.Function = x => -function(x);
            this.Gradient = x => gradient(x).Multiply(-1);

            minimize();

            return -Function(Solution);
        }

        // Augmented Lagrangian objective
        double objectiveFunction(double[] x)
        {
            // Compute
            //
            //   Phi(x) = f(x) + rho/2 sum(c_i(x)²) - sum(lambda_i * c_i(x))
            //

            double sumOfSquares = 0;
            double weightedSum = 0;
            double rho2 = rho / 2;

            // For each equality constraint
            for (int i = 0; i < equalityConstraints.Length; i++)
            {
                double c = equalityConstraints[i].Function(x) - equalityConstraints[i].Value;

                sumOfSquares += rho2 * c * c;
                weightedSum += lambda[i] * c;
            }

            // For each "lesser than" inequality constraint
            for (int i = 0; i < lesserThanConstraints.Length; i++)
            {
                double c = lesserThanConstraints[i].Function(x) - lesserThanConstraints[i].Value;

                if (c > 0)
                {
                    sumOfSquares += rho2 * c * c;
                    weightedSum += mu[i] * c;
                }
            }

            // For each "greater than" inequality constraint
            for (int i = 0; i < greaterThanConstraints.Length; i++)
            {
                double c = -greaterThanConstraints[i].Function(x) + greaterThanConstraints[i].Value;

                if (c > 0)
                {
                    sumOfSquares += rho2 * c * c;
                    weightedSum += nu[i] * c;
                }
            }

            double phi = Function(x) + sumOfSquares - weightedSum;

            return phi;
        }

        // Augmented Lagrangian gradient 
        double[] objectiveGradient(double[] x)
        {
            // Compute
            //
            //   Phi'(x) = f'(x) + rho sum(c_i(x)*c_i'(x)) - sum(lambda_i * c_i'(x))
            //

            double[] g = Gradient(x);


            double[] sum = new double[x.Length];
            double[] weightedSum = new double[x.Length];

            // For each equality constraint
            for (int i = 0; i < equalityConstraints.Length; i++)
            {
                double c = equalityConstraints[i].Function(x) - equalityConstraints[i].Value;
                double[] cg = equalityConstraints[i].Gradient(x);

                for (int j = 0; j < cg.Length; j++)
                {
                    sum[j] += rho * c * cg[j];
                    weightedSum[j] += lambda[i] * cg[j];
                }
            }

            // For each "lesser than" inequality constraint
            for (int i = 0; i < lesserThanConstraints.Length; i++)
            {
                double c = lesserThanConstraints[i].Function(x) - lesserThanConstraints[i].Value;
                double[] cg = lesserThanConstraints[i].Gradient(x);

                if (c > 0)
                {
                    // Constraint is being violated
                    for (int j = 0; j < cg.Length; j++)
                    {
                        sum[j] += rho * c * cg[j];
                        weightedSum[j] += mu[i] * cg[j];
                    }
                }
            }

            // For each "greater-than" inequality constraint
            for (int i = 0; i < greaterThanConstraints.Length; i++)
            {
                double c = -greaterThanConstraints[i].Function(x) + greaterThanConstraints[i].Value;
                double[] cg = greaterThanConstraints[i].Gradient(x);

                if (c > 0)
                {
                    // Constraint is being violated
                    for (int j = 0; j < cg.Length; j++)
                    {
                        sum[j] += rho * c * -cg[j];
                        weightedSum[j] += nu[i] * -cg[j];
                    }
                }
            }

            for (int i = 0; i < g.Length; i++)
                g[i] += sum[i] - weightedSum[i];

            return g;
        }

        double minimize()
        {
            double ICM = Double.PositiveInfinity;

            double minPenalty = Double.PositiveInfinity;
            double minValue = Double.PositiveInfinity;

            double penalty;
            double currentValue;

            bool minFeasible = false;
            int noProgressCounter = 0;
            int maxCount = 20;
            iterations = 0;

            // magic parameters from Birgin & Martinez
            const double tau = 0.5, gam = 10;
            const double lam_min = -1e20;
            const double lam_max = 1e20;
            const double mu_max = 1e20;
            const double nu_max = 1e20;


            double[] currentSolution = (double[])Solution.Clone();

            lambda = new double[equalityConstraints.Length];
            mu = new double[lesserThanConstraints.Length];
            nu = new double[greaterThanConstraints.Length];
            rho = 1;


            // Starting rho suggested by B & M 
            if (lambda.Length > 0 || mu.Length > 0 || nu.Length > 0)
            {
                double con2 = 0;
                penalty = 0;

                // Evaluate function
                functionEvaluations++;
                currentValue = dualSolver.Function(currentSolution);

                bool feasible = true;

                // For each equality constraint
                for (int i = 0; i < equalityConstraints.Length; i++)
                {
                    double c = equalityConstraints[i].Function(currentSolution) - equalityConstraints[i].Value;

                    penalty += Math.Abs(c);
                    feasible = feasible && Math.Abs(c) <= equalityConstraints[i].Tolerance;
                    con2 += c * c;
                }

                // For each "lesser than" inequality constraint
                for (int i = 0; i < lesserThanConstraints.Length; ++i)
                {
                    double c = lesserThanConstraints[i].Function(currentSolution) - lesserThanConstraints[i].Value;

                    penalty += c > 0 ? c : 0;
                    feasible = feasible && c <= lesserThanConstraints[i].Tolerance;
                    if (c > 0) con2 += c * c;
                }

                // For each "greater than" inequality constraint
                for (int i = 0; i < greaterThanConstraints.Length; ++i)
                {
                    double c = -greaterThanConstraints[i].Function(currentSolution) + greaterThanConstraints[i].Value;

                    penalty += c > 0 ? c : 0;
                    feasible = feasible && c <= greaterThanConstraints[i].Tolerance;
                    if (c > 0) con2 += c * c;
                }

                minValue = currentValue;
                minPenalty = penalty;
                minFeasible = feasible;


                double num = 2.0 * Math.Abs(minValue);
            
                if (num < 1e-300)
                    rho = rhoMin;

                else if (con2 < 1e-300) 
                    rho = rhoMax;

                else
                {
                    rho = num / con2;
                    if (rho < rhoMin)
                        rho = rhoMin;
                    else if (rho > rhoMax)
                        rho = rhoMax;
                }

                System.Diagnostics.Debug.Assert(!Double.IsNaN(rho));
            }


            while (true)
            {
                double prevICM = ICM;

                try
                {
                    // Minimize the dual problem using current solution
                    currentValue = dualSolver.Minimize(currentSolution);
                }
                catch (LineSearchFailedException)
                {
                }
                catch (NotFiniteNumberException)
                {
                }

                // Retrieve the solution found
                currentSolution = dualSolver.Solution;

                // Evaluate function
                functionEvaluations++;
                currentValue = dualSolver.Function(currentSolution);

                ICM = 0;
                penalty = 0;
                bool feasible = true;

                // Update lambdas
                for (int i = 0; i < equalityConstraints.Length; i++)
                {
                    double c = equalityConstraints[i].Function(currentSolution) - equalityConstraints[i].Value;

                    double newLambda = lambda[i] + rho * c;
                    penalty += Math.Abs(c);
                    feasible = feasible && Math.Abs(c) <= equalityConstraints[i].Tolerance;
                    ICM = Math.Max(ICM, Math.Abs(c));
                    lambda[i] = Math.Min(Math.Max(lam_min, newLambda), lam_max);
                }

                // Update mus
                for (int i = 0; i < lesserThanConstraints.Length; i++)
                {
                    double c = lesserThanConstraints[i].Function(currentSolution) - lesserThanConstraints[i].Value;

                    double newMu = mu[i] + rho * c;
                    penalty += c > 0 ? c : 0;
                    feasible = feasible && c <= lesserThanConstraints[i].Tolerance;
                    ICM = Math.Max(ICM, Math.Abs(Math.Max(c, -mu[i] / rho)));
                    mu[i] = Math.Min(Math.Max(0.0, newMu), mu_max);
                }

                // Update nus
                for (int i = 0; i < greaterThanConstraints.Length; i++)
                {
                    double c = -greaterThanConstraints[i].Function(currentSolution) + greaterThanConstraints[i].Value;

                    double newNu = nu[i] + rho * c;
                    penalty += c > 0 ? c : 0;
                    feasible = feasible && c <= greaterThanConstraints[i].Tolerance;
                    ICM = Math.Max(ICM, Math.Abs(Math.Max(c, -nu[i] / rho)));
                    nu[i] = Math.Min(Math.Max(0.0, newNu), nu_max);
                }

                // Update rho
                if (ICM > tau * prevICM)
                {
                    rho *= gam;
                }


                // Check if we should stop
                if (
                      (feasible &&
                         (!minFeasible || penalty < minPenalty || currentValue < minValue)
                      ) || (!minFeasible && penalty < minPenalty)
                    )
                {
                    if (feasible)
                    {
                        if (relstop(minValue, currentValue, ftol_rel, ftol_abs))
                            return minValue;

                        bool xtolreach = true;
                        for (int i = 0; i < currentSolution.Length; i++)
                            if (!relstop(Solution[i], currentSolution[i], xtol_rel, 0))
                                xtolreach = false;
                        if (xtolreach)
                            return minValue;
                    }

                    minValue = currentValue;
                    minPenalty = penalty;
                    minFeasible = feasible;

                    // Save the current solution 
                    for (int i = 0; i < Solution.Length; i++)
                        Solution[i] = currentSolution[i];

                    noProgressCounter = 0;
                }
                else
                {
                    if (ICM == 0)
                        return minValue;

                    noProgressCounter++;

                    if (noProgressCounter > maxCount)
                        return minValue;
                }


                // Go to next iteration
                iterations++;

                if (maxEvaluations > 0 && functionEvaluations >= maxEvaluations)
                    return minValue;

            }

        }

        static bool relstop(double vold, double vnew, double reltol, double abstol)
        {
            if (Double.IsInfinity(vold))
                return false;

            return (Math.Abs(vnew - vold) < abstol
               || Math.Abs(vnew - vold) < reltol * (Math.Abs(vnew) + Math.Abs(vold)) * 0.5
               || (reltol > 0 && vnew == vold)); // catch vnew == vold == 0 
        }

    }
}
