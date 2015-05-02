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
    /// The class represents a fuzzy rulebase, a set of fuzzy rules used in a Fuzzy Inference System.
    /// </summary>
    /// 
    public class Rulebase
    {
        // the fuzzy rules repository 
        private Dictionary<string, Rule> rules;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rulebase"/> class.
        /// </summary>
        /// 
        public Rulebase(  )
        {
            // instance of the rules list 
            this.rules = new Dictionary<string, Rule>( 20 );
        }

        /// <summary>
        /// Adds a fuzzy rule to the database. 
        /// </summary>
        /// 
        /// <param name="rule">A fuzzy <see cref="Rule"/> to add to the database.</param>
        /// 
        /// <exception cref="NullReferenceException">The fuzzy rule was not initialized.</exception>
        /// <exception cref="ArgumentException">The fuzzy rule name already exists in the rulebase.</exception>
        /// 
        public void AddRule( Rule rule )
        {
            // checking for existing name
            if ( this.rules.ContainsKey( rule.Name ) )
                throw new ArgumentException( "The fuzzy rule name already exists in the rulebase." );
            
            // adding rule
            this.rules.Add( rule.Name, rule );
        }

        /// <summary>
        /// Removes all the fuzzy rules of the database. 
        /// </summary>
        /// 
        public void ClearRules( )
        {
            this.rules.Clear( );
        }

        /// <summary>
        /// Returns an existing fuzzy rule from the rulebase.
        /// </summary>
        /// 
        /// <param name="ruleName">Name of the fuzzy <see cref="Rule"/> to retrieve.</param>
        /// 
        /// <returns>Reference to named <see cref="Rule"/>.</returns>
        /// 
        /// <exception cref="KeyNotFoundException">The rule indicated in ruleName was not found in the rulebase.</exception>
        /// 
        public Rule GetRule( string ruleName )
        {
            return rules [ruleName];
        }

        /// <summary>
        /// Gets all the rules of the rulebase.
        /// </summary>
        /// 
        /// <returns>An array with all the rulebase rules.</returns>
        /// 
        public Rule[] GetRules( )
        {
            Rule[] r = new Rule[rules.Count];

            int i = 0;
            foreach ( KeyValuePair<string, Rule> kvp in rules )
                r[i++] = kvp.Value;

            return r;
        }
    }
}
