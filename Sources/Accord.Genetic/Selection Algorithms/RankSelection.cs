// AForge Genetic Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

namespace AForge.Genetic
{
	using System;
	using System.Collections;
    using System.Collections.Generic;
    using AForge;

	/// <summary>
	/// Rank selection method.
	/// </summary>
	/// 
	/// <remarks><para>The algorithm selects chromosomes to the new generation depending on
	/// their fitness values - the better fitness value chromosome has, the more chances
	/// it has to become member of the new generation. Each chromosome can be selected
	/// several times to the new generation.</para>
    /// 
    /// <para>This algorithm is similar to <see cref="RouletteWheelSelection">Roulette Wheel
    /// Selection</see> algorithm, but the difference is in "wheel" and its sectors' size
    /// calculation method. The size of the wheel equals to <b>size * ( size + 1 ) / 2</b>,
    /// where <b>size</b> is the current size of population. The worst chromosome has its sector's
    /// size equal to 1, the next chromosome has its sector's size equal to 2, etc.</para>
    /// </remarks>
	/// 
	public class RankSelection : ISelectionMethod
	{
		// random number generator
        private static ThreadSafeRandom rand = new ThreadSafeRandom( );

		/// <summary>
		/// Initializes a new instance of the <see cref="RankSelection"/> class.
		/// </summary>
		public RankSelection( ) { }

		/// <summary>
		/// Apply selection to the specified population.
		/// </summary>
		/// 
		/// <param name="chromosomes">Population, which should be filtered.</param>
		/// <param name="size">The amount of chromosomes to keep.</param>
		/// 
        /// <remarks>Filters specified population keeping only those chromosomes, which
        /// won "roulette" game.</remarks>
		/// 
        public void ApplySelection( List<IChromosome> chromosomes, int size )
		{
			// new population, initially empty
            List<IChromosome> newPopulation = new List<IChromosome>( );
			// size of current population
			int currentSize = chromosomes.Count;

			// sort current population
			chromosomes.Sort( );

			// calculate amount of ranges in the wheel
			double ranges = currentSize * ( currentSize + 1 ) / 2;

			// create wheel ranges
			double[]	rangeMax = new double[currentSize];
			double		s = 0;

			for ( int i = 0, n = currentSize; i < currentSize; i++, n-- )
			{
				s += ( (double) n / ranges );
				rangeMax[i] = s;
			}

			// select chromosomes from old population to the new population
			for ( int j = 0; j < size; j++ )
			{
				// get wheel value
				double wheelValue = rand.NextDouble( );
				// find the chromosome for the wheel value
				for ( int i = 0; i < currentSize; i++ )
				{
					if ( wheelValue <= rangeMax[i] )
					{
						// add the chromosome to the new population
						newPopulation.Add( ((IChromosome) chromosomes[i]).Clone( ) );
						break;
					}
				}
			}

			// empty current population
			chromosomes.Clear( );

			// move elements from new to current population
            chromosomes.AddRange( newPopulation );
		}
	}
}
