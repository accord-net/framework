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
    using System.Data;
    using Accord.Statistics.Filters;
    using AForge;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass()]
    public class LinearScalingFilterTest
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
        public void ApplyTest()
        {
            DataTable input = new DataTable("Sample data");
            input.Columns.Add("x", typeof(double));
            input.Columns.Add("y", typeof(double));
            input.Rows.Add(0.0, 0);
            input.Rows.Add(0.2, -20);
            input.Rows.Add(0.8, -80);
            input.Rows.Add(1.0, -100);

            DataTable expected = new DataTable("Sample data");
            expected.Columns.Add("x", typeof(double));
            expected.Columns.Add("y", typeof(double));
            expected.Rows.Add(0, 1);
            expected.Rows.Add(20, 0.8);
            expected.Rows.Add(80, 0.2);
            expected.Rows.Add(100, 0.0);


            
            LinearScaling target = new LinearScaling("x","y");
            target.Columns["x"].OutputRange = new DoubleRange(0, 100);
            target.Columns["y"].OutputRange = new DoubleRange(0, 1);


            target.Detect(input);

            DataTable actual = target.Apply(input);

            for (int i = 0; i < actual.Rows.Count; i++)
            {
                double ex = (double)expected.Rows[i][0];
                double ey = (double)expected.Rows[i][1];

                double ax = (double)actual.Rows[i][0];
                double ay = (double)actual.Rows[i][1];

                Assert.AreEqual(ex, ax);
                Assert.AreEqual(ey, ay);
            }
            
        }
    }
}
