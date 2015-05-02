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
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using AForge;

    /// <summary>
    /// The chromosome represents a Gene Expression, which is used for
    /// different tasks of Genetic Expression Programming (GEP).
    /// </summary>
    /// 
    /// <remarks><para>This type of chromosome represents combination of ideas taken from
    /// Genetic Algorithms (GA), where chromosomes are linear structures of fixed length, and
    /// Genetic Programming (GP), where chromosomes are expression trees. The GEP chromosome
    /// is also a fixed length linear structure, but with some additional features which
    /// make it possible to generate valid expression tree from any GEP chromosome.</para>
    /// 
    /// <para>The theory of Gene Expression Programming is well described in the next paper:
    /// <b>Ferreira, C., 2001. Gene Expression Programming: A New Adaptive Algorithm for Solving
    /// Problems. Complex Systems, Vol. 13, issue 2: 87-129</b>. A copy of the paper may be
    /// obtained on the
    /// <a href="http://www.gene-expression-programming.com/">gene expression programming</a> web site.</para>
    /// </remarks>
    /// 
    public class GEPChromosome : ChromosomeBase
    {
        /// <summary>
        /// Length of GEP chromosome's head.
        /// </summary>
        /// 
        /// <remarks><para>GEP chromosome's head is a part of chromosome, which may contain both
        /// functions' and arguments' nodes. The rest of chromosome (tail) may contain only arguments' nodes.
        /// </para></remarks>
        /// 
        protected int headLength;

        /// <summary>
        /// GEP chromosome's length.
        /// </summary>
        /// 
        /// <remarks><para><note>The variable keeps chromosome's length, but not expression length represented by the
        /// chromosome.</note></para></remarks>
        /// 
        protected int length;

        /// <summary>
        /// Array of chromosome's genes.
        /// </summary>
        protected IGPGene[] genes;

        /// <summary>
        /// Random generator used for chromosoms' generation.
        /// </summary>
        protected static ThreadSafeRandom rand = new ThreadSafeRandom( );

        /// <summary>
        /// Initializes a new instance of the <see cref="GEPChromosome"/> class.
        /// </summary>
        /// 
        /// <param name="ancestor">A gene, which is used as generator for the genetic tree.</param>
        /// <param name="headLength">Length of GEP chromosome's head (see <see cref="headLength"/>).</param>
        /// 
        /// <remarks><para>This constructor creates a randomly generated GEP chromosome,
        /// which has all genes of the same type and properties as the specified <paramref name="ancestor"/>.
        /// </para></remarks>
        ///
        public GEPChromosome( IGPGene ancestor, int headLength )
        {
            // store head length
            this.headLength = headLength;
            // calculate chromosome's length
            length = headLength + headLength * ( ancestor.MaxArgumentsCount - 1 ) + 1;
            // allocate genes array
            genes = new IGPGene[length];
            // save ancestor as a temporary head
            genes[0] = ancestor;
            // generate the chromosome
            Generate( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GEPChromosome"/> class.
        /// </summary>
        /// 
        /// <param name="source">Source GEP chromosome to clone from.</param>
        /// 
        protected GEPChromosome( GEPChromosome source )
        {
            headLength = source.headLength;
            length = source.length;
            fitness = source.fitness;
            // allocate genes array
            genes = new IGPGene[length];
            // copy genes
            for ( int i = 0; i < length; i++ )
                genes[i] = source.genes[i].Clone( );
        }

        /// <summary>
        /// Get string representation of the chromosome by providing its expression in
        /// reverse polish notation (postfix notation).
        /// </summary>
        /// 
        /// <returns>Returns string representation of the expression represented by the GEP
        /// chromosome.</returns>
        /// 
        public override string ToString( )
        {
            // return string representation of the chromosomes tree
            return GetTree( ).ToString( );
        }

        /// <summary>
        /// Get string representation of the chromosome. 
        /// </summary>
        /// 
        /// <returns>Returns the chromosome in native linear representation.</returns>
        /// 
        /// <remarks><para><note>The method is used for debugging mostly.</note></para></remarks>
        /// 
        public string ToStringNative( )
        {
            StringBuilder sb = new StringBuilder( );

            foreach ( IGPGene gene in genes )
            {
                sb.Append( gene.ToString( ) );
                sb.Append( " " );
            }
            return sb.ToString( );
        }

        /// <summary>
        /// Generate random chromosome value.
        /// </summary>
        /// 
        /// <remarks><para>Regenerates chromosome's value using random number generator.</para>
        /// </remarks>
        ///
        public override void Generate( )
        {
            // randomize the root
            genes[0].Generate( );
            // generate the rest of the head
            for ( int i = 1; i < headLength; i++ )
            {
                genes[i] = genes[0].CreateNew( );
            }
            // generate the tail
            for ( int i = headLength; i < length; i++ )
            {
                genes[i] = genes[0].CreateNew( GPGeneType.Argument );
            }
        }

        /// <summary>
        /// Get tree representation of the chromosome.
        /// </summary>
        /// 
        /// <returns>Returns expression's tree represented by the chromosome.</returns>
        /// 
        /// <remarks><para>The method builds expression's tree for the native linear representation
        /// of the GEP chromosome.</para></remarks>
        /// 
        protected GPTreeNode GetTree( )
        {
            // function node queue. the queue contains function node,
            // which requires children. when a function node receives
            // all children, it will be removed from the queue
            Queue functionNodes = new Queue( );

            // create root node
            GPTreeNode root = new GPTreeNode( genes[0] );

            // check children amount of the root node
            if ( root.Gene.ArgumentsCount != 0 )
            {
                root.Children = new List<GPTreeNode>( );
                // place the root to the queue
                functionNodes.Enqueue( root );

                // go through genes
                for ( int i = 1; i < length; i++ )
                {
                    // create new node
                    GPTreeNode node = new GPTreeNode( genes[i] );

                    // if next gene represents function, place it to the queue
                    if ( genes[i].GeneType == GPGeneType.Function )
                    {
                        node.Children = new List<GPTreeNode>( );
                        functionNodes.Enqueue( node );
                    }

                    // get function node from the top of the queue
                    GPTreeNode parent = (GPTreeNode) functionNodes.Peek( );

                    // add new node to children of the parent node
                    parent.Children.Add( node );

                    // remove the parent node from the queue, if it is
                    // already complete
                    if ( parent.Children.Count == parent.Gene.ArgumentsCount )
                    {
                        functionNodes.Dequeue( );

                        // check the queue if it is empty
                        if ( functionNodes.Count == 0 )
                            break;
                    }
                }
            }
            // return formed tree
            return root;
        }

        /// <summary>
        /// Create new random chromosome with same parameters (factory method).
        /// </summary>
        /// 
        /// <remarks><para>The method creates new chromosome of the same type, but randomly
        /// initialized. The method is useful as factory method for those classes, which work
        /// with chromosome's interface, but not with particular chromosome type.</para></remarks>
        /// 
        public override IChromosome CreateNew( )
        {
            return new GEPChromosome( genes[0].Clone( ), headLength );
        }

        /// <summary>
        /// Clone the chromosome.
        /// </summary>
        /// 
        /// <returns>Return's clone of the chromosome.</returns>
        /// 
        /// <remarks><para>The method clones the chromosome returning the exact copy of it.</para>
        /// </remarks>
        ///
        public override IChromosome Clone( )
        {
            return new GEPChromosome( this );
        }

        /// <summary>
        /// Mutation operator.
        /// </summary>
        /// 
        /// <remarks><para>The method performs chromosome's mutation by calling on of the methods
        /// randomly: <see cref="MutateGene"/>, <see cref="TransposeIS"/>, <see cref="TransposeRoot"/>.
        /// </para></remarks>
        /// 
        public override void Mutate( )
        {
            // randomly choose mutation method
            switch ( rand.Next( 3 ) )
            {
                case 0:		// ordinary gene mutation
                    MutateGene( );
                    break;

                case 1:		// IS transposition
                    TransposeIS( );
                    break;

                case 2:		// root transposition
                    TransposeRoot( );
                    break;
            }
        }

        /// <summary>
        /// Usual gene mutation.
        /// </summary>
        /// 
        /// <remarks><para>The method performs usual gene mutation by randomly changing randomly selected
        /// gene.</para></remarks>
        /// 
        protected void MutateGene( )
        {
            // select random point of mutation
            int mutationPoint = rand.Next( length );

            if ( mutationPoint < headLength )
            {
                // genes from head can be randomized freely (type may change)
                genes[mutationPoint].Generate( );
            }
            else
            {
                // genes from tail cannot change their type - they
                // should be always arguments
                genes[mutationPoint].Generate( GPGeneType.Argument );
            }
        }

        /// <summary>
        /// Transposition of IS elements (insertion sequence).
        /// </summary>
        /// 
        /// <remarks><para>The method performs transposition of IS elements by copying randomly selected region
        /// of genes into chromosome's head (into randomly selected position). First gene of the chromosome's head 
        /// is not affected - can not be selected as target point.</para></remarks>
        /// 
        protected void TransposeIS( )
        {
            // select source point (may be any point of the chromosome)
            int sourcePoint = rand.Next( length );
            // calculate maxim source length
            int maxSourceLength = length - sourcePoint;
            // select tartget insertion point in the head (except first position)
            int targetPoint = rand.Next( headLength - 1 ) + 1;
            // calculate maximum target length
            int maxTargetLength = headLength - targetPoint;
            // select randomly transposon length
            int transposonLength = rand.Next( Math.Min( maxTargetLength, maxSourceLength ) ) + 1;
            // genes copy
            IGPGene[] genesCopy = new IGPGene[transposonLength];

            // copy genes from source point
            for ( int i = sourcePoint, j = 0; j < transposonLength; i++, j++ )
            {
                genesCopy[j] = genes[i].Clone( );
            }

            // copy genes to target point
            for ( int i = targetPoint, j = 0; j < transposonLength; i++, j++ )
            {
                genes[i] = genesCopy[j];
            }
        }

        /// <summary>
        /// Root transposition.
        /// </summary>
        ///
        /// <remarks><para>The method performs root transposition of the GEP chromosome - inserting
        /// new root of the chromosome and shifting existing one. The method first of all randomly selects
        /// a function gene in chromosome's head - starting point of the sequence to put into chromosome's
        /// head. Then it randomly selects the length of the sequence making sure that the entire sequence is
        /// located within head. Once the starting point and the length of the sequence are known, it is copied
        /// into chromosome's head shifting existing elements in the head.</para>
        /// </remarks>
        ///
        protected void TransposeRoot( )
        {
            // select source point (may be any point in the head of the chromosome)
            int sourcePoint = rand.Next( headLength );
            // scan downsrteam the head searching for function gene
            while ( ( genes[sourcePoint].GeneType != GPGeneType.Function ) && ( sourcePoint < headLength ) )
            {
                sourcePoint++;
            }
            // return (do nothing) if function gene was not found
            if ( sourcePoint == headLength )
                return;

            // calculate maxim source length
            int maxSourceLength = headLength - sourcePoint;
            // select randomly transposon length
            int transposonLength = rand.Next( maxSourceLength ) + 1;
            // genes copy
            IGPGene[] genesCopy = new IGPGene[transposonLength];

            // copy genes from source point
            for ( int i = sourcePoint, j = 0; j < transposonLength; i++, j++ )
            {
                genesCopy[j] = genes[i].Clone( );
            }

            // shift the head
            for ( int i = headLength - 1; i >= transposonLength; i-- )
            {
                genes[i] = genes[i - transposonLength];
            }

            // put new root
            for ( int i = 0; i < transposonLength; i++ )
            {
                genes[i] = genesCopy[i];
            }
        }

        /// <summary>
        /// Crossover operator.
        /// </summary>
        /// 
        /// <param name="pair">Pair chromosome to crossover with.</param>
        /// 
        /// <remarks><para>The method performs one-point or two-point crossover selecting
        /// them randomly with equal probability.</para></remarks>
        /// 
        public override void Crossover( IChromosome pair )
        {
            GEPChromosome p = (GEPChromosome) pair;

            // check for correct chromosome
            if ( p != null )
            {
                // choose recombination method
                if ( rand.Next( 2 ) == 0 )
                {
                    RecombinationOnePoint( p );
                }
                else
                {
                    RecombinationTwoPoint( p );
                }
            }
        }

        /// <summary>
        /// One-point recombination (crossover).
        /// </summary>
        /// 
        /// <param name="pair">Pair chromosome to crossover with.</param>
        /// 
        public void RecombinationOnePoint( GEPChromosome pair )
        {
            // check for correct pair
            if ( ( pair.length == length ) )
            {
                // crossover point
                int crossOverPoint = rand.Next( length - 1 ) + 1;
                // length of chromosome to be crossed
                int crossOverLength = length - crossOverPoint;

                // swap parts of chromosomes
                Recombine( genes, pair.genes, crossOverPoint, crossOverLength );
            }
        }

        /// <summary>
        /// Two point recombination (crossover).
        /// </summary>
        /// 
        /// <param name="pair">Pair chromosome to crossover with.</param>
        /// 
        public void RecombinationTwoPoint( GEPChromosome pair )
        {
            // check for correct pair
            if ( ( pair.length == length ) )
            {
                // crossover point
                int crossOverPoint = rand.Next( length - 1 ) + 1;
                // length of chromosome to be crossed
                int crossOverLength = length - crossOverPoint;

                // if crossover length already equals to 1, then it becomes
                // usual one point crossover. otherwise crossover length
                // also randomly chosen
                if ( crossOverLength != 1 )
                {
                    crossOverLength = rand.Next( crossOverLength - 1 ) + 1;
                }

                // swap parts of chromosomes
                Recombine( genes, pair.genes, crossOverPoint, crossOverLength );
            }
        }

        /// <summary>
        /// Swap parts of two chromosomes.
        /// </summary>
        /// 
        /// <param name="src1">First chromosome participating in genes' interchange.</param>
        /// <param name="src2">Second chromosome participating in genes' interchange.</param>
        /// <param name="point">Index of the first gene in the interchange sequence.</param>
        /// <param name="length">Length of the interchange sequence - number of genes
        /// to interchange.</param>
        ///
        /// <remarks><para>The method performs interchanging of genes between two chromosomes
        /// starting from the <paramref name="point"/> position.</para></remarks>
        ///
        protected static void Recombine( IGPGene[] src1, IGPGene[] src2, int point, int length )
        {
            // temporary array
            IGPGene[] temp = new IGPGene[length];

            // copy part of first chromosome to temp
            Array.Copy( src1, point, temp, 0, length );
            // copy part of second chromosome to the first
            Array.Copy( src2, point, src1, point, length );
            // copy temp to the second
            Array.Copy( temp, 0, src2, point, length );
        }
    }
}
