// Accord Unit Tests
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

namespace Accord.Tests.Statistics
{
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Testing;
    using NUnit.Framework;

    [TestFixture]
    public class ProportionalHazardsAnalysisTest
    {

        [Test]
        public void ComputeTest1()
        {
            // Consider the following example data, adapted from John C. Pezzullo's
            // example for his great Cox's proportional hazards model example in
            // JavaScript (http://statpages.org/prophaz2.html). 

            // In this data, we have three columns. The first column denotes the
            // input variables for the problem. The second column, the survival
            // times. And the last one is the output of the experiment (if the
            // subject has died [1] or has survived [0]).

            double[,] example =
            {
                // input  time censor
                {   50,    1,    0   },
                {   70,    2,    1   },
                {   45,    3,    0   },
                {   35,    5,    0   },
                {   62,    7,    1   },
                {   50,   11,    0   },
                {   45,    4,    0   },
                {   57,    6,    0   },
                {   32,    8,    0   },
                {   57,    9,    1   },
                {   60,   10,    1   },
            };

            // First we will extract the input, times and outputs
            double[,] inputs = example.GetColumns(new[] { 0 });
            double[] times = example.GetColumn(1);
            SurvivalOutcome[] output = example.GetColumn(2).To<SurvivalOutcome[]>();

            // Now we can proceed and create the analysis
            var cox = new ProportionalHazardsAnalysis(inputs, times, output);

            cox.Compute(); // compute the analysis

            // Now we can show an analysis summary
            // Accord.Controls.DataGridBox.Show(cox.Coefficients);


            // We can also investigate all parameters individually. For
            // example the coefficients values will be available at

            double[] coef = cox.CoefficientValues;
            double[] stde = cox.StandardErrors;

            // We can also obtain the hazards ratios
            double[] ratios = cox.HazardRatios;

            // And other information such as the partial
            // likelihood, the deviance and also make 
            // hypothesis tests on the parameters

            double partial = cox.LogLikelihood;
            double deviance = cox.Deviance;

            // Chi-Square for whole model
            ChiSquareTest chi = cox.ChiSquare;

            // Wald tests for individual parameters
            WaldTest wald = cox.Coefficients[0].Wald;


            // Finally, we can also use the model to predict
            // scores for new observations (without considering time)

            double y1 = cox.Regression.Compute(new double[] { 63 });
            double y2 = cox.Regression.Compute(new double[] { 32 });

            // Those scores can be interpreted by comparing then
            // to 1. If they are greater than one, the odds are
            // the patient will not survive. If the value is less
            // than one, the patient is likely to survive.

            // The first value, y1, gives approximately 86.138,
            // while the second value, y2, gives about 0.00072.


            // We can also consider instant estimates for a given time:
            double p1 = cox.Regression.Compute(new double[] { 63 }, 2);
            double p2 = cox.Regression.Compute(new double[] { 63 }, 10);

            // Here, p1 is the score after 2 time instants, with a 
            // value of 0.0656. The second value, p2, is the time
            // after 10 time instants, with a value of 6.2907.


            Assert.AreEqual(86.138421225296526, y1);
            Assert.AreEqual(0.00072281400325299814, y2, 1e-10);

            Assert.AreEqual(0.17989138010770425, p1, 1e-10);
            Assert.AreEqual(15.950244161356357, p2, 1e-10);

            Assert.AreEqual(1, coef.Length);
            Assert.AreEqual(0.37704239281490765, coef[0]);
            Assert.AreEqual(0.25415746361167235, stde[0]);
            Assert.AreEqual(1.4579661153488215, ratios[0]);

            Assert.AreEqual(-2.0252666205735466, partial, 1e-6);
            Assert.AreEqual(4.0505332411470931, deviance, 1e-6);

            Assert.AreEqual(0.13794183001851756, wald.PValue, 1e-4);

            Assert.AreEqual(1, chi.DegreesOfFreedom);
            Assert.AreEqual(7.3570, chi.Statistic, 1e-4);
            Assert.AreEqual(0.0067, chi.PValue, 1e-3);
        }

