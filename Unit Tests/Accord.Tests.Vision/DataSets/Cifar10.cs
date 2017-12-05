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

namespace Accord.Tests.Vision
{
    using Accord.Vision.Tracking;
    using NUnit.Framework;
    using System.Drawing;
    using Accord.Imaging;
    using Accord.Tests.Vision.Properties;
    using Accord.DataSets;
    using System.IO;
    using Accord.Math;

    [TestFixture]
    public class Cifar10Test
    {

        [Test]
        public void download_test()
        {
            string basePath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.WorkDirectory, "cifar10");

            var c = new Cifar10(basePath);

            Assert.AreEqual(50000, c.Training.Item1.Length);
            Assert.AreEqual(50000, c.Training.Item2.Length);

            Assert.AreEqual(10000, c.Testing.Item1.Length);
            Assert.AreEqual(10000, c.Testing.Item2.Length);

            int[] hist;
            hist = Vector.Histogram(c.Training.Item2);
            Assert.AreEqual(Vector.Create(size: 10, value: 5000), hist);
            hist = Vector.Histogram(c.Testing.Item2);
            Assert.AreEqual(Vector.Create(size: 10, value: 1000), hist);
        }


    }
}
