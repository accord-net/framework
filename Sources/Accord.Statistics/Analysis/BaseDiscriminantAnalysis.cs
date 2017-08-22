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

namespace Accord.Statistics.Analysis
{
    using System;
    using Accord.Math;
    using Accord.MachineLearning;
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   Base class for Discriminant Analysis (LDA, QDA or KDA).
    /// </summary>
    /// 
    [Serializable]
#pragma warning disable 612, 618
    public abstract class BaseDiscriminantAnalysis : TransformBase<double[], double[]>
#pragma warning restore 612, 618
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        private int numSamples;
        private int numClasses;
        private double[] totalMeans;
        private double[] totalStdDevs;
        private double threshold;

        internal int[] classCount;
        internal double[][] classMeans;
        internal double[][] classStdDevs;
        internal double[][][] classScatter;

        internal double[][] projectedMeans;

        // TODO: Use Mahalanobis distance instead of Euclidean
        // for classification, considering projection covariances.
        // internal double[][] projectedPrecision;

        private double[][] eigenvectors;
        private double[] eigenvalues;

        private double[,] result;
        private double[,] source;
        private int[] outputs;

        private double[][] Sw, Sb, St; // Scatter matrices

        private double[] discriminantProportions;
        private double[] discriminantCumulative;

        private DiscriminantCollection discriminantCollection;
        private DiscriminantAnalysisClassCollection classCollection;


        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete()]
        protected void init(double[,] inputs, int[] outputs)
        {
            // Gets the number of classes
            int startingClass = outputs.Min();
            this.NumberOfClasses = outputs.Max() - startingClass + 1;
            this.NumberOfSamples = inputs.Rows();
            this.NumberOfInputs = inputs.Columns();
            this.NumberOfOutputs = inputs.Columns();

            // Store the original data
            this.Source = inputs;
            this.Classifications = outputs;

            // Creates simple structures to hold information later
            this.classCount = new int[NumberOfClasses];
            this.classMeans = new double[NumberOfClasses][];
            this.classStdDevs = new double[NumberOfClasses][];
            this.classScatter = new double[NumberOfClasses][][];
            this.projectedMeans = new double[NumberOfClasses][];

            // Creates the object-oriented structure to hold information about the classes
            var collection = new DiscriminantAnalysisClass[NumberOfClasses];
            for (int i = 0; i < collection.Length; i++)
                collection[i] = new DiscriminantAnalysisClass(this, i, startingClass + i);
            this.Classes = new DiscriminantAnalysisClassCollection(collection);
        }

        /// <summary>
        ///   Initializes common properties.
        /// </summary>
        /// 
        protected void Init(double[][] inputs, int[] outputs)
        {
#pragma warning disable 612, 618
            if (this.Classifications != null)
                return; // TODO: remove this
#pragma warning restore 612, 618

            // Gets the number of classes
            this.NumberOfClasses = outputs.Max() + 1;
            this.NumberOfSamples = inputs.Rows();
            this.NumberOfInputs = inputs.Columns();
            //if (this.NumberOfOutputs == 0)
            //    this.NumberOfOutputs = inputs.Columns();

            // Creates simple structures to hold information later
            this.classCount = new int[NumberOfClasses];
            this.classMeans = new double[NumberOfClasses][];
            this.classStdDevs = new double[NumberOfClasses][];
            this.classScatter = new double[NumberOfClasses][][];
            this.projectedMeans = new double[NumberOfClasses][];

            // Creates the object-oriented structure to hold information about the classes
            var collection = new DiscriminantAnalysisClass[NumberOfClasses];
            for (int i = 0; i < collection.Length; i++)
                collection[i] = new DiscriminantAnalysisClass(this, i, i);
            this.Classes = new DiscriminantAnalysisClassCollection(collection);
        }


        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }


        /// <summary>
        ///   Returns the original supplied data to be analyzed.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[,] Source
        {
            get { return this.source; }
            protected set { this.source = value; }
        }

        /// <summary>
        ///   Gets or sets the minimum variance proportion needed to keep a
        ///   discriminant component. If set to zero, all components will be
        ///   kept. Default is 0.001 (all components which contribute less
        ///   than 0.001 to the variance in the data will be discarded).
        /// </summary>
        /// 
        public double Threshold
        {
            get { return threshold; }
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException("value", "Value must be between 0 and 1.");

                threshold = value;
            }
        }

        /// <summary>
        ///   Gets the resulting projection of the source data given on
        ///   the creation of the analysis into discriminant space.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public double[,] Result
        {
            get { return this.result; }
            protected set { this.result = value; }
        }

        /// <summary>
        ///   Gets the original classifications (labels) of the source data
        ///   given on the moment of creation of this analysis object.
        /// </summary>
        /// 
        [Obsolete("This property will be removed.")]
        public int[] Classifications
        {
            get { return this.outputs; }
            protected set { this.outputs = value; }
        }

        /// <summary>
        ///   Gets the number of samples used to create the analysis.
        /// </summary>
        /// 
        public int NumberOfSamples
        {
            get { return numSamples; }
            protected set { numSamples = value; }
        }

        /// <summary>
        ///   Gets the number of classes in the analysis.
        /// </summary>
        /// 
        public int NumberOfClasses
        {
            get { return numClasses; }
            protected set { numClasses = value; }
        }

        /// <summary>
        ///   Gets the mean of the original data given at method construction.
        /// </summary>
        /// 
        public double[] Means
        {
            get { return totalMeans; }
            protected set { totalMeans = value; }
        }

        /// <summary>
        ///   Gets the standard mean of the original data given at method construction.
        /// </summary>
        /// 
        public double[] StandardDeviations
        {
            get { return totalStdDevs; }
            protected set { totalStdDevs = value; }
        }

        /// <summary>
        ///   Gets the Within-Class Scatter Matrix for the data.
        /// </summary>
        /// 
        public double[][] ScatterWithinClass
        {
            get { return Sw; }
            protected set { Sw = value; }
        }

        /// <summary>
        ///   Gets the Between-Class Scatter Matrix for the data.
        /// </summary>
        /// 
        public double[][] ScatterBetweenClass
        {
            get { return Sb; }
            protected set { Sb = value; }
        }

        /// <summary>
        ///   Gets the Total Scatter Matrix for the data.
        /// </summary>
        /// 
        public double[][] ScatterMatrix
        {
            get { return St; }
            protected set { St = value; }
        }

        /// <summary>
        ///   Gets the Eigenvectors obtained during the analysis,
        ///   composing a basis for the discriminant factor space.
        /// </summary>
        /// 
        [Obsolete("Please use DiscriminantVectors.Transpose() instead.")]
        public double[,] DiscriminantMatrix
        {
            get
            {
                if (eigenvectors == null)
                    return null;
                return this.eigenvectors.Transpose().ToMatrix();
            }
        }

        /// <summary>
        ///   Gets the Eigenvectors obtained during the analysis,
        ///   composing a basis for the discriminant factor space.
        /// </summary>
        /// 
        public double[][] DiscriminantVectors
        {
            get { return this.eigenvectors; }
            protected set { this.eigenvectors = value; }
        }



        /// <summary>
        ///   Gets the Eigenvalues found by the analysis associated
        ///   with each vector of the ComponentMatrix matrix.
        /// </summary>
        /// 
        public double[] Eigenvalues
        {
            get { return eigenvalues; }
            protected set { eigenvalues = value; }
        }

        /// <summary>
        ///   Gets the level of importance each discriminant factor has in
        ///   discriminant space. Also known as amount of variance explained.
        /// </summary>
        /// 
        public double[] DiscriminantProportions
        {
            get { return discriminantProportions; }
        }

        /// <summary>
        ///   The cumulative distribution of the discriminants factors proportions.
        ///   Also known as the cumulative energy of the first dimensions of the discriminant
        ///   space or as the amount of variance explained by those dimensions.
        /// </summary>
        /// 
        public double[] CumulativeProportions
        {
            get { return discriminantCumulative; }
        }

        /// <summary>
        ///   Gets the discriminant factors in a object-oriented fashion.
        /// </summary>
        /// 
        public DiscriminantCollection Discriminants
        {
            get { return discriminantCollection; }
            protected set { discriminantCollection = value; }
        }

        /// <summary>
        ///   Gets information about the distinct classes in the analyzed data.
        /// </summary>
        ///   
        public DiscriminantAnalysisClassCollection Classes
        {
            get { return classCollection; }
            protected set { classCollection = value; }
        }

        /// <summary>
        ///   Gets the Scatter matrix for each class.
        /// </summary>
        /// 
        protected double[][][] ClassScatter
        {
            get { return classScatter; }
        }

        /// <summary>
        ///   Gets the Mean vector for each class.
        /// </summary>
        /// 
        protected double[][] ClassMeans
        {
            get { return classMeans; }
        }

        /// <summary>
        ///   Gets the feature space mean of the projected data.
        /// </summary>
        /// 
        protected double[][] ProjectionMeans
        {
            get { return projectedMeans; }
        }

        /// <summary>
        ///   Gets the Standard Deviation vector for each class.
        /// </summary>
        /// 
        protected double[][] ClassStandardDeviations
        {
            get { return classStdDevs; }
        }

        /// <summary>
        ///   Gets the observation count for each class.
        /// </summary>
        /// 
        protected int[] ClassCount
        {
            get { return classCount; }
        }



        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please set NumberOfOutputs to the desired number of dimensions and call Transform()")]
        public virtual double[,] Transform(double[,] data, int dimensions)
        {
            int previous = NumberOfOutputs;
            NumberOfOutputs = dimensions;
            var result = Transform(data.ToJagged()).ToMatrix();
            NumberOfOutputs = previous;
            return result;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please set NumberOfOutputs to the desired number of dimensions and call Transform()")]
        public virtual double[][] Transform(double[][] data, int dimensions)
        {
            int previous = NumberOfOutputs;
            NumberOfOutputs = dimensions;
            var result = Transform(data);
            NumberOfOutputs = previous;
            return result;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please set NumberOfOutputs to the desired number of dimensions and call Transform()")]
        public double[] Transform(double[] data, int discriminants)
        {
            return Transform(data.ToJagged(), discriminants).GetRow(0);
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use jagged matrices instead.")]
        public double[,] Transform(double[,] data)
        {
            return Transform(data.ToJagged(), NumberOfOutputs).ToMatrix();
        }

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public override double[] Transform(double[] input)
        {
            return Transform(new[] { input }, new[] { new double[NumberOfOutputs] })[0];
        }

        /// <summary>
        /// Applies the transformation to an input, producing an associated output.
        /// </summary>
        /// <param name="input">The input data to which the transformation should be applied.</param>
        /// <returns>
        /// The output generated by applying this transformation to the given input.
        /// </returns>
        public override double[][] Transform(double[][] input)
        {
            return Transform(input, Jagged.Zeros(input.Length, NumberOfOutputs));
        }

        /// <summary>
        ///   Returns the minimum number of discriminant space dimensions (discriminant
        ///   factors) required to represent a given percentile of the data.
        /// </summary>
        /// 
        /// <param name="threshold">The percentile of the data requiring representation.</param>
        /// <returns>The minimal number of dimensions required.</returns>
        /// 
        public int GetNumberOfDimensions(double threshold)
        {
            if (threshold < 0 || threshold > 1.0)
                throw new ArgumentException("Threshold should be a value between 0 and 1", "threshold");

            for (int i = 0; i < discriminantCumulative.Length; i++)
            {
                if (discriminantCumulative[i] >= threshold)
                    return i + 1;
            }

            return discriminantCumulative.Length;
        }

        /// <summary>
        ///   Returns the number of discriminant space dimensions (discriminant
        ///   factors) whose variance is greater than a given threshold.
        /// </summary>
        /// 
        protected static int GetNonzeroEigenvalues(double[] evals, double threshold)
        {
            // Calculate proportions
            double sum = 0.0;
            for (int i = 0; i < evals.Length; i++)
                sum += System.Math.Abs(evals[i]);
            sum = (sum == 0) ? 0 : (1.0 / sum);

            double[] proportions = new double[evals.Length];
            for (int i = 0; i < evals.Length; i++)
                proportions[i] = System.Math.Abs(evals[i]) * sum;

            for (int i = 0; i < proportions.Length; i++)
            {
                if (proportions[i] < threshold)
                    return i;
            }

            return 0;
        }

        /// <summary>
        ///   Classifies a new instance into one of the available classes.
        /// </summary>
        /// 
        [Obsolete("Please use Classifier.Decide() instead.")]
        public abstract int Classify(double[] input);

        /// <summary>
        ///   Classifies a new instance into one of the available classes.
        /// </summary>
        /// 
        [Obsolete("Please use Classifier.Decide() or Classifier.Scores() instead.")]
        public abstract int Classify(double[] input, out double[] responses);

        /// <summary>
        ///   Classifies new instances into one of the available classes.
        /// </summary>
        /// 
        [Obsolete("Please use Classifier.Decide() instead.")]
        public abstract int[] Classify(double[][] inputs);

        /// <summary>
        ///   Gets the output of the discriminant function for a given class.
        /// </summary>
        /// 
        public abstract double DiscriminantFunction(double[] input, int classIndex);

        /// <summary>
        ///   Creates additional information about principal components.
        /// </summary>
        /// 
        protected void CreateDiscriminants()
        {
            if (classCollection == null)
            {
                // Creates simple structures to hold information later
                this.classCount = new int[NumberOfOutputs];
                this.classMeans = new double[NumberOfOutputs][];
                this.classStdDevs = new double[NumberOfOutputs][];
                this.classScatter = new double[NumberOfOutputs][][];
                this.projectedMeans = new double[NumberOfOutputs][];


                // Creates the object-oriented structure to hold information about the classes
                var collection = new DiscriminantAnalysisClass[NumberOfOutputs];
                for (int i = 0; i < collection.Length; i++)
                    collection[i] = new DiscriminantAnalysisClass(this, i, i);
                this.classCollection = new DiscriminantAnalysisClassCollection(collection);
            }

            int numDiscriminants = eigenvalues.Length;
            discriminantProportions = new double[numDiscriminants];
            discriminantCumulative = new double[numDiscriminants];


            // Calculate total scatter matrix
            St = Sw.Add(Sb);

            // Calculate proportions
            double sum = 0.0;
            for (int i = 0; i < numDiscriminants; i++)
                sum += System.Math.Abs(eigenvalues[i]);
            sum = (sum == 0) ? 0 : (1.0 / sum);

            for (int i = 0; i < numDiscriminants; i++)
                discriminantProportions[i] = System.Math.Abs(eigenvalues[i]) * sum;


            // Calculate cumulative proportions
            if (numDiscriminants > 0)
            {
                this.discriminantCumulative[0] = this.discriminantProportions[0];
                for (int i = 1; i < this.discriminantCumulative.Length; i++)
                    this.discriminantCumulative[i] = this.discriminantCumulative[i - 1] + this.discriminantProportions[i];
            }


            // Creates the object-oriented structure to hold the linear discriminants
            var discriminants = new Discriminant[numDiscriminants];
            for (int i = 0; i < discriminants.Length; i++)
                discriminants[i] = new Discriminant(this, i);
            this.discriminantCollection = new DiscriminantCollection(discriminants);
        }


    }

}


