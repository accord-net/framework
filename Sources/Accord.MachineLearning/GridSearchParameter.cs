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

namespace Accord.MachineLearning
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    ///   Contains the name and value of a parameter that should be used during fitting.
    /// </summary>
    /// 
    [Serializable]
    public struct GridSearchParameter
    {
        private string name;
        private double value;

        /// <summary>
        ///   Gets the name of the parameter
        /// </summary>
        /// 
        public string Name { get { return name; } }

        /// <summary>
        ///   Gets the value of the parameter.
        /// </summary>
        /// 
        public double Value { get { return value; } }

        /// <summary>
        ///   Constructs a new parameter.
        /// </summary>
        /// 
        /// <param name="name">The name for the parameter.</param>
        /// <param name="value">The value for the parameter.</param>
        /// 
        public GridSearchParameter(string name, double value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        ///   Determines whether the specified object is equal
        ///   to the current GridSearchParameter object.
        /// </summary>
        /// 
        public override bool Equals(object obj)
        {
            if (obj is GridSearchParameter)
            {
                GridSearchParameter g = (GridSearchParameter)obj;
                if (g.name != name || g.value != value)
                    return false;
                return true;
            }
            return false;
        }

        /// <summary>
        ///   Returns the hash code for this GridSearchParameter
        /// </summary>
        /// 
        public override int GetHashCode()
        {
            return name.GetHashCode() ^ value.GetHashCode();
        }

        /// <summary>
        ///   Compares two GridSearchParameters for equality.
        /// </summary>
        /// 
        public static bool operator ==(GridSearchParameter parameter1, GridSearchParameter parameter2)
        {
            return (parameter1.name == parameter2.name && parameter1.value == parameter2.value);
        }

        /// <summary>
        ///   Compares two GridSearchParameters for inequality.
        /// </summary>
        /// 
        public static bool operator !=(GridSearchParameter parameter1, GridSearchParameter parameter2)
        {
            return (parameter1.name != parameter2.name || parameter1.value != parameter2.value);
        }
    }

    /// <summary>
    ///   Grid search parameter collection.
    /// </summary>
    /// 
    [Serializable]
    public class GridSearchParameterCollection : KeyedCollection<string, GridSearchParameter>
    {
        /// <summary>
        ///   Constructs a new collection of GridsearchParameter objects.
        /// </summary>
        /// 
        public GridSearchParameterCollection(params GridSearchParameter[] parameters)
        {
            foreach (GridSearchParameter param in parameters)
                this.Add(param);
        }

        /// <summary>
        ///   Constructs a new collection of GridsearchParameter objects.
        /// </summary>
        /// 
        public GridSearchParameterCollection(IEnumerable<GridSearchParameter> parameters)
        {
            foreach (GridSearchParameter param in parameters)
                this.Add(param);
        }

        /// <summary>
        ///   Returns the identifying value for an item on this collection.
        /// </summary>
        /// 
        protected override string GetKeyForItem(GridSearchParameter item)
        {
            return item.Name;
        }
    }

}
