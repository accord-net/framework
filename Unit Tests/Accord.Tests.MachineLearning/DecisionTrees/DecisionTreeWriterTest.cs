// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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

namespace Accord.Tests.MachineLearning
{
    using System.CodeDom.Compiler;
    using System.Text;
    using Accord.MachineLearning.DecisionTrees;
    using Microsoft.CSharp;
    using NUnit.Framework;

    [TestFixture]
    public class DecisionTreeWriterTest
    {
#if !NO_CODE_PROVIDER
        [Test]
        public void WriteTest()
        {
            DecisionTree tree;
            int[][] inputs;
            int[] outputs;
            ID3LearningTest.CreateMitchellExample(out tree, out inputs, out outputs);

            var csc = new CSharpCodeProvider();
            var parameters = new CompilerParameters(new[] { "System.dll", "mscorlib.dll" }) {GenerateInMemory = true};

            var compilerResult = csc.CompileAssemblyFromSource(parameters, tree.ToCode("AccordDecisionTree"));

            Assert.That(compilerResult.Errors.HasErrors, Is.False, ToString(compilerResult.Errors));
        }

        private static string ToString(CompilerErrorCollection compilerErrors)
        {
            var builder = new StringBuilder("Compiler errors:\n");
            foreach (var error in compilerErrors)
                builder.AppendLine(error.ToString());
            return builder.ToString();
        }
#endif
    }
}