using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;

namespace OpenSURFcs
{
    public class FastHessian
    {

        /// <summary>
        /// Response Layer 
        /// </summary>
        private class ResponseLayer
        {
            public int width, height, step, filter;
            public float[] responses;
            public byte[] laplacian;

            public ResponseLayer(int width, int height, int step, int filter)
            {
                this.width = width;
                this.height = height;
                this.step = step;
                this.filter = filter;

                responses = new float[width * height];
                laplacian = new byte[width * height];
            }

            public byte getLaplacian(int row, int column)
            {
                return laplacian[row * width + column];
            }

            public byte getLaplacian(int row, int column, ResponseLayer src)
            {
                int scale = this.width / src.width;
                return laplacian[(scale * row) * width + (scale * column)];
            }

            public float getResponse(int row, int column)
            {
                return responses[row * width + column];
            }

            public float getResponse(int row, int column, ResponseLayer src)
            {
                int scale = this.width / src.width;
                return responses[(scale * row) * width + (scale * column)];
            }
        }


        /// <summary>
        /// Static one-call do it all method
        /// </summary>
        /// <param name="thresh"></param>
        /// <param name="octaves"></param>
        /// <param name="init_sample"></param>
        /// <param name="img"></param>
        /// <returns></returns>
        public static List<IPoint> getIpoints(float thresh, int octaves, int init_sample, IntegralImage img)
        {
            FastHessian fh = new FastHessian(thresh, octaves, init_sample, img);
            return fh.getIpoints();
        }


        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="thresh"></param>
        /// <param name="octaves"></param>
        /// <param name="init_sample"></param>
        /// <param name="img"></param>
        public FastHessian(float thresh, int octaves, int init_sample, IntegralImage img)
        {
            this.thresh = thresh;
            this.octaves = octaves;
            this.init_sample = init_sample;
            this.img = img;
        }


        /// <summary>
        /// These are passed in
        /// </summary>
        private float thresh;
        private int octaves;
        private int init_sample;
        private IntegralImage img;


        /// <summary>
        /// These get built
        /// </summary>
        private List<IPoint> ipts;
        private List<ResponseLayer> responseMap;


        /// <summary>
        /// Find the image features and write into vector of features
        /// </summary>
        public List<IPoint> getIpoints()
        {
            // filter index map
            int[,] filter_map = { { 0, 1, 2, 3 }, { 1, 3, 4, 5 }, { 3, 5, 6, 7 }, { 5, 7, 8, 9 }, { 7, 9, 10, 11 } };

            // Clear the vector of existing ipts
            if (ipts == null) ipts = new List<IPoint>();
            else ipts.Clear();

            // Build the response map
            buildResponseMap();

            // Get the response layers
            ResponseLayer b, m, t;
            for (int o = 0; o < octaves; ++o) 
                for (int i = 0; i <= 1; ++i)
                {
                    b = responseMap[filter_map[o, i]];
                    m = responseMap[filter_map[o, i + 1]];
                    t = responseMap[filter_map[o, i + 2]];

                    // loop over middle response layer at density of the most 
                    // sparse layer (always top), to find maxima across scale and space
                    for (int r = 0; r < t.height; ++r)
                    {
                        for (int c = 0; c < t.width; ++c)
                        {
                            if (isExtremum(r, c, t, m, b))
                            {
                                interpolateExtremum(r, c, t, m, b);
                            }
                        }
                    }
                }

            return ipts;
        }


