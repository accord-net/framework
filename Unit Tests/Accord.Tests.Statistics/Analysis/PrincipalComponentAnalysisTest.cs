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
    using Accord.Statistics;
    using NUnit.Framework;
    using Accord.Math;
    using System;
    using Accord.Statistics.Models.Regression.Linear;
    using System.IO;
    using IO;

    [TestFixture]
    public class PrincipalComponentAnalysisTest
    {

        // Lindsay's tutorial data
        private static double[,] data =
        {
            { 2.5,  2.4 },
            { 0.5,  0.7 },
            { 2.2,  2.9 },
            { 1.9,  2.2 },
            { 3.1,  3.0 },
            { 2.3,  2.7 },
            { 2.0,  1.6 },
            { 1.0,  1.1 },
            { 1.5,  1.6 },
            { 1.1,  0.9 }
        };

        [Test]
        public void ConstructorTest()
        {
            // Reproducing Lindsay Smith's "Tutorial on Principal Component Analysis"
            // using the framework's default method. The tutorial can be found online
            // at http://www.sccg.sk/~haladova/principal_components.pdf

            // Step 1. Get some data
            // ---------------------

            double[,] data =
            {
                { 2.5,  2.4 },
                { 0.5,  0.7 },
                { 2.2,  2.9 },
                { 1.9,  2.2 },
                { 3.1,  3.0 },
                { 2.3,  2.7 },
                { 2.0,  1.6 },
                { 1.0,  1.1 },
                { 1.5,  1.6 },
                { 1.1,  0.9 }
            };


            // Step 2. Subtract the mean
            // -------------------------
            //   Note: The framework does this automatically. By default, the framework
            //   uses the "Center" method, which only subtracts the mean. However, it is
            //   also possible to remove the mean *and* divide by the standard deviation
            //   (thus performing the correlation method) by specifying "Standardize"
            //   instead of "Center" as the AnalysisMethod.

            AnalysisMethod method = AnalysisMethod.Center; // AnalysisMethod.Standardize


            // Step 3. Compute the covariance matrix
            // -------------------------------------
            //   Note: Accord.NET does not need to compute the covariance
            //   matrix in order to compute PCA. The framework uses the SVD
            //   method which is more numerically stable, but may require
            //   more processing or memory. In order to replicate the tutorial
            //   using covariance matrices, please see the next unit test.

            // Create the analysis using the selected method
            var pca = new PrincipalComponentAnalysis(data, method);

            // Compute it
            pca.Compute();


            // Step 4. Compute the eigenvectors and eigenvalues of the covariance matrix
            // -------------------------------------------------------------------------
            //   Note: Since Accord.NET uses the SVD method rather than the Eigendecomposition
            //   method, the Eigenvalues are computed from the singular values. However, it is
            //   not the Eigenvalues themselves which are important, but rather their proportion:

            // Those are the expected eigenvalues, in descending order:
            double[] eigenvalues = { 1.28402771, 0.0490833989 };

            // And this will be their proportion:
            double[] proportion = eigenvalues.Divide(eigenvalues.Sum());

            // Those are the expected eigenvectors,
            // in descending order of eigenvalues:
            double[,] eigenvectors =
            {
                { -0.677873399, -0.735178656 },
                { -0.735178656,  0.677873399 }
            };

            // Now, here is the place most users get confused. The fact is that
            // the Eigenvalue decomposition (EVD) is not unique, and both the SVD
            // and EVD routines used by the framework produces results which are
            // numerically different from packages such as STATA or MATLAB, but
            // those are correct.

            // If v is an eigenvector, a multiple of this eigenvector (such as a*v, with
            // a being a scalar) will also be an eigenvector. In the Lindsay case, the
            // framework produces a first eigenvector with inverted signs. This is the same
            // as considering a=-1 and taking a*v. The result is still correct.

            // Retrieve the first expected eigenvector
            double[] v = eigenvectors.GetColumn(0);

            // Multiply by a scalar and store it back
            eigenvectors.SetColumn(0, v.Multiply(-1));

            // Everything is alright (up to the 9 decimal places shown in the tutorial)
            Assert.IsTrue(eigenvectors.IsEqual(pca.ComponentMatrix, rtol: 1e-9));
            Assert.IsTrue(proportion.IsEqual(pca.ComponentProportions, rtol: 1e-9));
            Assert.IsTrue(eigenvalues.IsEqual(pca.Eigenvalues, rtol: 1e-5));

            // Step 5. Deriving the new data set
            // ---------------------------------

            double[,] actual = pca.Transform(data);

            // transformedData shown in pg. 18
            double[,] expected = new double[,]
            {
                {  0.827970186, -0.175115307 },
                { -1.77758033,   0.142857227 },
                {  0.992197494,  0.384374989 },
                {  0.274210416,  0.130417207 },
                {  1.67580142,  -0.209498461 },
                {  0.912949103,  0.175282444 },
                { -0.099109437, -0.349824698 },
                { -1.14457216,   0.046417258 },
                { -0.438046137,  0.017764629 },
                { -1.22382056,  -0.162675287 },
            };

            // Everything is correct (up to 8 decimal places)
            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));
        }

        [Test]
        public void learn_success()
        {
            #region doc_learn_2
            // Reproducing Lindsay Smith's "Tutorial on Principal Component Analysis"
            // using the framework's default method. The tutorial can be found online
            // at http://www.sccg.sk/~haladova/principal_components.pdf

            // Step 1. Get some data
            // ---------------------

            double[][] data =
            {
                new[] { 2.5,  2.4 },
                new[] { 0.5,  0.7 },
                new[] { 2.2,  2.9 },
                new[] { 1.9,  2.2 },
                new[] { 3.1,  3.0 },
                new[] { 2.3,  2.7 },
                new[] { 2.0,  1.6 },
                new[] { 1.0,  1.1 },
                new[] { 1.5,  1.6 },
                new[] { 1.1,  0.9 }
            };


            // Step 2. Subtract the mean
            // -------------------------
            //   Note: The framework does this automatically. By default, the framework
            //   uses the "Center" method, which only subtracts the mean. However, it is
            //   also possible to remove the mean *and* divide by the standard deviation
            //   (thus performing the correlation method) by specifying "Standardize"
            //   instead of "Center" as the AnalysisMethod.

            var method = PrincipalComponentMethod.Center;
            // var method = PrincipalComponentMethod.Standardize


            // Step 3. Compute the covariance matrix
            // -------------------------------------
            //   Note: Accord.NET does not need to compute the covariance
            //   matrix in order to compute PCA. The framework uses the SVD
            //   method which is more numerically stable, but may require
            //   more processing or memory. In order to replicate the tutorial
            //   using covariance matrices, please see the next unit test.

            // Create the analysis using the selected method
            var pca = new PrincipalComponentAnalysis(method);

            // Compute it
            pca.Learn(data);


            // Step 4. Compute the eigenvectors and eigenvalues of the covariance matrix
            // -------------------------------------------------------------------------
            //   Note: Since Accord.NET uses the SVD method rather than the Eigendecomposition
            //   method, the Eigenvalues are computed from the singular values. However, it is
            //   not the Eigenvalues themselves which are important, but rather their proportion:

            // Those are the expected eigenvalues, in descending order:
            double[] eigenvalues = { 1.28402771, 0.0490833989 };

            // And this will be their proportion:
            double[] proportion = eigenvalues.Divide(eigenvalues.Sum());

            // Those are the expected eigenvectors,
            // in descending order of eigenvalues:
            double[,] eigenvectors =
            {
                { -0.677873399, -0.735178656 },
                { -0.735178656,  0.677873399 }
            };

            // Now, here is the place most users get confused. The fact is that
            // the Eigenvalue decomposition (EVD) is not unique, and both the SVD
            // and EVD routines used by the framework produces results which are
            // numerically different from packages such as STATA or MATLAB, but
            // those are correct.

            // If v is an eigenvector, a multiple of this eigenvector (such as a*v, with
            // a being a scalar) will also be an eigenvector. In the Lindsay case, the
            // framework produces a first eigenvector with inverted signs. This is the same
            // as considering a=-1 and taking a*v. The result is still correct.

            // Retrieve the first expected eigenvector
            double[] v = eigenvectors.GetColumn(0);

            // Multiply by a scalar and store it back
            eigenvectors.SetColumn(0, v.Multiply(-1));

            // Everything is alright (up to the 9 decimal places shown in the tutorial)
            Assert.IsTrue(eigenvectors.IsEqual(pca.ComponentMatrix, rtol: 1e-9));
            Assert.IsTrue(proportion.IsEqual(pca.ComponentProportions, rtol: 1e-9));
            Assert.IsTrue(eigenvalues.IsEqual(pca.Eigenvalues, rtol: 1e-5));

            // Step 5. Deriving the new data set
            // ---------------------------------

            double[][] actual = pca.Transform(data);

            // transformedData shown in pg. 18
            double[,] expected = new double[,]
            {
                {  0.827970186, -0.175115307 },
                { -1.77758033,   0.142857227 },
                {  0.992197494,  0.384374989 },
                {  0.274210416,  0.130417207 },
                {  1.67580142,  -0.209498461 },
                {  0.912949103,  0.175282444 },
                { -0.099109437, -0.349824698 },
                { -1.14457216,   0.046417258 },
                { -0.438046137,  0.017764629 },
                { -1.22382056,  -0.162675287 },
            };

            // Everything is correct (up to 8 decimal places)
            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));

            // Let's say we would like to project down to one 
            // principal component. It suffices to set:
            pca.NumberOfOutputs = 1;

            // And then do the transform
            actual = pca.Transform(data);

            // transformedData shown in pg. 18
            expected = new double[,]
            {
                {  0.827970186 },
                { -1.77758033, },
                {  0.992197494 },
                {  0.274210416 },
                {  1.67580142, },
                {  0.912949103 },
                { -0.099109437 },
                { -1.14457216, },
                { -0.438046137 },
                { -1.22382056, },
            };

            // Everything is correct (up to 8 decimal places)
            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));
            #endregion

            // Create the analysis using the selected method
            pca = new PrincipalComponentAnalysis(method, numberOfOutputs: 1);

            // Compute it
            pca.Learn(data);

            actual = pca.Transform(data);

            // transformedData shown in pg. 18
            expected = new double[,]
            {
                {  0.827970186 },
                { -1.77758033, },
                {  0.992197494 },
                {  0.274210416 },
                {  1.67580142, },
                {  0.912949103 },
                { -0.099109437 },
                { -1.14457216, },
                { -0.438046137 },
                { -1.22382056, },
            };

            // Everything is correct (up to 8 decimal places)
            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));
        }

        [Test]
        public void learn_weights()
        {
            double[][] raw =
            {
                new[] { 2.5,  2.4, 1 },
                new[] { 0.5,  0.7, 1 },
                new[] { 2.2,  2.9, 0.5 },
                new[] { 2.2,  2.9, 0.5 },
                new[] { 1.9,  2.2, 1 },
                new[] { 3.1,  3.0, 1 },
                new[] { 2.3,  2.7, 1 },
                new[] { 2.0,  1.6, 1 },
                new[] { 1.0,  1.1, 0.25 },
                new[] { 1.0,  1.1, 0.25 },
                new[] { 1.0,  1.1, 0.25 },
                new[] { 1.0,  1.1, 0.25 },
                new[] { 1.5,  1.6, 1 },
                new[] { 42.5,  7.6, 0 },
                new[] { 743.5,  5.6, 0 },
                new[] { 1.5,  16, 0 },
                new[] { 1.1,  0.9, 1 }
            };

            double[][] data = raw.GetColumns(0, 1);
            double[] weights = raw.GetColumn(2);

            var method = PrincipalComponentMethod.Center;
            var pca = new PrincipalComponentAnalysis(method);

            pca.Learn(data, weights);

            double[] eigenvalues = { 1.28402771, 0.0490833989 };

            double[] proportion = eigenvalues.Divide(eigenvalues.Sum());

            double[,] eigenvectors =
            {
                { -0.677873399, -0.735178656 },
                { -0.735178656,  0.677873399 }
            };

            double[] v = eigenvectors.GetColumn(0);
            eigenvectors.SetColumn(0, v.Multiply(-1)); 

            Assert.IsTrue(eigenvectors.IsEqual(pca.ComponentMatrix, rtol: 1e-9));
            Assert.IsTrue(proportion.IsEqual(pca.ComponentProportions, rtol: 1e-9));
            Assert.IsTrue(eigenvalues.IsEqual(pca.Eigenvalues, rtol: 0.1));

            double[][] actual = pca.Transform(data);
            string a = actual.ToCSharp();

            /*
             double[,] expected = new double[,]
             {
                {  0.827970186, -0.175115307 },
                { -1.77758033,   0.142857227 },
                {  0.992197494,  0.384374989 },
                {  0.274210416,  0.130417207 },
                {  1.67580142,  -0.209498461 },
                {  0.912949103,  0.175282444 },
                { -0.099109437, -0.349824698 },
                { -1.14457216,   0.046417258 },
                { -0.438046137,  0.017764629 },
                { -1.22382056,  -0.162675287 },
            };
            */

            double[][] expected = 
            {
                new double[] { 0.827970186201088, -0.175115307046916 },
                new double[] { -1.77758032528043, 0.142857226544281 },
                new double[] { 0.992197494414889, 0.384374988880413 }, // weight is 0.5
                new double[] { 0.992197494414889, 0.384374988880413 }, // weight is 0.5
                new double[] { 0.2742104159754, 0.130417206574127 },
                new double[] { 1.67580141864454, -0.209498461256753 },
                new double[] { 0.912949103158809, 0.17528244362037 },
                new double[] { -0.0991094374984439, -0.349824698097121 },
                new double[] { -1.14457216379866, 0.0464172581832816 }, // weight is 0.25
                new double[] { -1.14457216379866, 0.0464172581832816 }, // weight is 0.25
                new double[] { -1.14457216379866, 0.0464172581832816 }, // weight is 0.25
                new double[] { -1.14457216379866, 0.0464172581832816 }, // weight is 0.25
                new double[] { -0.43804613676245, 0.0177646296750834 },
                new double[] { 31.7658351361525, -26.0573198564776 }, // weight is 0
                new double[] { 505.4847301932, -542.773304190164 },   // weight is 0
                new double[] { 10.148526503077, 9.77914156847845 },   // weight is 0
                new double[] { -1.22382055505474, -0.162675287076762 }
            };

            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));
        }

        [Test]
        public void learn_whiten_success()
        {
            #region doc_learn_1
            // Below is the same data used on the excellent paper "Tutorial
            //   On Principal Component Analysis", by Lindsay Smith (2002).
            double[][] data =
            {
                new double[] { 2.5,  2.4 },
                new double[] { 0.5,  0.7 },
                new double[] { 2.2,  2.9 },
                new double[] { 1.9,  2.2 },
                new double[] { 3.1,  3.0 },
                new double[] { 2.3,  2.7 },
                new double[] { 2.0,  1.6 },
                new double[] { 1.0,  1.1 },
                new double[] { 1.5,  1.6 },
                new double[] { 1.1,  0.9 }
            };

            // Let's create an analysis with centering (covariance method)
            // but no standardization (correlation method) and whitening:
            var pca = new PrincipalComponentAnalysis()
            {
                Method = PrincipalComponentMethod.Center,
                Whiten = true
            };

            // Now we can learn the linear projection from the data
            MultivariateLinearRegression transform = pca.Learn(data);

            // Finally, we can project all the data
            double[][] output1 = pca.Transform(data);

            // Or just its first components by setting 
            // NumberOfOutputs to the desired components:
            pca.NumberOfOutputs = 1;

            // And then calling transform again:
            double[][] output2 = pca.Transform(data);

            // We can also limit to 80% of explained variance:
            pca.ExplainedVariance = 0.8;

            // And then call transform again:
            double[][] output3 = pca.Transform(data);
            #endregion

            double[] eigenvalues = { 1.28402771, 0.0490833989 };
            double[] proportion = eigenvalues.Divide(eigenvalues.Sum());
            double[,] eigenvectors =
            {
                { 0.19940687993951403, -1.1061252858739095 },
                { 0.21626410214440508,  1.0199057073792104 }
            };

            // Everything is alright (up to the 9 decimal places shown in the tutorial)
            Assert.IsTrue(eigenvectors.IsEqual(pca.ComponentMatrix, rtol: 1e-9));
            Assert.IsTrue(proportion.IsEqual(pca.ComponentProportions, rtol: 1e-9));
            Assert.IsTrue(eigenvalues.IsEqual(pca.Eigenvalues, rtol: 1e-5));

            pca.ExplainedVariance = 1.0;
            double[][] actual = pca.Transform(data);

            double[][] expected =
            {
                new double[] {  0.243560157209023,  -0.263472650637184  },
                new double[] { -0.522902576315494,   0.214938218565977  },
                new double[] {  0.291870144299372,   0.578317788814594  },
                new double[] {  0.0806632088164338,  0.19622137941132   },
                new double[] {  0.492962746459375,  -0.315204397734004  },
                new double[] {  0.268558011864442,   0.263724118751361  },
                new double[] { -0.0291545644762578, -0.526334573603598  },
                new double[] { -0.336693495487974,   0.0698378585807067 },
                new double[] { -0.128858004446015,   0.0267280693333571 },
                new double[] { -0.360005627922904,  -0.244755811482527  }
            };

            // var str = actual.ToString(CSharpJaggedMatrixFormatProvider.InvariantCulture);

            // Everything is correct (up to 8 decimal places)
            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));
            Assert.IsTrue(expected.IsEqual(output1, atol: 1e-8));
            Assert.IsTrue(expected.Get(null, 0, 1).IsEqual(output2, atol: 1e-8));
            Assert.IsTrue(expected.Get(null, 0, 1).IsEqual(output3, atol: 1e-8));

            actual = transform.Transform(data);
            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));
        }



        [Test]
        public void learn_standardize()
        {
            double[][] data =
            {
                new double[] { 2.5,  2.4 },
                new double[] { 0.5,  0.7 },
                new double[] { 2.2,  2.9 },
                new double[] { 1.9,  2.2 },
                new double[] { 3.1,  3.0 },
                new double[] { 2.3,  2.7 },
                new double[] { 2.0,  1.6 },
                new double[] { 1.0,  1.1 },
                new double[] { 1.5,  1.6 },
                new double[] { 1.1,  0.9 }
            };

            var pca = new PrincipalComponentAnalysis()
            {
                Method = PrincipalComponentMethod.Standardize,
                Whiten = false
            };

            MultivariateLinearRegression transform = pca.Learn(data);

            double[][] output1 = pca.Transform(data);

            double[] eigenvalues = { 1.925929272692245, 0.074070727307754519 };
            double[] proportion = eigenvalues.Divide(eigenvalues.Sum());
            double[,] eigenvectors =
            {
                { 0.70710678118654791, -0.70710678118654791 },
                { 0.70710678118654791,  0.70710678118654791 }
            };

            Assert.IsTrue(eigenvectors.IsEqual(pca.ComponentMatrix, rtol: 1e-9));
            Assert.IsTrue(proportion.IsEqual(pca.ComponentProportions, rtol: 1e-9));
            Assert.IsTrue(eigenvalues.IsEqual(pca.Eigenvalues, rtol: 1e-5));

            pca.ExplainedVariance = 1.0;
            double[][] actual = pca.Transform(data);
      //      var str = actual.ToCSharp();
            double[][] expected =
           {
                new double[] { 1.03068028963519, -0.212053139513466 },
                new double[] { -2.19045015647317, 0.168942295968493 },
                new double[] { 1.17818776184333, 0.47577321493322 },
                new double[] { 0.323294642065681, 0.161198977394117 },
                new double[] { 2.07219946786664, -0.251171725759119 },
                new double[] { 1.10117414355213, 0.218653302562498 },
                new double[] { -0.0878525068874546, -0.430054465638535 },
                new double[] { -1.40605089061245, 0.0528100914316325 },
                new double[] { -0.538118242086245, 0.0202112695602547 },
                new double[] { -1.48306450890365, -0.204309820939091 }
            };

            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));
            Assert.IsTrue(expected.IsEqual(output1, atol: 1e-8));

            actual = transform.Transform(data);
            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));
        }


        [Test]
        public void ConstructorTest2()
        {
            // Reproducing Lindsay Smith's "Tutorial on Principal Component Analysis"
            // using the paper's original method. The tutorial can be found online
            // at http://www.sccg.sk/~haladova/principal_components.pdf

            // Step 1. Get some data
            // ---------------------

            double[,] data =
            {
                { 2.5,  2.4 },
                { 0.5,  0.7 },
                { 2.2,  2.9 },
                { 1.9,  2.2 },
                { 3.1,  3.0 },
                { 2.3,  2.7 },
                { 2.0,  1.6 },
                { 1.0,  1.1 },
                { 1.5,  1.6 },
                { 1.1,  0.9 }
            };


            // Step 2. Subtract the mean
            // -------------------------
            //   Note: The framework does this automatically 
            //   when computing the covariance matrix. In this
            //   step we will only compute the mean vector.

            double[] mean = Measures.Mean(data, dimension: 0);


            // Step 3. Compute the covariance matrix
            // -------------------------------------

            double[,] covariance = Measures.Covariance(data, mean);

            // Create the analysis using the covariance matrix
            var pca = PrincipalComponentAnalysis.FromCovarianceMatrix(mean, covariance);

            // Compute it
            pca.Compute();


            // Step 4. Compute the eigenvectors and eigenvalues of the covariance matrix
            //--------------------------------------------------------------------------

            // Those are the expected eigenvalues, in descending order:
            double[] eigenvalues = { 1.28402771, 0.0490833989 };

            // And this will be their proportion:
            double[] proportion = eigenvalues.Divide(eigenvalues.Sum());

            // Those are the expected eigenvectors,
            // in descending order of eigenvalues:
            double[,] eigenvectors =
            {
                { -0.677873399, -0.735178656 },
                { -0.735178656,  0.677873399 }
            };

            // Now, here is the place most users get confused. The fact is that
            // the Eigenvalue decomposition (EVD) is not unique, and both the SVD
            // and EVD routines used by the framework produces results which are
            // numerically different from packages such as STATA or MATLAB, but
            // those are correct.

            // If v is an eigenvector, a multiple of this eigenvector (such as a*v, with
            // a being a scalar) will also be an eigenvector. In the Lindsay case, the
            // framework produces a first eigenvector with inverted signs. This is the same
            // as considering a=-1 and taking a*v. The result is still correct.

            // Retrieve the first expected eigenvector
            double[] v = eigenvectors.GetColumn(0);

            // Multiply by a scalar and store it back
            eigenvectors.SetColumn(0, v.Multiply(-1));

            // Everything is alright (up to the 9 decimal places shown in the tutorial)
            Assert.IsTrue(eigenvectors.IsEqual(pca.ComponentMatrix, rtol: 1e-9));
            Assert.IsTrue(proportion.IsEqual(pca.ComponentProportions, rtol: 1e-9));
            Assert.IsTrue(eigenvalues.IsEqual(pca.Eigenvalues, rtol: 1e-8));


            // Step 5. Deriving the new data set
            // ---------------------------------

            double[,] actual = pca.Transform(data);

            // transformedData shown in pg. 18
            double[,] expected = new double[,]
            {
                {  0.827970186, -0.175115307 },
                { -1.77758033,   0.142857227 },
                {  0.992197494,  0.384374989 },
                {  0.274210416,  0.130417207 },
                {  1.67580142,  -0.209498461 },
                {  0.912949103,  0.175282444 },
                { -0.099109437, -0.349824698 },
                { -1.14457216,   0.046417258 },
                { -0.438046137,  0.017764629 },
                { -1.22382056,  -0.162675287 },
            };

            // Everything is correct (up to 8 decimal places)
            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-8));
        }

        [Test]
        public void TransformTest()
        {
            PrincipalComponentAnalysis target = new PrincipalComponentAnalysis(data);

            // Compute
            target.Compute();

            // Transform
            double[,] actual = target.Transform(data);

            // first inversed.. ?
            double[,] expected = new double[,]
            {
                {  0.827970186, -0.175115307 },
                { -1.77758033,   0.142857227 },
                {  0.992197494,  0.384374989 },
                {  0.274210416,  0.130417207 },
                {  1.67580142,  -0.209498461 },
                {  0.912949103,  0.175282444 },
                { -0.099109437, -0.349824698 },
                { -1.14457216,   0.046417258 },
                { -0.438046137,  0.017764629 },
                { -1.22382056,  -0.162675287 },
            };

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.01));

            // Assert the scores equals the transformation of the input
            double[,] result = target.Result;
            Assert.IsTrue(Matrix.IsEqual(result, expected, 0.01));
        }

        [Test]
        public void transform_more_columns_than_samples()
        {
            // Lindsay's tutorial data
            double[,] datat = data.Transpose();

            var target = new PrincipalComponentAnalysis(datat);

            // Compute
            target.Compute();

            // Transform
            double[,] actual = target.Transform(datat);

            // Assert the scores equals the transformation of the input

            double[,] result = target.Result;
            Assert.IsTrue(Matrix.IsEqual(result, actual, 0.01));
            Assert.AreEqual(2, result.Rows());
            Assert.AreEqual(2, result.Columns());
            Assert.IsTrue(result.IsSquare());
        }

        [Test]
        public void transform_more_columns_than_samples_new_interface()
        {
            // Lindsay's tutorial data
            var datat = data.Transpose().ToJagged();

            var target = new PrincipalComponentAnalysis();

            // Compute
            var regression = target.Learn(datat);

            // Transform
            double[][] actual = target.Transform(datat);

            // Assert the scores equals the transformation of the input
            Assert.IsNull(target.Result);

            double[,] expected =
            {
                {  0.50497524691810358, -0.00000000000000044408920985006262 },
                { -0.504975246918104,   -0.00000000000000035735303605122226 }
            };

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.01));

            actual = target.Transform(datat);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.01));
        }

        [Test]
        public void TransformTest3()
        {
            PrincipalComponentAnalysis target = new PrincipalComponentAnalysis(data);

            // Compute
            target.Compute();

            bool thrown = false;
            try
            {
                double[,] actual = target.Transform(data, 3);
            }
            catch { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [Test]
        public void covariance_success()
        {
            double[] mean = Measures.Mean(data, dimension: 0);
            double[,] cov = Measures.Covariance(data);

            var target = PrincipalComponentAnalysis.FromCovarianceMatrix(mean, cov);

            // Compute
            target.Compute();

            // Transform
            double[,] actual = target.Transform(data);

            double[,] expected = new double[,]
            {
                {  0.827970186, -0.175115307 },
                { -1.77758033,   0.142857227 },
                {  0.992197494,  0.384374989 },
                {  0.274210416,  0.130417207 },
                {  1.67580142,  -0.209498461 },
                {  0.912949103,  0.175282444 },
                { -0.099109437, -0.349824698 },
                { -1.14457216,   0.046417258 },
                { -0.438046137,  0.017764629 },
                { -1.22382056,  -0.162675287 },
            };

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.01));

            // Transform
            double[,] image = target.Transform(data);

            // Reverse
            double[,] reverse = target.Revert(image);

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(reverse, data, 0.01));
        }

        [Test]
        public void correlation_success()
        {
            double[] mean = Measures.Mean(data, dimension: 0);
            double[] stdDev = Measures.StandardDeviation(data);
            double[,] cov = Measures.Correlation(data);

            var actual = PrincipalComponentAnalysis.FromCorrelationMatrix(mean, stdDev, cov);
            var expected = new PrincipalComponentAnalysis(data, AnalysisMethod.Standardize);

            // Compute
            actual.Compute();
            expected.Compute();

            // Transform
            double[,] actualTransform = actual.Transform(data);
            double[,] expectedTransform = expected.Transform(data);


            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actualTransform, expectedTransform, 0.01));

            // Transform
            double[,] image = actual.Transform(data);
            double[,] reverse = actual.Revert(image);

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(reverse, data, 0.01));
        }


        [Test]
        public void correlation_new_interface()
        {
            double[] mean = Measures.Mean(data, dimension: 0);
            double[] stdDev = Measures.StandardDeviation(data);
            double[][] cov = Measures.Correlation(data.ToJagged());

            var actual = PrincipalComponentAnalysis.FromCorrelationMatrix(mean, stdDev, cov.ToMatrix());
            var expected = new PrincipalComponentAnalysis(PrincipalComponentMethod.CorrelationMatrix)
            {
                Means = mean,
                StandardDeviations = stdDev
            };

            // Compute
            actual.Compute();
            var transform = expected.Learn(cov);

            // Transform
            double[,] actualTransform = actual.Transform(data);
            double[,] expectedTransform = expected.Transform(data);

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actualTransform, expectedTransform, 0.01));

            // Transform
            double[,] image = actual.Transform(data);
            double[,] reverse = actual.Revert(image);

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(reverse, data, 1e-6));

            // Transform
            double[][] image2 = transform.Transform(data.ToJagged());
            double[][] reverse2 = transform.Inverse().Transform(image2);
            Assert.IsTrue(Matrix.IsEqual(reverse, reverse2, 1e-6));
            Assert.IsTrue(Matrix.IsEqual(reverse2, data, 1e-6));

            // Transform
            double[][] reverse3 = actual.Revert(image2);
            Assert.IsTrue(Matrix.IsEqual(reverse, reverse3, 1e-6));
            Assert.IsTrue(Matrix.IsEqual(reverse3, data, 1e-6));

            var a = transform.Transform(data.ToJagged()).ToMatrix();
            Assert.IsTrue(Matrix.IsEqual(a, expectedTransform, 0.01));
        }

        [Test]
        public void covariance_new_interface()
        {
            double[] mean = Measures.Mean(data, dimension: 0);
            double[][] cov = Measures.Covariance(data.ToJagged());

            #region doc_learn_3
            // Create the Principal Component Analysis 
            // specifying the CovarianceMatrix method:
            var pca = new PrincipalComponentAnalysis()
            {
                Method = PrincipalComponentMethod.CovarianceMatrix,
                Means = mean // pass the original data mean vectors
            };

            // Learn the PCA projection using passing the cov matrix
            MultivariateLinearRegression transform = pca.Learn(cov);

            // Now, we can transform data as usual
            double[,] actual = pca.Transform(data);
            #endregion

            double[,] expected = new double[,]
            {
                {  0.827970186, -0.175115307 },
                { -1.77758033,   0.142857227 },
                {  0.992197494,  0.384374989 },
                {  0.274210416,  0.130417207 },
                {  1.67580142,  -0.209498461 },
                {  0.912949103,  0.175282444 },
                { -0.099109437, -0.349824698 },
                { -1.14457216,   0.046417258 },
                { -0.438046137,  0.017764629 },
                { -1.22382056,  -0.162675287 },
            };

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.01));

            // Transform
            double[,] image = pca.Transform(data);

            // Reverse
            double[,] reverse = pca.Revert(image);

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(reverse, data, 1e-5));

            actual = transform.Transform(data.ToJagged()).ToMatrix();
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 1e-5));
        }

        [Test]
        public void Revert()
        {
            var target = new PrincipalComponentAnalysis(data);

            // Compute
            target.Compute();

            // Transform
            double[,] image = target.Transform(data);

            // Reverse
            double[,] actual = target.Revert(image);

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual, data, 0.01));
        }

        [Test]
        public void Revert_new_method()
        {
            var target = new PrincipalComponentAnalysis();

            // Compute
            var transform = target.Learn(data.ToJagged());

            // Transform
            double[][] image = target.Transform(data.ToJagged());

            // Reverse
            double[][] actual = target.Revert(image);

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual, data, 0.01));

            // Reverse
            double[][] actual2 = transform.Inverse().Transform(image);

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual2, data, 0.01));
            Assert.IsTrue(Matrix.IsEqual(actual2, actual, 1e-5));
        }

        [Test]
        public void ExceptionTest()
        {
            double[,] data =
            {
                { 1, 2 },
                { 5, 2 },
                { 2, 2 },
                { 4, 2 },
            };


            var pca = new PrincipalComponentAnalysis(data, AnalysisMethod.Standardize);

            bool thrown = false;

            try { pca.Compute(); }
            catch (ArithmeticException ex)
            {
                ex.ToString();
                thrown = true;
            }

            // Default behavior changed: now an exception is not thrown anymore.
            // Instead, a small constant is added when computing standard deviations.
            Assert.IsFalse(thrown);

            var str1 = pca.SingularValues.ToCSharp();
            var str2 = pca.ComponentVectors.ToCSharp();

            Assert.IsTrue(pca.SingularValues.IsEqual(new double[] { 1.73205080756888, 0 }, 1e-7));
            Assert.IsTrue(pca.ComponentVectors.IsEqual(new double[][] {
                new double[] { 1, 0 },
                new double[] { 0, -1 }
            }, 1e-7));
        }

        [Test]
        public void adjustTest()
        {
            double[,] data = (double[,])PrincipalComponentAnalysisTest.data.Clone();

            PrincipalComponentAnalysis target = new PrincipalComponentAnalysis(data, AnalysisMethod.Standardize);

            double[,] expected =
            {
                {  0.87874523495823,   0.578856809114491 },
                { -1.66834240260186,  -1.42942191638476  },
                {  0.496682089324217,  1.16952702249663  },
                {  0.114618943690204,  0.342588723761638 },
                {  1.64287152622626,   1.28766106517305  },
                {  0.624036471202221,  0.933258937143772 },
                {  0.241973325568208, -0.366215532296923 },
                { -1.03157049321184,  -0.956885745679056 },
                { -0.394798583821814, -0.366215532296923 },
                { -0.904216111333831, -1.19315383103191  }
            };


            double[,] actual = target.Adjust(data, false);

            Assert.IsTrue(expected.IsEqual(actual, 0.00001));
            Assert.AreNotEqual(data, actual);

            actual = target.Adjust(data, true);
            Assert.IsTrue(expected.IsEqual(actual, 0.00001));
            Assert.AreEqual(data, actual);
        }



        [Test]
        public void TransformTest1()
        {
            PrincipalComponentAnalysis target = new PrincipalComponentAnalysis(data);

            // Compute
            target.Compute();

            // Transform
            double[][] actual = target.Transform(data.ToJagged());

            // first inversed.. ?
            double[][] expected = new double[][]
            {
                new double[] {  0.827970186, -0.175115307 },
                new double[] { -1.77758033,   0.142857227 },
                new double[] {  0.992197494,  0.384374989 },
                new double[] {  0.274210416,  0.130417207 },
                new double[] {  1.67580142,  -0.209498461 },
                new double[] {  0.912949103,  0.175282444 },
                new double[] { -0.099109437, -0.349824698 },
                new double[] { -1.14457216,   0.046417258 },
                new double[] { -0.438046137,  0.017764629 },
                new double[] { -1.22382056,  -0.162675287 },
            };

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.01));

        }

