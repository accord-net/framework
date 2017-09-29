// Accord Statistics Library
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

namespace Accord.Statistics.Models.Regression
{
    using System;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Testing;
    using Accord.Math;
    using Accord.MachineLearning;
    using Accord.Compat;

    /// <summary>
    ///   Cox's Proportional Hazards Model.
    /// </summary>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Regression\CoxProportionalHazardsTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="Accord.Statistics.Models.Regression.Fitting.ProportionalHazardsNewtonRaphson"/>
    /// <seealso cref="Accord.Statistics.Analysis.ProportionalHazardsAnalysis"/>
    /// 
    [Serializable]
    public sealed class ProportionalHazards :
        BinaryLikelihoodClassifierBase<Tuple<double[], double>>
    {

        /// <summary>
        ///   Gets the mean vector used to center observations before computations.
        /// </summary>
        /// 
        [Obsolete("Please use Intercept instead.")]
        public double[] Offsets { get; private set; }

        /// <summary>
        ///   Gets or sets the intercept (bias) for the regression model.
        /// </summary>
        /// 
        public double Intercept { get; set; }

        /// <summary>
        ///   Gets the coefficient vector, in which the
        ///   first value is always the intercept value.
        /// </summary>
        /// 
        public double[] Coefficients { get; private set; }

        /// <summary>
        ///   Gets the standard errors associated with each
        ///   coefficient during the model estimation phase.
        /// </summary>
        /// 
        public double[] StandardErrors { get; private set; }

        /// <summary>
        ///   Gets the baseline hazard function, if specified.
        /// </summary>
        /// 
        public IUnivariateDistribution BaselineHazard { get; private set; }

        /// <summary>
        ///   Gets the number of inputs handled by this model.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfInputs instead.")]
        public int Inputs
        {
            get { return Coefficients.Length; }
        }

        /// <summary>
        ///   Creates a new Cox Proportional-Hazards Model.
        /// </summary>
        /// 
        /// <param name="inputs">The number of input variables for the model.</param>
        /// 
        public ProportionalHazards(int inputs)
            : this(inputs, new EmpiricalHazardDistribution()) { }

        /// <summary>
        ///   Creates a new Cox Proportional-Hazards Model.
        /// </summary>
        /// 
        /// <param name="inputs">The number of input variables for the model.</param>
        /// <param name="baseline">The initial baseline hazard distribution. Default is the <see cref="EmpiricalHazardDistribution"/>.</param>
        /// 
        public ProportionalHazards(int inputs, IUnivariateDistribution baseline)
        {
            Coefficients = new double[inputs];
            StandardErrors = new double[inputs];
#pragma warning disable 612, 618
            Offsets = new double[inputs];
#pragma warning restore 612, 618
            BaselineHazard = baseline;
        }

        /// <summary>
        ///   Creates a new Cox Proportional-Hazards Model.
        /// </summary>
        /// 
        public ProportionalHazards()
        {
            BaselineHazard = new EmpiricalHazardDistribution();
        }

        /// <summary>
        ///   Obsolete. Please use the Probability(input) method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Probability(input) method instead.")]
        public double Compute(double[] input)
        {
            return Probability(input);
        }

        /// <summary>
        ///   Obsolete. Please use the Probability(input) method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Probability(input) method instead.")]
        public double[] Compute(double[][] input)
        {
            return Probability(input);
        }

        /// <summary>
        ///   Obsolete. Please use the Probability(input, time) method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Probability(input) method instead.")]
        public double Compute(double[] input, double time)
        {
            return Probability(input, time);
        }

        /// <summary>
        ///   Obsolete. Please use the Probability(input) method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Probability(input) method instead.")]
        public double Compute(double time)
        {
            return Probability(time);
        }

        /// <summary>
        ///   Computes the model's baseline survival function. This method
        ///   simply calls the <see cref="UnivariateContinuousDistribution.ComplementaryDistributionFunction"/>
        ///   of the <see cref="BaselineHazard"/> function.
        /// </summary>
        /// 
        /// <param name="time">The event time.</param>
        /// 
        /// <returns>The baseline survival function at the given time.</returns>
        /// 
        public double Survival(double time)
        {
            if (BaselineHazard == null)
                throw new InvalidOperationException();

            return BaselineHazard.ComplementaryDistributionFunction(time);
        }

        /// <summary>
        ///   Computes the model output for the given input vector.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="time">The event times.</param>
        /// 
        /// <returns>The probabilities of the event occurring at 
        /// the given times for the given observations.</returns>
        /// 
        [Obsolete("Please use Probability() instead.")]
        public double[] Compute(double[][] input, double[] time)
        {
            return Probability(input, time);
        }

        /// <summary>
        ///   Gets the Log-Hazard Ratio between two observations.
        /// </summary>
        /// 
        /// <param name="x">The first observation.</param>
        /// <param name="y">The second observation.</param>
        /// 
        public double GetLogHazardRatio(double[] x, double[] y)
        {
            double sum = 0;
            for (int i = 0; i < Coefficients.Length; i++)
                sum += Coefficients[i] * (x[i] - y[i]);
            return sum;
        }

        /// <summary>
        ///   Gets the Deviance for the model.
        /// </summary>
        /// 
        /// <remarks>
        ///   The deviance is defined as -2*Log-Likelihood.
        /// </remarks>
        /// 
        /// <param name="inputs">A set of input data.</param>
        /// <param name="time">The time-to-event before the output occurs.</param>
        /// <param name="output">The corresponding output data.</param>
        /// 
        /// <returns>
        ///   The deviance (a measure of performance) of the model
        ///   calculated over the given data sets.
        /// </returns>
        /// 
        public double GetDeviance(double[][] inputs, double[] time, SurvivalOutcome[] output)
        {
            return -2.0 * GetPartialLogLikelihood(inputs, time, output);
        }

        /// <summary>
        ///   Gets the Partial Log-Likelihood for the model.
        /// </summary>
        /// 
        /// <param name="inputs">A set of input data.</param>
        /// <param name="time">The time-to-event before the output occurs.</param>
        /// <param name="output">The corresponding output data.</param>
        ///
        /// <returns>
        ///   The Partial Log-Likelihood (a measure of performance)
        ///   of the model calculated over the given data set.
        /// </returns>
        /// 
        public double GetPartialLogLikelihood(double[][] inputs, double[] time, int[] output)
        {
            return GetPartialLogLikelihood(inputs, time, output.To<SurvivalOutcome[]>());
        }

        /// <summary>
        ///   Gets the Partial Log-Likelihood for the model.
        /// </summary>
        /// 
        /// <param name="inputs">A set of input data.</param>
        /// <param name="time">The time-to-event before the output occurs.</param>
        /// <param name="output">The corresponding output data.</param>
        ///
        /// <returns>
        ///   The Partial Log-Likelihood (a measure of performance)
        ///   of the model calculated over the given data set.
        /// </returns>
        /// 
        public double GetPartialLogLikelihood(double[][] inputs, double[] time, SurvivalOutcome[] output)
        {
#pragma warning disable 612, 618

            double sum1 = 0, sum2 = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                if (output[i] == 0)
                    continue;

                // Compute the first sum
                for (int j = 0; j < Coefficients.Length; j++)
                    sum1 += Coefficients[j] * (inputs[i][j] - Offsets[j]);

                // Compute the second sum
                double sum = 0;
                for (int j = 0; j < inputs.Length; j++)
                {
                    if (time[j] >= time[i])
                    {
                        double s = 0;
                        for (int k = 0; k < Coefficients.Length; k++)
                            s += Coefficients[k] * (inputs[j][k] - Offsets[k]);
                        sum += Math.Exp(s);
                    }
                }
                sum2 += Math.Log(sum);
            }

            return sum1 - sum2;
#pragma warning restore 612, 618
        }

