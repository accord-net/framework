// Traveling Salesman Problem using Genetic Algorithms
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using AForge.Genetic;

namespace TSP
{
	/// <summary>
	/// The chromosome is to solve TSP task (Travailing Salesman Problem).
	/// </summary>
	public class TSPChromosome : PermutationChromosome
	{
		private double[,] map = null;

		/// <summary>
		/// Constructor
		/// </summary>
		public TSPChromosome( double[,] map ) : base( map.GetLength( 0 ) )
		{
			this.map = map;
		}

		/// <summary>
		/// Copy Constructor
		/// </summary>
		protected TSPChromosome( TSPChromosome source ) : base( source )
		{
			this.map = source.map;
		}

		/// <summary>
		/// Create new random chromosome (factory method)
		/// </summary>
		public override IChromosome CreateNew( )
		{
			return new TSPChromosome( map );
		}

		/// <summary>
		/// Clone the chromosome
		/// </summary>
		public override IChromosome Clone( )
		{
			return new TSPChromosome( this );
		}

		/// <summary>
		/// Crossover operator
		/// </summary>
		public override void Crossover( IChromosome pair )
		{
			TSPChromosome p = (TSPChromosome) pair;

			// check for correct pair
			if ( ( p != null ) && ( p.length == length ) )
			{
				ushort[] child1 = new ushort[length];
				ushort[] child2 = new ushort[length];

				// create two children
				CreateChildUsingCrossover( this.val, p.val, child1 );
				CreateChildUsingCrossover( p.val, this.val, child2 );

				// replace parents with children
				this.val	= child1;
				p.val		= child2;
			}
		}

		// Produce new child applying crossover to two parents
		private void CreateChildUsingCrossover( ushort[] parent1, ushort[] parent2, ushort[] child )
		{
			// temporary array to specify if certain gene already
			// present in the child
			bool[]	geneIsBusy = new bool[length];
			// previous gene in the child and two next candidates
			ushort	prev, next1, next2;
			// candidates validness - candidate is valid, if it is not
			// yet in the child
			bool	valid1, valid2;

			int		j, k = length - 1;

			// first gene of the child is taken from the second parent
			prev = child[0] = parent2[0];
			geneIsBusy[prev] = true;

			// resolve all other genes of the child
			for ( int i = 1; i < length; i++ )
			{
				// find the next gene after PREV in both parents
				// 1
				for ( j = 0; j < k; j++ )
				{
					if ( parent1[j] == prev )
						break;
				}
				next1 = ( j == k ) ? parent1[0] : parent1[j + 1];
				// 2
				for ( j = 0; j < k; j++ )
				{
					if ( parent2[j] == prev )
						break;
				}
				next2 = ( j == k ) ? parent2[0] : parent2[j + 1];

				// check candidate genes for validness
				valid1 = !geneIsBusy[next1];
				valid2 = !geneIsBusy[next2];

				// select gene
				if ( valid1 && valid2 )
				{
					// both candidates are valid
					// select one closest city
					double dx1 = map[next1, 0] - map[prev, 0];
					double dy1 = map[next1, 1] - map[prev, 1];
					double dx2 = map[next2, 0] - map[prev, 0];
					double dy2 = map[next2, 1] - map[prev, 1];

					prev = ( Math.Sqrt( dx1 * dx1 + dy1 * dy1 ) < Math.Sqrt( dx2 * dx2 + dy2 * dy2 ) ) ? next1 : next2; 
				}
				else if ( !( valid1 || valid2 ) )
				{
					// none of candidates is valid, so
					// select random gene which is not in the child yet
					int r = j = rand.Next( length );

					// go down first
					while ( ( r < length ) && ( geneIsBusy[r] == true ) )
						r++;
					if ( r == length )
					{
						// not found, try to go up
						r = j - 1;
						while ( geneIsBusy[r] == true )	// && ( r >= 0 )
							r--;
					}
					prev = (ushort) r;
				}
				else
				{
					// one of candidates is valid
					prev = ( valid1 ) ? next1 : next2;
				}

				child[i] = prev;
				geneIsBusy[prev] = true;
			}
		}
	}
}
