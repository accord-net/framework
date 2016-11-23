﻿// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using System.Globalization;
    using System.IO;

    /// <summary>
    ///   Decision Tree C# Writer.
    /// </summary>
    /// 
    internal class DecisionTreeWriter
    {

        TextWriter writer;

        /// <summary>
        ///   Initializes a new instance of the <see cref="DecisionTreeWriter"/> class.
        /// </summary>
        /// 
        internal DecisionTreeWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        ///   Creates a C# code for the tree.
        /// </summary>
        /// 
        public void Write(DecisionTree tree, string className)
        {
            writer.WriteLine("// This file has been automatically generated by the");
            writer.WriteLine("//");
            writer.WriteLine("// Accord Machine Learning Library");
            writer.WriteLine("// The Accord.NET Framework");
            writer.WriteLine("// http://accord-framework.net");
            writer.WriteLine("//");
            writer.WriteLine();
            writer.WriteLine("namespace DecisionTrees");
            writer.WriteLine("{");
            writer.WriteLine("    using System.CodeDom.Compiler;");
            writer.WriteLine("    using System.Collections.Generic;");
            writer.WriteLine();
            writer.WriteLine("    /// <summary>");
            writer.WriteLine("    ///   Automatically generated decision tree.");
            writer.WriteLine("    /// </summary>");
            writer.WriteLine("    /// ");
            writer.WriteLine("    [GeneratedCode(\"Accord.NET DecisionTree\", \"3.2\")]");
            writer.WriteLine("    public static class {0}", className);
            writer.WriteLine("    {");
            writer.WriteLine();
            writer.WriteLine("        /// <summary>");
            writer.WriteLine("        ///   Assigns a class label to a given input.");
            writer.WriteLine("        /// </summary>");
            writer.WriteLine("        /// ");
            writer.WriteLine("        public static int Function(double[] input)");
            writer.WriteLine("        {");

            create(tree.Root, 3);

            writer.WriteLine("        }");
            writer.WriteLine("    }");
            writer.WriteLine("}");
        }



        private void create(DecisionNode node, int depth)
        {
            string indent = new string(' ', depth * 4);

            if (!node.IsLeaf)
            {
                int attributeIndex = node.Branches.AttributeIndex;

                // Create all comparison expressions
                for (int i = 0; i < node.Branches.Count; i++)
                {
                    DecisionNode child = node.Branches[i];
                    string cmp = ComparisonExtensions.ToString(child.Comparison);

                    if (i == 0)
                        writer.Write(indent + "if ");
                    else
                        writer.Write(indent + "else if ");

                    string value = child.Value.Value.ToString(CultureInfo.InvariantCulture);

                    writer.Write("(input[{0}] {1} {2}) {{", attributeIndex, cmp, value);
                    writer.WriteLine();
                    create(child, depth + 1);
                    writer.WriteLine(indent + "}");
                }

                writer.WriteLine(indent + "else throw new ArgumentException(\"input\", \"Unexpected value at position " + attributeIndex + ".\");");
            }

            else // node is a leaf
            {
                if (node.Output.HasValue)
                {
                    string value = node.Output.Value.ToString(CultureInfo.InvariantCulture);
                    writer.WriteLine(indent + "return " + value + ";");
                }
                else
                {
                    writer.WriteLine(indent + "return -1;");
                }
            }
        }

    }
}
