// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © 1995, 1996, 1997, 1998
// Berwin A. Turlach <bturlach@stats.adelaide.edu.au>
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
// This work is based on the original Fortran implementation by Berwin Turlach,
// also shared under the LGPL license.
//

namespace Accord.Tests.Interop.Math
{
    using Accord.Math.Optimization;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UnsafeGoldfarbIdnani : BaseGradientOptimizationMethod,
        IOptimizationMethod, IOptimizationMethod<GoldfarbIdnaniStatus>
    {
        private double[,] hessian;
        private double[] linearTerms;

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
        ///   last call to the <see cref="Minimize"/> or <see cref="Maximize"/> methods.
        /// </summary>
        /// 
        public int Iterations { get; set; }

        /// <summary>
        ///   Gets the total number of constraint removals performed
        ///   on the last call to the <see cref="Minimize"/> or <see cref="Maximize"/> methods.
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
        ///   Gets the indices of the active constraints
        ///   found during the last call of the 
        ///   <see cref="Minimize"/> or <see cref="Maximize"/>
        ///   methods.
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
        ///   Gets the matrix of quadratic terms of
        ///   the quadratic optimization problem.
        /// </summary>
        /// 
        public double[,] QuadraticTerms { get { return hessian; } }

        /// <summary>
        ///   Gets the vector of linear terms of the
        ///   quadratic optimization problem.
        /// </summary>
        /// 
        public double[] LinearTerms { get { return linearTerms; } }

        /// <summary>
        ///   Get the exit code returned in the last call to the
        ///   <see cref="IOptimizationMethod.Maximize()"/> or 
        ///   <see cref="IOptimizationMethod.Minimize()"/> methods.
        /// </summary>
        /// 
        public GoldfarbIdnaniStatus Status { get; private set; }

        /// <summary>
        ///   Constructs a new <see cref="GoldfarbIdnani"/> class.
        /// </summary>
        /// 
        /// <param name="function">The objective function to be optimized.</param>
        /// <param name="constraints">The problem's constraints.</param>
        /// 
        public UnsafeGoldfarbIdnani(QuadraticObjectiveFunction function, IEnumerable<LinearConstraint> constraints)
            : this(function, new LinearConstraintCollection(constraints))
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="GoldfarbIdnani"/> class.
        /// </summary>
        /// 
        /// <param name="function">The objective function to be optimized.</param>
        /// <param name="constraints">The problem's constraints.</param>
        /// 
        public UnsafeGoldfarbIdnani(QuadraticObjectiveFunction function, LinearConstraintCollection constraints)
            : base(function.NumberOfVariables, function.Function, function.Gradient)
        {
            int equalities;

            double[] tolerances;

            // Create the constraint matrix A from the specified constraint list
            double[,] A = constraints.CreateMatrix(function.NumberOfVariables,
                out constraintValues, out tolerances, out equalities);

            Accord.Diagnostics.Debug.Assert(A.GetLength(1) == function.NumberOfVariables);

            initialize(function.NumberOfVariables,
                function.QuadraticTerms, function.LinearTerms,
                A, constraintValues, equalities);
        }


        /// <summary>
        ///   Constructs a new instance of the <see cref="GoldfarbIdnani"/> class.
        /// </summary>
        /// 
        /// <param name="function">The objective function to be optimized.</param>
        /// <param name="constraintMatrix">The constraints matrix <c>A</c>.</param>
        /// <param name="constraintValues">The constraints values <c>b</c>.</param>
        /// <param name="numberOfEqualities">The number of equalities in the constraints.</param>
        /// 
        public UnsafeGoldfarbIdnani(QuadraticObjectiveFunction function, double[,] constraintMatrix,
            double[] constraintValues, int numberOfEqualities = 0)
            : base(function.NumberOfVariables, function.Function, function.Gradient)
        {
            if (function.NumberOfVariables != constraintMatrix.GetLength(1))
            {
                throw new ArgumentException("The number of columns in the constraint matrix A "
                    + "should equal the number of variables in the problem.", "constraintMatrix");
            }

            if (constraintValues.Length != constraintMatrix.GetLength(0))
                throw new DimensionMismatchException("constraintValues");

            if (numberOfEqualities < 0 || numberOfEqualities > constraintValues.Length)
                throw new ArgumentOutOfRangeException("numberOfEqualities");

            initialize(function.NumberOfVariables, function.QuadraticTerms,
                function.LinearTerms, constraintMatrix, constraintValues, numberOfEqualities);
        }

        /// <summary>
        ///   Constructs a new instance of the <see cref="GoldfarbIdnani"/> class.
        /// </summary>
        /// 
        /// <param name="quadratic">The symmetric matrix of quadratic terms defining the objective function.</param>
        /// <param name="linear">The vector of linear terms defining the objective function.</param>
        /// <param name="constraintMatrix">The constraints matrix <c>A</c>.</param>
        /// <param name="constraintValues">The constraints values <c>b</c>.</param>
        /// <param name="numberOfEqualities">The number of equalities in the constraints.</param>
        /// 
        public UnsafeGoldfarbIdnani(double[,] quadratic, double[] linear,
            double[,] constraintMatrix, double[] constraintValues, int numberOfEqualities = 0)
            : this(new QuadraticObjectiveFunction(quadratic, linear), constraintMatrix, constraintValues, numberOfEqualities)
        {
        }

        private void initialize(int numberOfVariables, double[,] hessian, double[] linearTerms,
            double[,] constraintMatrix, double[] b, int numberOfEqualities)
        {
            this.NumberOfVariables = numberOfVariables;
            this.linearTerms = linearTerms;
            this.hessian = hessian;

            this.constraintMatrix = constraintMatrix;
            this.constraintValues = b;

            this.NumberOfEqualities = numberOfEqualities;
            this.NumberOfConstraints = constraintMatrix.GetLength(0);
            this.r = Math.Min(NumberOfVariables, NumberOfConstraints);

            this.ActiveConstraints = new int[NumberOfConstraints];
            this.Lagrangian = new double[NumberOfConstraints];
            this.Solution = new double[NumberOfVariables];

            // initialize work vector
            this.work = new double[NumberOfVariables];
            this.iwzv = new double[NumberOfVariables];
            this.iwrv = new double[r];
            this.iwuv = new double[r + 1];
            this.iwrm = new double[r * (r + 5) / 2];
            this.iwsv = new double[NumberOfConstraints];
            this.iwnbv = new double[NumberOfConstraints];
        }


        /// <summary>
        ///   Finds the minimum value of a function. The solution vector
        ///   will be made available at the <see cref="IOptimizationMethod.Solution"/> property.
        /// </summary>
        /// 
        /// <returns>
        ///   Returns <c>true</c> if the method converged to a <see cref="IOptimizationMethod.Solution"/>.
        ///   In this case, the found value will also be available at the <see cref="IOptimizationMethod.Value"/>
        ///   property.
        /// </returns>
        /// 
        public override bool Minimize()
        {
            double[,] h = new double[NumberOfVariables, NumberOfVariables];
            double[] d = new double[NumberOfVariables];

            // Prepare a maximization problem
            for (int i = 0; i < NumberOfVariables; i++)
                for (int j = 0; j < NumberOfVariables; j++)
                    h[i, j] = hessian[i, j];

            for (int i = 0; i < linearTerms.Length; i++)
                d[i] = -linearTerms[i];

            Status = minimize(h, d);

            Value = Function(Solution);

            return (Status == GoldfarbIdnaniStatus.Success);
        }


        /// <summary>
        ///   Finds the maximum value of a function. The solution vector
        ///   will be made available at the <see cref="IOptimizationMethod.Solution"/> property.
        /// </summary>
        /// <returns>
        ///   Returns <c>true</c> if the method converged to a <see cref="IOptimizationMethod.Solution"/>.
        ///   In this case, the found value will also be available at the <see cref="IOptimizationMethod.Value"/>
        ///   property.
        /// </returns>
        /// 
        public override bool Maximize()
        {
            double[,] h = new double[NumberOfVariables, NumberOfVariables];
            double[] d = new double[NumberOfVariables];

            // Prepare a maximization problem
            for (int i = 0; i < NumberOfVariables; i++)
                for (int j = 0; j < NumberOfVariables; j++)
                    h[i, j] = -hessian[i, j];

            for (int i = 0; i < d.Length; i++)
                d[i] = linearTerms[i];

            Status = minimize(h, d);

            Value = Function(Solution);

            return (Status == GoldfarbIdnaniStatus.Success);
        }

        /// <summary>
        ///   Not available.
        /// </summary>
        /// 
        protected override bool Optimize()
        {
            throw new NotImplementedException();
        }

        private GoldfarbIdnaniStatus minimize(double[,] D, double[] d)
        {
            int numberOfActiveConstraints;
            int[] activeConstraints = new int[NumberOfConstraints];

            int ierr = 0;

            // Call qpgen2 routine from Turlach's Fortran code
            qpgen2(D, d, activeConstraints, out numberOfActiveConstraints, ref ierr);

            if (ierr != 0)
            {
                if (ierr == 1)
                    return GoldfarbIdnaniStatus.NoPossibleSolution;

                if (ierr == 2)
                    return GoldfarbIdnaniStatus.NonPositiveDefinite;

                throw new InvalidOperationException("Unexpected error.");
            }

            // Extract Lagrange multipliers from the work vector
            //ActiveConstraints = activeConstraints.Submatrix(numberOfActiveConstraints);

            //for (int i = 0; i < ActiveConstraints.Length; i++)
            //    Lagrangian[ActiveConstraints[i]] = iwuv[i];

            return GoldfarbIdnaniStatus.Success;
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
        //        where r=Math.Min(n,q)
        //
        private unsafe void qpgen2(double[,] _dmat, double[] _dvec, int[] _iact, out int nact, ref int ierr)
        {

            double[] _iter = new double[2];

            int n = NumberOfVariables;
            int q = NumberOfConstraints;
            int meq = NumberOfEqualities;


            int r__ = Math.Min(n, q);

            double[] _work = new double[2 * n + r__ * (r__ + 5) / 2 + 2 * q + 1];

            //for (int i = 0; i < constraintValues.Length; i++)
            //    constraintValues[i] = -constraintValues[i];

            fixed (double* pamat = constraintMatrix)
            fixed (double* pbvec = constraintValues)
            fixed (double* psol = Solution)
            fixed (double* pdvec = _dvec)
            fixed (int* piact = _iact)
            fixed (double* pdmat = _dmat)
            fixed (double* pwork = _work)
            fixed (double* piter = _iter)
            {
                int l1;
                double gc, gs, tt, sum;

                nact = 0;

                double* dvec = pdvec;
                double* sol = psol;
                double* bvec = pbvec;
                double* dmat = pdmat;
                double* amat = pamat;
                int* iact = piact;
                double* work = pwork;
                double* iter = piter;

                /* Parameter adjustments */
                int fddmat = _dmat.GetLength(0);
                int fdamat = constraintMatrix.GetLength(0);

                --dvec;
                int dmat_dim1 = fddmat;
                var dmat_offset = 1 + dmat_dim1;
                dmat -= dmat_offset;
                --sol;
                --bvec;
                var amat_dim1 = fdamat;
                var amat_offset = 1 + amat_dim1;
                amat -= amat_offset;
                --iact;
                --iter;
                --work;

                double crval;

                /* Function Body */

                int iwzv = n;
                int iwrv = iwzv + n;
                int iwuv = iwrv + r__;
                int iwrm = iwuv + r__ + 1;
                int iwsv = iwrm + r__ * (r__ + 1) / 2;
                int iwnbv = iwsv + q;
                double t1 = 0;
                int it1 = 0;
                double d__1, d__3, d__4;

                int l = (n << 1) + r__ * (r__ + 5) / 2 + (q << 1) + 1;

                /* store the initial dvec to calculate below the unconstrained minima of */
                /* the critical value. */

                int i__1 = n;
                int i__;
                for (i__ = 1; i__ <= i__1; ++i__)
                {
                    work[i__] = dvec[i__];
                    /* L10: */
                }
                i__1 = l;
                for (i__ = n + 1; i__ <= i__1; ++i__)
                {
                    work[i__] = 0.0;
                    /* L11: */
                }
                i__1 = q;
                for (i__ = 1; i__ <= i__1; ++i__)
                {
                    iact[i__] = 0;
                    /* L12: */
                }

                int i__2 = 0;
                int j;

                /* get the initial solution */

                if (ierr == 0)
                {
                    int info = 0;
                    dpofa_(&dmat[dmat_offset], fddmat, n, ref info);
                    if (info != 0)
                    {
                        ierr = 2;
                        goto L999;
                    }
                    dposl_(&dmat[dmat_offset], fddmat, n, &dvec[1]);
                    dpori_(&dmat[dmat_offset], fddmat, n);
                }
                else
                {

                    /* Matrix D is already factorized, so we have to multiply d first with */
                    /* R^-T and then with R^-1.  R^-1 is stored in the upper half of the */
                    /* array dmat. */



                    i__1 = n;
                    for (j = 1; j <= i__1; ++j)
                    {
                        sol[j] = 0.0;
                        i__2 = j;
                        for (i__ = 1; i__ <= i__2; ++i__)
                        {
                            sol[j] += dmat[i__ + j * dmat_dim1] * dvec[i__];
                            /* L21: */
                        }
                        /* L20: */
                    }
                    i__1 = n;
                    for (j = 1; j <= i__1; ++j)
                    {
                        dvec[j] = 0.0;
                        i__2 = n;
                        for (i__ = j; i__ <= i__2; ++i__)
                        {
                            dvec[j] += dmat[j + i__ * dmat_dim1] * sol[i__];
                            /* L23: */
                        }
                        /* L22: */
                    }
                }

                /* set lower triangular of dmat to zero, store dvec in sol and */
                /* calculate value of the criterion at unconstrained minima */

                crval = 0.0;
                i__1 = n;
                for (j = 1; j <= i__1; ++j)
                {
                    sol[j] = dvec[j];
                    crval += work[j] * sol[j];
                    work[j] = 0.0;
                    i__2 = n;
                    for (i__ = j + 1; i__ <= i__2; ++i__)
                    {
                        dmat[i__ + j * dmat_dim1] = 0.0;
                        /* L32: */
                    }
                    /* L30: */
                }
                crval = -(crval) / 2.0;
                ierr = 0;

                /* calculate some constants, i.e., from which index on the different */
                /* quantities are stored in the work matrix */




                /* calculate the norm of each column of the A matrix */

                i__1 = q;
                for (i__ = 1; i__ <= i__1; ++i__)
                {
                    sum = 0.0;
                    i__2 = n;
                    for (j = 1; j <= i__2; ++j)
                    {
                        sum += amat[j + i__ * amat_dim1] * amat[j + i__ * amat_dim1];
                        /* L52: */
                    }
                    work[iwnbv + i__] = Math.Sqrt(sum);
                    /* L51: */
                }
                nact = 0;
                iter[1] = 0;
                iter[2] = 0;
            L50:

                /* start a new iteration */

                ++iter[1];

                /* calculate all constraints and check which are still violated */
                /* for the equality constraints we have to check whether the normal */
                /* vector has to be negated (as well as bvec in that case) */

                l = iwsv;
                i__1 = q;
                for (i__ = 1; i__ <= i__1; ++i__)
                {
                    ++l;
                    sum = -bvec[i__];
                    i__2 = n;
                    for (j = 1; j <= i__2; ++j)
                    {
                        sum += amat[j + i__ * amat_dim1] * sol[j];
                        /* L61: */
                    }
                    if (i__ > meq)
                    {
                        work[l] = sum;
                    }
                    else
                    {
                        work[l] = -Math.Abs(sum);
                        if (sum > 0.0)
                        {
                            i__2 = n;
                            for (j = 1; j <= i__2; ++j)
                            {
                                amat[j + i__ * amat_dim1] = -amat[j + i__ * amat_dim1];
                                /* L62: */
                            }
                            bvec[i__] = -bvec[i__];
                        }
                    }
                    /* L60: */
                }

                /* as safeguard against rounding errors set already active constraints */
                /* explicitly to zero */

                i__1 = nact;
                for (i__ = 1; i__ <= i__1; ++i__)
                {
                    work[iwsv + iact[i__]] = 0.0;
                    /* L70: */
                }

                /* we weight each violation by the number of non-zero elements in the */
                /* corresponding row of A. then we choose the violated constraint which */
                /* has maximal absolute value, i.e., the minimum. */
                /* by obvious commenting and uncommenting we can choose the strategy to */
                /* take always the first constraint which is violated. ;-) */

                int nvl = 0;
                double temp = 0.0;
                i__1 = q;
                for (i__ = 1; i__ <= i__1; ++i__)
                {
                    if (work[iwsv + i__] < temp * work[iwnbv + i__])
                    {
                        nvl = i__;
                        temp = work[iwsv + i__] / work[iwnbv + i__];
                    }
                    /*         if (work(iwsv+i) .LT. 0.d0) then */
                    /*            nvl = i */
                    /*            goto 72 */
                    /*         endif */
                    /* L71: */
                }
                /* L72: */
                if (nvl == 0)
                {
                    goto L999;
                }

/* calculate d=J^Tn^+ where n^+ is the normal vector of the violated */
            /* constraint. J is stored in dmat in this implementation!! */
            /* if we drop a constraint, we have to jump back here. */

L55:
                i__1 = n;
                for (i__ = 1; i__ <= i__1; ++i__)
                {
                    sum = 0.0;
                    i__2 = n;
                    for (j = 1; j <= i__2; ++j)
                    {
                        sum += dmat[j + i__ * dmat_dim1] * amat[j + nvl * amat_dim1];
                        /* L81: */
                    }
                    work[i__] = sum;
                    /* L80: */
                }

                /* Now calculate z = J_2 d_2 */

                l1 = iwzv;
                i__1 = n;
                for (i__ = 1; i__ <= i__1; ++i__)
                {
                    work[l1 + i__] = 0.0;
                    /* L90: */
                }
                i__1 = n;
                for (j = nact + 1; j <= i__1; ++j)
                {
                    i__2 = n;
                    for (i__ = 1; i__ <= i__2; ++i__)
                    {
                        work[l1 + i__] += dmat[i__ + j * dmat_dim1] * work[j];
                        /* L93: */
                    }
                    /* L92: */
                }

                /* and r = R^{-1} d_1, check also if r has positive elements (among the */
                /* entries corresponding to inequalities constraints). */

                bool t1inf = true;
                for (i__ = nact; i__ >= 1; --i__)
                {
                    sum = work[i__];
                    l = iwrm + i__ * (i__ + 3) / 2;
                    l1 = l - i__;
                    i__1 = nact;
                    for (j = i__ + 1; j <= i__1; ++j)
                    {
                        sum -= work[l] * work[iwrv + j];
                        l += j;
                        /* L96: */
                    }
                    sum /= work[l1];
                    work[iwrv + i__] = sum;
                    if (iact[i__] <= meq)
                    {
                        goto L95;
                    }
                    if (sum <= 0.0)
                    {
                        goto L95;
                    }
                    /* L7: */
                    t1inf = false;
                    it1 = i__;
                L95:
                    ;
                }

                /* if r has positive elements, find the partial step length t1, which is */
                /* the maximum step in dual space without violating dual feasibility. */
                /* it1  stores in which component t1, the min of u/r, occurs. */

                if (!t1inf)
                {
                    t1 = work[iwuv + it1] / work[iwrv + it1];
                    i__1 = nact;
                    for (i__ = 1; i__ <= i__1; ++i__)
                    {
                        if (iact[i__] <= meq)
                        {
                            goto L100;
                        }
                        if (work[iwrv + i__] <= 0.0)
                        {
                            goto L100;
                        }
                        temp = work[iwuv + i__] / work[iwrv + i__];
                        if (temp < t1)
                        {
                            t1 = temp;
                            it1 = i__;
                        }
                    L100:
                        ;
                    }
                }

                /* test if the z vector is equal to zero */

                sum = 0.0;
                i__1 = iwzv + n;
                for (i__ = iwzv + 1; i__ <= i__1; ++i__)
                {
                    sum += work[i__] * work[i__];
                    /* L110: */
                }
                temp = 1e3;
                sum += temp;
                if (temp == sum)
                {

                    /* No step in pmrimal space such that the new constraint becomes */
                    /* feasible. Take step in dual space and drop a constant. */

                    if (t1inf)
                    {

                        /* No step in dual space possible either, problem is not solvable */

                        ierr = 1;
                        goto L999;
                    }
                    else
                    {

                        /* we take a partial step in dual space and drop constraint it1, */
                        /* that is, we drop the it1-th active constraint. */
                        /* then we continue at step 2(a) (marked by label 55) */

                        i__1 = nact;
                        for (i__ = 1; i__ <= i__1; ++i__)
                        {
                            work[iwuv + i__] -= t1 * work[iwrv + i__];
                            /* L111: */
                        }
                        work[iwuv + nact + 1] += t1;
                        goto L700;
                    }
                }
                else
                {

                    /* compute full step length t2, minimum step in primal space such that */
                    /* the constraint becomes feasible. */
                    /* keep sum (which is z^Tn^+) to update crval below! */

                    bool t2min;
                    sum = 0.0;
                    i__1 = n;
                    for (i__ = 1; i__ <= i__1; ++i__)
                    {
                        sum += work[iwzv + i__] * amat[i__ + nvl * amat_dim1];
                        /* L120: */
                    }
                    tt = -work[iwsv + nvl] / sum;
                    t2min = true;
                    if (!t1inf)
                    {
                        if (t1 < tt)
                        {
                            tt = t1;
                            t2min = false;
                        }
                    }

                    /* take step in primal and dual space */



                    i__1 = n;
                    for (i__ = 1; i__ <= i__1; ++i__)
                    {
                        sol[i__] += tt * work[iwzv + i__];
                        /* L130: */
                    }
                    crval += tt * sum * (tt / 2.0 + work[iwuv + nact + 1]);
                    i__1 = nact;
                    for (i__ = 1; i__ <= i__1; ++i__)
                    {
                        work[iwuv + i__] -= tt * work[iwrv + i__];
                        /* L131: */
                    }
                    work[iwuv + nact + 1] += tt;

                    /* if it was a full step, then we check wheter further constraints are */
                    /* violated otherwise we can drop the current constraint and iterate once */
                    /* more */
                    if (t2min)
                    {

                        /* we took a full step. Thus add constraint nvl to the list of active */
                        /* constraints and update J and R */

                        ++(nact);
                        iact[nact] = nvl;

                        /* to update R we have to put the first nact-1 components of the d vector */
                        /* into column (nact) of R */

                        l = iwrm + (nact - 1) * nact / 2 + 1;
                        i__1 = nact - 1;
                        for (i__ = 1; i__ <= i__1; ++i__)
                        {
                            work[l] = work[i__];
                            ++l;
                            /* L150: */
                        }

                        /* if now nact=n, then we just have to add the last element to the new */
                        /* row of R. */
                        /* Otherwise we use Givens transformations to turn the vector d(nact:n) */
                        /* into a multiple of the first unit vector. That multiple goes into the */
                        /* last element of the new row of R and J is accordingly updated by the */
                        /* Givens transformations. */

                        if (nact == n)
                        {
                            work[l] = work[n];
                        }
                        else
                        {
                            i__1 = nact + 1;
                            for (i__ = n; i__ >= i__1; --i__)
                            {

                                /* we have to find the Givens rotation which will reduce the element */
                                /* (l1) of d to zero. */
                                /* if it is already zero we don't have to do anything, except of */
                                /* decreasing l1 */

                                if (work[i__] == 0.0)
                                {
                                    goto L160;
                                }


                                /* Computing MAX */
                                d__3 = Math.Abs(work[i__ - 1]);
                                d__4 = Math.Abs(work[i__]);
                                gc = Math.Max(d__3, d__4);
                                /* Computing MIN */
                                d__3 = Math.Abs(work[i__ - 1]);
                                d__4 = Math.Abs(work[i__]);
                                gs = Math.Min(d__3, d__4);
                                d__1 = gc * Math.Sqrt(gs * gs / (gc * gc) + 1);
                                temp = Accord.Math.Special.Sign(d__1, work[i__ - 1]);
                                gc = work[i__ - 1] / temp;
                                gs = work[i__] / temp;

                                /* The Givens rotation is done with the matrix (gc gs, gs -gc). */
                                /* If gc is one, then element (i) of d is zero compared with element */
                                /* (l1-1). Hence we don't have to do anything. */
                                /* If gc is zero, then we just have to switch column (i) and column (i-1) */
                                /* of J. Since we only switch columns in J, we have to be careful how we */
                                /* update d depending on the sign of gs. */
                                /* Otherwise we have to apply the Givens rotation to these columns. */
                                /* The i-1 element of d has to be updated to temp. */

                                if (gc == 1.0)
                                {
                                    goto L160;
                                }
                                if (gc == 0.0)
                                {
                                    work[i__ - 1] = gs * temp;
                                    i__2 = n;
                                    for (j = 1; j <= i__2; ++j)
                                    {
                                        temp = dmat[j + (i__ - 1) * dmat_dim1];
                                        dmat[j + (i__ - 1) * dmat_dim1] = dmat[j + i__ *
                                            dmat_dim1];
                                        dmat[j + i__ * dmat_dim1] = temp;
                                        /* L170: */
                                    }
                                }
                                else
                                {
                                    work[i__ - 1] = temp;
                                    double nu = gs / (gc + 1.0);
                                    i__2 = n;
                                    for (j = 1; j <= i__2; ++j)
                                    {
                                        temp = gc * dmat[j + (i__ - 1) * dmat_dim1] + gs *
                                             dmat[j + i__ * dmat_dim1];
                                        dmat[j + i__ * dmat_dim1] = nu * (dmat[j + (i__ -
                                            1) * dmat_dim1] + temp) - dmat[j + i__ *
                                            dmat_dim1];
                                        dmat[j + (i__ - 1) * dmat_dim1] = temp;
                                        /* L180: */
                                    }
                                }
                            L160:
                                ;
                            }

                            /* l is still pointing to element (nact,nact) of the matrix R. */
                            /* So store d(nact) in R(nact,nact) */
                            work[l] = work[nact];
                        }
                    }
                    else
                    {

                        /* we took a partial step in dual space. Thus drop constraint it1, */
                        /* that is, we drop the it1-th active constraint. */
                        /* then we continue at step 2(a) (marked by label 55) */
                        /* but since the fit changed, we have to recalculate now "how much" */
                        /* the fit violates the chosen constraint now. */

                        sum = -bvec[nvl];
                        i__1 = n;
                        for (j = 1; j <= i__1; ++j)
                        {
                            sum += sol[j] * amat[j + nvl * amat_dim1];
                            /* L190: */
                        }
                        if (nvl > meq)
                        {
                            work[iwsv + nvl] = sum;
                        }
                        else
                        {
                            work[iwsv + nvl] = -Math.Abs(sum);
                            if (sum > 0.0)
                            {
                                i__1 = n;
                                for (j = 1; j <= i__1; ++j)
                                {
                                    amat[j + nvl * amat_dim1] = -amat[j + nvl * amat_dim1]
                                        ;
                                    /* L191: */
                                }
                                bvec[i__] = -bvec[i__];
                            }
                        }
                        goto L700;
                    }
                }
                goto L50;

/* Drop constraint it1 */

L700:

                /* if it1 = nact it is only necessary to update the vector u and nact */

                if (it1 == nact)
                {
                    goto L799;
                }

/* After updating one row of R (column of J) we will also come back here */

L797:

                /* we have to find the Givens rotation which will reduce the element */
                /* (it1+1,it1+1) of R to zero. */
                /* if it is already zero we don't have to do anything except of updating */
                /* u, iact, and shifting column (it1+1) of R to column (it1) */
                /* l  will point to element (1,it1+1) of R */
                /* l1 will point to element (it1+1,it1+1) of R */

                l = iwrm + it1 * (it1 + 1) / 2;
                l1 = l + it1;
                if (work[l1] == 0.0)
                {
                    goto L798;
                }
                /* Computing MAX */
                d__3 = Math.Abs(work[l1 - 1]);
                d__4 = Math.Abs(work[l1]);
                gc = Math.Max(d__3, d__4);
                /* Computing MIN */
                d__3 = Math.Abs(work[l1 - 1]);
                d__4 = Math.Abs(work[l1]);
                gs = Math.Min(d__3, d__4);
                d__1 = gc * Math.Sqrt(gs * gs / (gc * gc) + 1);
                temp = Accord.Math.Special.Sign(d__1, work[l1 - 1]);
                gc = work[l1 - 1] / temp;
                gs = work[l1] / temp;

                /* The Givens rotatin is done with the matrix (gc gs, gs -gc). */
                /* If gc is one, then element (it1+1,it1+1) of R is zero compared with */
                /* element (it1,it1+1). Hence we don't have to do anything. */
                /* if gc is zero, then we just have to switch row (it1) and row (it1+1) */
                /* of R and column (it1) and column (it1+1) of J. Since we swithc rows in */
                /* R and columns in J, we can ignore the sign of gs. */
                /* Otherwise we have to apply the Givens rotation to these rows/columns. */

                if (gc == 1.0)
                {
                    goto L798;
                }
                if (gc == 0.0)
                {
                    i__1 = nact;
                    for (i__ = it1 + 1; i__ <= i__1; ++i__)
                    {
                        temp = work[l1 - 1];
                        work[l1 - 1] = work[l1];
                        work[l1] = temp;
                        l1 += i__;
                        /* L710: */
                    }
                    i__1 = n;
                    for (i__ = 1; i__ <= i__1; ++i__)
                    {
                        temp = dmat[i__ + it1 * dmat_dim1];
                        dmat[i__ + it1 * dmat_dim1] = dmat[i__ + (it1 + 1) * dmat_dim1];
                        dmat[i__ + (it1 + 1) * dmat_dim1] = temp;
                        /* L711: */
                    }
                }
                else
                {
                    double nu = gs / (gc + 1.0);
                    i__1 = nact;
                    for (i__ = it1 + 1; i__ <= i__1; ++i__)
                    {
                        temp = gc * work[l1 - 1] + gs * work[l1];
                        work[l1] = nu * (work[l1 - 1] + temp) - work[l1];
                        work[l1 - 1] = temp;
                        l1 += i__;
                        /* L720: */
                    }
                    i__1 = n;
                    for (i__ = 1; i__ <= i__1; ++i__)
                    {
                        temp = gc * dmat[i__ + it1 * dmat_dim1] + gs * dmat[i__ + (it1 +
                            1) * dmat_dim1];
                        dmat[i__ + (it1 + 1) * dmat_dim1] = nu * (dmat[i__ + it1 *
                            dmat_dim1] + temp) - dmat[i__ + (it1 + 1) * dmat_dim1];
                        dmat[i__ + it1 * dmat_dim1] = temp;
                        /* L721: */
                    }
                }

/* shift column (it1+1) of R to column (it1) (that is, the first it1 */
            /* elements). The posit1on of element (1,it1+1) of R was calculated above */
            /* and stored in l. */

L798:
                l1 = l - it1;
                i__1 = it1;
                for (i__ = 1; i__ <= i__1; ++i__)
                {
                    work[l1] = work[l];
                    ++l;
                    ++l1;
                    /* L730: */
                }

                /* update vector u and iact as necessary */
                /* Continue with updating the matrices J and R */

                work[iwuv + it1] = work[iwuv + it1 + 1];
                iact[it1] = iact[it1 + 1];
                ++it1;
                if (it1 < nact)
                {
                    goto L797;
                }
            L799:
                work[iwuv + nact] = work[iwuv + nact + 1];
                work[iwuv + nact + 1] = 0.0;
                iact[nact] = 0;
                --(nact);
                ++iter[2];
                goto L55;

            L999:

                Iterations = (int)_iter[0];
                Deletions = (int)_iter[1];


                for (int i = 0; i <= nact; i++)
                    Lagrangian[iact[i] - 1] = work[iwuv + i];

                return;
            }
        }


        static unsafe int dpori_(double* a, int lda, int n)
        {
            /* System generated locals */
            int a_dim1, a_offset, i__1, i__2;

            /* Local variables */
            int j, k;
            double t;
            int kp1;

            int c__1 = 1;

            /* Parameter adjustments */
            a_dim1 = lda;
            a_offset = 1 + a_dim1;
            a -= a_offset;

            /* Function Body */
            i__1 = n;
            for (k = 1; k <= i__1; ++k)
            {
                a[k + k * a_dim1] = 1.0 / a[k + k * a_dim1];
                t = -a[k + k * a_dim1];
                i__2 = k - 1;
                dscal_(&i__2, &t, &a[k * a_dim1 + 1], c__1);
                kp1 = k + 1;
                if (n < kp1)
                {
                    goto L90;
                }
                i__2 = n;
                for (j = kp1; j <= i__2; ++j)
                {
                    t = a[k + j * a_dim1];
                    a[k + j * a_dim1] = 0.0;
                    daxpy_(&k, &t, &a[k * a_dim1 + 1], c__1, &a[j * a_dim1 + 1], c__1);
                    /* L80: */
                }
            L90:
                /* L100: */
                ;
            }
            return 0;
        } /* dpori_ */

        static unsafe double ddot_(int* n, double* dx, int incx, double* dy, int incy)
        {
            /* System generated locals */
            int i__1;
            double ret_val;

            /* Local variables */
            int i__, m, ix, iy, mp1;
            double dtemp;


            /*     forms the dot product of two vectors. */
            /*     uses unrolled loops for increments equal to one. */
            /*     jack dongarra, linpack, 3/11/78. */


            /* Parameter adjustments */
            --dy;
            --dx;

            /* Function Body */
            ret_val = 0.0;
            dtemp = 0.0;
            if (*n <= 0)
            {
                return ret_val;
            }
            if (incx == 1 && incy == 1)
            {
                goto L20;
            }

            /*        code for unequal increments or equal increments */
            /*          not equal to 1 */

            ix = 1;
            iy = 1;
            if (incx < 0)
            {
                ix = (-(*n) + 1) * incx + 1;
            }
            if (incy < 0)
            {
                iy = (-(*n) + 1) * incy + 1;
            }
            i__1 = *n;
            for (i__ = 1; i__ <= i__1; ++i__)
            {
                dtemp += dx[ix] * dy[iy];
                ix += incx;
                iy += incy;
                /* L10: */
            }
            ret_val = dtemp;
            return ret_val;

/*        code for both increments equal to 1 */


/*        clean-up loop */

L20:
            m = *n % 5;
            if (m == 0)
            {
                goto L40;
            }
            i__1 = m;
            for (i__ = 1; i__ <= i__1; ++i__)
            {
                dtemp += dx[i__] * dy[i__];
                /* L30: */
            }
            if (*n < 5)
            {
                goto L60;
            }
        L40:
            mp1 = m + 1;
            i__1 = *n;
            for (i__ = mp1; i__ <= i__1; i__ += 5)
            {
                dtemp = dtemp + dx[i__] * dy[i__] + dx[i__ + 1] * dy[i__ + 1] + dx[
                    i__ + 2] * dy[i__ + 2] + dx[i__ + 3] * dy[i__ + 3] + dx[i__ +
                    4] * dy[i__ + 4];
                /* L50: */
            }
        L60:
            ret_val = dtemp;
            return ret_val;
        } /* ddot_ */


        static unsafe int dscal_(int* n, double* da, double* dx, int incx)
        {
            /* System generated locals */
            int i__1, i__2;

            /* Local variables */
            int i__, m, mp1, nincx;


            /*     scales a vector by a constant. */
            /*     uses unrolled loops for increment equal to one. */
            /*     jack dongarra, linpack, 3/11/78. */
            /*     modified 3/93 to return if incx .le. 0. */


            /* Parameter adjustments */
            --dx;

            /* Function Body */
            if (*n <= 0 || incx <= 0)
            {
                return 0;
            }
            if (incx == 1)
            {
                goto L20;
            }

            /*        code for increment not equal to 1 */

            nincx = *n * incx;
            i__1 = nincx;
            i__2 = incx;
            for (i__ = 1; i__2 < 0 ? i__ >= i__1 : i__ <= i__1; i__ += i__2)
            {
                dx[i__] = *da * dx[i__];
                /* L10: */
            }
            return 0;

/*        code for increment equal to 1 */


/*        clean-up loop */

L20:
            m = *n % 5;
            if (m == 0)
            {
                goto L40;
            }
            i__2 = m;
            for (i__ = 1; i__ <= i__2; ++i__)
            {
                dx[i__] = *da * dx[i__];
                /* L30: */
            }
            if (*n < 5)
            {
                return 0;
            }
        L40:
            mp1 = m + 1;
            i__2 = *n;
            for (i__ = mp1; i__ <= i__2; i__ += 5)
            {
                dx[i__] = *da * dx[i__];
                dx[i__ + 1] = *da * dx[i__ + 1];
                dx[i__ + 2] = *da * dx[i__ + 2];
                dx[i__ + 3] = *da * dx[i__ + 3];
                dx[i__ + 4] = *da * dx[i__ + 4];
                /* L50: */
            }
            return 0;
        } /* dscal_ */

        /* Subroutine */
        static unsafe int daxpy_(int* n, double* da, double* dx, int incx, double* dy, int incy)
        {
            /* System generated locals */
            int i__1;

            /* Local variables */
            int i__, m, ix, iy, mp1;


            /*     constant times a vector plus a vector. */
            /*     uses unrolled loops for increments equal to one. */
            /*     jack dongarra, linpack, 3/11/78. */


            /* Parameter adjustments */
            --dy;
            --dx;

            /* Function Body */
            if (*n <= 0)
            {
                return 0;
            }
            if (*da == 0.0)
            {
                return 0;
            }
            if (incx == 1 && incy == 1)
            {
                goto L20;
            }

            /*        code for unequal increments or equal increments */
            /*          not equal to 1 */

            ix = 1;
            iy = 1;
            if (incx < 0)
            {
                ix = (-(*n) + 1) * incx + 1;
            }
            if (incy < 0)
            {
                iy = (-(*n) + 1) * incy + 1;
            }
            i__1 = *n;
            for (i__ = 1; i__ <= i__1; ++i__)
            {
                dy[iy] += *da * dx[ix];
                ix += incx;
                iy += incy;
                /* L10: */
            }
            return 0;

/*        code for both increments equal to 1 */


/*        clean-up loop */

L20:
            m = *n % 4;
            if (m == 0)
            {
                goto L40;
            }
            i__1 = m;
            for (i__ = 1; i__ <= i__1; ++i__)
            {
                dy[i__] += *da * dx[i__];
                /* L30: */
            }
            if (*n < 4)
            {
                return 0;
            }
        L40:
            mp1 = m + 1;
            i__1 = *n;
            for (i__ = mp1; i__ <= i__1; i__ += 4)
            {
                dy[i__] += *da * dx[i__];
                dy[i__ + 1] += *da * dx[i__ + 1];
                dy[i__ + 2] += *da * dx[i__ + 2];
                dy[i__ + 3] += *da * dx[i__ + 3];
                /* L50: */
            }
            return 0;
        } /* daxpy_ */

        static unsafe int dposl_(double* a, int lda, int n, double* b)
        {
            /* System generated locals */
            int a_dim1, a_offset, i__1, i__2;

            /* Local variables */
            int k;
            double t;
            int kb;

            int c__1 = 1;


            /* Parameter adjustments */
            a_dim1 = lda;
            a_offset = 1 + a_dim1;
            a -= a_offset;
            --b;

            /* Function Body */
            i__1 = n;
            for (k = 1; k <= i__1; ++k)
            {
                i__2 = k - 1;
                t = ddot_(&i__2, &a[k * a_dim1 + 1], c__1, &b[1], c__1);
                b[k] = (b[k] - t) / a[k + k * a_dim1];
                /* L10: */
            }

            /*     solve r*x = y */

            i__1 = n;
            for (kb = 1; kb <= i__1; ++kb)
            {
                k = n + 1 - kb;
                b[k] /= a[k + k * a_dim1];
                t = -b[k];
                i__2 = k - 1;
                daxpy_(&i__2, &t, &a[k * a_dim1 + 1], c__1, &b[1], c__1);
                /* L20: */
            }
            return 0;
        } /* dposl_ */



        static unsafe int dpofa_(double* a, int lda, int n, ref int info)
        {
            /* System generated locals */
            int a_dim1, a_offset, i__1, i__2, i__3;

            /* Local variables */
            int j, k;
            double s, t;
            int jm1;

            /* Parameter adjustments */
            a_dim1 = lda;
            a_offset = 1 + a_dim1;
            a -= a_offset;

            int c__1 = 1;

            /* Function Body */
            i__1 = n;
            for (j = 1; j <= i__1; ++j)
            {
                info = j;
                s = 0.0;
                jm1 = j - 1;
                if (jm1 < 1)
                {
                    goto L20;
                }
                i__2 = jm1;
                for (k = 1; k <= i__2; ++k)
                {
                    i__3 = k - 1;
                    t = a[k + j * a_dim1] - ddot_(&i__3, &a[k * a_dim1 + 1], c__1, &a[j * a_dim1 + 1], c__1);
                    t /= a[k + k * a_dim1];
                    a[k + j * a_dim1] = t;
                }
            L20:
                s = a[j + j * a_dim1] - s;
                if (s <= 0.0)
                {
                    goto L40;
                }
                a[j + j * a_dim1] = Math.Sqrt(s);
            }
            info = 0;
        L40:
            return 0;
        } /* dpofa_ */
    }
}
