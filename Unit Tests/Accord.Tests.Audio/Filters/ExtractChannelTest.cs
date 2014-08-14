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
    using Accord.Audio.Filters;
    using Accord.Math;

    [TestClass()]
    public class ExtractChannelTest
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
        public void ApplyTest()
        {
            Signal target = Signal.FromArray(data, 8000);

            for (int c = 0; c < 2; c++)
            {
                ExtractChannel extract = new ExtractChannel(c);

                var result = extract.Apply(target);


                Assert.AreEqual(result.Length, 6);
                Assert.AreEqual(result.Samples, 6);
                Assert.AreEqual(result.Channels, 1);
                Assert.AreEqual(result.SampleRate, 8000);

                float[] actual = result.ToFloat();
                float[] expected = data.GetColumn(c);

                for (int i = 0; i < actual.Length; i++)
                    Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
