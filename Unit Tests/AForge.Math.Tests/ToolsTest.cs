using System;
using AForge;
using AForge.Math;
using MbUnit.Framework;

namespace AForge.Math.Tests
{
    [TestFixture]
    public class ToolsTest
    {
        [Test]
        [Row( 0, false )]
        [Row( 1, true )]
        [Row( 2, true )]
        [Row( 3, false )]
        [Row( 4, true )]
        [Row( 8, true )]
        [Row( 0x80, true )]
        [Row( 0x81, false )]
        [Row( 0x8000, true )]
        [Row( 0x8001, false )]
        [Row( 0x40000000, true )]
        [Row( 0x3FFFFFFF, false )]
        [Row( -1, false )]
        [Row( -8, false )]
        [Row( int.MinValue, false )]
        public void IsPowerOf2Test( int valueToTest, bool expectedResult )
        {
            Assert.AreEqual<bool>( expectedResult, Tools.IsPowerOf2( valueToTest ) );
        }
    }
}
