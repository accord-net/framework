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
    using System;
    using Accord.Statistics.Filters;
    using System.Data;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using Accord.IO;

    [TestFixture]
    public class GeneralConfusionMatrixTest
    {


        [Test]
        public void GeneralConfusionMatrixConstructorTest()
        {
            #region doc_ctor_values
            // Let's say we have a decision problem involving 3 classes. In a typical
            // machine learning problem, have a set of expected, ground truth values:
            //
            int[] expected = { 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2 };

            // And we have a set of values that have been predicted by a machine model:
            //
            int[] predicted = { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1 };

            // We can get different performance measures to assess how good our model was at 
            // predicting the true, expected, ground-truth labels for the decision problem:
            var cm = new GeneralConfusionMatrix(classes: 3, expected: expected, predicted: predicted);

            // We can obtain the proper confusion matrix using:
            int[,] matrix = cm.Matrix;

            // The values of this matrix should be the same as:
            int[,] expectedMatrix =
            {
                //              expected
                /*predicted*/ { 4, 0, 0 },
                              { 0, 4, 4 },
                              { 0, 0, 0 },
            };


            // We can get more information about our problem as well:
            int classes = cm.NumberOfClasses; // should be 3
            int samples = cm.NumberOfSamples; // should be 12

            // And multiple performance measures:
            double accuracy = cm.Accuracy;                      // should be 0.66666666666666663
            double error = cm.Error;                            // should be 0.33333333333333337
            double chanceAgreement = cm.ChanceAgreement;        // should be 0.33333333333333331
            double geommetricAgreement = cm.GeometricAgreement; // should be 0 (the classifier completely missed one class)
            double pearson = cm.Pearson;                        // should be 0.70710678118654757
            double kappa = cm.Kappa;                            // should be 0.49999999999999994
            double tau = cm.Tau;                                // should be 0.49999999999999994
            double chiSquare = cm.ChiSquare;                    // should be 12

            // and some of their standard errors:
            double kappaStdErr = cm.StandardError;              // should be 0.15590239111558091
            double kappaStdErr0 = cm.StandardErrorUnderNull;    // should be 0.16666666666666663
            #endregion

            Assert.AreEqual(3, classes);
            Assert.AreEqual(12, samples);

            Assert.IsTrue(expectedMatrix.IsEqual(matrix));

            Assert.AreEqual(0.66666666666666663, accuracy);
            Assert.AreEqual(0.33333333333333337, error);
            Assert.AreEqual(0.33333333333333331, chanceAgreement);
            Assert.AreEqual(0.70710678118654757, pearson);
            Assert.AreEqual(0.49999999999999994, kappa);
            Assert.AreEqual(0.49999999999999994, tau);
            Assert.AreEqual(12, chiSquare);

            Assert.AreEqual(0.15590239111558091, kappaStdErr);
            Assert.AreEqual(0.16666666666666663, kappaStdErr0);
        }

        [Test]
        public void class_confusion_matrices()
        {
            int[] expected = { 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2 };
            int[] predicted = { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1 };

            var target = new GeneralConfusionMatrix(expected, predicted);

            Assert.AreEqual(3, target.Classes);
            Assert.AreEqual(12, target.Samples);

            ConfusionMatrix actual1 = target.PerClassMatrices[0];
            ConfusionMatrix actual2 = target.PerClassMatrices[1];
            ConfusionMatrix actual3 = target.PerClassMatrices[2];

            ConfusionMatrix expected1 = new ConfusionMatrix(predicted, expected, positiveValue: 0);
            ConfusionMatrix expected2 = new ConfusionMatrix(predicted, expected, positiveValue: 1);
            ConfusionMatrix expected3 = new ConfusionMatrix(predicted, expected, positiveValue: 2);

            Assert.IsTrue(actual1.Matrix.IsEqual(expected1.Matrix));
            Assert.IsTrue(actual2.Matrix.IsEqual(expected2.Matrix));
            Assert.IsTrue(actual3.Matrix.IsEqual(expected3.Matrix));
        }

        [Test]
        public void class_confusion_matrices_larger()
        {
            int[] expected = { 0, 0, 0, 1, 1, 1, 1, 1, 2, 4, 4, 3, 2, 2 };
            int[] predicted = { 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1 };

            var target = new GeneralConfusionMatrix(expected, predicted);

            Assert.AreEqual(5, target.Classes);
            Assert.AreEqual(14, target.Samples);

            ConfusionMatrix actual1 = target.PerClassMatrices[0];
            ConfusionMatrix actual2 = target.PerClassMatrices[1];
            ConfusionMatrix actual3 = target.PerClassMatrices[2];
            ConfusionMatrix actual4 = target.PerClassMatrices[3];

            ConfusionMatrix expected1 = new ConfusionMatrix(predicted, expected, positiveValue: 0);
            ConfusionMatrix expected2 = new ConfusionMatrix(predicted, expected, positiveValue: 1);
            ConfusionMatrix expected3 = new ConfusionMatrix(predicted, expected, positiveValue: 2);
            ConfusionMatrix expected4 = new ConfusionMatrix(predicted, expected, positiveValue: 3);

            Assert.IsTrue(actual1.Matrix.IsEqual(expected1.Matrix));
            Assert.IsTrue(actual2.Matrix.IsEqual(expected2.Matrix));
            Assert.IsTrue(actual3.Matrix.IsEqual(expected3.Matrix));
            Assert.IsTrue(actual4.Matrix.IsEqual(expected4.Matrix));
        }

        [Test]
        public void gh669_predicted_label_not_in_expected_set()
        {
            // Example for https://github.com/accord-net/framework/issues/669
            string[] expectedLabels = { "A", "A", "B", "C", "A", "B", "B", "B" };
            string[] predictedLabels = { "A", "B", "C", "C", "A", "C", "B", "F" };

            // Create a codification object to translate char into symbols
            var codification = new Codification("Labels", expectedLabels.Concatenate(predictedLabels));
            int[] expected = codification.Transform(expectedLabels);   // ground truth data
            int[] predicted = codification.Transform(predictedLabels); // predicted from OCR

            // Create a new confusion matrix for multi-class problems
            var cm = new GeneralConfusionMatrix(expected, predicted);

            Assert.AreEqual(4, cm.Classes);
            Assert.AreEqual(8, cm.Samples);
            Assert.AreEqual(0.5, cm.Accuracy);
            Assert.AreEqual(4, cm.PerClassMatrices.Length);
        }


        [Test]
        public void GeneralConfusionMatrixConstructorTest2()
        {
            int[,] matrix =
            {
                { 4, 0, 0 },
                { 0, 4, 4 },
                { 0, 0, 0 },
            };

            var target = new GeneralConfusionMatrix(matrix);

            Assert.AreEqual(3, target.Classes);
            Assert.AreEqual(12, target.Samples);
            Assert.AreEqual(matrix, target.Matrix);
            Assert.AreEqual(0, target.GeometricAgreement);
        }

        [Test]
        public void KappaTest()
        {
            #region doc_ctor_matrix
            // Let's say we have the following matrix
            int[,] matrix =
            {
                { 29,  6,  5 },
                {  8, 20,  7 },
                {  1,  2, 22 },
            };

            // Create a new multi-class Confusion Matrix
            var cm = new GeneralConfusionMatrix(matrix);

            // Now we can use it to obtain info such as
            int classes = cm.NumberOfClasses; // should be 3
            int samples = cm.NumberOfSamples; // should be 100

            double acc = cm.Accuracy;          // should be 0.71
            double err = cm.Error;             // should be 0.29
            double ca = cm.ChanceAgreement;    // should be 0.335
            double ga = cm.GeometricAgreement; // should be 23.37
            double kappa = cm.Kappa;           // should be 0.563
            #endregion

            Assert.AreEqual(3, classes);
            Assert.AreEqual(100, samples);

            Assert.AreEqual(0.563, kappa, 1e-3);
            Assert.AreEqual(23.367749664961245, ga);

            Assert.AreEqual(0.71, acc, 1e-8);
            Assert.AreEqual(0.29, err, 1e-8);
            Assert.AreEqual(0.335, ca, 1e-8);
        }

        [Test]
        public void KappaTest2()
        {
            int[,] matrix =
            {
                { 24, 14 },
                {  8, 24 },
            };

            var target = new GeneralConfusionMatrix(matrix);


            Assert.AreEqual(2, target.Classes);
            Assert.AreEqual(70, target.Samples);

            double[,] p =
            {
                { 0.343, 0.200 },
                { 0.114, 0.343 }
            };

            Assert.IsTrue(p.IsEqual(target.ProportionMatrix, 1e-3));

            Assert.AreEqual(0.6857143, target.OverallAgreement, 1e-5);
            Assert.AreEqual(0.4963265, target.ChanceAgreement, 1e-5);
            Assert.AreEqual(0.376013, target.Kappa, 1e-5);
            Assert.AreEqual(0.1087717, target.StandardError, 1e-5);
            Assert.AreEqual(24, target.GeometricAgreement, 1e-5);
        }

        [Test]
        public void KappaTest4()
        {
            #region doc_congalton
            // Example from R. Congalton's book, "Assesssing 
            // the accuracy of remotely sensed data", 1999.

            int[,] table = // Analyst #1 (page 108)
            {
                { 65,  4, 22, 24 },
                {  6, 81,  5,  8 },
                {  0, 11, 85, 19 },
                {  4,  7,  3, 90 },
            };

            // Create a new multi-class confusion matrix
            var cm = new GeneralConfusionMatrix(table);

            int[] rowTotals = cm.RowTotals;     // should be { 115, 100, 115, 104 }
            int[] colTotals = cm.ColumnTotals;  // should be { 115, 100, 115, 104 }

            double kappa = cm.Kappa;            // should be 0.65
            double var = cm.Variance;           // should be 0.00076995084473426684
            double var0 = cm.VarianceUnderNull; // should be 0.00074886435981842887
            double varD = Accord.Statistics.Testing.KappaTest.DeltaMethodKappaVariance(cm); // should be 0.0007778
            #endregion

            int[] expectedRowTotals = new[] { 115, 100, 115, 104 };
            int[] expectedColTotals = new[] { 75, 103, 115, 141 };

            Assert.AreEqual(rowTotals, expectedRowTotals);
            Assert.AreEqual(colTotals, expectedColTotals);


            Assert.AreEqual(0.65, cm.Kappa, 1e-2);
            Assert.IsFalse(Double.IsNaN(cm.Kappa));

            Assert.AreEqual(0.0007778, varD, 1e-7);
            Assert.AreEqual(0.00076995084473426684, var, 1e-10);
            Assert.AreEqual(0.00074886435981842887, var0, 1e-10);

            Assert.IsFalse(double.IsNaN(var));
            Assert.IsFalse(double.IsNaN(var0));
            Assert.IsFalse(double.IsNaN(varD));
        }

        [Test]
        public void KappaTest5()
        {
            // Example from University of York Department of Health Sciences,
            // Measurement in Health and Disease, Cohen's Kappa
            // http://www-users.york.ac.uk/~mb55/msc/clinimet/week4/kappash2.pdf

            // warning: the paper seems to use an outdated variance formula for kappa.


            int[,] matrix =
            {
                { 61,  2 },
                {  6, 25 },
            };

            GeneralConfusionMatrix target = new GeneralConfusionMatrix(matrix);


            Assert.AreEqual(2, target.Classes);
            Assert.AreEqual(94, target.Samples);

            Assert.AreEqual(0.801, target.Kappa, 1e-4);
            Assert.AreEqual(0.067, target.StandardError, 1e-3);
        }


        [Test]
        public void KappaVarianceTest1()
        {
            {
                #region doc_ientilucci
                // Example from Ientilucci, Emmett (2006). "On Using and Computing the Kappa Statistic".
                // Available on: http://www.cis.rit.edu/~ejipci/Reports/On_Using_and_Computing_the_Kappa_Statistic.pdf 

                // Note: Congalton's method uses the Delta Method for approximating the Kappa variance,
                // but the framework uses the corrected methods by Cohen and Fleiss. For more information, see:
                // https://stats.stackexchange.com/questions/30604/computing-cohens-kappa-variance-and-standard-errors

                int[,] matrix = // Matrix A (page 1)
                {
                    { 317,  23,  0,  0 },
                    {  61, 120,  0,  0 },
                    {   2,   4, 60,  0 },
                    {  35,  29,  0,  8 },
                };

                // Create the multi-class confusion matrix
                var a = new GeneralConfusionMatrix(matrix);

                // Method A row totals (page 2)
                int[] rowTotals = a.RowTotals;       // should be { 340, 181, 66, 72 }

                // Method A col totals (page 2)
                int[] colTotals = a.ColumnTotals;    // should be { 415, 176, 60, 8 }

                // Number of samples for A (page 2)
                int samples = a.NumberOfSamples;     // should be 659
                int classes = a.NumberOfClasses;     // should be 4

                // Po for A (page 2)
                double po = a.OverallAgreement;      // should be 0.7663
                
                // Pc for A (page 2)
                double ca = a.ChanceAgreement;       // should be 0.4087

                // Kappa value k_hat for A (page 3)
                double kappa = a.Kappa;              // should be 0.605

                // Variance value var_k for A (page 4)
                double varD = a.VarianceDeltaMethod; // should be 0.00073735

                // Other variance values according to Fleiss and Cohen
                double var = a.Variance;             // should be 0.00071760415564207924
                double var0 = a.VarianceUnderNull;   // should be 0.00070251065008366978
                #endregion

                Assert.AreEqual(340, a.RowTotals[0]);
                Assert.AreEqual(181, a.RowTotals[1]);
                Assert.AreEqual(66, a.RowTotals[2]);
                Assert.AreEqual(72, a.RowTotals[3]);

                // Method A col totals (page 2)
                Assert.AreEqual(415, a.ColumnTotals[0]);
                Assert.AreEqual(176, a.ColumnTotals[1]);
                Assert.AreEqual(60, a.ColumnTotals[2]);
                Assert.AreEqual(8, a.ColumnTotals[3]);

                // Number of samples for A (page 2)
                Assert.AreEqual(659, a.Samples);
                Assert.AreEqual(4, a.Classes);

                // Po for A (page 2)
                Assert.AreEqual(0.7663, po, 1e-4);
                Assert.IsFalse(double.IsNaN(po));

                // Pc for A (page 3)
                Assert.AreEqual(0.4087, ca, 1e-5);
                Assert.IsFalse(double.IsNaN(ca));

                // Kappa value k_hat for A (page 3)
                Assert.AreEqual(0.605, kappa, 1e-3);
                Assert.IsFalse(double.IsNaN(kappa));

                // Variance value var_k for A (page 4)
                Assert.AreEqual(0.00073735, varD, 1e-8);

                Assert.AreEqual(0.00071760415564207924, var, 1e-10);
                Assert.AreEqual(0.00070251065008366978, var0, 1e-10);

                Assert.IsFalse(double.IsNaN(var));
                Assert.IsFalse(double.IsNaN(var0));
                Assert.IsFalse(double.IsNaN(varD));
            }

            {
                int[,] matrix = // Matrix B
                {
                    { 377,  79,  0,  0 },
                    {   2,  72,  0,  0 },
                    {  33,   5, 60,  0 },
                    {   3,  20,  0,  8 },
                };

                GeneralConfusionMatrix b = new GeneralConfusionMatrix(matrix);

                // Method B row totals (page 2)
                Assert.AreEqual(456, b.RowTotals[0]);
                Assert.AreEqual(74, b.RowTotals[1]);
                Assert.AreEqual(98, b.RowTotals[2]);
                Assert.AreEqual(31, b.RowTotals[3]);

                // Method B col totals (page 2)
                Assert.AreEqual(415, b.ColumnTotals[0]);
                Assert.AreEqual(176, b.ColumnTotals[1]);
                Assert.AreEqual(60, b.ColumnTotals[2]);
                Assert.AreEqual(8, b.ColumnTotals[3]);


                // Number of samples for B (page 2)
                Assert.AreEqual(659, b.Samples);
                Assert.AreEqual(4, b.Classes);

                // Po for B (page 2)
                Assert.AreEqual(0.7845, b.OverallAgreement, 1e-4);
                Assert.IsFalse(double.IsNaN(b.OverallAgreement));

                // Pc for B (page 3)
                Assert.AreEqual(0.47986, b.ChanceAgreement, 1e-5);
                Assert.IsFalse(double.IsNaN(b.ChanceAgreement));


                // Kappa value k_hat for B (page 3)
                Assert.AreEqual(0.586, b.Kappa, 1e-3);
                Assert.IsFalse(double.IsNaN(b.Kappa));


                double var = b.Variance;
                double var0 = b.VarianceUnderNull;
                double varD = Accord.Statistics.Testing.KappaTest.DeltaMethodKappaVariance(b);

                // Variance value var_k for A (page 4)
                Assert.AreEqual(0.00087457, varD, 1e-8);


                Assert.AreEqual(0.00083016849579382347, var, 1e-10);
                Assert.AreEqual(0.00067037111046188824, var0, 1e-10);

                Assert.IsFalse(double.IsNaN(var));
                Assert.IsFalse(double.IsNaN(var0));
                Assert.IsFalse(double.IsNaN(varD));
            }
        }

        [Test]
        public void KappaVarianceTest2()
        {
            // Example from http://vassarstats.net/kappa.html

            // Checked against http://graphpad.com/quickcalcs/Kappa2.cfm     (OK)

            int[,] matrix =
            {
                { 44,  5,  1 },
                {  7, 20,  3 },
                {  9,  5,  6 },
            };

            GeneralConfusionMatrix a = new GeneralConfusionMatrix(matrix);

            Assert.AreEqual(a.RowTotals[0], 50);
            Assert.AreEqual(a.RowTotals[1], 30);
            Assert.AreEqual(a.RowTotals[2], 20);

            Assert.AreEqual(a.ColumnTotals[0], 60);
            Assert.AreEqual(a.ColumnTotals[1], 30);
            Assert.AreEqual(a.ColumnTotals[2], 10);


            Assert.AreEqual(0.4915, a.Kappa, 1e-4);
            Assert.IsFalse(double.IsNaN(a.Kappa));

            double var = a.Variance;
            double var0 = a.VarianceUnderNull;
            double varD = Accord.Statistics.Testing.KappaTest.DeltaMethodKappaVariance(a);

            double se = System.Math.Sqrt(var);
            double se0 = System.Math.Sqrt(var0);
            double seD = System.Math.Sqrt(varD);

            Assert.AreEqual(0.072, a.StandardError, 0.0005);

            Assert.AreEqual(0.073534791185213152, seD, 1e-10);
            Assert.AreEqual(0.073509316753225237, se0, 1e-10);

            Assert.IsFalse(double.IsNaN(se));
            Assert.IsFalse(double.IsNaN(se0));
            Assert.IsFalse(double.IsNaN(seD));
        }

        [Test]
        public void KappaVarianceTest3()
        {
            #region doc_fleiss
            // Example from J. L. Fleiss, J. Cohen, B. S. Everitt, "Large sample
            //  standard errors of kappa and weighted kappa" Psychological Bulletin (1969)
            //  Volume: 72, Issue: 5, American Psychological Association, Pages: 323-327

            // This was the paper which presented the finally correct
            // large sample variance for Kappa after so many attempts.

            double[,] matrix =
            {
                { 0.53,  0.05,  0.02 },
                { 0.11,  0.14,  0.05 },
                { 0.01,  0.06,  0.03 },
            };

            // Create a new multi-class confusion matrix:
            var a = new GeneralConfusionMatrix(matrix, 200);

            double[] rowProportions = a.RowProportions;    // should be { 0.6, 0.3, 0.1 }
            double[] colProportions = a.ColumnProportions; // should be { 0.65, 0.25, 0.10 }

            double kappa = a.Kappa;                        // should be 0.429
            double var = a.Variance;                       // should be 0.002885
            double var0 = a.VarianceUnderNull;             // should be 0.003082
            #endregion

            Assert.AreEqual(rowProportions[0], .60, 1e-10);
            Assert.AreEqual(rowProportions[1], .30, 1e-10);
            Assert.AreEqual(rowProportions[2], .10, 1e-10);

            Assert.AreEqual(colProportions[0], .65, 1e-10);
            Assert.AreEqual(colProportions[1], .25, 1e-10);
            Assert.AreEqual(colProportions[2], .10, 1e-10);


            Assert.AreEqual(0.429, kappa, 1e-3);
            Assert.IsFalse(double.IsNaN(a.Kappa));

            Assert.AreEqual(0.002885, a.Variance, 1e-6);
            Assert.AreEqual(0.003082, a.VarianceUnderNull, 1e-6);

            Assert.IsFalse(double.IsNaN(a.Variance));
            Assert.IsFalse(double.IsNaN(a.VarianceUnderNull));
        }

        [Test]
        public void TotalTest()
        {
            int[,] matrix =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            };

            GeneralConfusionMatrix target = new GeneralConfusionMatrix(matrix);

            int[] colTotals = target.ColumnTotals;
            int[] rowTotals = target.RowTotals;

            Assert.AreEqual(1 + 2 + 3, rowTotals[0]);
            Assert.AreEqual(4 + 5 + 6, rowTotals[1]);
            Assert.AreEqual(7 + 8 + 9, rowTotals[2]);

            Assert.AreEqual(1 + 4 + 7, colTotals[0]);
            Assert.AreEqual(2 + 5 + 8, colTotals[1]);
            Assert.AreEqual(3 + 6 + 9, colTotals[2]);
        }

        [Test]
        public void GeometricAgreementTest()
        {
            int[,] matrix =
            {
                { 462,  241 },
                { 28,    59 },
            };

            GeneralConfusionMatrix target = new GeneralConfusionMatrix(matrix);

            double actual = target.GeometricAgreement;
            double expected = Math.Sqrt(462 * 59);
            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [Test]
        public void ChiSquareTest()
        {
            int[,] matrix =
            {
                {  10,      9,      5,      7,      8     },
                {   1,      2,      0,      1,      2     },
                {   0,      0,      1,      0,      1     },
                {   1,      0,      0,      3,      0     },
                {   0,      2,      0,      0,      2     },
            };

            GeneralConfusionMatrix target = new GeneralConfusionMatrix(matrix);

            double actual = target.ChiSquare;

            Assert.AreEqual(19.43, actual, 0.01);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [Test]
        public void ChiSquareTest2()
        {
            int[,] matrix =
            {
                { 296, 2, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                {   0, 293, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 1, 1, 0, 0, 1 },
                {   1, 0, 274, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 20 },
                {   1, 0, 0, 278, 0, 1, 0, 0, 7, 4, 2, 1, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 1, 2 },
                {   0, 1, 0, 0, 290, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 7 },
                {   0, 4, 1, 7, 2, 263, 0, 0, 0, 2, 0, 3, 0, 0, 0, 1, 0, 0, 0, 13, 0, 0, 0, 0, 0, 0, 4 },
                {   5, 7, 1, 29, 1, 20, 0, 0, 10, 10, 49, 28, 1, 6, 0, 4, 1, 29, 0, 21, 15, 9, 3, 4, 0, 32, 15 },
                {   0, 7, 0, 23, 9, 37, 0, 0, 0, 19, 34, 4, 8, 2, 0, 1, 6, 13, 0, 13, 53, 8, 6, 1, 0, 46, 10 },
                {   0, 0, 0, 0, 0, 0, 0, 0, 298, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0 },
                {   2, 1, 0, 2, 2, 0, 0, 0, 11, 250, 0, 0, 5, 2, 0, 3, 4, 0, 0, 1, 1, 0, 0, 5, 0, 11, 0 },
                {   1, 3, 0, 0, 2, 3, 0, 0, 0, 1, 251, 4, 0, 0, 0, 0, 0, 1, 2, 2, 1, 11, 10, 0, 0, 6, 2 },
                {   0, 0, 0, 4, 0, 2, 0, 0, 0, 0, 2, 291, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
                {   0, 0, 0, 0, 1, 0, 0, 0, 0, 2, 0, 0, 278, 10, 0, 0, 5, 0, 0, 0, 0, 0, 0, 2, 0, 2, 0 },
                {   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 14, 279, 0, 0, 3, 0, 0, 0, 0, 0, 0, 2, 0, 2, 0 },
                {   0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 292, 0, 0, 0, 0, 0, 1, 0, 0, 3, 0, 0, 1 },
                {   0, 0, 2, 3, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 289, 0, 0, 0, 0, 0, 0, 0, 2, 0, 2, 0 },
                {   0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 1, 3, 0, 0, 289, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0 },
                {   0, 0, 0, 2, 0, 0, 0, 0, 1, 0, 2, 2, 0, 0, 0, 0, 0, 276, 0, 2, 10, 0, 0, 0, 0, 4, 1 },
                {   4, 0, 0, 0, 7, 2, 0, 0, 1, 2, 0, 0, 3, 0, 2, 0, 0, 0, 274, 0, 0, 0, 0, 2, 0, 2, 1 },
                {   0, 0, 0, 0, 0, 25, 0, 0, 0, 0, 1, 0, 0, 0, 0, 8, 0, 0, 0, 262, 0, 0, 2, 0, 0, 1, 1 },
                {   0, 2, 0, 0, 0, 0, 0, 0, 1, 0, 2, 0, 0, 0, 0, 0, 0, 10, 0, 2, 278, 2, 0, 0, 0, 3, 0 },
                {   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 15, 0, 0, 0, 1, 0, 0, 0, 0, 0, 4, 273, 4, 0, 0, 2, 1 },
                {   0, 1, 1, 0, 1, 0, 0, 0, 0, 3, 7, 1, 1, 0, 0, 0, 0, 0, 0, 1, 2, 2, 277, 0, 0, 2, 1 },
                {   0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 7, 5, 0, 6, 4, 0, 0, 0, 1, 0, 0, 259, 0, 10, 0 },
                {   14, 13, 0, 14, 0, 28, 0, 0, 6, 94, 10, 2, 1, 10, 0, 8, 7, 0, 0, 16, 0, 16, 24, 5, 0, 32, 0 },
                {   0, 2, 1, 9, 0, 1, 0, 0, 0, 22, 7, 3, 11, 0, 1, 5, 1, 6, 0, 2, 4, 1, 2, 7, 0, 214, 1 },
                {   0, 1, 13, 0, 5, 0, 0, 0, 0, 0, 0, 0, 1, 0, 3, 0, 0, 0, 3, 1, 1, 0, 0, 0, 0, 0, 272 },
            };

            var target = new GeneralConfusionMatrix(matrix);

            double actual = target.ChiSquare;
            Assert.AreEqual(142057.46532326791, actual);

            Assert.AreEqual(0, target.GeometricAgreement);
            Assert.AreEqual(0.814320987654321, target.OverallAgreement);
            Assert.AreEqual(0.814320987654321, matrix.Diagonal().Sum() / (double)target.Samples);
        }


#if !NO_DATA_TABLE
        [Test]
        public void random_search()
        {
            string[] labels = Vector.Range(35).To<string[]>();
            var codification = new Codification("Labels", labels);

            var csv = CsvReader.FromText(Properties.Resources.labels, hasHeaders: false);
            int[] expected = csv.ReadLine().ToInt32();
            int[] predicted = csv.ReadLine().ToInt32();

            check_large(codification, new GeneralConfusionMatrix(expected, predicted));
        }

        private static void check_large(Codification codification, GeneralConfusionMatrix cm)
        {
            double[] precision = cm.Precision;
            double[] recall = cm.Recall;

            int[] rowErrors = cm.RowErrors;
            int[] colErrors = cm.ColumnErrors;

            int[] colTotal = cm.ColumnTotals;
            int[] rowTotal = cm.RowTotals;

            // Obtain relevant measures
            int[,] matrix = cm.Matrix;
            object[,] column01 = Matrix.ColumnVector(codification.Columns[0].Values).ToObject();
            object[,] columns2_to_35 = matrix.ToObject();
            object[,] column05 = Matrix.ColumnVector(rowErrors).ToObject();
            object[,] column06 = Matrix.ColumnVector(recall).ToObject();
            object[,] column07 = Matrix.ColumnVector(rowTotal).ToObject();

            object[,] values = Matrix.Concatenate(
                column01,
                columns2_to_35,
                column05,
                column06,
                column07
            );

            object[] row05 = Matrix.Concatenate<object>("Error", colErrors.ToObject());
            object[] row06 = Matrix.Concatenate<object>("Precision", precision.ToObject());
            object[] row07 = Matrix.Concatenate<object>("Total", colTotal.ToObject());

            values = values.InsertRow(row05)
                .InsertRow(row06)
                .InsertRow(row07);

            // Name of each of the columns in order to create a data table
            string[] columnNames = "Label".Concatenate(codification.Columns[0].Values.Concatenate("Error", "Recall", "Total"));

            // Create a table from the matrix and columns
            DataTable table = values.ToTable(columnNames);


            string[] actualNames;
            object[,] actualTable = table.ToMatrix<object>(out actualNames);

            actualTable = actualTable.InsertRow(columnNames, 0);

            string str = actualTable.ToCSharp();

            object[,] expectedTable =
            {
                { "Label",            "0",               "1",               "2",               "3",               "4",               "5",               "6",              "7",               "8",               "9",             "10",              "11",              "12",              "13",             "14",               "15",              "16",              "17",             "18",              "19",             "20",               "21",              "22",             "23",         "24",              "25",              "26",              "27",              "28",              "29",               "30",              "31",              "32",              "33",              "34", "Error",          "Recall",  "Total" },
                { "0" ,                 0,                 0,                 2,                 1,                 0,                 0,                 0,                0,                 0,                 0,                0,                 1,                 0,                 1,                0,                  0,                 0,                 0,                0,                 0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,       5, 0.000000000000000,       5  },
                { "1",                  0,              2095,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                20,               23,                  0,                 0,                 1,                0,                 0,                0,                  0,                 0,                0,            5,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,      49, 0.977145522388060,    2144  },
                { "2",                  0,                 0,              2755,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                0,                 0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,       0, 1.000000000000000,    2755  },
                { "3",                  0,                 0,                 0,              2384,                 0,                 0,                 0,                0,                 1,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                0,                 0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,       1, 0.999580712788260,    2385  },
                { "4",                  0,                 0,                 0,                 0,              2483,                 0,                 0,                0,                 1,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                0,                 0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,       1, 0.999597423510467,    2484  },
                { "5",                  0,                 0,                10,                 0,                 0,              2239,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                0,                 0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,      10, 0.995553579368608,    2249  },
                { "6",                  0,                 0,                 0,                 0,                 0,                 0,              1934,               53,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                0,                 0,                0,                  0,                 1,                0,            0,                 0,                 0,                 4,                 0,                 0,                  0,                 0,                 0,                 0,                 0,      58, 0.970883534136546,    1992  },
                { "7",                  0,                 0,                 0,                 0,                 0,                 0,                26,             2170,                 0,                 1,                0,                 0,                 0,                 0,                0,                  0,                 0,                 2,                0,                 0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,      29, 0.986812187357890,    2199  },
                { "8",                  0,                 0,                 0,                 0,                 0,                 0,                 0,                0,              2041,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                0,                 0,                1,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,       1, 0.999510284035260,    2042  },
                { "9",                  0,                 0,                 0,                 0,                 1,                 0,                 2,               13,                 0,              1744,                1,                 0,                 2,                 0,                0,                  0,                 1,                 0,                0,                60,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 1,                  0,                 0,                 0,                 0,                 0,      81, 0.955616438356164,    1825  },
                { "10",                 0,                 0,                 0,                 0,                 0,                 2,                 0,                0,                 0,                 0,             1668,                 0,                 0,                 0,                0,                  0,                 0,                 0,                0,                10,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,      12, 0.992857142857143,    1680  },
                { "11",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,              1401,                 0,                 0,                0,                  0,                 0,                 0,                0,                 0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,       0, 1.000000000000000,    1401  },
                { "12",                 0,                 0,                 0,                 0,                35,                 0,                59,              140,                 0,               139,              129,                 4,               572,                 0,               19,                 16,                 0,                 0,                0,                 2,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 1,                  0,                 0,                 0,                 0,                 0,     544, 0.512544802867383,    1116  },
                { "13",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,              1645,                0,                  0,                 0,                 0,                0,                 0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,       0, 1.000000000000000,    1645  },
                { "14",                 0,                36,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                13,              823,                  0,                 0,                 0,                0,                 0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,      49, 0.943807339449541,     872  },
                { "15",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                801,                21,                 0,                0,                 0,                0,                  1,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,      22, 0.973268529769137,     823  },
                { "16",                 0,                 0,                 1,                 0,                 0,                 0,                 0,                0,                 1,                 0,                0,                 0,                 0,                 0,                0,                  0,              1419,                 0,                0,                 0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 2,                 0,                  0,                 0,                 0,                 0,                 0,       4, 0.997189037245257,    1423  },
                { "17",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                11,                0,                  0,                 0,              1385,                0,                 0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,      11, 0.992120343839542,    1396  },
                { "18",                 0,                 0,              1219,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,              2210,                0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,    1219, 0.644502770487022,    3429  },
                { "19",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                 0,              439,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,       0, 1.000000000000000,     439  },
                { "20",                 0,                 0,                 0,                 0,                 0,                 0,                 1,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                 0,                0,              797,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,       1, 0.99874686716791983,   798  },
                { "21",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                 0,                0,                0,                337,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,       0, 1.000000000000000,     337  },
                { "22",                 0,                 0,                 1,                 0,                 0,                 5,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 1,                 0,               261,                0,                2,                  0,               604,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 1,                 0,                 0,     271, 0.690285714285714,     875  },
                { "23",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                 3,                0,                0,                  0,                 0,             1075,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,       3, 0.997217068645640,    1078  },
                { "24",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                 0,                0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,       0, 0.000000000000000,       0  },
                { "25",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                 0,                0,                0,                  0,                 0,                0,            0,               624,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,                 0,       0, 1.000000000000000,     624  },
                { "26",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 1,                 0,                0,                  0,                 0,                 0,                 0,                0,               13,                  0,                 0,                0,            0,                36,              1008,                 0,                 1,                 0,                  0,                 0,                 0,                 0,                 0,      51, 0.951841359773371,    1059  },
                { "27",                 0,                 0,                 0,                 0,                 0,                 0,                 3,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                 0,                0,                0,                  0,                 0,                0,            0,                 0,                 0,               446,                 0,                 0,                  0,                 0,                 0,                 0,                 0,       3, 0.993318485523385,     449  },
                { "28",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                 0,                0,                0,                  0,                 0,                0,            0,                 0,                 1,                 0,               943,                 0,                  0,                 0,                 0,                 0,                 0,       1, 0.998940677966102,     944  },
                { "29",                 0,                 0,                78,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                 1,              301,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,               437,                  0,                 0,                 0,                 0,                 0,     380, 0.534883720930233,     817  },
                { "30",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                 0,                0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                 25,                 0,                 0,                 0,                 0,       0, 1.000000000000000,      25  },
                { "31",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                 0,                0,                1,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  9,               472,                 0,                 0,                 0,      10, 0.979253112033195,     482  },
                { "32",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                 1,                0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  9,                 0,               410,                 0,                 0,      10, 0.976190476190476,     420  },
                { "33",                 0,                 0,                 0,                 0,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                 0,                0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 4,                 0,                215,                 0,                 0,               207,                 0,     219, 0.485915492957747,     426  },
                { "34",                 0,                 0,                 0,                 2,                 0,                 0,                 0,                0,                 0,                 0,                0,                 0,                 0,                 0,                0,                  0,                 0,                 0,                 0,                0,                0,                  0,                 0,                0,            0,                 0,                 0,                 0,                 0,                 0,                  0,                 0,                 0,                 0,               284,       2, 0.993006993006993,     286  },
                { "Error",              0,                36,              1311,                 3,                36,                 7,                91,               206,                3,               140,              130,                 5,                 3,                45,               42,                 16,                23,                 3,               266,              373,               17,                  1,                 1,                0,            5,                36,                 1,                 4,                 7,                 2,                233,                 0,                 1,                 0,                 0,    null,              null,    null  },
                { "Precision",          0, 0.983106522759268, 0.677570093457944, 0.998743192291579, 0.985708614529575, 0.996883348174533, 0.955061728395062, 0.913299663299663, 0.99853228962818, 0.925690021231423, 0.92769744160178, 0.996443812233286, 0.994782608695652, 0.973372781065089, 0.951445086705202, 0.980416156670747, 0.984049930651872, 0.997838616714697, 0.892568659127625, 0.54064039408867, 0.979115479115479, 0.997041420118343, 0.998347107438017,                1,            0, 0.945454545454545, 0.999008919722498, 0.991111111111111, 0.992631578947368, 0.995444191343964, 0.0968992248062016, 1.000000000000000, 0.997566909975669, 1.000000000000000, 1.000000000000000,    null,              null,    null  },
                { "Total",              0,              2131,              4066,              2387,              2519,              2246,              2025,              2376,             2044,              1884,             1798,              1406,               575,              1690,               865,               817,              1442,              1388,              2476,              812,               814,               338,               605,             1075,            5,               660,              1009,               450,               950,               439,                258,               472,               411,               207,               284,    null,              null,    null  }
            };

            var expectedMatrix = expectedTable.Get(1, 36, 1, 36);
            for (int i = 0; i < expectedMatrix.Rows(); i++)
            {
                for (int j = 0; j < expectedMatrix.Columns(); j++)
                {
                    object e = expectedMatrix[i, j];
                    object a = columns2_to_35[i, j];
                    Assert.IsTrue(a.IsEqual(e));
                }
            }

            var expectedSummaryRows = expectedTable.Get(-3, 0, 1, 36);
            var actualSummaryRows = actualTable.Get(-3, 0, 1, 36);
            for (int i = 0; i < expectedSummaryRows.Rows(); i++)
            {
                for (int j = 0; j < expectedSummaryRows.Columns(); j++)
                {
                    object e = expectedSummaryRows[i, j];
                    object a = actualSummaryRows[i, j];
                    Assert.IsTrue(a.IsEqual(e, atol: 1e-8M));
                }
            }

            var expectedSummaryCols = expectedTable.Get(null, 36, 36 + 3);
            var actualSummaryCols = actualTable.Get(null, 36, 36 + 3);
            for (int i = 0; i < expectedSummaryCols.Rows(); i++)
            {
                for (int j = 0; j < expectedSummaryCols.Columns(); j++)
                {
                    object e = expectedSummaryCols[i, j];
                    object a = actualSummaryCols[i, j];
                    Assert.IsTrue(a.IsEqual(e, atol: 1e-8M));
                }
            }

            for (int i = 0; i < expectedTable.Rows(); i++)
            {
                for (int j = 0; j < expectedTable.Columns(); j++)
                {
                    object e = expectedTable[i, j];
                    object a = actualTable[i, j];
                    Assert.IsTrue(a.IsEqual(e, atol: 1e-8M));
                }
            }
        }
#endif
    }
}