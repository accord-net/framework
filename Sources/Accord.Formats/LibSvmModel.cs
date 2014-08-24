// Accord Statistics Library
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

namespace Accord.IO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Links;

    /// <summary>
    ///   Solver types allowed in LibSVM/Liblinear model files.
    /// </summary>
    /// 
    public enum LibSvmSolverType
    {
        /// <summary>
        ///   Unknown solver type.
        /// </summary>
        /// 
        [Description("Unknown")]
        Unknown = -1,

        /// <summary>
        ///   L2-regularized logistic regression in the primal (-s 0, L2R_LR).
        /// </summary>
        /// 
        /// <seealso cref="ProbabilisticNewtonMethod"/>
        /// 
        [Description("L2R_LR")]
        L2RegularizedLogisticRegression = 0,

        /// <summary>
        ///   L2-regularized L2-loss support vector classification
        ///   in the dual (-s 1, L2R_L2LOSS_SVC_DUAL, the default).
        /// </summary>
        /// 
        /// <seealso cref="LinearCoordinateDescent"/>
        /// 
        [Description("L2R_L2LOSS_SVC_DUAL")]
        L2RegularizedL2LossSvcDual = 1,

        /// <summary>
        ///   L2-regularized L2-loss support vector classification
        ///   in the primal (-s 2, L2R_L2LOSS_SVC).
        /// </summary>
        /// 
        /// <seealso cref="ProbabilisticNewtonMethod"/>
        /// 
        [Description("L2R_L2LOSS_SVC")]
        L2RegularizedL2LossSvc = 2,

        /// <summary>
        ///   L2-regularized L1-loss support vector classification
        ///   in the dual (-s 3, L2R_L1LOSS_SVC_DUAL).
        /// </summary>
        /// 
        /// <seealso cref="LinearCoordinateDescent"/>
        /// 
        [Description("L2R_L1LOSS_SVC_DUAL")]
        L2RegularizedL1LossSvcDual = 3,

        /// <summary>
        ///   Support vector classification by 
        ///   Crammer and Singer (-s 4, MCSVM_CS).
        /// </summary>
        /// 
        [Description("MCSVM_CS")]
        MulticlassSvmCrammerSinger = 4,

        /// <summary>
        ///   L1-regularized L2-loss support vector 
        ///   classification (-s 5, L1R_L2LOSS_SVC).
        /// </summary>
        /// 
        [Description("L1R_L2LOSS_SVC")]
        L1RegularizedL2LossSvc = 5,

        /// <summary>
        ///   L1-regularized logistic regression (-s 6, L1R_LR).
        /// </summary>
        /// 
        /// <seealso cref="ProbabilisticCoordinateDescent"/>
        /// 
        [Description("L1R_LR")]
        L1RegularizedLogisticRegression = 6,

        /// <summary>
        ///   L2-regularized logistic regression in the dual (-s 7, L2R_LR_DUAL).
        /// </summary>
        /// 
        /// <seealso cref="ProbabilisticDualCoordinateDescent"/>
        /// 
        [Description("L2R_LR_DUAL")]
        L2RegularizedLogisticRegressionDual = 7,

        /// <summary>
        ///   L2-regularized L2-loss support vector regression 
        ///   in the primal (-s 11, L2R_L2LOSS_SVR).
        /// </summary>
        /// 
        [Description("L2R_L2LOSS_SVR")]
        L2RegularizedL2LossSvr = 11,

        /// <summary>
        ///   L2-regularized L2-loss support vector regression 
        ///   in the dual (-s 12, L2R_L2LOSS_SVR_DUAL).
        /// </summary>
        /// 
        [Description("L2R_L2LOSS_SVR_DUAL")]
        L2RegularizedL2LossSvrDual = 12,

        /// <summary>
        ///   L2-regularized L1-loss support vector regression 
        ///   in the dual (-s 13, L2R_L1LOSS_SVR_DUAL).
        /// </summary>
        /// 
        [Description("L2R_L1LOSS_SVR_DUAL")]
        L2RegularizedL1LossSvrDual = 13
    }

    /// <summary>
    ///   Reads <see cref="SupportVectorMachine">support vector machines</see>
    ///   created from LibSVM or Liblinear. Not all solver types are supported.
    /// </summary>
    /// 
    public class LibSvmModel
    {

        private LibSvmSolverType type;
        private Dictionary<string, LibSvmSolverType> map;

        /// <summary>
        ///   Gets or sets the solver type used to create the model.
        /// </summary>
        /// 
        public LibSvmSolverType Solver
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        ///   Gets or sets the number of classes that
        ///   this classification model can handle.
        /// </summary>
        /// 
        public int Classes { get; set; }

        /// <summary>
        ///   Gets or sets whether an initial double value should
        ///   be appended in the beginning of every feature vector.
        ///   If set to a negative number, this functionality is
        ///   disabled. Default is 0.
        /// </summary>
        /// 
        public double Bias { get; set; }

        /// <summary>
        ///   Gets or sets the number of dimensions (features) 
        ///   the classification or regression model can handle.
        /// </summary>
        /// 
        public int Dimension { get; set; }

        /// <summary>
        ///   Gets or sets the class label for each class
        ///   this classification model expects to handle.
        /// </summary>
        /// 
        public int[] Labels { get; set; }

        /// <summary>
        ///   Gets or sets the vector of linear weights used
        ///   by this model, if it is a compact model. If this
        ///   is not a compact model, this will be set to <c>null</c>.
        /// </summary>
        /// 
        /// <seealso cref="SupportVectorMachine.IsCompact"/>
        /// 
        public double[] Weights { get; set; }

        /// <summary>
        ///   Gets or sets the set of support vectors used
        ///   by this model. If the model is compact, this
        ///   will be set to <c>null</c>.
        /// </summary>
        /// 
        /// <seealso cref="SupportVectorMachine.IsCompact"/>
        /// 
        public double[][] Vectors { get; set; }


        /// <summary>
        ///   Creates a new <see cref="LibSvmModel"/> object.
        /// </summary>
        /// 
        public LibSvmModel()
        {
            map = new Dictionary<string, LibSvmSolverType>();
            foreach (LibSvmSolverType v in Enum.GetValues(typeof(LibSvmSolverType)))
                map.Add(v.GetDescription().ToUpperInvariant(), v);
        }

        /// <summary>
        ///   Creates a <see cref="SupportVectorMachine"/> that
        ///   attends the requisites specified in this model.
        /// </summary>
        /// 
        /// <returns>A <see cref="SupportVectorMachine"/> that represents this model.</returns>
        /// 
        public SupportVectorMachine CreateMachine()
        {
            switch (type)
            {
                case LibSvmSolverType.Unknown:
                    break;

                case LibSvmSolverType.L2RegularizedLogisticRegression:
                case LibSvmSolverType.L1RegularizedLogisticRegression:
                case LibSvmSolverType.L2RegularizedLogisticRegressionDual:
                    {
                        var svm = SupportVectorMachine.FromWeights(Weights);
                        svm.Link = new LogLinkFunction();
                        return svm;
                    }
                case LibSvmSolverType.L2RegularizedL2LossSvc:
                case LibSvmSolverType.L2RegularizedL1LossSvcDual:
                case LibSvmSolverType.L2RegularizedL2LossSvcDual:
                case LibSvmSolverType.L1RegularizedL2LossSvc:
                case LibSvmSolverType.L2RegularizedL2LossSvr:
                case LibSvmSolverType.L2RegularizedL2LossSvrDual:
                case LibSvmSolverType.L2RegularizedL1LossSvrDual:
                    {
                        return SupportVectorMachine.FromWeights(Weights);
                    }
            }

            throw new NotSupportedException("This solver type is unknown or not supported.");
        }

        /// <summary>
        ///   Creates a <see cref="ISupportVectorMachineLearning"> support
        ///   vector machine learning algorithm</see> that attends the 
        ///   requisites specified in this model.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="ISupportVectorMachineLearning"/> that represents this model.
        /// </returns>
        /// 
        public ISupportVectorMachineLearning CreateAlgorithm(double[][] inputs, int[] outputs)
        {
            var machine = CreateMachine();

            switch (type)
            {
                case LibSvmSolverType.L2RegularizedLogisticRegression:
                    return new ProbabilisticNewtonMethod(machine, inputs, outputs);

                case LibSvmSolverType.L1RegularizedLogisticRegression:
                    return new ProbabilisticCoordinateDescent(machine, inputs, outputs);

                case LibSvmSolverType.L2RegularizedLogisticRegressionDual:
                    return new ProbabilisticDualCoordinateDescent(machine, inputs, outputs);

                case LibSvmSolverType.L2RegularizedL2LossSvc:
                    return new LinearNewtonMethod(machine, inputs, outputs);

                case LibSvmSolverType.L2RegularizedL1LossSvcDual:
                    return new LinearCoordinateDescent(machine, inputs, outputs) { Loss = Loss.L1 };

                case LibSvmSolverType.L2RegularizedL2LossSvcDual:
                    return new LinearCoordinateDescent(machine, inputs, outputs) { Loss = Loss.L2 };

                case LibSvmSolverType.L1RegularizedL2LossSvc:
                case LibSvmSolverType.L2RegularizedL2LossSvr:
                case LibSvmSolverType.L2RegularizedL2LossSvrDual:
                case LibSvmSolverType.L2RegularizedL1LossSvrDual:
                    break;
            }

            throw new NotSupportedException("This solver type is unknown or not supported.");
        }


        /// <summary>
        ///   Saves this model to disk using LibSVM's model format.
        /// </summary>
        /// 
        /// <param name="path">The path where the file should be written.</param>
        /// 
        public void Save(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                Save(stream);
        }

        /// <summary>
        ///   Saves this model to disk using LibSVM's model format.
        /// </summary>
        /// 
        /// <param name="stream">The stream where the file should be written.</param>
        /// 
        public void Save(Stream stream)
        {
            StreamWriter writer = new StreamWriter(stream);

            writer.WriteLine("solver_type " + Solver.GetDescription().ToUpperInvariant());
            writer.WriteLine("nr_class " + Classes);

            writer.Write("label");
            for (int i = 0; i < Labels.Length; i++)
                writer.Write(" " + Labels[i]);
            writer.WriteLine();

            writer.WriteLine("nr_feature " + Dimension);
            writer.WriteLine("bias " + Bias.ToString("G17", CultureInfo.InvariantCulture));
            writer.WriteLine("w");

            for (int i = 0; i < Weights.Length; i++)
                writer.WriteLine(Weights[i].ToString("G17", CultureInfo.InvariantCulture) + " ");

            writer.Flush();
        }

        /// <summary>
        ///   Loads a model specified using LibSVM's model format from disk.
        /// </summary>
        /// 
        /// <param name="path">The file path from where the model should be loaded.</param>
        /// 
        /// <returns>The <see cref="LibSvmModel"/> stored on <paramref name="path"/>.</returns>
        /// 
        public static LibSvmModel Load(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return Load(stream);
        }

        /// <summary>
        ///   Loads a model specified using LibSVM's model format from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from where the model should be loaded.</param>
        /// 
        /// <returns>The <see cref="LibSvmModel"/> stored on <paramref name="stream"/>.</returns>
        /// 
        public static LibSvmModel Load(Stream stream)
        {
            LibSvmModel model = new LibSvmModel();

            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] words = line.Split(' ');

                    if (words[0] == "solver_type")
                    {
                        string key = words[1].ToUpperInvariant();
                        if (!model.map.TryGetValue(key, out model.type))
                            model.type = LibSvmSolverType.Unknown;
                    }

                    else if (words[0] == "nr_class")
                        model.Classes = Int32.Parse(words[1]);

                    else if (words[0] == "nr_feature")
                        model.Dimension = Int32.Parse(words[1]);

                    else if (words[0] == "bias")
                        model.Bias = Double.Parse(words[1], CultureInfo.InvariantCulture);

                    else if (words[0] == "w")
                        break;

                    else if (words[0] == "label")
                    {
                        model.Labels = new int[words.Length - 1];
                        for (int i = 1; i < words.Length; i++)
                            model.Labels[i - 1] = Int32.Parse(words[i]);
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine("Unknown field: " + words[0]);
                    }
                }

                List<double> weights = new List<double>();

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    weights.Add(Double.Parse(line, CultureInfo.InvariantCulture));
                }

                model.Weights = weights.ToArray();
            }

            return model;
        }




    }
}
