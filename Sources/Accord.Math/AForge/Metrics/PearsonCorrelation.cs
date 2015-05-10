// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2010
// contacts@aforgenet.com
//

namespace AForge.Math.Metrics
{
    using System;

    /// <summary>
    /// Pearson correlation metric. 
    /// </summary>
    /// 
    /// <remarks><para>This class represents the 
    /// <a href="http://en.wikipedia.org/wiki/Pearson_correlation">Pearson correlation metric</a>.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // instantiate new pearson correlation class
    /// PearsonCorrelation cor = new PearsonCorrelation( ); 
    /// // create two vectors for inputs
    /// double[] p = new double[] { 2.5, 3.5, 3.0, 3.5, 2.5, 3.0 };
    /// double[] q = new double[] { 3.0, 3.5, 1.5, 5.0, 3.5, 3.0 };
    /// // get correlation between the two vectors
    /// double correlation = cor.GetSimilarityScore( p, q );
    /// </code>    
    /// </remarks>
    /// 
    public sealed class PearsonCorrelation : ISimilarity
    {
        /// <summary>
        /// Returns the pearson correlation for two N-dimensional double vectors. 
        /// </summary>
        /// 
        /// <param name="p">1st point vector.</param>
        /// <param name="q">2nd point vector.</param>
        /// 
        /// <returns>Returns Pearson correlation between two supplied vectors.</returns>
        /// 
        /// <exception cref="ArgumentException">Thrown if the two vectors are of different dimensions (if specified
        /// array have different length).</exception>
        /// 
        public double GetSimilarityScore( double[] p, double[] q )
        {
            double pSum = 0, qSum = 0, pSumSq = 0, qSumSq = 0, productSum = 0;
            double pValue, qValue;
            int n = p.Length;

            if ( n != q.Length )
                throw new ArgumentException( "Input vectors must be of the same dimension." );

            for ( int x = 0; x < n; x++ )
            {
                pValue = p[x];
                qValue = q[x];

                pSum += pValue;
                qSum += qValue;
                pSumSq += pValue * pValue;
                qSumSq += qValue * qValue;
                productSum += pValue * qValue;
            }

            double numerator = productSum - ( ( pSum * qSum ) / (double) n );
            double denominator = Math.Sqrt( ( pSumSq - ( pSum * pSum ) / (double) n ) * ( qSumSq - ( qSum * qSum ) / (double) n ) );

            return ( denominator == 0 ) ? 0 : numerator / denominator;
        }
    }
}
