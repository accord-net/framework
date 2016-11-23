using Accord;
using NUnit.Framework;

namespace Accord.Tests
{
    [TestFixture]
    public class ExtensionsTest
    {
        [Test]
        public void CompareTest()
        {
            Assert.IsTrue(10.IsLessThan(11));
            Assert.IsTrue((-1).IsLessThanOrEqual(-1));

            Assert.IsTrue(10.IsGreaterThan(9));
            Assert.IsTrue((-1).IsGreaterThanOrEqual(-2));
        }
    }
}
