// Accord Machine Learning Library
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

namespace Accord.MachineLearning.Bayes
{
    using System;
    using Accord.Math;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using Accord.Statistics.Distributions;


    /// <summary>
    ///   Naïve Bayes Classifier.
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
    ///   In this example, we will be using the famous Play Tennis example by Tom Mitchell (1998).
    ///   In Mitchell's example, one would like to infer if a person would play tennis or not
    ///   based solely on four input variables. Those variables are all categorical, meaning that
    ///   there is no order between the possible values for the variable (i.e. there is no order
    ///   relationship between Sunny and Rain, one is not bigger nor smaller than the other, but are 
    ///   just distinct). Moreover, the rows, or instances presented below represent days on which the
    ///   behavior of the person has been registered and annotated, pretty much building our set of 
    ///   observation instances for learning:</para>
    /// 
    /// <code>
    ///   DataTable data = new DataTable("Mitchell's Tennis Example");
    ///   
    ///   data.Columns.Add("Day", "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");
    ///   
    ///   data.Rows.Add(   "D1",   "Sunny",      "Hot",       "High",   "Weak",    "No"  );
    ///   data.Rows.Add(   "D2",   "Sunny",      "Hot",       "High",  "Strong",   "No"  ); 
    ///   data.Rows.Add(   "D3",  "Overcast",    "Hot",       "High",   "Weak",    "Yes" );
    ///   data.Rows.Add(   "D4",   "Rain",       "Mild",      "High",   "Weak",    "Yes" ); 
    ///   data.Rows.Add(   "D5",   "Rain",       "Cool",     "Normal",  "Weak",    "Yes" ); 
    ///   data.Rows.Add(   "D6",   "Rain",       "Cool",     "Normal", "Strong",   "No"  ); 
    ///   data.Rows.Add(   "D7",  "Overcast",    "Cool",     "Normal", "Strong",   "Yes" );
    ///   data.Rows.Add(   "D8",   "Sunny",      "Mild",      "High",   "Weak",    "No"  );  
    ///   data.Rows.Add(   "D9",   "Sunny",      "Cool",     "Normal",  "Weak",    "Yes" ); 
    ///   data.Rows.Add(   "D10", "Rain",        "Mild",     "Normal",  "Weak",    "Yes" ); 
    ///   data.Rows.Add(   "D11",  "Sunny",      "Mild",     "Normal", "Strong",   "Yes" );
    ///   data.Rows.Add(   "D12", "Overcast",    "Mild",      "High",  "Strong",   "Yes" ); 
    ///   data.Rows.Add(   "D13", "Overcast",    "Hot",      "Normal",  "Weak",    "Yes" ); 
    ///   data.Rows.Add(   "D14",  "Rain",       "Mild",      "High",  "Strong",   "No"  );
    /// </code>
    /// <para>
    ///   <i>Obs: The DataTable representation is not required, and instead the NaiveBayes could
    ///   also be trained directly on integer arrays containing the integer codewords.</i></para>
    ///   
    /// <para>
    ///   In order to estimate a discrete Naive Bayes, we will first convert this problem to a more simpler
    ///   representation. Since all variables are categories, it does not matter if they are represented
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
    ///   Codification codebook = new Codification(data);
    ///   
    ///   // Translate our training data into integer symbols using our codebook:
    ///   DataTable symbols = codebook.Apply(data); 
    ///   int[][] inputs  = symbols.ToIntArray("Outlook", "Temperature", "Humidity", "Wind"); 
    ///   int[]   outputs = symbols.ToIntArray("PlayTennis").GetColumn(0);
    /// </code>
    /// 
    /// <para>
    ///   Now that we already have our learning input/ouput pairs, we should specify our
    ///   Bayes model. We will be trying to build a model to predict the last column, entitled
    ///   “PlayTennis”. For this, we will be using the “Outlook”, “Temperature”, “Humidity” and
    ///   “Wind” as predictors (variables which will we will use for our decision). Since those
    ///   are categorical, we must specify, at the moment of creation of our Bayes model, the
    ///   number of each possible symbols for those variables.
    /// </para>
    /// 
    /// <code>
    ///   // Gather information about decision variables
    ///   int[] symbolCounts =
    ///   {
    ///     codebook["Outlook"].Symbols,     // 3 possible values (Sunny, overcast, rain)
    ///     codebook["Temperature"].Symbols, // 3 possible values (Hot, mild, cool)
    ///     codebook["Humidity"].Symbols,    // 2 possible values (High, normal)
    ///     codebook["Wind"].Symbols         // 2 possible values (Weak, strong)
    ///   };
    ///   
    ///   int classCount = codebook["PlayTennis"].Symbols; // 2 possible values (yes, no)
    ///
    ///   // Create a new Naive Bayes classifiers for the two classes
    ///   NaiveBayes target = new NaiveBayes(classCount, symbolCounts);
    ///   
    ///   // Compute the Naive Bayes model
    ///   target.Estimate(inputs, outputs);
    /// </code>
    /// 
    /// <para>Now that we have created and estimated our classifier, we 
    /// can query the classifier for new input samples through the <see
    /// cref="NaiveBayes.Compute(int[])"/> method.</para>
    /// 
    /// <code>
    /// // We will be computing the label for a sunny, cool, humid and windy day:
    /// int[] instance = codebook.Translate("Sunny", "Cool", "High", "Strong");
    /// 
    /// // Now, we can feed this instance to our model
    /// int output = model.Compute(instance, out logLikelihood);
    /// 
    /// // Finally, the result can be translated back to one of the codewords using
    /// string result = codebook.Translate("PlayTennis", output); // result is "No"
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="NaiveBayes{T}"/>
    /// 
    [Serializable]
    public class NaiveBayes
    {

