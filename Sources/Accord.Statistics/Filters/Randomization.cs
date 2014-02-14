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

namespace Accord.Statistics.Filters
{
    using System;
    using System.Data;
    using Accord.Math;

    /// <summary>
    ///   Randomization filter.
    /// </summary>
    /// 
    [Serializable]
    public class Randomization : IFilter
    {


        /// <summary>
        ///   Gets or sets the fixed random seed to
        ///   be used in randomization, if any.
        /// </summary>
        /// 
        /// <value>The random seed, for fixed permutations;
        /// or null, for true random permutations.</value>
        /// 
        public int? Seed { get; set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="Randomization"/> class.
        /// </summary>
        /// 
        /// <param name="seed">A fixed random seed value to generate fixed
        /// permutations. If not specified, generates true random permutations. </param>
        /// 
        public Randomization(int seed)
        {
            this.Seed = seed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Randomization"/> class.
        /// </summary>
        /// 
        public Randomization()
        {
            this.Seed = null;
        }


        /// <summary>
        ///   Applies the filter to the current data.
        /// </summary>
        /// 
        public DataTable Apply(DataTable data)
        {
            DataTable result = data.Clone();

            int rows = data.Rows.Count;

            int[] indices = Matrix.Indices(0, rows);

            int r = Accord.Math.Tools.Random.Next();
            if (Seed.HasValue) 
                Accord.Math.Tools.SetupGenerator(Seed.Value);

            Accord.Statistics.Tools.Shuffle(indices);

            if (Seed.HasValue)
                Accord.Math.Tools.SetupGenerator(r);

            for (int i = 0; i < indices.Length; i++)
                result.ImportRow(data.Rows[indices[i]]);

            return result;
        }

    }
}
