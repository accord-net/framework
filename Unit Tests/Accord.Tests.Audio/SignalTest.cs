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
    using Accord.Audio.Windows;
    using Accord.Math;
    using System.IO;
    using Accord.DataSets;

    [TestFixture]
    public class SignalTest
    {
        public static FileStream GetSignal(string resourceName)
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", resourceName);
            return new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }

        private float[,] data = 
        {
            {  0.00f, 0.2f  },
            {  0.32f, 0.1f  },
            {  0.22f, 0.2f  },
            {  0.12f, 0.42f },
            { -0.12f, 0.1f  },
            { -0.22f, 0.2f  },
        };

        [Test]
        public void GetEnergyTest()
        {
            Signal target = Signal.FromArray(data, 8000);

            double expected = 0.54439;
            double actual = target.GetEnergy();
            Assert.AreEqual(expected, actual, 1e-4);
        }

        [Test]
        public void GetEnergyTest_doc()
        {
            string basePath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "energy");

            #region doc_energy
            // Let's say we would like to compute the energy of an audio signal. For this,
            // we will take an example signal from the Free Spoken Digits Dataset (FSDD):
            FreeSpokenDigitsDataset fsdd = new FreeSpokenDigitsDataset(basePath);
            Signal signal = fsdd.GetSignal(digit: 3, speaker: "jackson", index: 0);

            // The energy is defined as the sum of squared values in all 
            // channels of the audio signal. In this case, it should be:
            double energy = signal.GetEnergy(); // 19.448728048242629
            #endregion

            Assert.AreEqual(19.448728048242629, energy, 1e-10);
        }

        [Test]
        public void SignalConstructorTest()
        {
            Signal target = Signal.FromArray(data, 8000);
            Assert.AreEqual(target.Length, 6);
            Assert.AreEqual(target.Samples, 12);
            Assert.AreEqual(target.Channels, 2);
            Assert.AreEqual(target.SampleRate, 8000);
        }


        [Test]
        public void GetSampleTest()
        {
            float[,] data = (float[,])this.data.Clone();
            Signal target = Signal.FromArray(data, 8000);

            int channel = 1;
            int position = 3;

            float expected, actual;

            expected = 0.42f;
            actual = target.GetSample(channel, position);
            Assert.AreEqual(expected, actual);

            target.SetSample(channel, position, -1);

            expected = -1;
            actual = target.GetSample(channel, position);
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void RectangularWindowTest()
        {
            int length = 3;
            testWindow(length, RaisedCosineWindow.Rectangular(length));

            testWindow(length, new RectangularWindow(length));
        }

        private void testWindow(int length, IWindow window)
        {
            float[,] data = (float[,])this.data.Clone();
            Signal target = Signal.FromArray(data, 8000);

            float[] samples = target.ToFloat();

            Signal[] windows = target.Split(window, 1);

            for (int i = 0; i < windows.Length; i++)
            {
                int min = System.Math.Min(i * 2 + length * 2, samples.Length);

                float[] segment = windows[i].ToFloat();
                float[] sub = samples.Submatrix(i * 2, min - 1);

                float[] expected = new float[length * 2];
                for (int j = 0; j < sub.Length; j++)
                    expected[j] = sub[j];

                Assert.IsTrue(segment.IsEqual(expected));
            }
        }
    }
}
