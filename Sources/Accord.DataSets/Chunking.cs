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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Accord.Compat;

    /// <summary>
    ///   Chunking data set used in CoNLL-2000's shared task session. This is 
    ///   a part-of-speech (POS) tagging dataset created from the WSJ corpus. 
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://www.cnts.ua.ac.be/conll2000/chunking/">
    ///       CoNLL-2000 Organizers. "Chunking." Conference on Computational Natural Language Learning (CoNLL-2000). </a>
    ///       </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    public class Chunking : WebDataSet
    {
        /// <summary>
        /// Gets the training set of the Chunking dataset.
        /// </summary>
        /// 
        public Tuple<string[][], string[][]> Training { get; private set; }

        /// <summary>
        /// Gets the testing set of the Chunking dataset.
        /// </summary>
        /// 
        public Tuple<string[][], string[][]> Testing { get; private set; }

        /// <summary>
        ///   Gets the vocabulary of all words used in the dataset.
        /// </summary>
        /// 
        public string[] Words { get; private set; }

        /// <summary>
        ///   Gets the vocabulary of all words used in training set of the dataset.
        /// </summary>
        /// 
        public string[] TrainingWords { get; private set; }

        /// <summary>
        ///   Gets the vocabulary of all words used in testing set of the dataset.
        /// </summary>
        /// 
        public string[] TestingWords { get; private set; }

        /// <summary>
        ///   Gets the list of all tags used in this dataset.
        /// </summary>
        /// 
        public string[] Tags { get; private set; }


        /// <summary>
        ///   Downloads and prepares the Chunking dataset.
        /// </summary>
        /// 
        public Chunking(string path = null)
            : base(path)
        {
            //Training = download("http://www.cnts.ua.ac.be/conll2000/chunking/train.txt.gz");
            Training = download("https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21529599&authkey=AEZPMjdmJutTF3k", "train.txt.gz");

            //Testing = download("http://www.cnts.ua.ac.be/conll2000/chunking/test.txt.gz");
            Testing = download("https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21529598&authkey=AJ9SOABWAcm0FRY", "test.txt.gz");

            var trainWords = unique(Training.Item1);
            var testWords = unique(Testing.Item1);

            this.TrainingWords = trainWords.ToArray();
            this.TestingWords = testWords.ToArray();

            trainWords.UnionWith(testWords);
            this.Words = trainWords.ToArray();

            var tags = unique(Training.Item2);
            this.Tags = tags.ToArray();
        }

        private static SortedSet<string> unique(string[][] set)
        {
            var voc = new SortedSet<string>();
            foreach (string[] sequence in set)
                foreach (string word in sequence)
                    voc.Add(word);
            return voc;
        }

        private Tuple<string[][], string[][]> download(string url, string localFileName)
        {
            string uncompressedFileName;
            Download(url, Path, localFileName, out uncompressedFileName);

            var sentences = new List<string[]>();
            var tags = new List<string[]>();

            var currentWords = new List<string>();
            var currentTags = new List<string>();

#if NET35
            foreach (string line in File.ReadAllLines(uncompressedFileName))
            {
                if (line == null || String.IsNullOrEmpty(line.Trim()))
                {
#else
            foreach (string line in File.ReadLines(uncompressedFileName))
            {
                if (String.IsNullOrWhiteSpace(line))
                {
#endif
                    sentences.Add(currentWords.ToArray());
                    tags.Add(currentTags.ToArray());

                    currentWords.Clear();
                    currentTags.Clear();
                    continue;
                }

                string[] parts = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                currentWords.Add(parts[0]);
                currentTags.Add(parts[1]);
            }

            return Tuple.Create(sentences.ToArray(), tags.ToArray());
        }

    }
}
