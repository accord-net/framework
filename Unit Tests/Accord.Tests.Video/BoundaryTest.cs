// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmC:\Projects\Accord.NET\framework\Unit Tests\Accord.Tests.Video\BoundaryTest.csail.com
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
    using System.IO;

    [TestFixture]
    public class BoundaryTest
    {
        private string _value = "boundary";
        private Boundary _boundary;

        [SetUp]
        public void Initialization()
        {
            _boundary = new Boundary(_value);
        }

        [Test]
        public void HasValueTest()
        {
            Assert.IsTrue(_boundary.HasValue);
        }

        [Test]
        public void IsCheckedTest()
        {
            Assert.IsFalse(_boundary.IsChecked);
        }

        [Test]
        public void IsValidTest()
        {
            Assert.IsFalse(_boundary.IsValid);
        }

        [Test]
        public void LengthTest()
        {
            int expected = _value.Length;
            Assert.AreEqual(expected, _boundary.Length);
        }

        [Test]
        public void ContentTest()
        {
            string expected = _value;
            Assert.AreEqual(expected, _boundary.Content);
        }

        [Test]
        public void FixMalformedBoundaryTest()
        {
            string expected = "--" + _value;
            byte[] buffer = Encoding.ASCII.GetBytes(expected);

            MJPEGStreamParser parser = new MJPEGStreamParser(_boundary, new byte[0]);
            MemoryStream stream = new MemoryStream(buffer);

            parser.Read(stream);

            _boundary.FixMalformedBoundary(parser);

            Assert.AreEqual(expected, _boundary.Content);
        }

        [Test]
        public void FixMalformedAndCheckedBoundaryTest()
        {
            string expected = "--" + _value;
            byte[] buffer = Encoding.ASCII.GetBytes(expected);

            MJPEGStreamParser parser = new MJPEGStreamParser(_boundary, new byte[0]);
            MemoryStream stream = new MemoryStream(buffer);

            parser.Read(stream);

            _boundary.FixMalformedBoundary(parser);

            Assert.IsTrue(_boundary.IsChecked);
        }

        [Test]
        public void FixMalformedAndUnchangedBoundaryTest()
        {
            string expected = _value;
            byte[] buffer = Encoding.ASCII.GetBytes(expected);
            MemoryStream stream = new MemoryStream(buffer);

            MJPEGStreamParser parser = new MJPEGStreamParser(_boundary, new byte[0]);

            parser.Read(stream);

            _boundary.FixMalformedBoundary(parser);

            Assert.AreEqual(expected, _boundary.Content);
            Assert.IsTrue(_boundary.IsChecked);
        }

        [Test]
        public void FixMalformedAndValidBoundaryTest()
        {
            string expected = "--" + _value;
            byte[] buffer = Encoding.ASCII.GetBytes(expected);
            MemoryStream stream = new MemoryStream(buffer);

            MJPEGStreamParser parser = new MJPEGStreamParser(_boundary, new byte[0]);

            parser.Read(stream);

            _boundary.FixMalformedBoundary(parser);

            Assert.IsTrue(_boundary.IsValid);
        }

        [Test]
        public void FixMalformedBoundaryAndLengthTest()
        {
            string expected = "--" + _value;
            byte[] buffer = Encoding.ASCII.GetBytes(expected);
            MemoryStream stream = new MemoryStream(buffer);

            MJPEGStreamParser parser = new MJPEGStreamParser(_boundary, new byte[0]);

            parser.Read(stream);

            _boundary.FixMalformedBoundary(parser);

            Assert.AreEqual(_boundary.Length, expected.Length);
        }

        [Test]
        public void PrependTest()
        {
            string expected = "--" + _value;

            _boundary.Prepend('-');
            _boundary.Prepend('-');

            Assert.AreEqual(expected, _boundary.Content);
        }
    }
}
