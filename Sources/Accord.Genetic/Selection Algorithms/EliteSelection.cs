// AForge Genetic Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2006-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Genetic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Elite selection method.
    /// </summary>
    /// 
    /// <remarks>Elite selection method selects specified amount of
    /// best chromosomes to the next generation.</remarks> 
    /// 
    public class EliteSelection : ISelectionMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EliteSelection"/> class.
        /// </summary>
        public EliteSelection( ) { }

        /// <summary>
        /// Apply selection to the specified population.
        /// </summary>
        /// 
        /// <param name="chromosomes">Population, which should be filtered.</param>
        /// <param name="size">The amount of chromosomes to keep.</param>
        /// 
        /// <remarks>Filters specified population keeping only specified amount of best
        /// chromosomes.</remarks>
        /// 
        public void ApplySelection( List<IChromosome> chromosomes, int size )
        {
            // sort chromosomes
            chromosomes.Sort( );

            // remove bad chromosomes
            chromosomes.RemoveRange( size, chromosomes.Count - size );
        }
    }
}
