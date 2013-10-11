using System.Drawing;
using System.Windows.Forms;

namespace Handwriting.KDA
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
                    digit[i,j] = bmp.GetPixel(i,j).R == 255 ? 0 : 1;

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
                            e.Graphics.FillRectangle(black, i * 4, j * 4, 4, 4);
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
            if (capturing)
            {
                if (e.X > 0 && e.Y > 0)
                {
                    int x = (int)System.Math.Floor((double)e.X /4.0);
                    int y = (int)System.Math.Floor((double)e.Y /4.0);

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
