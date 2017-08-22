// Accord Math Library
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

namespace Accord.Math.Optimization
{
    using System.Runtime.CompilerServices;

    /// <summary>
    ///   Contains classes for constrained and unconstrained optimization. Includes 
    ///   <see cref="ConjugateGradient">Conjugate Gradient (CG)</see>, <see cref="BoundedBroydenFletcherGoldfarbShanno">
    ///   Bounded</see> and <see cref="BroydenFletcherGoldfarbShanno">Unbounded Broyden–Fletcher–Goldfarb–Shanno (BFGS)</see>,
    ///   gradient-free optimization methods such as <see cref="Cobyla"/> and the <see cref="GoldfarbIdnani">Goldfarb-Idnani
    ///   </see> solver for Quadratic Programming (QP) problems.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This namespace contains different methods for solving both constrained and unconstrained
    ///   optimization problems. For unconstrained optimization, methods available include 
    ///   <see cref="ConjugateGradient">Conjugate Gradient (CG)</see>, <see cref="BoundedBroydenFletcherGoldfarbShanno">
    ///   Bounded</see> and <see cref="BroydenFletcherGoldfarbShanno">Unbounded Broyden–Fletcher–Goldfarb–Shanno (BFGS)</see>,
    ///   <see cref="ResilientBackpropagation">Resilient Backpropagation</see> and a simplified implementation of the 
    ///   <see cref="TrustRegionNewtonMethod">Trust Region Newton Method (TRON)</see>.</para>
    ///   
    /// <para>
    ///   For constrained optimization problems, methods available include the <see cref="AugmentedLagrangian">
    ///   Augmented Lagrangian method</see> for general non-linear optimization, <see cref="Cobyla"/> for
    ///   gradient-free non-linear optimization, and the <see cref="GoldfarbIdnani">Goldfarb-Idnani</see>
    ///   method for solving Quadratic Programming (QP) problems.</para>
    ///   
    /// <para>
    ///   This namespace also contains optimizers specialized for least squares problems, such as <see cref="GaussNewton">
    ///   Gauss Newton</see> and the <see cref="LevenbergMarquardt">Levenberg-Marquart</see> least squares solvers.</para>
    ///   
    /// <para>
    ///   For univariate problems, standard search algorithms are also available, such as <see cref="BrentSearch">
    ///   Brent</see> and <see cref="BinarySearch">Binary search</see>.</para>
    ///  
    /// <para>
    ///   The namespace class diagram is shown below. </para>
    ///   <img src="..\diagrams\classes\Accord.Math.Optimization.png" />
    /// </remarks>
    /// 
    /// <seealso cref="Accord.Math"/>
    /// <seealso cref="Accord.Math.Differentiation"/>
    /// <seealso cref="Accord.Math.Integration"/>
    ///   
    [CompilerGenerated]
    class NamespaceDoc
    {
    }
}