        /// <summary>
        ///   Gets the Partial Log-Likelihood for the model.
        /// </summary>
        /// 
        /// <param name="time">The time-to-event before the output occurs.</param>
        /// <param name="output">The corresponding output data.</param>
        ///
        /// <returns>
        ///   The Partial Log-Likelihood (a measure of performance)
        ///   of the model calculated over the given data set.
        /// </returns>
        /// 
        public double GetPartialLogLikelihood(double[] time, int[] output)
        {
            return GetPartialLogLikelihood(time, output.To<SurvivalOutcome[]>());
        }

        /// <summary>
        ///   Gets the Partial Log-Likelihood for the model.
        /// </summary>
        /// 
        /// <param name="time">The time-to-event before the output occurs.</param>
        /// <param name="output">The corresponding output data.</param>
        ///
        /// <returns>
        ///   The Partial Log-Likelihood (a measure of performance)
        ///   of the model calculated over the given data set.
        /// </returns>
        /// 
        public double GetPartialLogLikelihood(double[] time, SurvivalOutcome[] output)
        {
            double sum2 = 0;
            for (int i = 0; i < time.Length; i++)
            {
                if (output[i] == 0)
                    continue;

                // Compute the second sum
                double sum = 0;
                for (int j = 0; j < time.Length; j++)
                {
                    if (time[j] >= time[i])
                        sum++;
                }

                sum2 += Math.Log(sum);
            }

            return -sum2;
        }


