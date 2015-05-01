// AForge Fuzzy Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2008-2009
// andrew.kirillov@aforgenet.com
//
// Copyright © Fabio L. Caversan, 2008-2009
// fabio.caversan@gmail.com
//

namespace AForge.Fuzzy
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The class represents a fuzzy database, a set of linguistic variables used in a Fuzzy
    /// Inference System.
    /// </summary>
    /// 
    public class Database
    {
        // the linguistic variables repository 
        private Dictionary<string, LinguisticVariable> variables;

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// 
        public Database(  )
        {
            // instance of the variables list 
            this.variables = new Dictionary<string, LinguisticVariable>( 10 );
        }

        /// <summary>
        /// Adds a linguistic variable to the database. 
        /// </summary>
        /// 
        /// <param name="variable">A linguistic variable to add.</param>
        /// 
        /// <exception cref="NullReferenceException">The linguistic variable was not initialized.</exception>
        /// <exception cref="ArgumentException">The linguistic variable name already exists in the database.</exception>
        /// 
        public void AddVariable( LinguisticVariable variable )
        {
            // checking for existing name
            if ( this.variables.ContainsKey( variable.Name ) )
                throw new ArgumentException( "The linguistic variable name already exists in the database." );
            
            // adding label
            this.variables.Add( variable.Name, variable );
        }

        /// <summary>
        /// Removes all the linguistic variables of the database. 
        /// </summary>
        /// 
        public void ClearVariables( )
        {
            this.variables.Clear( );
        }

        /// <summary>
        /// Returns an existing linguistic variable from the database.
        /// </summary>
        /// 
        /// <param name="variableName">Name of the linguistic variable to retrieve.</param>
        /// 
        /// <returns>Reference to named <see cref="LinguisticVariable"/>.</returns>
        /// 
        /// <exception cref="KeyNotFoundException">The variable indicated was not found in the database.</exception>
        /// 
        public LinguisticVariable GetVariable( string variableName )
        {
            return variables [variableName];
        }
    }
}
