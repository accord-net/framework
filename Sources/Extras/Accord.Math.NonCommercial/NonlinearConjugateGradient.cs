// Copyright (C) 2001 and 2002 by Carl Edward Rasmussen. Date 2002-02-13
//
// (C) Copyright 1999, 2000 & 2001, Carl Edward Rasmussen
// 
// Permission is granted for anyone to copy, use, or modify these
// programs and accompanying documents for purposes of research or
// education, provided this copyright notice is retained, and note is
// made of any changes that have been made.
// 
// These programs and documents are distributed without any warranty,
// express or implied.  As the programs were written for research
// purposes only, they have not been tested to the degree that would be
// advisable in any important application.  All use of these programs is
// entirely at the user's own risk.
//
//
// This code has been contributed by Peter Sergio Larsen based on the original
//   from Edward Rasmussen's FminCG. Please note that this code is only available
//   under a special license that specifically *denies* the use for commercial
//   applications and is thus *not compatible with the LGPL and the GPL*. Use
//   at your own risk.
//

namespace Accord.Math.Optimization
{
    using System;

    /// <summary>
    ///   Non-linear Conjugate Gradient (WARNING: This code <b>can not be used for 
    ///   commercial purposes</b>. It is MANDATORY to check the accompanying license
    ///   file for this particular module AND the source code for more details before
    ///   you use this code).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This code has been contributed by Peter Sergio Larsen based on the original
    ///   from Edward Rasmussen's FminCG. Please note that this code is only available
    ///   under a special license that specifically <b>denies</b> the use for commercial
    ///   applications and is thus <b>not compatible with the LGPL and the GPL</b>. Use
    ///   at your own risk.</para>
    ///   
    /// <para>
    ///   To use this class, add a reference to the <c>Accord.Math.Noncommercial.dll</c>
    ///   assembly that resides inside the Release/Noncommercial folder of the framework's
    ///   installation directory.</para>
    ///   
    /// <para>
    ///   The copyright license, plus the original documentation for this code, is
    ///   shown below.</para>
    ///   
    /// <code>
    ///   function [X, fX, i] = fmincg(f, X, options, P1, P2, P3, P4, P5)
    ///   % Minimize a continuous differentialble multivariate function. Starting point
    ///   % is given by "X" (D by 1), and the function named in the string "f", must
    ///   % return a function value and a vector of partial derivatives. The Polack-
    ///   % Ribiere flavour of conjugate gradients is used to compute search directions,
    ///   % and a line search using quadratic and cubic polynomial approximations and the
    ///   % Wolfe-Powell stopping criteria is used together with the slope ratio method
    ///   % for guessing initial step sizes. Additionally a bunch of checks are made to
    ///   % make sure that exploration is taking place and that extrapolation will not
    ///   % be unboundedly large. The "length" gives the length of the run: if it is
    ///   % positive, it gives the maximum number of line searches, if negative its
    ///   % absolute gives the maximum allowed number of function evaluations. You can
    ///   % (optionally) give "length" a second component, which will indicate the
    ///   % reduction in function value to be expected in the first line-search (defaults
    ///   % to 1.0). The function returns when either its length is up, or if no further
    ///   % progress can be made (ie, we are at a minimum, or so close that due to
    ///   % numerical problems, we cannot get any closer). If the function terminates
    ///   % within a few iterations, it could be an indication that the function value
    ///   % and derivatives are not consistent (ie, there may be a bug in the
    ///   % implementation of your "f" function). The function returns the found
    ///   % solution "X", a vector of function values "fX" indicating the progress made
    ///   % and "i" the number of iterations (line searches or function evaluations,
    ///   % depending on the sign of "length") used.
    ///   %
    ///   % Usage: [X, fX, i] = fmincg(f, X, options, P1, P2, P3, P4, P5)
    ///   %
    ///   % See also: checkgrad 
    ///   %
    ///   % Copyright (C) 2001 and 2002 by Carl Edward Rasmussen. Date 2002-02-13
    ///   %
    ///   %
    ///   % (C) Copyright 1999, 2000 &amp; 2001, Carl Edward Rasmussen
    ///   % 
    ///   % Permission is granted for anyone to copy, use, or modify these
    ///   % programs and accompanying documents for purposes of research or
    ///   % education, provided this copyright notice is retained, and note is
    ///   % made of any changes that have been made.
    ///   % 
    ///   % These programs and documents are distributed without any warranty,
    ///   % express or implied.  As the programs were written for research
    ///   % purposes only, they have not been tested to the degree that would be
    ///   % advisable in any important application.  All use of these programs is
    ///   % entirely at the user's own risk.
    /// </code>
    ///   
    /// <para>
    ///   Modifications have been made so this code could fit under Accord.NET's
    ///   <see cref="IGradientOptimizationMethod"/> interface. Modifications were
    ///   necessary to port the original code from MATLAB/Octave to C#.</para>
    /// </remarks>
    /// 
    /// <seealso cref="ConjugateGradient"/>
    /// <seealso cref="ResilientBackpropagation"/>
    /// 
    public class NonlinearConjugateGradient : BaseGradientOptimizationMethod,
        IGradientOptimizationMethod
    {

        // constants from Wolfe Powell conditions 
        private double RHO = 0.01;
        private double SIG = 0.5;
        private double INT = 0.1;

        // extrapolate three times the bracket 
        private double EXT = 3.0;

        // max 20 function evaluation pr. line search
        private double MAX = 20;

        // maximum allowed slope ratio
        private double RATIO = 100.0;
        private int Red = 1;

        private double[] s;
        private double[] df1;

        private int maxIterations = 100;

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   to be performed. Default is 100.
        /// </summary>
        /// 
        public int MaxIterations
        {
            get { return maxIterations; }
            set { maxIterations = value; }
        }

        /// <summary>
        ///   Gets the number of calls to the objective 
        ///   <see cref="BaseOptimizationMethod.Function"/> in the last call
        ///   to <see cref="BaseGradientOptimizationMethod.Minimize()"/> 
        ///   or <see cref="BaseGradientOptimizationMethod.Maximize()"/>.
        /// </summary>
        /// 
        public int Evaluations { get; private set; }

        /// <summary>
        ///   Gets the number of iterations done in the last call 
        ///   to <see cref="BaseGradientOptimizationMethod.Minimize()"/> 
        ///   or <see cref="BaseGradientOptimizationMethod.Maximize()"/>.
        /// </summary>
        /// 
        public int Iterations { get; private set; }





        /// <summary>
        ///   Constructs a new <see cref="NonlinearConjugateGradient"/> algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
        /// 
        public NonlinearConjugateGradient(int numberOfVariables)
            : base(numberOfVariables)
        {
            init();
        }

        /// <summary>
        ///   Constructs a new <see cref="NonlinearConjugateGradient"/> algorithm.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
        /// <param name="function">The function to be optimized.</param>
        /// <param name="gradient">The gradient of the function.</param>
        /// 
        public NonlinearConjugateGradient(int numberOfVariables,
            Func<double[], double> function, Func<double[], double[]> gradient)
            : base(numberOfVariables, function, gradient)
        {
            init();
        }

        private void init()
        {
            this.s = new double[NumberOfVariables];
        }

        /// <summary>
        ///   Implements the actual optimization algorithm. This
        ///   method should try to minimize the objective function.
        /// </summary>
        /// 
        /// <returns>True if the algorithm succeeded; false otherwise.</returns>
        /// 
        protected override bool Optimize()
        {
            Evaluations = 0;
            Iterations = 0;

            int length = MaxIterations;
            double[] X = base.Solution;
            double M = 0.0;
            int iteration = 0;
            int Linessearch_isfailed = 0;


            // get first function eval
            double f1 = Function(X);

            // get first grad eval
            df1 = Gradient(X);

            Evaluations++;

            if (length < 0)
                iteration++;

            // this is the steepest search directions 
            for (int i = 0; i < df1.Length; i++)
                s[i] = -df1[i];

            // this is the slope of the line in negative direction
            double d1 = 0.0;
            for (int j = 0; j < s.Length; j++)
                d1 += -s[j] * s[j];

            // initial step is red(red/|s|+1)
            double z1 = Red / (1 - d1);

            double[] X0 = new double[X.Length];
            double[] DF0 = new double[X.Length];

            while (iteration < Math.Abs(length))
            {
                if (Token.IsCancellationRequested)
                    break;

                if (length > 0)
                    iteration++; // count epoch(iter)

                Iterations = iteration;

                // make copy of current values
                for (int j = 0; j < X0.Length; j++)
                    X0[j] = X[j];

                for (int j = 0; j < DF0.Length; j++)
                    DF0[j] = df1[j];

                double F0 = f1;

                // begin line search
                for (int j = 0; j < X.Length; j++)
                    X[j] += s[j] * z1;

                // evaluate cost - and gradient function with new params
                double f2 = Function(X);
                double[] df2 = Gradient(X);

                Evaluations++;

                if (length < 0)
                    iteration++;

                double d2 = 0;
                for (int i = 0; i < df2.Length; i++)
                    d2 += df2[i] * s[i];

                // initialize point 3 equal to point 1
                double f3 = f1, d3 = d1, z3 = -z1;

                if (length > 0)
                {
                    M = MAX;
                }
                else
                {
                    M = Math.Min(MAX, -length - iteration);
                }


                // initialize quantities
                double succes = 0.0;
                double limit = -1.0;

                while (true)
                {
                    //f2 = 0.70; 
                    while (((f2 > f1 + z1 * RHO * d1) || (d2 > -SIG * d1)) && (M > 0))
                    {
                        limit = z1; // tighten bracket

                        // values for cubic or quadratic fit
                        double A = 0.0d;
                        double B = 0.0d;
                        double z2 = 0.0d;

                        if (f2 > f1)
                        {
                            // quadratic fit 
                            z2 = z3 - ((0.5 * d3 * z3 * z3) / (d3 * z3 + f2 - f3));
                        }
                        else
                        {
                            // cubic fit
                            A = (6 * (f2 - f3)) / (z3 + (3 * (d2 + d3)));
                            B = (3 * (f3 - f2) - (z3 * ((d3 + 2) * d2)));

                            // numerical error possible - ok!
                            z2 = Math.Sqrt(((B * B) - (A * d2 * z3)) - B) / A;
                        }
                        if (Double.IsNaN(z2) || Double.IsInfinity(z2) || Double.IsNegativeInfinity(z2))
                        {
                            z2 = z3 / 2.0;
                        }

                        // don't accept too close to limit
                        z2 = Math.Max(Math.Min(z2, INT * z3), (1 - INT) * z3);

                        z1 = z1 + z2; // update this step 
                        for (int j = 0; j < X.Length; j++)
                            X[j] += s[j] * z2;

                        f2 = Function(X);
                        df2 = Gradient(X);
                        Evaluations++;

                        M = M - 1;

                        // count epochs 
                        if (length < 0)
                            iteration++;

                        // new slope
                        d2 = 0.0;
                        for (int i = 0; i < df2.Length; i++)
                            d2 += df2[i] * s[i];

                        // z3 is now relative to the location of z2
                        z3 = z3 - z2;

                    }

                    if (f2 > (f1 + z1 * RHO * d1) || d2 > (-SIG * d1))
                    {
                        // this is a failure
                        break;
                    }
                    else if (d2 > (SIG * d1))
                    {
                        succes = 1.0;
                        break; // success
                    }
                    else if (M == 0)
                    {
                        break; // failure
                    }

                    // Make cubic extrapolation 
                    var A1 = 6 * (f2 - f3) / z3 + 3 * (d2 + d3);
                    var B1 = 3 * (f3 - f2) - z3 * (d3 + 2 * d2);
                    var z21 = -d2 * z3 * z3 / (B1 + Math.Sqrt(B1 * B1 - A1 * d2 * z3 * z3));

                    if (z21 < 0)
                    {
                        z21 = z21 * -1;
                    }

                    // num prop or wrong sign  ? 
                    if (double.IsNaN(z21) || double.IsInfinity(z21) || z21 < 0)
                    {
                        if (limit < -0.5) // no upper limit
                        {
                            z21 = z1 * (EXT - 1);
                        }
                        else
                        {
                            z21 = (limit - z1) / 2;
                        }
                    }
                    else if (limit > -0.5 && (z21 + z1 > limit))
                    {
                        z21 = (limit - z1) / 2;
                    }

                    else if (limit < -0.5 && (z21 + z1 > z1 * EXT))
                    {
                        z21 = z1 * (EXT - 1.0);
                    }

                    else if (z21 < -z3 * INT)
                    {
                        z21 = -z3 * INT;
                    }

                    else if ((limit > -0.5) && (z21 < (limit - z1) * (1.0 - INT)))
                    {
                        z21 = (limit - z1) * (1.0 - INT);
                    }

                    // set point 3 equal to point 2
                    f3 = f2;
                    d3 = d2;
                    z3 = -z21;
                    z1 = z1 + z21;

                    // update current estimate
                    for (int j = 0; j < X.Length; j++)
                        X[j] += s[j] * z21;

                    // evaluate functions
                    df2 = Gradient(X);
                    f2 = Function(X);

                    M = M - 1;
                    iteration = iteration + (length < 0 ? 1 : 0); // count epochs

                    d2 = 0;
                    for (int i = 0; i < df2.Length; i++)
                        d2 += df2[i] * s[i];

                }// end of line search


                if (succes == 1.0) // if line searched succeded 
                {
                    f1 = f2;

                    // Polack- Ribiere direction
                    var ptemp1 = df2.Dot(df2) - df1.Dot(df2);
                    var ptemp2 = df1.Dot(df1);
                    var ptemp3 = ptemp1 / ptemp2;

                    for (int j = 0; j < s.Length; j++)
                        s[j] = s[j] * ptemp3 - df2[j];


                    // swap derivatives
                    var tmp = df1;
                    df1 = df2;
                    df2 = tmp;

                    // get slope
                    d2 = 0;
                    for (int i = 0; i < df1.Length; i++)
                        d2 += df1[i] * s[i];

                    // new slope must be negative 
                    if (d2 > 0)
                    {
                        // use steepest direction
                        for (int i = 0; i < s.Length; i++)
                            s[i] = -df1[i];

                        d2 = 0;
                        for (int i = 0; i < df1.Length; i++)
                            d2 -= s[i] * s[i];
                    }

                    // slope ration but max ratio
                    z1 = z1 * Math.Min(RATIO, (d1 / (d2 - 2.2251e-308)));
                    d1 = d2;
                    Linessearch_isfailed = 0; // this linesearch did not fail
                }
                else
                {
                    // restore point from before failed line search
                    f1 = F0;

                    for (int j = 0; j < X.Length; j++)
                        X[j] = X0[j];

                    for (int j = 0; j < df1.Length; j++)
                        df1[j] = DF0[j];

                    // line search twice in a row
                    if (Linessearch_isfailed == 1 || iteration > Math.Abs(length))
                    {
                        break; // or we ran out of time , so we give up
                    }

                    var tmp = df1;
                    df1 = df2;
                    df2 = tmp; // swap derivatives

                    for (int i = 0; i < df1.Length; i++)
                        s[i] = -df1[i];

                    d1 = 0;
                    for (int i = 0; i < s.Length; i++)
                        d1 -= s[i] * s[i];

                    z1 = 1d / (1d - d1);

                    Linessearch_isfailed = 1; // this line search failed
                }
            }

            Value = f1;
            return true;
        }
    }
}
