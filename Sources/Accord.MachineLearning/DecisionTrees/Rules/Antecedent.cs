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

namespace Accord.MachineLearning.DecisionTrees.Rules
{
    using System;
    using Accord.Statistics.Filters;

    /// <summary>
    ///   Antecedent expression for <see cref="DecisionRule"/>s.
    /// </summary>
    /// 
    public struct Antecedent : IEquatable<Antecedent>
    {
        private int index;
        private ComparisonKind comparison;
        private double value;

        /// <summary>
        ///   Gets the index of the variable used as the
        ///   left hand side term of this expression.
        /// </summary>
        /// 
        public int Index { get { return index; } }

        /// <summary>
        ///   Gets the comparison being made between the variable
        ///   value at <see cref="Index"/> and <see cref="Value"/>.
        /// </summary>
        /// 
        public ComparisonKind Comparison { get { return comparison; } }

        /// <summary>
        ///   Gets the right hand side of this expression.
        /// </summary>
        /// 
        public double Value { get { return value; } }



        /// <summary>
        ///   Creates a new instance of the <see cref="Antecedent"/> class.
        /// </summary>
        /// 
        /// <param name="index">The variable index.</param>
        /// <param name="comparison">The comparison to be made using the value at 
        ///   <paramref name="index"/> and <paramref name="value"/>.</param>
        /// <param name="value">The value to be compared against.</param>
        /// 
        public Antecedent(int index, ComparisonKind comparison, double value)
        {
            this.index = index;
            this.comparison = comparison;
            this.value = value;
        }



        /// <summary>
        ///   Checks if this antecedent applies to a given input.
        /// </summary>
        /// 
        /// <param name="input">An input vector.</param>
        /// 
        /// <returns>True if the input element at position <see cref="Index"/>
        ///    compares to <see cref="Value"/> using <see cref="Comparison"/>; false
        ///    otherwise.
        /// </returns>
        /// 
        public bool Match(double[] input)
        {
            double x = input[Index];

            switch (Comparison)
            {
                case ComparisonKind.Equal:
                    return (x == Value);

                case ComparisonKind.GreaterThan:
                    return (x > Value);

                case ComparisonKind.GreaterThanOrEqual:
                    return (x >= Value);

                case ComparisonKind.LessThan:
                    return (x < Value);

                case ComparisonKind.LessThanOrEqual:
                    return (x <= Value);

                case ComparisonKind.NotEqual:
                    return (x != Value);

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        ///   Determines whether the specified <see cref="Antecedent"/>
        ///   is equal to this instance.
        /// </summary>
        /// 
        /// <param name="other">The <see cref="Antecedent"/> to compare with this instance.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the specified <see cref="Antecedent"/>
        ///   is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public bool Equals(Antecedent other)
        {
            if (other == null)
                return false;

            if (Comparison != other.Comparison)
                return false;

            if (Index != other.Index)
                return false;

            if (Value != other.Value)
                return false;

            return true;
        }

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object"/>
        ///   is equal to this instance.
        /// </summary>
        /// 
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/>
        ///   is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public override bool Equals(object obj)
        {
            if (!(obj is Antecedent))
                return false;

            return this.Equals((Antecedent)obj);
        }

        /// <summary>
        ///    Returns a hash code for this instance.
        /// </summary>
        /// 
        /// <returns>
        ///    A hash code for this instance, suitable for use in
        ///    hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// 
        public override int GetHashCode()
        {
            return Comparison.GetHashCode() +
                13 * Index.GetHashCode() +
                13 * Value.GetHashCode();
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            string op = ComparisonExtensions.ToString(Comparison);
            return String.Format("x[{0}] {1} {2}", Index, op, Value);
        }


        /// <summary>
        ///   Implements the operator ==.
        /// </summary>
        /// 
        public static bool operator ==(Antecedent a, Antecedent b)
        {
            if ((object)a == null && (object)b == null)
                return true;

            if ((object)a != null)
                return a.Equals(b);

            return b.Equals(a);
        }

        /// <summary>
        ///   Implements the operator !=.
        /// </summary>
        /// 
        public static bool operator !=(Antecedent a, Antecedent b)
        {
            if ((object)a == null && (object)b == null)
                return false;

            if ((object)a != null)
                return !a.Equals(b);

            return !b.Equals(a);
        }

    }
}
