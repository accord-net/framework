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

namespace Accord.Tests.Statistics
{
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class LogisticRegressionTest
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



        [TestMethod()]
        public void ComputeTest()
        {
            // Suppose we have the following data about some patients.
            // The first variable is continuous and represent patient
            // age. The second variable is dichotomic and give whether
            // they smoke or not (This is completely fictional data).
            double[][] input =
            {
                new double[] { 55, 0 }, // 0 - no cancer
                new double[] { 28, 0 }, // 0
                new double[] { 65, 1 }, // 0
                new double[] { 46, 0 }, // 1 - have cancer
                new double[] { 86, 1 }, // 1
                new double[] { 56, 1 }, // 1
                new double[] { 85, 0 }, // 0
                new double[] { 33, 0 }, // 0
                new double[] { 21, 1 }, // 0
                new double[] { 42, 1 }, // 1
            };

            // We also know if they have had lung cancer or not, and 
            // we would like to know whether smoking has any connection
            // with lung cancer (This is completely fictional data).
            double[] output =
            {
                0, 0, 0, 1, 1, 1, 0, 0, 0, 1
            };


            // To verify this hypothesis, we are going to create a logistic
            // regression model for those two inputs (age and smoking).
            LogisticRegression regression = new LogisticRegression(inputs: 2);

            // Next, we are going to estimate this model. For this, we
            // will use the Iteratively Reweighted Least Squares method.
            var teacher = new IterativeReweightedLeastSquares(regression);

            // Now, we will iteratively estimate our model. The Run method returns
            // the maximum relative change in the model parameters and we will use
            // it as the convergence criteria.

            double delta = 0;
            do
            {
                // Perform an iteration
                delta = teacher.Run(input, output);

            } while (delta > 0.001);

            // At this point, we can compute the odds ratio of our variables.
            // In the model, the variable at 0 is always the intercept term, 
            // with the other following in the sequence. Index 1 is the age
            // and index 2 is whether the patient smokes or not.

            // For the age variable, we have that individuals with
            //   higher age have 1.021 greater odds of getting lung
            //   cancer controlling for cigarette smoking.
            double ageOdds = regression.GetOddsRatio(1); // 1.0208597028836701

            // For the smoking/non smoking category variable, however, we
            //   have that individuals who smoke have 5.858 greater odds
            //   of developing lung cancer compared to those who do not 
            //   smoke, controlling for age (remember, this is completely
            //   fictional and for demonstration purposes only).
            double smokeOdds = regression.GetOddsRatio(2); // 5.8584748789881331


            Assert.AreEqual(1.0208597028836701, ageOdds, 1e-10);
            Assert.AreEqual(5.8584748789881331, smokeOdds,1e-10);
            Assert.IsFalse(double.IsNaN(ageOdds));
            Assert.IsFalse(double.IsNaN(smokeOdds));
        }

        [TestMethod()]
        public void RegressTest()
        {

            double[,] inputGrouped =
            {
                { 1, 4, 5 }, // product 1 has four occurrences of class 1 and five  of class 0
                { 2, 1, 3 }, // product 2 has one  occurrence  of class 1 and three of class 0
            };

            double[,] inputGroupProb =
            {
                { 1, 4.0 / (4 + 5) }, // product 1 has 0.44 probability of belonging to class 1
                { 2, 1.0 / (1 + 3) }, // product 2 has 0.25 probability of belonging to class 1
            };


            double[,] inputExtended =
            {
                { 1, 1 }, // observation of product 1 in class 1
                { 1, 1 }, // observation of product 1 in class 1
                { 1, 1 }, // observation of product 1 in class 1
                { 1, 1 }, // observation of product 1 in class 1
                { 1, 0 }, // observation of product 1 in class 0
                { 1, 0 }, // observation of product 1 in class 0
                { 1, 0 }, // observation of product 1 in class 0
                { 1, 0 }, // observation of product 1 in class 0
                { 1, 0 }, // observation of product 1 in class 0
                { 2, 1 }, // observation of product 2 in class 1
                { 2, 0 }, // observation of product 2 in class 0
                { 2, 0 }, // observation of product 2 in class 0
                { 2, 0 }, // observation of product 2 in class 0
            };

             
            // Fit using extended data
            double[][] inputs = Matrix.ColumnVector(inputExtended.GetColumn(0)).ToArray();
            double[] outputs = inputExtended.GetColumn(1);
            LogisticRegression target = new LogisticRegression(1);
            IterativeReweightedLeastSquares irls = new IterativeReweightedLeastSquares(target);
            irls.Run(inputs, outputs);

            // Fit using grouped data
            double[][] inputs2 = Matrix.ColumnVector(inputGroupProb.GetColumn(0)).ToArray();
            double[] outputs2 = inputGroupProb.GetColumn(1);
            LogisticRegression target2 = new LogisticRegression(1);
            IterativeReweightedLeastSquares irls2 = new IterativeReweightedLeastSquares(target2);
            irls2.Run(inputs2, outputs2);


            Assert.IsTrue(Matrix.IsEqual(target.Coefficients, target2.Coefficients, 0.000001));



            double[,] data = new double[,]
            {
                {  1, 0 },
                {  2, 0 },
                {  3, 0 },
                {  4, 0 },
                {  5, 1 },
                {  6, 0 },
                {  7, 1 },
                {  8, 0 },
                {  9, 1 },
                { 10, 1 }
            };


            double[][] inputs3 = Matrix.ColumnVector(data.GetColumn(0)).ToArray();
            double[] outputs3 = data.GetColumn(1);
            LogisticRegressionAnalysis analysis = new LogisticRegressionAnalysis(inputs3, outputs3);

            analysis.Compute();

            Assert.IsFalse(double.IsNaN(analysis.Deviance));
            Assert.IsFalse(double.IsNaN(analysis.ChiSquare.PValue));

            Assert.AreEqual(analysis.Deviance, 8.6202, 0.0005);
            Assert.AreEqual(analysis.ChiSquare.PValue, 0.0278, 0.0005);

            // Check intercept
            Assert.IsFalse(double.IsNaN(analysis.Coefficients[0].Value));
            Assert.AreEqual(analysis.Coefficients[0].Value, -4.3578, 0.0005);

            // Check coefficients
            Assert.IsFalse(double.IsNaN(analysis.Coefficients[1].Value));
            Assert.AreEqual(analysis.Coefficients[1].Value, 0.6622, 0.0005);

            // Check statistics
            Assert.AreEqual(analysis.Coefficients[1].StandardError, 0.4001, 0.0005);
            Assert.AreEqual(analysis.Coefficients[1].Wald.PValue, 0.0979, 0.0005);

            Assert.AreEqual(analysis.Coefficients[1].OddsRatio, 1.9391, 0.0005);

            Assert.AreEqual(analysis.Coefficients[1].ConfidenceLower, 0.8852, 0.0005);
            Assert.AreEqual(analysis.Coefficients[1].ConfidenceUpper, 4.2478, 0.0005);


            Assert.IsFalse(double.IsNaN(analysis.Coefficients[1].Wald.PValue));
            Assert.IsFalse(double.IsNaN(analysis.Coefficients[1].StandardError));
            Assert.IsFalse(double.IsNaN(analysis.Coefficients[1].OddsRatio));
            Assert.IsFalse(double.IsNaN(analysis.Coefficients[1].ConfidenceLower));
            Assert.IsFalse(double.IsNaN(analysis.Coefficients[1].ConfidenceUpper));
        }


    }
}
