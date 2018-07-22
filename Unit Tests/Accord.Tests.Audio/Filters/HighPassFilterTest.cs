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
    public class HighPassFilterTest
    {
        public HighPassFilterTest()
        {
            // Make sure we have loaded Accord.Audio.DirectSound
            Type t = typeof(Accord.DirectSound.AudioCaptureDevice);
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

            // Create a high-pass filter to keep only frequencies above 2000 Hz
            var filter = new HighPassFilter(frequency: 2000, sampleRate: sampleRate);

            // Apply the filter to the signal
            Signal result = filter.Apply(a);

            // Create a spectrogram for the original
            var sourceSpectrum = new Spectrogram(a);

            // Create a spectrogram for the filtered signal:
            var resultSpectrum = new Spectrogram(result);

            // Get the count for a high frequency before and after the high-pass filter:
            double before = sourceSpectrum.GetFrequencyCount(windowIndex: 0, frequency: 100); // 0.0015747246599406217
            double after = resultSpectrum.GetFrequencyCount(windowIndex: 0, frequency: 100);  // 7.7444174980265885E-05
            #endregion

            Assert.AreEqual(0.0015747246599406217, before, 1e-8);
            Assert.AreEqual(7.7444174980265885E-05, after, 1e-8);
        }

    }
}
