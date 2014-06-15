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

    public static class Romberg
    {
        public static double Integrate(Func<double, double> func, double a, double b)
        {
            return Integrate(func, a, b, 6);
        }

        public static double Integrate(Func<double, double> func, double a, double b,
            int steps)
        {
            double[] s = new double[steps];

            double sum = 0;
            for (int i = 0; i < s.Length; i++)
                s[i] = 1;

            for (int k = 0; k < s.Length; k++)
            {
                sum = s[0];
                s[0] = Trapezoidal.Integrate(func, a, b, 1 << k);

                for (int i = 1; i <= k; i++)
                {
                    int p = (int)Math.Pow(4, i);
                    s[k] = (p * s[i - 1] - sum) / (p - 1);

                    sum = s[i];
                    s[i] = s[k];
                }
            }

            return s[s.Length - 1];
        }
    }
}
