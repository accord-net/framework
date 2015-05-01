// AForge Neural Net Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2012
// contacts@aforgenet.com
//

namespace AForge.Neuro
{
    using System;

    /// <summary>
    /// Distance neuron.
    /// </summary>
    /// 
    /// <remarks><para>Distance neuron computes its output as distance between
    /// its weights and inputs - sum of absolute differences between weights'
    /// values and corresponding inputs' values. The neuron is usually used in Kohonen
    /// Self Organizing Map.</para></remarks>
    /// 
    [Serializable]
    public class DistanceNeuron : Neuron
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistanceNeuron"/> class.
        /// </summary>
        /// 
        /// <param name="inputs">Neuron's inputs count.</param>
        /// 
        public DistanceNeuron( int inputs ) : base( inputs ) { }

        /// <summary>
        /// Computes output value of neuron.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns neuron's output value.</returns>
        /// 
        /// <remarks><para>The output value of distance neuron is equal to the distance
        /// between its weights and inputs - sum of absolute differences.
        /// The output value is also stored in <see cref="Neuron.Output">Output</see>
        /// property.</para>
        /// 
        /// <para><note>The method may be called safely from multiple threads to compute neuron's
        /// output value for the specified input values. However, the value of
        /// <see cref="Neuron.Output"/> property in multi-threaded environment is not predictable,
        /// since it may hold neuron's output computed from any of the caller threads. Multi-threaded
        /// access to the method is useful in those cases when it is required to improve performance
        /// by utilizing several threads and the computation is based on the immediate return value
        /// of the method, but not on neuron's output property.</note></para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">Wrong length of the input vector, which is not
        /// equal to the <see cref="Neuron.InputsCount">expected value</see>.</exception>
        /// 
        public override double Compute( double[] input )
        {
            // check for corrent input vector
            if ( input.Length != inputsCount )
                throw new ArgumentException( "Wrong length of the input vector." );

            // difference value
            double dif = 0.0;

            // compute distance between inputs and weights
            for ( int i = 0; i < inputsCount; i++ )
            {
                dif += Math.Abs( weights[i] - input[i] );
            }

            // assign output property as well (works correctly for single threaded usage)
            this.output = dif;

            return dif;
        }
    }
}
