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
    using Accord.Math;
    using Accord.Math.Random;
    using Accord.Statistics;
    using Accord.Statistics.Running;
    using NUnit.Framework;

    [TestFixture]
    public class KalmanFilterTest
    {

        [Test]
        public void push_test()
        {
            Generator.Seed = 0;

            #region doc_push
            // Let's say we have the following sequence of coordinate points:
            double[] x = Vector.Range(0.0, 100.0); // z(t) = (t, 4.2*t + noise)
            double[] y = x.Apply(x_i => 4.2 * x_i + Generator.Random.NextDouble());

            // Create a new Kalman filter
            var kf = new KalmanFilter2D();

            // Push the points into the filter
            for (int i = 0; i < x.Length; i++)
                kf.Push(x[i], y[i]);

            // Estimate the points location
            double newX = kf.X; // should be 99.004000000912569
            double newY = kf.Y; // should be 415.99224675780607

            // Estimate the points velocity
            double velX = kf.XAxisVelocity; // should be 1.0020000004925984
            double velY = kf.YAxisVelocity; // should be 4.1356394152821094
            #endregion

            Assert.AreEqual(1.0020000004925984, velX);
            Assert.AreEqual(4.1356394152821094, velY);
            Assert.AreEqual(99.004000000912569, newX);
            Assert.AreEqual(415.99224675780607, newY);
        }

    }
}
