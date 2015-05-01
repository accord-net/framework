// Parallel computations sample application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;

namespace ParallelTest
{
    class Program
    {
        static void Main( string[] args )
        {
            int matrixSize = 250;
            int runs = 10;
            int tests = 5;

            double test1time = 0;
            double test2time = 0;

            Console.WriteLine( "Starting test with " + AForge.Parallel.ThreadsCount + " threads" );

            // allocate matrixes for all tests
            double[,] a  = new double[matrixSize, matrixSize];
            double[,] b  = new double[matrixSize, matrixSize];
            double[,] c1 = new double[matrixSize, matrixSize];
            double[,] c2 = new double[matrixSize, matrixSize];

            Random rand = new Random( );

            // fill source matrixes with random numbers
            for ( int i = 0; i < matrixSize; i++ )
            {
                for ( int j = 0; j < matrixSize; j++ )
                {
                    a[i, j] = rand.NextDouble( );
                    b[i, j] = rand.NextDouble( );
                }
            }

            // run specified number of tests
            for ( int test = 0; test < tests; test++ )
            {
                // test 1
                DateTime start = DateTime.Now;

                for ( int run = 0; run < runs; run++ )
                {
                    Test1( a, b, c1 );
                }

                DateTime end = DateTime.Now;
                TimeSpan span = end - start;

                Console.Write( span.TotalMilliseconds.ToString( "F3" ) + "\t | " );
                test1time += span.TotalMilliseconds;

                // test 2
                start = DateTime.Now;

                for ( int run = 0; run < runs; run++ )
                {
                    Test2( a, b, c2 );
                }

                end = DateTime.Now;
                span = end - start;

                Console.Write( span.TotalMilliseconds.ToString( "F3" ) + "\t | " );
                test2time += span.TotalMilliseconds;

                Console.WriteLine( " " );
            }

            // provide average performance
            test1time /= tests;
            test2time /= tests;

            Console.WriteLine( "----------- AVG -----------" );
            Console.WriteLine( test1time.ToString( "F3" ) + "\t | " + test2time.ToString( "F3" ) + "\t | " );

            Console.WriteLine( "Done" );
        }

        // Test #1 - multiply 2 square matrixes without using parallel computations
        private static void Test1( double[,] a, double[,] b, double[,] c )
        {
            int s = a.GetLength( 0 );

            for ( int i = 0; i < s; i++ )
            {
                for ( int j = 0; j < s; j++ )
                {
                    double v = 0;

                    for ( int k = 0; k < s; k++ )
                    {
                        v += a[i, k] * b[k, j];
                    }

                    c[i, j] = v;
                }
            }
        }

        // Test #2 - multiply 2 square matrixes using parallel computations
        private static void Test2( double[,] a, double[,] b, double[,] c )
        {
            int s = a.GetLength( 0 );

            AForge.Parallel.For( 0, s, delegate( int i )
            {
                for ( int j = 0; j < s; j++ )
                {
                    double v = 0;

                    for ( int k = 0; k < s; k++ )
                    {
                        v += a[i, k] * b[k, j];
                    }

                    c[i, j] = v;
                }
            }
            );
        }
    }
}
