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

namespace Accord.Tests.Imaging
{
    using Accord.DataSets;
    using Accord.Imaging.Converters;
    using Accord.Imaging.Filters;
    using Accord.Math;
    using NUnit.Framework;
    using System.Drawing;
    using System.Drawing.Imaging;

    [TestFixture]
    public class DistanceTransformTest
    {

        [Test]
        public void ApplyTest1()
        {
            // Bitmap cards = new TestImages().GetImage("cards.jpg");
            Bitmap cards = Properties.Resources.cards;
            cards.Save(@"c:\Temp\input.jpg");

            Bitmap grayCards = Grayscale.CommonAlgorithms.BT709.Apply(cards);
            grayCards.Save(@"c:\Temp\grayscale.jpg");

            Bitmap binaryCards = new Threshold(200).Apply(grayCards);
            binaryCards.Save(@"c:\Temp\binary.jpg");

            var dt = new DistanceTransform();
            Bitmap result = dt.Apply(binaryCards);
            Assert.AreEqual(35.805027, dt.MaximumDistance, 1e-5);
            Assert.AreEqual(new IntPoint(254, 129), dt.UltimateErodedPoint);
            result.Save(@"c:\Temp\result.jpg");

            Assert.IsNotNull(result);
        }

    }
}
