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
    /// The class represents a linguistic variable.
    /// </summary>
    /// 
    /// <remarks><para>Linguistic variables are variables that store linguistic values (labels). Fuzzy Inference Systems (FIS)
    /// use a set of linguistic variables, called the FIS database, to execute fuzzy computation (computing with words). A linguistic
    /// variable has a name and is composed by a set of <see cref="FuzzySet"/> called its linguistic labels. When declaring fuzzy 
    /// statements in a FIS, a linguistic variable can be only assigned or compared to one of its labels. </para>
    /// 
    /// <para>Let us consider, for example, a linguistic variable <b>temperature</b>. In a given application, temperature can be 
    /// cold, cool, warm or hot. Those will be the variable's linguistic labels, each one a fuzzy set with its own membership 
    /// function. Ideally, the labels will represent concepts related to the variable's meaning. Futhermore, fuzzy statements like
    /// "temperature is warm" or "temperature is not cold" can be used to build a Fuzzy Inference Systems. 
    /// </para>
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
    /// // showing the shape of the linguistic variable - the shape of its labels memberships from start to end
    /// Console.WriteLine( "Cold; Cool; Warm; Hot" );
    /// for ( float x = 0; x &lt; 80; x += 0.2 )
    /// {
    ///     float y1 = lvTemperature.GetLabelMembership( "Cold", x );
    ///     float y2 = lvTemperature.GetLabelMembership( "Cool", x );
    ///     float y3 = lvTemperature.GetLabelMembership( "Warm", x );
    ///     float y4 = lvTemperature.GetLabelMembership( "Hot" , x );
    ///
    ///     Console.WriteLine( String.Format( "{0:N}; {1:N}; {2:N}; {3:N}", y1, y2, y3, y4 ) );
    /// }
    /// </code>    
    /// </remarks>
    /// 
    public class LinguisticVariable
    {
        // name of the linguistic variable
        private string name;
        // right limit within the lingusitic variable works
        private float start;
        // left limit within the lingusitic variable works
        private float end;
        // the linguistic labels of the linguistic variable
        private Dictionary<string, FuzzySet> labels;
        // the numeric input of this variable
        private float numericInput;

        /// <summary>
        /// Numerical value of the input of this linguistic variable.
        /// </summary>
        public float NumericInput
        {
            get { return numericInput; }
            set { numericInput = value; }
        }

        /// <summary>
        /// Name of the linguistic variable.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Left limit of the valid variable range.
        /// </summary>
        public float Start
        {
            get { return start; }
        }

        /// <summary>
        /// Right limit of the valid variable range.
        /// </summary>
        public float End
        {
            get { return end; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinguisticVariable"/> class.
        /// </summary>
        /// 
        /// <param name="name">Name of the linguistic variable.</param>
        /// 
        /// <param name="start">Left limit of the valid variable range.</param>
        /// 
        /// <param name="end">Right limit of the valid variable range.</param>
        /// 
        public LinguisticVariable( string name, float start, float end )
        {
            this.name  = name;
            this.start = start;
            this.end   = end;

            // instance of the labels list - usually a linguistic variable has no more than 10 labels
            this.labels = new Dictionary<string, FuzzySet>( 10 );
        }

        /// <summary>
        /// Adds a linguistic label to the variable. 
        /// </summary>
        /// 
        /// <param name="label">A <see cref="FuzzySet"/> that will be a linguistic label of the linguistic variable.</param>
        /// 
        /// <remarks>Linguistic labels are fuzzy sets (<see cref="FuzzySet"/>). Each
        /// label of the variable must have a unique name. The range of the label 
        /// (left and right limits) cannot be greater than 
        /// the linguistic variable range (start/end).</remarks>
        /// 
        /// <exception cref="NullReferenceException">The fuzzy set was not initialized.</exception>
        /// <exception cref="ArgumentException">The linguistic label name already exists in the linguistic variable.</exception>
        /// <exception cref="ArgumentException">The left limit of the fuzzy set can not be lower than the linguistic variable's starting point.</exception>
        /// <exception cref="ArgumentException">"The right limit of the fuzzy set can not be greater than the linguistic variable's ending point."</exception>
        /// 
        public void AddLabel( FuzzySet label )
        {
            // checking for existing name
            if ( this.labels.ContainsKey( label.Name ) )
                throw new ArgumentException( "The linguistic label name already exists in the linguistic variable." );

            // checking ranges
            if ( label.LeftLimit < this.start )
                throw new ArgumentException( "The left limit of the fuzzy set can not be lower than the linguistic variable's starting point." );
            if ( label.RightLimit > this.end )
                throw new ArgumentException( "The right limit of the fuzzy set can not be greater than the linguistic variable's ending point." );

            // adding label
            this.labels.Add( label.Name, label );
        }

        /// <summary>
        /// Removes all the linguistic labels of the linguistic variable. 
        /// </summary>
        /// 
        public void ClearLabels( )
        {
            this.labels.Clear( );
        }

        /// <summary>
        /// Returns an existing label from the linguistic variable.
        /// </summary>
        /// 
        /// <param name="labelName">Name of the label to retrieve.</param>
        /// 
        /// <returns>Reference to named label (<see cref="FuzzySet"/>).</returns>
        /// 
        /// <exception cref="KeyNotFoundException">The label indicated was not found in the linguistic variable.</exception>
        /// 
        public FuzzySet GetLabel( string labelName )
        {
            return labels[labelName];
        }

        /// <summary>
        /// Calculate the membership of a given value to a given label. Used to evaluate linguistics clauses like 
        /// "X IS A", where X is a value and A is a linguistic label.
        /// </summary>
        /// 
        /// <param name="labelName">Label (fuzzy set) to evaluate value's membership.</param>
        /// <param name="value">Value which label's membership will to be calculated.</param>
        /// 
        /// <returns>Degree of membership [0..1] of the value to the label (fuzzy set).</returns>
        /// 
        /// <exception cref="KeyNotFoundException">The label indicated in labelName was not found in the linguistic variable.</exception>
        /// 
        public float GetLabelMembership( string labelName, float value )
        {
            FuzzySet fs = labels[labelName];
            return fs.GetMembership( value );
        }
    }
}
