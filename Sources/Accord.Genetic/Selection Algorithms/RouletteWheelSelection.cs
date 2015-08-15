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
	/// Roulette wheel selection method.
	/// </summary>
	/// 
	/// <remarks><para>The algorithm selects chromosomes to the new generation according to
	/// their fitness values - the more fitness value chromosome has, the more chances
	/// it has to become member of new generation. Each chromosome can be selected
    /// several times to the new generation.</para>
    /// 
    /// <para>The "roulette's wheel" is divided into sectors, which size is proportional to
    /// the fitness values of chromosomes - the  size of the wheel is the sum of all fitness
    /// values, size of each sector equals to fitness value of chromosome.</para>
    /// </remarks>
	/// 
	public class RouletteWheelSelection : ISelectionMethod
	{
		// random number generator
        private static ThreadSafeRandom rand = new ThreadSafeRandom( );

		/// <summary>
		/// Initializes a new instance of the <see cref="RouletteWheelSelection"/> class.
		/// </summary>
		public RouletteWheelSelection( ) { }

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

			// calculate summary fitness of current population
			double fitnessSum = 0;
			foreach ( IChromosome c in chromosomes )
			{
				fitnessSum += c.Fitness;
			}

			// create wheel ranges
			double[]	rangeMax = new double[currentSize];
			double		s = 0;
			int			k = 0;

			foreach ( IChromosome c in chromosomes )
			{
				// cumulative normalized fitness
				s += ( c.Fitness / fitnessSum );
				rangeMax[k++] = s;
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