        /// <summary>
        ///   Gets the 95% confidence interval for the
        ///   Hazard Ratio for a given coefficient.
        /// </summary>
        /// 
        /// <param name="index">
        ///   The coefficient's index.
        /// </param>
        /// 
        public DoubleRange GetConfidenceInterval(int index)
        {
            double coeff = Coefficients[index];
            double error = StandardErrors[index];

            double upper = coeff + 1.9599 * error;
            double lower = coeff - 1.9599 * error;

            DoubleRange ci = new DoubleRange(Math.Exp(lower), Math.Exp(upper));

            return ci;
        }

        /// <summary>
        ///   Gets the Wald Test for a given coefficient.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Wald statistical test is a test for a model parameter in which
        ///   the estimated parameter θ is compared with another proposed parameter
        ///   under the assumption that the difference between them will be approximately
        ///   normal. There are several problems with the use of the Wald test. Please
        ///   take a look on substitute tests based on the log-likelihood if possible.
        /// </remarks>
        /// 
        /// <param name="index">
        ///   The coefficient's index. The first value
        ///   (at zero index) is the intercept value.
        /// </param>
        /// 
        public WaldTest GetWaldTest(int index)
        {
            return new WaldTest(Coefficients[index], 0.0, StandardErrors[index]);
        }

        /// <summary>
        ///   Gets the Log-Likelihood Ratio between two models.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Log-Likelihood ratio is defined as 2*(LL - LL0).
        /// </remarks>
        /// 
        /// <param name="input">A set of input data.</param>
        /// <param name="time">The time-to-event before the output occurs.</param>
        /// <param name="output">The corresponding output data.</param>
        /// <param name="hazards">Another Cox Proportional Hazards model.</param>
        /// 
        /// <returns>The Log-Likelihood ratio (a measure of performance
        /// between two models) calculated over the given data sets.</returns>
        /// 
        public double GetLogLikelihoodRatio(double[][] input, double[] time, SurvivalOutcome[] output, ProportionalHazards hazards)
        {
            return 2.0 * (this.GetPartialLogLikelihood(input, time, output) - hazards.GetPartialLogLikelihood(input, time, output));
        }

        /// <summary>
        ///   The likelihood ratio test of the overall model, also called the model chi-square test.
        /// </summary>
        /// 
        /// <param name="input">A set of input data.</param>
        /// <param name="time">The time-to-event before the output occurs.</param>
        /// <param name="output">The corresponding output data.</param>
        /// 
        /// <remarks>
        ///   <para>
        ///   The Chi-square test, also called the likelihood ratio test or the log-likelihood test
        ///   is based on the deviance of the model (-2*log-likelihood). The log-likelihood ratio test 
        ///   indicates whether there is evidence of the need to move from a simpler model to a more
        ///   complicated one (where the simpler model is nested within the complicated one).</para>
        ///   <para>
        ///   The difference between the log-likelihood ratios for the researcher's model and a
        ///   simpler model is often called the "model chi-square".</para>
        /// </remarks>
        /// 
        public ChiSquareTest ChiSquare(double[][] input, double[] time, int[] output)
        {
            return ChiSquare(input, time, output.To<SurvivalOutcome[]>());
        }

