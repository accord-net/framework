// Accord Statistics Library
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

namespace Accord.Statistics.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Math;
    using Accord.Statistics.Visualizations;
    using Accord.Statistics.Testing;

    /// <summary>
    ///   Methods for computing the area under <see cref="ReceiverOperatingCharacteristic">
    ///   Receiver-Operating Characteristic</see> (ROC) curves (also known as the ROC AUC).
    /// </summary>
    /// 
    public enum RocAreaMethod
    {
        /// <summary>
        ///   Method of DeLong, E. R., D. M. DeLong, and D. L. Clarke-Pearson. 1988. Comparing 
        ///   the areas under two or more correlated receiver operating characteristic curves:
        ///   a nonparametric approach. Biometrics 44:837–845.
        /// </summary>
        /// 
        DeLong,

        /// <summary>
        ///   Method of Hanley, J.A. and McNeil, B.J. 1983. A method of comparing the areas under
        ///   receiver operating characteristic curves derived from the same cases. Radiology 148:839-843.
        /// </summary>
        /// 
        HanleyMcNeil
    }

    /// <summary>
    ///   Receiver Operating Characteristic (ROC) Curve.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In signal detection theory, a receiver operating characteristic (ROC), or simply
    ///   ROC curve, is a graphical plot of the sensitivity vs. (1 − specificity) for a 
    ///   binary classifier system as its discrimination threshold is varied. </para>
    /// <para>
    ///   This package does not attempt to fit a curve to the obtained points. It just
    ///   computes the area under the ROC curve directly using the trapezoidal rule.</para>  
    /// <para>
    ///   Also note that the curve construction algorithm uses the convention that a 
    ///   higher test value represents a positive for a condition while computing
    ///   sensitivity and specificity values.</para>  
    ///  
    /// <para>
    ///   References: 
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Receiver Operating Characteristic. Available on:
    ///       http://en.wikipedia.org/wiki/Receiver_operating_characteristic </description></item>
    ///     <item><description>
    ///       Anaesthesist. The magnificent ROC. Available on:
    ///       http://www.anaesthetist.com/mnm/stats/roc/Findex.htm </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to measure the accuracy
    ///   of a binary classifier using a ROC curve.</para>
    /// 
    /// <code>
    /// // This example shows how to measure the accuracy of a 
    /// // binary classifier using a ROC curve. For this example,
    /// // we will be creating a Support Vector Machine trained
    /// // on the following training instances:
    /// 
    /// double[][] inputs =
    /// {
    ///     // Those are from class -1
    ///     new double[] { 2, 4, 0 },
    ///     new double[] { 5, 5, 1 },
    ///     new double[] { 4, 5, 0 },
    ///     new double[] { 2, 5, 5 },
    ///     new double[] { 4, 5, 1 },
    ///     new double[] { 4, 5, 0 },
    ///     new double[] { 6, 2, 0 },
    ///     new double[] { 4, 1, 0 },
    /// 
    ///     // Those are from class +1
    ///     new double[] { 1, 4, 5 },
    ///     new double[] { 7, 5, 1 },
    ///     new double[] { 2, 6, 0 },
    ///     new double[] { 7, 4, 7 },
    ///     new double[] { 4, 5, 0 },
    ///     new double[] { 6, 2, 9 },
    ///     new double[] { 4, 1, 6 },
    ///     new double[] { 7, 2, 9 },
    /// };
    /// 
    /// int[] outputs =
    /// {
    ///     -1, -1, -1, -1, -1, -1, -1, -1, // fist eight from class -1
    ///     +1, +1, +1, +1, +1, +1, +1, +1  // last eight from class +1
    /// };
    /// 
    /// // Next, we create a linear Support Vector Machine with 4 inputs
    /// SupportVectorMachine machine = new SupportVectorMachine(inputs: 3);
    /// 
    /// // Create the sequential minimal optimization learning algorithm
    /// var smo = new SequentialMinimalOptimization(machine, inputs, outputs);
    /// 
    /// // We learn the machine
    /// double error = smo.Run();
    /// 
    /// // And then extract its predicted labels
    /// double[] predicted = new double[inputs.Length];
    /// for (int i = 0; i &lt; predicted.Length; i++)
    ///     predicted[i] = machine.Compute(inputs[i]);
    /// 
    /// // At this point, the output vector contains the labels which
    /// // should have been assigned by the machine, and the predicted
    /// // vector contains the labels which have been actually assigned.
    /// 
    /// // Create a new ROC curve to assess the performance of the model
    /// var roc = new ReceiverOperatingCharacteristic(outputs, predicted);
    /// roc.Compute(100); // Compute a ROC curve with 100 cut-off points
    /// 
    /// // Generate a connected scatter plot for the ROC curve and show it on-screen
    /// ScatterplotBox.Show(roc.GetScatterplot(includeRandom: true), nonBlocking: true)
    /// 
    ///     .SetSymbolSize(0)      // do not display data points
    ///     .SetLinesVisible(true) // show lines connecting points
    ///     .SetScaleTight(true)   // tighten the scale to points
    ///     .WaitForClose();
    /// </code>
    /// 
    /// <para>
    ///   The resulting graph is shown below.</para>
    ///   
    /// <img src="..\images\roc-curve.png" />
    /// 
    /// </example>
    /// 
    [Serializable]
    public class ReceiverOperatingCharacteristic
    {

        private double area;     // the exact area computed using the trapezoidal rule
        private double error;    // the AUC ROC standard error using DeLong's estimate

        private double[] measurement; // The ground truth, confirmed data
        private double[] prediction;  // The test predictions for the data

        private double[] positiveResults; // the subjects which should have been computed as positive
        private double[] negativeResults; // the subjects which should have been computed as negative

        private double[] positiveAccuracy; // DeLong's pseudoaccuracy for positive subjects
        private double[] negativeAccuracy; // DeLong's pseudoaccuracy for negative subjects

        // The real number of positives and negatives in the measured (true) data
        private int positiveCount;
        private int negativeCount;

        // The values which represent positive and negative values in our
        //  measurement data (such as presence or absence of some disease)
        private double dtrue;
        private double dfalse;

        // The minimum and maximum values in the prediction data (such
        // as categorical rankings collected from test subjects)
        private double min;
        private double max;


        // The collection to hold our curve point information
        private ReceiverOperatingCharacteristicPointCollection collection;



        /// <summary>
        ///   Constructs a new Receiver Operating Characteristic model
        /// </summary>
        /// 
        /// <param name="expected">
        ///   An array of binary values. Typically represented as 0 and 1, or -1 and 1,
        ///   indicating negative and positive cases, respectively. The maximum value
        ///   will be treated as the positive case, and the lowest as the negative.</param>
        /// <param name="actual">
        ///   An array of continuous values trying to approximate the measurement array.
        /// </param>
        /// 
        public ReceiverOperatingCharacteristic(bool[] expected, int[] actual)
        {
            // Initial argument checking
            if (expected.Length != actual.Length)
                throw new ArgumentException("The size of the measurement and prediction arrays must match.");

            double[] dexpected = new double[expected.Length];
            for (int i = 0; i < dexpected.Length; i++)
                dexpected[i] = expected[i] ? 1 : 0;

            double[] dactual = new double[actual.Length];
            for (int i = 0; i < dactual.Length; i++)
                dactual[i] = actual[i];

            init(dexpected, dactual);
        }

        /// <summary>
        ///   Constructs a new Receiver Operating Characteristic model
        /// </summary>
        /// 
        /// <param name="expected">
        ///   An array of binary values. Typically represented as 0 and 1, or -1 and 1,
        ///   indicating negative and positive cases, respectively. The maximum value
        ///   will be treated as the positive case, and the lowest as the negative.</param>
        /// <param name="actual">
        ///   An array of continuous values trying to approximate the measurement array.
        /// </param>
        /// 
        public ReceiverOperatingCharacteristic(int[] expected, double[] actual)
        {
            // Initial argument checking
            if (expected.Length != actual.Length)
                throw new ArgumentException("The size of the measurement and prediction arrays must match.");

            double[] dexpected = new double[expected.Length];
            for (int i = 0; i < dexpected.Length; i++)
                dexpected[i] = expected[i];

            init(dexpected, actual);
        }

        /// <summary>
        ///   Constructs a new Receiver Operating Characteristic model
        /// </summary>
        /// 
        /// <param name="expected">
        ///   An array of binary values. Typically represented as 0 and 1, or -1 and 1,
        ///   indicating negative and positive cases, respectively. The maximum value
        ///   will be treated as the positive case, and the lowest as the negative.</param>
        /// <param name="actual">
        ///   An array of continuous values trying to approximate the measurement array.
        /// </param>
        /// 
        public ReceiverOperatingCharacteristic(double[] expected, double[] actual)
        {
            // Initial argument checking
            if (expected.Length != actual.Length)
                throw new ArgumentException("The size of the measurement and prediction arrays must match.");

            init(expected, actual);
        }

        private void init(double[] expected, double[] actual)
        {
            this.measurement = expected;
            this.prediction = actual;

            // Determine which numbers correspond to each binary category
            dtrue = dfalse = expected[0];
            for (int i = 1; i < expected.Length; i++)
            {
                if (dtrue < expected[i]) dtrue = expected[i];
                if (dfalse > expected[i]) dfalse = expected[i];
            }

            // Count the real number of positive and negative cases
            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] == dtrue)
                    this.positiveCount++;
            }

            min = actual.Min();
            max = actual.Max();

            // Negative cases is just the number of cases minus the number of positives
            this.negativeCount = this.measurement.Length - this.positiveCount;

            this.calculatePlacements();
        }



        #region Properties
        /// <summary>
        ///   Gets the points of the curve.
        /// </summary>
        /// 
        public ReceiverOperatingCharacteristicPointCollection Points
        {
            get { return collection; }
        }

        /// <summary>
        ///   Gets the number of actual positive cases.
        /// </summary>
        /// 
        public int Positives
        {
            get { return positiveCount; }
        }

        /// <summary>
        ///   Gets the number of actual negative cases.
        /// </summary>
        /// 
        public int Negatives
        {
            get { return negativeCount; }
        }

        /// <summary>
        ///   Gets the number of cases (observations) being analyzed.
        /// </summary>
        /// 
        public int Observations
        {
            get { return this.measurement.Length; }
        }

        /// <summary>
        ///   Gets the area under this curve (AUC).
        /// </summary>
        /// 
        public double Area
        {
            get { return area; }
        }

        /// <summary>
        ///   Gets the standard error for the <see cref="Area"/>.
        /// </summary>
        /// 
        public double StandardError
        {
            get { return error; }
        }

        /// <summary>
        ///   Gets the variance of the curve's <see cref="Area"/>.
        /// </summary>
        /// 
        public double Variance
        {
            get { return error * error; }
        }

        /// <summary>
        ///   Gets the ground truth values, or the values
        ///   which should have been given by the test if
        ///   it was perfect.
        /// </summary>
        /// 
        public double[] Expected
        {
            get { return measurement; }
        }

        /// <summary>
        ///   Gets the actual values given by the test.
        /// </summary>
        /// 
        public double[] Actual
        {
            get { return prediction; }
        }

        /// <summary>
        ///   Gets the actual test results for subjects 
        ///   which should have been labeled as positive.
        /// </summary>
        /// 
        public double[] PositiveResults { get { return positiveResults; } }

        /// <summary>
        ///   Gets the actual test results for subjects 
        ///   which should have been labeled as negative.
        /// </summary>
        /// 
        public double[] NegativeResults { get { return negativeResults; } }

        /// <summary>
        ///   Gets DeLong's pseudoaccuracies for the <see cref="PositiveResults">positive subjects</see>.
        /// </summary>
        /// 
        public double[] PositiveAccuracies { get { return positiveAccuracy; } }

        /// <summary>
        ///   Gets DeLong's pseudoaccuracies for the <see cref="NegativeResults">negative subjects</see>
        /// </summary>
        /// 
        public double[] NegativeAccuracies { get { return negativeAccuracy; } }

        #endregion


        #region Public Methods
        /// <summary>
        ///   Computes a n-points ROC curve.
        /// </summary>
        /// 
        /// <remarks>
        ///   Each point in the ROC curve will have a threshold increase of
        ///   1/npoints over the previous point, starting at zero.
        /// </remarks>
        /// 
        /// <param name="points">The number of points for the curve.</param>
        /// 
        public void Compute(int points)
        {
            Compute((max - min) / points);
        }

        /// <summary>
        ///   Computes a ROC curve with 1/increment points
        /// </summary>
        /// 
        /// <param name="increment">The increment over the previous point for each point in the curve.</param>
        /// 
        public void Compute(double increment)
        {
            Compute(increment, true);
        }

        /// <summary>
        ///   Computes a ROC curve with 1/increment points
        /// </summary>
        /// 
        /// <param name="increment">The increment over the previous point for each point in the curve.</param>
        /// <param name="forceOrigin">True to force the inclusion of the (0,0) point, false otherwise. Default is false.</param>
        /// 
        public void Compute(double increment, bool forceOrigin)
        {
            var points = new List<ReceiverOperatingCharacteristicPoint>();
            double cutoff;

            // Create the curve, computing a point for each cutoff value
            for (cutoff = min; cutoff <= max; cutoff += increment)
                points.Add(ComputePoint(cutoff));


            // Sort the curve by descending specificity
            points.Sort(new Comparison<ReceiverOperatingCharacteristicPoint>(order));

            if (forceOrigin)
            {
                var last = points[points.Count - 1];
                if (last.FalsePositiveRate != 0.0 || last.Sensitivity != 0.0)
                    points.Add(ComputePoint(Double.PositiveInfinity));
            }


            // Create the point collection
            this.collection = new ReceiverOperatingCharacteristicPointCollection(points.ToArray());

            // Calculate area and error associated with this curve
            this.area = calculateAreaUnderCurve();
            this.error = calculateStandardError();
        }

        /// <summary>
        ///   Computes a ROC curve with the given increment points
        /// </summary>
        /// 
        public void Compute(params double[] cutpoints)
        {
            if (cutpoints.Length == 0) throw new ArgumentException(
                "Cutpoints array must have at least one value.", "cutpoints");

            List<ReceiverOperatingCharacteristicPoint> points = new List<ReceiverOperatingCharacteristicPoint>();

            // Create the curve, computing a point for each cut-point
            for (int i = 0; i < cutpoints.Length; i++)
                points.Add(ComputePoint(cutpoints[i]));


            // Sort the curve by descending specificity
            points.Sort(new Comparison<ReceiverOperatingCharacteristicPoint>(order));


            // Create the point collection
            this.collection = new ReceiverOperatingCharacteristicPointCollection(points.ToArray());

            // Calculate area and error associated with this curve
            this.area = calculateAreaUnderCurve();
            this.error = calculateStandardError();
        }

        private static int order(ReceiverOperatingCharacteristicPoint a, ReceiverOperatingCharacteristicPoint b)
        {
            // First order by descending specificity
            int c = a.Specificity.CompareTo(b.Specificity);

            if (c == 0) // then order by ascending sensitivity
                return -a.Sensitivity.CompareTo(b.Sensitivity);
            else return c;
        }


        /// <summary>
        ///   Computes a single point of a ROC curve using the given cutoff value.
        /// </summary>
        /// 
        public ReceiverOperatingCharacteristicPoint ComputePoint(double threshold)
        {
            int truePositives = 0;
            int trueNegatives = 0;

            for (int i = 0; i < this.measurement.Length; i++)
            {
                bool actual = (this.measurement[i] == dtrue);
                bool predicted = (this.prediction[i] >= threshold);


                // If the prediction equals the true measured value
                if (predicted == actual)
                {
                    // We have a hit. Now we have to see
                    //  if it was a positive or negative hit
                    if (predicted == true)
                        truePositives++; // Positive hit
                    else trueNegatives++;// Negative hit
                }
            }

            // The other values can be computed from available variables
            int falsePositives = negativeCount - trueNegatives;
            int falseNegatives = positiveCount - truePositives;

            return new ReceiverOperatingCharacteristicPoint(threshold,
                truePositives: truePositives, falseNegatives: falseNegatives,
                falsePositives: falsePositives, trueNegatives: trueNegatives);
        }

        /// <summary>
        ///   Generates a <see cref="Scatterplot"/> representing the ROC curve.
        /// </summary>
        /// 
        /// <param name="includeRandom">
        ///   True to include a plot of the random curve (a diagonal line
        ///   going from lower left to upper right); false otherwise.</param>
        /// 
        public Scatterplot GetScatterplot(bool includeRandom = false)
        {
            Scatterplot plot = new Scatterplot("Area under the ROC curve");

            plot.XAxisTitle = "1 - Specificity";
            plot.YAxisTitle = "Sensitivity";

            double[] x = Points.GetOneMinusSpecificity();
            double[] y = Points.GetSensitivity();

            if (includeRandom)
            {
                int points = x.Length;
                double[] newx = new double[points + 2];
                double[] newy = new double[points + 2];
                int[] labels = new int[points + 2];

                Array.Copy(x, newx, x.Length);
                Array.Copy(y, newy, y.Length);

                newx[points + 0] = 0;
                newy[points + 0] = 0;
                labels[points + 0] = 1;

                newx[points + 1] = 1;
                newy[points + 1] = 1;
                labels[points + 1] = 1;

                plot.Compute(newx, newy, labels);
                plot.Classes[0].Text = "Curve";
                plot.Classes[1].Text = "Random";
            }
            else
            {
                plot.Compute(x, y);
            }

            return plot;
        }



        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this curve.
        /// </summary>
        /// <returns>
        ///   A <see cref="System.String"/> that represents this curve.
        /// </returns>
        public override string ToString()
        {
            // TODO: Implement support for custom formats
            return String.Format(System.Globalization.CultureInfo.CurrentCulture,
                "AUC:{0:0.0} Error:{1:0.0} Positives:{2} Negatives:{3}",
                 this.Area, this.StandardError, this.Positives, this.Negatives);
        }
        #endregion


        #region Private Methods
        /// <summary>
        ///   Calculates the area under the ROC curve using the trapezium method.
        /// </summary>
        /// <remarks>
        ///   The area under a ROC curve can never be less than 0.50. If the area is first calculated as
        ///   less than 0.50, the definition of abnormal will be reversed from a higher test value to a
        ///   lower test value.
        /// </remarks>
        private double calculateAreaUnderCurve()
        {
            double sum = 0.0;
            double tpz = 0.0;

            for (int i = 0; i < collection.Count - 1; i++)
            {
                // Obs: False Positive Rate = (1-specificity)
                tpz = collection[i].Sensitivity + collection[i + 1].Sensitivity;
                tpz = tpz * (collection[i].FalsePositiveRate - collection[i + 1].FalsePositiveRate) / 2.0;
                sum += tpz;
            }

            if (sum < 0.5)
                return 1.0 - sum;
            else return sum;
        }

        private double calculateStandardError()
        {
            return Math.Sqrt(ReceiverOperatingCurveTest.DeLongVariance(positiveAccuracy, negativeAccuracy));
        }

        private void calculatePlacements()
        {
            // Get ratings for true positives
            int[] positiveIndices = this.measurement.Find(x => x == dtrue);
            double[] X = this.prediction.Submatrix(positiveIndices);

            int[] negativeIndices = this.measurement.Find(x => x == dfalse);
            double[] Y = this.prediction.Submatrix(negativeIndices);

            positiveResults = X;
            negativeResults = Y;

            // Transform into V variables
            positiveAccuracy = X.Apply(x => v(x, Y));
            negativeAccuracy = Y.Apply(y => 1.0 - v(y, X));
        }

        private static double v(double x, double[] y)
        {
            double sum = 0;
            for (int i = 0; i < y.Length; i++)
            {
                if (y[i] < x)
                    sum += 1;
                if (y[i] == x)
                    sum += 0.5;
            }

            return sum / y.Length;
        }

        #endregion




        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            min = dfalse;
            max = dtrue;

            calculatePlacements();
        }

        [OnDeserializing]
        private void onDeserializing(StreamingContext context)
        {
            min = max = 0;
        }


        /// <summary>
        ///   Saves the curve to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the curve is to be serialized.</param>
        /// 
        public void Save(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
        }

        /// <summary>
        ///   Loads a curve from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the curve is to be deserialized.</param>
        /// 
        /// <returns>The deserialized curve.</returns>
        /// 
        public static ReceiverOperatingCharacteristic Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (ReceiverOperatingCharacteristic)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a curve from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the curve is to be deserialized.</param>
        /// 
        /// <returns>The deserialized curve.</returns>
        /// 
        public static ReceiverOperatingCharacteristic Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load(fs);
            }
        }

        /// <summary>
        ///   Saves the curve to a stream.
        /// </summary>
        /// 
        /// <param name="path">The path to the file to which the curve is to be serialized.</param>
        /// 
        public void Save(String path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                Save(fs);
            }
        }
    }


    /// <summary>
    ///   Object to hold information about a Receiver Operating Characteristic Curve Point
    /// </summary>
    /// 
    [Serializable]
    public class ReceiverOperatingCharacteristicPoint : ConfusionMatrix
    {

        // Discrimination threshold (cutoff value)
        private double cutoff;


        /// <summary>
        ///   Constructs a new Receiver Operating Characteristic point.
        /// </summary>
        /// 
        internal ReceiverOperatingCharacteristicPoint(double cutoff,
            int truePositives, int falseNegatives,
            int falsePositives, int trueNegatives)
            : base(truePositives: truePositives, falseNegatives: falseNegatives,
                   falsePositives: falsePositives, trueNegatives: trueNegatives)
        {
            this.cutoff = cutoff;
        }


        /// <summary>
        ///   Gets the cutoff value (discrimination threshold) for this point.
        /// </summary>
        /// 
        public double Cutoff
        {
            get { return cutoff; }
        }

        /// <summary>
        ///   Returns a System.String that represents the current ReceiverOperatingCharacteristicPoint.
        /// </summary>
        /// 
        public override string ToString()
        {
            return String.Format(System.Globalization.CultureInfo.CurrentCulture,
                "({0}; {1})", Sensitivity, FalsePositiveRate);
        }

    }

    /// <summary>
    ///   Represents a Collection of Receiver Operating Characteristic (ROC) Curve points.
    ///   This class cannot be instantiated.
    /// </summary>
    /// 
    [Serializable]
    public class ReceiverOperatingCharacteristicPointCollection : ReadOnlyCollection<ReceiverOperatingCharacteristicPoint>
    {
        internal ReceiverOperatingCharacteristicPointCollection(ReceiverOperatingCharacteristicPoint[] points)
            : base(points) { }

        /// <summary>
        ///   Gets the (1-specificity, sensitivity) values as (x,y) coordinates.
        /// </summary>
        /// 
        /// <returns>
        ///   An jagged double array where each element is a double[] vector
        ///   with two positions; the first is the value for 1-specificity (x)
        ///   and the second the value for sensitivity (y).
        /// </returns>
        /// 
        public double[][] GetXYValues()
        {
            double[] x = new double[this.Count];
            double[] y = new double[this.Count];

            for (int i = 0; i < Count; i++)
            {
                x[i] = 1.0 - this[i].Specificity;
                y[i] = this[i].Sensitivity;
            }

            double[][] points = new double[this.Count][];
            for (int i = 0; i < points.Length; i++)
                points[i] = new[] { x[i], y[i] };

            return points;
        }

        /// <summary>
        ///   Gets an array containing (1-specificity) 
        ///   values for each point in the curve.
        /// </summary>
        /// 
        public double[] GetOneMinusSpecificity()
        {
            double[] x = new double[this.Count];
            for (int i = 0; i < x.Length; i++)
                x[i] = 1.0 - this[i].Specificity;
            return x;
        }

        /// <summary>
        ///   Gets an array containing (sensitivity) 
        ///   values for each point in the curve.
        /// </summary>
        /// 
        public double[] GetSensitivity()
        {
            double[] y = new double[this.Count];
            for (int i = 0; i < y.Length; i++)
                y[i] = this[i].Sensitivity;
            return y;
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            return Count.ToString();
        }

    }

}
