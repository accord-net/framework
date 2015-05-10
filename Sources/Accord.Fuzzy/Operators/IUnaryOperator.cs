// AForge Fuzzy Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Fuzzy
{
    using System;

    /// <summary>
    /// Interface with the common methods of Fuzzy Unary Operator.
    /// </summary>
    /// 
    /// <remarks><para>All fuzzy operators that act as a Unary Operator (NOT, VERY, LITTLE) must implement this interface.
    /// </para></remarks>
    /// 
    public interface IUnaryOperator
    {
        /// <summary>
        /// Calculates the numerical result of a Unary operation applied to one
        /// fuzzy membership value.
        /// </summary>
        /// 
        /// <param name="membership">A fuzzy membership value, [0..1].</param>
        /// 
        /// <returns>The numerical result of the operation applied to <paramref name="membership"/></returns>.
        /// 
        float Evaluate( float membership );
    }
}


