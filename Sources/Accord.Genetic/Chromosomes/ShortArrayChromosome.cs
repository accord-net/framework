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
    using System.Text;
    using AForge;

    /// <summary>
    /// Short array chromosome.
    /// </summary>
    /// 
    /// <remarks><para>Short array chromosome represents array of unsigned short values.
    /// Array length is in the range of [2, 65536].
    /// </para></remarks>
    //
    public class ShortArrayChromosome : ChromosomeBase
    {
        /// <summary>
        /// Chromosome's length in number of elements.
        /// </summary>
        protected int length;

        /// <summary>
        /// Maximum value of chromosome's gene (element).
        /// </summary>
        protected int maxValue;

        /// <summary>
        /// Chromosome's value.
        /// </summary>
        protected ushort[] val = null;

        /// <summary>
        /// Random number generator for chromosoms generation, crossover, mutation, etc.
        /// </summary>
        protected static ThreadSafeRandom	rand = new ThreadSafeRandom( );

        /// <summary>
        /// Chromosome's maximum length.
        /// </summary>
        /// 
        /// <remarks><para>Maxim chromosome's length in array elements.</para></remarks>
        /// 
        public const int MaxLength = 65536;

        /// <summary>
        /// Chromosome's length.
        /// </summary>
        /// 
        /// <remarks><para>Length of the chromosome in array elements.</para></remarks>
        ///
        public int Length
        {
            get { return length; }
        }

        /// <summary>
        /// Chromosome's value.
        /// </summary>
        /// 
        /// <remarks><para>Current value of the chromosome.</para></remarks>
        ///
        public ushort[] Value
        {
            get { return val; }
        }

        /// <summary>
        /// Max possible value of single chromosomes element - gene.
        /// </summary>
        /// 
        /// <remarks><para>Maximum possible numerical value, which may be represented
        /// by single chromosome's gene (array element).</para></remarks>
        /// 
        public int MaxValue
        {
            get { return maxValue; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortArrayChromosome"/> class.
        /// </summary>
        /// 
        /// <param name="length">Chromosome's length in array elements, [2, <see cref="MaxLength"/>].</param>
        /// 
        /// <remarks>This constructor initializes chromosome setting genes' maximum value to
        /// maximum posible value of <see langword="ushort"/> type.</remarks>
        /// 
        public ShortArrayChromosome( int length ) : this( length, ushort.MaxValue ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortArrayChromosome"/> class.
        /// </summary>
        /// 
        /// <param name="length">Chromosome's length in array elements, [2, <see cref="MaxLength"/>].</param>
        /// <param name="maxValue">Maximum value of chromosome's gene (array element).</param>
        /// 
        public ShortArrayChromosome( int length, int maxValue )
        {
            // save parameters
            this.length   = Math.Max( 2, Math.Min( MaxLength, length ) );
            this.maxValue = Math.Max( 1, Math.Min( ushort.MaxValue, maxValue ) );

            // allocate array
            val = new ushort[this.length];

            // generate random chromosome
            Generate( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortArrayChromosome"/> class.
        /// </summary>
        /// 
        /// <param name="source">Source chromosome to copy.</param>
        /// 
        /// <remarks><para>This is a copy constructor, which creates the exact copy
        /// of specified chromosome.</para></remarks>
        /// 
        protected ShortArrayChromosome( ShortArrayChromosome source )
        {
            // copy all properties
            length   = source.length;
            maxValue = source.maxValue;
            val      = (ushort[]) source.val.Clone( );
            fitness  = source.fitness;
        }

        /// <summary>
        /// Get string representation of the chromosome.
        /// </summary>
        /// 
        /// <returns>Returns string representation of the chromosome.</returns>
        ///
        public override string ToString( )
        {
            StringBuilder sb = new StringBuilder( );

            // append first gene
            sb.Append( val[0] );
            // append all other genes
            for ( int i = 1; i < length; i++ )
            {
                sb.Append( ' ' );
                sb.Append( val[i] );
            }

            return sb.ToString( );
        }

        /// <summary>
        /// Generate random chromosome value.
        /// </summary>
        /// 
        /// <remarks><para>Regenerates chromosome's value using random number generator.</para>
        /// </remarks>
        /// 
        public override void Generate( )
        {
            int max = maxValue + 1;

            for ( int i = 0; i < length; i++ )
            {
                // generate next value
                val[i] = (ushort) rand.Next( max );
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
            return new ShortArrayChromosome( length, maxValue );
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
            return new ShortArrayChromosome( this );
        }

        /// <summary>
        /// Mutation operator.
        /// </summary>
        /// 
        /// <remarks><para>The method performs chromosome's mutation, changing randomly
        /// one of its genes (array elements).</para></remarks>
        /// 
        public override void Mutate( )
        {
            // get random index
            int i = rand.Next( length );
            // randomize the gene
            val[i] = (ushort) rand.Next( maxValue + 1 );
        }

        /// <summary>
        /// Crossover operator.
        /// </summary>
        /// 
        /// <param name="pair">Pair chromosome to crossover with.</param>
        /// 
        /// <remarks><para>The method performs crossover between two chromosomes – interchanging
        /// range of genes (array elements) between these chromosomes.</para></remarks>
        ///
        public override void Crossover( IChromosome pair )
        {
            ShortArrayChromosome p = (ShortArrayChromosome) pair;

            // check for correct pair
            if ( ( p != null ) && ( p.length == length ) )
            {
                // crossover point
                int crossOverPoint = rand.Next( length - 1 ) + 1;
                // length of chromosome to be crossed
                int crossOverLength = length - crossOverPoint;
                // temporary array
                ushort[] temp = new ushort[crossOverLength];

                // copy part of first (this) chromosome to temp
                Array.Copy( val, crossOverPoint, temp, 0, crossOverLength );
                // copy part of second (pair) chromosome to the first
                Array.Copy( p.val, crossOverPoint, val, crossOverPoint, crossOverLength );
                // copy temp to the second
                Array.Copy( temp, 0, p.val, crossOverPoint, crossOverLength );
            }
        }
    }
}
