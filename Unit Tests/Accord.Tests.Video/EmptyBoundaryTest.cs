using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Accord.Video;

namespace Accord.Tests.Video
{
    [TestFixture]
    public class EmptyBoundaryTest
    {
        private Boundary _boundary;

        [SetUp]
        public void Initialization()
        {
            _boundary = new Boundary();
        }

        [Test]
        public void HasValueTest()
        {
            Assert.IsFalse(_boundary.HasValue);
        }

        [Test]
        public void IsCheckedTest()
        {
            Assert.IsFalse(_boundary.IsChecked);
        }

        [Test]
        public void IsValidTest()
        {
            Assert.IsTrue(_boundary.IsValid);
        }

        [Test]
        public void LengthTest()
        {
            int expected = 0;
            Assert.AreEqual(expected, _boundary.Length);
        }

        [Test]
        public void ContentTest()
        {
            string expected = string.Empty;
            Assert.AreEqual(expected, _boundary.Content);
        }
    }
}
