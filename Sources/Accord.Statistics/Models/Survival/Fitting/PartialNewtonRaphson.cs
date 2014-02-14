// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
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

namespace Accord.Statistics.Models.Regression.Fitting
{
    using System;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions;
    using Accord.Math.Comparers;

    /// <summary>
    ///   Newton-Raphson learning updates for Cox's Proportional Hazards models.
    /// </summary>
    /// 
    public class ProportionalHazardsNewtonRaphson : ISurvivalFitting, IConvergenceLearning
    {

        private ProportionalHazards regression;
        private int parameterCount;

        private double[,] hessian;
        private double[] gradient;

        private double[] partialGradient;
        private double[,] partialHessian;

        private bool computeStandardErrors = true;
        private bool computeBaselineFunction = true;
        private bool normalize = true;

        private ISolverMatrixDecomposition<double> decomposition;
        private RelativeParameterConvergence convergence;


        /// <summary>
        ///   Gets or sets the maximum absolute parameter change detectable
        ///   after an iteration of the algorithm used to detect convergence.
        ///   Default is 1e-5.
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return convergence.Tolerance; }
            set { convergence.Tolerance = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the learning algorithm.
        /// </summary>
        /// 
        public int Iterations
        {
            get { return convergence.Iterations; }
            set { convergence.Iterations = value; }
        }

        /// <summary>
        ///   Gets or sets the number of performed iterations.
        /// </summary>
        /// 
        public int CurrentIteration
        {
            get { return convergence.CurrentIteration; }
            set { convergence.CurrentIteration = value; }
        }


        /// <summary>
        ///   Gets the previous values for the coefficients which were
        ///   in place before the last learning iteration was performed.
        /// </summary>
        /// 
        public double[] Previous { get { return convergence.OldValues; } }

        /// <summary>
        ///   Gets the current values for the coefficients.
        /// </summary>
        /// 
        public double[] Solution { get { return regression.Coefficients; } }

        /// <summary>
        ///   Gets the Hessian matrix computed in 
        ///   the last Newton-Raphson iteration.
        /// </summary>
        /// 
        public double[,] Hessian { get { return hessian; } }

        /// <summary>
        ///   Gets the Gradient vector computed in
        ///   the last Newton-Raphson iteration.
        /// </summary>
        /// 
        public double[] Gradient { get { return gradient; } }

        /// <summary>
        ///   Gets the total number of parameters in the model.
        /// </summary>
        /// 
        public int Parameters { get { return parameterCount; } }


        /// <summary>
        ///   Gets or sets a value indicating whether standard
        ///   errors should be computed at the end of the next 
        ///   iterations.
        /// </summary>
        /// <value>
        /// 	<c>true</c> to compute standard errors; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool ComputeStandardErrors
        {
            get { return computeStandardErrors; }
            set { computeStandardErrors = value; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether an estimate
        ///   of the baseline hazard function should be computed
        ///   at the end of the next iterations.
        /// </summary>
        /// <value>
        /// 	<c>true</c> to compute the baseline function; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool ComputeBaselineFunction
        {
            get { return computeBaselineFunction; }
            set { computeBaselineFunction = value; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the Cox model should
        ///   be computed using the mean-centered version of the covariates.
        ///   Default is true.
        /// </summary>
        /// 
        public bool Normalize
        {
            get { return normalize; }
            set { normalize = value; }
        }


        /// <summary>
        ///   Constructs a new Newton-Raphson learning algorithm
        ///   for Cox's Proportional Hazards models.
        /// </summary>
        /// 
        /// <param name="hazards">The model to estimate.</param>
        /// 
        public ProportionalHazardsNewtonRaphson(ProportionalHazards hazards)
        {
            this.regression = hazards;
            this.parameterCount = hazards.Coefficients.Length;

            this.hessian = new double[parameterCount, parameterCount];
            this.gradient = new double[parameterCount];

            this.partialHessian = new double[parameterCount, parameterCount];
            this.partialGradient = new double[parameterCount];

            this.convergence = new RelativeParameterConvergence()
            {
                Iterations = 0,
                Tolerance = 1e-5
            };
        }


        /// <summary>
        ///   Runs one iteration of the Newton-Raphson update for Cox's hazards learning.
        /// </summary>
        /// 
        /// <param name="inputs">The input data.</param>
        /// <param name="time">The time-to-event for the training samples.</param>
        /// 
        /// <returns>The maximum relative change in the parameters after the iteration.</returns>
        /// 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
            "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "1#")]
        public double Run(double[][] inputs, double[] time)
        {
            int[] censor = new int[time.Length];
            for (int i = 0; i < censor.Length; i++)
                censor[i] = 1;

            return Run(inputs, time, censor);
        }

        /// <summary>
        ///   Runs one iteration of the Newton-Raphson update for Cox's hazards learning.
        /// </summary>
        /// 
        /// <param name="inputs">The input data.</param>
        /// <param name="censor">The output (event) associated with each input vector.</param>
        /// <param name="time">The time-to-event for the non-censored training samples.</param>
        /// 
        /// <returns>The maximum relative change in the parameters after the iteration.</returns>
        /// 
        public double Run(double[][] inputs, double[] time, int[] censor)
        {
            if (inputs.Length != time.Length || time.Length != censor.Length)
                throw new DimensionMismatchException("time",
                    "The inputs, time and output vector must have the same length.");

            double[] means = new double[parameterCount];
            double[] sdev = new double[parameterCount];
            for (int i = 0; i < sdev.Length; i++)
                sdev[i] = 1;

            if (normalize)
            {
                // Store means as regression centers
                means = inputs.Mean();
                for (int i = 0; i < means.Length; i++)
                    regression.Offsets[i] = means[i];

                // Convert to unit scores for increased accuracy
                sdev = Accord.Statistics.Tools.StandardDeviation(inputs);
                inputs = inputs.Subtract(means, 0).ElementwiseDivide(sdev, 0, inPlace: true);
            }

            // Sort data by time to accelerate performance
            if (!time.IsSorted(ComparerDirection.Descending))
                sort(ref inputs, ref time, ref censor);

            // Compute actual outputs
            double[] output = new double[inputs.Length];
            for (int i = 0; i < output.Length; i++)
                output[i] = regression.Compute(inputs[i]);

            // Compute ties
            int[] ties = new int[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                for (int j = 0; j < time.Length; j++)
                    if (time[j] == time[i]) ties[i]++;

            if (parameterCount == 0)
                return createBaseline(time, censor, output);

            CurrentIteration = 0;
            double smooth = 0.1;


            do // learning iterations until convergence
            {  // or maximum number of iterations reached

                CurrentIteration++;

                // Reset Hessian matrix and gradient
                Array.Clear(gradient, 0, gradient.Length);
                Array.Clear(hessian, 0, hessian.Length);

                // For each observation instance
                for (int i = 0; i < inputs.Length; i++)
                {
                    // Check if we should censor
                    if (censor[i] == 0) continue;

                    // Compute partials 
                    double den = 0;
                    Array.Clear(partialGradient, 0, partialGradient.Length);
                    Array.Clear(partialHessian, 0, partialHessian.Length);

                    for (int j = 0; j < inputs.Length; j++)
                    {
                        if (time[j] >= time[i])
                            den += output[j];
                    }

                    for (int j = 0; j < inputs.Length; j++)
                    {
                        if (time[j] >= time[i])
                        {
                            // Compute partial gradient
                            for (int k = 0; k < partialGradient.Length; k++)
                                partialGradient[k] += inputs[j][k] * output[j] / den;

                            // Compute partial Hessian
                            for (int ii = 0; ii < inputs[j].Length; ii++)
                                for (int jj = 0; jj < inputs[j].Length; jj++)
                                    partialHessian[ii, jj] += inputs[j][ii] * inputs[j][jj] * output[j] / den;
                        }
                    }

                    // Compute gradient vector
                    for (int j = 0; j < gradient.Length; j++)
                        gradient[j] += inputs[i][j] - partialGradient[j];

                    // Compute Hessian matrix
                    for (int j = 0; j < partialGradient.Length; j++)
                        for (int k = 0; k < partialGradient.Length; k++)
                            hessian[j, k] -= partialHessian[j, k] - partialGradient[j] * partialGradient[k];
                }


                // Decompose to solve the linear system. Usually the Hessian will
                // be invertible and LU will succeed. However, sometimes the Hessian
                // may be singular and a Singular Value Decomposition may be needed.

                // The SVD is very stable, but is quite expensive, being on average
                // about 10-15 times more expensive than LU decomposition. There are
                // other ways to avoid a singular Hessian. For a very interesting 
                // reading on the subject, please see:
                //
                //  - Jeff Gill & Gary King, "What to Do When Your Hessian Is Not Invertible",
                //    Sociological Methods & Research, Vol 33, No. 1, August 2004, 54-87.
                //    Available in: http://gking.harvard.edu/files/help.pdf
                //

                // Moreover, the computation of the inverse is optional, as it will
                // be used only to compute the standard errors of the regression.


                // Hessian Matrix is singular, try pseudo-inverse solution
                decomposition = new SingularValueDecomposition(hessian);
                double[] deltas = decomposition.Solve(gradient);


                // Update coefficients using the calculated deltas
                for (int i = 0; i < regression.Coefficients.Length; i++)
                    regression.Coefficients[i] -= smooth * deltas[i];

                smooth += 0.1;
                if (smooth > 1)
                    smooth = 1;

                // Check relative maximum parameter change
                convergence.NewValues = regression.Coefficients;


                if (convergence.HasDiverged)
                {
                    // Restore previous coefficients
                    for (int i = 0; i < regression.Coefficients.Length; i++)
                        regression.Coefficients[i] = convergence.OldValues[i];
                }


                // Recompute current outputs
                for (int i = 0; i < output.Length; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < regression.Coefficients.Length; j++)
                        sum += regression.Coefficients[j] * inputs[i][j];
                    output[i] = Math.Exp(sum);
                }

            } while (!convergence.HasConverged);


            for (int i = 0; i < regression.Coefficients.Length; i++)
                regression.Coefficients[i] /= sdev[i];

            if (computeStandardErrors)
            {
                // Grab the regression information matrix
                double[,] inverse = decomposition.Inverse();

                // Calculate coefficients' standard errors
                double[] standardErrors = regression.StandardErrors;
                for (int i = 0; i < standardErrors.Length; i++)
                    standardErrors[i] = Math.Sqrt(Math.Abs(inverse[i, i])) / sdev[i];
            }

            if (computeBaselineFunction)
                createBaseline(time, censor, output);

            return convergence.Delta;
        }

        private double createBaseline(double[] time, int[] censor, double[] output)
        {
            if (regression.BaselineHazard != null)
            {
                IFittingOptions options = null;

                if (regression.BaselineHazard is IFittableDistribution<double, EmpiricalHazardOptions>)
                {
                    // Compute an estimate of the cumulative Hazard
                    //   function using the Nelson-Aalen estimator
                    options = new EmpiricalHazardOptions()
                    {
                        Censor = censor,
                        Output = output,
                        Estimator = HazardEstimator.BreslowNelsonAalen
                    };
                }
                else if (regression.BaselineHazard is IFittableDistribution<double, SurvivalOptions>)
                {
                    // Compute an estimate of the cumulative Hazard
                    //   function using the Nelson-Aalen estimator
                    options = new SurvivalOptions()
                    {
                        Censor = censor
                    };
                }

                regression.BaselineHazard.Fit(time, null, options);
            }

            return 0;
        }


        private static void sort(ref double[][] inputs, ref double[] time, ref int[] output)
        {
            int[] idx = Matrix.Indices(0, inputs.Length);

            time = (double[])time.Clone();
            Array.Sort(time, idx, new GeneralComparer(ComparerDirection.Descending));

            inputs = inputs.Submatrix(idx);
            output = output.Submatrix(idx);
        }


    }
}
