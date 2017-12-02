// Accord Statistics Library
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

namespace Accord.Statistics.Visualizations
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Accord.Math;
    using Accord.Compat;

    /// <summary>
    ///   Scatter Plot.
    /// </summary>
    /// 
    [Serializable]
    public class Scatterplot
    {

        private string title = "Scatter plot";

        private string xAxisTitle = "X";
        private string yAxisTitle = "Y";
        private string labelTitle = "Label";

        /// <summary>
        ///   Gets the title of the scatter plot.
        /// </summary>
        /// 
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        ///   Gets the name of the X-axis.
        /// </summary>
        /// 
        public string XAxisTitle
        {
            get { return xAxisTitle; }
            set { xAxisTitle = value; }
        }

        /// <summary>
        ///   Gets the name of the Y-axis.
        /// </summary>
        /// 
        public string YAxisTitle
        {
            get { return yAxisTitle; }
            set { yAxisTitle = value; }
        }

        /// <summary>
        ///   Gets the name of the label axis.
        /// </summary>
        /// 
        public string LabelAxisTitle
        {
            get { return labelTitle; }
            set { labelTitle = value; }
        }


        /// <summary>
        ///   Gets the values associated with the X-axis.
        /// </summary>
        /// 
        public double[] XAxis { get; private set; }

        /// <summary>
        ///   Gets the corresponding Y values associated with each X.
        /// </summary>
        /// 
        public double[] YAxis { get; private set; }

        /// <summary>
        ///   Gets the label of each (x,y) pair.
        /// </summary>
        /// 
        public int[] LabelAxis { get; private set; }

        /// <summary>
        ///   Gets an integer array containing the integer labels
        ///   associated with each of the classes in the scatter plot.
        /// </summary>
        /// 
        internal int[] LabelValues { get; private set; }

        /// <summary>
        ///   Gets the class labels for each of the classes in the plot.
        /// </summary>
        /// 
        public string[] LabelNames { get; private set; }

        /// <summary>
        ///   Gets a collection containing information about
        ///   each of the classes presented in the scatter plot.
        /// </summary>
        /// 
        public ReadOnlyCollection<ScatterplotClassValueCollection> Classes { get; private set; }

        /// <summary>
        ///   Constructs an empty Scatter plot.
        /// </summary>
        /// 
        public Scatterplot()
        {
        }

        /// <summary>
        ///   Constructs an empty Scatter plot with given title. 
        /// </summary>
        /// 
        /// <param name="title">Scatter plot title.</param>
        /// 
        public Scatterplot(String title)
        {
            this.title = title;
        }

        /// <summary>
        ///   Constructs an empty scatter plot with
        ///   given title and axis names.
        /// </summary>
        /// 
        /// <param name="title">Scatter Plot title.</param>
        /// <param name="xAxisTitle">Title for the x-axis.</param>
        /// <param name="yAxisTitle">Title for the y-axis.</param>
        /// 
        public Scatterplot(String title, String xAxisTitle, String yAxisTitle)
        {
            this.title = title;
            this.xAxisTitle = xAxisTitle;
            this.yAxisTitle = yAxisTitle;
        }

        /// <summary>
        ///   Constructs an empty Scatter Plot with
        ///   given title and axis names.
        /// </summary>
        /// 
        /// <param name="title">Scatter Plot title.</param>
        /// <param name="xAxisTitle">Title for the x-axis.</param>
        /// <param name="yAxisTitle">Title for the y-axis.</param>
        /// <param name="labelTitle">Title for the labels.</param>
        /// 
        public Scatterplot(String title, String xAxisTitle, String yAxisTitle, String labelTitle)
        {
            this.title = title;
            this.xAxisTitle = xAxisTitle;
            this.yAxisTitle = yAxisTitle;
            this.labelTitle = labelTitle;
        }

        private void initialize(double[] x, double[] y, int[] z)
        {
            this.XAxis = x;
            this.YAxis = y;
            this.LabelAxis = z;

            if (z == null)
            {
                LabelValues = new int[] { };
            }
            else
            {
                LabelValues = LabelAxis.Distinct();
            }

            LabelNames = new string[LabelValues.Length];
            for (int i = 0; i < LabelNames.Length; i++)
                LabelNames[i] = i.ToString();

            var classes = new ScatterplotClassValueCollection[LabelValues.Length];
            for (int i = 0; i < classes.Length; i++)
                classes[i] = new ScatterplotClassValueCollection(this, i);
            Classes = new ReadOnlyCollection<ScatterplotClassValueCollection>(classes);
        }

        /// <summary>
        ///   Computes the scatter plot.
        /// </summary>
        /// 
        /// <param name="values">Array of values.</param>
        /// 
        public void Compute(double[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            double[] idx = Vector.Range(0.0, values.Length);

            initialize(idx, values, null);
        }

        /// <summary>
        ///   Computes the scatter plot.
        /// </summary>
        /// 
        /// <param name="x">Array of X values.</param>
        /// <param name="y">Array of corresponding Y values.</param>
        /// 
        public void Compute(double[] x, double[] y)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            if (x.Length != y.Length)
                throw new DimensionMismatchException("y", "The x and y arrays should have the same length");

            initialize(x, y, null);
        }

        /// <summary>
        ///   Computes the scatter plot.
        /// </summary>
        /// 
        /// <param name="x">Array of X values.</param>
        /// <param name="y">Array of corresponding Y values.</param>
        /// <param name="labels">Array of integer labels defining a class for each (x,y) pair.</param>
        /// 
        public void Compute(double[] x, double[] y, int[] labels)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            if (labels == null)
                throw new ArgumentNullException("labels");

            if (x.Length != y.Length)
                throw new DimensionMismatchException("y", "The x and y arrays should have the same length");

            if (x.Length != labels.Length)
                throw new DimensionMismatchException("labels",
                    "The labels array should have the same length as the data array.");

            initialize(x, y, labels);
        }

        /// <summary>
        ///   Computes the scatter plot.
        /// </summary>
        /// 
        /// <param name="data">Array of { x,y } values.</param>
        /// 
        public void Compute(double[][] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            for (int i = 0; i < data.Length; i++)
                if (data[i].Length != 2)
                    throw new DimensionMismatchException("data", "The matrix should have two columns.");

            compute(data, null);
        }

        /// <summary>
        ///   Computes the scatter plot.
        /// </summary>
        /// 
        /// <param name="data">Array of { x,y } values.</param>
        /// <param name="labels">Array of integer labels defining a class for each (x,y) pair.</param>
        /// 
        public void Compute(double[][] data, int[] labels)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (labels == null)
                throw new ArgumentNullException("labels");

            if (data.Length != labels.Length)
                throw new DimensionMismatchException("labels",
                    "The labels array should have the same length as the data array.");

            for (int i = 0; i < data.Length; i++)
                if (data[i].Length != 2)
                    throw new DimensionMismatchException("data", "The matrix should have two columns.");

            compute(data, labels);
        }

        /// <summary>
        ///   Computes the scatter plot.
        /// </summary>
        /// 
        /// <param name="data">Array of { x,y } values.</param>
        /// 
        public void Compute(double[,] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.GetLength(1) != 2)
                throw new DimensionMismatchException("data", "The matrix should have two columns.");

            compute(data, null);
        }

        /// <summary>
        ///   Computes the scatter plot.
        /// </summary>
        /// 
        /// <param name="data">Array of { x,y } values.</param>
        /// <param name="labels">Array of integer labels defining a class for each (x,y) pair.</param>
        /// 
        public void Compute(double[,] data, int[] labels)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (labels == null)
                throw new ArgumentNullException("labels");

            if (data.GetLength(1) != 2)
                throw new DimensionMismatchException("data", "The matrix should have two columns.");

            if (data.GetLength(0) != labels.Length)
                throw new DimensionMismatchException("labels",
                    "The labels array should have the same length as the data array.");

            compute(data, labels);
        }

        private void compute(double[,] data, int[] labels)
        {
            int rows = data.GetLength(0);
            double[] x = new double[rows];
            double[] y = new double[rows];

            for (int i = 0; i < x.Length; i++)
            {
                x[i] = data[i, 0];
                y[i] = data[i, 1];
            }

            initialize(x, y, labels);
        }

        private void compute(double[][] data, int[] labels)
        {
            double[] x = new double[data.Length];
            double[] y = new double[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                x[i] = data[i][0];
                y[i] = data[i][1];
            }

            initialize(x, y, labels);
        }
    }
}
