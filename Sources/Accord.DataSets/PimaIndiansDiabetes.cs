// Accord Datasets Library
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

namespace Accord.DataSets
{
    using Accord.Collections;
    using Accord.DataSets.Base;
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    /// <summary>
    ///   Pima Indians Diabetes Data Set.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Pima Indians Diabetes dataset has been created by the National Institute of Diabetes 
    ///   and Digestive and Kidney Diseases. It has been donated to the UCI's Machine Learning Repository
    ///   by Vincent Sigillito (vgs@aplcen.apl.jhu.edu), from the Applied Physics Laboratory, Johns Hopkins 
    ///   University in 9 May 1990.</para>
    /// <para>
    ///   From the dataset description: "The diagnostic, binary-valued variable investigated is whether the 
    ///   patient shows signs of diabetes according to World Health Organization criteria (i.e., if the 2-hour 
    ///   post-load plasma glucose was at least 200 mg/dl at any survey examination or if found during routine 
    ///   medical care). The population lives near Phoenix, Arizona, USA.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://archive.ics.uci.edu/ml/datasets/pima+indians+diabetes">
    ///       Lichman, M. "UCI Machine Learning Repository - Pima Indians Diabetes Data Set."
    ///       Irvine, CA: University of California, School of Information and Computer Science, 2013</a>
    ///       </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    public class PimaIndiansDiabetes : WebDataSet
    {
        /// <summary>
        ///   Gets the data instances contained in the Pima Indians Diabetes dataset.
        /// </summary>
        /// 
        public double[][] Instances { get; private set; }

        /// <summary>
        ///   Gets the class labels associated with each <see cref="Instances">instance</see>
        ///   in the Pima Indians Diabetes dataset.
        /// </summary>
        /// 
        public int[] ClassLabels { get; private set; }

        /// <summary>
        ///   Gets the class labels in the Pima Indians Diabetes dataset:
        ///   "tested-negative-for-diabetes", "tested-positive-for-diabetes"
        /// </summary>
        /// 
        public string[] ClassNames { get; private set; }

        /// <summary>
        ///   Gets the variable names in the Pima Indians Diabetes dataset.
        /// </summary>
        /// 
        public Dictionary<string, string> VariableNames { get; private set; }

        /// <summary>
        ///   Prepares the Pima Indians Diabetes dataset.
        /// </summary>
        /// 
        public PimaIndiansDiabetes(string path = null)
            : base(path)
        {
            ClassNames = new[] { "tested-negative-for-diabetes", "tested-positive-for-diabetes" };

            VariableNames = new Dictionary<string, string>
            {
                { "Pregnant", "Number of times pregnant"                                                 },
                { "Glucose",  "Plasma glucose concentration a 2 hours in an oral glucose tolerance test" },
                { "Pressure", "Diastolic blood pressure (mm Hg)"                                         },
                { "Triceps",  "Triceps skin fold thickness (mm)"                                         },
                { "Insulin",  "2-Hour serum insulin (mu U/ml)"                                           },
                { "BMI",      "Body mass index (weight in kg/(height in m)^2)"                           },
                { "Pedigree", "Diabetes pedigree function"                                               },
                { "Age",      "Age (years)"                                                              },
            };

            string uncompressedFileName;
            Download("https://archive.ics.uci.edu/ml/machine-learning-databases/pima-indians-diabetes/pima-indians-diabetes.data", Path, out uncompressedFileName);

            string text = File.ReadAllText(uncompressedFileName);
            string[] lines = text.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length != 768)
                throw new InvalidOperationException("The downloaded dataset definition file has a different number of lines than expected ({0} instead of expected 12960).".Format(lines.Length));

            Instances = new double[lines.Length][];
            ClassLabels = new int[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] words = lines[i].Split(',');

                if (words.Length != 9)
                    throw new InvalidOperationException("The downloaded dataset definition file has a different number of columns than expected ({0} instead of expected 9 at line {1}).".Format(words.Length, i));

                Instances[i] = new double[words.Length - 1];
                for (int j = 0; j < Instances[i].Length; j++)
                    Instances[i][j] = double.Parse(words[j], CultureInfo.InvariantCulture);

                ClassLabels[i] = int.Parse(words[words.Length - 1]);
            }
        }

    }
}

