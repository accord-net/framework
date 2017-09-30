' Accord Unit Tests
' The Accord.NET Framework
' http://accord-framework.net
'
' Copyright © César Souza, 2009-2017
' cesarsouza at gmail.com
'
'    This library Is free software; you can redistribute it And/Or
'    modify it under the terms of the GNU Lesser General Public
'    License as published by the Free Software Foundation; either
'    version 2.1 of the License, Or (at your option) any later version.
'
'    This library Is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY Or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
'    Lesser General Public License for more details.
'
'    You should have received a copy of the GNU Lesser General Public
'    License along with this library; if Not, write to the Free Software
'    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
'

Option Strict On

Imports NUnit.Framework
Imports Accord.Math
Imports Accord.Math.Optimization
Imports Accord.Statistics.Models.Regression
Imports Accord.Statistics.Models.Regression.Fitting
Imports Accord.Math.Optimization.Losses
Imports Accord.Statistics.Distributions.Univariate
Imports Accord.Statistics.Analysis
Imports Accord.Statistics.Testing

Public Class ProportionalHazardsAnalysisTest

    <Test>
    Public Sub learn_test()

#Region "doc_learn_part1"
        ' Consider the following example data, adapted from John C. Pezzullo's
        ' example for his great Cox's proportional hazards model example in
        ' JavaScript (http://statpages.org/prophaz2.html). 

        ' In this data, we have three columns. The first column denotes the
        ' input variables for the problem. The second column, the survival
        ' times. And the last one Is the output of the experiment (if the
        ' subject has died [1] Or has survived [0]).

        Dim example()() As Double =
        {
            New Double() {50, 1, 0},
            New Double() {70, 2, 1},
            New Double() {45, 3, 0},
            New Double() {35, 5, 0},
            New Double() {62, 7, 1},
            New Double() {50, 11, 0},
            New Double() {45, 4, 0},
            New Double() {57, 6, 0},
            New Double() {32, 8, 0},
            New Double() {57, 9, 1},
            New Double() {60, 10, 1}
        }

        ' First we will extract the input, times And outputs
        Dim inputs()() As Double = example.Get(Nothing, 0, 1)
        Dim times() As Double = example.GetColumn(1)
        Dim output() As SurvivalOutcome = example.GetColumn(2).To(Of SurvivalOutcome())

        ' Now we can proceed And create the analysis (giving optional variable names)
        Dim cox = New ProportionalHazardsAnalysis(New String() {"input"}, "time", "censor")

        ' Then compute the analysis, learning a regression in the process
        Dim regression As ProportionalHazards = cox.Learn(inputs, times, output)

        ' Now we can show an analysis summary
        ' Accord.Controls.DataGridBox.Show(cox.Coefficients);
#End Region

#Region "doc_learn_part2"

        ' We can also investigate all parameters individually. For
        ' example the coefficients values will be available at

        Dim coef As Double() = cox.CoefficientValues    ' should be { 0.37704239281490765 }
        Dim stde As Double() = cox.StandardErrors       ' should be { 0.25415746361167235 }

        ' We can also obtain the hazards ratios         
        Dim ratios As Double() = cox.HazardRatios       ' should be { 1.4579661153488215 }

        ' And other information such as the partial
        ' likelihood, the deviance And also make 
        ' hypothesis tests on the parameters

        Dim partialL = cox.LogLikelihood                ' should be -2.0252666205735466
        Dim deviance = cox.Deviance                     ' should be 4.0505332411470931

        ' Chi-Square for whole model              
        Dim chi As ChiSquareTest = cox.ChiSquare        ' should be 7.3570 (p=0.0067)

        ' Wald tests for individual parameters    
        Dim wald As WaldTest = cox.Coefficients(0).Wald ' should be 1.4834 (p=0.1379)


        ' Finally, we can also use the model to predict
        ' scores for New observations (without considering time)

        Dim y1 = cox.Regression.Probability(New Double() {63}) ' should be 86.138421225296526
        Dim y2 = cox.Regression.Probability(New Double() {32}) ' should be 0.00072281400325299814

        ' Those scores can be interpreted by comparing then
        ' to 1. If they are greater than one, the odds are
        ' the patient will Not survive. If the value Is less
        ' than one, the patient Is likely to survive.

        ' The first value, y1, gives approximately 86.138,
        ' while the second value, y2, gives about 0.00072.


        ' We can also consider instant estimates for a given time
        Dim p1 = cox.Regression.Probability(New Double() {63}, 2)  'should be 0.17989138010770425
        Dim p2 = cox.Regression.Probability(New Double() {63}, 10) ' should be 15.950244161356357

        ' Here, p1 Is the score after 2 time instants, with a 
        ' value of 0.0656. The second value, p2, Is the time
        ' after 10 time instants, with a value of 6.2907.

        ' In addition, if we would Like a higher precision when 
        ' computing very small probabilities using the methods 
        ' above, we can use the LogLikelihood methods instead

        Dim log_y1 = cox.Regression.LogLikelihood(New Double() {63})     ' should be  4.4559555514489091
        Dim log_y2 = cox.Regression.LogLikelihood(New Double() {32})     ' should be -7.2323586258132284
        Dim log_p1 = cox.Regression.LogLikelihood(New Double() {63}, 2)  ' should be -1.7154020540835324
        Dim log_p2 = cox.Regression.LogLikelihood(New Double() {63}, 10) ' should be  2.7694741370357177
#End Region

        Assert.AreEqual(86.138421225296526, y1, 0.0000000001)
        Assert.AreEqual(0.00072281400325299814, y2, 0.0000000001)

        Assert.AreEqual(0.17989138010770425, p1, 0.0000000001)
        Assert.AreEqual(15.950244161356357, p2, 0.0000000001)

        Assert.AreEqual(4.4559555514489091, log_y1, 0.0000000001)
        Assert.AreEqual(-7.2323586258132284, log_y2, 0.0000000001)

        Assert.AreEqual(-1.7154020540835324, log_p1, 0.0000000001)
        Assert.AreEqual(2.7694741370357177, log_p2, 0.0000000001)

        Assert.AreEqual(System.Math.Log(y1), log_y1, 0.0000000001)
        Assert.AreEqual(System.Math.Log(y2), log_y2, 0.0000000001)

        Assert.AreEqual(System.Math.Log(p1), log_p1, 0.0000000001)
        Assert.AreEqual(System.Math.Log(p2), log_p2, 0.0000000001)

        Assert.AreEqual(1, coef.Length)
        Assert.AreEqual(0.37704239281490765, coef(0))
        Assert.AreEqual(0.25415746361167235, stde(0))
        Assert.AreEqual(1.4579661153488215, ratios(0))

        Assert.AreEqual(-2.0252666205735466, partialL, 0.000001)
        Assert.AreEqual(4.0505332411470931, deviance, 0.000001)

        Assert.AreEqual(0.13794183001851756, wald.PValue, 0.0001)

        Assert.AreEqual(1, chi.DegreesOfFreedom)
        Assert.AreEqual(7.357, chi.Statistic, 0.0001)
        Assert.AreEqual(0.0067, chi.PValue, 0.001)
    End Sub

End Class
