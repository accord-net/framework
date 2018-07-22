using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Accord.Math;

namespace SampleApp
{
    public partial class WorldRendererControl : UserControl
    {
        private Vector3[] objectPoints = null;
        private Vector3[] objectScenePoints = null;
        private Accord.Point[] projectedPoints = null;
        private Color[] colors = null;
        private int[,] lines = null;


        private Matrix4x4 worldMatrix = new Matrix4x4();
        private Matrix4x4 viewMatrix = new Matrix4x4();
        private Matrix4x4 perspectiveMatrix = new Matrix4x4();

        public Matrix4x4 WorldMatrix
        {
            get { return worldMatrix; }
            set
            {
                worldMatrix = value;
                Recalculate();
            }
        }

        public Matrix4x4 ViewMatrix
        {
            get { return viewMatrix; }
            set
            {
                viewMatrix = value;
                Recalculate();
            }
        }

        public Accord.Point[] ProjectedPoints
        {
            get { return projectedPoints; }
        }

        public WorldRendererControl()
        {
            InitializeComponent();

            objectPoints = new Vector3[]
            {
                new Vector3( 0, 0, 0 ),
            };

            colors = new Color[]
            {
                Color.White,
            };

            lines = new int[0, 2];

            // create default matrices
            worldMatrix = Matrix4x4.Identity;
            viewMatrix = Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), new Vector3(0, 0, 0));
            perspectiveMatrix = Matrix4x4.CreatePerspective(1, 1, 1, 1000);

            Recalculate();
        }

        public void SetObject(Vector3[] vertices, Color[] colors, int[,] ribs)
        {
            if (vertices.Length != colors.Length)
            {
                throw new ArgumentException("Number of colors must be equal to number of vertices.");
            }

            if (ribs.GetLength(1) != 2)
            {
                throw new ArgumentException("Ribs array must have 2 coordinates per rib.");
            }

            this.objectPoints = (Vector3[])vertices.Clone();
            this.colors = (Color[])colors.Clone();
            this.lines = (int[,])ribs.Clone();

            Recalculate();
        }

        private void Recalculate()
        {
            int pointsCount = objectPoints.Length;
            objectScenePoints = new Vector3[pointsCount];
            projectedPoints = new Accord.Point[pointsCount];

            int cx = ClientRectangle.Width / 2;
            int cy = ClientRectangle.Height / 2;

            for (int i = 0; i < pointsCount; i++)
            {
                objectScenePoints[i] = (perspectiveMatrix *
                                       (viewMatrix *
                                       (worldMatrix * objectPoints[i].ToVector4()))).ToVector3();

                projectedPoints[i] = new Accord.Point(
                    (int)(cx * objectScenePoints[i].X),
                    (int)(cy * objectScenePoints[i].Y));
            }

            Invalidate();
        }

        private void WorldRendererControl_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen linesPen = new Pen(Color.White);

            using (SolidBrush brush = new SolidBrush(Color.Black))
            {
                g.FillRectangle(brush, this.ClientRectangle);
            }

            if (projectedPoints != null)
            {
                int cx = ClientRectangle.Width / 2;
                int cy = ClientRectangle.Height / 2;

                Point[] screenPoints = new Point[projectedPoints.Length];

                for (int i = 0, n = projectedPoints.Length; i < n; i++)
                {
                    screenPoints[i] = new Point((int)(cx + projectedPoints[i].X),
                                                 (int)(cy - projectedPoints[i].Y));
                }

                for (int i = 0; i < lines.GetLength(0); i++)
                {
                    int lineStart = lines[i, 0];
                    int lineEnd = lines[i, 1];

                    if ((lineStart < projectedPoints.Length) && (lineEnd < projectedPoints.Length))
                    {
                        g.DrawLine(linesPen, screenPoints[lineStart], screenPoints[lineEnd]);
                    }
                }

                for (int i = 0; i < projectedPoints.Length; i++)
                {
                    using (SolidBrush pointsBrush = new SolidBrush(colors[i]))
                    {
                        g.FillRectangle(pointsBrush, screenPoints[i].X - 2, screenPoints[i].Y - 2, 5, 5);
                    }
                }
            }

            linesPen.Dispose();
        }
    }
}
