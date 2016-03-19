

namespace Accord.MachineLearning.Metrics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using System.Threading.Tasks;
    public class Accuracy : IPerformanceMetric
    {
        private bool average = false;


        public bool Mean
        {
            get { return average; }
            set { average = true; }
        }

        public Accuracy()
        {

        }

        public double Compute(int[] expected, int[] actual)
        {
            int hits = 0;
            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] == actual[i])
                    hits++;
            }

            if (average)
                return (double)hits / expected.Length;
            return hits;
        }

    }
}