        /// <summary>
        ///   The likelihood ratio test of the overall model, also called the model chi-square test.
        /// </summary>
        /// 
        /// <param name="input">A set of input data.</param>
        /// <param name="time">The time-to-event before the output occurs.</param>
        /// <param name="output">The corresponding output data.</param>
        /// 
        /// <remarks>
        ///   <para>
        ///   The Chi-square test, also called the likelihood ratio test or the log-likelihood test
        ///   is based on the deviance of the model (-2*log-likelihood). The log-likelihood ratio test 
        ///   indicates whether there is evidence of the need to move from a simpler model to a more
        ///   complicated one (where the simpler model is nested within the complicated one).</para>
        ///   <para>
        ///   The difference between the log-likelihood ratios for the researcher's model and a
        ///   simpler model is often called the "model chi-square".</para>
        /// </remarks>
        /// 
        public ChiSquareTest ChiSquare(double[][] input, double[] time, SurvivalOutcome[] output)
        {
            ProportionalHazards regression = new ProportionalHazards(NumberOfInputs);

            double ratio = GetLogLikelihoodRatio(input, time, output, regression);

            return new ChiSquareTest(ratio, Coefficients.Length);
        }



        /// <summary>
        ///   Creates a new Cox's Proportional Hazards that is a copy of the current instance.
        /// </summary>
        /// 
        public object Clone()
        {
            var regression = new ProportionalHazards(Coefficients.Length);
            regression.Coefficients = (double[])this.Coefficients.Clone();
            regression.StandardErrors = (double[])this.StandardErrors.Clone();
            regression.Intercept = this.Intercept;
            return regression;
        }

        /// <summary>
        ///   Gets the Hazard Ratio for a given coefficient.
        /// </summary>
        /// 
        /// <remarks>
        ///   The hazard ratio can be computed raising Euler's number
        ///   (e ~~ 2.71) to the power of the associated coefficient.
        /// </remarks>
        /// <param name="index">
        ///   The coefficient's index. 
        /// </param>
        /// <returns>
        ///   The Hazard Ratio for the given coefficient.
        /// </returns>
        /// 
        public double GetHazardRatio(int index)
        {
            return Math.Exp(Coefficients[index]);
        }




        /// <summary>
        /// Predicts a class label vector for the given input vectors, returning the
        /// log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the log-likelihoods will be stored,
        /// avoiding unnecessary memory allocations.</param>
        /// 
        public override double[] LogLikelihood(Tuple<double[], double>[] input, double[] result)
        {
#pragma warning disable 612, 618
            for (int j = 0; j < input.Length; j++)
            {
                double sum = Intercept;
                for (int i = 0; i < Coefficients.Length; i++)
                    sum += Coefficients[i] * (input[j].Item1[i] - Offsets[i]);
                double h0 = BaselineHazard.LogCumulativeHazardFunction(input[j].Item2);
                result[j] = h0 + sum;
            }
            return result;
#pragma warning restore 612, 618
        }


        /// <summary>
        ///   Computes class-label decisions for each vector in the given <paramref name="input"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="time">The event times.</param>
        /// 
        public SurvivalOutcome Decide(double[] input, double time)
        {
            return Decide(Tuple.Create(input, time)) ? SurvivalOutcome.Failed : SurvivalOutcome.Censored;
        }

