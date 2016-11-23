namespace Accord.Tests.FSharp.Math

open System
open System.IO
open System.Collections.Generic

open Accord
open Accord.Math
open Accord.Math.Optimization

open NUnit.Framework


type CobylaTest() = 


    [<TestCase()>]
    member x.InconsistentConstraintsTest() =

        let obj12 = QuadraticObjectiveFunction("a - a*a");
        let c12 = QuadraticConstraint(obj12, Array2D.zeroCreate 1 1, [| 10.0 |], ConstraintType.LesserThanOrEqualTo, 4.0)
        let c13 = QuadraticConstraint(obj12, Array2D.zeroCreate 1 1, [| 10.0 |], ConstraintType.GreaterThanOrEqualTo, 45.0)

        let p1 = List<NonlinearConstraint>()
        p1.Add(c12)
        p1.Add(c13)
        let solver1 = Cobyla(obj12, p1)  
        let success = solver1.Maximize()
        let value = solver1.Value
        let solution = solver1.Solution
        let r = solver1.Status

        Assert.IsFalse(success)
        Assert.AreEqual(r, CobylaStatus.NoPossibleSolution)
        
