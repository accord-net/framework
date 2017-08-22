// Accord Statistics Library
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

namespace Accord.Statistics.Moving
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Accord.Compat;

    /// <summary>
    ///   Moving-window statistics.
    /// </summary>
    /// 
    [Serializable]
    public class MovingRangeStatistics : IMoving<double>, IEnumerable<double>
    {
        private Queue<double> values;

        /// <summary>
        ///   Gets the size of the window.
        /// </summary>
        /// 
        /// <value>The window's size.</value>
        /// 
        public int Window { get; private set; }

        /// <summary>
        ///   Gets the number of samples within the window.
        /// </summary>
        /// 
        /// <value>The number of samples within the window.</value>
        /// 
        public int Count { get { return values.Count; } }

        /// <summary>
        ///   Gets the minimum value in the window.
        /// </summary>
        /// 
        public double Min { get; private set; }

        /// <summary>
        ///   Gets the maximum value in the window.
        /// </summary>
        /// 
        public double Max { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="MovingNormalStatistics"/> class.
        /// </summary>
        /// 
        /// <param name="windowSize">The size of the moving window.</param>
        /// 
        public MovingRangeStatistics(int windowSize)
        {
            if (windowSize < 0 || windowSize == int.MaxValue)
                throw new ArgumentOutOfRangeException("windowSize");

            Window = windowSize;
            values = new Queue<double>(windowSize + 1);
        }

        /// <summary>
        ///   Pushes a value into the window.
        /// </summary>
        /// 
        public void Push(double value)
        {
            if (values.Count == Window)
            {
                values.Dequeue();
            }

            values.Enqueue(value);

            // TODO: Rewrite to avoid iterating on every addition
            Min = value;
            Max = value;
            foreach (double v in values)
            {
                if (v < Min)
                    Min = v;
                if (v > Max)
                    Max = v;
            }
        }


        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<double> GetEnumerator()
        {
            return values.GetEnumerator();
        }


        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }


        /// <summary>
        ///   Removes all elements from the window and resets statistics.
        /// </summary>
        /// 
        public void Clear()
        {
            values.Clear();
            Min = Max = 0;
        }
    }
}
