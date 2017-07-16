using System.CodeDom.Compiler;
using System.Text;
using Accord.MachineLearning.DecisionTrees;
using Microsoft.CSharp;
using NUnit.Framework;

namespace Accord.Tests.MachineLearning
{
    [TestFixture]
    public class DecisionTreeWriterTest
    {
#if !NETSTANDARD2_0
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