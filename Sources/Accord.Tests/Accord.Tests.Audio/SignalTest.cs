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
    using Accord.Audio;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class SignalTest
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


        private float[,] data = 
        {
            {  0.00f, 0.2f  },
            {  0.32f, 0.1f  },
            {  0.22f, 0.2f  },
            {  0.12f, 0.42f },
            { -0.12f, 0.1f  },
            { -0.22f, 0.2f  },
        };

        [TestMethod()]
        public void GetEnergyTest()
        {
            Signal target = Signal.FromArray(data, 8000);

            double expected = 0.54439;
            double actual = target.GetEnergy();
            Assert.AreEqual(expected, actual, 1e-4);
        }

        [TestMethod()]
        public void SignalConstructorTest()
        {
            Signal target = Signal.FromArray(data, 8000);
            Assert.AreEqual(target.Length, 6);
            Assert.AreEqual(target.Samples, 12);
            Assert.AreEqual(target.Channels, 2);
            Assert.AreEqual(target.SampleRate, 8000);
        }


        [TestMethod()]
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
    }
}
