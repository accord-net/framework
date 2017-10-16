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
    using System.IO;

    /// <summary>
    ///   Nursery Dataset.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Nursery Database was derived from a hierarchical decision model originally developed to 
    ///   rank applications for nursery schools. It was used during several years in 1980's when there
    ///   was excessive enrollment to these schools in Ljubljana, Slovenia, and the rejected applications 
    ///   frequently needed an objective explanation. The final decision depended on three subproblems:
    ///   occupation of parents and child's nursery, family structure and financial standing, and social
    ///   and health picture of the family. The model was developed within expert system shell for decision
    ///   making DEX (M. Bohanec, V. Rajkovic: Expert system for decision making. Sistemica 1(1), pp. 145-157, 1990.). </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://rexa.info/paper/97376fa474590bb12af93ddee9b23120f8854446">
    ///       M. Olave, V. Rajkovic, M. Bohanec: An application for admission in public school systems. 
    ///       In (I. Th. M. Snellen and W. B. H. J. van de Donk and J.-P. Baquiast, editors) Expert Systems 
    ///       in Public Administration, pages 145-160. Elsevier Science Publishers (North Holland), 1989. </a>
    ///       </description></item>
    ///     <item><description><a href="http://rexa.info/paper/0f23f96c4a89bbb221a151f5db381924c17a6eaa">
    ///       B. Zupan, M. Bohanec, I. Bratko, J. Demsar: Machine learning by function decomposition.ICML-97,
    ///       Nashville, TN. 1997.</a>
    ///       </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    public class Nursery : WebDataSet
    {
        /// <summary>
        ///   Gets the data instances contained in Fisher's Iris flower dataset.
        /// </summary>
        /// 
        public int[][] Instances { get; private set; }

        /// <summary>
        ///   Gets the class labels associated with each <see cref="Instances">instance</see>
        ///   in the Nursery dataset.
        /// </summary>
        /// 
        public int[] ClassLabels { get; private set; }

        /// <summary>
        ///   Gets the class labels in the Nursery dataset:
        ///   "not_recom", "recommend", "very_recom", "priority", "spec_prior".
        /// </summary>
        /// 
        public string[] ClassNames { get; private set; }

        /// <summary>
        ///   Gets the variable names in the Nursery dataset.
        /// </summary>
        /// 
        public OrderedDictionary<string, string[]> VariableNames { get; private set; }

        /// <summary>
        ///   Prepares the Nursery dataset.
        /// </summary>
        /// 
        public Nursery(string path = null)
            : base(path)
        {
            ClassNames = new[] { "not_recom", "recommend", "very_recom", "priority", "spec_prior" };

            VariableNames = new OrderedDictionary<string, string[]>
            {
                { "parents",     new[] { "usual", "pretentious", "great_pret" } },
                { "has_nurs",    new[] { "proper", "less_proper", "improper", "critical", "very_crit" }},
                { "form",        new[] { "complete", "completed", "incomplete", "foster" } },
                { "children",    new[] { "1", "2", "3", "more"}},
                { "housing",     new[] { "convenient", "less_conv", "critical" }},
                { "finance",     new[] { "convenient", "inconv"}},
                { "social",      new[] { "nonprob", "slightly_prob", "problematic"}},
                { "health",      new[] { "recommended", "priority", "not_recom"}},
            };

            string uncompressedFileName;
            // Download("https://archive.ics.uci.edu/ml/machine-learning-databases/nursery/nursery.data", Path, out uncompressedFileName);
            Download("https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21530794&authkey=AObheTbGp_LOi-U", Path, "nursery.data", out uncompressedFileName);

                     string text = File.ReadAllText(uncompressedFileName);
            string[] lines = text.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length != 12960)
                throw new InvalidOperationException("The downloaded dataset definition file has a different number of lines than expected ({0} instead of expected 12960).".Format(lines.Length));

            Instances = new int[lines.Length][];
            ClassLabels = new int[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] words = lines[i].Split(',');

                if (words.Length != 9)
                    throw new InvalidOperationException("The downloaded dataset definition file has a different number of columns than expected ({0} instead of expected 9 at line {1}).".Format(words.Length, i));

                Instances[i] = new int[words.Length - 1];
                for (int j = 0; j < Instances[i].Length; j++)
                {
                    int index = Array.IndexOf(VariableNames.GetValueByIndex(j), words[j]);
                    if (index < 0)
                        throw new InvalidOperationException("Unexpected input value at row {0}, column {1}".Format(i, j));
                    Instances[i][j] = index;
                }

                ClassLabels[i] = Array.IndexOf(ClassNames, words[words.Length - 1]);
                if (ClassLabels[i] < 0)
                    throw new InvalidOperationException("Unexpected output value at row {0}".Format(i));
            }
        }

    }
}
