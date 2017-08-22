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


type FSharpMultilabelSvmTest() = 

    static member Run(cost : double) =
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
        
        let features = 28 * 28
        let classes = 10
 
        let algorithm = 
            fun (svm: KernelSupportVectorMachine) 
                (classInputs: float[][]) 
                (classOutputs: int[]) (i: int) (j: int) -> 
                let strategy = LinearCoordinateDescent(svm, classInputs, classOutputs)
                if (cost <> 0.0) then
                  strategy.Complexity <- cost
                strategy :> ISupportVectorMachineLearning
 
        let kernel = Linear()
        let svm = new MultilabelSupportVectorMachine(features, kernel, classes)
        let learner = MultilabelSupportVectorLearning(svm, observations, labels)
        #if DEBUG
        svm.ParallelOptions.MaxDegreeOfParallelism <- 1
        learner.ParallelOptions.MaxDegreeOfParallelism <- 1
        #endif
        let config = SupportVectorMachineLearningConfigurationFunction(algorithm)
        learner.Algorithm <- config
 
        let error = 
            try
                learner.Run() 
            with
            | :? ConvergenceException -> System.Double.NaN
            | :? AggregateException -> System.Double.NaN
            
        let validationLabels, validationObservations = readData validation
 
        let correct =
            Array.zip validationLabels validationObservations 
            |> Array.map (fun (l, o) -> 
                let d = ref 0
                let output = svm.Scores(o, d)
                if !d = l then 1. else 0.)
            |> Array.average
            
        (error, correct)



    static member RunNew(cost : double) =
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

        let learner = MultilabelSupportVectorLearning<Linear>()
        learner.Configure(fun () -> LinearCoordinateDescent<Linear>())
        #if DEBUG
        learner.ParallelOptions.MaxDegreeOfParallelism <- 1
        #endif

        let svm = learner.Learn(observations, labels) 
  
        let training =
            Array.zip labels observations 
            |> Array.map (fun (l, o) -> 
                let d = ref 0
                let output = svm.Scores(o, d)
                if !d = l then 1. else 0.)
            |> Array.average

        let validationLabels, validationObservations = readData validation
 
        let validation =
            Array.zip validationLabels validationObservations 
            |> Array.map (fun (l, o) -> 
                let d = ref 0
                let output = svm.Scores(o, d)
                if !d = l then 1. else 0.)
            |> Array.average
            
        (training, validation)



    static member RunDual(cost : double) =

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

        let learner = MultilabelSupportVectorLearning<Linear>()
        learner.Configure(fun () -> LinearDualCoordinateDescent<Linear>())
        #if DEBUG
        learner.ParallelOptions.MaxDegreeOfParallelism <- 1
        #endif

        let svm = learner.Learn(observations, labels) 
  
        let training =
            Array.zip labels observations 
            |> Array.map (fun (l, o) -> 
                let d = ref 0
                let output = svm.Scores(o, d)
                if !d = l then 1. else 0.)
            |> Array.average

        let validationLabels, validationObservations = readData validation
 
        let validation =
            Array.zip validationLabels validationObservations 
            |> Array.map (fun (l, o) -> 
                let d = ref 0
                let output = svm.Scores(o, d)
                if !d = l then 1. else 0.)
            |> Array.average
            
        (training, validation)



#if RELEASE // the following tests can take a very long time
        
    [<TestCase()>]
    member x.ConvergenceException() =
        Accord.Math.Random.Generator.Seed <- Nullable<int>(0)
        let (error, validation) = FSharpMultilabelSvmTest.Run(1.0)
        Assert.AreEqual(0.0141, error)
        Assert.AreEqual(0.874, validation, 0.01)

    [<TestCase()>]
    member x.fsharp_multilabel_svm_AutoComplexity() =
        let (error, validation) = FSharpMultilabelSvmTest.Run(0.0);
        Assert.AreEqual(0.1, error, 0.002);
        Assert.AreEqual(0.084, validation, 0.005);


    [<TestCase()>]
    member x.fsharp_multilabel_svm_old_method() =
        Accord.Math.Random.Generator.Seed <- Nullable<int>(0)
        let (error, validation) = FSharpMultilabelSvmTest.Run(0.1)
        Assert.AreEqual(0.012, error, 0.005)
        Assert.AreEqual(0.878, validation, 0.005)

    [<TestCase()>]
    member x.fsharp_multilabel_svm_new_method() =
        Accord.Math.Random.Generator.Seed <- Nullable<int>(0)
        let (training, validation) = FSharpMultilabelSvmTest.RunNew(0.1)
        Assert.AreEqual(0.084, validation, 1e-3)
        Assert.AreEqual(0.098, training, 1e-3)
    
    [<TestCase()>]
    member x.fsharp_multilabel_svm_new_method_autocomplexity_primal() =
        Accord.Math.Random.Generator.Seed <- Nullable<int>(0)
        let (training, validation) = FSharpMultilabelSvmTest.RunNew(0.0);
        Assert.AreEqual(0.084, validation, 0.005);
        Assert.AreEqual(0.0988, training, 0.005);

    [<TestCase()>]
    member x.fsharp_multilabel_svm_new_method_autocomplexity_dual() =
        Accord.Math.Random.Generator.Seed <- Nullable<int>(0)
        let (training, validation) = FSharpMultilabelSvmTest.RunDual(0.0);
        Assert.AreEqual(0.884, validation, 0.005);
        Assert.AreEqual(0.9286, training, 0.005);

#endif
