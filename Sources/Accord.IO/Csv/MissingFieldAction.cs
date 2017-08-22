// Accord Formats Library
// The Accord.NET Framework
// http://accord-framework.net
//
// LumenWorks.Framework.IO.CSV.CsvReader
// Copyright (c) 2005 Sébastien Lorion
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// This class has been based on the original work by Sébastien Lorion, originally
// published under the MIT license (and thus compatible with the LGPL). Original
// license text is reproduced below:
//
//    MIT license (http://en.wikipedia.org/wiki/MIT_License)
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights 
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//    of the Software, and to permit persons to whom the Software is furnished to do so, 
//    subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all 
//    copies or substantial portions of the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//    INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//    PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
//    FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//    ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Accord.IO
{
    /// <summary>
    ///   Specifies the action to take when a field is missing.
    /// </summary>
    /// 
    public enum MissingFieldAction
    {
        /// <summary>
        ///   Treat as a parsing error.
        /// </summary>
        /// 
        ParseError = 0,

        /// <summary>
        ///   Replaces by an empty value.
        /// </summary>
        /// 
        ReplaceByEmpty = 1,

        /// <summary>
        ///   Replaces by a null value (<see langword="null"/>).
        /// </summary>
        /// 
        ReplaceByNull = 2,
    }
}