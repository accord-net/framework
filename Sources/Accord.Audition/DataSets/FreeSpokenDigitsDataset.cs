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
    using Accord.Audio;
    using System;
    using Accord;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Accord.Compat;

    /// <summary>
    /// Free Spoken Digits Dataset (FSDD)
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Free Spoken Digits Dataset (FSDD) is a simple audio/speech dataset consisting of 
    ///   recordings of spoken digits in wav files at 8kHz. The recordings are trimmed so that 
    ///   they have near minimal silence at the beginnings and ends.</para>
    /// <para>
    ///   FSDD is an open dataset, which means it will grow over time as data is contributed. 
    ///   Thus in order to enable reproducibility and accurate citation in scientific journals
    ///   the dataset is versioned using git tags.</para>
    /// <para>
    ///   The dataset is available under the Creative Commons Attribution-ShareAlike 4.0 International
    ///   license. If you use the dataset in your research, please cite it as indicated in
    ///   <a href="https://zenodo.org/record/1000742">https://zenodo.org/record/1000742</a> </para>
    /// </remarks>
    /// 
    public class FreeSpokenDigitsDataset : IDisposable
    {
        string basePath;
        WebClient webClient;

        /// <summary>
        ///   Gets a list of all recordings in the dataset.
        /// </summary>
        /// 
        public RecordCollection Records { get; private set; }

        /// <summary>
        ///   Gets all records in the training set.
        /// </summary>
        /// 
        public RecordCollection Training { get; private set; }

        /// <summary>
        ///   Gets all records in the testing set.
        /// </summary>
        /// 
        public RecordCollection Testing { get; private set; }

        /// <summary>
        ///   Gets all the digits currently present in the dataset.
        /// </summary>
        /// 
        public SortedSet<int> Digits { get; private set; }

        /// <summary>
        ///   Gets all the speakers currently present in the dataset.
        /// </summary>
        /// 
        public SortedSet<string> Speakers { get; private set; }

        /// <summary>
        ///   Gets all the file names currently present in the dataset.
        /// </summary>
        /// 
        public SortedSet<string> FileNames { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="FreeSpokenDigitsDataset" /> class.
        /// </summary>
        /// 
        /// <param name="path">The path where datasets will be stored. If null or empty, the dataset
        /// will be saved on a subfolder called "data" in the current working directory.</param>
        /// 
        public FreeSpokenDigitsDataset(string path)
        {
            this.basePath = path;
            this.webClient = ExtensionMethods.NewWebClient();
            this.Records = new RecordCollection(this);

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            foreach (string line in GetFileList())
            {
                if (line.Contains("\"path\":"))
                {
                    string name = line.Split(new[] { "path\":" }, StringSplitOptions.None)[1].Trim(' ', '"');
                    string[] parts = name.Replace(".wav", "").Split('_');
                    var record = new Record(
                        digit: int.Parse(parts[0]),
                        speaker: parts[1],
                        index: int.Parse(parts[2]));

                    if (this.Records.Contains(record))
                        throw new Exception();
                    this.Records.Add(record);
                }
            }

            this.Digits = new SortedSet<int>();
            this.Speakers = new SortedSet<string>();
            this.FileNames = new SortedSet<string>();
            foreach (Record r in Records)
            {
                Digits.Add(r.Digit);
                Speakers.Add(r.Speaker);
                FileNames.Add(r.FileName);
            }

            // From README.md: "The test set officially consists of the first 10% of the recordings. 
            // Recordings numbered 0-4 (inclusive) are in the test and 5-49 are in the training set."
            Testing = new RecordCollection(this, Records.Where(x => x.Index >= 0 && x.Index <= 4));
            Training = new RecordCollection(this, Records.Where(x => x.Index >= 5 && x.Index <= 49));
        }

        private string[] GetFileList()
        {
            string fileListUrl = "https://api.github.com/repositories/61622039/git/trees/ee8c3d6b556fd70607ccaae089abad378d4d257c";
            string filePath = Path.Combine(basePath, "filelist.txt");
            webClient.DownloadFileWithRetry(fileListUrl, filePath);
            string s = File.ReadAllText(filePath);
            return s.Split(',');
        }

        /// <summary>
        ///   Retrieves a single record from the dataset.
        /// </summary>
        /// 
        /// <param name="digit">The digit.</param>
        /// <param name="speaker">The speaker.</param>
        /// <param name="index">The index.</param>
        /// 
        public Record GetRecord(int digit, string speaker, int index)
        {
            return this.Records.Where(r => r.Digit == digit && r.Speaker == speaker && r.Index == index).First();
        }

        /// <summary>
        ///   Retrieves a single record from the dataset.
        /// </summary>
        /// 
        /// <param name="digit">The digit.</param>
        /// <param name="speaker">The speaker.</param>
        /// <param name="index">The index.</param>
        /// 
        public Signal GetSignal(int digit, string speaker, int index)
        {
            return GetRecord(digit, speaker, index).Open(new WebClient(), this.basePath);
        }

        /// <summary>
        ///   Retrieves a single record from the dataset.
        /// </summary>
        /// 
        /// <param name="record">A record from the dataset.</param>
        /// 
        public Signal GetSignal(Record record)
        {
            return record.Open(this.webClient, this.basePath);
        }

        /// <summary>
        ///   Retrieves a collection of record from the dataset as <see cref="Signal">audio signals</see>.
        /// </summary>
        /// 
        /// <param name="records">The digit.</param>
        /// 
        public IEnumerable<Signal> GetSignals(IEnumerable<Record> records)
        {
             return records.Select(x => x.Open(this.webClient, this.basePath));
        }



        /// <summary>
        ///   Collection of <see cref="FreeSpokenDigitsDataset"/> records.
        /// </summary>
        /// 
        public class RecordCollection : SortedSet<Record>
        {
            FreeSpokenDigitsDataset dataset;

            internal RecordCollection(FreeSpokenDigitsDataset dataset)
            {
                this.dataset = dataset;
            }

            internal RecordCollection(FreeSpokenDigitsDataset dataset, IEnumerable<Record> collection)
                : base(collection)
            {
                this.dataset = dataset;
            }

            /// <summary>
            ///   Gets the file names of the records in this set.
            /// </summary>
            /// 
            public string[] FileNames
            {
                get { return this.Select(x => x.FileName).ToArray(); }
            }

            /// <summary>
            ///   Gets the local paths of the records in this set. Acessing this
            ///   property will force the records to be downloaded such that the
            ///   local paths will be valid file locations in the local disk.
            /// </summary>
            /// 
            public string[] LocalPaths
            {
                get { return this.Select(x => x.Download(dataset.webClient, dataset.basePath)).ToArray(); }
            }

            /// <summary>
            ///   Gets the <see cref="Signal">audio signals</see> corresponding to the
            ///   records in this set. This will cause all records in this set to be
            ///   downloaded from the Free Spoken Digits Dataset GitHub repository.
            /// </summary>
            /// 
            public Signal[] Signals
            {
                get { return this.Select(x => x.Open(dataset.webClient, dataset.basePath)).ToArray(); }
            }

            /// <summary>
            ///   Gets the digits contained in this set.
            /// </summary>
            /// 
            public int[] Digits
            {
                get { return this.Select(x => x.Digit).ToArray(); }
            }

            /// <summary>
            ///   Gets the <see cref="Record"/> at the specified index.
            /// </summary>
            /// 
            /// <param name="index">The index.</param>
            /// 
            public Record this[int index]
            {
                get { return this.ElementAt(index); }
            }
        }



        /// <summary>
        ///   Single recording from the <see cref="FreeSpokenDigitsDataset"/>
        /// </summary>
        /// 
        [Serializable]
        public struct Record : IEquatable<Record>, IComparable<Record>
        {
            private int digit;
            private string speaker;
            private int index;

            /// <summary>
            ///   Gets or sets the digit being spoken.
            /// </summary>
            /// 
            public int Digit { get { return digit; } }

            /// <summary>
            ///   Gets or sets the name of the speaker.
            /// </summary>
            /// 
            public string Speaker { get { return speaker; } }

            /// <summary>
            ///   Gets or sets the index of the variation of the current <see cref="Digit"/> spoken 
            ///   by the <see cref="Speaker"/> (the same speaker could have recorded the same digit
            ///   multiple times).
            /// </summary>
            /// 
            public int Index { get { return index; } }

            /// <summary>
            ///   Gets or sets the URL of the recording.
            /// </summary>
            /// 
            public string Url { get { return "https://github.com/Jakobovski/free-spoken-digit-dataset/raw/master/recordings/" + FileName; } }

            /// <summary>
            ///   Gets or sets the file name of the recording (without its path).
            /// </summary>
            /// 
            public string FileName { get { return String.Format("{0}_{1}_{2}.wav", digit, speaker, index); } }

            /// <summary>
            ///   Initializes a new instance of the <see cref="Record"/> struct.
            /// </summary>
            /// 
            /// <param name="digit">The digit.</param>
            /// <param name="speaker">The speaker.</param>
            /// <param name="index">The index.</param>
            /// 
            public Record(int digit, string speaker, int index)
            {
                this.digit = digit;
                this.speaker = speaker;
                this.index = index;
            }

            /// <summary>
            ///   Downloads the recording to a base directory. The actual file path
            ///   where the file was stored will be given as return of this method.
            /// </summary>
            /// 
            /// <param name="localPath">The local directory where the recordings should be saved.</param>
            /// 
            /// <returns>The file path where the recording has been saved to.</returns>
            /// 
            public string Download(string localPath)
            {
                return Download(ExtensionMethods.NewWebClient(), localPath);
            }

            /// <summary>
            ///   Opens the recording as a <see cref="Signal"/>, downloading 
            ///   and storing the recording file to the disk if necessary.
            /// </summary>
            /// 
            /// <param name="localPath">The local directory where the recordings should be saved.</param>
            /// 
            /// <returns>A <see cref="Signal"/> containing the audio recording for this entry.</returns>
            /// 
            public Signal Open(string localPath)
            {
                return Open(ExtensionMethods.NewWebClient(), localPath);
            }

            internal string Download(WebClient client, string localPath)
            {
                if (!Directory.Exists(localPath))
                    Directory.CreateDirectory(localPath);

                string localFileName = Path.Combine(localPath, FileName);

                if (!File.Exists(localFileName))
                    client.DownloadFileWithRetry(Url, localFileName);
                return localFileName;
            }

            internal Signal Open(WebClient client, string localPath)
            {
                return Signal.FromFile(Download(ExtensionMethods.NewWebClient(), localPath));
            }


            /// <summary>
            ///   Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// 
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            /// 
            public override string ToString()
            {
                return String.Format("{0} {1} {2} ({3})", Digit, Speaker, Index, FileName);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
            public override int GetHashCode()
            {
                return unchecked(47 * digit + 13 * speaker.GetHashCode() + index);
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
            public bool Equals(Record other)
            {
                return this.digit == other.digit && this.index == other.index && this.speaker == other.speaker;
            }

            /// <summary>
            /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
            /// </summary>
            /// <param name="other">An object to compare with this instance.</param>
            /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows <paramref name="other" /> in the sort order.</returns>
            public int CompareTo(Record other)
            {
                return this.FileName.CompareTo(other.FileName);
            }
        }



        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    this.webClient.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                this.webClient = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FreeSpokenDigitsDataset() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
