// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © 2009-2017 César Souza <cesarsouza at gmail.com>
// and other contrinbutors.
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

namespace Accord.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Math;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///   Different methods for computing quantiles.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   When one has a sample drawn from an unknown population, the cumulative distribution function and quantile 
    ///   function of the underlying population are not known and the task becomes that of estimating the quantiles. 
    ///   There are several methods. Mathematica, Matlab, R and GNU Octave programming languages include
    ///   nine sample quantile methods.SAS includes five sample quantile methods, SciPy and Maple both include eight, 
    ///   EViews includes the six piecewise linear functions, STATA includes two, and Microsoft Excel includes two. 
    ///   Mathematica supports an arbitrary parameter for methods that allows for other, non-standard, methods.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Quantile">
    ///       Wikipedia contributors. Quantile. Wikipedia, The Free Encyclopedia. July 25, 2017, 21:56 UTC. 
    ///       Available at: https://en.wikipedia.org/wiki/Quantile. </a>
    ///       </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    public enum QuantileMethod
    {
        /// <summary>
        ///   The default quantile method in the framework (6).
        /// </summary>
        /// 
        Default = Type6,

        /// <summary>
        ///   The default method in R (7).
        /// </summary>
        /// 
        R = Type7,

        /// <summary>
        ///   The dafault method in Maple (8).
        /// </summary>
        /// 
        Maple = Type8,

        /// <summary>
        ///   Inverse of empirical distribution function. 
        ///   Equivalent types in other packages: R: 1, SAS: 3, Maple: 1.
        /// </summary>
        /// 
        Type1 = 1,

        /// <summary>
        ///   The same as R-1, but with averaging at discontinuities.
        ///   Equivalent types in other packages: R: 2, SAS: 5, Maple: 2.
        /// </summary>
        /// 
        Type2 = 2,

        /// <summary>
        ///   The observation numbered closest to Np. 
        ///   Equivalent types in other packages: R: 3, SAS: 2.
        /// </summary>
        /// 
        Type3 = 3,

        /// <summary>
        ///   Linear interpolation of the empirical distribution function. 
        ///   Equivalent types in other packages: R: 4, SAS: 1, SciPy: (0,1), Maple: 3.
        /// </summary>
        /// 
        Type4 = 4,

        /// <summary>
        ///   Piecewise linear function where the knots are the values midway through the steps of the empirical distribution function. R-5, SciPy-(.5,.5), Maple-4.
        /// </summary>
        /// 
        Type5 = 5,

        /// <summary>
        ///   Linear interpolation of the expectations for the order statistics for the uniform distribution on [0,1]. 
        ///   That is, it is the linear interpolation between points (ph, xh), where ph = h / (N + 1) is the probability 
        ///   that the last of (N+1) randomly drawn values will not exceed the h-th smallest of the first N randomly drawn
        ///   values. Equivalence to other packages: R-6, Excel, SAS-4, SciPy-(0,0), Maple-5.
        /// </summary>
        /// 
        Type6 = 6,

        /// <summary>
        ///   Linear interpolation of the modes for the order statistics for the uniform distribution on [0,1]. 
        ///   Equivalence to other packages: R: 7, Excel, SciPy: (1,1), Maple: 6.
        /// </summary>
        /// 
        Type7 = 7,

        /// <summary>
        ///   Linear interpolation of the approximate medians for order statistics. Equivalence to other packages:
        ///   R: 8, SciPy: (1/3,1/3), Maple: 7.
        /// </summary>
        /// 
        Type8 = 8,

        /// <summary>
        ///   The resulting quantile estimates are approximately unbiased for the expected order statistics if x is normally 
        ///   distributed. Equivalence to other packages: R: 9, SciPy: (3/8,3/8), Maple: 8.
        /// </summary>
        /// 
        Type9 = 9
    }

    
    public static partial class Measures
    {
        private static readonly double ONE_THIRD = 1.0 / 3.0;
        private static readonly double TWO_THIRDS = 2.0 / 3.0;


        /// <summary>
        ///   Computes the Median of the given values.
        /// </summary>
        /// 
        /// <param name="values">An integer array containing the vector members.</param>
        /// <param name="alreadySorted">A boolean parameter informing if the given values have already been sorted.</param>
        /// <param name="type">The quartile definition that should be used. See <see cref="QuantileMethod"/> for datails.</param>
        /// <param name="inPlace">Pass true if the method is allowed to sort <paramref name="values"/> in place, overwriting the
        ///   its original order.</param>
        ///   
        /// <returns>The median of the given data.</returns>
        /// 
        public static double Median(this double[] values, bool alreadySorted = false, QuantileMethod type = QuantileMethod.Default, bool inPlace = false)
        {
            return Quantile(values, type: type, probabilities: 0.5, alreadySorted: alreadySorted, inPlace: inPlace);
        }

        /// <summary>
        ///   Computes the Median of the given values.
        /// </summary>
        /// 
        /// <param name="values">An integer array containing the vector members.</param>
        /// <param name="alreadySorted">A boolean parameter informing if the given values have already been sorted.</param>
        /// <param name="type">The quartile definition that should be used. See <see cref="QuantileMethod"/> for datails.</param>
        /// <param name="inPlace">Pass true if the method is allowed to sort <paramref name="values"/> in place, overwriting the
        ///   its original order.</param>
        ///   
        /// <returns>The median of the given data.</returns>
        /// 
        public static double Median(this int[] values, bool alreadySorted = false, QuantileMethod type = QuantileMethod.Default, bool inPlace = false)
        {
            // TODO: Use T4 templates to generate separate implementations for each data type
            return Quantile(values.ToDouble(), type: type, probabilities: 0.5, alreadySorted: alreadySorted, inPlace: inPlace);
        }

        /// <summary>
        ///   Computes the Quartiles of the given values.
        /// </summary>
        /// 
        /// <param name="values">An integer array containing the vector members.</param>
        /// <param name="alreadySorted">A boolean parameter informing if the given values have already been sorted.</param>
        /// <param name="range">The inter-quartile range for the values.</param>
        /// <param name="type">The quartile definition that should be used. See <see cref="QuantileMethod"/> for datails.</param>
        /// <param name="inPlace">Pass true if the method is allowed to sort <paramref name="values"/> in place, overwriting the
        ///   its original order.</param>
        ///   
        /// <returns>The second quartile, the median of the given data.</returns>
        /// 
        public static double Quartiles(this double[] values, out DoubleRange range, bool alreadySorted = false, QuantileMethod type = QuantileMethod.Default, bool inPlace = false)
        {
            double[] result = Quantiles(values, type: type, probabilities: new[] { 0.25, 0.5, 0.75 }, alreadySorted: alreadySorted, inPlace: inPlace);
            range = new DoubleRange(result[0], result[2]);
            return result[1];
        }

        /// <summary>
        ///   Computes the Quartiles of the given values.
        /// </summary>
        /// 
        /// <param name="values">An integer array containing the vector members.</param>
        /// <param name="q1">The first quartile.</param>
        /// <param name="q3">The third quartile.</param>
        /// <param name="alreadySorted">A boolean parameter informing if the given values have already been sorted.</param>
        /// <param name="type">The quartile definition that should be used. See <see cref="QuantileMethod"/> for datails.</param>
        /// <param name="inPlace">Pass true if the method is allowed to sort <paramref name="values"/> in place, overwriting the
        ///   its original order.</param>
        ///   
        /// <returns>The second quartile, the median of the given data.</returns>
        /// 
        public static double Quartiles(this double[] values, out double q1, out double q3, bool alreadySorted = false, QuantileMethod type = QuantileMethod.Default, bool inPlace = false)
        {
            double[] result = Quantiles(values, type: type, probabilities: new[] { 0.25, 0.5, 0.75 }, alreadySorted: alreadySorted, inPlace: inPlace);
            q1 = result[0];
            q3 = result[2];
            return result[1];
        }

        /// <summary>
        ///   Computes the lower quartile (Q1) for the given data.
        /// </summary>
        /// 
        /// <param name="values">An integer array containing the vector members.</param>
        /// <param name="alreadySorted">A boolean parameter informing if the given values have already been sorted.</param>
        /// <param name="type">The quartile definition that should be used. See <see cref="QuantileMethod"/> for datails.</param>
        /// <param name="inPlace">Pass true if the method is allowed to sort <paramref name="values"/> in place, overwriting the
        ///   its original order.</param>
        ///   
        /// <returns>The first quartile of the given data.</returns>
        /// 
        public static double LowerQuartile(this double[] values, bool alreadySorted = false, QuantileMethod type = QuantileMethod.Default, bool inPlace = false)
        {
            return Quantile(values, type: type, probabilities: 0.25, alreadySorted: alreadySorted, inPlace: inPlace);
        }

        /// <summary>
        ///   Computes the upper quartile (Q3) for the given data.
        /// </summary>
        /// 
        /// <param name="values">An integer array containing the vector members.</param>
        /// <param name="alreadySorted">A boolean parameter informing if the given values have already been sorted.</param>
        /// <param name="type">The quartile definition that should be used. See <see cref="QuantileMethod"/> for datails.</param>
        /// <param name="inPlace">Pass true if the method is allowed to sort <paramref name="values"/> in place, overwriting the
        ///   its original order.</param>
        ///   
        /// <returns>The third quartile of the given data.</returns>
        /// 
        public static double UpperQuartile(this double[] values, bool alreadySorted = false, QuantileMethod type = QuantileMethod.Default, bool inPlace = false)
        {
            return Quantile(values, type: type, probabilities: 0.75, alreadySorted: alreadySorted, inPlace: inPlace);
        }






        /// <summary>
        ///   Computes single quantile for the given sequence.
        /// </summary>
        /// 
        /// <param name="values">The sequence of observations.</param>
        /// <param name="type">The quantile type, 1...9.</param>
        /// <param name="probabilities">The auantile probability.</param>
        /// <param name="alreadySorted">A boolean parameter informing if the given values have already been sorted.</param>
        /// <param name="inPlace">Pass true if the method is allowed to sort <paramref name="values"/> in place, overwriting the
        ///   its original order.</param>
        ///   
        /// <returns>Quantile value.</returns>
        /// 
        public static double Quantile(this double[] values, double probabilities, bool alreadySorted = false, QuantileMethod type = QuantileMethod.Default, bool inPlace = false)
        {
            return Quantiles(values, new double[] { probabilities }, alreadySorted: alreadySorted, type: type, inPlace: inPlace)[0];
        }

        /// <summary>
        ///   Computes multiple quantiles for the given sequence.
        /// </summary>
        /// 
        /// <param name="values">The sequence of observations.</param>
        /// <param name="probabilities">The sequence of quantile probabilities.</param>
        /// <param name="type">The quantile type, 1...9.</param>
        /// <param name="alreadySorted">A boolean parameter informing if the given values have already been sorted.</param>
        /// <param name="inPlace">Pass true if the method is allowed to sort <paramref name="values"/> in place, overwriting the
        ///   its original order.</param>
        /// 
        /// <returns>Quantile value.</returns>
        /// 
        public static double[] Quantiles(this double[] values, double[] probabilities, bool alreadySorted = false, QuantileMethod type = QuantileMethod.Default, bool inPlace = false)
        {
            if (values == null)
                throw new ArgumentNullException("Sequence of observations can't be null.", "values");
            if (values.Length == 0)
                throw new ArgumentException("Sequence of observations can't be empty.", "values");

            if (probabilities == null)
                throw new ArgumentNullException("Sequence of quantile probabilities can't be null.", "probabilities");
            if (probabilities.Length == 0)
                throw new ArgumentNullException("Sequence of quantile probabilities can't be empty.", "probabilities");

            if (probabilities.Any(pv => pv < 0.0 || pv > 1.0))
                throw new ArgumentException("There is invalid probability in the sequence of quantile probabilities.", "probabilities");

            double[] result = new double[probabilities.Length];

            if (!inPlace && !alreadySorted)
                values = values.Copy();

            switch (type)
            {
                case QuantileMethod.Type1: return Q1(values, probabilities, alreadySorted, result);
                case QuantileMethod.Type2: return Q2(values, probabilities, alreadySorted, result);
                case QuantileMethod.Type3: return Q3(values, probabilities, alreadySorted, result);
                case QuantileMethod.Type4: return Q4(values, probabilities, alreadySorted, result);
                case QuantileMethod.Type5: return Q5(values, probabilities, alreadySorted, result);
                case QuantileMethod.Type6: return Q6(values, probabilities, alreadySorted, result);
                case QuantileMethod.Type7: return Q7(values, probabilities, alreadySorted, result);
                case QuantileMethod.Type8: return Q8(values, probabilities, alreadySorted, result);
                case QuantileMethod.Type9: return Q9(values, probabilities, alreadySorted, result);
                default:
                    throw new ArgumentException("Invalid quantile type, must be between 1 and 9 (inclusive)", "type");
            }
        }




        private static double[] Q1(double[] x, double[] p, bool alreadySorted, double[] result)
        {
            if (!alreadySorted)
            {
                int n = (int)Math.Ceiling(x.Length * p.Max());
                if (n > 0) n--;
                Sort.Partial(x, n);
            }

            for (int j = 0; j < result.Length; j++)
                result[j] = Q1(x, p[j]);

            return result;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static double Q1(double[] x, double p)
        {
            if (p == 0.0)
                return x[0];

            int i = (int)Math.Ceiling(x.Length * p);
            if (i > 0)
                i--;

            return x[i];
        }




        private static double[] Q2(double[] x, double[] p, bool alreadySorted, double[] result)
        {
            if (!alreadySorted)
            {
                int n = (int)Math.Ceiling(x.Length * p.Max()) + 1;
                if (n > x.Length) n = x.Length;
                if (n > 0) n--;
                Sort.Partial(x, n);
            }

            for (int j = 0; j < result.Length; j++)
                result[j] = Q2(x, p[j]);

            return result;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static double Q2(double[] x, double p)
        {
            if (p == 0.0)
                return x[0];

            if (p == 1.0)
                return x[x.Length - 1];

            int i = (int)Math.Ceiling(x.Length * p);
            if (i > 0)
                i--;

            int i2 = i + 1;
            if (i2 == x.Length)
                i2--;

            return (x[i] + x[i2]) / 2;
        }




        private static T[] Q3<T>(T[] x, double[] p, bool alreadySorted, T[] result)
             where T : IComparable<T>
        {
            if (!alreadySorted)
            {
                int n = (int)Math.Ceiling(x.Length * p.Max());
                if (n > 0) n--;
                Sort.Partial(x, n);
            }

            double lowThreshold = 0.5 / x.Length;

            for (int j = 0; j < result.Length; j++)
                result[j] = Q3(x, p[j], lowThreshold);

            return result;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static T Q3<T>(T[] x, double p, double lowThreshold)
        {
            if (p <= lowThreshold)
                return x[0];

            int i = (int)Math.Round(x.Length * p);
            if (i > 0)
                i--;

            return x[i];
        }




        private static double[] Q4(double[] x, double[] p, bool alreadySorted, double[] result)
        {
            if (!alreadySorted)
            {
                int n = (int)Math.Ceiling(x.Length * p.Max()) + 1;
                if (n > x.Length) n = x.Length;
                if (n > 0) n--;
                Sort.Partial(x, n);
            }

            double lowThreshold = 1.0 / x.Length;

            for (int j = 0; j < result.Length; j++)
                result[j] = Q4(x, p[j], lowThreshold);

            return result;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static double Q4(double[] x, double pj, double lowThreshold)
        {
            if (pj < lowThreshold)
                return x[0];

            if (pj == 1.0)
                return x[x.Length - 1];

            double h = x.Length * pj;
            double hc = Math.Floor(h);

            int i = (int)hc;
            if (i > 0)
                i--;

            int i2 = i + 1;
            if (i2 == x.Length)
                i2--;

            return x[i] + (h - hc) * (x[i2] - x[i]);
        }




        private static double[] Q5(double[] x, double[] p, bool alreadySorted, double[] result)
        {
            if (!alreadySorted)
            {
                int n = (int)Math.Ceiling(x.Length * p.Max() + 0.5) + 1;
                if (n > x.Length) n = x.Length;
                if (n > 0) n--;
                Sort.Partial(x, n);
            }

            double lowThreshold = 0.5 / x.Length;
            double highThreshold = (x.Length - 0.5) / x.Length;

            for (int j = 0; j < result.Length; j++)
                result[j] = Q5(x, p[j], lowThreshold, highThreshold);

            return result;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static double Q5(double[] x, double p, double lowThreshold, double highThreshold)
        {
            if (p < lowThreshold)
                return x[0];

            if (p >= highThreshold)
                return x[x.Length - 1];

            double h = x.Length * p + 0.5;
            double hc = Math.Floor(h);

            int i = (int)hc;
            if (i > 0)
                i--;

            int i2 = i + 1;
            if (i2 == x.Length)
                i2--;

            return x[i] + (h - hc) * (x[i2] - x[i]);
        }




        private static double[] Q6(double[] x, double[] p, bool alreadySorted, double[] result)
        {
            if (!alreadySorted)
            {
                int n = (int)Math.Ceiling((x.Length + 1) * p.Max());
                if (n > x.Length) n = x.Length;
                if (n > 0) n--;
                Sort.Partial(x, n);
            }

            double lowThreshold = 1.0 / (x.Length + 1);
            double highThreshold = x.Length / (double)(x.Length + 1);

            for (int j = 0; j < result.Length; j++)
                result[j] = Q6(x, p[j], lowThreshold, highThreshold);

            return result;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static double Q6(double[] x, double p, double lowThreshold, double highThreshold)
        {
            if (p < lowThreshold)
                return x[0];

            if (p >= highThreshold)
                return x[x.Length - 1];

            double h = (x.Length + 1) * p;
            double hc = Math.Floor(h);

            int i = (int)hc;
            if (i > 0)
                i--;

            int i2 = i + 1;
            if (i2 == x.Length)
                i2--;

            return x[i] + (h - hc) * (x[i2] - x[i]);
        }



        private static double[] Q7(double[] x, double[] p, bool alreadySorted, double[] result)
        {
            if (!alreadySorted)
            {
                int n = (int)Math.Ceiling((x.Length - 1) * p.Max() + 1);
                if (n > x.Length) n = x.Length;
                if (n > 0) n--;
                Sort.Partial(x, n);
            }

            for (int j = 0; j < result.Length; j++)
                result[j] = Q7(x, p[j]);

            return result;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static double Q7(double[] x, double p)
        {
            if (p == 1.0)
                return x[x.Length - 1];

            double h = (x.Length - 1) * p + 1;
            double hc = Math.Floor(h);

            int i = (int)hc;
            if (i > 0)
                i--;

            int i2 = i + 1;
            if (i2 == x.Length)
                i2--;

            return x[i] + (h - hc) * (x[i2] - x[i]);
        }



        private static double[] Q8(double[] x, double[] p, bool alreadySorted, double[] result)
        {
            if (!alreadySorted)
            {
                int n = (int)Math.Ceiling((x.Length + ONE_THIRD) * p.Max() + ONE_THIRD);
                if (n >= x.Length) n = x.Length;
                if (n > 0) n--;
                Sort.Partial(x, n);
            }

            double lowThreshold = TWO_THIRDS / (x.Length + ONE_THIRD);
            double highThreshold = (x.Length - ONE_THIRD) / (x.Length + ONE_THIRD);

            for (int j = 0; j < result.Length; j++)
                result[j] = Q8(x, p[j], lowThreshold, highThreshold);

            return result;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static double Q8(double[] x, double p, double lowThreshold, double highThreshold)
        {
            if (p < lowThreshold)
                return x[0];

            if (p >= highThreshold)
                return x[x.Length - 1];

            double h = (x.Length + ONE_THIRD) * p + ONE_THIRD;
            double hc = Math.Floor(h);

            int i = (int)hc;
            if (i > 0)
                i--;

            int i2 = i + 1;
            if (i2 == x.Length)
                i2--;

            return x[i] + (h - hc) * (x[i2] - x[i]);
        }



        private static double[] Q9(double[] x, double[] p, bool alreadySorted, double[] result)
        {
            if (!alreadySorted)
            {
                int n = (int)Math.Ceiling((x.Length + 0.25) * p.Max() + 0.375);
                if (n >= x.Length) n = x.Length;
                if (n > 0) n--;
                Sort.Partial(x, n);
            }

            double lowThreshold = 0.625 / (x.Length + 0.25);
            double highThreshold = (x.Length - 0.375) / (x.Length + 0.25);

            for (int j = 0; j < result.Length; j++)
                result[j] = Q9(x, p[j], lowThreshold, highThreshold);

            return result;
        }

#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static double Q9(double[] x, double p, double lowThreshold, double highThreshold)
        {
            if (p < lowThreshold)
                return x[0];
            if (p >= highThreshold)
                return x[x.Length - 1];

            double h = (x.Length + 0.25) * p + 0.375;
            double hc = Math.Floor(h);

            int i = (int)hc;
            if (i > 0)
                i--;

            int i2 = i + 1;
            if (i2 == x.Length)
                i2--;

            return x[i] + (h - hc) * (x[i2] - x[i]);
        }
    }
}

