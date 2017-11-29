// Accord Machine Learning Library
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

namespace Accord.Text
{
    using Accord.Math.Random;
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    /// <summary>
    ///   Set of machine learning tools.
    /// </summary>
    /// 
    public static partial class ExtensionMethods
    {
        /// <summary>
        ///   Splits the given text into individual atomic words, 
        ///   irrespective of punctuation and other marks.
        /// </summary>
        /// 
        public static string[][] Tokenize(this string[] x)
        {
            var r = new string[x.Length][];
            for (int i = 0; i < x.Length; i++)
                r[i] = Tokenize(x[i]);
            return r;
        }

        /// <summary>
        ///   Splits the given text into individual atomic words, 
        ///   irrespective of punctuation and other marks.
        /// </summary>
        /// 
        public static string[] Tokenize(this string x)
        {
            string s = Regex.Replace(x, @"[^\w]", " ");
            string[] words = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
                words[i] = words[i].ToLowerInvariant();
            return words;
        }
    }
}