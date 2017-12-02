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

namespace Accord.Tests.Audio
{
    using NUnit.Framework;
    using Accord.Math;
    using Tools = Accord.Audio.Tools;
    using Accord.Compat;
    using System.Numerics;

    [TestFixture]
    public class ToolsTest
    {

        [Test]
        public void GetPowerCepstrumTest()
        {
            // http://mi.eng.cam.ac.uk/~ajr/SpeechAnalysis/node33.html

            Complex[] source =
            {
                (Complex)(-1.0), (Complex)(+0.5),
                (Complex)(+0.3), (Complex)(-0.4),
                (Complex)(+0.1), (Complex)(-0.9),
                (Complex)(-0.2), (Complex)(-0.5),
            };

            double[] expected =
            {
                0.3319, 0.1285, -0.0218, 0.2303, -0.2639, 0.2303, -0.0218, 0.1285
            };

            double[] actual = Tools.GetPowerCepstrum(source);

            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-4));
        }

        [Test]
        public void InterleaveTest()
        {
            float[,] channels =
            {
                {  0.1f, -5.1f },
                { -0.2f, -6.0f },
                {  2.3f, -3.4f },
                {  0.7f,  5.2f },
                {  0.6f, -1.7f },
            };

            float[] expected =
            {
                0.1f, -5.1f,
               -0.2f, -6.0f,
                2.3f, -3.4f,
                0.7f,  5.2f,
                0.6f, -1.7f,
            };

            float[] actual = Tools.Interleave(channels);

            Assert.IsTrue(actual.IsEqual(expected));
        }
    }
}
