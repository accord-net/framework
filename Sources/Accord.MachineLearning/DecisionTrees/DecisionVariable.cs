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

namespace Accord.MachineLearning.DecisionTrees
{
    using Accord.Statistics.Filters;
    using AForge;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    ///   Attribute category.
    /// </summary>
    /// 
    public enum DecisionVariableKind
    {
        /// <summary>
        ///   Attribute is discrete-valued.
        /// </summary>
        /// 
        Discrete,

        /// <summary>
        ///   Attribute is continuous-valued.
        /// </summary>
        /// 
        Continuous
    }


    /// <summary>
    ///   Decision attribute.
    /// </summary>
    /// 
    [Serializable]
    public class DecisionVariable
    {
        /// <summary>
        ///   Gets the name of the attribute.
        /// </summary>
        /// 
        public string Name { get; private set; }

        /// <summary>
        ///   Gets the nature of the attribute (i.e. real-valued or discrete-valued).
        /// </summary>
        /// 
        public DecisionVariableKind Nature { get; private set; }

        /// <summary>
        ///   Gets the valid range of the attribute.
        /// </summary>
        /// 
        public DoubleRange Range { get; private set; }


        /// <summary>
        ///   Creates a new <see cref="DecisionVariable"/>.
        /// </summary>
        /// 
        /// <param name="name">The name of the attribute.</param>
        /// <param name="range">The range of valid values for this attribute. Default is [0;1].</param>
        /// 
        public DecisionVariable(string name, DoubleRange range)
        {
            this.Name = name;
            this.Nature = DecisionVariableKind.Continuous;
            this.Range = range;
        }

        /// <summary>
        ///   Creates a new <see cref="DecisionVariable"/>.
        /// </summary>
        /// 
        /// <param name="name">The name of the attribute.</param>
        /// <param name="nature">The attribute's nature (i.e. real-valued or discrete-valued).</param>
        /// 
        public DecisionVariable(string name, DecisionVariableKind nature)
        {
            this.Name = name;
            this.Nature = nature;
            this.Range = new DoubleRange(0, 1);
        }

        /// <summary>
        ///   Creates a new <see cref="DecisionVariable"/>.
        /// </summary>
        /// 
        /// <param name="name">The name of the attribute.</param>
        /// <param name="range">The range of valid values for this attribute.</param>
        /// 
        public DecisionVariable(string name, IntRange range)
        {
            this.Name = name;
            this.Nature = DecisionVariableKind.Discrete;
            this.Range = new DoubleRange(range.Min, range.Max);
        }

        /// <summary>
        ///   Creates a new discrete-valued <see cref="DecisionVariable"/>.
        /// </summary>
        /// 
        /// <param name="name">The name of the attribute.</param>
        /// <param name="symbols">The number of possible values for this attribute.</param>
        /// 
        public DecisionVariable(string name, int symbols)
            : this(name, new IntRange(0, symbols - 1))
        {
        }


        /// <summary>
        ///   Creates a new continuous <see cref="DecisionVariable"/>.
        /// </summary>
        /// 
        /// <param name="name">The name of the attribute.</param>
        /// 
        public static DecisionVariable Continuous(string name)
        {
            return new DecisionVariable(name, DecisionVariableKind.Continuous);
        }

        /// <summary>
        ///   Creates a new continuous <see cref="DecisionVariable"/>.
        /// </summary>
        /// 
        /// <param name="name">The name of the attribute.</param>
        /// <param name="range">The range of valid values for this attribute. Default is [0;1].</param>
        /// 
        public static DecisionVariable Continuous(string name, DoubleRange range)
        {
            return new DecisionVariable(name, range);
        }

        /// <summary>
        ///   Creates a new discrete <see cref="DecisionVariable"/>.
        /// </summary>
        /// 
        /// <param name="name">The name of the attribute.</param>
        /// <param name="range">The range of valid values for this attribute.</param>
        /// 
        public static DecisionVariable Discrete(string name, IntRange range)
        {
            return new DecisionVariable(name, range);
        }

        /// <summary>
        ///   Creates a new discrete-valued <see cref="DecisionVariable"/>.
        /// </summary>
        /// 
        /// <param name="name">The name of the attribute.</param>
        /// <param name="symbols">The number of possible values for this attribute.</param>
        /// 
        public static DecisionVariable Discrete(string name, int symbols)
        {
            return new DecisionVariable(name, symbols);
        }




        /// <summary>
        ///   Creates a set of decision variables from a <see cref="Codification"/> codebook.
        /// </summary>
        /// 
        /// <param name="codebook">The codebook containing information about the variables.</param>
        /// <param name="columns">The columns to consider as decision variables.</param>
        /// 
        /// <returns>An array of <see cref="DecisionVariable"/> objects 
        /// initialized with the values from the codebook.</returns>
        /// 
        public static DecisionVariable[] FromCodebook(Codification codebook, params string[] columns)
        {
            DecisionVariable[] variables = new DecisionVariable[columns.Length];

            for (int i = 0; i < variables.Length; i++)
            {
                string name = columns[i];

                Codification.Options col;

                if (codebook.Columns.TryGetValue(name, out col))
                    variables[i] = new DecisionVariable(name, col.Symbols);
                else
                    variables[i] = new DecisionVariable(name, DecisionVariableKind.Continuous);
            }

            return variables;
        }

    }


    /// <summary>
    ///   Collection of decision attributes.
    /// </summary>
    /// 
    [Serializable]
    public class DecisionVariableCollection : ReadOnlyCollection<DecisionVariable>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="DecisionVariableCollection"/> class.
        /// </summary>
        /// 
        /// <param name="list">The list to initialize the collection.</param>
        /// 
        public DecisionVariableCollection(IList<DecisionVariable> list)
            : base(list) { }
    }
}
