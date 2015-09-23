using System;
using System.Collections.Generic;
using Accord.Math;

namespace Accord.Statistics.Models.Regression.Nonlinear.Fitting
{
    public class NonNegativeLeastSquares
    {
        private readonly List<int> _p = new List<int>();
        private readonly List<int> _r = new List<int>();
        private readonly double[] _s;
        private readonly double _tolerance;
        private readonly double[][] _transposeAndMultiply;
        private readonly double[] _vector;
        private double[] _weigths;
        private readonly double[][] _x;

        public NonNegativeLeastSquares(double[][] x, double[] y, double tolerance = 0.001, double[] param = null)
        {
            _x = x;
            _tolerance = tolerance;
            Coefficients = param ?? new double[_x.Columns()];
            _s = new double[_x.Columns()];
            _weigths = new double[_x.Columns()];

            _transposeAndMultiply = _x.TransposeAndMultiply(_x);
            _vector = _x.TransposeAndMultiply(y);
        }

        public double[] Coefficients { get; private set; }

        // Based on Fast NNLS
        // Nonnegativity Constraints in Numerical Analysis, Donghui Chen and Robert J.Plemmons
        // http://users.wfu.edu/plemmons/papers/nonneg.pdf
        public void Fit(int maxIter)
        {
            //Initialization
            _p.Clear();
            _r.Clear();
            for (var i = 0; i < _x.Columns(); i++)
            {
                _r.Add(i);
            }

            var x = Coefficients;

            ComputeWeights(x);
            var iter = 0;
            int maxWeightIndex;
            _weigths.Max(out maxWeightIndex);
            while (_r.Count > 0 && _weigths[maxWeightIndex] > _tolerance && iter < maxIter)
            {
                // Include the index j in P and remove it from R
                if (!_p.Contains(maxWeightIndex))
                    _p.Add(maxWeightIndex);

                if (_r.Contains(maxWeightIndex))
                    _r.Remove(maxWeightIndex);

                GetSP();
                var iter2 = 0;

                while (GetElements(_s, _p).Min() <= 0 && iter2 < maxIter)
                {
                    InnerLoop(x);
                    iter2++;
                }
                //5
                Array.Copy(_s, x, _s.Length);

                //6
                ComputeWeights(x);

                _weigths.Max(out maxWeightIndex);
                iter++;
            }
            Coefficients = x;
        }

        private void InnerLoop(double[] x)
        {
            var alpha = double.PositiveInfinity;
            foreach (var i in _p)
            {
                if (_s[i] <= 0)
                    alpha = System.Math.Min(alpha, x[i] / (x[i] - _s[i]));
            }

            if (System.Math.Abs(alpha) < 0.001 || double.IsNaN(alpha))
                return;

            x = (_s.Subtract(x)).Multiply(alpha).Add(x);

            //4.4 Update R and P
            for (var i = 0; i < _p.Count;)
            {
                var pItem = _p[i];
                if (System.Math.Abs(x[pItem]) < double.Epsilon)
                {
                    _r.Add(pItem);
                    _p.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            //4.5 
            GetSP();

            //4.6
            foreach (var i in _r)
            {
                _s[i] = 0;
            }
        }

        private void ComputeWeights(double[] x)
        {
            _weigths = _vector.Subtract(_transposeAndMultiply.Multiply(x));
        }

        private void GetSP()
        {
            var array = _p.ToArray();
            var left = _transposeAndMultiply.GetColumns(array).GetRows(array).PseudoInverse();

            var columnVector = GetElements(_vector, _p);
            var result = left.Multiply(columnVector);
            for (var i = 0; i < _p.Count; i++)
            {
                _s[_p[i]] = result[i];
            }
        }

        private static double[] GetElements(double[] vector, List<int> elementsIndex)
        {
            var z = new double[elementsIndex.Count];
            for (var i = 0; i < elementsIndex.Count; i++)
            {
                z[i] = vector[elementsIndex[i]];
            }
            return z;
        }
    }
}
