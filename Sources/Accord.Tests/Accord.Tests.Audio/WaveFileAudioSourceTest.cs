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

namespace Accord.Tests.Audio
{
    using Accord.DirectSound;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Audio;

    [TestClass()]
    public class WaveFileAudioSourceTest
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
        public void WaveFileAudioSourceConstructorTest()
        {
            string fileName = @"..\..\..\Accord.Tests\Accord.Tests.Audio\Resources\Grand Piano - Fazioli - major A middle.wav";

            WaveFileAudioSource target = new WaveFileAudioSource(fileName);

            Signal s = null;

            target.NewFrame += delegate(object sender, NewFrameEventArgs e)
            {
                if (s == null)
                    s = e.Signal;

                Assert.AreEqual(s.SampleRate, 44100);
                Assert.AreEqual(s.Channels, 2);
                Assert.AreEqual(s.Length, 8192);
                Assert.AreEqual(s.Channels * s.Length, s.Samples);
            };


            target.Start();

            target.WaitForStop();

            Assert.AreEqual(180224, target.FramesReceived);
            Assert.AreEqual(705600, target.BytesReceived);

        }

    }
}
