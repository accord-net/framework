// Accord.NET Sample Applications
// http://accord.googlecode.com
//
// Copyright © Rémy Dispagne, 2013
// cramer at libertysurf.fr
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

using Accord.Controls;
using Accord.Math.Kinematics;
using AForge.Math;
using System;
using System.Windows.Forms;

namespace DenavitHartenberg
{
    public partial class MainForm : Form
    {
        DenavitHartenbergModel model;           // The arm base model
        DenavitHartenbergModel model_tgripper;  // The model left gripper
        DenavitHartenbergModel model_bgripper;  // The model right gripper

        // The whole arm made of a combination of 
        // the three previously declared models:
        //
        DenavitHartenbergNode arm;  

        DenavitHartenbergViewer viewer;  // The visualization model
        double angle = 0;                // Angle variable for animation


        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Ok, let's start to build our virtual robot arm !

            model = new DenavitHartenbergModel(new Vector3(0, 0, 0));

            // Add the first joint 
            model.Joints.Add(alpha: 0, theta: Math.PI / 4, radius: 35, offset: 0);

            // Add the second joint
            model.Joints.Add(alpha: 0, theta: -Math.PI / 3, radius: 35, offset: 0);

            // Create the top finger
            model_tgripper = new DenavitHartenbergModel();
            model_tgripper.Joints.Add(alpha: 0, theta: Math.PI / 4, radius: 20, offset: 0);
            model_tgripper.Joints.Add(alpha: 0, theta: -Math.PI / 3, radius: 20, offset: 0);

            // Create the bottom finger
            model_bgripper = new DenavitHartenbergModel();
            model_bgripper.Joints.Add(alpha: 0, theta: -Math.PI / 4, radius: 20, offset: 0);
            model_bgripper.Joints.Add(alpha: 0, theta: Math.PI / 3, radius: 20, offset: 0);

            // Create the model combinator from the parent model
            arm = new DenavitHartenbergNode(model);

            // Add the top finger
            arm.Children.Add(model_tgripper);

            // Add the bottom finger
            arm.Children.Add(model_bgripper);

            // Calculate the whole model (parent model + children models)
            arm.Compute();

            // Create the model visualizer
            viewer = new DenavitHartenbergViewer(pictureBox1.Width, pictureBox1.Height);

            // Assign each projection image of the model to a picture box
            pictureBox1.Image = viewer.PlaneXY;
            pictureBox2.Image = viewer.PlaneXZ;
            pictureBox3.Image = viewer.PlaneYZ;

            // Start the animation
            timer1.Interval = 40;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Let's move some joints to make a "hello" or "help meeee !" gesture !
            model.Joints[0].Parameters.Theta = (float)(Math.Sin(angle) * Math.PI / 4 + Math.PI / 6);
            model.Joints[1].Parameters.Theta = (float)(Math.Sin(angle) * Math.PI / 4 + Math.PI / 6);

            // Increment the animation time
            angle += (float)Math.PI / 30;

            // Calculate the whole model
            arm.Compute();

            // Compute the images for displaying on the picture boxes
            viewer.ComputeImages(arm);

            // Refresh the pictures
            pictureBox1.Refresh();
            pictureBox2.Refresh();
            pictureBox3.Refresh();
        }

        // Pause/Start button
        private void button1_Click(object sender, EventArgs e)
        {
            // Toggle animation
            timer1.Enabled = !timer1.Enabled;
        }
    }
}

