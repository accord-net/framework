using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Accord.Controls.Vision;

namespace KinectController
{
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