        private double[,][] probabilities;
        private double[] priors;
        private int[] symbols;
        private int classCount;


        #region Constructors
        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="symbols">The number of symbols for each input variable.</param>
        /// 
        public NaiveBayes(int classes, params int[] symbols)
        {
            if (classes <= 0) throw new ArgumentOutOfRangeException("classes");
            if (symbols == null) throw new ArgumentNullException("symbols");

            initialize(classes, symbols, null);
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="classPriors">The prior probabilities for each output class.</param>
        /// <param name="symbols">The number of symbols for each input variable.</param>
        /// 
        public NaiveBayes(int classes, double[] classPriors, params int[] symbols)
        {
            if (classes <= 0) throw new ArgumentOutOfRangeException("classes");
            if (classPriors == null) throw new ArgumentNullException("classPriors");
            if (symbols == null) throw new ArgumentNullException("symbols");

            if (classPriors.Length != classes) throw new DimensionMismatchException("classPriors");

            initialize(classPriors.Length, symbols, classPriors);
        }

        private void initialize(int classes, int[] symbols, double[] priors)
        {
            this.classCount = classes;
            this.symbols = symbols;

            if (priors == null)
            {
                priors = new double[classes];
                for (int i = 0; i < priors.Length; i++)
                    priors[i] = 1.0 / priors.Length;
            }

            this.priors = priors;
            this.probabilities = new double[classes, symbols.Length][];

            for (int i = 0; i < classes; i++)
                for (int j = 0; j < symbols.Length; j++)
                    this.probabilities[i, j] = new double[symbols[j]];
        }
        #endregion


        /// <summary>
        ///   Gets the number of possible output classes.
        /// </summary>
        /// 
        public int ClassCount
        {
            get { return classCount; }
        }

        /// <summary>
        ///   Gets the number of inputs in the model.
        /// </summary>
        /// 
        public int InputCount
        {
            get { return symbols.Length; }
        }

        /// <summary>
        ///   Gets the number of symbols for each input in the model.
        /// </summary>
        /// 
        public int[] SymbolCount
        {
            get { return symbols; }
        }

        /// <summary>
        ///   Gets the tables of log-probabilities for the frequency of
        ///   occurrence of each symbol for each class and input.
        /// </summary>
        /// 
        /// <value>A double[,] array in with each row corresponds to a 
        /// class, each column corresponds to an input variable. Each
        /// element of this double[,] array is a frequency table containing
        /// the frequency of each symbol for the corresponding variable as
        /// a double[] array.</value>
        /// 
        public double[,][] Distributions
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
        ///   Initializes the frequency tables of a Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="inputs">The input data.</param>
        /// <param name="outputs">The corresponding output labels for the input data.</param>
        /// <param name="empirical">True to estimate class priors from the data, false otherwise.</param>
        /// <param name="regularization">
        ///   The amount of regularization to be used in the m-estimator. 
        ///   Default is 1e-5.</param>
        /// 
        public double Estimate(int[][] inputs, int[] outputs,
            bool empirical = true, double regularization = 1e-5)
        {
            if (inputs == null) throw new ArgumentNullException("inputs");
            if (outputs == null) throw new ArgumentNullException("outputs");
            if (inputs.Length == 0) throw new ArgumentException("The array has zero length.", "inputs");
            if (outputs.Length != inputs.Length) throw new DimensionMismatchException("outputs");

            // For each class
            for (int i = 0; i < classCount; i++)
            {
                // Estimate conditional distributions

                // Get variables values in class i
                var idx = outputs.Find(y => y == i);
                var values = inputs.Submatrix(idx);

                if (empirical)
                    priors[i] = (double)idx.Length / inputs.Length;



                // For each variable (col)
                for (int j = 0; j < symbols.Length; j++)
                {
                    // Count value occurrences and store
                    // frequencies to form probabilities
                    double[] frequencies = new double[symbols[j]];

                    // For each input row (instance)
                    // belonging to the current class
                    for (int k = 0; k < values.Length; k++)
                    {
                        int symbol = values[k][j];
                        frequencies[symbol]++;
                    }

                    // Transform into probabilities
                    for (int k = 0; k < frequencies.Length; k++)
                    {
                        // Use a M-estimator using the previously
                        // available probabilities as priors.
                        double prior = probabilities[i, j][k];
                        double num = frequencies[k] + regularization * prior;
                        double den = values.Length + regularization;

                        probabilities[i, j][k] = num / den;
                    }
                }
            }

            // Compute learning error
            int miss = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                if (Compute(inputs[i]) != outputs[i])
                    miss++;
            }

            return (double)miss / inputs.Length;
        }

