// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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

namespace Accord.MachineLearning.Bayes
{
    using Accord.Math;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;

    // TODO: Replace the example here
    /// <summary>
    ///   Naïve Bayes Classifier for arbitrary distributions.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   A naive Bayes classifier is a simple probabilistic classifier based on applying Bayes' theorem
    ///   with strong (naive) independence assumptions. A more descriptive term for the underlying probability
    ///   model would be "independent feature model".</para>
    /// <para>
    ///   In simple terms, a naive Bayes classifier assumes that the presence (or absence) of a particular
    ///   feature of a class is unrelated to the presence (or absence) of any other feature, given the class
    ///   variable. In spite of their naive design and apparently over-simplified assumptions, naive Bayes 
    ///   classifiers have worked quite well in many complex real-world situations.</para>
    ///   
    /// <para>
    ///   This class implements an arbitrary-distribution (real-valued) Naive-Bayes classifier. There is 
    ///   also a special <see cref="NaiveBayes.Normal(int, int)">named constructor to create classifiers 
    ///   assuming normal distributions for each variable</see>. For a discrete (integer-valued) distribution 
    ///   classifier, please see <see cref="NaiveBayes"/>. </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia contributors. "Naive Bayes classifier." Wikipedia, The Free Encyclopedia.
    ///       Wikipedia, The Free Encyclopedia, 16 Dec. 2011. Web. 5 Jan. 2012.</description></item>
    ///   </list>
    /// </para>  
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This page contains two examples, one using text and another one using normal double vectors.
    ///   The first example is the classic example given by Tom Mitchell. If you are not interested
    ///   in text or in this particular example, please jump to the second example below.</para>
    ///   
    /// <para>
    ///   In the first example, we will be using a mixed-continuous version of the famous Play Tennis
    ///   example by Tom Mitchell (1998). In Mitchell's example, one would like to infer if a person
    ///   would play tennis or not based solely on four input variables. The original variables were
    ///   categorical, but in this example, two of them will be categorical and two will be continuous.
    ///   The rows, or instances presented below represent days on which the behavior of the person
    ///   has been registered and annotated, pretty much building our set of observation instances for
    ///   learning:</para>
    /// 
    /// <code>
    ///   DataTable data = new DataTable("Mitchell's Tennis Example");
    /// 
    ///   data.Columns.Add("Day", "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");
    ///
    ///   // We will set Temperature and Humidity to be continuous
    ///   data.Columns["Temperature"].DataType = typeof(double); // (degrees Celsius)
    ///   data.Columns["Humidity"].DataType    = typeof(double); // (water percentage)
    /// 
    ///   data.Rows.Add(   "D1",   "Sunny",      38.0,         96.0,    "Weak",     "No"  );
    ///   data.Rows.Add(   "D2",   "Sunny",      39.0,         90.0,   "Strong",    "No"  );
    ///   data.Rows.Add(   "D3",  "Overcast",    38.0,         75.0,    "Weak",     "Yes" );
    ///   data.Rows.Add(   "D4",   "Rain",       25.0,         87.0,    "Weak",     "Yes" );
    ///   data.Rows.Add(   "D5",   "Rain",       12.0,         30.0,    "Weak",     "Yes" );
    ///   data.Rows.Add(   "D6",   "Rain",       11.0,         35.0,   "Strong",    "No"  );
    ///   data.Rows.Add(   "D7",  "Overcast",    10.0,         40.0,   "Strong",    "Yes" );
    ///   data.Rows.Add(   "D8",   "Sunny",      24.0,         90.0,    "Weak",     "No"  );
    ///   data.Rows.Add(   "D9",   "Sunny",      12.0,         26.0,    "Weak",     "Yes" );
    ///   data.Rows.Add(   "D10",  "Rain",       25.0,         30.0,    "Weak",     "Yes" );
    ///   data.Rows.Add(   "D11",  "Sunny",      26.0,         40.0,   "Strong",    "Yes" );
    ///   data.Rows.Add(   "D12", "Overcast",    27.0,         97.0,   "Strong",    "Yes" );
    ///   data.Rows.Add(   "D13", "Overcast",    39.0,         41.0,    "Weak",     "Yes" );
    ///   data.Rows.Add(   "D14",  "Rain",       23.0,         98.0,   "Strong",    "No"  );
    /// </code>
    /// <para>
    ///   <i>Obs: The DataTable representation is not required, and instead the NaiveBayes could
    ///   also be trained directly on double[] arrays containing the data.</i></para>
    ///   
    /// <para>
    ///   In order to estimate a discrete Naive Bayes, we will first convert this problem to a more simpler
    ///   representation. Since some variables are categories, it does not matter if they are represented
    ///   as strings, or numbers, since both are just symbols for the event they represent. Since numbers
    ///   are more easily representable than text strings, we will convert the problem to use a discrete 
    ///   alphabet through the use of a <see cref="Accord.Statistics.Filters.Codification">codebook</see>.</para>
    /// 
    /// <para>
    ///   A codebook effectively transforms any distinct possible value for a variable into an integer 
    ///   symbol. For example, “Sunny” could as well be represented by the integer label 0, “Overcast” 
    ///   by “1”, Rain by “2”, and the same goes by for the other variables. So:</para>
    /// 
    /// <code>
    ///   // Create a new codification codebook to 
    ///   // convert strings into integer symbols
    ///   Codification codebook = new Codification(data,
    ///     "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");
    ///   
    ///   // Translate our training data into integer symbols using our codebook:
    ///   DataTable symbols = codebook.Apply(data); 
    ///   double[][] inputs  = symbols.ToArray("Outlook", "Temperature", "Humidity", "Wind"); 
    ///   int[]      outputs = symbols.ToIntArray("PlayTennis").GetColumn(0);
    /// </code>
    /// 
    /// <para>
    ///   Now that we already have our learning input/ouput pairs, we should specify our
    ///   Bayes model. We will be trying to build a model to predict the last column, entitled
    ///   “PlayTennis”. For this, we will be using the “Outlook”, “Temperature”, “Humidity” and
    ///   “Wind” as predictors (variables which will we will use for our decision).
    /// </para>
    /// 
    /// <code>
    ///   // Gather information about decision variables
    ///   IUnivariateDistribution[] priors =
    ///   {
    ///       new GeneralDiscreteDistribution(codebook["Outlook"].Symbols),   // 3 possible values (Sunny, overcast, rain)
    ///       new NormalDistribution(),                                       // Continuous value (Celsius)
    ///       new NormalDistribution(),                                       // Continuous value (percentage)
    ///       new GeneralDiscreteDistribution(codebook["Wind"].Symbols)       // 2 possible values (Weak, strong)
    ///   };
    ///   
    ///   int inputCount = 4;       // 4 variables (Outlook, Temperature, Humidity, Wind)
    ///   int classCount = codebook["PlayTennis"].Symbols; // 2 possible values (yes, no)
    ///
    ///   // Create a new Naive Bayes classifiers for the two classes
    ///   var model = new NaiveBayes&lt;IUnivariateDistribution>(classCount, inputCount, priors);
    ///
    ///   // Compute the Naive Bayes model
    ///   model.Estimate(inputs, outputs);
    /// </code>
    /// 
    /// <para>Now that we have created and estimated our classifier, we 
    /// can query the classifier for new input samples through the <see
    /// cref="NaiveBayes{T}.Compute(double[])"/> method.</para>
    /// 
    /// <code>
    /// // We will be computing the output label for a sunny, cool, humid and windy day:
    /// 
    /// double[] instance = new double[] 
    /// {
    ///     codebook.Translate(columnName:"Outlook", value:"Sunny"), 
    ///     12.0, 
    ///     90.0,
    ///     codebook.Translate(columnName:"Wind", value:"Strong")
    /// };
    /// 
    /// // Now, we can feed this instance to our model
    /// int output = model.Compute(instance, out logLikelihood);
    /// 
    /// // Finally, the result can be translated back to one of the codewords using
    /// string result = codebook.Translate("PlayTennis", output); // result is "No"
    /// </code>
    /// 
    /// <para> 
    ///   </para>
    ///   
    /// <para>
    ///   In this second example, we will be creating a simple multi-class
    ///   classification problem using integer vectors and learning a discrete
    ///   Naive Bayes on those vectors.</para>
    /// 
    /// <code>
    /// // Let's say we have the following data to be classified
    /// // into three possible classes. Those are the samples:
    /// //
    /// double[][] inputs =
    /// {
    ///     //               input         output
    ///     new double[] { 0, 1, 1, 0 }, //  0 
    ///     new double[] { 0, 1, 0, 0 }, //  0
    ///     new double[] { 0, 0, 1, 0 }, //  0
    ///     new double[] { 0, 1, 1, 0 }, //  0
    ///     new double[] { 0, 1, 0, 0 }, //  0
    ///     new double[] { 1, 0, 0, 0 }, //  1
    ///     new double[] { 1, 0, 0, 0 }, //  1
    ///     new double[] { 1, 0, 0, 1 }, //  1
    ///     new double[] { 0, 0, 0, 1 }, //  1
    ///     new double[] { 0, 0, 0, 1 }, //  1
    ///     new double[] { 1, 1, 1, 1 }, //  2
    ///     new double[] { 1, 0, 1, 1 }, //  2
    ///     new double[] { 1, 1, 0, 1 }, //  2
    ///     new double[] { 0, 1, 1, 1 }, //  2
    ///     new double[] { 1, 1, 1, 1 }, //  2
    /// };
    /// 
    /// int[] outputs = // those are the class labels
    /// {
    ///     0, 0, 0, 0, 0,
    ///     1, 1, 1, 1, 1,
    ///     2, 2, 2, 2, 2,
    /// };
    /// 
    /// // Create a new continuous naive Bayes model for 3 classes using 4-dimensional Gaussian distributions
    /// var bayes = new NaiveBayes&lt;NormalDistribution>(inputs: 4, classes: 3, initial: NormalDistribution.Standard);
    /// 
    /// // Teach the Naive Bayes model. The error should be zero:
    /// double error = bayes.Estimate(inputs, outputs, options: new NormalOptions
    /// {
    ///     Regularization = 1e-5 // to avoid zero variances
    /// });
    /// 
    /// // Now, let's test  the model output for the first input sample:
    /// int answer = bayes.Compute(new double[] { 0, 1, 1, 0 }); // should be 1
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="NaiveBayes"/>
    /// 
    [Serializable]
    public class NaiveBayes<TDistribution> : GenerativeClassifierBase
        where TDistribution : IFittableDistribution<double>
    {

        private TDistribution[,] probabilities;
        private double[] priors;

        #region Constructors

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="initial">
        ///   An initial distribution to be used to initialized all independent
        ///   distribution components of this Naive Bayes. This distribution will
        ///   be cloned and made available in the <see cref="Distributions"/> property.
        /// </param>
        /// 
        public NaiveBayes(int classes, int inputs, TDistribution initial)
        {
            if (classes <= 0)
                throw new ArgumentOutOfRangeException("classes");

            if (inputs <= 0)
                throw new ArgumentOutOfRangeException("inputs");

            if (initial == null)
                throw new ArgumentNullException("initial");

            var priors = new TDistribution[classes, inputs];
            for (int i = 0; i < classes; i++)
                for (int j = 0; j < inputs; j++)
                    priors[i, j] = (TDistribution)initial.Clone();

            initialize(priors, null);
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="initial">
        ///   An initial distribution to be used to initialized all independent
        ///   distribution components of this Naive Bayes. This distribution will
        ///   be cloned and made available in the <see cref="Distributions"/> property.
        /// </param>
        /// <param name="classPriors">The prior probabilities for each output class.</param>
        /// 
        public NaiveBayes(int classes, int inputs, TDistribution initial, double[] classPriors)
        {
            if (classes <= 0)
                throw new ArgumentOutOfRangeException("classes");

            if (inputs <= 0)
                throw new ArgumentOutOfRangeException("inputs");

            if (classPriors == null)
                throw new ArgumentNullException("classPriors");

            if (initial == null)
                throw new ArgumentNullException("initial");

            if (classPriors.Length != classes)
                throw new DimensionMismatchException("classPriors");

            var priors = new TDistribution[classPriors.Length, inputs];
            for (int i = 0; i < classPriors.Length; i++)
                for (int j = 0; j < inputs; j++)
                    priors[i, j] = (TDistribution)initial.Clone();

            initialize(priors, classPriors);
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="initial">
        ///   An initial distribution to be used to initialized all independent
        ///   distribution components of this Naive Bayes. Those distributions
        ///   will made available in the <see cref="Distributions"/> property.
        /// </param>
        /// 
        public NaiveBayes(int classes, int inputs, TDistribution[] initial)
        {
            if (classes <= 0)
                throw new ArgumentOutOfRangeException("classes");

            if (inputs <= 0)
                throw new ArgumentOutOfRangeException("inputs");

            if (initial == null)
                throw new ArgumentNullException("initial");

            if (initial.Length != inputs)
                throw new DimensionMismatchException("initial");

            var priors = new TDistribution[classes, initial.Length];
            for (int i = 0; i < classes; i++)
                for (int j = 0; j < initial.Length; j++)
                    priors[i, j] = (TDistribution)initial[j].Clone();

            initialize(priors, null);
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="initial">
        ///   An initial distribution to be used to initialized all independent
        ///   distribution components of this Naive Bayes. Those distributions
        ///   will made available in the <see cref="Distributions"/> property.
        /// </param>
        /// 
        public NaiveBayes(int classes, int inputs, TDistribution[,] initial)
        {
            if (classes <= 0)
                throw new ArgumentOutOfRangeException("classes");

            if (inputs <= 0)
                throw new ArgumentOutOfRangeException("inputs");

            if (initial.GetLength(0) != classes)
                throw new DimensionMismatchException("initial");

            if (initial.GetLength(1) != inputs)
                throw new DimensionMismatchException("initial");

            initialize(initial, null);
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="initial">
        ///   An initial distribution to be used to initialized all independent
        ///   distribution components of this Naive Bayes. Those distributions
        ///   will made available in the <see cref="Distributions"/> property.
        /// </param>
        /// <param name="classPriors">The prior probabilities for each output class.</param>
        /// 
        public NaiveBayes(int classes, int inputs, TDistribution[,] initial, double[] classPriors)
        {
            if (classes <= 0)
                throw new ArgumentOutOfRangeException("classes");

            if (initial == null)
                throw new ArgumentNullException("initial");

            if (classPriors.Length != classes)
                throw new DimensionMismatchException("classPriors");

            if (initial.GetLength(0) != classes)
                throw new DimensionMismatchException("initial");

            if (initial.GetLength(1) != inputs)
                throw new DimensionMismatchException("initial");

            initialize(initial, classPriors);
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="initial">
        ///   An initial distribution to be used to initialized all independent
        ///   distribution components of this Naive Bayes. Those distributions
        ///   will made available in the <see cref="Distributions"/> property.
        /// </param>
        /// <param name="classPriors">The prior probabilities for each output class.</param>
        /// 
        public NaiveBayes(int classes, int inputs, TDistribution[] initial, double[] classPriors)
        {
            if (classes <= 0)
                throw new ArgumentOutOfRangeException("classes");

            if (inputs <= 0)
                throw new ArgumentOutOfRangeException("inputs");

            if (initial == null)
                throw new ArgumentNullException("initial");

            if (initial.Length != inputs)
                throw new DimensionMismatchException("initial");

            if (classPriors.Length != classes)
                throw new DimensionMismatchException("classPriors");

            TDistribution[,] priors = new TDistribution[classes, initial.Length];
            for (int i = 0; i < classes; i++)
                for (int j = 0; j < initial.Length; j++)
                    priors[i, j] = (TDistribution)initial[j].Clone();

            initialize(priors, classPriors);
        }

        private void initialize(TDistribution[,] initial, double[] classPriors)
        {
            this.NumberOfOutputs = initial.GetLength(0);
            this.NumberOfInputs = initial.GetLength(1);
            this.priors = classPriors;
            this.probabilities = initial;

            if (priors == null)
            {
                priors = new double[NumberOfOutputs];
                for (int i = 0; i < priors.Length; i++)
                    priors[i] = 1.0 / priors.Length;
            }
        }
        #endregion

        /// <summary>
        /// Computes the log-likelihood for one specific the class label
        /// for a given <paramref name="input">input vector</paramref>.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// any of the <see cref="ITransform{TInput, TOutput}.NumberOfOutputs" /> possible classes.</param>
        /// <param name="classIndex">The index for the class whose log-likelihood should be computed.</param>
        /// <returns>
        /// The log-likelihood for the specified <paramref name="classIndex">class label</paramref>.
        /// </returns>
        public override double LogLikelihood(double[] input, int classIndex)
        {
            double p = Math.Log(priors[classIndex]);
            for (int i = 0; i < input.Length; i++)
                p += probabilities[classIndex, i].LogProbabilityFunction(input[i]);
            return p;
        }



        /// <summary>
        ///   Gets the probability distributions for each class and input.
        /// </summary>
        /// 
        /// <value>A TDistribution[,] array in with each row corresponds to a 
        /// class, each column corresponds to an input variable. Each element
        /// of this double[,] array is a probability distribution modeling
        /// the occurrence of the input variable in the corresponding class.</value>
        /// 
        public TDistribution[,] Distributions
        {
            get { return probabilities; }
        }

        /// <summary>
        ///   Gets the prior beliefs for each class.
        /// </summary>
        /// 
        public double[] Priors
        {
            get { return priors; }
        }


        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfOutputs instead.")]
        public int ClassCount
        {
            get { return NumberOfOutputs; }
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfInputs instead.")]
        public int InputCount
        {
            get { return NumberOfInputs; }
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use NaiveBayesLearning<TDistribution> instead.")]
        public double Estimate<TOptions>(double[][] inputs, int[] outputs,
            bool empirical = true, TOptions options = null)
            where TOptions : class, IFittingOptions
        {
            var teacher = new NaiveBayesLearning<TDistribution, TOptions>(this);
            teacher.Empirical = empirical;
            teacher.Options = options;
            var result = teacher.Learn(inputs, outputs);
            this.probabilities = result.probabilities;
            this.priors = result.priors;
            return new ZeroOneLoss(outputs) { Mean = true }.Loss(Decide(inputs));
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use NaiveBayesLearning<TDistribution> instead.")]
        public double Estimate(double[][] inputs, int[] outputs,
            bool empirical = true)
        {
            var teacher = new NaiveBayesLearning<TDistribution>(this);
            teacher.Empirical = empirical;
            var result = teacher.Learn(inputs, outputs);
            this.probabilities = result.probabilities;
            this.priors = result.priors;
            return new ZeroOneLoss(outputs) { Mean = true }.Loss(Decide(inputs));
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use ZeroOneLoss function instead.")]
        public double Error(double[][] inputs, int[] outputs)
        {
            return new ZeroOneLoss(outputs) { Mean = true }.Loss(Decide(inputs));
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use Decide instead.")]
        public int Compute(double[] input)
        {
            return Decide(input);
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use Decide or LogLikelihood instead.")]
        public int Compute(double[] input, out double logLikelihood)
        {
            double[] responses;
            return Compute(input, out logLikelihood, out responses);
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use Decide or LogLikelihood instead.")]
        public int Compute(double[] input, out double logLikelihood, out double[] responses)
        {
            double[] ll = LogLikelihood(input);
            int imax;
            logLikelihood = ll.Max(out imax);
            responses = Special.Softmax(ll);
            return imax;
        }





        /// <summary>
        ///   Saves the Naïve Bayes model to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the Naïve Bayes model is to be serialized.</param>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Save() instead (or use it as an extension method).")]

        public virtual void Save(Stream stream)
        {
            Accord.IO.Serializer.Save(this, stream);
        }

        /// <summary>
        ///   Saves the Naïve Bayes model to a stream.
        /// </summary>
        /// 
        /// <param name="path">The path to the file to which the Naïve Bayes model is to be serialized.</param>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Save() instead (or use it as an extension method).")]

        public void Save(string path)
        {
            Accord.IO.Serializer.Save(this, path);
        }





        #region Serialization backwards compatibility
        internal static readonly NaiveBayesBinder Binder = new NaiveBayesBinder();

        internal class NaiveBayesBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                AssemblyName name = new AssemblyName(assemblyName);

                if (name.Version < new Version(3, 1, 0))
                {
                    if (typeName.StartsWith("Accord.MachineLearning.Bayes.NaiveBayes`1"))
                        return typeof(NaiveBayes_2_13);
                }

                return null;
            }
        }

#pragma warning disable 0169
#pragma warning disable 0649

        [Serializable]
        class NaiveBayes_2_13
        {
            private TDistribution[,] probabilities;
            private double[] priors;
            private int classCount;
            private int inputCount;


            public static implicit operator NaiveBayes<TDistribution>(NaiveBayes_2_13 obj)
            {
                var nb = new NaiveBayes<TDistribution>(
                    obj.classCount, obj.inputCount,
                    obj.probabilities, obj.priors);
                return nb;
            }
        }

#pragma warning restore 0169
#pragma warning restore 0649

        #endregion
    }
}
