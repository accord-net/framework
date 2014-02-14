// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © 1995, 1996, 1997, 1998
// Berwin A. Turlach <bturlach@stats.adelaide.edu.au>
//
// Copyright © César Souza, 2009-2014
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
// This work is based on the original Fortran implementation by Berwin Turlach,
// also shared under the LGPL license.
//

namespace Accord.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///   Goldfarb-Idnani Quadratic Programming Solver.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://www.javaquant.net/papers/GoldfarbIdnani.pdf">
    ///       Goldfarb D., Idnani A. (1982) Dual and Primal-Dual Methods for Solving Strictly Convex Quadratic Programs.
    ///       Available on: http://www.javaquant.net/papers/GoldfarbIdnani.pdf .</a></description></item>
    ///     <item><description><a href="http://www.javaquant.net/papers/GoldfarbIdnani.pdf">
    ///       Berwin A Turlach. QuadProg, Quadratic Programming Solver (implementation in Fortran).
    ///       Available on:  http://school.maths.uwa.edu.au/~berwin/software/quadprog.html .</a></description></item>
    ///   </list>
    /// </para>   
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   There are three ways to state a quadratic programming problem in this framework.</para>
    ///   
    /// <list type="bullet">
    ///   <item><description>
    ///   The first is to state the problem in its canonical form, explicitly stating the
    ///   matrix Q and vector d specifying the quadratic function and the matrices A and
    ///   vector b specifying the problem constraints.</description></item>
    ///   <item><description>
    ///   The second is to state the problem with lambda expressions using symbolic variables.</description></item>
    ///   <item><description>
    ///   The third is to state the problem using text strings.</description></item>
    /// </list>
    ///   
    /// <para>  
    ///   In the following section we will provide examples for those ways. 
    /// </para>
    /// 
    /// <para>
    ///   This is an example stating the problem using lambdas:</para>
    /// <code>
    /// // Solve the following optimization problem:
    /// //
    /// //  min f(x) = 2x² - xy + 4y² - 5x - 6y
    /// // 
    /// //  s.t.   x - y  ==   5  (x minus y should be equal to 5)
    /// //             x  >=  10  (x should be greater than or equal to 10)
    /// //
    ///
    /// // In this example we will be using some symbolic processing. 
    /// // The following variables could be initialized to any value.
    /// double x = 0, y = 0;
    ///
    /// // Create our objective function using a lambda expression
    /// var f = new QuadraticObjectiveFunction(() => 2 * (x * x) - (x * y) + 4 * (y * y) - 5 * x - 6 * y);
    ///
    /// // Now, create the constraints
    /// List&lt;LinearConstraint> constraints = new List&lt;LinearConstraint>();
    /// constraints.Add(new LinearConstraint(f, () => x - y == 5));
    /// constraints.Add(new LinearConstraint(f, () => x >= 10));
    ///
    /// // Now we create the quadratic programming solver for 2 variables, using the constraints.
    /// GoldfarbIdnaniQuadraticSolver solver = new GoldfarbIdnaniQuadraticSolver(2, constraints);
    ///
    /// // And attempt to solve it.
    /// double minimumValue = solver.Minimize(f);
    /// </code>
    /// 
    /// <para>
    ///   This is an example stating the problem using strings:</para>
    /// <code>
    /// // Solve the following optimization problem:
    /// //
    /// //  max f(x) = -2x² + xy - y² + 5y
    /// // 
    /// //  s.t.   x - y  ==   5  (x minus y should be equal to 5)
    /// //             x  >=  10  (x should be greater than or equal to 10)
    /// //
    /// //
    ///
    /// // Create our objective function using a text string
    /// var f = new QuadraticObjectiveFunction("-2x² + xy - y² + 5y");
    ///
    /// // Now, create the constraints
    /// List&lt;LinearConstraint> constraints = new List&lt;LinearConstraint>();
    /// constraints.Add(new LinearConstraint(f, "x - y ==  5"));
    /// constraints.Add(new LinearConstraint(f, "    x >= 10"));
    ///
    /// // Now we create the quadratic programming solver for 2 variables, using the constraints.
    /// GoldfarbIdnaniQuadraticSolver solver = new GoldfarbIdnaniQuadraticSolver(2, constraints);
    ///
    /// // And attempt to solve it.
    /// double maxValue = solver.Maximize(f);
    /// </code>
    ///   
    /// <para>
    ///   And finally, an example stating the problem using matrices:</para>
    /// <code>
    /// // Solve the following optimization problem:
    /// //
    /// //  min f(x) = 2x² - xy + 4y² - 5x - 6y
    /// // 
    /// //  s.t.   x - y  ==   5  (x minus y should be equal to 5)
    /// //             x  >=  10  (x should be greater than or equal to 10)
    /// //
    /// 
    /// // Lets first group the quadratic and linear terms. The
    /// // quadratic terms are +2x², +3y² and -4xy. The linear 
    /// // terms are -2x and +1y. So our matrix of quadratic
    /// // terms can be expressed as:
    ///
    /// double[,] Q = // 2x² -1xy +4y²
    /// {   
    ///     /*           x              y      */
    ///     /*x*/ { +2 /*xx*/ *2,  -1 /*xy*/    }, 
    ///     /*y*/ { -1 /*xy*/   ,  +4 /*yy*/ *2 },
    /// };
    ///
    /// // Accordingly, our vector of linear terms is given by:
    ///
    /// double[] d = { -5 /*x*/, -6 /*y*/ }; // -5x -6y
    ///
    /// // We have now to express our constraints. We can do it
    /// // either by directly specifying a matrix A in which each
    /// // line refers to one of the constraints, expressing the
    /// // relationship between the different variables in the
    /// // constraint, like this:
    ///
    /// double[,] A = 
    /// {
    ///     { 1, -1 }, // This line says that x + (-y) ... (a)
    ///     { 1,  0 }, // This line says that x alone  ... (b)
    /// };
    ///
    /// double[] b = 
    /// {
    ///      5, // (a) ... should be equal to 5.
    ///     10, // (b) ... should be greater than or equal to 10.
    /// };
    ///
    /// // Equalities must always come first, and in this case
    /// // we have to specify how many of the constraints are
    /// // actually equalities:
    ///
    /// int numberOfEqualities = 1;
    ///
    ///
    /// // Alternatively, we may use a more explicitly form:
    /// List&lt;LinearConstraint> list = new List&lt;LinearConstraint>();
    ///
    /// // Define the first constraint, which involves only x
    /// list.Add(new LinearConstraint(numberOfVariables: 1)
    ///     {
    ///         // x is the first variable, thus located at
    ///         // index 0. We are specifying that x >= 10:
    ///
    ///         VariablesAtIndices = new[] { 0 }, // index 0 (x)
    ///         ShouldBe = ConstraintType.GreaterThanOrEqualTo,
    ///         Value = 10
    ///     });
    ///
    /// // Define the second constraint, which involves x and y
    /// list.Add(new LinearConstraint(numberOfVariables: 2)
    ///     {
    ///         // x is the first variable, located at index 0, and y is
    ///         // the second, thus located at 1. We are specifying that
    ///         // x - y = 5 by saying that the variable at position 0 
    ///         // times 1 plus the variable at position 1 times -1 
    ///         // should be equal to 5.
    ///
    ///         VariablesAtIndices = new int[] { 0, 1 }, // index 0 (x) and index 1 (y)
    ///         CombinedAs = new double[] { 1, -1 }, // when combined as x - y
    ///         ShouldBe = ConstraintType.EqualTo,
    ///         Value = 5
    ///     });
    ///
    ///
    /// // Now we can finally create our optimization problem
    /// var target = new GoldfarbIdnaniQuadraticSolver(numberOfVariables: 2, constraints: list);
    ///
    /// // And attempt to solve it.
    /// double minimumValue = target.Minimize(Q, d);
    /// </code>
    /// </example>
    /// 
    public class GoldfarbIdnaniQuadraticSolver
    {

        private double[,] constraintMatrix;
        private double[] constraintValues;

        //private double[] work;
        private int r;
        private double[] work;
        private double[] iwrv;
        private double[] iwzv;
        private double[] iwuv;
        private double[] iwrm;
        private double[] iwsv;
        private double[] iwnbv;

        /// <summary>
        ///   Gets the number of variables in the quadratic problem.
        /// </summary>
        /// 
        public int NumberOfVariables { get; private set; }

        /// <summary>
        ///   Gets the total number of constraints in the problem.
        /// </summary>
        /// 
        public int NumberOfConstraints { get; private set; }

        /// <summary>
        ///   Gets how many constraints are inequality constraints.
        /// </summary>
        /// 
        public int NumberOfEqualities { get; private set; }

        /// <summary>
        ///   Gets the total number of iterations performed on the
        ///   last call to the <see cref="Minimize(QuadraticObjectiveFunction)"/> method.
        /// </summary>
        /// 
        public int Iterations { get; set; }

        /// <summary>
        ///   Gets the total number of constraint removals performed
        ///   on the last call to the <see cref="Minimize(QuadraticObjectiveFunction)"/> method.
        /// </summary>
        /// 
        public int Deletions { get; set; }

        /// <summary>
        ///   Gets the Lagrangian multipliers for the
        ///   last solution found.
        /// </summary>
        /// 
        public double[] Lagrangian { get; private set; }

        /// <summary>
        ///   Gets the last solution found on the last call
        ///   to the <see cref="Minimize(QuadraticObjectiveFunction)"/> method.
        /// </summary>
        /// 
        public double[] Solution { get; private set; }

        /// <summary>
        ///   Gets the value found at the <see cref="Solution"/>.
        /// </summary>
        /// 
        public double Value { get; private set; }

        /// <summary>
        ///   Gets the indices of the active constraints
        ///   found at the last <see cref="Solution"/>.
        /// </summary>
        /// 
        public int[] ActiveConstraints { get; private set; }

        /// <summary>
        ///   Gets the constraint matrix <c>A</c> for the problem.
        /// </summary>
        /// 
        public double[,] ConstraintMatrix
        {
            get { return constraintMatrix; }
        }

        /// <summary>
        ///   Gets the constraint values <c>b</c> for the problem.
        /// </summary>
        /// 
        public double[] ConstraintValues
        {
            get { return constraintValues; }
        }



        /// <summary>
        ///   Constructs a new <see cref="GoldfarbIdnaniQuadraticSolver"/> class.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of variables.</param>
        /// <param name="constraints">The problem's constraints.</param>
        /// 
        public GoldfarbIdnaniQuadraticSolver(int numberOfVariables, IEnumerable<LinearConstraint> constraints)
            : this(numberOfVariables, new LinearConstraintCollection(constraints))
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="GoldfarbIdnaniQuadraticSolver"/> class.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of variables.</param>
        /// <param name="constraints">The problem's constraints.</param>
        /// 
        public GoldfarbIdnaniQuadraticSolver(int numberOfVariables, LinearConstraintCollection constraints)
        {
            int equalities;

            // Create the constraint matrix A from the specified constraint list
            double[,] A = constraints.CreateMatrix(numberOfVariables, out constraintValues, out equalities);

            System.Diagnostics.Debug.Assert(A.GetLength(1) == numberOfVariables);

            initialize(numberOfVariables, A, constraintValues, equalities);
        }


        /// <summary>
        ///   Constructs a new instance of the <see cref="GoldfarbIdnaniQuadraticSolver"/> class.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of variables.</param>
        /// <param name="constraintMatrix">The constraints matrix <c>A</c>.</param>
        /// <param name="constraintValues">The constraints values <c>b</c>.</param>
        /// <param name="numberOfEqualities">The number of equalities in the constraints.</param>
        /// 
        public GoldfarbIdnaniQuadraticSolver(int numberOfVariables, double[,] constraintMatrix,
            double[] constraintValues, int numberOfEqualities = 0)
        {
            if (numberOfVariables != constraintMatrix.GetLength(1))
                throw new ArgumentException("The number of columns in the constraint matrix A " 
                    + "should equal the number of variables in the problem.", "constraintMatrix");

            initialize(numberOfVariables, constraintMatrix, constraintValues, numberOfEqualities);
        }

        private void initialize(int numberOfVariables, double[,] A, double[] b, int numberOfEqualities)
        {
            this.NumberOfVariables = numberOfVariables;

            this.constraintMatrix = A;
            this.constraintValues = b;

            this.NumberOfEqualities = numberOfEqualities;
            this.NumberOfConstraints = A.GetLength(0);
            this.r = Math.Min(NumberOfVariables, NumberOfConstraints);

            this.ActiveConstraints = new int[NumberOfConstraints];
            this.Lagrangian = new double[NumberOfConstraints];
            this.Solution = new double[NumberOfVariables];

            // initialize work vector
            this.work = new double[NumberOfVariables];
            this.iwzv = new double[NumberOfVariables];
            this.iwrv = new double[r];
            this.iwuv = new double[r + 1];
            this.iwrm = new double[r * (r + 1) / 2];
            this.iwsv = new double[NumberOfConstraints];
            this.iwnbv = new double[NumberOfConstraints];
        }

        /// <summary>
        ///   Maximizes the function.
        /// </summary>
        /// 
        /// <param name="function">The function to be maximized.</param>
        /// <returns>The maximum value at the solution found.</returns>
        /// 
        public double Maximize(QuadraticObjectiveFunction function)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            double[,] Q = function.GetQuadraticTermsMatrix();
            double[] d = function.GetLinearTermsVector();
            return Maximize(Q, d);
        }

        /// <summary>
        ///   Minimizes the function.
        /// </summary>
        /// 
        /// <param name="function">The function to be minimized.</param>
        /// <returns>The minimum value at the solution found.</returns>
        /// 
        public double Minimize(QuadraticObjectiveFunction function)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            double[,] Q = function.GetQuadraticTermsMatrix();
            double[] d = function.GetLinearTermsVector();
            return Minimize(Q, d);
        }

        /// <summary>
        ///   Minimizes the function.
        /// </summary>
        /// 
        /// <param name="hessian">The Hessian matrix <c>D</c> for the quadratic terms.</param>
        /// <param name="linearTerms">The vector of linear terms <c>d</c>.</param>
        /// <returns>The minimum value at the solution found.</returns>
        /// 
        public double Minimize(double[,] hessian, double[] linearTerms)
        {
            if (hessian == null)
                throw new ArgumentNullException("hessian");

            if (linearTerms == null)
                throw new ArgumentNullException("linearTerms");

            if (hessian.GetLength(0) != NumberOfVariables || hessian.GetLength(1) != NumberOfVariables)
                throw new ArgumentException("The number of rows and columns of the quadratic terms matrix D should equal the number of variables in the problem.");

            if (linearTerms.Length != NumberOfVariables)
                throw new ArgumentException("The length of the vector of linear terms d should equal the number of variables in the problem.");

            // Prepare a minimization problem
            for (int i = 0; i < linearTerms.Length; i++)
                linearTerms[i] = -linearTerms[i];

            minimize(hessian, linearTerms);

            return Value; // Return the value at the solution
        }

        /// <summary>
        ///   Maximizes the function.
        /// </summary>
        /// 
        /// <param name="hessian">The Hessian matrix <c>D</c> for the quadratic terms.</param>
        /// <param name="linearTerms">The vector of linear terms <c>d</c>.</param>
        /// <returns>The maximum value at the solution found.</returns>
        /// 
        public double Maximize(double[,] hessian, double[] linearTerms)
        {
            if (hessian == null)
                throw new ArgumentNullException("hessian");

            if (linearTerms == null)
                throw new ArgumentNullException("linearTerms");

            if (hessian.GetLength(0) != NumberOfVariables || hessian.GetLength(1) != NumberOfVariables)
                throw new ArgumentException("The number of rows and columns of the quadratic terms matrix D should equal the number of variables in the problem.");

            if (linearTerms.Length != NumberOfVariables)
                throw new ArgumentException("The length of the vector of linear terms d should equal the number of variables in the problem.");

            // Prepare a maximization problem
            for (int i = 0; i < NumberOfVariables; i++)
                for (int j = 0; j < NumberOfVariables; j++)
                    hessian[i, j] = -hessian[i, j];

            minimize(hessian, linearTerms);
            Value = -Value;

            return Value; // Return the value at the solution
        }

        private void minimize(double[,] D, double[] d)
        {
            int numberOfActiveConstraints;
            int[] activeConstraints = new int[NumberOfConstraints];

            int ierr = 0;

            // Call qpgen2 routine from Turlach's Fortran code
            qpgen2(D, d, activeConstraints, out numberOfActiveConstraints, ref ierr);

            if (ierr != 0)
            {
                if (ierr == 1)
                    throw new ConvergenceException("No possible solution.");

                if (ierr == 2)
                    throw new NonPositiveDefiniteMatrixException();

                throw new InvalidOperationException("Unexpected error.");
            }

            // Extract Lagrange multipliers from the work vector
            ActiveConstraints = activeConstraints.Submatrix(numberOfActiveConstraints);

            for (int i = 0; i < ActiveConstraints.Length; i++)
                Lagrangian[ActiveConstraints[i]] = iwuv[i];
        }



        //
        // This routine uses the Goldfarb/Idnani algorithm to solve the
        // following minimization problem:
        //
        //       minimize 1/2 * x^T D x + d^T x
        //       where   A1 x  = b1
        //               A2 x >= b2
        //
        // the matrix D is assumed to be positive definite.  Especially,
        // w.l.o.g. D is assumed to be symmetric. This is slightly different
        // from the original implementation by Berwin A. Turlach.
        // 
        // Input parameter:
        // dmat   nxn matrix, the matrix D from above (dp)
        //        *** WILL BE DESTROYED ON EXIT ***
        //        The user has two possibilities:
        //        a) Give D (ierr=0), in this case we use routines from LINPACK
        //           to decompose D.
        //        b) To get the algorithm started we need R^-1, where D=R^TR.
        //           So if it is cheaper to calculate R^-1 in another way (D may
        //           be a band matrix) then with the general routine, the user
        //           may pass R^{-1}.  Indicated by ierr not equal to zero.
        // dvec   nx1 vector, the vector d from above (dp)
        //        *** WILL BE DESTROYED ON EXIT ***
        //        contains on exit the solution to the initial, i.e.,
        //        unconstrained problem
        // fddmat scalar, the leading dimension of the matrix dmat
        // n      the dimension of dmat and dvec (int)
        // amat   nxq matrix, the matrix A from above (dp) [ A=(A1 A2) ]
        //        *** ENTRIES CORRESPONDING TO EQUALITY CONSTRAINTS MAY HAVE
        //            CHANGED SIGNES ON EXIT ***
        // bvec   qx1 vector, the vector of constants b in the constraints (dp)
        //        [ b = (b1^T b2^T)^T ]
        //        *** ENTRIES CORRESPONDING TO EQUALITY CONSTRAINTS MAY HAVE
        //            CHANGED SIGNES ON EXIT ***
        // fdamat the first dimension of amat as declared in the calling program. 
        //        fdamat >= n !!
        // q      int, the number of constraints.
        // meq    int, the number of equality constraints, 0 <= meq <= q.
        // ierr   int, code for the status of the matrix D:
        //           ierr =  0, we have to decompose D
        //           ierr != 0, D is already decomposed into D=R^TR and we were
        //                      given R^{-1}.
        //
        // Output parameter:
        // sol   nx1 the final solution (x in the notation above)
        // crval scalar, the value of the criterion at the minimum      
        // iact  qx1 vector, the constraints which are active in the final
        //       fit (int)
        // nact  scalar, the number of constraints active in the final fit (int)
        // iter  2x1 vector, first component gives the number of "main" 
        //       iterations, the second one says how many constraints were
        //        deleted after they became active
        //  ierr  int, error code on exit, if
        //          ierr = 0, no problems
        //          ierr = 1, the minimization problem has no solution
        //          ierr = 2, problems with decomposing D, in this case sol
        //                     contains garbage!!
        //
        //  Working space:
        //  work  vector with length at least 2n+r*(r+5)/2 + 2q +1
        //        where r=min(n,q)
        //
        private void qpgen2(double[,] dmat, double[] dvec, int[] iact, out int nact, ref int ierr)
        {

            int n = NumberOfVariables;
            int q = NumberOfConstraints;
            int meq = NumberOfEqualities;
            double[,] amat = constraintMatrix;
            double[] bvec = constraintValues;
            double[] sol = Solution;

            int l1;
            double gc, gs, tt, sum;

            Value = 0;
            nact = 0;


            // Store the initial dvec to calculate below the
            //  unconstrained minima of the critical value.

            Array.Clear(iwzv, 0, iwzv.Length);
            Array.Clear(iwrv, 0, iwrv.Length);
            Array.Clear(iwuv, 0, iwuv.Length);
            Array.Clear(iwrm, 0, iwrm.Length);
            Array.Clear(iwsv, 0, iwsv.Length);
            Array.Clear(iwnbv, 0, iwnbv.Length);

            for (int i = 0; i < dvec.Length; i++)
                work[i] = dvec[i];

            for (int i = 0; i < iact.Length; i++)
                iact[i] = -1;


            // Get the initial solution
            if (ierr == 0)
            {
                // L'L = chol(D)
                bool success = dpofa(dmat);

                if (!success)
                {
                    ierr = 2;
                    return;
                }

                // L*x = d
                dposl(dmat, dvec);

                // D = inv(L)
                dpori(dmat);
            }
            else
            {
                // Matrix D is already factorized, so we have to multiply d first with 
                // R^-T and then with R^-1.  R^-1 is stored in the upper half of the 
                // array dmat. 

                for (int j = 0; j < sol.Length; j++)
                {
                    sol[j] = 0.0;

                    for (int i = 0; i < j; i++)
                        sol[j] += dmat[j, i] * dvec[i];
                }

                for (int j = 0; j < dvec.Length; j++)
                {
                    dvec[j] = 0.0;

                    for (int i = j; i < sol.Length; i++)
                        dvec[j] += dmat[i, j] * sol[i];
                }
            }

            // Set upper triangular of dmat to zero, store dvec in sol and 
            //   calculate value of the criterion at unconstrained minima

            Value = 0.0;


            // calculate some constants, i.e., from which index on 
            // the different quantities are stored in the work matrix 

            for (int j = 0; j < sol.Length; j++)
            {
                sol[j] = dvec[j];
                Value += work[j] * sol[j];
                work[j] = 0.0;

                for (int i = j + 1; i < n; i++)
                    dmat[j, i] = 0.0;
            }

            Value = -Value / 2.0;
            ierr = 0;


            // calculate the norm of each column of the A matrix 

            for (int i = 0; i < iwnbv.Length; i++)
            {
                sum = 0.0;

                for (int j = 0; j < n; j++)
                    sum += amat[i, j] * amat[i, j];

                iwnbv[i] = Math.Sqrt(sum);
            }

            nact = 0;
            Iterations = 0;
            Deletions = 0;


        L50: // start a new iteration 

            Iterations++;

            // calculate all constraints and check which are still violated 
            // for the equality constraints we have to check whether the normal 
            // vector has to be negated (as well as bvec in that case) 

            int l = 0;

            for (int i = 0; i < bvec.Length; i++)
            {
                sum = -bvec[i];

                for (int j = 0; j < sol.Length; j++)
                    sum += amat[i, j] * sol[j];

                if (i >= meq)
                {
                    // this is a inequality constraint
                    iwsv[l] = sum;
                }
                else
                {
                    // this is an equality constraint
                    iwsv[l] = -Math.Abs(sum);
                    if (sum > 0.0)
                    {
                        for (int j = 0; j < n; j++)
                            amat[i, j] = -amat[i, j];
                        bvec[i] = -bvec[i];
                    }
                }

                l++;
            }

            // as safeguard against rounding errors set 
            // already active constraints explicitly to zero 

            for (int i = 0; i < nact; i++)
                iwsv[iact[i]] = 0.0;

            // We weight each violation by the number of non-zero elements in the 
            // corresponding row of A. then we choose the violated constraint which 
            // has maximal absolute value, i.e., the minimum. By obvious commenting
            // and uncommenting we can choose the strategy to take always the first
            // constraint which is violated. ;-) 

            int nvl = -1;
            double temp = 0.0;
            for (int i = 0; i < iwnbv.Length; i++)
            {
                if (iwsv[i] < temp * iwnbv[i])
                {
                    nvl = i;
                    temp = iwsv[i] / iwnbv[i];
                }

                // if (work(iwsv+i) .LT. 0.d0) then 
                //     nvl = i 
                //     goto 72 
                // endif 
            }

            if (nvl == -1)
                return;


        L55:

            // calculate d=J^Tn^+ where n^+ is the normal vector of the violated 
            // constraint. J is stored in dmat in this implementation!! 
            // if we drop a constraint, we have to jump back here. 

            for (int i = 0; i < work.Length; i++)
            {
                sum = 0.0;

                for (int j = 0; j < n; j++)
                    sum += dmat[i, j] * amat[nvl, j];

                work[i] = sum;
            }

            // Now calculate z = J_2 d_2 ...

            for (int i = 0; i < iwzv.Length; i++)
                iwzv[i] = 0.0;

            for (int j = nact; j < work.Length; j++)
                for (int i = 0; i < iwzv.Length; i++)
                    iwzv[i] += dmat[j, i] * work[j];

            // ... and r = R^{-1} d_1, check also if r has positive elements
            // (among the entries corresponding to inequalities constraints). 

            l1 = 0;
            int it1 = 0;
            double t1 = 0;
            bool t1inf = true;

            for (int i = nact - 1; i >= 0; i--)
            {
                sum = work[i];
                l = ((i + 1) * (i + 4)) / 2 - 1;
                l1 = l - i - 1;

                for (int j = i + 1; j < nact; j++)
                {
                    sum -= iwrm[l] * iwrv[j];
                    l += j + 1;
                }

                sum /= iwrm[l1];

                iwrv[i] = sum;

                if (iact[i] + 1 < meq)
                    continue;

                if (sum <= 0.0)
                    continue;

                if (Double.IsNaN(sum))
                    continue;

                t1inf = false;
                it1 = i;
            }

            // if r has positive elements, find the partial step length t1, which is 
            // the maximum step in dual space without violating dual feasibility. 
            // it1 stores in which component t1, the min of u/r, occurs. 

            if (!t1inf)
            {
                t1 = iwuv[it1] / iwrv[it1];

                for (int i = 0; i < nact; i++)
                {
                    if (iact[i] < meq)
                        continue;

                    if (iwrv[i] < 0.0)
                        continue;

                    temp = iwuv[i] / iwrv[i];

                    if (temp < t1)
                    {
                        t1 = temp;
                        it1 = i;
                    }
                }
            }


            // test if the z vector is equal to zero 

            sum = 0.0;
            for (int i = 0; i < iwzv.Length; i++)
                sum += iwzv[i] * iwzv[i];

            if (Math.Abs(sum) < Constants.DoubleEpsilon)
            {
                // No step in primal space such that the new constraint becomes 
                // feasible. Take step in dual space and drop a constant. 

                if (t1inf)
                {
                    // No step in dual space possible 
                    // either, problem is not solvable
                    ierr = 1;
                    return;
                }
                else
                {
                    // we take a partial step in dual space and drop constraint it1, 
                    // that is, we drop the it1-th active constraint. 
                    // then we continue at step 2(a) (marked by label 55) 

                    for (int i = 0; i < nact; i++)
                        iwuv[i] -= t1 * iwrv[i];

                    iwuv[nact] += t1;
                    goto L700;
                }
            }
            else
            {
                // compute full step length t2, minimum step in primal space such that 
                // the constraint becomes feasible. 
                // keep sum (which is z^Tn^+) to update crval below! 

                sum = 0.0;
                for (int i = 0; i < iwzv.Length; i++)
                    sum += iwzv[i] * amat[nvl, i];

                tt = -iwsv[nvl] / sum;
                bool t2min = true;

                if (!t1inf)
                {
                    if (t1 < tt)
                    {
                        tt = t1;
                        t2min = false;
                    }
                }

                // take step in primal and dual space 
                for (int i = 0; i < sol.Length; i++)
                    sol[i] += tt * iwzv[i];

                Value += tt * sum * (tt / 2.0 + iwuv[nact]);

                for (int i = 0; i < nact; i++)
                    iwuv[i] -= tt * iwrv[i];

                iwuv[nact] += tt;

                // if it was a full step, then we check whether further constraints are 
                // violated otherwise we can drop the current constraint and iterate once 
                // more 

                if (t2min)
                {

                    // we took a full step. Thus add constraint nvl to the list of active 
                    // constraints and update J and R 

                    iact[nact++] = nvl;


                    // to update R we have to put the first nact-1 components of the d vector 
                    // into column (nact) of R 

                    l = ((nact - 1) * (nact)) / 2;
                    for (int i = 0; i < nact - 1; i++, l++)
                        iwrm[l] = work[i];

                    // if now nact=n, then we just have to add the last element to the new 
                    // row of R. 

                    // Otherwise we use Givens transformations to turn the vector d(nact:n) 
                    // into a multiple of the first unit vector. That multiple goes into the 
                    // last element of the new row of R and J is accordingly updated by the 
                    // Givens transformations. 

                    if (nact == n)
                    {
                        iwrm[l] = work[n - 1];
                    }
                    else
                    {
                        for (int i = n - 1; i >= nact; i--)
                        {
                            // We have to find the Givens rotation which will reduce the element 
                            // (l1) of d to zero. If it is already zero we don't have to do anything,
                            // except of decreasing l1 

                            if (work[i] == 0.0)
                                continue;

                            gc = Math.Max(Math.Abs(work[i - 1]), Math.Abs(work[i]));
                            gs = Math.Min(Math.Abs(work[i - 1]), Math.Abs(work[i]));
                            temp = Special.Sign(gc * Math.Sqrt(1.0 + gs * gs / (gc * gc)), work[i - 1]);
                            gc = work[i - 1] / temp;
                            gs = work[i] / temp;

                            // The Givens rotation is done with the matrix (gc gs, gs -gc). If
                            // gc is one, then element (i) of d is zero compared with element 
                            // (l1-1). Hence we don't have to do anything. If gc is zero, then
                            // we just have to switch column (i) and column (i-1) of J. Since 
                            // we only switch columns in J, we have to be careful how we update
                            // d depending on the sign of gs. Otherwise we have to apply the
                            // Givens rotation to these columns. The i-1 element of d has to be
                            // updated to temp. 

                            if (gc == 1.0)
                                continue;

                            if (gc == 0.0)
                            {
                                work[i - 1] = gs * temp;

                                for (int j = 0; j < n; j++)
                                {
                                    temp = dmat[i - 1, j];
                                    dmat[i - 1, j] = dmat[i, j];
                                    dmat[i, j] = temp;
                                }
                            }
                            else
                            {
                                work[i - 1] = temp;
                                double nu = gs / (gc + 1.0);

                                for (int j = 0; j < n; j++)
                                {
                                    temp = gc * dmat[i - 1, j] + gs * dmat[i, j];
                                    dmat[i, j] = nu * (dmat[i - 1, j] + temp) - dmat[i, j];
                                    dmat[i - 1, j] = temp;
                                }
                            }
                        }

                        // l is still pointing to element (nact,nact) of
                        // the matrix R. So store d(nact) in R(nact,nact) 
                        iwrm[l] = work[nact - 1];
                    }
                }
                else
                {

                    // We took a partial step in dual space. Thus drop constraint it1, 
                    // that is, we drop the it1-th active constraint. Then we continue
                    // at step 2(a) (marked by label 55) but since the fit changed, we
                    // have to recalculate now "how much" the fit violates the chosen
                    // constraint now. 

                    sum = -bvec[nvl];

                    for (int j = 0; j < sol.Length; j++)
                        sum += sol[j] * amat[nvl, j];

                    if (nvl >= meq)
                    {
                        iwsv[nvl] = sum;
                    }
                    else
                    {
                        iwsv[nvl] = -Math.Abs(sum);

                        if (sum > 0.0)
                        {
                            for (int j = 0; j < n; j++)
                                amat[nvl, j] = -amat[nvl, j];

                            bvec[nvl] = -bvec[nvl];
                        }
                    }

                    goto L700;
                }
            }

            goto L50;


        L700: // Drop constraint it1 

            // if it1 = nact it is only necessary
            // to update the vector u and nact

            if (it1 == nact - 1)
                goto L799;


        L797: // After updating one row of R (column of J) we will also come back here

            // We have to find the Givens rotation which will reduce the element
            // (it1+1,it1+1) of R to zero. If it is already zero we don't have to
            // do anything except of updating u, iact, and shifting column (it1+1)
            // of R to column (it1). Then l  will point to element (1,it1+1) of R 
            // and l1 will point to element (it1+1,it1+1) of R.

            l = ((it1 + 1) * (it1 + 2)) / 2;
            l1 = l + it1 + 1;

            if (iwrm[l1 - 1] == 0.0)
                goto L798;

            gc = Math.Max(Math.Abs(iwrm[l1 - 1]), Math.Abs(iwrm[l1]));
            gs = Math.Min(Math.Abs(iwrm[l1 - 1]), Math.Abs(iwrm[l1]));
            temp = Special.Sign(gc * Math.Sqrt(1.0 + gs * gs / (gc * gc)), iwrm[l1 - 1]);
            gc = iwrm[l1 - 1] / temp;
            gs = iwrm[l1] / temp;


            // The Givens rotation is done with the matrix (gc gs, gs -gc). If gc is
            // one, then element (it1+1,it1+1) of R is zero compared with element 
            // (it1,it1+1). Hence we don't have to do anything. if gc is zero, then
            // we just have to switch row (it1) and row (it1+1) of R and column (it1)
            // and column (it1+1) of J. Since we switch rows in R and columns in J,
            // we can ignore the sign of gs. Otherwise we have to apply the Givens 
            // rotation to these rows/columns. 

            if (gc == 1.0)
                goto L798;

            if (gc == 0.0)
            {
                for (int i = it1 + 2; i <= nact; i++)
                {
                    temp = iwrm[l1 - 1];
                    iwrm[l1 - 1] = iwrm[l1];
                    iwrm[l1] = temp;
                    l1 += i;
                }

                for (int i = 0; i < n; i++)
                {
                    temp = dmat[it1, i];
                    dmat[it1, i] = dmat[it1 + 1, i];
                    dmat[it1 + 1, i] = temp;
                }
            }
            else
            {
                double nu = gs / (gc + 1.0);

                for (int i = it1 + 2; i <= nact; i++)
                {
                    temp = gc * iwrm[l1 - 1] + gs * iwrm[l1];
                    iwrm[l1] = nu * (iwrm[l1 - 2] + temp) - iwrm[l1];
                    iwrm[l1 - 1] = temp;
                    l1 += i;
                }

                for (int i = 0; i < n; i++)
                {
                    temp = gc * dmat[it1, i] + gs * dmat[it1 + 1, i];
                    dmat[it1 + 1, i] = nu * (dmat[it1, i] + temp) - dmat[it1 + 1, i];
                    dmat[it1, i] = temp;
                }
            }

        L798:

            // shift column (it1+1) of R to column (it1) (that is, the first it1 
            // elements). The position of element (1,it1+1) of R was calculated 
            // above and stored in l. 

            l1 = l - it1;
            for (int i = 0; i <= it1; i++, l++, l1++)
            {
                iwrm[l1 - 1] = iwrm[l];
            }

            // update vector u and iact as necessary 
            // Continue with updating the matrices J and R 

            iwuv[it1] = iwuv[it1 + 1];
            iact[it1] = iact[it1 + 1];
            it1++;

            if (it1 < nact - 1)
                goto L797;

        L799:

            iwuv[nact - 1] = iwuv[nact];
            iwuv[nact] = 0.0;
            iact[nact - 1] = -1;

            nact--;
            Deletions++;

            goto L55;
        }

        private void dpori(double[,] a)
        {
            int n = NumberOfVariables;

            for (int k = 0; k < n; k++)
            {
                a[k, k] = 1.0 / a[k, k];
                double t = -a[k, k];

                for (int j = 0; j < k; j++)
                    a[k, j] = a[k, j] * t;

                for (int j = k + 1; j < n; j++)
                {
                    t = a[j, k];
                    a[j, k] = 0.0;

                    for (int i = 0; i <= k; i++)
                        a[j, i] = t * a[k, i] + a[j, i];
                }
            }
        }

        private void dposl(double[,] a, double[] b)
        {
            int n = NumberOfVariables;

            // solve trans(r)*y = b
            for (int k = 0; k < b.Length; k++)
            {
                for (int i = 0; i < k; i++)
                    b[k] -= b[i] * a[k, i];
                b[k] = b[k] / a[k, k];
            }

            // solve r*x = y
            for (int k = n - 1; k >= 0; k--)
            {
                for (int i = k + 1; i < n; i++)
                    b[k] -= b[i] * a[i, k];
                b[k] /= a[k, k];
            }
        }

        private bool dpofa(double[,] a)
        {
            int n = NumberOfVariables;

            for (int j = 0; j < n; j++)
            {
                double s = 0;
                for (int k = 0; k < j; k++)
                {
                    double t = a[j, k];
                    for (int i = 0; i < k; i++)
                        t -= a[j, i] * a[k, i];
                    t = t / a[k, k];

                    a[j, k] = t;
                    s += t * t;
                }

                s = a[j, j] - s;

                // Use a tolerance for positive-definiteness
                if (s <= 1e-14 * Math.Abs(a[j, j]))
                    return false;

                a[j, j] = Math.Sqrt(s);
            }

            return true;
        }

    }
}
