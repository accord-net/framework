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

using Accord.Math;
using NUnit.Framework;

namespace Accord.Tests
{
    [TestFixture]
    public class SparseTest
    {
        [Test]
        public void SetTest()
        {
            var s = new Sparse<double>();
            s[0] = 1;
            s[99] = 99;
            s[10] = 42;

            double[] d = s.ToDense();

            for (int i = 0; i < 100; i++)
            {
                if (i == 0)
                {
                    Assert.AreEqual(1, s[i]);
                    Assert.AreEqual(1, d[i]);
                }
                else if (i == 10)
                {
                    Assert.AreEqual(42, s[i]);
                    Assert.AreEqual(42, d[i]);
                }
                else if (i == 99)
                {
                    Assert.AreEqual(99, s[i]);
                    Assert.AreEqual(99, d[i]);
                }
                else
                {
                    Assert.AreEqual(0, s[i]);
                    Assert.AreEqual(0, d[i]);
                }
            }
        }

        [Test]
        public void ToStringTest()
        {
            double[] v;
            string actual;
            Sparse<double> d;

            v = new double[] { 1, 2, 3, 0, 0, 6 };
            d = Sparse.FromDense(v);

            actual = d.ToString();
            Assert.AreEqual("1:1 2:2 3:3 6:6", actual);

            v = new double[] { 0, 0, 2, 3, 0, 0, 6 };
            d = Sparse.FromDense(v);

            actual = d.ToString();
            Assert.AreEqual("3:2 4:3 7:6", actual);
        }

        [Test]
        public void ParseTest()
        {
            double[] v;
            Sparse<double> actual;
            Sparse<double> expected;
            string s;

            v = new double[] { 1, 2, 3, 0, 0, 6 };
            expected = Sparse.FromDense(v);
            s = expected.ToString();
            actual = Sparse.Parse(s);
            Assert.AreEqual(expected, actual);

            v = new double[] { 0, 2, 3, 0, 0, 6 };
            expected = Sparse.FromDense(v);
            s = expected.ToString();
            actual = Sparse.Parse(s);
            Assert.AreEqual(expected, actual);


            v = new double[] { 1, 2, 3, 0, 0, 6 };
            expected = Sparse.FromDense(v);
            s = expected.ToString();
            actual = Sparse.Parse(s, insertValueAtBeginning: 0);
            Assert.AreEqual(expected, actual);

            v = new double[] { 0, 2, 3, 0, 0, 6 };
            expected = Sparse.FromDense(v);
            s = expected.ToString();
            actual = Sparse.Parse(s, insertValueAtBeginning: 0);
            Assert.AreEqual(expected, actual);



            v = new double[] { 1, 2, 3, 0, 0, 6 };
            expected = Sparse.FromDense(v);
            s = expected.ToString();
            actual = Sparse.Parse(s, insertValueAtBeginning: 1);
            expected = Sparse.Parse("1:1 2:1 3:2 4:3 7:6");
            Assert.AreEqual(expected, actual);

            v = new double[] { 0, 2, 3, 0, 0, 6 };
            expected = Sparse.FromDense(v);
            s = expected.ToString();
            actual = Sparse.Parse(s, insertValueAtBeginning: 42);
            expected = Sparse.Parse("1:42 3:2 4:3 7:6");
            Assert.AreEqual(expected, actual);
        }

    }
}
