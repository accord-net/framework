// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © 2009-2014, César Souza
// All rights reserved. 3-BSD License:
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions are met:
//
//      * Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//
//      * Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//
//      * Neither the name of the Accord.NET Framework authors nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
// 
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Drawing;

namespace Gestures.HMMs
{
    public class Database
    {
        public BindingList<string> Classes { get; private set; }
        public BindingList<Sequence> Samples { get; private set; }


        public Database()
        {
            Classes = new BindingList<string>();
            Samples = new BindingList<Sequence>();
        }

        public void Save(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(BindingList<Sequence>));
            serializer.Serialize(stream, Samples);
        }

        public void Load(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(BindingList<Sequence>));
            var samples = (BindingList<Sequence>)serializer.Deserialize(stream);

            Classes.Clear();
            foreach (string label in samples.First().Classes)
                Classes.Add(label);

            Samples.Clear();
            foreach (Sequence sample in samples)
            {
                sample.Classes = Classes;
                Samples.Add(sample);
            }
        }

        public Sequence Add(Point[] sequence, string classLabel)
        {
            if (sequence == null || sequence.Length == 0)
                return null;

            if (!Classes.Contains(classLabel))
                Classes.Add(classLabel);

            int classIndex = Classes.IndexOf(classLabel);

            Sequence sample = new Sequence()
            {
                Classes = Classes,
                SourcePath = sequence,
                Output = classIndex
            };

            Samples.Add(sample);

            return sample;
        }

        public void Clear()
        {
            Classes.Clear();
            Samples.Clear();
        }

        public int SamplesPerClass()
        {
            int min = 0;
            foreach (string label in Classes)
            {
                int c = Samples.Count(p => p.OutputName == label);

                if (min == 0) 
                    min = c;

                else if (c < min)
                    min = c;
            }

            return min;
        }
    }
}
