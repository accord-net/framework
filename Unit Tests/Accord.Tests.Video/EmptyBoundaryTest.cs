// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Copyright © Stig Voss, 2017
// http://stigvoss.dk
// stig.voss at gmail.com
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

namespace Accord.Tests.Video
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Accord.Video;

    [TestFixture]
    public class EmptyBoundaryTest
    {
        private Boundary _boundary;

        [SetUp]
        public void Initialization()
        {
            _boundary = new Boundary();
        }

        [Test]
        public void HasValueTest()
        {
            Assert.IsFalse(_boundary.HasValue);
        }

        [Test]
        public void IsCheckedTest()
        {
            Assert.IsFalse(_boundary.IsChecked);
        }

        [Test]
        public void IsValidTest()
        {
            Assert.IsTrue(_boundary.IsValid);
        }

        [Test]
        public void LengthTest()
        {
            int expected = 0;
            Assert.AreEqual(expected, _boundary.Length);
        }

        [Test]
        public void ContentTest()
        {
            string expected = string.Empty;
            Assert.AreEqual(expected, _boundary.Content);
        }
    }
}
