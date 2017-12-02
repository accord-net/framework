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

namespace Accord.MachineLearning
{
    using Accord.Math.Random;
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    /// <summary>
    ///   Set of machine learning tools.
    /// </summary>
    /// 
    public static class Tools
    {
        /// <summary>
        ///   Obsolete. Please use <see cref="Accord.Text.ExtensionMethods.Tokenize(string)"/> instead.
        /// </summary>
        /// 
        [Obsolete("please use Accord.Text.ExtensionMethods.Tokenize(x) instead.")]
        public static string[][] Tokenize(string[] x)
        {
            return Accord.Text.ExtensionMethods.Tokenize(x);
        }

        /// <summary>
        ///   Obsolete. Please use <see cref="Accord.Text.ExtensionMethods.Tokenize(string)"/> instead.
        /// </summary>
        /// 
        [Obsolete("please use Accord.Text.ExtensionMethods.Tokenize(x) instead.")]
        public static string[] Tokenize(string x)
        {
            return Accord.Text.ExtensionMethods.Tokenize(x);
        }
    }
}