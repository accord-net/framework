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
    public class GroupingFilterTest
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

            input.Columns.Add("Age", typeof(int));
            input.Columns.Add("Classification", typeof(string));

            input.Rows.Add(10, 0);
            input.Rows.Add(7, 0);
            input.Rows.Add(4, 0);
            input.Rows.Add(21, 1);
            input.Rows.Add(27, 1);
            input.Rows.Add(12, 0);
            input.Rows.Add(79, 0);
            input.Rows.Add(40, 0);
            input.Rows.Add(30, 0);
            input.Rows.Add(32, 0);


            {
                Grouping target = new Grouping();

                target.Proportion = 0.2;
                DataTable actual = target.Apply(input);

                DataRow[] a = actual.Select("Group = 0");
                DataRow[] b = actual.Select("Group = 1");

                Assert.AreEqual(2, a.Length);
                Assert.AreEqual(8, b.Length);
            }


            {
                Grouping target = new Grouping();

                target.Proportion = 0.5;
                DataTable actual = target.Apply(input);

                DataRow[] a = actual.Select("Group = 0");
                DataRow[] b = actual.Select("Group = 1");

                Assert.AreEqual(5, a.Length);
                Assert.AreEqual(5, b.Length);
            }

            {
                Grouping target = new Grouping();

                target.Columns.Add(new Grouping.Options("Classification"));

                target.Proportion = 0.5;
                DataTable actual = target.Apply(input);

                DataRow[] a = actual.Select("Group = 0");
                DataRow[] b = actual.Select("Group = 1");

                Assert.AreEqual(5, a.Length);
                Assert.AreEqual(5, b.Length);

                DataRow[] a0 = actual.Select("Group = 0 AND Classification = 0");
                DataRow[] a1 = actual.Select("Group = 0 AND Classification = 1");
                DataRow[] b0 = actual.Select("Group = 1 AND Classification = 0");
                DataRow[] b1 = actual.Select("Group = 1 AND Classification = 1");

                Assert.AreEqual(4, a0.Length);
                Assert.AreEqual(1, a1.Length);
                Assert.AreEqual(4, b0.Length);
                Assert.AreEqual(1, b1.Length);
            }

        }

        [TestMethod()]
        public void ApplyTest2()
        {

            DataTable input = new DataTable("Sample data");

            input.Columns.Add("Age", typeof(int));
            input.Columns.Add("Classification", typeof(string));

            for (int i = 0; i < 80; i++)
                input.Rows.Add(i, 0);

            for (int i = 0; i < 20; i++)
                input.Rows.Add(i, 1);

            {
                Grouping target = new Grouping();

                target.Proportion = 0.2;
                DataTable actual = target.Apply(input);

                DataRow[] a = actual.Select("Group = 0");
                DataRow[] b = actual.Select("Group = 1");

                Assert.AreEqual(20, a.Length);
                Assert.AreEqual(80, b.Length);
            }


            {
                Grouping target = new Grouping();

                target.Proportion = 0.5;
                DataTable actual = target.Apply(input);

                DataRow[] a = actual.Select("Group = 0");
                DataRow[] b = actual.Select("Group = 1");

                Assert.AreEqual(50, a.Length);
                Assert.AreEqual(50, b.Length);
            }

            {
                Grouping target = new Grouping();

                target.Columns.Add(new Grouping.Options("Classification"));

                target.Proportion = 0.5;
                DataTable actual = target.Apply(input);

                DataRow[] a = actual.Select("Group = 0");
                DataRow[] b = actual.Select("Group = 1");

                Assert.AreEqual(50, a.Length);
                Assert.AreEqual(50, b.Length);

                DataRow[] a0 = actual.Select("Group = 0 AND Classification = 0");
                DataRow[] a1 = actual.Select("Group = 0 AND Classification = 1");
                DataRow[] b0 = actual.Select("Group = 1 AND Classification = 0");
                DataRow[] b1 = actual.Select("Group = 1 AND Classification = 1");

                Assert.AreEqual(40, a0.Length);
                Assert.AreEqual(10, a1.Length);
                Assert.AreEqual(40, b0.Length);
                Assert.AreEqual(10, b1.Length);
            }

            {
                Grouping target = new Grouping();

                target.Columns.Add(new Grouping.Options("Classification"));

                target.Proportion = 0.6;
                DataTable actual = target.Apply(input);

                DataRow[] a = actual.Select("Group = 0");
                DataRow[] b = actual.Select("Group = 1");

                Assert.AreEqual(60, a.Length);
                Assert.AreEqual(40, b.Length);

                DataRow[] a0 = actual.Select("Group = 0 AND Classification = 0");
                DataRow[] a1 = actual.Select("Group = 0 AND Classification = 1");
                DataRow[] b0 = actual.Select("Group = 1 AND Classification = 0");
                DataRow[] b1 = actual.Select("Group = 1 AND Classification = 1");

                Assert.AreEqual(48, a0.Length);
                Assert.AreEqual(12, a1.Length);
                Assert.AreEqual(32, b0.Length);
                Assert.AreEqual(8, b1.Length);
            }


        }

    }
}
