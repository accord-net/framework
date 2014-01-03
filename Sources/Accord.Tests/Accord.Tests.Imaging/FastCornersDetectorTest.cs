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

namespace Accord.Tests.Imaging
{
    using Accord.Imaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AForge.Imaging;
    using AForge;
    using System.Collections.Generic;
    using System.Drawing;
    using Accord.Controls;
    using System.Windows.Forms;
    using Accord.Imaging.Filters;

    [TestClass()]
    public class FastCornersDetectorTest
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
        public void ProcessImageTest()
        {
            UnmanagedImage image = UnmanagedImage.FromManagedImage(Properties.Resources.sample_black);

            FastCornersDetector target = new FastCornersDetector();
            target.Suppress = false;
            target.Threshold = 20;

            List<IntPoint> actual = target.ProcessImage(image);

            Assert.AreEqual(237, actual.Count);
            Assert.AreEqual(404, actual[0].X);
            Assert.AreEqual(35, actual[0].Y);
            Assert.AreEqual(407, actual[6].X);
            Assert.AreEqual(36, actual[6].Y);
            Assert.AreEqual(407, actual[11].X);
            Assert.AreEqual(38, actual[11].Y);
            Assert.AreEqual(55, actual[65].X);
            Assert.AreEqual(135, actual[65].Y);
            Assert.AreEqual(103, actual[73].X);
            Assert.AreEqual(137, actual[73].Y);
        }

        [TestMethod()]
        public void ProcessImageTest2()
        {
            UnmanagedImage image = UnmanagedImage.FromManagedImage(Properties.Resources.lena512);

            FastCornersDetector target = new FastCornersDetector();
            target.Suppress = true;
            target.Threshold = 40;

            List<IntPoint> actual = target.ProcessImage(image);

            Assert.AreEqual(324, actual.Count);
            Assert.AreEqual(506, actual[0].X);
            Assert.AreEqual(4, actual[0].Y);
            Assert.AreEqual(152, actual[6].X);
            Assert.AreEqual(75, actual[6].Y);
            Assert.AreEqual(416, actual[11].X);
            Assert.AreEqual(115, actual[11].Y);
            Assert.AreEqual(140, actual[65].X);
            Assert.AreEqual(246, actual[65].Y);
            Assert.AreEqual(133, actual[73].X);
            Assert.AreEqual(253, actual[73].Y);
        }

    }
}
