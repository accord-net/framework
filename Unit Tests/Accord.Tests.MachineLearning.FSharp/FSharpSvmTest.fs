namespace Accord.Tests.FSharp.MachineLearning

#nowarn "0044"

open System
open System.IO

open Accord
open Accord.MachineLearning
open Accord.MachineLearning.VectorMachines
open Accord.MachineLearning.VectorMachines.Learning
open Accord.Statistics.Kernels
open NUnit.Framework


type FSharpSvmTest() = 


    (* 
    The dataset I am using here is a subset of the Kaggle digit recognizer;
    download it first on your machine, and correct path accordingly.
    Training set of 5,000 examples: 
    http://brandewinder.blob.core.windows.net/public/trainingsample.csv
    Validation set of 500 examples, to test your model:
    http://brandewinder.blob.core.windows.net/public/validationsample.csv
    *)

    // static member error = 0.0 

    static member Run(cost : double, ?methd: MulticlassComputeMethod) =

        let root = __SOURCE_DIRECTORY__ 
        let training = root + "/trainingsample.csv"
        let validation = root + "/validationsample.csv"
 
            
        let readData filePath =
            File.ReadAllLines filePath
            |> fun lines -> lines.[1..]
            |> Array.map (fun line -> line.Split(','))
            |> Array.map (fun line -> 
                (line.[0] |> Convert.ToInt32), (line.[1..] |> Array.map Convert.ToDouble))
            |> Array.unzip
 
        let labels, observations = readData training

        // Let's visualize a couple of the data points 
        // for i in 0 .. 9 do      
        //    Digits.Visualizer.draw (labels.[i], observations.[i]) (string labels.[i])

        (*
        Note that while this is a small dataset, loading the data
        is an expensive part of the process. 
        *)

        let features = 28 * 28
        let classes = 10
 
        let algorithm = 
            fun (svm: KernelSupportVectorMachine) 
                (classInputs: float[][]) 
                (classOutputs: int[]) (i: int) (j: int) -> 
                let strategy = SequentialMinimalOptimization(svm, classInputs, classOutputs)
                
                if (cost <> 0.0) then
                  strategy.Complexity <- cost

                strategy :> ISupportVectorMachineLearning
 
        let kernel = Linear()
        let svm = new MulticlassSupportVectorMachine(features, kernel, classes)
        let learner = MulticlassSupportVectorLearning(svm, observations, labels)
        #if DEBUG
        svm.ParallelOptions.MaxDegreeOfParallelism <- 1
        learner.ParallelOptions.MaxDegreeOfParallelism <- 1
        #endif
        let config = SupportVectorMachineLearningConfigurationFunction(algorithm)
        learner.Algorithm <- config
 
        match methd with
        | Some(m) -> svm.Method <- m
        | None -> printfn "no methd"

        let error = 
            try
                learner.Run() 
            with
            | :? ConvergenceException -> System.Double.NaN
            | :? AggregateException -> System.Double.NaN
            

        (*
        Are we done yet? Not quite.
        The proof of the model is in how it deals with data 
        it has never seen before, hence the validation set.
        *)
  
        let validationLabels, validationObservations = readData validation
 
        let correct =
            Array.zip validationLabels validationObservations 
            |> Array.map (fun (l, o) -> if l = svm.Compute(o) then 1. else 0.)
            |> Array.average
            
        (error, correct)
 (*
        let view =
            let rng = Random()
            let size = Array.length validationLabels
            seq { for i in 0 .. 9 do 
                      let x = rng.Next(size)
                      let (real,pixels) = validationLabels.[x], validationObservations.[x]
                      let pred = svm.Compute(pixels)
                      yield (real,pixels),pred
                }
                |> Seq.iter (fun ((real,pixels),pred) -> 
                    Digits.Visualizer.draw (real,pixels) (sprintf "%i -> %i" real pred))
*)
        (*
        At that point in time we have a decent prediction model.
        Now we can work on making it better - that is, either
        faster or more accurate (or both!). The beauty of having
        a REPL here is that I don't need to reload data, 
        I can just keep going. 
        *)




    static member RunNew(cost : double, ?methd: MulticlassComputeMethod) =

        let root = __SOURCE_DIRECTORY__ 
        let training = root + "/trainingsample.csv"
        let validation = root + "/validationsample.csv"
 
            
        let readData filePath =
            File.ReadAllLines filePath
            |> fun lines -> lines.[1..]
            |> Array.map (fun line -> line.Split(','))
            |> Array.map (fun line -> 
                (line.[0] |> Convert.ToInt32), (line.[1..] |> Array.map Convert.ToDouble))
            |> Array.unzip
 
        let labels, observations = readData training

        // Let's visualize a couple of the data points 
        // for i in 0 .. 9 do      
        //    Digits.Visualizer.draw (labels.[i], observations.[i]) (string labels.[i])

        (*
        Note that while this is a small dataset, loading the data
        is an expensive part of the process. 
        *)

        let learner = MulticlassSupportVectorLearning<Linear>()
        learner.Configure(fun () -> SequentialMinimalOptimization<Linear>())
        #if DEBUG
        learner.ParallelOptions.MaxDegreeOfParallelism <- 1
        #endif

        let svm = learner.Learn(observations, labels) 
            
        match methd with
        | Some(m) -> svm.Method <- m
        | None -> printfn "no methd"


        (*
        Are we done yet? Not quite.
        The proof of the model is in how it deals with data 
        it has never seen before, hence the validation set.
        *)
  
        let error =
            Array.zip labels observations 
            |> Array.map (fun (l, o) -> if l = svm.Decide(o) then 1. else 0.)
            |> Array.average

        let validationLabels, validationObservations = readData validation
 
        let validation =
            Array.zip validationLabels validationObservations 
            |> Array.map (fun (l, o) -> if l = svm.Decide(o) then 1. else 0.)
            |> Array.average
            
        (1.0 - error, validation)
 (*
        let view =
            let rng = Random()
            let size = Array.length validationLabels
            seq { for i in 0 .. 9 do 
                      let x = rng.Next(size)
                      let (real,pixels) = validationLabels.[x], validationObservations.[x]
                      let pred = svm.Compute(pixels)
                      yield (real,pixels),pred
                }
                |> Seq.iter (fun ((real,pixels),pred) -> 
                    Digits.Visualizer.draw (real,pixels) (sprintf "%i -> %i" real pred))
*)
        (*
        At that point in time we have a decent prediction model.
        Now we can work on making it better - that is, either
        faster or more accurate (or both!). The beauty of having
        a REPL here is that I don't need to reload data, 
        I can just keep going. 
        *)

