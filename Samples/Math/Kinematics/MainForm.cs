// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © Rémy Dispagne, 2013
// cramer at libertysurf.fr
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
using System.Windows.Forms;
using Accord.Controls;
using Accord.Math.Kinematics;
using AForge.Math;

namespace DenavitKinematics
{
    /// <summary>
    ///   Denavit-Hartenberg models for kinematic chains.
    /// </summary>
    /// 
    /// <remarks>
    ///   This sample application, together with the original Denavit-Hartenberg model
    ///   classes, were contributed to the framework by Rémy Dispagne. The framework
    ///   author is immensely grateful to Rémy for this outstanding contribution!
    /// </remarks>
    /// 
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

