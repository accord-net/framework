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
    using Accord.Controls;
    using Accord.Statistics.Visualizations;
    using NUnit.Framework;

    [TestFixture]
    public class ScatterplotViewTest
    {


        [Test]
        [Category("WinForms")]
        public void ComputeTest1()
        {
            ScatterplotView target = new ScatterplotView();

            double[] data = new double[] { 200.0, 200.0, 200.0 };

            target.DataSource = data;

            target.DataSource = null;

            target.DataSource = data;

            Assert.IsNotNull(target.DataSource);
            // ScatterplotBox.Show(data);
        }

        [Test]
        [Category("WinForms")]
        public void ComputeTest2()
        {
            ScatterplotView target = new ScatterplotView();

            Scatterplot histogram = new Scatterplot();
            histogram.Compute(new double[] { 200.0, 200.0, 200.0 });

            target.DataSource = null;

            target.DataSource = histogram;

            target.DataSource = null;

            Assert.IsNull(target.DataSource);

            // ScatterplotBox.Show(histogram);
        }

    }
}
