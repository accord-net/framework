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
    using System;
    using Accord.Math.Differentiation;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Models.Fields;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Fields.Learning;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Tests.Statistics.Models.Fields;
    using NUnit.Framework;

    [TestFixture]
    public class MultivariateNormalQuasiNewtonHiddenLearningTest
    {


        public static double[][][] inputs1 = new double[][][]
        {
            new double[][] 
            { 
                new double[] { 0 },
                new double[] { 1 },
                new double[] { 2 },
                new double[] { 3 },
                new double[] { 4 },
            }, 

            new double[][]
            {
                new double[] { 4 },
                new double[] { 3 },
                new double[] { 2 },
                new double[] { 1 },
                new double[] { 0 },
            }
        };

        public static int[] outputs1 = { 0, 1 };



        [Test]
        public void RunTest()
        {
            var hmm = MultivariateMarkovFunctionTest.CreateModel1();
            var function = new MarkovMultivariateFunction(hmm);

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new HiddenQuasiNewtonLearning<double[]>(model);

            var inputs = inputs1;
            var outputs = outputs1;

            double[] actual = new double[inputs.Length];
            double[] expected = new double[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            double llm = hmm.LogLikelihood(inputs, outputs);
            double ll0 = model.LogLikelihood(inputs, outputs);
            Assert.AreEqual(llm, ll0, 1e-10);
            Assert.IsFalse(double.IsNaN(llm));
            Assert.IsFalse(double.IsNaN(ll0));

            double error = target.Run(inputs, outputs);
            double ll1 = model.LogLikelihood(inputs, outputs);
            Assert.AreEqual(-ll1, error, 1e-10);
            Assert.IsFalse(double.IsNaN(ll1));
            Assert.IsFalse(double.IsNaN(error));


            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }

            Assert.AreEqual(-0.0000041736023117522336, ll0, 1e-10);
            
            Assert.AreEqual(error, -ll1);
            Assert.IsFalse(Double.IsNaN(ll0));
            Assert.IsFalse(Double.IsNaN(error));

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            Assert.IsTrue(ll1 > ll0);
        }

    }
}
