// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
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

namespace Accord.MachineLearning.DecisionTrees
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    ///   Decision tree.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Represent a decision tree which can be compiled to
    ///   code at run-time. For sample usage and example of
    ///   learning, please see the <see cref="Learning.ID3Learning">
    ///   ID3 learning algorithm for decision trees</see>.</para>
    /// </remarks>
    ///
    /// <seealso cref="Learning.ID3Learning"/>
    /// <seealso cref="Learning.C45Learning"/>
    ///
    [Serializable]
    public class DecisionTree : IEnumerable<DecisionNode>
    {
        /// <summary>
        ///   Gets or sets the root node for this tree.
        /// </summary>
        /// 
        public DecisionNode Root { get; set; }

        /// <summary>
        ///   Gets the collection of attributes processed by this tree.
        /// </summary>
        /// 
        public DecisionVariableCollection Attributes { get; private set; }

        /// <summary>
        ///   Gets the number of distinct output
        ///   classes classified by this tree.
        /// </summary>
        /// 
        public int OutputClasses { get; private set; }

        /// <summary>
        ///   Gets the number of input attributes
        ///   expected by this tree.
        /// </summary>
        /// 
        public int InputCount { get; private set; }

        /// <summary>
        ///   Creates a new <see cref="DecisionTree"/> to process
        ///   the given <paramref name="attributes"/> and the given
        ///   number of possible <paramref name="outputClasses"/>.
        /// </summary>
        /// 
        /// <param name="attributes">An array specifying the attributes to be processed by this tree.</param>
        /// <param name="outputClasses">The number of possible output classes for the given attributes.</param>
        /// 
        public DecisionTree(IList<DecisionVariable> attributes, int outputClasses)
        {
            if (outputClasses <= 0)
                throw new ArgumentOutOfRangeException("outputClasses");
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            for (int i = 0; i < attributes.Count; i++)
                if (attributes[i].Range.Length == 0)
                    throw new ArgumentException("Attribute " + i + " is a constant.");


            this.Attributes = new DecisionVariableCollection(attributes);
            this.InputCount = attributes.Count;
            this.OutputClasses = outputClasses;
        }


        /// <summary>
        ///   Computes the decision for a given input.
        /// </summary>
        /// 
        /// <param name="input">The input data.</param>
        /// 
        /// <returns>A predicted class for the given input.</returns>
        /// 
        public int Compute(int[] input)
        {
            double[] x = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
                x[i] = input[i];

            return Compute(x);
        }

        /// <summary>
        ///   Computes the tree decision for a given input.
        /// </summary>
        /// 
        /// <param name="input">The input data.</param>
        /// 
        /// <returns>A predicted class for the given input.</returns>
        /// 
        public int Compute(double[] input)
        {
            if (Root == null)
                throw new InvalidOperationException();

            return Compute(input, Root);
        }

        /// <summary>
        ///   Computes the tree decision for a given input.
        /// </summary>
        /// 
        /// <param name="input">The input data.</param>
        /// <param name="subtree">The node where the decision starts.</param>
        /// 
        /// <returns>A predicted class for the given input.</returns>
        /// 
        public int Compute(double[] input, DecisionNode subtree)
        {
            if (subtree == null) 
                throw new ArgumentNullException("subtree");

            if (subtree.Owner != this) 
                throw new ArgumentException("The node does not belong to this tree.", "subtree");

            DecisionNode current = subtree;

            // Start reasoning
            while (current != null)
            {
                // Check if this is a leaf
                if (current.IsLeaf)
                {
                    // This is a leaf node. The decision
                    // process thus should stop here.

                    return (current.Output.HasValue) ? current.Output.Value : -1;
                }

                // This node is not a leaf. Continue the
                // decision process following the children

                // Get the next attribute to guide reasoning
                int attribute = current.Branches.AttributeIndex;

                // Check which child is responsible for dealing
                // which the particular value of the attribute
                DecisionNode nextNode = null;

                foreach (DecisionNode branch in current.Branches)
                {
                    if (branch.Compute(input[attribute]))
                    {
                        // This is the child node responsible for dealing
                        // which this particular attribute value. Choose it
                        // to continue reasoning.

                        nextNode = branch; break;
                    }
                }

                current = nextNode;
            }

            // Normal execution should not reach here.
            throw new InvalidOperationException("The tree is degenerated. This is often a sign that "
                + "the tree is expecting discrete inputs, but it was given only real values.");
        }



        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            foreach (DecisionNode node in this)
            {
                node.Owner = this;
            }
        }


        /// <summary>
        ///   Returns an enumerator that iterates through the tree.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<DecisionNode> GetEnumerator()
        {
            if (Root == null)
                yield break;

            var stack = new Stack<DecisionNode>(new[] { Root });

            while (stack.Count != 0)
            {
                DecisionNode current = stack.Pop();

                yield return current;

                if (current.Branches != null)
                    for (int i = current.Branches.Count - 1; i >= 0; i--)
                        stack.Push(current.Branches[i]);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///   Traverse the tree using a <see cref="DecisionTreeTraversal">tree 
        ///   traversal method</see>. Can be iterated with a foreach loop.
        /// </summary>
        /// 
        /// <param name="method">The tree traversal method. Common methods are
        /// available in the <see cref="TreeTraversal"/>static class.</param>
        /// 
        /// <returns>An <see cref="IEnumerable{T}"/> object which can be used to
        /// traverse the tree using the chosen traversal method.</returns>
        /// 
        public IEnumerable<DecisionNode> Traverse(DecisionTreeTraversalMethod method)
        {
            return new TreeTraversal(Root, method);
        }

        /// <summary>
        ///   Traverse a subtree using a <see cref="DecisionTreeTraversal">tree 
        ///   traversal method</see>. Can be iterated with a foreach loop.
        /// </summary>
        /// 
        /// <param name="method">The tree traversal method. Common methods are
        /// available in the <see cref="TreeTraversal"/>static class.</param>
        /// <param name="subtree">The root of the subtree to be traversed.</param>
        /// 
        /// <returns>An <see cref="IEnumerable{T}"/> object which can be used to
        /// traverse the tree using the chosen traversal method.</returns>
        /// 
        public IEnumerable<DecisionNode> Traverse(DecisionTreeTraversalMethod method, DecisionNode subtree)
        {
            if (subtree.Owner != this) throw new ArgumentException(
                "The node does not belong to this tree.", "subtree");
            return new TreeTraversal(subtree, method);
        }


#if !NET35
        /// <summary>
        ///   Creates an <see cref="Expression">Expression Tree</see> representation
        ///   of this decision tree, which can in turn be compiled into code.
        /// </summary>
        /// 
        /// <returns>A tree in the form of an expression tree.</returns>
        /// 
        public Expression<Func<double[], int>> ToExpression()
        {
            DecisionTreeExpressionCreator compiler = new DecisionTreeExpressionCreator(this);
            return compiler.Create();
        }

        /// <summary>
        ///   Creates a .NET assembly (.dll) containing a static class of
        ///   the given name implementing the decision tree. The class will
        ///   contain a single static Compute method implementing the tree.
        /// </summary>
        /// 
        /// <param name="assemblyName">The name of the assembly to generate.</param>
        /// <param name="className">The name of the generated static class.</param>
        /// 
        public void ToAssembly(string assemblyName, string className)
        {
            ToAssembly(assemblyName, "Accord.MachineLearning.DecisionTrees.Custom", className);
        }

        /// <summary>
        ///   Creates a .NET assembly (.dll) containing a static class of
        ///   the given name implementing the decision tree. The class will
        ///   contain a single static Compute method implementing the tree.
        /// </summary>
        /// 
        /// <param name="assemblyName">The name of the assembly to generate.</param>
        /// <param name="moduleName">The namespace which should contain the class.</param>
        /// <param name="className">The name of the generated static class.</param>
        /// 
        public void ToAssembly(string assemblyName, string moduleName, string className)
        {
            AssemblyBuilder da = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(assemblyName), AssemblyBuilderAccess.Save);

            ModuleBuilder dm = da.DefineDynamicModule(moduleName, assemblyName);
            TypeBuilder dt = dm.DefineType(className);
            MethodBuilder method = dt.DefineMethod("Compute",
                MethodAttributes.Public | MethodAttributes.Static);

            // Compile the tree into a method
            ToExpression().CompileToMethod(method);

            dt.CreateType();
            da.Save(assemblyName);
        }
#endif


     

        /// <summary>
        ///   Generates a C# class implementing the decision tree.
        /// </summary>
        /// 
        /// <param name="className">The name for the generated class.</param>
        /// 
        /// <returns>A string containing the generated class.</returns>
        /// 
        public string ToCode(string className)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                TextWriter writer = new StreamWriter(stream);
                var treeWriter = new DecisionTreeWriter(writer);
                treeWriter.Write(this, className);
                writer.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                TextReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        ///   Generates a C# class implementing the decision tree.
        /// </summary>
        /// 
        /// <param name="className">The name for the generated class.</param>
        /// <param name="writer">The <see cref="TextWriter"/> where the class should be written.</param>
        /// 
        public void ToCode(TextWriter writer, string className)
        {
            var treeWriter = new DecisionTreeWriter(writer);
            treeWriter.Write(this, className);
        }


        /// <summary>
        ///   Loads a tree from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the tree is to be deserialized.</param>
        /// 
        /// <returns>The deserialized tree.</returns>
        /// 
        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                Save(fs);
            }
        }

        /// <summary>
        ///   Saves the tree to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the tree is to be serialized.</param>
        /// 
        public void Save(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
        }

        /// <summary>
        ///   Loads a tree from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the tree is to be deserialized.</param>
        /// 
        /// <returns>The deserialized tree.</returns>
        /// 
        public static DecisionTree Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (DecisionTree)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a tree from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the tree from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized tree.</returns>
        /// 
        public static DecisionTree Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load(fs);
            }
        }



        private class TreeTraversal : IEnumerable<DecisionNode>
        {
            private DecisionNode tree;
            private DecisionTreeTraversalMethod method;

            public TreeTraversal(DecisionNode tree, DecisionTreeTraversalMethod method)
            {
                this.tree = tree;
                this.method = method;
            }

            public IEnumerator<DecisionNode> GetEnumerator()
            {
                return method(tree);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return method(tree);
            }
        }
    }
}