#if !NO_BINARY_SERIALIZATION
        [Test]
        [Category("Serialization")]
        public void SerializeTest()
        {
            double[][] actual, expected = new double[][]
            {
                new double[] {  0.827970186, -0.175115307 },
                new double[] { -1.77758033,   0.142857227 },
                new double[] {  0.992197494,  0.384374989 },
                new double[] {  0.274210416,  0.130417207 },
                new double[] {  1.67580142,  -0.209498461 },
                new double[] {  0.912949103,  0.175282444 },
                new double[] { -0.099109437, -0.349824698 },
                new double[] { -1.14457216,   0.046417258 },
                new double[] { -0.438046137,  0.017764629 },
                new double[] { -1.22382056,  -0.162675287 },
            };



            var target = new PrincipalComponentAnalysis();

            target.Learn(data.ToJagged());

            actual = target.Transform(data.ToJagged());
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.01));

            var copy = Serializer.DeepClone(target);

            actual = copy.Transform(data.ToJagged());
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.01));

            Assert.IsTrue(target.ComponentProportions.IsEqual(copy.ComponentProportions));
            Assert.IsTrue(target.ComponentVectors.IsEqual(copy.ComponentVectors));
            Assert.IsTrue(target.CumulativeProportions.IsEqual(copy.CumulativeProportions));
            Assert.IsTrue(target.Eigenvalues.IsEqual(copy.Eigenvalues));
            Assert.IsTrue(target.MaximumNumberOfOutputs.IsEqual(copy.MaximumNumberOfOutputs));
            Assert.IsTrue(target.Method.Equals(copy.Method));
            Assert.IsTrue(target.NumberOfInputs.IsEqual(copy.NumberOfInputs));
            Assert.IsTrue(target.NumberOfOutputs.IsEqual(copy.NumberOfOutputs));
            Assert.IsTrue(target.Overwrite.Equals(copy.Overwrite));
            Assert.IsTrue(target.Whiten.Equals(copy.Whiten));
        }
#endif
    }
}
