// AForge Framework
// Traveling Salesman Problem using Genetic Algorithms
//
// Copyright © Andrew Kirillov, 2006-2008
// andrew.kirillov@gmail.com
//

using System;
using AForge.Genetic;

namespace TSP
{
	/// <summary>
	/// Fitness function for TSP task (Travaling Salasman Problem)
	/// </summary>
	public class TSPFitnessFunction : IFitnessFunction
	{
		// map
		private double[,]	map = null;

		// Constructor
		public TSPFitnessFunction( double[,] map )
		{
			this.map = map;
		}

		/// <summary>
		/// Evaluate chromosome - calculates its fitness value
		/// </summary>
		public double Evaluate( IChromosome chromosome )
		{
			return 1 / ( PathLength( chromosome ) + 1 );
		}

		/// <summary>
		/// Translate genotype to phenotype 
		/// </summary>
		public object Translate( IChromosome chromosome )
		{
			return chromosome.ToString( );
		}

		/// <summary>
		/// Calculate path length represented by the specified chromosome 
		/// </summary>
		public double PathLength( IChromosome chromosome )
		{
			// salesman path
			ushort[] path = ((PermutationChromosome) chromosome).Value;

			// check path size
			if ( path.Length != map.GetLength( 0 ) )
			{
				throw new ArgumentException( "Invalid path specified - not all cities are visited" );
			}

			// path length
			int		prev = path[0];
			int		curr = path[path.Length - 1];

			// calculate distance between the last and the first city
			double	dx = map[curr, 0] - map[prev, 0];
			double	dy = map[curr, 1] - map[prev, 1];
			double	pathLength = Math.Sqrt( dx * dx + dy * dy );

			// calculate the path length from the first city to the last
			for ( int i = 1, n = path.Length; i < n; i++ )
			{
				// get current city
				curr = path[i];

				// calculate distance
				dx = map[curr, 0] - map[prev, 0];
				dy = map[curr, 1] - map[prev, 1];
				pathLength += Math.Sqrt( dx * dx + dy * dy );

				// put current city as previous
				prev = curr;
			}

			return pathLength;
		}
	}
}
