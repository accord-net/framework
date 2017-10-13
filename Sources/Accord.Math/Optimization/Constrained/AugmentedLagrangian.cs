// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Steven G. Johnson, 2008
// stevenj@alum.mit.edu
//
// Copyright © César Souza, 2009-2017
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
    ///   Status codes for the <see cref="AugmentedLagrangian"/> 
    ///   optimization algorithm.
    /// </summary>
    /// 
    public enum AugmentedLagrangianStatus
    {
        /// <summary>
        ///   The algorithm has found a feasible solution.
        /// </summary>
        /// 
        Converged = 1,

        /// <summary>
        ///   The optimization could not make progress towards finding a feasible
        ///   solution. Try increasing the <see cref="IConstraint.Tolerance"/>
        ///   of the constraints.
        /// </summary>
        /// 
        NoProgress = -1,

        /// <summary>
        ///   The optimization has reached the <see cref="AugmentedLagrangian.MaxEvaluations">
        ///   maximum number of function evaluations</see>.
        /// </summary>
        /// 
        MaxEvaluations = -2,

        /// <summary>
        ///   The optimization has been cancelled by the user (such as for
        ///   example by using <see cref="BaseOptimizationMethod.Token"/>).
        /// </summary>
        /// 
        Cancelled = -3
    }

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
    ///   example demonstrates the symbolic processing case:</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Math\Optimization\AugmentedLagrangianTest.cs" region="doc_lambda"/>
    /// 
    /// <para>
    ///   And this is the same example as before, but using standard vectors instead.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Math\Optimization\AugmentedLagrangianTest.cs" region="doc_vector"/>
    /// </example>
    /// 
    /// <seealso cref="GoldfarbIdnani"/>
    /// 
    public class AugmentedLagrangian : BaseGradientOptimizationMethod, IGradientOptimizationMethod, IOptimizationMethod<AugmentedLagrangianStatus>
    {

        IGradientOptimizationMethod dualSolver;

        IConstraint[] lesserThanConstraints;
        IConstraint[] greaterThanConstraints;
        IConstraint[] equalityConstraints;


        private double rho;
        private double rhoMax = 1e+1;
        private double rhoMin = 1e-6;

        private double[] lambda; // equality multipliers
        private double[] mu;     // "lesser than"  inequality multipliers
        private double[] nu;     // "greater than" inequality multipliers

        private double[] g;

        // Stopping criteria
        private double ftol_abs = 0;
        private double ftol_rel = 1e-10;
        private double xtol_rel = 1e-10;

        private int functionEvaluations;
        private int maxEvaluations;
        private int iterations;

        /// <summary>
        ///   Get the exit code returned in the last call to the
        ///   <see cref="IOptimizationMethod{TInput, TOutput}.Maximize()"/> or 
        ///   <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()"/> methods.
        /// </summary>
        /// 
        public AugmentedLagrangianStatus Status { get; private set; }

        /// <summary>
        ///   Gets the number of iterations performed in the
        ///   last call to the <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()"/> or
        ///   <see cref="IOptimizationMethod{TInput, TOutput}.Maximize"/> methods.
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
        ///   in the last call to the <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()"/> or
        ///   <see cref="IOptimizationMethod{TInput, TOutput}.Maximize"/> methods.
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
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                maxEvaluations = value;
            }
        }

        /// <summary>
        ///   Gets the inner dual problem optimization algorithm.
        /// </summary>
        /// 
        public IGradientOptimizationMethod Optimizer { get { return dualSolver; } }

        /// <summary>
        ///   Creates a new instance of the Augmented Lagrangian algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
        /// <param name="constraints">
        ///   The <see cref="IConstraint"/>s to which the solution must be subjected.</param>
        /// 
        public AugmentedLagrangian(int numberOfVariables, IEnumerable<IConstraint> constraints)
            : base(numberOfVariables)
        {
            init(null, constraints, null);
        }

        /// <summary>
        ///   Creates a new instance of the Augmented Lagrangian algorithm.
        /// </summary>
        /// 
        /// <param name="function">The objective function to be optimized.</param>
        /// <param name="constraints">
        ///   The <see cref="IConstraint"/>s to which the solution must be subjected.</param>
        /// 
        public AugmentedLagrangian(NonlinearObjectiveFunction function, IEnumerable<IConstraint> constraints)
            : base(function.NumberOfVariables)
        {
            init(function, constraints, null);
        }

        /// <summary>
        ///   Creates a new instance of the Augmented Lagrangian algorithm.
        /// </summary>
        /// 
        /// <param name="innerSolver">The <see cref="IGradientOptimizationMethod">unconstrained 
        ///   optimization method</see> used internally to solve the dual of this optimization 
        ///   problem.</param>
        /// <param name="function">The objective function to be optimized.</param>
        /// <param name="constraints">
        ///   The <see cref="IConstraint"/>s to which the solution must be subjected.</param>
        /// 
        public AugmentedLagrangian(IGradientOptimizationMethod innerSolver,
            NonlinearObjectiveFunction function, IEnumerable<IConstraint> constraints)
            : base(innerSolver.NumberOfVariables)
        {
            if (innerSolver.NumberOfVariables != function.NumberOfVariables)
                throw new ArgumentException("The inner unconstrained optimization algorithm and the "
                    + "objective function should have the same number of variables.", "function");

            init(function, constraints, innerSolver);
        }

        /// <summary>
        ///   Creates a new instance of the Augmented Lagrangian algorithm.
        /// </summary>
        /// 
        /// <param name="innerSolver">The <see cref="IGradientOptimizationMethod">unconstrained 
        ///   optimization method</see> used internally to solve the dual of this optimization 
        ///   problem.</param>
        /// <param name="constraints">
        ///   The <see cref="IConstraint"/>s to which the solution must be subjected.</param>
        /// 
        public AugmentedLagrangian(IGradientOptimizationMethod innerSolver, IEnumerable<IConstraint> constraints)
            : base(innerSolver.NumberOfVariables)
        {
            init(null, constraints, innerSolver);
        }


        private void init(NonlinearObjectiveFunction function,
            IEnumerable<IConstraint> constraints, IGradientOptimizationMethod innerSolver)
        {
            if (function != null)
            {
                if (function.NumberOfVariables != NumberOfVariables)
                {
                    throw new ArgumentOutOfRangeException("function",
                        "Incorrect number of variables in the objective function. " +
                        "The number of variables must match the number of variables set in the solver.");
                }

                this.Function = function.Function;
                this.Gradient = function.Gradient;
            }

            if (innerSolver == null)
            {
                innerSolver = new BroydenFletcherGoldfarbShanno(NumberOfVariables)
                {
                    LineSearch = Optimization.LineSearch.BacktrackingArmijo,
                    Corrections = 10,
                    Epsilon = 1e-10,
                    MaxIterations = 100000
                };
            }

            var equality = new List<IConstraint>();
            var lesserThan = new List<IConstraint>();
            var greaterThan = new List<IConstraint>();

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

            this.lambda = new double[equalityConstraints.Length];
            this.mu = new double[lesserThanConstraints.Length];
            this.nu = new double[greaterThanConstraints.Length];

            this.g = new double[NumberOfVariables];

            this.dualSolver = innerSolver;
            dualSolver.Function = objectiveFunction;
            dualSolver.Gradient = objectiveGradient;
        }



        // Augmented Lagrangian objective
        double objectiveFunction(double[] x)
        {
            // Compute
            //
            //   Phi(x) = f(x) + rho/2 sum(c_i(x)²) - sum(lambda_i * c_i(x))
            //

            double phi = Function(x);
            double rho2 = rho / 2.0;

            // For each equality constraint
            for (int i = 0; i < equalityConstraints.Length; i++)
            {
                double actual = equalityConstraints[i].Function(x);
                double c = actual - equalityConstraints[i].Value;

                phi += rho2 * c * c; // - lambda[i] * c;
            }

            // For each "lesser than" inequality constraint
            for (int i = 0; i < lesserThanConstraints.Length; i++)
            {
                double actual = lesserThanConstraints[i].Function(x);
                double c = actual - lesserThanConstraints[i].Value;

                if (c > 0)
                    phi += rho2 * c * c; // - mu[i] * c;
            }

            // For each "greater than" inequality constraint
            for (int i = 0; i < greaterThanConstraints.Length; i++)
            {
                double actual = greaterThanConstraints[i].Function(x);
                double c = greaterThanConstraints[i].Value - actual;

                if (c > 0)
                    phi += rho2 * c * c; // - nu[i] * c;
            }

            return phi;
        }

        // Augmented Lagrangian gradient 
        double[] objectiveGradient(double[] x)
        {
            // Compute
            //
            //   Phi'(x) = f'(x) + rho sum(c_i(x)*c_i'(x)) - sum(lambda_i * c_i'(x))
            //

            double[] orig = Gradient(x);
            for (int i = 0; i < g.Length; i++)
                g[i] = orig[i];

            // For each equality constraint
            for (int i = 0; i < equalityConstraints.Length; i++)
            {
                double actual = equalityConstraints[i].Function(x);
                double c = actual - equalityConstraints[i].Value;
                double[] cg = equalityConstraints[i].Gradient(x);

                for (int j = 0; j < cg.Length; j++)
                    g[j] += rho * c * cg[j]; // - lambda[i] * cg[j];
            }

            // For each "lesser than" inequality constraint
            for (int i = 0; i < lesserThanConstraints.Length; i++)
            {
                double actual = lesserThanConstraints[i].Function(x);
                double c = actual - lesserThanConstraints[i].Value;
                double[] cg = lesserThanConstraints[i].Gradient(x);

                if (c > 0)
                {
                    // Constraint is being violated
                    for (int j = 0; j < cg.Length; j++)
                        g[j] += rho * c * cg[j]; //- mu[i] * cg[j];
                }
            }

            // For each "greater-than" inequality constraint
            for (int i = 0; i < greaterThanConstraints.Length; i++)
            {
                double actual = greaterThanConstraints[i].Function(x);
                double c = greaterThanConstraints[i].Value - actual;
                double[] cg = greaterThanConstraints[i].Gradient(x);

                if (c > 0)
                {
                    // Constraint is being violated
                    for (int j = 0; j < cg.Length; j++)
                        g[j] += rho * c * -cg[j]; //- nu[i] * -cg[j];
                }
            }

            return g;
        }

        /// <summary>
        ///   Implements the actual optimization algorithm. This
        ///   method should try to minimize the objective function.
        /// </summary>
        /// 
        protected override bool Optimize()
        {
            Status = optimize();

            return Status == AugmentedLagrangianStatus.Converged;
        }

        private AugmentedLagrangianStatus optimize()
        {
            double ICM = Double.PositiveInfinity;

            double minPenalty = Double.PositiveInfinity;
            double minValue = Double.PositiveInfinity;

            double penalty;
            double currentValue;

            bool minFeasible = false;
            int noProgressCounter = 0;
            int maxCount = 100;
            iterations = 0;
            functionEvaluations = 0;

            // magic parameters from Birgin & Martinez
            const double tau = 0.5, gam = 10;
            const double lam_min = -1e20;
            const double lam_max = 1e20;
            const double mu_max = 1e20;
            const double nu_max = 1e20;


            double[] currentSolution = Solution.Copy();

            Array.Clear(lambda, 0, lambda.Length);
            Array.Clear(mu, 0, mu.Length);
            Array.Clear(nu, 0, nu.Length);
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
                    double actual = equalityConstraints[i].Function(currentSolution);
                    double c = actual - equalityConstraints[i].Value;

                    penalty += Math.Abs(c);
                    con2 += c * c;

                    feasible = feasible && Math.Abs(c) <= equalityConstraints[i].Tolerance;
                }

                // For each "lesser than" inequality constraint
                for (int i = 0; i < lesserThanConstraints.Length; i++)
                {
                    double actual = lesserThanConstraints[i].Function(currentSolution);
                    double c = actual - lesserThanConstraints[i].Value;

                    if (c > 0)
                    {
                        penalty += c;
                        con2 += c * c;
                    }

                    feasible = feasible && c <= lesserThanConstraints[i].Tolerance;
                }

                // For each "greater than" inequality constraint
                for (int i = 0; i < greaterThanConstraints.Length; i++)
                {
                    double actual = greaterThanConstraints[i].Function(currentSolution);
                    double c = greaterThanConstraints[i].Value - actual;
                    if (c > 0)
                    {
                        penalty += c;
                        con2 += c * c;
                    }

                    feasible = feasible && c <= greaterThanConstraints[i].Tolerance;
                }

                minValue = currentValue;
                minPenalty = penalty;
                minFeasible = feasible;


                double num = 2.0 * Math.Abs(minValue);

                if (num < 1e-300)
                {
                    rho = rhoMin;
                }
                else if (con2 < 1e-300)
                {
                    rho = rhoMax;
                }
                else
                {
                    rho = num / con2;
                    if (rho < rhoMin)
                        rho = rhoMin;
                    else if (rho > rhoMax)
                        rho = rhoMax;
                }

                Accord.Diagnostics.Debug.Assert(!Double.IsNaN(rho));
            }


            while (true)
            {
                if (Token.IsCancellationRequested)
                    return AugmentedLagrangianStatus.Cancelled;

                double prevICM = ICM;

                // Minimize the dual problem using current solution
                for (int i = 0; i < dualSolver.Solution.Length; i++)
                    dualSolver.Solution[i] = currentSolution[i];

                dualSolver.Minimize();

                // Retrieve the solution found
                for (int i = 0; i < currentSolution.Length; i++)
                {
                    if (!Double.IsNaN(dualSolver.Solution[i]))
                        currentSolution[i] = dualSolver.Solution[i];
                }

                // Evaluate function
                functionEvaluations++;
                currentValue = dualSolver.Function(currentSolution);

                ICM = 0;
                penalty = 0;
                bool feasible = true;

                // Update lambdas
                for (int i = 0; i < equalityConstraints.Length; i++)
                {
                    double actual = equalityConstraints[i].Function(currentSolution);
                    double c = actual - equalityConstraints[i].Value;

                    double newLambda = lambda[i] + rho * c;
                    penalty += Math.Abs(c);
                    feasible = feasible && Math.Abs(c) <= equalityConstraints[i].Tolerance;
                    ICM = Math.Max(ICM, Math.Abs(c));
                    lambda[i] = Math.Min(Math.Max(lam_min, newLambda), lam_max);
                }

                // Update mus
                for (int i = 0; i < lesserThanConstraints.Length; i++)
                {
                    double actual = lesserThanConstraints[i].Function(currentSolution);
                    double c = actual - lesserThanConstraints[i].Value;

                    double newMu = mu[i] + rho * c;
                    penalty += c > 0 ? c : 0;
                    feasible = feasible && c <= lesserThanConstraints[i].Tolerance;
                    ICM = Math.Max(ICM, Math.Abs(Math.Max(c, -mu[i] / rho)));
                    mu[i] = Math.Min(Math.Max(0.0, newMu), mu_max);
                }

                // Update nus
                for (int i = 0; i < greaterThanConstraints.Length; i++)
                {
                    double actual = greaterThanConstraints[i].Function(currentSolution);
                    double c = greaterThanConstraints[i].Value - actual;

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
                bool a = !minFeasible || penalty < minPenalty || currentValue < minValue;
                bool b = !minFeasible && penalty < minPenalty;
                if ((feasible && a) || b)
                {
                    AugmentedLagrangianStatus? r = null;

                    if (feasible)
                    {
                        if (relstop(minValue, currentValue, ftol_rel, ftol_abs))
                            r = AugmentedLagrangianStatus.Converged;

                        if (xtolreach(currentSolution, xtol_rel, 0))
                            r = AugmentedLagrangianStatus.Converged;
                    }

                    minValue = currentValue;
                    minPenalty = penalty;
                    minFeasible = feasible;

                    // Save the current solution 
                    for (int i = 0; i < Solution.Length; i++)
                        Solution[i] = currentSolution[i];

                    if (r.HasValue)
                        return r.Value;

                    noProgressCounter = 0;
                }
                else
                {
                    if (ICM == 0)
                        return AugmentedLagrangianStatus.Converged;

                    noProgressCounter++;

                    if (noProgressCounter > maxCount)
                        return AugmentedLagrangianStatus.NoProgress;
                }


                // Go to next iteration
                iterations++;

                if (maxEvaluations > 0 && functionEvaluations >= maxEvaluations)
                    return AugmentedLagrangianStatus.MaxEvaluations;
            }
        }



        private bool xtolreach(double[] currentSolution, double reltol, double abstol)
        {
            for (int i = 0; i < currentSolution.Length; i++)
                if (!relstop(Solution[i], currentSolution[i], reltol, abstol))
                    return false;
            return true;
        }

        static bool relstop(double vold, double vnew, double reltol, double abstol)
        {
            if (Double.IsInfinity(vold))
                return false;

            if (Double.IsNaN(vold) || Double.IsNaN(vnew))
                return true;

            return (Math.Abs(vnew - vold) < abstol
               || Math.Abs(vnew - vold) < reltol * (Math.Abs(vnew) + Math.Abs(vold)) * 0.5
               || (reltol > 0 && vnew == vold)); // catch vnew == vold == 0 
        }

    }
}
