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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Accord.Statistics.Filters;

    public class DecisionExpression
    {
        public DecisionRule Owner { get; private set; }

        public DecisionVariable Variable { get { return Owner.Variables[Index]; } }

        int Index { get; set; }

        ComparisonKind Comparison { get; set; }

        double Value { get; set; }

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

        public DecisionExpression(DecisionRule owner, int index, ComparisonKind comparison, double value)
        {
            this.Owner = owner;
            this.Index = index;
            this.Comparison = comparison;
            this.Value = value;
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
