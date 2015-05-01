using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Statistics.Distributions.Univariate
{
    class RademacherDistribution : UnivariateDiscreteDistribution
    {
        public override double Mean
        {
            get { return 0; }
        }

        public override double Variance
        {
            get { return 1; }
        }

        public override double Entropy
        {
            get { return System.Math.Log(2); }
        }

        public override AForge.IntRange Support
        {
            get { return new AForge.IntRange(-1, +1); }
        }

        public override double DistributionFunction(int k)
        {
            if (k < -1)
                return 0;
            if (k >= 1)
                return 1;
            return 0.5;
        }

        public override double ProbabilityMassFunction(int k)
        {
            if (k == -1)
                return 0.5;
            if (k == +1)
                return 0.5;
            return 0.0;
        }

        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return "Rademacher(x)";
        }

        public override object Clone()
        {
            return new RademacherDistribution();
        }
    }
}
