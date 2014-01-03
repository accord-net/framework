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
    using Accord.Statistics.Filters;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Data;
    
    [TestClass()]
    public class StratificationFilterTest
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
            DataTable data = new DataTable("Sample data");
            data.Columns.Add("x", typeof(double));
            data.Columns.Add("Class", typeof(int));
            data.Rows.Add(0.21, 0);
            data.Rows.Add(0.25, 0);
            data.Rows.Add(0.54, 0);
            data.Rows.Add(0.19, 1);

            DataTable expected = new DataTable("Sample data");
            expected.Columns.Add("x", typeof(double));
            expected.Columns.Add("Class", typeof(int));
            expected.Rows.Add(0.21, 0);
            expected.Rows.Add(0.25, 0);
            expected.Rows.Add(0.54, 0);
            expected.Rows.Add(0.19, 1);
            expected.Rows.Add(0.19, 1);
            expected.Rows.Add(0.19, 1);


            DataTable actual;

            Stratification target = new Stratification("Class");
            target.Columns["Class"].Classes = new int[] { 0, 1 };
            
            actual = target.Apply(data);

            for (int i = 0; i < actual.Rows.Count; i++)
            {
                double ex = (double)expected.Rows[i][0];
                int ec = (int)expected.Rows[i][1];

                double ax = (double)actual.Rows[i][0];
                int ac = (int)actual.Rows[i][1];

                Assert.AreEqual(ex, ax);
                Assert.AreEqual(ec, ac);                    
                
            }
            
        }
    }
}
