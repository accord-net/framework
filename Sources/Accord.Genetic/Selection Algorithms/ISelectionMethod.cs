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
    /// Genetic selection method interface.
    /// </summary>
    /// 
    /// <remarks>The interface should be implemented by all classes, which
    /// implement genetic selection algorithm. These algorithms select members of
    /// current generation, which should be kept in the new generation. Basically,
    /// these algorithms filter provided population keeping only specified amount of
    /// members.</remarks>
    /// 
    public interface ISelectionMethod
    {
        /// <summary>
        /// Apply selection to the specified population.
        /// </summary>
        /// 
        /// <param name="chromosomes">Population, which should be filtered.</param>
        /// <param name="size">The amount of chromosomes to keep.</param>
        /// 
        /// <remarks>Filters specified population according to the implemented
        /// selection algorithm.</remarks>
        /// 
        void ApplySelection( List<IChromosome> chromosomes, int size );
    }
}