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

namespace Accord.Tests.Statistics
{
    using System.Data;
    using Accord.Math;
    using Accord.Statistics.Filters;
    using NUnit.Framework;
    using Accord.IO;
    using System;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.MachineLearning.VectorMachines;
    using Accord.Statistics.Kernels;
    using Math.Optimization.Losses;

    [TestFixture]
    public class CodificationFilterSvmTest
    {
#if !NO_DATA_TABLE
        [Test]
        public void ApplyTest2()
        {
            // Suppose we have a data table relating the age of
            // a person and its categorical classification, as 
            // in "child", "adult" or "elder".

            // The Codification filter is able to extract those
            // string labels and transform them into discrete
            // symbols, assigning integer labels to each of them
            // such as "child" = 0, "adult" = 1, and "elder" = 3.

            // Create the aforementioned sample table
            DataTable table = new DataTable("Sample data");
            table.Columns.Add("Age", typeof(int));
            table.Columns.Add("Label", typeof(string));

            //            age   label
            table.Rows.Add(10, "child");
            table.Rows.Add(07, "child");
            table.Rows.Add(04, "child");
            table.Rows.Add(21, "adult");
            table.Rows.Add(27, "adult");
            table.Rows.Add(12, "child");
            table.Rows.Add(79, "elder");
            table.Rows.Add(40, "adult");
            table.Rows.Add(30, "adult");


            // Now, let's say we need to translate those text labels
            // into integer symbols. Let's use a Codification filter:

            Codification codebook = new Codification(table);


            // After that, we can use the codebook to "translate"
            // the text labels into discrete symbols, such as:

            int a = codebook.Translate("Label", "child"); // returns 0
            int b = codebook.Translate("Label", "adult"); // returns 1
            int c = codebook.Translate("Label", "elder"); // returns 2

            // We can also do the reverse:
            string labela = codebook.Translate("Label", 0); // returns "child"
            string labelb = codebook.Translate("Label", 1); // returns "adult"
            string labelc = codebook.Translate("Label", 2); // returns "elder"


            // We can also process an entire data table at once:
            DataTable result = codebook.Apply(table);

            // The resulting table can be transformed to jagged array:
            double[][] matrix = Matrix.ToArray(result);

            // and the resulting matrix will be given by
            string str = matrix.ToString(CSharpJaggedMatrixFormatProvider.InvariantCulture);

            // str == new double[][] 
            // {
            //     new double[] { 10, 0 },
            //     new double[] {  7, 0 },
            //     new double[] {  4, 0 },
            //     new double[] { 21, 1 },
            //     new double[] { 27, 1 },
            //     new double[] { 12, 0 },
            //     new double[] { 79, 2 },
            //     new double[] { 40, 1 },
            //     new double[] { 30, 1 } 
            // };



            // Now we will be able to feed this matrix to any machine learning
            // algorithm without having to worry about text labels in our data:

            int classes = codebook["Label"].Symbols; // 3 classes (child, adult, elder)

            // Use the first column as input variables,
            // and the second column as outputs classes
            //
            double[][] inputs = matrix.GetColumns(new[] { 0 });
            int[] outputs = matrix.GetColumn(1).ToInt32();


            // Create a multi-class SVM for 1 input (Age) and 3 classes (Label)
            var machine = new MulticlassSupportVectorMachine(inputs: 1, classes: classes);

            // Create a Multi-class learning algorithm for the machine
            var teacher = new MulticlassSupportVectorLearning(machine, inputs, outputs)
            {
                // Configure the learning algorithm to use SMO to train the
                //  underlying SVMs in each of the binary class subproblems.
                Algorithm = (svm, classInputs, classOutputs, i, j) =>
                {
                    return new SequentialMinimalOptimization(svm, classInputs, classOutputs)
                    {
                        Complexity = 1
                    };
                }
            };

            // Run the learning algorithm
            double error = teacher.Run();


            // After we have learned the machine, we can use it to classify
            // new data points, and use the codebook to translate the machine
            // outputs to the original text labels:

            string result1 = codebook.Translate("Label", machine.Compute(10)); // child
            string result2 = codebook.Translate("Label", machine.Compute(40)); // adult
            string result3 = codebook.Translate("Label", machine.Compute(70)); // elder


            Assert.AreEqual(0, a);
            Assert.AreEqual(1, b);
            Assert.AreEqual(2, c);
            Assert.AreEqual("child", labela);
            Assert.AreEqual("adult", labelb);
            Assert.AreEqual("elder", labelc);

            Assert.AreEqual("child", result1);
            Assert.AreEqual("adult", result2);
            Assert.AreEqual("elder", result3);
        }

