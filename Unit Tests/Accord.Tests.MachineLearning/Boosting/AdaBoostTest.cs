// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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

namespace Accord.Tests.MachineLearning
{
    using System;
    using Accord.MachineLearning.Boosting;
    using Accord.MachineLearning.Boosting.Learners;
    using Accord.Math;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using NUnit.Framework;


    [TestFixture]
    public class AdaBoostTest
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




        [Test]
        public void ConstructorTest()
        {
            double[][] inputs =
            {
                new double[] { 10, 42 },
                new double[] { 162, 96 },
                new double[] { 125, 20 },
                new double[] { 96, 6 },
                new double[] { 2, 73 },
                new double[] { 52, 51 },
                new double[] { 71, 49 },
            };

            int[] outputs = 
            {
                -1, -1, +1, +1, -1, -1, +1
            };


            var classifier = new Boost<DecisionStump>();

            var teacher = new AdaBoost<DecisionStump>(classifier)
            {
                Creation = (weights) =>
                {
                    var stump = new DecisionStump(2);
                    stump.Learn(inputs, outputs, weights);
                    return stump;
                },

                Iterations = 5,
                Tolerance = 1e-3
            };


            double error = teacher.Run(inputs, outputs);

            Assert.AreEqual(0, error);

            Assert.AreEqual(5, classifier.Models.Count);
            Assert.AreEqual(0.16684734250395147, classifier.Models[0].Weight);
            Assert.AreEqual(0.22329026900109736, classifier.Models[1].Weight);
            Assert.AreEqual(0.28350372170582383, classifier.Models[2].Weight);
            Assert.AreEqual(0.16684734250395139, classifier.Models[3].Weight);
            Assert.AreEqual(0.15951132428517592, classifier.Models[4].Weight);

            int[] actual = new int[outputs.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = classifier.Compute(inputs[i]);

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(outputs[i], actual[i]);
        }

        [Test]
        public void ConstructorTest2()
        {
            double[][] inputs = LeastSquaresLearningTest.yinyang.GetColumns(new[] { 0, 1 }).ToJagged();
            int[] outputs = LeastSquaresLearningTest.yinyang.GetColumn(2).ToInt32();

            var outputs2 = outputs.Apply(x => x > 0 ? 1.0 : 0.0);

            var classifier = new Boost<Weak<LogisticRegression>>();

            var teacher = new AdaBoost<Weak<LogisticRegression>>(classifier)
            {
                Creation = (weights) =>
                {
                    LogisticRegression reg = new LogisticRegression(2, intercept: 1);

                    IterativeReweightedLeastSquares irls = new IterativeReweightedLeastSquares(reg)
                    {
                        ComputeStandardErrors = false
                    };

                    for (int i = 0; i < 50; i++)
                        irls.Run(inputs, outputs2, weights);

                    return new Weak<LogisticRegression>(reg, (s, x) => Math.Sign(s.Compute(x) - 0.5));
                },

                Iterations = 50,
                Tolerance = 1e-5,
            };



            double error = teacher.Run(inputs, outputs);


            Assert.AreEqual(0.11, error);

            Assert.AreEqual(2, classifier.Models.Count);
            Assert.AreEqual(0.63576818449825168, classifier.Models[0].Weight);
            Assert.AreEqual(0.36423181550174832, classifier.Models[1].Weight);

            int[] actual = new int[outputs.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = classifier.Compute(inputs[i]);

            //for (int i = 0; i < actual.Length; i++)
            //    Assert.AreEqual(outputs[i], actual[i]);
        }


    }
}