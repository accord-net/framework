// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    ///   Pair of class labels.
    /// </summary>
    /// 
    [Serializable]
    public struct ClassPair
    {
        private int class1;
        private int class2;

        /// <summary>
        ///   Gets the first class in the pair.
        /// </summary>
        /// 
        public int Class1 { get { return class1; } }

        /// <summary>
        ///   Gets the second class in the pair.
        /// </summary>
        /// 
        public int Class2 { get { return class2; } }


        /// <summary>
        /// Initializes a new instance of the <see cref="ClassPair"/> struct.
        /// </summary>
        /// <param name="i">The first class index in the pair.</param>
        /// <param name="j">The second class index in the pair.</param>
        /// 
        public ClassPair(int i, int j)
        {
            class1 = i;
            class2 = j;
        }

        /// <summary>
        ///   Converts to a tuple (class_a, class_b).
        /// </summary>
        /// 
        public Tuple<int, int> ToTuple()
        {
            return Tuple.Create(Class1, Class2);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "{0},{1}".Format(Class1, Class2);
        }
    }

    /// <summary>
    ///   Decision between two class labels. Indicates the class index
    ///   of the first class, the class index of the adversary, and the
    ///   class index of the winner.
    /// </summary>
    /// 
    [Serializable]
    public struct Decision
    {
        private ClassPair pair;
        private int winner;

        /// <summary>
        ///   Gets the adversarial classes.
        /// </summary>
        /// 
        public ClassPair Pair { get { return pair; } }

        /// <summary>
        ///   Gets the class label of the winner.
        /// </summary>
        /// 
        public int Winner { get { return winner; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Decision"/> struct.
        /// </summary>
        /// <param name="i">The first class index.</param>
        /// <param name="j">The second class index.</param>
        /// <param name="winner">The class index that won.</param>
        public Decision(int i, int j, int winner)
        {
            if (winner != i && winner != j)
                throw new ArgumentOutOfRangeException("winner");

            this.pair = new ClassPair(i, j);
            this.winner = winner;
        }

        /// <summary>
        ///   Converts to a triplet (class_a, class_b, winner).
        /// </summary>
        /// 
        public Tuple<int, int, int> ToTuple()
        {
            return Tuple.Create(Pair.Class1, Pair.Class2, Winner);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "{0},{1}>{2}".Format(Pair.Class1, Pair.Class2, Winner);
        }
    }
}