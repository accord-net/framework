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

    /// <summary>
    /// Chromosomes' base class.
    /// </summary>
    /// 
    /// <remarks><para>The base class provides implementation of some <see cref="IChromosome"/>
    /// methods and properties, which are identical to all types of chromosomes.</para></remarks>
    /// 
    public abstract class ChromosomeBase : IChromosome
    {
        /// <summary>
        /// Chromosome's fintess value.
        /// </summary>
        protected double fitness = 0;

        /// <summary>
        /// Chromosome's fintess value.
        /// </summary>
        /// 
        /// <remarks><para>Fitness value (usefulness) of the chromosome calculate by calling
        /// <see cref="Evaluate"/> method. The greater the value, the more useful the chromosome.
        /// </para></remarks>
        /// 
        public double Fitness
        {
            get { return fitness; }
        }

        /// <summary>
        /// Generate random chromosome value.
        /// </summary>
        /// 
        /// <remarks><para>Regenerates chromosome's value using random number generator.</para>
        /// </remarks>
        /// 
        public abstract void Generate( );

        /// <summary>
        /// Create new random chromosome with same parameters (factory method).
        /// </summary>
        /// 
        /// <remarks><para>The method creates new chromosome of the same type, but randomly
        /// initialized. The method is useful as factory method for those classes, which work
        /// with chromosome's interface, but not with particular chromosome class.</para></remarks>
        /// 
        public abstract IChromosome CreateNew( );

        /// <summary>
        /// Clone the chromosome.
        /// </summary>
        /// 
        /// <remarks><para>The method clones the chromosome returning the exact copy of it.</para>
        /// </remarks>
        /// 
        public abstract IChromosome Clone( );

        /// <summary>
        /// Mutation operator.
        /// </summary>
        /// 
        /// <remarks><para>The method performs chromosome's mutation, changing its part randomly.</para></remarks>
        /// 
        public abstract void Mutate( );

        /// <summary>
        /// Crossover operator.
        /// </summary>
        /// 
        /// <param name="pair">Pair chromosome to crossover with.</param>
        /// 
        /// <remarks><para>The method performs crossover between two chromosomes – interchanging some parts of chromosomes.</para></remarks>
        /// 
        public abstract void Crossover( IChromosome pair );

        /// <summary>
        /// Evaluate chromosome with specified fitness function.
        /// </summary>
        /// 
        /// <param name="function">Fitness function to use for evaluation of the chromosome.</param>
        /// 
        /// <remarks><para>Calculates chromosome's fitness using the specifed fitness function.</para></remarks>
        ///
        public void Evaluate( IFitnessFunction function )
        {
            fitness = function.Evaluate( this );
        }

        /// <summary>
        /// Compare two chromosomes.
        /// </summary>
        /// 
        /// <param name="o">Binary chromosome to compare to.</param>
        /// 
        /// <returns>Returns comparison result, which equals to 0 if fitness values
        /// of both chromosomes are equal, 1 if fitness value of this chromosome
        /// is less than fitness value of the specified chromosome, -1 otherwise.</returns>
        /// 
        public int CompareTo( object o )
        {
            double f = ( (ChromosomeBase) o ).fitness;

            return ( fitness == f ) ? 0 : ( fitness < f ) ? 1 : -1;
        }
    }
}
