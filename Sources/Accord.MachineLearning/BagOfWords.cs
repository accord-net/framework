// Accord Machine Learning Library
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

namespace Accord.MachineLearning
{
    using System;
    using System.Collections.Generic;
    using Accord.Math;

    /// <summary>
    ///   Common interface for Bag of Words objects.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the element to be 
    /// converted to a fixed-length vector representation.</typeparam>
    /// 
    public interface IBagOfWords<T>
    {
        /// <summary>
        ///   Gets the number of words in this codebook.
        /// </summary>
        /// 
        int NumberOfWords { get; }

        /// <summary>
        ///   Gets the codeword representation of a given value.
        /// </summary>
        /// 
        /// <param name="value">The value to be processed.</param>
        /// 
        /// <returns>A double vector with the same length as words
        /// in the code book.</returns>
        /// 
        double[] GetFeatureVector(T value);
    }

    /// <summary>
    ///   Bag of words.
    /// </summary>
    /// 
    /// <remarks>
    ///   The bag-of-words (BoW) model can be used to extract finite
    ///   length features from otherwise varying length representations.
    /// </remarks>
    /// 
    [Serializable]
    public class BagOfWords : IBagOfWords<string[]>
    {

        private Dictionary<string, int> stringToCode;
        private ReadOnlyDictionary<string, int> readOnlyStringToCode;

        private Dictionary<int, string> codeToString;
        private ReadOnlyDictionary<int, string> readOnlyCodeToString;


        /// <summary>
        ///   Gets the number of words in this codebook.
        /// </summary>
        /// 
        public int NumberOfWords { get { return stringToCode.Count; } }

        /// <summary>
        ///   Gets the forward dictionary which translates
        ///   string tokens to integer labels.
        /// </summary>
        /// 
        public ReadOnlyDictionary<string, int> StringToCode
        {
            get { return readOnlyStringToCode; }
        }

        /// <summary>
        ///   Gets the reverse dictionary which translates
        ///   integer labels into string tokens.
        /// </summary>
        /// 
        public ReadOnlyDictionary<int, string> CodeToString
        {
            get { return readOnlyCodeToString; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of occurrences of a word which
        ///   should be registered in the feature vector. Default is 1 (if a
        ///   word occurs, corresponding feature is set to 1).
        /// </summary>
        /// 
        public int MaximumOccurance { get; set; }

        /// <summary>
        ///   Constructs a new <see cref="BagOfWords"/>.
        /// </summary>
        /// 
        /// <param name="texts">The texts to build the bag of words model from.</param>
        /// 
        public BagOfWords(params string[][] texts)
        {
            if (texts == null)
                throw new ArgumentNullException("texts");

            initialize(texts);
        }

        /// <summary>
        ///   Constructs a new <see cref="BagOfWords"/>.
        /// </summary>
        /// 
        /// <param name="texts">The texts to build the bag of words model from.</param>
        /// 
        public BagOfWords(params string[] texts)
        {
            if (texts == null)
                throw new ArgumentNullException("texts");

            initialize(new[] { texts });
        }

        /// <summary>
        ///   Constructs a new <see cref="BagOfWords"/>.
        /// </summary>
        /// 
        public BagOfWords()
        {
            initialize(null);
        }

        private void initialize(string[][] texts)
        {
            stringToCode = new Dictionary<string, int>();
            readOnlyStringToCode = new ReadOnlyDictionary<string, int>(stringToCode);

            codeToString = new Dictionary<int, string>();
            readOnlyCodeToString = new ReadOnlyDictionary<int, string>(codeToString);

            MaximumOccurance = 1;

            if (texts != null)
                Compute(texts);
        }


        /// <summary>
        ///   Computes the Bag of Words model.
        /// </summary>
        /// 
        public void Compute(string[][] texts)
        {
            checkArgs(texts);


            int symbol = 0;
            foreach (string[] text in texts)
            {
                foreach (string word in text)
                {
                    if (!stringToCode.ContainsKey(word))
                    {
                        stringToCode[word] = symbol;
                        codeToString[symbol] = word;
                        symbol++;
                    }
                }
            }
        }

        private static void checkArgs(string[][] texts)
        {
            if (texts == null)
                throw new ArgumentNullException("texts");

            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i] == null)
                    throw new ArgumentNullException("texts",
                        "Text " + i + "cannot be null.");

                for (int j = 0; j < texts[i].Length; j++)
                    if (texts[i][j] == null)
                        throw new ArgumentNullException("texts",
                            "Token at text " + i + ", position " + i + " cannot be null.");
            }
        }

        /// <summary>
        ///   Gets the codeword representation of a given text.
        /// </summary>
        /// 
        /// <param name="text">The text to be processed.</param>
        /// 
        /// <returns>An integer vector with the same length as words
        /// in the code book.</returns>
        /// 
        public int[] GetFeatureVector(params string[] text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            int[] features = new int[NumberOfWords];

            // Detect all feature words
            foreach (string word in text)
            {
                int j;

                if (!stringToCode.TryGetValue(word, out j))
                    continue;

                features[j]++;

                if (features[j] > MaximumOccurance)
                    features[j] = MaximumOccurance;
            }

            return features;
        }


        double[] IBagOfWords<string[]>.GetFeatureVector(string[] value)
        {
            return GetFeatureVector(value).ToDouble();
        }
    }
}
