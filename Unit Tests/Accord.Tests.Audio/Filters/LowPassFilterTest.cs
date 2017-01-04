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
    using Accord.Audio;
    using NUnit.Framework;
    using Accord.Audio.Filters;
    using Accord.Math;
    using Accord.Audio.Generators;
    using System;
    using Accord.Audio.Windows;

    [TestFixture]
    public class LowPassFilterTest
    {

        [Test]
        public void ApplyTest()
        {
            int n = 16384;
            int sampleRate = 1000;

            double f1 = 22;
            double f2 = 300;

            Signal cosine = new CosineGenerator(f1, 1, sampleRate).Generate(n);
            Signal sine = new SineGenerator(f2, 1, sampleRate).Generate(n);

            var merge = new AddFilter(cosine);
            merge.Normalize = true;
            Signal original = merge.Apply(sine);

            var of1 = FindFrequencyCount(sampleRate, original, f1);
            var of2 = FindFrequencyCount(sampleRate, original, f2);
            Assert.AreEqual(2.1919473889115457E-05, of1, 1e-8);
            Assert.AreEqual(2.9269612275882952E-05, of2, 1e-8);

            Signal lowFiltered1 = new LowPassFilter(f1, sampleRate).Apply(original);
            Signal lowFiltered2 = new LowPassFilter(f2, sampleRate).Apply(original);

            Signal highFiltered1 = new HighPassFilter(f1, sampleRate).Apply(original);
            Signal highFiltered2 = new HighPassFilter(f2, sampleRate).Apply(original);

            var lf11 = FindFrequencyCount(sampleRate, lowFiltered1, f1);
            var lf12 = FindFrequencyCount(sampleRate, lowFiltered1, f2);
            Assert.AreEqual(1.5008301894378632E-05, lf11, 1e-8); // should be higher
            Assert.AreEqual(2.33561994410787E-06, lf12, 1e-8);
            Assert.IsTrue(lf11 > lf12);

            var lf21 = FindFrequencyCount(sampleRate, lowFiltered2, f1);
            var lf22 = FindFrequencyCount(sampleRate, lowFiltered2, f2);
            Assert.AreEqual(2.1754311480113727E-05, lf21, 1e-8); // should not have much difference
            Assert.AreEqual(1.6551627449395776E-05, lf22, 1e-8);

            var hf11 = FindFrequencyCount(sampleRate, highFiltered1, f1);
            var hf12 = FindFrequencyCount(sampleRate, highFiltered1, f2);
            Assert.AreEqual(1.4979563644182712E-05, hf11, 1e-8);  // should not have much difference
            Assert.AreEqual(2.7342436340623498E-05, hf12, 1e-8);

            var hf21 = FindFrequencyCount(sampleRate, highFiltered2, f1);
            var hf22 = FindFrequencyCount(sampleRate, highFiltered2, f2);
            Assert.AreEqual(1.5938293048394034E-06, hf21, 1e-8); 
            Assert.AreEqual(1.420896515288728E-05, hf22, 1e-8); // should be higher
            Assert.IsTrue(hf22 > hf21);

            Assert.AreEqual(16384, cosine.Duration.TotalMilliseconds);
            Assert.AreEqual(16384, sine.Duration.TotalMilliseconds);
            Assert.AreEqual(16384, original.Duration.TotalMilliseconds);
        }

        private static double FindFrequencyCount(int sampleRate, Signal signal, double frequency)
        {
            ComplexSignal c = signal.ToComplex();
            c.ForwardFourierTransform();

            double[] freq = Accord.Audio.Tools.GetFrequencyVector(c.Length, sampleRate);
            double[] ms = Accord.Audio.Tools.GetMagnitudeSpectrum(c.GetChannel(0));

            int idx = freq.Subtract(frequency).Abs().ArgMin();

            return ms[idx];
        }
    }
}
