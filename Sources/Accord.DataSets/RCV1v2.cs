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
    ///   RCV1-v2/LYRL2004, a text categorization test collection originally 
    ///   distributed as a set of on-line appendices to a JMLR journal article.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://jmlr.csail.mit.edu/papers/volume5/lewis04a/lyrl2004_rcv1v2_README.htm">
    ///       Lewis, D. D.  RCV1-v2/LYRL2004: The LYRL2004 Distribution of the RCV1-v2 Text Categorization
    ///       Test Collection (5-Mar-2015 Version). http://www.jmlr.org/papers/volume5/lewis04a/lyrl2004_rcv1v2_README.htm.  </a>
    ///       </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public class RCV1v2 : SparseDataSet
    {
        string[] classes;
        Dictionary<string, HashSet<int>> classLabelToDocument;
        Dictionary<int, HashSet<string>> documentToClassLabel;
        Dictionary<int, string[]> documents;

        Tuple<string[][], string[][]> training;
        Tuple<string[][], string[][]> testing;

        /// <summary>
        ///   Gets the document class labels in this dataset.
        /// </summary>
        /// 
        public string[] Classes { get { return classes; } }

        /// <summary>
        ///   Gets a mapping giving a list of all document labels 
        ///   that have been labelled with a given class label.
        /// </summary>
        /// 
        public Dictionary<string, HashSet<int>> ClassLabelToDocument { get { return classLabelToDocument; } }

        /// <summary>
        ///   Gets a mapping giving a list of all class labels that
        ///   a document has been labelled with.
        /// </summary>
        /// 
        public Dictionary<int, HashSet<string>> DocumentToClassLabel { get { return documentToClassLabel; } }

        /// <summary>
        ///   Gets a mapping of a document identifier and all the words it contains.
        /// </summary>
        /// 
        public Dictionary<int, string[]> Documents { get { return documents; } }

        /// <summary>
        /// Gets the training set of the RCV1-v2 dataset.
        /// </summary>
        /// 
        public Tuple<string[][], string[][]> Training { get { return training; } }

        /// <summary>
        /// Gets the testiong set of the RCV1-v2 dataset.
        /// </summary>
        /// 
        public Tuple<string[][], string[][]> Testing { get { return testing; } }

        /// <summary>
        ///   Downloads and prepares the RCV1-v2 dataset.
        /// </summary>
        /// 
        /// <param name="path">The path where datasets will be stored. If null or empty, the dataset
        ///   will be saved on a subfolder called "data" in the current working directory.</param>
        /// <param name="downloadTrainingSet">Pass true to download the training set; false otherwise.</param>
        /// <param name="downloadTestingSet">Pass true to download the testing set; false otherwise.</param>
        /// 
        public RCV1v2(string path = null, bool downloadTrainingSet = true, bool downloadTestingSet = true)
            : base(path)
        {
            classLabelToDocument = new Dictionary<string, HashSet<int>>();
            documentToClassLabel = new Dictionary<int, HashSet<string>>();
            documents = new Dictionary<int, string[]>();

            readClasses();

            if (downloadTrainingSet)
            {
                this.training = readData(new Dictionary<string, string>
                {
                    // "http://jmlr.csail.mit.edu/papers/volume5/lewis04a/a12-token-files/lyrl2004_tokens_train.dat.gz"
                    { "https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21529127&authkey=AF9sB4hwJ050wWI", "lyrl2004_tokens_train.dat.gz" }
                    // { "https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21529120&authkey=AEJar_-6N0UQ7MY", "lyrl2004_vectors_train.dat.gz" }
                });
            }

            if (downloadTestingSet)
            {
                this.testing = readData(new Dictionary<string, string>
                {
                    // "http://jmlr.csail.mit.edu/papers/volume5/lewis04a/a12-token-files/lyrl2004_tokens_test_pt0.dat.gz",
                    // "http://jmlr.csail.mit.edu/papers/volume5/lewis04a/a12-token-files/lyrl2004_tokens_test_pt1.dat.gz",
                    // "http://jmlr.csail.mit.edu/papers/volume5/lewis04a/a12-token-files/lyrl2004_tokens_test_pt2.dat.gz",
                    // "http://jmlr.csail.mit.edu/papers/volume5/lewis04a/a12-token-files/lyrl2004_tokens_test_pt3.dat.gz",

                    { "https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21529130&authkey=AGdQKsNwmddL8yo", "lyrl2004_tokens_test_pt0.dat.gz" },
                    { "https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21529131&authkey=AH1m8AN6j5Z6-js", "lyrl2004_tokens_test_pt1.dat.gz" },
                    { "https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21529129&authkey=ANJ63koY7A3iYBU", "lyrl2004_tokens_test_pt2.dat.gz" },
                    { "https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21529128&authkey=AO8CdcH64kYnLAc", "lyrl2004_tokens_test_pt3.dat.gz" },

                    //{ "https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21529122&authkey=AIe7fmy1mHKNkwg", "lyrl2004_vectors_test_pt0.dat.gz" },
                    //{ "https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21529123&authkey=AE9VdmBSjRpPRoE", "lyrl2004_vectors_test_pt1.dat.gz" },
                    //{ "https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21529119&authkey=AKO6aa34fcR19yg", "lyrl2004_vectors_test_pt2.dat.gz" },
                    //{ "https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21529121&authkey=AOOMHflvx58qGEA", "lyrl2004_vectors_test_pt3.dat.gz" },
                });
            }
        }

        private Tuple<string[][], string[][]> readData(Dictionary<string, string> urls)
        {
            var outputList = new List<string[]>();
            var inputList = new List<string[]>();
            var idList = new List<int>();
            int currentId = -1;
            List<string> currentTokens = null;

            string[] categories;
            string[] inputTokens;

            Action register = () =>
            {
                categories = DocumentToClassLabel[currentId].ToArray();
                inputTokens = currentTokens.ToArray();

                idList.Add(currentId);
                inputList.Add(inputTokens);
                outputList.Add(categories);
                Documents[currentId] = inputTokens;
                currentTokens = null;
            };

            foreach (KeyValuePair<string, string> pair in urls)
            {
                string url = pair.Key;
                string fullFileName = System.IO.Path.Combine(Path, pair.Value);
                string train;
                Download(url, Path, fullFileName, out train);

#if NET35
                var lines = File.ReadAllLines(train);
#else
                var lines = File.ReadLines(train);
#endif

                foreach (string line in lines)
                {
                    if (line.StartsWith(".I "))
                    {
                        if (currentTokens != null)
                            register();

                        currentId = Int32.Parse(line.Substring(".I ".Length));
                    }
                    else if (line.StartsWith(".W"))
                    {
                        currentTokens = new List<string>();
                    }
                    else if (line.Length > 0)
                    {
                        currentTokens.AddRange(line.Split(' '));
                    }
                }

                // Save the last line, if any
                if (currentTokens != null)
                    register();
            }


            //int[] idx = new int[idList.Count];
            //for (int i = 0; i < idx.Length; i++)
            //    idx[i] = i;

            //Array.Sort(idList.ToArray(), idx);

            //var outputs = new string[outputList.Count][];
            //for (int i = 0; i < outputs.Length; i++)
            //    outputs[i] = outputList[idx[i]];

            //var inputs = new string[inputList.Count][];
            //for (int i = 0; i < inputs.Length; i++)
            //    inputs[i] = inputList[idx[i]];

            var inputs = inputList.ToArray();
            var outputs = outputList.ToArray();

            return Tuple.Create(inputs, outputs);
        }

        private void readClasses()
        {
            string classesPath;
            //Download("http://jmlr.csail.mit.edu/papers/volume5/lewis04a/a08-topic-qrels/rcv1-v2.topics.qrels.gz", Path, out classesPath);
            Download("https://onedrive.live.com/download?cid=808347681CC09388&resid=808347681CC09388%21529118&authkey=AJrVXqi2QpwaHKQ", Path, "rcv1-v2.topics.qrels.gz", out classesPath);

#if NET35
            var lines = File.ReadAllLines(classesPath);
#else
            var lines = File.ReadLines(classesPath);
#endif

            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');
                string topic = parts[0];
                int label = Int32.Parse(parts[1]);

                if (!ClassLabelToDocument.ContainsKey(topic))
                    ClassLabelToDocument[topic] = new HashSet<int>();

                if (!DocumentToClassLabel.ContainsKey(label))
                    DocumentToClassLabel[label] = new HashSet<string>();

                ClassLabelToDocument[topic].Add(label);
                DocumentToClassLabel[label].Add(topic);
            }

            this.classes = ClassLabelToDocument.Keys.OrderBy(x => x).ToArray();
        }


    }
}
