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

namespace Gestures.HMMs
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using System.Drawing.Drawing2D;

    public partial class Canvas : UserControl
    {
        private bool capturing;
        private List<Point> sequence;



        public Canvas()
        {
            InitializeComponent();

            sequence = new List<Point>();
            this.DoubleBuffered = true;
        }

        public Point[] GetSequence()
        {
            return sequence.ToArray();
        }


        public void Clear()
        {
            sequence.Clear();
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e);

            if (!this.DesignMode)
            {
                if (sequence.Count > 1)
                {
                    for (int i = 1; i < sequence.Count; i++)
                    {
                        int x = (int)sequence[i].X;
                        int y = (int)sequence[i].Y;
                        int p = (int)Accord.Math.Tools.Scale(0, sequence.Count, 0, 255, i);

                        int prevX = (int)sequence[i - 1].X;
                        int prevY = (int)sequence[i - 1].Y;
                        int prevP = (int)Accord.Math.Tools.Scale(0, sequence.Count, 0, 255, i - 1);

                        if (x == prevX && y == prevY)
                            continue;

                        Point start = new Point(prevX, prevY);
                        Point end = new Point(x, y);
                        Color colorStart = Color.FromArgb(255 - p, 0, p);
                        Color colorEnd = Color.FromArgb(255 - prevP, 0, prevP);

                        using (Brush brush = new LinearGradientBrush(start, end, colorStart, colorEnd))
                        using (Pen pen = new Pen(brush, 10))
                        {
                            pen.StartCap = LineCap.Round;
                            pen.EndCap = LineCap.Round;

                            e.Graphics.DrawLine(pen, prevX, prevY, x, y);
                        }
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Clear();

            capturing = true;

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            capturing = false;

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (capturing)
            {
                if (e.X > 0 && e.Y > 0)
                {
                    sequence.Add(new Point(e.X, e.Y));
                    this.Refresh();
                }
            }

            base.OnMouseMove(e);
        }

    }
}
