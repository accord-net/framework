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
    /// This class represents a Fuzzy Inference System. 
    /// </summary>
    /// 
    /// <remarks><para>A Fuzzy Inference System is a model capable of executing fuzzy computing.
    /// It is mainly composed by a <see cref="Database"/> with the linguistic variables
    /// (see <see cref="LinguisticVariable"/>) and a <see cref="Rulebase"/>
    /// with the fuzzy rules (see <see cref="Rule"/>) that represent the behavior of the system.
    /// The typical operation of a Fuzzy Inference System is:
    /// <list type="bullet">
    /// <item>Get the numeric inputs;</item>
    /// <item>Use the <see cref="Database"/> with the linguistic variables
    /// (see <see cref="LinguisticVariable"/>) to obtain linguistic meaning for each
    /// numerical input;</item>
    /// <item>Verify which rules (see <see cref="Rule"/>) of the <see cref="Rulebase"/> are
    /// activated by the input;</item>
    /// <item>Combine the consequent of the activated rules to obtain a <see cref="FuzzyOutput"/>;</item>
    /// <item>Use some defuzzifier (see <see cref="IDefuzzifier"/>) to obtain a numerical output. </item>
    /// </list>
    /// </para>
    /// 
    /// <para>The following sample usage is a Fuzzy Inference System that controls an
    /// auto guided vehicle avoing frontal collisions:</para>
    /// <code>
    /// // linguistic labels (fuzzy sets) that compose the distances
    /// FuzzySet fsNear = new FuzzySet( "Near",
    ///     new TrapezoidalFunction( 15, 50, TrapezoidalFunction.EdgeType.Right ) );
    /// FuzzySet fsMedium = new FuzzySet( "Medium",
    ///     new TrapezoidalFunction( 15, 50, 60, 100 ) );
    /// FuzzySet fsFar = new FuzzySet( "Far",
    ///     new TrapezoidalFunction( 60, 100, TrapezoidalFunction.EdgeType.Left ) );
    ///             
    /// // front distance (input)
    /// LinguisticVariable lvFront = new LinguisticVariable( "FrontalDistance", 0, 120 );
    /// lvFront.AddLabel( fsNear );
    /// lvFront.AddLabel( fsMedium );
    /// lvFront.AddLabel( fsFar );
    /// 
    /// // linguistic labels (fuzzy sets) that compose the angle
    /// FuzzySet fsZero = new FuzzySet( "Zero",
    ///     new TrapezoidalFunction( -10, 5, 5, 10 ) );
    /// FuzzySet fsLP = new FuzzySet( "LittlePositive",
    ///     new TrapezoidalFunction( 5, 10, 20, 25 ) );
    /// FuzzySet fsP = new FuzzySet( "Positive",
    ///     new TrapezoidalFunction( 20, 25, 35, 40 ) );
    /// FuzzySet fsVP = new FuzzySet( "VeryPositive",
    ///     new TrapezoidalFunction( 35, 40, TrapezoidalFunction.EdgeType.Left ) );
    /// 
    /// // angle
    /// LinguisticVariable lvAngle = new LinguisticVariable( "Angle", -10, 50 );
    /// lvAngle.AddLabel( fsZero );
    /// lvAngle.AddLabel( fsLP );
    /// lvAngle.AddLabel( fsP );
    /// lvAngle.AddLabel( fsVP );
    /// 
    /// // the database
    /// Database fuzzyDB = new Database( );
    /// fuzzyDB.AddVariable( lvFront );
    /// fuzzyDB.AddVariable( lvAngle );
    /// 
    /// // creating the inference system
    /// InferenceSystem IS = new InferenceSystem( fuzzyDB, new CentroidDefuzzifier( 1000 ) );
    /// 
    /// // going Straight
    /// IS.NewRule( "Rule 1", "IF FrontalDistance IS Far THEN Angle IS Zero" );
    /// // Turning Left
    /// IS.NewRule( "Rule 2", "IF FrontalDistance IS Near THEN Angle IS Positive" );
    /// 
    /// ...
    /// // inference section
    /// 
    /// // setting inputs
    /// IS.SetInput( "FrontalDistance", 20 );
    /// 
    /// // getting outputs
    /// try
    /// {
    ///     float newAngle = IS.Evaluate( "Angle" );
    /// }
    /// catch ( Exception )
    /// {
    /// ...
    /// }
    /// </code>    
    /// </remarks>
    /// 
    public class InferenceSystem
    {
        // The linguistic variables of this system
        private Database database;
        // The fuzzy rules of this system
        private Rulebase rulebase;
        // The defuzzifier method choosen 
        private IDefuzzifier defuzzifier;
        // Norm operator used in rules and deffuzification
        private INorm normOperator;
        // CoNorm operator used in rules
        private ICoNorm conormOperator;


        /// <summary>
        /// Initializes a new Fuzzy <see cref="InferenceSystem"/>.
        /// </summary>
        /// 
        /// <param name="database">A fuzzy <see cref="Database"/> containing the system linguistic variables.</param>
        /// <param name="defuzzifier">A defuzzyfier method used to evaluate the numeric uotput of the system.</param>
        /// 
        public InferenceSystem( Database database, IDefuzzifier defuzzifier )
            : this( database, defuzzifier, new MinimumNorm( ), new MaximumCoNorm( ) )
        {
        }

        /// <summary>
        /// Initializes a new Fuzzy <see cref="InferenceSystem"/>.
        /// </summary>
        /// 
        /// <param name="database">A fuzzy <see cref="Database"/> containing the system linguistic
        /// variables.</param>
        /// <param name="defuzzifier">A defuzzyfier method used to evaluate the numeric otput
        /// of the system.</param>
        /// <param name="normOperator">A <see cref="INorm"/> operator used to evaluate the norms
        /// in the <see cref="InferenceSystem"/>. For more information of the norm evaluation see <see cref="Rule"/>.</param>
        /// <param name="conormOperator">A <see cref="ICoNorm"/> operator used to evaluate the
        /// conorms in the <see cref="InferenceSystem"/>. For more information of the conorm evaluation see <see cref="Rule"/>.</param>
        /// 
        public InferenceSystem( Database database, IDefuzzifier defuzzifier, INorm normOperator, ICoNorm conormOperator )
        {
            this.database = database;
            this.defuzzifier = defuzzifier;
            this.normOperator = normOperator;
            this.conormOperator = conormOperator;
            this.rulebase = new Rulebase( );
        }

        /// <summary>
        /// Creates a new <see cref="Rule"/> and add it to the <see cref="Rulebase"/> of the 
        /// <see cref="InferenceSystem"/>.
        /// </summary>
        /// 
        /// <param name="name">Name of the <see cref="Rule"/> to create.</param>
        /// <param name="rule">A string representing the fuzzy rule.</param>
        /// 
        /// <returns>The new <see cref="Rule"/> reference. </returns>
        /// 
        public Rule NewRule( string name, string rule )
        {
            Rule r = new Rule( database, name, rule, normOperator, conormOperator );
            this.rulebase.AddRule( r );
            return r;
        }

        /// <summary>
        /// Sets a numerical input for one of the linguistic variables of the <see cref="Database"/>. 
        /// </summary>
        /// 
        /// <param name="variableName">Name of the <see cref="LinguisticVariable"/>.</param>
        /// <param name="value">Numeric value to be used as input.</param>
        /// 
        /// <exception cref="KeyNotFoundException">The variable indicated in <paramref name="variableName"/>
        /// was not found in the database.</exception>
        /// 
        public void SetInput( string variableName, float value )
        {
            this.database.GetVariable( variableName ).NumericInput = value;
        }

        /// <summary>
        /// Gets one of the <see cref="LinguisticVariable"/> of the <see cref="Database"/>. 
        /// </summary>
        /// 
        /// <param name="variableName">Name of the <see cref="LinguisticVariable"/> to get.</param>
        /// 
        /// <exception cref="KeyNotFoundException">The variable indicated in <paramref name="variableName"/>
        /// was not found in the database.</exception>
        /// 
        public LinguisticVariable GetLinguisticVariable( string variableName )
        {
           return this.database.GetVariable( variableName );
        }

        /// <summary>
        /// Gets one of the Rules of the <see cref="Rulebase"/>. 
        /// </summary>
        /// 
        /// <param name="ruleName">Name of the <see cref="Rule"/> to get.</param>
        /// 
        /// <exception cref="KeyNotFoundException">The rule indicated in <paramref name="ruleName"/>
        /// was not found in the rulebase.</exception>
        /// 
        public Rule GetRule( string ruleName )
        {
            return this.rulebase.GetRule ( ruleName );
        }

        /// <summary>
        /// Executes the fuzzy inference, obtaining a numerical output for a choosen output
        /// linguistic variable. 
        /// </summary>
        /// 
        /// <param name="variableName">Name of the <see cref="LinguisticVariable"/> to evaluate.</param>
        /// 
        /// <returns>The numerical output of the Fuzzy Inference System for the choosen variable.</returns>
        /// 
        /// <exception cref="KeyNotFoundException">The variable indicated was not found in the database.</exception>
        /// 
        public float Evaluate( string variableName )
        {
            // call the defuzzification on fuzzy output 
            FuzzyOutput fuzzyOutput = ExecuteInference( variableName );
            float res = defuzzifier.Defuzzify( fuzzyOutput, normOperator );
            return res;
        }

        /// <summary>
        /// Executes the fuzzy inference, obtaining the <see cref="FuzzyOutput"/> of the system for the required
        /// <see cref="LinguisticVariable"/>. 
        /// </summary>
        /// 
        /// <param name="variableName">Name of the <see cref="LinguisticVariable"/> to evaluate.</param>
        /// 
        /// <returns>A <see cref="FuzzyOutput"/> containing the fuzzy output of the system for the
        /// <see cref="LinguisticVariable"/> specified in <paramref name="variableName"/>.</returns>
        /// 
        /// <exception cref="KeyNotFoundException">The variable indicated was not found in the database.</exception>
        /// 
        public FuzzyOutput ExecuteInference( string variableName )
        {
            // gets the variable
            LinguisticVariable lingVar = database.GetVariable( variableName );

            // object to store the fuzzy output
            FuzzyOutput fuzzyOutput = new FuzzyOutput( lingVar );

            // select only rules with the variable as output
            Rule[] rules = rulebase.GetRules( );
            foreach ( Rule r in rules )
            {
                if ( r.Output.Variable.Name == variableName )
                {
                    string labelName = r.Output.Label.Name;
                    float firingStrength = r.EvaluateFiringStrength( );
                    if ( firingStrength > 0 )
                        fuzzyOutput.AddOutput( labelName, firingStrength );
                }
            }

            // returns the fuzzy output obtained
            return fuzzyOutput;
        }
    }
}
