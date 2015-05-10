// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2014
// aforge.net@gmail.com
//

namespace AForge.Math
{
    using System;

    // Just a copy-paste of SVD algorithm from Numerical Recipes but updated for C#
    // (as authors state, the code is aimed to be machine readable, so blame them
    // for all those c/f/g/h/s variable)
    internal class svd
    {
        public static void svdcmp( double[,] a, out double[] w, out double[,] v )
        {
            // number of rows in A
            int m = a.GetLength( 0 );
            // number of columns in A
            int n = a.GetLength( 1 );

            if ( m < n )
            {
                throw new ArgumentException( "Number of rows in A must be greater or equal to number of columns" );
            }

            w = new double[n];
            v = new double[n, n];


            int flag, i, its, j, jj, k, l = 0, nm = 0;
            double anorm, c, f, g, h, s, scale, x, y, z;

            double[] rv1 = new double[n];

            // householder reduction to bidiagonal form
            g = scale = anorm = 0.0;

            for ( i = 0; i < n; i++ )
            {
                l = i + 1;
                rv1[i] = scale * g;
                g = s = scale = 0;

                if ( i < m )
                {
                    for ( k = i; k < m; k++ )
                    {
                        scale += System.Math.Abs( a[k, i] );
                    }

                    if ( scale != 0.0 )
                    {
                        for ( k = i; k < m; k++ )
                        {
                            a[k, i] /= scale;
                            s += a[k, i] * a[k, i];
                        }

                        f = a[i, i];
                        g = -Sign( System.Math.Sqrt( s ), f );
                        h = f * g - s;
                        a[i, i] = f - g;

                        if ( i != n - 1 )
                        {
                            for ( j = l; j < n; j++ )
                            {
                                for ( s = 0.0, k = i; k < m; k++ )
                                {
                                    s += a[k, i] * a[k, j];
                                }

                                f = s / h;

                                for ( k = i; k < m; k++ )
                                {
                                    a[k, j] += f * a[k, i];
                                }
                            }
                        }

                        for ( k = i; k < m; k++ )
                        {
                            a[k, i] *= scale;
                        }
                    }
                }

                w[i] = scale * g;
                g = s = scale = 0.0;

                if ( ( i < m ) && ( i != n - 1 ) )
                {
                    for ( k = l; k < n; k++ )
                    {
                        scale += System.Math.Abs( a[i, k] );
                    }

                    if ( scale != 0.0 )
                    {
                        for ( k = l; k < n; k++ )
                        {
                            a[i, k] /= scale;
                            s += a[i, k] * a[i, k];
                        }

                        f = a[i, l];
                        g = -Sign( System.Math.Sqrt( s ), f );
                        h = f * g - s;
                        a[i, l] = f - g;

                        for ( k = l; k < n; k++ )
                        {
                            rv1[k] = a[i, k] / h;
                        }

                        if ( i != m - 1 )
                        {
                            for ( j = l; j < m; j++ )
                            {
                                for ( s = 0.0, k = l; k < n; k++ )
                                {
                                    s += a[j, k] * a[i, k];
                                }
                                for ( k = l; k < n; k++ )
                                {
                                    a[j, k] += s * rv1[k];
                                }
                            }
                        }

                        for ( k = l; k < n; k++ )
                        {
                            a[i, k] *= scale;
                        }
                    }
                }
                anorm = System.Math.Max( anorm, ( System.Math.Abs( w[i] ) + System.Math.Abs( rv1[i] ) ) );
            }

            // accumulation of right-hand transformations
            for ( i = n - 1; i >= 0; i-- )
            {
                if ( i < n - 1 )
                {
                    if ( g != 0.0 )
                    {
                        for ( j = l; j < n; j++ )
                        {
                            v[j, i] = ( a[i, j] / a[i, l] ) / g;
                        }

                        for ( j = l; j < n; j++ )
                        {
                            for ( s = 0, k = l; k < n; k++ )
                            {
                                s += a[i, k] * v[k, j];
                            }
                            for ( k = l; k < n; k++ )
                            {
                                v[k, j] += s * v[k, i];
                            }
                        }
                    }
                    for ( j = l; j < n; j++ )
                    {
                        v[i, j] = v[j, i] = 0;
                    }
                }
                v[i, i] = 1;
                g = rv1[i];
                l = i;
            }

            // accumulation of left-hand transformations
            for ( i = n - 1; i >= 0; i-- )
            {
                l = i + 1;
                g = w[i];

                if ( i < n - 1 )
                {
                    for ( j = l; j < n; j++ )
                    {
                        a[i, j] = 0.0;
                    }
                }

                if ( g != 0 )
                {
                    g = 1.0 / g;

                    if ( i != n - 1 )
                    {
                        for ( j = l; j < n; j++ )
                        {
                            for ( s = 0, k = l; k < m; k++ )
                            {
                                s += a[k, i] * a[k, j];
                            }

                            f = ( s / a[i, i] ) * g;

                            for ( k = i; k < m; k++ )
                            {
                                a[k, j] += f * a[k, i];
                            }
                        }
                    }

                    for ( j = i; j < m; j++ )
                    {
                        a[j, i] *= g;
                    }
                }
                else
                {
                    for ( j = i; j < m; j++ )
                    {
                        a[j, i] = 0;
                    }
                }
                ++a[i, i];
            }

            // diagonalization of the bidiagonal form: Loop over singular values
            // and over allowed iterations
            for ( k = n - 1; k >= 0; k-- )
            {
                for ( its = 1; its <= 30; its++ )
                {
                    flag = 1;

                    for ( l = k; l >= 0; l-- )
                    {
                        // test for splitting
                        nm = l - 1;

                        if ( System.Math.Abs( rv1[l] ) + anorm == anorm )
                        {
                            flag = 0;
                            break;
                        }

                        if ( System.Math.Abs( w[nm] ) + anorm == anorm )
                            break;
                    }

                    if ( flag != 0 )
                    {
                        c = 0.0;
                        s = 1.0;
                        for ( i = l; i <= k; i++ )
                        {
                            f = s * rv1[i];

                            if ( System.Math.Abs( f ) + anorm != anorm )
                            {
                                g = w[i];
                                h = Pythag( f, g );
                                w[i] = h;
                                h = 1.0 / h;
                                c = g * h;
                                s = -f * h;

                                for ( j = 0; j < m; j++ )
                                {
                                    y = a[j, nm];
                                    z = a[j, i];
                                    a[j, nm] = y * c + z * s;
                                    a[j, i]  = z * c - y * s;
                                }
                            }
                        }
                    }

                    z = w[k];

                    if ( l == k )
                    {
                        // convergence
                        if ( z < 0.0 )
                        {
                            // singular value is made nonnegative
                            w[k] = -z;

                            for ( j = 0; j < n; j++ )
                            {
                                v[j, k] = -v[j, k];
                            }
                        }
                        break;
                    }

                    if ( its == 30 )
                    {
                        throw new ApplicationException( "No convergence in 30 svdcmp iterations" );
                    }

                    // shift from bottom 2-by-2 minor
                    x = w[l];
                    nm = k - 1;
                    y = w[nm];
                    g = rv1[nm];
                    h = rv1[k];
                    f = ( ( y - z ) * ( y + z ) + ( g - h ) * ( g + h ) ) / ( 2.0 * h * y );
                    g = Pythag( f, 1.0 );
                    f = ( ( x - z ) * ( x + z ) + h * ( ( y / ( f + Sign( g, f ) ) ) - h ) ) / x;

                    // next QR transformation
                    c = s = 1.0;

                    for ( j = l; j <= nm; j++ )
                    {
                        i = j + 1;
                        g = rv1[i];
                        y = w[i];
                        h = s * g;
                        g = c * g;
                        z = Pythag( f, h );
                        rv1[j] = z;
                        c = f / z;
                        s = h / z;
                        f = x * c + g * s;
                        g = g * c - x * s;
                        h = y * s;
                        y *= c;

                        for ( jj = 0; jj < n; jj++ )
                        {
                            x = v[jj, j];
                            z = v[jj, i];
                            v[jj, j] = x * c + z * s;
                            v[jj, i] = z * c - x * s;
                        }

                        z = Pythag( f, h );
                        w[j] = z;

                        if ( z != 0 )
                        {
                            z = 1.0 / z;
                            c = f * z;
                            s = h * z;
                        }

                        f = c * g + s * y;
                        x = c * y - s * g;

                        for ( jj = 0; jj < m; jj++ )
                        {
                            y = a[jj, j];
                            z = a[jj, i];
                            a[jj, j] = y * c + z * s;
                            a[jj, i] = z * c - y * s;
                        }
                    }

                    rv1[l] = 0.0;
                    rv1[k] = f;
                    w[k] = x;
                }
            }
        }

        private static double Sign( double a, double b )
        {
            return ( b >= 0.0 ) ? System.Math.Abs( a ) : -System.Math.Abs( a );
        }

        private static double Pythag( double a, double b )
        {
            double at = System.Math.Abs( a ), bt = System.Math.Abs( b ), ct, result;

            if ( at > bt )
            {
                ct = bt / at;
                result = at * System.Math.Sqrt( 1.0 + ct * ct );
            }
            else if ( bt > 0.0 )
            {
                ct = at / bt;
                result = bt * System.Math.Sqrt( 1.0 + ct * ct );
            }
            else
            {
                result = 0.0;
            }

            return result;
        }
    }
}
