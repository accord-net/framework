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
    using Accord.Statistics.Visualizations;
    using NUnit.Framework;

    [TestFixture]
    public class HistogramTest
    {

        [Test]
        public void example1()
        {
            #region doc_example1
            // Let's say we would like to create a new histogram
            // visualization representing the following values:
            double[] data = { 0.0, 0.005, 0.124, 0.0, 0.004, 0.0, 0.111, 0.112 };

            // Create a new histogram object
            var histogram = new Histogram()
            {
                Title = "My Histogram", // like a DataTable, we can give a name to the histogram

                // If we don't want to specify how many bins the histogram should have,
                // we can select any of the following bin adjustment rules to determine
                // the number of bins for us (where N is the number of samples):
                AutoAdjustmentRule = BinAdjustmentRule.Scott,         // 3.49*σ*N, where σ is the std. dev.
                // AutoAdjustmentRule = BinAdjustmentRule.SquareRoot  // sqrt(N)
                // AutoAdjustmentRule = BinAdjustmentRule.Sturges     // ceiling(log2(N) + 1)
                Cumulative = false, // whether to compute a cumulative histogram or normal histogram
                InclusiveUpperBound = true // whether to force the inclusion of the last bin or not
            };

            // Update the histogram
            histogram.Compute(data);

            // We can determine the number of bins according to our chosen rule
            int numberOfBins = histogram.Bins.Count; // should be 2

            // At this point, we can check where the bin edges would have been:
            double[] edges = histogram.Edges; // should be { 0, 0.062, 0.124 }

            // We can also inspect individual bins and their particular properties:
            DoubleRange firstBinRange = histogram.Bins[0].Range;// should be (0, 0.062)
            int value = histogram.Bins[0].Value;    // should be 5
            double width = histogram.Bins[0].Width; // should be 0.062

            // We can plot the histogram in WinForms (needs a reference to Accord.Controls.dll):
            // HistogramBox.Show(histogram).Hold();

            // If we would like to plot the histogram in another platform, we can inspect 
            // the histogram's bin count, widths and ranges to determine how such histogram 
            // should have been plotted. If you would like to plot this histogram in WebForms 
            // for example, please take a look at the source code for "HistogramView.cs"
            #endregion

            Assert.AreEqual(2, numberOfBins);
            Assert.AreEqual(new[] { 0, 0.062, 0.124 }, edges);
            Assert.AreEqual(new DoubleRange(0, 0.062), firstBinRange);
            Assert.AreEqual(5, value);
            Assert.AreEqual(0.062, width);
        }

        [Test]
        public void HistogramConstructorTest()
        {
            Histogram target = new Histogram();
            Assert.IsNotNull(target.Bins);
            Assert.IsNotNull(target.Values);
            Assert.AreEqual(0, target.Bins.Count);
        }

        [Test]
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

        [Test]
        public void ComputeTest2()
        {
            Histogram target = new Histogram();
            double[] data = new double[] { };

            target.Compute(data);

            Assert.AreEqual(0, target.Bins.Count);
            Assert.AreEqual(0.0, target.Range.Min);
            Assert.AreEqual(0.0, target.Range.Max);
        }



        [Test]
        public void ComputeTest3()
        {
            Histogram target;
            HistogramBin bin;

            double[] values = { 1, 2, 2, 3, 3, 3, 3, 4, 4, 5, 6 };

            target = new Histogram();
            target.Compute(values, 5);

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

        [Test]
        public void ComputeTest4()
        {
            Histogram target;
            HistogramBin bin;

            double[] values = { 1, 2, 2, 3, 3, 3, 3, 4, 4, 5, 6 };

            target = new Histogram();
            target.Compute(values, 5, true);

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

        [Test]
        public void ComputeTest5()
        {
            Histogram target;
            HistogramBin bin;

            double[] values = { 1, 2, 2, 3, 3, 3, 3, 4, 4, 5, 6 };

            target = new Histogram();
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

        [Test]
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
