//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
// 
//
// Copyright © Sylvain Roy, 2002
// sro33 at student.canterbury.ac.nz
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//   Portions of this file have been based on the GPL code by Sylvain
//   Roy in SMOreg.java, a part of the Weka software package. It is,
//   thus, available under the same GPL license. This file is not linked
//   against the rest of the Accord.NET Framework and can only be used
//   in GPL applications. Please see the GPL license for more details.
//

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using System;
    using System.Collections.Generic;
    using Accord.Statistics.Kernels;

    /// <summary>
    ///   Sequential Minimal Optimization (SMO) Algorithm for Regression. Warning:
    ///   this code is contained in a GPL assembly. Thus, if you link against this
    ///   assembly, you should comply with the GPL license.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The SMO algorithm is an algorithm for solving large quadratic programming (QP)
    ///   optimization problems, widely used for the training of support vector machines.
    ///   First developed by John C. Platt in 1998, SMO breaks up large QP problems into
    ///   a series of smallest possible QP problems, which are then solved analytically.</para>
    /// <para>
    ///   This class incorporates modifications in the original SMO algorithm to solve
    ///   regression problems as suggested by Alex J. Smola and Bernhard Schölkopf and
    ///   further modifications for better performance by Shevade et al.</para> 
    ///   
    /// <para>
    ///   Portions of this implementation has been based on the GPL code by Sylvain Roy in SMOreg.java, a 
    ///   part of the Weka software package. It is, thus, available under the same GPL license. This file is
    ///   not linked against the rest of the Accord.NET Framework and can only be used in GPL applications.
    ///   This class is only available in the special Accord.MachineLearning.GPL assembly, which has to be
    ///   explicitly selected in the framework installation. Before linking against this assembly, please
    ///   read the <a href="http://www.gnu.org/copyleft/gpl.html">GPL license</a> for more details. This
    ///   assembly also should have been distributed with a copy of the GNU GPLv3 alongside with it.
    /// </para>
    /// 
    /// <para>
    ///   For a non-GPL'd version, see <see cref="FanChenLinSupportVectorRegression{TKernel}"/>.
    /// </para>
    /// 
    /// <para>
    ///   To use this class, add a reference to the <c>Accord.MachineLearning.GPL.dll</c> assembly
    ///   that resides inside the Release/GPL folder of the framework's installation directory.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///      A. J. Smola and B. Schölkopf. A Tutorial on Support Vector Regression. NeuroCOLT2
    ///      Technical Report Series, 1998. Available on: <a href="http://www.kernel-machines.org/publications/SmoSch98c">
    ///      http://www.kernel-machines.org/publications/SmoSch98c </a></description></item>
    ///     <item><description>
    ///      S.K. Shevade et al. Improvements to SMO Algorithm for SVM Regression, 1999. Available
    ///      on: <a href="http://drona.csa.iisc.ernet.in/~chiru/papers/ieee_smo_reg.ps.gz">
    ///      http://drona.csa.iisc.ernet.in/~chiru/papers/ieee_smo_reg.ps.gz </a></description></item>
    ///     <item><description>
    ///      G. W. Flake, S. Lawrence. Efficient SVM Regression Training with SMO.
    ///      Available on: <a href="http://www.keerthis.com/smoreg_ieee_shevade_00.pdf">
    ///      http://www.keerthis.com/smoreg_ieee_Shevade_00.pdf </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Sources\Extras\Accord.Tests.MachineLearning.GPL\SequentialMinimalOptimizationRegressionTest.cs" region="doc_learn" />
    /// </example>
    /// 
    //[Obsolete("Please use SequentialMinimalOptimizationRegression<TKernel> instead.")]
    public class SequentialMinimalOptimizationRegression :
        BaseSequentialMinimalOptimizationRegression<
            SupportVectorMachine<IKernel>, IKernel, double[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialMinimalOptimizationRegression"/> class.
        /// </summary>
        public SequentialMinimalOptimizationRegression()
        {

        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public SequentialMinimalOptimizationRegression(ISupportVectorMachine<double[]> machine, double[][] inputs, double[] outputs)
            : base(machine, inputs, outputs)
        {

        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        protected override SupportVectorMachine<IKernel> Create(int inputs, IKernel kernel)
        {
            return new SupportVectorMachine<IKernel>(inputs, kernel);
        }
    }

    /// <summary>
    ///   Sequential Minimal Optimization (SMO) Algorithm for Regression. Warning:
    ///   this code is contained in a GPL assembly. Thus, if you link against this
    ///   assembly, you should comply with the GPL license.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The SMO algorithm is an algorithm for solving large quadratic programming (QP)
    ///   optimization problems, widely used for the training of support vector machines.
    ///   First developed by John C. Platt in 1998, SMO breaks up large QP problems into
    ///   a series of smallest possible QP problems, which are then solved analytically.</para>
    /// <para>
    ///   This class incorporates modifications in the original SMO algorithm to solve
    ///   regression problems as suggested by Alex J. Smola and Bernhard Schölkopf and
    ///   further modifications for better performance by Shevade et al.</para> 
    ///   
    /// <para>
    ///   Portions of this implementation has been based on the GPL code by Sylvain Roy in SMOreg.java, a 
    ///   part of the Weka software package. It is, thus, available under the same GPL license. This file is
    ///   not linked against the rest of the Accord.NET Framework and can only be used in GPL applications.
    ///   This class is only available in the special Accord.MachineLearning.GPL assembly, which has to be
    ///   explicitly selected in the framework installation. Before linking against this assembly, please
    ///   read the <a href="http://www.gnu.org/copyleft/gpl.html">GPL license</a> for more details. This
    ///   assembly also should have been distributed with a copy of the GNU GPLv3 alongside with it.
    /// </para>
    /// 
    /// <para>
    ///   To use this class, add a reference to the <c>Accord.MachineLearning.GPL.dll</c> assembly
    ///   that resides inside the Release/GPL folder of the framework's installation directory.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///      A. J. Smola and B. Schölkopf. A Tutorial on Support Vector Regression. NeuroCOLT2
    ///      Technical Report Series, 1998. Available on: <a href="http://www.kernel-machines.org/publications/SmoSch98c">
    ///      http://www.kernel-machines.org/publications/SmoSch98c </a></description></item>
    ///     <item><description>
    ///      S.K. Shevade et al. Improvements to SMO Algorithm for SVM Regression, 1999. Available
    ///      on: <a href="http://drona.csa.iisc.ernet.in/~chiru/papers/ieee_smo_reg.ps.gz">
    ///      http://drona.csa.iisc.ernet.in/~chiru/papers/ieee_smo_reg.ps.gz </a></description></item>
    ///     <item><description>
    ///      G. W. Flake, S. Lawrence. Efficient SVM Regression Training with SMO.
    ///      Available on: <a href="http://www.keerthis.com/smoreg_ieee_shevade_00.pdf">
    ///      http://www.keerthis.com/smoreg_ieee_Shevade_00.pdf </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Sources\Extras\Accord.Tests.MachineLearning.GPL\SequentialMinimalOptimizationRegressionTest.cs" region="doc_learn" />
    /// </example>
    /// 
    public class SequentialMinimalOptimizationRegression<TKernel> :
        BaseSequentialMinimalOptimizationRegression<
            SupportVectorMachine<TKernel>, TKernel, double[]>
        where TKernel : IKernel<double[]>
    {
        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override SupportVectorMachine<TKernel> Create(int inputs, TKernel kernel)
        {
            return new SupportVectorMachine<TKernel>(inputs, kernel);
        }
    }

    /// <summary>
    ///   Sequential Minimal Optimization (SMO) Algorithm for Regression. Warning:
    ///   this code is contained in a GPL assembly. Thus, if you link against this
    ///   assembly, you should comply with the GPL license.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The SMO algorithm is an algorithm for solving large quadratic programming (QP)
    ///   optimization problems, widely used for the training of support vector machines.
    ///   First developed by John C. Platt in 1998, SMO breaks up large QP problems into
    ///   a series of smallest possible QP problems, which are then solved analytically.</para>
    /// <para>
    ///   This class incorporates modifications in the original SMO algorithm to solve
    ///   regression problems as suggested by Alex J. Smola and Bernhard Schölkopf and
    ///   further modifications for better performance by Shevade et al.</para> 
    ///   
    /// <para>
    ///   Portions of this implementation has been based on the GPL code by Sylvain Roy in SMOreg.java, a 
    ///   part of the Weka software package. It is, thus, available under the same GPL license. This file is
    ///   not linked against the rest of the Accord.NET Framework and can only be used in GPL applications.
    ///   This class is only available in the special Accord.MachineLearning.GPL assembly, which has to be
    ///   explicitly selected in the framework installation. Before linking against this assembly, please
    ///   read the <a href="http://www.gnu.org/copyleft/gpl.html">GPL license</a> for more details. This
    ///   assembly also should have been distributed with a copy of the GNU GPLv3 alongside with it.
    /// </para>
    /// 
    /// <para>
    ///   To use this class, add a reference to the <c>Accord.MachineLearning.GPL.dll</c> assembly
    ///   that resides inside the Release/GPL folder of the framework's installation directory.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///      A. J. Smola and B. Schölkopf. A Tutorial on Support Vector Regression. NeuroCOLT2
    ///      Technical Report Series, 1998. Available on: <a href="http://www.kernel-machines.org/publications/SmoSch98c">
    ///      http://www.kernel-machines.org/publications/SmoSch98c </a></description></item>
    ///     <item><description>
    ///      S.K. Shevade et al. Improvements to SMO Algorithm for SVM Regression, 1999. Available
    ///      on: <a href="http://drona.csa.iisc.ernet.in/~chiru/papers/ieee_smo_reg.ps.gz">
    ///      http://drona.csa.iisc.ernet.in/~chiru/papers/ieee_smo_reg.ps.gz </a></description></item>
    ///     <item><description>
    ///      G. W. Flake, S. Lawrence. Efficient SVM Regression Training with SMO.
    ///      Available on: <a href="http://www.keerthis.com/smoreg_ieee_shevade_00.pdf">
    ///      http://www.keerthis.com/smoreg_ieee_Shevade_00.pdf </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Sources\Extras\Accord.Tests.MachineLearning.GPL\SequentialMinimalOptimizationRegressionTest.cs" region="doc_learn" />
    /// </example>
    /// 
    public class SequentialMinimalOptimizationRegression<TKernel, TInput> :
        BaseSequentialMinimalOptimizationRegression<
            SupportVectorMachine<TKernel, TInput>, TKernel, TInput>
        where TKernel : IKernel<TInput>
#if !NETSTANDARD1_4
        where TInput : ICloneable
#endif
    {
        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override SupportVectorMachine<TKernel, TInput> Create(int inputs, TKernel kernel)
        {
            return new SupportVectorMachine<TKernel, TInput>(inputs, kernel);
        }
    }

    /// <summary>
    ///   Base class for Sequential Minimal Optimization for regression.
    /// </summary>
    public abstract class BaseSequentialMinimalOptimizationRegression<TModel, TKernel, TInput> :
        BaseSupportVectorRegression<TModel, TKernel, TInput>
        where TKernel : IKernel<TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>
#if !NETSTANDARD1_4
        where TInput : ICloneable
#endif
    {

        // Learning algorithm parameters
        private double tolerance = 1e-3;
        private double epsilon = 1e-3;
        private double roundingEpsilon = 1e-12;

        // Support Vector Machine parameters
        private double[] alpha_a;
        private double[] alpha_b;


        // Improvements on the original SMO algorithm
        //  for better performance and efficiency:

        //  - Multiple thresholds
        private double biasLower;
        private double biasUpper;
        private int biasLowerIndex;
        private int biasUpperIndex;

        //  - Sets of indices
        private HashSet<int> I0;
        private HashSet<int> I1;
        private HashSet<int> I2;
        private HashSet<int> I3;

        // Error cache to speed up computations
        private double[] errors;



        /// <summary>
        ///   Initializes a new instance of a Sequential Minimal Optimization (SMO) algorithm.
        /// </summary>
        /// 
        public BaseSequentialMinimalOptimizationRegression()
        {
        }



        /// <summary>
        ///   Convergence tolerance. Default value is 1e-3.
        /// </summary>
        /// 
        /// <remarks>
        ///   The criterion for completing the model training process. The default is 0.001.
        /// </remarks>
        /// 
        public double Tolerance
        {
            get { return this.tolerance; }
            set { this.tolerance = value; }
        }


        /// <summary>
        /// Runs the learning algorithm.
        /// </summary>
        protected override void InnerRun()
        {
            // The SMO algorithm chooses to solve the smallest possible optimization problem
            // at every step. At every step, SMO chooses two Lagrange multipliers to jointly
            // optimize, finds the optimal values for these multipliers, and updates the SVM
            // to reflect the new optimal values
            //
            // References:
            //   - http://research.microsoft.com/en-us/um/people/jplatt/smoTR.pdf
            //   - http://www.kernel-machines.org/publications/SmoSch98c
            //   - http://drona.csa.iisc.ernet.in/~chiru/papers/ieee_smo_reg.ps.gz


            // Initialize variables
            int N = Inputs.Length;
            
            // Set the value of the ε parameter controlled the width of the ε-insensitive zone
            epsilon = Epsilon;

            // Lagrange multipliers
            this.alpha_a = new double[N];
            this.alpha_b = new double[N];

            // Error cache
            this.errors = new double[N];

            // Indices sets
            this.I0 = new HashSet<int>();
            this.I1 = new HashSet<int>();
            this.I2 = new HashSet<int>();
            this.I3 = new HashSet<int>();

            // Set the second set of indices to contain all inputs
            for (int i = 0; i < N; i++) this.I1.Add(i);

            // Choose any example from the training set as starting bias
            this.biasUpperIndex = 0; // any example, such as 0
            this.biasLowerIndex = 0;
            this.biasUpper = Outputs[0] + epsilon;
            this.biasLower = Outputs[0] - epsilon;


            // Algorithm:
            int numChanged = 0;
            bool examineAll = true;
            while (numChanged > 0 || examineAll)
            {
                if (Token.IsCancellationRequested)
                    break;

                numChanged = 0;
                if (examineAll)
                {
                    // loop over all examples
                    for (int i = 0; i < N; i++)
                        numChanged += examineExample(i);
                }
                else
                {
                    // loop over all examples where a and a*
                    //  are greater than 0 and less than C.

                    for (int i = 0; i < N; i++)
                    {
                        if ((0 < alpha_a[i] && alpha_a[i] < this.C[i]) ||
                            (0 < alpha_b[i] && alpha_b[i] < this.C[i]))
                        {
                            numChanged += examineExample(i);

                            if (biasUpper > biasLower - 2.0 * tolerance)
                            {
                                numChanged = 0;
                                break;
                            }
                        }
                    }
                }

                if (examineAll)
                    examineAll = false;
                else if (numChanged == 0)
                    examineAll = true;
            }


            // Store Support Vectors in the SV Machine. Only vectors which have Lagrange multipliers
            // greater than zero will be stored as only those are actually required during evaluation.
            List<int> indices = new List<int>();
            for (int i = 0; i < N; i++)
            {
                // Only store vectors with multipliers > 0
                if (alpha_a[i] > 0 || alpha_b[i] > 0) indices.Add(i);
            }

            int vectors = indices.Count;
            Model.SupportVectors = new TInput[vectors];
            Model.Weights = new double[vectors];
            for (int i = 0; i < vectors; i++)
            {
                int j = indices[i];
                Model.SupportVectors[i] = Inputs[j];
                Model.Weights[i] = alpha_a[j] - alpha_b[j];
            }
            Model.Threshold = (biasLower + biasUpper) / 2.0;
        }


        /// <summary>
        ///  Chooses which multipliers to optimize using heuristics.
        /// </summary>
        /// 
        private int examineExample(int i2)
        {

            double alpha2a = alpha_a[i2]; // Lagrange multiplier a  for i2
            double alpha2b = alpha_b[i2]; // Lagrange multiplier a* for i2 


#region Compute example error
            double e2 = 0.0;
            if (I0.Contains(i2))
            {
                // Value is cached
                e2 = errors[i2];
            }
            else
            {
                // Value is not cached and should be computed
                errors[i2] = e2 = Outputs[i2] - compute(Inputs[i2]);

                // Update thresholds
                if (I1.Contains(i2))
                {
                    if (e2 + epsilon < biasUpper)
                    {
                        biasUpper = e2 + epsilon;
                        biasUpperIndex = i2;
                    }
                    else if (e2 - epsilon > biasLower)
                    {
                        biasLower = e2 - epsilon;
                        biasLowerIndex = i2;
                    }
                }
                else if (I2.Contains(i2) && (e2 + epsilon > biasLower))
                {
                    biasLower = e2 + epsilon;
                    biasLowerIndex = i2;
                }
                else if (I3.Contains(i2) && (e2 - epsilon < biasUpper))
                {
                    biasUpper = e2 - epsilon;
                    biasUpperIndex = i2;
                }
            }
#endregion


#region Check optimality using current thresholds
            // Check optimality using current thresholds then select
            //   the best i1 to joint optimize when appropriate.

            int i1 = -1;
            bool optimal = true;


            // In case i2 is in the first set of indices:
            if (I0.Contains(i2))
            {
                if (0 < alpha2a && alpha2a < C[i2])
                {
                    if (biasLower - (e2 - epsilon) > 2.0 * tolerance)
                    {
                        optimal = false;

                        i1 = biasLowerIndex;
                        if ((e2 - epsilon) - biasUpper > biasLower - (e2 - epsilon))
                            i1 = biasUpperIndex;
                    }
                    else if ((e2 - epsilon) - biasUpper > 2.0 * tolerance)
                    {
                        optimal = false;

                        i1 = biasUpperIndex;
                        if (biasLower - (e2 - epsilon) > (e2 - epsilon) - biasUpper)
                            i1 = biasLowerIndex;
                    }
                }

                else if (0 < alpha2b && alpha2b < C[i2])
                {
                    if (biasLower - (e2 + epsilon) > 2.0 * tolerance)
                    {
                        optimal = false;

                        i1 = biasLowerIndex;
                        if ((e2 + epsilon) - biasUpper > biasLower - (e2 + epsilon))
                            i1 = biasUpperIndex;
                    }
                    else if ((e2 + epsilon) - biasUpper > 2.0 * tolerance)
                    {
                        optimal = false;

                        i1 = biasUpperIndex;
                        if (biasLower - (e2 + epsilon) > (e2 + epsilon) - biasUpper)
                            i1 = biasLowerIndex;
                    }
                }
            }

            // In case i2 is in the second set of indices:
            else if (I1.Contains(i2))
            {
                if (biasLower - (e2 + epsilon) > 2.0 * tolerance)
                {
                    optimal = false;

                    i1 = biasLowerIndex;
                    if ((e2 + epsilon) - biasUpper > biasLower - (e2 + epsilon))
                        i1 = biasUpperIndex;
                }
                else if ((e2 - epsilon) - biasUpper > 2.0 * tolerance)
                {
                    optimal = false;

                    i1 = biasUpperIndex;
                    if (biasLower - (e2 - epsilon) > (e2 - epsilon) - biasUpper)
                        i1 = biasLowerIndex;
                }
            }

            // In case i2 is in the third set of indices:
            else if (I2.Contains(i2))
            {
                if ((e2 + epsilon) - biasUpper > 2.0 * tolerance)
                {
                    optimal = false;
                    i1 = biasUpperIndex;
                }
            }

            // In case i2 is in the fourth set of indices:
            else if (I3.Contains(i2))
            {
                if (biasLower - (e2 - epsilon) > 2.0 * tolerance)
                {
                    optimal = false;
                    i1 = biasLowerIndex;
                }
            }
            else
            {
                throw new InvalidOperationException("The index could not be found.");
            }
#endregion


            if (optimal)
            {
                // The examples are already optimal.
                return 0;//  No need to optimize.
            }
            else
            {
                // Optimize i1 and i2
                if (takeStep(i1, i2)) return 1;
            }

            return 0;
        }

        /// <summary>
        ///   Analytically solves the optimization problem for two Lagrange multipliers.
        /// </summary>
        /// 
        private bool takeStep(int i1, int i2)
        {
            if (i1 == i2)
                return false;

            // Lagrange multipliers
            double alpha1a = alpha_a[i1];
            double alpha1b = alpha_b[i1];
            double alpha2a = alpha_a[i2];
            double alpha2b = alpha_b[i2];

            // Errors
            double e1 = errors[i1];
            double e2 = errors[i2];
            double delta = e1 - e2;

            double c1 = C[i1];
            double c2 = C[i2];

            // Kernel evaluation
            double k11 = Kernel.Function(Inputs[i1], Inputs[i1]);
            double k12 = Kernel.Function(Inputs[i1], Inputs[i2]);
            double k22 = Kernel.Function(Inputs[i2], Inputs[i2]);
            double eta = k11 + k22 - 2.0 * k12;
            double gamma = alpha1a - alpha1b + alpha2a - alpha2b;

            // Assume the kernel is positive definite.
            if (eta < 0) eta = 0;



#region Optimize
            bool case1 = false;
            bool case2 = false;
            bool case3 = false;
            bool case4 = false;

            bool changed = false;
            bool finished = false;

            double L, H, a1, a2;

            while (!finished)
            {

                if (!case1
                         && (alpha1a > 0 || (alpha1b == 0 && delta > 0))
                         && (alpha2a > 0 || (alpha2b == 0 && delta < 0)))
                {
                    // Compute L and H (w.r.t. alpha1, alpha2)
                    L = Math.Max(0, gamma - c1);
                    H = Math.Min(c1, gamma);

                    if (L < H)
                    {
                        if (eta > 0)
                        {
                            a2 = alpha2a - (delta / eta);

                            if (a2 > H) a2 = H;
                            else if (a2 < L) a2 = L;
                        }
                        else
                        {
                            double Lobj = -L * delta;
                            double Hobj = -H * delta;

                            if (Lobj > Hobj) a2 = L;
                            else a2 = H;
                        }

                        a1 = alpha1a - (a2 - alpha2a);

                        // Update alpha1, alpha2 if change is larger than some epsilon
                        if (Math.Abs(a1 - alpha1a) > roundingEpsilon ||
                            Math.Abs(a2 - alpha2a) > roundingEpsilon)
                        {
                            alpha1a = a1;
                            alpha2a = a2;
                            changed = true;
                        }
                    }
                    else finished = true;

                    case1 = true;
                }

                else if (!case2
                         && (alpha1a > 0 || (alpha1b == 0 && delta > 2 * epsilon))
                         && (alpha2b > 0 || (alpha2a == 0 && delta > 2 * epsilon)))
                {
                    // Compute L and H  (w.r.t. alpha1, alpha2*)
                    L = Math.Max(0, -gamma);
                    H = Math.Min(c1, -gamma + c1);

                    if (L < H)
                    {
                        if (eta > 0)
                        {
                            a2 = alpha2b + ((delta - 2 * epsilon) / eta);

                            if (a2 > H) a2 = H;
                            else if (a2 < L) a2 = L;
                        }
                        else
                        {
                            double Lobj = L * (-2 * epsilon + delta);
                            double Hobj = H * (-2 * epsilon + delta);

                            if (Lobj > Hobj) a2 = L;
                            else a2 = H;
                        }
                        a1 = alpha1a + (a2 - alpha2b);

                        // Update alpha1, alpha2* if change is larger than some epsilon
                        if (Math.Abs(a1 - alpha1a) > roundingEpsilon ||
                            Math.Abs(a2 - alpha2b) > roundingEpsilon)
                        {
                            alpha1a = a1;
                            alpha2b = a2;
                            changed = true;
                        }
                    }
                    else finished = true;

                    case2 = true;
                }

                else if (!case3
                      && (alpha1b > 0 || (alpha1a == 0 && delta < -2 * epsilon))
                      && (alpha2a > 0 || (alpha2b == 0 && delta < -2 * epsilon)))
                {
                    // Compute L and H (w.r.t. alpha1*, alpha2)
                    L = Math.Max(0, gamma);
                    H = Math.Min(c1, c1 + gamma);

                    if (L < H)
                    {
                        if (eta > 0)
                        {
                            a2 = alpha2a - ((delta + 2 * epsilon) / eta);

                            if (a2 > H) a2 = H;
                            else if (a2 < L) a2 = L;
                        }
                        else
                        {
                            double Lobj = -L * (2 * epsilon + delta);
                            double Hobj = -H * (2 * epsilon + delta);

                            if (Lobj > Hobj) a2 = L;
                            else a2 = H;
                        }
                        a1 = alpha1b + (a2 - alpha2a);

                        // Update alpha1*, alpha2 if change is larger than some epsilon
                        if (Math.Abs(a1 - alpha1b) > roundingEpsilon ||
                            Math.Abs(a2 - alpha2a) > roundingEpsilon)
                        {
                            alpha1b = a1;
                            alpha2a = a2;
                            changed = true;
                        }
                    }
                    else finished = true;

                    case3 = true;
                }

                else if (!case4
                      && (alpha1b > 0 || (alpha1a == 0 && delta < 0))
                      && (alpha2b > 0 || (alpha2a == 0 && delta > 0)))
                {
                    // Compute L and H (w.r.t. alpha1*, alpha2*)
                    L = Math.Max(0, -gamma - c1);
                    H = Math.Min(c1, -gamma);

                    if (L < H)
                    {
                        if (eta > 0)
                        {
                            a2 = alpha2b + delta / eta;

                            if (a2 > H) a2 = H;
                            else if (a2 < L) a2 = L;
                        }
                        else
                        {
                            double Lobj = L * delta;
                            double Hobj = H * delta;

                            if (Lobj > Hobj) a2 = L;
                            else a2 = H;
                        }

                        a1 = alpha1b - (a2 - alpha2b);

                        // Update alpha1*, alpha2* if change is larger than some epsilon
                        if (Math.Abs(a1 - alpha1b) > roundingEpsilon ||
                            Math.Abs(a2 - alpha2b) > roundingEpsilon)
                        {
                            alpha1b = a1;
                            alpha2b = a2;
                            changed = true;
                        }
                    }
                    else finished = true;

                    case4 = true;
                }

                else finished = true;

                // Update the delta
                delta += eta * ((alpha2a - alpha2b) - (alpha_a[i2] - alpha_b[i2]));
            }


            // If nothing has changed, return false.
            if (!changed)
                return false;
#endregion



            // Update error cache using new Lagrange multipliers
            foreach (int i in I0)
            {
                if (i != i1 && i != i2)
                {
                    // Update all in set i0 except i1 and i2 (because we have the kernel function cached for them)
                    errors[i] += ((alpha_a[i1] - alpha_b[i1]) - (alpha1a - alpha1b)) * Kernel.Function(Inputs[i1], Inputs[i])
                               + ((alpha_a[i2] - alpha_b[i2]) - (alpha2a - alpha2b)) * Kernel.Function(Inputs[i2], Inputs[i]);
                }
            }

            // Update error cache using new Lagrange multipliers for i1 and i2
            errors[i1] += ((alpha_a[i1] - alpha_b[i1]) - (alpha1a - alpha1b)) * k11
                        + ((alpha_a[i2] - alpha_b[i2]) - (alpha2a - alpha2b)) * k12;
            errors[i2] += ((alpha_a[i1] - alpha_b[i1]) - (alpha1a - alpha1b)) * k12
                        + ((alpha_a[i2] - alpha_b[i2]) - (alpha2a - alpha2b)) * k22;


            // to prevent precision problems
            double m_Del = 1e-10;
            if (alpha1a > c1 - m_Del * c1)
            {
                alpha1a = c1;
            }
            else if (alpha1a <= m_Del * c1)
            {
                alpha1a = 0;
            }
            if (alpha1b > c1 - m_Del * c1)
            {
                alpha1b = c1;
            }
            else if (alpha1b <= m_Del * c1)
            {
                alpha1b = 0;
            }
            if (alpha2a > c2 - m_Del * c2)
            {
                alpha2a = c2;
            }
            else if (alpha2a <= m_Del * c2)
            {
                alpha2a = 0;
            }
            if (alpha2b > c2 - m_Del * c2)
            {
                alpha2b = c2;
            }
            else if (alpha2b <= m_Del * c2)
            {
                alpha2b = 0;
            }


            // Store the changes in the alpha, alpha* arrays
            alpha_a[i1] = alpha1a;
            alpha_b[i1] = alpha1b;
            alpha_a[i2] = alpha2a;
            alpha_b[i2] = alpha2b;


            // Update the sets of indices (for i1)
            if ((0 < alpha1a && alpha1a < c1) || (0 < alpha1b && alpha1b < c1))
                I0.Add(i1);
            else I0.Remove(i1);

            if (alpha1a == 0 && alpha1b == 0)
                I1.Add(i1);
            else I1.Remove(i1);

            if (alpha1a == 0 && alpha1b == c2)
                I2.Add(i1);
            else I2.Remove(i1);

            if (alpha1a == c1 && alpha1b == 0)
                I3.Add(i1);
            else I3.Remove(i1);

            // Update the sets of indices (for i2)
            if ((0 < alpha2a && alpha2a < c2) || (0 < alpha2b && alpha2b < c2))
                I0.Add(i2);
            else I0.Remove(i2);

            if (alpha2a == 0 && alpha2b == 0)
                I1.Add(i2);
            else I1.Remove(i2);

            if (alpha2a == 0 && alpha2b == c2)
                I2.Add(i2);
            else I2.Remove(i2);

            if (alpha2a == c2 && alpha2b == 0)
                I3.Add(i2);
            else I3.Remove(i2);


            // Compute the new thresholds
            biasLower = Double.MinValue;
            biasUpper = Double.MaxValue;
            biasLowerIndex = -1;
            biasUpperIndex = -1;

            foreach (int i in I0)
            {
                if (0 < alpha_b[i] && alpha_b[i] < C[i]
                    && errors[i] + epsilon > biasLower)
                {
                    biasLower = errors[i] + epsilon;
                    biasLowerIndex = i;
                }
                else if (0 < alpha_a[i] && alpha_a[i] < C[i]
                    && errors[i] - epsilon > biasLower)
                {
                    biasLower = errors[i] - epsilon;
                    biasLowerIndex = i;
                }
                if (0 < alpha_a[i] && alpha_a[i] < C[i]
                    && errors[i] - epsilon < biasUpper)
                {
                    biasUpper = errors[i] - epsilon;
                    biasUpperIndex = i;
                }
                else if (0 < alpha_b[i] && alpha_b[i] < C[i]
                    && errors[i] + epsilon < biasUpper)
                {
                    biasUpper = errors[i] + epsilon;
                    biasUpperIndex = i;
                }
            }

            if (!I0.Contains(i1))
            {
                if (I2.Contains(i1) && errors[i1] + epsilon > biasLower)
                {
                    biasLower = errors[i1] + epsilon;
                    biasLowerIndex = i1;
                }
                else if (I1.Contains(i1) && errors[i1] - epsilon > biasLower)
                {
                    biasLower = errors[i1] - epsilon;
                    biasLowerIndex = i1;
                }

                if (I3.Contains(i1) && errors[i1] - epsilon < biasUpper)
                {
                    biasUpper = errors[i1] - epsilon;
                    biasUpperIndex = i1;
                }
                else if (I1.Contains(i1) && errors[i1] + epsilon < biasUpper)
                {
                    biasUpper = errors[i1] + epsilon;
                    biasUpperIndex = i1;
                }
            }

            if (!I0.Contains(i2))
            {
                if (I2.Contains(i2) && errors[i2] + epsilon > biasLower)
                {
                    biasLower = errors[i2] + epsilon;
                    biasLowerIndex = i2;
                }
                else if (I1.Contains(i2) && errors[i2] - epsilon > biasLower)
                {
                    biasLower = errors[i2] - epsilon;
                    biasLowerIndex = i2;
                }

                if (I3.Contains(i2) && errors[i2] - epsilon < biasUpper)
                {
                    biasUpper = errors[i2] - epsilon;
                    biasUpperIndex = i2;
                }
                else if (I1.Contains(i2) && errors[i2] + epsilon < biasUpper)
                {
                    biasUpper = errors[i2] + epsilon;
                    biasUpperIndex = i2;
                }
            }

            if (biasLowerIndex == -1 || biasUpperIndex == -1)
                throw new InvalidOperationException("Unexpected status.");


            // Success.
            return true;
        }

        /// <summary>
        ///   Computes the SVM output for a given point.
        /// </summary>
        /// 
        private double compute(TInput point)
        {
            double sum = 0;
            for (int j = 0; j < alpha_a.Length; j++)
                sum += (alpha_a[j] - alpha_b[j]) * Kernel.Function(point, Inputs[j]);
            return sum;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        public BaseSequentialMinimalOptimizationRegression(ISupportVectorMachine<TInput> machine, TInput[] inputs, double[] outputs)
            : base(machine, inputs, outputs)
        {

        }
    }
}
