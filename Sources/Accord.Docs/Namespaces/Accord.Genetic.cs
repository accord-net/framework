// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Copyright © Andrew Kirillov, 2006-2015
// http://www.aforgenet.com/
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Genetic
{
    using System.Runtime.CompilerServices;

    /// <summary>
    ///  Genetic algorithms, genetic programming and evolutionary learning namespace.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Accord.Genetics namespace framework provides classes and functions related to
    ///   genetic algorithms. Those classes have been originated from the <c>AForge.NET Framework</c>
    ///   and can be used to solve many different problems with the help of evolutionary computations
    ///   based on Genetic Algorithms (GA), Genetic Programming (GP) and Gene Expression Programming (GEP).
    ///   The following documentation has been based on the original AForge.NET description of the
    ///   <c>AForge.Genetics</c> namespace: </para>
    /// 
    /// <para>
    ///   The library consists of 3 main sets of classes:
    /// <list>
    ///   <item>
    ///     Collection of different chromosomes' classes implementing IChromosome interface suiting different common problems. These classes implement such operators like crossover and mutation, which are used for driving search through solutions' space by creating new chromosomes representing new solutions.</item>
    ///   <item>
    ///     Collection of fitness functions' classes implementing IFitnessFunction interface, which are used to evaluate chromosomes by calculating their fitness values for different tasks;</item>
    ///   <item>
    ///     Collection of selection algorithms' classes implementing ISelectionMethod interface, which are used to perform selection of population members (chromosomes).</item>
    /// </list>
    /// </para>
    /// 
    /// <para>
    ///   The main class of the library is Population class, which organizes the work of genetic algorithm 
    ///   (GA/GP/GEP) creating initial population of random members, creating new members with the help of 
    ///   crossover and mutations operators, calculating fitness values of new members and performing selection
    ///   of members to keep basing on members' usefulness (fitness). Creating population object it is required 
    ///   to specify which chromosomes, fitness function and selection algorithm to use. Once this is done, the 
    ///   population object is ready to be used for searching of problem's solution:</para>
    ///   
    /// <code>
    /// // create genetic population
    /// var population = new Population(40, baseChromosome, fitnessFunction, selectionAlgorithm);
    /// 
    /// while (true)
    /// {
    ///     // run one epoch of the population
    ///     population.RunEpoch();
    ///     
    ///     // check current best fitness
    ///     if (population.FitnessMax > limit)
    ///     {
    ///         // ...
    ///     }
    /// }
    /// </code>
    /// 
    /// <para>
    ///   As an example, below is small a sample code, which demonstrates usage of Population class for solving 
    ///   quite simple problem - searching function's maximum value.</para>
    ///   
    /// <code>
    /// // define optimization function
    /// public class UserFunction : OptimizationFunction1D
    /// {
    ///     public UserFunction() :
    ///        base(new DoubleRange(0, 255)) { }
    /// 
    ///     public override double OptimizationFunction(double x)
    ///     {
    ///        return Math.Cos(x / 23) * Math.Sin(x / 50) + 2;
    ///     }
    /// }
    /// ...
    /// // create genetic population
    /// var population = new Population(40, new BinaryChromosome(32), new UserFunction(), new EliteSelection());
    /// 
    /// // run one epoch of the population
    /// population.RunEpoch( );
    /// </code>
    /// 
    /// <para>
    ///   The Population class also supports migration of members from one population to another,
    ///   which allows to exchange by good solutions between populations. Also this feature allows
    ///   to run several populations simultaneously in different threads (distributing computations
    ///   on multiple cores/CPUs if they are available) and exchange by good solutions from time to
    ///   time bringing "fresh blood" to populations.</para>
    ///   
    /// <para>
    ///   The namespace class diagram is shown below. </para>
    ///   <img src="..\diagrams\classes\Accord.Genetic.png" />
    /// </remarks>
    ///   
    [CompilerGenerated]
    class NamespaceDoc
    {
    }
}
