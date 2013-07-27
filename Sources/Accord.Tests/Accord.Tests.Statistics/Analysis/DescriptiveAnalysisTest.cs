// Accord Unit Tests
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Math;
    using AForge;

    [TestClass()]
    public class DescriptiveAnalysisTest
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod()]
        public void DescriptiveAnalysisConstructorTest1()
        {
            // Suppose we would like to compute descriptive
            // statistics from the following data samples:
            double[,] data =
            {
                { 1, 52, 5 },
                { 2, 12, 5 },
                { 1, 65, 5 },
                { 1, 25, 5 },
                { 2, 62, 5 },
            };

            // Create the analysis
            DescriptiveAnalysis analysis = new DescriptiveAnalysis(data);

            // Compute
            analysis.Compute();

            // Retrieve interest measures
            double[] means = analysis.Means; // { 1.4, 43.2, 5.0 }
            double[] modes = analysis.Modes; // { 1.0, 52.0, 5.0 }

            Assert.AreEqual(3, means.Length);
            Assert.AreEqual(1.4, means[0]);
            Assert.AreEqual(43.2, means[1]);
            Assert.AreEqual(5.0, means[2]);

            Assert.AreEqual(1.0, modes[0]);
            Assert.AreEqual(52.0, modes[1]);
            Assert.AreEqual(5.0, modes[2]);
        }

        [TestMethod()]
        public void DescriptiveAnalysisConstructorTest()
        {
            double[,] data = Matrix.Magic(3);
            string[] columnNames = { "x", "y", "z" };
            DescriptiveAnalysis target = new DescriptiveAnalysis(data, columnNames);
            target.Compute();

            Assert.AreEqual("x", target.ColumnNames[0]);
            Assert.AreEqual("y", target.ColumnNames[1]);
            Assert.AreEqual("z", target.ColumnNames[2]);


            Assert.IsTrue(target.CorrelationMatrix.IsEqual(new double[,]
            {
                {  1.0000,   -0.7559,    0.1429 },
                { -0.7559,    1.0000,   -0.7559 },
                {  0.1429,   -0.7559,    1.0000 },
            }, 0.0001));

            Assert.IsTrue(target.CovarianceMatrix.IsEqual(new double[,]
            {
                {  7,    -8,     1 },
                { -8,    16,    -8 },
                {  1,    -8,     7 },
            }, 0.00000001));

            Assert.IsTrue(target.StandardScores.IsEqual(new double[,]
            { 
                { 1.1339,   -1.0000,    0.3780 },
                { -0.7559,         0,    0.7559 },
                { -0.3780,    1.0000,   -1.1339 },
            }, 0.001));

            Assert.IsTrue(target.Means.IsEqual(new double[] { 5, 5, 5 }));

            Assert.IsTrue(target.StandardDeviations.IsEqual(new double[] { 2.6458, 4.0000, 2.6458 }, 0.001));

            Assert.IsTrue(target.Medians.IsEqual(new double[] { 4, 5, 6 }));


            Assert.AreEqual(3, target.Ranges[0].Min);
            Assert.AreEqual(8, target.Ranges[0].Max);
            Assert.AreEqual(1, target.Ranges[1].Min);
            Assert.AreEqual(9, target.Ranges[1].Max);
            Assert.AreEqual(2, target.Ranges[2].Min);
            Assert.AreEqual(7, target.Ranges[2].Max);

            Assert.AreEqual(3, target.Samples);
            Assert.AreEqual(3, target.Variables);

            Assert.IsTrue(target.Source.IsEqual(Matrix.Magic(3)));

            Assert.IsTrue(target.Sums.IsEqual(new double[] { 15, 15, 15 }));

            Assert.IsTrue(target.Variances.IsEqual(new double[] { 7, 16, 7 }));
        }
   
    }
}
