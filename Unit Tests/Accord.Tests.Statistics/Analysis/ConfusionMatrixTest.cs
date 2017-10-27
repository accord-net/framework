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
    using Accord.Statistics.Analysis;
    using NUnit.Framework;
    using Accord.Math;
    using Accord.Statistics.Testing;


    [TestFixture]
    public class ConfusionMatrixTest
    {


        [Test]
        public void ConfusionMatrixConstructorTest()
        {
            // The correct and expected output values (as confirmed by a Gold
            //  standard rule, actual experiment or true verification)
            int[] expected = { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0 };

            // The values as predicted by the decision system or
            //  the test whose performance is being measured.
            int[] predicted = { 0, 0, 0, 1, 1, 0, 0, 0, 0, 1 };


            // In this test, 1 means positive, 0 means negative
            int positiveValue = 1;
            int negativeValue = 0;

            // Create a new confusion matrix using the given parameters
            ConfusionMatrix matrix = new ConfusionMatrix(predicted, expected,
                positiveValue, negativeValue);

            // At this point,
            //   True Positives should be equal to 1;
            //   True Negatives should be equal to 6;
            //   False Negatives should be equal to 1;
            //   False Positives should be equal to 2.


            int falseNegatives = 1;
            int falsePositives = 2;
            int truePositives = 1;
            int trueNegatives = 6;

            Assert.AreEqual(predicted.Length, matrix.Samples);
            Assert.AreEqual(8, matrix.ActualNegatives);
            Assert.AreEqual(2, matrix.ActualPositives);
            Assert.AreEqual(7, matrix.PredictedNegatives);
            Assert.AreEqual(3, matrix.PredictedPositives);

            Assert.AreEqual(falseNegatives, matrix.FalseNegatives);
            Assert.AreEqual(falsePositives, matrix.FalsePositives);
            Assert.AreEqual(truePositives, matrix.TruePositives);
            Assert.AreEqual(trueNegatives, matrix.TrueNegatives);

            Assert.AreEqual(0.7, matrix.Accuracy);
            Assert.AreEqual(0.5, matrix.Sensitivity);
            Assert.AreEqual(0.75, matrix.Specificity);
            Assert.AreEqual((0.5 + 0.75) / 2.0, matrix.Efficiency);

            Assert.AreEqual(0.21821789023599239, matrix.MatthewsCorrelationCoefficient);
        }

        [Test]
        public void ConfusionMatrixConstructorTest2()
        {
            #region doc_ctor_values
            // Let's say we have a decision problem involving 2 classes. In a typical
            // machine learning problem, have a set of expected, ground truth values:
            //
            bool[] expected = { false, false, true, false, true, false, false, false, false, false };

            // And we have a set of values that have been predicted by a machine model,
            // a procedure or a laboratory test:
            //
            bool[] predicted = { false, false, false, true, true, false, false, false, false, true };

            // We can get different performance measures to assess how good our model was at 
            // predicting the true, expected, ground-truth labels for the decision problem:
            var cm = new ConfusionMatrix(predicted, expected);
            
            // Those are the individual values of the confusion matrix:
            int falseNegatives = cm.FalseNegatives; // should be 1
            int falsePositives = cm.FalsePositives; // should be 2
            int truePositives = cm.TruePositives;   // should be 1
            int trueNegatives = cm.TrueNegatives;   // should be 6

            // We can also get it in matrix form:
            int[,] matrix = cm.Matrix;

            // The values of this matrix should be the same as:
            int[,] expectedMatrix =
            {
                //              expected
                /*predicted*/ { 1, 1 },
                              { 2, 6 },
            };


            // We can get more information about our problem as well:
            int classes = cm.NumberOfClasses; // should be 2
            int samples = cm.NumberOfSamples; // should be 10

            // And multiple performance measures:
            double accuracy = cm.Accuracy;                      // should be 0.7
            double error = cm.Error;                            // should be 0.3
            double chanceAgreement = cm.ChanceAgreement;        // should be 0.62
            double geommetricAgreement = cm.GeometricAgreement; // should be 2.4494897427831779
            double pearson = cm.Pearson;                        // should be 0.21320071635561044
            double kappa = cm.Kappa;                            // should be 0.21052631578947359
            double chiSquare = cm.ChiSquare;                    // should be 0.47619047619047622

            // and some of their standard errors:
            double kappaStdErr = cm.StandardError;              // should be 0.32822854833459553
            double kappaStdErr0 = cm.StandardErrorUnderNull;    // should be 0.16666666666666663
            #endregion

            Assert.AreEqual(2, classes);
            Assert.AreEqual(10, samples);

            Assert.IsTrue(expectedMatrix.IsEqual(matrix));

            Assert.AreEqual(0.7, accuracy);
            Assert.AreEqual(0.3, error);
            Assert.AreEqual(0.62, chanceAgreement);
            Assert.AreEqual(2.4494897427831779, geommetricAgreement);
            Assert.AreEqual(0.21320071635561044, pearson);
            Assert.AreEqual(0.21052631578947359, kappa);
            Assert.AreEqual(0.47619047619047622, chiSquare);

            Assert.AreEqual(0.32822854833459553, kappaStdErr);
            Assert.AreEqual(0.30508161570925141, kappaStdErr0);

            Assert.AreEqual(predicted.Length, cm.Samples);
            Assert.AreEqual(8, cm.ActualNegatives);
            Assert.AreEqual(2, cm.ActualPositives);
            Assert.AreEqual(7, cm.PredictedNegatives);
            Assert.AreEqual(3, cm.PredictedPositives);

            Assert.AreEqual(1, cm.FalseNegatives);
            Assert.AreEqual(2, cm.FalsePositives);
            Assert.AreEqual(1, cm.TruePositives);
            Assert.AreEqual(6, cm.TrueNegatives);
        }

        [Test]
        public void ConfusionMatrixConstructorTest3()
        {
            // System output
            int[] predicted = new int[] { 2, 0, 1 };

            // Correct output
            int[] expected = new int[] { 5, 2, 1 };

            // 1 means positive (the others shall be treated as negatives)
            int positiveValue = 1;


            ConfusionMatrix target = new ConfusionMatrix(predicted, expected, positiveValue);


            int falseNegatives = 0;
            int falsePositives = 0;
            int truePositives = 1;
            int trueNegatives = 2;

            Assert.AreEqual(predicted.Length, target.Samples);
            Assert.AreEqual(2, target.ActualNegatives);
            Assert.AreEqual(1, target.ActualPositives);
            Assert.AreEqual(2, target.PredictedNegatives);
            Assert.AreEqual(1, target.PredictedPositives);

            Assert.AreEqual(falseNegatives, target.FalseNegatives);
            Assert.AreEqual(falsePositives, target.FalsePositives);
            Assert.AreEqual(truePositives, target.TruePositives);
            Assert.AreEqual(trueNegatives, target.TrueNegatives);

            Assert.AreEqual(1.0, target.Accuracy);
            Assert.AreEqual(1.0, target.Sensitivity);
            Assert.AreEqual(1.0, target.Specificity);
            Assert.AreEqual(1.0, target.Efficiency);

            // Perfect prediction
            Assert.AreEqual(1.0, target.MatthewsCorrelationCoefficient);
        }

        [Test]
        public void ConfusionMatrixConstructorTest4()
        {
            // Example from http://www.iph.ufrgs.br/corpodocente/marques/cd/rd/presabs.htm

            ConfusionMatrix matrix = new ConfusionMatrix
            (
                truePositives: 70,
                trueNegatives: 95,
                falsePositives: 5,
                falseNegatives: 30
            );

            Assert.AreEqual(70, matrix.TruePositives);
            Assert.AreEqual(5, matrix.FalsePositives);
            Assert.AreEqual(95, matrix.TrueNegatives);
            Assert.AreEqual(30, matrix.FalseNegatives);

            // Prevalence	    0.500	0.100	0.011
            // Overall Power	0.500	0.900	0.989
            // Sensitivity	    0.700	0.700	0.700
            // Specificity	    0.950	0.950	0.950
            // PPP	            0.933	0.610	0.130
            // NPP	            0.760	0.970	0.997
            // Misc. Rate	    0.175	0.075	0.053
            // Odds Ratio	    44.333	44.333	44.333
            // Kappa	        0.650	0.610	0.210
            // NMI	            0.371	0.360	0.264

            Assert.AreEqual(0.500, matrix.OverallDiagnosticPower, 1e-3);
            Assert.AreEqual(0.700, matrix.Sensitivity, 1e-3);
            Assert.AreEqual(0.950, matrix.Specificity, 1e-3);
            Assert.AreEqual(0.933, matrix.PositivePredictiveValue, 1e-3);
            Assert.AreEqual(0.760, matrix.NegativePredictiveValue, 1e-3);
            Assert.AreEqual(0.175, 1 - matrix.Accuracy, 1e-3);
            Assert.AreEqual(44.333, matrix.OddsRatio, 1e-3);
            Assert.AreEqual(0.650, matrix.Kappa, 1e-3);
            Assert.AreEqual(0.371, matrix.NormalizedMutualInformation, 1e-3);

            Assert.IsFalse(double.IsNaN(matrix.OverallDiagnosticPower));
            Assert.IsFalse(double.IsNaN(matrix.Sensitivity));
            Assert.IsFalse(double.IsNaN(matrix.Specificity));
            Assert.IsFalse(double.IsNaN(matrix.PositivePredictiveValue));
            Assert.IsFalse(double.IsNaN(matrix.NegativePredictiveValue));
            Assert.IsFalse(double.IsNaN(matrix.Accuracy));
            Assert.IsFalse(double.IsNaN(matrix.OddsRatio));
            Assert.IsFalse(double.IsNaN(matrix.Kappa));
            Assert.IsFalse(double.IsNaN(matrix.NormalizedMutualInformation));
        }

        [Test]
        public void ConfusionMatrixConstructorTest5()
        {
            // The correct and expected output values (as confirmed by a Gold
            //  standard rule, actual experiment or true verification)
            int[] expected = { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0 };

            // The values as predicted by the decision system or
            //  the test whose performance is being measured.
            int[] predicted = { 0, 0, 0, 1, 1, 0, 0, 0, 0, 1 };


            // Create a new confusion matrix using the given parameters
            ConfusionMatrix matrix = new ConfusionMatrix(predicted, expected);

            // At this point,
            //   True Positives should be equal to 1;
            //   True Negatives should be equal to 6;
            //   False Negatives should be equal to 1;
            //   False Positives should be equal to 2.


            int falseNegatives = 1;
            int falsePositives = 2;
            int truePositives = 1;
            int trueNegatives = 6;

            Assert.AreEqual(predicted.Length, matrix.Samples);
            Assert.AreEqual(8, matrix.ActualNegatives);
            Assert.AreEqual(2, matrix.ActualPositives);
            Assert.AreEqual(7, matrix.PredictedNegatives);
            Assert.AreEqual(3, matrix.PredictedPositives);

            Assert.AreEqual(falseNegatives, matrix.FalseNegatives);
            Assert.AreEqual(falsePositives, matrix.FalsePositives);
            Assert.AreEqual(truePositives, matrix.TruePositives);
            Assert.AreEqual(trueNegatives, matrix.TrueNegatives);

            Assert.AreEqual(0.7, matrix.Accuracy);
            Assert.AreEqual(0.5, matrix.Sensitivity);
            Assert.AreEqual(0.75, matrix.Specificity);
            Assert.AreEqual((0.5 + 0.75) / 2.0, matrix.Efficiency);

            Assert.AreEqual(0.21821789023599239, matrix.MatthewsCorrelationCoefficient);
        }

        [Test]
        public void ConfusionMatrixConstructorTest6()
        {
            // Create a new confusion matrix using the given parameters
            ConfusionMatrix matrix = new ConfusionMatrix(
                truePositives: 10, falsePositives: 40,
                falseNegatives: 5, trueNegatives: 45);

            Assert.AreEqual(5, matrix.FalseNegatives);
            Assert.AreEqual(40, matrix.FalsePositives);
            Assert.AreEqual(10, matrix.TruePositives);
            Assert.AreEqual(45, matrix.TrueNegatives);

            Assert.AreEqual(0.15, matrix.Prevalence);
            Assert.AreEqual(0.67, matrix.Sensitivity, 1e-2);
            Assert.AreEqual(0.53, matrix.Specificity, 1e-2);
            Assert.AreEqual(0.20, matrix.PositivePredictiveValue);
            Assert.AreEqual(0.90, matrix.NegativePredictiveValue);

        }

        [Test]
        public void ChiSquareTest()
        {
            ConfusionMatrix target = new ConfusionMatrix(6, 2, 6, 18);
            double[,] expected = target.ExpectedValues;

            Assert.AreEqual(3, target.ExpectedValues[0, 0]);
            Assert.AreEqual(5, target.ExpectedValues[0, 1]);
            Assert.AreEqual(9, target.ExpectedValues[1, 0]);
            Assert.AreEqual(15, target.ExpectedValues[1, 1]);

            Assert.AreEqual(6.4, target.ChiSquare, 1e-5);

            ChiSquareTest test = new ChiSquareTest(target);
            Assert.AreEqual(target.ChiSquare, test.Statistic);
            Assert.AreEqual(1, test.DegreesOfFreedom);
            Assert.IsTrue(test.Significant);
        }

        [Test]
        public void MatthewsCorrelationCoefficientTest()
        {
            ConfusionMatrix matrix = new ConfusionMatrix(1, 1, 2, 6);
            Assert.AreEqual(0.21821789023599239, matrix.MatthewsCorrelationCoefficient);
        }

        [Test]
        public void MatthewsCorrelationCoefficientTest2()
        {
            ConfusionMatrix matrix = new ConfusionMatrix(100, 100, 200, 600);
            Assert.AreEqual(0.21821789023599236, matrix.MatthewsCorrelationCoefficient);
        }

        [Test]
        public void ToGeneralMatrixTest1()
        {
            // Example from http://www.iph.ufrgs.br/corpodocente/marques/cd/rd/presabs.htm

            ConfusionMatrix matrix = new ConfusionMatrix
            (
                truePositives: 70,
                trueNegatives: 95,
                falsePositives: 5,
                falseNegatives: 30
            );

            Assert.AreEqual(70, matrix.TruePositives);
            Assert.AreEqual(5, matrix.FalsePositives);
            Assert.AreEqual(95, matrix.TrueNegatives);
            Assert.AreEqual(30, matrix.FalseNegatives);

            GeneralConfusionMatrix general = matrix.ToGeneralMatrix();

            Assert.AreEqual(matrix.Kappa, general.Kappa, 1e-10);
            Assert.AreEqual(matrix.Accuracy, general.OverallAgreement, 1e-10);
            Assert.AreEqual(matrix.StandardError, general.StandardError, 1e-10);
            Assert.AreEqual(matrix.StandardErrorUnderNull, general.StandardErrorUnderNull, 1e-10);
            Assert.AreEqual(matrix.Variance, general.Variance, 1e-10);
            Assert.AreEqual(matrix.VarianceUnderNull, general.VarianceUnderNull, 1e-10);
            Assert.AreEqual(matrix.Samples, general.Samples);

            Assert.IsFalse(double.IsNaN(general.Kappa));
            Assert.IsFalse(double.IsNaN(general.OverallAgreement));
            Assert.IsFalse(double.IsNaN(general.StandardError));
            Assert.IsFalse(double.IsNaN(general.StandardErrorUnderNull));
            Assert.IsFalse(double.IsNaN(general.Variance));
            Assert.IsFalse(double.IsNaN(general.VarianceUnderNull));
        }


    }
}
