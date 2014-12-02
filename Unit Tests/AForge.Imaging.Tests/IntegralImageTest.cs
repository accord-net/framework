using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using AForge;
using AForge.Imaging;
using NUnit.Framework;

namespace AForge.Imaging.Tests
{
    [TestFixture]
    public class IntegralImageTest
    {
        private IntegralImage integralImage = null;

        public IntegralImageTest( )
        {
            UnmanagedImage uImage = UnmanagedImage.Create( 10, 10, PixelFormat.Format8bppIndexed );

            for ( int y = 0; y < 10; y++ )
            {
                for ( int x = 0; x < 10; x++ )
                {
                    uImage.SetPixel( x, y, ( ( x + y ) % 2 == 0 ) ? Color.FromArgb( 0, 0, 0 ) : Color.FromArgb( 1, 1, 1 ) );
                }
            }

            integralImage = IntegralImage.FromBitmap( uImage );
        }

        [Test]
        [TestCase( 0, 0, 0, 0, 0u )]
        [TestCase( 0, 0, 1, 0, 1u )]
        [TestCase( 0, 0, 0, 1, 1u )]
        [TestCase( 0, 0, 1, 1, 2u )]
        [TestCase( -1, -1, 1, 1, 2u )]
        [TestCase( 0, 0, 9, 9, 50u )]
        [TestCase( 9, 9, 9, 9, 0u )]
        [TestCase( 9, 9, 10, 10, 0u )]
        public void GetRectangleSumTest( int x1, int y1, int x2, int y2, uint expectedSum )
        {
            uint sum = integralImage.GetRectangleSum( x1, y1, x2, y2 );
            Assert.AreEqual( sum, expectedSum );
        }

        [Test]
        [TestCase( 0, 0, 0, 0, 0u )]
        [TestCase( 0, 0, 1, 0, 1u )]
        [TestCase( 0, 0, 0, 1, 1u )]
        [TestCase( 0, 0, 1, 1, 2u )]
        [TestCase( 0, 0, 9, 9, 50u )]
        [TestCase( 9, 9, 9, 9, 0u )]
        public void GetRectangleSumUnsafeTest( int x1, int y1, int x2, int y2, uint expectedSum )
        {
            uint sum = integralImage.GetRectangleSum( x1, y1, x2, y2 );
            Assert.AreEqual( sum, expectedSum );
        }

        [Test]
        [TestCase( 0, 0, 1, 2u )]
        [TestCase( 1, 1, 1, 4u )]
        [TestCase( 9, 9, 1, 2u )]
        [TestCase( 8, 8, 1, 4u )]
        [TestCase( 2, 1, 1, 5u )]
        public void GetRectangleSumTest( int x, int y, int radius, uint expectedSum )
        {
            uint sum = integralImage.GetRectangleSum( x, y, radius );
            Assert.AreEqual( sum, expectedSum );
        }

        [Test]
        [TestCase( 1, 1, 1, 4u )]
        [TestCase( 8, 8, 1, 4u )]
        [TestCase( 2, 1, 1, 5u )]
        public void GetRectangleSumUnsafeTest( int x, int y, int radius, uint expectedSum )
        {
            uint sum = integralImage.GetRectangleSum( x, y, radius );
            Assert.AreEqual( sum, expectedSum );
        }

        [Test]
        [TestCase( 0, 0, 0, 0, 0 )]
        [TestCase( 0, 0, 1, 0, 0.5f )]
        [TestCase( 0, 0, 0, 1, 0.5f )]
        [TestCase( 0, 0, 1, 1, 0.5f )]
        [TestCase( -1, -1, 1, 1, 0.5f )]
        [TestCase( 0, 0, 9, 9, 0.5f )]
        [TestCase( 9, 9, 9, 9, 0 )]
        [TestCase( 9, 9, 10, 10, 0 )]
        [TestCase( 9, 0, 9, 0, 1 )]
        public void GetRectangleMeanTest( int x1, int y1, int x2, int y2, float expectedMean )
        {
            float mean = integralImage.GetRectangleMean( x1, y1, x2, y2 );
            Assert.AreEqual( mean, expectedMean );
        }

        [Test]
        [TestCase( 1, 1, 1, 0 )]
        [TestCase( 1, 2, 1, 0 )]
        [TestCase( 2, 2, 1, 0 )]
        [TestCase( 2, 2, 2, 0 )]
        [TestCase( 8, 8, 1, 0 )]
        [TestCase( 5, 5, 5, 0 )]
        [TestCase( 0, 1, 1, 1 )]
        [TestCase( 10, 9, 1, -1 )]
        public void GetHaarXWavelet( int x, int y, int radius, int expectedValue )
        {
            int value = integralImage.GetHaarXWavelet( x, y, radius );
            Assert.AreEqual( value, expectedValue );
        }

        [Test]
        [TestCase( 1, 1, 1, 0 )]
        [TestCase( 1, 2, 1, 0 )]
        [TestCase( 2, 2, 1, 0 )]
        [TestCase( 2, 2, 2, 0 )]
        [TestCase( 8, 8, 1, 0 )]
        [TestCase( 5, 5, 5, 0 )]
        [TestCase( 1, 0, 1, 1 )]
        [TestCase( 9, 10, 1, -1 )]
        public void GetHaarYWavelet( int x, int y, int radius, int expectedValue )
        {
            int value = integralImage.GetHaarYWavelet( x, y, radius );
            Assert.AreEqual( value, expectedValue );
        }
    }
}
