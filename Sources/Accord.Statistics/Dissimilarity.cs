using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accord.Statistics
{
    /// <summary>
    /// Static class Dissimilarity. Defines a set of extension methods defining dissimilarity measures.
    /// </summary>
    public static class Dissimilarity
    {
        /// <summary>
        /// Gets the Dice dissimilarity between two vectors.
        /// </summary>
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// <returns>The Dice dissimilarity between x and y.</returns>
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
        /// Gets the Jaccard dissimilarity between two vectors.
        /// </summary>
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// <returns>The Jaccard dissimilarity between x and y.</returns>
        public static double Jaccard(int[] x, int[] y)
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

            return (tf + ft) / (double)(tt + ft + tf);
        }

        /// <summary>
        /// Gets the Kulsinsk dissimilarity between two vectors.
        /// </summary>
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// <returns>The Kulsinsk dissimilarity between x and y.</returns>
        public static double Kulsinsk(int[] x, int[] y)
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

            return (tf + ft - tt + x.Length) / (double)(ft + tf + x.Length);
        }

        /// <summary>
        /// Gets the Matching dissimilarity between two vectors.
        /// </summary>
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// <returns>The Matching dissimilarity between x and y.</returns>
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
        /// Gets the Rogers Tanimoto dissimilarity between two vectors.
        /// </summary>
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// <returns>The Rogers Tanimoto dissimilarity between x and y.</returns>
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
        /// Gets the Russel Rao dissimilarity between two vectors.
        /// </summary>
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// <returns>The Russel Rao dissimilarity between x and y.</returns>
        public static double RusselRao(int[] x, int[] y)
        {

            int tt = 0;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] == 1 && y[i] == 1) tt++;
            }

            return (x.Length - tt) / (double)(x.Length);
        }

        /// <summary>
        /// Gets the Sokal Michener dissimilarity between two vectors.
        /// </summary>
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// <returns>The Sokal Michener dissimilarity between x and y.</returns>
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
        /// Gets the Sokal Sneath dissimilarity between two vectors.
        /// </summary>
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// <returns>The Sokal Sneath dissimilarity between x and y.</returns>
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
        /// Gets the Yule dissimilarity between two vectors.
        /// </summary>
        /// <param name="x">A vector.</param>
        /// <param name="y">A vector.</param>
        /// <returns>The Yule dissimilarity between x and y.</returns>
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
