// Accord Core Library
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

namespace Accord.Audio
{
    using System.Runtime.CompilerServices;
    using Accord.Audio;
    using Accord.Audition;
    using Accord.Audio.ComplexFilters;
    using Accord.Audio.Filters;
    using Accord.Audio.Formats;
    using Accord.Audio.Generators;
    using Accord.Audio.Windows;

    /// <summary>
    ///  Process, transforms, filters and handles audio signals 
    ///  for machine learning and statistical applications.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///  The audio processing module provides ways to load, parse, save, filter 
    ///  and transform <see cref="Signal">audio signals</see>, such as applying
    ///  <see cref="Accord.Audio.Filters">audio processing filters</see> in both
    ///  space and <see cref="ComplexSignal">frequency domain</see>. Those audio 
    ///  signals can be either loaded from <see cref="Accord.Audio.Formats">
    ///  standard files</see> (such as <see cref="Accord.Audio.Formats.WaveDecoder">
    ///  WAV files</see>) or captured in real-time <see cref="Accord.DirectSound.AudioCaptureDevice">
    ///  through a capturing device</see>.</para>
    ///  
    /// <para>
    ///   Filters include time-domain filters such as the <see cref="Accord.Audio.Filters.EnvelopeFilter">
    ///   envelope</see>, <see cref="HighPassFilter">high-pass</see>, <see cref="LowPassFilter">low-pass</see>,
    ///   and <see cref="WaveRectifier">wave rectification</see> filters. They also include frequency-domain
    ///   operators such as the <see cref="DifferentialRectificationFilter">differential rectification
    ///   filter</see> and the <see cref="CombFilter">comb filter with Dirac's delta functions</see>.
    /// </para>
    /// 
    /// <para>
    ///   The framework also include standard signal generators, such as <see cref="CosineGenerator"/>,
    ///   <see cref="ImpulseGenerator"/> and a <see cref="SquareGenerator"/>. Those can be used in
    ///   combination with the other filters to apply convolutions and other operations with audio 
    ///   <see cref="Signal"/>s.</para>
    /// 
    /// <para>
    ///   One typical application may include parsing wave files and extracting
    ///   the the numeric signals for using in statistics applications. Please
    ///   see the "Independent Component Analysis Sample Application" for more
    ///   details on how this can be done.
    /// </para>
    ///   
    /// <para>
    ///   The namespace class diagram is shown below. </para>
    ///   <img src="..\diagrams\classes\Accord.Audio.png" />
    /// </remarks>
    /// 
    /// <seealso cref="Accord.Audio.Windows"/>
    /// <seealso cref="Accord.Audio.Filters"/>
    /// <seealso cref="Accord.Audio.ComplexFilters"/>
    /// <seealso cref="Accord.Audio.Generators"/>
    ///   
    [CompilerGenerated]
    class NamespaceDoc
    {
    }
}
