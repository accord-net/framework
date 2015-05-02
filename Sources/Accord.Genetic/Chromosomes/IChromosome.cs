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
    /// Chromosome interface.
    /// </summary>
    /// 
    /// <remarks><para>The interfase should be implemented by all classes, which implement
    /// particular chromosome type.</para></remarks>
    /// 
    public interface IChromosome : IComparable
    {
        /// <summary>
        /// Chromosome's fintess value.
        /// </summary>
        /// 
        /// <remarks><para>The fitness value represents chromosome's usefulness - the greater the
        /// value, the more useful it.</para></remarks>
        /// 
        double Fitness { get; }

        /// <summary>
        /// Generate random chromosome value.
        /// </summary>
        /// 
        /// <remarks><para>Regenerates chromosome's value using random number generator.</para>
        /// </remarks>
        /// 
        void Generate( );

        /// <summary>
        /// Create new random chromosome with same parameters (factory method).
        /// </summary>
        /// 
        /// <remarks><para>The method creates new chromosome of the same type, but randomly
        /// initialized. The method is useful as factory method for those classes, which work
        /// with chromosome's interface, but not with particular chromosome class.</para></remarks>
        /// 
        IChromosome CreateNew( );

        /// <summary>
        /// Clone the chromosome.
        /// </summary>
        /// 
        /// <remarks><para>The method clones the chromosome returning the exact copy of it.</para>
        /// </remarks>
        /// 
        IChromosome Clone( );

        /// <summary>
        /// Mutation operator.
        /// </summary>
        /// 
        /// <remarks><para>The method performs chromosome's mutation, changing its part randomly.</para></remarks>
        /// 
        void Mutate( );

        /// <summary>
        /// Crossover operator.
        /// </summary>
        /// 
        /// <param name="pair">Pair chromosome to crossover with.</param>
        /// 
        /// <remarks><para>The method performs crossover between two chromosomes – interchanging some parts of chromosomes.</para></remarks>
        /// 
        void Crossover( IChromosome pair );

        /// <summary>
        /// Evaluate chromosome with specified fitness function.
        /// </summary>
        /// 
        /// <param name="function">Fitness function to use for evaluation of the chromosome.</param>
        /// 
        /// <remarks><para>Calculates chromosome's fitness using the specifed fitness function.</para></remarks>
        /// 
        void Evaluate( IFitnessFunction function );
    }
}
