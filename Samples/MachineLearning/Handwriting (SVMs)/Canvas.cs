using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SampleApp
{
    public partial class Canvas : UserControl
    {
        int size = 5;
        private bool capturing;
        private int[,] digit;


        public Canvas()
        {
            InitializeComponent();

            digit = new int[32, 32];
            this.DoubleBuffered = true;
        }

        public double[] GetDigit()
        {
            double[] features = new double[32 * 32];
            for (int i = 0; i < 32; i++)
                for (int j = 0; j < 32; j++)
                    features[i * 32 + j] = digit[j, i];

            return features;
        }

        public void SetDigit(Bitmap bmp)
        {
            for (int i = 0; i < 32; i++)
                for (int j = 0; j < 32; j++)
                    digit[i, j] = bmp.GetPixel(i, j).R == 255 ? 0 : 1;

            this.Refresh();
        }

        public int PenSize
        {
            get { return size; }
            set { size = value; }
        }

        public void Clear()
        {
            digit = new int[32, 32];
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            float scaleX = this.Width / 32f;
            float scaleY = this.Height / 32f;

            if (!this.DesignMode)
            {
                Brush black = Brushes.Black;
                Brush white = Brushes.White;


                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if (digit[i, j] == 1)
                        {
                            e.Graphics.FillRectangle(black, i * scaleX, j * scaleY, scaleX, scaleY);
                        }
                    }
                }
            }

            base.OnPaint(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            capturing = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            capturing = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            double scaleX = this.Width / 32.0;
            double scaleY = this.Height / 32.0;

            if (capturing)
            {
                if (e.X > 0 && e.Y > 0)
                {
                    int x = (int)System.Math.Floor((double)e.X / scaleX);
                    int y = (int)System.Math.Floor((double)e.Y / scaleY);

                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            if (x + i < 32 && y + j < 32)
                                digit[x + i, y + j] = 1;
                        }
                    }


                    this.Refresh();
                }
            }

            base.OnMouseMove(e);

        }


    }
}
