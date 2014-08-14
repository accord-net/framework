// Accord Unit Tests
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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Analysis;
    using Accord.Statistics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Math;
    using System;

    [TestClass()]
    public class PrincipalComponentAnalysisTest
    {

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



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

        [TestMethod()]
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
            Assert.IsTrue(eigenvectors.IsEqual(pca.ComponentMatrix, threshold: 1e-9));
            Assert.IsTrue(proportion.IsEqual(pca.ComponentProportions, threshold: 1e-9));
            Assert.IsTrue(eigenvalues.IsEqual(pca.Eigenvalues, threshold: 1e-5));

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
            Assert.IsTrue(expected.IsEqual(actual, threshold: 1e-8));
        }

        [TestMethod()]
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

            double[] mean = Accord.Statistics.Tools.Mean(data);


            // Step 3. Compute the covariance matrix
            // -------------------------------------

            double[,] covariance = Accord.Statistics.Tools.Covariance(data, mean);

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
            Assert.IsTrue(eigenvectors.IsEqual(pca.ComponentMatrix, threshold: 1e-9));
            Assert.IsTrue(proportion.IsEqual(pca.ComponentProportions, threshold: 1e-9));
            Assert.IsTrue(eigenvalues.IsEqual(pca.Eigenvalues, threshold: 1e-8));


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
            Assert.IsTrue(expected.IsEqual(actual, threshold: 1e-8));
        }

        [TestMethod()]
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

        [TestMethod()]
        public void TransformTest2()
        {
            // Lindsay's tutorial data
            double[,] datat = data.Transpose();

            PrincipalComponentAnalysis target = new PrincipalComponentAnalysis(datat);

            // Compute
            target.Compute();

            // Transform
            double[,] actual = target.Transform(datat);

            // Assert the scores equals the transformation of the input

            double[,] result = target.Result;
            Assert.IsTrue(Matrix.IsEqual(result, actual, 0.01));
        }

        [TestMethod()]
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

        [TestMethod()]
        public void FromCovarianceConstructorTest()
        {
            double[] mean = Accord.Statistics.Tools.Mean(data);
            double[,] cov = Accord.Statistics.Tools.Covariance(data);

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

        [TestMethod()]
        public void FromCorrelationConstructorTest()
        {
            double[] mean = Accord.Statistics.Tools.Mean(data);
            double[] stdDev = Accord.Statistics.Tools.StandardDeviation(data);
            double[,] cov = Accord.Statistics.Tools.Correlation(data);

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


        [TestMethod()]
        public void Revert()
        {
            PrincipalComponentAnalysis target = new PrincipalComponentAnalysis(data);

            // Compute
            target.Compute();

            // Transform
            double[,] image = target.Transform(data);

            // Reverse
            double[,] actual = target.Revert(image);

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual, data, 0.01));
        }

        [TestMethod]
        public void ExceptionTest()
        {
            double[,] data = 
            {
                { 1, 2 },
                { 5, 2 },
                { 2, 2 },
                { 4, 2 },
            };


            PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis(data, AnalysisMethod.Standardize);

            bool thrown = false;

            try { pca.Compute(); }
            catch (ArithmeticException ex)
            {
                ex.ToString();
                thrown = true;
            }

            // Assert that an appropriate exception has been
            //   thrown in the case of a constant variable.
            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
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



        /// <summary>
        ///A test for Transform
        ///</summary>
        [TestMethod()]
        public void TransformTest1()
        {
            PrincipalComponentAnalysis target = new PrincipalComponentAnalysis(data);

            // Compute
            target.Compute();

            // Transform
            double[][] actual = target.Transform(data.ToArray());

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
    }
}
