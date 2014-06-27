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
    using AForge;

    public class Romberg : IUnivariateIntegration
    {

        private double[] s;
        private DoubleRange range;

        public Func<double, double> Function { get; set; }

        public double Area { get; private set; }

        public int Steps { get { return s.Length; } }

        public DoubleRange Range
        {
            get { return range; }
            set
            {
                if (Double.IsInfinity(range.Min) || Double.IsNaN(range.Min))
                    throw new ArgumentOutOfRangeException("value", "Minimum is out of range.");

                if (Double.IsInfinity(range.Max) || Double.IsNaN(range.Max))
                    throw new ArgumentOutOfRangeException("value", "Maximum is out of range.");

                range = value;
            }
        }

        public Romberg()
            : this(6)
        {
        }

        public Romberg(Func<double, double> function)
            : this(6, function)
        {
        }

        public Romberg(Func<double, double> function, double a, double b)
            : this(6, function, a, b)
        {
        }

        public Romberg(int steps)
        {
            this.s = new double[steps];
            Range = new DoubleRange(0, 1);
        }

        public Romberg(int steps, Func<double, double> function)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            Range = new DoubleRange(0, 1);
            Function = function;
            this.s = new double[steps];
        }

        public Romberg(int steps, Func<double, double> function, double a, double b)
        {
            if (Double.IsInfinity(a) || Double.IsNaN(a))
                throw new ArgumentOutOfRangeException("a");

            if (Double.IsInfinity(b) || Double.IsNaN(b))
                throw new ArgumentOutOfRangeException("b");

            Function = function;
            Range = new DoubleRange(a, b);
            this.s = new double[steps];
        }

        public bool Compute()
        {
            for (int i = 0; i < s.Length; i++)
                s[i] = 1;

            double sum = 0;
            double a = range.Min;
            double b = range.Max;

            for (int k = 0; k < s.Length; k++)
            {
                sum = s[0];
                s[0] = Trapezoidal.Integrate(Function, a, b, 1 << k);

                for (int i = 1; i <= k; i++)
                {
                    int p = (int)Math.Pow(4, i);
                    s[k] = (p * s[i - 1] - sum) / (p - 1);

                    sum = s[i];
                    s[i] = s[k];
                }
            }

            Area = s[s.Length - 1];

            return true;
        }

        public static double Integrate(Func<double, double> func, double a, double b)
        {
            return Integrate(func, a, b, 6);
        }

        public static double Integrate(Func<double, double> func, double a, double b, int steps)
        {
            var romberg = new Romberg(steps, func, a, b);

            romberg.Compute();

            return romberg.Area;
        }


        public object Clone()
        {
            Romberg clone = new Romberg(
                this.Steps, this.Function,
                this.Range.Min, this.Range.Max);

            return clone;
        }
    }
}
