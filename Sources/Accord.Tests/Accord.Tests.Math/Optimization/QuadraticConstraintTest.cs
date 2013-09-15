

namespace Accord.Tests.Math
{
    using Accord.Math.Optimization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Math.Differentiation;
    using Accord.Math;


    [TestClass()]
    public class QuadraticConstraintTest
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
        public void QuadraticConstraintConstructorTest()
        {
            IObjectiveFunction objective = null;

            double[,] quadraticTerms = 
            {
                {  1, 2, 3 },
                {  4, 5, 6 },
                {  7, 8, 9 },
            };

            double[] linearTerms = { 1, 2, 3 };

            objective = new NonlinearObjectiveFunction(3, f => f[0] + f[1] + f[2]);

            QuadraticConstraint target = new QuadraticConstraint(objective,
                quadraticTerms, linearTerms,
                ConstraintType.LesserThanOrEqualTo, 0);

            var function = target.Function;
            var gradient = target.Gradient;

            FiniteDifferences fd = new FiniteDifferences(3, function);

            double[][] x =
            {
                new double[] { 1, 2, 3 },
                new double[] { 3, 1, 4 },
                new double[] { -6 , 5, 9 },
                new double[] { 31, 25, 246 },
                new double[] { -0.102, 0, 10 },
            };


            { // Function test
                for (int i = 0; i < x.Length; i++)
                {
                    double expected =
                        (x[i].Multiply(quadraticTerms)).InnerProduct(x[i])
                        + linearTerms.InnerProduct(x[i]);

                    double actual = function(x[i]);

                    Assert.AreEqual(expected, actual, 1e-8);
                }
            }

            { // Gradient test
                for (int i = 0; i < x.Length; i++)
                {
                    double[] expected = fd.Compute(x[i]);
                    double[] actual = gradient(x[i]);

                    for (int j = 0; j < actual.Length; j++)
                        Assert.AreEqual(expected[j], actual[j], 1e-8);
                }
            }


        }
    }
}
