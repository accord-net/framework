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
    ///   Point Box control.
    /// </summary>
    /// 
    public partial class PointBox : UserControl, IDisposable
    {

        private float pointX;
        private float pointY;
        private Pen pen;


        /// <summary>
        ///   Gets or sets the x-coordinate
        ///   of the displayed point.
        /// </summary>
        /// 
        /// <value>The point's x-coordinate.</value>
        /// 
        public float PointX
        {
            get { return pointX; }
            set
            {
                pointX = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Gets or sets the y-coordinate
        ///   of the displayed point.
        /// </summary>
        /// 
        /// <value>The point's y-coordinate.</value>
        /// 
        public float PointY
        {
            get { return pointY; }
            set
            {
                pointY = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PointBox"/> class.
        /// </summary>
        /// 
        public PointBox()
        {
            InitializeComponent();

            pen = new Pen(Color.Black, 3);

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

            float r = 5;

            float width = ClientRectangle.Width;
            float height = ClientRectangle.Height;

            // Convert from [-1, +1] to [0, width]
            float cursorX = width * (pointX + 1f) / 2f;
            float cursorY = height * (-pointY + 1f) / 2f;

            cursorX = Math.Max(0, Math.Min(cursorX, width));
            cursorY = Math.Max(0, Math.Min(cursorY, height));


            // Draw crosshair
            g.DrawLine(pen, cursorX - r, cursorY, cursorX + r, cursorY);
            g.DrawLine(pen, cursorX, cursorY - r, cursorX, cursorY + r);
        }

        /// <summary> 
        ///   Clean up any resources being used.
        /// </summary>
        /// 
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        /// 
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null; 
                }

                if (pen != null)
                {
                    pen.Dispose();
                    pen = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
