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
    /// Types of genes in Genetic Programming.
    /// </summary>
    public enum GPGeneType
    {
        /// <summary>
        /// Function gene - represents function to be executed.
        /// </summary>
        Function,
        /// <summary>
        /// Argument gene - represents argument of function.
        /// </summary>
        Argument
    }


    /// <summary>
    /// Genetic Programming's gene interface.
    /// </summary>
    /// 
    /// <remarks><para>This is a gene interface, which is used for building chromosomes
    /// in Genetic Programming (GP) and Gene Expression Programming (GEP).
    /// </para></remarks>
    /// 
    public interface IGPGene
    {
        /// <summary>
        /// Gene type.
        /// </summary>
        /// 
        /// <remarks><para>The property represents type of a gene - function, argument, etc.</para>
        /// </remarks>
        /// 
        GPGeneType GeneType { get; }

        /// <summary>
        /// Arguments count.
        /// </summary>
        /// 
        /// <remarks><para>Arguments count of a particular function gene.</para></remarks>
        /// 
        int ArgumentsCount { get; }

        /// <summary>
        /// Maximum arguments count.
        /// </summary>
        /// 
        /// <remarks><para>Maximum arguments count of a function gene. The property may be used
        /// by chromosomes' classes to allocate correctly memory for functions' arguments,
        /// for example.</para></remarks>
        /// 
        int MaxArgumentsCount { get; }

        /// <summary>
        /// Clone gene.
        /// </summary>
        /// 
        /// <remarks><para>The method clones gene returning the exact copy of it.</para></remarks>
        /// 
        IGPGene Clone( );

        /// <summary>
        /// Randomize gene with random type and value.
        /// </summary>
        /// 
        /// <remarks><para>The method randomizes a gene, setting its type and value randomly.</para></remarks>
        /// 
        void Generate( );

        /// <summary>
        /// Randomize gene with random value.
        /// </summary>
        /// 
        /// <param name="type">Gene type to set.</param>
        /// 
        /// <remarks><para>The method randomizes a gene, setting its value randomly, but type
        /// is set to the specified one.</para></remarks>
        /// 
        void Generate( GPGeneType type );

        /// <summary>
        /// Creates new gene with random type and value.
        /// </summary>
        /// 
        /// <remarks><para>The method creates new randomly initialized gene .
        /// The method is useful as factory method for those classes, which work with gene's interface,
        /// but not with particular gene class.</para>
        /// </remarks>
        /// 
        IGPGene CreateNew( );

        /// <summary>
        /// Creates new gene with certain type and random value.
        /// </summary>
        /// 
        /// <param name="type">Gene type to create.</param>
        /// 
        /// <remarks><para>The method creates new gene with specified type, but random value.
        /// The method is useful as factory method for those classes, which work with gene's interface,
        /// but not with particular gene class.</para>
        /// </remarks>
        /// 
        IGPGene CreateNew( GPGeneType type );
    }
}