        /// <summary>
        ///   Build map of DoH responses
        /// </summary>
        /// 
        void buildResponseMap()
        {
            // Calculate responses for the first 4 octaves:
            // Oct1: 9,  15, 21, 27
            // Oct2: 15, 27, 39, 51
            // Oct3: 27, 51, 75, 99
            // Oct4: 51, 99, 147,195
            // Oct5: 99, 195,291,387

            // Release memory and clear any existing response layers
            if (responseMap == null) 
                responseMap = new List<ResponseLayer>();
            else responseMap.Clear();

            // Get image attributes
            int w = (img.Width / init_sample);
            int h = (img.Height / init_sample);
            int s = (init_sample);

            // Calculate approximated determinant of Hessian values
            if (octaves >= 1)
            {
                responseMap.Add(new ResponseLayer(w, h, s, 9));
                responseMap.Add(new ResponseLayer(w, h, s, 15));
                responseMap.Add(new ResponseLayer(w, h, s, 21));
                responseMap.Add(new ResponseLayer(w, h, s, 27));
            }

            if (octaves >= 2)
            {
                responseMap.Add(new ResponseLayer(w / 2, h / 2, s * 2, 39));
                responseMap.Add(new ResponseLayer(w / 2, h / 2, s * 2, 51));
            }

            if (octaves >= 3)
            {
                responseMap.Add(new ResponseLayer(w / 4, h / 4, s * 4, 75));
                responseMap.Add(new ResponseLayer(w / 4, h / 4, s * 4, 99));
            }

            if (octaves >= 4)
            {
                responseMap.Add(new ResponseLayer(w / 8, h / 8, s * 8, 147));
                responseMap.Add(new ResponseLayer(w / 8, h / 8, s * 8, 195));
            }

            if (octaves >= 5)
            {
                responseMap.Add(new ResponseLayer(w / 16, h / 16, s * 16, 291));
                responseMap.Add(new ResponseLayer(w / 16, h / 16, s * 16, 387));
            }

            // Extract responses from the image
            for (int i = 0; i < responseMap.Count; ++i)
            {
                buildResponseLayer(responseMap[i]);
            }
        }


        /// <summary>
        /// Build Responses for a given ResponseLayer
        /// </summary>
        /// <param name="rl"></param>
        private void buildResponseLayer(ResponseLayer rl)
        {
            int step = rl.step;                      // step size for this filter
            int b = (rl.filter - 1) / 2;             // border for this filter
            int l = rl.filter / 3;                   // lobe for this filter (filter size / 3)
            int w = rl.filter;                       // filter size
            float inverse_area = 1f / (w * w);       // normalization factor
            float Dxx, Dyy, Dxy;

            for (int r, c, ar = 0, index = 0; ar < rl.height; ++ar)
            {
                for (int ac = 0; ac < rl.width; ++ac, index++)
                {
                    // get the image coordinates
                    r = ar * step;
                    c = ac * step;

                    // Compute response components
                    Dxx = img.BoxIntegral(r - l + 1, c - b, 2 * l - 1, w)
                        - img.BoxIntegral(r - l + 1, c - l / 2, 2 * l - 1, l) * 3;
                    Dyy = img.BoxIntegral(r - b, c - l + 1, w, 2 * l - 1)
                        - img.BoxIntegral(r - l / 2, c - l + 1, l, 2 * l - 1) * 3;
                    Dxy = +img.BoxIntegral(r - l, c + 1, l, l)
                          + img.BoxIntegral(r + 1, c - l, l, l)
                          - img.BoxIntegral(r - l, c - l, l, l)
                          - img.BoxIntegral(r + 1, c + 1, l, l);

                    // Normalize the filter responses with respect to their size
                    Dxx *= inverse_area;
                    Dyy *= inverse_area;
                    Dxy *= inverse_area;

                    // Get the determinant of Hessian response & Laplacian sign
                    rl.responses[index] = (Dxx * Dyy - 0.81f * Dxy * Dxy);
                    rl.laplacian[index] = (byte)(Dxx + Dyy >= 0 ? 1 : 0);
                }
            }
        }


