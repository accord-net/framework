// Accord Unit Tests
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

namespace Accord.Tests.Statistics
{
    using System.Data;
    using Accord.Math;
    using Accord.Statistics.Filters;
    using AForge;
    using NUnit.Framework;

    [TestFixture]
    public class BranchingFilterTest
    {
#if !NO_DATA_TABLE
        [Test]
        public void ApplyTest()
        {
            object[,] data = 
            {
                { "Id", "IsSmoker", "Age" },
                {   0,       1,        10  },
                {   1,       1,        15  },
                {   2,       0,        40  },
                {   3,       1,        20  },
                {   4,       0,        70  },
                {   5,       0,        55  },
            };

            DataTable input = data.ToTable();

            var smoker = new LinearScaling();
            var common = new LinearScaling();

            smoker.Columns.Add(new LinearScaling.Options("Age")
            {
                SourceRange = new DoubleRange(10, 20),
                OutputRange = new DoubleRange(-1, 0)
            });

            common.Columns.Add(new LinearScaling.Options("Age")
            {
                SourceRange = new DoubleRange(40, 70),
                OutputRange = new DoubleRange(0, 1)
            });


            var settings = new Branching.Options("IsSmoker");
            settings.Filters.Add(1, smoker);
            settings.Filters.Add(0, common);

            Branching branching = new Branching(settings);



            DataTable actual = branching.Apply(input);

            double[] expected = { -1, -0.5, 0, 0, 1, 0.5 };

            foreach (DataRow row in actual.Rows)
            {
                int id = (int)row[0];
                double age = (double)row[2];
                Assert.AreEqual(expected[id], age);
            }
        }
#endif
    }
}
