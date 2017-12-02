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
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Accord.Statistics.Testing;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class MultinomialLogisticRegressionTest
    {

        [Test]
        public void RegressTest2()
        {
            double[][] inputs;
            int[] outputs;

            CreateInputOutputsExample1(out inputs, out outputs);

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

            Assert.AreEqual(52, iteration);
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

        [Test]
        public void RegressTest2_new_method()
        {
            double[][] inputs;
            int[] outputs;

            CreateInputOutputsExample1(out inputs, out outputs);

            // Create a estimation algorithm to estimate the regression
            LowerBoundNewtonRaphson lbnr = new LowerBoundNewtonRaphson()
            {
                MaxIterations = 52,
                Tolerance = 0
            };

            // Now, we will iteratively estimate our model. The Run method returns
            // the maximum relative change in the model parameters and we will use
            // it as the convergence criteria.

            MultinomialLogisticRegression mlr = lbnr.Learn(inputs, outputs);

            Assert.AreEqual(52, lbnr.CurrentIteration);

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

        [Test]
        public void doc_learn()
        {
            #region doc_learn
            // Declare a very simple classification/regression
            // problem with only 2 input variables (x and y):
            double[][] inputs =
            {
                new[] { 3.0, 1.0 },
                new[] { 7.0, 1.0 },
                new[] { 3.0, 1.1 },
                new[] { 3.0, 2.0 },
                new[] { 6.0, 1.0 },
            };

            // Class labels for each of the inputs
            int[] outputs =
            {
                0, 2, 0, 1, 2
            };

            // Create a estimation algorithm to estimate the regression
            LowerBoundNewtonRaphson lbnr = new LowerBoundNewtonRaphson()
            {
                MaxIterations = 100,
                Tolerance = 1e-6
            };

            // Now, we will iteratively estimate our model:
            MultinomialLogisticRegression mlr = lbnr.Learn(inputs, outputs);

            // We can compute the model answers
            int[] answers = mlr.Decide(inputs);

            // And also the probability of each of the answers
            double[][] probabilities = mlr.Probabilities(inputs);

            // Now we can check how good our model is at predicting
            double error = new ZeroOneLoss(outputs).Loss(answers);

            // We can also verify the classes with highest 
            // probability are the ones being decided for:
            int[] argmax = probabilities.ArgMax(dimension: 1); // should be same as 'answers'
            #endregion

            Assert.AreEqual(0, error);
            Assert.AreEqual(answers, argmax);
        }

        [Test]
        public void ComputeTest2()
        {
            MultinomialLogisticRegression mlr = createExample1();

            double[][] inputs = example1.Submatrix(null, 1, 2).ToJagged();
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


        [Test]
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

        [Test]
        public void ChiSquareMethodTest()
        {
            double[][] inputs;
            int[] outputs;

            CreateInputOutputsExample1(out inputs, out outputs);

            MultinomialLogisticRegression target = createExample1();

            ChiSquareTest actual = target.ChiSquare(inputs, outputs);
            Assert.AreEqual(4, actual.DegreesOfFreedom);
            Assert.AreEqual(185.85, actual.Statistic, 1e-2);
        }


        [Test]
        public void CloneTest()
        {
            MultinomialLogisticRegression target = createExample1();
            MultinomialLogisticRegression actual = (MultinomialLogisticRegression)target.Clone();

            Assert.AreNotEqual(target, actual);
            Assert.AreEqual(target.Categories, actual.Categories);
            Assert.AreEqual(target.Inputs, actual.Inputs);

            Assert.AreNotSame(target.Coefficients, actual.Coefficients);
            Assert.AreNotSame(target.StandardErrors, actual.StandardErrors);

            for (int i = 0; i < target.Coefficients.Length; i++)
            {
                for (int j = 0; j < target.Coefficients[i].Length; j++)
                {
                    Assert.AreEqual(target.Coefficients[i][j], actual.Coefficients[i][j]);
                    Assert.AreEqual(target.StandardErrors[i][j], actual.StandardErrors[i][j]);
                }
            }
        }


        [Test]
        public void GetLogLikelihoodTest()
        {
            MultinomialLogisticRegression mlr = createExample1();

            double[][] inputs;
            int[] outputs;

            CreateInputOutputsExample1(out inputs, out outputs);

            double expected = -702.97;
            double actual = mlr.GetLogLikelihood(inputs, outputs);
            Assert.AreEqual(expected, actual, 1e-2);
        }


        [Test]
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

        [Test]
        public void GetWaldTestTest()
        {
            MultinomialLogisticRegression target = createExample1();

            double[][] inputs;
            int[] outputs;

            CreateInputOutputsExample1(out inputs, out outputs);

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

        [Test]
        [Category("Slow")]
        public void gh_803()
        {
            // Example from https://github.com/accord-net/framework/issues/803

            double[][] inputs =
            {
                new[] { 0, 0, 15.9960060119629, 9, 32.6011238098145, -1.89428806304932, 7.47045946121216, 0.536165952682495, -2.84551954269409, 16.2021484375, 16.7839775085449, 0, 0, 1, 1, 0, 0, 1, 0, 1, 255.872207641602, 1.12985539146848E-07, 81, 0.000123409801744856, 1062.83325195313, 6.94243492514323E-15, 3.58832716941834, 6.64781379699707, 8.09698104858398, 17.2104988098145, 262.509613037109, 9.19382756592313E-08, 281.701904296875, 5.1382020416213E-08, 1, 0.367879450321198, 55.8077659606934, 0.000569666502997279, 0.287473917007446, 0.584986805915833, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 0, -2, 15.9433555603027, -7.84066534042358, 32.5062866210938, -0.716531932353973, 7.56672859191895, 1.77490830421448, -1.49884343147278, 15.1011009216309, 16.5958766937256, 0, 0, 1, 1, 0, 0, 1, 4, 7.38905620574951, 254.190582275391, 1.19093662931391E-07, 61.4760322570801, 2541.8955078125, 1056.65869140625, 7.63306705033528E-15, 0.513418018817902, 2.04732060432434, 2.24653172492981, 4.47650861740112, 228.043243408203, 2.76487298833672E-07, 275.423126220703, 6.20158004949189E-08, 1, 0.367879450321198, 57.2553825378418, 0.000517382228281349, 3.15029954910278, 0.16949899494648, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 0, -2, 15.8133296966553, -6.63396549224854, 32.5105972290039, 0.508405864238739, 7.72873210906982, 3.03013849258423, -0.222389459609985, 15.0367679595947, 16.5742988586426, 0, 0, 1, 1, 0, 0, 1, 4, 7.38905620574951, 250.061401367188, 1.35630756403771E-07, 44.0094985961914, 760.491943359375, 1056.93896484375, 7.60023520626676E-15, 0.25847652554512, 0.601453602313995, 0.0494570732116699, 1.24905776977539, 226.104385375977, 2.94859177074613E-07, 274.707397460938, 6.33685033335496E-08, 1, 0.367879450321198, 59.7332992553711, 0.000440001633251086, 9.18173885345459, 0.0483089461922646, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 0, -2, 15.5640602111816, -5.05492830276489, 32.6443138122559, 2.10835695266724, 7.99698829650879, 4.62681913375854, 1.45856726169586, 15.1721878051758, 16.6098442077637, 0, 0, 1, 1, 0, 0, 1, 4, 7.38905620574951, 242.239974975586, 1.74026155264073E-07, 25.5522994995117, 156.79328918457, 1065.65124511719, 6.64897400781987E-15, 4.44516897201538, 0.121437333524227, 2.12741851806641, 0.232569247484207, 230.195281982422, 2.57515011981013E-07, 275.886932373047, 6.11556103535804E-08, 1, 0.367879450321198, 63.9518203735352, 0.000336474477080628, 21.4074554443359, 0.00978583749383688, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 2, -2, 15.3881740570068, -3.84450721740723, 31.9708518981934, 3.42879939079285, 8.59982872009277, 6.27867126464844, 3.08211016654968, 13.543719291687, 16.1791038513184, 0, 0, 1, 1, 0, 4, 0.13533528149128, 4, 7.38905620574951, 236.7958984375, 2.07491794412817E-07, 14.7802352905273, 46.7356491088867, 1022.13537597656, 1.30387347579562E-14, 11.7566652297974, 0.0324258469045162, 9.49940299987793, 0.0458623766899109, 183.432327270508, 1.31231308841961E-06, 261.763397216797, 9.40815496619507E-08, 1, 0.367879450321198, 73.9570541381836, 0.000184137330506928, 39.4217109680176, 0.00187589146662503, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 0, -2, 15.3432788848877, -3.63553857803345, 32.0133514404297, 3.63945651054382, 8.63861083984375, 6.47920751571655, 3.32499885559082, 30.5709476470947, 16.1866722106934, 0, 0, 1, 1, 0, 0, 1, 4, 7.38905620574951, 235.416213989258, 2.1701944774577E-07, 13.2171411514282, 37.9222717285156, 1024.85461425781, 1.24962042260092E-14, 13.2456436157227, 0.0262666158378124, 11.0556173324585, 0.0359725616872311, 934.582824707031, 5.28696112226095E-14, 262.008361816406, 9.33722006379867E-08, 1, 0.367879450321198, 74.6255950927734, 0.000177132795215584, 41.9801292419434, 0.00153502670582384, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 0, 2, 15.8523807525635, -5.20052146911621, 31.6573257446289, 2.04027128219604, 8.12680912017822, 4.9469518661499, -5.62986612319946, 2.53497529029846, 18.1027507781982, 0, 0, 1, 1, 0, 0, 1, 4, 0.13533528149128, 251.297973632813, 1.30436319523142E-07, 27.0454235076904, 181.366790771484, 1002.18627929688, 1.78401333961707E-14, 4.16270685195923, 0.129993438720703, 31.6953926086426, 278.624816894531, 6.42609977722168, 0.0792636796832085, 327.709594726563, 1.37427997870532E-08, 1, 0.367879450321198, 66.0450286865234, 0.000295509642455727, 24.4723320007324, 0.00710503291338682, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 0, 2, 16.0148315429688, -8.18133544921875, 31.6773452758789, -0.968354821205139, 7.87726211547852, 1.90134263038635, -1.90072727203369, 15.0141248703003, 16.4788360595703, 0, 0, 1, 1, 0, 0, 1, 4, 0.13533528149128, 256.474822998047, 1.10878417558524E-07, 66.9342498779297, 3573.62377929688, 1003.45422363281, 1.74865344101417E-14, 0.93771106004715, 2.63360810279846, 3.61276412010193, 6.69075870513916, 225.423950195313, 3.01611862596474E-07, 271.552032470703, 6.97159947549153E-08, 1, 0.367879450321198, 62.0512580871582, 0.00037927002995275, 3.61510372161865, 0.149367943406105, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 0, 2, 16.0222930908203, 8.98222351074219, 31.7461948394775, -1.81469142436981, 7.82322025299072, 1.02214419841766, -2.70340394973755, 14.7527132034302, 16.3994216918945, 0, 0, 1, 1, 0, 0, 1, 4, 0.13533528149128, 256.7138671875, 1.10054173774188E-07, 80.6803359985352, 0.000125623206258751, 1007.82086181641, 1.63231041639018E-14, 3.29310488700867, 6.13918161392212, 7.3083930015564, 14.9304676055908, 217.642547607422, 3.91722096537706E-07, 268.941040039063, 7.54782192302628E-08, 1, 0.367879450321198, 61.2027740478516, 0.000400330434786156, 1.04477870464325, 0.359822571277618, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 0, 2, 15.9642181396484, 7.83014059066772, 31.9124965667725, -2.97841000556946, 7.80850887298584, -0.207009360194206, -3.92859625816345, 15.4842300415039, 16.5914611816406, 0, 0, 1, 1, 0, 0, 1, 4, 0.13533528149128, 254.856262207031, 1.16634801372584E-07, 61.3111000061035, 0.000397569587221369, 1018.40740966797, 1.38222529396607E-14, 8.87092590332031, 19.6565380096436, 15.4338684082031, 50.8355674743652, 239.761383056641, 1.88488272101495E-07, 275.276580810547, 6.22902334157516E-08, 1, 0.367879450321198, 60.9728126525879, 0.000406263396143913, 0.0428528748452663, 1.22999405860901, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 0, 0, 15.9628734588623, 7.57881641387939, 31.9146461486816, -3.22983598709106, 7.80878067016602, -0.459216833114624, -3.99851727485657, 14.3275060653687, 16.3168258666992, 0, 0, 1, 1, 0, 0, 1, 0, 1, 254.813323974609, 1.16791746052058E-07, 57.4384574890137, 0.000511165882926434, 1018.54461669922, 1.37925729051889E-14, 10.4318408966064, 25.2755107879639, 15.9881401062012, 54.5172576904297, 205.277435302734, 5.99298459746933E-07, 266.238800048828, 8.19771059923369E-08, 1, 0.367879450321198, 60.9770545959473, 0.00040615297621116, 0.210880100727081, 1.58283388614655, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 0, -2, 16.0214576721191, -8.95163345336914, 31.7009048461914, -1.7427282333374, 7.85236406326294, 1.11497819423676, -2.26028609275818, 12.9394121170044, 16.081127166748, 0, 0, 1, 1, 0, 0, 1, 4, 7.38905620574951, 256.687103271484, 1.10146153531332E-07, 80.1317443847656, 7720.49267578125, 1004.94738769531, 1.70793741427261E-14, 3.03710174560547, 5.71290826797485, 5.10889339447021, 9.58583164215088, 167.42839050293, 2.4015116650844E-06, 258.602661132813, 1.0376603398754E-07, 1, 0.367879450321198, 61.6596221923828, 0.000388831656891853, 1.24317634105682, 0.327922433614731, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 0, 0, 16.0212802886963, -8.73417568206787, 31.6999568939209, -1.52512669563293, 7.85315942764282, 1.33304369449615, -2.23371720314026, 13.6999225616455, 16.1664524078369, 0, 0, 1, 1, 0, 0, 1, 0, 1, 256.681427001953, 1.10165693456565E-07, 76.2858276367188, 6211.61181640625, 1004.88726806641, 1.70955728008093E-14, 2.32601141929626, 4.59572601318359, 4.98949241638184, 9.33450031280518, 187.687881469727, 1.12253326278733E-06, 261.354187011719, 9.52793826058951E-08, 1, 0.367879450321198, 61.6721115112305, 0.000388522516004741, 1.77700543403625, 0.263673484325409, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 0, -2, 15.9689350128174, -7.83264684677124, 31.6375694274902, -0.607630908489227, 7.96865367889404, 2.28794169425964, -1.09942674636841, 12.8561420440674, 16.0405445098877, 0, 0, 1, 1, 0, 0, 1, 4, 7.38905620574951, 255.006881713867, 1.16085942636346E-07, 61.3503570556641, 2521.5947265625, 1000.93579101563, 1.81960939100567E-14, 0.369215309619904, 1.83607637882233, 1.20873916149139, 3.00244426727295, 165.280395507813, 2.61004765889084E-06, 257.299072265625, 1.08063751724785E-07, 1, 0.367879450321198, 63.4994430541992, 0.000346144690411165, 5.2346773147583, 0.101475112140179, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 0, -2, 15.7725219726563, -6.18729543685913, 31.6788959503174, 1.06329429149628, 8.23021030426025, 3.97376227378845, 0.688895702362061, 12.8045082092285, 16.0368938446045, 0, 0, 1, 1, 0, 0, 1, 4, 7.38905620574951, 248.772445678711, 1.4128001168956E-07, 38.2826232910156, 486.528472900391, 1003.55242919922, 1.7459439520225E-14, 1.1305947303772, 0.345316350460052, 0.474577277898788, 0.502130270004272, 163.955429077148, 2.74835451818944E-06, 257.181976318359, 1.08458976910697E-07, 1, 0.367879450321198, 67.7363586425781, 0.000266480288701132, 15.7907867431641, 0.0188025590032339, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 2, -2, 16.1303768157959, -4.83644771575928, 30.556324005127, 2.50819730758667, 7.88676404953003, 4.2143931388855, 2.24188351631165, 27.5528507232666, 15.4230127334595, 0, 0, 1, 1, 0, 4, 0.13533528149128, 4, 7.38905620574951, 260.189056396484, 9.87793953299843E-08, 23.3912258148193, 126.020896911621, 933.68896484375, 5.36484410769509E-14, 6.29105377197266, 0.0814148709177971, 5.02604150772095, 0.106258176267147, 759.159606933594, 1.0813069190041E-12, 237.869323730469, 2.00387518134448E-07, 1, 0.367879450321198, 62.2010459899902, 0.000375683302991092, 17.7611103057861, 0.0147812888026237, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 2, 0, 16.2119674682617, -4.62169504165649, 29.7894287109375, 2.82443904876709, 7.93790006637573, 4.90562677383423, 2.68020558357239, 26.5600395202637, 15.0680046081543, 0, 0, 1, 1, 0, 4, 0.13533528149128, 0, 1, 262.827880859375, 9.10399435838372E-08, 21.3600654602051, 101.666213989258, 887.410034179688, 1.15508907235114E-13, 7.97745609283447, 0.0593419335782528, 7.18350219726563, 0.0685490593314171, 705.435668945313, 2.91824268743024E-12, 227.044769287109, 2.85791116994005E-07, 1, 0.367879450321198, 63.0102577209473, 0.000356955279130489, 24.0651741027832, 0.00740480050444603, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 2, 0, 16.2696895599365, -4.6183180809021, 29.0513858795166, 2.94138145446777, 8.24777412414551, 5.38568067550659, 2.92890810966492, 25.1033306121826, 14.6790084838867, 0, 0, 1, 1, 0, 4, 0.13533528149128, 0, 1, 264.702789306641, 8.59337205838528E-08, 21.3288612365723, 101.323471069336, 843.983032226563, 2.4162586536626E-13, 8.65172481536865, 0.0527927465736866, 8.5785026550293, 0.0534553751349449, 630.177185058594, 1.25245473714397E-11, 215.473297119141, 4.21684489992913E-07, 1, 0.367879450321198, 68.0257797241211, 0.000261840730672702, 29.0055561065674, 0.00458172056823969, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 2, -2, 15.0370044708252, -2.23827481269836, 29.1831932067871, 5.56872797012329, 10.2896995544434, 8.22345066070557, 5.73420238494873, 37.7330856323242, 14.7281246185303, 0, 0, 1, 1, 0, 4, 0.13533528149128, 4, 7.38905620574951, 226.11149597168, 2.94789458621381E-07, 5.00987434387207, 9.37714004516602, 851.658752441406, 2.11787456062124E-13, 31.0107307434082, 0.00381533056497574, 32.8810768127441, 0.00323346047662199, 1423.78576660156, 4.09948329301116E-17, 216.91764831543, 4.01473386091311E-07, 1, 0.367879450321198, 105.877914428711, 3.39813232130837E-05, 67.6251373291016, 0.000268287694780156, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 2, 0, 13.9362535476685, -2.04516673088074, 29.8177394866943, 5.89225482940674, 11.7418537139893, 8.4976053237915, 2.68637633323669, 18.5636329650879, 21.9541530609131, 0, 0, 1, 1, 0, 4, 0.13533528149128, 0, 1, 194.219161987305, 8.86261716459558E-07, 4.18270683288574, 7.73044729232788, 889.097595214844, 1.1228461872315E-13, 34.7186660766602, 0.00276074465364218, 7.21661758422852, 0.0681273639202118, 344.608459472656, 8.66795524245845E-09, 481.984832763672, 2.92033369708733E-10, 1, 0.367879450321198, 137.871124267578, 7.95385585661279E-06, 72.2092971801758, 0.000203956195036881, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 0, 0, 13.9275999069214, -2.0436327457428, 29.8231410980225, 5.89475393295288, 11.7533168792725, 8.49961853027344, 2.68898153305054, 18.5359649658203, 21.9524555206299, 0, 0, 1, 1, 0, 0, 1, 0, 1, 193.978042602539, 8.93964397619129E-07, 4.1764349937439, 7.71859788894653, 889.419738769531, 1.11679735554857E-13, 34.7481231689453, 0.0027538537979126, 7.23062181472778, 0.0679501071572304, 343.582000732422, 8.91112872380972E-09, 481.910308837891, 2.92529528378438E-10, 1, 0.367879450321198, 138.14045715332, 7.86320015322417E-06, 72.2435150146484, 0.000203546005650423, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 2, -2, 12.3403081893921, 1.13194251060486, 31.4992694854736, -8.92325973510742, 12.9020881652832, -6.60931539535522, 6.21320390701294, 18.395938873291, 22.3536701202393, 0, 0, 1, 1, 0, 4, 0.13533528149128, 4, 7.38905620574951, 152.283203125, 4.37192056779168E-06, 1.28129386901855, 0.322406381368637, 992.203979492188, 2.08949373916526E-14, 79.6245651245117, 7504.51220703125, 38.6039009094238, 0.0020028103608638, 338.410552978516, 1.02505053334312E-08, 499.686553955078, 1.95850377582296E-10, 1, 0.367879450321198, 166.463882446289, 2.49283948505763E-06, 43.683048248291, 741.974914550781, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 2, -2, 10.6407413482666, 1.144078373909, 33.1791610717773, -8.87888240814209, 14.3632564544678, -6.78769445419312, 6.66027021408081, 31.5851936340332, 22.7248210906982, 0, 0, 1, 1, 0, 4, 0.13533528149128, 4, 7.38905620574951, 113.225379943848, 2.39212986343773E-05, 1.30891537666321, 0.318517327308655, 1100.85668945313, 3.89469476092495E-15, 78.8345565795898, 7178.76318359375, 44.3591995239258, 0.00128080020658672, 997.624450683594, 1.91745287724844E-14, 516.41748046875, 1.35124966771372E-10, 1, 0.367879450321198, 206.303131103516, 5.78251786009787E-07, 46.0727958679199, 886.866516113281, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 2, -2, 9.51274871826172, 2.50688695907593, 34.6307945251465, -7.56981897354126, 15.0769958496094, -5.70046281814575, 8.15421199798584, 34.9211959838867, 23.4378471374512, 0, 0, 1, 1, 0, 4, 0.13533528149128, 4, 7.38905620574951, 90.4923858642578, 7.3903618613258E-05, 6.2844820022583, 0.0815216228365898, 1199.29187011719, 9.12088571614338E-16, 57.3021583557129, 1938.78930664063, 66.4911727905273, 0.000287521776044741, 1219.48986816406, 6.82208753108527E-16, 549.332702636719, 6.62326790634715E-11, 1, 0.367879450321198, 227.315811157227, 2.83233021036722E-07, 32.4952774047852, 299.005767822266, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 },
                new[] { 2, 2, 8.61646747589111, 1.61963295936584, 35.7087860107422, -8.47996139526367, 15.783821105957, -6.74762439727783, 7.48704767227173, 40.1082496643066, 23.8930282592773, 0, 0, 1, 1, 0, 4, 0.13533528149128, 4, 0.13533528149128, 74.2435150146484, 0.000181098861503415, 2.62321090698242, 0.197971343994141, 1275.11743164063, 3.10363962711629E-16, 71.9097442626953, 4817.26416015625, 56.0558815002441, 0.000560294720344245, 1608.67163085938, 3.8124882645564E-18, 570.876770019531, 4.2013576645461E-11, 1, 0.367879450321198, 249.129013061523, 1.39692659217872E-07, 45.5304336547852, 852.032287597656, 1, 0.367879450321198, 0, 1, 0, 1, 0, 1 }
            };

            // Class labels for each of the inputs
            int[] outputs =
            {
                0, 0, 0, 1, 0, 2, 2, 2, 2, 3, 0, 3, 0, 0, 1, 4, 4, 3, 4, 3, 0, 2, 3, 5, 0
            };

            // Create a estimation algorithm to estimate the regression
            LowerBoundNewtonRaphson lbnr = new LowerBoundNewtonRaphson()
            {
                MaxIterations = 100,
                Tolerance = 1e-6
            };

            // Now, we will iteratively estimate our model:
            MultinomialLogisticRegression mlr = lbnr.Learn(inputs, outputs);

            // We can compute the model answers
            int[] answers = mlr.Decide(inputs);

            // And also the probability of each of the answers
            double[][] probabilities = mlr.Probabilities(inputs);
            Assert.IsFalse(probabilities.HasNaN());

            // Now we can check how good our model is at predicting
            double error = new ZeroOneLoss(outputs).Loss(answers);

            // We can also verify the classes with highest 
            // probability are the ones being decided for:
            int[] argmax = probabilities.ArgMax(dimension: 1); // should be same as 'answers

            double[] testInput = { 2, 2, 0.4309521, 8.443765, 46.42793, -1.915977, 22.90634, -0.532284, -3.313488, 63.01553, 29.22279, 0, 0, 1, 1, 0, 4, 0.1353353, 4, 0.1353353, 0.1857197, 0.6498901, 71.29716, 0.0002152383, 2155.552, 6.864464E-21, 3.67097, 6.793576, 10.9792, 27.48081, 3970.957, 4.29243E-28, 853.9717, 2.035645E-13, 1, 0.3678795, 524.7004, 1.126946E-10, 0.2833262, 1.702817, 1, 0.3678795, 0, 1, 0, 1, 0, 1 };
            double[] testInputProbs = mlr.Probabilities(testInput);

            Assert.IsFalse(testInputProbs.HasNaN());

            double[] testInputLogs = mlr.LogLikelihoods(testInput);

            double[] expected = Matrix.Exp(testInputLogs.Subtract(Special.LogSumExp(testInputLogs)));
            double[] actual = testInputProbs;

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(new double[] { 0, 0, 1, 0, 0, 0 }, actual);
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

        public static void CreateInputOutputsExample1(out double[][] inputs, out int[] outputs)
        {
            inputs = example1.Submatrix(null, 1, 2).ToJagged();
            outputs = example1.Submatrix(null, 0, 0).Reshape(0).Select(x => (int)x - 1).ToArray();
        }

        public static void CreateInputOutputsExample2(out double[][] inputs, out int[] outputs)
        {
            inputs = example1.Submatrix(null, 1, 2).ToJagged().InsertColumn().InsertColumn().InsertColumn();
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
