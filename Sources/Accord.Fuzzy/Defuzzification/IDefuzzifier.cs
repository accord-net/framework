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
    using System.Collections.Generic;

    /// <summary>
    /// Interface which specifies set of methods required to be implemented by all defuzzification methods 
    /// that can be used in Fuzzy Inference Systems. 
    /// </summary>
    /// 
    /// <remarks><para>In many applications, a Fuzzy Inference System is used to perform linguistic computation, 
    /// but at the end of the inference process, a numerical value is needed. It does not mean that the system 
    /// needs precision, but simply that a numerical value is required, most of the times because it will be used to 
    /// control another system that needs the number. To obtain this numer, a defuzzification method is performed.</para>
    /// 
    /// <para>Several deffuzification methods were proposed, and they can be created as classes that 
    /// implements this interface.</para></remarks>
    /// 
    public interface IDefuzzifier
    {
        /// <summary>
        /// Defuzzification method to obtain the numerical representation of a fuzzy output.
        /// </summary>
        /// 
        /// <param name="fuzzyOutput">A <see cref="FuzzyOutput"/> containing the output of
        /// several rules of a Fuzzy Inference System.</param>
        /// <param name="normOperator">A <see cref="INorm"/> operator to be used when constraining
        /// the label to the firing strength.</param>
        /// 
        /// <returns>The numerical representation of the fuzzy output.</returns>
        /// 
        float Defuzzify( FuzzyOutput fuzzyOutput, INorm normOperator );
    }
}
