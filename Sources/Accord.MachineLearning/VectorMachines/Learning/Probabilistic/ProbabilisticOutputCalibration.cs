// Accord Machine Learning Library
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

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using System;
    using Accord.Math;
    using Accord.Math.Optimization;
    using Accord.Statistics.Links;
    using Accord.Statistics.Kernels;
    using System.Threading;
    using Accord.Math.Optimization.Losses;

    /// <summary>
    ///   Probabilistic Output Calibration for Linear machines.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>Instead of producing probabilistic outputs, Support Vector Machines
    ///   express their decisions in the form of a distance from support vectors in
    ///   feature space. In order to convert the SVM outputs into probabilities,
    ///   Platt (1999) proposed the calibration of the SVM outputs using a sigmoid
    ///   (Logit) link function. Later, Lin et al (2007) provided a corrected and
    ///   improved version of Platt's probabilistic outputs. This class implements
    ///   the later.</para>
    ///   
    ///   <para>This class is not an actual learning algorithm, but a calibrator.
    ///   Machines passed as input to this algorithm should already have been trained
    ///   by a proper learning algorithm such as <see cref="SequentialMinimalOptimization{TKernel}">
    ///   Sequential Minimal Optimization (SMO)</see>.</para>
    ///   
    /// <para>
    ///   This class can also be used in combination with <see cref="MulticlassSupportVectorLearning{TKernel}"/>
    ///   or <see cref="MultilabelSupportVectorLearning{TKernel}"/> to learn <see cref="MulticlassSupportVectorMachine{TKernel}"/>s
    ///   using the <c>one-vs-one</c> or <c>one-vs-all</c> multi-class decision strategies, respectively.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///        John C. Platt. 1999. Probabilistic Outputs for Support Vector Machines and Comparisons to
    ///        Regularized Likelihood Methods. In ADVANCES IN LARGE MARGIN CLASSIFIERS (1999), pp. 61-74.</description></item>
    ///     <item><description>
    ///       Hsuan-Tien Lin, Chih-Jen Lin, and Ruby C. Weng. 2007. A note on Platt's probabilistic outputs
    ///       for support vector machines. Mach. Learn. 68, 3 (October 2007), 267-276. </description></item>
    ///   </list></para>   
    /// </remarks>
    /// 
    /// <example>
    ///   <para>
    ///   The following example shows how to calibrate a SVM that has
    ///   been trained to perform a simple XOR function.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\ProbabilisticOutputCalibrationTest.cs" region="doc_learn" />
    ///   
    ///   <para>
    ///   The next example shows how to solve a multi-class problem using a one-vs-one SVM 
    ///   where the binary machines are learned using SMO and calibrated using Platt's scaling.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn_calibration" />
    /// </example>
    ///   
    /// <seealso cref="SupportVectorMachine"/>
    /// <seealso cref="MulticlassSupportVectorLearning{TKernel}"/>
    /// <seealso cref="MultilabelSupportVectorLearning{TKernel}"/>
    /// 
    public class ProbabilisticOutputCalibration
        : ProbabilisticOutputCalibrationBase<SupportVectorMachine<IKernel<double[]>, double[]>, IKernel<double[]>, double[]>,
        ISupportVectorMachineLearning
    {
        /// <summary>
        ///   Initializes a new instance of Platt's Probabilistic Output Calibration algorithm.
        /// </summary>
        /// 
        /// <param name="machine">The support vector machine to be calibrated.</param>
        /// 
        public ProbabilisticOutputCalibration(SupportVectorMachine<IKernel<double[]>, double[]> machine)
            : base(machine)
        {
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please do not pass parameters in the constructor. Use the default constructor and the Learn method instead.")]
        public ProbabilisticOutputCalibration(ISupportVectorMachine<double[]> model, double[][] input, int[] output)
            : base(model, input, output)
        {
        }

        /// <summary>
        ///   Initializes a new instance of Platt's Probabilistic Output Calibration algorithm.
        /// </summary>
        /// 
        public ProbabilisticOutputCalibration()
        {

        }
    }

    /// <summary>
    ///   Probabilistic Output Calibration for Kernel machines.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>Instead of producing probabilistic outputs, Support Vector Machines
    ///   express their decisions in the form of a distance from support vectors in
    ///   feature space. In order to convert the SVM outputs into probabilities,
    ///   Platt (1999) proposed the calibration of the SVM outputs using a sigmoid
    ///   (Logit) link function. Later, Lin et al (2007) provided a corrected and
    ///   improved version of Platt's probabilistic outputs. This class implements
    ///   the later.</para>
    ///   
    ///   <para>This class is not an actual learning algorithm, but a calibrator.
    ///   Machines passed as input to this algorithm should already have been trained
    ///   by a proper learning algorithm such as <see cref="SequentialMinimalOptimization{TKernel}">
    ///   Sequential Minimal Optimization (SMO)</see>.</para>
    ///   
    /// <para>
    ///   This class can also be used in combination with <see cref="MulticlassSupportVectorLearning{TKernel}"/>
    ///   or <see cref="MultilabelSupportVectorLearning{TKernel}"/> to learn <see cref="MulticlassSupportVectorMachine{TKernel}"/>s
    ///   using the <c>one-vs-one</c> or <c>one-vs-all</c> multi-class decision strategies, respectively.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///        John C. Platt. 1999. Probabilistic Outputs for Support Vector Machines and Comparisons to
    ///        Regularized Likelihood Methods. In ADVANCES IN LARGE MARGIN CLASSIFIERS (1999), pp. 61-74.</description></item>
    ///     <item><description>
    ///       Hsuan-Tien Lin, Chih-Jen Lin, and Ruby C. Weng. 2007. A note on Platt's probabilistic outputs
    ///       for support vector machines. Mach. Learn. 68, 3 (October 2007), 267-276. </description></item>
    ///   </list></para>   
    /// </remarks>
    /// 
    /// <example>
    ///   <para>
    ///   The following example shows how to calibrate a SVM that has
    ///   been trained to perform a simple XOR function.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\ProbabilisticOutputCalibrationTest.cs" region="doc_learn" />
    ///   
    ///   <para>
    ///   The next example shows how to solve a multi-class problem using a one-vs-one SVM 
    ///   where the binary machines are learned using SMO and calibrated using Platt's scaling.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn_calibration" />
    /// </example>
    ///   
    ///   
    /// <seealso cref="SupportVectorMachine"/>
    /// <seealso cref="SequentialMinimalOptimization{TKernel}"/>
    /// <seealso cref="MulticlassSupportVectorLearning{TKernel}"/>
    /// <seealso cref="MultilabelSupportVectorLearning{TKernel}"/>
    /// 
    public class ProbabilisticOutputCalibration<TKernel> : 
        ProbabilisticOutputCalibrationBase<SupportVectorMachine<TKernel>, TKernel, double[]>
        where TKernel : IKernel<double[]>
    {
        /// <summary>
        ///   Initializes a new instance of Platt's Probabilistic Output Calibration algorithm.
        /// </summary>
        /// 
        /// <param name="machine">The support vector machine to be calibrated.</param>
        /// 
        public ProbabilisticOutputCalibration(SupportVectorMachine<TKernel> machine)
            : base(machine)
        {
        }

        /// <summary>
        ///   Initializes a new instance of Platt's Probabilistic Output Calibration algorithm.
        /// </summary>
        /// 
        public ProbabilisticOutputCalibration()
        {

        }
    }

    /// <summary>
    ///   Probabilistic Output Calibration for structured Kernel machines.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>Instead of producing probabilistic outputs, Support Vector Machines
    ///   express their decisions in the form of a distance from support vectors in
    ///   feature space. In order to convert the SVM outputs into probabilities,
    ///   Platt (1999) proposed the calibration of the SVM outputs using a sigmoid
    ///   (Logit) link function. Later, Lin et al (2007) provided a corrected and
    ///   improved version of Platt's probabilistic outputs. This class implements
    ///   the later.</para>
    ///   
    ///   <para>This class is not an actual learning algorithm, but a calibrator.
    ///   Machines passed as input to this algorithm should already have been trained
    ///   by a proper learning algorithm such as <see cref="SequentialMinimalOptimization{TKernel}">
    ///   Sequential Minimal Optimization (SMO)</see>.</para>
    ///   
    /// <para>
    ///   This class can also be used in combination with <see cref="MulticlassSupportVectorLearning{TKernel}"/>
    ///   or <see cref="MultilabelSupportVectorLearning{TKernel}"/> to learn <see cref="MulticlassSupportVectorMachine{TKernel}"/>s
    ///   using the <c>one-vs-one</c> or <c>one-vs-all</c> multi-class decision strategies, respectively.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///        John C. Platt. 1999. Probabilistic Outputs for Support Vector Machines and Comparisons to
    ///        Regularized Likelihood Methods. In ADVANCES IN LARGE MARGIN CLASSIFIERS (1999), pp. 61-74.</description></item>
    ///     <item><description>
    ///       Hsuan-Tien Lin, Chih-Jen Lin, and Ruby C. Weng. 2007. A note on Platt's probabilistic outputs
    ///       for support vector machines. Mach. Learn. 68, 3 (October 2007), 267-276. </description></item>
    ///   </list></para>   
    /// </remarks>
    /// 
    /// <example>
    ///   <para>
    ///   The following example shows how to calibrate a SVM that has
    ///   been trained to perform a simple XOR function.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\ProbabilisticOutputCalibrationTest.cs" region="doc_learn" />
    ///   
    ///   <para>
    ///   The next example shows how to solve a multi-class problem using a one-vs-one SVM 
    ///   where the binary machines are learned using SMO and calibrated using Platt's scaling.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn_calibration" />
    /// </example>
    ///   
    ///   
    /// <seealso cref="SupportVectorMachine"/>
    /// <seealso cref="MulticlassSupportVectorLearning{TKernel}"/>
    /// <seealso cref="MultilabelSupportVectorLearning{TKernel}"/>
    /// 
    public class ProbabilisticOutputCalibration<TKernel, TInput>
        : ProbabilisticOutputCalibrationBase<SupportVectorMachine<TKernel, TInput>, TKernel, TInput>
        where TKernel : IKernel<TInput>
#if !NETSTANDARD1_4
        where TInput : ICloneable
#endif
    {
        /// <summary>
        ///   Initializes a new instance of Platt's Probabilistic Output Calibration algorithm.
        /// </summary>
        /// 
        /// <param name="machine">The support vector machine to be calibrated.</param>
        /// 
        public ProbabilisticOutputCalibration(SupportVectorMachine<TKernel, TInput> machine)
            : base(machine)
        {
        }

        /// <summary>
        ///   Initializes a new instance of Platt's Probabilistic Output Calibration algorithm.
        /// </summary>
        /// 
        public ProbabilisticOutputCalibration()
        {
        }
    }

    /// <summary>
    ///   Probabilistic Output Calibration.
    /// </summary>
    /// 
    public abstract class ProbabilisticOutputCalibrationBase<TModel, TKernel, TInput> :
        BinaryLearningBase<TModel, TInput>,
        ISupportVectorMachineLearning<TInput>
        where TKernel : IKernel<TInput>
        where TModel : SupportVectorMachine<TKernel, TInput>
#if !NETSTANDARD1_4
        where TInput : ICloneable
#endif
    {

        private double[] distances;
        private double[] targets;

        // Parameter setting
        private int maxIterations = 100;     // Maximum number of iterations
        private double minStepSize = 1e-10;  // Minimum step taken in line search
        private double sigma = 1e-12;        // Set to any value > 0
        private double tolerance = 1e-5;


        /// <summary>
        ///   Initializes a new instance of Platt's Probabilistic Output Calibration algorithm.
        /// </summary>
        /// 
        /// <param name="machine">The support vector machine to be calibrated.</param>
        /// 
        public ProbabilisticOutputCalibrationBase(TModel machine)
        {
            this.Model = machine;
        }

        /// <summary>
        ///   Gets or sets the maximum number of
        ///   iterations. Default is 100. 
        /// </summary>
        /// 
        public int Iterations
        {
            get { return maxIterations; }
            set { maxIterations = value; }
        }

        /// <summary>
        ///   Gets or sets the tolerance under which the
        ///   answer must be found. Default is 1-e5.
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return tolerance; }
            set { tolerance = value; }
        }

        /// <summary>
        ///   Gets or sets the minimum step size used 
        ///   during line search. Default is 1e-10.
        /// </summary>
        /// 
        public double StepSize
        {
            get { return minStepSize; }
            set { minStepSize = value; }
        }



        /// <summary>
        ///   Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// 
        /// <param name="inputs">The model inputs.</param>
        /// <param name="outputs">The desired outputs associated with each <paramref name="inputs">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// 
        /// <returns>A model that has learned how to produce <paramref name="outputs"/> given <paramref name="inputs"/>.</returns>
        /// 
        public override TModel Learn(TInput[] inputs, bool[] outputs, double[] weights)
        {
            if (weights != null)
                throw new NotSupportedException();

            // This method is a direct implementation of the algorithm
            // as published by Hsuan-Tien Lin, Chih-Jen Lin and Ruby C.
            // Weng, 2007. See references in documentation for more details.
            // 

            // Learning data
            this.distances = new double[outputs.Length];
            this.targets = new double[outputs.Length];
            int positives = 0;
            int negatives = 0;

            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i])
                    positives++;
                else
                    negatives++;
            }

            // Compute the Support Vector Machine outputs
            // TODO: rename all 'results' to 'result'
            Model.Score(inputs, result: distances);

            // Define the target probabilities we aim to produce
            double high = (positives + 1.0) / (positives + 2.0);
            double low = 1.0 / (negatives + 2.0);

            for (int i = 0; i < outputs.Length; i++)
                targets[i] = outputs[i] ? high : low;

            // Initialize 
            double A = 0.0;
            double B = Math.Log((negatives + 1.0) / (positives + 1.0));
            double logLikelihood = 0;
            int iterations = 0;

            // Compute the log-likelihood function
            for (int i = 0; i < distances.Length; i++)
            {
                double y = distances[i] * A + B;

                if (y >= 0)
                    logLikelihood += targets[i] * y + Special.Log1p(Math.Exp(-y));
                else
                    logLikelihood += (targets[i] - 1) * y + Special.Log1p(Math.Exp(y));
            }

            // Start main algorithm loop.
            while (iterations < maxIterations)
            {
                if (Token.IsCancellationRequested)
                    break;

                iterations++;

                // Update the Gradient and Hessian
                //  (Using that H' = H + sigma I)

                double h11 = sigma;
                double h22 = sigma;
                double h21 = 0;

                double g1 = 0;
                double g2 = 0;

                for (int i = 0; i < distances.Length; i++)
                {
                    double p, q;
                    double y = distances[i] * A + B;

                    if (y >= 0)
                    {
                        p = Math.Exp(-y) / (1.0 + Math.Exp(-y));
                        q = 1.0 / (1.0 + Math.Exp(-y));
                    }
                    else
                    {
                        p = 1.0 / (1.0 + Math.Exp(y));
                        q = Math.Exp(y) / (1.0 + Math.Exp(y));
                    }

                    double d1 = targets[i] - p;
                    double d2 = p * q;

                    // Update Hessian
                    h11 += distances[i] * distances[i] * d2;
                    h22 += d2;
                    h21 += distances[i] * d2;

                    // Update Gradient
                    g1 += distances[i] * d1;
                    g2 += d1;
                }

                // Check if the gradient is near zero as stopping criteria
                if (Math.Abs(g1) < tolerance && Math.Abs(g2) < tolerance)
                    break;

                // Compute modified Newton directions
                double det = h11 * h22 - h21 * h21;
                double dA = -(h22 * g1 - h21 * g2) / det;
                double dB = -(-h21 * g1 + h11 * g2) / det;
                double gd = g1 * dA + g2 * dB;

                double stepSize = 1;

                // Perform a line search
                while (stepSize >= minStepSize)
                {
                    double newA = A + stepSize * dA;
                    double newB = B + stepSize * dB;
                    double newLogLikelihood = 0.0;

                    // Compute the new log-likelihood function
                    for (int i = 0; i < distances.Length; i++)
                    {
                        double y = distances[i] * newA + newB;

                        if (y >= 0)
                            newLogLikelihood += (targets[i]) * y + Special.Log1p(Math.Exp(-y));
                        else
                            newLogLikelihood += (targets[i] - 1) * y + Special.Log1p(Math.Exp(y));
                    }

                    // Check if a sufficient decrease has been obtained
                    if (newLogLikelihood < logLikelihood + 1e-4 * stepSize * gd)
                    {
                        // Yes, it has. Update parameters with the new values
                        A = newA; B = newB; logLikelihood = newLogLikelihood;
                        break;
                    }
                    else
                    {
                        // Decrease the step size until it can achieve
                        // a sufficient decrease or until it fails.
                        stepSize /= 2.0;
                    }

                    if (stepSize < minStepSize)
                    {
                        // No decrease could be obtained. 
                        break; // throw new LineSearchFailedException("No sufficient decrease was obtained.");
                    }
                }
            }


            // The iterative algorithm has converged
            for (int i = 0; i < Model.Weights.Length; i++)
                Model.Weights[i] *= -A;
            Model.Threshold = Model.Threshold * -A - B;
            Model.IsProbabilistic = true;

            return Model;
        }






        /// <summary>
        /// Initializes a new instance of the <see cref="ProbabilisticOutputCalibrationBase{TModel, TKernel, TInput}"/> class.
        /// </summary>
        public ProbabilisticOutputCalibrationBase()
        {
        }

        TInput[] input;
        int[] output;

        /// <summary>
        ///   Obsolete.
        /// </summary>
        protected ProbabilisticOutputCalibrationBase(ISupportVectorMachine<double[]> model, TInput[] input, int[] output)
        {
            this.Model = (TModel)model;
            this.input = input;
            this.output = output;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please use Learn() instead.")]
        public double Run()
        {
            Learn(input, output, null);
            return logLikelihood(input, output);
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please use Learn() instead.")]
        public double Run(bool computeError)
        {
            Learn(input, output, null);
            if (computeError)
                return logLikelihood(input, output);
            return 0;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please use Accord.Math.Optimization.BinaryCrossEntropyLoss or any other losses of your choice from the Accord.Math.Optimization namespace.")]
        public double LogLikelihood(TInput[] inputs, int[] outputs)
        {
            return logLikelihood(inputs, outputs);
        }

        private double logLikelihood(TInput[] inputs, int[] outputs)
        {
            // Compute the log-likelihood of the model
            double logLikelihood = 0.0;

            // Compute the new log-likelihood function
            for (int i = 0; i < inputs.Length; i++)
            {
                double y = Model.Score(inputs[i]);
                double t = outputs[i] == 1 ? 1 : 0;

                if (y >= 0)
                    logLikelihood += (t) * y + Special.Log1p(Math.Exp(-y));
                else
                    logLikelihood += (t - 1) * y + Special.Log1p(Math.Exp(y));
            }

            return logLikelihood;
        }



        ISupportVectorMachine<TInput> ISupervisedLearning<ISupportVectorMachine<TInput>, TInput, int>.Learn(TInput[] x, int[] y, double[] weights)
        {
            return Learn(x, y, weights);
        }

        ISupportVectorMachine<TInput> ISupervisedLearning<ISupportVectorMachine<TInput>, TInput, int[]>.Learn(TInput[] x, int[][] y, double[] weights)
        {
            return Learn(x, y, weights);
        }

        ISupportVectorMachine<TInput> ISupervisedLearning<ISupportVectorMachine<TInput>, TInput, bool[]>.Learn(TInput[] x, bool[][] y, double[] weights)
        {
            return Learn(x, y, weights);
        }

        ISupportVectorMachine<TInput> ISupervisedLearning<ISupportVectorMachine<TInput>, TInput, bool>.Learn(TInput[] x, bool[] y, double[] weights)
        {
            return Learn(x, y, weights);
        }

        ISupportVectorMachine<TInput> ISupervisedLearning<ISupportVectorMachine<TInput>, TInput, double>.Learn(TInput[] x, double[] y, double[] weights)
        {
            return Learn(x, y, weights);
        }
    }
}
