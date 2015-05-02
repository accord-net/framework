// AForge Genetic Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

namespace AForge.Genetic
{
    using System;
    using AForge;

    /// <summary>
    /// Genetic programming gene, which represents arithmetic functions, common mathematical functions
    /// and arguments.
    /// </summary>
    ///
    /// <remarks><para>Extended gene function may represent arithmetic functions (+, -, *, /),
    /// some common mathematical functions (sin, cos, ln, exp, sqrt) or an argument to functions.
    /// This class is used by Genetic Programming (or Gene Expression Programming)
    /// chromosomes to build arbitrary expressions with help of genetic operators.</para>
    /// </remarks>
    ///
    public class ExtendedGeneFunction : IGPGene
    {
        /// <summary>
        /// Enumeration of supported functions.
        /// </summary>
        protected enum Functions
        {
            /// <summary>
            /// Addition operator.
            /// </summary>
            Add,
            /// <summary>
            /// Suntraction operator.
            /// </summary>
            Subtract,
            /// <summary>
            /// Multiplication operator.
            /// </summary>
            Multiply,
            /// <summary>
            /// Division operator.
            /// </summary>
            Divide,
            /// <summary>
            /// Sine function.
            /// </summary>
            Sin,
            /// <summary>
            /// Cosine function.
            /// </summary>
            Cos,
            /// <summary>
            /// Natural logarithm function.
            /// </summary>
            Ln,
            /// <summary>
            /// Exponent function.
            /// </summary>
            Exp,
            /// <summary>
            /// Square root function.
            /// </summary>
            Sqrt
        }

        /// <summary>
        /// Number of different functions supported by the class.
        /// </summary>
        protected const int FunctionsCount = 9;

        // gene type
        private GPGeneType	type;
        // total amount of variables in the task which is supposed to be solved
        private int			variablesCount;
        //
        private int			val;
        // arguments count
        private int			argumentsCount = 0;

        /// <summary>
        /// Random number generator for chromosoms generation.
        /// </summary>
        protected static ThreadSafeRandom rand = new ThreadSafeRandom( );

        /// <summary>
        /// Gene type.
        /// </summary>
        /// 
        /// <remarks><para>The property represents type of a gene - function, argument, etc.</para>
        /// </remarks>
        /// 
        public GPGeneType GeneType
        {
            get { return type; }
        }

        /// <summary>
        /// Arguments count.
        /// </summary>
        /// 
        /// <remarks><para>Arguments count of a particular function gene.</para></remarks>
        /// 
        public int ArgumentsCount
        {
            get { return argumentsCount; }
        }

        /// <summary>
        /// Maximum arguments count.
        /// </summary>
        /// 
        /// <remarks><para>Maximum arguments count of a function gene supported by the class.
        /// The property may be used by chromosomes' classes to allocate correctly memory for
        /// functions' arguments, for example.</para></remarks>
        /// 
        public int MaxArgumentsCount
        {
            get { return 2; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedGeneFunction"/> class.
        /// </summary>
        /// 
        /// <param name="variablesCount">Total amount of variables in the task which is supposed
        /// to be solved.</param>
        /// 
        /// <remarks><para>The constructor creates randomly initialized gene with random type
        /// and value by calling <see cref="Generate( )"/> method.</para></remarks>
        /// 
        public ExtendedGeneFunction( int variablesCount ) : this( variablesCount, true ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedGeneFunction"/> class.
        /// </summary>
        /// 
        /// <param name="variablesCount">Total amount of variables in the task which is supposed
        /// to be solved.</param>
        /// <param name="type">Gene type to set.</param>
        /// 
        /// <remarks><para>The constructor creates randomly initialized gene with random
        /// value and preset gene type.</para></remarks>
        /// 
        public ExtendedGeneFunction( int variablesCount, GPGeneType type )
        {
            this.variablesCount = variablesCount;
            // generate the gene value
            Generate( type );
        }

        // Private constructor
        private ExtendedGeneFunction( int variablesCount, bool random )
        {
            this.variablesCount = variablesCount;
            // generate the gene value
            if ( random )
                Generate( );
        }

        /// <summary>
        /// Get string representation of the gene.
        /// </summary>
        /// 
        /// <returns>Returns string representation of the gene.</returns>
        ///
        public override string ToString( )
        {
            if ( type == GPGeneType.Function )
            {
                // get function string representation
                switch ( (Functions) val )
                {
                    case Functions.Add:			// addition
                        return "+";

                    case Functions.Subtract:	// subtraction
                        return "-";

                    case Functions.Multiply:	// multiplication
                        return "*";

                    case Functions.Divide:		// division
                        return "/";

                    case Functions.Sin:			// sine
                        return "sin";

                    case Functions.Cos:			// cosine
                        return "cos";

                    case Functions.Ln:			// natural logarithm
                        return "ln";

                    case Functions.Exp:			// exponent
                        return "exp";

                    case Functions.Sqrt:		// square root
                        return "sqrt";
                }
            }

            // get argument string representation
            return string.Format( "${0}", val );
        }

        /// <summary>
        /// Clone the gene.
        /// </summary>
        /// 
        /// <remarks><para>The method clones the chromosome returning the exact copy of it.</para></remarks>
        /// 
        public IGPGene Clone( )
        {
            // create new gene ...
            ExtendedGeneFunction clone = new ExtendedGeneFunction( variablesCount, false );
            // ... with the same type and value
            clone.type = type;
            clone.val  = val;
            clone.argumentsCount = argumentsCount;

            return clone;
        }

        /// <summary>
        /// Randomize gene with random type and value.
        /// </summary>
        /// 
        /// <remarks><para>The method randomizes the gene, setting its type and value randomly.</para></remarks>
        /// 
        public void Generate( )
        {
            // give more chance to function
            Generate( ( rand.Next( 4 ) == 3 ) ? GPGeneType.Argument : GPGeneType.Function );
        }

        /// <summary>
        /// Randomize gene with random value.
        /// </summary>
        /// 
        /// <param name="type">Gene type to set.</param>
        /// 
        /// <remarks><para>The method randomizes a gene, setting its value randomly, but type
        /// is set to the specified one.</para></remarks>
        ///
        public void Generate( GPGeneType type )
        {
            // gene type
            this.type = type;
            // gene value
            val = rand.Next( ( type == GPGeneType.Function ) ? FunctionsCount : variablesCount );
            // arguments count
            argumentsCount = ( type == GPGeneType.Argument ) ? 0 :
                ( val <= (int) Functions.Divide ) ? 2 : 1;
        }

        /// <summary>
        /// Creates new gene with random type and value.
        /// </summary>
        /// 
        /// <remarks><para>The method creates new randomly initialized gene .
        /// The method is useful as factory method for those classes, which work with gene's interface,
        /// but not with particular gene class.</para>
        /// </remarks>
        ///
        public IGPGene CreateNew( )
        {
            return new ExtendedGeneFunction( variablesCount );
        }

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
        public IGPGene CreateNew( GPGeneType type )
        {
            return new ExtendedGeneFunction( variablesCount, type );
        }
    }
}
