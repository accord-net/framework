using System;
using System.Collections.Generic;
using Accord;
using Accord.Math.Geometry;
using NUnit.Framework;

namespace Accord.Tests.Math
{
    [TestFixture]
    public class FlatAnglesOptimizerTest
    {
        private IShapeOptimizer optimizer = new FlatAnglesOptimizer( 160 );

        
        [TestCase( new int[] { 0, 0, 10, 0, 10, 10 }, new int[] { 0, 0, 10, 0, 10, 10 } )]
        [TestCase( new int[] { 0, 0, 20, 0, 10, 1 }, new int[] { 0, 0, 20, 0, 10, 1 } )]
        [TestCase( new int[] { 0, 0, 10, 1, 20, 0, 20, 20 }, new int[] { 0, 0, 20, 0, 20, 20 } )]
        [TestCase( new int[] { 0, 0, 5, 1, 10, 0, 10, 10 }, new int[] { 0, 0, 5, 1, 10, 0, 10, 10 } )]
        [TestCase( new int[] { 0, 0, 20, 0, 20, 20, 11, 9 }, new int[] { 0, 0, 20, 0, 20, 20 } )]
        [TestCase( new int[] { 0, 0, 20, 0, 20, 20, 9, 11 }, new int[] { 0, 0, 20, 0, 20, 20 } )]
        [TestCase( new int[] { 9, 11, 0, 0, 10, 1, 20, 0, 21, 10, 20, 20 }, new int[] { 0, 0, 20, 0, 20, 20 } )]
        [TestCase( new int[] { 11, 9, 0, 0, 10, -1, 20, 0, 19, 10, 20, 20 }, new int[] { 0, 0, 20, 0, 20, 20 } )]
        public void OptimizationTest( int[] coordinates, int[] expectedCoordinates )
        {
            ShapeOptimizerTestBase.TestOptimizer( coordinates, expectedCoordinates, optimizer );
        }
    }
}
