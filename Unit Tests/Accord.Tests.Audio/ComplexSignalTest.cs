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
    using System.IO;
    using Accord.Audio;
    using Accord.Audio.Formats;
    using Accord.Audio.Windows;
    using Accord.Math;
    using NUnit.Framework;
    using System;
    using Accord.Compat;
    using System.Numerics;

    [TestFixture]
    public class ComplexSignalTest
    {

        private Complex[,] data =
        {
            { new Complex( 0.42, 0.0), new Complex(0.2, 0.0) },
            { new Complex( 0.32, 0.0), new Complex(0.1, 0.0) },
            { new Complex( 0.22, 0.0), new Complex(0.2, 0.0) },
            { new Complex( 0.12, 0.0), new Complex(0.0, 0.0) },
            { new Complex(-0.12, 0.0), new Complex(0.1, 0.0) },
            { new Complex(-0.22, 0.0), new Complex(0.2, 0.0) },
            { new Complex( 0.00, 0.0), new Complex(0.0, 0.0) },
            { new Complex( 0.00, 0.0), new Complex(0.0, 0.0) },
        };


        [Test]
        public void GetEnergyTest()
        {
            ComplexSignal target = ComplexSignal.FromArray(data, 8000);
            double expected = 0.5444;
            double actual = target.GetEnergy();
            Assert.AreEqual(expected, actual, 1e-4);
        }

        [Test]
        public void ComplexSignalConstructorTest()
        {
            ComplexSignal target = ComplexSignal.FromArray(data, 8000);
            Assert.AreEqual(target.Channels, 2);
            Assert.AreEqual(target.Length, 8);
            Assert.AreEqual(target.Samples, 16);
            Assert.AreEqual(target.SampleRate, 8000);
            Assert.IsNotNull(target.RawData);
        }

        [Test]
        public void ComplexSignalConstructor()
        {
            var sourceStream = SignalTest.GetSignal("a.wav");
            MemoryStream destinationStream = new MemoryStream();

            // Create a decoder for the source stream
            WaveDecoder sourceDecoder = new WaveDecoder(sourceStream);

            // Decode the signal in the source stream
            Signal sourceSignal = sourceDecoder.Decode();

            int length = (int)Math.Pow(2, 12);

            RaisedCosineWindow window = RaisedCosineWindow.Hamming(length);

            Assert.AreEqual(length, window.Length);

            Signal[] windows = sourceSignal.Split(window, 1024);

            Assert.AreEqual(windows.Length, 172);

            foreach (var w in windows)
                Assert.AreEqual(length, w.Length);

            ComplexSignal[] complex = windows.Apply(ComplexSignal.FromSignal);

            for (int i = 0; i < complex.Length - 1; i++)
            {
                ComplexSignal c = complex[i];
                Assert.AreEqual(2, c.Channels);
                Assert.AreEqual(93, c.Duration.TotalMilliseconds);
                Assert.AreEqual(4096, c.Length);
                Assert.AreEqual(SampleFormat.Format128BitComplex, c.SampleFormat);
                Assert.AreEqual(44100, c.SampleRate);
                Assert.AreEqual(ComplexSignalStatus.Normal, c.Status);
            }

            complex.ForwardFourierTransform();

            for (int i = 0; i < complex.Length - 1; i++)
            {
                ComplexSignal c = complex[i];
                Assert.AreEqual(ComplexSignalStatus.FourierTransformed, c.Status);
            }
        }


        [Test]
        public void ComplexRectangularWindowTest()
        {
            int length = 2;
            testWindow(length, RaisedCosineWindow.Rectangular(length));

            testWindow(length, new RectangularWindow(length));
        }

        private void testWindow(int length, IWindow window)
        {
            Complex[,] data = (Complex[,])this.data.Clone();
            ComplexSignal target = ComplexSignal.FromArray(data, 8000);

            Complex[,] samples = target.ToArray();

            Assert.IsTrue(data.IsEqual(samples));

            ComplexSignal[] windows = target.Split(window, 1);

            for (int i = 0; i < windows.Length; i++)
            {
                int min = System.Math.Min(i + length, samples.Length / 2);

                Complex[] segment = windows[i].ToArray().Reshape();
                Complex[] sub = samples.Submatrix(i, min - 1, null).Reshape();

                var expected = new Complex[length * 2];
                for (int j = 0; j < sub.Length; j++)
                    expected[j] = sub[j];

                Assert.IsTrue(segment.IsEqual(expected));
            }
        }
    }
}
