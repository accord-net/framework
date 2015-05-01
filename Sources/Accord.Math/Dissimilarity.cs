// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2014
// diego.catalano at live.com
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

namespace Accord.Math
{

    /// <summary>
    ///   Static class Dissimilarity. Provides extension methods defining dissimilarity measures.
    /// </summary>
    /// 
    public static class Dissimilarity
    {

        /// <summary>
        ///   Computes the Dice dissimilarity between two vectors.
        /// </summary>
        /// 
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// 
        /// <returns>The Dice dissimilarity between x and y.</returns>
        /// 
        public static double Dice(int[] x, int[] y)
        {
            int tf = 0;
            int ft = 0;
            int tt = 0;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] == 1 && y[i] == 0) tf++;
                if (x[i] == 0 && y[i] == 1) ft++;
                if (x[i] == 1 && y[i] == 1) tt++;
            }

            return (tf + ft) / (double)(2 * tt + ft + tf);
        }

        /// <summary>
        ///   Computes the Jaccard dissimilarity between two vectors.
        /// </summary>
        /// 
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// 
        /// <returns>The Jaccard dissimilarity between x and y.</returns>
        /// 
        public static double Jaccard(int[] x, int[] y)
        {
            int tf = 0;
            int ft = 0;
            int tt = 0;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != 0 && y[i] == 0) tf++;
                if (x[i] == 0 && y[i] != 0) ft++;
                if (x[i] != 0 && y[i] != 0) tt++;
            }

            double den = tt + ft + tf;
            double num = tf + ft;
            
            return num / den;
        }

        /// <summary>
        ///   Computes the Kulczynski dissimilarity between two vectors.
        /// </summary>
        /// 
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// 
        /// <returns>The Kulczynski dissimilarity between x and y.</returns>
        /// 
        public static double Kulczynski(int[] x, int[] y)
        {
            int tf = 0;
            int ft = 0;
            int tt = 0;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != 0 && y[i] == 0) tf++;
                if (x[i] == 0 && y[i] != 0) ft++;
                if (x[i] != 0 && y[i] != 0) tt++;
            }

            double num = tf + ft - tt + x.Length;
            double den = ft + tf + x.Length;
            return num / den;
        }

        /// <summary>
        ///   Computes the Matching dissimilarity between two vectors.
        /// </summary>
        /// 
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// 
        /// <returns>The Matching dissimilarity between x and y.</returns>
        /// 
        public static double Matching(int[] x, int[] y)
        {
            int tf = 0;
            int ft = 0;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] == 1 && y[i] == 0) tf++;
                if (x[i] == 0 && y[i] == 1) ft++;
            }

            return (tf + ft) / (double)(x.Length);
        }

        /// <summary>
        ///   Computes the Rogers-Tanimoto dissimilarity between two vectors.
        /// </summary>
        /// 
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// 
        /// <returns>The Rogers Tanimoto dissimilarity between x and y.</returns>
        /// 
        public static double RogersTanimoto(int[] x, int[] y)
        {
            int tf = 0;
            int ft = 0;
            int tt = 0;
            int ff = 0;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] == 1 && y[i] == 0) tf++;
                if (x[i] == 0 && y[i] == 1) ft++;
                if (x[i] == 1 && y[i] == 1) tt++;
                if (x[i] == 0 && y[i] == 0) ff++;
            }

            int r = 2 * (tf + ft);
            return r / (double)(tt + ff + r);
        }

        /// <summary>
        ///   Computes the Russel Rao dissimilarity between two vectors.
        /// </summary>
        /// 
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// 
        /// <returns>The Russel Rao dissimilarity between x and y.</returns>
        /// 
        public static double RusselRao(int[] x, int[] y)
        {
            int tt = 0;
            for (int i = 0; i < x.Length; i++)
                if (x[i] != 0 && y[i] != 0) tt++;

            return (x.Length - tt) / (double)(x.Length);
        }

        /// <summary>
        ///   Computes the Sokal-Michener dissimilarity between two vectors.
        /// </summary>
        /// 
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// 
        /// <returns>The Sokal-Michener dissimilarity between x and y.</returns>
        /// 
        public static double SokalMichener(int[] x, int[] y)
        {
            int tf = 0;
            int ft = 0;
            int tt = 0;
            int ff = 0;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] == 1 && y[i] == 0) tf++;
                if (x[i] == 0 && y[i] == 1) ft++;
                if (x[i] == 1 && y[i] == 1) tt++;
                if (x[i] == 0 && y[i] == 0) ff++;
            }

            int r = 2 * (tf + ft);
            return r / (double)(ff + tt + r);
        }

        /// <summary>
        ///   Computes the Sokal Sneath dissimilarity between two vectors.
        /// </summary>
        /// 
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// 
        /// <returns>The Sokal Sneath dissimilarity between x and y.</returns>
        /// 
        public static double SokalSneath(int[] x, int[] y)
        {
            int tf = 0;
            int ft = 0;
            int tt = 0;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] == 1 && y[i] == 0) tf++;
                if (x[i] == 0 && y[i] == 1) ft++;
                if (x[i] == 1 && y[i] == 1) tt++;
            }

            int r = 2 * (tf + ft);
            return r / (double)(tt + r);
        }

        /// <summary>
        ///   Computes the Yule dissimilarity between two vectors.
        /// </summary>
        /// 
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// 
        /// <returns>The Yule dissimilarity between x and y.</returns>
        /// 
        public static double Yule(int[] x, int[] y)
        {
            int tf = 0;
            int ft = 0;
            int tt = 0;
            int ff = 0;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] == 1 && y[i] == 0) tf++;
                if (x[i] == 0 && y[i] == 1) ft++;
                if (x[i] == 1 && y[i] == 1) tt++;
                if (x[i] == 0 && y[i] == 0) ff++;
            }

            double r = 2 * (tf + ft);
            return r / (tt + ff + r / 2);
        }

    }
}