        /// <summary>
        ///   Test whether the point (r,c) in the middle layer
        ///   is a local maximum in the <c>3x3x3</c> neighborhood.
        /// </summary>
        /// 
        /// <param name="r">The row to be tested.</param>
        /// <param name="c">The column to be tested.</param>
        /// 
        /// <param name="t">Top response layer.</param>
        /// <param name="m">Middle response layer.</param>
        /// <param name="b">Bottom response layer.</param>
        /// 
        bool isExtremum(int r, int c, ResponseLayer t, ResponseLayer m, ResponseLayer b)
        {
            // bounds check
            int layerBorder = (t.filter + 1) / (2 * t.step);
            if (r <= layerBorder || r >= t.height - layerBorder || c <= layerBorder || c >= t.width - layerBorder)
                return false;

            // check the candidate point in the middle layer is above thresh 
            float candidate = m.getResponse(r, c, t);
            if (candidate < thresh)
                return false;

            for (int rr = -1; rr <= 1; ++rr)
            {
                for (int cc = -1; cc <= 1; ++cc)
                {
                    // if any response in 3x3x3 is greater candidate not maximum
                    if (t.getResponse(r + rr, c + cc) >= candidate ||
                      ((rr != 0 || cc != 0) && m.getResponse(r + rr, c + cc, t) >= candidate) ||
                      b.getResponse(r + rr, c + cc, t) >= candidate)
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        ///   Interpolate scale-space maximum points to subpixel accuracy to form an image feature.
        /// </summary>
        /// 
        /// <param name="r">The row to be tested.</param>
        /// <param name="c">The column to be tested.</param>
        /// 
        /// <param name="t">Top response layer.</param>
        /// <param name="m">Middle response layer.</param>
        /// <param name="b">Bottom response layer.</param>
        /// 
        void interpolateExtremum(int r, int c, ResponseLayer t, ResponseLayer m, ResponseLayer b)
        {
            var D = BuildDerivative(r, c, t, m, b);
            var H = BuildHessian(r, c, t, m, b);
            var Hi = Matrix.Inverse(H);
            var Of = (-1.0).Multiply(Hi).Multiply(D);

            // get the offsets from the interpolation
            double[] O = { Of[0, 0], Of[1, 0], Of[2, 0] };

            // get the step distance between filters
            int filterStep = (m.filter - b.filter);

            // If point is sufficiently close to the actual extremum
            if (Math.Abs(O[0]) < 0.5f && Math.Abs(O[1]) < 0.5f && Math.Abs(O[2]) < 0.5f)
            {
                IPoint ipt = new IPoint();
                ipt.x = (float)((c + O[0]) * t.step);
                ipt.y = (float)((r + O[1]) * t.step);
                ipt.scale = (float)((0.1333f) * (m.filter + O[2] * filterStep));
                ipt.laplacian = (int)(m.getLaplacian(r, c, t));
                ipts.Add(ipt);
            }
        }


        /// <summary>
        /// Build Matrix of First Order Scale-Space derivatives
        /// </summary>
        /// <param name="octave"></param>
        /// <param name="interval"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns>3x1 Matrix of Derivatives</returns>
        private double[,] BuildDerivative(int r, int c, ResponseLayer t, ResponseLayer m, ResponseLayer b)
        {
            double dx, dy, ds;

            dx = (m.getResponse(r, c + 1, t) - m.getResponse(r, c - 1, t)) / 2f;
            dy = (m.getResponse(r + 1, c, t) - m.getResponse(r - 1, c, t)) / 2f;
            ds = (t.getResponse(r, c) - b.getResponse(r, c, t)) / 2f;

            double[,] D = { { dx }, { dy }, { ds } };
            return D;
        }


        /// <summary>
        /// Build Hessian Matrix 
        /// </summary>
        /// <param name="octave"></param>
        /// <param name="interval"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns>3x3 Matrix of Second Order Derivatives</returns>
        private double[,] BuildHessian(int r, int c, ResponseLayer t, ResponseLayer m, ResponseLayer b)
        {
            double v, dxx, dyy, dss, dxy, dxs, dys;

            v = m.getResponse(r, c, t);
            dxx = m.getResponse(r, c + 1, t) + m.getResponse(r, c - 1, t) - 2 * v;
            dyy = m.getResponse(r + 1, c, t) + m.getResponse(r - 1, c, t) - 2 * v;
            dss = t.getResponse(r, c) + b.getResponse(r, c, t) - 2 * v;
            dxy = (m.getResponse(r + 1, c + 1, t) - m.getResponse(r + 1, c - 1, t) -
                    m.getResponse(r - 1, c + 1, t) + m.getResponse(r - 1, c - 1, t)) / 4f;
            dxs = (t.getResponse(r, c + 1) - t.getResponse(r, c - 1) -
                    b.getResponse(r, c + 1, t) + b.getResponse(r, c - 1, t)) / 4f;
            dys = (t.getResponse(r + 1, c) - t.getResponse(r - 1, c) -
                    b.getResponse(r + 1, c, t) + b.getResponse(r - 1, c, t)) / 4f;

            double[,] H = new double[3, 3];
            H[0, 0] = dxx;
            H[0, 1] = dxy;
            H[0, 2] = dxs;
            H[1, 0] = dxy;
            H[1, 1] = dyy;
            H[1, 2] = dys;
            H[2, 0] = dxs;
            H[2, 1] = dys;
            H[2, 2] = dss;
            return H;
        }


    } // FastHessian
} // OpenSURFcs
