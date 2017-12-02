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

Public Class NonlinearLeastSquaresTest

    <Test>
    Public Sub learn_test()

#Region "doc_learn_lm"
        ' Suppose we would Like to map the continuous values in the
        ' second column to the integer values in the first column.
        Dim data(,) As Double =
        {
            {-40, -21142.1111111111},
            {-30, -21330.1111111111},
            {-20, -12036.1111111111},
            {-10, 7255.3888888889},
            {0, 32474.8888888889},
            {10, 32474.8888888889},
            {20, 9060.8888888889},
            {30, -11628.1111111111},
            {40, -15129.6111111111}
        }

        ' Extract inputs And outputs
        Dim inputs As Double()() = data.GetColumn(0).ToJagged()
        Dim outputs As Double() = data.GetColumn(1)

        ' Create a Nonlinear regression using 
        Dim nls As NonlinearLeastSquares = New NonlinearLeastSquares
        With nls
            .NumberOfParameters = 3

            ' Initialize to some random values
            .StartValues = {4.2, 0.3, 1}

            ' Let's assume a quadratic model function: ax² + bx + c
            .Function = Function(w, x) w(0) * x(0) * x(0) + w(1) * x(0) + w(2)

            ' Derivative in respect to the weights
            .Gradient = Sub(w, x, r)
                            r(0) = 2 * w(0) ' w.r.t a:     2a  
                            r(1) = w(1)     ' w.r.t b: b
                            r(2) = w(2)     ' w.r.t c: 0
                        End Sub
        End With

        Dim algorithm As LevenbergMarquardt = New LevenbergMarquardt
        With algorithm
            .MaxIterations = 100
            .Tolerance = 0
        End With

        nls.Algorithm = algorithm

        Dim regression As NonlinearRegression = nls.Learn(inputs, outputs)

        ' Use the function to compute the input values
        Dim predict As Double() = regression.Transform(inputs)
#End Region


        Assert.IsTrue(TypeOf nls.Algorithm Is LevenbergMarquardt)

        Dim loss As SquareLoss = New SquareLoss(outputs)
        With loss
            .Mean = False
        End With

        Dim err As Double = loss.Loss(predict) / 2.0

        Assert.AreEqual(2145404235.739383, err, 1e-7)

        Assert.AreEqual(-11.916652026711853, regression.Coefficients(0), 0.001)
        Assert.AreEqual(-358.9758898959638, regression.Coefficients(1), 0.001)
        Assert.AreEqual(-107.31273008811895, regression.Coefficients(2), 0.001)

        Assert.AreEqual(-4814.9203769986034, predict(0), 0.0000000001)
        Assert.AreEqual(-63.02285725721211, predict(1), 0.0000000001)
        Assert.AreEqual(2305.5442571416661, predict(2), 0.0000000001)
        Assert.AreEqual(-4888.736831716782, predict(5), 0.0000000001)
    End Sub


    <Test>
    Public Sub simple_gauss_newton_test()
#Region "doc_learn_gn"
        ' Suppose we would Like to map the continuous values in the
        ' second row to the integer values in the first row.
        Dim data As Double(,) =
        {
            {0.03, 0.1947, 0.425, 0.626, 1.253, 2.5, 3.74},
            {0.05, 0.127, 0.094, 0.2122, 0.2729, 0.2665, 0.3317}
        }

        ' Extract inputs And outputs
        Dim inputs As Double()() = data.GetRow(0).ToJagged()
        Dim outputs As Double() = data.GetRow(1)

        ' Create a Nonlinear regression using 
        Dim nls As NonlinearLeastSquares = New NonlinearLeastSquares
        With nls
            ' Initialize to some random values
            .StartValues = {0.9, 0.2}

            ' Let's assume a quadratic model function: ax² + bx + c
            .Function = Function(w, x) w(0) * x(0) / (w(1) + x(0))

            ' Derivative in respect to the weights
            .Gradient = Sub(w, x, r)
                            r(0) = -((-x(0)) / (w(1) + x(0)))
                            r(1) = -((w(0) * x(0)) / System.Math.Pow(w(1) + x(0), 2))
                        End Sub
        End With

        Dim algorithm As GaussNewton = New GaussNewton
        With algorithm
            .MaxIterations = 0
            .Tolerance = 0.00001
        End With

        nls.Algorithm = algorithm

        Dim regression As NonlinearRegression = nls.Learn(inputs, outputs)

        ' Use the function to compute the input values
        Dim predict As Double() = regression.Transform(inputs)
#End Region

        Dim alg As GaussNewton = TryCast(nls.Algorithm, GaussNewton)
        Assert.AreEqual(0, alg.MaxIterations)
        Assert.AreEqual(0.00001, alg.Tolerance)
        Assert.AreEqual(6, alg.CurrentIteration)

        Dim loss As SquareLoss = New SquareLoss(outputs)
        With loss
            .Mean = False
        End With

        Dim err As Double = loss.Loss(predict) / 2.0

        Assert.AreEqual(0.004048452937977628, err, 0.00000001)

        Dim b1 As Double = regression.Coefficients(0)
        Dim b2 As Double = regression.Coefficients(1)

        Assert.AreEqual(0.362, b1, 0.001)
        Assert.AreEqual(0.556, b2, 0.003)

        Assert.AreEqual(1.23859, regression.StandardErrors(0), 0.001)
        Assert.AreEqual(6.06352, regression.StandardErrors(1), 0.005)
    End Sub

End Class
