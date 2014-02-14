// Accord Statistics Library
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

namespace Accord.Statistics.Models.Fields.Learning
{

    /// <summary>
    ///   Common interface for Conditional Random Fields learning algorithms.
    /// </summary>
    /// 
    public interface IConditionalRandomFieldLearning<T>
    {
        /// <summary>
        ///   Runs the learning algorithm with the specified input
        ///   training observations and corresponding output labels.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="labels">The observation's labels.</param>
        /// 
        double Run(T[][] observations, int[][] labels);
    }
}
