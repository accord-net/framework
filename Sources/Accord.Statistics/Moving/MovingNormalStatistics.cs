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

namespace Accord.Statistics.Moving
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    ///   Moving-window statistics.
    /// </summary>
    /// 
    public class MovingNormalStatistics : IMovingStatistics, IEnumerable<double>
    {
        private Queue<double> values;
        private Queue<double> squares;

        /// <summary>
        ///   Gets the sum the values within the window.
        /// </summary>
        /// 
        /// <value>The sum of values within the window.</value>
        /// 
        public double Sum { get; private set; }

        /// <summary>
        ///   Gets the sum of squared values within the window.
        /// </summary>
        /// 
        /// <value>The sum of squared values.</value>
        /// 
        public double SumOfSquares { get; private set; }

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
        ///   Gets the mean of the values within the window.
        /// </summary>
        /// 
        /// <value>The mean of the values.</value>
        /// 
        public double Mean { get; private set; }

        /// <summary>
        ///   Gets the variance of the values within the window.
        /// </summary>
        /// 
        /// <value>The variance of the values.</value>
        /// 
        public double Variance { get; private set; }

        /// <summary>
        ///   Gets the standard deviation of the values within the window.
        /// </summary>
        /// 
        /// <value>The standard deviation of the values.</value>
        /// 
        public double StandardDeviation
        {
            get { return System.Math.Sqrt(Variance); }
        }



        /// <summary>
        ///   Initializes a new instance of the <see cref="MovingNormalStatistics"/> class.
        /// </summary>
        /// 
        /// <param name="windowSize">The size of the moving window.</param>
        /// 
        public MovingNormalStatistics(int windowSize)
        {
            if (windowSize < 0 || windowSize == int.MaxValue)
                throw new ArgumentOutOfRangeException("windowSize");

            Window = windowSize;
            values = new Queue<double>(windowSize + 1);
            squares = new Queue<double>(windowSize + 1);
        }

        /// <summary>
        ///   Pushes a value into the window.
        /// </summary>
        /// 
        public void Push(double value)
        {
            if (values.Count == Window)
            {
                Sum -= values.Dequeue();
                SumOfSquares -= squares.Dequeue();
            }

            double square = value * value;

            values.Enqueue(value);
            squares.Enqueue(square);
            
            Sum += value;
            SumOfSquares += square;

            int N = values.Count;

            Mean = Sum / N;
            Variance = (N * SumOfSquares - (Sum * Sum)) / (N * (N - 1));
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
            squares.Clear();

            Mean = 0;
            Variance = 0;

            SumOfSquares = 0;
            Sum = 0;
        }
    }
}
