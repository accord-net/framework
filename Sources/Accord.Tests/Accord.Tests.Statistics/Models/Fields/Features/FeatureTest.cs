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

namespace Accord.Tests.Statistics.Models.Fields
{
    using Accord.Statistics.Models.Fields.Features;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Tests.Statistics.Models.Fields;

    [TestClass()]
    public class InitialFeatureTest
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



        [TestMethod()]
        public void ComputeTest()
        {
            var hmm = HiddenMarkovClassifierPotentialFunctionTest.CreateModel1();

            IPotentialFunction<int> owner = new MarkovDiscreteFunction(hmm);

            int[] x = new int[] { 0, 0, 1, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 0 };

            foreach (var factor in owner.Factors)
            {
                for (int y = 0; y < owner.Outputs; y++)
                {
                    double[,] fwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.Forward(factor, x, y);

                    double[,] bwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.Backward(factor, x, y);

                    double[,] lnfwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.LogForward(factor, x, y);

                    double[,] lnbwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.LogBackward(factor, x, y);

                    for (int i = 0; i < fwd.GetLength(0); i++)
                        for (int j = 0; j < fwd.GetLength(1); j++)
                            Assert.AreEqual(System.Math.Log(fwd[i, j]), lnfwd[i, j], 1e-10);

                    for (int i = 0; i < bwd.GetLength(0); i++)
                        for (int j = 0; j < bwd.GetLength(1); j++)
                            Assert.AreEqual(System.Math.Log(bwd[i, j]), lnbwd[i, j], 1e-10);


                    foreach (var feature in factor)
                    {
                        double expected = System.Math.Log(feature.Marginal(fwd, bwd, x, y));
                        double actual = feature.LogMarginal(lnfwd, lnbwd, x, y);

                        Assert.AreEqual(expected, actual, 1e-10);
                        Assert.IsFalse(Double.IsNaN(actual));
                    }

                }
            }
        }

        [TestMethod()]
        public void ComputeTest2()
        {
            var hmm = NormalHiddenMarkovClassifierPotentialFunctionTest.CreateModel1();

            IPotentialFunction<double> owner = new MarkovContinuousFunction(hmm);


            double[] x = new double[] { 0, 1, 2, 1, 7, 2, 1, 2, 5, 3, 4 };

            foreach (var factor in owner.Factors)
            {
                for (int y = 0; y < owner.Outputs; y++)
                {
                    double[,] fwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.Forward(factor, x, y);

                    double[,] bwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.Backward(factor, x, y);

                    double[,] lnfwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.LogForward(factor, x, y);

                    double[,] lnbwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.LogBackward(factor, x, y);


                    for (int i = 0; i < fwd.GetLength(0); i++)
                        for (int j = 0; j < fwd.GetLength(1); j++)
                            Assert.AreEqual(System.Math.Log(fwd[i, j]), lnfwd[i, j], 1e-10);

                    for (int i = 0; i < bwd.GetLength(0); i++)
                        for (int j = 0; j < bwd.GetLength(1); j++)
                            Assert.AreEqual(System.Math.Log(bwd[i, j]), lnbwd[i, j], 1e-10);

                    foreach (var feature in factor)
                    {
                        double expected = System.Math.Log(feature.Marginal(fwd, bwd, x, y));
                        double actual = feature.LogMarginal(lnfwd, lnbwd, x, y);

                        Assert.AreEqual(expected, actual, 1e-10);
                        Assert.IsFalse(Double.IsNaN(actual));
                    }
                }
            }
        }

        [TestMethod()]
        public void ComputeTest3()
        {
            var hmm = MultivariateNormalHiddenMarkovClassifierPotentialFunctionTest.CreateModel1();

            var owner = new MarkovMultivariateFunction(hmm);


            double[][] x = 
            { 
                new double[] { 0 },
                new double[] { 1 },
                new double[] { 3 },
                new double[] { 1 },
                new double[] { 2 },
                new double[] { 8 },
                new double[] { 0 },
                new double[] { 10 },
                new double[] { System.Math.PI },
            };

            foreach (var factor in owner.Factors)
            {
                for (int y = 0; y < owner.Outputs; y++)
                {
                    double[,] fwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.Forward(factor, x, y);

                    double[,] bwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.Backward(factor, x, y);

                    double[,] lnfwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.LogForward(factor, x, y);

                    double[,] lnbwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.LogBackward(factor, x, y);


                    for (int i = 0; i < fwd.GetLength(0); i++)
                        for (int j = 0; j < fwd.GetLength(1); j++)
                            Assert.AreEqual(System.Math.Log(fwd[i, j]), lnfwd[i, j], 1e-10);

                    for (int i = 0; i < bwd.GetLength(0); i++)
                        for (int j = 0; j < bwd.GetLength(1); j++)
                            Assert.AreEqual(System.Math.Log(bwd[i, j]), lnbwd[i, j], 1e-10);


                    foreach (var feature in factor)
                    {
                        double expected = System.Math.Log(feature.Marginal(fwd, bwd, x, y));
                        double actual = feature.LogMarginal(lnfwd, lnbwd, x, y);

                        Assert.AreEqual(expected, actual, 1e-10);
                        Assert.IsFalse(Double.IsNaN(actual));
                    }

                }
            }
        }

    }
}
