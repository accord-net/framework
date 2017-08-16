// Accord Machine Learning Library
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

namespace Accord.MachineLearning.Bayes
{
#if !MONO

    using Accord.Math;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Compat;
    using System.Threading.Tasks;

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
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Bayes\NaiveBayes`1Test.cs" region="doc_mitchell_1" />
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
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Bayes\NaiveBayes`1Test.cs" region="doc_mitchell_2" />
    ///
    /// <para>
    ///   Now that we already have our learning input/output pairs, we should specify our
    ///   Bayes model. We will be trying to build a model to predict the last column, entitled
    ///   “PlayTennis”. For this, we will be using the “Outlook”, “Temperature”, “Humidity” and
    ///   “Wind” as predictors (variables which will we will use for our decision).
    /// </para>
    /// 
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Bayes\NaiveBayes`1Test.cs" region="doc_mitchell_3" />
    /// 
    /// <para>Now that we have created and estimated our classifier, we 
    /// can query the classifier for new input samples through the 
    /// <c>NaiveBayes{TDistribution}.Decide(double[])</c> method.</para>
    /// 
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Bayes\NaiveBayes`1Test.cs" region="doc_mitchell_4" />
    /// 
    /// <para>
    ///   In this second example, we will be creating a simple multi-class
    ///   classification problem using integer vectors and learning a discrete
    ///   Naive Bayes on those vectors.</para>
    /// 
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Bayes\NaiveBayes`1Test.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="NaiveBayes"/>
    /// <seealso cref="NaiveBayesLearning{TDistribution}"/>
    /// 
    [Serializable]
    public class NaiveBayes<TDistribution> : NaiveBayes<TDistribution, double>
        where TDistribution : IFittableDistribution<double>,
                              IUnivariateDistribution,
                              IUnivariateDistribution<double>
    {

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
            : base(classes, inputs, initial)
        {
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
            : base(classes, inputs, initial)
        {
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
            : base(classes, inputs, initial.ToJagged())
        {
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
        public NaiveBayes(int classes, int inputs, TDistribution[][] initial)
            : base(classes, inputs, initial)
        {
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="initial">
        ///   A function that can initialized the distribution components of all classes
        ///   modeled by this Naive Bayes. This distribution will be cloned and made 
        ///   available in the <see cref="Distributions"/> property. The first argument
        ///   in the function should be the classIndex, and the second the variableIndex.
        /// </param>
        /// 
        public NaiveBayes(int classes, int inputs, Func<int, int, TDistribution> initial)
            : base(classes, inputs, initial)
        {
        }

        /// <summary>
        /// Gets the probability distributions for each class and input.
        /// </summary>
        /// <value>
        /// A TDistribution[,] array in with each row corresponds to a
        /// class, each column corresponds to an input variable. Each element
        /// of this double[,] array is a probability distribution modeling
        /// the occurrence of the input variable in the corresponding class.
        /// </value>
        public new TDistribution[,] Distributions
        {
            // TODO: Remove
            // For backwards compatibility
            get
            {
                var freqs = new TDistribution[NumberOfOutputs, NumberOfInputs];
                for (int i = 0; i < base.Distributions.Length; i++)
                    for (int j = 0; j < base.Distributions[i].Components.Length; j++)
                        freqs[i, j] = base.Distributions[i][j];
                return freqs;
            }
        }

    #region Obsolete
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
            where TOptions : class, IFittingOptions, new()
        {
            var teacher = new NaiveBayesLearning<TDistribution, TOptions>()
            {
                Model = this
            };
#if DEBUG
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif
            teacher.Empirical = empirical;
            teacher.Options.InnerOption = options;
            NaiveBayes<TDistribution, double> result = teacher.Learn(inputs, outputs);
            base.Distributions = result.Distributions;
            this.Priors = result.Priors;
            return new ZeroOneLoss(outputs) { Mean = true }.Loss(Decide(inputs));
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use NaiveBayesLearning<TDistribution> instead.")]
        public double Estimate(double[][] inputs, int[] outputs, bool empirical = true)
        {
            var teacher = new NaiveBayesLearning<TDistribution>()
            {
                Model = this
            };
#if DEBUG
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif
            teacher.Empirical = empirical;
            NaiveBayes<TDistribution, double> result = teacher.Learn(inputs, outputs);
            base.Distributions = result.Distributions;
            this.Priors = result.Priors;
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
            double[] ll = LogLikelihoods(input);
            int imax;
            logLikelihood = ll.Max(out imax);
            responses = Special.Softmax(ll);
            return imax;
        }
#if !NETSTANDARD1_4
        /// <summary>
        ///   Saves the Naïve Bayes model to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the Naïve Bayes model is to be serialized.</param>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Save(stream) instead (or use it as an extension method).")]

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
        [Obsolete("Please use Accord.IO.Serializer.Save(path) instead (or use it as an extension method).")]

        public void Save(string path)
        {
            Accord.IO.Serializer.Save(this, path);
        }
#endif
        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="priors">Obsolete</param>.
        /// <param name="initial">
        ///   An initial distribution to be used to initialized all independent
        ///   distribution components of this Naive Bayes. This distribution will
        ///   be cloned and made available in the <see cref="Distributions"/> property.
        /// </param>
        /// 
        [Obsolete("Please specify priors using the Priors property.")]
        public NaiveBayes(int inputs, int classes, TDistribution initial, double[] priors)
            : base(inputs, classes, initial)
        {
            this.Priors = priors;
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="priors">Obsolete</param>.
        /// <param name="initial">
        ///   An initial distribution to be used to initialized all independent
        ///   distribution components of this Naive Bayes. Those distributions
        ///   will made available in the <see cref="Distributions"/> property.
        /// </param>
        /// 
        [Obsolete("Please specify priors using the Priors property.")]
        public NaiveBayes(int inputs, int classes, TDistribution[] initial, double[] priors)
            : base(inputs, classes, initial)
        {
            this.Priors = priors;
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="priors">Obsolete</param>.
        /// <param name="initial">
        ///   An initial distribution to be used to initialized all independent
        ///   distribution components of this Naive Bayes. Those distributions
        ///   will made available in the <see cref="Distributions"/> property.
        /// </param>
        /// 
        [Obsolete("Please specify priors using the Priors property.")]
        public NaiveBayes(int inputs, int classes, TDistribution[,] initial, double[] priors)
            : base(inputs, classes, initial.ToJagged())
        {
            this.Priors = priors;
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="priors">Obsolete</param>.
        /// <param name="initial">
        ///   An initial distribution to be used to initialized all independent
        ///   distribution components of this Naive Bayes. Those distributions
        ///   will made available in the <see cref="Distributions"/> property.
        /// </param>
        /// 
        [Obsolete("Please specify priors using the Priors property.")]
        public NaiveBayes(int inputs, int classes, TDistribution[][] initial, double[] priors)
            : base(inputs, classes, initial)
        {
            this.Priors = priors;
        }
    #endregion


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
                    obj.probabilities)
                {
                    Priors = obj.priors
                };
                return nb;
            }
        }

#pragma warning restore 0169
#pragma warning restore 0649

    #endregion
    }
#else
    /// <summary>
    ///   This class is currently not supported in Mono due to
    ///   a bug in the Mono compiler.
    /// </summary>
    /// 
    [System.Obsolete("This class is not supported in Mono.")]
    public class NaiveBayes<T>
    {
       
    }
#endif
}
