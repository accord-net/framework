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

using Accord.Statistics.Visualizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for HistogramTest and is intended
    ///to contain all HistogramTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HistogramTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        public void HistogramConstructorTest()
        {
            Histogram target = new Histogram();
            Assert.IsNotNull(target.Bins);
            Assert.IsNotNull(target.Values);
            Assert.AreEqual(0, target.Bins.Count);
        }

        [TestMethod()]
        public void ComputeTest1()
        {
            Histogram target = new Histogram();
            double[] data = new double[] { 200.0, 200.0, 200.0 };

            target.AutoAdjustmentRule = BinAdjustmentRule.Scott;

            target.Compute(data);

            Assert.AreEqual(1, target.Bins.Count);
            Assert.AreEqual(3, target.Bins[0].Value);
            Assert.AreEqual(0.0, target.Bins[0].Width);

            Assert.IsTrue(target.Bins[0].Contains(200));
            Assert.IsFalse(target.Bins[0].Contains(201));

            Assert.AreEqual(target.Bins[0], target.Bins.Search(200));

        }

        [TestMethod()]
        public void ComputeTest2()
        {
            Histogram target = new Histogram();
            double[] data = new double[] { };

            target.Compute(data);

            Assert.AreEqual(0, target.Bins.Count);
            Assert.AreEqual(0.0, target.Range.Min);
            Assert.AreEqual(0.0, target.Range.Max);
        }



        [TestMethod()]
        public void ComputeTest3()
        {
            Histogram target;
            HistogramBin bin;

            double[] values = { 1, 2, 2, 3, 3, 3, 3, 4, 4, 5, 6 };

            target = new Histogram("histogram");
            target.Compute(values, 5);

            Assert.AreEqual("histogram", target.Title);

            Assert.AreEqual(1, target.Range.Min);
            Assert.AreEqual(6, target.Range.Max);

            Assert.AreEqual(5, target.Bins.Count);

            bin = target.Bins[0];
            Assert.AreEqual(1, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 1 && 1 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(1));
            Assert.IsFalse(bin.Contains(2));
            Assert.AreEqual(bin, target.Bins.Search(1));
            Assert.AreEqual(0, target.Bins.SearchIndex(1));

            bin = target.Bins[1];
            Assert.AreEqual(2.0, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 2 && 2 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(2));
            Assert.IsFalse(bin.Contains(3));
            Assert.AreEqual(bin, target.Bins.Search(2));
            Assert.AreEqual(1, target.Bins.SearchIndex(2));

            bin = target.Bins[2];
            Assert.AreEqual(4.0, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 3 && 3 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(3));
            Assert.IsFalse(bin.Contains(4));
            Assert.AreEqual(bin, target.Bins.Search(3));
            Assert.AreEqual(2, target.Bins.SearchIndex(3));

            bin = target.Bins[3];
            Assert.AreEqual(2.0, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 4 && 4 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(4));
            Assert.IsFalse(bin.Contains(5));
            Assert.AreEqual(bin, target.Bins.Search(4));
            Assert.AreEqual(3, target.Bins.SearchIndex(4));

            bin = target.Bins[4];
            Assert.AreEqual(2.0, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 5 && 5 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(5));
            Assert.IsTrue(bin.Contains(6));
            Assert.IsFalse(bin.Contains(4));
            Assert.AreEqual(bin, target.Bins.Search(5));
            Assert.AreEqual(4, target.Bins.SearchIndex(5));

            int sum = 0;
            for (int i = 0; i < target.Values.Length; i++)
                sum += target.Values[i];

            Assert.AreEqual(values.Length, sum);
        }

        [TestMethod()]
        public void ComputeTest4()
        {
            Histogram target;
            HistogramBin bin;

            double[] values = { 1, 2, 2, 3, 3, 3, 3, 4, 4, 5, 6 };

            target = new Histogram("histogram");
            target.Compute(values, 5, true);

            Assert.AreEqual("histogram", target.Title);

            Assert.AreEqual(1, target.Range.Min);
            Assert.AreEqual(7, target.Range.Max);

            Assert.AreEqual(6, target.Bins.Count);

            bin = target.Bins[0];
            Assert.AreEqual(1, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 1 && 1 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(1));
            Assert.IsFalse(bin.Contains(2));
            Assert.AreEqual(bin, target.Bins.Search(1));
            Assert.AreEqual(0, target.Bins.SearchIndex(1));

            bin = target.Bins[1];
            Assert.AreEqual(2.0, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 2 && 2 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(2));
            Assert.IsFalse(bin.Contains(3));
            Assert.AreEqual(bin, target.Bins.Search(2));
            Assert.AreEqual(1, target.Bins.SearchIndex(2));

            bin = target.Bins[2];
            Assert.AreEqual(4.0, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 3 && 3 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(3));
            Assert.IsFalse(bin.Contains(4));
            Assert.AreEqual(bin, target.Bins.Search(3));
            Assert.AreEqual(2, target.Bins.SearchIndex(3));

            bin = target.Bins[3];
            Assert.AreEqual(2.0, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 4 && 4 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(4));
            Assert.IsFalse(bin.Contains(5));
            Assert.AreEqual(bin, target.Bins.Search(4));
            Assert.AreEqual(3, target.Bins.SearchIndex(4));

            bin = target.Bins[4];
            Assert.AreEqual(1.0, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 5 && 5 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(5));
            Assert.IsFalse(bin.Contains(6));
            Assert.AreEqual(bin, target.Bins.Search(5));
            Assert.AreEqual(4, target.Bins.SearchIndex(5));

            int sum = 0;
            for (int i = 0; i < target.Values.Length; i++)
                sum += target.Values[i];

            Assert.AreEqual(values.Length, sum);
        }

        [TestMethod()]
        public void ComputeTest5()
        {
            Histogram target;
            HistogramBin bin;

            double[] values = { 1, 2, 2, 3, 3, 3, 3, 4, 4, 5, 6 };

            target = new Histogram("histogram");
            target.Compute(values, 6, 1.0);

            Assert.AreEqual(1, target.Range.Min);
            Assert.AreEqual(7, target.Range.Max);
            Assert.AreEqual(6, target.Bins.Count);

            bin = target.Bins[0];
            Assert.AreEqual(1, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 1 && 1 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(1));
            Assert.IsFalse(bin.Contains(2));
            Assert.AreEqual(bin, target.Bins.Search(1));
            Assert.AreEqual(0, target.Bins.SearchIndex(1));

            bin = target.Bins[1];
            Assert.AreEqual(2.0, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 2 && 2 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(2));
            Assert.IsFalse(bin.Contains(3));
            Assert.AreEqual(bin, target.Bins.Search(2));
            Assert.AreEqual(1, target.Bins.SearchIndex(2));

            bin = target.Bins[2];
            Assert.AreEqual(4.0, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 3 && 3 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(3));
            Assert.IsFalse(bin.Contains(4));
            Assert.AreEqual(bin, target.Bins.Search(3));
            Assert.AreEqual(2, target.Bins.SearchIndex(3));

            bin = target.Bins[3];
            Assert.AreEqual(2.0, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 4 && 4 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(4));
            Assert.IsFalse(bin.Contains(5));
            Assert.AreEqual(bin, target.Bins.Search(4));
            Assert.AreEqual(3, target.Bins.SearchIndex(4));

            bin = target.Bins[4];
            Assert.AreEqual(1.0, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 5 && 5 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(5));
            Assert.IsFalse(bin.Contains(6));
            Assert.AreEqual(bin, target.Bins.Search(5));
            Assert.AreEqual(4, target.Bins.SearchIndex(5));

            bin = target.Bins[5];
            Assert.AreEqual(1.0, bin.Value);
            Assert.AreEqual(1.0, bin.Width);
            Assert.IsTrue(bin.Range.Min <= 6 && 6 < bin.Range.Max);
            Assert.IsTrue(bin.Contains(6));
            Assert.IsFalse(bin.Contains(5));
            Assert.AreEqual(bin, target.Bins.Search(6));
            Assert.AreEqual(5, target.Bins.SearchIndex(6));

            int sum = 0;
            for (int i = 0; i < target.Values.Length; i++)
                sum += target.Values[i];

            Assert.AreEqual(values.Length, sum);
        }

        [TestMethod()]
        public void ComputeTest6()
        {
            Histogram target = new Histogram();

            double[] data = { 0.0, 0.005, 0.124, 0.0, 0.004, 0.0, 0.111, 0.112 };

            target.Compute(data, 4);

            Assert.AreEqual(5, target.Edges.Length);
            Assert.AreEqual(0.000, target.Edges[0]);
            Assert.AreEqual(0.031, target.Edges[1]);
            Assert.AreEqual(0.062, target.Edges[2]);
            Assert.AreEqual(0.093, target.Edges[3]);
            Assert.AreEqual(0.124, target.Edges[4]);

        }

    }
}
