// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Rémy Dispagne, 2013
// cramer at libertysurf.fr
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

namespace Accord.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Accord.Math.Kinematics;
    using AForge.Math;
    using Accord.Math;

    /// <summary>
    ///   Denavit Hartenberg Viewer.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class can be used to visualize a D-H model as bitmaps.
    /// </remarks>
    /// 
    public class DenavitHartenbergViewer : IDisposable
    {
        // Bitmaps (one for XY, one for YZ and one for XZ planes)
        //
        Bitmap xy;
        Bitmap yz;
        Bitmap xz;

        Graphics gxy;
        Graphics gyz;
        Graphics gxz;


        /// <summary>
        ///   Gets or sets the color of the links between joints
        /// </summary>
        /// 
        public Color LinksColor { get; set; }

        /// <summary>
        ///   Gets or sets the color of the joints
        /// </summary>
        /// 
        public Color JointsColor { get; set; }

        /// <summary>
        ///   Gets or sets the color of the last joint of a model
        /// </summary>
        /// 
        public Color EndJointColor { get; set; }

        /// <summary>
        ///   Gets or sets the color of the first joint of a model
        /// </summary>
        /// 
        public Color BaseJointColor { get; set; }

        /// <summary>
        ///   Gets or sets the color of the rendering surface background
        /// </summary>
        /// 
        public Color BackColor { get; set; }

        /// <summary>
        ///   Gets or sets the value to scale the drawing of the model to fit the window. Default is 1.
        /// </summary>
        /// 
        public float Scale { get; set; }

        /// <summary>
        ///   Gets or sets the radius of the joints circles. Default is 8.
        /// </summary>
        /// 
        public float JointRadius { get; set; }

        /// <summary>
        ///   Gets or sets the arrows indicating the axes on each drawing represented as a Rectangle object.
        /// </summary>
        /// 
        public Rectangle ArrowsBoundingBox { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DenavitHartenbergViewer"/> class.
        /// </summary>
        /// 
        /// <param name="width">Width of the drawing window</param>
        /// <param name="height">Height of the drawing window</param>
        /// 
        public DenavitHartenbergViewer(int width, int height)
        {
            // Setting the default parameters
            BackColor = Color.Black;
            LinksColor = Color.Green;
            JointsColor = Color.Red;
            BaseJointColor = Color.Gold;
            EndJointColor = Color.Blue;
            Scale = 1;
            JointRadius = 8;

            ArrowsBoundingBox = new Rectangle(10, height - 50 - 10, 50, 50);


            // Creating the bitmap and graphics

            xy = new Bitmap(width, height);
            yz = new Bitmap(width, height);
            xz = new Bitmap(width, height);

            gxy = Graphics.FromImage(xy);
            gyz = Graphics.FromImage(yz);
            gxz = Graphics.FromImage(xz);
        }

        /// <summary>
        ///   Image of the model viewed on the XY plane.
        /// </summary>
        /// 
        public Bitmap PlaneXY
        {
            get { return xy; }
        }

        /// <summary>
        ///   Image of the model viewed on the YZ plane.
        /// </summary>
        /// 
        public Bitmap PlaneYZ
        {
            get { return yz; }
        }

        /// <summary>
        ///   Image of the model viewed on the XZ plane.
        /// </summary>
        /// 
        public Bitmap PlaneXZ
        {
            get { return xz; }
        }

        /// <summary>
        ///   Makes a list of all the models contained on a
        ///   ModelCombinator. This function is recursive.
        /// </summary>
        /// 
        /// <param name="model">
        ///   ModelCombinator model in which to extract all the models. </param>
        /// <param name="models">
        ///   List of already extracted models. It accumulates all the 
        ///   models at each call of this function.</param>
        ///   
        /// <returns>Returns a list of all the models contained in the 
        /// ModelCombinator 'model' plus all previously extracted models</returns>
        /// 
        private List<DenavitHartenbergModel> GetAllModels(DenavitHartenbergNode model,
            List<DenavitHartenbergModel> models)
        {
            // If it's the first call
            if (models == null)
            {
                // Create the models list
                models = new List<DenavitHartenbergModel>();
            }

            // Add the model contained in the ModelCombinator
            models.Add(model.Model);

            // For all the children of the ModelCombinator
            foreach (DenavitHartenbergNode child in model.Children)
            {
                // Execute recursively this function 
                models = GetAllModels(child, models);
            }

            // Return the models list
            return models;
        }

        /// <summary>
        ///   Computes the three images of a list of ModelCombinator
        /// </summary>
        /// 
        /// <param name="model">List of arguments of models to be drawn</param>
        /// 
        /// <remarks>This function assumes that the models have already been calculated.</remarks>
        /// 
        public void ComputeImages(params DenavitHartenbergNode[] model)
        {
            // List of models we will render
            List<DenavitHartenbergModel> models_to_render = new List<DenavitHartenbergModel>();
            // For each model on the argument list
            for (int i = 0; i < model.Length; i++)
            {
                // Add the models extracted by the GetAllModels function to the list
                models_to_render.AddRange(GetAllModels(model[i], null));
            }

            // Draw the extracted models
            ComputeImages(models_to_render.ToArray());
        }

        /// <summary>
        ///   Computes the three images of a list of models
        /// </summary>
        /// 
        /// <param name="model">List of arguments of models</param>
        /// 
        public void ComputeImages(params DenavitHartenbergModel[] model)
        {
            // Clear the 3 images with the background color
            gxy.Clear(BackColor);
            gxz.Clear(BackColor);
            gyz.Clear(BackColor);

            // Draw each model
            for (int i = 0; i < model.Length; i++)
            {
                DenavitHartenbergModel mdl = model[i];

                Pen pLinks = new Pen(LinksColor, 1);

                // Draw each link of the model
                Vector3 previous = mdl.Position;

                for (int j = 0; j < mdl.Joints.Count; j++)
                {
                    Vector3 current = mdl.Joints[j].Position;

                    // XY
                    gxy.DrawLine(pLinks,
                        (xy.Width / 2) + previous.X * Scale,
                        (xy.Height / 2) - previous.Y * Scale,
                        (xy.Width / 2) + current.X * Scale,
                        (xy.Height / 2) - current.Y * Scale);

                    // ZY
                    gyz.DrawLine(pLinks,
                        (yz.Width / 2) + previous.Z * Scale,
                        (yz.Height / 2) - previous.Y * Scale,
                        (yz.Width / 2) + current.Z * Scale,
                        (yz.Height / 2) - current.Y * Scale);

                    // XZ
                    gxz.DrawLine(pLinks,
                        (xz.Width / 2) + previous.X * Scale,
                        (xz.Height / 2) - previous.Z * Scale,
                        (xz.Width / 2) + current.X * Scale,
                        (xz.Height / 2) - current.Z * Scale);

                    previous = current;
                }

                Brush pJoints = new SolidBrush(JointsColor);

                // Draw each joint
                for (int j = 0; j < mdl.Joints.Count + 1; j++)
                {
                    Vector3 current;

                    // Select the color of the joint
                    if (j == 0)
                    {
                        pJoints = new SolidBrush(BaseJointColor);
                        current = mdl.Position;
                    }
                    else
                    {
                        current = mdl.Joints[j - 1].Position;

                        if (j == mdl.Joints.Count)
                        {
                            pJoints = new SolidBrush(EndJointColor);
                        }
                        else
                        {
                            pJoints = new SolidBrush(JointsColor);
                        }
                    }

                    // XY
                    gxy.FillEllipse(pJoints,
                        (xy.Width / 2) + current.X * Scale - (JointRadius / 2),
                        (xy.Height / 2) - current.Y * Scale - (JointRadius / 2),
                        JointRadius, JointRadius);

                    // YZ
                    gyz.FillEllipse(pJoints,
                        (yz.Width / 2) + current.Z * Scale - (JointRadius / 2),
                        (yz.Height / 2) - current.Y * Scale - (JointRadius / 2),
                        JointRadius, JointRadius);

                    // XZ
                    gxz.FillEllipse(pJoints,
                        (xz.Width / 2) + current.X * Scale - (JointRadius / 2),
                        (xz.Height / 2) - current.Z * Scale - (JointRadius / 2),
                        JointRadius, JointRadius);
                }
            }

            // Draw the arrows on the windows
            DrawArrows(ref gxy, "Y", "X");
            DrawArrows(ref gyz, "Y", "Z");
            DrawArrows(ref gxz, "Z", "X");


        }

        /// <summary>
        ///   Method to draw arrows to indicate the axis.
        /// </summary>
        /// 
        /// <param name="g">Graphics variable to use to draw.</param>
        /// <param name="topArrowText">Text to draw on the top of the arrow.</param>
        /// <param name="rightArrowText">Text to draw on the right arrow.</param>
        /// 
        private void DrawArrows(ref Graphics g, string topArrowText, string rightArrowText)
        {
            Pen pAxes = new Pen(Color.White);
            SolidBrush bText = new SolidBrush(Color.White);

            // Draw the top arrow
            g.DrawString(topArrowText, new Font("Arial", 8), bText,
                ArrowsBoundingBox.Left - 5, ArrowsBoundingBox.Top - 15,
                StringFormat.GenericDefault);

            g.DrawLine(pAxes,
                ArrowsBoundingBox.Left, ArrowsBoundingBox.Top,
                ArrowsBoundingBox.Left, ArrowsBoundingBox.Bottom);

            g.DrawLine(pAxes,
                ArrowsBoundingBox.Left, ArrowsBoundingBox.Top,
                ArrowsBoundingBox.Left - 5, ArrowsBoundingBox.Top + 10);

            g.DrawLine(pAxes,
                ArrowsBoundingBox.Left, ArrowsBoundingBox.Top,
                ArrowsBoundingBox.Left + 5, ArrowsBoundingBox.Top + 10);

            // Draw the right arrow
            g.DrawString(rightArrowText, new Font("Arial", 8), bText,
                ArrowsBoundingBox.Right - 10 + 12, ArrowsBoundingBox.Bottom - 6,
                StringFormat.GenericDefault);

            g.DrawLine(pAxes,
                ArrowsBoundingBox.Left, ArrowsBoundingBox.Bottom,
                ArrowsBoundingBox.Right, ArrowsBoundingBox.Bottom);

            g.DrawLine(pAxes,
                ArrowsBoundingBox.Right - 10, ArrowsBoundingBox.Bottom - 5,
                ArrowsBoundingBox.Right, ArrowsBoundingBox.Bottom);

            g.DrawLine(pAxes,
                ArrowsBoundingBox.Right - 10, ArrowsBoundingBox.Bottom + 5,
                ArrowsBoundingBox.Right, ArrowsBoundingBox.Bottom);
        }






        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref="DenavitHartenbergViewer"/> is reclaimed by garbage collection.
        /// </summary>
        /// 
        ~DenavitHartenbergViewer()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Performs application-defined tasks associated with
        ///   freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// 
        /// <param name="disposing"><c>true</c> to release both managed
        /// and unmanaged resources; <c>false</c> to release only unmanaged
        /// resources.</param>
        ///
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (xy != null)
                    xy.Dispose();

                if (yz != null)
                    yz.Dispose();

                if (xz != null)
                    xz.Dispose();
            }
        }
    }
}
