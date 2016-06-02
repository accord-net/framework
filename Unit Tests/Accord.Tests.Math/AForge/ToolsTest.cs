using System;
using AForge;
using AForge.Math;
using NUnit.Framework;
using Accord.Math;

namespace Accord.Tests.Math
{
    [TestFixture]
    public class ToolsTest
    {

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, false)]
        [TestCase(4, true)]
        [TestCase(8, true)]
        [TestCase(0x80, true)]
        [TestCase(0x81, false)]
        [TestCase(0x8000, true)]
        [TestCase(0x8001, false)]
        [TestCase(0x40000000, true)]
        [TestCase(0x3FFFFFFF, false)]
        [TestCase(-1, false)]
        [TestCase(-8, false)]
        [TestCase(int.MinValue, false)]
        public void IsPowerOf2Test(int valueToTest, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, Tools.IsPowerOf2(valueToTest));
        }
    }
}
