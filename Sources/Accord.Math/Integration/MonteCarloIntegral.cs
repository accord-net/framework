// Accord Math Library
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

namespace Accord.Math.Integration
{
    using System;

    public class MonteCarloIntegral
    {
        public int NumberOfParameters { get; private set; }

        public double[] Min { get; private set; }
        public double[] Max { get; private set; }

        private double[] ranges;

        public Func<double[], double> Function { get; set; }

        public Random Random { get; set; }

        private ISingleValueConvergence convergence;

        public double Area { get; set; }
        public double Error { get; set; }

        public double Tolerance { get { return convergence.Tolerance; } }
        public int Iterations { get { return convergence.Iterations; } }

        public MonteCarloIntegral(int parameters, Func<double[], double> function)
        {
            this.NumberOfParameters = parameters;
            this.Min = new double[parameters];
            this.Max = new double[parameters];
            this.ranges = new double[parameters];

            for (int i = 0; i < Max.Length; i++)
                Max[i] = 1;

            this.Function = function;
            this.convergence = new RelativeConvergence();
        }

        public double Compute()
        {
            for (int i = 0; i < Min.Length; i++)
                ranges[i] = Max[i] - Min[i];

            double volume = 1;
            for (int i = 0; i < ranges.Length; i++)
                volume *= ranges[i];

            int count = 0;
            double sum = 0;
            double sum2 = 0;

            double[] sample = new double[NumberOfParameters];

            do
            {
                for (int i = 0; i < sample.Length; i++)
                    sample[i] = Random.Next() * ranges[i] + Min[i];

                double f = Function(sample);

                count++;
                sum += f;
                sum2 += f * f;

                convergence.NewValue = sum;
            }
            while (!convergence.HasConverged);

            double avg = sum / count;
            double avg2 = sum2 / count;

            Area = volume * avg;
            Error = volume * Math.Sqrt((avg2 - avg * avg) / count);

            return Area;
        }

        public static double Integrate(Func<double, double> func, double a, double b,
            int samples, double tol)
        {
            double volume = (b - a);

            int count = 0;
            double sum = 0;
            double old = 0;

            var random = Accord.Math.Tools.Random;

            for (count = 0; count < samples; count++)
            {
                double u = random.Next() * (b - a) + a;

                double f = func(u);

                count++;
                sum += f;

                if ((f - old) < f * tol)
                    break;
            }

            double avg = sum / count;

            return volume * avg;
        }
    }
}
