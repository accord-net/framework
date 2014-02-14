// Accord Control Library
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

namespace Accord.Controls
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <summary>
    ///   Angle Box control.
    /// </summary>
    /// 
    public partial class AngleBox : UserControl, IDisposable
    {

        private float angle;
        private Pen pen;


        /// <summary>
        ///   Gets or sets the angle to be displayed.
        /// </summary>
        /// 
        /// <value>The angle.</value>
        /// 
        public float Angle
        {
            get { return angle; }
            set
            {
                angle = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="AngleBox"/> class.
        /// </summary>
        /// 
        public AngleBox()
        {
            InitializeComponent();

            pen = new Pen(Color.Black, 10);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.ArrowAnchor;

            DoubleBuffered = true;
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.Resize"/> event.
        /// </summary>
        /// 
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        /// 
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Invalidate();
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// 
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        /// 
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillRectangle(Brushes.White, ClientRectangle);

            float cx = ClientRectangle.Width / 2f;
            float cy = ClientRectangle.Height / 2f;

            float ex = cx + (float)Math.Cos(-angle) * cx;
            float ey = cy + (float)Math.Sin(-angle) * cy;

            g.DrawLine(pen, cx, cy, ex, ey);
        }

        /// <summary> 
        ///   Clean up any resources being used.
        /// </summary>
        /// 
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        /// 
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                pen.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
