// Accord Audio Library
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

namespace Accord.Audition.Beat
{
    using System;
    using System.Collections.Generic;
    using Accord.Audio;

    /// <summary>
    ///   Energy-based beat detector.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Frederic Patin, Beat Detection Algorithms. Available on:
    ///       http://www.gamedev.net/reference/programming/features/beatdetection. </description></item>
    ///   </list>
    /// </para>
    /// </remarks>
    /// 
    public class EnergyBeatDetector : IBeatDetector
    {
        private EnergyBuffer energyBuffer;
        private double sensitivity;
        private bool autoSensitivity;

        /// <summary>
        ///   Raised when a beat has been detected.
        /// </summary>
        /// 
        public event EventHandler Beat;

        /// <summary>
        ///   Gets or sets whether the detector should
        ///   compute the best sensitivity automatically.
        /// </summary>
        /// 
        public bool AutoSensitivity
        {
            get { return autoSensitivity; }
            set { autoSensitivity = value; }
        }

        /// <summary>
        ///   Gets or sets the sensitivity of the detector.
        /// </summary>
        /// 
        public double Sensitivity
        {
            get { return sensitivity; }
            set
            {
                sensitivity = value;
                autoSensitivity = false;
            }
        }

        /// <summary>
        ///   Creates a new Energy-based beat detector.
        /// </summary>
        /// 
        /// <param name="bufferSize">The size for the buffer.</param>
        /// 
        public EnergyBeatDetector(int bufferSize)
        {
            energyBuffer = new EnergyBuffer(bufferSize);
            autoSensitivity = true;
        }

        /// <summary>
        ///   Detects if there is a beat in the given signal.
        /// </summary>
        /// 
        /// <param name="signal">A signal (window).</param>
        /// 
        public void Detect(Signal signal)
        {
            // signal should have around 1024 samples
            double signalEnergy = signal.GetEnergy();
            double averageEnergy = energyBuffer.Average;


            if (autoSensitivity)
            {
                //Compute the variance 'V' of the energies in (E)
                sensitivity = (-0.0025714 * energyBuffer.Variance) + 1.5142857;
            }


            energyBuffer.Add(signalEnergy);


            if (signalEnergy > sensitivity * averageEnergy)
            {
                if (Beat != null)
                    Beat(this, EventArgs.Empty);
            }
        }
    }

    // TODO: Implement this as an circular array buffer, not as a List<T>
    internal class EnergyBuffer : List<double>
    {
        int maxSize;
        double average;
        double variance;

        public EnergyBuffer(int maxSize)
            : base(maxSize + 1)
        {
            this.maxSize = maxSize;
        }

        public double Average
        {
            get
            {
                average = 0.0;
                if (this.Count > 0)
                {
                    for (int i = 0; i < Count; i++)
                        average += this[i];
                    average /= Count;
                }
                return average;
            }
        }

        public double Variance
        {
            get
            {
                variance = 0.0;
                for (int i = 0; i < this.Count; i++)
                {
                    double v = (this[i] - average);
                    variance += v * v;
                }
                variance /= Count;
                return variance;
            }
        }

        public new void Add(double item)
        {
            base.Add(item);

            if (Count > maxSize)
                RemoveAt(0);
        }
    }

}
