using System;
using System.Collections.Generic;
using AForge;
using AForge.Math.Geometry;
using MbUnit.Framework;

namespace AForge.Math.Geometry.Tests
{
    [TestFixture]
    public class GrahamConvexHullTest
    {
        private List<IntPoint> pointsList0 = new List<IntPoint>( );
        private List<IntPoint> pointsList1 = new List<IntPoint>( );
        private List<IntPoint> pointsList2 = new List<IntPoint>( );
        private List<IntPoint> pointsList3 = new List<IntPoint>( );
        private List<IntPoint> pointsList4 = new List<IntPoint>( );
        private List<IntPoint> pointsList5 = new List<IntPoint>( );
        private List<IntPoint> pointsList6 = new List<IntPoint>( );

        private List<IntPoint> pointsList7 = new List<IntPoint>( );
        private List<IntPoint> pointsList8 = new List<IntPoint>( );
        private List<IntPoint> pointsList9 = new List<IntPoint>( );

        private List<IntPoint> expectedHull8 = new List<IntPoint>( );

        private List<List<IntPoint>> pointsLists = new List<List<IntPoint>>( );
        private List<List<IntPoint>> expectedHulls = new List<List<IntPoint>>( );

        public GrahamConvexHullTest( )
        {
            // prepare 0st list
            pointsList0.Add( new IntPoint( 0, 0 ) );

            // prepare 1st list
            pointsList1.Add( new IntPoint( 0, 0 ) );
            pointsList1.Add( new IntPoint( 100, 0 ) );

            // prepare 2nd list
            pointsList2.AddRange( pointsList1 );
            pointsList2.Add( new IntPoint( 100, 100 ) );

            // prepare 3rd list
            pointsList3.AddRange( pointsList2 );
            pointsList3.Add( new IntPoint( 0, 100 ) );

            // prepare 4th list
            pointsList4.AddRange( pointsList2 );
            pointsList4.Add( new IntPoint( 60, 40 ) );

            // prepare 5th list
            pointsList5.AddRange( pointsList3 );
            pointsList5.Add( new IntPoint( 50, 50 ) );

            // prepare 6th list
            pointsList6.AddRange( pointsList3 );
            pointsList6.Add( new IntPoint( 0, 0 ) );

            // prepare 7th list
            pointsList7.AddRange( pointsList3 );
            pointsList7.AddRange( pointsList3 );

            // prepare 8th list
            pointsList8.AddRange( pointsList3 );
            pointsList8.Add( new IntPoint( 50, -10 ) );
            pointsList8.Add( new IntPoint( 110, 50 ) );
            pointsList8.Add( new IntPoint( 50, 110 ) );

            expectedHull8.AddRange( pointsList3 );
            expectedHull8.Insert( 1, new IntPoint( 50, -10 ) );
            expectedHull8.Insert( 3, new IntPoint( 110, 50 ) );
            expectedHull8.Insert( 5, new IntPoint( 50, 110 ) );

            // prepare 9th list
            pointsList9.AddRange( pointsList8 );
            pointsList9.Add( new IntPoint( 50, 10 ) );
            pointsList9.Add( new IntPoint( 90, 50 ) );
            pointsList9.Add( new IntPoint( 50, 90 ) );
            pointsList9.Add( new IntPoint( 10, 50 ) );

            // now prepare list of tests
            pointsLists.Add( pointsList0 );
            pointsLists.Add( pointsList1 );
            pointsLists.Add( pointsList2 );
            pointsLists.Add( pointsList3 );

            expectedHulls.AddRange( pointsLists );

            pointsLists.Add( pointsList4 );
            expectedHulls.Add( pointsList2 );

            pointsLists.Add( pointsList5 );
            expectedHulls.Add( pointsList3 );

            pointsLists.Add( pointsList6 );
            expectedHulls.Add( pointsList3 );

            pointsLists.Add( pointsList7 );
            expectedHulls.Add( pointsList3 );

            pointsLists.Add( pointsList8 );
            expectedHulls.Add( expectedHull8 );

            pointsLists.Add( pointsList9 );
            expectedHulls.Add( expectedHull8 );
        }

        [Test]
        public void FindHullTest( )
        {
            GrahamConvexHull grahamHull = new GrahamConvexHull( );

            for ( int i = 0, n = pointsLists.Count; i < n; i++ )
            {
                ComparePointsLists( grahamHull.FindHull( pointsLists[i] ), expectedHulls[i] );
            }
        }

        private void ComparePointsLists( List<IntPoint> list1, List<IntPoint> list2 )
        {
            Assert.AreEqual<int>( list1.Count, list2.Count );

            if ( list1.Count == list2.Count )
            {
                for ( int i = 0, n = list1.Count; i < n; i++ )
                {
                    Assert.AreEqual<IntPoint>( list2[i], list1[i] );
                }
            }
        }
    }
}
