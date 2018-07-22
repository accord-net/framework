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
        // For funsies make the zoom levels a fibonacci sequence (except for 25, 50, 75, 100, 200) - always need to have a 1 to 1 view
        private int[] zoomLevelsFibonacci = new int[] { 13, 21, 25, 34, 50, 55, 75, 89, 100, 144, 200, 233, 377, 610, 987, 1597, 2584, 4181 };
        private int zoom = 100;
        public Image Image { get; private set; }

        public int Zoom
        {
            get
            {
                return zoom;
            }
            private set
            {
                var tempZoom = value.Between(pbPreview.ZoomLevels[0], pbPreview.ZoomLevels[pbPreview.ZoomLevels.Count - 1]);
                //var currentZoomText = tempZoom < 1 ? (tempZoom).ToString("N0") : (tempZoom).ToString();
                tsddbZoom.Text = $"Zoom: {tempZoom}%";

                if (tempZoom != zoom)
                {
                    zoom = tempZoom;
                    UpdatePreview();
                }
            }
        }

        private bool escapeKeyDown;

        public ImageView(Image img)
        {
            InitializeComponent();
            Image = img;
            Zoom = 100;//1;
            Text = $"Accord Image Visualizer v{Helpers.AssemblyVersion}";
            pbPreview.MouseWheel += PbPreview_MouseWheel;
            pbPreview.ZoomLevels = new Accord.Controls.Cyotek.ZoomLevelCollection(zoomLevelsFibonacci);
            pbPreview.Image = Image;
            UpdateControls();
        }

        private void UpdateControls()
        {
            tsMain.Renderer = new CustomToolStripProfessionalRenderer();
            tsddbZoom.HideImageMargin();

            for (int i = 0; i < pbPreview.ZoomLevels.Count; i++)
            {
                var currentZoom = pbPreview.ZoomLevels[i];
                //var currentZoomText = currentZoom < 1 ? (currentZoom).ToString("N0") : (currentZoom).ToString();
                ToolStripMenuItem tsmi = new ToolStripMenuItem(currentZoom + "%")
                {
                    Tag = currentZoom
                };
                tsmi.Click += Click_ZoomDropDown;
                tsddbZoom.DropDownItems.Add(tsmi);
            }

            UpdatePreview();
        }

        private void Click_ZoomDropDown(object sender, EventArgs e)
        {
            Zoom = (int)((ToolStripMenuItem)sender).Tag;
            pbPreview.Zoom = zoom;
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
            }
        }

        private Bitmap RenderPreview(Image img)
        {
            const int lineSize = 2;
            var previewWidth = img.Width * Zoom;
            var previewHeight = img.Height * Zoom;

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
            var pb = new Accord.Controls.Cyotek.ImageBox();
            pb.IsValidImage();

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
            {
                Zoom = pbPreview.ZoomLevels.NextZoom(pbPreview.Zoom);
                pbPreview.Zoom = zoom;
            }

            if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract || e.KeyCode == Keys.K)
            {
                Zoom = pbPreview.ZoomLevels.PreviousZoom(pbPreview.Zoom);
                pbPreview.Zoom = zoom;
            }
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

        private void pbPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (pbPreview.IsPointInImage(e.Location))
            {
                var pt = pbPreview.PointToImage(e.Location);
                tsslX.Text = $"X: {pt.X}";
                tsslY.Text = $"Y: {pt.Y}";
            }
        }

        private void PbPreview_MouseWheel(object sender, MouseEventArgs e)
        {
            // This is not doing the zooming - it's only updating the combobox

            if (e.Delta > 0)
                Zoom = pbPreview.ZoomLevels.NextZoom(pbPreview.Zoom);
            else
                Zoom = pbPreview.ZoomLevels.PreviousZoom(pbPreview.Zoom);
        }
    }
}
