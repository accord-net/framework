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
    public class SelectionFilterTest
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
            DataTable table = new DataTable("myData");
            table.Columns.Add("Double", typeof(double));
            table.Columns.Add("Integer", typeof(int));
            table.Columns.Add("Boolean", typeof(bool));

            table.Rows.Add(4.20, 42, true);
            table.Rows.Add(-3.14, -17, false);
            table.Rows.Add(21.00, 0, false);

            Selection target = new Selection("[Boolean] = false");
            
            DataTable actual = target.Apply(table);

            Assert.AreEqual("Double", actual.Columns[0].ColumnName);
            Assert.AreEqual("Integer", actual.Columns[1].ColumnName);
            Assert.AreEqual("Boolean", actual.Columns[2].ColumnName);

            Assert.AreEqual(2, actual.Rows.Count);
        }
    }
}
