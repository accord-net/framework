// Accord Debugging Visualizers
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// AForge debugging visualizers
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2011
// contacts@aforgenet.com
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
// Accord Debugging Visualizers
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Darren Schroeder, 2017
// https://github.com/fdncred
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// AForge debugging visualizers
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2011
// contacts@aforgenet.com
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Accord.DebuggerVisualizers
{
    public partial class ImageView : Form
    {
        private const int MaxZoom = 10;
        public Image Image { get; private set; }
        private int zoom = 1;

        public int Zoom
        {
            get
            {
                return zoom;
            }
            private set
            {
                int tempZoom = value.Between(1, MaxZoom);
                tsddbZoom.Text = $"Zoom: {tempZoom * 100}%";

                if (tempZoom != zoom)
                {
                    zoom = tempZoom;
                    UpdatePreview();
                }
            }
        }

        private System.Drawing.Point dragStartPosition;
        private bool escapeKeyDown;

        public ImageView(Image img)
        {
            InitializeComponent();
            Image = img;
            Zoom = 1;
            UpdateControls();
            Text = $"Accord Image Visualizer v{Helpers.AssemblyVersion}";
            pbPreview.MouseWheel += PbPreview_MouseWheel;
        }

        private void UpdateControls()
        {
            tsMain.Renderer = new CustomToolStripProfessionalRenderer();
            tsddbZoom.HideImageMargin();

            for (int i = 0; i < MaxZoom; i++)
            {
                int currentZoom = i + 1;
                ToolStripMenuItem tsmi = new ToolStripMenuItem(currentZoom * 100 + "%");
                tsmi.Click += (sender, e) => Zoom = currentZoom;
                tsddbZoom.DropDownItems.Add(tsmi);
            }

            UpdatePreview();
        }

        private void UpdatePreview()
        {
            UpdatePreview(Image);
        }

        private void UpdatePreview(Image img)
        {
            if (img != null)
            {
                tsslStatusWidth.Text = $"Width: {img.Width}px";
                tsslStatusHeight.Text = $"Height: {img.Height}px";
                tsslStatusPixelFormat.Text = $"Pixel format: {img.PixelFormat}";
                tsslStatusType.Text = $"Type: {img.GetType().Name}";
                tsslDPI.Text = $"DPI: {System.Math.Ceiling(img.HorizontalResolution)}x{System.Math.Ceiling(img.VerticalResolution)}";

                Bitmap bmpPreview = RenderPreview(img);

                SetPreviewImage(bmpPreview);
            }
        }

        private Bitmap RenderPreview(Image img)
        {
            int lineSize = 2;
            int previewWidth = img.Width * Zoom;
            int previewHeight = img.Height * Zoom;

            Bitmap bmpPreview = new Bitmap(previewWidth + lineSize * 2, previewHeight + lineSize * 2, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(bmpPreview))
            {
                if (img.PixelFormat == PixelFormat.Format32bppArgb)
                {
                    using (Image checkers = Helpers.DrawCheckers(previewWidth, previewHeight))
                    {
                        g.DrawImage(checkers, lineSize, lineSize, checkers.Width, checkers.Height);
                    }
                }

                g.DrawRectangle(Pens.White, 0, 0, bmpPreview.Width - 1, bmpPreview.Height - 1);
                g.DrawRectangle(Pens.Black, 1, 1, bmpPreview.Width - 3, bmpPreview.Height - 3);

                if (Zoom > 1)
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                }

                g.DrawImage(img, lineSize, lineSize, previewWidth, previewHeight);
            }

            return bmpPreview;
        }

        private void SetPreviewImage(Image img)
        {
            if (pbPreview.IsValidImage())
            {
                pbPreview.Image.Dispose();
            }

            pbPreview.Image = img;
        }

        private void ImageView_Shown(object sender, EventArgs e)
        {
            Activate();
        }

        private void ImageView_KeyDown(object sender, KeyEventArgs e)
        {
            escapeKeyDown = e.KeyCode == Keys.Escape;
        }

        private void ImageView_KeyUp(object sender, KeyEventArgs e)
        {
            if (escapeKeyDown && e.KeyCode == Keys.Escape)
            {
                Close();
            }

            if (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add || e.KeyCode == Keys.J)
                Zoom += 1;
            if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract || e.KeyCode == Keys.K)
                Zoom -= 1;
        }

        private void tsbCopyImage_Click(object sender, EventArgs e)
        {
            Helpers.CopyImage(Image);
        }

        private void tsbSaveImage_Click(object sender, EventArgs e)
        {
            Helpers.SaveImageAsFile(Image);
        }

        private void tsbOpenGitHub_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL("https://github.com/accord-net/framework");
        }

        private void pbPreview_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (pMain.HorizontalScroll.Visible || pMain.VerticalScroll.Visible))
            {
                Cursor = Cursors.SizeAll;
                dragStartPosition = e.Location;
            }
        }

        private void pbPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (pMain.HorizontalScroll.Visible || pMain.VerticalScroll.Visible))
            {
                System.Drawing.Point scrollOffset = new System.Drawing.Point(e.X - dragStartPosition.X, e.Y - dragStartPosition.Y);
                pMain.AutoScrollPosition = new System.Drawing.Point(-pMain.AutoScrollPosition.X - scrollOffset.X, -pMain.AutoScrollPosition.Y - scrollOffset.Y);
            }

            tsslX.Text = $"X: {e.X / Zoom}";
            tsslY.Text = $"Y: {e.Y / Zoom}";
        }

        private void pbPreview_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void PbPreview_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                Zoom++;
            }
            else
            {
                Zoom--;
            }
        }
    }
}