        /// <summary>
        ///   Computes the most likely class for a given instance.
        /// </summary>
        /// 
        /// <param name="input">The input instance.</param>
        /// <returns>The most likely class for the instance.</returns>
        /// 
        public int Compute(int[] input)
        {
            double logLikelihood;
            double[] responses;

            return Compute(input, out logLikelihood, out responses);
        }

        /// <summary>
        ///   Computes the most likely class for a given instance.
        /// </summary>
        /// 
        /// <param name="input">The input instance.</param>
        /// <param name="logLikelihood">The log-likelihood for the instance.</param>
        /// <param name="responses">The response probabilities for each class.</param>
        /// <returns>The most likely class for the instance.</returns>
        /// 
        public int Compute(int[] input, out double logLikelihood, out double[] responses)
        {
            // Select the class argument which
            //   maximizes the log-likelihood:

            responses = new double[ClassCount];

            for (int i = 0; i < responses.Length; i++)
                responses[i] = this.logLikelihood(i, input);

            // Get the class with maximum log-likelihood
            int argmax; logLikelihood = responses.Max(out argmax);

            double evidence = 0;
            for (int i = 0; i < responses.Length; i++)
                evidence += responses[i] = Math.Exp(responses[i]);

            // Transform back into probabilities
            responses.Divide(evidence, inPlace: true);

            return argmax;
        }

        private double logLikelihood(int c, int[] input)
        {
            double p = Math.Log(priors[c]);

            // For each variable
            for (int i = 0; i < input.Length; i++)
            {
                int symbol = input[i];
                p += Math.Log(probabilities[c, i][symbol]);
            }

            return p;
        }



        /// <summary>
        ///   Saves the Naïve Bayes model to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the Naïve Bayes model is to be serialized.</param>
        /// 
        public virtual void Save(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
        }

        /// <summary>
        ///   Saves the Naïve Bayes model to a stream.
        /// </summary>
        /// 
        /// <param name="path">The path to the file to which the Naïve Bayes model is to be serialized.</param>
        /// 
        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                Save(fs);
            }
        }

        /// <summary>
        ///   Loads a machine from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the Naïve Bayes model is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        public static NaiveBayes Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (NaiveBayes)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a machine from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the Naïve Bayes model is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        public static NaiveBayes Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load(fs);
            }
        }

        /// <summary>
        ///   Loads a machine from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the Naïve Bayes model is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        public static NaiveBayes<TDistribution> Load<TDistribution>(Stream stream)
            where TDistribution : IUnivariateDistribution
        {
            BinaryFormatter b = new BinaryFormatter();
            return (NaiveBayes<TDistribution>)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a machine from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the Naïve Bayes model is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        public static NaiveBayes<TDistribution> Load<TDistribution>(string path)
            where TDistribution : IUnivariateDistribution
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load<TDistribution>(fs);
            }
        }
    }
}