        [Test]
        public void learn_test()
        {
            #region doc_learn_part1
            // Consider the following example data, adapted from John C. Pezzullo's
            // example for his great Cox's proportional hazards model example in
            // JavaScript (http://statpages.org/prophaz2.html). 

            // In this data, we have three columns. The first column denotes the
            // input variables for the problem. The second column, the survival
            // times. And the last one is the output of the experiment (if the
            // subject has died [1] or has survived [0]).

            double[][] example =
            {
                //             input  time censor
                new double[] {   50,    1,    0   },
                new double[] {   70,    2,    1   },
                new double[] {   45,    3,    0   },
                new double[] {   35,    5,    0   },
                new double[] {   62,    7,    1   },
                new double[] {   50,   11,    0   },
                new double[] {   45,    4,    0   },
                new double[] {   57,    6,    0   },
                new double[] {   32,    8,    0   },
                new double[] {   57,    9,    1   },
                new double[] {   60,   10,    1   },
            };

            // First we will extract the input, times and outputs
            double[][] inputs = example.Get(null, 0, 1);
            double[] times = example.GetColumn(1);
            SurvivalOutcome[] output = example.GetColumn(2).To<SurvivalOutcome[]>();

            // Now we can proceed and create the analysis (giving optional variable names)
            var cox = new ProportionalHazardsAnalysis(new[] { "input" }, "time", "censor");

            // Then compute the analysis, learning a regression in the process:
            ProportionalHazards regression = cox.Learn(inputs, times, output);

            // Now we can show an analysis summary
            // Accord.Controls.DataGridBox.Show(cox.Coefficients);
            #endregion

            #region doc_learn_part2

            // We can also investigate all parameters individually. For
            // example the coefficients values will be available at

            double[] coef = cox.CoefficientValues;     // should be { 0.37704239281490765 }
            double[] stde = cox.StandardErrors;        // should be { 0.25415746361167235 }

            // We can also obtain the hazards ratios
            double[] ratios = cox.HazardRatios;        // should be { 1.4579661153488215 }

            // And other information such as the partial
            // likelihood, the deviance and also make 
            // hypothesis tests on the parameters

            double partialL = cox.LogLikelihood;       // should be -2.0252666205735466
            double deviance = cox.Deviance;            // should be 4.0505332411470931
                                                       
            // Chi-Square for whole model              
            ChiSquareTest chi = cox.ChiSquare;         // should be 7.3570 (p=0.0067)

            // Wald tests for individual parameters
            WaldTest wald = cox.Coefficients[0].Wald;  // should be 1.4834 (p=0.1379)


            // Finally, we can also use the model to predict
            // scores for new observations (without considering time)

            double y1 = cox.Regression.Probability(new double[] { 63 }); // should be 86.138421225296526
            double y2 = cox.Regression.Probability(new double[] { 32 }); // should be 0.00072281400325299814

            // Those scores can be interpreted by comparing then
            // to 1. If they are greater than one, the odds are
            // the patient will not survive. If the value is less
            // than one, the patient is likely to survive.

            // The first value, y1, gives approximately 86.138,
            // while the second value, y2, gives about 0.00072.


            // We can also consider instant estimates for a given time:
            double p1 = cox.Regression.Probability(new double[] { 63 }, 2);   // should be 0.17989138010770425
            double p2 = cox.Regression.Probability(new double[] { 63 }, 10);  // should be 15.950244161356357

            // Here, p1 is the score after 2 time instants, with a 
            // value of 0.0656. The second value, p2, is the time
            // after 10 time instants, with a value of 6.2907.

            // In addition, if we would like a higher precision when 
            // computing very small probabilities using the methods 
            // above, we can use the LogLikelihood methods instead:

            double log_y1 = cox.Regression.LogLikelihood(new double[] { 63 });      // should be  4.4559555514489091
            double log_y2 = cox.Regression.LogLikelihood(new double[] { 32 });      // should be -7.2323586258132284
            double log_p1 = cox.Regression.LogLikelihood(new double[] { 63 }, 2);   // should be -1.7154020540835324
            double log_p2 = cox.Regression.LogLikelihood(new double[] { 63 }, 10);  // should be  2.7694741370357177
            #endregion

            Assert.AreEqual(86.138421225296526, y1, 1e-10);
            Assert.AreEqual(0.00072281400325299814, y2, 1e-10);

            Assert.AreEqual(0.17989138010770425, p1, 1e-10);
            Assert.AreEqual(15.950244161356357, p2, 1e-10);

            Assert.AreEqual(4.4559555514489091, log_y1, 1e-10);
            Assert.AreEqual(-7.2323586258132284, log_y2, 1e-10);

            Assert.AreEqual(-1.7154020540835324, log_p1, 1e-10);
            Assert.AreEqual(2.7694741370357177, log_p2, 1e-10);

            Assert.AreEqual(System.Math.Log(y1), log_y1, 1e-10);
            Assert.AreEqual(System.Math.Log(y2), log_y2, 1e-10);

            Assert.AreEqual(System.Math.Log(p1), log_p1, 1e-10);
            Assert.AreEqual(System.Math.Log(p2), log_p2, 1e-10);

            Assert.AreEqual(1, coef.Length);
            Assert.AreEqual(0.37704239281490765, coef[0]);
            Assert.AreEqual(0.25415746361167235, stde[0]);
            Assert.AreEqual(1.4579661153488215, ratios[0]);

            Assert.AreEqual(-2.0252666205735466, partialL, 1e-6);
            Assert.AreEqual(4.0505332411470931, deviance, 1e-6);

            Assert.AreEqual(1.4834991955655938, wald.Statistic, 1e-4);
            Assert.AreEqual(0.13794183001851756, wald.PValue, 1e-4);

            Assert.AreEqual(1, chi.DegreesOfFreedom);
            Assert.AreEqual(7.3570, chi.Statistic, 1e-4); 
            Assert.AreEqual(0.0067, chi.PValue, 1e-3);
        }


