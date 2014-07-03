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

    /// <summary>
    ///   Trapezoidal rule for numerical integration.
    /// </summary>
    /// 
    public class Trapezoidal : IIntegrationMethod
    {
        private DoubleRange range;

        public Func<double, double> Function { get; set; }

        public double Area { get; private set; }

        public int Steps { get; set; }

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

        public Trapezoidal()
            : this(6)
        {
        }

        public Trapezoidal(Func<double, double> function)
            : this(6, function)
        {
        }

        public Trapezoidal(Func<double, double> function, double a, double b)
            : this(6, function, a, b)
        {
        }

        public Trapezoidal(int steps)
        {
            Steps = steps;
            Range = new DoubleRange(0, 1);
        }

        public Trapezoidal(int steps, Func<double, double> function)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            Range = new DoubleRange(0, 1);
            Function = function;
            Steps = steps;
        }

        public Trapezoidal(int steps, Func<double, double> function, double a, double b)
        {
            if (Double.IsInfinity(a) || Double.IsNaN(a))
                throw new ArgumentOutOfRangeException("a");

            if (Double.IsInfinity(b) || Double.IsNaN(b))
                throw new ArgumentOutOfRangeException("b");

            Function = function;
            Range = new DoubleRange(a, b);
            Steps = steps;
        }

        public bool Compute()
        {
            Area = Integrate(Function, range.Min, range.Max, Steps);

            return true;
        }

        public static double Integrate(Func<double, double> func, double a, double b, int steps)
        {
            double h = (b - a) / steps;

            double sum = 0.5 * (func(a) + func(b));

            for (int i = 1; i < steps; i++)
                sum += func(a + i * h);

            return h * sum;
        }


        public object Clone()
        {
            Trapezoidal clone = new Trapezoidal(
                this.Steps, this.Function,
                this.Range.Min, this.Range.Max);

            return clone;
        }

    }
}
