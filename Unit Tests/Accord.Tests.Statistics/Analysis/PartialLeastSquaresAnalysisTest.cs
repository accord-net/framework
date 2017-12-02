﻿// Accord Unit Tests
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
    using Tools = Accord.Statistics.Tools;
    using System;
    using Accord.Statistics.Models.Regression.Linear;
    using Accord.Statistics;

    [TestFixture]
    public class PartialLeastSquaresAnalysisTest
    {

        private static PartialLeastSquaresAnalysis CreateWineExample(out double[,] inputs, out double[,] outputs)
        {
            // References: http://www.utdallas.edu/~herve/Abdi-PLSR2007-pretty.pdf

            // Following the small example by Hervé Abdi (Hervé Abdi, Partial Least Square Regression),
            // we will create a simple example where the goal is to predict the subjective evaluation of
            // a set of 5 wines. The dependent variables that we want to predict for each wine are its 
            // likeability, and how well it goes with meat, or dessert (as rated by a panel of experts).
            // The predictors are the price, the sugar, alcohol, and acidity content of each wine.


            // Here we will list the inputs, or characteristics we would like to use in order to infer
            // information from our wines. Each row denotes a different wine and lists its corresponding
            // observable characteristics. The inputs are usually denoted by X in the literature.

            inputs = new double[,]
            {
            // Wine | Price | Sugar | Alcohol | Acidity
                    {   7,     7,      13,        7 },
                    {   4,     3,      14,        7 },
                    {  10,     5,      12,        5 },
                    {  16,     7,      11,        3 },
                    {  13,     3,      10,        3 },
            };


            // Here we will list our dependent variables. Dependent variables are the outputs, or what we
            // would like to infer or predict from our available data, given a new observation. The outputs
            // are usually denotes as Y in the literature.

            outputs = new double[,]
            {
                // Wine | Hedonic | Goes with meat | Goes with dessert
                {           14,          7,                 8 },
                {           10,          7,                 6 },
                {            8,          5,                 5 },
                {            2,          4,                 7 },
                {            6,          2,                 4 },
            };


            // Next, we will create our Partial Least Squares Analysis passing the inputs (values for 
            // predictor variables) and the associated outputs (values for dependent variables).

            // We will also be using the using the Covariance Matrix/Center method (data will only
            // be mean centered but not normalized) and the NIPALS algorithm. 
            PartialLeastSquaresAnalysis pls = new PartialLeastSquaresAnalysis(inputs, outputs,
                AnalysisMethod.Center, PartialLeastSquaresAlgorithm.SIMPLS);

            // Compute the analysis with all factors. The number of factors
            // could also have been specified in a overload of this method.

            pls.Compute();

            // After computing the analysis, we can create a linear regression model in order
            // to predict new variables. To do that, we may call the CreateRegression() method.

            MultivariateLinearRegression regression = pls.CreateRegression();

            // After the regression has been created, we will be able to classify new instances. 
            // For example, we will compute the outputs for the first input sample:

            double[] y = regression.Compute(new double[] { 7, 7, 13, 7 });

            // The y output will be very close to the corresponding output used as reference.
            // In this case, y is a vector of length 3 with values { 14.00, 7.00, 7.75 }.

            Assert.AreEqual(14.00, y[0], 1e-2);
            Assert.AreEqual(+7.00, y[1], 1e-2);
            Assert.AreEqual(+7.75, y[2], 1e-2);

            return pls;
        }


        private static PartialLeastSquaresAnalysis CreateWineExample_new_method(out double[][] inputs, out double[][] outputs)
        {
            #region doc_learn
            // References: http://www.utdallas.edu/~herve/Abdi-PLSR2007-pretty.pdf

            // Following the small example by Hervé Abdi (Hervé Abdi, Partial Least Square Regression),
            // we will create a simple example where the goal is to predict the subjective evaluation of
            // a set of 5 wines. The dependent variables that we want to predict for each wine are its 
            // likeability, and how well it goes with meat, or dessert (as rated by a panel of experts).
            // The predictors are the price, the sugar, alcohol, and acidity content of each wine.


            // Here we will list the inputs, or characteristics we would like to use in order to infer
            // information from our wines. Each row denotes a different wine and lists its corresponding
            // observable characteristics. The inputs are usually denoted by X in the literature.

            inputs = new double[][]
            {
                //      Wine | Price | Sugar | Alcohol | Acidity
                new double[] {   7,     7,      13,        7 },
                new double[] {   4,     3,      14,        7 },
                new double[] {  10,     5,      12,        5 },
                new double[] {  16,     7,      11,        3 },
                new double[] {  13,     3,      10,        3 },
            };


            // Here we will list our dependent variables. Dependent variables are the outputs, or what we
            // would like to infer or predict from our available data, given a new observation. The outputs
            // are usually denotes as Y in the literature.

            outputs = new double[][]
            {
                //             Wine | Hedonic | Goes with meat | Goes with dessert
                new double[] {           14,          7,                 8 },
                new double[] {           10,          7,                 6 },
                new double[] {            8,          5,                 5 },
                new double[] {            2,          4,                 7 },
                new double[] {            6,          2,                 4 },
            };


            // Next, we will create our Partial Least Squares Analysis passing the inputs (values for 
            // predictor variables) and the associated outputs (values for dependent variables).

            // We will also be using the using the Covariance Matrix/Center method (data will only
            // be mean centered but not normalized) and the NIPALS algorithm. 
            var pls = new PartialLeastSquaresAnalysis()
            {
                Method = AnalysisMethod.Center,
                Algorithm = PartialLeastSquaresAlgorithm.SIMPLS
            };

            // Compute the analysis with all factors. The number of factors
            // could also have been specified in a overload of this method.

            var regression = pls.Learn(inputs, outputs);

            // After the regression has been created, we will be able to classify new instances. 
            // For example, we will compute the outputs for the first input sample:

            double[] y = regression.Transform(new double[] { 7, 7, 13, 7 });

            // The y output will be very close to the corresponding output used as reference.
            // In this case, y is a vector of length 3 with values { 14.00, 7.00, 7.75 }.
            #endregion

            Assert.AreEqual(14.00, y[0], 1e-2);
            Assert.AreEqual(+7.00, y[1], 1e-2);
            Assert.AreEqual(+7.75, y[2], 1e-2);

            return pls;
        }


        [Test]
        public void SimplsRegressionTest()
        {
            double[,] inputs;
            double[,] outputs;

            var pls = CreateWineExample(out inputs, out outputs);

            MultivariateLinearRegression regression = pls.CreateRegression();

            // test regression intercepts
            double[] intercepts = regression.Intercepts;
            double[] expectedI = { 60.717, -8.509, -4.362 };

            Assert.IsTrue(intercepts.IsEqual(expectedI, 0.01));


            // test regression coefficients
            double[,] coefficients = regression.Coefficients;
            double[,] expectedC =
                {
                    { -1.6981, -0.0566, 0.07075 },
                    {  1.2735,  0.2924, 0.57193 },
                    { -4.0000,  1.0000, 0.50000 },
                    {  1.1792,  0.1226, 0.15919 }
                };

            Assert.IsTrue(coefficients.IsEqual(expectedC, 0.01));


            // Test computation
            double[][] aY = regression.Compute(inputs.ToJagged());

            for (int i = 0; i < outputs.GetLength(0); i++)
            {
                for (int j = 0; j < outputs.GetLength(1); j++)
                {
                    double actualOutput = aY[i][j];
                    double expectedOutput = outputs[i, j];

                    double delta = System.Math.Abs(actualOutput - expectedOutput);
                    double tol = 0.21 * expectedOutput;

                    Assert.IsTrue(delta <= tol);
                }
            }

        }

        [Test]
        public void SimplsRegressionTest_new_method()
        {
            double[][] inputs;
            double[][] outputs;

            var pls = CreateWineExample_new_method(out inputs, out outputs);

            MultivariateLinearRegression regression = pls.CreateRegression();

            // test regression intercepts
            double[] intercepts = regression.Intercepts;
            double[] expectedI = { 60.717, -8.509, -4.362 };

            Assert.IsTrue(intercepts.IsEqual(expectedI, 0.01));


            // test regression coefficients
            double[,] coefficients = regression.Coefficients;
            double[,] expectedC =
                {
                    { -1.6981, -0.0566, 0.07075 },
                    {  1.2735,  0.2924, 0.57193 },
                    { -4.0000,  1.0000, 0.50000 },
                    {  1.1792,  0.1226, 0.15919 }
                };

            Assert.IsTrue(coefficients.IsEqual(expectedC, 0.01));


            // Test computation
            double[][] aY = regression.Transform(inputs);

            for (int i = 0; i < outputs.Length; i++)
            {
                for (int j = 0; j < outputs[i].Length; j++)
                {
                    double actualOutput = aY[i][j];
                    double expectedOutput = outputs[i][j];

                    double delta = System.Math.Abs(actualOutput - expectedOutput);
                    double tol = 0.21 * expectedOutput;

                    Assert.IsTrue(delta <= tol);
                }
            }

            // Test output transform
            double[][] bY = pls.TransformOutput(outputs);
            //string str = bY.ToCSharp();
            double[][] expected = new double[][] {
                new double[] { 55.1168117173553, 19.9814652854436, 23.1058025430891, 8.50770527860088E-15 },
                new double[] { 22.6848893220127, 7.39445606402423, 6.83504323067901, 3.73246403168482E-15 },
                new double[] { -0.877117473967739, -2.79934089543989, 0.379941427036153, -1.88457062440663E-16 },
                new double[] { -48.8124364962975, -9.63329642474673, -25.0857657504786, -7.17568385594517E-15 },
                new double[] { -28.1121470691027, -14.9432840292812, -5.23502145032562, -4.87602839189987E-15 }
            };

            Assert.IsTrue(expected.IsEqual(bY, 1e-6));
        }


        [Test]
        public void SimplsComputeTest()
        {
            double[,] inputs;
            double[,] outputs;

            var pls = CreateWineExample(out inputs, out outputs);

            MultivariateLinearRegression regression = pls.CreateRegression();


            // test factor proportions
            double[] expectedX = { 0.86, 0.12, 0.00, 0.86 };
            double[] actualX = pls.Predictors.FactorProportions;
            Assert.IsTrue(expectedX.IsEqual(actualX, atol: 0.01));

            double[] expectedY = { 0.67, 0.13, 0.17, 0.00 };
            double[] actualY = pls.Dependents.FactorProportions;
            Assert.IsTrue(expectedY.IsEqual(actualY, atol: 0.01));


            // Test Properties
            double[][] weights = pls.Weights;
            double[,] actual = pls.Predictors.Result;

            double[,] X0 = (double[,])pls.Source.Clone(); Tools.Center(X0, inPlace: true);
            double[,] Y0 = (double[,])pls.Output.Clone(); Tools.Center(Y0, inPlace: true);

            // XSCORES = X0*W
            double[][] expected = X0.Dot(weights);
            Assert.IsTrue(expected.IsEqual(actual, 0.01));

        }


        [Test]
        public void NipalsComputeTest()
        {
            double[,] X =
            {
                { 2.5, 2.4 },
                { 0.5, 0.7 },
                { 2.2, 2.9 },
                { 1.9, 2.2 },
                { 3.1, 3.0 },
                { 2.3, 2.7 },
                { 2.0, 1.6 },
                { 1.0, 1.1 },
                { 1.5, 1.6 },
                { 1.1, 0.9 },
            };

            double[,] Y =
            {
                { 1 },
                { 0 },
                { 1 },
                { 0 },
                { 1 },
                { 1 },
                { 0 },
                { 0 },
                { 0 },
                { 0 },
            };


            PartialLeastSquaresAnalysis target = new PartialLeastSquaresAnalysis(X, Y,
                AnalysisMethod.Center, PartialLeastSquaresAlgorithm.NIPALS);

            double[,] X0 = X;
            double[,] Y0 = Y;

            target.Compute();

            double[,] x1 = Matrix.Multiply(target.Predictors.Result,
                target.Predictors.FactorMatrix.Transpose().ToMatrix()).Add(Measures.Mean(X, dimension: 0), 0);
            double[,] y1 = Matrix.Multiply(target.Dependents.Result,
                target.Dependents.FactorMatrix.Transpose().ToMatrix()).Add(Measures.Mean(Y, dimension: 0), 0);


            // XS*XL' ~ X0
            Assert.IsTrue(Matrix.IsEqual(x1, X, 0.01));

            // XS*YL' ~ Y0
            Assert.IsTrue(Matrix.IsEqual(y1, Y, 0.60));


            // ti' * tj = 0; 
            double[][] t = target.scoresX;
            for (int i = 0; i < t.Columns(); i++)
            {
                for (int j = 0; j < t.Columns(); j++)
                {
                    if (i != j)
                        Assert.AreEqual(0, t.GetColumn(i).InnerProduct(t.GetColumn(j)), 0.01);
                }
            }

            // wi' * wj = 0;
            double[][] w = target.Weights;
            for (int i = 0; i < w.Columns(); i++)
            {
                for (int j = 0; j < w.Columns(); j++)
                {
                    if (i != j)
                        Assert.AreEqual(0, w.GetColumn(i).InnerProduct(w.GetColumn(j)), 0.01);
                }
            }

        }

        [Test]
        public void NipalsComputeTest2()
        {
            // Example data from Chiang, Y.Q., Zhuang, Y.M and Yang, J.Y, "Optimal Fisher
            //   discriminant analysis using the rank decomposition", Pattern Recognition,
            //   25 (1992), 101--111, as given by Yi Cao in his excellent PLS tutorial.

            double[,] x1 =
            { 
                // Class 1
                { 5.1, 3.5, 1.4, 0.2 }, { 4.9, 3.0, 1.4, 0.2 }, { 4.7, 3.2, 1.3, 0.2 }, { 4.6, 3.1, 1.5, 0.2 },
                { 5.0, 3.6, 1.4, 0.2 }, { 5.4, 3.9, 1.7, 0.4 }, { 4.6, 3.4, 1.4, 0.3 }, { 5.0, 3.4, 1.5, 0.2 },
                { 4.4, 2.9, 1.4, 0.2 }, { 4.9, 3.1, 1.5, 0.1 }, { 5.4, 3.7, 1.5, 0.2 }, { 4.8, 3.4, 1.6, 0.2 },
                { 4.8, 3.0, 1.4, 0.1 }, { 4.3, 3.0, 1.1, 0.1 }, { 5.8, 4.0, 1.2, 0.2 }, { 5.7, 4.4, 1.5, 0.4 },
                { 5.4, 3.9, 1.3, 0.4 }, { 5.1, 3.5, 1.4, 0.3 }, { 5.7, 3.8, 1.7, 0.3 }, { 5.1, 3.8, 1.5, 0.3 },
                { 5.4, 3.4, 1.7, 0.2 }, { 5.1, 3.7, 1.5, 0.4 }, { 4.6, 3.6, 1.0, 0.2 }, { 5.1, 3.3, 1.7, 0.5 },
                { 4.8, 3.4, 1.9, 0.2 }, { 5.0, 3.0, 1.6, 0.2 }, { 5.0, 3.4, 1.6, 0.4 }, { 5.2, 3.5, 1.5, 0.2 },
                { 5.2, 3.4, 1.4, 0.2 }, { 4.7, 3.2, 1.6, 0.2 }, { 4.8, 3.1, 1.6, 0.2 }, { 5.4, 3.4, 1.5, 0.4 },
                { 5.2, 4.1, 1.5, 0.1 }, { 5.5, 4.2, 1.4, 0.2 }, { 4.9, 3.1, 1.5, 0.2 }, { 5.0, 3.2, 1.2, 0.2 },
                { 5.5, 3.5, 1.3, 0.2 }, { 4.9, 3.6, 1.4, 0.1 }, { 4.4, 3.0, 1.3, 0.2 }, { 5.1, 3.4, 1.5, 0.2 },
                { 5.0, 3.5, 1.3, 0.3 }, { 4.5, 2.3, 1.3, 0.3 }, { 4.4, 3.2, 1.3, 0.2 }, { 5.0, 3.5, 1.6, 0.6 },
                { 5.1, 3.8, 1.9, 0.4 }, { 4.8, 3.0, 1.4, 0.3 }, { 5.1, 3.8, 1.6, 0.2 }, { 4.6, 3.2, 1.4, 0.2 },
                { 5.3, 3.7, 1.5, 0.2 }, { 5.0, 3.3, 1.4, 0.2 }
           };

            double[,] x2 =
            {
                // Class 2
                {7.0, 3.2, 4.7, 1.4 }, { 6.4, 3.2, 4.5, 1.5 }, { 6.9, 3.1, 4.9, 1.5 }, { 5.5, 2.3, 4.0, 1.3 },
                {6.5, 2.8, 4.6, 1.5 }, { 5.7, 2.8, 4.5, 1.3 }, { 6.3, 3.3, 4.7, 1.6 }, { 4.9, 2.4, 3.3, 1.0 },
                {6.6, 2.9, 4.6, 1.3 }, { 5.2, 2.7, 3.9, 1.4 }, { 5.0, 2.0, 3.5, 1.0 }, { 5.9, 3.0, 4.2, 1.5 },
                {6.0, 2.2, 4.0, 1.0 }, { 6.1, 2.9, 4.7 ,1.4 }, { 5.6, 2.9, 3.9, 1.3 }, { 6.7, 3.1, 4.4, 1.4 },
                {5.6, 3.0, 4.5, 1.5 }, { 5.8, 2.7, 4.1, 1.0 }, { 6.2, 2.2, 4.5, 1.5 }, { 5.6, 2.5, 3.9, 1.1 },
                {5.9, 3.2, 4.8, 1.8 }, { 6.1, 2.8, 4.0, 1.3 }, { 6.3, 2.5, 4.9, 1.5 }, { 6.1, 2.8, 4.7, 1.2 },
                {6.4, 2.9, 4.3, 1.3 }, { 6.6, 3.0, 4.4, 1.4 }, { 6.8, 2.8, 4.8, 1.4 }, { 6.7, 3.0, 5.0, 1.7 },
                {6.0, 2.9, 4.5, 1.5 }, { 5.7, 2.6, 3.5, 1.0 }, { 5.5, 2.4, 3.8, 1.1 }, { 5.5, 2.4, 3.7, 1.0 },
                {5.8, 2.7, 3.9, 1.2 }, { 6.0, 2.7, 5.1, 1.6 }, { 5.4, 3.0, 4.5, 1.5 }, { 6.0, 3.4, 4.5, 1.6 },
                {6.7, 3.1, 4.7, 1.5 }, { 6.3, 2.3, 4.4, 1.3 }, { 5.6, 3.0, 4.1, 1.3 }, { 5.5, 2.5, 5.0, 1.3 },
                {5.5, 2.6, 4.4, 1.2 }, { 6.1, 3.0, 4.6, 1.4 }, { 5.8, 2.6, 4.0, 1.2 }, { 5.0, 2.3, 3.3, 1.0 },
                {5.6, 2.7, 4.2, 1.3 }, { 5.7, 3.0, 4.2, 1.2 }, { 5.7, 2.9, 4.2, 1.3 }, { 6.2, 2.9, 4.3, 1.3 },
                {5.1, 2.5, 3.0, 1.1 }, { 5.7, 2.8, 4.1, 1.3 }
            };

            double[,] x3 =
            {
                // Class 3
                { 6.3, 3.3, 6.0, 2.5}, { 5.8, 2.7, 5.1, 1.9 }, { 7.1, 3.0, 5.9, 2.1 }, { 6.3, 2.9, 5.6, 1.8 },
                { 6.5, 3.0, 5.8, 2.2}, { 7.6, 3.0, 6.6, 2.1 }, { 4.9, 2.5, 4.5, 1.7 }, { 7.3, 2.9, 6.3, 1.8 },
                { 6.7, 2.5, 5.8, 1.8}, { 7.2, 3.6, 6.1, 2.5 }, { 6.5, 3.2, 5.1, 2.0 }, { 6.4, 2.7, 5.3, 1.9 },
                { 6.8, 3.0, 5.5, 2.1}, { 5.7, 2.5, 5.0, 2.0 }, { 5.8, 2.8, 5.1, 2.4 }, { 6.4, 3.2, 5.3, 2.3 },
                { 6.5, 3.0, 5.5, 1.8}, { 7.7, 3.8, 6.7, 2.2 }, { 7.7, 2.6, 6.9, 2.3 }, { 6.0, 2.2, 5.0, 1.5 },
                { 6.9, 3.2, 5.7, 2.3}, { 5.6, 2.8, 4.9, 2.0 }, { 7.7, 2.8, 6.7, 2.0 }, { 6.3, 2.7, 4.9, 1.8 },
                { 6.7, 3.3, 5.7, 2.1}, { 7.2, 3.2, 6.0, 1.8 }, { 6.2, 2.8, 4.8, 1.8 }, { 6.1, 3.0, 4.9, 1.8 },
                { 6.4, 2.8, 5.6, 2.1}, { 7.2, 3.0, 5.8, 1.6 }, { 7.4, 2.8, 6.1, 1.9 }, { 7.9, 3.8, 6.4, 2.0 },
                { 6.4, 2.8, 5.6, 2.2}, { 6.3, 2.8, 5.1, 1.5 }, { 6.1, 2.6, 5.6, 1.4 }, { 7.7, 3.0, 6.1, 2.3 },
                { 6.3 ,3.4, 5.6, 2.4}, { 6.4, 3.1, 5.5, 1.8 }, { 6.0, 3.0, 4.8, 1.8 }, { 6.9, 3.1, 5.4, 2.1 },
                { 6.7, 3.1, 5.6, 2.4}, { 6.9, 3.1, 5.1, 2.3 }, { 5.8, 2.7, 5.1, 1.9 }, { 6.8, 3.2, 5.9, 2.3 },
                { 6.7, 3.3, 5.7, 2.5}, { 6.7, 3.0, 5.2, 2.3 }, { 6.3, 2.5, 5.0, 1.9 }, { 6.5, 3.0, 5.2, 2.0 },
                { 6.2, 3.4, 5.4, 2.3}, { 5.9, 3.0, 5.1, 1.8 }
            };

            // Split data set into training (1:25) and testing (26:50)
            var idxTrain = Matrix.Indices(0, 25);
            var idxTest = Matrix.Indices(25, 50);

            double[,] a = x1.Submatrix(idxTrain);
            double[,] b = x2.Submatrix(idxTrain);
            double[,] c = x3.Submatrix(idxTrain);
            double[,] inputs = Matrix.Stack<double>(new double[][,] { a, b, c });


            double[,] outputs = Matrix.Expand(
                new double[,]
                {
                    { 1, 0, 0 }, // repeat 25 times
                    { 0, 1, 0 }, // repeat 25 times
                    { 0, 0, 1 }, // repeat 25 times
                },
                new int[] { 25, 25, 25 });


            PartialLeastSquaresAnalysis target = new PartialLeastSquaresAnalysis(inputs, outputs,
                AnalysisMethod.Standardize, PartialLeastSquaresAlgorithm.NIPALS);

            target.Compute();


            double[] xmean = target.Predictors.Means;
            double[] xstdd = target.Predictors.StandardDeviations;

            // Test X
            double[,] t = target.Predictors.Result;
            double[,] p = target.Predictors.FactorMatrix.ToMatrix();
            double[,] tp = t.Dot(p.Transpose());
            for (int i = 0; i < tp.GetLength(0); i++)
                for (int j = 0; j < tp.GetLength(1); j++)
                    tp[i, j] = tp[i, j] * xstdd[j] + xmean[j];
            Assert.IsTrue(inputs.IsEqual(tp, 0.01));

            // Test Y
            double[] ymean = target.Dependents.Means;
            double[] ystdd = target.Dependents.StandardDeviations;
            double[,] u = target.Dependents.Result;
            double[,] q = target.Dependents.FactorMatrix.ToMatrix();
            double[,] uq = u.Dot(q.Transpose());
            for (int i = 0; i < uq.GetLength(0); i++)
            {
                for (int j = 0; j < uq.GetLength(1); j++)
                {
                    uq[i, j] = uq[i, j] * ystdd[j] + ymean[j];
                }
            }

            Assert.IsTrue(Matrix.IsEqual(outputs, uq, 0.45));


            a = x1.Submatrix(idxTest);
            b = x2.Submatrix(idxTest);
            c = x3.Submatrix(idxTest);
            double[,] test = Matrix.Stack(new double[][,] { a, b, c });

            // test regression for classification
            var regression = target.CreateRegression();

            double[][] Y = regression.Compute(test.ToJagged());

            int cl;
            Matrix.Max(Y[0], out cl);
            Assert.AreEqual(0, cl);

            Matrix.Max(Y[11], out cl);
            Assert.AreEqual(0, cl);

            Matrix.Max(Y[29], out cl);
            Assert.AreEqual(1, cl);

            Matrix.Max(Y[30], out cl);
            Assert.AreEqual(1, cl);

            Matrix.Max(Y[52], out cl);
            Assert.AreEqual(2, cl);

            Matrix.Max(Y[70], out cl);
            Assert.AreEqual(2, cl);


            PartialLeastSquaresAnalysis target2 = new PartialLeastSquaresAnalysis(inputs, outputs,
                AnalysisMethod.Standardize, PartialLeastSquaresAlgorithm.SIMPLS);

            target2.Compute();

            // First columns should be equal
            Assert.IsTrue(Matrix.IsEqual(
                target.Predictors.Result.GetColumn(0).Abs(),
                target2.Predictors.Result.GetColumn(0).Abs(), atol: 0.00001));

            Assert.IsTrue(Matrix.IsEqual(
                target.Predictors.FactorMatrix.GetColumn(0).Abs(),
                target2.Predictors.FactorMatrix.GetColumn(0).Abs(), atol: 0.00001));

            // Others are approximations
            Assert.IsTrue(Matrix.IsEqual(
                target.Predictors.Result.GetColumn(1).Abs(),
                target2.Predictors.Result.GetColumn(1).Abs(), atol: 0.001));

            Assert.IsTrue(Matrix.IsEqual(
                target.Predictors.FactorMatrix.GetColumn(1).Abs(),
                target2.Predictors.FactorMatrix.GetColumn(1).Abs(), atol: 0.01));

            // Explained variance proportion should be similar
            Assert.IsTrue(Matrix.IsEqual(
                target.Predictors.FactorProportions.Submatrix(2),
                target2.Predictors.FactorProportions.Submatrix(2), atol: 0.05));

            Assert.IsTrue(Matrix.IsEqual(
                target.Dependents.FactorProportions,
                target2.Dependents.FactorProportions, atol: 0.8));

        }

        [Test]
        public void NipalsComputeTest3()
        {
            // Example data from  P. Geladi and B.R. Kowalski, "An example of 2-block
            //   predictive partial least-squares regression with simulated data",
            //   Analytica Chemica Acta, 185(1996) 19--32, as given by Yi Cao in his
            //   excellent PLS tutorial.

            double[,] x =
            {
                { 4,   9,  6,  7,  7,  8,  3,  2 },
                { 6,  15, 10, 15, 17, 22,  9,  4 },
                { 8,  21, 14, 23, 27, 36, 15,  6 },
                { 10, 21, 14, 13, 11, 10,  3,  4 },
                { 12, 27, 18, 21, 21, 24,  9,  6 },
                { 14, 33, 22, 29, 31, 38, 15,  8 },
                { 16, 33, 22, 19, 15, 12,  3,  6 },
                { 18, 39, 26, 27, 25, 26,  9,  8 },
                { 20, 45, 30, 35, 35, 40, 15, 10 }
            };

            double[,] y =
            {
                { 1, 1 },
                { 3, 1 },
                { 5, 1 },
                { 1, 3 },
                { 3, 3 },
                { 5, 3 },
                { 1, 5 },
                { 3, 5 },
                { 5, 5 }
            };


            PartialLeastSquaresAnalysis pls = new PartialLeastSquaresAnalysis(x, y,
                AnalysisMethod.Center, PartialLeastSquaresAlgorithm.NIPALS);

            pls.Compute();


            double[,] eYL =
            {
                { 0.808248528018965, -0.588841504103759},
                { 0.588841504103759,  0.808248528018964}
            };

            double[,] eYS =
            {
                { -2.79418006424545,   -0.438814047830411 },
                { -1.17768300820752,   -1.61649705603793  },
                {  0.438814047830411,  -2.79418006424545  },
                { -1.61649705603793,    1.17768300820752  },
                {  0.00000000000000,    0.00000000000     },
                {  1.61649705603793,   -1.17768300820752  },
                { -0.438814047830411,   2.79418006424545  },
                {  1.17768300820752,    1.61649705603793  },
                {  2.79418006424545,    0.438814047830411 }
            };


            double[] eProportionsX = { 0.82623088878551032, 0.17376911121448976, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00 };
            double[] eProportionsY = { 0.50000000000000033, 0.50000000000000011, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00 };


            double[,] aXL = pls.Predictors.FactorMatrix.ToMatrix();
            double[,] aYL = pls.Dependents.FactorMatrix.ToMatrix();
            double[,] aXS = pls.Predictors.Result;
            double[,] aYS = pls.Dependents.Result;
            double[][] aW = pls.Weights;

            var regression = pls.CreateRegression();
            double[,] aB = regression.Coefficients;
            double[] aC = regression.Intercepts;



            for (int i = 0; i < eProportionsX.Length; i++)
            {
                Assert.AreEqual(eProportionsX[i], pls.Predictors.FactorProportions[i], 0.01);
                Assert.AreEqual(eProportionsY[i], pls.Dependents.FactorProportions[i], 0.01);
            }



            for (int i = 0; i < eYL.GetLength(0); i++)
                for (int j = 0; j < eYL.GetLength(1); j++)
                    Assert.AreEqual(aYL[i, j], eYL[i, j], 0.01);

            for (int i = 0; i < eYS.GetLength(0); i++)
                for (int j = 0; j < eYS.GetLength(1); j++)
                    Assert.AreEqual(aYS[i, j], eYS[i, j], 0.01);

        }



        [Test]
        public void NipalsComputeTest_new_method()
        {
            double[][] X =
            {
                new double[] { 2.5, 2.4 },
                new double[] { 0.5, 0.7 },
                new double[] { 2.2, 2.9 },
                new double[] { 1.9, 2.2 },
                new double[] { 3.1, 3.0 },
                new double[] { 2.3, 2.7 },
                new double[] { 2.0, 1.6 },
                new double[] { 1.0, 1.1 },
                new double[] { 1.5, 1.6 },
                new double[] { 1.1, 0.9 },
            };

            double[][] Y =
            {
                new double[] { 1 },
                new double[] { 0 },
                new double[] { 1 },
                new double[] { 0 },
                new double[] { 1 },
                new double[] { 1 },
                new double[] { 0 },
                new double[] { 0 },
                new double[] { 0 },
                new double[] { 0 },
            };


            var target = new PartialLeastSquaresAnalysis(
                AnalysisMethod.Center, PartialLeastSquaresAlgorithm.NIPALS);

            double[][] X0 = X.Copy();
            double[][] Y0 = Y.Copy();

            target.Learn(X, Y);

            double[,] x1 = Matrix.Multiply(target.Predictors.Result,
                target.Predictors.FactorMatrix.Transpose().ToMatrix()).Add(Measures.Mean(X, dimension: 0), 0);
            double[,] y1 = Matrix.Multiply(target.Dependents.Result,
                target.Dependents.FactorMatrix.Transpose().ToMatrix()).Add(Measures.Mean(Y, dimension: 0), 0);


            // XS*XL' ~ X0
            Assert.IsTrue(Matrix.IsEqual(x1, X, 0.01));

            // XS*YL' ~ Y0
            Assert.IsTrue(Matrix.IsEqual(y1, Y, 0.60));


            // ti' * tj = 0; 
            double[][] t = target.scoresX;
            for (int i = 0; i < t.Columns(); i++)
            {
                for (int j = 0; j < t.Columns(); j++)
                {
                    if (i != j)
                        Assert.AreEqual(0, t.GetColumn(i).InnerProduct(t.GetColumn(j)), 0.01);
                }
            }

            // wi' * wj = 0;
            double[][] w = target.Weights;
            for (int i = 0; i < w.Columns(); i++)
            {
                for (int j = 0; j < w.Columns(); j++)
                {
                    if (i != j)
                        Assert.AreEqual(0, w.GetColumn(i).InnerProduct(w.GetColumn(j)), 0.01);
                }
            }

        }

        [Test]
        public void NipalsComputeTest2_new_method()
        {
            // Example data from Chiang, Y.Q., Zhuang, Y.M and Yang, J.Y, "Optimal Fisher
            //   discriminant analysis using the rank decomposition", Pattern Recognition,
            //   25 (1992), 101--111, as given by Yi Cao in his excellent PLS tutorial.

            double[][] x1 = new double[,]
            { 
                // Class 1
                { 5.1, 3.5, 1.4, 0.2 }, { 4.9, 3.0, 1.4, 0.2 }, { 4.7, 3.2, 1.3, 0.2 }, { 4.6, 3.1, 1.5, 0.2 },
                { 5.0, 3.6, 1.4, 0.2 }, { 5.4, 3.9, 1.7, 0.4 }, { 4.6, 3.4, 1.4, 0.3 }, { 5.0, 3.4, 1.5, 0.2 },
                { 4.4, 2.9, 1.4, 0.2 }, { 4.9, 3.1, 1.5, 0.1 }, { 5.4, 3.7, 1.5, 0.2 }, { 4.8, 3.4, 1.6, 0.2 },
                { 4.8, 3.0, 1.4, 0.1 }, { 4.3, 3.0, 1.1, 0.1 }, { 5.8, 4.0, 1.2, 0.2 }, { 5.7, 4.4, 1.5, 0.4 },
                { 5.4, 3.9, 1.3, 0.4 }, { 5.1, 3.5, 1.4, 0.3 }, { 5.7, 3.8, 1.7, 0.3 }, { 5.1, 3.8, 1.5, 0.3 },
                { 5.4, 3.4, 1.7, 0.2 }, { 5.1, 3.7, 1.5, 0.4 }, { 4.6, 3.6, 1.0, 0.2 }, { 5.1, 3.3, 1.7, 0.5 },
                { 4.8, 3.4, 1.9, 0.2 }, { 5.0, 3.0, 1.6, 0.2 }, { 5.0, 3.4, 1.6, 0.4 }, { 5.2, 3.5, 1.5, 0.2 },
                { 5.2, 3.4, 1.4, 0.2 }, { 4.7, 3.2, 1.6, 0.2 }, { 4.8, 3.1, 1.6, 0.2 }, { 5.4, 3.4, 1.5, 0.4 },
                { 5.2, 4.1, 1.5, 0.1 }, { 5.5, 4.2, 1.4, 0.2 }, { 4.9, 3.1, 1.5, 0.2 }, { 5.0, 3.2, 1.2, 0.2 },
                { 5.5, 3.5, 1.3, 0.2 }, { 4.9, 3.6, 1.4, 0.1 }, { 4.4, 3.0, 1.3, 0.2 }, { 5.1, 3.4, 1.5, 0.2 },
                { 5.0, 3.5, 1.3, 0.3 }, { 4.5, 2.3, 1.3, 0.3 }, { 4.4, 3.2, 1.3, 0.2 }, { 5.0, 3.5, 1.6, 0.6 },
                { 5.1, 3.8, 1.9, 0.4 }, { 4.8, 3.0, 1.4, 0.3 }, { 5.1, 3.8, 1.6, 0.2 }, { 4.6, 3.2, 1.4, 0.2 },
                { 5.3, 3.7, 1.5, 0.2 }, { 5.0, 3.3, 1.4, 0.2 }
           }.ToJagged();

            double[][] x2 = new double[,]
            {
                // Class 2
                {7.0, 3.2, 4.7, 1.4 }, { 6.4, 3.2, 4.5, 1.5 }, { 6.9, 3.1, 4.9, 1.5 }, { 5.5, 2.3, 4.0, 1.3 },
                {6.5, 2.8, 4.6, 1.5 }, { 5.7, 2.8, 4.5, 1.3 }, { 6.3, 3.3, 4.7, 1.6 }, { 4.9, 2.4, 3.3, 1.0 },
                {6.6, 2.9, 4.6, 1.3 }, { 5.2, 2.7, 3.9, 1.4 }, { 5.0, 2.0, 3.5, 1.0 }, { 5.9, 3.0, 4.2, 1.5 },
                {6.0, 2.2, 4.0, 1.0 }, { 6.1, 2.9, 4.7 ,1.4 }, { 5.6, 2.9, 3.9, 1.3 }, { 6.7, 3.1, 4.4, 1.4 },
                {5.6, 3.0, 4.5, 1.5 }, { 5.8, 2.7, 4.1, 1.0 }, { 6.2, 2.2, 4.5, 1.5 }, { 5.6, 2.5, 3.9, 1.1 },
                {5.9, 3.2, 4.8, 1.8 }, { 6.1, 2.8, 4.0, 1.3 }, { 6.3, 2.5, 4.9, 1.5 }, { 6.1, 2.8, 4.7, 1.2 },
                {6.4, 2.9, 4.3, 1.3 }, { 6.6, 3.0, 4.4, 1.4 }, { 6.8, 2.8, 4.8, 1.4 }, { 6.7, 3.0, 5.0, 1.7 },
                {6.0, 2.9, 4.5, 1.5 }, { 5.7, 2.6, 3.5, 1.0 }, { 5.5, 2.4, 3.8, 1.1 }, { 5.5, 2.4, 3.7, 1.0 },
                {5.8, 2.7, 3.9, 1.2 }, { 6.0, 2.7, 5.1, 1.6 }, { 5.4, 3.0, 4.5, 1.5 }, { 6.0, 3.4, 4.5, 1.6 },
                {6.7, 3.1, 4.7, 1.5 }, { 6.3, 2.3, 4.4, 1.3 }, { 5.6, 3.0, 4.1, 1.3 }, { 5.5, 2.5, 5.0, 1.3 },
                {5.5, 2.6, 4.4, 1.2 }, { 6.1, 3.0, 4.6, 1.4 }, { 5.8, 2.6, 4.0, 1.2 }, { 5.0, 2.3, 3.3, 1.0 },
                {5.6, 2.7, 4.2, 1.3 }, { 5.7, 3.0, 4.2, 1.2 }, { 5.7, 2.9, 4.2, 1.3 }, { 6.2, 2.9, 4.3, 1.3 },
                {5.1, 2.5, 3.0, 1.1 }, { 5.7, 2.8, 4.1, 1.3 }
            }.ToJagged();

            double[][] x3 = new double[,]
            {
                // Class 3
                { 6.3, 3.3, 6.0, 2.5}, { 5.8, 2.7, 5.1, 1.9 }, { 7.1, 3.0, 5.9, 2.1 }, { 6.3, 2.9, 5.6, 1.8 },
                { 6.5, 3.0, 5.8, 2.2}, { 7.6, 3.0, 6.6, 2.1 }, { 4.9, 2.5, 4.5, 1.7 }, { 7.3, 2.9, 6.3, 1.8 },
                { 6.7, 2.5, 5.8, 1.8}, { 7.2, 3.6, 6.1, 2.5 }, { 6.5, 3.2, 5.1, 2.0 }, { 6.4, 2.7, 5.3, 1.9 },
                { 6.8, 3.0, 5.5, 2.1}, { 5.7, 2.5, 5.0, 2.0 }, { 5.8, 2.8, 5.1, 2.4 }, { 6.4, 3.2, 5.3, 2.3 },
                { 6.5, 3.0, 5.5, 1.8}, { 7.7, 3.8, 6.7, 2.2 }, { 7.7, 2.6, 6.9, 2.3 }, { 6.0, 2.2, 5.0, 1.5 },
                { 6.9, 3.2, 5.7, 2.3}, { 5.6, 2.8, 4.9, 2.0 }, { 7.7, 2.8, 6.7, 2.0 }, { 6.3, 2.7, 4.9, 1.8 },
                { 6.7, 3.3, 5.7, 2.1}, { 7.2, 3.2, 6.0, 1.8 }, { 6.2, 2.8, 4.8, 1.8 }, { 6.1, 3.0, 4.9, 1.8 },
                { 6.4, 2.8, 5.6, 2.1}, { 7.2, 3.0, 5.8, 1.6 }, { 7.4, 2.8, 6.1, 1.9 }, { 7.9, 3.8, 6.4, 2.0 },
                { 6.4, 2.8, 5.6, 2.2}, { 6.3, 2.8, 5.1, 1.5 }, { 6.1, 2.6, 5.6, 1.4 }, { 7.7, 3.0, 6.1, 2.3 },
                { 6.3 ,3.4, 5.6, 2.4}, { 6.4, 3.1, 5.5, 1.8 }, { 6.0, 3.0, 4.8, 1.8 }, { 6.9, 3.1, 5.4, 2.1 },
                { 6.7, 3.1, 5.6, 2.4}, { 6.9, 3.1, 5.1, 2.3 }, { 5.8, 2.7, 5.1, 1.9 }, { 6.8, 3.2, 5.9, 2.3 },
                { 6.7, 3.3, 5.7, 2.5}, { 6.7, 3.0, 5.2, 2.3 }, { 6.3, 2.5, 5.0, 1.9 }, { 6.5, 3.0, 5.2, 2.0 },
                { 6.2, 3.4, 5.4, 2.3}, { 5.9, 3.0, 5.1, 1.8 }
            }.ToJagged();

            // Split data set into training (1:25) and testing (26:50)
            var idxTrain = Matrix.Indices(0, 25);
            var idxTest = Matrix.Indices(25, 50);

            double[][] a = x1.Submatrix(idxTrain);
            double[][] b = x2.Submatrix(idxTrain);
            double[][] c = x3.Submatrix(idxTrain);
            double[][] inputs = Matrix.Stack<double>(new double[][][] { a, b, c });


            double[][] outputs = Matrix.Expand(
                new double[,]
                {
                    { 1, 0, 0 }, // repeat 25 times
                    { 0, 1, 0 }, // repeat 25 times
                    { 0, 0, 1 }, // repeat 25 times
                },
                new int[] { 25, 25, 25 }).ToJagged();


            var target = new PartialLeastSquaresAnalysis(
                AnalysisMethod.Standardize, PartialLeastSquaresAlgorithm.NIPALS);

            target.Learn(inputs, outputs);


            double[] xmean = target.Predictors.Means;
            double[] xstdd = target.Predictors.StandardDeviations;

            // Test X
            double[,] t = target.Predictors.Result;
            double[,] p = target.Predictors.FactorMatrix.ToMatrix();
            double[,] tp = t.Dot(p.Transpose());
            for (int i = 0; i < tp.GetLength(0); i++)
                for (int j = 0; j < tp.GetLength(1); j++)
                    tp[i, j] = tp[i, j] * xstdd[j] + xmean[j];
            Assert.IsTrue(inputs.IsEqual(tp, 0.01));

            // Test Y
            double[] ymean = target.Dependents.Means;
            double[] ystdd = target.Dependents.StandardDeviations;
            double[,] u = target.Dependents.Result;
            double[,] q = target.Dependents.FactorMatrix.ToMatrix();
            double[,] uq = u.Dot(q.Transpose());
            for (int i = 0; i < uq.GetLength(0); i++)
                for (int j = 0; j < uq.GetLength(1); j++)
                    uq[i, j] = uq[i, j] * ystdd[j] + ymean[j];

            Assert.IsTrue(Matrix.IsEqual(outputs, uq, 0.45));


            a = x1.Submatrix(idxTest);
            b = x2.Submatrix(idxTest);
            c = x3.Submatrix(idxTest);
            double[][] test = Matrix.Stack(new double[][][] { a, b, c });

            // test regression for classification
            var regression = target.CreateRegression();

            double[][] Y = regression.Compute(test);

            int cl;
            Matrix.Max(Y[0], out cl);
            Assert.AreEqual(0, cl);

            Matrix.Max(Y[11], out cl);
            Assert.AreEqual(0, cl);

            Matrix.Max(Y[29], out cl);
            Assert.AreEqual(1, cl);

            Matrix.Max(Y[30], out cl);
            Assert.AreEqual(1, cl);

            Matrix.Max(Y[52], out cl);
            Assert.AreEqual(2, cl);

            Matrix.Max(Y[70], out cl);
            Assert.AreEqual(2, cl);


            var target2 = new PartialLeastSquaresAnalysis()
            {
                Method = AnalysisMethod.Standardize,
                Algorithm = PartialLeastSquaresAlgorithm.SIMPLS
            };

            target2.Learn(inputs, outputs);

            // First columns should be equal
            Assert.IsTrue(Matrix.IsEqual(
                target.Predictors.Result.GetColumn(0).Abs(),
                target2.Predictors.Result.GetColumn(0).Abs(), atol: 0.00001));

            Assert.IsTrue(Matrix.IsEqual(
                target.Predictors.FactorMatrix.GetColumn(0).Abs(),
                target2.Predictors.FactorMatrix.GetColumn(0).Abs(), atol: 0.00001));

            // Others are approximations
            Assert.IsTrue(Matrix.IsEqual(
                target.Predictors.Result.GetColumn(1).Abs(),
                target2.Predictors.Result.GetColumn(1).Abs(), atol: 0.001));

            Assert.IsTrue(Matrix.IsEqual(
                target.Predictors.FactorMatrix.GetColumn(1).Abs(),
                target2.Predictors.FactorMatrix.GetColumn(1).Abs(), atol: 0.01));

            // Explained variance proportion should be similar
            Assert.IsTrue(Matrix.IsEqual(
                target.Predictors.FactorProportions.Submatrix(2),
                target2.Predictors.FactorProportions.Submatrix(2), atol: 0.05));

            Assert.IsTrue(Matrix.IsEqual(
                target.Dependents.FactorProportions,
                target2.Dependents.FactorProportions, atol: 0.8));

        }

        [Test]
        public void NipalsComputeTest3_new_method()
        {
            // Example data from  P. Geladi and B.R. Kowalski, "An example of 2-block
            //   predictive partial least-squares regression with simulated data",
            //   Analytica Chemica Acta, 185(1996) 19--32, as given by Yi Cao in his
            //   excellent PLS tutorial.

            double[][] x =
            {
                new double[] { 4,   9,  6,  7,  7,  8,  3,  2 },
                new double[] { 6,  15, 10, 15, 17, 22,  9,  4 },
                new double[] { 8,  21, 14, 23, 27, 36, 15,  6 },
                new double[] { 10, 21, 14, 13, 11, 10,  3,  4 },
                new double[] { 12, 27, 18, 21, 21, 24,  9,  6 },
                new double[] { 14, 33, 22, 29, 31, 38, 15,  8 },
                new double[] { 16, 33, 22, 19, 15, 12,  3,  6 },
                new double[] { 18, 39, 26, 27, 25, 26,  9,  8 },
                new double[] { 20, 45, 30, 35, 35, 40, 15, 10 }
            };

            double[][] y =
            {
                new double[] { 1, 1 },
                new double[] { 3, 1 },
                new double[] { 5, 1 },
                new double[] { 1, 3 },
                new double[] { 3, 3 },
                new double[] { 5, 3 },
                new double[] { 1, 5 },
                new double[] { 3, 5 },
                new double[] { 5, 5 }
            };


            var pls = new PartialLeastSquaresAnalysis(
                AnalysisMethod.Center, PartialLeastSquaresAlgorithm.NIPALS);

            var regression = pls.Learn(x, y);


            double[,] eYL =
            {
                { 0.808248528018965, -0.588841504103759},
                { 0.588841504103759,  0.808248528018964}
            };

            double[,] eYS =
            {
                { -2.79418006424545,   -0.438814047830411 },
                { -1.17768300820752,   -1.61649705603793  },
                {  0.438814047830411,  -2.79418006424545  },
                { -1.61649705603793,    1.17768300820752  },
                {  0.00000000000000,    0.00000000000     },
                {  1.61649705603793,   -1.17768300820752  },
                { -0.438814047830411,   2.79418006424545  },
                {  1.17768300820752,    1.61649705603793  },
                {  2.79418006424545,    0.438814047830411 }
            };


            double[] eProportionsX = { 0.82623088878551032, 0.17376911121448976, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00 };
            double[] eProportionsY = { 0.50000000000000033, 0.50000000000000011, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00 };


            double[,] aXL = pls.Predictors.FactorMatrix.ToMatrix();
            double[,] aYL = pls.Dependents.FactorMatrix.ToMatrix();
            double[,] aXS = pls.Predictors.Result;
            double[,] aYS = pls.Dependents.Result;
            double[][] aW = pls.Weights;

            double[,] aB = regression.Coefficients;
            double[] aC = regression.Intercepts;



            for (int i = 0; i < eProportionsX.Length; i++)
            {
                Assert.AreEqual(eProportionsX[i], pls.Predictors.FactorProportions[i], 0.01);
                Assert.AreEqual(eProportionsY[i], pls.Dependents.FactorProportions[i], 0.01);
            }



            for (int i = 0; i < eYL.GetLength(0); i++)
                for (int j = 0; j < eYL.GetLength(1); j++)
                    Assert.AreEqual(aYL[i, j], eYL[i, j], 0.01);

            for (int i = 0; i < eYS.GetLength(0); i++)
                for (int j = 0; j < eYS.GetLength(1); j++)
                    Assert.AreEqual(aYS[i, j], eYS[i, j], 0.01);

        }


        [Test]
        public void VariableImportanceTest()
        {
            double[,] X =
            {
                { 2.5, 2.4 },
                { 0.5, 0.7 },
                { 2.2, 2.9 },
                { 1.9, 2.2 },
                { 3.1, 3.0 },
                { 2.3, 2.7 },
                { 2.0, 1.6 },
                { 1.0, 1.1 },
                { 1.5, 1.6 },
                { 1.1, 0.9 },
            };

            double[,] Y =
            {
                { 1 },
                { 0 },
                { 1 },
                { 0 },
                { 1 },
                { 1 },
                { 0 },
                { 0 },
                { 0 },
                { 0 },
            };


            var pls = new PartialLeastSquaresAnalysis(X, Y, AnalysisMethod.Center,
                PartialLeastSquaresAlgorithm.NIPALS);


            pls.Compute(1);

            double[][] actual1 = pls.Importance;
            double[] actual1v = pls.Factors[0].VariableImportance;
            double[] expected1v = { 0.9570761, 1.041156 };
            Assert.IsTrue(Matrix.IsEqual(actual1v, expected1v, 0.0001));



            pls.Compute(2);

            double[] actual2v1 = pls.Factors[0].VariableImportance;
            double[] expected2v1 = { 0.9570761, 1.041156 };
            Assert.IsTrue(Matrix.IsEqual(actual2v1, expected2v1, 0.0001));

            double[] actual2v2 = pls.Factors[1].VariableImportance;
            double[] expected2v2 = { 1.0187709, 0.980870 };
            Assert.IsTrue(Matrix.IsEqual(actual2v2, expected2v2, 0.0001));

            double[][] actual2 = pls.Importance;
            double[,] expected2 = new double[,]
            {
                { 0.9570761, 1.0187709 },
                { 1.041156, 0.980870 }
            };

            Assert.IsTrue(Matrix.IsEqual(actual2, expected2, 0.0001));

        }

        [Test]
        public void ExceptionTest()
        {
            double[,] X =
            {
                { 2.5, 2.4, 0 },
                { 0.5, 0.7, 0 },
                { 2.2, 2.9, 0 },
                { 1.9, 2.2, 0 },
                { 3.1, 3.0, 0 },
                { 2.3, 2.7, 0 },
                { 2.0, 1.6, 0 },
                { 1.0, 1.1, 0 },
                { 1.5, 1.6, 0 },
                { 1.1, 0.9, 0 },
            };

            double[,] Y =
            {
                { 1 },
                { 0 },
                { 1 },
                { 0 },
                { 1 },
                { 1 },
                { 0 },
                { 0 },
                { 0 },
                { 0 },
            };


            var pls = new PartialLeastSquaresAnalysis(X, Y, AnalysisMethod.Standardize,
                PartialLeastSquaresAlgorithm.NIPALS);

            bool thrown = false;

            try
            {
                pls.Compute();
            }
            catch (ArithmeticException ex)
            {
                ex.ToString();
                thrown = true;
            }

            // Assert that an appropriate exception has been
            //   thrown in the case of a constant variable.
            Assert.IsTrue(thrown);
        }

        [Test]
        public void factor_exception_test()
        {
            // https://github.com/accord-net/framework/issues/332

            double[,] x = { { 1.05, 1.96, 3.02 }, { 3.94, 4.98, 6.04 }, { 5, 6, 7 } };
            double[,] y = { { 11 }, { 13 }, { 13.68 } };

            {
                PartialLeastSquaresAnalysis pls = new PartialLeastSquaresAnalysis(x, y, PartialLeastSquaresAlgorithm.SIMPLS);
                int factors = 1;
                pls.Compute(factors);
                Assert.AreEqual(1, pls.Factors.Count);
            }

            {
                PartialLeastSquaresAnalysis pls = new PartialLeastSquaresAnalysis(x, y, PartialLeastSquaresAlgorithm.NIPALS);
                pls.Compute();
                Assert.AreEqual(2, pls.Factors.Count);
            }

            {
                PartialLeastSquaresAnalysis pls = new PartialLeastSquaresAnalysis(x, y, PartialLeastSquaresAlgorithm.NIPALS);
                int factors = 1;
                pls.Compute(factors);
                Assert.AreEqual(1, pls.Factors.Count);
            }
        }

        [Test]
        public void matlab_herve_example()
        {
            // Example from https://stackoverflow.com/questions/40685649/pls-regression-coefficients-in-matlab-and-c-sharp-accord-net

            double[][] inputs = new double[][]
            {
                //      Wine | Price | Sugar | Alcohol | Acidity
                new double[] {   7,     7,      13,        7 },
                new double[] {   4,     3,      14,        7 },
                new double[] {  10,     5,      12,        5 },
                new double[] {  16,     7,      11,        3 },
                new double[] {  13,     3,      10,        3 },
            };

            double[][] outputs = new double[][]
            {
                //             Wine | Hedonic | Goes with meat | Goes with dessert
                new double[] {           14,          7,                 8 },
                new double[] {           10,          7,                 6 },
                new double[] {            8,          5,                 5 },
                new double[] {            2,          4,                 7 },
                new double[] {            6,          2,                 4 },
            };

            // Create the Partial Least Squares Analysis
            var pls = new PartialLeastSquaresAnalysis()
            {
                Method = AnalysisMethod.Center,
                Algorithm = PartialLeastSquaresAlgorithm.SIMPLS,
            };

            // Learn the analysis
            var regression = pls.Learn(inputs, outputs);

            // Create a regression with just 1 component
            var regression1 = pls.CreateRegression(factors: 1);

            // Get the matrix of weights and intercept
            double[][] w = regression1.Weights.Transpose();
            double[] b = regression1.Intercepts;
            double[][] coeffs = (w.InsertColumn(b, index: 0)).Transpose();

            // Show results in Octave format
            string str = coeffs.ToOctave();

            double[][] expected =
            {
                new[] { 1.0484e+01 ,  6.1899e+00,   6.2841e+00 },
                new[] { -6.3488e-01, -3.0405e-01, -7.2608e-02 },
                new[] { 2.1949e-02 ,  1.0512e-02,   2.5102e-03 },
                new[] { 1.9226e-01 ,  9.2078e-02,   2.1988e-02 },
                new[] { 2.8948e-01 ,  1.3864e-01,   3.3107e-02 },
            };

            Assert.IsTrue(expected.IsEqual(coeffs, 1e-3));

            pls = new PartialLeastSquaresAnalysis()
            {
                Method = AnalysisMethod.Center,
                Algorithm = PartialLeastSquaresAlgorithm.SIMPLS,
                NumberOfLatentFactors = 1
            };

            var regression2 = pls.Learn(inputs, outputs);

            Assert.IsTrue(regression1.Weights.IsEqual(regression2.Weights));
            Assert.IsTrue(regression1.Intercepts.IsEqual(regression2.Intercepts));

            Assert.Throws<InvalidOperationException>(() => pls.NumberOfOutputs = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => pls.NumberOfLatentFactors = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => pls.NumberOfLatentFactors = 5);

            pls.NumberOfLatentFactors = 4;

            var regression3 = pls.CreateRegression();
            Assert.IsTrue(regression.Weights.IsEqual(regression3.Weights));
            Assert.IsTrue(regression.Intercepts.IsEqual(regression3.Intercepts));

            Assert.IsFalse(regression1.Weights.IsEqual(regression3.Weights));
            Assert.IsFalse(regression1.Intercepts.IsEqual(regression3.Intercepts));
        }
    }
}
