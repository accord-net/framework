// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © 2009-2014, César Souza
// All rights reserved. 3-BSD License:
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions are met:
//
//      * Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//
//      * Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//
//      * Neither the name of the Accord.NET Framework authors nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
// 
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;
using Accord.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Liblinear
{
    public class Train
    {
        public static void Main(params string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            if (args.Length == 0)
                return;

            if (args[0] == "--self")
                ExecuteTestAndExit(args);


            string input_file_name;
            string model_file_name;

            var parameters = parse_command_line(args, out input_file_name, out model_file_name);
            var problem = read_problem(input_file_name, parameters.Bias);

            string error_msg = check_parameter(problem, parameters);

            if (!String.IsNullOrEmpty(error_msg))
            {
                Console.WriteLine("ERROR:" + error_msg);
                Environment.Exit(1);
            }

            // Learn the specified problem
            var model = train(problem, parameters);


            try
            {
                // Save the model to disk
                model.Save(model_file_name);
            }
            catch (Exception ex)
            {
                Console.WriteLine("can't save model to file " + model_file_name);
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }




        static void exit_with_help()
        {
            Console.WriteLine("Usage: train [options] training_set_file [model_file]");
            Console.WriteLine("options:");
            Console.WriteLine("-s type : set type of solver (default 1)");
            Console.WriteLine("  for multi-class classification");
            Console.WriteLine("     0 -- L2-regularized logistic regression (primal)");
            Console.WriteLine("     1 -- L2-regularized L2-loss support vector classification (dual)");
            Console.WriteLine("     2 -- L2-regularized L2-loss support vector classification (primal)");
            Console.WriteLine("     3 -- L2-regularized L1-loss support vector classification (dual)");
            Console.WriteLine("     4 -- support vector classification by Crammer and Singer");
            Console.WriteLine("     5 -- L1-regularized L2-loss support vector classification");
            Console.WriteLine("     6 -- L1-regularized logistic regression");
            Console.WriteLine("     7 -- L2-regularized logistic regression (dual)");
            Console.WriteLine("  for regression");
            Console.WriteLine("    11 -- L2-regularized L2-loss support vector regression (primal)");
            Console.WriteLine("    12 -- L2-regularized L2-loss support vector regression (dual)");
            Console.WriteLine("    13 -- L2-regularized L1-loss support vector regression (dual)");
            Console.WriteLine("-c cost : set the parameter C (default 1)");
            Console.WriteLine("-p epsilon : set the epsilon in loss function of SVR (default 0.1)");
            Console.WriteLine("-e epsilon : set tolerance of termination criterion");
            Console.WriteLine("    -s 0 and 2");
            Console.WriteLine("        |f'(w)|_2 <= eps*min(pos,neg)/l*|f'(w0)|_2,");
            Console.WriteLine("        where f is the primal function and pos/neg are # of");
            Console.WriteLine("        positive/negative data (default 0.01)");
            Console.WriteLine("    -s 11");
            Console.WriteLine("        |f'(w)|_2 <= eps*|f'(w0)|_2 (default 0.001)");
            Console.WriteLine("    -s 1, 3, 4, and 7");
            Console.WriteLine("        Dual maximal violation <= eps; similar to libsvm (default 0.1)");
            Console.WriteLine("    -s 5 and 6");
            Console.WriteLine("        |f'(w)|_1 <= eps*min(pos,neg)/l*|f'(w0)|_1,");
            Console.WriteLine("        where f is the primal function (default 0.01)");
            Console.WriteLine("    -s 12 and 13");
            Console.WriteLine("        |f'(alpha)|_1 <= eps |f'(alpha0)|,");
            Console.WriteLine("        where f is the dual function (default 0.1)");
            Console.WriteLine("-B bias : if bias >= 0, instance x becomes [x; bias]; if < 0, no bias term added (default -1)");
            Console.WriteLine("-wi weight: weights adjust the parameter C of different classes (see README for details)");
            Console.WriteLine("-v n: n-fold cross validation mode");
            Console.WriteLine("-q : quiet mode (no outputs)");
            Environment.Exit(1);
        }



        public static Parameters parse_command_line(string[] args,
            out string input_file_name, out string model_file_name)
        {
            // default values
            var parameters = new Parameters()
            {
                Solver = LibSvmSolverType.L2RegularizedL1LossSvcDual,
                Complexity = 1,
                Tolerance = Double.PositiveInfinity,
                Epsilon = 0.1,
                Bias = -1
            };

            input_file_name = null;
            model_file_name = null;

            int i;

            try
            {
                // parse options
                for (i = 0; i < args.Length; i++)
                {
                    if (args[i][0] != '-')
                        break;

                    if (++i >= args.Length)
                        exit_with_help();

                    switch (args[i - 1][1])
                    {
                        case 's':
                            parameters.Solver = (LibSvmSolverType)Int32.Parse(args[i]);
                            break;

                        case 'c':
                            parameters.Complexity = Double.Parse(args[i]);
                            break;

                        case 'p':
                            parameters.Epsilon = Double.Parse(args[i]);
                            break;

                        case 'e':
                            parameters.Tolerance = Double.Parse(args[i]);
                            break;

                        case 'B':
                            parameters.Bias = Double.Parse(args[i]);
                            break;

                        case 'w':
                            parameters.ClassLabels.Add(Int32.Parse(args[i - 1][2].ToString()));
                            parameters.ClassWeights.Add(Double.Parse(args[i]));
                            break;

                        case 'v':
                            parameters.CrossValidation = true;
                            parameters.ValidationFolds = Int32.Parse(args[i]);
                            if (parameters.ValidationFolds < 2)
                            {
                                Console.WriteLine("n-fold cross validation: n must >= 2");
                                exit_with_help();
                            }
                            break;

                        case 'q':
                            Trace.Listeners.Clear();
                            i--;
                            break;

                        default:
                            Console.WriteLine("unknown option: -%c" + args[i - 1][1]);
                            exit_with_help();
                            break;
                    }
                }

                // determine filenames
                if (i >= args.Length)
                    exit_with_help();

                input_file_name = args[i];

                if (i < args.Length - 1)
                    model_file_name = args[i + 1];
            }
            catch
            {
                exit_with_help();
            }

            if (parameters.Tolerance == Double.PositiveInfinity)
            {
                switch (parameters.Solver)
                {
                    case LibSvmSolverType.L2RegularizedLogisticRegression:
                    case LibSvmSolverType.L2RegularizedL2LossSvc:
                        parameters.Tolerance = 0.01;
                        break;
                    case LibSvmSolverType.L2RegularizedL2LossSvr:
                        parameters.Tolerance = 0.001;
                        break;
                    case LibSvmSolverType.L2RegularizedL2LossSvcDual:
                    case LibSvmSolverType.L2RegularizedL1LossSvcDual:
                    case LibSvmSolverType.MulticlassSvmCrammerSinger:
                    case LibSvmSolverType.L2RegularizedLogisticRegressionDual:
                        parameters.Tolerance = 0.1;
                        break;
                    case LibSvmSolverType.L1RegularizedL2LossSvc:
                    case LibSvmSolverType.L1RegularizedLogisticRegression:
                        parameters.Tolerance = 0.01;
                        break;
                    case LibSvmSolverType.L2RegularizedL1LossSvrDual:
                    case LibSvmSolverType.L2RegularizedL2LossSvrDual:
                        parameters.Tolerance = 0.1;
                        break;
                }
            }

            return parameters;
        }

        /// <summary>
        ///   Reads a problem specified in LibSVM's sparse format.
        /// </summary>
        /// 
        public static Problem read_problem(string filename, double bias)
        {
            // Create a LibSVM's sparse data reader
            var reader = new SparseReader(filename);

            if (bias > 0)
                reader.Intercept = bias;

            var r = reader.ReadDenseToEnd();
            double[][] x = r.Item1;
            double[] y = r.Item2;

            return new Problem()
            {
                Dimensions = reader.Dimensions,
                Inputs = x,
                Outputs = y,
            };
        }

        public static string check_parameter(Problem prob, Parameters param)
        {
            if (param.Epsilon <= 0)
                return "eps <= 0";

            if (param.Complexity <= 0)
                return "C <= 0";

            if (param.Epsilon < 0)
                return "p < 0";

            if (!Enum.IsDefined(typeof(LibSvmSolverType), param.Solver))
                return "unknown solver type";

            if (param.CrossValidation)
                return "cross-validation is not supported at this time.";

            return null;
        }

        public static LibSvmModel train(Problem prob, Parameters parameters)
        {
            double[] w;
            double Cp = parameters.Complexity;
            double Cn = parameters.Complexity;

            if (parameters.ClassWeights != null)
            {
                for (int i = 0; i < parameters.ClassLabels.Count; i++)
                {
                    if (parameters.ClassLabels[i] == -1)
                        Cn *= parameters.ClassWeights[i];

                    else if (parameters.ClassLabels[i] == +1)
                        Cn *= parameters.ClassWeights[i];
                }
            }

            train_one(prob, parameters, out w, Cp, Cn);

            return new LibSvmModel()
            {
                Dimension = prob.Dimensions,
                Classes = 2,
                Labels = new[] { +1, -1 },
                Solver = parameters.Solver,
                Weights = w,
                Bias = 0
            };
        }

        public static void train_one(Problem prob, Parameters param, out double[] w, double Cp, double Cn)
        {
            double[][] inputs = prob.Inputs;
            int[] labels = prob.Outputs.Apply(x => x >= 0 ? 1 : -1);

            double eps = param.Tolerance;

            int pos = 0;
            for (int i = 0; i < labels.Length; i++)
                if (labels[i] >= 0) pos++;
            int neg = prob.Outputs.Length - pos;

            double primal_solver_tol = eps * Math.Max(Math.Min(pos, neg), 1.0) / prob.Inputs.Length;

            SupportVectorMachine svm = new SupportVectorMachine(prob.Dimensions);
            ISupportVectorMachineLearning teacher = null;


            switch (param.Solver)
            {
                case LibSvmSolverType.L2RegularizedLogisticRegression:

                    // l2r_lr_fun
                    teacher = new ProbabilisticNewtonMethod(svm, inputs, labels)
                    {
                        PositiveWeight = Cp,
                        NegativeWeight = Cn,
                        Tolerance = primal_solver_tol
                    }; break;


                case LibSvmSolverType.L2RegularizedL2LossSvc:

                    // fun_obj=new l2r_l2_svc_fun(prob, C);
                    teacher = new LinearNewtonMethod(svm, inputs, labels)
                    {
                        PositiveWeight = Cp,
                        NegativeWeight = Cn,
                        Tolerance = primal_solver_tol
                    }; break;


                case LibSvmSolverType.L2RegularizedL2LossSvcDual:

                    // solve_l2r_l1l2_svc(prob, w, eps, Cp, Cn, L2R_L2LOSS_SVC_DUAL);
                    teacher = new LinearDualCoordinateDescent(svm, inputs, labels)
                    {
                        Loss = Loss.L2,
                        PositiveWeight = Cp,
                        NegativeWeight = Cn,
                    }; break;


                case LibSvmSolverType.L2RegularizedL1LossSvcDual:

                    // solve_l2r_l1l2_svc(prob, w, eps, Cp, Cn, L2R_L1LOSS_SVC_DUAL);
                    teacher = new LinearDualCoordinateDescent(svm, inputs, labels)
                    {
                        Loss = Loss.L1,
                        PositiveWeight = Cp,
                        NegativeWeight = Cn,
                    }; break;


                case LibSvmSolverType.L1RegularizedLogisticRegression:

                    // solve_l1r_lr(&prob_col, w, primal_solver_tol, Cp, Cn);
                    teacher = new ProbabilisticCoordinateDescent(svm, inputs, labels)
                    {
                        PositiveWeight = Cp,
                        NegativeWeight = Cn,
                        Tolerance = primal_solver_tol
                    }; break;


                case LibSvmSolverType.L2RegularizedLogisticRegressionDual:

                    // solve_l2r_lr_dual(prob, w, eps, Cp, Cn);
                    teacher = new ProbabilisticDualCoordinateDescent(svm, inputs, labels)
                    {
                        PositiveWeight = Cp,
                        NegativeWeight = Cn,
                        Tolerance = primal_solver_tol,
                    }; break;
            }


            Trace.WriteLine("Training " + param.Solver);
            
            // run the learning algorithm
            var sw = Stopwatch.StartNew();
            double error = teacher.Run();
            sw.Stop();

            // save the solution
            w = svm.ToWeights();

            Trace.WriteLine(String.Format("Finished {0}: {1} in {2}", 
                param.Solver, error, sw.Elapsed));
        }




        private static void ExecuteTestAndExit(string[] args)
        {
            string[] commands = 
            {
                "-s 0 -c 4.0 -e 1e-06 Resources/a9a.train ../../Results/L2R_LR_a9a.txt",
                "-s 7 -c 4.0 -e 1e-06 Resources/a9a.train ../../Results/L1R_LR_DUAL_a9a.txt"
            };

            foreach (string cmd in commands)
                Main(cmd.Split(' '));

            //Console.ReadLine();
            Environment.Exit(0);
        }

    }
}
