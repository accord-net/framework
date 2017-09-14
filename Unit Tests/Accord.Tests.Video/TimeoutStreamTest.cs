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

#if !NET35 && !NET40
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
    public class TimeoutStreamTest
    {
        [Test]
        public void CanTimeoutTest()
        {
            MemoryStream baseStream = new MemoryStream();
            TimeoutStream timeoutStream = new TimeoutStream(baseStream);

            Assert.IsFalse(baseStream.CanTimeout);
            Assert.IsTrue(timeoutStream.CanTimeout);
        }

        [Test]
        public void CanReadTest()
        {
            MemoryStream baseStream = new MemoryStream();
            TimeoutStream timeoutStream = new TimeoutStream(baseStream);

            Assert.IsTrue(baseStream.CanRead);
            Assert.IsTrue(timeoutStream.CanRead);
        }

        [Test]
        public void SetReadTimeoutBaseStream()
        {
            MemoryStream baseStream = new MemoryStream();
            TimeoutStream timeoutStream = new TimeoutStream(baseStream);

            Assert.Throws<InvalidOperationException>(() => baseStream.ReadTimeout = 10000);
        }

        [Test]
        public void SetReadTimeoutTimeoutStream()
        {
            int expected = 10000;

            MemoryStream baseStream = new MemoryStream();
            TimeoutStream timeoutStream = new TimeoutStream(baseStream);

            timeoutStream.ReadTimeout = expected;

            Assert.AreEqual(expected, timeoutStream.ReadTimeout);
        }

        [Test]
        public void SetWriteTimeoutBaseStream()
        {
            MemoryStream baseStream = new MemoryStream();
            TimeoutStream timeoutStream = new TimeoutStream(baseStream);

            Assert.Throws<InvalidOperationException>(() => baseStream.WriteTimeout = 10000);
        }

        [Test]
        public void SetWriteTimeoutTimeoutStream()
        {
            int expected = 10000;

            MemoryStream baseStream = new MemoryStream();
            TimeoutStream timeoutStream = new TimeoutStream(baseStream);

            timeoutStream.WriteTimeout = expected;

            Assert.AreEqual(expected, timeoutStream.WriteTimeout);
        }
    }
}
#endif