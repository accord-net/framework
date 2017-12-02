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
    using Accord.DataSets.Base;
    using Accord.IO;
    using Accord.Compat;
    using System;

    /// <summary>
    ///   Pendigits data set.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Iris_flower_data_set">
    ///       Wikipedia contributors. "Iris flower data set." Wikipedia, The Free Encyclopedia. 
    ///       Wikipedia, The Free Encyclopedia, 14 Nov. 2016. Web. 14 Nov. 2016. </a>
    ///       </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="Accord.DataSets.SparseIris" />
    /// 
    public class Pendigits : WebDataSet
    {
        /// <summary>
        /// Gets the training set of the Pendigits dataset.
        /// </summary>
        /// 
        public Tuple<double[][][], int[]> Training { get; private set; }

        /// <summary>
        /// Gets the testing set of the Pendigits dataset.
        /// </summary>
        /// 
        public Tuple<double[][][], int[]> Testing { get; private set; }

        /// <summary>
        ///   Gets the names of the classes contained in the Pendigits dataset.
        /// </summary>
        /// 
        public string[] ClassNames { get; private set; }

        /// <summary>
        ///   Gets the names of the variables contained in the Pendigits dataset.
        /// </summary>
        /// 
        public string[] VariableNames { get; private set; }

        /// <summary>
        ///   Gets the number of classes in this dataset (10).
        /// </summary>
        /// 
        /// <value>Returns 10 (since there are 10 digits).</value>
        /// 
        public int NumberOfClasses { get { return ClassNames.Length; } }

        /// <summary>
        ///   Downloads and prepares the Pendigits dataset.
        /// </summary>
        /// 
        public Pendigits(string path = null)
            : base(path)
        {
            // https://archive.ics.uci.edu/ml/machine-learning-databases/pendigits/pendigits-orig.tra.Z => https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21530792&authkey=APIFw0rzzh3OuQM
            // https://archive.ics.uci.edu/ml/machine-learning-databases/pendigits/pendigits-orig.tes.Z => https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21530791&authkey=AEBfVB3d3bil-oU

            // Training = download("https://archive.ics.uci.edu/ml/machine-learning-databases/pendigits/pendigits-orig.tes.Z");
            // Testing = download("https://archive.ics.uci.edu/ml/machine-learning-databases/pendigits/pendigits-orig.tra.Z");

            Training = download("https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21530792&authkey=APIFw0rzzh3OuQM", "pendigits-orig.tes.Z");
            Testing = download("https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21530791&authkey=AEBfVB3d3bil-oU", "pendigits-orig.tra.Z");
        }

        private Tuple<double[][][], int[]> download(string url, string localName)
        {
            this.VariableNames = new[] { "X", "Y" };

            string uncompressedFileName;
            Download(url, Path, localName, out uncompressedFileName);

            using (var reader = new UnipenReader(uncompressedFileName, compressed: false))
            {
                this.ClassNames = reader.Lexicon;

                var tuple = reader.ReadToEnd();
                double[][][] sequences = tuple.Item1;
                int[] classLabels = new int[tuple.Item2.Length];
                for (int i = 0; i < tuple.Item2.Length; i++)
                    classLabels[i] = int.Parse(tuple.Item2[i]);

                Accord.Diagnostics.Debug.Assert(sequences.Length == classLabels.Length, "Number of input and output rows should match.");
                return Tuple.Create(sequences, classLabels);
            }
        }

    }
}
