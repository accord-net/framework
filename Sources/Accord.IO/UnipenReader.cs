// Accord Formats Library
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

namespace Accord.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Accord.Compat;

    /// <summary>
    ///   Reader for UNIPEN files (such as Pendigits dataset).
    /// </summary>
    /// 
    public class UnipenReader : IDisposable
    {

        private TextReader reader;
        private List<double[]> buffer;

        /// <summary>
        ///   Gets the lexicon information that is usually included at the header of the file.
        ///   This should represent the different class labels that each sample can belong to.
        /// </summary>
        /// 
        public string[] Lexicon { get; private set; }

        /// <summary>
        ///   Gets the hierarchy iniformation that is usually included at the header of the file.
        ///   This usually gives a description of what the <see cref="Lexicon">class labels</see> represent.
        /// </summary>
        /// 
        public string Hierarchy { get; private set; }


        /// <summary>
        ///   Creates a new <see cref="UnipenReader"/>.
        /// </summary>
        /// 
        /// <param name="path">The path for the IDX file.</param>
        /// <param name="compressed">Pass <c>true</c> if the stream contains a compressed (.gz) file. Default is true.</param>
        /// 
        public UnipenReader(string path, bool compressed = true)
        {
            init(new FileStream(path, FileMode.Open, FileAccess.Read), compressed);
        }

        /// <summary>
        ///   Creates a new <see cref="UnipenReader"/>.
        /// </summary>
        /// 
        /// <param name="input">The input stream containing the UNIPEN file.</param>
        /// <param name="compressed">Pass <c>true</c> if the stream contains a compressed (.Z) file. Default is true.</param>
        /// 
        public UnipenReader(Stream input, bool compressed = true)
        {
            init(input, compressed);
        }

        private void init(Stream input, bool compressed = true)
        {
            if (compressed)
                reader = new StreamReader(new Accord.IO.Compression.LzwInputStream(input));
            else
                reader = new StreamReader(input);

            this.buffer = new List<double[]>();

            string line;

            do // Read the lexicon
            {
                line = reader.ReadLine();
            } while (!line.StartsWith(".LEXICON"));

            string[] parts = line.Split(' ');
            Lexicon = new string[parts.Length - 1];
            for (int i = 0; i < Lexicon.Length; i++)
                Lexicon[i] = parts[i + 1].Trim().TrimStart('"').TrimEnd('"');


            do // Read the hierarchy
            {
                line = reader.ReadLine();
            } while (!line.StartsWith(".HIERARCHY"));

            parts = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            Hierarchy = parts[1];
            if (parts.Length != 2)
                throw new FormatException();
        }


        /// <summary>
        ///   Reads the next sequence.
        /// </summary>
        /// 
        /// <param name="label">The class label of the sample that has been read.</param>
        /// <param name="comment">The comment associated with the sample that has just been read.</param>
        /// 
        /// <returns>The sequence of (X,Y) coordinates stored in the file.</returns>
        /// 
        public double[][] Read(out string label, out string comment)
        {
            double[][] coordinates;
            if (TryRead(out coordinates, out label, out comment))
                return coordinates;
            throw new FormatException();
        }

        private bool TryRead(out double[][] coordinates, out string label, out string comment)
        {
            coordinates = null;
            label = null;
            comment = null;

            buffer.Clear();

            string line;

            do // Find the next ".SEGMENT" region
            {
                try
                {
                    line = reader.ReadLine();
                }
                catch
                {
                    line = null; // TODO: This try-catch block is only necessary because of the 
                    // current version of SharpZipLib being used. This block can be removed after
                    // SharpZipLib NuGet's package is finally updated.
                }

                if (line == null)
                    return false;
            } while (!line.StartsWith(".SEGMENT"));

            // Get the label
            label = line.Split('?').Last().Trim().TrimStart('"').TrimEnd('"');
            if (!Lexicon.Contains(label))
                return false;

            // Read .COMMENT line
            comment = reader.ReadLine().Replace(".COMMENT", "").Trim();

            // Read .PEN_DOWN
            line = reader.ReadLine();
            if (line != ".PEN_DOWN")
                return false;

            do // Read until empty line
            {
                line = reader.ReadLine();

                if (line == ".PEN_UP")
                    continue;

                if (line == ".PEN_DOWN")
                    continue;

                if (line.StartsWith(".DT"))
                    break;

                string[] parts = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    return false;

                double x = double.Parse(parts[0]);
                double y = double.Parse(parts[1]);

                buffer.Add(new[] { x, y });
#if NET35
            } while (line != null && String.IsNullOrEmpty(line.Trim()));
#else
            } while (!String.IsNullOrWhiteSpace(line));
#endif

            coordinates = buffer.ToArray();
            return true;
        }

        /// <summary>
        ///   Reads all samples in the file, starting from the current position.
        /// </summary>
        /// 
        /// <returns>
        ///   An array containing all samples from the current point until the end of the stream.
        /// </returns>
        /// 
        public Tuple<double[][][], string[]> ReadToEnd()
        {
            double[][] coordinates;
            string label;
            string comment;

            var vectors = new List<double[][]>();
            var labels = new List<string>();

            while (TryRead(out coordinates, out label, out comment))
            {
                vectors.Add(coordinates);
                labels.Add(label);
            }

            return Tuple.Create(vectors.ToArray(), labels.ToArray());
        }



        #region IDisposable members
        /// <summary>
        ///   Performs application-defined tasks associated with
        ///   freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// 
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged
        ///   resources; <c>false</c> to release only unmanaged resources.</param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (reader != null)
                {
#if !NETSTANDARD1_4
                    reader.Close();
#endif
                    reader = null;
                }
            }
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref="IdxReader"/> is reclaimed by garbage collection.
        /// </summary>
        /// 
        ~UnipenReader()
        {
            Dispose(false);
        }
        #endregion


    }
}
