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
    using System.Linq;
    using Accord.Math;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class MultinomialLogisticRegressionTest
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



        [TestMethod]
        public void RegressTest2()
        {
            double[][] inputs;
            int[] outputs;

            createInputOutputsExample1(out inputs, out outputs);

            // Create a new Multinomial Logistic Regression for 3 categories
            var mlr = new MultinomialLogisticRegression(inputs: 2, categories: 3);

            // Create a estimation algorithm to estimate the regression
            LowerBoundNewtonRaphson lbnr = new LowerBoundNewtonRaphson(mlr);

            // Now, we will iteratively estimate our model. The Run method returns
            // the maximum relative change in the model parameters and we will use
            // it as the convergence criteria.

            double delta;
            int iteration = 0;

            do
            {
                // Perform an iteration
                delta = lbnr.Run(inputs, outputs);
                iteration++;

            } while (iteration < 100 && delta > 1e-6);


            Assert.IsFalse(double.IsNaN(mlr.Coefficients[0][0]));
            Assert.IsFalse(double.IsNaN(mlr.Coefficients[0][1]));
            Assert.IsFalse(double.IsNaN(mlr.Coefficients[0][2]));
            Assert.IsFalse(double.IsNaN(mlr.Coefficients[1][0]));
            Assert.IsFalse(double.IsNaN(mlr.Coefficients[1][1]));
            Assert.IsFalse(double.IsNaN(mlr.Coefficients[1][2]));


            // This is the same example given in R Data Analysis Examples for
            // Multinomial Logistic Regression: http://www.ats.ucla.edu/stat/r/dae/mlogit.htm
            
            // brand 2
            Assert.AreEqual(-11.774655, mlr.Coefficients[0][0], 1e-4); // intercept
            Assert.AreEqual(0.523814, mlr.Coefficients[0][1], 1e-4); // female
            Assert.AreEqual(0.368206, mlr.Coefficients[0][2], 1e-4); // age

            // brand 3
            Assert.AreEqual(-22.721396, mlr.Coefficients[1][0], 1e-4); // intercept
            Assert.AreEqual(0.465941, mlr.Coefficients[1][1], 1e-4); // female
            Assert.AreEqual(0.685908, mlr.Coefficients[1][2], 1e-4); // age


            Assert.IsFalse(double.IsNaN(mlr.StandardErrors[0][0]));
            Assert.IsFalse(double.IsNaN(mlr.StandardErrors[0][1]));
            Assert.IsFalse(double.IsNaN(mlr.StandardErrors[0][2]));
            Assert.IsFalse(double.IsNaN(mlr.StandardErrors[1][0]));
            Assert.IsFalse(double.IsNaN(mlr.StandardErrors[1][1]));
            Assert.IsFalse(double.IsNaN(mlr.StandardErrors[1][2]));

           /*
            // Using the standard Hessian estimation
            Assert.AreEqual(1.774612, mlr.StandardErrors[0][0], 1e-6);
            Assert.AreEqual(0.194247, mlr.StandardErrors[0][1], 1e-6);
            Assert.AreEqual(0.055003, mlr.StandardErrors[0][2], 1e-6);

            Assert.AreEqual(2.058028, mlr.StandardErrors[1][0], 1e-6);
            Assert.AreEqual(0.226090, mlr.StandardErrors[1][1], 1e-6);
            Assert.AreEqual(0.062627, mlr.StandardErrors[1][2], 1e-6);
            */

            // Using the lower-bound approximation
            Assert.AreEqual(1.047378039787443, mlr.StandardErrors[0][0], 1e-6);
            Assert.AreEqual(0.153150051082552, mlr.StandardErrors[0][1], 1e-6);
            Assert.AreEqual(0.031640507386863, mlr.StandardErrors[0][2], 1e-6);

            Assert.AreEqual(1.047378039787443, mlr.StandardErrors[1][0], 1e-6);
            Assert.AreEqual(0.153150051082552, mlr.StandardErrors[1][1], 1e-6);
            Assert.AreEqual(0.031640507386863, mlr.StandardErrors[1][2], 1e-6);

            double ll = mlr.GetLogLikelihood(inputs, outputs);

            Assert.AreEqual(-702.97, ll, 1e-2);
            Assert.IsFalse(double.IsNaN(ll));

            var chi = mlr.ChiSquare(inputs, outputs);
            Assert.AreEqual(185.85, chi.Statistic, 1e-2);
            Assert.IsFalse(double.IsNaN(chi.Statistic));

            var wald00 = mlr.GetWaldTest(0, 0);
            var wald01 = mlr.GetWaldTest(0, 1);
            var wald02 = mlr.GetWaldTest(0, 2);

            var wald10 = mlr.GetWaldTest(1, 0);
            var wald11 = mlr.GetWaldTest(1, 1);
            var wald12 = mlr.GetWaldTest(1, 2);

            Assert.IsFalse(double.IsNaN(wald00.Statistic));
            Assert.IsFalse(double.IsNaN(wald01.Statistic));
            Assert.IsFalse(double.IsNaN(wald02.Statistic));

            Assert.IsFalse(double.IsNaN(wald10.Statistic));
            Assert.IsFalse(double.IsNaN(wald11.Statistic));
            Assert.IsFalse(double.IsNaN(wald12.Statistic));

            /*
            // Using standard Hessian estimation
            Assert.AreEqual(-6.6351, wald00.Statistic, 1e-4);
            Assert.AreEqual( 2.6966, wald01.Statistic, 1e-4);
            Assert.AreEqual( 6.6943, wald02.Statistic, 1e-4);

            Assert.AreEqual(-11.0404, wald10.Statistic, 1e-4);
            Assert.AreEqual( 2.0609, wald11.Statistic, 1e-4);
            Assert.AreEqual(10.9524, wald12.Statistic, 1e-4);
            */

            // Using Lower-Bound approximation
            Assert.AreEqual(-11.241995503283842, wald00.Statistic, 1e-4);
            Assert.AreEqual(3.4202662152119889, wald01.Statistic, 1e-4);
            Assert.AreEqual(11.637150673342207, wald02.Statistic, 1e-4);

            Assert.AreEqual(-21.693553825772664, wald10.Statistic, 1e-4);
            Assert.AreEqual(3.0423802097069097, wald11.Statistic, 1e-4);
            Assert.AreEqual(21.678124991086548, wald12.Statistic, 1e-4);


        }


        [TestMethod]
        public void ComputeTest2()
        {
            MultinomialLogisticRegression mlr = createExample1();

            double[][] inputs = example1.Submatrix(null, 1, 2).ToArray();
            double[] outputs = example1.Submatrix(null, 0, 0).Reshape(0);
            double[] responses;

            // Tested against values extracted from predicted probabilities
            // table from: http://www.ats.ucla.edu/stat/r/dae/mlogit.htm

            responses = mlr.Compute(inputs[0]);
            Assert.AreEqual(0.9479577862063925, responses[0], 1e-5);
            Assert.AreEqual(0.0502297144022469, responses[1], 1e-5);
            Assert.AreEqual(0.0018124993913602, responses[2], 1e-5);

            responses = mlr.Compute(inputs[5]);
            Assert.AreEqual(0.772875639435192, responses[0], 1e-5);
            Assert.AreEqual(0.208690558456066, responses[1], 1e-5);
            Assert.AreEqual(0.018433802108742, responses[2], 1e-5);

            responses = mlr.Compute(inputs[11]);
            Assert.AreEqual(0.772875639435192, responses[0], 1e-5);
            Assert.AreEqual(0.208690558456066, responses[1], 1e-5);
            Assert.AreEqual(0.018433802108742, responses[2], 1e-5);

            responses = mlr.Compute(inputs[12]);
            Assert.AreEqual(0.695617266629850, responses[0], 1e-5);
            Assert.AreEqual(0.271439833912059, responses[1], 1e-5);
            Assert.AreEqual(0.032942899458091, responses[2], 1e-5);
        }


        [TestMethod()]
        public void MultinomialLogisticRegressionConstructorTest()
        {
            int inputs = 4;
            int categories = 7;
            MultinomialLogisticRegression target = new MultinomialLogisticRegression(inputs, categories);
            Assert.AreEqual(4, target.Inputs);
            Assert.AreEqual(7, target.Categories);

            Assert.AreEqual(6, target.Coefficients.Length);
            for (int i = 0; i < target.Coefficients.Length; i++)
                Assert.AreEqual(5, target.Coefficients[i].Length);

            Assert.AreEqual(6, target.StandardErrors.Length);
            for (int i = 0; i < target.StandardErrors.Length; i++)
                Assert.AreEqual(5, target.StandardErrors[i].Length);
        }

        [TestMethod()]
        public void ChiSquareMethodTest()
        {
            double[][] inputs;
            int[] outputs;

            createInputOutputsExample1(out inputs, out outputs);

            MultinomialLogisticRegression target = createExample1();

            ChiSquareTest actual = target.ChiSquare(inputs, outputs);
            Assert.AreEqual(4, actual.DegreesOfFreedom);
            Assert.AreEqual(185.85, actual.Statistic, 1e-2);
        }


        [TestMethod()]
        public void CloneTest()
        {

            MultinomialLogisticRegression target = createExample1();
            MultinomialLogisticRegression actual = (MultinomialLogisticRegression)target.Clone();

            Assert.AreNotEqual(target, actual);
            Assert.AreEqual(target.Categories, actual.Categories);
            Assert.AreEqual(target.Inputs, actual.Inputs);

            Assert.AreNotEqual(target.Coefficients, actual.Coefficients);
            Assert.AreNotEqual(target.StandardErrors, actual.StandardErrors);

            for (int i = 0; i < target.Coefficients.Length; i++)
            {
                for (int j = 0; j < target.Coefficients[i].Length; j++)
                {
                    Assert.AreEqual(target.Coefficients[i][j], actual.Coefficients[i][j]);
                    Assert.AreEqual(target.StandardErrors[i][j], actual.StandardErrors[i][j]);
                }
            }
        }


        [TestMethod()]
        public void GetLogLikelihoodTest()
        {
            MultinomialLogisticRegression mlr = createExample1();

            double[][] inputs;
            int[] outputs;

            createInputOutputsExample1(out inputs, out outputs);

            double expected = -702.97;
            double actual = mlr.GetLogLikelihood(inputs, outputs);
            Assert.AreEqual(expected, actual, 1e-2);
        }


        [TestMethod()]
        public void GetOddsRatioTest()
        {
            MultinomialLogisticRegression target = createExample1();
            double actual;


            actual = target.GetOddsRatio(0, 1);
            Assert.AreEqual(System.Math.Exp(target.Coefficients[0][1]), actual);

            actual = target.GetOddsRatio(0, 2);
            Assert.AreEqual(System.Math.Exp(target.Coefficients[0][2]), actual);

            actual = target.GetOddsRatio(1, 1);
            Assert.AreEqual(System.Math.Exp(target.Coefficients[1][1]), actual);

            actual = target.GetOddsRatio(1, 2);
            Assert.AreEqual(System.Math.Exp(target.Coefficients[1][2]), actual);
        }

        [TestMethod()]
        public void GetWaldTestTest()
        {
            MultinomialLogisticRegression target = createExample1();

            double[][] inputs;
            int[] outputs;

            createInputOutputsExample1(out inputs, out outputs);

            WaldTest actual;

            actual = target.GetWaldTest(0, 0);
            Assert.AreEqual(-6.6351, actual.Statistic, 1e-4);
            Assert.AreEqual(3.244e-11, actual.PValue, 1e-14);

            actual = target.GetWaldTest(0, 1);
            Assert.AreEqual(2.6966, actual.Statistic, 1e-4);
            Assert.AreEqual(0.007004, actual.PValue, 1e-5);

            actual = target.GetWaldTest(0, 2);
            Assert.AreEqual(6.6943, actual.Statistic, 1e-4);
            Assert.AreEqual(2.167e-11, actual.PValue, 1e-14);

            actual = target.GetWaldTest(1, 0);
            Assert.AreEqual(-11.0404, actual.Statistic, 1e-4);
            Assert.AreEqual(0.0, actual.PValue, 1e-25);

            actual = target.GetWaldTest(1, 1);
            Assert.AreEqual(2.0609, actual.Statistic, 1e-4);
            Assert.AreEqual(0.039315, actual.PValue, 1e-6);

            actual = target.GetWaldTest(1, 2);
            Assert.AreEqual(10.9524, actual.Statistic, 1e-3);
            Assert.AreEqual(0.0, actual.PValue, 1e-25);
        }





        private static MultinomialLogisticRegression createExample1()
        {
            MultinomialLogisticRegression mlr = new MultinomialLogisticRegression(2, 3);

            
            // brand 2
            mlr.Coefficients[0][0] = -11.774655; // intercept
            mlr.Coefficients[0][1] = 0.523814; // female
            mlr.Coefficients[0][2] = 0.368206; // age

            // brand 3
            mlr.Coefficients[1][0] = -22.721396; // intercept
            mlr.Coefficients[1][1] = 0.465941; // female
            mlr.Coefficients[1][2] = 0.685908; // age


            mlr.StandardErrors[0][0] = 1.774612;
            mlr.StandardErrors[0][1] = 0.194247;
            mlr.StandardErrors[0][2] = 0.055003;

            mlr.StandardErrors[1][0] = 2.058028;
            mlr.StandardErrors[1][1] = 0.226090;
            mlr.StandardErrors[1][2] = 0.062627;

            return mlr;
        }

        private static void createInputOutputsExample1(out double[][] inputs, out int[] outputs)
        {
            inputs = example1.Submatrix(null, 1, 2).ToArray();
            outputs = example1.Submatrix(null, 0, 0).Reshape(0).Select(x => (int)x - 1).ToArray();
        }

        public static double[,] example1 =
        {
            #region r/dae/mlogit.csv
                // [1] "brand",  "female", "age"
                {         1,         0,      24   }, 

                { 1,0,26 }, { 1,0,26 }, { 1,1,27 }, { 1,1,27 }, { 3,1,27 }, { 1,0,27 }, { 1,0,27 }, { 1,1,27 }, 
                { 1,0,27 }, { 1,0,27 }, { 1,1,27 }, { 2,1,28 }, { 3,1,28 }, { 1,1,28 }, { 1,0,28 }, { 1,0,28 }, 
                { 2,1,28 }, { 1,0,28 }, { 1,0,28 }, { 1,1,28 }, { 1,1,28 }, { 3,0,28 }, { 1,1,28 }, { 3,0,28 }, 
                { 1,1,28 }, { 1,1,28 }, { 1,1,29 }, { 1,1,29 }, { 1,1,29 }, { 2,1,29 }, { 1,1,29 }, { 2,1,29 }, 
                { 2,0,29 }, { 2,1,29 }, { 1,1,29 }, { 1,0,29 }, { 1,0,29 }, { 1,0,29 }, { 1,1,29 }, { 2,1,29 }, 
                { 1,0,29 }, { 1,0,29 }, { 1,1,29 }, { 1,0,29 }, { 1,0,29 }, { 2,1,30 }, { 3,0,30 }, { 3,1,30 }, 
                { 1,0,30 }, { 3,1,30 }, { 2,1,30 }, { 1,0,30 }, { 1,1,30 }, { 1,1,30 }, { 1,1,30 }, { 1,1,30 }, 
                { 3,1,30 }, { 1,1,30 }, { 1,1,30 }, { 2,1,30 }, { 1,1,30 }, { 2,0,30 }, { 2,0,30 }, { 2,1,30 }, 
                { 1,1,30 }, { 1,1,30 }, { 1,1,30 }, { 1,0,30 }, { 1,1,31 }, { 1,0,31 }, { 3,1,31 }, { 2,1,31 }, 
                { 2,0,31 }, { 2,1,31 }, { 3,1,31 }, { 1,0,31 }, { 2,1,31 }, { 2,1,31 }, { 1,0,31 }, { 1,0,31 }, 
                { 1,1,31 }, { 2,0,31 }, { 3,1,31 }, { 3,0,31 }, { 1,0,31 }, { 1,1,31 }, { 1,1,31 }, { 3,0,31 }, 
                { 2,0,31 }, { 1,0,31 }, { 2,1,31 }, { 1,1,31 }, { 3,1,31 }, { 1,0,31 }, { 1,1,31 }, { 2,1,31 }, 
                { 1,0,31 }, { 3,1,31 }, { 1,0,31 }, { 2,1,31 }, { 2,0,31 }, { 1,0,31 }, { 2,1,31 }, { 1,1,31 }, 
                { 2,1,31 }, { 2,0,31 }, { 1,1,31 }, { 1,1,31 }, { 2,0,32 }, { 2,1,32 }, { 1,1,32 }, { 2,0,32 }, 
                { 2,0,32 }, { 2,1,32 }, { 2,1,32 }, { 1,1,32 }, { 1,0,32 }, { 2,1,32 }, { 2,1,32 }, { 3,1,32 }, 
                { 2,1,32 }, { 2,0,32 }, { 2,1,32 }, { 1,1,32 }, { 1,1,32 }, { 1,1,32 }, { 1,1,32 }, { 3,1,32 }, 
                { 2,1,32 }, { 1,1,32 }, { 2,1,32 }, { 1,1,32 }, { 2,1,32 }, { 2,0,32 }, { 1,1,32 }, { 3,1,32 }, 
                { 2,1,32 }, { 3,1,32 }, { 1,1,32 }, { 2,1,32 }, { 1,1,32 }, { 1,0,32 }, { 2,1,32 }, { 2,0,32 }, 
                { 2,1,32 }, { 3,1,32 }, { 3,1,32 }, { 2,1,32 }, { 3,0,32 }, { 1,0,32 }, { 2,1,32 }, { 1,1,32 }, 
                { 2,1,32 }, { 2,1,32 }, { 2,1,32 }, { 2,1,32 }, { 1,1,32 }, { 1,0,32 }, { 2,0,32 }, { 2,0,32 }, 
                { 2,0,32 }, { 1,1,32 }, { 3,0,32 }, { 2,0,32 }, { 1,1,32 }, { 3,1,32 }, { 2,1,32 }, { 1,1,32 }, 
                { 3,1,32 }, { 1,1,32 }, { 3,1,32 }, { 1,0,32 }, { 2,1,32 }, { 2,1,32 }, { 1,1,32 }, { 2,0,32 }, 
                { 2,1,32 }, { 2,0,32 }, { 3,1,32 }, { 2,1,32 }, { 2,1,32 }, { 3,1,32 }, { 2,1,32 }, { 2,1,32 }, 
                { 3,0,32 }, { 2,1,32 }, { 2,1,32 }, { 1,1,32 }, { 2,1,32 }, { 2,1,32 }, { 1,1,32 }, { 1,0,32 }, 
                { 1,1,32 }, { 1,0,32 }, { 3,1,32 }, { 2,1,32 }, { 3,1,32 }, { 3,1,32 }, { 1,1,32 }, { 2,1,32 }, 
                { 1,1,32 }, { 1,1,32 }, { 2,0,32 }, { 2,1,32 }, { 1,1,32 }, { 2,0,32 }, { 2,1,32 }, { 1,0,32 }, 
                { 2,0,32 }, { 3,1,32 }, { 1,0,32 }, { 1,0,32 }, { 1,1,32 }, { 1,1,32 }, { 2,0,32 }, { 3,1,32 }, 
                { 1,0,32 }, { 2,1,32 }, { 2,1,32 }, { 2,1,32 }, { 2,0,32 }, { 2,0,32 }, { 3,0,32 }, { 1,1,32 }, 
                { 2,1,32 }, { 2,0,32 }, { 2,0,32 }, { 2,1,32 }, { 1,0,32 }, { 2,1,32 }, { 2,1,32 }, { 1,1,32 }, 
                { 2,0,32 }, { 2,0,32 }, { 3,1,32 }, { 1,0,32 }, { 3,1,32 }, { 3,0,32 }, { 2,1,32 }, { 1,1,32 }, 
                { 2,1,32 }, { 2,1,32 }, { 2,0,32 }, { 2,0,32 }, { 3,0,32 }, { 1,1,32 }, { 1,1,32 }, { 1,1,32 }, 
                { 3,0,32 }, { 2,0,32 }, { 1,1,32 }, { 2,1,32 }, { 1,0,32 }, { 1,1,32 }, { 2,1,32 }, { 2,0,32 }, 
                { 2,1,32 }, { 2,1,32 }, { 2,1,32 }, { 1,0,32 }, { 1,0,32 }, { 2,0,32 }, { 2,1,32 }, { 2,1,32 }, 
                { 1,0,32 }, { 3,0,32 }, { 2,1,32 }, { 2,1,32 }, { 2,1,32 }, { 3,1,32 }, { 1,1,32 }, { 2,0,32 }, 
                { 2,1,32 }, { 2,1,32 }, { 1,1,32 }, { 3,1,32 }, { 3,0,32 }, { 1,1,32 }, { 2,0,32 }, { 1,1,32 }, 
                { 2,0,32 }, { 1,0,32 }, { 3,1,32 }, { 2,1,32 }, { 1,1,32 }, { 1,0,32 }, { 2,0,32 }, { 1,1,32 }, 
                { 2,0,32 }, { 2,1,32 }, { 1,0,32 }, { 1,0,32 }, { 1,0,32 }, { 2,1,32 }, { 1,1,32 }, { 2,1,32 }, 
                { 1,1,32 }, { 2,0,32 }, { 1,1,32 }, { 2,1,32 }, { 2,1,32 }, { 2,1,32 }, { 2,1,32 }, { 3,1,32 }, 
                { 3,1,32 }, { 3,1,32 }, { 1,1,32 }, { 2,1,32 }, { 2,1,32 }, { 2,0,32 }, { 3,0,32 }, { 3,0,32 }, 
                { 1,0,32 }, { 1,0,32 }, { 2,1,32 }, { 1,0,32 }, { 1,1,32 }, { 3,0,32 }, { 1,0,32 }, { 2,0,32 }, 
                { 2,0,32 }, { 1,0,32 }, { 2,1,32 }, { 1,1,32 }, { 2,0,32 }, { 1,0,32 }, { 2,1,32 }, { 2,1,32 }, 
                { 2,0,32 }, { 2,1,32 }, { 2,1,32 }, { 1,0,32 }, { 2,1,32 }, { 2,1,32 }, { 1,0,32 }, { 1,1,32 }, 
                { 1,0,32 }, { 1,0,32 }, { 2,0,32 }, { 3,1,32 }, { 2,1,32 }, { 3,1,32 }, { 2,1,32 }, { 1,0,32 }, 
                { 1,0,32 }, { 3,0,32 }, { 1,1,32 }, { 2,1,32 }, { 2,1,32 }, { 2,1,32 }, { 1,0,32 }, { 3,0,32 }, 
                { 2,1,32 }, { 2,1,32 }, { 2,1,32 }, { 2,1,32 }, { 2,1,32 }, { 2,1,32 }, { 3,1,32 }, { 1,1,32 }, 
                { 3,0,32 }, { 1,1,32 }, { 2,1,32 }, { 2,0,32 }, { 2,0,32 }, { 3,0,32 }, { 2,1,32 }, { 3,1,32 }, 
                { 1,0,32 }, { 1,0,32 }, { 2,1,32 }, { 1,0,32 }, { 3,1,32 }, { 1,0,32 }, { 2,1,32 }, { 1,1,32 }, 
                { 1,1,32 }, { 2,1,32 }, { 2,1,32 }, { 2,0,32 }, { 1,0,32 }, { 2,1,32 }, { 3,1,32 }, { 2,1,32 }, 
                { 2,1,32 }, { 2,0,32 }, { 3,1,32 }, { 2,0,32 }, { 2,1,32 }, { 1,0,32 }, { 2,1,32 }, { 1,0,32 }, 
                { 2,1,32 }, { 2,0,32 }, { 2,1,32 }, { 2,0,32 }, { 2,1,32 }, { 2,1,32 }, { 2,0,32 }, { 3,0,32 }, 
                { 1,1,32 }, { 3,1,32 }, { 2,1,32 }, { 2,1,32 }, { 1,1,32 }, { 2,1,32 }, { 1,0,32 }, { 2,0,32 }, 
                { 1,1,32 }, { 1,1,32 }, { 3,1,32 }, { 2,1,32 }, { 1,0,32 }, { 2,1,32 }, { 1,1,32 }, { 2,1,32 }, 
                { 2,1,32 }, { 1,1,32 }, { 3,1,32 }, { 2,1,32 }, { 2,1,32 }, { 2,1,32 }, { 2,1,32 }, { 3,1,32 }, 
                { 2,1,32 }, { 3,1,32 }, { 2,0,32 }, { 2,1,32 }, { 3,0,32 }, { 1,0,32 }, { 1,1,32 }, { 1,0,32 }, 
                { 2,0,32 }, { 1,0,32 }, { 1,0,32 }, { 2,0,32 }, { 2,1,32 }, { 2,1,32 }, { 1,1,32 }, { 3,0,32 }, 
                { 1,1,32 }, { 3,1,33 }, { 3,1,33 }, { 1,1,33 }, { 2,0,33 }, { 1,1,33 }, { 3,0,33 }, { 3,1,33 }, 
                { 2,1,33 }, { 1,0,33 }, { 3,1,33 }, { 1,0,33 }, { 2,1,33 }, { 2,1,33 }, { 2,1,33 }, { 2,1,33 }, 
                { 1,0,33 }, { 2,1,33 }, { 3,0,33 }, { 3,1,33 }, { 2,0,33 }, { 2,0,33 }, { 3,0,33 }, { 2,1,33 }, 
                { 2,1,33 }, { 2,1,33 }, { 2,1,33 }, { 2,1,33 }, { 2,1,33 }, { 2,1,33 }, { 3,0,33 }, { 3,1,33 }, 
                { 3,1,33 }, { 1,0,33 }, { 3,0,33 }, { 2,1,33 }, { 2,1,33 }, { 2,1,33 }, { 2,1,33 }, { 2,1,33 }, 
                { 2,1,33 }, { 2,0,33 }, { 2,0,33 }, { 2,1,33 }, { 3,1,33 }, { 2,1,33 }, { 2,1,33 }, { 3,1,33 }, 
                { 2,0,33 }, { 3,0,33 }, { 3,1,33 }, { 2,1,33 }, { 2,0,33 }, { 2,0,33 }, { 2,0,33 }, { 1,1,33 }, 
                { 2,1,34 }, { 3,0,34 }, { 2,1,34 }, { 3,1,34 }, { 2,0,34 }, { 3,1,34 }, { 1,1,34 }, { 2,1,34 }, 
                { 1,0,34 }, { 1,0,34 }, { 2,0,34 }, { 3,1,34 }, { 1,1,34 }, { 2,1,34 }, { 1,1,34 }, { 2,0,34 }, 
                { 2,0,34 }, { 3,0,34 }, { 2,1,34 }, { 1,1,34 }, { 2,1,34 }, { 3,1,34 }, { 3,0,34 }, { 3,1,34 }, 
                { 3,1,34 }, { 2,1,34 }, { 2,0,34 }, { 2,0,34 }, { 3,0,34 }, { 2,1,34 }, { 2,0,34 }, { 3,1,34 }, 
                { 3,0,34 }, { 2,1,34 }, { 2,0,34 }, { 2,0,34 }, { 3,0,34 }, { 2,1,34 }, { 2,0,34 }, { 2,1,34 }, 
                { 3,1,34 }, { 3,1,34 }, { 3,1,34 }, { 3,1,34 }, { 3,1,34 }, { 1,1,34 }, { 3,1,34 }, { 2,1,34 }, 
                { 3,0,34 }, { 3,1,34 }, { 2,1,34 }, { 3,1,34 }, { 3,1,34 }, { 1,0,34 }, { 2,1,34 }, { 3,1,34 }, 
                { 1,1,34 }, { 3,1,34 }, { 2,1,34 }, { 2,0,34 }, { 2,1,34 }, { 2,0,34 }, { 1,0,34 }, { 2,0,34 }, 
                { 3,1,35 }, { 1,1,35 }, { 3,0,35 }, { 3,1,35 }, { 3,1,35 }, { 3,1,35 }, { 3,1,35 }, { 3,1,35 }, 
                { 3,1,35 }, { 3,0,35 }, { 2,0,35 }, { 3,0,35 }, { 2,1,35 }, { 2,1,35 }, { 3,1,35 }, { 3,1,35 }, 
                { 3,0,35 }, { 3,1,35 }, { 3,1,35 }, { 3,1,35 }, { 3,1,35 }, { 2,1,35 }, { 3,1,35 }, { 3,1,35 }, 
                { 2,0,35 }, { 3,0,35 }, { 2,1,35 }, { 2,1,35 }, { 2,1,35 }, { 3,1,35 }, { 3,0,35 }, { 3,0,35 }, 
                { 3,0,35 }, { 3,0,35 }, { 3,1,35 }, { 3,0,36 }, { 2,1,36 }, { 2,0,36 }, { 3,1,36 }, { 3,0,36 }, 
                { 1,1,36 }, { 1,1,36 }, { 1,0,36 }, { 1,1,36 }, { 3,1,36 }, { 3,1,36 }, { 2,1,36 }, { 2,1,36 }, 
                { 3,1,36 }, { 2,1,36 }, { 2,1,36 }, { 3,1,36 }, { 2,1,36 }, { 3,1,36 }, { 3,1,36 }, { 2,0,36 }, 
                { 3,0,36 }, { 1,1,36 }, { 3,0,36 }, { 2,1,36 }, { 3,1,36 }, { 2,0,36 }, { 3,1,36 }, { 2,1,36 }, 
                { 3,1,36 }, { 2,1,36 }, { 2,1,36 }, { 1,1,36 }, { 1,0,36 }, { 3,1,36 }, { 2,0,36 }, { 3,0,36 }, 
                { 3,0,36 }, { 2,1,36 }, { 3,0,36 }, { 3,1,36 }, { 2,0,36 }, { 3,0,36 }, { 2,1,36 }, { 2,0,36 }, 
                { 2,0,36 }, { 3,0,36 }, { 3,1,36 }, { 3,1,36 }, { 3,1,36 }, { 2,1,36 }, { 3,1,36 }, { 3,0,36 }, 
                { 2,1,36 }, { 2,0,36 }, { 3,1,36 }, { 3,1,36 }, { 3,1,36 }, { 3,0,36 }, { 3,0,36 }, { 2,0,36 }, 
                { 1,1,36 }, { 3,0,36 }, { 3,1,36 }, { 1,0,36 }, { 2,1,36 }, { 3,1,36 }, { 3,1,36 }, { 3,0,36 }, 
                { 3,1,36 }, { 3,1,36 }, { 3,1,36 }, { 3,1,36 }, { 2,1,36 }, { 3,0,36 }, { 3,1,36 }, { 3,0,36 }, 
                { 1,0,36 }, { 3,1,36 }, { 2,1,36 }, { 2,0,36 }, { 2,1,36 }, { 2,1,36 }, { 3,1,36 }, { 3,1,36 }, 
                { 3,1,37 }, { 3,1,37 }, { 2,0,37 }, { 3,1,37 }, { 3,1,37 }, { 3,1,37 }, { 1,0,37 }, { 3,1,37 }, 
                { 3,1,37 }, { 2,1,37 }, { 2,1,37 }, { 3,1,37 }, { 2,1,37 }, { 3,1,37 }, { 3,0,37 }, { 3,1,37 }, 
                { 3,0,37 }, { 3,1,37 }, { 2,1,37 }, { 3,1,37 }, { 3,0,37 }, { 2,1,37 }, { 2,0,38 }, { 3,1,38 }, 
                { 3,0,38 }, { 2,0,38 }, { 2,1,38 }, { 3,0,38 }, { 3,1,38 }, { 2,0,38 }, { 3,1,38 }, { 3,1,38 }, 
                { 3,1,38 }, { 3,0,38 }, { 2,0,38 }, { 3,0,38 }, { 2,1,38 }, { 2,0,38 }, { 3,1,38 }, { 3,0,38 }, 
                { 3,1,38 }, { 3,0,38 }, { 3,0,38 }, { 3,0,38 }, { 2,1,38 }, { 3,0,38 }, { 3,0,38 }, { 3,1,38 }, 
                { 3,0,38 }, { 3,1,38 }, { 3,0,38 }, { 3,0,38 }, { 3,1,38 }, { 1,1,38 }, 
                #endregion
        };


    }
}
