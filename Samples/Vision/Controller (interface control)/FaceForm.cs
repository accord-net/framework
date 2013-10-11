// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2013
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

namespace Controller
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using Accord.Controls.Vision;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Statistics.Models.Markov;

    public partial class FaceForm : Form
    {


        public FaceForm(MainForm mainForm)
            : this()
        {
            faceController.Device = mainForm.controller;

            faceController.Start();
        }

        public FaceForm()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            faceController.Stop();
        }

        private void controller_FaceMove(object sender, FaceEventArgs e)
        {
            int x = (int)(e.X * 20f);
            int y = (int)(e.Y * 20f);

            tbHorizontal.Value = Math.Min(Math.Max(x, tbHorizontal.Minimum), tbHorizontal.Maximum);
            tbVertical.Value = Math.Min(Math.Max(y, tbVertical.Minimum), tbVertical.Maximum);

            pointBox1.PointX = e.X;
            pointBox1.PointY = e.Y;
        }

        private void btnHorizontalMin_Click(object sender, EventArgs e)
        {
            faceController.Calibrate(FaceMovement.Left);
        }

        private void btnHorizontalMax_Click(object sender, EventArgs e)
        {
            faceController.Calibrate(FaceMovement.Right);
        }

        private void btnVerticalMin_Click(object sender, EventArgs e)
        {
            faceController.Calibrate(FaceMovement.Down);
        }

        private void btnVerticalMax_Click(object sender, EventArgs e)
        {
            faceController.Calibrate(FaceMovement.Up);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            faceController.Reset();
        }

    }
}
