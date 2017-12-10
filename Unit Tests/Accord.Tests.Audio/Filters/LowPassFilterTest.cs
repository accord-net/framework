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
    using System.IO;
    using Accord.DataSets;
    using Accord.Audio.Visualizations;

    [TestFixture]
    public class LowPassFilterTest
    {
        public LowPassFilterTest()
        {
            // Make sure we have loaded Accord.Audio.DirectSound
            Type t = typeof(Accord.DirectSound.AudioCaptureDevice);
        }

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
            Assert.AreEqual(0.359128660199268, of1, 1e-8);
            Assert.AreEqual(0.47955332752802149, of2, 1e-8);

            Signal lowFiltered1 = new LowPassFilter(f1, sampleRate).Apply(original);
            Signal lowFiltered2 = new LowPassFilter(f2, sampleRate).Apply(original);

            Signal highFiltered1 = new HighPassFilter(f1, sampleRate).Apply(original);
            Signal highFiltered2 = new HighPassFilter(f2, sampleRate).Apply(original);

            var lf11 = FindFrequencyCount(sampleRate, lowFiltered1, f1);
            var lf12 = FindFrequencyCount(sampleRate, lowFiltered1, f2);
            Assert.AreEqual(0.24589601823749971, lf11, 1e-8); // should be higher
            Assert.AreEqual(0.038266797164259778, lf12, 1e-8);
            Assert.IsTrue(lf11 > lf12);

            var lf21 = FindFrequencyCount(sampleRate, lowFiltered2, f1);
            var lf22 = FindFrequencyCount(sampleRate, lowFiltered2, f2);
            Assert.AreEqual(0.35642263929018364, lf21, 1e-8); // should not have much difference
            Assert.AreEqual(0.271181864130875, lf22, 1e-8);

            var hf11 = FindFrequencyCount(sampleRate, highFiltered1, f1);
            var hf12 = FindFrequencyCount(sampleRate, highFiltered1, f2);
            Assert.AreEqual(0.24542517074628975, hf11, 1e-8);  // should not have much difference
            Assert.AreEqual(0.44797847700473359, hf12, 1e-8);

            var hf21 = FindFrequencyCount(sampleRate, highFiltered2, f1);
            var hf22 = FindFrequencyCount(sampleRate, highFiltered2, f2);
            Assert.AreEqual(0.026113299330488803, hf21, 1e-8);
            Assert.AreEqual(0.23279968506488344, hf22, 1e-8); // should be higher
            Assert.IsTrue(hf22 > hf21);

            Assert.AreEqual(16384, cosine.Duration.TotalMilliseconds);
            Assert.AreEqual(16384, sine.Duration.TotalMilliseconds);
            Assert.AreEqual(16384, original.Duration.TotalMilliseconds);
        }

        [Test]
        public void sample_test()
        {
            string basePath = NUnit.Framework.TestContext.CurrentContext.TestDirectory;
            string pathWhereTheDatasetShouldBeStored = Path.Combine(basePath, "mfcc");

            #region doc_example1
            // Let's say we would like to analyse an audio sample. To give an example that
            // could be reproduced by anyone without having to give a specific sound file
            // that would need to have been downloaded by every user trying to run this example,
            // we will use obtain an example from the Free Spoken Digits Dataset instead:
            var fsdd = new FreeSpokenDigitsDataset(path: pathWhereTheDatasetShouldBeStored);

            // Let's obtain one of the audio signals:
            Signal a = fsdd.GetSignal(0, "jackson", 10);
            int sampleRate = a.SampleRate; // 8000

            // Note: if you would like to load a signal from the 
            // disk, you could use the following method directly:
            // Signal a = Signal.FromFile(fileName);

            // Create a low-pass filter to keep only frequencies below 100 Hz
            var filter = new LowPassFilter(frequency: 100, sampleRate: sampleRate);

            // Apply the filter to the signal
            Signal result = filter.Apply(a);

            // Create a spectrogram for the original
            var sourceSpectrum = new Spectrogram(a);

            // Create a spectrogram for the filtered signal:
            var resultSpectrum = new Spectrogram(result);

            // Get the count for a high frequency before and after the low-pass filter:
            double before = sourceSpectrum.GetFrequencyCount(windowIndex: 0, frequency: 1000); // 0.00028203820434203334
            double after = resultSpectrum.GetFrequencyCount(windowIndex: 0, frequency: 1000);  // 2.9116651158267508E-05
            #endregion

            Assert.AreEqual(0.00028203820434203334, before, 1e-8);
            Assert.AreEqual(2.9116651158267508E-05, after, 1e-8);
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
