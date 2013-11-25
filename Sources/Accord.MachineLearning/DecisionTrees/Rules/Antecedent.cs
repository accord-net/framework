// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2013
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
    public class Antecedent : IEquatable<Antecedent>, IComparable<Antecedent>, IComparable
    {
        /// <summary>
        ///   Gets the <see cref="DecisionRule"/> that contains this antecedent.
        /// </summary>
        /// 
        public DecisionRule Owner { get; private set; }

        /// <summary>
        ///   Gets the index of the variable used as the
        ///   left hand side term of this expression.
        /// </summary>
        /// 
        public int Index { get; private set; }

        /// <summary>
        ///   Gets the comparison being made between the variable
        ///   value at <see cref="Index"/> and <see cref="Value"/>.
        /// </summary>
        /// 
        public ComparisonKind Comparison { get; private set; }

        /// <summary>
        ///   Gets the right hand side of this expression.
        /// </summary>
        /// 
        public double Value { get; private set; }


        /// <summary>
        ///   Gets the <see cref="DecisionVariable"/> being
        ///   handled by this <see cref="Antecedent"/>.
        /// </summary>
        /// 
        public DecisionVariable Variable { get { return Owner.Variables[Index]; } }



        /// <summary>
        ///   Creates a new instance of the <see cref="Antecedent"/> class.
        /// </summary>
        /// 
        /// <param name="owner">The <see cref="DecisionRule"/> to whom this antecedent will belong.</param>
        /// <param name="index">The variable index.</param>
        /// <param name="comparison">The comparison to be made using the value at 
        ///   <paramref name="index"/> and <paramref name="value"/>.</param>
        /// <param name="value">The value to be compared against.</param>
        /// 
        public Antecedent(DecisionRule owner, int index, ComparisonKind comparison, double value)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            if (index < 0 || index >= owner.Variables.Count)
                throw new ArgumentOutOfRangeException("index");

            this.Owner = owner;
            this.Index = index;
            this.Comparison = comparison;
            this.Value = value;
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
            return this.Equals(obj as Antecedent);
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
        ///   Compares the current instance with another object of 
        ///   the same type and returns an integer that indicates
        ///   whether the current instance precedes, follows, or 
        ///   occurs in the same position in the sort order as the
        ///   other object.
        /// </summary>
        /// 
        /// <param name="other">An object to compare with this instance.</param>
        /// 
        public int CompareTo(Antecedent other)
        {
            return Index.CompareTo(other.Index);
        }

        /// <summary>
        ///   Compares the current instance with another object of 
        ///   the same type and returns an integer that indicates
        ///   whether the current instance precedes, follows, or 
        ///   occurs in the same position in the sort order as the
        ///   other object.
        /// </summary>
        /// 
        /// <param name="obj">An object to compare with this instance.</param>
        /// 
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="obj"/> is not the same type as this instance. </exception>
        ///   
        public int CompareTo(object obj)
        {
            return CompareTo(obj as Antecedent);
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
            return toString(null);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(Codification codebook)
        {
            return toString(codebook);
        }


        /// <summary>
        ///   Implements the operator &gt;.
        /// </summary>
        /// 
        public static bool operator >(Antecedent a, Antecedent b)
        {
            return a.Index > b.Index;
        }

        /// <summary>
        ///   Implements the operator &lt;.
        /// </summary>
        /// 
        public static bool operator <(Antecedent a, Antecedent b)
        {
            return a.Index < b.Index;
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


        private string toString(Codification codebook)
        {
            String name = Variable.Name;

            if (String.IsNullOrEmpty(name))
                name = "x" + Index;

            String op = ComparisonExtensions.ToString(Comparison);

            String value;
            if (codebook != null && codebook.Columns.Contains(name))
                value = codebook.Translate(name, (int)Value);

            else value = Value.ToString();

            return String.Format("{0} {1} {2}", name, op, value);
        }

    }
}
