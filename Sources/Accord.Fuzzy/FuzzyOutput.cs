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
    /// The class represents the output of a Fuzzy Inference System. 
    /// </summary>
    /// 
    /// <remarks><para>The class keeps set of rule's output - pairs with the output fuzzy label
    /// and the rule's firing strength.
    /// </para>
    /// 
    /// 
    /// <para>Sample usage:</para>
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
    /// // going straight
    /// IS.NewRule( "Rule 1", "IF FrontalDistance IS Far THEN Angle IS Zero" );
    /// // turning left
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
    ///     FuzzyOutput fuzzyOutput = IS.ExecuteInference ( "Angle" );
    /// 
    ///     // showing the fuzzy output
    ///     foreach ( FuzzyOutput.OutputConstraint oc in fuzzyOutput.OutputList )
    ///     {
    ///         Console.WriteLine( oc.Label + " - " + oc.FiringStrength.ToString( ) );
    ///     }
    /// }
    /// catch ( Exception )
    /// {
    ///    ...
    /// }
    /// </code>  
    /// </remarks>
    /// 
    public class FuzzyOutput
    {
        /// <summary>
        /// Inner class to store the pair fuzzy label / firing strength of 
        /// a fuzzy output.
        /// </summary>
        public class OutputConstraint
        {
            // The label of a fuzzy output
            private string label;
            // The firing strength of a fuzzy rule, to be applied to the label
            private float firingStrength;

            /// <summary>
            /// Initializes a new instance of the <see cref="OutputConstraint"/> class.
            /// </summary>
            /// 
            /// <param name="label">A string representing the output label of a <see cref="Rule"/>.</param>
            /// <param name="firingStrength">The firing strength of a <see cref="Rule"/>, to be applied to its output label.</param>
            /// 
            internal OutputConstraint( string label, float firingStrength )
            {
                this.label          = label;
                this.firingStrength = firingStrength;
            }
            
            /// <summary>
            /// The <see cref="FuzzySet"/> representing the output label of a <see cref="Rule"/>.
            /// </summary>
            /// 
            public string Label
            {
                get { return label; }
            }
            
            /// <summary>
            /// The firing strength of a <see cref="Rule"/>, to be applied to its output label.
            /// </summary>
            /// 
            public float FiringStrength
            {
                get { return firingStrength; }
            }

        }

        // the linguistic variables repository 
        private List<OutputConstraint> outputList;

        // the output linguistic variable 
        private LinguisticVariable outputVar;

        /// <summary>
        /// A list with <see cref="OutputConstraint"/> of a Fuzzy Inference System's output.
        /// </summary>
        /// 
        public List<OutputConstraint> OutputList
        {
            get
            {
                return outputList;
            }
        }

        /// <summary>
        /// Gets the <see cref="LinguisticVariable"/> acting as a Fuzzy Inference System Output.
        /// </summary>
        /// 
        public LinguisticVariable OutputVariable
        {
            get { return outputVar; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuzzyOutput"/> class.
        /// </summary>
        /// 
        /// <param name="outputVar">A <see cref="LinguisticVariable"/> representing a Fuzzy Inference System's output.</param>
        /// 
        internal FuzzyOutput( LinguisticVariable outputVar )
        {
            // instance of the constraints list 
            this.outputList = new List<OutputConstraint>( 20 );

            // output linguistic variable
            this.outputVar  = outputVar; 
        }

        /// <summary>
        /// Adds an output to the Fuzzy Output. 
        /// </summary>
        /// 
        /// <param name="labelName">The name of a label representing a fuzzy rule's output.</param>
        /// <param name="firingStrength">The firing strength [0..1] of a fuzzy rule.</param>
        /// 
        /// <exception cref="KeyNotFoundException">The label indicated was not found in the linguistic variable.</exception>
        /// 
        internal void AddOutput( string labelName, float firingStrength )
        {
            // check if the label exists in the linguistic variable
            this.outputVar.GetLabel( labelName );

            // adding label
            this.outputList.Add( new OutputConstraint( labelName, firingStrength ) );
        }

        /// <summary>
        /// Removes all the linguistic variables of the database. 
        /// </summary>
        /// 
        internal void ClearOutput( )
        {
            this.outputList.Clear( );
        }
    }
}