        [Test]
        public void ComputeTest1_Compat()
        {
            // Consider the following example data, adapted from John C. Pezzullo's
            // example for his great Cox's proportional hazards model example in
            // JavaScript (http://statpages.org/prophaz2.html). 

            // In this data, we have three columns. The first column denotes the
            // input variables for the problem. The second column, the survival
            // times. And the last one is the output of the experiment (if the
            // subject has died [1] or has survived [0]).

            double[,] example =
            {
                // input  time censor
                {   50,    1,    0   },
                {   70,    2,    1   },
                {   45,    3,    0   },
                {   35,    5,    0   },
                {   62,    7,    1   },
                {   50,   11,    0   },
                {   45,    4,    0   },
                {   57,    6,    0   },
                {   32,    8,    0   },
                {   57,    9,    1   },
                {   60,   10,    1   },
            };

            // First we will extract the input, times and outputs
            double[,] inputs = example.GetColumns(new[] { 0 });
            double[] times = example.GetColumn(1);
            int[] output = example.GetColumn(2).To<int[]>();

            // Now we can proceed and create the analysis
            var cox = new ProportionalHazardsAnalysis(inputs, times, output);

            cox.Compute(); // compute the analysis

            // Now we can show an analysis summary
            // Accord.Controls.DataGridBox.Show(cox.Coefficients);


            // We can also investigate all parameters individually. For
            // example the coefficients values will be available at

            double[] coef = cox.CoefficientValues;
            double[] stde = cox.StandardErrors;

            // We can also obtain the hazards ratios
            double[] ratios = cox.HazardRatios;

            // And other information such as the partial
            // likelihood, the deviance and also make 
            // hypothesis tests on the parameters

            double partial = cox.LogLikelihood;
            double deviance = cox.Deviance;

            // Chi-Square for whole model
            ChiSquareTest chi = cox.ChiSquare;

            // Wald tests for individual parameters
            WaldTest wald = cox.Coefficients[0].Wald;


            // Finally, we can also use the model to predict
            // scores for new observations (without considering time)

            double y1 = cox.Regression.Compute(new double[] { 63 });
            double y2 = cox.Regression.Compute(new double[] { 32 });

            // Those scores can be interpreted by comparing then
            // to 1. If they are greater than one, the odds are
            // the patient will not survive. If the value is less
            // than one, the patient is likely to survive.

            // The first value, y1, gives approximately 86.138,
            // while the second value, y2, gives about 0.00072.


            // We can also consider instant estimates for a given time:
            double p1 = cox.Regression.Compute(new double[] { 63 }, 2);
            double p2 = cox.Regression.Compute(new double[] { 63 }, 10);

            // Here, p1 is the score after 2 time instants, with a 
            // value of 0.0656. The second value, p2, is the time
            // after 10 time instants, with a value of 6.2907.


            Assert.AreEqual(86.138421225296526, y1);
            Assert.AreEqual(0.00072281400325299814, y2, 1e-10);

            Assert.AreEqual(0.17989138010770425, p1, 1e-10);
            Assert.AreEqual(15.950244161356357, p2, 1e-10);

            Assert.AreEqual(1, coef.Length);
            Assert.AreEqual(0.37704239281490765, coef[0]);
            Assert.AreEqual(0.25415746361167235, stde[0]);
            Assert.AreEqual(1.4579661153488215, ratios[0]);

            Assert.AreEqual(-2.0252666205735466, partial, 1e-6);
            Assert.AreEqual(4.0505332411470931, deviance, 1e-6);

            Assert.AreEqual(0.13794183001851756, wald.PValue, 1e-4);

            Assert.AreEqual(1, chi.DegreesOfFreedom);
            Assert.AreEqual(7.3570, chi.Statistic, 1e-4);
            Assert.AreEqual(0.0067, chi.PValue, 1e-3);

        }

    }
}
