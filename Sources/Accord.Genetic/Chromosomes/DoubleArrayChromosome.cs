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
    using AForge.Math.Random;

    /// <summary>
    /// Double array chromosome.
    /// </summary>
    /// 
    /// <remarks><para>Double array chromosome represents array of double values.
    /// Array length is in the range of [2, 65536].
    /// </para>
    /// 
    /// <para>See documentation to <see cref="Mutate"/> and <see cref="Crossover"/> methods
    /// for information regarding implemented mutation and crossover operators.</para>
    /// </remarks>
    /// 
    public class DoubleArrayChromosome : ChromosomeBase
    {
        /// <summary>
        /// Chromosome generator.
        /// </summary>
        /// 
        /// <remarks><para>This random number generator is used to initialize chromosome's genes,
        /// which is done by calling <see cref="Generate"/> method.</para></remarks>
        /// 
        protected IRandomNumberGenerator chromosomeGenerator;

        /// <summary>
        /// Mutation multiplier generator.
        /// </summary>
        /// 
        /// <remarks><para>This random number generator is used to generate random multiplier values,
        /// which are used to multiply chromosome's genes during mutation.</para></remarks>
        /// 
        protected IRandomNumberGenerator mutationMultiplierGenerator;

        /// <summary>
        /// Mutation addition generator.
        /// </summary>
        /// 
        /// <remarks><para>This random number generator is used to generate random addition values,
        /// which are used to add to chromosome's genes during mutation.</para></remarks>
        /// 
        protected IRandomNumberGenerator mutationAdditionGenerator;

        /// <summary>
        /// Random number generator for crossover and mutation points selection.
        /// </summary>
        /// 
        /// <remarks><para>This random number generator is used to select crossover
        /// and mutation points.</para></remarks>
        /// 
        protected static ThreadSafeRandom rand = new ThreadSafeRandom( );

        /// <summary>
        /// Chromosome's maximum length.
        /// </summary>
        /// 
        /// <remarks><para>Maxim chromosome's length in array elements.</para></remarks>
        /// 
        public const int MaxLength = 65536;
        
        /// <summary>
        /// Chromosome's length in number of elements.
        /// </summary>
        private int length;

        /// <summary>
        /// Chromosome's value.
        /// </summary>
        protected double[] val = null;

        // balancers to control type of mutation and crossover
        private double mutationBalancer = 0.5;
        private double crossoverBalancer = 0.5;

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
        public double[] Value
        {
            get { return val; }
        }

        /// <summary>
        /// Mutation balancer to control mutation type, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>The property controls type of mutation, which is used more
        /// frequently. A radnom number is generated each time before doing mutation -
        /// if the random number is smaller than the specified balance value, then one
        /// mutation type is used, otherwse another. See <see cref="Mutate"/> method
        /// for more information.</para>
        /// 
        /// <para>Default value is set to <b>0.5</b>.</para>
        /// </remarks>
        /// 
        public double MutationBalancer
        {
            get { return mutationBalancer; }
            set { mutationBalancer = Math.Max( 0.0, Math.Min( 1.0, value ) ); }
        }

        /// <summary>
        /// Crossover balancer to control crossover type, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>The property controls type of crossover, which is used more
        /// frequently. A radnom number is generated each time before doing crossover -
        /// if the random number is smaller than the specified balance value, then one
        /// crossover type is used, otherwse another. See <see cref="Crossover"/> method
        /// for more information.</para>
        /// 
        /// <para>Default value is set to <b>0.5</b>.</para>
        /// </remarks>
        /// 
        public double CrossoverBalancer
        {
            get { return crossoverBalancer; }
            set { crossoverBalancer = Math.Max( 0.0, Math.Min( 1.0, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleArrayChromosome"/> class.
        /// </summary>
        /// 
        /// <param name="chromosomeGenerator">Chromosome generator - random number generator, which is 
        /// used to initialize chromosome's genes, which is done by calling <see cref="Generate"/> method
        /// or in class constructor.</param>
        /// <param name="mutationMultiplierGenerator">Mutation multiplier generator - random number
        /// generator, which is used to generate random multiplier values, which are used to
        /// multiply chromosome's genes during mutation.</param>
        /// <param name="mutationAdditionGenerator">Mutation addition generator - random number
        /// generator, which is used to generate random addition values, which are used to
        /// add to chromosome's genes during mutation.</param>
        /// <param name="length">Chromosome's length in array elements, [2, <see cref="MaxLength"/>].</param>
        /// 
        /// <remarks><para>The constructor initializes the new chromosome randomly by calling
        /// <see cref="Generate"/> method.</para></remarks>
        /// 
        public DoubleArrayChromosome(
            IRandomNumberGenerator chromosomeGenerator,
            IRandomNumberGenerator mutationMultiplierGenerator,
            IRandomNumberGenerator mutationAdditionGenerator,
            int length )
        {

            // save parameters
            this.chromosomeGenerator = chromosomeGenerator;
            this.mutationMultiplierGenerator = mutationMultiplierGenerator;
            this.mutationAdditionGenerator = mutationAdditionGenerator;
            this.length = Math.Max( 2, Math.Min( MaxLength, length ) ); ;

            // allocate array
            val = new double[length];

            // generate random chromosome
            Generate( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleArrayChromosome"/> class.
        /// </summary>
        /// 
        /// <param name="chromosomeGenerator">Chromosome generator - random number generator, which is 
        /// used to initialize chromosome's genes, which is done by calling <see cref="Generate"/> method
        /// or in class constructor.</param>
        /// <param name="mutationMultiplierGenerator">Mutation multiplier generator - random number
        /// generator, which is used to generate random multiplier values, which are used to
        /// multiply chromosome's genes during mutation.</param>
        /// <param name="mutationAdditionGenerator">Mutation addition generator - random number
        /// generator, which is used to generate random addition values, which are used to
        /// add to chromosome's genes during mutation.</param>
        /// <param name="values">Values used to initialize the chromosome.</param>
        /// 
        /// <remarks><para>The constructor initializes the new chromosome with specified <paramref name="values">values</paramref>.
        /// </para></remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">Invalid length of values array.</exception>
        /// 
        public DoubleArrayChromosome(
            IRandomNumberGenerator chromosomeGenerator,
            IRandomNumberGenerator mutationMultiplierGenerator,
            IRandomNumberGenerator mutationAdditionGenerator,
            double[] values )
        {
            if ( ( values.Length < 2 ) || ( values.Length > MaxLength ) )
                throw new ArgumentOutOfRangeException( "Invalid length of values array." );

          // save parameters
            this.chromosomeGenerator = chromosomeGenerator;
            this.mutationMultiplierGenerator = mutationMultiplierGenerator;
            this.mutationAdditionGenerator = mutationAdditionGenerator;
            this.length = values.Length;

            // copy specified values
            val = (double[]) values.Clone( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleArrayChromosome"/> class.
        /// </summary>
        /// 
        /// <param name="source">Source chromosome to copy.</param>
        /// 
        /// <remarks><para>This is a copy constructor, which creates the exact copy
        /// of specified chromosome.</para></remarks>
        /// 
        public DoubleArrayChromosome( DoubleArrayChromosome source )
        {
            this.chromosomeGenerator = source.chromosomeGenerator;
            this.mutationMultiplierGenerator = source.mutationMultiplierGenerator;
            this.mutationAdditionGenerator = source.mutationAdditionGenerator;
            this.length  = source.length;
            this.fitness = source.fitness;
            this.mutationBalancer = source.mutationBalancer;
            this.crossoverBalancer = source.crossoverBalancer;

            // copy genes
            val = (double[]) source.val.Clone( );
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
            for ( int i = 0; i < length; i++ )
            {
                // generate next value
                val[i] = chromosomeGenerator.Next( );
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
            return new DoubleArrayChromosome( chromosomeGenerator, mutationMultiplierGenerator, mutationAdditionGenerator, length );
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
            return new DoubleArrayChromosome( this );
        }

        /// <summary>
        /// Mutation operator.
        /// </summary>
        /// 
        /// <remarks><para>The method performs chromosome's mutation, adding random number
        /// to chromosome's gene or multiplying the gene by random number. These random
        /// numbers are generated with help of <see cref="mutationMultiplierGenerator">mutation
        /// multiplier</see> and <see cref="mutationAdditionGenerator">mutation
        /// addition</see> generators.</para>
        /// 
        /// <para>The exact type of mutation applied to the particular gene
        /// is selected randomly each time and depends on <see cref="MutationBalancer"/>.
        /// Before mutation is done a random number is generated in [0, 1] range - if the
        /// random number is smaller than <see cref="MutationBalancer"/>, then multiplication
        /// mutation is done, otherwise addition mutation.
        /// </para></remarks>
        /// 
        public override void Mutate( )
        {
            int mutationGene = rand.Next( length );

            if ( rand.NextDouble( ) < mutationBalancer )
            {
                val[mutationGene] *= mutationMultiplierGenerator.Next( );
            }
            else
            {
                val[mutationGene] += mutationAdditionGenerator.Next( );
            }
        }

        /// <summary>
        /// Crossover operator.
        /// </summary>
        /// 
        /// <param name="pair">Pair chromosome to crossover with.</param>
        /// 
        /// <remarks><para>The method performs crossover between two chromosomes, selecting
        /// randomly the exact type of crossover to perform, which depends on <see cref="CrossoverBalancer"/>.
        /// Before crossover is done a random number is generated in [0, 1] range - if the
        /// random number is smaller than <see cref="CrossoverBalancer"/>, then the first crossover
        /// type is used, otherwise second type is used.</para>
        /// 
        /// <para>The <b>first crossover type</b> is based on interchanging
        /// range of genes (array elements) between these chromosomes and is known
        /// as one point crossover. A crossover point is selected randomly and chromosomes
        /// interchange genes, which start from the selected point.</para>
        /// 
        /// <para>The <b>second crossover type</b> is aimed to produce one child, which genes'
        /// values are between corresponding genes of parents, and another child, which genes'
        /// values are outside of the range formed by corresponding genes of parents. 
        /// Let take, for example, two genes with 1.0 and 3.0 valueû (of course chromosomes have
        /// more genes, but for simplicity lets think about one). First of all we randomly choose
        /// a factor in the [0, 1] range, let's take 0.4. Then, for each pair of genes (we have
        /// one pair) we calculate difference value, which is 2.0 in our case. In the result we’ll
        /// have two children – one between and one outside of the range formed by parents genes' values.
        /// We may have 1.8 and 3.8 children, or we may have 0.2 and 2.2 children. As we can see
        /// we add/subtract (chosen randomly) <i>difference * factor</i>. So, this gives us exploration
        /// in between and in near outside. The randomly chosen factor is applied to all genes
        /// of the chromosomes participating in crossover.</para>
        /// </remarks>
        ///
        public override void Crossover( IChromosome pair )
        {
            DoubleArrayChromosome p = (DoubleArrayChromosome) pair;

            // check for correct pair
            if ( ( p != null ) && ( p.length == length ) )
            {
                if ( rand.NextDouble( ) < crossoverBalancer )
                {
                    // crossover point
                    int crossOverPoint = rand.Next( length - 1 ) + 1;
                    // length of chromosome to be crossed
                    int crossOverLength = length - crossOverPoint;
                    // temporary array
                    double[] temp = new double[crossOverLength];

                    // copy part of first (this) chromosome to temp
                    Array.Copy( val, crossOverPoint, temp, 0, crossOverLength );
                    // copy part of second (pair) chromosome to the first
                    Array.Copy( p.val, crossOverPoint, val, crossOverPoint, crossOverLength );
                    // copy temp to the second
                    Array.Copy( temp, 0, p.val, crossOverPoint, crossOverLength );
                }
                else
                {
                    double[] pairVal = p.val;

                    double factor = rand.NextDouble( );
                    if ( rand.Next( 2 ) == 0 )
                        factor = -factor;

                    for ( int i = 0; i < length; i++ )
                    {
                        double portion = ( val[i] - pairVal[i] ) * factor;

                        val[i] -= portion;
                        pairVal[i] += portion;
                    }
                }
            }
        }
    }
}
