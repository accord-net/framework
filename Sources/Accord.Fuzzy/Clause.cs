// AForge Fuzzy Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Fuzzy
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class represents a fuzzy clause, a linguistic expression of the type "Variable IS Value".
    /// </summary>
    /// 
    /// <remarks><para>A Fuzzy Clause is used to verify if a linguistic variable can be considered
    /// as a specific value at a specific moment. Linguistic variables can only assume value of
    /// their linugistic labels. Because of the nature of the Fuzzy Logic, a Variable can be 
    /// several of its labels at the same time, with different membership values.</para>
    /// 
    /// <para>For example, a linguistic variable "temperature" can be "hot" with a membership 0.3
    /// and "warm" with a membership 0.7 at the same time. To obtain those memberships, Fuzzy Clauses
    /// "temperature is hot" and "temperature is war" can be built.</para>
    /// 
    /// <para>Typically Fuzzy Clauses are used to build Fuzzy Rules (<see cref="Rule"/>).</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create a linguistic variable to represent temperature
    /// LinguisticVariable lvTemperature = new LinguisticVariable( "Temperature", 0, 80 );
    ///
    /// // create the linguistic labels (fuzzy sets) that compose the temperature 
    /// TrapezoidalFunction function1 = new TrapezoidalFunction( 10, 15, TrapezoidalFunction.EdgeType.Right );
    /// FuzzySet fsCold = new FuzzySet( "Cold", function1 );
    /// TrapezoidalFunction function2 = new TrapezoidalFunction( 10, 15, 20, 25 );
    /// FuzzySet fsCool = new FuzzySet( "Cool", function2 );
    /// TrapezoidalFunction function3 = new TrapezoidalFunction( 20, 25, 30, 35 );
    /// FuzzySet fsWarm = new FuzzySet( "Warm", function3 );
    /// TrapezoidalFunction function4 = new TrapezoidalFunction( 30, 35, TrapezoidalFunction.EdgeType.Left );
    /// FuzzySet fsHot  = new FuzzySet( "Hot" , function4 );
    ///
    /// // adding labels to the variable
    /// lvTemperature.AddLabel( fsCold );
    /// lvTemperature.AddLabel( fsCool );
    /// lvTemperature.AddLabel( fsWarm );
    /// lvTemperature.AddLabel( fsHot  );
    /// 
    /// // creating the Clause
    /// Clause fuzzyClause = new Clause( lvTemperature, fsHot );
    /// // setting the numerical input of the variable to evaluate the Clause
    /// lvTemperature.NumericInput = 35;
    /// float result = fuzzyClause.Evaluate( );
    /// Console.WriteLine ( result.ToString( ) );
    /// </code>    
    /// </remarks>
    /// 
    public class Clause
    {
        // the linguistic var of the clause
        private LinguisticVariable variable;
        // the label of the clause
        private FuzzySet label;

        /// <summary>
        /// Gets the <see cref="LinguisticVariable"/> of the <see cref="Clause"/>.
        /// </summary>
        public LinguisticVariable Variable
        {
            get { return variable; }
        }

        /// <summary>
        /// Gets the <see cref="FuzzySet"/> of the <see cref="Clause"/>.
        /// </summary>
        public FuzzySet Label
        {
            get { return label; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Clause"/> class.
        /// </summary>
        /// 
        /// <param name="variable">Linguistic variable of the clause. </param>
        /// 
        /// <param name="label">Label of the linguistic variable, a fuzzy set used as label into the linguistic variable.</param>
        /// 
        /// <exception cref="KeyNotFoundException">The label indicated was not found in the linguistic variable.</exception>
        /// 
        public Clause( LinguisticVariable variable, FuzzySet label )
        {
            // check if label belongs to var.
            variable.GetLabel( label.Name );
            
            // initializing attributes
            this.label    = label;
            this.variable = variable;
        }

        /// <summary>
        /// Evaluates the fuzzy clause.
        /// </summary>
        /// 
        /// <returns>Degree of membership [0..1] of the clause.</returns>
        /// 
        public float Evaluate( )
        {
            return label.GetMembership( variable.NumericInput );
        }

        /// <summary>
        /// Returns the fuzzy clause in its linguistic representation.
        /// </summary>
        /// 
        /// <returns>A string representing the fuzzy clause.</returns>
        /// 
        public override string ToString( )
        {
            return this.variable.Name + " IS " + this.label.Name;
        }
    }
}