#if RELEASE // the following tests can take a very long time
                
    [<TestCase()>]
    member x.fsharp_multiclass_old_methd() =
        let (error, validation) = FSharpSvmTest.Run(0.1)
        Assert.AreEqual(0.9, validation, 0.005)
        Assert.AreEqual(0, error)

    [<TestCase()>]
    member x.fsharp_multiclass_elimination() =
        let (error, validation) = FSharpSvmTest.Run(0.1, MulticlassComputeMethod.Elimination)
        Assert.AreEqual(0.9, validation, 0.005)
        Assert.AreEqual(0, error)

    [<TestCase()>]
    member x.fsharp_multiclass_convergence_exception() =
        let (error, validation) = FSharpSvmTest.Run(1.0)
        Assert.AreEqual(Double.NaN, error)
        Assert.AreEqual(0.9, validation, 0.01)

    [<TestCase()>]
    member x.fsharp_multiclass_voting() =
        let (error, validation) = FSharpSvmTest.Run(0.1, MulticlassComputeMethod.Voting)
        Assert.AreEqual(0.9, validation, 0.005)
        Assert.AreEqual(0, error)

    [<TestCase()>]
    member x.fsharp_multiclass_elimination_new_methd() =
        let (error, validation) = FSharpSvmTest.RunNew(0.1, MulticlassComputeMethod.Elimination)
        Assert.AreEqual(0.918, validation, 1e-3)
        Assert.AreEqual(0.054, error, 1e-3)

    [<TestCase()>]
    member x.fsharp_multiclass_voting_new_methd() =
        let (error, validation) = FSharpSvmTest.RunNew(0.1, MulticlassComputeMethod.Voting)
        Assert.AreEqual(0.922, validation, 0.005)
        Assert.AreEqual(0.054, error, 1e-3)

    [<TestCase()>]
    member x.fsharp_multiclass_auto_complexity() =
        let (error, validation) = FSharpSvmTest.Run(0.0);
        Assert.AreEqual(0.92, validation, 0.005);
        Assert.AreEqual(0.0546, error, 0.002);

    [<TestCase()>]
    member x.fsharp_multiclass_auto_complexity_new_methd() =
        let (error, validation) = FSharpSvmTest.RunNew(0.0);
        Assert.AreEqual(0.92, validation, 0.005);
        Assert.AreEqual(0.0546, error, 0.002);

#endif

