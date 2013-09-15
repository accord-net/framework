// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2013
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

using Accord.Audio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AForge.Math;

namespace Accord.Tests.Audio
{
    
    
    /// <summary>
    ///This is a test class for ComplexSignalTest and is intended
    ///to contain all ComplexSignalTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ComplexSignalTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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

        /// <summary>
        ///A test for GetEnergy
        ///</summary>
        [TestMethod()]
        public void GetEnergyTest()
        {
            ComplexSignal target = ComplexSignal.FromArray(data, 8000);
            double expected = 0.5444;
            double actual = target.GetEnergy();
            Assert.AreEqual(expected, actual, 1e-4);
        }

        /// <summary>
        ///A test for ComplexSignal Constructor
        ///</summary>
        [TestMethod()]
        public void ComplexSignalConstructorTest()
        {
            ComplexSignal target = ComplexSignal.FromArray(data, 8000);
            Assert.AreEqual(target.Channels, 2);
            Assert.AreEqual(target.Length, 8);
            Assert.AreEqual(target.Samples, 16);
            Assert.AreEqual(target.SampleRate, 8000);
            //Assert.IsNotNull(target.RawData);
        }
    }
}
