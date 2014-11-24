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
    using AForge;

    /// <summary>
    /// Tree chromosome represents a tree of genes, which is is used for
    /// different tasks of Genetic Programming (GP).
    /// </summary>
    /// 
    /// <remarks><para>This type of chromosome represents a tree, where each node
    /// is represented by <see cref="GPTreeNode"/> containing <see cref="IGPGene"/>.
    /// Depending on type of genes used to build the tree, it may represent different
    /// types of expressions aimed to solve different type of tasks. For example, a
    /// particular implementation of <see cref="IGPGene"/> interface may represent
    /// simple algebraic operations and their arguments.
    /// </para>
    /// 
    /// <para>See documentation to <see cref="IGPGene"/> implementations for additional
    /// information about possible Genetic Programming trees.</para>
    /// </remarks>
    /// 
    public class GPTreeChromosome : ChromosomeBase
    {
        // tree root
        private GPTreeNode root = new GPTreeNode( );

        // maximum initial level of the tree
        private static int maxInitialLevel = 3;
        // maximum level of the tree
        private static int maxLevel = 5;

        /// <summary>
        /// Random generator used for chromosoms' generation.
        /// </summary>
        protected static ThreadSafeRandom rand = new ThreadSafeRandom( );

        /// <summary>
        /// Maximum initial level of genetic trees, [1, 25].
        /// </summary>
        /// 
        /// <remarks><para>The property sets maximum possible initial depth of new
        /// genetic programming tree. For example, if it is set to 1, then largest initial
        /// tree may have a root and one level of children.</para>
        /// 
        /// <para>Default value is set to <b>3</b>.</para>
        /// </remarks>
        ///
        public static int MaxInitialLevel
        {
            get { return maxInitialLevel; }
            set { maxInitialLevel = Math.Max( 1, Math.Min( 25, value ) ); }
        }

        /// <summary>
        /// Maximum level of genetic trees, [1, 50].
        /// </summary>
        /// 
        /// <remarks><para>The property sets maximum possible depth of 
        /// genetic programming tree, which may be created with mutation and crossover operators.
        /// This property guarantees that genetic programmin tree will never have
        /// higher depth, than the specified value.</para>
        /// 
        /// <para>Default value is set to <b>5</b>.</para>
        /// </remarks>
        ///
        public static int MaxLevel
        {
            get { return maxLevel; }
            set { maxLevel = Math.Max( 1, Math.Min( 50, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GPTreeChromosome"/> class.
        /// </summary>
        /// 
        /// <param name="ancestor">A gene, which is used as generator for the genetic tree.</param>
        /// 
        /// <remarks><para>This constructor creates a randomly generated genetic tree,
        /// which has all genes of the same type and properties as the specified <paramref name="ancestor"/>.
        /// </para></remarks>
        /// 
        public GPTreeChromosome( IGPGene ancestor )
        {
            // make the ancestor gene to be as temporary root of the tree
            root.Gene = ancestor.Clone( );
            // call tree regeneration function
            Generate( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GPTreeChromosome"/> class.
        /// </summary>
        /// 
        /// <param name="source">Source genetic tree to clone from.</param>
        /// 
        /// <remarks><para>This constructor creates new genetic tree as a copy of the
        /// specified <paramref name="source"/> tree.</para></remarks>
        /// 
        protected GPTreeChromosome( GPTreeChromosome source )
        {
            root = (GPTreeNode) source.root.Clone( );
            fitness = source.fitness;
        }

        /// <summary>
        /// Get string representation of the chromosome by providing its expression in
        /// reverse polish notation (postfix notation).
        /// </summary>
        /// 
        /// <returns>Returns string representation of the genetic tree.</returns>
        /// 
        /// <remarks><para>The method returns string representation of the tree's root node
        /// (see <see cref="GPTreeNode.ToString"/>).</para></remarks>
        ///
        public override string ToString( )
        {
            return root.ToString( );
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
            root.Gene.Generate( );
            // create children
            if ( root.Gene.ArgumentsCount != 0 )
            {
                root.Children = new List<GPTreeNode>( );
                for ( int i = 0; i < root.Gene.ArgumentsCount; i++ )
                {
                    // create new child
                    GPTreeNode child = new GPTreeNode( );
                    Generate( child, rand.Next( maxInitialLevel ) );
                    // add the new child
                    root.Children.Add( child );
                }
            }
        }

        /// <summary>
        /// Generate chromosome's subtree of specified level.
        /// </summary>
        /// 
        /// <param name="node">Sub tree's node to generate.</param>
        /// <param name="level">Sub tree's level to generate.</param>
        /// 
        protected void Generate( GPTreeNode node, int level )
        {
            // create gene for the node
            if ( level == 0 )
            {
                // the gene should be an argument
                node.Gene = root.Gene.CreateNew( GPGeneType.Argument );
            }
            else
            {
                // the gene can be function or argument
                node.Gene = root.Gene.CreateNew( );
            }

            // add children
            if ( node.Gene.ArgumentsCount != 0 )
            {
                node.Children = new List<GPTreeNode>( );
                for ( int i = 0; i < node.Gene.ArgumentsCount; i++ )
                {
                    // create new child
                    GPTreeNode child = new GPTreeNode( );
                    Generate( child, level - 1 );
                    // add the new child
                    node.Children.Add( child );
                }
            }
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
            return new GPTreeChromosome( root.Gene.Clone( ) );
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
            return new GPTreeChromosome( this );
        }

        /// <summary>
        /// Mutation operator.
        /// </summary>
        /// 
        /// <remarks><para>The method performs chromosome's mutation by regenerating tree's
        /// randomly selected node.</para></remarks>
        ///
        public override void Mutate( )
        {
            // current tree level
            int			currentLevel = 0;
            // current node
            GPTreeNode	node = root;

            for ( ; ; )
            {
                // regenerate node if it does not have children
                if ( node.Children == null )
                {
                    if ( currentLevel == maxLevel )
                    {
                        // we reached maximum possible level, so the gene
                        // can be an argument only
                        node.Gene.Generate( GPGeneType.Argument );
                    }
                    else
                    {
                        // generate subtree
                        Generate( node, rand.Next( maxLevel - currentLevel ) );
                    }
                    break;
                }

                // if it is a function node, than we need to get a decision, about
                // mutation point - the node itself or one of its children
                int r = rand.Next( node.Gene.ArgumentsCount + 1 );

                if ( r == node.Gene.ArgumentsCount )
                {
                    // node itself should be regenerated
                    node.Gene.Generate( );

                    // check current type
                    if ( node.Gene.GeneType == GPGeneType.Argument )
                    {
                        node.Children = null;
                    }
                    else
                    {
                        // create children's list if it was absent
                        if ( node.Children == null )
                            node.Children = new List<GPTreeNode>( );

                        // check for missing or extra children
                        if ( node.Children.Count != node.Gene.ArgumentsCount )
                        {
                            if ( node.Children.Count > node.Gene.ArgumentsCount )
                            {
                                // remove extra children
                                node.Children.RemoveRange( node.Gene.ArgumentsCount, node.Children.Count - node.Gene.ArgumentsCount );
                            }
                            else
                            {
                                // add missing children
                                for ( int i = node.Children.Count; i < node.Gene.ArgumentsCount; i++ )
                                {
                                    // create new child
                                    GPTreeNode child = new GPTreeNode( );
                                    Generate( child, rand.Next( maxLevel - currentLevel ) );
                                    // add the new child
                                    node.Children.Add( child );
                                }
                            }
                        }
                    }
                    break;
                }

                // mutation goes further to one of the children
                node = (GPTreeNode) node.Children[r];
                currentLevel++;
            }
        }

        /// <summary>
        /// Crossover operator.
        /// </summary>
        /// 
        /// <param name="pair">Pair chromosome to crossover with.</param>
        /// 
        /// <remarks><para>The method performs crossover between two chromosomes – interchanging
        /// randomly selected sub trees.</para></remarks>
        ///
        public override void Crossover( IChromosome pair )
        {
            GPTreeChromosome p = (GPTreeChromosome) pair;

            // check for correct pair
            if ( p != null )
            {
                // do we need to use root node for crossover ?
                if ( ( root.Children == null ) || ( rand.Next( maxLevel ) == 0 ) )
                {
                    // give the root to the pair and use pair's part as a new root
                    root = p.RandomSwap( root );
                }
                else
                {
                    GPTreeNode node = root;

                    for ( ; ; )
                    {
                        // choose random child
                        int r = rand.Next( node.Gene.ArgumentsCount );
                        GPTreeNode child = (GPTreeNode) node.Children[r];

                        // swap the random node, if it is an end node or
                        // random generator "selected" this node
                        if ( ( child.Children == null ) || ( rand.Next( maxLevel ) == 0 ) )
                        {
                            // swap the node with pair's one
                            node.Children[r] = p.RandomSwap( child );
                            break;
                        }

                        // go further by tree
                        node = child;
                    }
                }
                // trim both of them
                Trim( root, maxLevel );
                Trim( p.root, maxLevel );
            }
        }

        /// <summary>
        /// Crossover helper routine - selects random node of chromosomes tree and
        /// swaps it with specified node.
        /// </summary>
        private GPTreeNode RandomSwap( GPTreeNode source )
        {
            GPTreeNode retNode = null;

            // swap root node ?
            if ( ( root.Children == null ) || ( rand.Next( maxLevel ) == 0 ) )
            {
                // replace current root and return it
                retNode = root;
                root = source;
            }
            else
            {
                GPTreeNode node = root;

                for ( ; ; )
                {
                    // choose random child
                    int r = rand.Next( node.Gene.ArgumentsCount );
                    GPTreeNode child = (GPTreeNode) node.Children[r];

                    // swap the random node, if it is an end node or
                    // random generator "selected" this node
                    if ( ( child.Children == null ) || ( rand.Next( maxLevel ) == 0 ) )
                    {
                        // swap the node with pair's one
                        retNode = child;
                        node.Children[r] = source;
                        break;
                    }

                    // go further by tree
                    node = child;
                }
            }
            return retNode;
        }

        /// <summary>
        /// Trim tree node, so its depth does not exceed specified level.
        /// </summary>
        private static void Trim( GPTreeNode node, int level )
        {
            // check if the node has children
            if ( node.Children != null )
            {
                if ( level == 0 )
                {
                    // remove all children
                    node.Children = null;
                    // and make the node of argument type
                    node.Gene.Generate( GPGeneType.Argument );
                }
                else
                {
                    // go further to children
                    foreach ( GPTreeNode n in node.Children )
                    {
                        Trim( n, level - 1 );
                    }
                }
            }
        }
    }
}
