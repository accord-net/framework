// Accord Unit Tests
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

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning.DecisionTrees;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Linq.Expressions;
    using System.Text;
    using Accord.Math;
    using System.Reflection;
    using System.Reflection.Emit;

    [TestClass()]
    public class DecisionTreeCompilerTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



#if !NET35
        [TestMethod()]
        public void CreateTest()
        {
            DecisionTree tree;
            int[][] inputs;
            int[] outputs;

            ID3LearningTest.CreateMitchellExample(out tree, out inputs, out outputs);

            // Convert to an expression tree
            var expression = tree.ToExpression();

            // Compiles the expression
            var func = expression.Compile();


            for (int i = 0; i < inputs.Length; i++)
            {
                int y = func(inputs[i].ToDouble());
                Assert.AreEqual(outputs[i], y);
            }
        }
#endif

    }
}
