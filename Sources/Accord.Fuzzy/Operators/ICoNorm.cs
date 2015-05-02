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
    /// Interface with the common methods of a Fuzzy CoNorm.
    /// </summary>
    /// 
    /// <remarks><para>All fuzzy operators that act as a CoNorm must implement this interface.
    /// </para></remarks>
    /// 
    public interface ICoNorm
    {
        /// <summary>
        /// Calculates the numerical result of a CoNorm (OR) operation applied to
        /// two fuzzy membership values.
        /// </summary>
        /// 
        /// <param name="membershipA">A fuzzy membership value, [0..1].</param>
        /// <param name="membershipB">A fuzzy membership value, [0..1].</param>
        /// 
        /// <returns>The numerical result the operation OR applied to <paramref name="membershipA"/>
        /// and <paramref name="membershipB"/>.</returns>
        /// 
        float Evaluate( float membershipA, float membershipB );
    }
}