        [Test]
        public void learn_new_method()
        {
            #region doc_learn_1
            // Suppose we have a data table relating the age of
            // a person and its categorical classification, as 
            // in "child", "adult" or "elder".

            // The Codification filter is able to extract those
            // string labels and transform them into discrete
            // symbols, assigning integer labels to each of them
            // such as "child" = 0, "adult" = 1, and "elder" = 3.

            // Create the aforementioned sample table
            DataTable table = new DataTable("Sample data");
            table.Columns.Add("Age", typeof(int));
            table.Columns.Add("Label", typeof(string));

            //            age   label
            table.Rows.Add(10, "child");
            table.Rows.Add(07, "child");
            table.Rows.Add(04, "child");
            table.Rows.Add(21, "adult");
            table.Rows.Add(27, "adult");
            table.Rows.Add(12, "child");
            table.Rows.Add(79, "elder");
            table.Rows.Add(40, "adult");
            table.Rows.Add(30, "adult");


            // Now, let's say we need to translate those text labels
            // into integer symbols. Let's use a Codification filter:

            var codebook = new Codification(table);


            // After that, we can use the codebook to "translate"
            // the text labels into discrete symbols, such as:

            int a = codebook.Transform(columnName: "Label", value: "child"); // returns 0
            int b = codebook.Transform(columnName: "Label", value: "adult"); // returns 1
            int c = codebook.Transform(columnName: "Label", value: "elder"); // returns 2

            // We can also do the reverse:
            string labela = codebook.Revert(columnName: "Label", codeword: 0); // returns "child"
            string labelb = codebook.Revert(columnName: "Label", codeword: 1); // returns "adult"
            string labelc = codebook.Revert(columnName: "Label", codeword: 2); // returns "elder"
            #endregion

            #region doc_learn_2
            // We can also process an entire data table at once:
            DataTable result = codebook.Apply(table);

            // The resulting table can be transformed to jagged array:
            double[][] matrix = Matrix.ToArray(result);

            // and the resulting matrix will be given by
            string str = matrix.ToCSharp();
            #endregion

            // str == new double[][] 
            // {
            //     new double[] { 10, 0 },
            //     new double[] {  7, 0 },
            //     new double[] {  4, 0 },
            //     new double[] { 21, 1 },
            //     new double[] { 27, 1 },
            //     new double[] { 12, 0 },
            //     new double[] { 79, 2 },
            //     new double[] { 40, 1 },
            //     new double[] { 30, 1 } 
            // };


            #region doc_learn_3
            // Now we will be able to feed this matrix to any machine learning
            // algorithm without having to worry about text labels in our data:

            // Use the first column as input variables,
            // and the second column as outputs classes
            //
            double[][] inputs = matrix.GetColumns(0);
            int[] outputs = matrix.GetColumn(1).ToInt32();

            // Create a Multi-class learning algorithm for the machine
            var teacher = new MulticlassSupportVectorLearning<Linear>()
            {
                Learner = (p) => new SequentialMinimalOptimization<Linear>()
                {
                    Complexity = 1
                }
            };

            // Run the learning algorithm
            var svm = teacher.Learn(inputs, outputs);

            // Compute the classification error (should be 0)
            double error = new ZeroOneLoss(outputs).Loss(svm.Decide(inputs));


            // After we have learned the machine, we can use it to classify
            // new data points, and use the codebook to translate the machine
            // outputs to the original text labels:

            string result1 = codebook.Revert("Label", svm.Decide(new double[] { 10 })); // child
            string result2 = codebook.Revert("Label", svm.Decide(new double[] { 40 })); // adult
            string result3 = codebook.Revert("Label", svm.Decide(new double[] { 70 })); // elder
            #endregion

            Assert.AreEqual(0, a);
            Assert.AreEqual(1, b);
            Assert.AreEqual(2, c);
            Assert.AreEqual("child", labela);
            Assert.AreEqual("adult", labelb);
            Assert.AreEqual("elder", labelc);

            Assert.AreEqual("child", result1);
            Assert.AreEqual("adult", result2);
            Assert.AreEqual("elder", result3);

        }
#endif
    }
}
