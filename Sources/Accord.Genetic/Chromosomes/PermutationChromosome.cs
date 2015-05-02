// AForge Genetic Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2010
// contacts@aforgenet.com
//

namespace AForge.Genetic
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Permutation chromosome.
    /// </summary>
    /// 
    /// <remarks><para>Permutation chromosome is based on short array chromosome,
    /// but has two features:</para>
    /// <list type="bullet">
    /// <item>all genes are unique within chromosome, i.e. there are no two genes
    /// with the same value;</item>
    /// <item>maximum value of each gene is equal to chromosome length minus 1.</item>
    /// </list>
    /// </remarks>
    /// 
	public class PermutationChromosome : ShortArrayChromosome
	{
		/// <summary>
        /// Initializes a new instance of the <see cref="PermutationChromosome"/> class.
        /// </summary>
		public PermutationChromosome( int length ) : base( length, length - 1 ) { }

		/// <summary>
        /// Initializes a new instance of the <see cref="PermutationChromosome"/> class.
        /// </summary>
        /// 
        /// <param name="source">Source chromosome to copy.</param>
        /// 
        /// <remarks><para>This is a copy constructor, which creates the exact copy
        /// of specified chromosome.</para></remarks>
        /// 
        protected PermutationChromosome( PermutationChromosome source ) : base( source ) { }

        /// <summary>
        /// Generate random chromosome value.
        /// </summary>
        /// 
        /// <remarks><para>Regenerates chromosome's value using random number generator.</para>
        /// </remarks>
        ///
		public override void Generate( )
		{
			// create ascending permutation initially
			for ( int i = 0; i < length; i++ )
			{
				val[i] = (ushort) i;
			}

			// shufle the permutation
			for ( int i = 0, n = length >> 1; i < n; i++ )
			{
				ushort t;
				int j1 = rand.Next( length );
				int j2 = rand.Next( length );

				// swap values
				t		= val[j1];
				val[j1]	= val[j2];
				val[j2]	= t;
			}
		}

        /// <summary>
        /// Create new random chromosome with same parameters (factory method).
        /// </summary>
        /// 
        /// <remarks><para>The method creates new chromosome of the same type, but randomly
        /// initialized. The method is useful as factory method for those classes, which work
        /// with chromosome's interface, but not with particular chromosome type.</para></remarks>
        ///
		public override IChromosome CreateNew( )
		{
			return new PermutationChromosome( length );
		}

        /// <summary>
        /// Clone the chromosome.
        /// </summary>
        /// 
        /// <returns>Return's clone of the chromosome.</returns>
        /// 
        /// <remarks><para>The method clones the chromosome returning the exact copy of it.</para>
        /// </remarks>
        ///
		public override IChromosome Clone( )
		{
			return new PermutationChromosome( this );
		}

        /// <summary>
        /// Mutation operator.
        /// </summary>
        /// 
        /// <remarks><para>The method performs chromosome's mutation, swapping two randomly
        /// chosen genes (array elements).</para></remarks>
        ///
		public override void Mutate( )
		{
			ushort t;
			int j1 = rand.Next( length );
			int j2 = rand.Next( length );

			// swap values
			t		= val[j1];
			val[j1]	= val[j2];
			val[j2]	= t;
		}

        /// <summary>
        /// Crossover operator.
        /// </summary>
        /// 
        /// <param name="pair">Pair chromosome to crossover with.</param>
        /// 
        /// <remarks><para>The method performs crossover between two chromosomes – interchanging
        /// some parts between these chromosomes.</para></remarks>
        ///
		public override void Crossover( IChromosome pair )
		{
			PermutationChromosome p = (PermutationChromosome) pair;

			// check for correct pair
			if ( ( p != null ) && ( p.length == length ) )
			{
				ushort[] child1 = new ushort[length];
				ushort[] child2 = new ushort[length];

				// create two children
				CreateChildUsingCrossover( this.val, p.val, child1 );
				CreateChildUsingCrossover( p.val, this.val, child2 );

				// replace parents with children
				this.val = child1;
				p.val    = child2;
			}
		}

		// Produce new child applying crossover to two parents
		private void CreateChildUsingCrossover( ushort[] parent1, ushort[] parent2, ushort[] child )
		{
            ushort[] indexDictionary1 = CreateIndexDictionary( parent1 );
            ushort[] indexDictionary2 = CreateIndexDictionary( parent2 );

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
                j = indexDictionary1[prev];
				next1 = ( j == k ) ? parent1[0] : parent1[j + 1];
				// 2
                j = indexDictionary2[prev];
                next2 = ( j == k ) ? parent2[0] : parent2[j + 1];

				// check candidate genes for validness
				valid1 = !geneIsBusy[next1];
				valid2 = !geneIsBusy[next2];

				// select gene
				if ( valid1 && valid2 )
				{
					// both candidates are valid
					// select one of theme randomly
					prev = ( rand.Next( 2 ) == 0 ) ? next1 : next2;
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

        // Create dictionary for fast lookup of genes' indexes
        private static ushort[] CreateIndexDictionary( ushort[] genes )
        {
            ushort[] indexDictionary = new ushort[genes.Length];

            for ( int i = 0, n = genes.Length; i < n; i++ )
            {
                indexDictionary[genes[i]] = (ushort) i;
            }

            return indexDictionary;
        }
	}
}