        /// <summary>
        ///   Computes class-label decisions for each vector in the given <paramref name="input"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="time">The event times.</param>
        /// 
        public SurvivalOutcome[] Decide(double[][] input, double[] time)
        {
            var result = new SurvivalOutcome[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = Decide(input[i], time[i]);
            return result;
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="time">The event times.</param>
        /// 
        public double Score(double[] input, double time)
        {
            return Probability(Tuple.Create(input, time));
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="time">The event times.</param>
        /// 
        public double[] Score(double[][] input, double[] time)
        {
            var result = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = Probability(input[i], time[i]);
            return result;
        }


        /// <summary>
        /// Predicts a class label for the given input vector, returning the
        /// probability that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="time">The event times.</param>
        /// 
        /// <returns>The probabilities of the event occurring at 
        /// the given times for the given observations.</returns>
        /// 
        public double Probability(double[] input, double time)
        {
            return Probability(Tuple.Create(input, time));
        }

        /// <summary>
        /// Predicts a class label for the given input vector, returning the
        /// probability that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="time">The event times.</param>
        /// 
        /// <returns>The probabilities of the event occurring at 
        /// the given times for the given observations.</returns>
        /// 
        public double[] Probability(double[][] input, double[] time)
        {
            var result = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = Probability(input[i], time[i]);
            return result;
        }


        /// <summary>
        /// Predicts a class label vector for the given input vectors, returning the
        /// log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="time">The event times.</param>
        /// 
        public double LogLikelihood(double[] input, double time)
        {
            return LogLikelihood(Tuple.Create(input, time));
        }

        /// <summary>
        /// Predicts a class label vector for the given input vectors, returning the
        /// log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="time">The event times.</param>
        /// 
        public double[] LogLikelihood(double[][] input, double[] time)
        {
            var result = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = LogLikelihood(input[i], time[i]);
            return result;
        }






        /// <summary>
        ///   Computes class-label decisions for each vector in the given <paramref name="input"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vectors that should be classified into
        ///   one of the <see cref="ITransform.NumberOfOutputs"/> possible classes.</param>
        /// 
        public SurvivalOutcome Decide(double[] input)
        {
            return Probability(input) > 0.5 ? SurvivalOutcome.Failed : SurvivalOutcome.Censored;
        }

        /// <summary>
        ///   Computes class-label decisions for each vector in the given <paramref name="input"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vectors that should be classified into
        ///   one of the <see cref="ITransform.NumberOfOutputs"/> possible classes.</param>
        /// 
        public SurvivalOutcome[] Decide(double[][] input)
        {
            var result = new SurvivalOutcome[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = Decide(input[i]);
            return result;
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        public double Score(double[] input)
        {
            return LogLikelihood(input) - Math.Log(0.5);
        }

        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        public double[] Score(double[][] input)
        {
            var result = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = Score(input[i]);
            return result;
        }


        /// <summary>
        /// Predicts a class label for the given input vector, returning the
        /// probability that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        public double Probability(double[] input)
        {
            return Math.Exp(LogLikelihood(input));
        }

        /// <summary>
        /// Predicts a class label for the given input vector, returning the
        /// probability that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        public double[] Probability(double[][] input)
        {
            var result = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = Probability(input[i]);
            return result;
        }


        /// <summary>
        /// Predicts a class label vector for the given input vectors, returning the
        /// log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        public double LogLikelihood(double[] input)
        {
#pragma warning disable 612, 618
            double sum = Intercept;
            for (int i = 0; i < Coefficients.Length; i++)
                sum += Coefficients[i] * (input[i] - Offsets[i]);
            return sum;
#pragma warning restore 612, 618
        }

        /// <summary>
        /// Predicts a class label vector for the given input vectors, returning the
        /// log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        public double[] LogLikelihood(double[][] input)
        {
            var result = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = LogLikelihood(input[i]);
            return result;
        }


        /// <summary>
        /// Predicts a class label for the given input vector, returning the
        /// probability that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="time">The event times.</param>
        /// 
        public double Probability(double time)
        {
            if (BaselineHazard == null)
                throw new InvalidOperationException();

            return BaselineHazard.CumulativeHazardFunction(time);
        }

        /// <summary>
        /// Predicts a class label for the given input vector, returning the
        /// log-likelihood that the input vector belongs to its predicted class.
        /// </summary>
        /// 
        /// <param name="time">The event times.</param>
        /// 
        public double LogLikelihood(double time)
        {
            if (BaselineHazard == null)
                throw new InvalidOperationException();

            return BaselineHazard.LogCumulativeHazardFunction(time);
        }

    }
}
